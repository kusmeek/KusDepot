namespace DataPodServices.CatalogDB;

public sealed class CatalogPageRequestException : ArgumentException
{
    public CatalogPageRequestException(String message) : base(message) { }
}
