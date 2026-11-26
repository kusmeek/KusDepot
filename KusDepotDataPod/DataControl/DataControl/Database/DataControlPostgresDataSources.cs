namespace DataPodServices.DataControl;

internal sealed class DataControlPostgresDataSources : IAsyncDisposable
{
    public NpgsqlDataSource DataControl { get; }

    public NpgsqlDataSource Admin { get; }

    public DataControlPostgresDataSources(NpgsqlDataSource dataControl , NpgsqlDataSource admin)
    {
        this.DataControl = dataControl ?? throw new ArgumentNullException(nameof(dataControl));

        this.Admin = admin ?? throw new ArgumentNullException(nameof(admin));
    }

    public async ValueTask DisposeAsync()
    {
        await this.DataControl.DisposeAsync();

        await this.Admin.DisposeAsync();
    }
}
