namespace KusDepot.Data;

[StatePersistence(StatePersistence.Persisted)]
internal sealed partial class CoreCache : Actor , ICoreCache , IRemindable
{
    public CoreCache(ActorService actor , ActorId id) : base(actor,id) { SetupConfiguration(); SetupDiagnostics(); }

    public Task<Boolean> CleanUp(TimeSpan? age = null , String? traceid = null , String? spanid = null)
    {
        try
        {
            Log.Information(CleanUpStarted); using DiagnosticActivity? __ = StartDiagnostic("CleanUp",traceid,spanid);

            DateTimeOffset?                   ex = age is not null ? DateTimeOffset.Now - age : DateTimeOffset.Now.AddDays(-7);
            Dictionary<String,String>         cc = this.StateManager.GetStateAsync<Dictionary<String,String>>(StateNameCore).Result;
            Dictionary<String,DateTimeOffset> dc = this.StateManager.GetStateAsync<Dictionary<String,DateTimeOffset>>(StateNameDate).Result;

            foreach(KeyValuePair<String,DateTimeOffset> _ in dc)
            {
                if(_.Value < ex)
                {
                    if(cc.Remove(_.Key) && dc.Remove(_.Key))
                    {
                        Log.Information(CleanUpSuccessID,_.Key);
                    }
                    Log.Error(CleanUpFailedID,_.Key);
                }
            }

            Task c = this.StateManager.SetStateAsync(StateNameCore,cc); c.Wait();
            Task d = this.StateManager.SetStateAsync(StateNameDate,dc); d.Wait();
            Task s = this.SaveStateAsync();                             s.Wait();

            if(c.IsCompletedSuccessfully && d.IsCompletedSuccessfully && s.IsCompletedSuccessfully)
            {
                Log.Information(CleanUpFinished); SetOk(__); return Task.FromResult(true);;
            }

            Log.Error(CleanUpFailed); SetErr(__); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,CleanUpFailed); return Task.FromResult(false); }
    }

    public Task<Boolean> Delete(String? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("Delete",traceid,spanid)?.AddTag("id",id);

            if(id is null) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }

            if(this.Exists(id).Result is not true) { Log.Information(DeleteNotFoundID,id); SetOk(_); return Task.FromResult(false); }

            if(this.StateManager.GetStateAsync<Dictionary<String,String>>(StateNameCore).Result.Remove(id) &&
               this.StateManager.GetStateAsync<Dictionary<String,DateTimeOffset>>(StateNameDate).Result.Remove(id))
            {
                if(Task.WhenAny(this.SaveStateAsync()).Result.IsCompletedSuccessfully)
                {
                    Log.Information(DeleteSuccessID,id); SetOk(_); return Task.FromResult(true);
                }                
            }
            Log.Error(DeleteFailID,id); SetErr(_); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,DeleteFailID,id); return Task.FromResult(false); }
    }

    public Task<Boolean?> Exists(String? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("Exists",traceid,spanid)?.AddTag("id",id);

            if(id is null) { Log.Error(BadArg); SetErr(__); return Task.FromResult<Boolean?>(null); }

            ConditionalValue<Dictionary<String,String>> _ = this.StateManager.TryGetStateAsync<Dictionary<String,String>>(StateNameCore).Result;

            SetOk(__); return Task.FromResult(_.HasValue && _.Value.ContainsKey(id) ? true : (Boolean?)false);
        }
        catch ( Exception _ ) { Log.Error(_,ExistsFailID,id); return Task.FromResult<Boolean?>(null); }
    }

    public Task<String?> Get(String? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("Get",traceid,spanid)?.AddTag("id",id);

            if(id is null) { Log.Error(BadArg); SetErr(_); return Task.FromResult<String?>(null); }

            if(this.Exists(id).Result is false) { Log.Information(GetNotFoundID,id); SetOk(_); return Task.FromResult<String?>(null); }

            String r = this.StateManager.GetStateAsync<Dictionary<String,String>>(StateNameCore).Result[id];

            Log.Information(GetSuccessID,id); SetOk(_); return Task.FromResult<String?>(r);
        }
        catch ( Exception _ ) { Log.Error(_,GetFailID,id); return Task.FromResult<String?>(null); }
    }

    public Task<Boolean> Store(String? id , String? it , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("Store",traceid,spanid)?.AddTag("id",id);

            if(id is null || it is null) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }

            if(this.Exists(id).Result is not false) { Log.Information(StoreConflictID,id); SetOk(_); return Task.FromResult(false); }

            Dictionary<String,String>         cc = this.StateManager.GetOrAddStateAsync(StateNameCore,new Dictionary<String,String>()).Result;
            Dictionary<String,DateTimeOffset> dc = this.StateManager.GetOrAddStateAsync(StateNameDate,new Dictionary<String,DateTimeOffset>()).Result;

            cc[id] = it; dc[id] = DateTimeOffset.Now;

            Task c = this.StateManager.SetStateAsync(StateNameCore,cc); c.Wait();
            Task d = this.StateManager.SetStateAsync(StateNameDate,dc); d.Wait();
            Task s = this.SaveStateAsync();                             s.Wait();

            if(c.IsCompletedSuccessfully && d.IsCompletedSuccessfully && s.IsCompletedSuccessfully)
            {
                Log.Information(StoreSuccessID,id); SetOk(_); return Task.FromResult(true);
            }

            Log.Error(StoreFailID,id); SetErr(_); return Task.FromResult(false);
        }
        catch( Exception _ ) { Log.Error(_ ,StoreFailID,id); return Task.FromResult(false); }
    }

    protected override Task OnActivateAsync()
    {
        try
        {
            this.RegisterReminderAsync("CleanUp",Array.Empty<Byte>(),TimeSpan.FromDays(7),TimeSpan.FromDays(7)).Wait();

            Log.Information(Activated); return Task.FromResult(true);
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail); return Task.FromResult(false); }
    }

    protected override Task OnDeactivateAsync() { Log.Information(Deactivated); return Task.FromResult(true); }

    public Task ReceiveReminderAsync(String n , Byte[] s , TimeSpan d , TimeSpan p) { return Task.FromResult(this.CleanUp(null).Result);}
}