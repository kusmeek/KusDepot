namespace DataPodServices.DataConfigs;

internal sealed class DataConfigsCoordinatorGrain : Grain , IDataConfigsAdminGrain , IDataConfigsCoordinatorGrain
{
    private readonly IDataConfigsSnapshotLoader loader;
    private readonly DataConfigsAuthorizer authorizer;
    private readonly DataConfigsDiagnostics diagnostics;
    private DataConfigsSnapshot? snapshot;

    public DataConfigsCoordinatorGrain(IDataConfigsSnapshotLoader loader , DataConfigsAuthorizer authorizer , DataConfigsDiagnostics diagnostics)
    {
        this.loader = loader; this.authorizer = authorizer; this.diagnostics = diagnostics;
    }

    public override async Task OnActivateAsync(CancellationToken cancel)
    {
        ConfigurationGeneration generation = new(Guid.NewGuid());

        this.snapshot = await this.loader.LoadAsync(generation,cancel);
    }

    public async Task<RefreshResult> RefreshCatalogsAsync(String token , TraceContext? tracecontext , CancellationToken cancel = default)
    {
        using DiagnosticActivity? activity = this.diagnostics.Start(tracecontext);

        try
        {
            DataConfigsSnapshot current = this.CurrentSnapshot;

            if(await this.authorizer.IsAdminAsync(token,current,cancel) is false)
            {
                throw new UnauthorizedAccessException("The caller is not authorized to refresh DataConfigs.");
            }

            ConfigurationGeneration generation = new(Guid.NewGuid());

            DataConfigsSnapshot candidate = await this.loader.LoadAsync(generation,cancel);

            this.snapshot = candidate;

            Log.Information("Published DataConfigs generation {Generation} containing {CatalogCount} catalogs.",generation.Value,candidate.CatalogsByName.Count);

            activity?.SetTag("dataconfigs.result","published");

            activity?.SetTag("dataconfigs.catalog_count",candidate.CatalogsByName.Count);

            DataConfigsDiagnostics.SetOk(activity);

            return new RefreshResult(true,generation,candidate.CatalogsByName.Count);
        }
        catch ( Exception exception )
        {
            DataConfigsDiagnostics.SetErr(activity);

            activity?.SetStatus(ActivityStatusCode.Error,exception.GetType().Name);

            throw;
        }
    }

    public Task<ConfigurationGeneration> GetCurrentGenerationAsync(CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        return Task.FromResult(this.CurrentSnapshot.Generation);
    }

    public Task<SnapshotTransferResult> GetSnapshotIfNewerAsync(ConfigurationGeneration knownGeneration , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        DataConfigsSnapshot current = this.CurrentSnapshot;

        if(current.Generation == knownGeneration)
        {
            return Task.FromResult(new SnapshotTransferResult(false,null));
        }

        return Task.FromResult(new SnapshotTransferResult(true,DataConfigsSnapshotTransfer.FromSnapshot(current)));
    }

    private DataConfigsSnapshot CurrentSnapshot => this.snapshot ?? throw new InvalidOperationException("The DataConfigs coordinator has not initialized its snapshot.");
}
