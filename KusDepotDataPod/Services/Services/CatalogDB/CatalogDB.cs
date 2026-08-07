using KusDepot.CatalogDb;
using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService : Grain , ICatalogDB
{
    public CatalogDBService()
    {
        try
        {
            SetupConfiguration(); SetupLogging(); SetupDiagnostics(); SetupTelemetry();
        }
        catch ( Exception _ ) { Log.Fatal(_,CatalogDBFail); Log.CloseAndFlush(); throw; }
    }

    private readonly CatalogDBContextFactory ctxfactory = new CatalogDBContextFactory();

    public override async Task OnActivateAsync(CancellationToken cancel)
    {
        try
        {
            await InitializeReadyDatabase(cancel);

            Log.Information(ActivatedCatalog,GetActorID());
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail,GetActorID()); }
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason , CancellationToken cancel)
    {
        try
        {
            ShutdownTelemetry(); Log.Information(Deactivated,GetActorID());
        
            await Log.CloseAndFlushAsync();
        }
        catch ( Exception _ ) { Log.Error(_,DeactivateFail,GetActorID()); }
    }
}