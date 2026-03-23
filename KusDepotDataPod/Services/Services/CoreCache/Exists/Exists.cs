using static DataPodServices.CoreCache.CoreCacheStrings;

namespace DataPodServices.CoreCache;

public sealed partial class CoreCacheService
{
    public async Task<Boolean?> Exists(String? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.ExistsStart(id); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(id is null) { Logger.Error(BadArg); SetErr(__); ETW.Log.ExistsError(BadArg,id); return null; }

            Boolean e = await (await RedisAccess.GetDatabaseAsync()).KeyExistsAsync(id);

            Logger.Information(ExistsSuccessID,id); SetOk(__); ETW.Log.ExistsSuccess(id); return e;
        }
        catch ( Exception _ ) { Logger.Error(_,ExistsFailID,id); ETW.Log.ExistsError(_.Message,id); return null; }
    }
}