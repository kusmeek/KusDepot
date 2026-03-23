using static DataPodServices.DataConfigs.DataConfigsStrings;

namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService
{
    public async Task<StorageSilo?> GetAuthorizedReadSilo(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetReadStart(); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { Logger.Error(BadArg); SetErr(__); ETW.Log.GetReadError(BadArg); return null; }

            await this.ReadStateAsync();

            if(State.Silos is null || State.Silos.Count == 0) { Logger.Error(GetReadEmpty); SetErr(__); ETW.Log.GetReadError(GetReadEmpty); return null; }

            foreach(StorageSilo s in State.Silos)
            {
                if(await Security.ValidateTokenVerifyRole(token,String.Concat(s.CatalogName,".Read"),s.TenantID,s.AppClientID))
                {
                    Logger.Information(GetReadSuccess); SetOk(__); ETW.Log.GetReadSuccess(GetReadSuccess); return s;
                }
            }

            Logger.Information(GetReadNone); SetOk(__); ETW.Log.GetReadSuccess(GetReadNone); return null;
        }
        catch ( Exception _ ) { Logger.Error(_,GetReadFail); ETW.Log.GetReadError(_.Message); return null; }
    }
}