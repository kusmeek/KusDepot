namespace KusDepot.Data;

internal sealed partial class CoreCache
{
    public async Task<String?> Get(String? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetStart(id); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(id is null) { Log.Error(BadArg); SetErr(_); ETW.Log.GetError(BadArg,id); return null; }

            var it = await (await RedisAccess.GetDatabaseAsync()).StringGetAsync(id);

            if(it.IsNull)
            {
                Log.Error(GetNotFoundID,id); SetErr(_); ETW.Log.GetError(GetNotFound,id); return null;
            }

            Log.Information(GetSuccessID,id); SetOk(_); ETW.Log.GetSuccess(id); return it;
        }
        catch ( Exception _ ) { Log.Error(_,GetFailID,id); ETW.Log.GetError(_.Message,id); return null; }
    }
}