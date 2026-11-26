namespace DataPodServices.CatalogDB;

public static class CatalogPageSize
{
    public const Int32 Default = 50;

    public const Int32 Maximum = 100;

    public static Int32 Resolve(Int32? requested)
    {
        if(requested is null) { return Default; }

        if(requested < 1 || requested > Maximum)
        {
            throw new CatalogPageRequestException($"CatalogPageSize must be between 1 and {Maximum}.");
        }

        return requested.Value;
    }
}
