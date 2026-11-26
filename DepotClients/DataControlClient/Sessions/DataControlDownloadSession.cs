namespace KusDepot.Data.Clients;

/**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/main/*'/>*/
public sealed class DataControlDownloadSession : IDisposable , IAsyncDisposable
{
    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="DownloadRequestSizeBytes"]/*'/>*/
    private readonly Int64 DownloadRequestSizeBytes;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="DownloadStateRefreshInterval"]/*'/>*/
    private readonly TimeSpan DownloadStateRefreshInterval;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="client"]/*'/>*/
    private readonly DataControlClient client;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="storage"]/*'/>*/
    private readonly DataControlClientWorkingDirectoryStorage storage;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="token"]/*'/>*/
    private readonly String? token;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="FollowStream"]/*'/>*/
    private readonly BufferedFollowStream? FollowStream;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="FollowSubscription"]/*'/>*/
    private readonly IDataControlNotificationSubscriptionHandle? FollowSubscription;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="disposed"]/*'/>*/
    private Int32 disposed;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="OperationGate"]/*'/>*/
    private readonly SemaphoreSlim OperationGate = new(1,1);

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="PendingStreamCheckpoint"]/*'/>*/
    private DataStreamTransferState? PendingStreamCheckpoint;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="ShutdownCancellation"]/*'/>*/
    private readonly CancellationTokenSource ShutdownCancellation = new();

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/constructor[@name="Constructor"]/*'/>*/
    internal DataControlDownloadSession(DataControlClient client , DataControlClientWorkingDirectoryStorage storage , String? token,
        DataControlTransferSessionInfo session , BufferedFollowStream? followstream = null , DataStreamItem? liveitem = null,
        IDataControlNotificationSubscriptionHandle? followsubscription = null , Int64 downloadRequestSizeBytes = 8*MiB,
        TimeSpan downloadStateRefreshInterval = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(downloadRequestSizeBytes);

        this.client = client;
        this.storage = storage;
        this.token = token;
        this.DownloadRequestSizeBytes = downloadRequestSizeBytes;
        this.DownloadStateRefreshInterval = downloadStateRefreshInterval > TimeSpan.Zero ? downloadStateRefreshInterval : TimeSpan.FromHours(1);
        this.FollowStream = followstream;
        this.FollowSubscription = followsubscription;
        this.Session = session;
        this.LiveItem = liveitem;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/property[@name="Session"]/*'/>*/
    public DataControlTransferSessionInfo Session { get; private set; }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/property[@name="LiveItem"]/*'/>*/
    public DataStreamItem? LiveItem { get; }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="Dispose"]/*'/>*/
    public void Dispose()
    {
        if(Interlocked.Exchange(ref this.disposed,1) != 0) { return; }

        this.ShutdownCancellation.Cancel();

        this.OperationGate.Wait();

        try
        {
            this.FollowStream?.Dispose();

            if(this.FollowSubscription is IAsyncDisposable asyncDisposable)
            {
                asyncDisposable.DisposeAsync().AsTask().GetAwaiter().GetResult();
            }
        }
        finally
        {
            this.ShutdownCancellation.Dispose();

            this.OperationGate.Release();
        }

        GC.SuppressFinalize(this);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DisposeAsync"]/*'/>*/
    public async ValueTask DisposeAsync()
    {
        if(Interlocked.Exchange(ref this.disposed,1) != 0) { return; }

        this.ShutdownCancellation.Cancel();

        await this.OperationGate.WaitAsync().ConfigureAwait(false);

        try
        {
            this.FollowStream?.Dispose();

            if(this.FollowSubscription is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            this.ShutdownCancellation.Dispose();

            this.OperationGate.Release();
        }

        GC.SuppressFinalize(this);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="Abort"]/*'/>*/
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

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="Close"]/*'/>*/
    public async Task<Boolean> Close(CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        await this.OperationGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return await this.CloseCore(cancel).ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="AbortCore"]/*'/>*/
    private async Task<Boolean> AbortCore(CancellationToken cancel)
    {
        DataItemTransferMode mode = this.Session.TransferFamily is DataControlTransferFamily.Stream ? this.Session.StreamState?.Mode ?? this.Session.State.Mode : this.Session.State.Mode;

        if(mode is DataItemTransferMode.PublishedRead or DataItemTransferMode.StagedRead or DataItemTransferMode.StreamFollow) { return await this.CloseCore(cancel).ConfigureAwait(false); }

        DataControlResponse<AbortTransferResponse> aborted = await this.client.AbortTransfer(new AbortTransferRequest(){ SessionID = this.Session.SessionId , ItemID = this.Session.ItemId },this.token,cancel).ConfigureAwait(false);

        if(aborted.StatusCode != HttpStatusCode.OK || aborted.Data?.Aborted is not true) { return false; }

        await this.DeleteLocalSessionAsync(cancel).ConfigureAwait(false);

        return true;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="CloseCore"]/*'/>*/
    private async Task<Boolean> CloseCore(CancellationToken cancel)
    {
        DataControlResponse<CloseTransferSessionResponse> closed = await this.client.CloseTransferSession(new CloseTransferSessionRequest { SessionID = this.Session.SessionId , ItemID = this.Session.ItemId },this.token,cancel).ConfigureAwait(false);

        if(closed.StatusCode != HttpStatusCode.OK || closed.Data?.Closed is not true) { return false; }

        await this.DeleteLocalSessionAsync(cancel).ConfigureAwait(false);

        return true;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DeleteLocalSessionAsync"]/*'/>*/
    private async Task DeleteLocalSessionAsync(CancellationToken cancel)
    {
        if(this.Session.TransferFamily is DataControlTransferFamily.Stream)
        {
            this.FollowStream?.MarkAborted();

            this.LiveItem?.AbortLiveContent();

            await this.storage.DeleteStreamSession(this.Session.SessionId,cancel).ConfigureAwait(false);
        }
        else
        {
            await this.storage.DeleteSession(this.Session.SessionId,cancel).ConfigureAwait(false);
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DeleteLocalState"]/*'/>*/
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

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DeleteLocalStateCore"]/*'/>*/
    private Task<Boolean> DeleteLocalStateCore(CancellationToken cancel)
    {
        return this.Session.TransferFamily is DataControlTransferFamily.Stream
            ? this.storage.DeleteStreamSession(this.Session.SessionId,cancel)
            : this.storage.DeleteSession(this.Session.SessionId,cancel);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadNext"]/*'/>*/
    public async Task<Boolean> DownloadNext(CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        await this.OperationGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return await this.DownloadNextCore(cancel).ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadNextCore"]/*'/>*/
    private async Task<Boolean> DownloadNextCore(CancellationToken cancel)
    {
        if(this.Session.TransferFamily is DataControlTransferFamily.Stream) { return await this.DownloadStreamNext(cancel).ConfigureAwait(false) is not null; }

        return await this.DownloadSegmentNext(cancel).ConfigureAwait(false) is not null;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadSegmentNext"]/*'/>*/
    private async Task<DataItemTransferRange?> DownloadSegmentNext(CancellationToken cancel)
    {
        if(this.Session.TransferFamily is DataControlTransferFamily.Stream) { return null; }

        DataItemTransferState state = this.Session.State;

        if((state.Mode != DataItemTransferMode.PublishedRead && state.Mode != DataItemTransferMode.StagedRead) || state.MissingRanges.Length == 0) { return null; }

        if(this.client is null) { return null; }

        DataItemTransferRange range = PlanNextRange(state);

        if(state.Mode is DataItemTransferMode.StagedRead && state.SourceSessionID is Guid sourceSessionId)
        {
            DataControlResponse<GetTransferStateResponse> sourceState = await this.client.GetTransferState(
                new DataItemTransferIdentity(){ SessionID = sourceSessionId , ItemID = state.ItemID },this.token,cancel).ConfigureAwait(false);

            if(sourceState.StatusCode != HttpStatusCode.OK || sourceState.Data is null) { return null; }

            DataItemTransferRange? available = sourceState.Data.State.RealizedRanges
                .FirstOrDefault(realized => realized.Offset <= range.Offset && realized.EndOffsetExclusive > range.Offset);

            if(available is null) { return null; }

            range = range with { Length = Math.Min(range.Length,available.EndOffsetExclusive - range.Offset) };
        }

        using MemoryStream payload = new();

        SegmentDownloadInfo? segment = await this.client.DownloadTransferSegmentToStream(new GetTransferSegmentRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            Offset = range.Offset,
            Length = range.Length,
        },payload,this.token,cancel).ConfigureAwait(false);

        if(segment is null || segment.StatusCode != HttpStatusCode.OK || segment.Footer.ReturnedOffset != range.Offset || segment.Footer.ReturnedLength != range.Length) { return null; }

        payload.Position = 0;

        SegmentedTransferWriteResult written = await this.storage.WriteRange(state.SessionID,new DataItemTransferRange(){ Offset = segment.Footer.ReturnedOffset , Length = segment.Footer.ReturnedLength },payload,cancel).ConfigureAwait(false);

        if(written.Accepted is false) { return null; }

        DataItemTransferState merged;

        if(state.Mode is DataItemTransferMode.PublishedRead)
        {
            merged = state with
            {
                RealizedRanges = state.RealizedRanges.Concat(new[]{ new DataItemTransferRange(){ Offset = segment.Footer.ReturnedOffset , Length = segment.Footer.ReturnedLength } }).ToArray(),
                Updated = DateTimeOffset.UtcNow,
            };
        }
        else
        {
            DataControlResponse<GetTransferStateResponse> refreshed = await this.client.GetTransferState(new DataItemTransferIdentity(){ SessionID = state.SessionID , ItemID = state.ItemID },this.token,cancel).ConfigureAwait(false);

            if(refreshed.StatusCode != HttpStatusCode.OK || refreshed.Data is null) { return null; }

            merged = refreshed.Data.State with { RealizedRanges = state.RealizedRanges.Concat(new[]{ new DataItemTransferRange(){ Offset = segment.Footer.ReturnedOffset , Length = segment.Footer.ReturnedLength } }).ToArray() };
        }
        
        DataItemTransferState persisted = DataControlClientWorkingDirectoryStorage.CreateManifest(merged).ToState();

        if(persisted.Mode is DataItemTransferMode.PublishedRead && persisted.MissingRanges.Length == 0)
        {
            persisted = persisted with { Status = DataItemTransferStatus.Complete };
        }

        if(await this.storage.SaveState(persisted,cancel).ConfigureAwait(false) is false)
        {
            KusDepotLog.Error(LocalSessionPersistenceFail,state.SessionID,state.ItemID,DownloadTransferSegmentToStreamFail);

            return null;
        }
        
        this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(DataControlClientWorkingDirectoryStorage.CreateManifest(persisted),this.storage.GetSessionPaths(state.SessionID).SessionDirectoryPath);
        
        return new DataItemTransferRange(){ Offset = segment.Footer.ReturnedOffset , Length = segment.Footer.ReturnedLength };
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadStreamNext"]/*'/>*/
    private async Task<StreamTransferSegmentFooter?> DownloadStreamNext(CancellationToken cancel)
    {
        DataStreamTransferState? state = this.Session.StreamState;

        if(state is null || state.Mode != DataItemTransferMode.StreamFollow) { return null; }

        if(this.client is null) { return null; }

        if(state.SourceSessionID.HasValue is false || state.SourceSessionID.Value == Guid.Empty) { return null; }

        StreamTransferStorageSessionPaths sessionPaths = this.storage.GetStreamSessionPaths(state.SessionID);

        if(this.PendingStreamCheckpoint is DataStreamTransferState pending)
        {
            DataStreamTransferState? saved = await this.storage.LoadStreamState(state.SessionID,cancel).ConfigureAwait(false);

            if(saved is null || saved.SessionID != state.SessionID || saved.ItemID != state.ItemID || saved.SourceSessionID != state.SourceSessionID
                || saved.Mode != state.Mode || (saved.AppendedLength != state.AppendedLength && saved.AppendedLength != pending.AppendedLength)) { return null; }

            if(this.storage.ReconcileStreamFollowFile(state.SessionID,saved.AppendedLength) is false) { return null; }

            if(this.FollowStream is not null) { await this.FollowStream.AdvanceReadableLengthAsync(saved.AppendedLength).ConfigureAwait(false); }

            state = saved;

            this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                DataControlClientWorkingDirectoryStorage.CreateManifest(saved,Array.Empty<Byte>()),sessionPaths.SessionDirectoryPath);

            this.PendingStreamCheckpoint = null;

            if(saved.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Published)
            {
                this.FollowStream?.MarkCompleted(); this.LiveItem?.CompleteLiveContent();

                return null;
            }
        }

        if(this.storage.ReconcileStreamFollowFile(state.SessionID,state.AppendedLength) is false) { return null; }

        DataControlResponse<ReOpenFollowStreamTransferResponse> refreshed = await this.client.ReOpenFollowStreamTransfer(new ReOpenFollowStreamTransferRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            SourceSessionID = state.SourceSessionID.Value,
        },this.token,cancel).ConfigureAwait(false);

        if(refreshed.StatusCode != HttpStatusCode.OK || refreshed.Data is null) { return null; }

        DataStreamTransferState sourceState = refreshed.Data.State;

        Int64 localLength = state.AppendedLength;

        if(sourceState.SessionID != state.SessionID || sourceState.ItemID != state.ItemID || sourceState.SourceSessionID != state.SourceSessionID
            || sourceState.Mode != state.Mode || localLength > sourceState.AppendedLength) { return null; }

        DataStreamTransferState persisted = sourceState with
        {
            AppendedLength = localLength,
            Status = localLength == sourceState.AppendedLength ? sourceState.Status : DataItemTransferStatus.Open,
        };

        if(await this.storage.SaveState(persisted,refreshed.Data.ObjectPayload,cancel).ConfigureAwait(false) is false)
        {
            KusDepotLog.Error(LocalSessionPersistenceFail,state.SessionID,state.ItemID,ReOpenFollowStreamTransferFail);

            return null;
        }

        this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
            DataControlClientWorkingDirectoryStorage.CreateManifest(persisted,refreshed.Data.ObjectPayload),sessionPaths.SessionDirectoryPath);

        Int64 available = Math.Max(0,sourceState.AppendedLength - localLength);

        if(available <= 0)
        {
            if(sourceState.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Published)
            {
                DataStreamTransferState completed = persisted with { Status = sourceState.Status };

                if(await this.storage.SaveState(completed,refreshed.Data.ObjectPayload,cancel).ConfigureAwait(false) is false) { return null; }

                this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                    DataControlClientWorkingDirectoryStorage.CreateManifest(completed,refreshed.Data.ObjectPayload),sessionPaths.SessionDirectoryPath);

                this.FollowStream?.MarkCompleted();

                this.LiveItem?.CompleteLiveContent();
            }

            return null;
        }

        Int64 plannedLength = this.PlanNextDownloadLength(available); if(plannedLength <= 0) { return null; }

        using MemoryStream payload = new();

        StreamSegmentDownloadInfo? segment = await this.client.DownloadStreamTransferSegmentToStream(new GetStreamTransferSegmentRequest()
        {
            SessionID = sourceState.SessionID,
            ItemID = sourceState.ItemID,
            Offset = localLength,
            Length = plannedLength,
        },payload,this.token,cancel).ConfigureAwait(false);

        if(segment is null || segment.StatusCode != HttpStatusCode.OK || segment.Footer.ReturnedLength <= 0) { return null; }

        if(segment.Footer.ReturnedOffset != localLength || segment.Footer.ReturnedLength > plannedLength || payload.Length != segment.Footer.ReturnedLength) { return null; }

        payload.Position = 0;

        await using (FileStream stream = new(sessionPaths.StreamPath,new FileStreamOptions(){ Access = FileAccess.Write , Mode = FileMode.Open,
            Share = FileShare.ReadWrite , Options = FileOptions.Asynchronous | FileOptions.SequentialScan }))
        {
            if(stream.Length != localLength) { return null; }

            try
            {
                stream.Position = localLength;
                await payload.CopyToAsync(stream,CancellationToken.None).ConfigureAwait(false);

                await stream.FlushAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch
            {
                stream.SetLength(localLength);

                throw;
            }
        }

        Int64 realizedLength = checked(localLength + segment.Footer.ReturnedLength);

        DataStreamTransferState localPersisted = persisted with
        {
            AppendedLength = realizedLength,

            Status = realizedLength >= sourceState.AppendedLength ? sourceState.Status : DataItemTransferStatus.Open,
        };

        this.PendingStreamCheckpoint = localPersisted;

        if(await this.storage.SaveState(localPersisted,cancel:CancellationToken.None).ConfigureAwait(false) is false)
        {
            KusDepotLog.Error(LocalSessionPersistenceFail,state.SessionID,state.ItemID,DownloadStreamTransferSegmentToStreamFail);

            return null;
        }

        this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
            DataControlClientWorkingDirectoryStorage.CreateManifest(localPersisted,refreshed.Data.ObjectPayload),sessionPaths.SessionDirectoryPath);

        if(this.FollowStream is not null) { await this.FollowStream.AdvanceReadableLengthAsync(realizedLength).ConfigureAwait(false); }

        this.PendingStreamCheckpoint = null;

        if(localPersisted.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Published)
        {
            this.FollowStream?.MarkCompleted(); this.LiveItem?.CompleteLiveContent();
        }

        return segment.Footer with { AppendedLength = sourceState.AppendedLength };
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadToCompletion"]/*'/>*/
    public async Task<Int32> DownloadToCompletion(IProgress<DataItemTransferProgress>? progress = null , CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        await this.OperationGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return await this.DownloadToCompletionCore(progress,cancel).ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadToCompletionCore"]/*'/>*/
    private Task<Int32> DownloadToCompletionCore(IProgress<DataItemTransferProgress>? progress , CancellationToken cancel)
    {
        return this.DownloadCore(progress,Timeout.InfiniteTimeSpan,cancel);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadToCurrent"]/*'/>*/
    public async Task<Int32> DownloadToCurrent(IProgress<DataItemTransferProgress>? progress = null , TimeSpan wait = default , CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        await this.OperationGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return await this.DownloadToCurrentCore(progress,wait,cancel).ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadToCurrentCore"]/*'/>*/
    private Task<Int32> DownloadToCurrentCore(IProgress<DataItemTransferProgress>? progress , TimeSpan wait , CancellationToken cancel)
    {
        return this.DownloadCore(progress,wait,cancel);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadCore"]/*'/>*/
    private async Task<Int32> DownloadCore(IProgress<DataItemTransferProgress>? progress , TimeSpan wait , CancellationToken cancel)
    {
        Boolean shouldWaitForFutureData = wait != TimeSpan.Zero;

        if(this.Session.TransferFamily is DataControlTransferFamily.Stream && this.Session.StreamState is { } initialStreamState)
        {
            progress?.Report(CreateProgress(initialStreamState,null));
        }
        else if(this.Session.TransferFamily is DataControlTransferFamily.Segmented)
        {
            progress?.Report(CreateProgress(this.Session.State,null));
        }

        cancel.ThrowIfCancellationRequested();

        if(this.Session.TransferFamily is DataControlTransferFamily.Stream)
        {
            Int32 downloadedStreamSegments = 0;

            while(this.Session.StreamState?.Mode == DataItemTransferMode.StreamFollow)
            {
                StreamTransferSegmentFooter? downloadedSegment = await this.DownloadStreamNext(cancel).ConfigureAwait(false);

                if(downloadedSegment is null)
                {
                    if(this.Session.StreamState?.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Published
                                                        or DataItemTransferStatus.Aborted or DataItemTransferStatus.Faulted) { break; }

                    if(shouldWaitForFutureData)
                    {
                        await this.WaitForFollowWakeAsync(wait,cancel).ConfigureAwait(false);

                        continue;
                    }

                    break;
                }

                downloadedStreamSegments++;

                if(this.Session.StreamState is { } refreshedState)
                {
                    progress?.Report(CreateProgress(refreshedState,downloadedSegment));

                }
            }

            return downloadedStreamSegments;
        }

        Int32 downloaded = 0;

        while(this.Session.State.Mode == DataItemTransferMode.PublishedRead || this.Session.State.Mode == DataItemTransferMode.StagedRead)
        {
            if(IsCaughtUpStagedFollow(this.Session.State))
            {
                if(ShouldStopCaughtUpStagedFollow(this.Session.State,shouldWaitForFutureData))
                {
                    break;
                }

                if(await this.ReOpenSegmentedFollowSession(this.Session.State,cancel).ConfigureAwait(false) is false) { break; }

                if(ShouldContinueAfterStagedFollowReopen(this.Session.State))
                {
                    continue;
                }

                await this.WaitForFollowWakeAsync(wait,cancel).ConfigureAwait(false);

                if(await this.ReOpenSegmentedFollowSession(this.Session.State,cancel).ConfigureAwait(false) is false) { break; }

                continue;
            }

            DataItemTransferRange? range = await this.DownloadSegmentNext(cancel).ConfigureAwait(false);

            if(range is null)
            {
                if(CanWaitForStagedFollowData(this.Session.State,shouldWaitForFutureData))
                {
                    if(await this.ReOpenSegmentedFollowSession(this.Session.State,cancel).ConfigureAwait(false) is false) { break; }

                    if(ShouldContinueAfterStagedFollowReopen(this.Session.State))
                    {
                        continue;
                    }

                    await this.WaitForFollowWakeAsync(wait,cancel).ConfigureAwait(false);

                    if(await this.ReOpenSegmentedFollowSession(this.Session.State,cancel).ConfigureAwait(false) is false) { break; }

                    continue;
                }

                break;
            }

            downloaded++;

            progress?.Report(CreateProgress(this.Session.State,range));
        }

        return downloaded;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="ReOpenSegmentedFollowSession"]/*'/>*/
    private async Task<Boolean> ReOpenSegmentedFollowSession(DataItemTransferState state,CancellationToken cancel)
    {
        DataControlResponse<ReOpenGetTransferResponse> reopened = await this.client.ReOpenGetTransfer(new ReOpenGetTransferRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
        },this.token,cancel).ConfigureAwait(false);

        if(reopened.StatusCode != HttpStatusCode.OK || reopened.Data is null) { return false; }

        DataItemTransferState refreshed = reopened.Data.State;

        Boolean integrityValid = refreshed.Integrity is not null
            && refreshed.Integrity.Validate()
            && refreshed.Integrity.Length == refreshed.StreamLength;

        if(refreshed.StreamLength != state.StreamLength
            || (refreshed.Mode is DataItemTransferMode.PublishedRead
                ? integrityValid is false
                : IntegrityMatches(refreshed.Integrity,state.Integrity) is false)) { return false; }

        DataItemTransferState persisted = refreshed.Mode is DataItemTransferMode.PublishedRead
            ? refreshed with
            {
                RealizedRanges = state.RealizedRanges,
                Status = state.MissingRanges.Length == 0 ? DataItemTransferStatus.Complete : refreshed.Status,
            }
            : refreshed with
            {
                RealizedRanges = state.RealizedRanges,
            };

        persisted = DataControlClientWorkingDirectoryStorage.CreateManifest(persisted).ToState();

        if(await this.storage.SaveState(persisted,cancel).ConfigureAwait(false) is false)
        {
            KusDepotLog.Error(LocalSessionPersistenceFail,state.SessionID,state.ItemID,ReOpenGetTransferFail);

            return false;
        }

        this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
            DataControlClientWorkingDirectoryStorage.CreateManifest(persisted),
            this.storage.GetSessionPaths(state.SessionID).SessionDirectoryPath);

        return true;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="Materialize"]/*'/>*/
    public Task<DataControlDownloadCompletionResult?> Materialize(CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        return this.Materialize(null,cancel);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="MaterializeWithDestination"]/*'/>*/
    public async Task<DataControlDownloadCompletionResult?> Materialize(String? destinationdirectory , CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed != 0,this);

        await this.OperationGate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            ObjectDisposedException.ThrowIf(this.disposed != 0,this);

            return await this.MaterializeCore(destinationdirectory,cancel).ConfigureAwait(false);
        }
        finally
        {
            this.OperationGate.Release();
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="MaterializeCore"]/*'/>*/
    private async Task<DataControlDownloadCompletionResult?> MaterializeCore(String? destinationdirectory , CancellationToken cancel)
    {
        if(this.PendingStreamCheckpoint is not null) { return null; }

        if(this.Session.TransferFamily is DataControlTransferFamily.Stream) { return await this.MaterializeStream(destinationdirectory,cancel).ConfigureAwait(false); }

        DataItemTransferState state = this.Session.State;

        SegmentedTransferStorageSessionPaths paths = this.storage.GetSessionPaths(state.SessionID);

        if(state.MissingRanges.Length != 0) { return null; }

        Byte[]? objectPayload = await this.storage.ReadObjectPayload(state.SessionID,cancel).ConfigureAwait(false);

        if(objectPayload is null || objectPayload.Length == 0) { return null; }

        GetTransferIntegrityEvidenceResponse? evidence = await this.VerifyMaterializationAsync(state.SessionID,state.ItemID,state.SourceSessionID,state.ObjectSHA512,state.Integrity,state.StreamLength,objectPayload,paths.StreamPath,DataControlTransferFamily.Segmented,cancel).ConfigureAwait(false);

        if(evidence is null) { return null; }

        DataItem? item = DataItem.Deserialize(objectPayload); if(item is null) { return null; }

        if(item.GetLocked()) { return null; }

        String objectPath = paths.ObjectPath;
        String streamPath = paths.StreamPath;

        if(destinationdirectory is not null)
        {
            String? exportedObjectPath = await this.ExportMaterializedFile(paths.ObjectPath,destinationdirectory,item.GetObjectFILE(),(path,token) => VerifyObjectFileAsync(path,state.ObjectSHA512,token),cancel).ConfigureAwait(false);

            if(exportedObjectPath is null) { return null; }

            objectPath = exportedObjectPath;

            String? exportedStreamPath = await this.ExportMaterializedFile(paths.StreamPath,destinationdirectory,item.GetFILE(),(path,token) => VerifyStreamPayloadAsync(evidence,path,token),cancel,objectPath).ConfigureAwait(false);

            if(exportedStreamPath is null) { return null; }

            streamPath = exportedStreamPath;
        }

        if(File.Exists(streamPath) is false) { return null; }

        if(PathsMatch(item.GetObjectFILE(),objectPath) is false && item.SetObjectFILE(objectPath) is false) { return null; }

        if(await this.BindMaterializedStreamPath(item,streamPath,bindstreamcontent:state.StreamLength > 0,cancel).ConfigureAwait(false) is false) { return null; }

        if(destinationdirectory is not null && await this.DeleteLocalStateCore(cancel).ConfigureAwait(false) is false) { return null; }

        return new()
        {
            LiveItem = this.LiveItem,
            TransferFamily = DataControlTransferFamily.Segmented,
            Item = item,
            StreamPath = streamPath,
            Session = this.Session,
            IsProvisional = false,
        };
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="MaterializeStream"]/*'/>*/
    private async Task<DataControlDownloadCompletionResult?> MaterializeStream(String? destinationdirectory , CancellationToken cancel)
    {
        DataStreamTransferState? state = this.Session.StreamState;

        if(state is null) { return null; }

        StreamTransferStorageSessionPaths paths = this.storage.GetStreamSessionPaths(state.SessionID);

        if(state.Mode is DataItemTransferMode.StreamFollow && this.LiveItem is null && state.Status is not DataItemTransferStatus.Complete and not DataItemTransferStatus.Published) { return null; }

        Byte[]? objectPayload = await this.storage.ReadStreamObjectPayload(state.SessionID,cancel).ConfigureAwait(false);

        if(objectPayload is null || objectPayload.Length == 0) { return null; }

        Boolean isFollowingStream = state.Mode is DataItemTransferMode.StreamFollow;

        GetTransferIntegrityEvidenceResponse? evidence = await this.VerifyMaterializationAsync(state.SessionID,state.ItemID,isFollowingStream ? state.SourceSessionID : null,state.ObjectSHA512,state.Integrity,state.AppendedLength,objectPayload,paths.StreamPath,DataControlTransferFamily.Stream,cancel).ConfigureAwait(false);

        if(evidence is null) { return null; }

        DataItem? item = this.LiveItem ?? DataItem.Deserialize(objectPayload); if(item is null) { return null; }

        if(item.GetLocked()) { return null; }

        Boolean allowStreamExport = state.Mode is not DataItemTransferMode.StreamFollow || state.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Published;

        if(destinationdirectory is not null && allowStreamExport is false) { return null; }

        String objectPath = paths.ObjectPath;
        String streamPath = paths.StreamPath;

        if(destinationdirectory is not null)
        {
            String? exportedObjectPath = await this.ExportMaterializedFile(paths.ObjectPath,destinationdirectory,item.GetObjectFILE(),(path,token) => VerifyObjectFileAsync(path,state.ObjectSHA512,token),cancel).ConfigureAwait(false);

            if(exportedObjectPath is null) { return null; }

            objectPath = exportedObjectPath;

            String? exportedStreamPath = await this.ExportMaterializedFile(paths.StreamPath,destinationdirectory,item.GetFILE(),(path,token) => VerifyStreamPayloadAsync(evidence,path,token),cancel,objectPath).ConfigureAwait(false);

            if(exportedStreamPath is null) { return null; }

            streamPath = exportedStreamPath;
        }

        if(File.Exists(streamPath) is false) { return null; }

        if(PathsMatch(item.GetObjectFILE(),objectPath) is false && item.SetObjectFILE(objectPath) is false) { return null; }

        if(item is DataStreamItem live && ReferenceEquals(live,this.LiveItem))
        {
            if(state.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Published)
            {
                this.FollowStream?.MarkCompleted();

                live.CompleteLiveContent();
            }
        }
        else if(await this.BindMaterializedStreamPath(item,streamPath,bindstreamcontent:true,cancel).ConfigureAwait(false) is false) { return null; }

        if(destinationdirectory is not null && await this.DeleteLocalStateCore(cancel).ConfigureAwait(false) is false) { return null; }

        return new()
        {
            LiveItem = this.LiveItem,
            TransferFamily = DataControlTransferFamily.Stream,
            Item = item,
            StreamPath = streamPath,
            Session = this.Session,
            IsProvisional = isFollowingStream && !evidence.IsPublished,
        };
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="BindMaterializedStreamPath"]/*'/>*/
    private async Task<Boolean> BindMaterializedStreamPath(DataItem item , String streampath , Boolean bindstreamcontent , CancellationToken cancel)
    {
        switch(item)
        {
            case DataStreamItem:

                return PathsMatch(item.GetFILE(),streampath) || item.SetFILE(streampath);

            case DataItem dataItem when dataItem.GetContentStreamed() is true:

                return PathsMatch(dataItem.GetFILE(),streampath) || dataItem.SetFILE(streampath);

            case DataItem dataItem when bindstreamcontent:

                { dataItem.SetFILE(streampath); return await dataItem.SetContentStreamed(true,cancel:cancel).ConfigureAwait(false); }

            default: return true;
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="VerifyMaterializationAsync"]/*'/>*/
    private async Task<GetTransferIntegrityEvidenceResponse?> VerifyMaterializationAsync(Guid sessionid , Guid itemid , Guid? sourceSessionID , Byte[] expectedObjectSHA512 , TransferContentIntegrity? expectedIntegrity , Int64 expectedStreamLength , Byte[] objectPayload , String streamPath , DataControlTransferFamily family , CancellationToken cancel)
    {
        if(await VerifyObjectPayloadAsync(expectedObjectSHA512,objectPayload,cancel).ConfigureAwait(false) is false) { return null; }

        if(expectedStreamLength < 0 || File.Exists(streamPath) is false) { return null; }

        FileInfo streamInfo = new(streamPath); if(streamInfo.Length != expectedStreamLength) { return null; }

        DataControlResponse<GetTransferIntegrityEvidenceResponse> response = await this.client.GetTransferIntegrityEvidence(new GetTransferIntegrityEvidenceRequest
        {
            ItemID = itemid,
            SessionID = sessionid,
            TransferFamily = family,
            PrefixLength = expectedStreamLength,
        },this.token,cancel).ConfigureAwait(false);

        GetTransferIntegrityEvidenceResponse? evidence = response.StatusCode is HttpStatusCode.OK ? response.Data : null;

        if(evidence is null || evidence.ItemID != itemid || evidence.SessionID != sessionid || evidence.Integrity is null || !evidence.Integrity.Validate() || evidence.Integrity.Length != expectedStreamLength) { return null; }
        if(sourceSessionID.HasValue && evidence.SourceSessionID != sourceSessionID) { return null; }
        if(evidence.IsPublished && evidence.SourceSessionID.HasValue && !evidence.SourceStateVersion.HasValue) { return null; }
        if(!evidence.IsPublished && evidence.SourceSessionID.HasValue && evidence.SourceStateVersion is null) { return null; }
        if(expectedIntegrity is not null && expectedIntegrity.Length == expectedStreamLength && !IntegrityMatches(expectedIntegrity,evidence.Integrity)) { return null; }

        if(await VerifyStreamPayloadAsync(evidence,streamPath,cancel).ConfigureAwait(false) is false) { return null; }

        return evidence;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="VerifyObjectPayloadAsync"]/*'/>*/
    private static async Task<Boolean> VerifyObjectPayloadAsync(Byte[] expectedObjectSHA512 , Byte[] objectPayload , CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        if(expectedObjectSHA512 is null || expectedObjectSHA512.Length != 64 || objectPayload is null || objectPayload.Length == 0) { return false; }

        Byte[] objectHash = objectPayload.Length >= LargeObjectPayloadSize
            ? await ComputeSHA512Async(objectPayload,MiB,cancel).ConfigureAwait(false)
            : SHA512.HashData(objectPayload);

        return objectHash.AsSpan().SequenceEqual(expectedObjectSHA512);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="VerifyStreamPayloadAsync"]/*'/>*/
    private static async Task<Boolean> VerifyStreamPayloadAsync(GetTransferIntegrityEvidenceResponse evidence , String streamPath , CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        if(evidence.Integrity is null || !evidence.Integrity.Validate() || evidence.Integrity.Length < 0 || File.Exists(streamPath) is false || evidence.Segments is null) { return false; }

        List<TransferContentIntegritySegment> actualSegments = new(evidence.Segments.Length);

        await using FileStream stream = new(streamPath,FileMode.Open,FileAccess.Read,FileShare.Read,MiB,FileOptions.Asynchronous | FileOptions.SequentialScan);

        Int64 expectedOffset = 0;

        foreach(TransferContentIntegritySegment segment in evidence.Segments)
        {
            if(segment is null || segment.Offset != expectedOffset || segment.Length <= 0 || segment.SHA512 is null || segment.SHA512.Length != 64 || segment.Length > evidence.Integrity.Length - expectedOffset) { return false; }

            Byte[] actualHash = await HashNextStreamSegmentAsync(stream,segment.Length,cancel).ConfigureAwait(false);

            if(actualHash.AsSpan().SequenceEqual(segment.SHA512) is false) { return false; }

            actualSegments.Add(new() { Offset = segment.Offset , Length = segment.Length , SHA512 = actualHash });

            expectedOffset = checked(expectedOffset + segment.Length);
        }

        if(expectedOffset != evidence.Integrity.Length) { return false; }

        TransferContentIntegrity calculated;

        try { calculated = TransferContentIntegrityCalculator.Compute(actualSegments); } catch ( ArgumentException ) { return false; }

        return IntegrityMatches(calculated,evidence.Integrity);
    }

    private static async Task<Byte[]> HashNextStreamSegmentAsync(Stream stream , Int64 length , CancellationToken cancel)
    {
        using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA512);
        Byte[] buffer = new Byte[MiB];
        Int64 remaining = length;

        while(remaining > 0)
        {
            Int32 requested = (Int32)Math.Min(buffer.Length,remaining);
            Int32 read = await stream.ReadAsync(buffer.AsMemory(0,requested),cancel).ConfigureAwait(false);

            if(read == 0) { return Array.Empty<Byte>(); }

            hash.AppendData(buffer,0,read);
            remaining -= read;
        }

        return hash.GetHashAndReset();
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="IntegrityMatches"]/*'/>*/
    private static Boolean IntegrityMatches(TransferContentIntegrity? left , TransferContentIntegrity? right)
    {
        if(left is null || right is null) { return left is null && right is null; }

        return String.Equals(left.Algorithm,right.Algorithm,StringComparison.Ordinal)
            && left.Length == right.Length
            && left.SegmentCount == right.SegmentCount
            && left.Digest.AsSpan().SequenceEqual(right.Digest);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="WaitForFollowWakeAsync"]/*'/>*/
    private async Task WaitForFollowWakeAsync(TimeSpan wait , CancellationToken cancel)
    {
        if(wait == TimeSpan.Zero) { return; }

        using CancellationTokenSource shutdown = CancellationTokenSource.CreateLinkedTokenSource(cancel,this.ShutdownCancellation.Token);

        if(this.FollowSubscription is not null)
        {
            TimeSpan boundedWait = wait == Timeout.InfiniteTimeSpan || wait > this.DownloadStateRefreshInterval
                ? this.DownloadStateRefreshInterval
                : wait;

            await this.FollowSubscription.WaitForWakeAsync(boundedWait,shutdown.Token).ConfigureAwait(false);

            return;
        }

        TimeSpan delay = wait == Timeout.InfiniteTimeSpan || wait > this.DownloadStateRefreshInterval
            ? this.DownloadStateRefreshInterval
            : wait;

        if(delay > TimeSpan.Zero)
        {
            await Task.Delay(delay,shutdown.Token).ConfigureAwait(false);
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="ExportMaterializedFile"]/*'/>*/
    private async Task<String?> ExportMaterializedFile(String currentPath , String targetDirectory , String? preferredPath , Func<String,CancellationToken,Task<Boolean>> verify , CancellationToken cancel , params String?[] reservedPaths)
    {
        String targetPath = GetMaterializedTargetPath(currentPath,targetDirectory,preferredPath);

        if(AreSamePath(currentPath,targetPath)) { return currentPath; }

        if(reservedPaths?.Any(_ => String.IsNullOrWhiteSpace(_) is false && AreSamePath(_,targetPath)) is true) { return null; }

        if(File.Exists(targetPath))
        {
            if(await verify(targetPath,cancel).ConfigureAwait(false)) { return targetPath; }

            return null;
        }

        if(File.Exists(currentPath) is false) { return null; }

        String targetDirectoryPath = Path.GetDirectoryName(targetPath)!;

        Directory.CreateDirectory(targetDirectoryPath);

        String temporaryPath = Path.Combine(targetDirectoryPath,$".{Path.GetFileName(targetPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            FileStream source = new(currentPath,new FileStreamOptions()
            {
                Access = FileAccess.Read,
                Mode = FileMode.Open,
                Share = FileShare.Read,
                Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
            });

            await using (source.ConfigureAwait(false))
            {
                FileStream destination = new(temporaryPath,new FileStreamOptions()
                {
                    Access = FileAccess.Write,
                    Mode = FileMode.CreateNew,
                    Share = FileShare.None,
                    Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
                });

                await using (destination.ConfigureAwait(false))
                {
                    await source.CopyToAsync(destination,cancel).ConfigureAwait(false);

                    await destination.FlushAsync(cancel).ConfigureAwait(false);
                }
            }

            if(await verify(temporaryPath,cancel).ConfigureAwait(false) is false) { return null; }

            cancel.ThrowIfCancellationRequested();

            try { File.Move(temporaryPath,targetPath); }

            catch ( IOException ) when(File.Exists(targetPath))
            {
                if(await verify(targetPath,cancel).ConfigureAwait(false)) { return targetPath; }

                return null;
            }

            return targetPath;
        }
        finally
        {
            if(File.Exists(temporaryPath)) { File.Delete(temporaryPath); }
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="VerifyObjectFileAsync"]/*'/>*/
    private static async Task<Boolean> VerifyObjectFileAsync(String path , Byte[] expectedSHA512 , CancellationToken cancel)
    {
        cancel.ThrowIfCancellationRequested();

        if(File.Exists(path) is false || expectedSHA512 is null || expectedSHA512.Length != 64) { return false; }

        await using FileStream stream = new(path,new FileStreamOptions()
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Share = FileShare.Read,
            Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
        });

        Byte[] hash = await SHA512.HashDataAsync(stream,cancel).ConfigureAwait(false);

        return hash.AsSpan().SequenceEqual(expectedSHA512);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="GetMaterializedTargetPath"]/*'/>*/
    private static String GetMaterializedTargetPath(String currentPath , String targetDirectory , String? preferredPath)
    {
        String? fileName = String.IsNullOrWhiteSpace(preferredPath) ? Path.GetFileName(currentPath) : Path.GetFileName(preferredPath);

        if(String.IsNullOrWhiteSpace(fileName)) { return currentPath; }

        if(fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) { throw new InvalidOperationException($"Invalid materialized file name '{fileName}'."); }

        return Path.Combine(Path.GetFullPath(targetDirectory),fileName);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="AreSamePath"]/*'/>*/
    private static Boolean AreSamePath(String left , String right)
    {
        return String.Equals(Path.GetFullPath(left),Path.GetFullPath(right),StringComparison.OrdinalIgnoreCase);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="PathsMatch"]/*'/>*/
    private static Boolean PathsMatch(String? currentPath , String targetPath)
    {
        return String.IsNullOrWhiteSpace(currentPath) is false && AreSamePath(currentPath,targetPath);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="RemoveTransfer"]/*'/>*/
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

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="RemoveTransferCore"]/*'/>*/
    private async Task<Boolean> RemoveTransferCore(CancellationToken cancel)
    {
        DataControlResponse<RemoveTransferResponse> removed = await this.client.RemoveTransfer(new RemoveTransferRequest(){ SessionID = this.Session.SessionId , ItemID = this.Session.ItemId },this.token,cancel).ConfigureAwait(false);

        if(removed.StatusCode != HttpStatusCode.OK || removed.Data?.Removed is not true) { return false; }

        if(this.Session.TransferFamily is DataControlTransferFamily.Stream)
        {
            this.FollowStream?.MarkAborted();

            this.LiveItem?.AbortLiveContent();

            await this.storage.DeleteStreamSession(this.Session.SessionId,cancel).ConfigureAwait(false);
        }
        else
        {
            await this.storage.DeleteSession(this.Session.SessionId,cancel).ConfigureAwait(false);
        }

        return true;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="CreateProgressSnapshot"]/*'/>*/
    internal static DataItemTransferProgress CreateProgressSnapshot(DataItemTransferState state)
    {
        return CreateProgress(state,null);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="CreateProgressSnapshot"]/*'/>*/
    internal static DataItemTransferProgress CreateProgressSnapshot(DataStreamTransferState state)
    {
        return CreateProgress(state,null);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="CreateSegmentedProgress"]/*'/>*/
    private static DataItemTransferProgress CreateProgress(DataItemTransferState state , DataItemTransferRange? latestRange)
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
            LatestRange = latestRange,
            StateVersion = state.StateVersion,
        };
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="CreateStreamProgress"]/*'/>*/
    private static DataItemTransferProgress CreateProgress(DataStreamTransferState state , StreamTransferSegmentFooter? footer)
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
            LatestRange = footer is null ? null : new DataItemTransferRange(){ Offset = footer.ReturnedOffset , Length = footer.ReturnedLength },
            LatestAppendedLength = footer?.AppendedLength,
            StateVersion = state.StateVersion,
        };
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="PlanNextRange"]/*'/>*/
    private DataItemTransferRange PlanNextRange(DataItemTransferState state)
    {
        DataItemTransferRange missing = state.MissingRanges[0];

        if(missing.Length <= 0) { return missing; }

        Int64 planned = Math.Min(missing.Length,this.DownloadRequestSizeBytes);

        return planned == missing.Length ? missing : new DataItemTransferRange(){ Offset = missing.Offset , Length = planned };
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="PlanNextDownloadLength"]/*'/>*/
    private Int64 PlanNextDownloadLength(Int64 availableLength)
    {
        Int64 available = Math.Max(0,availableLength);

        if(available <= 0) { return 0; }

        return Math.Min(this.DownloadRequestSizeBytes,available);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="IsFaultedOrAborted"]/*'/>*/
    private static Boolean IsFaultedOrAborted(DataItemTransferState state)
    {
        return state.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Faulted;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="IsPublishedReadTerminal"]/*'/>*/
    private static Boolean IsPublishedReadTerminal(DataItemTransferState state)
    {
        return state.Mode is DataItemTransferMode.PublishedRead &&
               state.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Published &&
               state.MissingRanges.Length == 0;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="IsCaughtUpStagedFollow"]/*'/>*/
    private static Boolean IsCaughtUpStagedFollow(DataItemTransferState state)
    {
        return state.Mode is DataItemTransferMode.StagedRead && state.MissingRanges.Length == 0;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="NeedsPublishedReadPromotion"]/*'/>*/
    private static Boolean NeedsPublishedReadPromotion(DataItemTransferState state)
    {
        return state.Mode is DataItemTransferMode.StagedRead && state.Status is DataItemTransferStatus.Published;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="CanWaitForStagedFollowData"]/*'/>*/
    private static Boolean CanWaitForStagedFollowData(DataItemTransferState state , Boolean shouldWaitForFutureData)
    {
        return state.Mode is DataItemTransferMode.StagedRead &&
               shouldWaitForFutureData &&
               IsFaultedOrAborted(state) is false &&
               state.Status is not DataItemTransferStatus.Complete;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="ShouldStopCaughtUpStagedFollow"]/*'/>*/
    private static Boolean ShouldStopCaughtUpStagedFollow(DataItemTransferState state , Boolean shouldWaitForFutureData)
    {
        return shouldWaitForFutureData is false || IsFaultedOrAborted(state);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="ShouldContinueAfterStagedFollowReopen"]/*'/>*/
    private static Boolean ShouldContinueAfterStagedFollowReopen(DataItemTransferState state)
    {
        return IsFaultedOrAborted(state) ||
               IsPublishedReadTerminal(state) ||
               NeedsPublishedReadPromotion(state) ||
               state.MissingRanges.Length > 0 ||
               state.Mode is not DataItemTransferMode.StagedRead;
    }
}