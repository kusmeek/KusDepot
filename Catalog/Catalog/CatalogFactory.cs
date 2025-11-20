namespace KusDepot.Data;

public static class CatalogFactory
{
    public static Catalog Create(StatelessServiceContext context)
    {
        return new Catalog(context);
    }
}