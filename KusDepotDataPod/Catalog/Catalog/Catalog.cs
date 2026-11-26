namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    public String URL { get; private set; } = String.Empty;

    public Catalog()
    {
        try { SetupDiagnostics(); }

        catch ( Exception _ ) { Log.Fatal(_,CatalogFail); Log.CloseAndFlush(); throw; }
    }
}