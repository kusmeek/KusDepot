namespace DataPodServices.DataControl;

internal sealed class DataControlNotificationAuthorization : IDataControlNotificationAuthorization
{
    private readonly IDataControlDataConfigsClient dataConfigs;

    private readonly IDataControlTransferSessionStore sessions;

    private readonly ITenantTransferStorageContext storage;

    public DataControlNotificationAuthorization(IDataControlDataConfigsClient dataConfigs , IDataControlTransferSessionStore sessions , ITenantTransferStorageContext storage)
    {
        this.dataConfigs = dataConfigs ?? throw new ArgumentNullException(nameof(dataConfigs));

        this.sessions = sessions ?? throw new ArgumentNullException(nameof(sessions));

        this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public async Task<Boolean> CanSubscribeItemAsync(Guid itemid , String token , CancellationToken cancel = default)
    {
        if(itemid == Guid.Empty) { return false; }

        StorageSilo? silo = await ResolveSiloAsync(token,cancel);

        if(String.IsNullOrWhiteSpace(silo?.CatalogName)) { return false; }

        this.storage.Set(silo);

        String catalogName = silo.CatalogName;

        IReadOnlyList<TransferSessionState> states = await this.sessions.LoadSessionsAsync(new TransferSessionQuery { ItemID = itemid , CatalogName = catalogName },cancel);

        foreach(TransferSessionState state in states)
        {
            if(IsSessionInCatalog(state,catalogName)) { return true; }
        }

        return false;
    }

    public async Task<Boolean> CanSubscribeSessionAsync(Guid sessionid , String token , CancellationToken cancel = default)
    {
        if(sessionid == Guid.Empty) { return false; }

        StorageSilo? silo = await ResolveSiloAsync(token,cancel);

        if(String.IsNullOrWhiteSpace(silo?.CatalogName)) { return false; }

        this.storage.Set(silo);

        String catalogName = silo.CatalogName;

        TransferSessionState? state = (await this.sessions.LoadSessionsAsync(new TransferSessionQuery { SessionID = sessionid , CatalogName = catalogName },cancel)).SingleOrDefault();

        return state is not null && IsSessionInCatalog(state,catalogName);
    }

    public async Task<Boolean> CanSubscribeSourceSessionAsync(Guid sourcesessionid , String token , CancellationToken cancel = default)
    {
        if(sourcesessionid == Guid.Empty) { return false; }

        StorageSilo? silo = await ResolveSiloAsync(token,cancel);

        if(String.IsNullOrWhiteSpace(silo?.CatalogName)) { return false; }

        this.storage.Set(silo);

        String catalogName = silo.CatalogName;

        IReadOnlyList<TransferSessionState> states = await this.sessions.LoadSessionsAsync(new TransferSessionQuery
        {
            CatalogName = catalogName,
            SourceSessionID = sourcesessionid,
        },cancel);

        foreach(TransferSessionState state in states)
        {
            if(state.Family is DataControlTransferFamily.Segmented)
            {
                if(TryGetManifestCatalogName(state,out String? manifestCatalogName) && manifestCatalogName is not null && String.Equals(manifestCatalogName,catalogName,Ordinal))
                {
                    DataItemTransferManifest? manifest = DataItemTransferManifest.Parse(state.ManifestJson);

                    if(manifest?.SourceSessionID == sourcesessionid) { return true; }
                }
            }
            else if(state.Family is DataControlTransferFamily.Stream)
            {
                if(TryGetManifestCatalogName(state,out String? manifestCatalogName) && manifestCatalogName is not null && String.Equals(manifestCatalogName,catalogName,Ordinal))
                {
                    DataStreamTransferManifest? manifest = DataStreamTransferManifest.Parse(state.ManifestJson);

                    if(manifest?.SourceSessionID == sourcesessionid) { return true; }
                }
            }
        }

        return false;
    }

    private async Task<StorageSilo?> ResolveSiloAsync(String token , CancellationToken cancel)
    {
        if(String.IsNullOrWhiteSpace(token)) { return null; }

        return await this.dataConfigs.ResolveStorageAsync(token,CatalogAccess.Read,null,cancel);
    }

    private static Boolean IsSessionInCatalog(TransferSessionState state , String catalogName)
    {
        return TryGetManifestCatalogName(state,out String? manifestCatalogName) && manifestCatalogName is not null && String.Equals(manifestCatalogName,catalogName,Ordinal);
    }

    private static Boolean TryGetManifestCatalogName(TransferSessionState state , out String? catalogName)
    {
        catalogName = null;

        try
        {
            if(state.Family is DataControlTransferFamily.Segmented)
            {
                DataItemTransferManifest? manifest = DataItemTransferManifest.Parse(state.ManifestJson);

                if(manifest is null || manifest.SessionID != state.SessionID || manifest.ItemID != state.ItemID || String.IsNullOrWhiteSpace(manifest.CatalogName)) { return false; }

                catalogName = manifest.CatalogName;

                return true;
            }

            if(state.Family is DataControlTransferFamily.Stream)
            {
                DataStreamTransferManifest? manifest = DataStreamTransferManifest.Parse(state.ManifestJson);

                if(manifest is null || manifest.SessionID != state.SessionID || manifest.ItemID != state.ItemID || String.IsNullOrWhiteSpace(manifest.CatalogName)) { return false; }

                catalogName = manifest.CatalogName;

                return true;
            }
        }
        catch ( Exception )
        {
            return false;
        }

        return false;
    }
}