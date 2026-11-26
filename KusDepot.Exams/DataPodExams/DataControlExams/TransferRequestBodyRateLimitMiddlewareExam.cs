using DataPodServices.DataControl;

namespace KusDepot.DataPodExams;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class TransferRequestBodyRateLimitMiddlewareExam
{
    [Test]
    public async Task MarkedEndpoint_RateLimitsRequestBody_AndRestoresOriginalBody()
    {
        Byte[] payload = new Byte[32];
        using MemoryStream source = new(payload,false);
        DefaultHttpContext context = CreateContext(source,new EndpointMetadataCollection(new TransferRequestBodyRateLimitMetadata()));
        Boolean downstreamSawWrapper = false;
        RequestDelegate next = async request =>
        {
            downstreamSawWrapper = !ReferenceEquals(request.Request.Body,source);

            await request.Request.Body.CopyToAsync(Stream.Null,request.RequestAborted);
        };
        TransferRequestBodyRateLimitMiddleware middleware = new(next);
        Stopwatch stopwatch = Stopwatch.StartNew();

        await middleware.InvokeAsync(context,new TransferPayloadRateLimiters(320,0));

        stopwatch.Stop();

        Assert.That(downstreamSawWrapper, Is.True);
        Assert.That(stopwatch.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(75)));
        Assert.That(context.Request.Body, Is.SameAs(source));
        Assert.That(source.Position, Is.EqualTo(source.Length));
    }

    [Test]
    public async Task UnmarkedEndpoint_LeavesRequestBodyUnchanged()
    {
        using MemoryStream source = new(new Byte[] { 1 },false);
        DefaultHttpContext context = CreateContext(source,new EndpointMetadataCollection());
        Boolean downstreamSawOriginal = false;
        RequestDelegate next = request =>
        {
            downstreamSawOriginal = ReferenceEquals(request.Request.Body,source);

            return Task.CompletedTask;
        };
        TransferRequestBodyRateLimitMiddleware middleware = new(next);

        await middleware.InvokeAsync(context,new TransferPayloadRateLimiters(1,0));

        Assert.That(downstreamSawOriginal, Is.True);
        Assert.That(context.Request.Body, Is.SameAs(source));
    }

    private static DefaultHttpContext CreateContext(Stream body , EndpointMetadataCollection metadata)
    {
        DefaultHttpContext context = new();

        context.Request.Body = body;
        context.SetEndpoint(new Endpoint(_ => Task.CompletedTask,metadata,"test"));

        return context;
    }
}
