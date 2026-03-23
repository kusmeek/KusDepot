namespace KusDepot.Data;

internal sealed partial class DataConfigs
{
    public async Task<StorageSilo?> GetAuthorizedWriteSilo(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetWriteStart(); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { Log.Error(BadArg); SetErr(__); ETW.Log.GetWriteError(BadArg); return null; }

            ConditionalValue<HashSet<StorageSilo>> _ = await this.StateManager.TryGetStateAsync<HashSet<StorageSilo>>(StateName);

            if(_.HasValue is false) { Log.Error(GetWriteEmpty); SetErr(__); ETW.Log.GetWriteError(GetWriteEmpty); return null; }

            foreach(StorageSilo s in _.Value)
            {
                if(await Security.ValidateTokenVerifyRole(token,String.Concat(s.CatalogName,".Write"),s.TenantID,s.AppClientID).ConfigureAwait(false))
                {
                    Log.Information(GetWriteSuccess); SetOk(__); ETW.Log.GetWriteSuccess(GetWriteSuccess); return s;
                }
            }

            Log.Information(GetWriteNone); SetOk(__); ETW.Log.GetWriteSuccess(GetWriteNone); return null;
        }
        catch ( Exception _ ) { Log.Error(_,GetWriteFail); ETW.Log.GetWriteError(_.Message); return null; }
    }
}