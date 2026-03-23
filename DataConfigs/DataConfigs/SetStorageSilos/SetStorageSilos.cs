namespace KusDepot.Data;

internal sealed partial class DataConfigs
{
    public async Task<Boolean> SetStorageSilos(HashSet<StorageSilo>? silos , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.SetSilosStart(); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(silos is null) { Log.Error(BadArg); SetErr(_); ETW.Log.SetSilosError(BadArg); return false; }

            if(String.IsNullOrEmpty(token)) { Log.Error(SetSilosAuth); SetErr(_); ETW.Log.SetSilosError(SetSilosAuth); return false; }

            if(await Security.IsAdmin(token).ConfigureAwait(false) is false) { Log.Error(SetSilosAuth); SetErr(_); ETW.Log.SetSilosError(SetSilosAuth); return false; }

            if(silos.SetEquals(await this.StateManager.AddOrUpdateStateAsync(StateName,silos,(s,v) => { return silos; })))
            {
                Task s = this.SaveStateAsync(); await s.ConfigureAwait(false);

                if(s.IsCompletedSuccessfully)
                {
                    Log.Information(SetSilosSuccess); SetOk(_); ETW.Log.SetSilosSuccess(SetSilosSuccess); return true;
                }
            }
            Log.Error(SetSilosFail); SetErr(_); ETW.Log.SetSilosError(SetSilosFail); return false;
        }
        catch ( Exception _ ) { Log.Error(_,SetSilosFail); ETW.Log.SetSilosError(_.Message); return false; }
    }
}