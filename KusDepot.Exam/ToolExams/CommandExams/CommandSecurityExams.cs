namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.None)]
public class CommandSecurityExam
{
    private Tool? _tool;
    private OwnerKey? _ownerKey;
    private ExecutiveKey? _executiveKey;
    private ClientKey? _clientKey;
    private SecurityCommand? _testCommand;
    private const String TestCommandHandle = "SecurityCommand";

    [SetUp]
    public void Setup()
    {
        _tool = new Tool();
        _ownerKey = _tool.CreateOwnerKey("Owner");
        Check.That(_tool.TakeOwnership(_ownerKey)).IsTrue();
        _executiveKey = _tool.RequestAccess(new ManagementRequest(_ownerKey!)) as ExecutiveKey;
        _clientKey = _tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        _testCommand = new SecurityCommand();

        Check.That(_ownerKey).IsNotNull(); Check.That(_executiveKey).IsNotNull();
        Check.That(_clientKey).IsNotNull(); Check.That(_testCommand).IsNotNull();
    }

    [Test]
    public async Task RegisterCommand_Security()
    {
        Check.That(await _tool!.Activate()).IsTrue();
        Check.That(_tool!.Lock(_ownerKey)).IsTrue();

        Check.That(await _tool.RegisterCommand(TestCommandHandle,_testCommand!)).IsFalse();

        Check.That(await _tool.RegisterCommand(TestCommandHandle,_testCommand!,key: _clientKey)).IsFalse();

        Check.That(await _tool.RegisterCommand(TestCommandHandle,_testCommand!,key: _executiveKey)).IsTrue();

        Check.That(await _tool.UnRegisterCommand(TestCommandHandle,key: _executiveKey)).IsTrue();

        Check.That(_tool.UnLock(_ownerKey)).IsTrue();
    }

    [Test]
    public async Task ExecuteCommand_FailsOnInactiveTool()
    {
        Check.That(await _tool!.RegisterCommand(TestCommandHandle, _testCommand!, key: _executiveKey)).IsTrue();
        Check.That(_tool.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);

        _tool.ExecuteCommand(new CommandDetails(handle: TestCommandHandle), key: _clientKey);
        Check.That(_testCommand!.ExecutedSuccessfully).IsFalse();

        _tool.ExecuteCommand(new CommandDetails(handle: TestCommandHandle), key: _executiveKey);
        Check.That(_testCommand!.ExecutedSuccessfully).IsFalse();

        Check.That(await _tool.UnRegisterCommand(TestCommandHandle, key: _executiveKey)).IsTrue();
    }

    [Test]
    public async Task ExecuteCommand_SucceedsOnActiveTool()
    {
        Check.That(await _tool!.RegisterCommand(TestCommandHandle, _testCommand!, key: _executiveKey)).IsTrue();
        
        Check.That(await _tool.Activate()).IsTrue();
        Check.That(_tool.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);

        _tool.ExecuteCommand(new CommandDetails(handle: TestCommandHandle), key: _clientKey);
        Check.That(_testCommand!.ExecutedSuccessfully).IsTrue();
        _testCommand.Reset();

        _tool.ExecuteCommand(new CommandDetails(handle: TestCommandHandle), key: _executiveKey);
        Check.That(_testCommand!.ExecutedSuccessfully).IsTrue();

        Check.That(await _tool.UnRegisterCommand(TestCommandHandle,key: _executiveKey)).IsTrue();
    }

    [Test]
    public async Task RegisterCommand_AttachFails_Unlocked()
    {
        var cmd = new CommandAttachFail();

        Check.That(await _tool!.RegisterCommand("AttachFail",cmd,key: _executiveKey)).IsFalse();
        Check.That(_tool.GetCommands()?.ContainsKey("AttachFail") ?? false).IsFalse();
        Check.That(cmd.GetLocked()).IsFalse();

        Check.That(await _tool.RegisterCommand("AttachFail",_testCommand!,key: _executiveKey)).IsTrue();
        Check.That(await _tool.UnRegisterCommand("AttachFail",key: _executiveKey)).IsTrue();
    }

    [Test]
    public async Task RegisterCommand_EnableFails_Unlocked()
    {
        var cmd = new CommandEnableFail();

        Check.That(await _tool!.RegisterCommand("EnableFail",cmd,enable:true,key: _executiveKey)).IsFalse();
        Check.That(_tool.GetCommands()?.ContainsKey("EnableFail") ?? false).IsFalse();
        Check.That(cmd.GetLocked()).IsFalse();

        Check.That(await _tool.RegisterCommand("EnableFail",_testCommand!,key: _executiveKey)).IsTrue();
        Check.That(await _tool.UnRegisterCommand("EnableFail",key: _executiveKey)).IsTrue();
    }

    [Test]
    public async Task RegisterCommand_AttachFails_Locked()
    {
        var cmd = new CommandAttachFail();

        Check.That(_tool!.Lock(_ownerKey)).IsTrue();

        Check.That(await _tool.RegisterCommand("AttachFailLocked",cmd,key: _executiveKey)).IsFalse();
        Check.That(_tool.GetCommands()?.ContainsKey("AttachFailLocked") ?? false).IsFalse();
        Check.That(cmd.GetLocked()).IsFalse();

        Check.That(_tool.UnLock(_ownerKey)).IsTrue();
    }

    [Test]
    public async Task RegisterCommand_EnableFails_Locked()
    {
        var cmd = new CommandEnableFail();

        Check.That(_tool!.Lock(_ownerKey)).IsTrue();

        Check.That(await _tool.RegisterCommand("EnableFailLocked",cmd,enable:true,key: _executiveKey)).IsFalse();
        Check.That(_tool.GetCommands()?.ContainsKey("EnableFailLocked") ?? false).IsFalse();
        Check.That(cmd.GetLocked()).IsFalse();

        Check.That(_tool.UnLock(_ownerKey)).IsTrue();
    }

    [Test]
    public async Task RegisterCommand_LockFails_Unlocked()
    {
        var cmd = new CommandLockFail();

        Check.That(await _tool!.RegisterCommand("LockFail",cmd,key: _executiveKey)).IsFalse();
        Check.That(_tool.GetCommands()?.ContainsKey("LockFail") ?? false).IsFalse();
        Check.That(cmd.GetLocked()).IsFalse();

        Check.That(await _tool.RegisterCommand("LockFail",_testCommand!,key: _executiveKey)).IsTrue();
        Check.That(await _tool.UnRegisterCommand("LockFail",key: _executiveKey)).IsTrue();
    }
}
