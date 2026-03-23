namespace KusDepot.Data;

internal sealed partial class CoreCache
{
    public async Task<Boolean?> Exists(String? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.ExistsStart(id); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(id is null) { Log.Error(BadArg); SetErr(__); ETW.Log.ExistsError(BadArg,id); return null; }

            Boolean e = await (await RedisAccess.GetDatabaseAsync()).KeyExistsAsync(id);

            Log.Information(ExistsSuccessID,id); SetOk(__); ETW.Log.ExistsSuccess(id); return e;
        }
        catch ( Exception _ ) { Log.Error(_,ExistsFailID,id); ETW.Log.ExistsError(_.Message,id); return null; }
    }
}