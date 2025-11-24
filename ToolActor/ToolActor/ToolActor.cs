using Serilog;

namespace KusDepot.ToolActor;

[ActorService(Name = "ToolActorService")]
[StatePersistence(StatePersistence.Persisted)]
public partial class ToolActor : Actor , IToolActor
{
    protected ITool? Tool;

    protected HostKey? HostKey;

    public ToolActor(ActorService actor , ActorId id) : base(actor,id)
    {
        try
        {
            SetupLogging();

            Tool = ToolBuilderFactory.CreateBuilder()

                .ConfigureServices((x,s) =>
                {
                    s.AddSingletonWithInterfaces(Log.Logger!);

                    s.AddSingletonWithInterfaces(actor);

                    s.AddSingletonWithInterfaces(id);
                })

                .ConfigureTool((x,t) =>
                {
                    HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey;
                })

                .UseAccessManager<TestAccessManager>().Seal()

                .RegisterCommand<CommandTest>("CommandTest")

                .Build();
        }
        catch ( Exception _ ) { Log.Fatal(_,ToolActorFail,this.Id.ToString()); throw; }
    }

    public async Task<Guid?> ExecuteCommand(CommandDetails? details , AccessKey? key = null)
    {
        try
        {
            Log.Information(ExecutingCommand,this.Id.ToString(),details?.Handle,details?.ID);

            return Tool?.ExecuteCommand(details,key);
        }
        catch ( Exception _ ) { Log.Error(_,ExecuteCommandFail,this.Id.ToString(),details?.Handle,details?.ID); await Task.CompletedTask; return null; }
    }

    public async Task<Guid?> ExecuteCommandCab(KusDepotCab? cab , AccessKey? key = null)
    {
        CommandDetails? d = cab?.GetCommandDetails();

        try { return await this.ExecuteCommand(d,key); }

        catch ( Exception _ ) { Log.Error(_,ExecuteCommandFail,this.Id.ToString(),d?.Handle,d?.ID); return null; }
    }

    public async Task<Object?> GetOutput(Guid? id , AccessKey? key = null)
    {
        try
        {
            if( id is null || Equals(Guid.Empty,id) || key is null ) { return null; }

            Log.Information(GetOutputStart,this.Id.ToString(),id);

            return Tool?.GetOutput(id,key);
        }
        catch ( Exception _ ) { Log.Error(_,GetOutputFail,this.Id.ToString(),id); await Task.CompletedTask; return null; }
    }

    public async Task<AccessKey?> RequestAccess(AccessRequest? request = null)
    {
        try
        {
            Log.Information(RequestAccessStart,this.Id.ToString());

            return Tool?.RequestAccess(request);
        }
        catch ( Exception _ ) { Log.Error(_,RequestAccessFail,this.Id.ToString()); await Task.CompletedTask; return null; }
    }

    public async Task<Boolean> RevokeAccess(AccessKey? key = null)
    {
        try
        {
            Log.Information(RevokeAccessStart,this.Id.ToString());

            return Tool?.RevokeAccess(key) ?? false;
        }
        catch ( Exception _ ) { Log.Error(_,RevokeAccessFail,this.Id.ToString()); await Task.CompletedTask; return false; }
    }

    protected override async Task OnActivateAsync()
    {
        try
        {
            Log.Information(OnActivate,this.Id.ToString());

            await (Tool?.StartHostAsync(key:HostKey) ?? Task.CompletedTask);

            if(Tool?.GetLifeCycleState() is not Active) { throw new OperationFailedException(); }
        }
        catch ( Exception _ ) { Log.Error(_,OnActivateFail,this.Id.ToString()); }
    }

    protected override async Task OnDeactivateAsync()
    {
        try
        {
            Log.Information(OnDeactivate,this.Id.ToString());

            await (Tool?.StopHostAsync(key:HostKey) ?? Task.CompletedTask);

            if(Tool?.GetLifeCycleState() is not InActive) { throw new OperationFailedException(); }
        }
        catch ( Exception _ ) { Log.Error(_,OnDeactivateFail,this.Id.ToString()); }
    }
}