namespace DataPodServices.DataControl;

/**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/main/*'/>*/
internal sealed partial class DataControlStreamTransferService : IDataStreamTransferService
{
    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/field[@name="sessions"]/*'/>*/
    private readonly IDataControlTransferSessionStore sessions;

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/field[@name="registry"]/*'/>*/
    private readonly IDataItemTransferRegistry registry;

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/field[@name="coordinator"]/*'/>*/
    private readonly IDataItemTransferCoordinator coordinator;

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/field[@name="publications"]/*'/>*/
    private readonly IDataControlPublicationTransferStore publications;

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/field[@name="storageContext"]/*'/>*/
    private readonly ITenantTransferStorageContext storageContext;

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/field[@name="grainFactory"]/*'/>*/
    private readonly IGrainFactory grainFactory;

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/field[@name="options"]/*'/>*/
    private readonly DataItemTransferServiceOptions options;

    /**<include file='DataControlStreamTransferService.xml' path='DataControlStreamTransferService/class[@name="DataControlStreamTransferService"]/constructor[@name="DataControlStreamTransferService"]/*'/>*/
    public DataControlStreamTransferService(IDataControlTransferSessionStore sessions , IDataItemTransferRegistry registry , IDataItemTransferCoordinator coordinator , IDataControlPublicationTransferStore publications , ITenantTransferStorageContext storageContext , IGrainFactory grainFactory , IOptions<DataItemTransferServiceOptions> options)
    {
        this.sessions = sessions; this.registry = registry; this.coordinator = coordinator;

        this.publications = publications; this.storageContext = storageContext; this.grainFactory = grainFactory; this.options = options?.Value ?? new DataItemTransferServiceOptions();
    }

    ///<inheritdoc/>
    public async Task<AbortTransferResponse> AbortTransfer(AbortTransferRequest request , CancellationToken cancel = default)
    {
        ValidateAbortRequest(request);

        await using ITransferMutationScope mutationScope = await this.coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,cancel);

        DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        ValidateAbortableSession(manifest);

        if(await this.sessions.DeleteSessionAsync(request.SessionID,cancel) is false) { throw new OperationFailedException(AbortFailed,StreamTransferFailureCode.AbortFailed); }

        await this.publications.DeleteJournalAsync(request.SessionID,this.storageContext.CatalogName,cancel);

        return new() { SessionID = manifest.SessionID , ItemID = manifest.ItemID , Aborted = true };
    }

    ///<inheritdoc/>
    public async Task<PublishStreamTransferResponse> BeginPublishStreamTransfer(PublishStreamTransferRequest request , ITransferMutationScope scope , CancellationToken cancel = default)
    {
        (DataStreamTransferManifest manifest,Boolean resumingPreparation) = await ValidateBeginPublishStreamTransferCore(request,scope,cancel);

        if(manifest.Status is DataItemTransferStatus.Published) { return new() { State = manifest.ToState() , Published = false }; }

        DataStreamTransferManifest publishing = resumingPreparation ? manifest : manifest.WithStatus(DataItemTransferStatus.Publishing);

        if(resumingPreparation is false) { await this.sessions.SaveStreamManifestAsync(publishing,cancel:cancel); }

        TransferContentIntegrity? integrity = await this.sessions.RecomputeIntegrityAsync(request.SessionID,cancel);

        integrity ??= publishing.AppendedLength == 0
            ? TransferContentIntegrityCalculator.Compute(Array.Empty<TransferContentIntegritySegment>())
            : null;

        if(integrity is null || !integrity.Validate() || integrity.Length != publishing.AppendedLength) { throw new OperationFailedException(PublishFailed,StreamTransferFailureCode.PublishFailed); }

        Boolean replacingObject = request.FinalObjectPayload.Length > 0;

        Byte[] objectPayload = replacingObject ? request.FinalObjectPayload : publishing.ObjectPayload;

        Byte[] objectSHA512 = replacingObject ? SHA512.HashData(objectPayload) : publishing.ObjectSHA512;

        DataStreamTransferManifest prepared = publishing with
        {
            ObjectInfo = request.FinalObjectInfo ?? publishing.ObjectInfo,
            ObjectPayload = objectPayload,
            ObjectSHA512 = objectSHA512,
            Integrity = integrity,
        };

        TransferBlobReference? objectReference = null;

        if(replacingObject)
        {
            await using MemoryStream payload = new(request.FinalObjectPayload,false);

            objectReference = await this.sessions.ReplaceObjectAsync(prepared.SessionID,payload,cancel);

            if(objectReference is null) { throw new OperationFailedException(PublishFailed,StreamTransferFailureCode.PublishFailed); }
        }

        await this.sessions.SaveStreamManifestAsync(prepared,objectReference,cancel);

        return new() { State = prepared.ToState() , Published = false };
    }

    ///<inheritdoc/>
    public async Task ValidateBeginPublishStreamTransfer(PublishStreamTransferRequest request , ITransferMutationScope scope , CancellationToken cancel = default)
    {
        await ValidateBeginPublishStreamTransferCore(request,scope,cancel);
    }

    private async Task<(DataStreamTransferManifest Manifest,Boolean ResumingPreparation)> ValidateBeginPublishStreamTransferCore(PublishStreamTransferRequest request , ITransferMutationScope scope , CancellationToken cancel)
    {
        ValidateCompleteRequest(request); ValidateMutationScope(scope,request.SessionID,request.ItemID);

        DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

        if(manifest.Mode is not DataItemTransferMode.StreamUpload) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

        if(manifest.Status is DataItemTransferStatus.Published) { return (manifest,false); }

        Boolean resumingPreparation = manifest.Status is DataItemTransferStatus.Publishing;

        if(resumingPreparation is false)
        {
            ValidateWritableSource(manifest);

            if(manifest.StateVersion != request.ExpectedStateVersion) { throw new OperationFailedException(InvalidStateVersion,StreamTransferFailureCode.InvalidStateVersion); }
        }

        if(request.FinalStreamLength.HasValue && request.FinalStreamLength.Value != manifest.AppendedLength) { throw new OperationFailedException(InvalidArgument,StreamTransferFailureCode.InvalidArgument); }

        return (manifest,resumingPreparation);
    }

    ///<inheritdoc/>
    public async Task<PublishStreamTransferResponse> FinishPublishStreamTransfer(DataItemTransferIdentity identity , ITransferMutationScope scope , CancellationToken cancel = default)
    {
        ValidateIdentity(identity); ValidateMutationScope(scope,identity.SessionID,identity.ItemID);

        DataStreamTransferManifest manifest = await LoadRequiredManifest(identity.SessionID,identity.ItemID,cancel);

        ValidateManifestIdentity(manifest,identity.SessionID,identity.ItemID);

        if(manifest.Mode is not DataItemTransferMode.StreamUpload) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

        if(manifest.Status is DataItemTransferStatus.Published)
        {
            return new() { State = manifest.ToState() , Published = true };
        }

        if(manifest.Status is not DataItemTransferStatus.Publishing) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

        DataStreamTransferManifest published = manifest.WithStatus(DataItemTransferStatus.Published);

        await this.sessions.SaveStreamManifestAsync(published,cancel:cancel);

        return new() { State = published.ToState() , Published = true };
    }

    ///<inheritdoc/>
    public async Task<StreamTransferReadResult> GetStreamTransferSegment(GetStreamTransferSegmentRequest request , CancellationToken cancel = default)
    {
        ValidateReadRequest(request);

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(request.ItemID,cancel);

        DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        ValidateManifestIdentity(manifest,request.SessionID,request.ItemID); ValidateReadable(manifest);

        DataStreamTransferManifest readable = manifest;

        Guid sourceSessionID = manifest.SessionID;

        if(manifest.Mode is DataItemTransferMode.StreamFollow)
        {
            if(manifest.SourceSessionID.HasValue is false) { throw new OperationFailedException(SourceSessionNotFound,StreamTransferFailureCode.SourceSessionNotFound); }

            readable = await LoadRequiredManifest(manifest.SourceSessionID.Value,request.ItemID,cancel);

            ValidateReadable(readable);

            sourceSessionID = readable.SessionID;
        }
        else if(manifest.Mode is not DataItemTransferMode.StreamUpload) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

        DataItemTransferRange range = CreateRange(request.Offset,request.Length);

        if(readable.CanReadRange(range) is false) { throw new OperationFailedException(RangeUnavailable,StreamTransferFailureCode.RangeUnavailable); }

        Stream? payload = await this.sessions.OpenStreamRangeAsync(sourceSessionID,range,cancel);

        if(payload is null) { throw new OperationFailedException(TransferReadFailed,StreamTransferFailureCode.TransferReadFailed); }

        await TouchSessionActivity(manifest.SessionID,manifest.ItemID,cancel);

        return new()
        {
            Footer = new() { SessionID = request.SessionID , ItemID = request.ItemID , RequestedOffset = request.Offset , RequestedLength = request.Length , ReturnedOffset = request.Offset , ReturnedLength = request.Length , AppendedLength = readable.AppendedLength , StateVersion = readable.StateVersion },
            Payload = new HashingReadStream(payload),
        };
    }

    ///<inheritdoc/>
    public async Task<GetStreamTransferStateResponse> GetStreamTransferState(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        ValidateIdentity(identity);

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(identity.ItemID,cancel);

        DataStreamTransferManifest manifest = await LoadRequiredManifest(identity.SessionID,identity.ItemID,cancel);

        await TouchSessionActivity(manifest.SessionID,manifest.ItemID,cancel);

        return new() { State = manifest.ToState() };
    }

    ///<inheritdoc/>
    public async Task<GetTransferIntegrityEvidenceResponse> GetTransferIntegrityEvidence(GetTransferIntegrityEvidenceRequest request , CancellationToken cancel = default)
    {
        if(request is null || request.ItemID == Guid.Empty || request.SessionID == Guid.Empty || request.TransferFamily is not DataControlTransferFamily.Stream || request.PrefixLength < 0) { throw new ArgumentException(InvalidArgument); }

        await using ITransferItemLockLease itemLock = await this.coordinator.AcquireItemLockLeaseAsync(request.ItemID,cancel);

        DataStreamTransferManifest transfer = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        if(transfer.Mode is not DataItemTransferMode.StreamFollow || !transfer.SourceSessionID.HasValue) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

        Guid sourceSessionID = transfer.SourceSessionID.Value;

        await itemLock.AcquireSessionLockAsync(sourceSessionID,cancel);

        DataStreamTransferManifest source = await LoadRequiredManifest(sourceSessionID,request.ItemID,cancel);

        ValidateReadable(source);

        if(source.Mode is not DataItemTransferMode.StreamUpload || String.IsNullOrWhiteSpace(source.CatalogName) || !String.Equals(source.CatalogName,this.storageContext.CatalogName,Ordinal)) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

        if(request.PrefixLength > source.AppendedLength || request.PrefixLength > 0 && !source.CanReadRange(CreateRange(0,request.PrefixLength))) { throw new OperationFailedException(RangeUnavailable,StreamTransferFailureCode.RangeUnavailable); }

        IReadOnlyList<TransferContentIntegritySegment>? sourceSegments = await this.sessions.LoadIntegritySegmentsAsync(sourceSessionID,cancel);

        if(sourceSegments is null) { throw new OperationFailedException(TransferReadFailed,StreamTransferFailureCode.TransferReadFailed); }

        (TransferContentIntegrity Integrity,TransferContentIntegritySegment[] Segments)? evidence = await TransferIntegrityEvidenceBuilder.BuildAsync(
            sourceSegments,
            request.PrefixLength,
            (segment,length,token) => TransferIntegrityEvidenceBuilder.HashPartialSegmentAsync(segment,length,(range,readToken) => this.sessions.OpenStreamRangeAsync(sourceSessionID,range,readToken),token),cancel);

        if(evidence is null) { throw new OperationFailedException(TransferReadFailed,StreamTransferFailureCode.TransferReadFailed); }

        await TouchSessionActivity(transfer.SessionID,transfer.ItemID,cancel);

        return new() { ItemID = request.ItemID , SessionID = request.SessionID , SourceSessionID = sourceSessionID , SourceStateVersion = source.StateVersion , Integrity = evidence.Value.Integrity , Segments = evidence.Value.Segments , IsPublished = source.Status is DataItemTransferStatus.Published };
    }

    ///<inheritdoc/>
    public async Task<DataControlServerSessionInfo[]> GetSessions(GetTransferSessionsRequest? request = null , CancellationToken cancel = default)
    {
        if(request?.TransferFamily.HasValue is true && request.TransferFamily.Value is not DataControlTransferFamily.Stream) { return Array.Empty<DataControlServerSessionInfo>(); }

        TransferSessionQuery query = new() { ItemID = request?.ItemID , SessionID = request?.SessionID , Status = request?.Status , Family = DataControlTransferFamily.Stream };

        IReadOnlyList<TransferSessionState> states = await this.sessions.LoadSessionsForAdminAsync(query,cancel);

        List<DataControlServerSessionInfo> result = new();

        foreach(TransferSessionState state in states)
        {
            DataStreamTransferManifest? manifest = DataStreamTransferManifest.Parse(state.ManifestJson);

            if(manifest is not null) { result.Add(ToServerSessionInfo(manifest)); }
        }
        return result.ToArray();
    }

    ///<inheritdoc/>
    public async Task<OpenFollowStreamTransferResponse> OpenFollowStreamTransfer(OpenFollowStreamTransferRequest request , CancellationToken cancel = default)
    {
        ValidateOpenFollowRequest(request);

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(request.ItemID,cancel);

        DataStreamTransferManifest source = await LoadRequiredManifest(request.SourceSessionID,request.ItemID,cancel);

        ValidateReadable(source);

        if(String.IsNullOrWhiteSpace(source.CatalogName) || !String.Equals(source.CatalogName,this.storageContext.CatalogName,Ordinal)) { throw new OperationFailedException(CatalogBindingMismatch,StreamTransferFailureCode.InvalidSessionState); }

        DataStreamTransferManifest follower = CreateFollowerManifest(request,source);

        if(await this.sessions.CreateStreamSessionAsync(follower,cancel) is null) { throw new OperationFailedException(OpenFailed,StreamTransferFailureCode.OpenFailed); }

        return new() { ObjectPayload = follower.ObjectPayload , State = follower.ToState() };
    }

    ///<inheritdoc/>
    public async Task<OpenStreamTransferResponse> OpenStreamTransfer(OpenStreamTransferRequest request , CancellationToken cancel = default)
    {
        await ValidateOpenRequest(request,cancel);

        DataItemTransferSegmentSizePolicy policy;

        try { policy = TransferSegmentSizePolicyResolver.Resolve(request.RequestedSegmentSizePolicy,this.options.DefaultSegmentSizePolicy); }

        catch ( ArgumentException error ) { throw new OperationFailedException(InvalidSegmentPolicy,StreamTransferFailureCode.InvalidArgument,error); }

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(request.ItemID,cancel);

        if(await this.registry.FindOpenConflictByItemId(request.ItemID,this.storageContext.CatalogName,cancel) is not null) { throw new OperationFailedException(ConflictOpenSession,StreamTransferFailureCode.ConflictOpenSession); }

        CatalogItemState? itemState = await this.grainFactory.GetGrain<ICatalogDB>(this.storageContext.CatalogName).GetItemState(request.ItemID,TraceContext.FromActivityContext(DiagnosticActivity.Current?.Context),cancel);

        if(itemState?.IsPendingDelete is true) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

        DataStreamTransferManifest manifest = CreateOpenManifest(request,policy,itemState?.ItemInstanceToken ?? Guid.NewGuid());

        if(await this.sessions.CreateStreamSessionAsync(manifest,cancel) is null) { throw new OperationFailedException(OpenFailed,StreamTransferFailureCode.OpenFailed); }

        return new() { State = manifest.ToState() };
    }

    ///<inheritdoc/>
    public async Task<PutStreamTransferSegmentResponse> PutStreamTransferSegment(PutStreamTransferSegmentRequest request , Stream payload , CancellationToken cancel = default)
    {
        ValidateWriteRequest(request,payload);

        DataStreamTransferManifest preflight = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        ValidateWritableSource(preflight); ValidateSegmentLength(preflight,request.Length);

        await using ITransferMutationScope mutationScope = await this.coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,cancel);

        DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        ValidateWritableSource(manifest); ValidateSegmentLength(manifest,request.Length);

        if(request.Offset > manifest.AppendedLength) { throw new OperationFailedException(AppendOffsetMismatch,StreamTransferFailureCode.AppendOffsetMismatch); }

        if(request.Offset == manifest.AppendedLength && request.ExpectedStateVersion.HasValue && manifest.StateVersion != request.ExpectedStateVersion.Value) { throw new OperationFailedException(InvalidStateVersion,StreamTransferFailureCode.InvalidStateVersion); }

        Guid operationID = request.RequestID.GetValueOrDefault();

        if(operationID == Guid.Empty) { operationID = Guid.NewGuid(); }

        DataItemTransferRange range = CreateRange(request.Offset,request.Length);

        StreamTransferAppendResult appended = await this.sessions.WriteVerifiedStreamChunkAsync(request.SessionID,operationID,manifest,range,payload,request.Length,request.SegmentSHA512,cancel);

        return new() { SessionID = request.SessionID , ItemID = request.ItemID , Accepted = appended.Accepted , AppendedLength = appended.AppendedLength , StateVersion = appended.StateVersion , Replayed = appended.Replayed };
    }

    ///<inheritdoc/>
    public async Task<RemoveTransferResponse> RemoveTransfer(RemoveTransferRequest request , CancellationToken cancel = default)
    {
        ValidateRemoveRequest(request);

        await using ITransferMutationScope mutationScope = await this.coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,cancel);

        DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        if(manifest.Status is not DataItemTransferStatus.Complete and not DataItemTransferStatus.Published) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

        if(await this.sessions.HasDependentSessionAsync(request.SessionID,cancel)) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

        if(await this.sessions.DeleteSessionAsync(request.SessionID,cancel) is false) { throw new OperationFailedException(RemoveFailed,StreamTransferFailureCode.RemoveFailed); }

        await this.publications.DeleteJournalAsync(request.SessionID,this.storageContext.CatalogName,cancel);

        return new() { SessionID = manifest.SessionID , ItemID = manifest.ItemID , Removed = true };
    }

    ///<inheritdoc/>
    public async Task<Boolean> SessionExists(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { return false; }

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(identity.ItemID,cancel);

        DataStreamTransferManifest? manifest = await this.sessions.LoadStreamManifestAsync(identity.SessionID,cancel);

        if(manifest?.ItemID != identity.ItemID || manifest.ItemInstanceToken == Guid.Empty) { return false; }

        CatalogItemState? current = await this.grainFactory.GetGrain<ICatalogDB>(this.storageContext.CatalogName).GetItemState(identity.ItemID,TraceContext.FromActivityContext(DiagnosticActivity.Current?.Context),cancel);

        if(current is null) { return manifest.Mode is DataItemTransferMode.StreamUpload; }

        return !current.IsPendingDelete && current.ItemInstanceToken == manifest.ItemInstanceToken;
    }

    ///<inheritdoc/>
    public async Task<ReOpenFollowStreamTransferResponse> ReOpenFollowStreamTransfer(ReOpenFollowStreamTransferRequest request , CancellationToken cancel = default)
    {
        ValidateReOpenFollowRequest(request);

        await using ITransferMutationScope mutationScope = await this.coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,cancel);

        DataStreamTransferManifest follower = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        if(follower.Mode is not DataItemTransferMode.StreamFollow || follower.SourceSessionID != request.SourceSessionID) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

        DataStreamTransferManifest source = await LoadRequiredManifest(request.SourceSessionID,request.ItemID,cancel);

        ValidateReadable(source);

        if(String.IsNullOrWhiteSpace(follower.CatalogName) || String.IsNullOrWhiteSpace(source.CatalogName) || !String.Equals(follower.CatalogName,source.CatalogName,Ordinal) || !String.Equals(source.CatalogName,this.storageContext.CatalogName,Ordinal)) { throw new OperationFailedException(CatalogBindingMismatch,StreamTransferFailureCode.InvalidSessionState); }

        DataStreamTransferManifest updated = SynchronizeFollowerManifest(follower,source);

        await this.sessions.SaveStreamManifestAsync(updated,cancel:cancel);

        return new() { ObjectPayload = updated.ObjectPayload , State = updated.ToState() };
    }

    ///<inheritdoc/>
    public async Task<ReOpenStreamTransferResponse> ReOpenStreamTransfer(ReOpenStreamTransferRequest request , CancellationToken cancel = default)
    {
        ValidateReOpenRequest(request);

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(request.ItemID,cancel);

        DataStreamTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        if(manifest.Mode is not DataItemTransferMode.StreamUpload) { throw new OperationFailedException(InvalidSessionState,StreamTransferFailureCode.InvalidSessionState); }

        ValidateWritableSource(manifest);

        if(request.ObjectSHA512.Length > 0) { ValidateHash(request.ObjectSHA512,manifest.ObjectSHA512,InvalidObjectHash); }

        await TouchSessionActivity(manifest.SessionID,manifest.ItemID,cancel);

        return new() { State = manifest.ToState() };
    }
}
