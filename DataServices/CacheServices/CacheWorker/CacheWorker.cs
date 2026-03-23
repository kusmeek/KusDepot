using static KusDepot.Data.Services.CoreCache.Strings;

namespace KusDepot.Data.Services.CoreCache;

/**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/main/*'/>*/
internal sealed class CacheWorker : BackgroundService
{
    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/field[@name="CoreCache"]/*'/>*/
    private ICoreCache? CoreCache;

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/field[@name="Queue"]/*'/>*/
    private readonly CacheQueue Queue;

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/field[@name="Options"]/*'/>*/
    private readonly CacheWorkerOptions Options;

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/field[@name="CoreCacheFactory"]/*'/>*/
    private readonly ICoreCacheFactory CoreCacheFactory;

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/field[@name="ProcessPolicy"]/*'/>*/
    private readonly AsyncRetryPolicy<Boolean> ProcessPolicy;

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/field[@name="ItemBlock"]/*'/>*/
    private readonly ActionBlock<CacheItemRequest> ItemBlock;

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/constructor[@name="Constructor"]/*'/>*/
    public CacheWorker(ICoreCacheFactory corecachefactory , CacheQueue queue , IOptions<CacheWorkerOptions> options)
    {
        this.CoreCacheFactory = corecachefactory; this.Queue = queue; this.Options = options.Value;

        ExecutionDataflowBlockOptions processoptions = new()
        {
            MaxDegreeOfParallelism = Options.MaxConcurrency,

            BoundedCapacity = Options.Capacity , EnsureOrdered = false
        };

        ProcessPolicy = Policy<Boolean>
            .Handle<Exception>().WaitAndRetryAsync(Options.MaxRetries,
                a => TimeSpan.FromMilliseconds(Options.RetryBaseDelayMilliseconds * Math.Pow(2,a)));

        ItemBlock = new ActionBlock<CacheItemRequest>(this.ProcessItemRequestAsync,processoptions);
    }

    ///<inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken cancel)
    {
        try { await ReadItemChannelAsync(cancel); }

        catch ( OperationCanceledException ) {}

        finally { ItemBlock.Complete(); await ItemBlock.Completion; }
    }

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/method[@name="ReadItemChannelAsync"]/*'/>*/
    private async Task ReadItemChannelAsync(CancellationToken cancel)
    {
        try
        {
            await foreach(CacheItemRequest _ in Queue.ItemChannel.Reader.ReadAllAsync(cancel))
            {
                await ItemBlock.SendAsync(_,cancel);
            }
        }
        catch ( OperationCanceledException ) {}
    }

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/method[@name="ProcessItemRequestAsync"]/*'/>*/
    private async Task ProcessItemRequestAsync(CacheItemRequest request)
    {
        String? id = request.ID;

        if(DateTimeOffset.Now - request.EnqueuedAt > TimeSpan.FromMinutes(Options.MaxAgeMinutes))
        {
            Log.Error(AddCacheFailID,id); return;
        }

        try
        {
            ResolveCoreCache();

            if(CoreCache is not null) { await ExecuteItemCoreCacheAsync(id,request); }

            Log.Information(AddCacheSuccessID,id);
        }
        catch ( Exception _ )
        {
            if(DateTimeOffset.Now - request.EnqueuedAt > TimeSpan.FromMinutes(Options.MaxAgeMinutes))
            {
                Log.Error(_,AddCacheFailID,id); return;
            }

            try
            {
                await Task.Delay(GetRandomDelay());

                CacheItemRequest retry = request with { RetryCount = request.RetryCount + 1 };

                await ItemBlock.SendAsync(retry);
            }
            catch ( Exception __ )
            {
                Log.Error(__,AddCacheFailID,id);
            }
        }
    }

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/method[@name="ExecuteItemCoreCacheAsync"]/*'/>*/
    private Task<Boolean> ExecuteItemCoreCacheAsync(String? id , CacheItemRequest request)
    {
        TimeSpan t = Options.CacheTimeoutSeconds > 0
            ? TimeSpan.FromSeconds(Options.CacheTimeoutSeconds)
            : Timeout.InfiniteTimeSpan;

        return ProcessPolicy.ExecuteAsync(async () =>
        {
            Task<Boolean> s = CoreCache!.Store(id,request.Item,request.TraceId,request.SpanId);

            if(t != Timeout.InfiniteTimeSpan) { await s.WaitAsync(t); }

            else { await s; }

            return true;
        });
    }

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/method[@name="GetRandomDelay"]/*'/>*/
    private TimeSpan GetRandomDelay()
    {
        return TimeSpan.FromMilliseconds(RandomNumberGenerator.GetInt32(Options.MinJitterMilliseconds,Options.MaxJitterMilliseconds));
    }

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/method[@name="ResolveCoreCache"]/*'/>*/
    private void ResolveCoreCache()
    {
        try
        {
            if(CoreCache is null)
            {
                CoreCache = CoreCacheFactory.Create();
            }
        }
        catch ( Exception _ ) { Log.Error(_,ResolveCacheFail); }
    }

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/field[@name="CacheProcessFail"]/*'/>*/
    private const String CacheProcessFail = "Cache Processing Failed";

    /**<include file='CacheWorker.xml' path='CacheWorker/class[@name="CacheWorker"]/field[@name="ResolveCacheFail"]/*'/>*/
    private const String ResolveCacheFail = "Resolve Cache Failed";
}