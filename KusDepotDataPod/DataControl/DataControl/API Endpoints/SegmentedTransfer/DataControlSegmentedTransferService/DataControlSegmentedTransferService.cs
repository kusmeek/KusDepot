namespace DataPodServices.DataControl;

/**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/main/*'/>*/
internal sealed partial class DataControlSegmentedTransferService : IDataItemSegmentedTransferService
{
    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/field[@name="sessions"]/*'/>*/
    private readonly IDataControlTransferSessionStore sessions;

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/field[@name="registry"]/*'/>*/
    private readonly IDataItemTransferRegistry registry;

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/field[@name="coordinator"]/*'/>*/
    private readonly IDataItemTransferCoordinator coordinator;

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/field[@name="publications"]/*'/>*/
    private readonly IDataControlPublicationTransferStore publications;

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/field[@name="storageContext"]/*'/>*/
    private readonly ITenantTransferStorageContext storageContext;

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/field[@name="grainFactory"]/*'/>*/
    private readonly IGrainFactory grainFactory;

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/field[@name="publishedSource"]/*'/>*/
    private readonly IPublishedTransferSource publishedSource;

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/field[@name="options"]/*'/>*/
    private readonly DataItemTransferServiceOptions options;

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/constructor[@name="DataControlSegmentedTransferService"]/*'/>*/
    public DataControlSegmentedTransferService(IDataControlTransferSessionStore sessions , IDataItemTransferRegistry registry , IDataItemTransferCoordinator coordinator , IDataControlPublicationTransferStore publications , IPublishedTransferSource publishedSource , ITenantTransferStorageContext storageContext , IGrainFactory grainFactory , IOptions<DataItemTransferServiceOptions> options)
    {
        this.sessions = sessions; this.registry = registry; this.coordinator = coordinator;

        this.publications = publications; this.publishedSource = publishedSource; this.storageContext = storageContext; this.grainFactory = grainFactory;

        this.options = options?.Value ?? new DataItemTransferServiceOptions();
    }

    ///<inheritdoc/>
    public async Task<AbortTransferResponse> AbortTransfer(AbortTransferRequest request , CancellationToken cancel = default)
    {
        ValidateAbortRequest(request);

        await using ITransferMutationScope mutationScope = await this.coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,cancel);

        DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        ValidateAbortableSession(manifest);

        if(await this.sessions.DeleteSessionAsync(request.SessionID,cancel) is false) { throw new OperationFailedException(AbortFailed,SegmentedTransferFailureCode.AbortFailed); }

        try
        {
            await this.publications.DeleteJournalAsync(request.SessionID,this.storageContext.CatalogName,cancel);
        }
        catch ( Exception error ) when(error is not OperationCanceledException)
        {
            throw new OperationFailedException(AbortFailed,SegmentedTransferFailureCode.AbortFailed,error);
        }

        return new() { SessionID = manifest.SessionID , ItemID = manifest.ItemID , Aborted = true };
    }

    ///<inheritdoc/>
    public async Task<PublishSegmentedTransferResponse> BeginPublishSegmentedTransfer(PublishSegmentedTransferRequest request , ITransferMutationScope scope , CancellationToken cancel = default)
    {
        DataItemTransferManifest manifest = await ValidateBeginPublishSegmentedTransferCore(request,scope,cancel);

        if(manifest.Status is DataItemTransferStatus.Publishing or DataItemTransferStatus.Published) { return new() { State = manifest.ToState() , Published = false }; }

        TransferContentIntegrity? integrity = await this.sessions.RecomputeIntegrityAsync(request.SessionID,cancel);

        if(integrity is null || !integrity.Validate() || integrity.Length != manifest.StreamLength)
        {
            throw new OperationFailedException(BeginPublishUploadFailed,SegmentedTransferFailureCode.PublishFailed);
        }

        DataItemTransferManifest publishing = manifest.WithStatus(DataItemTransferStatus.Publishing) with { Integrity = integrity };

        await this.sessions.SaveSegmentedManifestAsync(publishing,cancel:cancel);

        return new() { State = publishing.ToState() , Published = false };
    }

    ///<inheritdoc/>
    public async Task ValidateBeginPublishSegmentedTransfer(PublishSegmentedTransferRequest request , ITransferMutationScope scope , CancellationToken cancel = default)
    {
        await ValidateBeginPublishSegmentedTransferCore(request,scope,cancel);
    }

    private async Task<DataItemTransferManifest> ValidateBeginPublishSegmentedTransferCore(PublishSegmentedTransferRequest request , ITransferMutationScope scope , CancellationToken cancel)
    {
        ValidatePublishRequest(request); ValidateMutationScope(scope,request.SessionID,request.ItemID);

        DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        if(manifest.Status is DataItemTransferStatus.Publishing or DataItemTransferStatus.Published) { return manifest; }

        ValidateSessionWritable(manifest);

        if(manifest.StateVersion != request.ExpectedStateVersion) { throw new OperationFailedException(InvalidStateVersion,SegmentedTransferFailureCode.InvalidStateVersion); }

        if(manifest.HasFullCoverage() is false) { throw new OperationFailedException(NotFullyRealized,SegmentedTransferFailureCode.NotFullyRealized); }

        return manifest;
    }

    ///<inheritdoc/>
    public async Task<PublishSegmentedTransferResponse> FinishPublishSegmentedTransfer(DataItemTransferIdentity identity , ITransferMutationScope scope , CancellationToken cancel = default)
    {
        if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { throw new ArgumentException(InvalidArgument); }

        ValidateMutationScope(scope,identity.SessionID,identity.ItemID);

        DataItemTransferManifest manifest = await LoadRequiredManifest(identity.SessionID,identity.ItemID,cancel);

        if(manifest.Mode is not DataItemTransferMode.Upload) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }

        if(manifest.Status is DataItemTransferStatus.Published)
        {
            return new() { State = manifest.ToState() , Published = true };
        }

        if(manifest.Status is not DataItemTransferStatus.Publishing) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }

        DataItemTransferManifest published = manifest.WithStatus(DataItemTransferStatus.Published);

        await this.sessions.SaveSegmentedManifestAsync(published,cancel:cancel);

        return new() { State = published.ToState() , Published = true };
    }

    ///<inheritdoc/>
    public async Task<SegmentedTransferReadResult> GetTransferSegment(GetTransferSegmentRequest request , CancellationToken cancel = default)
    {
        ValidateGetTransferSegmentRequest(request);

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(request.ItemID,cancel);

        DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        ValidateManifestIdentity(manifest,request.SessionID,request.ItemID);

        ValidateSessionReadable(manifest);

        DataItemTransferRange range = CreateRange(request.Offset,request.Length);

        Stream? payload;

        if(manifest.Mode is DataItemTransferMode.PublishedRead)
        {
            PublishedTransferSnapshot snapshot = await LoadRequiredPublishedSnapshot(request.ItemID,manifest.ItemInstanceToken,cancel);

            ValidatePublishedSnapshotIdentity(snapshot,manifest);

            if(manifest.CanWriteRange(range) is false) { throw new OperationFailedException(RangeUnavailable,SegmentedTransferFailureCode.RangeUnavailable); }

            payload = await this.publishedSource.OpenReadRange(snapshot,range,cancel);
        }
        else if(manifest.Mode is DataItemTransferMode.StagedRead)
        {
            if(manifest.SourceSessionID.HasValue is false) { throw new OperationFailedException(RangeUnavailable,SegmentedTransferFailureCode.RangeUnavailable); }

            DataItemTransferManifest source = await LoadRequiredManifest(manifest.SourceSessionID.Value,request.ItemID,cancel);

            if(source.CanReadRange(range) is false) { throw new OperationFailedException(RangeUnavailable,SegmentedTransferFailureCode.RangeUnavailable); }

            payload = await this.sessions.OpenSegmentedRangeAsync(source.SessionID,range,cancel);
        }
        else
        {
            if(manifest.CanReadRange(range) is false) { throw new OperationFailedException(RangeUnavailable,SegmentedTransferFailureCode.RangeUnavailable); }

            payload = await this.sessions.OpenSegmentedRangeAsync(request.SessionID,range,cancel);
        }

        if(payload is null) { throw new OperationFailedException(TransferReadFailed,SegmentedTransferFailureCode.TransferReadFailed); }

        await TouchSessionActivity(manifest.SessionID,manifest.ItemID,cancel);

        return new()
        {
            Footer = new() { SessionID = request.SessionID , ItemID = request.ItemID , RequestedOffset = request.Offset , RequestedLength = request.Length , ReturnedOffset = request.Offset , ReturnedLength = request.Length , StateVersion = manifest.StateVersion },
            Payload = new HashingReadStream(payload),
        };
    }

    ///<inheritdoc/>
    public async Task<GetTransferStateResponse> GetTransferState(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        ValidateIdentity(identity);

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(identity.ItemID,cancel);

        DataItemTransferManifest manifest = await LoadRequiredManifest(identity.SessionID,identity.ItemID,cancel);

        await TouchSessionActivity(manifest.SessionID,manifest.ItemID,cancel);

        return new() { State = manifest.ToState() };
    }

    ///<inheritdoc/>
    public async Task<GetTransferIntegrityEvidenceResponse> GetTransferIntegrityEvidence(GetTransferIntegrityEvidenceRequest request , CancellationToken cancel = default)
    {
        if(request is null || request.ItemID == Guid.Empty || request.SessionID == Guid.Empty || request.TransferFamily is not DataControlTransferFamily.Segmented || request.PrefixLength < 0) { throw new ArgumentException(InvalidArgument); }

        await using ITransferItemLockLease itemLock = await this.coordinator.AcquireItemLockLeaseAsync(request.ItemID,cancel);

        DataItemTransferManifest transfer = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        if(transfer.Mode is DataItemTransferMode.PublishedRead)
        {
            PublishedTransferSnapshot snapshot = await LoadRequiredPublishedSnapshot(request.ItemID,transfer.ItemInstanceToken,cancel);

            ValidatePublishedSnapshotIdentity(snapshot,transfer);

            if(request.PrefixLength > snapshot.StreamLength) { throw new OperationFailedException(RangeUnavailable,SegmentedTransferFailureCode.RangeUnavailable); }

            (TransferContentIntegrity Integrity,TransferContentIntegritySegment[] Segments)? built = await TransferIntegrityEvidenceBuilder.BuildAsync(
                snapshot.IntegritySegments,
                request.PrefixLength,
                (segment,length,token) => TransferIntegrityEvidenceBuilder.HashPartialSegmentAsync(segment,length,(range,readToken) => this.publishedSource.OpenReadRange(snapshot,range,readToken),token),cancel);

            if(built is null) { throw new OperationFailedException(InvalidPublishedSnapshot,SegmentedTransferFailureCode.InvalidPublishedSnapshot); }

            await TouchSessionActivity(transfer.SessionID,transfer.ItemID,cancel);

            return new() { ItemID = request.ItemID , SessionID = request.SessionID , Integrity = built.Value.Integrity , Segments = built.Value.Segments , IsPublished = true };
        }

        if(transfer.Mode is not DataItemTransferMode.StagedRead || !transfer.SourceSessionID.HasValue) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }

        Guid sourceSessionID = transfer.SourceSessionID.Value;

        await itemLock.AcquireSessionLockAsync(sourceSessionID,cancel);

        DataItemTransferManifest source = await LoadRequiredManifest(sourceSessionID,request.ItemID,cancel);

        ValidateSessionReadable(source);

        if(source.Mode is not DataItemTransferMode.Upload || String.IsNullOrWhiteSpace(source.CatalogName) || !String.Equals(source.CatalogName,this.storageContext.CatalogName,Ordinal)) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }

        if(request.PrefixLength > source.StreamLength || request.PrefixLength > 0 && !source.CanReadRange(CreateRange(0,request.PrefixLength))) { throw new OperationFailedException(RangeUnavailable,SegmentedTransferFailureCode.RangeUnavailable); }

        IReadOnlyList<TransferContentIntegritySegment>? sourceSegments = await this.sessions.LoadIntegritySegmentsAsync(sourceSessionID,cancel);

        if(sourceSegments is null) { throw new OperationFailedException(TransferReadFailed,SegmentedTransferFailureCode.TransferReadFailed); }

        (TransferContentIntegrity Integrity,TransferContentIntegritySegment[] Segments)? active = await TransferIntegrityEvidenceBuilder.BuildAsync(
            sourceSegments,
            request.PrefixLength,
            (segment,length,token) => TransferIntegrityEvidenceBuilder.HashPartialSegmentAsync(segment,length,(range,readToken) => this.sessions.OpenSegmentedRangeAsync(sourceSessionID,range,readToken),token),cancel);

        if(active is null) { throw new OperationFailedException(TransferReadFailed,SegmentedTransferFailureCode.TransferReadFailed); }

        await TouchSessionActivity(transfer.SessionID,transfer.ItemID,cancel);

        return new() { ItemID = request.ItemID , SessionID = request.SessionID , SourceSessionID = sourceSessionID , SourceStateVersion = source.StateVersion , Integrity = active.Value.Integrity , Segments = active.Value.Segments , IsPublished = source.Status is DataItemTransferStatus.Published };
    }

    /**<include file='DataControlSegmentedTransferService.xml' path='DataControlSegmentedTransferService/class[@name="DataControlSegmentedTransferService"]/method[@name="ValidateOpenRequest"]/*'/>*/
    private static async Task ValidateOpenRequest(OpenUploadTransferRequest request , CancellationToken cancel)
    {
        if(request is null || request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || request.StreamLength < 0 || request.ObjectPayload is null)
        {
            throw new ArgumentException(InvalidArgument);
        }

        if(request.RequestedSegmentSizePolicy is not null && request.RequestedSegmentSizePolicy.Validate() is false)
        {
            throw new ArgumentException(InvalidArgument);
        }

        if(request.ObjectSHA512 is null || request.ObjectSHA512.Length != 64)
        {
            throw new ArgumentException(InvalidArgument);
        }

        Byte[] objectHash = SHA512.HashData(request.ObjectPayload);

        if(objectHash.AsSpan().SequenceEqual(request.ObjectSHA512) is false)
        {
            throw new OperationFailedException(InvalidObjectHash,SegmentedTransferFailureCode.InvalidObjectHash);
        }

        await Task.CompletedTask;

        cancel.ThrowIfCancellationRequested();
    }

    ///<inheritdoc/>
    public async Task<DataControlServerSessionInfo[]> GetSessions(GetTransferSessionsRequest? request = null , CancellationToken cancel = default)
    {
        if(request?.TransferFamily.HasValue is true && request.TransferFamily.Value is not DataControlTransferFamily.Segmented) { return Array.Empty<DataControlServerSessionInfo>(); }

        IReadOnlyList<TransferSessionState> states = await this.sessions.LoadSessionsForAdminAsync(new TransferSessionQuery { ItemID = request?.ItemID , SessionID = request?.SessionID , Status = request?.Status , Family = DataControlTransferFamily.Segmented },cancel);

        List<DataControlServerSessionInfo> result = new();

        foreach(TransferSessionState state in states)
        {
            DataItemTransferManifest? manifest = DataItemTransferManifest.Deserialize(Encoding.UTF8.GetBytes(state.ManifestJson))?.NormalizeRanges();

            if(manifest is not null) { result.Add(new() { ItemID = manifest.ItemID , SessionID = manifest.SessionID , TransferFamily = DataControlTransferFamily.Segmented , SegmentedState = manifest.ToState() }); }
        }
        return result.ToArray();
    }

    ///<inheritdoc/>
    public async Task<OpenGetTransferResponse> OpenGetTransfer(OpenGetTransferRequest request , CancellationToken cancel = default)
    {
        ValidateOpenGetRequest(request);

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(request.ItemID,cancel);

        DataItemTransferManifest manifest; Byte[] objectPayload;

        if(request.SourceSessionID.HasValue)
        {
            DataItemTransferManifest source = await LoadRequiredManifest(request.SourceSessionID.Value,request.ItemID,cancel);

            ValidateSessionReadable(source);

            if(String.IsNullOrWhiteSpace(source.CatalogName) || !String.Equals(source.CatalogName,this.storageContext.CatalogName,Ordinal)) { throw new OperationFailedException(CatalogBindingMismatch,SegmentedTransferFailureCode.InvalidSessionState); }

            manifest = CreateStagedReadManifest(request,source);

            objectPayload = await this.sessions.ReadObjectPayloadAsync(source.SessionID,cancel) ?? throw new OperationFailedException(LoadSessionFailed,SegmentedTransferFailureCode.LoadSessionFailed);
        }
        else
        {
            CatalogItemIdentity? identity = await this.grainFactory.GetGrain<ICatalogDB>(this.storageContext.CatalogName).GetItemIdentity(request.ItemID,TraceContext.FromActivityContext(System.Diagnostics.Activity.Current?.Context),cancel);

            if(identity is null) { throw new OperationFailedException(PublishedSnapshotUnavailable,SegmentedTransferFailureCode.PublishedSnapshotUnavailable); }

            PublishedTransferSnapshot snapshot = await LoadRequiredPublishedSnapshot(request.ItemID,identity.ItemInstanceToken,cancel);

            manifest = CreatePublishedReadManifest(request,snapshot);

            objectPayload = snapshot.ObjectPayload;
        }
        if(await this.sessions.CreateSegmentedSessionAsync(manifest,objectPayload,cancel) is null) { throw new OperationFailedException(OpenFailed,SegmentedTransferFailureCode.OpenFailed); }

        return new() { State = manifest.ToState() , ObjectPayload = objectPayload };
    }

    ///<inheritdoc/>
    public async Task<OpenUploadTransferResponse> OpenUploadTransfer(OpenUploadTransferRequest request , CancellationToken cancel = default)
    {
        await ValidateOpenRequest(request,cancel);

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(request.ItemID,cancel);

        if(await this.registry.FindOpenConflictByItemId(request.ItemID,this.storageContext.CatalogName,cancel) is not null) { throw new OperationFailedException(ConflictOpenUploadSession,SegmentedTransferFailureCode.ConflictOpenSession); }

        DataItemTransferSegmentSizePolicy policy;

        try { policy = TransferSegmentSizePolicyResolver.Resolve(request.RequestedSegmentSizePolicy,this.options.DefaultSegmentSizePolicy); }

        catch ( ArgumentException error ) { throw new OperationFailedException(InvalidSegmentPolicy,SegmentedTransferFailureCode.InvalidArgument,error); }

        CatalogItemState? itemState = await this.grainFactory.GetGrain<ICatalogDB>(this.storageContext.CatalogName).GetItemState(request.ItemID,TraceContext.FromActivityContext(DiagnosticActivity.Current?.Context),cancel);

        if(itemState?.IsPendingDelete is true) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }

        DataItemTransferManifest manifest = CreateManifest(request,policy,itemState?.ItemInstanceToken ?? Guid.NewGuid());

        if(await this.sessions.CreateSegmentedSessionAsync(manifest,request.ObjectPayload,cancel) is null) { throw new OperationFailedException(OpenFailed,SegmentedTransferFailureCode.OpenFailed); }

        return new() { State = manifest.ToState() };
    }

    ///<inheritdoc/>
    public async Task<PutTransferSegmentResponse> PutTransferSegment(PutTransferSegmentRequest request , Stream payload , CancellationToken cancel = default)
    {
        ValidatePutRequest(request,payload);

        DataItemTransferManifest preflight = await LoadRequiredManifest(request.SessionID,request.ItemID,true,cancel);

        ValidateSessionWritable(preflight); ValidateSegmentLength(preflight,request.Length);

        await using ITransferMutationScope mutationScope = await this.coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,cancel);

        DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,true,cancel);

        ValidateSessionWritable(manifest);

        ValidateSegmentLength(manifest,request.Length);

        DataItemTransferRange range = CreateRange(request.Offset,request.Length);

        Boolean isRetryAtOccupiedOffset = await this.sessions.IsSegmentedRangeCoveredAsync(request.SessionID,range,cancel);

        if(!isRetryAtOccupiedOffset && request.ExpectedStateVersion.HasValue && manifest.StateVersion != request.ExpectedStateVersion.Value) { throw new OperationFailedException(InvalidStateVersion,SegmentedTransferFailureCode.InvalidStateVersion); }

        SegmentedTransferWriteResult written = await this.sessions.WriteVerifiedSegmentedRangeAsync(request.SessionID,manifest,range,payload,request.Length,request.SegmentSHA512,cancel);

        return new() { SessionID = request.SessionID , ItemID = request.ItemID , StateVersion = written.StateVersion , Range = written.Range , Accepted = written.Accepted , CompletedCoverage = written.CompletedCoverage , Replayed = written.Replayed , Updated = written.Updated };
    }

    ///<inheritdoc/>
    public async Task<RemoveTransferResponse> RemoveTransfer(RemoveTransferRequest request , CancellationToken cancel = default)
    {
        ValidateRemoveRequest(request);

        await using ITransferMutationScope mutationScope = await this.coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,cancel);

        DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        if(manifest.Status is not DataItemTransferStatus.Complete and not DataItemTransferStatus.Published) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }

        if(await this.sessions.HasDependentSessionAsync(request.SessionID,cancel)) { throw new OperationFailedException(InvalidSessionState,SegmentedTransferFailureCode.InvalidSessionState); }

        if(await this.sessions.DeleteSessionAsync(request.SessionID,cancel) is false) { throw new OperationFailedException(RemoveFailed,SegmentedTransferFailureCode.RemoveFailed); }

        try
        {
            await this.publications.DeleteJournalAsync(request.SessionID,this.storageContext.CatalogName,cancel);
        }
        catch ( Exception error ) when(error is not OperationCanceledException)
        {
            throw new OperationFailedException(RemoveFailed,SegmentedTransferFailureCode.RemoveFailed,error);
        }

        return new() { SessionID = manifest.SessionID , ItemID = manifest.ItemID , Removed = true };
    }

    ///<inheritdoc/>
    public async Task<Boolean> SessionExists(DataItemTransferIdentity identity , CancellationToken cancel = default)
    {
        if(identity is null || identity.SessionID == Guid.Empty || identity.ItemID == Guid.Empty) { return false; }

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(identity.ItemID,cancel);

        DataItemTransferManifest? manifest = await this.sessions.LoadSegmentedManifestAsync(identity.SessionID,cancel);

        if(manifest?.ItemID != identity.ItemID || manifest.ItemInstanceToken == Guid.Empty) { return false; }

        CatalogItemState? current = await this.grainFactory.GetGrain<ICatalogDB>(this.storageContext.CatalogName).GetItemState(identity.ItemID,TraceContext.FromActivityContext(DiagnosticActivity.Current?.Context),cancel);

        if(current is null) { return manifest.Mode is DataItemTransferMode.Upload; }

        return !current.IsPendingDelete && current.ItemInstanceToken == manifest.ItemInstanceToken;
    }

    ///<inheritdoc/>
    public async Task<ReOpenGetTransferResponse> ReOpenGetTransfer(ReOpenGetTransferRequest request , CancellationToken cancel = default)
    {
        ValidateReOpenGetRequest(request);

        await using ITransferMutationScope mutationScope = await this.coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,cancel);

        DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        ValidateSessionReadable(manifest);

        if(manifest.Mode is DataItemTransferMode.StagedRead && manifest.SourceSessionID.HasValue)
        {
            PublishedTransferSnapshot? published = await this.publishedSource.TryLoadSnapshot(request.ItemID,manifest.ItemInstanceToken,cancel);

            if(published is not null)
            {
                await ValidatePublishedSnapshot(published,request.ItemID,manifest.ItemInstanceToken,cancel);

                if(String.IsNullOrWhiteSpace(manifest.CatalogName) || !String.Equals(manifest.CatalogName,this.storageContext.CatalogName,Ordinal)) { throw new OperationFailedException(CatalogBindingMismatch,SegmentedTransferFailureCode.InvalidSessionState); }

                DataItemTransferManifest promoted = CreatePublishedReadManifest(new OpenGetTransferRequest { SessionID = manifest.SessionID , ItemID = manifest.ItemID },published) with { CatalogName = manifest.CatalogName , RealizedRanges = manifest.RealizedRanges };

                if(await this.sessions.PromoteStagedReadManifestAsync(promoted,cancel) is false) { throw new OperationFailedException(LoadSessionFailed,SegmentedTransferFailureCode.LoadSessionFailed); }

                manifest = promoted;
            }

            else
            {
                DataItemTransferManifest source = await LoadRequiredManifest(manifest.SourceSessionID.Value,request.ItemID,cancel);

                if(String.IsNullOrWhiteSpace(manifest.CatalogName) || String.IsNullOrWhiteSpace(source.CatalogName) || !String.Equals(manifest.CatalogName,source.CatalogName,Ordinal) || !String.Equals(source.CatalogName,this.storageContext.CatalogName,Ordinal)) { throw new OperationFailedException(CatalogBindingMismatch,SegmentedTransferFailureCode.InvalidSessionState); }

                manifest = manifest with { FaultMessage = source.FaultMessage , ObjectInfo = source.ObjectInfo , Status = source.Status , Updated = DateTimeOffset.UtcNow };
            }

            if(manifest.Mode is DataItemTransferMode.StagedRead) { await this.sessions.SaveSegmentedManifestAsync(manifest,cancel:cancel); }
        }

        Byte[] objectPayload;

        if(manifest.Mode is DataItemTransferMode.PublishedRead)
        {
            PublishedTransferSnapshot snapshot = await LoadRequiredPublishedSnapshot(request.ItemID,manifest.ItemInstanceToken,cancel);

            ValidatePublishedSnapshotIdentity(snapshot,manifest);

            manifest = manifest with { Integrity = snapshot.Integrity };

            objectPayload = snapshot.ObjectPayload;
        }
        else
        {
            objectPayload = await this.sessions.ReadObjectPayloadAsync(request.SessionID,cancel) ?? throw new OperationFailedException(LoadSessionFailed,SegmentedTransferFailureCode.LoadSessionFailed);
        }

        await TouchSessionActivity(manifest.SessionID,manifest.ItemID,cancel);

        return new() { State = manifest.ToState() , ObjectPayload = objectPayload };
    }

    ///<inheritdoc/>
    public async Task<ReOpenUploadTransferResponse> ReOpenUploadTransfer(ReOpenUploadTransferRequest request , CancellationToken cancel = default)
    {
        ValidateReOpenRequest(request);

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(request.ItemID,cancel);

        DataItemTransferManifest manifest = await LoadRequiredManifest(request.SessionID,request.ItemID,cancel);

        ValidateManifestHashes(manifest,request.ObjectSHA512,request.StreamLength);

        ValidateSessionWritable(manifest);

        await TouchSessionActivity(manifest.SessionID,manifest.ItemID,cancel);

        return new() { State = manifest.ToState() };
    }
}
