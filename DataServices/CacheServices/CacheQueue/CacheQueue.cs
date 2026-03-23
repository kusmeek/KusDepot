namespace KusDepot.Data.Services.CoreCache;

/**<include file='CacheQueue.xml' path='CacheQueue/class[@name="CacheQueue"]/main/*'/>*/
internal sealed class CacheQueue
{
    /**<include file='CacheQueue.xml' path='CacheQueue/class[@name="CacheQueue"]/property[@name="ItemChannel"]/*'/>*/
    public Channel<CacheItemRequest> ItemChannel { get; }

    /**<include file='CacheQueue.xml' path='CacheQueue/class[@name="CacheQueue"]/constructor[@name="Constructor"]/*'/>*/
    public CacheQueue() { ItemChannel = Channel.CreateUnbounded<CacheItemRequest>(); }
}