namespace DataPodServices.DataControl;

/**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/main/*'/>*/
internal sealed class DeletionAgent : BackgroundService
{
    /**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/field[@name="jobs"]/*'/>*/
    private readonly IDataControlDeletionJobStore jobs;

    /**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/field[@name="dataConfigs"]/*'/>*/
    private readonly IDataControlDataConfigsClient dataConfigs;

    /**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/field[@name="grains"]/*'/>*/
    private readonly IGrainFactory grains;

    /**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/field[@name="options"]/*'/>*/
    private readonly DeletionAgentOptions options;

    /**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/field[@name="processor"]/*'/>*/
    private readonly DataControlDeletionProcessor processor;

    /**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/field[@name="leaderLock"]/*'/>*/
    private readonly IPostgresAdvisoryLeaderLock leaderLock;

    /**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/constructor[@name="DeletionAgent"]/*'/>*/
    public DeletionAgent(IDataControlDeletionJobStore jobs , IDataControlDataConfigsClient dataConfigs , IGrainFactory grains , DataControlDeletionProcessor processor , IPostgresAdvisoryLeaderLock leaderLock , IOptions<DeletionAgentOptions> options)
    {
        this.jobs = jobs ?? throw new ArgumentNullException(nameof(jobs));

        this.dataConfigs = dataConfigs ?? throw new ArgumentNullException(nameof(dataConfigs));

        this.grains = grains ?? throw new ArgumentNullException(nameof(grains));

        this.processor = processor ?? throw new ArgumentNullException(nameof(processor));

        this.leaderLock = leaderLock ?? throw new ArgumentNullException(nameof(leaderLock));

        this.options = options?.Value ?? new DeletionAgentOptions();

        if(this.options.PollingInterval <= TimeSpan.Zero) { throw new ArgumentOutOfRangeException(nameof(options),"The deletion polling interval must be greater than zero."); }

        if(this.options.LeaderRetryInterval <= TimeSpan.Zero) { throw new ArgumentOutOfRangeException(nameof(options),"The deletion leader retry interval must be greater than zero."); }

        if(this.options.InitialJitterMaximum < TimeSpan.Zero || this.options.InitialJitterMaximum.Ticks == Int64.MaxValue) { throw new ArgumentOutOfRangeException(nameof(options),"The initial jitter maximum must be non-negative and less than Int64.MaxValue ticks."); }

        if(this.options.CycleJitterMaximum < TimeSpan.Zero || this.options.CycleJitterMaximum.Ticks == Int64.MaxValue) { throw new ArgumentOutOfRangeException(nameof(options),"The cycle jitter maximum must be non-negative and less than Int64.MaxValue ticks."); }

        if(this.options.RetryBaseDelay <= TimeSpan.Zero || this.options.MaximumRetryDelay < this.options.RetryBaseDelay) { throw new ArgumentOutOfRangeException(nameof(options),"The retry delays must be positive and the maximum must not be less than the base delay."); }

        if(this.options.CandidatePageSize <= 0) { throw new ArgumentOutOfRangeException(nameof(options),"The candidate page size must be greater than zero."); }
    }

    /**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/method[@name="ExecuteAsync"]/*'/>*/
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try { await Task.Delay(GetRandomDelay(this.options.InitialJitterMaximum),stoppingToken); }

        catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }

        while(stoppingToken.IsCancellationRequested is false)
        {
            IPostgresAdvisoryLeaderLease? leadership = null;

            try
            {
                leadership = await this.leaderLock.TryAcquireAsync(DeletionAgentOptions.LeaderLockName,stoppingToken);

                if(leadership is null)
                {
                    await Task.Delay(GetRandomDelay(this.options.LeaderRetryInterval),stoppingToken);

                    continue;
                }

                while(stoppingToken.IsCancellationRequested is false)
                {
                    await leadership.VerifyAsync(stoppingToken);

                    await ProcessDueJobsAsync(leadership,stoppingToken);

                    await Task.Delay(this.options.PollingInterval,stoppingToken);

                    await Task.Delay(GetRandomDelay(this.options.CycleJitterMaximum),stoppingToken);
                }
            }
            catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }

            catch ( Exception error )
            {
                Log.Warning(error,"Deletion leadership or job scan failed");

                try { await Task.Delay(GetRandomDelay(this.options.LeaderRetryInterval),stoppingToken); }

                catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }
            }
            finally { if(leadership is not null) { await leadership.DisposeAsync(); } }
        }
    }

    /**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/method[@name="ProcessDueJobsAsync"]/*'/>*/
    private async Task ProcessDueJobsAsync(IPostgresAdvisoryLeaderLease leadership , CancellationToken cancel)
    {
        await leadership.VerifyAsync(cancel);

        ConfiguredCatalogNames configured = await this.dataConfigs.GetConfiguredCatalogNamesAsync(cancel);

        foreach(String catalogName in configured.CatalogNames)
        {
            await leadership.VerifyAsync(cancel); cancel.ThrowIfCancellationRequested();

            try
            {
                TenantStorageResolution storage = await this.dataConfigs.GetConfiguredStorageForRecoveryAsync(catalogName,cancel);

                if(storage.Status is not CatalogResolutionStatus.Authorized) { continue; }

                ICatalogDB catalog = this.grains.GetGrain<ICatalogDB>(catalogName);

                Guid? after = null;

                while(true)
                {
                    await leadership.VerifyAsync(cancel);

                    CatalogPendingDeletePage page = await catalog.GetPendingDeletes(after,this.options.CandidatePageSize,cancel);

                    if(page.Items.Length == 0) { break; }

                    foreach(CatalogItemIdentity pending in page.Items)
                    {
                        await leadership.VerifyAsync(cancel); cancel.ThrowIfCancellationRequested();

                        DataControlDeletionJobState job = await this.jobs.EnsureDeletionJobAsync(catalogName,pending.ItemID,pending.ItemInstanceToken,cancel);

                        if(job.NextAttempt is DateTimeOffset retryAt && retryAt > DateTimeOffset.UtcNow) { continue; }

                        try { await ProcessJobAsync(job,cancel); }

                        catch ( OperationCanceledException ) when(cancel.IsCancellationRequested) { throw; }

                        catch ( Exception error ) { Log.Warning(error,"Pending catalog deletion reconciliation failed for item {ItemID} instance {ItemInstanceToken}",pending.ItemID,pending.ItemInstanceToken); }
                    }

                    if(page.NextAfterItemID is not Guid next) { break; }

                    after = next;
                }
            }

            catch ( OperationCanceledException ) when(cancel.IsCancellationRequested) { throw; }

            catch ( PostgresAdvisoryLeaderLeaseLostException ) { throw; }

            catch ( Exception error ) { Log.Warning(error,"Pending deletion reconciliation failed for catalog {CatalogName}",catalogName); }
        }

        while(cancel.IsCancellationRequested is false)
        {
            await leadership.VerifyAsync(cancel);

            IReadOnlyList<DataControlDeletionJobState> jobs = await this.jobs.LoadDueDeletionJobsAsync(DateTimeOffset.UtcNow,this.options.CandidatePageSize,cancel);

            if(jobs.Count == 0) { return; }

            foreach(DataControlDeletionJobState job in jobs)
            {
                await leadership.VerifyAsync(cancel); cancel.ThrowIfCancellationRequested();

                try { await ProcessJobAsync(job,cancel); }

                catch ( OperationCanceledException ) when(cancel.IsCancellationRequested) { throw; }

                catch ( PostgresAdvisoryLeaderLeaseLostException ) { throw; }

                catch ( Exception error ) { Log.Warning(error,"Deletion job attempt failed for item {ItemID} instance {ItemInstanceToken}",job.ItemID,job.ItemInstanceToken); }
            }

            if(jobs.Count < this.options.CandidatePageSize) { return; }
        }
    }

    /**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/method[@name="ProcessJobAsync"]/*'/>*/
    private async Task ProcessJobAsync(DataControlDeletionJobState candidate , CancellationToken cancel)
    {
        try
        {
            await this.processor.ProcessAsync(candidate,cancel);
        }
        catch ( OperationCanceledException ) when(cancel.IsCancellationRequested) { throw; }

        catch ( Exception error ) { Log.Warning(error,"Deletion job attempt failed for item {ItemID} instance {ItemInstanceToken}",candidate.ItemID,candidate.ItemInstanceToken); }
    }

    /**<include file='DeletionAgent.xml' path='DeletionAgent/class[@name="DeletionAgent"]/method[@name="GetRandomDelay"]/*'/>*/
    private static TimeSpan GetRandomDelay(TimeSpan maximum)
    {
        return maximum <= TimeSpan.Zero ? TimeSpan.Zero : TimeSpan.FromTicks(Random.Shared.NextInt64(maximum.Ticks + 1));
    }
}
