using static DataPodServices.DataConfigs.DataConfigsStrings;

namespace DataPodServices.DataConfigs;

public sealed partial class DataConfigsService
{
    public async Task<Boolean> IsAdmin(String token , String? traceid = null , String? spanid = null , CancellationToken cancel = default)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { Logger.Error(BadArg); SetErr(__); return false; }

            var result = await Security.IsAdmin(token,cancel);

            SetOk(__); return result;
        }
        catch ( Exception _ ) { Logger.Error(_,IsAdminFail); return false; }
    }
}