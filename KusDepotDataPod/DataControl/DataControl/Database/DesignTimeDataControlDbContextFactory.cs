namespace DataPodServices.DataControl;

public sealed class DesignTimeDataControlDbContextFactory : IDesignTimeDbContextFactory<DataControlDbContext>
{
    public DataControlDbContext CreateDbContext(String[] args)
    {
        DbContextOptionsBuilder<DataControlDbContext> builder = new();

        builder.UseNpgsql();
        builder.EnableSensitiveDataLogging(false);
        builder.EnableDetailedErrors(false);

        return new DataControlDbContext(builder.Options);
    }
}
