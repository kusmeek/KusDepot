namespace DataPodServices.DataConfigs;

[StatelessWorker]
internal sealed class DataConfigsReaderGrain : Grain , IDataConfigsReaderGrain , IDataConfigsRecoveryGrain
{
    private readonly IDataConfigsSnapshotProvider snapshots;
    private readonly DataConfigsAuthorizer authorizer;
    private readonly DataConfigsDiagnostics diagnostics;

    public DataConfigsReaderGrain(IDataConfigsSnapshotProvider snapshots , DataConfigsAuthorizer authorizer , DataConfigsDiagnostics diagnostics)
    {
        this.snapshots = snapshots; this.authorizer = authorizer; this.diagnostics = diagnostics;
    }

    public async Task<CatalogResolution> ResolveCatalogAsync(String token , CatalogAccess access , TraceContext? tracecontext , CancellationToken cancel = default)
    {
        using DiagnosticActivity? activity = this.diagnostics.Start(tracecontext);

        try
        {
            DataConfigsSnapshot snapshot = this.snapshots.Current;

            if(DataConfigsAuthorizer.TryGetTokenIdentity(token,out SnapshotCatalogIdentity identity) is false)
            {
                activity?.SetTag("dataconfigs.result","forbidden");

                DataConfigsDiagnostics.SetOk(activity);

                return new CatalogResolution(CatalogResolutionStatus.Forbidden,null,snapshot.Generation);
            }

            if(snapshot.TryGetCatalog(identity,out SnapshotCatalog catalog) is false)
            {
                activity?.SetTag("dataconfigs.result","not_configured");

                DataConfigsDiagnostics.SetOk(activity);

                return new CatalogResolution(CatalogResolutionStatus.NotConfigured,null,snapshot.Generation);
            }

            Boolean authorized = await this.authorizer.IsCatalogAuthorizedAsync(token,catalog,access,snapshot,cancel);

            activity?.SetTag("dataconfigs.result",authorized ? "authorized" : "forbidden");

            DataConfigsDiagnostics.SetOk(activity);

            return authorized
                ? new CatalogResolution(CatalogResolutionStatus.Authorized,catalog.CatalogName,snapshot.Generation)
                : new CatalogResolution(CatalogResolutionStatus.Forbidden,null,snapshot.Generation);
        }
        catch ( Exception exception )
        {
            DataConfigsDiagnostics.SetErr(activity);

            activity?.SetStatus(ActivityStatusCode.Error,exception.GetType().Name);

            throw;
        }
    }

    public async Task<TenantStorageResolution> ResolveTenantStorageAsync(String token , CatalogAccess access , TraceContext? tracecontext , CancellationToken cancel = default)
    {
        using DiagnosticActivity? activity = this.diagnostics.Start(tracecontext);

        try
        {
            DataConfigsSnapshot snapshot = this.snapshots.Current;

            if(DataConfigsAuthorizer.TryGetTokenIdentity(token,out SnapshotCatalogIdentity identity) is false)
            {
                activity?.SetTag("dataconfigs.result","forbidden");

                DataConfigsDiagnostics.SetOk(activity);

                return new TenantStorageResolution(CatalogResolutionStatus.Forbidden,null,null,snapshot.Generation);
            }

            if(snapshot.TryGetCatalog(identity,out SnapshotCatalog catalog) is false)
            {
                activity?.SetTag("dataconfigs.result","not_configured");

                DataConfigsDiagnostics.SetOk(activity);

                return new TenantStorageResolution(CatalogResolutionStatus.NotConfigured,null,null,snapshot.Generation);
            }

            Boolean authorized = await this.authorizer.IsCatalogAuthorizedAsync(token,catalog,access,snapshot,cancel);

            activity?.SetTag("dataconfigs.result",authorized ? "authorized" : "forbidden");

            DataConfigsDiagnostics.SetOk(activity);

            return authorized
                ? new TenantStorageResolution(CatalogResolutionStatus.Authorized,catalog.CatalogName,catalog.ConnectionString,snapshot.Generation)
                : new TenantStorageResolution(CatalogResolutionStatus.Forbidden,null,null,snapshot.Generation);
        }
        catch ( Exception exception )
        {
            DataConfigsDiagnostics.SetErr(activity);

            activity?.SetStatus(ActivityStatusCode.Error,exception.GetType().Name);

            throw;
        }
    }

    public Task<TenantStorageResolution> GetConfiguredStorageForRecoveryAsync(String catalogName , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        DataConfigsSnapshot snapshot = this.snapshots.Current;

        return Task.FromResult(snapshot.TryGetCatalog(catalogName, out SnapshotCatalog catalog)
            ? new TenantStorageResolution(CatalogResolutionStatus.Authorized,catalog.CatalogName,catalog.ConnectionString,snapshot.Generation)
            : new TenantStorageResolution(CatalogResolutionStatus.NotConfigured,null,null,snapshot.Generation));
    }

    public Task<ConfiguredCatalogNames> GetConfiguredCatalogNamesAsync(CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        DataConfigsSnapshot snapshot = this.snapshots.Current;

        return Task.FromResult(new ConfiguredCatalogNames(snapshot.CatalogsByName.Keys.OrderBy(name => name,StringComparer.Ordinal).ToArray(),snapshot.Generation));
    }

    public async Task<AdminCheckResult> CheckAdminAsync(String token , TraceContext? tracecontext , CancellationToken cancel = default)
    {
        using DiagnosticActivity? activity = this.diagnostics.Start(tracecontext);

        try
        {
            DataConfigsSnapshot snapshot = this.snapshots.Current;

            Boolean authorized = await this.authorizer.IsAdminAsync(token,snapshot,cancel);

            activity?.SetTag("dataconfigs.result",authorized ? "authorized" : "forbidden");

            DataConfigsDiagnostics.SetOk(activity);

            return new AdminCheckResult(authorized ? AdminCheckStatus.Authorized : AdminCheckStatus.Forbidden,snapshot.Generation);
        }
        catch ( Exception exception )
        {
            DataConfigsDiagnostics.SetErr(activity);

            activity?.SetStatus(ActivityStatusCode.Error,exception.GetType().Name);

            throw;
        }
    }

    public async Task<CatalogSnapshotResult> GetCatalogSnapshotAsync(String token , TraceContext? tracecontext , CancellationToken cancel = default)
    {
        using DiagnosticActivity? activity = this.diagnostics.Start(tracecontext);

        try
        {
            DataConfigsSnapshot snapshot = this.snapshots.Current;

            if(await this.authorizer.IsAdminAsync(token,snapshot,cancel) is false)
            {
                throw new UnauthorizedAccessException("The caller is not authorized to inspect catalog configuration.");
            }

            CatalogSnapshotEntry[] catalogs = snapshot.CatalogsByName.Values
                .Select(static catalog => new CatalogSnapshotEntry
                {
                    CatalogName = catalog.CatalogName,
                    TenantId    = catalog.Identity.TenantId.ToString(),
                    AppClientId = catalog.Identity.AppClientId.ToString()
                })
                .OrderBy(static catalog => catalog.CatalogName,StringComparer.Ordinal)
                .ToArray();

            activity?.SetTag("dataconfigs.result","authorized");

            DataConfigsDiagnostics.SetOk(activity);

            return new CatalogSnapshotResult(catalogs,snapshot.Generation);
        }
        catch ( Exception exception )
        {
            DataConfigsDiagnostics.SetErr(activity);

            activity?.SetStatus(ActivityStatusCode.Error,exception.GetType().Name);

            throw;
        }
    }
}
