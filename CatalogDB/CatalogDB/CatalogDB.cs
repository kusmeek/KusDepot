namespace KusDepot.Data;

[StatePersistence(StatePersistence.None)]
internal sealed partial class CatalogDB : Actor , ICatalogDB
{
    public CatalogDB(ActorService actor , ActorId id) : base(actor,id)
    {
        try
        {
            SetupConfiguration(); SetupLogging(); SetupDiagnostics(); SetupTelemetry();
        }
        catch ( Exception _ ) { Log.Fatal(_,CatalogDBFail,GetActorID()); Log.CloseAndFlush(); throw; }
    }

    private readonly CatalogDBContextFactory ctxfactory = new CatalogDBContextFactory();

    protected override async Task<Boolean> OnActivateAsync()
    {
        try
        {
            await InitializeReadyDatabase();

            Log.Information(ActivatedCatalog,GetActorID()); ETW.Log.OnActivateSuccess(Activated,GetActorID()); return true;
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail,GetActorID()); ETW.Log.OnActivateError(_.Message,GetActorID()); return false; }
    }

    protected override async Task<Boolean> OnDeactivateAsync()
    {
        ETW.Log.OnDeactivateStart(GetActorID()); ShutdownTelemetry(); Log.Information(Deactivated,GetActorID());
        
        await Log.CloseAndFlushAsync(); ETW.Log.OnDeactivateSuccess(GetActorID()); return true;
    }
}