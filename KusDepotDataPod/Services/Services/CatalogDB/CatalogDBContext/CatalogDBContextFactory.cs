using Npgsql;

namespace KusDepot.CatalogDb;

public interface ICatalogDBContextFactory
{
    CatalogDBContext Create(NpgsqlDataSource dataSource);
}

public sealed class CatalogDBContextFactory : ICatalogDBContextFactory
{
    public CatalogDBContext Create(NpgsqlDataSource dataSource)
    {
        var ob = new DbContextOptionsBuilder<CatalogDBContext>()
            .UseNpgsql(dataSource)
            .EnableSensitiveDataLogging(false)
            .EnableDetailedErrors(false);

        return new CatalogDBContext(ob.Options);
    }
}
