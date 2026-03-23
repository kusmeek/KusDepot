using static DataPodServices.DataConfigs.DataConfigsStrings;

namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService
{
    public async Task<Boolean> SetStorageSilos(HashSet<StorageSilo>? silos , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.SetSilosStart(); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(silos is null) { Logger.Error(BadArg); SetErr(_); ETW.Log.SetSilosError(BadArg); return false; }

            if(String.IsNullOrEmpty(token)) { Logger.Error(SetSilosAuth); SetErr(_); ETW.Log.SetSilosError(SetSilosAuth); return false; }

            if(await Security.IsAdmin(token) is false) { Logger.Error(SetSilosAuth); SetErr(_); ETW.Log.SetSilosError(SetSilosAuth); return false; }

            await this.ReadStateAsync();

            State.Silos = silos;

            await this.WriteStateAsync();

            Logger.Information(SetSilosSuccess); SetOk(_); ETW.Log.SetSilosSuccess(SetSilosSuccess); return true;
        }
        catch ( Exception __ ) { Logger.Error(__,SetSilosFail); ETW.Log.SetSilosError(__.Message); return false; }
    }
}