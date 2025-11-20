namespace KusDepot.Data;

[StatePersistence(StatePersistence.Persisted)]
internal sealed partial class CoreCache : Actor , ICoreCache
{
    public CoreCache(ActorService actor , ActorId id) : base(actor,id)
    {
        try
        {
            SetupConfiguration(); SetupLogging(); SetupDiagnostics(); SetupTelemetry();
        }
        catch ( Exception _ ) { Log.Fatal(_,CoreCacheFail); Log.CloseAndFlush(); throw; }
    }

    protected override async Task<Boolean> OnActivateAsync()
    {
        try
        {
            ETW.Log.OnActivateStart();

            RedisAccess.Initialize(() => this.Config?[RedisConnectKey]);

            if(await RedisAccess.ConnectAsync() is false) { return false; }

            Log.Information(Activated); ETW.Log.OnActivateSuccess();

            return true;
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail); ETW.Log.OnActivateError(_.Message); return false; }
    }

    protected override async Task<Boolean> OnDeactivateAsync()
    {
        try
        {
            ETW.Log.OnDeactivateStart();

            await RedisAccess.CloseAsync(); ShutdownTelemetry();

            Log.Information(Deactivated); await Log.CloseAndFlushAsync(); ETW.Log.OnDeactivateSuccess();

            return true;
        }
        catch ( Exception _ ) { Log.Error(_,DeactivateFail); return false; }
    }
}