namespace DataPodServices.DataControl;

internal static class DataControlDataConfigsExtensions
{
    public static async Task<StorageSilo?> ResolveStorageAsync(this IDataControlDataConfigsClient dc , String token , CatalogAccess access , TraceContext? tracecontext , CancellationToken cancel)
    {
        TenantStorageResolution resolution = await dc.ResolveTenantStorageAsync(token,access,tracecontext,cancel);

        return ToStorageSilo(resolution);
    }

    public static async Task<StorageSilo?> ResolveRecoveryStorageAsync(this IDataControlDataConfigsClient dc , String catalogname , CancellationToken cancel)
    {
        TenantStorageResolution resolution = await dc.GetConfiguredStorageForRecoveryAsync(catalogname,cancel);

        return ToStorageSilo(resolution);
    }

    private static StorageSilo? ToStorageSilo(TenantStorageResolution resolution)
    {
        if(resolution.Status is not CatalogResolutionStatus.Authorized || String.IsNullOrWhiteSpace(resolution.CatalogName) || String.IsNullOrWhiteSpace(resolution.ConnectionString)) { return null; }

        return new StorageSilo(resolution.CatalogName,resolution.ConnectionString);
    }
}
