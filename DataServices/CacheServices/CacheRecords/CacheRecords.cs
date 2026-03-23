namespace KusDepot.Data.Services.CoreCache;

/**<include file='CacheRecords.xml' path='CacheRecords/record[@name="CacheItemRequest"]/main/*'/>*/
internal sealed record CacheItemRequest
{
    /**<include file='CacheRecords.xml' path='CacheRecords/record[@name="CacheItemRequest"]/field[@name="ID"]/*'/>*/
    public String? ID { get; init; }

    /**<include file='CacheRecords.xml' path='CacheRecords/record[@name="CacheItemRequest"]/field[@name="Item"]/*'/>*/
    public String? Item { get; init; }

    /**<include file='CacheRecords.xml' path='CacheRecords/record[@name="CacheItemRequest"]/field[@name="TraceId"]/*'/>*/
    public String? TraceId { get; init; }

    /**<include file='CacheRecords.xml' path='CacheRecords/record[@name="CacheItemRequest"]/field[@name="SpanId"]/*'/>*/
    public String? SpanId { get; init; }

    /**<include file='CacheRecords.xml' path='CacheRecords/record[@name="CacheItemRequest"]/field[@name="EnqueuedAt"]/*'/>*/
    public DateTimeOffset EnqueuedAt { get; init; }

    /**<include file='CacheRecords.xml' path='CacheRecords/record[@name="CacheItemRequest"]/field[@name="RetryCount"]/*'/>*/
    public Int32 RetryCount { get; init; }

    /**<include file='CacheRecords.xml' path='CacheRecords/record[@name="CacheItemRequest"]/constructor[@name="Constructor"]/*'/>*/
    public CacheItemRequest(String? id , String? item , String? traceid , String? spanid , DateTimeOffset enqueuedat , Int32 retrycount)
    {
        ID = id; Item = item; TraceId = traceid; SpanId = spanid; EnqueuedAt = enqueuedat; RetryCount = retrycount;
    }
}