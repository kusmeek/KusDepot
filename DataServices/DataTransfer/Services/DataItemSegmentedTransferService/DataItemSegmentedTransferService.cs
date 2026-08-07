using static KusDepot.Data.Services.DataTransfer.Strings;

namespace KusDepot.Data.Services.DataTransfer;

/**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/main/*'/>*/
public sealed partial class DataItemSegmentedTransferService : IDataItemSegmentedTransferService
{
    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/field[@name="Coordinator"]/*'/>*/
    private readonly IDataItemTransferCoordinator Coordinator;

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/field[@name="PublishedSource"]/*'/>*/
    private readonly IPublishedTransferSource PublishedSource;

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/field[@name="Registry"]/*'/>*/
    private readonly IDataItemTransferRegistry Registry;

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/field[@name="Storage"]/*'/>*/
    private readonly ISegmentedTransferStorage Storage;

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/field[@name="Options"]/*'/>*/
    private readonly DataItemTransferServiceOptions Options;

    /**<include file='DataItemSegmentedTransferService.xml' path='DataItemSegmentedTransferService/class[@name="DataItemSegmentedTransferService"]/constructor[@name="DataItemSegmentedTransferService"]/*'/>*/
    public DataItemSegmentedTransferService(ISegmentedTransferStorage storage , IDataItemTransferRegistry registry , IDataItemTransferCoordinator coordinator,
        IPublishedTransferSource publishedsource , IOptions<DataItemTransferServiceOptions> options)
    {
        this.Storage = storage; this.Registry = registry; this.Coordinator = coordinator;

        this.PublishedSource = publishedsource; this.Options = options?.Value ?? new DataItemTransferServiceOptions();
    }

    ///<inheritdoc/>
    public async Task<AbortTransferResponse> AbortTransfer(AbortTransferRequest request, CancellationToken cancel = default)
    {
        try
        {
            ValidateAbortRequest(request);

            IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(request.SessionID,cancel).ConfigureAwait(false);

            await using (sessionLock.ConfigureAwait(false))
            {
                DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

                if(await this.Storage.DeleteSession(request.SessionID,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(AbortFailed,SegmentedTransferFailureCode.AbortFailed); }

                return new()
                {
                    SessionID = manifest.SessionID,
                    ItemID = manifest.ItemID,
                    Aborted = true,
                };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,AbortFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<CommitUploadTransferResponse> BeginCommitUploadTransfer(CommitUploadTransferRequest request, CancellationToken cancel = default)
    {
        try
        {
            ValidateCommitRequest(request);

            IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(request.SessionID,cancel).ConfigureAwait(false);

            await using (sessionLock.ConfigureAwait(false))
            {
                DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

                ValidateSessionWritable(manifest);

                if(manifest.StateVersion != request.ExpectedStateVersion) { throw new OperationFailedException(InvalidStateVersion,SegmentedTransferFailureCode.InvalidStateVersion); }

                if(manifest.HasFullCoverage() is false) { throw new OperationFailedException(NotFullyRealized,SegmentedTransferFailureCode.NotFullyRealized); }

                DataItemTransferManifest committing = manifest.WithStatus(DataItemTransferStatus.Committing);

                if(await this.Storage.SaveManifest(committing,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BeginCommitUploadFailed,SegmentedTransferFailureCode.CommitFailed); }

                if(committing.StreamLength > 0)
                {
                    using Stream? streamPayload = await this.Storage.ReadRange(request.SessionID, new DataItemTransferRange() { Offset = 0, Length = committing.StreamLength }, cancel).ConfigureAwait(false);

                    if(streamPayload is null) { throw new OperationFailedException(BeginCommitUploadFailed,SegmentedTransferFailureCode.CommitFailed); }

                    await ValidateHash(committing.StreamSHA512,streamPayload,StreamHashMismatch,cancel).ConfigureAwait(false);
                }

                return new()
                {
                    State = committing.ToState(),
                    Published = false,
                };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,BeginCommitUploadFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<GetTransferStateResponse> CancelCommitUploadTransfer(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        try
        {
            if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }

            IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(identity.SessionID,cancel).ConfigureAwait(false);

            await using (sessionLock.ConfigureAwait(false))
            {
                DataItemTransferManifest manifest = await LoadRequiredManifest(identity.SessionID,identity.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(manifest,identity.SessionID,identity.ItemID);

                if(manifest.Status is not DataItemTransferStatus.Committing) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }

                DataItemTransferStatus restoredStatus = manifest.HasFullCoverage() ? DataItemTransferStatus.Complete : DataItemTransferStatus.Open;

                DataItemTransferManifest restored = manifest.WithStatus(restoredStatus,null);

                if(await this.Storage.SaveManifest(restored,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(CancelCommitUploadFailed,SegmentedTransferFailureCode.CommitFailed); }

                return new() { State = restored.ToState() };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,CancelCommitUploadFailed,identity?.SessionID,identity?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<CommitUploadTransferResponse> CompleteCommitUploadTransfer(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        try
        {
            if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }

            IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(identity.SessionID,cancel).ConfigureAwait(false);

            await using (sessionLock.ConfigureAwait(false))
            {
                DataItemTransferManifest manifest = await LoadRequiredManifest(identity.SessionID,identity.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(manifest,identity.SessionID,identity.ItemID);

                if(manifest.Status is not DataItemTransferStatus.Committing) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }

                DataItemTransferManifest committed = manifest.WithStatus(DataItemTransferStatus.Committed,null);

                if(await this.Storage.SaveManifest(committed,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(CompleteCommitUploadFailed,SegmentedTransferFailureCode.CommitFailed); }

                return new()
                {
                    State = committed.ToState(),
                    Published = false,
                };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,CompleteCommitUploadFailed,identity?.SessionID,identity?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<SegmentedTransferReadResult> GetTransferSegment(GetTransferSegmentRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateGetTransferSegmentRequest(request);

            DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

            ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

            ValidateSessionReadable(manifest);

            DataItemTransferRange range = new() { Offset = request.Offset , Length = request.Length };

            Stream? payload;

            if(manifest.Mode is DataItemTransferMode.ReadCommitted)
            {
                PublishedTransferSnapshot snapshot = await LoadRequiredPublishedSnapshot(request.ItemID,cancel).ConfigureAwait(false);

                if(manifest.CanReadRange(range) is false)
                {
                    Stream? committedPayload = await this.PublishedSource.OpenReadRange(snapshot,range,cancel).ConfigureAwait(false);

                    if(committedPayload is null) { throw new OperationFailedException(RangeUnavailable,SegmentedTransferFailureCode.RangeUnavailable); }

                    await using (committedPayload.ConfigureAwait(false))
                    {
                        SegmentedTransferWriteResult writeResult = await this.Storage.WriteRange(request.SessionID,range,committedPayload,cancel).ConfigureAwait(false);

                        if(writeResult.Accepted is false) { throw new OperationFailedException(TransferReadFailed,SegmentedTransferFailureCode.TransferReadFailed); }
                    }

                    DataItemTransferManifest refreshed = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

                    DataItemTransferManifest updated = refreshed.WithAddedRange(range);

                    if(await this.Storage.SaveManifest(updated,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(TransferReadFailed,SegmentedTransferFailureCode.TransferReadFailed); }

                    manifest = updated;
                }

                payload = await this.Storage.ReadRange(request.SessionID,range,cancel).ConfigureAwait(false);
            }
            else if(manifest.Mode is DataItemTransferMode.ReadStaged)
            {
                if(!manifest.SourceSessionID.HasValue) { throw new OperationFailedException(RangeUnavailable,SegmentedTransferFailureCode.RangeUnavailable); }

                DataItemTransferManifest sourceManifest = await LoadRequiredManifest(manifest.SourceSessionID.Value,request.ItemID,cancel).ConfigureAwait(false);

                if(sourceManifest.CanReadRange(range) is false) { throw new OperationFailedException(RangeUnavailable,SegmentedTransferFailureCode.RangeUnavailable); }

                payload = await this.Storage.ReadRange(sourceManifest.SessionID,range,cancel).ConfigureAwait(false);
            }
            else
            {
                if(manifest.CanReadRange(range) is false) { throw new OperationFailedException(RangeUnavailable,SegmentedTransferFailureCode.RangeUnavailable); }

                payload = await this.Storage.ReadRange(request.SessionID,range,cancel).ConfigureAwait(false);
            }

            if(payload is null) { throw new OperationFailedException(TransferReadFailed,SegmentedTransferFailureCode.TransferReadFailed); }

            Stream hashedPayload = new HashingReadStream(payload);

            return new()
            {
                Footer = new()
                {
                    SessionID = request.SessionID,
                    ItemID = request.ItemID,
                    RequestedOffset = request.Offset,
                    RequestedLength = request.Length,
                    ReturnedOffset = request.Offset,
                    ReturnedLength = request.Length,
                    StateVersion = manifest.StateVersion,
                },
                Payload = hashedPayload,
            };
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,TransferReadFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<GetTransferStateResponse> GetTransferState(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        DataItemTransferManifest manifest = await LoadRequiredManifest(identity.SessionID,identity.ItemID,cancel).ConfigureAwait(false);

        return new() { State = manifest.ToState() };
    }

    ///<inheritdoc/>
    public Task<DataControlServerSessionInfo[]> GetSessions(GetTransferSessionsRequest? request = null , CancellationToken cancel = default)
    {
        if(request?.TransferFamily.HasValue is true && request.TransferFamily.Value is not DataControlTransferFamily.Segmented)
        {
            return Task.FromResult(Array.Empty<DataControlServerSessionInfo>());
        }

        List<DataControlServerSessionInfo> sessions = new();

        foreach(DataItemTransferManifest manifest in EnumerateStoredManifests(request,cancel))
        {
            sessions.Add(ToServerSessionInfo(manifest));
        }

        return Task.FromResult(sessions.ToArray());
    }

    ///<inheritdoc/>
    public async Task<OpenGetTransferResponse> OpenGetTransfer(OpenGetTransferRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateOpenGetRequest(request);

            IAsyncDisposable itemLock = await this.Coordinator.AcquireItemLockAsync(request.ItemID,cancel).ConfigureAwait(false);

            await using (itemLock.ConfigureAwait(false))
            {
                if(request.SourceSessionID.HasValue)
                {
                    DataItemTransferManifest sourceManifest = await LoadRequiredManifest(request.SourceSessionID.Value,request.ItemID,cancel).ConfigureAwait(false);

                    ValidateManifestIdentity(sourceManifest,request.SourceSessionID.Value,request.ItemID);

                    ValidateSessionReadable(sourceManifest);

                    DataItemTransferManifest stagedReadManifest = CreateStagedReadManifest(request,sourceManifest);

                    Byte[]? objectPayload = await this.Storage.ReadObjectPayload(sourceManifest.SessionID,cancel).ConfigureAwait(false);

                    if(objectPayload is null) { throw new OperationFailedException(LoadSessionFailed,SegmentedTransferFailureCode.LoadSessionFailed); }

                    SegmentedTransferStorageSessionPaths? stagedCreated = await this.Storage.CreateSession(stagedReadManifest,objectPayload,cancel).ConfigureAwait(false);

                    if(stagedCreated is null) { throw new OperationFailedException(OpenFailed,SegmentedTransferFailureCode.OpenFailed); }

                    return new() { State = stagedReadManifest.ToState() , ObjectPayload = objectPayload };
                }

                PublishedTransferSnapshot snapshot = await LoadRequiredPublishedSnapshot(request.ItemID,cancel).ConfigureAwait(false);

                DataItemTransferManifest manifest = CreateCommittedReadManifest(request,snapshot);

                SegmentedTransferStorageSessionPaths? created = await this.Storage.CreateSession(manifest,snapshot.ObjectPayload,cancel).ConfigureAwait(false);

                if(created is null) { throw new OperationFailedException(OpenFailed,SegmentedTransferFailureCode.OpenFailed); }

                return new() { State = manifest.ToState() , ObjectPayload = snapshot.ObjectPayload };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,OpenFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<OpenUploadTransferResponse> OpenUploadTransfer(OpenUploadTransferRequest request, CancellationToken cancel = default)
    {
        try
        {
            await ValidateOpenRequest(request,cancel).ConfigureAwait(false);

            IAsyncDisposable itemLock = await this.Coordinator.AcquireItemLockAsync(request.ItemID,cancel).ConfigureAwait(false);

            await using (itemLock.ConfigureAwait(false))
            {
                DataItemTransferOpenConflict? existing = await this.Registry.FindOpenConflictByItemId(request.ItemID,cancel).ConfigureAwait(false);

                if(existing is not null) { throw new OperationFailedException(ConflictOpenUploadSession,SegmentedTransferFailureCode.ConflictOpenSession); }

                DataItemTransferManifest manifest = CreateManifest(request);

                SegmentedTransferStorageSessionPaths? created = await this.Storage.CreateSession(manifest,request.ObjectPayload,cancel).ConfigureAwait(false);

                if(created is null) { throw new OperationFailedException(OpenFailed,SegmentedTransferFailureCode.OpenFailed); }

                return new() { State = manifest.ToState() };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,OpenFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<PutTransferSegmentResponse> PutTransferSegment(PutTransferSegmentRequest request , Stream payload , CancellationToken cancel = default)
    {
        try
        {
            ValidatePutTransferSegmentRequest(request,payload);

            DataItemTransferManifest preflightManifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

            ValidateManifestIdentity(preflightManifest,request.SessionID,request.ItemID);

            ValidateSessionWritable(preflightManifest);

            ValidateSegmentLengthAgainstPolicy(preflightManifest,request.Length,request.Offset);

            SeekablePayloadBuffer stagedPayload = await SeekablePayloadBuffer.Create(payload,request.Length,this.Options.TemporaryUploadPath,cancel).ConfigureAwait(false);

            await using (stagedPayload.ConfigureAwait(false))
            {
                IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(request.SessionID,cancel).ConfigureAwait(false);

                await using (sessionLock.ConfigureAwait(false))
                {
                    DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

                    ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

                    ValidateSessionWritable(manifest);

                    ValidateSegmentLengthAgainstPolicy(manifest,request.Length,request.Offset);

                    if(request.ExpectedStateVersion.HasValue && manifest.StateVersion != request.ExpectedStateVersion.Value) { throw new OperationFailedException(InvalidStateVersion,SegmentedTransferFailureCode.InvalidStateVersion); }

                    ValidateHash(request.StreamSHA512,manifest.StreamSHA512,StreamHashMismatch);

                    await ValidateSegmentHash(request.SegmentSHA512,stagedPayload.Stream,request.Length,cancel).ConfigureAwait(false);

                    DataItemTransferRange range = new() { Offset = request.Offset , Length = request.Length };

                    if(stagedPayload.Stream.CanSeek) { stagedPayload.Stream.Position = 0; }

                    SegmentedTransferWriteResult writeResult = await this.Storage.WriteRange(request.SessionID,range,stagedPayload.Stream,cancel).ConfigureAwait(false);

                    if(writeResult.Accepted is false)
                    {
                        return new()
                        {
                            SessionID = request.SessionID,
                            ItemID = request.ItemID,
                            StateVersion = manifest.StateVersion,
                            Range = range,
                            Accepted = false,
                            CompletedCoverage = manifest.HasFullCoverage(),
                        };
                    }

                    DataItemTransferManifest updated = manifest.WithAddedRange(range);

                    if(await this.Storage.SaveManifest(updated,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(SegmentUploadFailed,SegmentedTransferFailureCode.SegmentUploadFailed); }

                    return new()
                    {
                        SessionID = request.SessionID,
                        ItemID = request.ItemID,
                        StateVersion = updated.StateVersion,
                        Range = writeResult.Range,
                        Accepted = true,
                        CompletedCoverage = updated.HasFullCoverage(),
                    };
                }
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,SegmentUploadFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<RemoveTransferResponse> RemoveTransfer(RemoveTransferRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateRemoveRequest(request);

            IAsyncDisposable sessionLock = await this.Coordinator.AcquireSessionLockAsync(request.SessionID,cancel).ConfigureAwait(false);

            await using (sessionLock.ConfigureAwait(false))
            {
                DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

                ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

                if(manifest.Status is not DataItemTransferStatus.Complete and not DataItemTransferStatus.Committed)
                {
                    throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState);
                }

                if(await this.Storage.DeleteSession(request.SessionID,cancel).ConfigureAwait(false) is false)
                {
                    throw new OperationFailedException(RemoveFailed,SegmentedTransferFailureCode.RemoveFailed);
                }

                return new()
                {
                    SessionID = manifest.SessionID,
                    ItemID = manifest.ItemID,
                    Removed = true,
                };
            }
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,RemoveFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<Boolean> SessionExists(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { return false; }

        DataItemTransferManifest? manifest = await this.Storage.LoadManifest(identity.SessionID,cancel).ConfigureAwait(false);

        return manifest?.ItemID == identity.ItemID;
    }

    ///<inheritdoc/>
    public async Task<ReOpenGetTransferResponse> ReOpenGetTransfer(ReOpenGetTransferRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateReOpenGetRequest(request);

            DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

            ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

            if(manifest.Mode is not DataItemTransferMode.ReadCommitted and not DataItemTransferMode.ReadStaged) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }

            ValidateHash(request.StreamSHA512,manifest.StreamSHA512,StreamHashMismatch);

            ValidateSessionReadable(manifest);

            if(manifest.Mode is DataItemTransferMode.ReadStaged && manifest.SourceSessionID.HasValue)
            {
                PublishedTransferSnapshot? published = await this.PublishedSource.TryLoadSnapshot(request.ItemID,cancel).ConfigureAwait(false);

                if(published is not null)
                {
                    await ValidatePublishedSnapshot(published,request.ItemID,cancel).ConfigureAwait(false);

                    manifest = CreateCommittedReadManifest(new OpenGetTransferRequest()
                    {
                        SessionID = manifest.SessionID,
                        ItemID = manifest.ItemID,
                    },published) with
                    {
                        RealizedRanges = manifest.RealizedRanges,
                    };
                }
                else
                {
                    DataItemTransferManifest sourceManifest = await LoadRequiredManifest(manifest.SourceSessionID.Value,request.ItemID,cancel).ConfigureAwait(false);

                    manifest = manifest with
                    {
                        FaultMessage = sourceManifest.FaultMessage,
                        ObjectInfo = sourceManifest.ObjectInfo,
                        Status = sourceManifest.Status,
                        Updated = DateTimeOffset.UtcNow,
                    };
                }

                if(await this.Storage.SaveManifest(manifest,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(ReOpenFailed,SegmentedTransferFailureCode.ReOpenFailed); }
            }

            Byte[]? objectPayload = await this.Storage.ReadObjectPayload(request.SessionID,cancel).ConfigureAwait(false);

            if(objectPayload is null) { throw new OperationFailedException(LoadSessionFailed,SegmentedTransferFailureCode.LoadSessionFailed); }

            return new() { State = manifest.ToState() , ObjectPayload = objectPayload };
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,ReOpenFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }

    ///<inheritdoc/>
    public async Task<ReOpenUploadTransferResponse> ReOpenUploadTransfer(ReOpenUploadTransferRequest request , CancellationToken cancel = default)
    {
        try
        {
            ValidateReOpenRequest(request);

            DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel).ConfigureAwait(false);

            ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

            ValidateManifestHashes(manifest,request.ObjectSHA512,request.StreamSHA512,request.StreamLength);

            ValidateSessionWritable(manifest);

            return new() { State = manifest.ToState() };
        }
        catch ( Exception _ ) when(_ is not OperationCanceledException)
        {
            Log.Error(_,ReOpenFailed,request?.SessionID,request?.ItemID);

            throw;
        }
    }
}