namespace KusDepot.Data.Clients;

public sealed partial class DataControlClient
{
    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="CreateSessionManager"]/*'/>*/
    private DataControlClientSessionManager CreateSessionManager()
    {
        return new DataControlClientSessionManager(this.WorkingDirectoryOptions);
    }

    ///<inheritdoc/>
    public Task<Boolean> DeleteLocalSession(Guid sessionid , CancellationToken cancel = default)
    {
        try
        {
            return this.CreateSessionManager().DeleteLocalSession(sessionid,cancel);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DeleteLocalSessionFail); if(NoExceptions) { return Task.FromResult(false); } throw; }
    }

    ///<inheritdoc/>
    public Task<DataControlTransferSessionInfo?> GetLocalSession(Guid sessionid , CancellationToken cancel = default)
    {
        try
        {
            return this.CreateSessionManager().GetLocalSession(sessionid,cancel);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetLocalSessionFail); if(NoExceptions) { return Task.FromResult<DataControlTransferSessionInfo?>(null); } throw; }
    }

    ///<inheritdoc/>
    public Task<IReadOnlyList<DataControlTransferSessionInfo>> GetLocalSessions(CancellationToken cancel = default)
    {
        try
        {
            return this.CreateSessionManager().ListLocalSessions(cancel);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetLocalSessionsFail); if(NoExceptions) { return Task.FromResult((IReadOnlyList<DataControlTransferSessionInfo>)Array.Empty<DataControlTransferSessionInfo>()); } throw; }
    }

    ///<inheritdoc/>
    public async Task<RestResponse<DataControlServerSessionInfo[]>> GetServerSessions(GetTransferSessionsRequest? request = null , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            RestRequest _ = new RestRequest("/GetTransferSessions",Method.Post).AddJsonBody(request ?? new GetTransferSessionsRequest());

            if(!String.IsNullOrEmpty(token)) { this.Auth.SetBearerToken(token); }

            return await this.Client.ExecuteAsync<DataControlServerSessionInfo[]>(_,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetServerSessionsFail); if(NoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlUploadSession?> ResumeUpload(Guid sessionid , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            DataControlTransferSessionInfo? session = await this.GetLocalSession(sessionid,cancel).ConfigureAwait(false);

            if(session is null || session.TransferFamily == DataControlTransferFamily.Stream && session.StreamState is null) { return null; }

            DataControlClientWorkingDirectoryStorage storage = new(this.WorkingDirectoryOptions);

            if(session.TransferFamily is DataControlTransferFamily.Stream)
            {
                DataStreamTransferState state = session.StreamState!;

                RestResponse<ReOpenStreamTransferResponse> reopened = await this.ReOpenStreamTransfer(new ReOpenStreamTransferRequest()
                {
                    SessionID = state.SessionID,
                    ItemID = state.ItemID,
                    ObjectSHA512 = state.ObjectSHA512,
                    StreamSHA512 = state.StreamSHA512,
                },token,cancel).ConfigureAwait(false);

                if(reopened.StatusCode != HttpStatusCode.OK || reopened.Data is null) { return null; }

                Byte[] objectPayload = await storage.ReadStreamObjectPayload(state.SessionID,cancel).ConfigureAwait(false) ?? Array.Empty<Byte>();

                await storage.SaveState(reopened.Data.State,objectPayload,cancel).ConfigureAwait(false);

                DataControlTransferSessionInfo refreshed = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                    DataControlClientWorkingDirectoryStorage.CreateManifest(reopened.Data.State,objectPayload),
                    storage.GetStreamSessionPaths(state.SessionID).SessionDirectoryPath);

                Stream? streamSource = await storage.ReadStreamRange(state.SessionID,
                    new DataItemTransferRange(){ Offset = 0 , Length = Math.Max(reopened.Data.State.AppendedLength,0) },cancel).ConfigureAwait(false);

                return new DataControlUploadSession(this,storage,token,refreshed,streamSource,publishfinalstreamhash:true);
            }

            DataItemTransferState segmentedState = session.State;

            if(segmentedState.StreamLength > 0 && session.UploadSource is null) { return null; }

            if(segmentedState.StreamLength > 0 && session.UploadSource is { Kind: DataControlTransferSourceKind.ExternalFile, Path: { Length: > 0 } externalPath })
            {
                if(File.Exists(externalPath) is false) { return null; }

                FileInfo file = new(externalPath);

                if(file.Length != segmentedState.StreamLength) { return null; }
            }

            if(segmentedState.StreamLength > 0 && session.UploadSource?.Kind is DataControlTransferSourceKind.WorkingDirectoryCopy)
            {
                String persistedPath = storage.GetSessionPaths(segmentedState.SessionID).StreamPath;

                if(File.Exists(persistedPath) is false) { return null; }

                FileInfo file = new(persistedPath);

                if(file.Length != segmentedState.StreamLength) { return null; }
            }

            RestResponse<ReOpenUploadTransferResponse> segmented = await this.ReOpenUploadTransfer(new ReOpenUploadTransferRequest()
            {
                SessionID = segmentedState.SessionID,
                ItemID = segmentedState.ItemID,
                ObjectSHA512 = segmentedState.ObjectSHA512,
                StreamSHA512 = segmentedState.StreamSHA512,
                StreamLength = segmentedState.StreamLength,
            },token,cancel).ConfigureAwait(false);

            if(segmented.StatusCode != HttpStatusCode.OK || segmented.Data is null) { return null; }

            await storage.SaveState(segmented.Data.State,cancel).ConfigureAwait(false);

            if(session.UploadSource is not null && await storage.SaveUploadSourceDescriptor(segmented.Data.State.SessionID,session.UploadSource,cancel:cancel).ConfigureAwait(false) is false) { return null; }

            DataControlTransferSessionInfo refreshedSegmented = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                DataControlClientWorkingDirectoryStorage.CreateManifest(segmented.Data.State),
                storage.GetSessionPaths(segmentedState.SessionID).SessionDirectoryPath,
                session.UploadSource);

            return new DataControlUploadSession(this,storage,token,refreshedSegmented);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ResumeUploadFail); if(NoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public async Task<DataControlDownloadSession?> ResumeDownload(Guid sessionid , String? token = null , DataControlDownloadOptions? options = null , CancellationToken cancel = default)
    {
        try
        {
            options ??= new();

            DataControlTransferSessionInfo? session = await this.GetLocalSession(sessionid,cancel).ConfigureAwait(false);

            if(session is null) { return null; }

            DataControlClientWorkingDirectoryStorage storage = new(this.WorkingDirectoryOptions);

            if(session.TransferFamily is DataControlTransferFamily.Stream)
            {
                return await this.ResumeStreamFollowDownload(session,storage,options,token,cancel).ConfigureAwait(false);
            }

            return session.State.Mode switch
            {
                DataItemTransferMode.ReadCommitted => await this.ResumeCommittedDownload(session,storage,options,token,cancel).ConfigureAwait(false),

                DataItemTransferMode.ReadStaged => await this.ResumeSegmentedFollowDownload(session,storage,options,token,cancel).ConfigureAwait(false),

                _ => null,
            };
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ResumeDownloadFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="ResumeCommittedDownload"]/*'/>*/
    private async Task<DataControlDownloadSession?> ResumeCommittedDownload(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , DataControlDownloadOptions options , String? token , CancellationToken cancel)
    {
        DataItemTransferState state = session.State;

        if(await this.ValidateSegmentedDownloadObjectPayload(state,storage,cancel).ConfigureAwait(false) is false) { return null; }

        DataControlDownloadSession? reopenedCommitted = await this.OpenDownload(state.ItemID,token,new DataControlDownloadOptions()
        {
            Mode = DataControlDownloadMode.Committed,
            CompletionMode = options.CompletionMode,
            MaxBytes = options.MaxBytes,
        },cancel).ConfigureAwait(false);

        if(reopenedCommitted is null || reopenedCommitted.Session.State.ItemID != state.ItemID) { return null; }

        DataItemTransferState refreshedState = reopenedCommitted.Session.State;

        Byte[] refreshedObjectPayload = await storage.ReadObjectPayload(refreshedState.SessionID,cancel).ConfigureAwait(false) ?? Array.Empty<Byte>();

        if(refreshedState.StreamSHA512.SequenceEqual(state.StreamSHA512) is false) { return null; }

        if(refreshedObjectPayload.Length == 0) { return null; }

        Byte[] refreshedObjectHash = refreshedObjectPayload.Length >= LargeObjectPayloadSize
            ? await ComputeSHA512Async(refreshedObjectPayload,MiB,cancel).ConfigureAwait(false)
            : SHA512.HashData(refreshedObjectPayload);

        if(refreshedObjectHash.SequenceEqual(state.ObjectSHA512) is false) { return null; }

        return reopenedCommitted;
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="ResumeSegmentedFollowDownload"]/*'/>*/
    private async Task<DataControlDownloadSession?> ResumeSegmentedFollowDownload(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , DataControlDownloadOptions options , String? token , CancellationToken cancel)
    {
        DataItemTransferState state = session.State;

        if(await this.ValidateSegmentedDownloadObjectPayload(state,storage,cancel).ConfigureAwait(false) is false) { return null; }

        if(state.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Committed) { return await this.CreateDownloadSessionAsync(storage,token,session,options.CompletionMode,options.MaxBytes,null,null,cancel).ConfigureAwait(false); }

        if(state.SourceSessionID.HasValue is false || state.SourceSessionID.Value == Guid.Empty)
        {
            return await this.CreateDownloadSessionAsync(storage,token,session,options.CompletionMode,options.MaxBytes,null,null,cancel).ConfigureAwait(false);
        }

        RestResponse<ReOpenGetTransferResponse> reopened = await this.ReOpenGetTransfer(new ReOpenGetTransferRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            StreamSHA512 = state.StreamSHA512,
        },token,cancel).ConfigureAwait(false);

        if(reopened.StatusCode == HttpStatusCode.OK && reopened.Data is not null)
        {
            DataItemTransferState reopenedState = reopened.Data.State;

            if(reopenedState.StreamSHA512.SequenceEqual(state.StreamSHA512) is false || reopenedState.StreamLength != state.StreamLength) { return null; }

            if(reopenedState.Mode is DataItemTransferMode.ReadCommitted or DataItemTransferMode.ReadStaged)
            {
                reopenedState = reopenedState with
                {
                    RealizedRanges = state.RealizedRanges,
                    Status = state.MissingRanges.Length == 0 ? DataItemTransferStatus.Complete : reopenedState.Status,
                };
            }

            if(await storage.SaveState(reopenedState,cancel).ConfigureAwait(false) is false) { return null; }

            DataControlTransferSessionInfo refreshed = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                DataControlClientWorkingDirectoryStorage.CreateManifest(reopenedState),
                storage.GetSessionPaths(reopenedState.SessionID).SessionDirectoryPath);

            return await this.CreateDownloadSessionAsync(storage,token,refreshed,options.CompletionMode,options.MaxBytes,null,null,cancel).ConfigureAwait(false);
        }

        DataControlDownloadSession? replacement = await this.OpenDownload(state.ItemID,token,new DataControlDownloadOptions()
        {
            Mode = DataControlDownloadMode.StagedFollow,
            SourceSessionID = state.SourceSessionID,
            CompletionMode = options.CompletionMode,
            MaxBytes = options.MaxBytes,
        },cancel).ConfigureAwait(false);

        if(replacement is not null && replacement.Session.TransferFamily is DataControlTransferFamily.Segmented)
        {
            DataControlTransferSessionInfo? migrated = await storage.ReplaceSegmentedSessionIdentity(state.SessionID,replacement.Session.State,cancel).ConfigureAwait(false);

            if(migrated is not null)
            {
                await replacement.DeleteLocalState(cancel).ConfigureAwait(false);

                return await this.CreateDownloadSessionAsync(storage,token,migrated,options.CompletionMode,options.MaxBytes,null,null,cancel).ConfigureAwait(false);
            }
        }

        return await this.CreateDownloadSessionAsync(storage,token,session,options.CompletionMode,options.MaxBytes,null,null,cancel).ConfigureAwait(false);
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="ResumeStreamFollowDownload"]/*'/>*/
    private async Task<DataControlDownloadSession?> ResumeStreamFollowDownload(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , DataControlDownloadOptions options , String? token , CancellationToken cancel)
    {
        DataStreamTransferState? state = session.StreamState;

        if(state is null || state.Mode is not DataItemTransferMode.StreamFollow) { return null; }

        Byte[]? objectPayload = await storage.ReadStreamObjectPayload(state.SessionID,cancel).ConfigureAwait(false);

        if(objectPayload is null || objectPayload.Length == 0) { return null; }

        Byte[] objectHash = objectPayload.Length >= LargeObjectPayloadSize
            ? await ComputeSHA512Async(objectPayload,MiB,cancel).ConfigureAwait(false)
            : SHA512.HashData(objectPayload);

        if(objectHash.SequenceEqual(state.ObjectSHA512) is false) { return null; }

        if(state.Status is DataItemTransferStatus.Complete or DataItemTransferStatus.Committed)
        {
            return await this.CreateResumedStreamFollowSession(session,storage,options,token,cancel).ConfigureAwait(false);
        }

        if(state.SourceSessionID.HasValue is false || state.SourceSessionID.Value == Guid.Empty)
        {
            return await this.CreateResumedStreamFollowSession(session,storage,options,token,cancel).ConfigureAwait(false);
        }

        RestResponse<ReOpenFollowStreamTransferResponse> reopened = await this.ReOpenFollowStreamTransfer(new ReOpenFollowStreamTransferRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            SourceSessionID = state.SourceSessionID.Value,
        },token,cancel).ConfigureAwait(false);

        if(reopened.StatusCode == HttpStatusCode.OK && reopened.Data is not null)
        {
            await storage.SaveState(reopened.Data.State,reopened.Data.ObjectPayload,cancel).ConfigureAwait(false);

            DataControlTransferSessionInfo refreshed = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                DataControlClientWorkingDirectoryStorage.CreateManifest(reopened.Data.State,reopened.Data.ObjectPayload),
                storage.GetStreamSessionPaths(reopened.Data.State.SessionID).SessionDirectoryPath);

            return await this.CreateResumedStreamFollowSession(refreshed,storage,options,token,cancel).ConfigureAwait(false);
        }

        DataControlDownloadSession? replacement = await this.OpenDownload(state.ItemID,token,new DataControlDownloadOptions()
        {
            Mode = DataControlDownloadMode.StreamFollow,
            SourceSessionID = state.SourceSessionID,
            CompletionMode = options.CompletionMode,
            MaxBytes = options.MaxBytes,
            BindLiveStreamItem = options.BindLiveStreamItem,
        },cancel).ConfigureAwait(false);

        if(replacement is not null && replacement.Session.TransferFamily is DataControlTransferFamily.Stream && replacement.Session.StreamState is not null)
        {
            DataControlTransferSessionInfo? migrated = await storage.ReplaceStreamSessionIdentity(state.SessionID,replacement.Session.StreamState,cancel).ConfigureAwait(false);

            if(migrated is not null)
            {
                await replacement.DeleteLocalState(cancel).ConfigureAwait(false);

                return await this.CreateResumedStreamFollowSession(migrated,storage,options,token,cancel).ConfigureAwait(false);
            }
        }

        DataControlDownloadSession? committedReplacement = await this.TryPromoteStreamFollowResumeToCommittedDownload(session,storage,options,token,cancel).ConfigureAwait(false);

        if(committedReplacement is not null) { return committedReplacement; }

        return await this.CreateResumedStreamFollowSession(session,storage,options,token,cancel).ConfigureAwait(false);
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="TryPromoteStreamFollowToCommittedDownload"]/*'/>*/
    internal async Task<DataControlDownloadSession?> TryPromoteStreamFollowToCommittedDownload(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , StreamCompletionMode completionmode , Int64? maxbytes , String? token , CancellationToken cancel)
    {
        DataStreamTransferState? state = session.StreamState;

        if(state is null) { return null; }

        String localStreamPath = storage.GetStreamSessionPaths(state.SessionID).StreamPath;

        if(File.Exists(localStreamPath) is false) { return null; }

        FileInfo localStreamFile = new(localStreamPath); Int64 localLength = localStreamFile.Length;

        if(localLength < 0 || localLength > state.AppendedLength) { return null; }

        DataControlDownloadSession? committed = await this.OpenDownload(state.ItemID,token,new DataControlDownloadOptions()
        {
            Mode = DataControlDownloadMode.Committed,
            CompletionMode = completionmode,
            MaxBytes = maxbytes,
        },cancel).ConfigureAwait(false);

        if(committed is null || committed.Session.TransferFamily is not DataControlTransferFamily.Segmented) { return null; }

        DataItemTransferState committedState = committed.Session.State;

        if(localLength > committedState.StreamLength) { await committed.DeleteLocalState(cancel).ConfigureAwait(false); return null; }

        if(localLength > 0)
        {
            Stream? localPrefix = await storage.ReadStreamRange(state.SessionID,new DataItemTransferRange(){ Offset = 0 , Length = localLength },cancel).ConfigureAwait(false);

            if(localPrefix is null)
            {
                await committed.DeleteLocalState(cancel).ConfigureAwait(false);

                return null;
            }

            await using (localPrefix.ConfigureAwait(false))
            {
                if(await storage.SaveSourceStream(committedState.SessionID,localPrefix,localLength,cancel).ConfigureAwait(false) is false)
                {
                    await committed.DeleteLocalState(cancel).ConfigureAwait(false);

                    return null;
                }
            }
        }

        DataItemTransferState promotedState = committedState with
        {
            RealizedRanges = localLength > 0 ? new[]{ new DataItemTransferRange(){ Offset = 0 , Length = localLength } } : Array.Empty<DataItemTransferRange>(),
            Status = localLength == committedState.StreamLength ? DataItemTransferStatus.Complete : committedState.Status,
        };

        DataControlTransferSessionInfo? migrated = await storage.ReplaceStreamSessionWithSegmentedIdentity(state.SessionID,promotedState,cancel).ConfigureAwait(false);

        if(migrated is null)
        {
            await committed.DeleteLocalState(cancel).ConfigureAwait(false);

            return null;
        }

        return await this.CreateDownloadSessionAsync(storage,token,migrated,completionmode,maxbytes,null,null,cancel).ConfigureAwait(false);
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="TryPromoteStreamFollowResumeToCommittedDownload"]/*'/>*/
    private Task<DataControlDownloadSession?> TryPromoteStreamFollowResumeToCommittedDownload(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , DataControlDownloadOptions options , String? token , CancellationToken cancel)
    {
        return this.TryPromoteStreamFollowToCommittedDownload(session,storage,options.CompletionMode,options.MaxBytes,token,cancel);
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="CreateResumedStreamFollowSession"]/*'/>*/
    private async Task<DataControlDownloadSession?> CreateResumedStreamFollowSession(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , DataControlDownloadOptions options , String? token , CancellationToken cancel)
    {
        DataStreamTransferState? state = session.StreamState;

        if(state is null) { return null; }

        String streamPath = storage.GetStreamSessionPaths(state.SessionID).StreamPath;

        if(File.Exists(streamPath) is false) { return null; }

        if(options.BindLiveStreamItem)
        {
            Byte[]? objectPayload = await storage.ReadStreamObjectPayload(state.SessionID,cancel).ConfigureAwait(false);

            if(objectPayload is null || objectPayload.Length == 0) { return null; }

            DataItem? liveItem = DataItem.Deserialize(objectPayload);

            if(liveItem is not DataStreamItem dataStreamItem) { return null; }

            BufferedFollowStream followStream = new(streamPath,state.AppendedLength);

            if(dataStreamItem.BindLiveContent(followStream) is false)
            {
                followStream.Dispose();

                return null;
            }

            DataControlTransferSessionInfo bound = session with { SupportsLiveFollowBinding = true };

            return await this.CreateDownloadSessionAsync(storage,token,bound,options.CompletionMode,options.MaxBytes,followStream,dataStreamItem,cancel).ConfigureAwait(false);
        }

        return await this.CreateDownloadSessionAsync(storage,token,session,options.CompletionMode,options.MaxBytes,null,null,cancel).ConfigureAwait(false);
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="ValidateSegmentedDownloadObjectPayload"]/*'/>*/
    private async Task<Boolean> ValidateSegmentedDownloadObjectPayload(DataItemTransferState state , DataControlClientWorkingDirectoryStorage storage , CancellationToken cancel)
    {
        Byte[]? objectPayload = await storage.ReadObjectPayload(state.SessionID,cancel).ConfigureAwait(false);

        if(objectPayload is null || objectPayload.Length == 0) { return false; }

        Byte[] objectHash = objectPayload.Length >= LargeObjectPayloadSize
            ? await ComputeSHA512Async(objectPayload,MiB,cancel).ConfigureAwait(false)
            : SHA512.HashData(objectPayload);

        return objectHash.SequenceEqual(state.ObjectSHA512);
    }
}
