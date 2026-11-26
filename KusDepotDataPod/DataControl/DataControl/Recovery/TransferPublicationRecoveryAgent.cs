namespace DataPodServices.DataControl;

/**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/main/*'/>*/
internal sealed class TransferPublicationRecoveryAgent : BackgroundService
{
    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/field[@name="grainFactory"]/*'/>*/
    private readonly IGrainFactory grainFactory;

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/field[@name="dataConfigs"]/*'/>*/
    private readonly IDataControlDataConfigsClient dataConfigs;

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/field[@name="scopes"]/*'/>*/
    private readonly IServiceScopeFactory scopes;

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/field[@name="coordinator"]/*'/>*/
    private readonly IDataItemTransferCoordinator coordinator;

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/field[@name="storageFactory"]/*'/>*/
    private readonly ITenantTransferStorageFactory storageFactory;

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/field[@name="options"]/*'/>*/
    private readonly TransferPublicationRecoveryAgentOptions options;

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/field[@name="leaderLock"]/*'/>*/
    private readonly IPostgresAdvisoryLeaderLock leaderLock;

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/constructor[@name="TransferPublicationRecoveryAgent"]/*'/>*/
    public TransferPublicationRecoveryAgent(IServiceScopeFactory scopes , IDataItemTransferCoordinator coordinator , IGrainFactory grainFactory , IDataControlDataConfigsClient dataConfigs , ITenantTransferStorageFactory storageFactory , IPostgresAdvisoryLeaderLock leaderLock , IOptions<TransferPublicationRecoveryAgentOptions> options)
    {
        this.scopes = scopes; this.coordinator = coordinator; this.grainFactory = grainFactory; this.dataConfigs = dataConfigs; this.storageFactory = storageFactory;

        this.leaderLock = leaderLock ?? throw new ArgumentNullException(nameof(leaderLock));

        this.options = options?.Value ?? new TransferPublicationRecoveryAgentOptions();

        if(this.options.PollingInterval <= TimeSpan.Zero) { throw new ArgumentOutOfRangeException(nameof(options),"The recovery polling interval must be greater than zero."); }

        if(this.options.LeaderRetryInterval <= TimeSpan.Zero) { throw new ArgumentOutOfRangeException(nameof(options),"The recovery leader retry interval must be greater than zero."); }

        if(this.options.InitialJitterMaximum < TimeSpan.Zero || this.options.InitialJitterMaximum.Ticks == Int64.MaxValue) { throw new ArgumentOutOfRangeException(nameof(options),"The initial jitter maximum must be non-negative and less than Int64.MaxValue ticks."); }

        if(this.options.CycleJitterMaximum < TimeSpan.Zero || this.options.CycleJitterMaximum.Ticks == Int64.MaxValue) { throw new ArgumentOutOfRangeException(nameof(options),"The cycle jitter maximum must be non-negative and less than Int64.MaxValue ticks."); }

        if(this.options.TerminalFailureThreshold <= 0) { throw new ArgumentOutOfRangeException(nameof(options),"The terminal failure threshold must be greater than zero."); }

        if(this.options.CandidatePageSize <= 0) { throw new ArgumentOutOfRangeException(nameof(options),"The candidate page size must be greater than zero."); }
    }

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/method[@name="ExecuteAsync"]/*'/>*/
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try { await Task.Delay(GetRandomDelay(this.options.InitialJitterMaximum),stoppingToken); }

        catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }

        while(stoppingToken.IsCancellationRequested is false)
        {
            IPostgresAdvisoryLeaderLease? leadership = null;

            try
            {
                leadership = await this.leaderLock.TryAcquireAsync(TransferPublicationRecoveryAgentOptions.LeaderLockName,stoppingToken);

                if(leadership is null)
                {
                    await Task.Delay(GetRandomDelay(this.options.LeaderRetryInterval),stoppingToken);

                    continue;
                }

                try
                {
                    while(stoppingToken.IsCancellationRequested is false)
                    {
                        await leadership.VerifyAsync(stoppingToken);

                        try
                        {
                            await using AsyncServiceScope serviceScope = this.scopes.CreateAsyncScope();

                            IDataControlPublicationTransferStore publications = serviceScope.ServiceProvider.GetRequiredService<IDataControlPublicationTransferStore>();

                            DateTimeOffset cutoff = DateTimeOffset.UtcNow;

                            TransferPublicationRecoveryCandidate? after = null;

                            while(stoppingToken.IsCancellationRequested is false)
                            {
                                IReadOnlyList<TransferPublicationRecoveryCandidate> candidates = await publications.LoadRecoverableCandidatesAsync(cutoff,after,this.options.CandidatePageSize,stoppingToken);

                                if(candidates.Count == 0) { break; }

                                foreach(TransferPublicationRecoveryCandidate candidate in candidates)
                                {
                                    if(stoppingToken.IsCancellationRequested) { break; }

                                    await leadership.VerifyAsync(stoppingToken);

                                    try { await RecoverPublicationAsync(candidate,publications,serviceScope.ServiceProvider,stoppingToken); }

                                    catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }

                                    catch ( Exception error ) { Log.Warning(error,"Transfer publication recovery attempt failed for {SessionID}",candidate.SessionID); }
                                }

                                after = candidates[^1];
                            }
                        }
                        catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }

                        catch ( PostgresAdvisoryLeaderLeaseLostException ) { throw; }

                        catch ( Exception error ) { Log.Warning(error,"Transfer publication journal load failed"); }

                        try
                        {
                            await Task.Delay(this.options.PollingInterval,stoppingToken);

                            await Task.Delay(GetRandomDelay(this.options.CycleJitterMaximum),stoppingToken);
                        }
                        catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }

                        catch ( Exception error )
                        {
                            Log.Warning(error,"Transfer publication recovery scan failed");

                            break;
                        }
                    }
                }
                finally { if(leadership is not null) { await leadership.DisposeAsync(); } }
            }

            catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }

            catch ( Exception error )
            {
                Log.Warning(error,"Transfer publication recovery leadership or scan failed");

                try
                {
                    await Task.Delay(GetRandomDelay(this.options.LeaderRetryInterval),stoppingToken);
                }

                catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }
            }
        }
    }

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/method[@name="RecoverPublicationAsync"]/*'/>*/
    private async Task RecoverPublicationAsync(TransferPublicationRecoveryCandidate candidate , IDataControlPublicationTransferStore publications , IServiceProvider serviceProvider , CancellationToken cancel)
    {
        ITransferMutationScope mutationScope = await this.coordinator.AcquireMutationScopeAsync(candidate.ItemID,candidate.SessionID,cancel);

        await using (mutationScope)
        {
            TransferPublicationJournal? current = await publications.LoadJournalAsync(candidate.SessionID,candidate.CatalogName,cancel);

            if(current is null || current.OperationID != candidate.OperationID || current.ItemID != candidate.ItemID || !String.Equals(current.CatalogName,candidate.CatalogName,Ordinal) || current.ItemInstanceToken == Guid.Empty || current.Phase is TransferPublicationPhase.OperatorActionRequired or TransferPublicationPhase.Failed) { return; }

            if(current.Phase is TransferPublicationPhase.Finalized)
            {
                await publications.DeleteJournalAsync(current.SessionID,candidate.CatalogName,cancel);

                return;
            }

            StorageSilo? silo = await this.dataConfigs.ResolveRecoveryStorageAsync(candidate.CatalogName,cancel);

            if(silo is null) { return; }

            CatalogItemState? identity = await this.grainFactory.GetGrain<ICatalogDB>(current.CatalogName).GetItemState(current.ItemID,TraceContext.FromActivityContext(DiagnosticActivity.Current?.Context),cancel);

            if(identity is not null && (identity.IsPendingDelete || identity.ItemInstanceToken != current.ItemInstanceToken)) { return; }

            StorageSilo storageSilo = silo;

            IDataControlPublicationTransferStore recoveryPublications = CreateRecoveryStore(publications,storageSilo,serviceProvider);

            try
            {
                if(current.FailureCount >= this.options.TerminalFailureThreshold)
                {
                    await recoveryPublications.AdvanceJournalAsync(current.SessionID,current.OperationID,current.CatalogName,TransferPublicationPhase.Failed,current.LastError,null,cancel);

                    Log.Error("Transfer publication journal reached its terminal failure threshold for {SessionID}: {Error}",current.SessionID,current.LastError);

                    return;
                }

                IDataItemSegmentedTransferService segmented = serviceProvider.GetRequiredService<IDataItemSegmentedTransferService>();

                IDataStreamTransferService stream = serviceProvider.GetRequiredService<IDataStreamTransferService>();

                PublicationRecoveryResult result = current.Family is TransferPublicationFamily.Segmented
                    ? await RecoverSegmentedAsync(current,storageSilo,mutationScope,segmented,recoveryPublications,cancel)
                    : current.Family is TransferPublicationFamily.Stream
                        ? await RecoverStreamAsync(current,storageSilo,mutationScope,stream,recoveryPublications,cancel)
                        : PublicationRecoveryResult.Failure($"Unsupported transfer publication family: {current.Family}.");

                if(result.Succeeded) { await recoveryPublications.DeleteJournalAsync(current.SessionID,candidate.CatalogName,cancel); }

                else { await RecordFailureAsync(current,recoveryPublications,result.FailureReason!,cancel); }
            }
            catch ( OperationCanceledException ) when(cancel.IsCancellationRequested) { throw; }

            catch ( Exception error ) { await RecordFailureAsync(current,recoveryPublications,error.ToString(),cancel); }
        }
    }

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/method[@name="CreateRecoveryStore"]/*'/>*/
    private static IDataControlPublicationTransferStore CreateRecoveryStore(IDataControlPublicationTransferStore publications , StorageSilo silo , IServiceProvider serviceProvider)
    {
        ITenantTransferStorageContext storageContext = serviceProvider.GetRequiredService<ITenantTransferStorageContext>();

        storageContext.Set(silo);

        return publications;
    }

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/method[@name="GetRandomDelay"]/*'/>*/
    private static TimeSpan GetRandomDelay(TimeSpan maximum)
    {
        return maximum <= TimeSpan.Zero ? TimeSpan.Zero : TimeSpan.FromTicks(Random.Shared.NextInt64(maximum.Ticks + 1));
    }

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/method[@name="RecoverSegmentedAsync"]/*'/>*/
    private async Task<PublicationRecoveryResult> RecoverSegmentedAsync(TransferPublicationJournal journal , StorageSilo silo , ITransferMutationScope mutationScope , IDataItemSegmentedTransferService segmented , IDataControlPublicationTransferStore publications , CancellationToken cancel)
    {
        DataItemTransferManifest? manifest = await publications.LoadSegmentedManifestAsync(journal.SessionID,cancel);

        if(manifest is null || manifest.ItemID != journal.ItemID || manifest.ItemInstanceToken != journal.ItemInstanceToken) { return PublicationRecoveryResult.Failure("The segmented transfer manifest was not found or belongs to another item instance."); }

        PublishSegmentedTransferResponse response = new() { State = manifest.ToState() , Published = manifest.Status is DataItemTransferStatus.Published };

        if(journal.Phase is TransferPublicationPhase.Prepared)
        {
            if(manifest.Status is DataItemTransferStatus.Open or DataItemTransferStatus.Complete)
            {
                response = await segmented.BeginPublishSegmentedTransfer(new PublishSegmentedTransferRequest { ExpectedStateVersion = journal.ExpectedStateVersion , ItemID = journal.ItemID , SessionID = journal.SessionID },mutationScope,cancel);
            }

            TransferContentIntegrity? integrity = response.State.Integrity ?? journal.Integrity ?? await publications.RecomputeIntegrityAsync(journal.SessionID,cancel);

            journal = await publications.AdvanceJournalAsync(journal.SessionID,journal.OperationID,journal.CatalogName,TransferPublicationPhase.Publishing,cancel:cancel,integrity:integrity);
        }

        Byte[]? objectPayload = await publications.ReadObjectPayloadAsync(journal.SessionID,cancel);

        if(objectPayload is null || objectPayload.Length == 0) { return PublicationRecoveryResult.Failure("The segmented object payload was not found."); }

        DataItem? item = DataItem.Deserialize(objectPayload); Descriptor? descriptor = item?.GetDescriptor();

        if(descriptor is null) { return PublicationRecoveryResult.Failure("The segmented object payload could not be deserialized."); }

        manifest = await publications.LoadSegmentedManifestAsync(journal.SessionID,cancel);

        if(manifest is null || manifest.ItemID != journal.ItemID || manifest.ItemInstanceToken != journal.ItemInstanceToken) { return PublicationRecoveryResult.Failure("The segmented transfer manifest was not found or belongs to another item instance."); }

        manifest = manifest with { Integrity = journal.Integrity };

        Boolean publishStream = manifest.ObjectInfo?.ContentStreamed is true;

        IReadOnlyList<TransferContentIntegritySegment>? integritySegments = publishStream ? await publications.LoadIntegritySegmentsAsync(journal.SessionID,cancel) : null;

        if(journal.Phase is TransferPublicationPhase.Publishing)
        {
            TenantTransferBlobReference? stagedObject = await publications.LoadObjectReferenceAsync(journal.SessionID,cancel);

            TransferBlockPlan? streamPlan = publishStream ? await publications.CreateSegmentedBlockPlanAsync(journal.SessionID,cancel) : null;

            if(stagedObject is null || publishStream && streamPlan is null) { return PublicationRecoveryResult.Failure("The segmented publication payload could not be resolved."); }

            ITenantTransferStorage storage = this.storageFactory.Create(silo,journal.SessionID,journal.ItemID,journal.ItemInstanceToken);

            Boolean published = await Publisher.PublishArtifactsAsync(storage,stagedObject,manifest.ObjectSHA512,streamPlan,publishStream,manifest.StreamLength,publishStream ? manifest.Integrity : null,publishStream ? integritySegments : null,cancel);

            if(published is false) { return PublicationRecoveryResult.Failure("The segmented publication artifacts could not be published or verified."); }

            journal = await publications.AdvanceJournalAsync(journal.SessionID,journal.OperationID,journal.CatalogName,TransferPublicationPhase.Published,cancel:cancel);
        }

        if(journal.Phase is TransferPublicationPhase.Published)
        {
            if(await Publisher.UpdateCatalogAsync(this.grainFactory,silo,descriptor,journal.ItemInstanceToken,TraceContext.FromActivityContext(DiagnosticActivity.Current?.Context),cancel) is false) { return PublicationRecoveryResult.Failure("The segmented publication catalog update failed."); }

            journal = await publications.AdvanceJournalAsync(journal.SessionID,journal.OperationID,journal.CatalogName,TransferPublicationPhase.CatalogUpdated,cancel:cancel);
        }

        if(journal.Phase is TransferPublicationPhase.CatalogUpdated)
        {
            _ = await segmented.FinishPublishSegmentedTransfer(new DataItemTransferIdentity { ItemID = journal.ItemID , SessionID = journal.SessionID },mutationScope,cancel);

            await publications.AdvanceJournalAsync(journal.SessionID,journal.OperationID,journal.CatalogName,TransferPublicationPhase.Finalized,cancel:cancel);

            return PublicationRecoveryResult.Success();
        }

        if(journal.Phase is TransferPublicationPhase.Finalized) { return PublicationRecoveryResult.Success(); }

        return PublicationRecoveryResult.Failure($"The segmented publication recovery stopped at an unsupported phase: {journal.Phase}.");
    }

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/method[@name="RecoverStreamAsync"]/*'/>*/
    private async Task<PublicationRecoveryResult> RecoverStreamAsync(TransferPublicationJournal journal , StorageSilo silo , ITransferMutationScope mutationScope , IDataStreamTransferService stream , IDataControlPublicationTransferStore publications , CancellationToken cancel)
    {
        DataStreamTransferManifest? manifest = await publications.LoadStreamManifestAsync(journal.SessionID,cancel);

        if(manifest is null || manifest.ItemID != journal.ItemID || manifest.ItemInstanceToken != journal.ItemInstanceToken) { return PublicationRecoveryResult.Failure("The stream transfer manifest was not found or belongs to another item instance."); }

        PublishStreamTransferResponse response = new() { State = manifest.ToState() , Published = manifest.Status is DataItemTransferStatus.Published };

        if(journal.Phase is TransferPublicationPhase.Prepared or TransferPublicationPhase.Publishing)
        {
            response = await stream.BeginPublishStreamTransfer(new PublishStreamTransferRequest
            {
                ExpectedStateVersion = journal.ExpectedStateVersion,
                FinalObjectInfo = journal.FinalObjectInfo,
                FinalObjectPayload = journal.FinalObjectPayload,
                FinalStreamLength = journal.FinalStreamLength,
                ItemID = journal.ItemID,
                SessionID = journal.SessionID,
            },mutationScope,cancel);

            if(journal.Phase is TransferPublicationPhase.Prepared)
            {
                TransferContentIntegrity? integrity = response.State.Integrity ?? journal.Integrity ?? await publications.RecomputeIntegrityAsync(journal.SessionID,cancel);

                journal = await publications.AdvanceJournalAsync(journal.SessionID,journal.OperationID,journal.CatalogName,TransferPublicationPhase.Publishing,cancel:cancel,integrity:integrity);
            }
        }

        Byte[]? objectPayload = await publications.ReadObjectPayloadAsync(journal.SessionID,cancel);

        if(objectPayload is null || objectPayload.Length == 0) { return PublicationRecoveryResult.Failure("The stream object payload was not found."); }

        DataItem? item = DataItem.Deserialize(objectPayload); Descriptor? descriptor = item?.GetDescriptor();

        if(descriptor is null) { return PublicationRecoveryResult.Failure("The stream object payload could not be deserialized."); }

        manifest = await publications.LoadStreamManifestAsync(journal.SessionID,cancel);

        if(manifest is null || manifest.ItemID != journal.ItemID || manifest.ItemInstanceToken != journal.ItemInstanceToken) { return PublicationRecoveryResult.Failure("The stream transfer manifest was not found or belongs to another item instance."); }

        manifest = manifest with { Integrity = journal.Integrity };

        IReadOnlyList<TransferContentIntegritySegment>? integritySegments = await publications.LoadIntegritySegmentsAsync(journal.SessionID,cancel);

        if(journal.Phase is TransferPublicationPhase.Publishing)
        {
            TenantTransferBlobReference? stagedObject = await publications.LoadObjectReferenceAsync(journal.SessionID,cancel);

            TransferBlockPlan? streamPlan = await publications.CreateStreamBlockPlanAsync(journal.SessionID,cancel);

            if(stagedObject is null || streamPlan is null) { return PublicationRecoveryResult.Failure("The stream publication payload could not be resolved."); }

            ITenantTransferStorage storage = this.storageFactory.Create(silo,journal.SessionID,journal.ItemID,journal.ItemInstanceToken);

            Boolean published = await Publisher.PublishArtifactsAsync(storage,stagedObject,manifest.ObjectSHA512,streamPlan,true,manifest.AppendedLength,manifest.Integrity,integritySegments,cancel);

            if(published is false) { return PublicationRecoveryResult.Failure("The stream publication artifacts could not be published or verified."); }

            journal = await publications.AdvanceJournalAsync(journal.SessionID,journal.OperationID,journal.CatalogName,TransferPublicationPhase.Published,cancel:cancel);
        }

        if(journal.Phase is TransferPublicationPhase.Published)
        {
            if(await Publisher.UpdateCatalogAsync(this.grainFactory,silo,descriptor,journal.ItemInstanceToken,TraceContext.FromActivityContext(DiagnosticActivity.Current?.Context),cancel) is false) { return PublicationRecoveryResult.Failure("The stream publication catalog update failed."); }

            journal = await publications.AdvanceJournalAsync(journal.SessionID,journal.OperationID,journal.CatalogName,TransferPublicationPhase.CatalogUpdated,cancel:cancel);
        }

        if(journal.Phase is TransferPublicationPhase.CatalogUpdated)
        {
            _ = await stream.FinishPublishStreamTransfer(new DataItemTransferIdentity { ItemID = journal.ItemID , SessionID = journal.SessionID },mutationScope,cancel);

            await publications.AdvanceJournalAsync(journal.SessionID,journal.OperationID,journal.CatalogName,TransferPublicationPhase.Finalized,cancel:cancel);

            return PublicationRecoveryResult.Success();
        }

        if(journal.Phase is TransferPublicationPhase.Finalized) { return PublicationRecoveryResult.Success(); }

        return PublicationRecoveryResult.Failure($"The stream publication recovery stopped at an unsupported phase: {journal.Phase}.");
    }

    /**<include file='TransferPublicationRecoveryAgent.xml' path='TransferPublicationRecoveryAgent/class[@name="TransferPublicationRecoveryAgent"]/method[@name="RecordFailureAsync"]/*'/>*/
    private async Task RecordFailureAsync(TransferPublicationJournal journal , IDataControlPublicationTransferStore publications , String error , CancellationToken cancel)
    {
        try
        {
            if(cancel.IsCancellationRequested) { cancel.ThrowIfCancellationRequested(); }

            if(journal.Phase is TransferPublicationPhase.OperatorActionRequired or TransferPublicationPhase.Failed) { return; }

            TransferPublicationJournal recorded = await publications.RecordJournalFailureAsync(journal.SessionID,journal.OperationID,journal.CatalogName,error,cancel);

            if(recorded.FailureCount >= this.options.TerminalFailureThreshold && recorded.Phase is not TransferPublicationPhase.Finalized)
            {
                recorded = await publications.AdvanceJournalAsync(recorded.SessionID,recorded.OperationID,recorded.CatalogName,TransferPublicationPhase.Failed,recorded.LastError,null,CancellationToken.None);

                Log.Error("Transfer publication journal reached its terminal failure threshold for {SessionID}: {Error}",recorded.SessionID,error);
            }
        }
        catch ( OperationCanceledException ) when(cancel.IsCancellationRequested) { throw; }

        catch ( Exception recordError ) { Log.Warning(recordError,"Transfer publication recovery failure could not be journaled for {SessionID}",journal.SessionID); }
    }
}
