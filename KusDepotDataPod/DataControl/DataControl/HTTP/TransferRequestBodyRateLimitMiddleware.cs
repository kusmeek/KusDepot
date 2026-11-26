namespace DataPodServices.DataControl;

internal sealed class TransferRequestBodyRateLimitMetadata;

internal sealed class TransferRequestBodyRateLimitMiddleware
{
    private readonly RequestDelegate Next;

    public TransferRequestBodyRateLimitMiddleware(RequestDelegate next) => this.Next = next;

    public async Task InvokeAsync(HttpContext context , TransferPayloadRateLimiters ratelimiters)
    {
        if(context.GetEndpoint()?.Metadata.GetMetadata<TransferRequestBodyRateLimitMetadata>() is null || ratelimiters.Upload is not AsyncByteRateLimiter limiter)
        {
            await this.Next(context);

            return;
        }

        Stream original = context.Request.Body;

        using RateLimitedReadStream limited = new(original,limiter,leaveopen:true);

        context.Request.Body = limited;

        try { await this.Next(context); }

        finally { context.Request.Body = original; }
    }
}
