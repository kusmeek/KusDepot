namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolSecurityExam
{
    private ManagerKey? KeyM;

    private OwnerKey? KeyO;

    [OneTimeSetUp]
    public void Calibrate()
    {
        KeyM = new(SerializeCertificate(CreateCertificate(Guid.NewGuid(),"Management"))!);
        KeyO = new(SerializeCertificate(CreateCertificate(Guid.NewGuid(),"Owner"))!);
    }

    [Test]
    public void CheckManager()
    {
        Tool _ = new();

        OwnerKey? KeyO = _.CreateOwnerKey("OwnerKey");

        Check.That(KeyO).IsNotNull();

        ManagerKey? KeyM = _.CreateManagementKey("ManagerKey") as ManagerKey;

        Check.That(KeyM).IsNotNull();

        Check.That(_.Lock(KeyO)).IsTrue();

        Check.That(_.CheckOwner(KeyO)).IsTrue();

        Check.That(_.CheckManager(KeyO)).IsTrue();

        Check.That(_.CheckOwner(new OwnerKey(Array.Empty<Byte>()))).IsFalse();

        Check.That(_.CheckManager(new OwnerKey(Array.Empty<Byte>()))).IsFalse();

        Check.That(_.CheckOwner(KeyM)).IsFalse();

        Check.That(_.CheckManager(KeyM)).IsTrue();

        Check.That(_.CreateManagementKey("ManagerKey")).IsNull();

        Check.That(_.CreateOwnerKey("OwnerKey")).IsNull();

        ExecutiveKey? KeyE = _.RequestAccess(new ManagementRequest(KeyO!)) as ExecutiveKey;

        Check.That(KeyE).IsNotNull();

        Check.That(_.CreateManagementKey("ManagerKey2",KeyE)).IsInstanceOf<ManagerKey>();
    }

    [Test]
    public void CheckOwner()
    {
        Tool _ = new();

        OwnerKey? KeyO = _.CreateOwnerKey("OwnerKey");

        Check.That(KeyO).IsNotNull();

        ManagerKey? KeyM = _.CreateManagementKey("ManagerKey") as ManagerKey;

        Check.That(KeyM).IsNotNull();

        Check.That(_.Lock(KeyO)).IsTrue();

        Check.That(_.CheckOwner(KeyO)).IsTrue();

        Check.That(_.CheckManager(KeyO)).IsTrue();

        Check.That(_.CheckOwner(new OwnerKey(Array.Empty<Byte>()))).IsFalse();

        Check.That(_.CheckManager(new OwnerKey(Array.Empty<Byte>()))).IsFalse();

        Check.That(_.CheckOwner(KeyM)).IsFalse();

        Check.That(_.CheckManager(KeyM)).IsTrue();

        Check.That(_.CreateManagementKey("ManagerKey")).IsNull();

        Check.That(_.CreateOwnerKey("OwnerKey")).IsNull();

        ExecutiveKey? KeyE = _.RequestAccess(new ManagementRequest(KeyO!)) as ExecutiveKey;

        Check.That(KeyE).IsNotNull();

        Check.That(_.CreateManagementKey("ManagerKey2",KeyE)).IsInstanceOf<ManagerKey>();
    }

    [Test]
    public void CreateOwnerKey()
    {
        Tool _ = new();

        OwnerKey? KeyO = _.CreateOwnerKey("OwnerKey");

        Check.That(KeyO).IsNotNull();

        ManagerKey? KeyM = _.CreateManagementKey("ManagerKey") as ManagerKey;

        Check.That(KeyM).IsNotNull();

        Check.That(_.Lock(KeyO)).IsTrue();

        Check.That(_.CheckOwner(KeyO)).IsTrue();

        Check.That(_.CheckManager(KeyO)).IsTrue();

        Check.That(_.CheckOwner(new OwnerKey(Array.Empty<Byte>()))).IsFalse();

        Check.That(_.CheckManager(new OwnerKey(Array.Empty<Byte>()))).IsFalse();

        Check.That(_.CheckOwner(KeyM)).IsFalse();

        Check.That(_.CheckManager(KeyM)).IsTrue();

        Check.That(_.CreateManagementKey("ManagerKey")).IsNull();

        Check.That(_.CreateOwnerKey("OwnerKey")).IsNull();

        ExecutiveKey? KeyE = _.RequestAccess(new ManagementRequest(KeyO!)) as ExecutiveKey;

        Check.That(KeyE).IsNotNull();

        Check.That(_.CreateManagementKey("ManagerKey2",KeyE)).IsInstanceOf<ManagerKey>();
    }

    [Test]
    public void CreateManagementKey()
    {
        Tool _ = new();

        OwnerKey? KeyO = _.CreateOwnerKey("OwnerKey");

        Check.That(KeyO).IsNotNull();

        ManagerKey? KeyM = _.CreateManagementKey("ManagerKey") as ManagerKey;

        Check.That(KeyM).IsNotNull();

        Check.That(_.Lock(KeyO)).IsTrue();

        Check.That(_.CheckOwner(KeyO)).IsTrue();

        Check.That(_.CheckManager(KeyO)).IsTrue();

        Check.That(_.CheckOwner(new OwnerKey(Array.Empty<Byte>()))).IsFalse();

        Check.That(_.CheckManager(new OwnerKey(Array.Empty<Byte>()))).IsFalse();

        Check.That(_.CheckOwner(KeyM)).IsFalse();

        Check.That(_.CheckManager(KeyM)).IsTrue();

        Check.That(_.CreateManagementKey("ManagerKey")).IsNull();

        Check.That(_.CreateOwnerKey("OwnerKey")).IsNull();

        ExecutiveKey? KeyE = _.RequestAccess(new ManagementRequest(KeyO!)) as ExecutiveKey;

        Check.That(KeyE).IsNotNull();

        Check.That(_.CreateManagementKey("ManagerKey2",KeyE)).IsInstanceOf<ManagerKey>();
    }

    [Test]
    public void GetAccessManager()
    {
        Tool005 _0 = new();

        Check.That(_0.GetAccessManager()).IsNotNull();
    }

    [Test]
    public void GetLocked()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }

    [Test]
    public void Lock()
    {
        Tool _0 = new Tool();

        Check.That(_0.TakeOwnership(KeyO)).IsTrue();

        Check.That(_0.TakeOwnership(KeyM)).IsFalse();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyM)).IsFalse();

        Check.That(_0.UnLock(KeyO)).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyM)).IsFalse();

        Check.That(_0.Lock(KeyO)).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyO)).IsTrue();
    }

    [Test]
    public void RegisterManager()
    {
        Tool _ = new();

        OwnerKey? KeyO = _.CreateOwnerKey("OwnerKey");

        Check.That(KeyO).IsNotNull();

        ManagerKey? KeyM = new(Utility.CreateCertificate(_,"ManagerKey")!);

        Check.That(KeyM).IsNotNull();

        Check.That(_.Lock(KeyO)).IsTrue();

        ExecutiveKey? KeyE = _.RequestAccess(new ManagementRequest(KeyO!)) as ExecutiveKey;

        Check.That(KeyE).IsNotNull();

        Check.That(_.CheckManager(KeyM)).IsFalse();

        Check.That(_.RegisterManager(KeyM)).IsFalse();

        Check.That(_.RegisterManager(KeyM,KeyE)).IsTrue();

        Check.That(_.RegisterManager(KeyM,KeyE)).IsTrue();

        Check.That(_.CheckManager(KeyM)).IsTrue();

        Check.That(_.UnRegisterManager(KeyM)).IsFalse();

        Check.That(_.UnRegisterManager(KeyM,KeyE)).IsTrue();

        Check.That(_.CheckManager(KeyM)).IsFalse();
    }

    [Test]
    public void ReleaseOwnership()
    {
        Tool _0 = new Tool();

        Check.That(_0.TakeOwnership(KeyO)).IsTrue();

        Check.That(_0.TakeOwnership(KeyM)).IsFalse();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyM)).IsFalse();

        Check.That(_0.UnLock(KeyO)).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyM)).IsFalse();

        Check.That(_0.Lock(KeyO)).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyO)).IsTrue();
    }

    [Test]
    public void RequestAccess()
    {
        Tool _0 = new();

        Check.That(_0.RequestAccess(new ManagementRequest(KeyO!))).IsNull();

        Check.That(_0.TakeOwnership(KeyO)).IsTrue();

        Check.That(_0.RequestAccess(new StandardRequest("RequestAccess"))).IsInstanceOf<ClientKey>();

        Check.That(_0.RequestAccess(new ManagementRequest(KeyM!))).IsNull();

        Check.That(_0.RequestAccess(new ManagementRequest(KeyO!))).IsInstanceOf<ExecutiveKey>();

        Check.That(_0.RequestAccess(new HostRequest(null,true))).IsInstanceOf<HostKey>();

        Check.That(_0.RequestAccess(new HostRequest(null,false))).IsNull();

        Check.That(_0.RequestAccess(new ServiceRequest(new Tool(),""))).IsInstanceOf<ServiceKey>();

        Check.That(_0.RequestAccess(new ServiceRequest(_0,""))).IsInstanceOf<ExecutiveKey>();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.RequestAccess(new HostRequest(null,true))).IsNull();

        Check.That(_0.RequestAccess(new HostRequest(null,false))).IsNull();

        Check.That(_0.RequestAccess(new HostRequest(new Tool(),true))).IsNull();

        Check.That(_0.RequestAccess(new HostRequest(new Tool(),false))).IsNull();

        Check.That(_0.RequestAccess(new ManagementRequest(KeyM!))).IsInstanceOf<ExecutiveKey>();

        Check.That(_0.RequestAccess(new ManagementRequest(KeyO!))).IsInstanceOf<ExecutiveKey>();

        Check.That(_0.RequestAccess(new ServiceRequest(_0,""))).IsNull();

        Check.That(_0.RequestAccess(new ServiceRequest(new Tool(),""))).IsNull();

        Check.That(_0.RequestAccess(new StandardRequest("RequestAccess"))).IsNull();
    }

    [Test]
    public void RevokeAccess()
    {
        Tool _0 = new();

        Check.That(_0.TakeOwnership(KeyO)).IsTrue();

        ClientKey? k1 = _0.RequestAccess(new StandardRequest("RequestAccess")) as ClientKey; Check.That(k1).IsNotNull();

        ExecutiveKey? k2 = _0.RequestAccess(new ManagementRequest(KeyO!)) as ExecutiveKey; Check.That(k2).IsNotNull();

        HostKey? k3 = _0.RequestAccess(new HostRequest(null,true)) as HostKey; Check.That(k3).IsNotNull();

        ServiceKey? k4 = _0.RequestAccess(new ServiceRequest(new Tool(),"")) as ServiceKey; Check.That(k4).IsNotNull();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.RevokeAccess(new ClientKey(Array.Empty<Byte>()))).IsFalse();

        Check.That(_0.RevokeAccess(k1)).IsTrue();

        Check.That(_0.RevokeAccess(k2)).IsTrue();

        Check.That(_0.RevokeAccess(k3)).IsTrue();

        Check.That(_0.RevokeAccess(k4)).IsTrue();
    }

    [Test]
    public void TakeOwnership()
    {
        Tool _0 = new Tool();

        Check.That(_0.TakeOwnership(KeyO)).IsTrue();

        Check.That(_0.TakeOwnership(KeyM)).IsFalse();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyM)).IsFalse();

        Check.That(_0.UnLock(KeyO)).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyM)).IsFalse();

        Check.That(_0.Lock(KeyO)).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyO)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        Tool _0 = new Tool();

        Check.That(_0.TakeOwnership(KeyO)).IsTrue();

        Check.That(_0.TakeOwnership(KeyM)).IsFalse();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyM)).IsFalse();

        Check.That(_0.UnLock(KeyO)).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyM)).IsFalse();

        Check.That(_0.Lock(KeyO)).IsTrue();

        Check.That(_0.ReleaseOwnership(KeyO)).IsTrue();
    }

    [Test]
    public void UnRegisterManager()
    {
        Tool _ = new();

        OwnerKey? KeyO = _.CreateOwnerKey("OwnerKey");

        Check.That(KeyO).IsNotNull();

        ManagerKey? KeyM = new(Utility.CreateCertificate(_,"ManagerKey")!);

        Check.That(KeyM).IsNotNull();

        Check.That(_.Lock(KeyO)).IsTrue();

        ExecutiveKey? KeyE = _.RequestAccess(new ManagementRequest(KeyO!)) as ExecutiveKey;

        Check.That(KeyE).IsNotNull();

        Check.That(_.CheckManager(KeyM)).IsFalse();

        Check.That(_.RegisterManager(KeyM)).IsFalse();

        Check.That(_.RegisterManager(KeyM,KeyE)).IsTrue();

        Check.That(_.RegisterManager(KeyM,KeyE)).IsTrue();

        Check.That(_.CheckManager(KeyM)).IsTrue();

        Check.That(_.UnRegisterManager(KeyM)).IsFalse();

        Check.That(_.UnRegisterManager(KeyM,KeyE)).IsTrue();

        Check.That(_.CheckManager(KeyM)).IsFalse();
    }

    [Test]
    public async Task ActivateDeactivate_Security()
    {
        Tool _tool = new();
        OwnerKey? _ownerKey = _tool.CreateOwnerKey("Owner");
        Check.That(_tool.TakeOwnership(_ownerKey)).IsTrue();
        ExecutiveKey? _executiveKey = _tool.RequestAccess(new ManagementRequest(_ownerKey!)) as ExecutiveKey;
        ClientKey? _clientKey = _tool.RequestAccess(new StandardRequest("Client")) as ClientKey;

        Check.That(_tool!.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(await _tool.Activate()).IsTrue();
        Check.That(_tool.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(await _tool.Activate()).IsFalse();

        Check.That(await _tool.Deactivate()).IsTrue();
        Check.That(_tool.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
        Check.That(await _tool.Deactivate()).IsFalse();

        Check.That(_tool.Lock(_ownerKey)).IsTrue();

        Check.That(await _tool.Activate()).IsFalse();
        Check.That(await _tool.Activate(key: _clientKey)).IsFalse();
        Check.That(_tool.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(await _tool.Activate(key: _executiveKey)).IsTrue();
        Check.That(_tool.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await _tool.Deactivate()).IsFalse();
        Check.That(await _tool.Deactivate(key: _clientKey)).IsFalse();
        Check.That(_tool.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        Check.That(await _tool.Deactivate(key: _executiveKey)).IsTrue();
        Check.That(_tool.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);

        Check.That(_tool.UnLock(_ownerKey)).IsTrue();
    }

    [Test]
    public async Task DestroySecrets()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        var managerKey = tool.CreateManagementKey("Manager");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        Check.That(tool.RegisterManager(managerKey)).IsTrue();
        Check.That(tool.AddOutput(Guid.NewGuid(),"DestroySecrets")).IsTrue();
        var command = new ToolCommandSecurityExam.TestCommand();
        Check.That(await tool.RegisterCommand("test", command)).IsTrue();
        var clientKey = tool.RequestAccess(new StandardRequest("Client"));
        Check.That(tool.Lock(ownerKey)).IsTrue();

        Check.That(tool.CheckOwner(ownerKey)).IsTrue();
        Check.That(tool.CheckManager(managerKey)).IsTrue();
        Check.That(tool.GetOutputIDs(clientKey)).IsNotNull();

        Func<Tool, List<Byte[]>> getSecrets = t => (typeof(Common).GetField("Secrets", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(t) as List<Byte[]>)!;
        Func<Tool, Byte[]> getOwnerSecret = t => (typeof(Tool).GetField("OwnerSecret", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(t) as Byte[])!;
        Func<Tool, IDictionary> getCommandKeys = t => (typeof(Tool).GetField("CommandKeys", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(t) as IDictionary)!;
        Func<Tool, IAccessManager> getAccessManager = t => (typeof(Tool).GetField("AccessManager", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(t) as IAccessManager)!;
        Func<IAccessManager, IDictionary> getAccessManagerKeys = am => (typeof(AccessManager).GetField("AccessKeys", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(am) as IDictionary)!;

        Check.That(getOwnerSecret(tool)).IsNotNull();
        Check.That(getSecrets(tool)).IsNotNull().And.Not.IsEmpty();
        Check.That(getCommandKeys(tool)).IsNotNull().And.Not.IsEmpty();
        var accessManager = getAccessManager(tool);
        Check.That(accessManager).IsNotNull();
        Check.That(getAccessManagerKeys(accessManager!)).IsNotNull().And.Not.IsEmpty();

        var destroySecretsMethod = typeof(Tool).GetMethod("DestroySecrets", BindingFlags.NonPublic | BindingFlags.Instance);
        Check.That(destroySecretsMethod).IsNotNull();

        var resultFalse = (Boolean)destroySecretsMethod!.Invoke(tool, new Object?[] { new ManagerKey(Array.Empty<Byte>()) })!;
        Check.That(resultFalse).IsFalse();
        Check.That(getSecrets(tool)).IsNotNull().And.Not.IsEmpty();

        var resultTrue = (Boolean)destroySecretsMethod!.Invoke(tool, new Object?[] { ownerKey })!;
        Check.That(resultTrue).IsTrue();

        Check.That(tool.CheckOwner(ownerKey)).IsFalse();
        Check.That(tool.CheckManager(managerKey)).IsFalse();
        Check.That(tool.GetOutputIDs(clientKey)).IsNull();
        Check.That(getOwnerSecret(tool)).IsNull();
        Check.That(getSecrets(tool)).IsNotNull().And.IsEmpty();
        Check.That(getCommandKeys(tool)).IsNotNull().And.IsEmpty();
        Check.That(getAccessManagerKeys(accessManager!)).IsNotNull().And.IsEmpty();
    }
}