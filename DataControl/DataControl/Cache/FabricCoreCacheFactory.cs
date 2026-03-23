namespace KusDepot.Data.Services.CoreCache;

public class FabricCoreCacheFactory : ICoreCacheFactory
{
    public KusDepot.Data.Services.CoreCache.ICoreCache Create()
    {
        return new FabricCacheAdapter(

            ActorProxy.Create<KusDepot.Data.ICoreCache>(new(Guid.NewGuid()),ServiceLocators.CoreCacheService));
    }
}