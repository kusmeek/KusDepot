namespace DataPodServices.DataControl;

internal sealed class PublishedTransferRequestContextMetadata;

internal sealed class PublishedTransferRequestContextMiddleware
{
    private readonly RequestDelegate Next;

    public PublishedTransferRequestContextMiddleware(RequestDelegate next) => this.Next = next;

    public Task InvokeAsync(HttpContext context , PublishedTransferRequestContext requestcontext)
    {
        if(context.GetEndpoint()?.Metadata.GetMetadata<PublishedTransferRequestContextMetadata>() is not null)
        {
            String authorization = context.Request.Headers.Authorization.ToString();

            requestcontext.SetBearerToken(authorization.StartsWith("Bearer ",Ordinal) ? authorization[7..] : null);
        }

        return this.Next(context);
    }
}
