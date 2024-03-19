namespace KusDepot.Data;

[StatePersistence(StatePersistence.Persisted)]
internal sealed partial class DataConfigs : Actor , IDataConfigs
{
    public DataConfigs(ActorService actor , ActorId id) : base(actor,id) { SetupConfiguration(); SetupDiagnostics(); }

    public Task<StorageSilo?> GetAuthorizedReadSilo(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("GetAuthorizedReadSilo",traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { Log.Error(BadArg); SetErr(__); return Task.FromResult<StorageSilo?>(null); }

            ConditionalValue<HashSet<StorageSilo>> _ = this.StateManager.TryGetStateAsync<HashSet<StorageSilo>>(StateName).Result;

            if(!_.HasValue) { Log.Error(GetReadEmpty); SetErr(__); return Task.FromResult<StorageSilo?>(null); }

            ISecure se = ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService);

            foreach(StorageSilo s in _.Value)
            {
                if(se.ValidateTokenVerifyRole(token,String.Concat(s.CatalogName,".Read"),s.TenantID,s.AppClientID,traceid,spanid).Result)
                { Log.Information(GetReadSuccess); SetOk(__); return Task.FromResult<StorageSilo?>(s); }
            }

            Log.Information(GetReadNone); SetOk(__); return Task.FromResult<StorageSilo?>(null);
        }
        catch ( Exception _ ) { Log.Error(_,GetReadFail); return Task.FromResult<StorageSilo?>(null); }
    }

    public Task<StorageSilo?> GetAuthorizedWriteSilo(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("GetAuthorizedWriteSilo",traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            if(String.IsNullOrEmpty(token)) { Log.Error(BadArg); SetErr(__); return Task.FromResult<StorageSilo?>(null); }

            ConditionalValue<HashSet<StorageSilo>> _ = this.StateManager.TryGetStateAsync<HashSet<StorageSilo>>(StateName).Result;

            if(!_.HasValue) { Log.Error(GetWriteEmpty); SetErr(__); return Task.FromResult<StorageSilo?>(null); }

            ISecure se = ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService);

            foreach(StorageSilo s in _.Value)
            {
                if(se.ValidateTokenVerifyRole(token,String.Concat(s.CatalogName,".Write"),s.TenantID,s.AppClientID,traceid,spanid).Result)
                { Log.Information(GetWriteSuccess); SetOk(__); return Task.FromResult<StorageSilo?>(s); }
            }

            Log.Information(GetWriteNone); SetOk(__); return Task.FromResult<StorageSilo?>(null);
        }
        catch ( Exception _ ) { Log.Error(_,GetWriteFail); return Task.FromResult<StorageSilo?>(null); }
    }

    public Task<HashSet<StorageSilo>?> GetStorageSilos(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("GetStorageSilos",traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            String? dt = __?.Context.TraceId.ToString(); String? ds = __?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(token)) { Log.Error(GetSilosAuth); SetErr(__); return Task.FromResult<HashSet<StorageSilo>?>(null); }

            ISecure se = ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService);

            if(!se.IsAdmin(token,dt,ds).Result) { Log.Error(GetSilosAuth); SetErr(__); return Task.FromResult<HashSet<StorageSilo>?>(null); }

            ConditionalValue<HashSet<StorageSilo>> _ = this.StateManager.TryGetStateAsync<HashSet<StorageSilo>>(StateName).Result;

            if(_.HasValue) { SetOk(__); return Task.FromResult<HashSet<StorageSilo>?>(_.Value); }

            Log.Error(GetSilosEmpty); SetErr(__); return Task.FromResult<HashSet<StorageSilo>?>(null);
        }
        catch ( Exception _ ) { Log.Error(_,GetSilosFail); return Task.FromResult<HashSet<StorageSilo>?>(null); }
    }

    public Task<Boolean> SetStorageSilos(HashSet<StorageSilo>? silos , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("SetStorageSilos",traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(silos is null) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }
            
            if(String.IsNullOrEmpty(token)) { Log.Error(SetSilosAuth); SetErr(_); return Task.FromResult(false); }

            ISecure se = ActorProxy.Create<ISecure>(ActorIds.Sentinel,ServiceLocators.SecureService);

            if(!se.IsAdmin(token,dt,ds).Result) { Log.Error(SetSilosAuth); SetErr(_); return Task.FromResult(false); }

            if(silos.SetEquals(this.StateManager.AddOrUpdateStateAsync(StateName,silos,(s,v) => { return silos; }).Result))
            {
                if(Task.WhenAny(this.SaveStateAsync()).Result.IsCompletedSuccessfully)
                {
                    Log.Information(SetSilosSuccess); SetOk(_); return Task.FromResult(true);
                }
            }
            Log.Error(SetSilosFail); SetErr(_); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,SetSilosFail); return Task.FromResult(false); }
    }

    protected override Task OnActivateAsync()
    {
        try
        {
            ConditionalValue<HashSet<StorageSilo>> _ = this.StateManager.TryGetStateAsync<HashSet<StorageSilo>>(StateName).Result;

            if(!_.HasValue)
            {
                StorageSilo? s = StorageSilo.FromFile(StartUpSiloFilePath); if(s is null) { Log.Error(StartUpLoadFail); return Task.FromResult(false); }

                HashSet<StorageSilo> ns = new HashSet<StorageSilo>(){s};

                if(ns.SetEquals(this.StateManager.AddOrUpdateStateAsync(StateName,ns,(s,v) => { return ns; }).Result))
                {
                    if(Task.WhenAny(this.SaveStateAsync()).Result.IsCompletedSuccessfully)
                    {
                        Log.Information(StartUpLoadSuccess); Log.Information(Activated); return Task.FromResult(true);
                    }
                }
            }
            Log.Information(Activated); return Task.FromResult(true);
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail); return Task.FromResult(false); }
    }

    protected override Task OnDeactivateAsync() { Log.Information(Deactivated); return Task.FromResult(true); }
}