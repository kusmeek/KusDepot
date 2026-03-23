using static DataPodServices.DataConfigs.DataConfigsStrings;
using DataPodServices.DataConfigs.Security;

namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService : Grain<DataConfigsState> , IDataConfigs
{
    private readonly SecureComponent Security = new();

    public DataConfigsService()
    {
        try
        {
            SetupConfiguration(); SetupLogging(); SetupDiagnostics(); SetupTelemetry();
        }
        catch ( Exception _ ) { Log.Fatal(_,DataConfigsFail); Log.CloseAndFlush(); throw; }
    }

    public override async Task OnActivateAsync(CancellationToken cancel)
    {
        try
        {
            ETW.Log.OnActivateStart(); await this.ReadStateAsync();

            if(State.Silos is null || State.Silos.Count == 0)
            {
                StorageSilo? s = StorageSilo.FromFile(StartUpSiloFilePath); if(s is null) { Log.Error(ActivateLoadFail); ETW.Log.OnActivateError(ActivateLoadFail); return; }

                HashSet<StorageSilo> ns = new HashSet<StorageSilo>(){s}; State.Silos = ns; await this.WriteStateAsync();

                Log.Information(ActivateLoadSuccess); ETW.Log.OnActivateSuccess(ActivateLoadSuccess);
            }
            else
            {
                Log.Information(Activated); ETW.Log.OnActivateSuccess(Activated);
            }

            Security.LoadAdmin(StorageSilo.FromFile(AdminFilePath)); return;
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail); ETW.Log.OnActivateError(_.Message); return; }
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason , CancellationToken cancel)
    {
        try
        {
            ETW.Log.OnDeactivateStart(); ShutdownTelemetry(); Log.Information(Deactivated); await Log.CloseAndFlushAsync(); ETW.Log.OnDeactivateSuccess();
        }
        catch ( Exception _ ) { Log.Error(_,DeactivateFail); }
    }
}