using static DataPodServices.DataConfigs.DataConfigsStrings;

namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService
{
    public async Task<HashSet<StorageSilo>?> GetStorageSilos(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetSilosStart(); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { Logger.Error(GetSilosAuth); SetErr(__); ETW.Log.GetSilosError(GetSilosAuth); return null; }

            if(await Security.IsAdmin(token) is false) { Logger.Error(GetSilosAuth); SetErr(__); ETW.Log.GetSilosError(GetSilosAuth); return null; }

            await this.ReadStateAsync();

            if(State.Silos is not null && State.Silos.Count > 0) { Logger.Information(GetSilosSuccess); SetOk(__); ETW.Log.GetSilosSuccess(GetSilosSuccess); return State.Silos; }

            Logger.Error(GetSilosEmpty); SetErr(__); ETW.Log.GetSilosError(GetSilosEmpty); return null;
        }
        catch ( Exception _ ) { Logger.Error(_,GetSilosFail); ETW.Log.GetSilosError(_.Message); return null; }
    }
}