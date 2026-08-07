namespace LabX08;

internal sealed class ConsumerController
{
    private readonly LabDemoOptions options;
    private readonly DataControlClient client;
    private readonly ConsumerRuntimeState state = new();
    private readonly List<Byte[]> observedSegments = new();
    private readonly Lock sync = new();
    private readonly SemaphoreSlim outputRefreshGate = new(1,1);

    private CancellationTokenSource? followCancellation;
    private Task? followTask;
    private DataControlDownloadSession? currentSession;
    private String? activeStreamPath;
    private Int64 activeReadOffset;

    public event Action<ConsumerRuntimeState>? StateChanged;

    public ConsumerController(LabDemoOptions options , DataControlClient client)
    {
        this.options = options;
        this.client = client;
        this.state.Status = "Ready";
    }

    public ConsumerRuntimeState Snapshot()
    {
        lock(this.sync)
        {
            return new ConsumerRuntimeState()
            {
                ItemId = this.state.ItemId,
                SourceSessionId = this.state.SourceSessionId,
                LocalSessionId = this.state.LocalSessionId,
                Mode = this.state.Mode,
                IsStreaming = this.state.IsStreaming,
                BytesObserved = this.state.BytesObserved,
                ChunksObserved = this.state.ChunksObserved,
                Status = this.state.Status,
                MaterializedSummary = this.state.MaterializedSummary,
                OutputMode = this.state.OutputMode,
                FinalItemType = this.state.FinalItemType,
                FinalItemName = this.state.FinalItemName,
                FinalItemDataType = this.state.FinalItemDataType,
                FinalItemTags = this.state.FinalItemTags,
                FinalItemNotes = this.state.FinalItemNotes,
                ProgressStatus = this.state.ProgressStatus,
                ProgressRange = this.state.ProgressRange,
                ProgressTotals = this.state.ProgressTotals,
                OutputLines = new List<String>(this.state.OutputLines)
            };
        }
    }

    public void ToggleOutputMode()
    {
        Boolean changed = false;

        lock(this.sync)
        {
            this.state.OutputMode = this.state.OutputMode == ConsumerRuntimeState.TextOutputMode
                ? ConsumerRuntimeState.HexOutputMode
                : ConsumerRuntimeState.TextOutputMode;

            this.RebuildOutputLinesUnsafe();
            changed = true;
        }

        if(changed) { this.PublishStateChanged(); }
    }

    public void ClearOutput()
    {
        Boolean changed = false;

        lock(this.sync)
        {
            this.observedSegments.Clear();
            this.state.OutputLines.Clear();
            changed = true;
        }

        if(changed) { this.PublishStateChanged(); }
    }

    public void ResetDisplay()
    {
        lock(this.sync)
        {
            this.state.ItemId = null;
            this.state.SourceSessionId = null;
            this.state.LocalSessionId = null;
            this.state.Mode = null;
            this.state.IsStreaming = false;
            this.state.BytesObserved = 0;
            this.state.ChunksObserved = 0;
            this.state.Status = "Ready";
            this.state.MaterializedSummary = "None";
            this.state.FinalItemType = "None";
            this.state.FinalItemName = "-";
            this.state.FinalItemDataType = "-";
            this.state.FinalItemTags = "-";
            this.state.FinalItemNotes = "-";
            this.state.ProgressStatus = "No progress yet.";
            this.state.ProgressRange = "-";
            this.state.ProgressTotals = "-";
            this.observedSegments.Clear();
            this.state.OutputLines.Clear();
        }

        this.PublishStateChanged();
    }

    public async Task OpenLiveFollowAsync(Guid itemId , Guid sourceSessionId)
    {
        await this.StopFollowAsync().ConfigureAwait(false);

        DataControlDownloadSession? session = await this.client.OpenDownload(itemId,null,new DataControlDownloadOptions()
        {
            Mode = DataControlDownloadMode.StreamFollow,
            SourceSessionID = sourceSessionId,
            BindLiveStreamItem = true,
            CompletionMode = StreamCompletionMode.UntilSourceCompletes,
        }).ConfigureAwait(false);

        if(session is null || session.Session.StreamState is null) { throw new InvalidOperationException("Unable to open the live follow session."); }

        CancellationTokenSource cancellation = new();

        lock(this.sync)
        {
            this.currentSession = session;
            this.followCancellation = cancellation;
            this.followTask = null;
                this.activeStreamPath = session.LiveItem?.GetContentStream() is not null ? null : GetStreamPath(session.Session);
            this.activeReadOffset = 0;
            this.observedSegments.Clear();
            this.state.ItemId = itemId;
            this.state.SourceSessionId = sourceSessionId;
            this.state.LocalSessionId = session.Session.SessionId;
            this.state.Mode = DataControlDownloadMode.StreamFollow;
            this.state.IsStreaming = true;
            this.state.BytesObserved = session.Session.StreamState.AppendedLength;
            this.state.ChunksObserved = 0;
            this.state.Status = "Following live stream.";
            this.state.MaterializedSummary = "None";
            this.state.FinalItemType = "None";
            this.state.FinalItemName = "-";
            this.state.FinalItemDataType = "-";
            this.state.FinalItemTags = "-";
            this.state.FinalItemNotes = "-";
            this.state.ProgressStatus = "No progress yet.";
            this.state.ProgressRange = "-";
            this.state.ProgressTotals = "-";
            this.state.OutputLines.Clear();
        }

        this.PublishStateChanged();

        await this.ReadAppendedBytesAsync(CancellationToken.None).ConfigureAwait(false);

        Task loopTask = Task.Run(() => this.RunFollowLoopAsync(cancellation.Token));

        lock(this.sync)
        {
            this.followTask = loopTask;
        }

        this.PublishStateChanged();
    }

    public async Task ResumeLocalAsync(Guid localSessionId)
    {
        await this.StopFollowAsync().ConfigureAwait(false);

        DataControlDownloadSession? session = await this.client.ResumeDownload(localSessionId,null,new DataControlDownloadOptions()
        {
            BindLiveStreamItem = true,
            CompletionMode = StreamCompletionMode.UntilSourceCompletes,
        }).ConfigureAwait(false);

        if(session is null) { throw new InvalidOperationException("Unable to resume the local session."); }

        if(session.Session.TransferFamily is DataControlTransferFamily.Stream && session.Session.StreamState is not null)
        {
            CancellationTokenSource cancellation = new();

            lock(this.sync)
            {
                this.currentSession = session;
                this.followCancellation = cancellation;
                this.followTask = null;
                this.activeStreamPath = session.LiveItem?.GetContentStream() is not null ? null : GetStreamPath(session.Session);
                this.activeReadOffset = 0;
                this.observedSegments.Clear();
                this.state.ItemId = session.Session.ItemId;
                this.state.SourceSessionId = session.Session.StreamState.SourceSessionID;
                this.state.LocalSessionId = session.Session.SessionId;
                this.state.Mode = DataControlDownloadMode.StreamFollow;
                this.state.IsStreaming = true;
                this.state.BytesObserved = session.Session.StreamState.AppendedLength;
                this.state.ChunksObserved = 0;
                this.state.Status = "Resumed live follow session.";
                this.state.MaterializedSummary = "None";
                this.state.FinalItemType = "None";
                this.state.FinalItemName = "-";
                this.state.FinalItemDataType = "-";
                this.state.FinalItemTags = "-";
                this.state.FinalItemNotes = "-";
                this.state.ProgressStatus = "No progress yet.";
                this.state.ProgressRange = "-";
                this.state.ProgressTotals = "-";
                this.state.OutputLines.Clear();
            }

            this.PublishStateChanged();

            await this.ReadAppendedBytesAsync(CancellationToken.None).ConfigureAwait(false);

            Task loopTask = Task.Run(() => this.RunFollowLoopAsync(cancellation.Token));

            lock(this.sync)
            {
                this.followTask = loopTask;
            }

            this.PublishStateChanged();

            return;
        }

        lock(this.sync)
        {
            this.currentSession = session;
            this.followCancellation = null;
            this.followTask = null;
            this.activeStreamPath = null;
            this.activeReadOffset = 0;
            this.observedSegments.Clear();
            this.state.ItemId = session.Session.ItemId;
            this.state.SourceSessionId = session.Session.State.SourceSessionID;
            this.state.LocalSessionId = session.Session.SessionId;
            this.state.Mode = MapDownloadMode(session.Session.State.Mode);
            this.state.IsStreaming = false;
            this.state.BytesObserved = session.Session.State.RealizedBytes;
            this.state.ChunksObserved = session.Session.State.RealizedRanges.Length;
            this.state.Status = "Resumed committed local session.";
            this.state.OutputLines.Clear();
            this.ApplyFinalMetadataUnsafe(null);
            this.state.ProgressStatus = "No progress yet.";
            this.state.ProgressRange = "-";
            this.state.ProgressTotals = "-";
        }

        this.PublishStateChanged();
    }

    public async Task MaterializeAsync()
    {
        DataControlDownloadSession? session;

        lock(this.sync)
        {
            session = this.currentSession;
        }

        if(session is null) { throw new InvalidOperationException("There is no active consumer session to materialize."); }

        DataItemTransferProgressReporter progress = DataItemTransferProgressReporter.Create(this.OnDownloadProgressReported);

        if(session.Session.TransferFamily is DataControlTransferFamily.Stream)
        {
            _ = await session.DownloadToCurrent(progress).ConfigureAwait(false);
        }

        if(session.Session.TransferFamily is DataControlTransferFamily.Segmented)
        {
            _ = await session.DownloadToCompletion(progress).ConfigureAwait(false);
        }

        DataControlDownloadCompletionResult? result = await session.Materialize().ConfigureAwait(false);

        if(result is null) { throw new InvalidOperationException("Unable to materialize the current session."); }

        DataItem? finalMetadataItem = await LoadLatestMetadataItemAsync(result.Session,cancel:default).ConfigureAwait(false) ?? result.Item;

        Byte[]? materializedBytes = result.Item is DataStreamItem liveItem
            ? await TryReadAllBytesAsync(liveItem.GetContentStream(),cancel:default).ConfigureAwait(false)
            : await TryReadAllBytesAsync(result.StreamPath,cancel:default).ConfigureAwait(false);

        lock(this.sync)
        {
            this.state.MaterializedSummary = $"Item {result.Session.ItemId} / Session {result.Session.SessionId} / {result.TransferFamily} / {result.StreamPath}";
            this.state.Status = "Materialized completed item.";
            this.state.LocalSessionId = result.Session.SessionId;
            this.state.Mode = MapDownloadMode(result.Session.State.Mode);
            this.state.BytesObserved = result.Session.TransferFamily is DataControlTransferFamily.Stream
                ? result.Session.StreamState?.AppendedLength ?? this.state.BytesObserved
                : result.Session.State.RealizedBytes;
            this.state.ChunksObserved = result.Session.TransferFamily is DataControlTransferFamily.Stream
                ? this.state.ChunksObserved
                : result.Session.State.RealizedRanges.Length;

            if(materializedBytes is { Length: > 0 })
            {
                this.observedSegments.Clear();
                this.observedSegments.Add(materializedBytes);
                this.RebuildOutputLinesUnsafe();
            }

            this.ApplyFinalMetadataUnsafe(finalMetadataItem);
        }

        this.PublishStateChanged();
    }

    public async Task StopAsync()
    {
        await this.StopFollowAsync().ConfigureAwait(false);
    }

    private async Task StopFollowAsync()
    {
        CancellationTokenSource? cancellation;
        Task? loopTask;

        lock(this.sync)
        {
            cancellation = this.followCancellation;
            loopTask = this.followTask;
            this.followCancellation = null;
            this.followTask = null;
            this.state.IsStreaming = false;
        }

        this.PublishStateChanged();

        cancellation?.Cancel();

        if(loopTask is not null)
        {
            await loopTask.ConfigureAwait(false);
        }
    }

    private async Task RunFollowLoopAsync(CancellationToken cancel)
    {
        try
        {
            DataItemTransferProgressReporter progress = DataItemTransferProgressReporter.Create(this.OnDownloadProgressReported);

            while(cancel.IsCancellationRequested is false)
            {
                DataControlDownloadSession? session;

                lock(this.sync)
                {
                    session = this.currentSession;
                }

                if(session?.Session.StreamState is null) { break; }

                Int32 downloaded = await session.DownloadToCurrent(progress,this.GetFollowWait(),cancel).ConfigureAwait(false);

                lock(this.sync)
                {
                    if(session.Session.StreamState is { } state)
                    {
                        this.state.BytesObserved = state.AppendedLength;
                        this.state.LocalSessionId = session.Session.SessionId;

                        if(state.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Committed)
                        {
                            this.state.Status = "Source committed. Ready to materialize.";
                            this.state.IsStreaming = false;
                        }
                        else if(downloaded == 0)
                        {
                            this.state.Status = "Waiting for live stream data.";
                        }
                    }
                }

                this.PublishStateChanged();

                if(downloaded > 0)
                {
                    await this.ReadAppendedBytesAsync(cancel).ConfigureAwait(false);

                    lock(this.sync)
                    {
                        if(this.state.IsStreaming)
                        {
                            this.state.Status = "Following live stream.";
                        }
                    }

                    this.PublishStateChanged();
                }
                else if(session.Session.StreamState?.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Committed)
                {
                    break;
                }
            }
        }
        catch ( OperationCanceledException )
        {
        }
        catch ( Exception exception )
        {
            Log.Error(exception,"Consumer follow loop failed.");
            lock(this.sync)
            {
                this.state.Status = $"Faulted: {exception.Message}";
                this.state.IsStreaming = false;
            }

            this.PublishStateChanged();
        }
    }

    private async Task ReadAppendedBytesAsync(CancellationToken cancel)
    {
        await this.outputRefreshGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            await this.ReadAppendedBytesCoreAsync(cancel).ConfigureAwait(false);
        }
        finally
        {
            this.outputRefreshGate.Release();
        }
    }

    private async Task ReadAppendedBytesCoreAsync(CancellationToken cancel)
    {
        DataControlDownloadSession? session;
        String? path;
        Int64 offset;

        lock(this.sync)
        {
            session = this.currentSession;
            path = this.activeStreamPath;
            offset = this.activeReadOffset;
        }

        if(session?.LiveItem?.GetContentStream() is Stream liveStream)
        {
            if(offset > liveStream.Length) { offset = 0; }

            Int64 availableLength = session.Session.StreamState?.AppendedLength ?? liveStream.Length;

            if(offset >= availableLength) { return; }

            liveStream.Position = offset;

            Int64 remaining = Math.Max(0,availableLength - offset);

            if(remaining == 0) { return; }

            Byte[] bytes = new Byte[remaining];

            Int32 totalRead = 0;

            while(totalRead < bytes.Length)
            {
                Int32 read = await liveStream.ReadAsync(bytes.AsMemory(totalRead,bytes.Length-totalRead),cancel).ConfigureAwait(false);

                if(read <= 0) { break; }

                totalRead += read;
            }

            if(totalRead != bytes.Length)
            {
                Array.Resize(ref bytes,totalRead);
            }

            lock(this.sync)
            {
                this.activeReadOffset = liveStream.Position;

                if(bytes.Length == 0) { return; }

                this.observedSegments.Add(bytes);

                this.RebuildOutputLinesUnsafe();
            }

            this.PublishStateChanged();

            return;
        }
    }

    private void PublishStateChanged()
    {
        this.StateChanged?.Invoke(this.Snapshot());
    }

    private TimeSpan GetFollowWait()
    {
        return this.options.StatusRefreshMilliseconds <= 0
            ? TimeSpan.Zero
            : TimeSpan.FromMilliseconds(this.options.StatusRefreshMilliseconds);
    }

    private void OnDownloadProgressReported(DataItemTransferProgress progress)
    {
        Boolean refreshOutput = false;

        lock(this.sync)
        {
            this.state.ItemId = progress.ItemID;
            this.state.LocalSessionId = progress.SessionID;
            this.state.BytesObserved = progress.LatestAppendedLength ?? progress.RealizedBytes;
            this.state.ChunksObserved++;
            this.state.ProgressStatus = $"{progress.TransferFamily} / {progress.Mode} / {progress.Status}";
            this.state.ProgressRange = progress.LatestRange is null
                ? "-"
                : $"offset {progress.LatestRange.Offset:N0}, length {progress.LatestRange.Length:N0}";
            this.state.ProgressTotals = progress.TransferFamily is DataControlTransferFamily.Stream
                ? $"appended {progress.LatestAppendedLength ?? progress.RealizedBytes:N0} bytes / version {progress.StateVersion:N0}"
                : $"realized {progress.RealizedBytes:N0} bytes / remaining {(progress.RemainingBytes?.ToString("N0",CultureInfo.InvariantCulture) ?? "unknown")} / version {progress.StateVersion:N0}";
            refreshOutput = progress.TransferFamily is DataControlTransferFamily.Stream;
        }

        this.PublishStateChanged();

        if(refreshOutput)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await this.ReadAppendedBytesAsync(CancellationToken.None).ConfigureAwait(false);
                }
                catch ( Exception exception )
                {
                    Log.Error(exception,"LabX08 output refresh failed.");
                }
            });
        }
    }

    private void RebuildOutputLinesUnsafe()
    {
        this.state.OutputLines.Clear();

        if(this.state.OutputMode == ConsumerRuntimeState.HexOutputMode)
        {
            for(Int32 index = 0; index < this.observedSegments.Count; index++)
            {
                Byte[] segment = this.observedSegments[index];

                this.state.AddOutputLine($"chunk {index + 1:D4} | {segment.Length,5} bytes | {Convert.ToHexString(segment[..Math.Min(segment.Length,12)])}");
            }

            return;
        }

        Byte[] allBytes = this.observedSegments.SelectMany(_ => _).ToArray(); if(allBytes.Length == 0) { return; }

        String combined = Encoding.UTF8.GetString(allBytes);

        foreach(String line in combined.Replace("\r\n","\n",StringComparison.Ordinal).Split('\n',StringSplitOptions.RemoveEmptyEntries))
        {
            this.state.AddOutputLine(line);
        }
    }

    private void ApplyFinalMetadataUnsafe(DataItem? item)
    {
        if(item is null)
        {
            this.state.FinalItemType = "None";
            this.state.FinalItemName = "-";
            this.state.FinalItemDataType = "-";
            this.state.FinalItemTags = "-";
            this.state.FinalItemNotes = "-";
            return;
        }

        this.state.FinalItemType = item.GetType().Name;
        this.state.FinalItemName = item.GetName() ?? "-";
        this.state.FinalItemDataType = item.GetDataType() ?? "-";
        this.state.FinalItemTags = FormatMetadataLines(item.GetTags());
        this.state.FinalItemNotes = FormatMetadataLines(item.GetNotes());
    }

    private static async Task<DataItem?> LoadLatestMetadataItemAsync(DataControlTransferSessionInfo session , CancellationToken cancel)
    {
        String objectPath = Path.Combine(session.WorkingDirectory,DataStreamTransferManifest.ObjectFileName);

        Byte[]? objectPayload = await TryReadAllBytesAsync(objectPath,cancel).ConfigureAwait(false);

        return objectPayload is { Length: > 0 } ? DataItem.Deserialize(objectPayload) : null;
    }

    private static async Task<Byte[]?> TryReadAllBytesAsync(Stream? stream , CancellationToken cancel)
    {
        if(stream is null || stream.CanRead is false) { return null; }

        if(stream.CanSeek) { stream.Position = 0; }

        using MemoryStream buffer = new();
        await stream.CopyToAsync(buffer,cancel).ConfigureAwait(false);
        return buffer.ToArray();
    }

    private static async Task<Byte[]?> TryReadAllBytesAsync(String path , CancellationToken cancel)
    {
        if(String.IsNullOrWhiteSpace(path) || File.Exists(path) is false) { return null; }

        return await File.ReadAllBytesAsync(path,cancel).ConfigureAwait(false);
    }

    private static String GetStreamPath(DataControlTransferSessionInfo session)
    {
        return Path.Combine(session.WorkingDirectory,DataStreamTransferManifest.StreamFileName);
    }

    private static String FormatMetadataLines(IEnumerable<String>? values)
    {
        String[] entries = values?.Where(_ => String.IsNullOrWhiteSpace(_) is false).ToArray() ?? Array.Empty<String>();
        return entries.Length == 0 ? "-" : String.Join(Environment.NewLine,entries);
    }

    private static DataControlDownloadMode MapDownloadMode(DataItemTransferMode mode)
    {
        return mode switch
        {
            DataItemTransferMode.ReadCommitted => DataControlDownloadMode.Committed,
            DataItemTransferMode.ReadStaged => DataControlDownloadMode.StagedFollow,
            DataItemTransferMode.StreamFollow => DataControlDownloadMode.StreamFollow,
            _ => DataControlDownloadMode.Committed
        };
    }
}
