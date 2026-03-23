namespace KusDepot.Data;

internal sealed partial class DataConfigs
{
    public async Task<HashSet<StorageSilo>?> GetStorageSilos(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetSilosStart(); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { Log.Error(GetSilosAuth); SetErr(__); ETW.Log.GetSilosError(GetSilosAuth); return null; }

            if(await Security.IsAdmin(token).ConfigureAwait(false) is false) { Log.Error(GetSilosAuth); SetErr(__); ETW.Log.GetSilosError(GetSilosAuth); return null; }

            ConditionalValue<HashSet<StorageSilo>> _ = await this.StateManager.TryGetStateAsync<HashSet<StorageSilo>>(StateName);

            if(_.HasValue) { Log.Information(GetSilosSuccess); SetOk(__); ETW.Log.GetSilosSuccess(GetSilosSuccess); return _.Value; }

            Log.Error(GetSilosEmpty); SetErr(__); ETW.Log.GetSilosError(GetSilosEmpty); return null;
        }
        catch ( Exception _ ) { Log.Error(_,GetSilosFail); ETW.Log.GetSilosError(_.Message); return null; }
    }
}