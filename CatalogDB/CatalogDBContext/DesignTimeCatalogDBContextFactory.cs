namespace KusDepot.CatalogDb;

public sealed class DesignTimeCatalogDBContextFactory : IDesignTimeDbContextFactory<CatalogDBContext>
{
    public CatalogDBContext CreateDbContext(String[] args)
    {
        var ob = new DbContextOptionsBuilder<CatalogDBContext>()
            .UseNpgsql()
            .EnableSensitiveDataLogging(false)
            .EnableDetailedErrors(false);

        return new CatalogDBContext(ob.Options);
    }
}