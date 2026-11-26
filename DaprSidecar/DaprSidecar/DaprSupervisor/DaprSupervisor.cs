namespace KusDepot.Dapr;

public sealed class DaprSupervisor : Tool
{
    private SemaphoreSlim LifeSync { get; set; } = new(1,1);

    public static readonly Dictionary<String,IManager> Managers = new();

    public DaprSupervisor(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null,
           ToolServiceProvider? services = null , Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null , ILoggerFactory? logger = null)
           : base(accessmanager:accessmanager,data:data,id:id,services:services,commands:commands,configuration:configuration,logger:logger){}

    public DaprSupervisor(){}

    public override async Task<Boolean> StartingAsync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean ls = false;
        DC();
        try
        {
            cancel ??= CancellationToken.None; try { await this.LifeSync.WaitAsync(cancel.Value).ConfigureAwait(false); ls = true; }

            catch ( Exception _ ) { Logger?.Error(_,StartingFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return false; } throw; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            IHostedService[] services = this.GetHostedServicesSnapshot();

            Managers.TryAdd(nameof(SidecarManager),(services.FirstOrDefault(_ => Equals(_.GetType(),typeof(SidecarManager))) as SidecarManager)!);
            Managers.TryAdd(nameof(PlacementManager),(services.FirstOrDefault(_ => Equals(_.GetType(),typeof(PlacementManager))) as PlacementManager)!);
            Managers.TryAdd(nameof(SchedulerManager),(services.FirstOrDefault(_ => Equals(_.GetType(),typeof(SchedulerManager))) as SchedulerManager)!);

            return await base.StartingAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartingFail,this.GetType().Name); return false; }

        finally { if(ls) { this.LifeSync.Release(); } }
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

            return await base.StartedAsync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartedFail,this.GetType().Name); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.LifeSync.Release(); } }
    }
}