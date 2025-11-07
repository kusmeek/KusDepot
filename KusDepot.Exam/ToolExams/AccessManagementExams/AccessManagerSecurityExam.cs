using KusDepot.Exams.Tools;

namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AccessManagerExam
{
    [Test]
    public void AccessCheck()
    {
        Tool _0 = new();

        var KeyO = new OwnerKey(SerializeCertificate(CreateCertificate(Guid.NewGuid(),"Owner"))!);

        Check.That(KeyO.ID).IsNotNull(); Check.That(_0.TakeOwnership(KeyO)).IsTrue();

        ClientKey? k1 = _0.RequestAccess(new StandardRequest("AccessCheck")) as ClientKey; Check.That(k1).IsNotNull();

        ExecutiveKey? k2 = _0.RequestAccess(new ManagementRequest(KeyO!)) as ExecutiveKey; Check.That(k2).IsNotNull();

        HostKey? k3 = _0.RequestAccess(new HostRequest(null,true)) as HostKey; Check.That(k3).IsNotNull();

        ServiceKey? k4 = _0.RequestAccess(new ServiceRequest(new Tool(),"")) as ServiceKey; Check.That(k4).IsNotNull();

        Check.That(_0.Lock(KeyO)).IsTrue();

        Check.That(_0.AddInput("AccessCheck",k4)).IsTrue();

        Check.That(_0.AddOutput(Guid.NewGuid(),"AccessCheck",k2)).IsTrue();

        Check.That(_0.GetWorkingSet(k1)).IsNull();

        Check.That(_0.GetWorkingSet(k2)).IsNotNull();

        Check.That(_0.GetWorkingSet(k3)).IsNull();

        Check.That(_0.GetWorkingSet(k4)).IsNull();

        Check.That(_0.GetInputs(k1)).IsNull();

        Check.That(_0.GetInputs(k2)).IsNotNull();

        Check.That(_0.GetInputs(k3)).IsNull();

        Check.That(_0.GetInputs(k4)).IsNull();

        Check.That(_0.GetOutputIDs(k1)).IsNotNull();

        Check.That(_0.GetOutputIDs(k2)).IsNotNull();

        Check.That(_0.GetOutputIDs(k3)).IsNull();

        Check.That(_0.GetOutputIDs(k4)).IsNotNull();
    }

    [Test]
    public void GenerateCommandKey()
    {
        var _ = new AccessManager(new Tool());

        var c = new CommandTest();

        var k = _.GenerateCommandKey(c);

        Check.That(k).IsNotNull();

        Check.That(k).IsInstanceOf<CommandKey>();
    }

    public class DenyingAccessManager : AccessManager
    {
        public DenyingAccessManager(ITool? tool = null, ILogger? logger = null, X509Certificate2? certificate = null)
            : base(tool, logger, certificate) { }

        public override Boolean AccessCheck(AccessKey? key = null, [CallerMemberName] String? operationname = null)
        {
            return false;
        }
    }

    [Test]
    public async Task Denying_AccessManager()
    {
        var denyingManager = new DenyingAccessManager();
        var tool = new Tool(accessmanager: denyingManager);
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;

        Check.That(tool.Lock(ownerKey)).IsTrue();

        Boolean activationResult = await tool.Activate(key: executiveKey);

        Check.That(activationResult).IsFalse();
        Check.That(tool.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);
    }

    [Test]
    public async Task RevokeAccess_PreventsFurtherUse()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        Check.That(tool.Lock(ownerKey)).IsTrue();

        Check.That(clientKey).IsNotNull();
        Check.That(await tool.Activate(key:executiveKey)).IsTrue();

        var command = new ToolCommandSecurityExam.TestCommand();
        Check.That(await tool.RegisterCommand("test", command, key: executiveKey)).IsTrue();

        var result = tool.ExecuteCommand(new CommandDetails().SetHandle("test"), key: clientKey);
        Check.That(command.ExecutedSuccessfully).IsTrue();
        command.Reset();

        Check.That(tool.RevokeAccess(clientKey)).IsTrue();

        tool.ExecuteCommand(new CommandDetails().SetHandle("test"), key: clientKey);
        Check.That(command.ExecutedSuccessfully).IsFalse();

        Check.That(await tool.UnRegisterCommand("test", key: executiveKey)).IsTrue();
    }

    [Test]
    public void ExportAndImportState()
    {
        var tool1 = new Tool();
        var ownerKey1 = tool1.CreateOwnerKey("Owner1");
        Check.That(tool1.TakeOwnership(ownerKey1)).IsTrue();
        var executiveKey1 = tool1.RequestAccess(new ManagementRequest(ownerKey1!)) as ExecutiveKey;
        var manager1 = tool1.GetAccessManager(executiveKey1) as AccessManager;
        Check.That(manager1).IsNotNull();

        var clientKey = tool1.RequestAccess(new StandardRequest("Client"));
        Check.That(clientKey).IsNotNull();
        Check.That(tool1.Lock(ownerKey1)).IsTrue();
        Check.That(tool1.AddOutput(Guid.NewGuid(),"AccessCheck",executiveKey1)).IsTrue();
        Check.That(tool1.GetOutputIDs(clientKey)).IsNotNull();

        Check.That(manager1!.ExportAccessManagerState(clientKey)).IsNull();
        var state = manager1.ExportAccessManagerState(executiveKey1);
        Check.That(state).IsNotNull();
        Check.That(state!.ID).IsNotNull().And.Not.Equals(Guid.Empty);
        Check.That(state.AccessKeys).IsNotNull().And.Not.IsEmpty();

        var manager2 = new AccessManager(state);
        var tool2 = new Tool(id:tool1.GetID(),accessmanager:manager2);
        Check.That(tool2.AddOutput(Guid.NewGuid(),"AccessCheck",executiveKey1)).IsTrue();

        var ownerKey2 = tool2.CreateOwnerKey("Owner2");
        Check.That(tool2.TakeOwnership(ownerKey2)).IsTrue();
        Check.That(tool2.Lock(ownerKey2)).IsTrue();

        Check.That(tool2.GetOutputIDs(clientKey)).IsNotNull();
    }

    [Test]
    public void ImportAccessManagementKeys()
    {
        var sharedCertificate = CreateCertificate(Guid.NewGuid(), "SharedCertificate");
        var serializedCert = SerializeCertificate(sharedCertificate);
        var serializedCert2 = serializedCert.CloneByteArray();
        var sharedId = Guid.NewGuid();

        var state1 = new AccessManagerState { ID = sharedId, Certificate = serializedCert };
        var manager1 = new AccessManager(state1);
        var tool1 = new Tool(id: sharedId , accessmanager: manager1);
        var ownerKey1 = tool1.CreateOwnerKey("Owner1");
        Check.That(tool1.TakeOwnership(ownerKey1)).IsTrue();
        var executiveKey1 = tool1.RequestAccess(new ManagementRequest(ownerKey1!)) as ExecutiveKey;
        var clientKey1 = tool1.RequestAccess(new StandardRequest("Client1")) as ClientKey;
        Check.That(clientKey1).IsNotNull();
        Check.That(tool1.Lock(ownerKey1)).IsTrue();
        Check.That(tool1.AddOutput(Guid.NewGuid(), "output1", executiveKey1)).IsTrue();
        Check.That(tool1.GetOutputIDs(clientKey1)).IsNotNull();

        var state2 = new AccessManagerState { ID = sharedId, Certificate = serializedCert2 };
        var manager2 = new AccessManager(state2);
        var tool2 = new Tool(id: sharedId , accessmanager: manager2);
        var ownerKey2 = tool2.CreateOwnerKey("Owner2");
        Check.That(tool2.TakeOwnership(ownerKey2)).IsTrue();
        var executiveKey2 = tool2.RequestAccess(new ManagementRequest(ownerKey2!)) as ExecutiveKey;
        var clientKey2 = tool2.RequestAccess(new StandardRequest("Client2")) as ClientKey;
        Check.That(clientKey2).IsNotNull();
        Check.That(tool2.Lock(ownerKey2)).IsTrue();
        Check.That(tool2.AddOutput(Guid.NewGuid(), "output2", executiveKey2)).IsTrue();
        Check.That(tool2.GetOutputIDs(clientKey2)).IsNotNull();

        var stateToImport = manager1.ExportAccessManagerState(executiveKey1);
        Check.That(stateToImport).IsNotNull();

        Check.That(manager2.ImportAccessManagementKeys(stateToImport!, clientKey2)).IsFalse();
        Check.That(manager2.ImportAccessManagementKeys(stateToImport!, executiveKey2)).IsTrue();

        Check.That(tool2.GetOutputIDs(clientKey1)).IsNotNull();
    }

    [Test]
    public void DestroySecrets()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        Check.That(tool.AddOutput(Guid.NewGuid(), "output1", executiveKey)).IsTrue();
        Check.That(tool.Lock(ownerKey)).IsTrue();

        Check.That(clientKey).IsNotNull();
        Check.That(tool.GetOutputIDs(clientKey)).IsNotNull();

        var manager = tool.GetAccessManager(executiveKey) as AccessManager;
        Check.That(manager).IsNotNull();

        Check.That(manager!.DestroySecrets(clientKey)).IsFalse();
        Check.That(manager.DestroySecrets(executiveKey)).IsTrue();

        Check.That(tool.GetOutputIDs(clientKey)).IsNull();
        Check.That(tool.GetAccessManager(executiveKey)).IsNull();

        var certificateField = typeof(AccessManager).GetField("Certificate", BindingFlags.NonPublic | BindingFlags.Instance);
        Check.That(certificateField).IsNotNull();
        Check.That(certificateField!.GetValue(manager)).IsNull();

        var accessKeysField = typeof(AccessManager).GetField("AccessKeys", BindingFlags.NonPublic | BindingFlags.Instance);
        Check.That(accessKeysField).IsNotNull();
        var accessKeysValue = accessKeysField!.GetValue(manager) as System.Collections.IDictionary;
        Check.That(accessKeysValue).IsNotNull();
        Check.That(accessKeysValue!.Count).IsEqualTo(0);
    }
}