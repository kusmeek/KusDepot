namespace KusDepot.Data;

[StatePersistence(StatePersistence.Persisted)]
internal sealed partial class DataConfigs : Actor , IDataConfigs
{
    private readonly SecureComponent Security = new();

    public DataConfigs(ActorService actor , ActorId id) : base(actor,id)
    {
        try
        {
            SetupConfiguration(); SetupLogging(); SetupDiagnostics(); SetupTelemetry();
        }
        catch ( Exception _ ) { Log.Fatal(_,DataConfigsFail); Log.CloseAndFlush(); throw; }
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
                    Task st = this.SaveStateAsync(); await st.ConfigureAwait(false);

                    if(st.IsCompletedSuccessfully)
                    {
                        Log.Information(ActivateLoadSuccess); ETW.Log.OnActivateSuccess(ActivateLoadSuccess);
                    }
                }
            }
            else
            {
                Log.Information(Activated); ETW.Log.OnActivateSuccess(Activated);
            }

            Security.LoadAdmin(StorageSilo.FromFile(AdminFilePath)); return true;
        }
        catch ( Exception _ ) { Log.Error(_,ActivateFail); ETW.Log.OnActivateError(_.Message); return false; }
    }

    protected override async Task<Boolean> OnDeactivateAsync()
    {
        try
        {
            ETW.Log.OnDeactivateStart(); ShutdownTelemetry(); Log.Information(Deactivated); await Log.CloseAndFlushAsync(); ETW.Log.OnDeactivateSuccess(); return true;
        }
        catch ( Exception _ ) { Log.Error(_,DeactivateFail); return false; }
    }
}