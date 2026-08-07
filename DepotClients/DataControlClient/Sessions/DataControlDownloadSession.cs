namespace KusDepot.Data.Clients;

/**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/main/*'/>*/
public sealed class DataControlDownloadSession : IDisposable , IAsyncDisposable
{
    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="client"]/*'/>*/
    private readonly DataControlClient client;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="storage"]/*'/>*/
    private readonly DataControlClientWorkingDirectoryStorage storage;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="token"]/*'/>*/
    private readonly String? token;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="CompletionMode"]/*'/>*/
    private readonly StreamCompletionMode CompletionMode;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="MaxBytes"]/*'/>*/
    private readonly Int64? MaxBytes;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="FollowStream"]/*'/>*/
    private readonly BufferedFollowStream? FollowStream;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="FollowSubscription"]/*'/>*/
    private readonly IDataControlNotificationSubscriptionHandle? FollowSubscription;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="disposed"]/*'/>*/
    private Int32 disposed;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/field[@name="materializationVerified"]/*'/>*/
    private Boolean materializationVerified;

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/constructor[@name="Constructor"]/*'/>*/
    internal DataControlDownloadSession(DataControlClient client , DataControlClientWorkingDirectoryStorage storage , String? token,
        DataControlTransferSessionInfo session , StreamCompletionMode completionmode = StreamCompletionMode.UntilSourceCompletes,
        Int64? maxbytes = null , BufferedFollowStream? followstream = null , DataStreamItem? liveitem = null ,
        IDataControlNotificationSubscriptionHandle? followsubscription = null)
    {
        this.client = client;
        this.storage = storage;
        this.token = token;
        this.CompletionMode = completionmode;
        this.MaxBytes = maxbytes;
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

        this.FollowStream?.Dispose();

        if(this.FollowSubscription is IAsyncDisposable asyncDisposable)
        {
            asyncDisposable.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        GC.SuppressFinalize(this);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DisposeAsync"]/*'/>*/
    public async ValueTask DisposeAsync()
    {
        if(Interlocked.Exchange(ref this.disposed,1) != 0) { return; }

        this.FollowStream?.Dispose();

        if(this.FollowSubscription is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync().ConfigureAwait(false);
        }

        GC.SuppressFinalize(this);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="Abort"]/*'/>*/
    public async Task<Boolean> Abort(CancellationToken cancel = default)
    {
        RestResponse<AbortTransferResponse> aborted = await this.client.AbortTransfer(new AbortTransferRequest(){ SessionID = this.Session.SessionId , ItemID = this.Session.ItemId },this.token,cancel).ConfigureAwait(false);

        if(aborted.StatusCode != HttpStatusCode.OK || aborted.Data?.Aborted is not true) { return false; }

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

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DeleteLocalState"]/*'/>*/
    public Task<Boolean> DeleteLocalState(CancellationToken cancel = default)
    {
        return this.Session.TransferFamily is DataControlTransferFamily.Stream
            ? this.storage.DeleteStreamSession(this.Session.SessionId,cancel)
            : this.storage.DeleteSession(this.Session.SessionId,cancel);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadNext"]/*'/>*/
    public async Task<Boolean> DownloadNext(CancellationToken cancel = default)
    {
        if(this.Session.TransferFamily is DataControlTransferFamily.Stream) { return await this.DownloadStreamNext(cancel).ConfigureAwait(false); }

        return await this.DownloadSegmentNext(cancel).ConfigureAwait(false) is not null;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadSegmentNext"]/*'/>*/
    private async Task<DataItemTransferRange?> DownloadSegmentNext(CancellationToken cancel)
    {
        if(this.Session.TransferFamily is DataControlTransferFamily.Stream) { return null; }

        DataItemTransferState state = this.Session.State;

        if((state.Mode != DataItemTransferMode.ReadCommitted && state.Mode != DataItemTransferMode.ReadStaged) || state.MissingRanges.Length == 0) { return null; }

        if(this.client is null) { return null; }

        DataItemTransferRange range = PlanNextRange(state);

        using MemoryStream payload = new();

        SegmentDownloadInfo? segment = await this.client.DownloadTransferSegmentToStream(new GetTransferSegmentRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            Offset = range.Offset,
            Length = range.Length,
        },payload,this.token,cancel).ConfigureAwait(false);

        if(segment is null || segment.StatusCode != HttpStatusCode.OK || segment.Footer.ReturnedLength <= 0) { return null; }

        payload.Position = 0;

        SegmentedTransferWriteResult written = await this.storage.WriteRange(state.SessionID,new DataItemTransferRange(){ Offset = segment.Footer.ReturnedOffset , Length = segment.Footer.ReturnedLength },payload,cancel).ConfigureAwait(false);

        if(written.Accepted is false) { return null; }

        RestResponse<GetTransferStateResponse> refreshed = await this.client.GetTransferState(new DataItemTransferIdentity(){ SessionID = state.SessionID , ItemID = state.ItemID },this.token,cancel).ConfigureAwait(false);

        if(refreshed.StatusCode != HttpStatusCode.OK || refreshed.Data is null) { return null; }

        DataItemTransferState merged = refreshed.Data.State with { RealizedRanges = state.RealizedRanges.Concat(new[]{ new DataItemTransferRange(){ Offset = segment.Footer.ReturnedOffset , Length = segment.Footer.ReturnedLength } }).ToArray() };
        
        DataItemTransferState persisted = DataControlClientWorkingDirectoryStorage.CreateManifest(merged).ToState();

        await this.storage.SaveState(persisted,cancel).ConfigureAwait(false);
        
        this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(DataControlClientWorkingDirectoryStorage.CreateManifest(persisted),this.storage.GetSessionPaths(state.SessionID).SessionDirectoryPath);
        
        return new DataItemTransferRange(){ Offset = segment.Footer.ReturnedOffset , Length = segment.Footer.ReturnedLength };
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadStreamNext"]/*'/>*/
    private async Task<Boolean> DownloadStreamNext(CancellationToken cancel)
    {
        DataStreamTransferState? state = this.Session.StreamState;

        if(state is null || state.Mode != DataItemTransferMode.StreamFollow) { return false; }

        if(this.client is null) { return false; }

        if(state.SourceSessionID.HasValue is false || state.SourceSessionID.Value == Guid.Empty) { return false; }

        StreamTransferStorageSessionPaths sessionPaths = this.storage.GetStreamSessionPaths(state.SessionID);

        RestResponse<ReOpenFollowStreamTransferResponse> refreshed = await this.client.ReOpenFollowStreamTransfer(new ReOpenFollowStreamTransferRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            SourceSessionID = state.SourceSessionID.Value,
        },this.token,cancel).ConfigureAwait(false);

        if(refreshed.StatusCode != HttpStatusCode.OK || refreshed.Data is null) { return false; }

        DataStreamTransferState persisted = refreshed.Data.State;

        await this.storage.SaveState(persisted,refreshed.Data.ObjectPayload,cancel).ConfigureAwait(false);

        this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
            DataControlClientWorkingDirectoryStorage.CreateManifest(persisted,refreshed.Data.ObjectPayload),sessionPaths.SessionDirectoryPath);

        Directory.CreateDirectory(sessionPaths.SessionDirectoryPath);

        Int64 localLength = File.Exists(sessionPaths.StreamPath) ? new FileInfo(sessionPaths.StreamPath).Length : 0;

        Int64 available = Math.Max(0,persisted.AppendedLength - localLength);

        if(available <= 0)
        {
            if(persisted.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Committed)
            {
                this.FollowStream?.MarkCompleted();

                this.LiveItem?.CompleteLiveContent();
            }

            return false;
        }

        Int64 plannedLength = this.PlanNextDownloadLength(persisted,localLength,available); if(plannedLength <= 0) { return false; }

        using MemoryStream payload = new();

        StreamSegmentDownloadInfo? segment = await this.client.DownloadStreamTransferSegmentToStream(new GetStreamTransferSegmentRequest()
        {
            SessionID = persisted.SessionID,
            ItemID = persisted.ItemID,
            Offset = localLength,
            Length = plannedLength,
        },payload,this.token,cancel).ConfigureAwait(false);

        if(segment is null || segment.StatusCode != HttpStatusCode.OK || segment.Footer.ReturnedLength <= 0) { return false; }

        payload.Position = 0;

        if(this.FollowStream is not null)
        {
            await this.FollowStream.AppendAsync(payload,cancel).ConfigureAwait(false);
        }
        else
        {
            FileStream stream = new(sessionPaths.StreamPath,new FileStreamOptions(){ Access = FileAccess.Write , Mode = FileMode.OpenOrCreate,
                Share = FileShare.Read , Options = FileOptions.Asynchronous | FileOptions.SequentialScan });

            await using (stream.ConfigureAwait(false))
            {
                stream.Position = segment.Footer.ReturnedOffset;

                await payload.CopyToAsync(stream,cancel).ConfigureAwait(false);

                await stream.FlushAsync(cancel).ConfigureAwait(false);
            }
        }

        Int64 realizedLength = Math.Max(localLength + segment.Footer.ReturnedLength,localLength);

        DataStreamTransferState localPersisted = persisted with
        {
            AppendedLength = realizedLength,

            Status = realizedLength >= persisted.AppendedLength ? persisted.Status : DataItemTransferStatus.Open,
        };

        await this.storage.SaveState(localPersisted,refreshed.Data.ObjectPayload,cancel).ConfigureAwait(false);

        this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
            DataControlClientWorkingDirectoryStorage.CreateManifest(localPersisted,refreshed.Data.ObjectPayload),sessionPaths.SessionDirectoryPath);

        if(localPersisted.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Committed)
        {
            this.FollowStream?.MarkCompleted(); this.LiveItem?.CompleteLiveContent();
        }

        return true;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadToCompletion"]/*'/>*/
    public async Task<Int32> DownloadToCompletion(IProgress<DataItemTransferProgress>? progress = null , CancellationToken cancel = default)
    {
        return await this.DownloadCore(progress,Timeout.InfiniteTimeSpan,cancel).ConfigureAwait(false);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadToCurrent"]/*'/>*/
    public async Task<Int32> DownloadToCurrent(IProgress<DataItemTransferProgress>? progress = null , TimeSpan wait = default , CancellationToken cancel = default)
    {
        return await this.DownloadCore(progress,wait,cancel).ConfigureAwait(false);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="DownloadCore"]/*'/>*/
    private async Task<Int32> DownloadCore(IProgress<DataItemTransferProgress>? progress , TimeSpan wait , CancellationToken cancel)
    {
        Boolean shouldWaitForFutureData = wait != TimeSpan.Zero;

        if(this.Session.TransferFamily is DataControlTransferFamily.Stream)
        {
            Int32 downloadedStreamSegments = 0;

            while(this.Session.StreamState?.Mode == DataItemTransferMode.StreamFollow)
            {
                DataStreamTransferState stateBeforeDownload = this.Session.StreamState;

                if(this.HasReachedConfiguredByteLimit(stateBeforeDownload)) { break; }

                Int64 priorLength = this.Session.StreamState.AppendedLength;

                Boolean downloadedNext = await this.DownloadStreamNext(cancel).ConfigureAwait(false);

                if(downloadedNext is false)
                {
                    if(this.Session.StreamState?.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Committed
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
                    progress?.Report(CreateProgress(refreshedState,
                        new StreamTransferSegmentFooter(){ ReturnedOffset = priorLength,
                        ReturnedLength = refreshedState.AppendedLength - priorLength,
                        AppendedLength = refreshedState.AppendedLength }));

                    if(this.CompletionMode is StreamCompletionMode.UntilByteLimit && this.HasReachedConfiguredByteLimit(refreshedState)) { break; }
                }
            }

            return downloadedStreamSegments;
        }

        Int32 downloaded = 0;

        while(this.Session.State.Mode == DataItemTransferMode.ReadCommitted || this.Session.State.Mode == DataItemTransferMode.ReadStaged)
        {
            if(IsCaughtUpStagedFollow(this.Session.State))
            {
                if(ShouldStopCaughtUpStagedFollow(this.Session.State,shouldWaitForFutureData))
                {
                    break;
                }

                await this.ReOpenSegmentedFollowSession(this.Session.State,cancel).ConfigureAwait(false);

                if(ShouldContinueAfterStagedFollowReopen(this.Session.State))
                {
                    continue;
                }

                await this.WaitForFollowWakeAsync(wait,cancel).ConfigureAwait(false);

                await this.ReOpenSegmentedFollowSession(this.Session.State,cancel).ConfigureAwait(false);

                continue;
            }

            DataItemTransferRange? range = await this.DownloadSegmentNext(cancel).ConfigureAwait(false);

            if(range is null)
            {
                if(CanWaitForStagedFollowData(this.Session.State,shouldWaitForFutureData))
                {
                    await this.ReOpenSegmentedFollowSession(this.Session.State,cancel).ConfigureAwait(false);

                    if(ShouldContinueAfterStagedFollowReopen(this.Session.State))
                    {
                        continue;
                    }

                    await this.WaitForFollowWakeAsync(wait,cancel).ConfigureAwait(false);

                    await this.ReOpenSegmentedFollowSession(this.Session.State,cancel).ConfigureAwait(false);

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
    private async Task ReOpenSegmentedFollowSession(DataItemTransferState state,CancellationToken cancel)
    {
        RestResponse<ReOpenGetTransferResponse> reopened = await this.client.ReOpenGetTransfer(new ReOpenGetTransferRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            StreamSHA512 = state.StreamSHA512,
        },this.token,cancel).ConfigureAwait(false);

        if(reopened.StatusCode != HttpStatusCode.OK || reopened.Data is null) { return; }

        DataItemTransferState refreshed = reopened.Data.State;

        if(refreshed.StreamSHA512.SequenceEqual(state.StreamSHA512) is false || refreshed.StreamLength != state.StreamLength) { return; }

        DataItemTransferState persisted = refreshed.Mode is DataItemTransferMode.ReadCommitted
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

        await this.storage.SaveState(persisted,cancel).ConfigureAwait(false);

        this.Session = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
            DataControlClientWorkingDirectoryStorage.CreateManifest(persisted),
            this.storage.GetSessionPaths(state.SessionID).SessionDirectoryPath);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="Materialize"]/*'/>*/
    public Task<DataControlDownloadCompletionResult?> Materialize(CancellationToken cancel = default) => this.Materialize(null,cancel);

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="MaterializeWithDestination"]/*'/>*/
    public async Task<DataControlDownloadCompletionResult?> Materialize(String? destinationdirectory , CancellationToken cancel = default)
    {
        if(this.Session.TransferFamily is DataControlTransferFamily.Stream) { return await this.MaterializeStream(destinationdirectory,cancel).ConfigureAwait(false); }

        DataItemTransferState state = this.Session.State;

        SegmentedTransferStorageSessionPaths paths = this.storage.GetSessionPaths(state.SessionID);

        if(state.MissingRanges.Length != 0) { return null; }

        Byte[]? objectPayload = await this.storage.ReadObjectPayload(state.SessionID,cancel).ConfigureAwait(false);

        if(objectPayload is null || objectPayload.Length == 0) { return null; }

        if(await this.VerifyMaterializationAsync(state.ObjectSHA512,state.StreamSHA512,state.StreamLength,objectPayload,paths.StreamPath,cancel).ConfigureAwait(false) is false) { return null; }

        DataItem? item = DataItem.Deserialize(objectPayload); if(item is null) { return null; }

        if(item.GetLocked()) { return null; }

        if(destinationdirectory is not null && this.EnsureExportPossible(paths.ObjectPath,destinationdirectory,item.GetObjectFILE()) is false) { return null; }

        if(destinationdirectory is not null && this.EnsureExportPossible(paths.StreamPath,destinationdirectory,item.GetFILE(),paths.ObjectPath,item.GetObjectFILE()) is false) { return null; }

        String objectPath = destinationdirectory is null
            ? await this.ResolveSessionMaterializedFilePath(paths.SessionDirectoryPath,paths.ObjectPath,item.GetObjectFILE(),cancel).ConfigureAwait(false)
            : await this.ExportMaterializedFile(paths.ObjectPath,destinationdirectory,item.GetObjectFILE(),cancel).ConfigureAwait(false);

        String streamPath = destinationdirectory is null
            ? await this.ResolveSessionMaterializedFilePath(paths.SessionDirectoryPath,paths.StreamPath,item.GetFILE(),cancel,objectPath).ConfigureAwait(false)
            : await this.ExportMaterializedFile(paths.StreamPath,destinationdirectory,item.GetFILE(),cancel,objectPath).ConfigureAwait(false);

        if(File.Exists(streamPath) is false) { return null; }

        if(PathsMatch(item.GetObjectFILE(),objectPath) is false && item.SetObjectFILE(objectPath) is false) { return null; }

        if(await this.BindMaterializedStreamPath(item,streamPath,bindstreamcontent:state.StreamLength > 0,cancel).ConfigureAwait(false) is false) { return null; }

        if(destinationdirectory is not null && await this.DeleteLocalState(cancel).ConfigureAwait(false) is false) { return null; }

        this.materializationVerified = true;

        return new()
        {
            LiveItem = this.LiveItem,
            TransferFamily = DataControlTransferFamily.Segmented,
            Item = item,
            StreamPath = streamPath,
            Session = this.Session,
        };
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="MaterializeStream"]/*'/>*/
    private async Task<DataControlDownloadCompletionResult?> MaterializeStream(String? destinationdirectory , CancellationToken cancel)
    {
        DataStreamTransferState? state = this.Session.StreamState;

        if(state is null) { return null; }

        StreamTransferStorageSessionPaths paths = this.storage.GetStreamSessionPaths(state.SessionID);

        if(state.Mode is DataItemTransferMode.StreamFollow && this.LiveItem is null && state.Status is not DataItemTransferStatus.Complete and not DataItemTransferStatus.Committed) { return null; }

        Byte[]? objectPayload = await this.storage.ReadStreamObjectPayload(state.SessionID,cancel).ConfigureAwait(false);

        if(objectPayload is null || objectPayload.Length == 0) { return null; }

        if(await this.VerifyMaterializationAsync(state.ObjectSHA512,state.StreamSHA512,state.AppendedLength,objectPayload,paths.StreamPath,cancel).ConfigureAwait(false) is false) { return null; }

        DataItem? item = this.LiveItem ?? DataItem.Deserialize(objectPayload); if(item is null) { return null; }

        if(item.GetLocked()) { return null; }

        Boolean allowStreamExport = state.Mode is not DataItemTransferMode.StreamFollow || state.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Committed;

        if(destinationdirectory is not null && allowStreamExport is false) { return null; }

        if(destinationdirectory is not null && this.EnsureExportPossible(paths.ObjectPath,destinationdirectory,item.GetObjectFILE()) is false) { return null; }

        if(destinationdirectory is not null && this.EnsureExportPossible(paths.StreamPath,destinationdirectory,item.GetFILE(),paths.ObjectPath,item.GetObjectFILE()) is false) { return null; }

        String objectPath = destinationdirectory is null
            ? await this.ResolveSessionMaterializedFilePath(paths.SessionDirectoryPath,paths.ObjectPath,item.GetObjectFILE(),cancel).ConfigureAwait(false)
            : await this.ExportMaterializedFile(paths.ObjectPath,destinationdirectory,item.GetObjectFILE(),cancel).ConfigureAwait(false);

        String streamPath = destinationdirectory is null
            ? await this.ResolveSessionMaterializedFilePath(paths.SessionDirectoryPath,paths.StreamPath,item.GetFILE(),cancel,objectPath).ConfigureAwait(false)
            : await this.ExportMaterializedFile(paths.StreamPath,destinationdirectory,item.GetFILE(),cancel,objectPath).ConfigureAwait(false);

        if(File.Exists(streamPath) is false) { return null; }

        if(PathsMatch(item.GetObjectFILE(),objectPath) is false && item.SetObjectFILE(objectPath) is false) { return null; }

        if(item is DataStreamItem live && ReferenceEquals(live,this.LiveItem))
        {
            if(state.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Committed)
            {
                this.FollowStream?.MarkCompleted();

                live.CompleteLiveContent();
            }
        }
        else if(await this.BindMaterializedStreamPath(item,streamPath,bindstreamcontent:true,cancel).ConfigureAwait(false) is false) { return null; }

        if(destinationdirectory is not null && await this.DeleteLocalState(cancel).ConfigureAwait(false) is false) { return null; }

        this.materializationVerified = true;

        return new()
        {
            LiveItem = this.LiveItem,
            TransferFamily = DataControlTransferFamily.Stream,
            Item = item,
            StreamPath = streamPath,
            Session = this.Session,
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

            default:

                return true;
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="VerifyMaterializationAsync"]/*'/>*/
    private async Task<Boolean> VerifyMaterializationAsync(Byte[] expectedObjectSHA512 , Byte[] expectedStreamSHA512 , Int64 expectedStreamLength , Byte[] objectPayload , String streamPath , CancellationToken cancel)
    {
        if(this.materializationVerified) { return true; }

        if(await VerifyObjectPayloadAsync(expectedObjectSHA512,objectPayload,cancel).ConfigureAwait(false) is false) { return false; }

        if(await VerifyStreamPayloadAsync(expectedStreamSHA512,expectedStreamLength,streamPath,cancel).ConfigureAwait(false) is false) { return false; }

        return true;
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
    private static async Task<Boolean> VerifyStreamPayloadAsync(Byte[] expectedStreamSHA512 , Int64 expectedStreamLength , String streamPath , CancellationToken cancel)
    {
        if(expectedStreamLength < 0 || File.Exists(streamPath) is false) { return false; }

        FileInfo streamInfo = new(streamPath);

        if(streamInfo.Length != expectedStreamLength) { return false; }

        if(expectedStreamLength == 0) { return expectedStreamSHA512 is not null && expectedStreamSHA512.Length == 0; }

        if(expectedStreamSHA512 is null || expectedStreamSHA512.Length == 0) { return true; }

        if(expectedStreamSHA512.Length != 64) { return false; }

        FileStream stream = new(streamPath,new FileStreamOptions()
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Share = FileShare.Read,
            Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
        });

        await using (stream.ConfigureAwait(false))
        {
            Byte[] streamHash = await SHA512.HashDataAsync(stream,cancel).ConfigureAwait(false);

            return streamHash.AsSpan().SequenceEqual(expectedStreamSHA512);
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="WaitForFollowWakeAsync"]/*'/>*/
    private async Task WaitForFollowWakeAsync(TimeSpan wait , CancellationToken cancel)
    {
        if(wait == TimeSpan.Zero) { return; }

        if(this.FollowSubscription is not null)
        {
            await this.FollowSubscription.WaitForWakeAsync(wait,cancel).ConfigureAwait(false);

            return;
        }

        if(wait > TimeSpan.Zero)
        {
            await Task.Delay(wait,cancel).ConfigureAwait(false);
        }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="EnsureExportPossible"]/*'/>*/
    private Boolean EnsureExportPossible(String currentPath , String targetDirectory , String? preferredPath , params String?[] reservedPaths)
    {
        try
        {
            String targetPath = GetMaterializedTargetPath(currentPath,targetDirectory,preferredPath);

            if(reservedPaths?.Any(_ => String.IsNullOrWhiteSpace(_) is false && AreSamePath(_,targetPath)) is true) { return false; }

            return CanExportMaterializedPath(currentPath,targetPath);
        }
        catch { return false; }
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="ResolveSessionMaterializedFilePath"]/*'/>*/
    private async Task<String> ResolveSessionMaterializedFilePath(String sessionDirectoryPath , String currentPath , String? preferredPath , CancellationToken cancel , params String?[] reservedPaths)
    {
        if(File.Exists(currentPath) is false) { return currentPath; }

        String targetPath = GetMaterializedTargetPath(currentPath,sessionDirectoryPath,preferredPath);

        if(AreSamePath(currentPath,targetPath)) { return currentPath; }

        if(reservedPaths?.Any(_ => String.IsNullOrWhiteSpace(_) is false && AreSamePath(_,targetPath)) is true) { throw new InvalidOperationException($"Materialized file target '{targetPath}' conflicts with another managed session file."); }

        Directory.CreateDirectory(sessionDirectoryPath);

        if(File.Exists(targetPath)) { throw new InvalidOperationException($"Materialized file target '{targetPath}' already exists."); }

        cancel.ThrowIfCancellationRequested();

        File.Move(currentPath,targetPath);

        return targetPath;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="ExportMaterializedFile"]/*'/>*/
    private async Task<String> ExportMaterializedFile(String currentPath , String targetDirectory , String? preferredPath , CancellationToken cancel , params String?[] reservedPaths)
    {
        if(File.Exists(currentPath) is false) { return currentPath; }

        if(this.EnsureExportPossible(currentPath,targetDirectory,preferredPath,reservedPaths) is false)
        {
            throw new InvalidOperationException("Materialized export target is not available.");
        }

        String targetPath = GetMaterializedTargetPath(currentPath,targetDirectory,preferredPath);

        if(AreSamePath(currentPath,targetPath)) { return currentPath; }

        Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);

        FileStream source = new(currentPath,new FileStreamOptions()
        {
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Share = FileShare.Read,
            Options = FileOptions.Asynchronous | FileOptions.SequentialScan,
        });

        await using (source.ConfigureAwait(false))
        {
            FileStream destination = new(targetPath,new FileStreamOptions()
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

        return targetPath;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="CanExportMaterializedPath"]/*'/>*/
    private static Boolean CanExportMaterializedPath(String currentPath , String targetPath)
    {
        if(File.Exists(currentPath) is false) { return false; }

        if(AreSamePath(currentPath,targetPath)) { return true; }

        if(File.Exists(targetPath)) { return false; }

        try
        {
            String? targetDirectory = Path.GetDirectoryName(targetPath);

            if(String.IsNullOrWhiteSpace(targetDirectory)) { return false; }

            using FileStream _ = new(currentPath,new FileStreamOptions()
            {
                Access = FileAccess.Read,
                Mode = FileMode.Open,
                Share = FileShare.None,
                Options = FileOptions.SequentialScan,
            });

            return true;
        }
        catch { return false; }
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
        RestResponse<RemoveTransferResponse> removed = await this.client.RemoveTransfer(new RemoveTransferRequest(){ SessionID = this.Session.SessionId , ItemID = this.Session.ItemId },this.token,cancel).ConfigureAwait(false);

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

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="CreateProgress"]/*'/>*/
    private static DataItemTransferProgress CreateProgress(DataItemTransferState state , DataItemTransferRange latestRange)
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
    private static DataItemTransferProgress CreateProgress(DataStreamTransferState state , StreamTransferSegmentFooter footer)
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
            LatestRange = new DataItemTransferRange(){ Offset = footer.ReturnedOffset , Length = footer.ReturnedLength },
            LatestAppendedLength = footer.AppendedLength,
            StateVersion = state.StateVersion,
        };
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="PlanNextRange"]/*'/>*/
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

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="PlanNextDownloadLength"]/*'/>*/
    private Int64 PlanNextDownloadLength(DataStreamTransferState state , Int64 localLength , Int64 availableLength)
    {
        Int64 preferred = state.SegmentSizePolicy.Validate() ? state.SegmentSizePolicy.PreferredSegmentBytes : 80*KiB;

        preferred = Math.Max(1,preferred);

        Int64 available = Math.Max(0,availableLength);

        if(this.MaxBytes.HasValue)
        {
            Int64 remaining = this.MaxBytes.Value - localLength;

            if(remaining <= 0) { return 0; }

            available = Math.Min(available,remaining);
        }

        if(available <= 0) { return 0; }

        return Math.Min(preferred,available);
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="HasReachedConfiguredByteLimit"]/*'/>*/
    private Boolean HasReachedConfiguredByteLimit(DataStreamTransferState state)
    {
        return this.MaxBytes.HasValue && state.AppendedLength >= this.MaxBytes.Value;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="IsFaultedOrAborted"]/*'/>*/
    private static Boolean IsFaultedOrAborted(DataItemTransferState state)
    {
        return state.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Faulted;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="IsCommittedSegmentedTerminal"]/*'/>*/
    private static Boolean IsCommittedSegmentedTerminal(DataItemTransferState state)
    {
        return state.Mode is DataItemTransferMode.ReadCommitted &&
               state.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Committed &&
               state.MissingRanges.Length == 0;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="IsCaughtUpStagedFollow"]/*'/>*/
    private static Boolean IsCaughtUpStagedFollow(DataItemTransferState state)
    {
        return state.Mode is DataItemTransferMode.ReadStaged && state.MissingRanges.Length == 0;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="NeedsCommittedPromotion"]/*'/>*/
    private static Boolean NeedsCommittedPromotion(DataItemTransferState state)
    {
        return state.Mode is DataItemTransferMode.ReadStaged && state.Status is DataItemTransferStatus.Committed;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="CanWaitForStagedFollowData"]/*'/>*/
    private static Boolean CanWaitForStagedFollowData(DataItemTransferState state , Boolean shouldWaitForFutureData)
    {
        return state.Mode is DataItemTransferMode.ReadStaged &&
               shouldWaitForFutureData &&
               IsFaultedOrAborted(state) is false &&
               state.Status is not DataItemTransferStatus.Complete;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="ShouldStopCaughtUpStagedFollow"]/*'/>*/
    private static Boolean ShouldStopCaughtUpStagedFollow(DataItemTransferState state , Boolean shouldWaitForFutureData)
    {
        return shouldWaitForFutureData is false || IsFaultedOrAborted(state) || state.Status is DataItemTransferStatus.Complete;
    }

    /**<include file='DataControlDownloadSession.xml' path='DataControlDownloadSession/class[@name="DataControlDownloadSession"]/method[@name="ShouldContinueAfterStagedFollowReopen"]/*'/>*/
    private static Boolean ShouldContinueAfterStagedFollowReopen(DataItemTransferState state)
    {
        return IsFaultedOrAborted(state) ||
               IsCommittedSegmentedTerminal(state) ||
               NeedsCommittedPromotion(state) ||
               state.MissingRanges.Length > 0 ||
               state.Mode is not DataItemTransferMode.ReadStaged;
    }
}