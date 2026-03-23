namespace KusDepot.Data.Services.CoreCache;

public class FabricCacheAdapter : KusDepot.Data.Services.CoreCache.ICoreCache
{
    private readonly KusDepot.Data.ICoreCache Actor;

    public FabricCacheAdapter(KusDepot.Data.ICoreCache actor)
    {
        Actor = actor;
    }

    public Task<Boolean> Delete(String? id , String? traceid , String? spanid)
    {
        return Actor.Delete(id,traceid,spanid);
    }

    public Task<Boolean?> Exists(String? id , String? traceid , String? spanid)
    {
        return Actor.Exists(id,traceid,spanid);
    }

    public Task<String?> Get(String? id , String? traceid , String? spanid)
    {
        return Actor.Get(id,traceid,spanid);
    }

    public Task<Boolean> Store(String? id , String? it , String? traceid , String? spanid)
    {
        return Actor.Store(id,it,traceid,spanid);
    }
}