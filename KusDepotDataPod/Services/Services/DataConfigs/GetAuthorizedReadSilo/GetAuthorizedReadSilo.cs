using static DataPodServices.DataConfigs.DataConfigsStrings;

namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService
{
    public async Task<StorageSilo?> GetAuthorizedReadSilo(String token , String? traceid = null , String? spanid = null , CancellationToken cancel = default)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { Logger.Error(BadArg); SetErr(__); return null; }

            await this.ReadStateAsync();

            if(State.Silos is null || State.Silos.Count == 0) { Logger.Error(GetReadEmpty); SetErr(__); return null; }

            foreach(StorageSilo s in State.Silos)
            {
                if(await DataConfigs.Security.SecureComponent.ValidateTokenVerifyRole(token,String.Concat(s.CatalogName,".Read"),s.TenantID,s.AppClientID,cancel))
                {
                    Logger.Information(GetReadSuccess); SetOk(__); return s;
                }
            }

            Logger.Information(GetReadNone); SetOk(__); return null;
        }
        catch ( Exception _ ) { Logger.Error(_,GetReadFail); return null; }
    }
}