namespace KusDepot.Data;

public sealed partial class DataControl
{
    private static void AddCacheDCU(Object? obj)
    {
        Tuple<DataControlUpload?,String?,String?>? input; String? id = default;

        try
        {
            if((input = obj as Tuple<DataControlUpload?,String?,String?>) is null)  { return; }

            DataControlUpload? dcu = input.Item1; id = dcu?.Descriptor.ID.ToString(); ETW.Log.CacheStart(id); String? dt = input.Item2; String? ds = input.Item3;

            if(ActorProxy.Create<ICoreCache>(ActorIds.CoreCache,ServiceLocators.CoreCacheService).Store(id,dcu?.Object,dt,ds).GetAwaiter().GetResult())
            {
                Log.Information(AddCacheSuccessID,id); ETW.Log.CacheSuccess(AddCacheSuccess,id); return;
            }

            Log.Error(AddCacheFailID,id); ETW.Log.CacheError(AddCacheFail,id);
        }
        catch ( Exception _ ) { Log.Error(_,AddCacheFailID,id); ETW.Log.CacheError(AddCacheFail,id); }
    }

    private static void AddCacheIDIT(Object? obj)
    {
        Tuple<String?,String?,String?,String?>? input; String? id = default;

        try
        {
            if((input = obj as Tuple<String?,String?,String?,String?>) is null)  { return; }

            id = input.Item1; ETW.Log.CacheStart(id); String? it = input.Item2; String? dt = input.Item3; String? ds = input.Item4;

            if(ActorProxy.Create<ICoreCache>(ActorIds.CoreCache,ServiceLocators.CoreCacheService).Store(id,it,dt,ds).GetAwaiter().GetResult())
            {
                Log.Information(AddCacheSuccessID,id); ETW.Log.CacheSuccess(AddCacheSuccess,id); return;
            }

            Log.Error(AddCacheFailID,id); ETW.Log.CacheError(AddCacheFail,id);
        }
        catch ( Exception _ ) { Log.Error(_,AddCacheFailID,id); ETW.Log.CacheError(AddCacheFail,id); }
    }
}