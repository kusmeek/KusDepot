namespace DataPodServices.DataControl;

internal interface IPublishedTransferRequestContext
{
    Boolean TryGetBearerToken(out String? token);
}

internal sealed class PublishedTransferRequestContext : IPublishedTransferRequestContext
{
    private String? BearerToken;

    internal void SetBearerToken(String? token)
    {
        this.BearerToken = String.IsNullOrWhiteSpace(token) ? null : token;
    }

    public bool TryGetBearerToken(out String? token)
    {
        token = this.BearerToken;

        return String.IsNullOrEmpty(token) is false;
    }
}
