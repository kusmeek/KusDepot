namespace DataPodServices.DataConfigs;

internal sealed class DataConfigsSnapshotUpdater : BackgroundService , ILifecycleParticipant<ISiloLifecycle>
{
    private readonly IGrainFactory grainFactory;
    private readonly SiloProbeHealthState health;
    private readonly IDataConfigsSnapshotProvider snapshots;

    private const String CoordinatorGrainKey = "DataConfigsCoordinator";
    private static readonly TimeSpan PollPeriod = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan PollJitter = TimeSpan.FromSeconds(30);

    public DataConfigsSnapshotUpdater(IDataConfigsSnapshotProvider snapshots , SiloProbeHealthState health , IGrainFactory grainFactory)
    {
        this.snapshots = snapshots; this.health = health; this.grainFactory = grainFactory;

        this.health.RequireDataConfigsSnapshot();
    }

    public void Participate(ISiloLifecycle lifecycle)
    {
        lifecycle.Subscribe(nameof(DataConfigsSnapshotUpdater),ServiceLifecycleStage.Active,this.StartAsync,this.StopAsync);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Int32 failureCount = 0;

        try
        {
            await this.PollAsync(stoppingToken);
        }
        catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested)
        {
            return;
        }
        catch ( Exception exception )
        {
            failureCount++;

            this.LogPollFailure(exception,"Initial DataConfigs snapshot synchronization failed; readiness remains false.");
        }

        while(stoppingToken.IsCancellationRequested is false)
        {
            Int32 jitterSeconds = (Int32)PollJitter.TotalSeconds;

            TimeSpan delay = failureCount == 0
                ? PollPeriod + TimeSpan.FromSeconds(Random.Shared.Next(-jitterSeconds,jitterSeconds + 1))
                : TimeSpan.FromSeconds(Math.Min(300,10 * Math.Pow(2,Math.Min(failureCount - 1,5))) + Random.Shared.Next(0,6));

            try
            {
                await Task.Delay(delay,stoppingToken);

                await this.PollAsync(stoppingToken);

                failureCount = 0;
            }
            catch ( OperationCanceledException ) when(stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch ( Exception exception )
            {
                failureCount++;

                this.LogPollFailure(exception,"DataConfigs snapshot synchronization failed; the current snapshot will be retained and retried.");
            }
        }
    }

    private async Task PollAsync(CancellationToken cancel)
    {
        IDataConfigsCoordinatorGrain coordinator = this.grainFactory.GetGrain<IDataConfigsCoordinatorGrain>(CoordinatorGrainKey);

        ConfigurationGeneration generation = this.snapshots.IsReady ? this.snapshots.Current.Generation : ConfigurationGeneration.Empty;

        SnapshotTransferResult result = await coordinator.GetSnapshotIfNewerAsync(generation,cancel);

        if(result.HasUpdate is false)
        {
            if(this.snapshots.IsReady is false)
            {
                throw new InvalidOperationException("The coordinator returned no snapshot while this Silo has no installed DataConfigs snapshot.");
            }

            this.health.SetDataConfigsSnapshotReady(true);

            return;
        }

        DataConfigsSnapshot snapshot = result.Snapshot?.ToSnapshot() ?? throw new InvalidOperationException("The coordinator returned an empty DataConfigs snapshot update.");

        if(this.snapshots.TryReplace(snapshot))
        {
            Log.Information("Installed DataConfigs generation {Generation} containing {CatalogCount} catalogs.",snapshot.Generation.Value,snapshot.CatalogsByName.Count);
        }

        this.health.SetDataConfigsSnapshotReady(this.snapshots.IsReady);
    }

    private void LogPollFailure(Exception exception , String message)
    {
        if(this.snapshots.IsReady)
        {
            DataConfigsSnapshot snapshot = this.snapshots.Current;

            Log.Warning(exception,"{Message} Current generation {Generation} is {SnapshotAge} old.",message,snapshot.Generation.Value,DateTimeOffset.UtcNow - snapshot.PublishedAt);

            return;
        }

        Log.Warning(exception,"{Message} No DataConfigs snapshot is installed.",message);
    }
}
