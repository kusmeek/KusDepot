namespace DataPodServices.DataControl;

internal sealed class PublishedTransferRequestContextMiddleware
{
    private static readonly PathString[] PublishedTransferPaths =
    {
        new("/GetTransferSegment"),
        new("/OpenGetTransfer"),
        new("/ReOpenGetTransfer"),
    };

    private readonly RequestDelegate Next;

    public PublishedTransferRequestContextMiddleware(RequestDelegate next) => this.Next = next;

    public Task InvokeAsync(HttpContext context , PublishedTransferRequestContext requestcontext)
    {
        if(PublishedTransferPaths.Contains(context.Request.Path))
        {
            String authorization = context.Request.Headers.Authorization.ToString();

            requestcontext.SetBearerToken(authorization.StartsWith("Bearer ",Ordinal) ? authorization[7..] : null);
        }

        return this.Next(context);
    }
}
