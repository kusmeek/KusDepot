using static DataPodServices.CoreCache.CoreCacheStrings;

namespace DataPodServices.CoreCache;

public sealed partial class CoreCacheService
{
    public async Task<String?> Get(String? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetStart(id); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(id is null) { Logger.Error(BadArg); SetErr(_); ETW.Log.GetError(BadArg,id); return null; }

            var it = await (await RedisAccess.GetDatabaseAsync()).StringGetAsync(id);

            if(it.IsNull)
            {
                Logger.Error(GetNotFoundID,id); SetErr(_); ETW.Log.GetError(GetNotFound,id); return null;
            }

            Logger.Information(GetSuccessID,id); SetOk(_); ETW.Log.GetSuccess(id); return it;
        }
        catch ( Exception _ ) { Logger.Error(_,GetFailID,id); ETW.Log.GetError(_.Message,id); return null; }
    }
}