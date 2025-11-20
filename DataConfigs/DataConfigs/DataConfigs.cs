namespace KusDepot.Data;

[StatePersistence(StatePersistence.Persisted)]
internal sealed partial class DataConfigs : Actor , IDataConfigs
{
    public DataConfigs(ActorService actor , ActorId id) : base(actor,id)
    {
        try
        {
            SetupConfiguration(); SetupLogging(); SetupDiagnostics(); SetupTelemetry();
        }
        catch ( Exception _ ) { Log.Fatal(_,DataConfigsFail); Log.CloseAndFlush(); throw; }
    }

    public async Task<StorageSilo?> GetAuthorizedReadSilo(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetReadStart(); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            String? dt = __?.Context.TraceId.ToString(); String? ds = __?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(token)) { Log.Error(BadArg); SetErr(__); ETW.Log.GetReadError(BadArg); return null; }

            ConditionalValue<HashSet<StorageSilo>> _ = await this.StateManager.TryGetStateAsync<HashSet<StorageSilo>>(StateName);

            if(_.HasValue is false) { Log.Error(GetReadEmpty); SetErr(__); ETW.Log.GetReadError(GetReadEmpty); return null; }

            ISecure se = ActorProxy.Create<ISecure>(new(Guid.NewGuid()),ServiceLocators.SecureService);

            foreach(StorageSilo s in _.Value)
            {
                if(await se.ValidateTokenVerifyRole(token,String.Concat(s.CatalogName,".Read"),s.TenantID,s.AppClientID,dt,ds))

                { DeleteSecureActor(se.GetActorId()); Log.Information(GetReadSuccess); SetOk(__); ETW.Log.GetReadSuccess(GetReadSuccess); return s; }
            }

            DeleteSecureActor(se.GetActorId()); Log.Information(GetReadNone); SetOk(__); ETW.Log.GetReadSuccess(GetReadNone); return null;
        }
        catch ( Exception _ ) { Log.Error(_,GetReadFail); ETW.Log.GetReadError(_.Message); return null; }
    }

    public async Task<StorageSilo?> GetAuthorizedWriteSilo(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetWriteStart(); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            String? dt = __?.Context.TraceId.ToString(); String? ds = __?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(token)) { Log.Error(BadArg); SetErr(__); ETW.Log.GetWriteError(BadArg); return null; }

            ConditionalValue<HashSet<StorageSilo>> _ = await this.StateManager.TryGetStateAsync<HashSet<StorageSilo>>(StateName);

            if(_.HasValue is false) { Log.Error(GetWriteEmpty); SetErr(__); ETW.Log.GetWriteError(GetWriteEmpty); return null; }

            ISecure se = ActorProxy.Create<ISecure>(new(Guid.NewGuid()),ServiceLocators.SecureService);

            foreach(StorageSilo s in _.Value)
            {
                if(await se.ValidateTokenVerifyRole(token,String.Concat(s.CatalogName,".Write"),s.TenantID,s.AppClientID,dt,ds))

                { DeleteSecureActor(se.GetActorId()); Log.Information(GetWriteSuccess); SetOk(__); ETW.Log.GetWriteSuccess(GetWriteSuccess); return s; }
            }

            DeleteSecureActor(se.GetActorId()); Log.Information(GetWriteNone); SetOk(__); ETW.Log.GetWriteSuccess(GetWriteNone); return null;
        }
        catch ( Exception _ ) { Log.Error(_,GetWriteFail); ETW.Log.GetWriteError(_.Message); return null; }
    }

    public async Task<HashSet<StorageSilo>?> GetStorageSilos(String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.GetSilosStart(); using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            String? dt = __?.Context.TraceId.ToString(); String? ds = __?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(token)) { Log.Error(GetSilosAuth); SetErr(__); ETW.Log.GetSilosError(GetSilosAuth); return null; }

            ISecure se = ActorProxy.Create<ISecure>(new(Guid.NewGuid()),ServiceLocators.SecureService);

            if(await se.IsAdmin(token,dt,ds) is false) { DeleteSecureActor(se.GetActorId()); Log.Error(GetSilosAuth); SetErr(__); ETW.Log.GetSilosError(GetSilosAuth); return null; }

            ConditionalValue<HashSet<StorageSilo>> _ = await this.StateManager.TryGetStateAsync<HashSet<StorageSilo>>(StateName);

            if(_.HasValue) { DeleteSecureActor(se.GetActorId()); Log.Information(GetSilosSuccess); SetOk(__); ETW.Log.GetSilosSuccess(GetSilosSuccess); return _.Value; }

            DeleteSecureActor(se.GetActorId()); Log.Error(GetSilosEmpty); SetErr(__); ETW.Log.GetSilosError(GetSilosEmpty); return null;
        }
        catch ( Exception _ ) { Log.Error(_,GetSilosFail); ETW.Log.GetSilosError(_.Message); return null; }
    }

    public async Task<Boolean> SetStorageSilos(HashSet<StorageSilo>? silos , String token , String? traceid = null , String? spanid = null)
    {
        try
        {
            ETW.Log.SetSilosStart(); using DiagnosticActivity? _ = StartDiagnostic(traceid,spanid)?.AddTag("enduser.id",GetUPN(token));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(silos is null) { Log.Error(BadArg); SetErr(_); ETW.Log.SetSilosError(BadArg); return false; }

            if(String.IsNullOrEmpty(token)) { Log.Error(SetSilosAuth); SetErr(_); ETW.Log.SetSilosError(SetSilosAuth); return false; }

            ISecure se = ActorProxy.Create<ISecure>(new(Guid.NewGuid()),ServiceLocators.SecureService);

            if(await se.IsAdmin(token,dt,ds) is false) { DeleteSecureActor(se.GetActorId()); Log.Error(SetSilosAuth); SetErr(_); ETW.Log.SetSilosError(SetSilosAuth); return false; }

            if(silos.SetEquals(await this.StateManager.AddOrUpdateStateAsync(StateName,silos,(s,v) => { return silos; })))
            {
                Task s = this.SaveStateAsync(); await s;

                if(s.IsCompletedSuccessfully)
                {
                    DeleteSecureActor(se.GetActorId()); Log.Information(SetSilosSuccess); SetOk(_); ETW.Log.SetSilosSuccess(SetSilosSuccess); return true;
                }
            }
            DeleteSecureActor(se.GetActorId()); Log.Error(SetSilosFail); SetErr(_); ETW.Log.SetSilosError(SetSilosFail); return false;
        }
        catch ( Exception _ ) { Log.Error(_,SetSilosFail); ETW.Log.SetSilosError(_.Message); return false; }
    }

    protected override async Task<Boolean> OnActivateAsync()
    {
        try
        {
            ETW.Log.OnActivateStart(); ConditionalValue<HashSet<StorageSilo>> _ = await this.StateManager.TryGetStateAsync<HashSet<StorageSilo>>(StateName);

            if(_.HasValue is false)
            {
                StorageSilo? s = StorageSilo.FromFile(StartUpSiloFilePath); if(s is null) { Log.Error(ActivateLoadFail); ETW.Log.OnActivateError(ActivateLoadFail); return false; }

                HashSet<StorageSilo> ns = new HashSet<StorageSilo>(){s};

                if(ns.SetEquals(await this.StateManager.AddOrUpdateStateAsync(StateName,ns,(s,v) => { return ns; })))
                {
                    Task st = this.SaveStateAsync(); await st;

                    if(st.IsCompletedSuccessfully)
                    {
                        Log.Information(ActivateLoadSuccess); ETW.Log.OnActivateSuccess(ActivateLoadSuccess); return true;
                    }
                }
            }
            Log.Information(Activated); ETW.Log.OnActivateSuccess(Activated); return true;
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail); ETW.Log.OnActivateError(_.Message); return false; }
    }

    protected override async Task<Boolean> OnDeactivateAsync() { ETW.Log.OnDeactivateStart(); ShutdownTelemetry(); Log.Information(Deactivated); await Log.CloseAndFlushAsync(); ETW.Log.OnDeactivateSuccess(); return true; }
}