using System.Threading.Channels;

namespace LabX02;

internal sealed class ProducerController
{
    private static readonly String?[] BinaryDataTypes = DataValidation.BinaryItemValidDataTypes
        .Where(_ => String.IsNullOrWhiteSpace(_) is false)
        .OrderBy(_ => _,StringComparer.OrdinalIgnoreCase)
        .ToArray();

    private static readonly String?[] TextDataTypes = DataValidation.TextItemValidDataTypes
        .Where(_ => String.IsNullOrWhiteSpace(_) is false)
        .OrderBy(_ => _,StringComparer.OrdinalIgnoreCase)
        .ToArray();

    private readonly LabDemoOptions options;
    private readonly DataControlClient client;
    private readonly LabTextGenerator textGenerator = new();
    private readonly Random random = new();
    private readonly ProducerRuntimeState state = new();
    private readonly Lock stateSync = new();
    private readonly Channel<ProducerCommand> commandQueue = Channel.CreateUnbounded<ProducerCommand>(new UnboundedChannelOptions()
    {
        SingleReader = true,
        SingleWriter = false,
    });
    private readonly Task processorTask;

    private DataControlUploadSession? liveUploadSession;
    private StreamWriter? liveWriter;
    private ProducerBufferStream? liveStream;

    public event Action<ProducerRuntimeState>? StateChanged;

    public ProducerController(LabDemoOptions options , DataControlClient client)
    {
        this.options = options;
        this.client = client;

        lock(this.stateSync)
        {
            this.state.RandomPauseEnabled = options.RandomPauseEnabled;
            this.state.SetPhase(ProducerPhase.Ready,GetReadyStatus(this.state.TextMode));
        }

        this.processorTask = Task.Run(this.ProcessCommandsAsync);
    }

    public ProducerRuntimeState Snapshot()
    {
        lock(this.stateSync)
        {
            return this.state.Clone();
        }
    }

    public void TogglePayloadMode()
    {
        this.EnqueueCommandAsync(static (controller , cancel) => controller.TogglePayloadModeCoreAsync(cancel)).GetAwaiter().GetResult();
    }

    public static IReadOnlyList<String?> GetAvailableFinalItemDataTypes(String itemType)
    {
        return NormalizeFinalItemType(itemType) switch
        {
            ProducerRuntimeState.TextItemType => TextDataTypes,
            ProducerRuntimeState.GenericItemType => Array.Empty<String?>(),
            _ => BinaryDataTypes,
        };
    }

    public void UpdateFinalItemMetadata(String itemType , String? dataType , String? name , IEnumerable<String>? tags , IEnumerable<String>? notes)
    {
        String normalizedItemType = NormalizeFinalItemType(itemType);
        String finalItemName = name ?? String.Empty;
        String tagsText = NormalizeMultilineText(tags);
        String notesText = NormalizeMultilineText(notes);

        this.EnqueueCommandAsync((controller , cancel) => controller.UpdateFinalItemMetadataCoreAsync(
            normalizedItemType,
            dataType,
            finalItemName,
            tagsText,
            notesText,
            cancel)).GetAwaiter().GetResult();
    }

    public void ToggleRandomPause()
    {
        this.EnqueueCommandAsync(static (controller , cancel) => controller.ToggleRandomPauseCoreAsync(cancel)).GetAwaiter().GetResult();
    }

    public void TogglePause()
    {
        this.EnqueueCommandAsync(static (controller , cancel) => controller.TogglePauseCoreAsync(cancel)).GetAwaiter().GetResult();
    }

    public Task StartLiveAsync()
    {
        return this.EnqueueCommandAsync(static (controller , cancel) => controller.StartLiveCoreAsync(cancel));
    }

    public Task CompleteLiveAsync()
    {
        return this.EnqueueCommandAsync(static (controller , cancel) => controller.CompleteLiveCoreAsync(cancel));
    }

    public Task AbortLiveAsync()
    {
        return this.EnqueueCommandAsync(static (controller , cancel) => controller.AbortLiveCoreAsync(cancel));
    }

    public void ResetDisplay()
    {
        this.EnqueueCommandAsync(static (controller , cancel) => controller.ResetDisplayCoreAsync(cancel)).GetAwaiter().GetResult();
    }

    public Task StopAsync()
    {
        return this.EnqueueCommandAsync(static (controller , cancel) => controller.StopCoreAsync(cancel));
    }

    private async Task ProcessCommandsAsync()
    {
        ChannelReader<ProducerCommand> reader = this.commandQueue.Reader;

        while(await this.WaitForWorkAsync(reader).ConfigureAwait(false))
        {
            while(reader.TryRead(out ProducerCommand? command))
            {
                await this.ExecuteCommandAsync(command).ConfigureAwait(false);
            }

            if(this.CanProduceNextChunk())
            {
                await this.ProduceNextChunkAsync().ConfigureAwait(false);
            }
        }
    }

    private async Task<Boolean> WaitForWorkAsync(ChannelReader<ProducerCommand> reader)
    {
        if(this.CanProduceNextChunk())
        {
            Task<Boolean> waitToReadTask = reader.WaitToReadAsync().AsTask();
            Task delayTask = Task.Delay(this.options.StatusRefreshMilliseconds);
            Task completedTask = await Task.WhenAny(waitToReadTask,delayTask).ConfigureAwait(false);

            if(completedTask == delayTask) { return true; }

            return await waitToReadTask.ConfigureAwait(false);
        }

        return await reader.WaitToReadAsync().ConfigureAwait(false);
    }

    private async Task ExecuteCommandAsync(ProducerCommand command)
    {
        try
        {
            await command.ExecuteAsync(this,CancellationToken.None).ConfigureAwait(false);
            command.Completion.SetResult();
        }
        catch ( Exception exception )
        {
            command.Completion.SetException(exception);
        }
    }

    private Task EnqueueCommandAsync(Func<ProducerController,CancellationToken,Task> action)
    {
        ProducerCommand command = new(action);

        if(this.commandQueue.Writer.TryWrite(command) is false)
        {
            throw new InvalidOperationException("The producer controller is not accepting commands.");
        }

        return command.Completion.Task;
    }

    private Task TogglePayloadModeCoreAsync(CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        Boolean changed = false;

        lock(this.stateSync)
        {
            if(this.state.IsRunning) { return Task.CompletedTask; }

            this.state.TextMode = !this.state.TextMode;
            this.state.SetPhase(ProducerPhase.Ready,GetReadyStatus(this.state.TextMode));
            changed = true;
        }

        if(changed) { this.PublishStateChanged(); }

        return Task.CompletedTask;
    }

    private Task UpdateFinalItemMetadataCoreAsync(String itemType , String? dataType , String name , String tagsText , String notesText , CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        lock(this.stateSync)
        {
            this.state.FinalItemType = itemType;
            this.state.FinalItemName = name;
            this.state.FinalItemTagsText = tagsText;
            this.state.FinalItemNotesText = notesText;

            if(this.state.FinalItemType is ProducerRuntimeState.GenericItemType)
            {
                this.state.FinalItemDataType = String.IsNullOrWhiteSpace(dataType) ? DataTypes.NONE : dataType.Trim();
            }
            else
            {
                IReadOnlyList<String?> validTypes = GetAvailableFinalItemDataTypes(this.state.FinalItemType);
                String? normalizedDataType = String.IsNullOrWhiteSpace(dataType) ? null : dataType.Trim();
                this.state.FinalItemDataType = validTypes.Contains(normalizedDataType,StringComparer.OrdinalIgnoreCase)
                    ? normalizedDataType!
                    : validTypes[0]!;
            }
        }

        return Task.CompletedTask;
    }

    private Task ToggleRandomPauseCoreAsync(CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        lock(this.stateSync)
        {
            this.state.RandomPauseEnabled = !this.state.RandomPauseEnabled;
        }

        this.PublishStateChanged();
        return Task.CompletedTask;
    }

    private Task TogglePauseCoreAsync(CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        Boolean changed = false;

        lock(this.stateSync)
        {
            switch(this.state.Phase)
            {
                case ProducerPhase.Streaming:
                    this.state.SetPhase(ProducerPhase.Paused,"Paused");
                    changed = true;
                    break;
                case ProducerPhase.Paused:
                    this.state.SetPhase(ProducerPhase.Streaming,"Streaming");
                    changed = true;
                    break;
            }
        }

        if(changed) { this.PublishStateChanged(); }

        return Task.CompletedTask;
    }

    private async Task StartLiveCoreAsync(CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        if(this.liveUploadSession is not null) { return; }

        lock(this.stateSync)
        {
            this.state.ItemId = null;
            this.state.SessionId = null;
            this.state.BytesProduced = 0;
            this.state.ChunksProduced = 0;
            this.state.RecentLines.Clear();
            this.state.SetPhase(ProducerPhase.Starting,"Starting");
        }

        this.PublishStateChanged();

        try
        {
            DataStreamItem item = new();
            ProducerBufferStream stream = new();

            if(item.SetContent(stream) is false) { throw new InvalidOperationException("Unable to bind the producer stream."); }

            DataControlUploadSession? session = await this.client.OpenUpload(item,null,new DataControlUploadOptions()
            {
                CompletionMode = StreamCompletionMode.UntilSourceCompletes,
            },cancel).ConfigureAwait(false);

            if(session is null) { throw new InvalidOperationException("Unable to open the live upload session."); }

            this.liveStream = stream;
            this.liveWriter = new StreamWriter(stream,new UTF8Encoding(false),leaveOpen:true);
            this.liveUploadSession = session;

            lock(this.stateSync)
            {
                this.state.ItemId = session.Session.ItemId;
                this.state.SessionId = session.Session.SessionId;
                this.state.SetPhase(ProducerPhase.Streaming,"Streaming");
            }

            this.PublishStateChanged();
        }
        catch ( Exception exception )
        {
            await this.CleanupLiveResourcesAsync().ConfigureAwait(false);

            lock(this.stateSync)
            {
                this.state.ItemId = null;
                this.state.SessionId = null;
                this.state.SetPhase(ProducerPhase.Faulted,$"Start failed: {exception.Message}");
            }

            this.PublishStateChanged();
            throw;
        }
    }

    private async Task CompleteLiveCoreAsync(CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        DataControlUploadSession? session = this.liveUploadSession;
        StreamWriter? writer = this.liveWriter;

        if(session is null) { throw new InvalidOperationException("There is no active live session to complete."); }

        ProducerRuntimeState snapshot = this.Snapshot();
        DataItem finalItem = this.CreateFinalCommittedItem(snapshot);

        lock(this.stateSync)
        {
            this.state.SetPhase(ProducerPhase.Completing,"Completing");
        }

        this.PublishStateChanged();

        try
        {
            if(writer is not null)
            {
                await writer.FlushAsync(cancel).ConfigureAwait(false);
            }

            _ = await session.UploadToCompletion(cancel:cancel).ConfigureAwait(false);

            DataControlUploadCompletionResult? committed = await session.Commit(finalitem:finalItem,cancel).ConfigureAwait(false);

            if(committed is null)
            {
                throw new InvalidOperationException("Unable to commit the final metadata item for the completed live stream.");
            }

            await this.CleanupLiveResourcesAsync().ConfigureAwait(false);

            lock(this.stateSync)
            {
                this.state.CommittedItemSummary = $"{finalItem.GetType().Name} / {finalItem.GetName()} / {finalItem.GetDataType()}";
                this.state.SetPhase(ProducerPhase.Completed,"Completed");
            }

            this.PublishStateChanged();
        }
        catch ( Exception exception )
        {
            await this.TryAbortLiveSessionAsync().ConfigureAwait(false);
            await this.CleanupLiveResourcesAsync().ConfigureAwait(false);

            lock(this.stateSync)
            {
                this.state.SetPhase(ProducerPhase.Faulted,$"Commit failed: {exception.Message}");
            }

            this.PublishStateChanged();
            throw;
        }
    }

    private async Task AbortLiveCoreAsync(CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        if(this.liveUploadSession is null && this.liveWriter is null && this.liveStream is null) { return; }

        lock(this.stateSync)
        {
            this.state.SetPhase(ProducerPhase.Aborting,"Aborting");
        }

        this.PublishStateChanged();

        await this.TryAbortLiveSessionAsync().ConfigureAwait(false);
        await this.CleanupLiveResourcesAsync().ConfigureAwait(false);

        lock(this.stateSync)
        {
            this.state.SetPhase(ProducerPhase.Aborted,"Aborted");
        }

        this.PublishStateChanged();
    }

    private Task ResetDisplayCoreAsync(CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        lock(this.stateSync)
        {
            this.state.BytesProduced = 0;
            this.state.ChunksProduced = 0;
            this.state.CommittedItemSummary = "None";
            this.state.RecentLines.Clear();

            if(this.state.IsRunning is false)
            {
                this.state.ItemId = null;
                this.state.SessionId = null;
                this.state.SetPhase(ProducerPhase.Ready,GetReadyStatus(this.state.TextMode));
            }
        }

        this.PublishStateChanged();
        return Task.CompletedTask;
    }

    private Task StopCoreAsync(CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        if(this.liveUploadSession is null && this.liveWriter is null && this.liveStream is null)
        {
            return Task.CompletedTask;
        }

        return this.AbortLiveCoreAsync(cancel);
    }

    private Boolean CanProduceNextChunk()
    {
        lock(this.stateSync)
        {
            return this.state.Phase is ProducerPhase.Streaming;
        }
    }

    private async Task ProduceNextChunkAsync()
    {
        try
        {
            ProducerBufferStream? stream = this.liveStream;
            StreamWriter? writer = this.liveWriter;
            DataControlUploadSession? session = this.liveUploadSession;

            if(stream is null || writer is null || session is null) { return; }

            Byte[] payload;
            Boolean textMode;

            lock(this.stateSync)
            {
                textMode = this.state.TextMode;
            }

            payload = textMode ? this.CreateTextPayload() : this.CreateBinaryPayload();

            await stream.AppendAsync(payload,CancellationToken.None).ConfigureAwait(false);
            await writer.FlushAsync(CancellationToken.None).ConfigureAwait(false);

            if(await session.UploadNext(CancellationToken.None).ConfigureAwait(false) is false)
            {
                return;
            }

            Boolean pauseRandomly;

            lock(this.stateSync)
            {
                this.state.BytesProduced += payload.Length;
                this.state.ChunksProduced++;
                this.state.SetPhase(ProducerPhase.Streaming,"Streaming");
                if(textMode) { this.state.AddLine(Encoding.UTF8.GetString(payload).TrimEnd()); }
                else { this.state.AddLine($"chunk {this.state.ChunksProduced:D4} | {payload.Length,5} bytes | {Convert.ToHexString(payload[..Math.Min(payload.Length,12)])}"); }
                pauseRandomly = this.state.RandomPauseEnabled && this.random.Next(0,8) == 0;
                if(pauseRandomly)
                {
                    this.state.SetPhase(ProducerPhase.Paused,"Random Pause");
                }
            }

            this.PublishStateChanged();
        }
        catch ( Exception exception )
        {
            Log.Error(exception,"Producer live loop failed.");
            await this.TryAbortLiveSessionAsync().ConfigureAwait(false);
            await this.CleanupLiveResourcesAsync().ConfigureAwait(false);

            lock(this.stateSync)
            {
                this.state.SetPhase(ProducerPhase.Faulted,$"Faulted: {exception.Message}");
            }

            this.PublishStateChanged();
        }
    }

    private Byte[] CreateTextPayload()
    {
        String line = this.textGenerator.NextLine() + Environment.NewLine;

        return Encoding.UTF8.GetBytes(line);
    }

    private Byte[] CreateBinaryPayload()
    {
        Int32 length = this.random.Next(this.options.DefaultChunkBytes,Math.Max(this.options.DefaultChunkBytes + 1,this.options.MaximumChunkBytes + 1));
        Byte[] payload = new Byte[length];
        this.random.NextBytes(payload);
        return payload;
    }

    private DataItem CreateFinalCommittedItem(ProducerRuntimeState snapshot)
    {
        String[] nlcs = { Environment.NewLine, "\n", "\r\n", "\r" };
        String[] tags = NormalizeEntries(snapshot.FinalItemTagsText.Split(nlcs,StringSplitOptions.None));
        String[] notes = NormalizeEntries(snapshot.FinalItemNotesText.Split(nlcs,StringSplitOptions.None));

        return snapshot.FinalItemType switch
        {
            ProducerRuntimeState.TextItemType => new TextItem(content:null,file:null,id:null,name:snapshot.FinalItemName,notes:notes,tags:tags,type:snapshot.FinalItemDataType),
            ProducerRuntimeState.GenericItemType => new GenericItem(content:null,file:null,id:null,name:snapshot.FinalItemName,notes:notes,tags:tags,type:snapshot.FinalItemDataType),
            _ => new BinaryItem(content:null,dllpaths:null,file:null,id:null,name:snapshot.FinalItemName,notes:notes,tags:tags,type:snapshot.FinalItemDataType),
        };
    }

    private async Task TryAbortLiveSessionAsync()
    {
        DataControlUploadSession? session = this.liveUploadSession;
        if(session is null) { return; }

        try
        {
            await session.Abort().ConfigureAwait(false);
        }
        catch ( Exception abortException )
        {
            Log.Error(abortException,"Producer abort failed.");
        }
    }

    private async Task CleanupLiveResourcesAsync()
    {
        StreamWriter? writer = this.liveWriter;
        ProducerBufferStream? stream = this.liveStream;

        this.liveWriter = null;
        this.liveStream = null;
        this.liveUploadSession = null;

        if(writer is not null)
        {
            await writer.DisposeAsync().ConfigureAwait(false);
        }

        if(stream is not null)
        {
            await stream.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static String GetReadyStatus(Boolean textMode)
    {
        return textMode ? "Ready (Text)" : "Ready (Bytes)";
    }

    private static String NormalizeFinalItemType(String? itemType)
    {
        return itemType switch
        {
            ProducerRuntimeState.TextItemType => ProducerRuntimeState.TextItemType,
            ProducerRuntimeState.GenericItemType => ProducerRuntimeState.GenericItemType,
            _ => ProducerRuntimeState.BinaryItemType,
        };
    }

    private static String[] NormalizeEntries(IEnumerable<String>? entries)
    {
        return entries?
            .Select(_ => _?.Trim())
            .Where(_ => String.IsNullOrWhiteSpace(_) is false)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Cast<String>()
            .ToArray()
            ?? Array.Empty<String>();
    }

    private static String NormalizeMultilineText(IEnumerable<String>? entries)
    {
        return String.Join(Environment.NewLine,entries?.Select(_ => _ ?? String.Empty) ?? Array.Empty<String>());
    }

    private void PublishStateChanged()
    {
        this.StateChanged?.Invoke(this.Snapshot());
    }

    private sealed class ProducerCommand
    {
        public ProducerCommand(Func<ProducerController,CancellationToken,Task> executeAsync)
        {
            this.ExecuteAsync = executeAsync;
        }

        public Func<ProducerController,CancellationToken,Task> ExecuteAsync { get; }

        public TaskCompletionSource Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    private sealed class ProducerBufferStream : Stream
    {
        private readonly MemoryStream buffer = new();
        private readonly Lock sync = new();
        private Int64 readPosition;

        public override Boolean CanRead => true;
        public override Boolean CanSeek => true;
        public override Boolean CanWrite => true;

        public override Int64 Length
        {
            get
            {
                lock(this.sync)
                {
                    return this.buffer.Length;
                }
            }
        }

        public override Int64 Position
        {
            get
            {
                lock(this.sync)
                {
                    return this.readPosition;
                }
            }
            set
            {
                lock(this.sync)
                {
                    this.readPosition = Math.Max(0,Math.Min(value,this.buffer.Length));
                }
            }
        }

        public async Task AppendAsync(Byte[] payload , CancellationToken cancel)
        {
            lock(this.sync)
            {
                this.buffer.Position = this.buffer.Length;
                this.buffer.Write(payload,0,payload.Length);
            }

            await Task.CompletedTask.ConfigureAwait(false);
            cancel.ThrowIfCancellationRequested();
        }

        public override void Flush()
        {
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count)
        {
            lock(this.sync)
            {
                if(this.readPosition >= this.buffer.Length) { return 0; }

                this.buffer.Position = this.readPosition;
                Int32 read = this.buffer.Read(buffer,offset,count);
                this.readPosition = this.buffer.Position;
                return read;
            }
        }

        public override Int64 Seek(Int64 offset , SeekOrigin origin)
        {
            lock(this.sync)
            {
                Int64 target = origin switch
                {
                    SeekOrigin.Begin => offset,
                    SeekOrigin.Current => this.readPosition + offset,
                    SeekOrigin.End => this.buffer.Length + offset,
                    _ => this.readPosition
                };

                this.readPosition = Math.Max(0,Math.Min(target,this.buffer.Length));
                return this.readPosition;
            }
        }

        public override void SetLength(Int64 value)
        {
            lock(this.sync)
            {
                this.buffer.SetLength(value);
                if(this.readPosition > value) { this.readPosition = value; }
            }
        }

        public override void Write(Byte[] buffer , Int32 offset , Int32 count)
        {
            lock(this.sync)
            {
                this.buffer.Position = this.buffer.Length;
                this.buffer.Write(buffer,offset,count);
            }
        }

        protected override void Dispose(Boolean disposing)
        {
            if(disposing)
            {
                lock(this.sync)
                {
                    this.buffer.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
