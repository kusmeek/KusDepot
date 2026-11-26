namespace DataPodServices.DataControl;

/**<include file='DataControlDeletionProcessor.xml' path='DataControlDeletionProcessor/class[@name="DataControlDeletionProcessor"]/main/*'/>*/
internal sealed class DataControlDeletionProcessor
{
    /**<include file='DataControlDeletionProcessor.xml' path='DataControlDeletionProcessor/class[@name="DataControlDeletionProcessor"]/field[@name="scopes"]/*'/>*/
    private readonly IServiceScopeFactory scopes;

    /**<include file='DataControlDeletionProcessor.xml' path='DataControlDeletionProcessor/class[@name="DataControlDeletionProcessor"]/field[@name="coordinator"]/*'/>*/
    private readonly IDataItemTransferCoordinator coordinator;

    /**<include file='DataControlDeletionProcessor.xml' path='DataControlDeletionProcessor/class[@name="DataControlDeletionProcessor"]/field[@name="jobs"]/*'/>*/
    private readonly IDataControlDeletionJobStore jobs;

    /**<include file='DataControlDeletionProcessor.xml' path='DataControlDeletionProcessor/class[@name="DataControlDeletionProcessor"]/field[@name="dataConfigs"]/*'/>*/
    private readonly IDataControlDataConfigsClient dataConfigs;

    /**<include file='DataControlDeletionProcessor.xml' path='DataControlDeletionProcessor/class[@name="DataControlDeletionProcessor"]/field[@name="options"]/*'/>*/
    private readonly DeletionAgentOptions options;

    /**<include file='DataControlDeletionProcessor.xml' path='DataControlDeletionProcessor/class[@name="DataControlDeletionProcessor"]/constructor[@name="DataControlDeletionProcessor"]/*'/>*/
    public DataControlDeletionProcessor(IServiceScopeFactory scopes , IDataItemTransferCoordinator coordinator , IDataControlDeletionJobStore jobs , IDataControlDataConfigsClient dataConfigs , IOptions<DeletionAgentOptions> options)
    {
        this.scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        this.coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
        this.jobs = jobs ?? throw new ArgumentNullException(nameof(jobs));
        this.dataConfigs = dataConfigs ?? throw new ArgumentNullException(nameof(dataConfigs));
        this.options = options?.Value ?? new DeletionAgentOptions();
    }

    /**<include file='DataControlDeletionProcessor.xml' path='DataControlDeletionProcessor/class[@name="DataControlDeletionProcessor"]/method[@name="ProcessAsync"]/*'/>*/
    public async Task<DataControlDeletionJobState?> ProcessAsync(DataControlDeletionJobState candidate , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        await using IAsyncDisposable itemLock = await this.coordinator.AcquireItemLockAsync(candidate.ItemID,cancel);

        return await ProcessWithLockAsync(candidate,cancel);
    }

    /**<include file='DataControlDeletionProcessor.xml' path='DataControlDeletionProcessor/class[@name="DataControlDeletionProcessor"]/method[@name="ProcessWithLockAsync"]/*'/>*/
    public async Task<DataControlDeletionJobState?> ProcessWithLockAsync(DataControlDeletionJobState candidate , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        DataControlDeletionJobState? job = await this.jobs.LoadDeletionJobAsync(candidate.CatalogName,candidate.ItemID,candidate.ItemInstanceToken,cancel);

        if(job is null) { return null; }

        try
        {
            while(true)
            {
                cancel.ThrowIfCancellationRequested();

                await ProcessProgressAsync(job,cancel);

                if(await this.jobs.LoadDeletionJobAsync(job.CatalogName,job.ItemID,job.ItemInstanceToken,cancel) is not DataControlDeletionJobState next) { return null; }

                job = next;
            }
        }
        catch ( OperationCanceledException ) when(cancel.IsCancellationRequested) { throw; }
        catch ( Exception error )
        {
            Int32 exponent = Math.Clamp(job.AttemptCount,0,20);

            Double ticks = Math.Min(this.options.MaximumRetryDelay.Ticks,this.options.RetryBaseDelay.Ticks * Math.Pow(2,exponent));

            TimeSpan delay = TimeSpan.FromTicks((Int64)Math.Max(1,ticks));

            DateTimeOffset retry = DateTimeOffset.UtcNow.AddTicks(Math.Min(this.options.MaximumRetryDelay.Ticks,delay.Ticks / 2 + Random.Shared.NextInt64(Math.Max(1,delay.Ticks / 2 + 1))));

            await this.jobs.RecordDeletionFailureAsync(job.CatalogName,job.ItemID,job.ItemInstanceToken,error.ToString(),retry,cancel);

            Log.Warning(error,"Deletion job scheduled for retry for item {ItemID} instance {ItemInstanceToken} at {NextAttempt}",job.ItemID,job.ItemInstanceToken,retry);

            return await this.jobs.LoadDeletionJobAsync(job.CatalogName,job.ItemID,job.ItemInstanceToken,cancel);
        }
    }

    /**<include file='DataControlDeletionProcessor.xml' path='DataControlDeletionProcessor/class[@name="DataControlDeletionProcessor"]/method[@name="ProcessProgressAsync"]/*'/>*/
    private async Task ProcessProgressAsync(DataControlDeletionJobState job , CancellationToken cancel)
    {
        await using AsyncServiceScope scope = this.scopes.CreateAsyncScope();

        IServiceProvider services = scope.ServiceProvider;

        IGrainFactory grains = services.GetRequiredService<IGrainFactory>();

        ICatalogDB catalog = grains.GetGrain<ICatalogDB>(job.CatalogName);

        switch(job.Progress)
        {
            case DataControlDeletionProgress.CatalogPending:
            {
                StorageSilo silo = await this.dataConfigs.GetConfiguredStorageForRecoveryAsync(job.CatalogName,cancel) is { Status: CatalogResolutionStatus.Authorized, ConnectionString: not null } resolution
                    ? new StorageSilo(job.CatalogName,resolution.ConnectionString)
                    : throw new InvalidOperationException("Configured tenant storage is unavailable for deletion recovery.");

                if(await services.GetRequiredService<ITenantTransferStorageFactory>().DeleteInstanceTransfersAsync(silo,job.ItemID,job.ItemInstanceToken,cancel) is false) { throw new OperationFailedException("Instance artifact cleanup failed."); }

                await AdvanceAsync(job,DataControlDeletionProgress.PublishedArtifactsRemoved,cancel);

                break;
            }
            case DataControlDeletionProgress.PublishedArtifactsRemoved:
            {
                StorageSilo silo = await this.dataConfigs.GetConfiguredStorageForRecoveryAsync(job.CatalogName,cancel) is { Status: CatalogResolutionStatus.Authorized, ConnectionString: not null } resolution
                    ? new StorageSilo(job.CatalogName,resolution.ConnectionString)
                    : throw new InvalidOperationException("Configured tenant storage is unavailable for deletion recovery.");

                services.GetRequiredService<ITenantTransferStorageContext>().Set(silo);

                IDataControlTransferSessionStore sessions = services.GetRequiredService<IDataControlTransferSessionStore>();

                IReadOnlyList<TransferSessionState> states = await sessions.LoadSessionsAsync(new TransferSessionQuery { CatalogName = job.CatalogName , ItemID = job.ItemID },cancel);

                foreach(TransferSessionState session in states.Where(item => item.ItemInstanceToken == job.ItemInstanceToken)) { await sessions.DeleteSessionAsync(session.SessionID,cancel); }

                IDataControlPublicationTransferStore publications = services.GetRequiredService<IDataControlPublicationTransferStore>();

                await publications.DeleteJournalsForInstanceAsync(job.ItemID,job.ItemInstanceToken,job.CatalogName,cancel);

                await AdvanceAsync(job,DataControlDeletionProgress.TransferStateRemoved,cancel);

                break;
            }
            case DataControlDeletionProgress.TransferStateRemoved:
            {
                CatalogDeleteFinalizationResult finalized = await catalog.FinalizeDelete(job.ItemID,job.ItemInstanceToken,TraceContext.FromActivityContext(DiagnosticActivity.Current?.Context),cancel);

                if(finalized is CatalogDeleteFinalizationResult.TokenMismatch)
                {
                    await this.jobs.DeleteDeletionJobAsync(job.CatalogName,job.ItemID,job.ItemInstanceToken,cancel);

                    break;
                }

                if(finalized is CatalogDeleteFinalizationResult.NotPending) { throw new InvalidOperationException("Catalog item is not pending deletion for this token."); }

                await this.jobs.DeleteDeletionJobAsync(job.CatalogName,job.ItemID,job.ItemInstanceToken,cancel);

                break;
            }
            default: throw new InvalidOperationException($"Unsupported deletion progress: {job.Progress}.");
        }
    }

    /**<include file='DataControlDeletionProcessor.xml' path='DataControlDeletionProcessor/class[@name="DataControlDeletionProcessor"]/method[@name="AdvanceAsync"]/*'/>*/
    private async Task AdvanceAsync(DataControlDeletionJobState job , DataControlDeletionProgress progress , CancellationToken cancel)
    {
        if(await this.jobs.AdvanceDeletionAsync(job.CatalogName,job.ItemID,job.ItemInstanceToken,job.Progress,progress,cancel) is false)
        {
            throw new InvalidOperationException("Deletion progress changed before the cleanup phase could be committed.");
        }
    }
}
