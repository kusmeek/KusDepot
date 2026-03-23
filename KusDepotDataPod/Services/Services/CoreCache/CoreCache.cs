using static DataPodServices.CoreCache.CoreCacheStrings;

namespace DataPodServices.CoreCache;

public sealed partial class CoreCacheService : Grain , ICoreCache
{
    public CoreCacheService()
    {
        try
        {
            SetupConfiguration(); SetupLogging(); SetupDiagnostics(); SetupTelemetry();
        }
        catch ( Exception _ ) { Log.Fatal(_,CoreCacheFail); Log.CloseAndFlush(); throw; }
    }

    public override async Task OnActivateAsync(CancellationToken cancel)
    {
        try
        {
            ETW.Log.OnActivateStart();

            RedisAccess.Initialize(() => this.Config?[RedisConnectKey]);

            if(await RedisAccess.ConnectAsync() is false) { return; }

            Log.Information(Activated); ETW.Log.OnActivateSuccess();
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail); ETW.Log.OnActivateError(_.Message); }
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason , CancellationToken cancel)
    {
        try
        {
            ETW.Log.OnDeactivateStart();

            await RedisAccess.CloseAsync(); ShutdownTelemetry();

            Log.Information(Deactivated); await Log.CloseAndFlushAsync(); ETW.Log.OnDeactivateSuccess();
        }
        catch ( Exception _ ) { Log.Error(_,DeactivateFail); }
    }
}