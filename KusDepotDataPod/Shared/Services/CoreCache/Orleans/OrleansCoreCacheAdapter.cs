namespace DataPodServices.CoreCache;

public class OrleansCoreCacheAdapter : KusDepot.Data.Services.CoreCache.ICoreCache
{
    private readonly ICoreCache Grain;

    public OrleansCoreCacheAdapter(ICoreCache grain)
    {
        Grain = grain;
    }

    public Task<Boolean> Delete(String? id , String? traceid , String? spanid)
    {
        return Grain.Delete(id,traceid,spanid);
    }

    public Task<Boolean?> Exists(String? id , String? traceid , String? spanid)
    {
        return Grain.Exists(id,traceid,spanid);
    }

    public Task<String?> Get(String? id , String? traceid , String? spanid)
    {
        return Grain.Get(id,traceid,spanid);
    }

    public Task<Boolean> Store(String? id , String? it , String? traceid , String? spanid)
    {
        return Grain.Store(id,it,traceid,spanid);
    }
}