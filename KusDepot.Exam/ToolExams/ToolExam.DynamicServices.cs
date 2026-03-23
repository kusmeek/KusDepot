namespace KusDepot.Exams.Tools;

[TestFixture]
public partial class ToolExam
{
    [Test]
    public async Task AddHostedService_ActiveStart()
    {
        using Tool host = CreateHost();
        Check.That(await host.StartHostAsync()).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        using Tool001 child = new(new TestAccessManager());

        Check.That(await host.AddHostedService(child,start:true)).IsTrue();
        Check.That(host.IsHosting(child)).IsTrue();
        Check.That(Tool.GetHost(child)).IsSameReferenceAs(host);

        AssertHostKeySet(child);

        AssertHostingKeyPresent(host,child);

        Check.That(await host.RemoveHostedService(child,stop:true)).IsTrue();
        Check.That(host.IsHosting(child)).IsFalse();

        AssertHostKeyCleared(child);
    }

    [Test]
    public async Task AddHostedService_Background()
    {
        using Tool host = CreateHost();

        Check.That(await host.AddHostedService(new BackgroundServce006(),start:true)).IsTrue();

        Check.That(await host.StartHostAsync()).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        var bg = host.GetHostedServices()?.OfType<BackgroundServce006>().FirstOrDefault();
        Check.That(bg).IsNotNull();
        Check.That(host.IsHosting(bg)).IsTrue();
    }

    [Test]
    public async Task AddHostedService_Type_NameProvided_CreatesAndRemoves()
    {
        using Tool host = CreateHost();

        Check.That(await host.AddHostedService(typeof(Tool000),name:"type-a",start:false)).IsTrue();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("type-a")).IsTrue();
        Check.That(hosted["type-a"]).IsInstanceOf<Tool000>();

        Check.That(await host.RemoveHostedService("type-a",stop:false)).IsTrue();
    }

    [Test]
    public async Task AddHostedService_Type_Arguments_MultiTypeConstructor_Succeeds()
    {
        using Tool host = CreateHost();

        Guid id = Guid.NewGuid();
        TimeSpan delay = TimeSpan.FromMilliseconds(25);

        Check.That(await host.AddHostedService(typeof(ConstructorArgsBackgroundService),name:"type-ctor",arguments:new Object?[] { 7 , "svc" , true , id , delay },start:false)).IsTrue();

        var svc = host.GetHostedService("type-ctor") as ConstructorArgsBackgroundService;
        Check.That(svc).IsNotNull();
        Check.That(svc!.Count).IsEqualTo(7);
        Check.That(svc.Name).IsEqualTo("svc");
        Check.That(svc.Online).IsTrue();
        Check.That(svc.Id).IsEqualTo(id);
        Check.That(svc.Delay).IsEqualTo(delay);

        Check.That(await host.RemoveHostedService("type-ctor",stop:false)).IsTrue();
    }

    [Test]
    public async Task AddHostedService_Type_InvalidType_ReturnsFalse()
    {
        using Tool host = CreateHost();

        Check.That(await host.AddHostedService(typeof(String),name:"bad-type",start:false)).IsFalse();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted is null || hosted.ContainsKey("bad-type") is false).IsTrue();
    }

    [Test]
    public async Task AddHostedService_DuplicateAdd_IdempotentKeying()
    {
        using Tool host = CreateHost();

        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();

        var keys = GetProtectedField<Dictionary<ITool,HostKey>>(host,"HostingKeys");
        Check.That(keys).IsNotNull();
        Check.That(keys!.ContainsKey(t)).IsTrue();

        var k1 = keys[t];
        var childHostKey1 = GetProtectedField<AccessKey>(t,"MyHostKey");
        Check.That(childHostKey1).IsNotNull();

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();

        var keys2 = GetProtectedField<Dictionary<ITool,HostKey>>(host,"HostingKeys");
        Check.That(keys2).IsNotNull();
        Check.That(keys2!.ContainsKey(t)).IsTrue();
        Check.That(keys2[t]).IsSameReferenceAs(k1);

        var childHostKey2 = GetProtectedField<AccessKey>(t,"MyHostKey");
        Check.That(childHostKey2).IsSameReferenceAs(childHostKey1);

        Check.That(await host.RemoveHostedService(t,stop:false)).IsTrue();
    }

    [Test]
    public async Task AddHostedService_Fail()
    {
        using Tool host = CreateHost();

        Check.That(host.DisableMyExceptions()).IsTrue();

        using StartFaultService child = new(new TestAccessManager());

        Check.That(await host.StartHostAsync()).IsTrue();

        Check.That(await host.AddHostedService(child,start:true)).IsFalse();
        Check.That(host.IsHosting(child)).IsFalse();

        AssertHostKeyCleared(child);

        AssertHostingKeyAbsent(host,child);
    }

    [Test]
    public async Task AddHostedService_LockOwnership_ExternalLockPreserved()
    {
        using Tool host = CreateHost();

        using Tool000 t = CreateTool000();
        var ak = t.CreateManagementKey("t");
        Check.That(t.RegisterManager(ak)).IsTrue();
        Check.That(t.Lock(ak)).IsTrue();
        Check.That(t.GetLocked()).IsTrue();

        Check.That(await host.AddHostedService(t,start:true)).IsTrue();
        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(t.GetLocked()).IsTrue();

        Check.That(await host.RemoveHostedService(t,stop:false)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(t.GetLocked()).IsTrue();

        AssertHostKeyCleared(t);
    }

    [Test]
    public async Task AddHostedService_LockOwnership_HostManagedLockRemoved()
    {
        using Tool host = CreateHost();

        using Tool000 t = CreateTool000();
        Check.That(t.GetLocked()).IsFalse();

        Check.That(await host.AddHostedService(t,start:true)).IsTrue();
        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(t.GetLocked()).IsTrue();

        AssertHostKeySet(t);

        Check.That(await host.RemoveHostedService(t,stop:false)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(t.GetLocked()).IsFalse();

        AssertHostKeyCleared(t);
    }

    [Test]
    public async Task AddHostedService_LockedStart()
    {
        using Tool host = CreateHost();

        using Tool000 t0 = CreateTool000();
        var ak = t0.CreateManagementKey("t0");
        Check.That(t0.RegisterManager(ak)).IsTrue();
        Check.That(t0.Lock(ak)).IsTrue();

        Check.That(await host.AddHostedService(t0,start:true)).IsTrue();
        Check.That(t0.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(t0.GetLocked()).IsTrue();

        var bg = new BackgroundServce006();
        Check.That(await host.AddHostedService(bg,start:false)).IsTrue();

        using Tool000 t1 = CreateTool000();

        Check.That(await host.AddHostedService(t1,start:false)).IsTrue();
        Check.That(t1.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.StartHostAsync()).IsTrue(); Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(t1.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(t1.GetLocked()).IsTrue();

        var t2 = new Tool001(new TestAccessManager());
        Check.That(t2.RegisterManager(ak)).IsTrue();
        Check.That(t2.Lock(ak)).IsTrue();

        Check.That(await host.AddHostedService(t2,start:true)).IsTrue();

        Check.That(t2.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await host.RemoveHostedService(bg,stop:true)).IsTrue();
        Check.That(host.IsHosting(bg)).IsFalse();

        Check.That(t0.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(await host.RemoveHostedService(t0,stop:true)).IsTrue();
        Check.That(t0.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(await host.RemoveHostedService(t1,stop:false)).IsTrue();
        Check.That(t1.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(t1.GetLocked()).IsFalse();

        Check.That(host.IsHosting(t0)).IsFalse();
        Check.That(host.IsHosting(t1)).IsFalse();

        Check.That(await host.StopHostAsync()).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
        Check.That(t2.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public async Task AddHostedService_MyHostKeyStored()
    {
        using Tool host = CreateHost();
        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();

        var childHostKey = GetProtectedField<AccessKey>(t,"MyHostKey");
        Check.That(childHostKey).IsInstanceOf<MyHostKey>();

        var myHostKeys = GetHostingMyHostKeys(host);
        Check.That(myHostKeys).IsNotNull();
        Check.That(myHostKeys!.ContainsKey(t)).IsTrue();
        Check.That(myHostKeys[t]).Equals(childHostKey);

        Check.That(await host.RemoveHostedService(t,stop:false)).IsTrue();
    }

    [Test]
    public async Task AddHostedService_StartFalse_RemoveHostedService_StopFalse_ExternalLocked_ActiveHost()
    {
        using Tool host = CreateHost();
        Check.That(await host.StartHostAsync()).IsTrue();

        using Tool000 t = CreateTool000();
        var ak = t.CreateManagementKey("t");
        Check.That(t.RegisterManager(ak)).IsTrue();
        Check.That(t.Lock(ak)).IsTrue();

        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();
        Check.That(host.IsHosting(t)).IsTrue();
        AssertHostKeySet(t);
        AssertHostingKeyPresent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
        Check.That(t.GetLocked()).IsTrue();

        Check.That(await host.RemoveHostedService(t,stop:false)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();
        AssertHostKeyCleared(t);
        AssertHostingKeyAbsent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
        Check.That(t.GetLocked()).IsTrue();
    }

    [Test]
    public async Task AddHostedService_StartFalse_RemoveHostedService_StopFalse_Unlocked_ActiveHost()
    {
        using ToolServiceControl host = CreateToolServiceControl();
        Check.That(await host.StartHostAsync()).IsTrue();

        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();
        Check.That(host.IsHosting(t)).IsTrue();
        AssertHostKeySet(t);
        AssertHostingKeyPresent(host,t);

        Check.That(t.GetLocked()).IsTrue();

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.StartHosted(t)).IsTrue();
        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await host.RemoveHostedService(t,stop:false)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();
        AssertHostKeyCleared(t);
        AssertHostingKeyAbsent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(t.GetLocked()).IsFalse();
    }

    [Test]
    public async Task AddHostedService_StartFalse_RemoveHostedService_StopFalse_Unlocked_InactiveHost()
    {
        using Tool host = CreateHost();

        using Tool000 t = CreateTool000();

        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();
        AssertHostKeySet(t);
        AssertHostingKeyPresent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
        Check.That(t.GetLocked()).IsTrue();

        Check.That(await host.RemoveHostedService(t,stop:false)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();
        AssertHostKeyCleared(t);
        AssertHostingKeyAbsent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
        Check.That(t.GetLocked()).IsFalse();
    }

    [Test]
    public async Task AddHostedService_StartFalse_RemoveHostedService_StopTrue_ExternalLocked_InactiveHost()
    {
        using Tool host = CreateHost();

        using Tool000 t = CreateTool000();
        var ak = t.CreateManagementKey("t");
        Check.That(t.RegisterManager(ak)).IsTrue();
        Check.That(t.Lock(ak)).IsTrue();

        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();
        AssertHostKeySet(t);
        AssertHostingKeyPresent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
        Check.That(t.GetLocked()).IsTrue();

        Check.That(await host.RemoveHostedService(t,stop:true)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();
        AssertHostKeyCleared(t);
        AssertHostingKeyAbsent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
        Check.That(t.GetLocked()).IsTrue();
    }

    [Test]
    public async Task AddHostedService_StartFalse_RemoveHostedService_StopTrue_Unlocked_ActiveHost()
    {
        using ToolServiceControl host = CreateToolServiceControl();
        Check.That(await host.StartHostAsync()).IsTrue();

        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();
        AssertHostKeySet(t);
        AssertHostingKeyPresent(host,t);

        Check.That(t.GetLocked()).IsTrue();
        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.StartHosted(t)).IsTrue();
        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await host.RemoveHostedService(t,stop:true)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();
        AssertHostKeyCleared(t);
        AssertHostingKeyAbsent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
        Check.That(t.GetLocked()).IsFalse();
    }

    [Test]
    public async Task AddHostedService_StartFalse_RemoveHostedService_StopTrue_Unlocked_InactiveHost()
    {
        using Tool host = CreateHost();

        using Tool000 t = CreateTool000();

        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();
        AssertHostKeySet(t);
        AssertHostingKeyPresent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
        Check.That(t.GetLocked()).IsTrue();

        Check.That(await host.RemoveHostedService(t,stop:true)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();
        AssertHostKeyCleared(t);
        AssertHostingKeyAbsent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
        Check.That(t.GetLocked()).IsFalse();
    }

    [Test]
    public async Task AddHostedService_StartHost()
    {
        using Tool host = CreateHost();
        using Tool child = new(new TestAccessManager());

        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
        Check.That(child.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.AddHostedService(child,start:false)).IsTrue();
        Check.That(child.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.StartHostAsync()).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(child.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await host.StopHostAsync()).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
        Check.That(child.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(await host.StartHostAsync()).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(child.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
    }

    [Test]
    public async Task AddHostedService_StartTrue_RemoveHostedService_StopFalse_Unlocked_ActiveHost()
    {
        using Tool host = CreateHost();
        Check.That(await host.StartHostAsync()).IsTrue();

        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,start:true)).IsTrue();
        Check.That(host.IsHosting(t)).IsTrue();
        AssertHostKeySet(t);
        AssertHostingKeyPresent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(t.GetLocked()).IsTrue();

        Check.That(await host.RemoveHostedService(t,stop:false)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();
        AssertHostKeyCleared(t);
        AssertHostingKeyAbsent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(t.GetLocked()).IsFalse();
    }

    [Test]
    public async Task AddHostedService_StartTrue_RemoveHostedService_StopTrue_Unlocked_ActiveHost()
    {
        using Tool host = CreateHost();
        Check.That(await host.StartHostAsync()).IsTrue();

        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,start:true)).IsTrue();
        Check.That(host.IsHosting(t)).IsTrue();
        AssertHostKeySet(t);
        AssertHostingKeyPresent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(t.GetLocked()).IsTrue();

        Check.That(await host.RemoveHostedService(t,stop:true)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();
        AssertHostKeyCleared(t);
        AssertHostingKeyAbsent(host,t);

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
        Check.That(t.GetLocked()).IsFalse();
    }

    [Test]
    public async Task HostedServices_MaskEnforced_ThenUnmaskedVisible()
    {
        using Tool host = CreateHost(false);
        var mk = host.CreateManagementKey("host");
        var ek = host.RequestAccess(new ManagementRequest(mk!)) as ExecutiveKey;

        using Tool000 t = new(new TestAccessManager());
        t.EnableMyExceptions();

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();

        Check.That(host.Lock(mk)).IsTrue();

        Check.That(host.IsHosting(t)).IsFalse();

        Check.That(host.UnMaskHostedServices(ek)).IsTrue();
        Check.That(host.IsHosting(t)).IsTrue();

        Check.That(await host.RemoveHostedService(t,stop:false,key:ek)).IsTrue();
    }

    [Test]
    public async Task RemoveHostedService_Background()
    {
        using Tool host = CreateHost();

        var bg = new BackgroundServce006();

        Check.That(await host.StartHostAsync()).IsTrue();
        Check.That(await host.AddHostedService(bg,start:true)).IsTrue();
        Check.That(host.IsHosting(bg)).IsTrue();

        Check.That(await host.RemoveHostedService(bg,stop:true)).IsTrue();
        Check.That(host.IsHosting(bg)).IsFalse();
    }

    [Test]
    public async Task RemoveHostedService_MyHostKeyRemoved()
    {
        using Tool host = CreateHost();
        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();
        Check.That(await host.RemoveHostedService(t,stop:false)).IsTrue();

        AssertHostingMyHostKeyAbsent(host,t);
    }

    [Test]
    public async Task RemoveHostedService_StopTrue_InActiveHost_Fails_ThenStopFalseRemoves()
    {
        using Tool host = CreateHost();

        var s = new StopFaultService();

        Check.That(host.DisableMyExceptions()).IsTrue();

        Check.That(await host.StartHostAsync()).IsTrue();
        Check.That(await host.AddHostedService(s,start:true)).IsTrue();
        Check.That(await host.StopHostAsync()).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(host.IsHosting(s)).IsTrue();

        Check.That(await host.RemoveHostedService(s,stop:true)).IsFalse();
        Check.That(host.IsHosting(s)).IsTrue();

        Check.That(await host.RemoveHostedService(s,stop:false)).IsTrue();
        Check.That(host.IsHosting(s)).IsFalse();
    }

    [Test]
    public async Task RemoveHostedService_StopTrue_InitializedHost_Fails_ThenStopFalseRemoves()
    {
        using Tool host = CreateHost();

        Check.That(host.DisableMyExceptions()).IsTrue();

        var s = new StopFaultService();

        Check.That(await host.AddHostedService(s,start:false)).IsTrue();
        Check.That(host.IsHosting(s)).IsTrue();

        Check.That(await host.RemoveHostedService(s,stop:true)).IsFalse();
        Check.That(host.IsHosting(s)).IsTrue();

        Check.That(await host.RemoveHostedService(s,stop:false)).IsTrue();
        Check.That(host.IsHosting(s)).IsFalse();
    }

    [Test]
    public async Task RemoveHostedService_StopTrue_Tool_StopFails_NoPartialCleanup_StopFalseRemoves()
    {
        using Tool host = CreateHost();
        Check.That(host.DisableMyExceptions()).IsTrue();
        Check.That(await host.StartHostAsync()).IsTrue();

        using ToolStopFail t = new();

        Check.That(await host.AddHostedService(t,start:true)).IsTrue();
        Check.That(host.IsHosting(t)).IsTrue();

        AssertHostKeySet(t);
        AssertHostingKeyPresent(host,t);

        Check.That(await host.RemoveHostedService(t,stop:true)).IsFalse();

        Check.That(host.IsHosting(t)).IsTrue();
        AssertHostKeySet(t);
        AssertHostingKeyPresent(host,t);

        Check.That(t.GetLocked()).IsTrue();

        Check.That(await host.RemoveHostedService(t,stop:false)).IsTrue();

        Check.That(host.IsHosting(t)).IsFalse();
        AssertHostKeyCleared(t);
        AssertHostingKeyAbsent(host,t);

        Check.That(t.GetLocked()).IsFalse();
    }

    [Test]
    public async Task AddHostedService_StartTrue_Timeout_FailsFast_StopFalseRemoves()
    {
        using ToolHostingOptionsControl host = new(startup: TimeSpan.FromMilliseconds(50));
        Check.That(host.AddInstance()).IsTrue();
        Check.That(host.UnMaskHostedServices()).IsTrue();

        var s = new StartHangService();

        Check.That(await host.StartHostAsync()).IsTrue();
        Check.That(await host.AddHostedService(s,start:true)).IsFalse();
        Check.That(host.IsHosting(s)).IsFalse();
    }

    [Test]
    public async Task RemoveHostedService_StopTrue_Timeout_FailsFast_StopFalseRemoves()
    {
        using ToolHostingOptionsControl host = new(TimeSpan.FromMilliseconds(50));
        Check.That(host.AddInstance()).IsTrue();
        Check.That(host.UnMaskHostedServices()).IsTrue();

        var s = new StopHangService();

        Check.That(await host.StartHostAsync()).IsTrue();
        Check.That(await host.AddHostedService(s,start:true)).IsTrue();
        Check.That(host.IsHosting(s)).IsTrue();

        Check.That(await host.RemoveHostedService(s,stop:true)).IsFalse();
        Check.That(host.IsHosting(s)).IsTrue();

        Check.That(await host.RemoveHostedService(s,stop:false)).IsTrue();
        Check.That(host.IsHosting(s)).IsFalse();
    }

    [Test]
    public async Task RemoveHostedService_StopTrue_WhenHostInactive()
    {
        using Tool host = CreateHost();

        using Tool000 t = CreateTool000();

        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.AddHostedService(t,start:false)).IsTrue();
        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.RemoveHostedService(t,stop:true)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();

        Check.That(t.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
        Check.That(t.GetLocked()).IsFalse();

        var childHostKeyAfter = GetProtectedField<AccessKey>(t,"MyHostKey");
        Check.That(childHostKeyAfter).IsNull();
    }

    [Test]
    public async Task ResolveHostedServices_KeyedAndUnkeyedRegistered_AssignsKeys()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool<Tool000>("tool-keyed")
            .ConfigureServices((x,s) => s.AddHostedService<BackgroundServce006>())
            .Build() as Tool ?? throw new InvalidOperationException();

        host.EnableMyExceptions();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("tool-keyed")).IsTrue();
        Check.That(hosted["tool-keyed"]).IsInstanceOf<Tool000>();

        var unkeyed = hosted.Where(_ => !String.Equals(_.Key,"tool-keyed",StringComparison.Ordinal)).ToList();
        Check.That(unkeyed.Count).IsEqualTo(1);
        Check.That(unkeyed[0].Value).IsInstanceOf<BackgroundServce006>();
        Check.That(Guid.TryParse(unkeyed[0].Key,out _)).IsTrue();
    }

    [Test]
    public async Task ResolveHostedServices_UnkeyedOnly_AssignsGuidKey()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .ConfigureServices((x,s) => s.AddHostedService<BackgroundServce006>())
            .Build() as Tool ?? throw new InvalidOperationException();

        host.EnableMyExceptions();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.Count).IsEqualTo(1);

        var only = hosted.First();
        Check.That(only.Value).IsInstanceOf<BackgroundServce006>();
        Check.That(Guid.TryParse(only.Key,out _)).IsTrue();
    }

    [Test]
    public async Task StartStopHostedService_ByKey_KeyedToolService()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool<Tool000>("tool-keyed")
            .Build() as Tool ?? throw new InvalidOperationException();

        host.EnableMyExceptions();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.TryGetValue("tool-keyed",out IHostedService? service)).IsTrue();
        Check.That(service).IsInstanceOf<Tool000>();

        var child = service as Tool000;
        Check.That(child).IsNotNull();
        Check.That(child!.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.StartHostedService("tool-keyed")).IsTrue();
        Check.That(child.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await host.StopHostedService("tool-keyed")).IsTrue();
        Check.That(child.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public async Task StartStopHostedService_ByUnknownKey_ReturnsFalse()
    {
        using Tool host = CreateHost();

        Check.That(await host.StartHostedService("missing-key")).IsFalse();
        Check.That(await host.StopHostedService("missing-key")).IsFalse();
        Check.That(await host.StartHostedService(String.Empty)).IsFalse();
        Check.That(await host.StopHostedService(String.Empty)).IsFalse();
    }

    [Test]
    public async Task StartHostedService_ByKey_StartHangService_TimeoutReturnsFalse()
    {
        using ToolHostingOptionsControl host = new(startup: TimeSpan.FromMilliseconds(50));
        Check.That(host.AddInstance()).IsTrue();
        Check.That(host.UnMaskHostedServices()).IsTrue();

        var s = new StartHangService();

        Check.That(await host.AddHostedService(s,start:false)).IsTrue();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();

        var k = hosted!.FirstOrDefault(_ => ReferenceEquals(_.Value,s)).Key;
        Check.That(String.IsNullOrEmpty(k)).IsFalse();

        Check.That(await host.StartHostedService(k)).IsFalse();
        Check.That(host.IsHosting(s)).IsTrue();
    }

    [Test]
    public async Task StopHostedService_ByKey_StopHangService_TimeoutReturnsFalse()
    {
        using ToolHostingOptionsControl host = new(TimeSpan.FromMilliseconds(50));
        Check.That(host.AddInstance()).IsTrue();
        Check.That(host.UnMaskHostedServices()).IsTrue();

        var s = new StopHangService();

        Check.That(await host.AddHostedService(s,start:true)).IsTrue();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();

        var k = hosted!.FirstOrDefault(_ => ReferenceEquals(_.Value,s)).Key;
        Check.That(String.IsNullOrEmpty(k)).IsFalse();

        Check.That(await host.StopHostedService(k)).IsFalse();
        Check.That(host.IsHosting(s)).IsTrue();
    }

    [Test]
    public async Task HostedService_StartByInstance_StopByKey_Parity()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool<Tool000>("tool-keyed")
            .Build() as Tool ?? throw new InvalidOperationException();

        host.EnableMyExceptions();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.TryGetValue("tool-keyed",out IHostedService? service)).IsTrue();
        Check.That(service).IsInstanceOf<Tool000>();

        var child = service as Tool000;
        Check.That(child).IsNotNull();

        Check.That(await host.StartHostedService(service)).IsTrue();
        Check.That(child!.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await host.StopHostedService("tool-keyed")).IsTrue();
        Check.That(child.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public async Task HostedService_StartByKey_StopByInstance_Parity()
    {
        using Tool host = ToolBuilderFactory.CreateBuilder()
            .RegisterTool<Tool000>("tool-keyed")
            .Build() as Tool ?? throw new InvalidOperationException();

        host.EnableMyExceptions();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.TryGetValue("tool-keyed",out IHostedService? service)).IsTrue();
        Check.That(service).IsInstanceOf<Tool000>();

        var child = service as Tool000;
        Check.That(child).IsNotNull();
        Check.That(child!.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        Check.That(await host.StartHostedService("tool-keyed")).IsTrue();
        Check.That(child.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await host.StopHostedService(service)).IsTrue();
        Check.That(child.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public async Task AddHostedService_Factory()
    {
        using Tool host = CreateHost();

        Check.That(await host.AddHostedService(() => new Tool000(new TestAccessManager()),name:"factory-svc",start:false)).IsTrue();

        var byName = host.GetHostedService("factory-svc");
        Check.That(byName).IsNotNull();
        Check.That(byName).IsInstanceOf<Tool000>();

        Check.That(await host.RemoveHostedService("factory-svc",stop:false)).IsTrue();
    }

    private static T? GetProtectedField<T>(Object obj , String fieldName) where T : class
    {
        return obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj) as T;
    }

    private static Tool CreateHost(Boolean unmask = true)
    {
        Tool host = new();
        host.EnableMyExceptions();
        Check.That(host.AddInstance()).IsTrue();
        if(unmask) { Check.That(host.UnMaskHostedServices()).IsTrue(); }
        return host;
    }

    private static ToolServiceControl CreateToolServiceControl(Boolean unmask = true)
    {
        ToolServiceControl host = new();
        Check.That(host.AddInstance()).IsTrue();
        if(unmask) { Check.That(host.UnMaskHostedServices()).IsTrue(); }
        return host;
    }

    private static Tool000 CreateTool000(Boolean enablemyexceptions = true)
    {
        Tool000 t = new(new TestAccessManager());
        if(enablemyexceptions) { t.EnableMyExceptions(); }
        return t;
    }

    private static void AssertHostKeySet(ITool tool)
    {
        var k = GetProtectedField<AccessKey>(tool,"MyHostKey");
        Check.That(k).IsNotNull();
    }

    private static void AssertHostKeyCleared(ITool tool)
    {
        var k = GetProtectedField<AccessKey>(tool,"MyHostKey");
        Check.That(k).IsNull();
    }

    private static Dictionary<ITool,HostKey>? GetHostingKeys(Tool host) => GetProtectedField<Dictionary<ITool,HostKey>>(host,"HostingKeys");

    private static Dictionary<ITool,MyHostKey>? GetHostingMyHostKeys(Tool host) => GetProtectedField<Dictionary<ITool,MyHostKey>>(host,"HostingMyHostKeys");

    private static void AssertHostingKeyPresent(Tool host , ITool tool)
    {
        var hk = GetHostingKeys(host);
        Check.That(hk).IsNotNull();
        Check.That(hk!.ContainsKey(tool)).IsTrue();
        Check.That(hk[tool]).IsInstanceOf<HostKey>();
    }

    private static void AssertHostingKeyAbsent(Tool host , ITool tool)
    {
        var hk = GetHostingKeys(host);
        Check.That(hk is null || hk.ContainsKey(tool) is false).IsTrue();
    }

    private static void AssertHostingMyHostKeyAbsent(Tool host , ITool tool)
    {
        var mhk = GetHostingMyHostKeys(host);
        Check.That(mhk is null || mhk.ContainsKey(tool) is false).IsTrue();
    }

    private static Dictionary<String,IHostedService>? GetHostedServicesMap(Tool host)
        => GetProtectedField<Dictionary<String,IHostedService>>(host,"HostedServices");

    [Test]
    public async Task AddHostedService_NameProvided_RemoveByName_Succeeds()
    {
        using Tool host = CreateHost();
        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,name:"svc-a",start:false)).IsTrue();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.ContainsKey("svc-a")).IsTrue();
        Check.That(ReferenceEquals(hosted["svc-a"],t)).IsTrue();

        Check.That(await host.RemoveHostedService("svc-a",stop:false)).IsTrue();
        Check.That(host.IsHosting(t)).IsFalse();
    }

    [Test]
    public async Task AddHostedService_NameDuplicateSameInstance_ReturnsTrue()
    {
        using Tool host = CreateHost();
        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,name:"svc-a",start:false)).IsTrue();
        Check.That(await host.AddHostedService(t,name:"svc-a",start:false)).IsTrue();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.Count).IsEqualTo(1);

        Check.That(await host.RemoveHostedService("svc-a",stop:false)).IsTrue();
    }

    [Test]
    public async Task AddHostedService_NameDuplicateDifferentInstance_ReturnsFalse()
    {
        using Tool host = CreateHost();
        using Tool000 t1 = CreateTool000();
        using Tool000 t2 = CreateTool000();

        Check.That(await host.AddHostedService(t1,name:"svc-a",start:false)).IsTrue();
        Check.That(await host.AddHostedService(t2,name:"svc-a",start:false)).IsFalse();

        Check.That(await host.RemoveHostedService("svc-a",stop:false)).IsTrue();
    }

    [Test]
    public async Task AddHostedService_SameInstanceDifferentName_ReturnsFalse()
    {
        using Tool host = CreateHost();
        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,name:"svc-a",start:false)).IsTrue();
        Check.That(await host.AddHostedService(t,name:"svc-b",start:false)).IsFalse();

        var hosted = GetHostedServicesMap(host);
        Check.That(hosted).IsNotNull();
        Check.That(hosted!.Count).IsEqualTo(1);
        Check.That(hosted.ContainsKey("svc-a")).IsTrue();

        Check.That(await host.RemoveHostedService("svc-a",stop:false)).IsTrue();
    }

    [Test]
    public async Task RemoveHostedService_ByName_MissingOrEmpty_ReturnsFalse()
    {
        using Tool host = CreateHost();
        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,name:"svc-a",start:false)).IsTrue();

        Check.That(await host.RemoveHostedService(String.Empty,stop:false)).IsFalse();
        Check.That(await host.RemoveHostedService("missing",stop:false)).IsFalse();

        Check.That(await host.RemoveHostedService("svc-a",stop:false)).IsTrue();
    }

    [Test]
    public async Task RemoveHostedService_ByName_OrdinalSensitive()
    {
        using Tool host = CreateHost();
        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,name:"Svc-A",start:false)).IsTrue();

        Check.That(await host.RemoveHostedService("svc-a",stop:false)).IsFalse();
        Check.That(await host.RemoveHostedService("Svc-A",stop:false)).IsTrue();
    }

    [Test]
    public async Task GetHostedServiceNames_ReturnsRegisteredNames()
    {
        using Tool host = CreateHost();
        using Tool000 t0 = CreateTool000();
        using Tool000 t1 = CreateTool000();

        Check.That(await host.AddHostedService(t0,name:"svc-a",start:false)).IsTrue();
        Check.That(await host.AddHostedService(t1,name:"svc-b",start:false)).IsTrue();

        var names = host.GetHostedServiceNames();
        Check.That(names).IsNotNull();
        Check.That(names!).Contains("svc-a","svc-b");

        Check.That(await host.RemoveHostedService("svc-a",stop:false)).IsTrue();
        Check.That(await host.RemoveHostedService("svc-b",stop:false)).IsTrue();
    }

    [Test]
    public async Task GetHostedService_ByName_ReturnsInstance_AndMissingNull()
    {
        using Tool host = CreateHost();
        using Tool000 t = CreateTool000();

        Check.That(await host.AddHostedService(t,name:"svc-a",start:false)).IsTrue();

        var byName = host.GetHostedService("svc-a");
        Check.That(byName).IsNotNull();
        Check.That(ReferenceEquals(byName,t)).IsTrue();

        Check.That(host.GetHostedService("missing")).IsNull();
        Check.That(host.GetHostedService(String.Empty)).IsNull();

        Check.That(await host.RemoveHostedService("svc-a",stop:false)).IsTrue();
    }

    [Test]
    public async Task AddHostedService_Permissions_ControlsMyHostKeyAccess()
    {
        using Tool host = CreateHost();
        Check.That(await host.Activate()).IsTrue();
        var k = host.CreateManagementKey("KM"); Check.That(host.Lock(k)).IsTrue();
        var ke = host.CreateExecutiveKey(k); Check.That(ke).IsNotNull();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        using Tool000 childDefault = CreateTool000();

        Check.That(await host.AddHostedService(childDefault,name:"perm-default",start:false,key:ke)).IsTrue();

        var keyDefault = GetProtectedField<AccessKey>(childDefault,"MyHostKey");
        Check.That(keyDefault).IsNotNull();

        Check.That(await host.Deactivate(key:keyDefault)).IsFalse();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await host.RemoveHostedService("perm-default",stop:false,key:ke)).IsTrue();

        using Tool000 childExec = CreateTool000();

        Check.That(await host.AddHostedService(childExec,name:"perm-exec",permissions:ProtectedOperationSets.Executive,start:false,key:ke)).IsTrue();

        var keyExec = GetProtectedField<AccessKey>(childExec,"MyHostKey");
        Check.That(keyExec).IsNotNull();

        Check.That(await host.Deactivate(key:keyExec)).IsTrue();
        Check.That(host.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }
}