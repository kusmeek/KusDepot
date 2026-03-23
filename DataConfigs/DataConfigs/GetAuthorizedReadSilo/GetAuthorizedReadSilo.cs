namespace KusDepot.Data;

internal sealed partial class DataConfigs
{
    public async Task<StorageSilo?> GetAuthorizedReadSilo(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetReadStart(); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { Log.Error(BadArg); SetErr(__); ETW.Log.GetReadError(BadArg); return null; }

            ConditionalValue<HashSet<StorageSilo>> _ = await this.StateManager.TryGetStateAsync<HashSet<StorageSilo>>(StateName);

            if(_.HasValue is false) { Log.Error(GetReadEmpty); SetErr(__); ETW.Log.GetReadError(GetReadEmpty); return null; }

            foreach(StorageSilo s in _.Value)
            {
                if(await Security.ValidateTokenVerifyRole(token,String.Concat(s.CatalogName,".Read"),s.TenantID,s.AppClientID).ConfigureAwait(false))
                {
                    Log.Information(GetReadSuccess); SetOk(__); ETW.Log.GetReadSuccess(GetReadSuccess); return s;
                }
            }

            Log.Information(GetReadNone); SetOk(__); ETW.Log.GetReadSuccess(GetReadNone); return null;
        }
        catch ( Exception _ ) { Log.Error(_,GetReadFail); ETW.Log.GetReadError(_.Message); return null; }
    }
}