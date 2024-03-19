namespace KusDepot.Data;

internal sealed partial class DataControl
{
    private static void AddCacheDCM(Object? obj)
    {
        try
        {
            if(obj is null) { return; }

            DataControlUpload? dcu = ((Object?[]?)obj!)[0] as DataControlUpload;

            String? dt = ((Object?[]?)obj!)[1] as String; String? ds = ((Object?[]?)obj!)[2] as String;

            if(ActorProxy.Create<ICoreCache>(ActorIds.CoreCache,ServiceLocators.CoreCacheService).Store(dcu!.Descriptor.ID.ToString(),dcu.Object,dt,ds).Result)
            {
                Log.Information(AddCacheSuccessID,dcu.Descriptor.ID); return;
            }

            Log.Error(AddCacheFailID,dcu.Descriptor.ID);
        }
        catch ( Exception _ ) { Log.Error(_,AddCacheFailID,((DataControlUpload?)obj)!.Descriptor.ID); }
    }

    private static void AddCacheIDIT(Object? obj)
    {
        try
        {
            if(obj is null) { return; }

            String? id = ((Object?[]?)obj!)[0] as String; String? it = ((Object?[]?)obj!)[1] as String;
            String? dt = ((Object?[]?)obj!)[2] as String; String? ds = ((Object?[]?)obj!)[3] as String;

            if(ActorProxy.Create<ICoreCache>(ActorIds.CoreCache,ServiceLocators.CoreCacheService).Store(id,it,dt,ds).Result)
            {
                Log.Information(AddCacheSuccessID,id); return;
            }

            Log.Error(AddCacheFailID,id);
        }
        catch ( Exception _ ) { Log.Error(_,AddCacheFailID,((Object?[]?)obj!)[0]); }
    }
}