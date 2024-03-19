namespace TempDB;

[StatePersistence(StatePersistence.Volatile)]
internal sealed partial class TempDB : Actor , ITempDB
{
    public TempDB(ActorService actor , ActorId id) : base(actor,id) { this.ActorID = this.GetActorID(); SetupConfiguration(); SetupDiagnostics(); }

    public Task<Boolean> Delete(String? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("Delete",traceid,spanid)?.AddTag("id",id);

            if(String.IsNullOrEmpty(id)) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }

            if(this.StateManager.TryRemoveStateAsync(id).Result)
            {
                this.SaveStateAsync().Wait(); Log.Information(DeleteSuccessID,this.ActorID,id); SetOk(_); return Task.FromResult(true);
            }
            Log.Error(DeleteFailID,this.ActorID,id); SetErr(_); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,DeleteFailID,this.ActorID,id); return Task.FromResult(false); }
    }

    public Task<Boolean?> Exists(String? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("Exists",traceid,spanid)?.AddTag("id",id);

            if(String.IsNullOrEmpty(id)) { Log.Error(BadArg); SetErr(__); return Task.FromResult<Boolean?>(null); }

            ConditionalValue<Byte[]> _ = this.StateManager.TryGetStateAsync<Byte[]>(id).Result; SetOk(__);

            return Task.FromResult<Boolean?>(_.HasValue ? true : false);
        }
        catch ( Exception _ ) { Log.Error(_,ExistsFailID,this.ActorID,id); return Task.FromResult<Boolean?>(null); }
    }

    public Task<Byte[]> Get(String? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("Get",traceid,spanid)?.AddTag("id",id);

            if(String.IsNullOrEmpty(id)) { Log.Error(BadArg); SetErr(_); return Task.FromResult(Array.Empty<Byte>()); }

            Byte[] r = this.StateManager.TryGetStateAsync<Byte[]>(id).Result.Value; SetOk(_); return Task.FromResult(r);
        }
        catch ( Exception _ ) { Log.Error(_,GetFailID,this.ActorID,id); return Task.FromResult(Array.Empty<Byte>()); }
    }

    public Task<Boolean> Store(String? id , Byte[]? it , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("Store",traceid,spanid)?.AddTag("id",id);

            if(it is null || String.IsNullOrEmpty(id)) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }

            if(this.StateManager.TryAddStateAsync<Byte[]>(id,it).Result)
            {
                this.SaveStateAsync().Wait(); Log.Information(StoreSuccessID,this.ActorID,id); SetOk(_); return Task.FromResult(true);
            }
            Log.Error(StoreFailID,this.ActorID,id); SetErr(_); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,StoreFailID,this.ActorID,id); return Task.FromResult(false); }
    }

    protected override Task OnActivateAsync() { Log.Information(Activated,this.ActorID); return Task.FromResult(true); }

    protected override Task OnDeactivateAsync() { Log.Information(Deactivated,this.ActorID); return Task.FromResult(true); }

    private String ActorID {get;init;}

    private String GetActorID()
    {
        switch(this.Id.Kind)
        {
            case ActorIdKind.Guid:   return this.Id.GetGuidId().ToString();
            case ActorIdKind.Long:   return this.Id.GetLongId().ToString(System.Globalization.CultureInfo.InvariantCulture);
            case ActorIdKind.String: return this.Id.GetStringId();
            default:                 return String.Empty;
        }
    }
}