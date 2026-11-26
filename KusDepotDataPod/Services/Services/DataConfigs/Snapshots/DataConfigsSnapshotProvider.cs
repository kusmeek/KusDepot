namespace DataPodServices.DataConfigs;

internal interface IDataConfigsSnapshotProvider
{
    Boolean IsReady { get; }

    DataConfigsSnapshot Current { get; }

    Boolean TryReplace(DataConfigsSnapshot snapshot);
}

internal sealed class DataConfigsSnapshotProvider : IDataConfigsSnapshotProvider
{
    private readonly OidcMetadataManagerCache metadataManagers;

    private DataConfigsSnapshot? current;

    public DataConfigsSnapshotProvider(OidcMetadataManagerCache metadataManagers)
    {
        this.metadataManagers = metadataManagers;
    }

    public Boolean IsReady => Volatile.Read(ref this.current) is not null;

    public DataConfigsSnapshot Current => Volatile.Read(ref this.current) ?? throw new InvalidOperationException("The DataConfigs snapshot is not ready on this Silo.");

    public Boolean TryReplace(DataConfigsSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        DataConfigsSnapshot? existing = Volatile.Read(ref this.current);

        if(existing?.Generation == snapshot.Generation)
        {
            return false;
        }

        Interlocked.Exchange(ref this.current,snapshot);

        this.metadataManagers.RemoveTenantsNotIn(snapshot);

        return true;
    }
}
