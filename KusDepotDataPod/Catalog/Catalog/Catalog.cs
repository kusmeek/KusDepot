namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    public String URL { get; private set; }

    public Catalog()
    {
        try { this.URL = null!; SetURL(); SetupDiagnostics(); SetupTelemetry(); }

        catch ( Exception _ ) { Log.Fatal(_,CatalogFail); Log.CloseAndFlush(); throw; }
    }
}