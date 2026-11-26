namespace DataPodServices.DataControl;

/**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/main/*'/>*/
internal sealed class TransferSessionCleanupAgent : BackgroundService
{
    /**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/field[@name="scopes"]/*'/>*/
    private readonly IServiceScopeFactory scopes;

    /**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/field[@name="coordinator"]/*'/>*/
    private readonly IDataItemTransferCoordinator coordinator;

    /**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/field[@name="dataConfigs"]/*'/>*/
    private readonly IDataControlDataConfigsClient dataConfigs;

    /**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/field[@name="state"]/*'/>*/
    private readonly ITransferStateStore state;

    /**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/field[@name="options"]/*'/>*/
    private readonly TransferSessionCleanupAgentOptions options;

    /**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/field[@name="leaderLock"]/*'/>*/
    private readonly IPostgresAdvisoryLeaderLock leaderLock;

    /**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/constructor[@name="TransferSessionCleanupAgent"]/*'/>*/
    public TransferSessionCleanupAgent(IServiceScopeFactory scopes , IDataItemTransferCoordinator coordinator , IDataControlDataConfigsClient dataConfigs , ITransferStateStore state , IPostgresAdvisoryLeaderLock leaderLock , IOptions<TransferSessionCleanupAgentOptions> options)
    {
        this.scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        this.coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
        this.dataConfigs = dataConfigs ?? throw new ArgumentNullException(nameof(dataConfigs));
        this.state = state ?? throw new ArgumentNullException(nameof(state));
        this.leaderLock = leaderLock ?? throw new ArgumentNullException(nameof(leaderLock));
        this.options = options?.Value ?? new TransferSessionCleanupAgentOptions();

        if(this.options.InactivityTimeout <= TimeSpan.Zero) { throw new ArgumentOutOfRangeException(nameof(options),"The transfer-session inactivity timeout must be greater than zero."); }
        if(this.options.PollingInterval <= TimeSpan.Zero) { throw new ArgumentOutOfRangeException(nameof(options),"The transfer-session cleanup polling interval must be greater than zero."); }
        if(this.options.LeaderRetryInterval <= TimeSpan.Zero) { throw new ArgumentOutOfRangeException(nameof(options),"The transfer-session cleanup leader retry interval must be greater than zero."); }
        if(this.options.InitialJitterMaximum < TimeSpan.Zero || this.options.InitialJitterMaximum.Ticks == Int64.MaxValue) { throw new ArgumentOutOfRangeException(nameof(options),"The initial jitter maximum must be non-negative and less than Int64.MaxValue ticks."); }
        if(this.options.CandidatePageSize <= 0) { throw new ArgumentOutOfRangeException(nameof(options),"The candidate page size must be greater than zero."); }
    }

    /**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/method[@name="ExecuteAsync"]/*'/>*/
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try { await Task.Delay(GetRandomDelay(this.options.InitialJitterMaximum),stoppingToken); }

        catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }

        while(stoppingToken.IsCancellationRequested is false)
        {
            IPostgresAdvisoryLeaderLease? leadership = null;

            try
            {
                leadership = await this.leaderLock.TryAcquireAsync(TransferSessionCleanupAgentOptions.LeaderLockName,stoppingToken);

                if(leadership is null)
                {
                    await Task.Delay(GetRandomDelay(this.options.LeaderRetryInterval),stoppingToken);

                    continue;
                }

                while(stoppingToken.IsCancellationRequested is false)
                {
                    await leadership.VerifyAsync(stoppingToken);

                    await ProcessStaleSessionsAsync(leadership,stoppingToken);

                    await Task.Delay(this.options.PollingInterval,stoppingToken);
                }
            }
            catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }

            catch ( Exception error )
            {
                Log.Warning(error,"Transfer-session cleanup leadership or scan failed");

                try { await Task.Delay(GetRandomDelay(this.options.LeaderRetryInterval),stoppingToken); }

                catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested) { return; }
            }
            finally { if(leadership is not null) { await leadership.DisposeAsync(); } }
        }
    }

    /**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/method[@name="ProcessStaleSessionsAsync"]/*'/>*/
    private async Task ProcessStaleSessionsAsync(IPostgresAdvisoryLeaderLease leadership , CancellationToken cancel)
    {
        await leadership.VerifyAsync(cancel);

        ConfiguredCatalogNames configured = await this.dataConfigs.GetConfiguredCatalogNamesAsync(cancel);

        DateTimeOffset cutoff = DateTimeOffset.UtcNow - this.options.InactivityTimeout;

        foreach(String catalogName in configured.CatalogNames)
        {
            await leadership.VerifyAsync(cancel); cancel.ThrowIfCancellationRequested();

            StorageSilo? silo = await this.dataConfigs.ResolveRecoveryStorageAsync(catalogName,cancel); if(silo is null) { continue; }

            await using AsyncServiceScope scope = this.scopes.CreateAsyncScope();

            ITenantTransferStorageContext storageContext = scope.ServiceProvider.GetRequiredService<ITenantTransferStorageContext>(); storageContext.Set(silo);

            IDataControlTransferSessionStore sessions = scope.ServiceProvider.GetRequiredService<IDataControlTransferSessionStore>();

            TransferSessionCleanupCandidate? after = null;

            while(cancel.IsCancellationRequested is false)
            {
                await leadership.VerifyAsync(cancel);

                IReadOnlyList<TransferSessionCleanupCandidate> candidates = await this.state.LoadStaleTransferSessionCandidatesAsync(catalogName,cutoff,after,this.options.CandidatePageSize,cancel);

                if(candidates.Count == 0) { break; }

                foreach(TransferSessionCleanupCandidate candidate in candidates)
                {
                    await leadership.VerifyAsync(cancel); cancel.ThrowIfCancellationRequested();

                    try { await CleanupCandidateAsync(candidate,catalogName,cutoff,sessions,cancel); }

                    catch ( OperationCanceledException ) when(cancel.IsCancellationRequested) { throw; }

                    catch ( PostgresAdvisoryLeaderLeaseLostException ) { throw; }

                    catch ( Exception error ) { Log.Warning(error,"Stale transfer-session cleanup failed for {SessionID} in catalog {CatalogName}",candidate.SessionID,catalogName); }
                }

                after = candidates[^1];
            }
        }
    }

    /**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/method[@name="CleanupCandidateAsync"]/*'/>*/
    private async Task CleanupCandidateAsync(TransferSessionCleanupCandidate candidate , String catalogname , DateTimeOffset cutoff , IDataControlTransferSessionStore sessions , CancellationToken cancel)
    {
        await using ITransferMutationScope mutation = await this.coordinator.AcquireMutationScopeAsync(candidate.ItemID,candidate.SessionID,cancel);

        IReadOnlyList<TransferSessionState> currentSessions = await sessions.LoadSessionsAsync(new TransferSessionQuery { CatalogName = catalogname , SessionID = candidate.SessionID },cancel);

        TransferSessionState? current = currentSessions.SingleOrDefault();

        if(current is null
            || current.ItemID != candidate.ItemID
            || !String.Equals(candidate.CatalogName,catalogname,Ordinal)
            || current.LastActivity > cutoff) { return; }

        if(current.Status is DataItemTransferStatus.Publishing) { return; }

        TransferPublicationJournalState? journal = await this.state.LoadJournalAsync(current.SessionID,catalogname,cancel); if(journal is not null) { return; }

        if(await this.state.HasDependentSessionAsync(current.SessionID,catalogname,cancel)) { return; }

        if(await sessions.DeleteSessionAsync(current.SessionID,cancel))
        {
            Log.Information("Removed inactive transfer session {SessionID} in catalog {CatalogName}",current.SessionID,catalogname);
        }
    }

    /**<include file='TransferSessionCleanupAgent.xml' path='TransferSessionCleanupAgent/class[@name="TransferSessionCleanupAgent"]/method[@name="GetRandomDelay"]/*'/>*/
    private static TimeSpan GetRandomDelay(TimeSpan maximum)
    {
        return maximum <= TimeSpan.Zero ? TimeSpan.Zero : TimeSpan.FromTicks(Random.Shared.NextInt64(maximum.Ticks + 1));
    }
}
