namespace DataPodServices.CoreCache;

public class OrleansCoreCacheFactory : ICoreCacheFactory
{
    private readonly IGrainFactory GrainFactory;

    public OrleansCoreCacheFactory(IGrainFactory grainfactory) { GrainFactory = grainfactory; }

    public KusDepot.Data.Services.CoreCache.ICoreCache Create()
    {
        return new OrleansCoreCacheAdapter(

            GrainFactory.GetGrain<ICoreCache>(Guid.NewGuid().ToStringInvariant()!));
    }
}