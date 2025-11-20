namespace KusDepot.Data;

public sealed partial class Catalog
{
    public readonly String URL;

    private readonly StatelessServiceContext Context;

    public Catalog(StatelessServiceContext context)
    {
        try { this.Context = context; SetupDiagnostics(); this.URL = GetURL(); }

        catch ( Exception _ ) { Log.Fatal(_,CatalogFail); Log.CloseAndFlush(); throw; }
    }
}