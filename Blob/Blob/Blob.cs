namespace KusDepot.Data;

[StatePersistence(StatePersistence.None)]
internal sealed partial class Blob : Actor , IBlob
{
    public Blob(ActorService actor , ActorId id) : base(actor,id)
    {
        try
        {
            SetupConfiguration(); SetupLogging(); SetupDiagnostics(); SetupTelemetry();
        }
        catch ( Exception _ ) { Log.Fatal(_,BlobFail); Log.CloseAndFlush(); throw; }
    }

    protected override Task<Boolean> OnActivateAsync()
    {
        ETW.Log.OnActivateStart(); Log.Information(Activated); ETW.Log.OnActivateSuccess(); return Task.FromResult(true);
    }

    protected override async Task<Boolean> OnDeactivateAsync()
    {
        ETW.Log.OnDeactivateStart();

        ShutdownTelemetry();

        Log.Information(Deactivated); await Log.CloseAndFlushAsync(); ETW.Log.OnDeactivateSuccess();

        return true;
    }
}