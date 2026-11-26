namespace KusDepot.Dapr;

public class ServiceManager : Tool , IManager
{
    protected Int32? ServicePID;

    private IScheduler? Scheduler;

    private ManagerConfig? ManagerConfig;

    private DaprServiceProcess? DaprProcess;

    private CancellationTokenSource? ManagerLifetime;

    protected SemaphoreSlim LifeSync { get; set; } = new(1,1);

    public ServiceManager(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null,
           ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null , ILoggerFactory? logger = null)
           : base(accessmanager:accessmanager,data:data,id:id,services:services,commands:commands,configuration:configuration,logger:logger)
            { ManagerConfig = configuration?.GetSection($"DaprSupervisorConfig:{GetType().Name}")?.Get<ManagerConfig>(); }

    protected virtual async Task<Boolean> InitializeScheduler(Quartz.IScheduler? scheduler = null , CancellationToken cancel = default)
    {
        DC();
        try { this.Scheduler ??= scheduler ?? await QuartzSchedulerBuilder.Create().BuildScheduler(cancel).ConfigureAwait(false); return true; }

        catch ( OperationCanceledException ) when ( cancel.IsCancellationRequested ) { throw; }

        catch ( Exception _ ) { Logger?.Error(_,InitializeSchedulerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    public virtual async Task Manage(CancellationToken cancel = default)
    {
        try
        {
            using CancellationTokenSource timeout = new(); timeout.CancelAfter(TimeSpan.FromSeconds(ManagerConfig!.Timeout));

            using CancellationTokenSource linked = CancellationTokenSource.CreateLinkedTokenSource(cancel,timeout.Token);

            if((await CheckServiceHealth(linked.Token)) is false)
            {
                linked.Token.ThrowIfCancellationRequested();

                await TerminateService(linked.Token);

                linked.Token.ThrowIfCancellationRequested();

                await RestartService(linked.Token);
            }
        }
        catch ( OperationCanceledException ) when ( cancel.IsCancellationRequested ) { throw; }

        catch ( Exception _ ) { Logger?.Error(_,ManageFail,this.GetType().Name,ManagerConfig!.ToString()); }
    }

    protected virtual async Task<Boolean> CheckServiceHealth(CancellationToken? cancel = null)
    {
        try
        {
            Object? svcid = await this.GetRemoveOutputAsync(
                await this.ExecuteCommandAsync(
                    CommandDetails.Create("CheckServiceHealth").SetArgument("ManagerConfig",ManagerConfig).SetLogger(Logger),cancel,key:this.SelfKey)!,cancel,key:this.SelfKey);

            if(svcid is not null) { ServicePID = (Int32)svcid; Logger?.Information(ServiceHealthy,this.GetType().Name,svcid?.ToString(),ManagerConfig!.ToString()); return true; }

            else { Logger?.Information(ServiceUnHealthy,this.GetType().Name,ManagerConfig!.ToString()); return false; }
        }
        catch ( OperationCanceledException ) { throw; }

        catch ( Exception _ ) { Logger?.Error(_,CheckServiceHealthFail,this.GetType().Name); return false; }
    }

    protected virtual async Task<Boolean> RestartService(CancellationToken? cancel = null)
    {
        try
        {
            if(ManagerLifetime is null || ManagerLifetime.IsCancellationRequested) { return false; }

            DaprProcess = await DaprServiceProcess.StartAsync(
                (observer,lifetime) => this.ExecuteCommandAsync(
                    CommandDetails.Create("RestartService").SetArgument("ManagerConfig",ManagerConfig).SetLogger(Logger).SetFreeMode(true).SetConsoleObserver(observer,overwrite:true),lifetime,key:this.SelfKey),
                id => this.GetRemoveOutputAsync(id,CancellationToken.None,Timeout.InfiniteTimeSpan,key:this.SelfKey),
                ManagerLifetime.Token,cancel ?? CancellationToken.None).ConfigureAwait(false);

            if(DaprProcess is null) { return false; }

            ServicePID = DaprProcess.ProcessID;

            return true;
        }
        catch ( OperationCanceledException ) { throw; }

        catch ( Exception _ ) { Logger?.Error(_,RestartServiceFail,this.GetType().Name); return false; }
    }

    protected virtual async Task<Boolean> TerminateService(CancellationToken? cancel = null)
    {
        try
        {
            Boolean terminated = true;

            if(ServicePID is not null)
            {
                terminated = (Boolean?)await this.GetRemoveOutputAsync(
                    await this.ExecuteCommandAsync(
                        CommandDetails.Create("TerminateService").SetArgument("ManagerConfig",ManagerConfig).SetLogger(Logger).SetArgument("ProcessID",ServicePID),cancel,key:this.SelfKey)!,cancel,key:this.SelfKey) ?? false;
            }

            if(DaprProcess is not null)
            {
                await DaprProcess.StopAsync(cancel ?? CancellationToken.None).ConfigureAwait(false);

                await DaprProcess.DisposeAsync().ConfigureAwait(false);

                DaprProcess = null;
            }

            ServicePID = null;

            return terminated;
        }
        catch ( OperationCanceledException ) { throw; }

        catch ( Exception _ ) { Logger?.Error(_,TerminateServiceFail,this.GetType().Name); return false; }
    }

    public override async Task<Boolean> StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartingFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return false; } throw; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(await InitializeScheduler(cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(ManagerLifetime is null || ManagerLifetime.IsCancellationRequested)
            {
                ManagerLifetime?.Dispose();

                ManagerLifetime = new();
            }

            Logger?.Information(ManagerStarting,this.GetType().Name,ManagerConfig!.ToString());

            if(await base.StartingAsync(cancel,key).ConfigureAwait(false) is false) { return false; }

            await Scheduler!.ScheduleJob(

                JobBuilder.Create<ManageJob>().UsingJobData("Manager",GetType().Name).Build(),

                TriggerBuilder.Create().WithSimpleSchedule(s => s.RepeatForever().WithInterval(TimeSpan.FromSeconds(ManagerConfig!.Interval))).Build()).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartingFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }

        return true;
    }

    public override async Task<Boolean> StartAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return false; } throw; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(await base.StartAsync(cancel,key).ConfigureAwait(false) is false) { return false; }

            await Scheduler!.Start().ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }

        return true;
    }

    public override async Task<Boolean> StartedAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartedFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return false; } throw; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(await base.StartedAsync(cancel,key).ConfigureAwait(false) is false) { return false; }

            Logger?.Information(ManagerStarted,this.GetType().Name,ManagerConfig!.ToString());
        }
        catch ( Exception _ ) { Logger?.Error(_,StartedFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }

        return true;
    }

    public override async Task<Boolean> StopAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StopFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return false; } throw; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            ManagerLifetime?.Cancel();

            await Scheduler!.Shutdown().ConfigureAwait(false);

            if(DaprProcess is not null)
            {
                await DaprProcess.DisposeAsync().ConfigureAwait(false);

                DaprProcess = null;
            }

            ManagerLifetime?.Dispose(); ManagerLifetime = null;

            ServicePID = null;

            if(await base.StopAsync(cancel,key).ConfigureAwait(false) is false) { return false; }
        }
        catch ( Exception _ ) { Logger?.Error(_,StopFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }

        return true;
    }
}