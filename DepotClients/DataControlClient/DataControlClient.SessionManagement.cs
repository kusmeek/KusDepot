namespace KusDepot.Data.Clients;

public sealed partial class DataControlClient
{
    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="CreateSessionManager"]/*'/>*/
    private DataControlClientSessionManager CreateSessionManager()
    {
        return new DataControlClientSessionManager(this.Options.WorkingDirectoryOptions);
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
    public async Task<DataControlUploadSession?> ResumeUpload(Guid sessionid , String? token = null , CancellationToken cancel = default)
    {
        try
        {
            DataControlTransferSessionInfo? session = await this.GetLocalSession(sessionid,cancel).ConfigureAwait(false);

            if(session is null || session.TransferFamily == DataControlTransferFamily.Stream && session.StreamState is null) { return null; }

            DataControlClientWorkingDirectoryStorage storage = new(this.Options.WorkingDirectoryOptions);

            if(session.TransferFamily is DataControlTransferFamily.Stream)
            {
                DataStreamTransferState state = session.StreamState!;

                DataControlResponse<ReOpenStreamTransferResponse> reopened = await this.ReOpenStreamTransfer(new ReOpenStreamTransferRequest()
                {
                    SessionID = state.SessionID,
                    ItemID = state.ItemID,
                    ObjectSHA512 = state.ObjectSHA512,
                },token,cancel).ConfigureAwait(false);

                if(reopened.StatusCode != HttpStatusCode.OK || reopened.Data is null) { return null; }

                Byte[] objectPayload = await storage.ReadStreamObjectPayload(state.SessionID,cancel).ConfigureAwait(false) ?? Array.Empty<Byte>();

                if(await storage.SaveState(reopened.Data.State,objectPayload,cancel).ConfigureAwait(false) is false)
                {
                    KusDepotLog.Error(LocalSessionPersistenceFail,state.SessionID,state.ItemID,ResumeUploadFail);

                    return null;
                }

                DataControlTransferSessionInfo refreshed = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                    DataControlClientWorkingDirectoryStorage.CreateManifest(reopened.Data.State,objectPayload),
                    storage.GetStreamSessionPaths(state.SessionID).SessionDirectoryPath);

                return new DataControlUploadSession(this,storage,token,refreshed);
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

            DataControlResponse<ReOpenUploadTransferResponse> segmented = await this.ReOpenUploadTransfer(new ReOpenUploadTransferRequest()
            {
                SessionID = segmentedState.SessionID,
                ItemID = segmentedState.ItemID,
                ObjectSHA512 = segmentedState.ObjectSHA512,
                StreamLength = segmentedState.StreamLength,
            },token,cancel).ConfigureAwait(false);

            if(segmented.StatusCode != HttpStatusCode.OK || segmented.Data is null) { return null; }

            if(await storage.SaveState(segmented.Data.State,cancel).ConfigureAwait(false) is false)
            {
                KusDepotLog.Error(LocalSessionPersistenceFail,segmentedState.SessionID,segmentedState.ItemID,ResumeUploadFail);

                return null;
            }

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
            options ??= new(); if(options.DownloadRequestSizeBytes <= 0) { return null; }

            DataControlTransferSessionInfo? session = await this.GetLocalSession(sessionid,cancel).ConfigureAwait(false);

            if(session is null) { return null; }

            DataControlClientWorkingDirectoryStorage storage = new(this.Options.WorkingDirectoryOptions);

            if(session.TransferFamily is DataControlTransferFamily.Stream)
            {
                return await this.ResumeStreamFollowDownload(session,storage,options,token,cancel).ConfigureAwait(false);
            }

            return session.State.Mode switch
            {
                DataItemTransferMode.PublishedRead => await this.ResumePublishedReadDownload(session,storage,options,token,cancel).ConfigureAwait(false),

                DataItemTransferMode.StagedRead => await this.ResumeSegmentedFollowDownload(session,storage,options,token,cancel).ConfigureAwait(false),

                _ => null,
            };
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ResumeDownloadFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="ResumePublishedReadDownload"]/*'/>*/
    private async Task<DataControlDownloadSession?> ResumePublishedReadDownload(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , DataControlDownloadOptions options , String? token , CancellationToken cancel)
    {
        DataItemTransferState state = session.State;

        if(await this.ValidateSegmentedDownloadObjectPayload(state,storage,cancel).ConfigureAwait(false) is false) { return null; }

        DataControlResponse<ReOpenGetTransferResponse> reopened = await this.ReOpenGetTransfer(new ReOpenGetTransferRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
        },token,cancel).ConfigureAwait(false);

        if(reopened.StatusCode != HttpStatusCode.OK || reopened.Data is null) { return null; }

        DataItemTransferState refreshedState = reopened.Data.State;

        Boolean refreshedIntegrityValid = refreshedState.Integrity is not null
            && refreshedState.Integrity.Validate()
            && refreshedState.Integrity.Length == refreshedState.StreamLength;

        if(refreshedState.SessionID != state.SessionID
            || refreshedState.ItemID != state.ItemID
            || refreshedState.Mode is not DataItemTransferMode.PublishedRead
            || refreshedState.StreamLength != state.StreamLength
            || refreshedIntegrityValid is false
            || IntegrityMatches(refreshedState.Integrity,state.Integrity) is false) { return null; }

        Byte[] refreshedObjectPayload = reopened.Data.ObjectPayload ?? Array.Empty<Byte>();

        if(refreshedObjectPayload.Length == 0 || refreshedState.ObjectSHA512.AsSpan().SequenceEqual(state.ObjectSHA512) is false) { return null; }

        Byte[] refreshedObjectHash = refreshedObjectPayload.Length >= LargeObjectPayloadSize
            ? await ComputeSHA512Async(refreshedObjectPayload,MiB,cancel).ConfigureAwait(false)
            : SHA512.HashData(refreshedObjectPayload);

        if(refreshedObjectHash.SequenceEqual(state.ObjectSHA512) is false) { return null; }

        DataItemTransferState persisted = refreshedState with
        {
            RealizedRanges = state.RealizedRanges,
            Status = state.MissingRanges.Length == 0 ? DataItemTransferStatus.Complete : refreshedState.Status,
        };

        if(await storage.SaveState(persisted,cancel).ConfigureAwait(false) is false) { return null; }

        DataControlTransferSessionInfo refreshed = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
            DataControlClientWorkingDirectoryStorage.CreateManifest(persisted),
            storage.GetSessionPaths(persisted.SessionID).SessionDirectoryPath);

        return await this.CreateDownloadSessionAsync(storage,token,refreshed,downloadRequestSizeBytes:options.DownloadRequestSizeBytes,cancel:cancel).ConfigureAwait(false);
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="ResumeSegmentedFollowDownload"]/*'/>*/
    private async Task<DataControlDownloadSession?> ResumeSegmentedFollowDownload(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , DataControlDownloadOptions options , String? token , CancellationToken cancel)
    {
        DataItemTransferState state = session.State;

        if(await this.ValidateSegmentedDownloadObjectPayload(state,storage,cancel).ConfigureAwait(false) is false) { return null; }

        if(state.Mode is not DataItemTransferMode.StagedRead && state.Status is (DataItemTransferStatus.Complete or DataItemTransferStatus.Published))
        {
            return await this.CreateDownloadSessionAsync(storage,token,session,downloadRequestSizeBytes:options.DownloadRequestSizeBytes,cancel:cancel).ConfigureAwait(false);
        }

        if(state.SourceSessionID.HasValue is false || state.SourceSessionID.Value == Guid.Empty)
        {
            return await this.CreateDownloadSessionAsync(storage,token,session,downloadRequestSizeBytes:options.DownloadRequestSizeBytes,cancel:cancel).ConfigureAwait(false);
        }

        DataControlResponse<ReOpenGetTransferResponse> reopened = await this.ReOpenGetTransfer(new ReOpenGetTransferRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
        },token,cancel).ConfigureAwait(false);

        if(reopened.StatusCode == HttpStatusCode.OK && reopened.Data is not null)
        {
            DataItemTransferState reopenedState = reopened.Data.State;

            Boolean integrityValid = reopenedState.Integrity is not null
                && reopenedState.Integrity.Validate()
                && reopenedState.Integrity.Length == reopenedState.StreamLength;

            if(reopenedState.StreamLength != state.StreamLength
                || (reopenedState.Mode is DataItemTransferMode.PublishedRead
                    ? integrityValid is false
                    : IntegrityMatches(reopenedState.Integrity,state.Integrity) is false)) { return null; }

            if(reopenedState.Mode is DataItemTransferMode.PublishedRead or DataItemTransferMode.StagedRead)
            {
                reopenedState = reopenedState with
                {
                    RealizedRanges = state.RealizedRanges,
                    Status = reopenedState.Mode is DataItemTransferMode.PublishedRead && state.MissingRanges.Length == 0
                        ? DataItemTransferStatus.Complete
                        : reopenedState.Status,
                };
            }

            if(await storage.SaveState(reopenedState,cancel).ConfigureAwait(false) is false) { return null; }

            DataControlTransferSessionInfo refreshed = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                DataControlClientWorkingDirectoryStorage.CreateManifest(reopenedState),
                storage.GetSessionPaths(reopenedState.SessionID).SessionDirectoryPath);

            return await this.CreateDownloadSessionAsync(storage,token,refreshed,downloadRequestSizeBytes:options.DownloadRequestSizeBytes,cancel:cancel).ConfigureAwait(false);
        }

        DataControlDownloadSession? publishedReadReplacement = await this.TryPromoteSegmentedFollowToPublishedRead(
            session,storage,options,token,cancel).ConfigureAwait(false);

        return publishedReadReplacement;
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

        if(storage.ReconcileStreamFollowFile(state.SessionID,state.AppendedLength) is false) { return null; }

        if(state.Status is DataItemTransferStatus.Published)
        {
            return await this.TryPromoteStreamFollowResumeToPublishedRead(session,storage,options,token,cancel).ConfigureAwait(false);
        }

        if(state.SourceSessionID.HasValue is false || state.SourceSessionID.Value == Guid.Empty)
        {
            return await this.CreateResumedStreamFollowSession(session,storage,options,token,cancel).ConfigureAwait(false);
        }

        DataControlResponse<ReOpenFollowStreamTransferResponse> reopened = await this.ReOpenFollowStreamTransfer(new ReOpenFollowStreamTransferRequest()
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            SourceSessionID = state.SourceSessionID.Value,
        },token,cancel).ConfigureAwait(false);

        if(reopened.StatusCode == HttpStatusCode.OK && reopened.Data is not null)
        {
            Int64 localLength = state.AppendedLength;

            if(reopened.Data.State.SessionID != state.SessionID || reopened.Data.State.ItemID != state.ItemID
                || reopened.Data.State.SourceSessionID != state.SourceSessionID || reopened.Data.State.Mode is not DataItemTransferMode.StreamFollow
                || localLength > reopened.Data.State.AppendedLength) { return null; }

            DataStreamTransferState localState = reopened.Data.State with
            {
                AppendedLength = localLength,
                Status = localLength == reopened.Data.State.AppendedLength
                    ? reopened.Data.State.Status
                    : DataItemTransferStatus.Open,
            };

            if(await storage.SaveState(localState,reopened.Data.ObjectPayload,cancel).ConfigureAwait(false) is false)
            {
                KusDepotLog.Error(LocalSessionPersistenceFail,state.SessionID,state.ItemID,ResumeDownloadFail);

                return null;
            }

            DataControlTransferSessionInfo refreshed = DataControlClientWorkingDirectoryStorage.CreateSessionInfo(
                DataControlClientWorkingDirectoryStorage.CreateManifest(localState,reopened.Data.ObjectPayload),
                storage.GetStreamSessionPaths(localState.SessionID).SessionDirectoryPath);

            return await this.CreateResumedStreamFollowSession(refreshed,storage,options,token,cancel).ConfigureAwait(false);
        }

        DataControlDownloadSession? publishedReadReplacement = await this.TryPromoteStreamFollowResumeToPublishedRead(
            session,storage,options,token,cancel).ConfigureAwait(false);

        if(publishedReadReplacement is not null) { return publishedReadReplacement; }

        return await this.CreateResumedStreamFollowSession(session,storage,options,token,cancel).ConfigureAwait(false);
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="TryPromoteSegmentedFollowToPublishedRead"]/*'/>*/
    private async Task<DataControlDownloadSession?> TryPromoteSegmentedFollowToPublishedRead(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , DataControlDownloadOptions options , String? token , CancellationToken cancel)
    {
        DataItemTransferState state = session.State;

        DataControlDownloadSession? published = await this.OpenDownload(state.ItemID,token,new DataControlDownloadOptions()
        {
            Mode = DataControlDownloadMode.PublishedRead,
            DownloadRequestSizeBytes = options.DownloadRequestSizeBytes,
        },cancel).ConfigureAwait(false);

        if(published is null || published.Session.TransferFamily is not DataControlTransferFamily.Segmented) { return null; }

        DataItemTransferState publishedState = published.Session.State;

        if(publishedState.StreamLength != state.StreamLength || IntegrityMatches(publishedState.Integrity,state.Integrity) is false)
        {
            await published.DeleteLocalState(cancel).ConfigureAwait(false);

            return null;
        }

        if(await published.DeleteLocalState(cancel).ConfigureAwait(false) is false) { return null; }

        DataItemTransferState promotedState = publishedState with
        {
            RealizedRanges = state.RealizedRanges,
            Status = state.MissingRanges.Length == 0 ? DataItemTransferStatus.Complete : publishedState.Status,
        };

        DataControlTransferSessionInfo? migrated = await storage.ReplaceSegmentedSessionIdentity(
            state.SessionID,promotedState,cancel).ConfigureAwait(false);

        if(migrated is null) { return null; }

        return await this.CreateDownloadSessionAsync(storage,token,migrated,downloadRequestSizeBytes:options.DownloadRequestSizeBytes,cancel:cancel).ConfigureAwait(false);
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="TryPromoteStreamFollowToPublishedRead"]/*'/>*/
    internal async Task<DataControlDownloadSession?> TryPromoteStreamFollowToPublishedRead(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , DataControlDownloadOptions options , String? token , CancellationToken cancel)
    {
        DataStreamTransferState? state = session.StreamState;

        if(state is null) { return null; }

        if(storage.ReconcileStreamFollowFile(state.SessionID,state.AppendedLength) is false) { return null; }

        Int64 localLength = state.AppendedLength;

        DataControlDownloadSession? published = await this.OpenDownload(state.ItemID,token,new DataControlDownloadOptions()
        {
            Mode = DataControlDownloadMode.PublishedRead,
            DownloadRequestSizeBytes = options.DownloadRequestSizeBytes,
        },cancel).ConfigureAwait(false);

        if(published is null || published.Session.TransferFamily is not DataControlTransferFamily.Segmented) { return null; }

        DataItemTransferState publishedState = published.Session.State;

        if(localLength > publishedState.StreamLength) { await published.DeleteLocalState(cancel).ConfigureAwait(false); return null; }

        if(localLength > 0)
        {
            Stream? localPrefix = await storage.ReadStreamRange(state.SessionID,new DataItemTransferRange(){ Offset = 0 , Length = localLength },cancel).ConfigureAwait(false);

            if(localPrefix is null)
            {
                await published.DeleteLocalState(cancel).ConfigureAwait(false);

                return null;
            }

            await using (localPrefix.ConfigureAwait(false))
            {
                if(await storage.SaveSourceStream(publishedState.SessionID,localPrefix,localLength,cancel).ConfigureAwait(false) is false)
                {
                    await published.DeleteLocalState(cancel).ConfigureAwait(false);

                    return null;
                }
            }
        }

        DataItemTransferState promotedState = publishedState with
        {
            RealizedRanges = localLength > 0 ? new[]{ new DataItemTransferRange(){ Offset = 0 , Length = localLength } } : Array.Empty<DataItemTransferRange>(),
            Status = localLength == publishedState.StreamLength ? DataItemTransferStatus.Complete : publishedState.Status,
        };

        DataControlTransferSessionInfo? migrated = await storage.ReplaceStreamSessionWithSegmentedIdentity(state.SessionID,promotedState,cancel).ConfigureAwait(false);

        if(migrated is null)
        {
            await published.DeleteLocalState(cancel).ConfigureAwait(false);

            return null;
        }

        return await this.CreateDownloadSessionAsync(storage,token,migrated,downloadRequestSizeBytes:options.DownloadRequestSizeBytes,cancel:cancel).ConfigureAwait(false);
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="TryPromoteStreamFollowResumeToPublishedRead"]/*'/>*/
    private Task<DataControlDownloadSession?> TryPromoteStreamFollowResumeToPublishedRead(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , DataControlDownloadOptions options , String? token , CancellationToken cancel)
    {
        return this.TryPromoteStreamFollowToPublishedRead(session,storage,options,token,cancel);
    }

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="CreateResumedStreamFollowSession"]/*'/>*/
    private async Task<DataControlDownloadSession?> CreateResumedStreamFollowSession(DataControlTransferSessionInfo session , DataControlClientWorkingDirectoryStorage storage , DataControlDownloadOptions options , String? token , CancellationToken cancel)
    {
        DataStreamTransferState? state = session.StreamState;

        if(state is null) { return null; }

        String streamPath = storage.GetStreamSessionPaths(state.SessionID).StreamPath;

        if(storage.ReconcileStreamFollowFile(state.SessionID,state.AppendedLength) is false) { return null; }

        Int64 localLength = state.AppendedLength;

        if(options.BindLiveStreamItem)
        {
            Byte[]? objectPayload = await storage.ReadStreamObjectPayload(state.SessionID,cancel).ConfigureAwait(false);

            if(objectPayload is null || objectPayload.Length == 0) { return null; }

            DataItem? liveItem = DataItem.Deserialize(objectPayload);

            if(liveItem is not DataStreamItem dataStreamItem) { return null; }

            BufferedFollowStream followStream = new(streamPath,localLength);

            if(dataStreamItem.BindLiveContent(followStream) is false)
            {
                followStream.Dispose();

                return null;
            }

            DataControlTransferSessionInfo bound = session with { SupportsLiveFollowBinding = true };

            return await this.CreateDownloadSessionAsync(storage,token,bound,followStream,dataStreamItem,options.DownloadRequestSizeBytes,cancel).ConfigureAwait(false);
        }

        return await this.CreateDownloadSessionAsync(storage,token,session,downloadRequestSizeBytes:options.DownloadRequestSizeBytes,cancel:cancel).ConfigureAwait(false);
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

    /**<include file='DataControlClient.xml' path='DataControlClient/class[@name="DataControlClient"]/method[@name="IntegrityMatches"]/*'/>*/
    private static Boolean IntegrityMatches(TransferContentIntegrity? left , TransferContentIntegrity? right)
    {
        if(left is null || right is null) { return left is null && right is null; }

        return String.Equals(left.Algorithm,right.Algorithm,StringComparison.Ordinal)
            && left.Length == right.Length
            && left.SegmentCount == right.SegmentCount
            && left.Digest.AsSpan().SequenceEqual(right.Digest);
    }
}
