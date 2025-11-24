using Serilog;
using Serilog.Extensions.Logging;

namespace KusDepot.ToolGrains;

[Reentrant]
public partial class TestToolGrain : Grain , ITestToolGrain
{
    protected ITool? Tool;

    protected HostKey? HostKey;

    public TestToolGrain([TransactionalState("KusDepotCab")] ITransactionalState<KusDepotCab> cab)
    {
        try
        {
            SetupLogging();

            Tool = ToolBuilderFactory.CreateBuilder()

                .ConfigureServices((x,s) =>
                {
                    s.AddSingletonWithInterfaces(this);

                    s.AddSingletonWithInterfaces(cab);
                })

                .ConfigureTool((x,t) =>
                {
                    HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey;
                })

                .UseAccessManager<TestAccessManager>().Seal()

                .RegisterCommand<CommandTest>("CommandTest")

                .UseLogger(new SerilogLoggerFactory())

                .Build();
        }
        catch ( Exception _ ) { Log.Fatal(_,ToolGrainFail,this.GetPrimaryKeyString()); throw; }
    }

    public async Task<Guid?> ExecuteCommand(CommandDetails? details , AccessKey? key = null)
    {
        try
        {
            Log.Information(ExecutingCommand,this.GetPrimaryKeyString(),details?.Handle,details?.ID);

            return Tool?.ExecuteCommand(details,key);
        }
        catch ( Exception _ ) { Log.Error(_,ExecuteCommandFail); await Task.CompletedTask; return null; }
    }

    public async Task<Guid?> ExecuteCommandCab(KusDepotCab? cab , AccessKey? key = null)
    {
        CommandDetails? d = cab?.GetCommandDetails();

        try { return await this.ExecuteCommand(d,key); }

        catch ( Exception _ ) { Log.Error(_,ExecuteCommandFail,this.GetPrimaryKeyString(),d?.Handle,d?.ID); return null; }
    }

    public async Task<Object?> GetOutput(Guid? id , AccessKey? key = null)
    {
        try
        {
            if( id is null || Equals(Guid.Empty,id) || key is null ) { return null; }

            Log.Information(GetOutputStart,this.GetPrimaryKeyString(),id);

            return Tool?.GetOutput(id,key);
        }
        catch ( Exception _ ) { Log.Error(_,GetOutputFail,this.GetPrimaryKeyString(),id); await Task.CompletedTask; return null; }
    }

    public async Task<AccessKey?> RequestAccess(AccessRequest? request = null)
    {
        try
        {
            Log.Information(RequestAccessStart,this.GetPrimaryKeyString());

            return Tool?.RequestAccess(request);
        }
        catch ( Exception _ ) { Log.Error(_,RequestAccessFail,this.GetPrimaryKeyString()); await Task.CompletedTask; return null; }
    }

    public async Task<Boolean> RevokeAccess(AccessKey? key = null)
    {
        try
        {
            Log.Information(RevokeAccessStart,this.GetPrimaryKeyString());

            return Tool?.RevokeAccess(key) ?? false;
        }
        catch ( Exception _ ) { Log.Error(_,RevokeAccessFail,this.GetPrimaryKeyString()); await Task.CompletedTask; return false; }
    }

    public override async Task OnActivateAsync(CancellationToken cancel)
    {
        try
        {
            Log.Information(OnActivate,this.GetPrimaryKeyString());

            await (Tool?.StartHostAsync(cancel:cancel,key:HostKey) ?? Task.CompletedTask);

            if(Tool?.GetLifeCycleState() is not Active) { throw new OperationFailedException(); }
        }
        catch ( Exception _ ) { Log.Error(_,OnActivateFail,this.GetPrimaryKeyString()); }
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason , CancellationToken cancel)
    {
        try
        {
            if(reason.Exception is not null) { throw reason.Exception; }

            Log.Information(OnDeactivate,this.GetPrimaryKeyString(),Enum.GetName(reason.ReasonCode),reason.Description);

            await (Tool?.StopHostAsync(cancel:cancel,key:HostKey) ?? Task.CompletedTask);

            if(Tool?.GetLifeCycleState() is not InActive) { throw new OperationFailedException(); }
        }
        catch ( Exception _ ) { Log.Error(_,OnDeactivateFail,this.GetPrimaryKeyString(),Enum.GetName(reason.ReasonCode),reason.Description,reason.Exception?.ToString()); }
    }
}