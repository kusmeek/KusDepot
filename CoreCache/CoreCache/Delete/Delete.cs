namespace KusDepot.Data;

internal sealed partial class CoreCache
{
    public async Task<Boolean> Delete(String? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.DeleteStart(id); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(id is null) { Log.Error(BadArg); SetErr(_); ETW.Log.DeleteError(BadArg,id); return false; }

            if(await this.Exists(id) is not true) { Log.Error(DeleteNotFoundID,id); SetErr(_); ETW.Log.DeleteError(DeleteNotFound,id); return false; }

            if(await (await RedisAccess.GetDatabaseAsync()).KeyDeleteAsync(id))
            {
                Log.Information(DeleteSuccessID,id); SetOk(_); ETW.Log.DeleteSuccess(id); return true;
            }

            Log.Error(DeleteFailID,id); SetErr(_); ETW.Log.DeleteError(DeleteFail,id); return false;
        }
        catch ( Exception _ ) { Log.Error(_,DeleteFailID,id); ETW.Log.DeleteError(_.Message,id); return false; }
    }
}