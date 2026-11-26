namespace KusDepot.Data.Clients;

/**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/main/*'/>*/
public sealed class DataControlUploadSession : IDisposable , IAsyncDisposable
{
    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/field[@name="client"]/*'/>*/
    private readonly IDataControlClient client;

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/field[@name="storage"]/*'/>*/
    private readonly DataControlClientWorkingDirectoryStorage storage;

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/field[@name="token"]/*'/>*/
    private readonly String? token;

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/field[@name="StreamSource"]/*'/>*/
    private Stream? StreamSource;

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/field[@name="CompletionMode"]/*'/>*/
    private readonly StreamCompletionMode CompletionMode;

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/field[@name="MaxBytes"]/*'/>*/
    private readonly Int64? MaxBytes;

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/field[@name="disposed"]/*'/>*/
    private Int32 disposed;

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/field[@name="OperationGate"]/*'/>*/
    private readonly SemaphoreSlim OperationGate = new(1,1);

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/constructor[@name="Constructor"]/*'/>*/
    internal DataControlUploadSession(IDataControlClient client , DataControlClientWorkingDirectoryStorage storage , String? token , DataControlTransferSessionInfo session,
        Stream? streamsource = null , StreamCompletionMode completionmode = StreamCompletionMode.UntilSourceCompletes , Int64? maxbytes = null)
    {
        this.client = client; this.storage = storage; this.token = token; this.StreamSource = streamsource;
        this.CompletionMode = completionmode; this.MaxBytes = maxbytes; this.Session = session;
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/property[@name="Session"]/*'/>*/
    public DataControlTransferSessionInfo Session { get; private set; }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="Dispose"]/*'/>*/
    public void Dispose()
    {
        if(Interlocked.Exchange(ref this.disposed,1) != 0) { return; }

        this.OperationGate.Wait();

        try
        {
            this.DisposeStreamSource().AsTask().GetAwaiter().GetResult();
        }
        finally
        {
            this.OperationGate.Release();
        }

        GC.SuppressFinalize(this);
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="DisposeAsync"]/*'/>*/
    public async ValueTask DisposeAsync()
    {
        if(Interlocked.Exchange(ref this.disposed,1) != 0) { return; }

        await this.OperationGate.WaitAsync().ConfigureAwait(false);

        try
        {
            await this.DisposeStreamSource().ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }

        GC.SuppressFinalize(this);
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="Abort"]/*'/>*/
    public async Task<Boolean> Abort(CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        await this.OperationGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return await this.AbortCore(cancel).ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="AbortCore"]/*'/>*/
    private async Task<Boolean> AbortCore(CancellationToken cancel)
    {
        DataControlResponse<AbortTransferResponse> aborted = await this.client.AbortTransfer(new AbortTransferRequest(){ SessionID = this.Session.SessionId , ItemID = this.Session.ItemId },this.token,cancel).ConfigureAwait(false);

        if(aborted.StatusCode != HttpStatusCode.OK || aborted.Data?.Aborted is not true) { return false; }

        if(this.Session.TransferFamily is DataControlTransferFamily.Stream)
        {
            await this.storage.DeleteStreamSession(this.Session.SessionId,cancel).ConfigureAwait(false);

            await this.DisposeStreamSource().ConfigureAwait(false);
        }
        else
        {
            await this.storage.DeleteSession(this.Session.SessionId,cancel).ConfigureAwait(false);
        }

        return true;
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="Publish"]/*'/>*/
    public async Task<DataControlUploadCompletionResult?> Publish(DataItem? finalitem = null , CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        await this.OperationGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return await this.PublishCore(finalitem,cancel).ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="PublishCore"]/*'/>*/
    private async Task<DataControlUploadCompletionResult?> PublishCore(DataItem? finalitem , CancellationToken cancel)
    {
        if(this.Session.TransferFamily is DataControlTransferFamily.Stream)
        {
            DataStreamTransferState? streamState = this.Session.StreamState;

            if(streamState is null || streamState.Mode != DataItemTransferMode.StreamUpload) { return null; }

            Byte[]? objectPayload = finalitem is null
                ? await this.storage.ReadStreamObjectPayload(streamState.SessionID,cancel).ConfigureAwait(false)
                : this.CreateFinalPublishedObjectPayload(finalitem);

            if(objectPayload is null) { return null; }

            Byte[] objectHash = finalitem is null
                ? streamState.ObjectSHA512
                : objectPayload.Length >= LargeObjectPayloadSize
                    ? await ComputeSHA512Async(objectPayload,MiB,cancel).ConfigureAwait(false)
                    : SHA512.HashData(objectPayload);

            DataItemTransferObjectInfo? objectInfo = finalitem is null
                ? streamState.ObjectInfo
                : new()
                {
                    ContentStreamed = finalitem.GetContentStreamed(),
                    DataType = finalitem.GetDataType(),
                    Name = finalitem.GetName(),
                    ObjectType = finalitem.GetType().FullName,
                };

            DataControlResponse<PublishStreamTransferResponse> completed = await this.client.PublishStreamTransfer(new PublishStreamTransferRequest()
            {
                SessionID = streamState.SessionID,
                ItemID = streamState.ItemID,
                ExpectedStateVersion = streamState.StateVersion,
                FinalObjectInfo = objectInfo,
                FinalObjectPayload = objectPayload,
                FinalStreamLength = streamState.AppendedLength,
            },this.token,cancel).ConfigureAwait(false);

            ThrowIfBlockLimitExceeded(completed.StatusCode,completed.Content);

            if(completed.StatusCode != HttpStatusCode.OK || completed.Data is null) { return null; }

            if(await this.storage.SaveState(completed.Data.State,objectPayload,cancel).ConfigureAwait(false) is false)
            {
                KusDepotLog.Error(LocalSessionPersistenceFail,streamState.SessionID,streamState.ItemID,PublishStreamTransferFail);

                return null;
            }

            this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                DataControlClientWorkingDirectoryStorage.CreateManifest(completed.Data.State,objectPayload),
                this.storage.GetStreamSessionPaths(this.Session.SessionId).SessionDirectoryPath);

            await this.DisposeStreamSource().ConfigureAwait(false);

            return new()
            {
                Stream = completed.Data,
                Session = this.Session,
                TransferFamily = DataControlTransferFamily.Stream,
            };
        }

        if(this.Session.State.Mode != DataItemTransferMode.Upload) { return null; }

        DataControlResponse<PublishSegmentedTransferResponse> published = await this.client.PublishSegmentedTransfer(new PublishSegmentedTransferRequest()
        {
            SessionID = this.Session.SessionId,
            ItemID = this.Session.ItemId,
            ExpectedStateVersion = this.Session.State.StateVersion,
        },this.token,cancel).ConfigureAwait(false);

        ThrowIfBlockLimitExceeded(published.StatusCode,published.Content);

        if(published.StatusCode != HttpStatusCode.OK || published.Data is null) { return null; }

        if(await this.storage.SaveState(published.Data.State,cancel).ConfigureAwait(false) is false)
        {
            KusDepotLog.Error(LocalSessionPersistenceFail,this.Session.SessionId,this.Session.ItemId,PublishSegmentedTransferFail);

            return null;
        }

        this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
            DataControlClientWorkingDirectoryStorage.CreateManifest(published.Data.State),
            this.storage.GetSessionPaths(this.Session.SessionId).SessionDirectoryPath,
            this.Session.UploadSource);

        return new()
        {
            Segmented = published.Data,
            Session = this.Session,
            TransferFamily = DataControlTransferFamily.Segmented,
        };
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="CreateFinalPublishedObjectPayload"]/*'/>*/
    private Byte[] CreateFinalPublishedObjectPayload(DataItem finalitem)
    {
        ArgumentNullException.ThrowIfNull(finalitem);

        if(finalitem.SetID(this.Session.ItemId) is false)
        {
            throw new InvalidOperationException("Unable to match the upload session item ID to the final published item identity.");
        }

        if(finalitem.IsMetadataOnly() is false)
        {
            throw new InvalidOperationException("The final published item must be metadata-only and must not expose active content bindings.");
        }

        return finalitem.Serialize();
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="DeleteLocalState"]/*'/>*/
    public async Task<Boolean> DeleteLocalState(CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        await this.OperationGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return await this.DeleteLocalStateCore(cancel).ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="DeleteLocalStateCore"]/*'/>*/
    private Task<Boolean> DeleteLocalStateCore(CancellationToken cancel)
    {
        return this.Session.TransferFamily is DataControlTransferFamily.Stream
            ? this.storage.DeleteStreamSession(this.Session.SessionId,cancel)
            : this.storage.DeleteSession(this.Session.SessionId,cancel);
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="RemoveTransfer"]/*'/>*/
    public async Task<Boolean> RemoveTransfer(CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        await this.OperationGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return await this.RemoveTransferCore(cancel).ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="RemoveTransferCore"]/*'/>*/
    private async Task<Boolean> RemoveTransferCore(CancellationToken cancel)
    {
        DataControlResponse<RemoveTransferResponse> removed = await this.client.RemoveTransfer(new RemoveTransferRequest(){ SessionID = this.Session.SessionId , ItemID = this.Session.ItemId },this.token,cancel).ConfigureAwait(false);

        if(removed.StatusCode != HttpStatusCode.OK || removed.Data?.Removed is not true) { return false; }

        if(this.Session.TransferFamily is DataControlTransferFamily.Stream)
        {
            await this.storage.DeleteStreamSession(this.Session.SessionId,cancel).ConfigureAwait(false);

            await this.DisposeStreamSource().ConfigureAwait(false);
        }
        else
        {
            await this.storage.DeleteSession(this.Session.SessionId,cancel).ConfigureAwait(false);
        }

        return true;
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="UploadNext"]/*'/>*/
    public async Task<Boolean> UploadNext(CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        await this.OperationGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return await this.UploadNextCore(cancel).ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="UploadNextCore"]/*'/>*/
    private async Task<Boolean> UploadNextCore(CancellationToken cancel)
    {
        if(this.Session.TransferFamily is DataControlTransferFamily.Stream) { return await this.UploadStreamNext(cancel).ConfigureAwait(false); }

        return await this.UploadSegmentNext(cancel).ConfigureAwait(false) is not null;
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="UploadSegmentNext"]/*'/>*/
    private async Task<DataItemTransferRange?> UploadSegmentNext(CancellationToken cancel)
    {
        if(this.Session.TransferFamily is DataControlTransferFamily.Stream) { return null; }

        DataItemTransferState state = this.Session.State;

        if(state.Mode != DataItemTransferMode.Upload || state.MissingRanges.Length == 0) { return null; }

        DataItemTransferRange range = PlanNextRange(state);

        using Stream? payload = await this.storage.OpenUploadSourceRange(state.SessionID,this.Session.UploadSource,range,cancel).ConfigureAwait(false)
            ?? await this.storage.OpenPersistedRange(state.SessionID,range,cancel).ConfigureAwait(false);

        if(payload is null) { return null; }

        Byte[] segmentHash = await SHA512.HashDataAsync(payload,cancel).ConfigureAwait(false);

        if(payload.CanSeek) { payload.Position = 0; }

        PutTransferSegmentClientResponse? response = await this.client.PutTransferSegment(new PutTransferSegmentRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            Offset = range.Offset,
            Length = range.Length,
            ExpectedStateVersion = state.StateVersion,
            SegmentSHA512 = segmentHash,
            RequestID = Guid.NewGuid(),
        },payload,this.token,cancel).ConfigureAwait(false);

        if(response is not null) { ThrowIfBlockLimitExceeded(response.StatusCode,response.Content); }

        if(response is null || response.StatusCode is not (HttpStatusCode.OK or HttpStatusCode.Conflict or HttpStatusCode.PreconditionFailed)) { return null; }

        PutTransferSegmentResponse? written = response.Response;

        Boolean confirmedRange = response.StatusCode == HttpStatusCode.OK && written is not null && written.Accepted &&
            written.SessionID == state.SessionID && written.ItemID == state.ItemID && written.Range == range;

        DataItemTransferState? next = response.StatusCode == HttpStatusCode.OK ? TryAdvanceSegmentedState(state,range,written) : null;

        if(next is null)
        {
            DataControlResponse<GetTransferStateResponse> refreshed = await this.client.GetTransferState(
                new DataItemTransferIdentity(){ SessionID = state.SessionID , ItemID = state.ItemID },this.token,cancel).ConfigureAwait(false);

            if(refreshed.Data is null || refreshed.StatusCode != HttpStatusCode.OK || refreshed.Data.State is null ||
                refreshed.Data.State.SessionID != state.SessionID || refreshed.Data.State.ItemID != state.ItemID ||
                refreshed.Data.State.Mode != DataItemTransferMode.Upload || refreshed.Data.State.StreamLength != state.StreamLength ||
                refreshed.Data.State.StateVersion < state.StateVersion || refreshed.Data.State.ObjectSHA512 is null ||
                state.ObjectSHA512 is null || !refreshed.Data.State.ObjectSHA512.AsSpan().SequenceEqual(state.ObjectSHA512)) { return null; }

            next = refreshed.Data.State;

            confirmedRange = confirmedRange && next.RealizedRanges is not null && next.RealizedRanges.Any(item => item is not null && item.Contains(range));
        }

        if(await this.storage.SaveState(next,cancel).ConfigureAwait(false) is false)
        {
            KusDepotLog.Error(LocalSessionPersistenceFail,state.SessionID,state.ItemID,PutTransferSegmentFail);

            return null;
        }

        this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
            DataControlClientWorkingDirectoryStorage.CreateManifest(next),
            this.storage.GetSessionPaths(state.SessionID).SessionDirectoryPath,
            this.Session.UploadSource);

        return confirmedRange ? range : null;
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="TryAdvanceSegmentedState"]/*'/>*/
    private static DataItemTransferState? TryAdvanceSegmentedState(DataItemTransferState state , DataItemTransferRange range , PutTransferSegmentResponse? response)
    {
        if(response is null || !response.Accepted || response.Replayed || response.SessionID != state.SessionID || response.ItemID != state.ItemID || response.Range != range ||
            response.Updated is null || response.Updated == default(DateTimeOffset) || state.StateVersion < 0 || state.StateVersion == Int64.MaxValue || response.StateVersion != state.StateVersion + 1 ||
            state.Status != DataItemTransferStatus.Open || state.StreamLength < 0 || state.RealizedRanges is null || state.MissingRanges is null ||
            !range.Validate() || range.Length <= 0 || range.EndOffsetExclusive > state.StreamLength) { return null; }

        DataItemTransferManifest manifest = DataControlClientWorkingDirectoryStorage.CreateManifest(state);
        DataItemTransferState canonical = manifest.ToState();

        if(!canonical.RealizedRanges.SequenceEqual(state.RealizedRanges) || !canonical.MissingRanges.SequenceEqual(state.MissingRanges) ||
            canonical.RealizedBytes != state.RealizedBytes || canonical.RemainingBytes != state.RemainingBytes ||
            !canonical.MissingRanges.Any(item => item.Contains(range))) { return null; }

        DataItemTransferState next = (manifest.WithAddedRange(range) with { Updated = response.Updated.Value , Integrity = null }).ToState();

        return next.StateVersion == response.StateVersion && (next.RemainingBytes == 0) == response.CompletedCoverage ? next : null;
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="UploadStreamNext"]/*'/>*/
    private async Task<Boolean> UploadStreamNext(CancellationToken cancel)
    {
        DataStreamTransferState? state = this.Session.StreamState;

        if(state is null || state.Mode != DataItemTransferMode.StreamUpload) { return false; }

        StreamTransferStorageSessionPaths paths = this.storage.GetStreamSessionPaths(state.SessionID);

        Directory.CreateDirectory(paths.SessionDirectoryPath);

        Int64 bufferedLength = File.Exists(paths.StreamPath) ? new FileInfo(paths.StreamPath).Length : 0;

        Int64 unsentLength = Math.Max(0,bufferedLength - state.AppendedLength);

        Int64 plannedLength = PlanNextAppendLength(state);

        if(plannedLength <= 0) { return false; }

        if(unsentLength <= 0)
        {
            unsentLength = await this.BufferNextStreamPayload(paths.StreamPath,plannedLength,cancel).ConfigureAwait(false);

            if(unsentLength <= 0) { return false; }
        }

        Int64 length = Math.Min(unsentLength,plannedLength);

        if(length <= 0) { return false; }

        Stream payload = new BoundedReadStream(new FileStream(paths.StreamPath,
            new FileStreamOptions(){ Access = FileAccess.Read , Mode = FileMode.Open , Share = FileShare.ReadWrite,
            Options = FileOptions.Asynchronous | FileOptions.SequentialScan }),state.AppendedLength,length,leaveopen:false);

        await using (payload.ConfigureAwait(false))
        {
            Byte[] segmentHash = await SHA512.HashDataAsync(payload,cancel).ConfigureAwait(false);

            if(payload.CanSeek) { payload.Position = 0; }

            PutStreamTransferSegmentClientResponse? response = await this.client.PutStreamTransferSegment(new PutStreamTransferSegmentRequest()
            {
                SessionID = state.SessionID,
                ItemID = state.ItemID,
                Offset = state.AppendedLength,
                Length = length,
                ExpectedStateVersion = state.StateVersion,
                SegmentSHA512 = segmentHash,
                RequestID = Guid.NewGuid(),
            },payload,this.token,cancel).ConfigureAwait(false);

            if(response is not null) { ThrowIfBlockLimitExceeded(response.StatusCode,response.Content); }

            if(response?.Response is null || response.StatusCode != HttpStatusCode.OK || response.Response.Accepted is false) { return false; }

            DataControlResponse<GetStreamTransferStateResponse> refreshed = await this.client.GetStreamTransferState(
                new DataItemTransferIdentity(){ SessionID = state.SessionID , ItemID = state.ItemID },this.token,cancel).ConfigureAwait(false);

            if(refreshed.StatusCode != HttpStatusCode.OK || refreshed.Data is null) { return false; }

            Byte[] objectPayload = await this.storage.ReadStreamObjectPayload(state.SessionID,cancel).ConfigureAwait(false) ?? Array.Empty<Byte>();

            if(await this.storage.SaveState(refreshed.Data.State,objectPayload,cancel).ConfigureAwait(false) is false)
            {
                KusDepotLog.Error(LocalSessionPersistenceFail,state.SessionID,state.ItemID,PutStreamTransferSegmentFail);

                return false;
            }

            this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                DataControlClientWorkingDirectoryStorage.CreateManifest(refreshed.Data.State,objectPayload),paths.SessionDirectoryPath);

            return true;
        }
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="UploadToCompletion"]/*'/>*/
    public async Task<Int32> UploadToCompletion(IProgress<DataItemTransferProgress>? progress = null , CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        await this.OperationGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return await this.UploadToCompletionCore(progress,cancel).ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="UploadToCompletionCore"]/*'/>*/
    private async Task<Int32> UploadToCompletionCore(IProgress<DataItemTransferProgress>? progress , CancellationToken cancel)
    {
        if(this.Session.TransferFamily is DataControlTransferFamily.Stream)
        {
            Int32 uploadedStreamSegments = 0;

            while(this.Session.StreamState?.Mode == DataItemTransferMode.StreamUpload)
            {
                DataStreamTransferState currentState = this.Session.StreamState;

                if(this.HasReachedConfiguredByteLimit(currentState)) { break; }

                Int64 priorLength = currentState.AppendedLength;

                if(await this.UploadNextCore(cancel).ConfigureAwait(false) is false) { break; }

                uploadedStreamSegments++;

                if(this.Session.StreamState is { } refreshed && refreshed.AppendedLength > priorLength)
                {
                    progress?.Report(CreateProgress(refreshed,new DataItemTransferRange(){ Offset = priorLength , Length = refreshed.AppendedLength - priorLength }));

                    if(this.CompletionMode is StreamCompletionMode.UntilByteLimit && this.HasReachedConfiguredByteLimit(refreshed)) { break; }
                }
            }

            return uploadedStreamSegments;
        }

        Int32 uploaded = 0;

        while(this.Session.State.Mode == DataItemTransferMode.Upload && this.Session.State.MissingRanges.Length > 0)
        {
            DataItemTransferRange? range = await this.UploadSegmentNext(cancel).ConfigureAwait(false);

            if(range is null) { break; }

            uploaded++;

            progress?.Report(CreateProgress(this.Session.State,range));
        }

        return uploaded;
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="CreateProgress"]/*'/>*/
    private static DataItemTransferProgress CreateProgress(DataItemTransferState state , DataItemTransferRange latestrange)
    {
        return new()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            TransferFamily = DataControlTransferFamily.Segmented,
            Mode = state.Mode,
            Status = state.Status,
            RealizedBytes = state.RealizedBytes,
            RemainingBytes = state.RemainingBytes,
            LatestRange = latestrange,
            StateVersion = state.StateVersion,
        };
    }

    private static DataItemTransferProgress CreateProgress(DataStreamTransferState state , DataItemTransferRange latestrange)
    {
        return new()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            TransferFamily = DataControlTransferFamily.Stream,
            Mode = state.Mode,
            Status = state.Status,
            RealizedBytes = state.AppendedLength,
            RemainingBytes = null,
            LatestRange = latestrange,
            LatestAppendedLength = state.AppendedLength,
            StateVersion = state.StateVersion,
        };
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="PlanNextRange"]/*'/>*/
    private static DataItemTransferRange PlanNextRange(DataItemTransferState state)
    {
        DataItemTransferRange missing = state.MissingRanges[0];

        DataItemTransferSegmentSizePolicy policy = state.SegmentSizePolicy;

        if(policy.Validate() is false || missing.Length <= 0) { return missing; }

        Int64 planned = Math.Min(missing.Length,policy.PreferredSegmentBytes);

        planned = Math.Min(planned,policy.MaxSegmentBytes);

        planned = Math.Max(planned,Math.Min(missing.Length,policy.MinSegmentBytes));

        Int64 remaining = missing.Length - planned;

        Boolean missingEndsAtStreamEnd = missing.EndOffsetExclusive == state.StreamLength;

        if(remaining > 0 && remaining < policy.MinSegmentBytes && !missingEndsAtStreamEnd)
        {
            Int64 adjusted = planned - (policy.MinSegmentBytes - remaining);

            if(adjusted >= policy.MinSegmentBytes) { planned = adjusted; }
        }

        planned = Math.Min(planned,missing.Length);

        return planned == missing.Length ? missing : new DataItemTransferRange(){ Offset = missing.Offset , Length = planned };
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="BufferNextStreamPayload"]/*'/>*/
    private async Task<Int64> BufferNextStreamPayload(String path , Int64 requestedLength , CancellationToken cancel)
    {
        Stream? source = this.StreamSource;

        if(source is null || source.CanRead is false || requestedLength <= 0) { return 0; }

        Byte[] buffer = ArrayPool<Byte>.Shared.Rent(80*KiB);

        try
        {
            await using FileStream stream = new(path,new FileStreamOptions(){ Access = FileAccess.Write , Mode = FileMode.OpenOrCreate,
                Share = FileShare.Read , Options = FileOptions.Asynchronous | FileOptions.SequentialScan });

            stream.Position = stream.Length;

            Int64 buffered = 0;

            while(buffered < requestedLength)
            {
                Int32 read = await source.ReadAsync(buffer.AsMemory(0,(Int32)Math.Min(buffer.Length,requestedLength - buffered)),cancel).ConfigureAwait(false);

                if(read <= 0) { break; }

                await stream.WriteAsync(buffer.AsMemory(0,read),cancel).ConfigureAwait(false);

                buffered += read;
            }

            await stream.FlushAsync(cancel).ConfigureAwait(false);

            return buffered;
        }
        finally
        {
            ArrayPool<Byte>.Shared.Return(buffer);
        }
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="HasReachedConfiguredByteLimit"]/*'/>*/
    private Boolean HasReachedConfiguredByteLimit(DataStreamTransferState state)
    {
        return this.MaxBytes.HasValue && state.AppendedLength >= this.MaxBytes.Value;
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="ThrowIfBlockLimitExceeded"]/*'/>*/
    private static void ThrowIfBlockLimitExceeded(HttpStatusCode status , String? content)
    {
        TransferBlockLimitExceededException? error = TransferBlockLimitExceededException.FromResponse(status,content);

        if(error is not null) { throw error; }
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="PlanNextAppendLength"]/*'/>*/
    private Int64 PlanNextAppendLength(DataStreamTransferState state)
    {
        Int64 preferred = state.SegmentSizePolicy.Validate() ? state.SegmentSizePolicy.PreferredSegmentBytes : 80*KiB;

        preferred = Math.Max(1,preferred);

        if(this.MaxBytes.HasValue)
        {
            Int64 remaining = this.MaxBytes.Value - state.AppendedLength;

            if(remaining <= 0) { return 0; }

            preferred = Math.Min(preferred,remaining);
        }

        return preferred;
    }

    /**<include file='DataControlUploadSession.xml' path='DataControlUploadSession/class[@name="DataControlUploadSession"]/method[@name="DisposeStreamSource"]/*'/>*/
    private async ValueTask DisposeStreamSource()
    {
        Stream? streamSource = Interlocked.Exchange(ref this.StreamSource,null);

        if(streamSource is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);

            return;
        }

        streamSource?.Dispose();
    }
}