namespace KusDepot.Data;

internal sealed partial class CoreCache
{
    public async Task<Boolean> Store(String? id , String? it , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.StoreStart(id); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(id is null || it is null) { Log.Error(BadArg); SetErr(_); ETW.Log.StoreError(BadArg,id); return false; }

            TimeSpan e = TimeSpan.FromDays(7);

            if(Int32.TryParse(Config?["CoreCache:DefaultTTL"] , out Int32 s) && s > 0) { e = TimeSpan.FromSeconds(s); }

            if(await (await RedisAccess.GetDatabaseAsync()).StringSetAsync(id,it,e,When.NotExists))
            {
                Log.Information(StoreSuccessID,id); SetOk(_); ETW.Log.StoreSuccess(id); return true;
            }

            Log.Error(StoreConflictID,id); SetErr(_); ETW.Log.StoreError(StoreConflict,id); return false;
        }
        catch( Exception _ ) { Log.Error(_,StoreFailID,id); ETW.Log.StoreError(_.Message,id); return false; }
    }
}