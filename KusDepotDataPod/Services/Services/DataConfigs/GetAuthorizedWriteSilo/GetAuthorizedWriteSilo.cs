using static DataPodServices.DataConfigs.DataConfigsStrings;

namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService
{
    public async Task<StorageSilo?> GetAuthorizedWriteSilo(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetWriteStart(); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { Logger.Error(BadArg); SetErr(__); ETW.Log.GetWriteError(BadArg); return null; }

            await this.ReadStateAsync();

            if(State.Silos is null || State.Silos.Count == 0) { Logger.Error(GetWriteEmpty); SetErr(__); ETW.Log.GetWriteError(GetWriteEmpty); return null; }

            foreach(StorageSilo s in State.Silos)
            {
                if(await Security.ValidateTokenVerifyRole(token,String.Concat(s.CatalogName,".Write"),s.TenantID,s.AppClientID))
                {
                    Logger.Information(GetWriteSuccess); SetOk(__); ETW.Log.GetWriteSuccess(GetWriteSuccess); return s;
                }
            }

            Logger.Information(GetWriteNone); SetOk(__); ETW.Log.GetWriteSuccess(GetWriteNone); return null;
        }
        catch ( Exception _ ) { Logger.Error(_,GetWriteFail); ETW.Log.GetWriteError(_.Message); return null; }
    }
}