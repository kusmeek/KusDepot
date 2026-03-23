namespace KusDepot.Data.Services.CoreCache;

/**<include file='CacheWorkerOptions.xml' path='CacheWorkerOptions/class[@name="CacheWorkerOptions"]/main/*'/>*/
internal sealed class CacheWorkerOptions
{
    /**<include file='CacheWorkerOptions.xml' path='CacheWorkerOptions/class[@name="CacheWorkerOptions"]/property[@name="CacheTimeoutSeconds"]/*'/>*/
    public Int32 CacheTimeoutSeconds        { get; init; } = 5;

    /**<include file='CacheWorkerOptions.xml' path='CacheWorkerOptions/class[@name="CacheWorkerOptions"]/property[@name="Capacity"]/*'/>*/
    public Int32 Capacity                   { get; init; } = 100;

    /**<include file='CacheWorkerOptions.xml' path='CacheWorkerOptions/class[@name="CacheWorkerOptions"]/property[@name="MaxAgeMinutes"]/*'/>*/
    public Int32 MaxAgeMinutes              { get; init; } = 20;

    /**<include file='CacheWorkerOptions.xml' path='CacheWorkerOptions/class[@name="CacheWorkerOptions"]/property[@name="MaxConcurrency"]/*'/>*/
    public Int32 MaxConcurrency             { get; init; } = 4;

    /**<include file='CacheWorkerOptions.xml' path='CacheWorkerOptions/class[@name="CacheWorkerOptions"]/property[@name="MaxJitterMilliseconds"]/*'/>*/
    public Int32 MaxJitterMilliseconds      { get; init; } = 60000;

    /**<include file='CacheWorkerOptions.xml' path='CacheWorkerOptions/class[@name="CacheWorkerOptions"]/property[@name="MaxRetries"]/*'/>*/
    public Int32 MaxRetries                 { get; init; } = 3;

    /**<include file='CacheWorkerOptions.xml' path='CacheWorkerOptions/class[@name="CacheWorkerOptions"]/property[@name="MinJitterMilliseconds"]/*'/>*/
    public Int32 MinJitterMilliseconds      { get; init; } = 2000;

    /**<include file='CacheWorkerOptions.xml' path='CacheWorkerOptions/class[@name="CacheWorkerOptions"]/property[@name="RetryBaseDelayMilliseconds"]/*'/>*/
    public Int32 RetryBaseDelayMilliseconds { get; init; } = 20000;
}