namespace KusDepot.Dapr;

public sealed class DaprSupervisor : Tool , IDaprSupervisor
{
    private SemaphoreSlim LifeSync { get; set; } = new(1,1);

    public static readonly Dictionary<String,IManager> Managers = new();

    public DaprSupervisor(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null,
           ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null , ILoggerFactory? logger = null)
           : base(accessmanager:accessmanager,data:data,id:id,services:services,commands:commands,configuration:configuration,logger:logger){}

    public DaprSupervisor(){}

    public override async Task StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartingFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return; } throw; }

            if((LifeState.StartingOK() is false) || this.AccessCheck(key) is false) { return; }

            DaprSupervisorConfig? sc = this.Configuration?.GetSection("DaprSupervisorConfig")?.Get<DaprSupervisorConfig>();

            if(sc is null || sc.Validate() is false) { throw new InvalidOperationException("Invalid DaprSupervisor Configuration"); }

            Managers.TryAdd(nameof(SidecarManager),(this.GetHostedServices(this.SelfAccessKey)?.ToList().Find(_ => Equals(_.GetType(),typeof(SidecarManager))) as SidecarManager)!);

            Managers.TryAdd(nameof(PlacementManager),(this.GetHostedServices(this.SelfAccessKey)?.ToList().Find(_ => Equals(_.GetType(),typeof(PlacementManager))) as PlacementManager)!);

            Managers.TryAdd(nameof(SchedulerManager),(this.GetHostedServices(this.SelfAccessKey)?.ToList().Find(_ => Equals(_.GetType(),typeof(SchedulerManager))) as SchedulerManager)!);

            await base.StartingAsync(cancel,key);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartingFail,this.GetType().Name); }

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

            if((LifeState.StartedOK() is false) || (this.Locked && this.AccessCheck(key) is false)) { return; }

            await base.StartedAsync(cancel,key);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartedFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }
}