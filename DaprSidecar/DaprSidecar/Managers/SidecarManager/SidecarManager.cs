namespace KusDepot.Dapr;

public sealed class SidecarManager : Tool , IManager
{
    private Int32? ServicePID;

    private IScheduler? Scheduler;

    private SidecarConfig? SidecarConfig;

    private SemaphoreSlim LifeSync { get; set; } = new(1,1);

    public SidecarManager(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null,
           ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null , ILoggerFactory? logger = null)
           : base(accessmanager:accessmanager,data:data,id:id,services:services,commands:commands,configuration:configuration,logger:logger)
           { SidecarConfig = configuration?.GetSection($"DaprSupervisorConfig:{GetType().Name}")?.Get<SidecarConfig>(); InitializeScheduler(); }

    public SidecarManager(){}

    private Boolean InitializeScheduler(Quartz.IScheduler? scheduler = null)
    {
        DC(); try { this.Scheduler ??= scheduler ?? new StdSchedulerFactory().GetScheduler().GetAwaiter().GetResult(); return true; }

        catch ( Exception _ ) { Logger?.Error(_,InitializeSchedulerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    public async Task Manage()
    {
        try
        {
            CancellationTokenSource _ = new(); _.CancelAfter(TimeSpan.FromSeconds(SidecarConfig!.Timeout));

            if((await CheckServiceHealth(_.Token)) is false) { await TerminateService(_.Token); await RestartService(_.Token); }
        }
        catch ( Exception _ ) { Logger?.Error(_,ManageFail,this.GetType().Name,SidecarConfig!.ToString()); }
    }

    private async Task<Boolean> CheckServiceHealth(CancellationToken? cancel = null)
    {
        try
        {
            Object? svcid = await GetRemoveOutputAsync(
                await ExecuteCommandAsync(CommandDetails.Create("CheckServiceHealth").SetArgument("SidecarConfig",SidecarConfig).SetLogger(Logger),cancel,SelfAccessKey)!,
                cancel,key:SelfAccessKey);

            if(svcid is not null) { ServicePID = (Int32)svcid; Logger?.Information(ServiceHealthy,this.GetType().Name,svcid?.ToString(),SidecarConfig!.ToString()); return true; }

            else { Logger?.Information(ServiceUnHealthy,this.GetType().Name,SidecarConfig!.ToString()); return false; }
        }
        catch ( Exception _ ) { Logger?.Error(_,CheckServiceHealthFail,this.GetType().Name); return false; }
    }

    private async Task<Boolean> TerminateService(CancellationToken? cancel = null)
    {
        try
        {
            if(ServicePID is null) { return true; }

            return (Boolean?)await GetRemoveOutputAsync(
                await ExecuteCommandAsync(CommandDetails.Create("TerminateService").SetArgument("SidecarConfig",SidecarConfig).SetLogger(Logger).SetArgument("ProcessID",ServicePID),cancel,SelfAccessKey)!,
                cancel,key:SelfAccessKey) ?? false;
        }
        catch ( Exception _ ) { Logger?.Error(_,TerminateServiceFail,this.GetType().Name); return false; }
    }

    private async Task<Boolean> RestartService(CancellationToken? cancel = null)
    {
        try
        {
            ManagerConfig? PC = this.GetConfiguration(SelfAccessKey)?.GetSection($"DaprSupervisorConfig:{typeof(PlacementManager).Name}")?.Get<ManagerConfig>(); ManagerConfig? SC = this.GetConfiguration(SelfAccessKey)?.GetSection($"DaprSupervisorConfig:{typeof(SchedulerManager).Name}")?.Get<ManagerConfig>();

            return (Boolean?)await GetRemoveOutputAsync(
                await ExecuteCommandAsync(CommandDetails.Create("RestartService").SetArgument("SidecarConfig",SidecarConfig).SetArgument("PlacementConfig",PC).SetArgument("SchedulerConfig",SC).SetLogger(Logger),cancel,SelfAccessKey)!,
                cancel,key:SelfAccessKey) ?? false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RestartServiceFail,this.GetType().Name); return false; }
    }

    public override async Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartingFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartingOK() is false) || this.AccessCheck(key) is false) { return; }

            Logger?.Information(ManagerStarting,this.GetType().Name,SidecarConfig!.ToString()); await base.StartingAsync(cancel,key);

            await Scheduler!.ScheduleJob(

                JobBuilder.Create<ManageJob>().UsingJobData("Manager",GetType().Name).Build(),

                TriggerBuilder.Create().WithSimpleSchedule(s => s.RepeatForever().WithInterval(TimeSpan.FromSeconds(SidecarConfig!.Interval))).Build());
        }
        catch ( Exception _ ) { Logger?.Error(_,StartingFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }

    public override async Task StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartOK() is false) || this.AccessCheck(key) is false) { return; }

            await base.StartAsync(cancel,key); await Scheduler!.Start();
        }
        catch ( Exception _ ) { Logger?.Error(_,StartFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }

    public override async Task StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartedFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartedOK() is false) || this.AccessCheck(key) is false) { return; }

            await base.StartedAsync(cancel,key); Logger?.Information(ManagerStarted,this.GetType().Name,SidecarConfig!.ToString());
        }
        catch ( Exception _ ) { Logger?.Error(_,StartedFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }

    public override async Task StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StopFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StopOK() is false) || this.AccessCheck(key) is false) { return; }

            await Scheduler!.Shutdown(); await base.StopAsync(cancel,key);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }
}