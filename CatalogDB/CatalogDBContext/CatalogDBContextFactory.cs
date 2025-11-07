namespace KusDepot.CatalogDb;

public interface ICatalogDBContextFactory
{
    CatalogDBContext Create(String connectionString);
}

public sealed class CatalogDBContextFactory : ICatalogDBContextFactory
{
    public CatalogDBContext Create(String connectionString)
    {
        var ob = new DbContextOptionsBuilder<CatalogDBContext>()
            .UseNpgsql(connectionString)
            .EnableSensitiveDataLogging(false)
            .EnableDetailedErrors(false);

        return new CatalogDBContext(ob.Options);
    }
}
