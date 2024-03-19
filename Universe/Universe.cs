namespace KusDepot.Data;

[StatePersistence(StatePersistence.Persisted)]
internal sealed partial class Universe : Actor , IUniverse
{
    public Universe(ActorService actor , ActorId id) : base(actor,id) { SetupConfiguration(); SetupDiagnostics(); }

    public Task<Boolean> Add(Guid? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("Add",traceid,spanid)?.AddTag("id",id);

            if(id is null || Equals(id,Guid.Empty)) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }

            if(this.StateManager.AddOrUpdateStateAsync<Object?>(id.ToString(),null,(s,v) => { return null; }).Result is null)
            {
                this.SaveStateAsync().Wait(); Log.Information(AddSuccessID,id); SetOk(_); return Task.FromResult(true);
            }
            Log.Error(AddFailID,id); SetErr(_); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,AddFailID,id); return Task.FromResult(false); }
    }

    public Task<Boolean?> Exists(Guid? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("Exists",traceid,spanid)?.AddTag("id",id);

            if(id is null || Equals(id,Guid.Empty)) { Log.Error(BadArg); SetErr(_); return Task.FromResult<Boolean?>(null); }

            Boolean r = this.StateManager.ContainsStateAsync(id.ToString()).Result; SetOk(_); return Task.FromResult<Boolean?>(r);
        }
        catch ( Exception _ ) { Log.Error(_,ExistsFailID,id); return Task.FromResult<Boolean?>(null); }
    }

    public Task<Int32> GetSize(String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("GetSize",traceid,spanid);

            Int32 r = this.StateManager.GetStateNamesAsync().Result.Count(); SetOk(_); return Task.FromResult(r);
        }
        catch ( Exception _ ) { Log.Error(_,GetSizeFail); return Task.FromResult(-1); }
    }

    public Task<HashSet<Guid>?> ListAll(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("ListAll",traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(!ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService).IsAdmin(token,traceid,spanid).Result)

            { Log.Error(ListAllAuthFail); SetErr(_); return Task.FromResult<HashSet<Guid>?>(null); }

            HashSet<Guid> r = this.StateManager.GetStateNamesAsync().Result.Select(_=>Guid.Parse(_)).ToHashSet(); SetOk(_); return Task.FromResult<HashSet<Guid>?>(r);
        }
        catch ( Exception _ ) { Log.Error(_,ListAllFail); return Task.FromResult<HashSet<Guid>?>(null); }
    }

    public Task<Boolean> Remove(Guid? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("Remove",traceid,spanid)?.AddTag("id",id);

            if(id is null || Equals(id,Guid.Empty)) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }

            if(this.StateManager.TryRemoveStateAsync(id.ToString()).Result)
            {
                this.SaveStateAsync().Wait(); Log.Information(RemoveSuccessID,id); SetOk(_); return Task.FromResult(true);
            }
            Log.Error(RemoveFailID,id); SetErr(_); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,RemoveFailID,id); return Task.FromResult(false); }
    }

    public Task<Boolean> Reset(HashSet<Guid>? ids , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("Reset",traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(ids is null) { Log.Error(BadArg); SetErr(__); return Task.FromResult(false); }

            if(!ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService).IsAdmin(token,traceid,spanid).Result)
            { Log.Error(ResetAuthFail); SetErr(__); return Task.FromResult(false); }

            foreach(Guid id in this.ListAll(token,traceid,spanid).Result!)
            {
                this.StateManager.RemoveStateAsync(id.ToString()).Wait();
            }

            Task r = this.SaveStateAsync(); r.Wait(); if(!r.IsCompletedSuccessfully) { Log.Error(ResetFail); SetErr(__); return Task.FromResult(false); }

            foreach(Guid id in ids)
            {
                Task _ = this.StateManager.SetStateAsync<Object?>(id.ToString(),null); _.Wait();
                if(!_.IsCompletedSuccessfully) { Log.Error(ResetFail); SetErr(__); return Task.FromResult(false); }
            }

            Task s = this.SaveStateAsync(); s.Wait(); if(!s.IsCompletedSuccessfully) { Log.Error(ResetFail); SetErr(__); return Task.FromResult(false); }

            Log.Information(ResetSuccess); SetOk(__); return Task.FromResult(true);
        }
        catch ( Exception _ ) { Log.Error(_,ResetFail); return Task.FromResult(false); }
    }

    protected override Task OnActivateAsync() { Log.Information(Activated); return Task.FromResult(true); }

    protected override Task OnDeactivateAsync() { Log.Information(Deactivated); return Task.FromResult(true); }
}