namespace KusDepot.Data;

[StatePersistence(StatePersistence.Persisted)]
internal sealed partial class ArkKeeper : Actor , IArkKeeper
{
    public ArkKeeper(ActorService actor , ActorId id) : base(actor,id) { SetupConfiguration(); SetupDiagnostics(); }

    public Task<Boolean> AddUpdate(Descriptor? descriptor , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("AddUpdate",traceid,spanid)?.AddTag("id",descriptor?.ID);

            if(descriptor is null) { Log.Error(BadArg); SetErr(__); return Task.FromResult(false); }

            Ark? _ = Ark.Parse(this.GetArk(traceid,spanid).Result); if(_ is null) { Log.Error(AddUpdateArkNull); SetErr(__); return Task.FromResult(false);}

            if(!_.AddUpdate(descriptor)) { Log.Error(AddUpdateFailDescriptor,descriptor); SetErr(__); return Task.FromResult(false); }

            if(this.StoreArk(Ark.GetBytes(_),traceid,spanid).Result) { Log.Information(AddUpdateSuccessDescriptor,descriptor); SetOk(__); return Task.FromResult(true); }

            Log.Error(AddUpdateFailDescriptor,descriptor); SetErr(__); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,AddUpdateFailDescriptor,descriptor); return Task.FromResult(false); }
    }

    public Task<Boolean?> Exists(Descriptor? descriptor , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("Exists",traceid,spanid)?.AddTag("id",descriptor?.ID);

            if(descriptor is null) { Log.Error(BadArg); SetErr(__); return Task.FromResult<Boolean?>(null); }

            Ark? _ = Ark.Parse(this.GetArk(traceid,spanid).Result); if(_ is null) { Log.Error(ExistsArkNull); SetErr(__);  return Task.FromResult<Boolean?>(false);}

            SetOk(__); return Task.FromResult<Boolean?>(_.Exists(descriptor.ID));
        }
        catch ( Exception _ ) { Log.Error(_,ExistsFailDescriptor,descriptor); return Task.FromResult<Boolean?>(null); }
    }

    public Task<Boolean?> ExistsID(Guid? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("ExistsID",traceid,spanid)?.AddTag("id",id);

            if(id is null) { Log.Error(BadArg); SetErr(__); return Task.FromResult<Boolean?>(null); }

            Ark? _ = Ark.Parse(this.GetArk(traceid,spanid).Result); if(_ is null) { Log.Error(ExistsArkNull); SetErr(__);  return Task.FromResult<Boolean?>(false);}

            SetOk(__); return Task.FromResult<Boolean?>(_.Exists(id));
        }
        catch ( Exception _ ) { Log.Error(_,ExistsFailID,id); return Task.FromResult<Boolean?>(null); }
    }

    public Task<Byte[]?> GetArk(String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("GetArk",traceid,spanid);

            ConditionalValue<Byte[]> _ = this.StateManager.TryGetStateAsync<Byte[]>(StateName).Result;

            if(!_.HasValue) { SetErr(__);  return Task.FromResult<Byte[]?>(Array.Empty<Byte>()); }

            SetOk(__); return Task.FromResult<Byte[]?>(_.Value);
        }
        catch ( Exception _ ) { Log.Error(_,GetFail); return Task.FromResult<Byte[]?>(null); }
    }

    public Task<Boolean> Remove(Descriptor? descriptor , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("Remove",traceid,spanid)?.AddTag("id",descriptor?.ID);

            if(descriptor is null) { Log.Error(BadArg); SetErr(__); return Task.FromResult(false); }

            Ark? _ = Ark.Parse(this.GetArk(traceid,spanid).Result); if(_ is null) { Log.Error(RemoveArkNull); SetErr(__); return Task.FromResult(false);}

            if(!_.Remove(descriptor)) { Log.Error(RemoveFailDescriptor,descriptor); SetErr(__); return Task.FromResult(false); }

            if(this.StoreArk(Ark.GetBytes(_),traceid,spanid).Result) { Log.Information(RemoveSuccessDescriptor,descriptor); SetOk(__); return Task.FromResult(true); }

            Log.Error(RemoveFailDescriptor,descriptor); SetErr(__); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,RemoveFailDescriptor,descriptor); return Task.FromResult(false); }
    }

    public Task<Boolean> RemoveID(Guid? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("RemoveID",traceid,spanid)?.AddTag("id",id);

            if(id is null) { Log.Error(BadArg); SetErr(__); return Task.FromResult(false); }

            Ark? _ = Ark.Parse(this.GetArk(traceid,spanid).Result); if(_ is null) { Log.Error(RemoveArkNull); SetErr(__); return Task.FromResult(false);}

            if(!_.Remove(id)) { Log.Error(RemoveFailID,id); SetErr(__); return Task.FromResult(false); }

            if(this.StoreArk(Ark.GetBytes(_),traceid,spanid).Result) { Log.Information(RemoveSuccessID,id); SetOk(__); return Task.FromResult(true); }

            Log.Error(RemoveFailID,id); SetErr(__); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,RemoveFailID,id); return Task.FromResult(false); }
    }

    public Task<Boolean> StoreArk(Byte[]? ark , String? traceid = null , String? spanid = null)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic("StoreArk",traceid,spanid);

            if(ark is null) { Log.Error(BadArg); SetErr(_); return Task.FromResult(false); }

            if(ark.SequenceEqual(this.StateManager.AddOrUpdateStateAsync(StateName,ark,(s,v) => { return ark; }).Result))
            {
                if(Task.WhenAny(this.SaveStateAsync()).Result.IsCompletedSuccessfully)
                {
                    Log.Information(StoreSuccess); SetOk(_); return Task.FromResult(true);
                }
            }
            Log.Error(StoreFail); SetErr(_); return Task.FromResult(false);
        }
        catch ( Exception _ ) { Log.Error(_,StoreFail); return Task.FromResult(false); }
    }

    protected override Task OnActivateAsync()
    {
        try
        {
            using DiagnosticActivity? __ = StartDiagnostic("OnActivateAsync");

            String? dt = __?.Context.TraceId.ToString(); String? ds = __?.Context.SpanId.ToString();

            Byte[]? _ = this.GetArk(dt,ds).Result;

            if(_ is null || Array.Empty<Byte>().SequenceEqual(_))
            {
                if(this.StoreArk(Ark.GetBytes(new Ark()),dt,ds).Result)
                {
                    Log.Information(ActivatedNew); SetOk(__); return Task.FromResult(true);
                }
            }
            Log.Information(Activated); SetOk(__); return Task.FromResult(true);
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail); return Task.FromResult(false); }
    }

    protected override Task OnDeactivateAsync() { Log.Information(Deactivated); return Task.FromResult(true); }
}