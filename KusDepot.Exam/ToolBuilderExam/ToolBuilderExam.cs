namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolBuilderExam
{
    [Test]
    public void Build()
    {
        Check.That(ToolBuilderFactory.CreateBuilder().Build().GetType().IsAssignableTo(typeof(ITool))).IsTrue();
    }

    [Test]
    public void BuildOpen()
    {
        Check.That(ToolBuilderFactory.CreateBuilder().Build<Tool>().GetType().IsAssignableTo(typeof(ITool))).IsTrue();
    }

    [Test]
    public async Task BuildAsync()
    {
        Check.That((await ToolBuilderFactory.CreateBuilder().BuildAsync()).GetType().IsAssignableTo(typeof(ITool))).IsTrue();
    }

    [Test]
    public async Task BuildAsyncOpen()
    {
        Check.That((await ToolBuilderFactory.CreateBuilder().BuildAsync<Tool>()).GetType().IsAssignableTo(typeof(ITool))).IsTrue();
    }

    [Test]
    public async Task EnableAllCommands()
    {
        var _ = ToolBuilderFactory.CreateBuilder()
            .RegisterCommand("OK",typeof(CommandTestE))
            .RegisterCommand("FAIL",typeof(CommandEnableFail))
            .EnableAllCommands().AutoStart().Build();

        Check.That(_.ExecuteCommand(new(){Handle = "OK"})).HasAValue();

        Check.That(_.ExecuteCommand(new(){Handle = "FAIL"})).HasNoValue();
    }

    [Test]
    public async Task RegisterCommandInstance()
    {
        var _ = ToolBuilderFactory.CreateBuilder().RegisterCommand("Work",new CommandTestR()).Build();

        Check.That(await _.Activate()).IsTrue();

        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).IsNotNull();
    }

    [Test]
    public void RegisterToolTypeBulk()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool<Tool>(3,i => $"tool-{i}")
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();

        Check.That(hosted!.ContainsKey("tool-0")).IsTrue();
        Check.That(hosted.ContainsKey("tool-1")).IsTrue();
        Check.That(hosted.ContainsKey("tool-2")).IsTrue();

        Check.That(hosted["tool-0"]).IsInstanceOf<Tool>();
        Check.That(hosted["tool-1"]).IsInstanceOf<Tool>();
        Check.That(hosted["tool-2"]).IsInstanceOf<Tool>();
    }

    [Test]
    public async Task RegisterCommandGenericType_WithArguments()
    {
        var tool = ToolBuilderFactory.CreateBuilder()
            .RegisterCommand<CommandArgsProbe>("Work",new Object?[] { 7 , "ok" , true })
            .Build();

        Check.That(await tool.Activate()).IsTrue();
        Check.That(tool.ExecuteCommand(new(){ Handle = "Work" })).HasAValue();
    }

    [Test]
    public async Task RegisterCommandType_WithArguments()
    {
        var tool = ToolBuilderFactory.CreateBuilder()
            .RegisterCommand("Work",typeof(CommandArgsProbe),new Object?[] { 7 , "ok" , true })
            .Build();

        Check.That(await tool.Activate()).IsTrue();
        Check.That(tool.ExecuteCommand(new(){ Handle = "Work" })).HasAValue();
    }

    [Test]
    public async Task RegisterCommandFactory()
    {
        var _ = ToolBuilderFactory.CreateBuilder()
            .RegisterCommand("Work",() => new CommandTestR())
            .Build();

        Check.That(await _.Activate()).IsTrue();

        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).IsNotNull();
    }

    [Test]
    public async Task RegisterCommand_DefaultPermissions_CommandKey_NoExecute()
    {
        var cmd = new CommandTestE();
        var tool = ToolBuilderFactory.CreateBuilder()
            .RegisterCommand("P1",cmd,null)
            .EnableAllCommands().Build();
        var k = tool.CreateManagementKey("KM");

        Check.That(await tool.Activate()).IsTrue();
        Check.That(tool.Lock(k)).IsTrue();

        var ck = GetCommandKey(cmd);
        Check.That(ck).IsNotNull();

        Check.That(tool.ExecuteCommand(new(){ Handle = "P1" },ck)).IsNull();
    }

    [Test]
    public async Task RegisterCommand_CustomPermissions_CommandKey_CanExecute()
    {
        var cmd = new CommandTestE();
        var permissions = ImmutableArray.Create(new Int32[]
        {
            ProtectedOperation.ExecuteCommand,
            ProtectedOperation.AddActivity,
            ProtectedOperation.AddOutput,
            ProtectedOperation.RemoveActivity,
        });
        var tool = ToolBuilderFactory.CreateBuilder()
            .RegisterCommand("P2",cmd,permissions)
            .EnableAllCommands().Build();
        var k = tool.CreateManagementKey("KM");

        Check.That(await tool.Activate()).IsTrue();
        Check.That(tool.Lock(k)).IsTrue();

        var ck = GetCommandKey(cmd);
        Check.That(ck).IsNotNull();

        Check.That(tool.ExecuteCommand(new(){ Handle = "P2" },ck)).HasAValue();
    }

    [Test]
    public async Task RegisterTool_DefaultPermissions_MyHostKey_NoDeactivate()
    {
        using Tool000 child = CreateTool000();
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(child,name:"perm-default")
            .Build() as Tool ?? throw new InvalidOperationException();

        Check.That(await host.Activate()).IsTrue();
        var k = host.CreateManagementKey("KM"); Check.That(host.Lock(k)).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        var keyDefault = GetProtectedField<AccessKey>(child,"MyHostKey");
        Check.That(keyDefault).IsNotNull();

        Check.That(await host.Deactivate(key:keyDefault)).IsFalse();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test]
    public async Task RegisterTool_CustomPermissions_MyHostKey_CanDeactivate()
    {
        using Tool000 child = CreateTool000();
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(child,name:"perm-exec",ProtectedOperationSets.Executive)
            .Build() as Tool ?? throw new InvalidOperationException();

        Check.That(await host.Activate()).IsTrue();
        var k = host.CreateManagementKey("KM"); Check.That(host.Lock(k)).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        var keyExec = GetProtectedField<AccessKey>(child,"MyHostKey");
        Check.That(keyExec).IsNotNull();

        Check.That(await host.Deactivate(key:keyExec)).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public async Task RegisterToolTypeBulk_PermissionsFactory_AppliesPerIndex()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool<Tool000>(2,i => $"tool-{i}",i => Equals(i,1) ? ProtectedOperationSets.Executive : ImmutableArray<Int32>.Empty)
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("tool-0")).IsTrue();
        Check.That(hosted.ContainsKey("tool-1")).IsTrue();

        var child0 = hosted["tool-0"] as Tool000;
        var child1 = hosted["tool-1"] as Tool000;
        Check.That(child0).IsNotNull();
        Check.That(child1).IsNotNull();

        Check.That(await host.Activate()).IsTrue();
        var k = host.CreateManagementKey("KM"); Check.That(host.Lock(k)).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        var key0 = GetProtectedField<AccessKey>(child0!,"MyHostKey");
        var key1 = GetProtectedField<AccessKey>(child1!,"MyHostKey");
        Check.That(key0).IsNotNull();
        Check.That(key1).IsNotNull();

        Check.That(await host.Deactivate(key:key0)).IsFalse();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await host.Deactivate(key:key1)).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public void RegisterService_Instance_AppearsInHostedServices()
    {
        var bg = new BackgroundServce006();

        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterService("bg-inst",bg)
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("bg-inst")).IsTrue();
        Check.That(ReferenceEquals(hosted["bg-inst"],bg)).IsTrue();
    }

    [Test]
    public void RegisterServiceFactory_AppearsInHostedServices()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterService("bg-fact",() => new BackgroundServce006())
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("bg-fact")).IsTrue();
        Check.That(hosted["bg-fact"]).IsInstanceOf<BackgroundServce006>();
    }

    [Test]
    public void RegisterServiceFactory_NullResult_Ignored()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterService("null-fact",() => (IHostedService?)null)
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted is null || hosted.ContainsKey("null-fact") is false).IsTrue();
    }

    [Test]
    public void RegisterServiceFactory_DuplicateName_Ignored()
    {
        var bg1 = new BackgroundServce006();

        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterService("dup-sf",bg1)
            .RegisterService("dup-sf",() => new BackgroundServce006())
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("dup-sf")).IsTrue();
        Check.That(ReferenceEquals(hosted["dup-sf"],bg1)).IsTrue();
    }

    [Test]
    public void RegisterService_Type_AppearsInHostedServices()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterService("bg-type",typeof(BackgroundServce006))
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("bg-type")).IsTrue();
        Check.That(hosted["bg-type"]).IsInstanceOf<BackgroundServce006>();
    }

    [Test]
    public void RegisterService_GenericType_AppearsInHostedServices()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterService<BackgroundServce006>("bg-gen")
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("bg-gen")).IsTrue();
        Check.That(hosted["bg-gen"]).IsInstanceOf<BackgroundServce006>();
    }

    [Test]
    public void RegisterService_DuplicateName_Ignored()
    {
        var bg1 = new BackgroundServce006();
        var bg2 = new BackgroundServce006();

        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterService("bg-dup",bg1)
            .RegisterService("bg-dup",bg2)
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("bg-dup")).IsTrue();
        Check.That(ReferenceEquals(hosted["bg-dup"],bg1)).IsTrue();
    }

    [Test]
    public void RegisterService_InvalidType_Ignored()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterService("bad",typeof(String))
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted is null || hosted.ContainsKey("bad") is false).IsTrue();
    }

    [Test]
    public void RegisterService_CoexistsWithRegisterTool()
    {
        using Tool000 child = CreateTool000();

        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(child,name:"tool-a")
            .RegisterService<BackgroundServce006>("svc-a")
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("tool-a")).IsTrue();
        Check.That(hosted["tool-a"]).IsInstanceOf<Tool000>();
        Check.That(hosted.ContainsKey("svc-a")).IsTrue();
        Check.That(hosted["svc-a"]).IsInstanceOf<BackgroundServce006>();
    }

    [Test]
    public void RegisterService_CoexistsWithConfigureServices()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterService<BackgroundServce006>("named-svc")
            .ConfigureServices((x,s) => s.AddHostedService<BackgroundServce006>())
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("named-svc")).IsTrue();
        Check.That(hosted["named-svc"]).IsInstanceOf<BackgroundServce006>();

        var unnamed = hosted.Where(_ => !String.Equals(_.Key,"named-svc",StringComparison.Ordinal)).ToList();
        Check.That(unnamed.Count).IsEqualTo(1);
        Check.That(unnamed[0].Value).IsInstanceOf<BackgroundServce006>();
        Check.That(Guid.TryParse(unnamed[0].Key,out _)).IsTrue();
    }

    [Test]
    public void RegisterToolFactory_AppearsInHostedServices()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(() => new Tool000(),name:"fact-tool")
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("fact-tool")).IsTrue();
        Check.That(hosted["fact-tool"]).IsInstanceOf<Tool000>();
    }

    [Test]
    public void RegisterToolFactory_NullResult_Ignored()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(() => (ITool?)null,name:"null-fact")
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted is null || hosted.ContainsKey("null-fact") is false).IsTrue();
    }

    [Test]
    public void RegisterToolFactory_DuplicateName_Ignored()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(() => new Tool000(),name:"dup-fact")
            .RegisterTool(() => new Tool000(),name:"dup-fact")
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("dup-fact")).IsTrue();
    }

    [Test]
    public void RegisterToolType_AppearsInHostedServices()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(typeof(Tool000),name:"type-tool")
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("type-tool")).IsTrue();
        Check.That(hosted["type-tool"]).IsInstanceOf<Tool000>();
    }

    [Test]
    public void RegisterToolType_InvalidType_Ignored()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(typeof(String),name:"bad-type")
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted is null || hosted.ContainsKey("bad-type") is false).IsTrue();
    }

    [Test]
    public void RegisterToolType_DuplicateName_Ignored()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(typeof(Tool000),name:"dup-type")
            .RegisterTool(typeof(Tool000),name:"dup-type")
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("dup-type")).IsTrue();
    }

    [Test]
    public void RegisterToolType_WithArguments()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(typeof(ToolArgsProbe),new Object?[] { 42 , "probe" },name:"args-tool")
            .Build() as Tool ?? throw new InvalidOperationException();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("args-tool")).IsTrue();
        Check.That(hosted["args-tool"]).IsInstanceOf<ToolArgsProbe>();

        var probe = hosted["args-tool"] as ToolArgsProbe;
        Check.That(probe!.Configured).IsTrue();
    }

    [Test]
    public async Task RegisterToolFactory_CustomPermissions_MyHostKey_CanDeactivate()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(() => CreateTool000(),name:"fact-perm",ProtectedOperationSets.Executive)
            .Build() as Tool ?? throw new InvalidOperationException();

        Check.That(await host.Activate()).IsTrue();
        var k = host.CreateManagementKey("KM"); Check.That(host.Lock(k)).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        var hosted = GetHostedServicesMap(host);
        var child = hosted!["fact-perm"] as Tool000;
        Check.That(child).IsNotNull();

        var keyExec = GetProtectedField<AccessKey>(child!,"MyHostKey");
        Check.That(keyExec).IsNotNull();

        Check.That(await host.Deactivate(key:keyExec)).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public async Task RegisterToolType_CustomPermissions_MyHostKey_CanDeactivate()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool(typeof(Tool000),name:"type-perm",permissions:ProtectedOperationSets.Executive)
            .Build() as Tool ?? throw new InvalidOperationException();

        Check.That(await host.Activate()).IsTrue();
        var k = host.CreateManagementKey("KM"); Check.That(host.Lock(k)).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        var hosted = GetHostedServicesMap(host);
        var child = hosted!["type-perm"] as Tool000;
        Check.That(child).IsNotNull();

        var keyExec = GetProtectedField<AccessKey>(child!,"MyHostKey");
        Check.That(keyExec).IsNotNull();

        Check.That(await host.Deactivate(key:keyExec)).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    private static Dictionary<String,IHostedService>? GetHostedServicesMap(Tool host)
        => host.GetType().GetField("HostedServices",BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(host) as Dictionary<String,IHostedService>;

    private static T? GetProtectedField<T>(Object obj , String fieldName) where T : class
    {
        return obj.GetType().GetField(fieldName,BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj) as T;
    }

    private static Tool000 CreateTool000(Boolean enablemyexceptions = true)
    {
        Tool000 t = new(new TestAccessManager());
        if(enablemyexceptions) { t.EnableMyExceptions(); }
        return t;
    }

    private static CommandKey? GetCommandKey(Command command)
        => typeof(Command).GetField("Key",BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(command) as CommandKey;
}

public sealed class CommandArgsProbe : Command
{
    private readonly Boolean _configured;

    public CommandArgsProbe() { _configured = false; }

    public CommandArgsProbe(Int32 count , String name , Boolean enabled)
    {
        _configured = count == 7 && String.Equals(name,"ok",StringComparison.Ordinal) && enabled;
    }

    public override Guid? Execute(Activity? activity , CommandKey? key = null)
    {
        if(_configured is false) { return null; }

        return activity?.ID ?? Guid.CreateVersion7();
    }
}

public sealed class ToolArgsProbe : Tool
{
    public Boolean Configured { get; }

    public ToolArgsProbe() { Configured = false; }

    public ToolArgsProbe(Int32 count , String name)
    {
        Configured = count == 42 && String.Equals(name,"probe",StringComparison.Ordinal);
    }
}