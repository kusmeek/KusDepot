namespace KusDepot.Data.Services.CoreCache;

/**<include file='CacheManager.xml' path='CacheManager/class[@name="CacheManager"]/main/*'/>*/
internal sealed class CacheManager : ICacheManager
{
    /**<include file='CacheManager.xml' path='CacheManager/class[@name="CacheManager"]/field[@name="Queue"]/*'/>*/
    private readonly CacheQueue Queue;

    /**<include file='CacheManager.xml' path='CacheManager/class[@name="CacheManager"]/field[@name="CoreCache"]/*'/>*/
    private readonly ICoreCache CoreCache;

    /**<include file='CacheManager.xml' path='CacheManager/class[@name="CacheManager"]/constructor[@name="Constructor"]/*'/>*/
    public CacheManager(CacheQueue queue , ICoreCacheFactory corecachefactory)
    {
        Queue = queue; CoreCache = corecachefactory.Create();
    }

    ///<inheritdoc/>
    public Task<Boolean> Delete(String? id , String? traceid , String? spanid)
    {
        return CoreCache.Delete(id,traceid,spanid);
    }

    ///<inheritdoc/>
    public Task<Boolean?> Exists(String? id , String? traceid , String? spanid)
    {
        return CoreCache.Exists(id,traceid,spanid);
    }

    ///<inheritdoc/>
    public Task<String?> Get(String? id , String? traceid , String? spanid)
    {
        return CoreCache.Get(id,traceid,spanid);
    }

    ///<inheritdoc/>
    public Task<Boolean> Store(String? id , String? it , String? traceid , String? spanid)
    {
        return CoreCache.Store(id,it,traceid,spanid);
    }

    ///<inheritdoc/>
    public void EnqueueItem(String? id , String? it , String? dt , String? ds)
    {
        _ = Queue.ItemChannel.Writer.TryWrite(new CacheItemRequest(id,it,dt,ds,DateTimeOffset.Now,0));
    }
}