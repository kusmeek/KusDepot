namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.None)]
public class ToolCommandSecurityExam
{
    private Tool? _tool;
    private OwnerKey? _ownerKey;
    private ExecutiveKey? _executiveKey;
    private ClientKey? _clientKey;
    private TestCommand? _testCommand;
    private const String TestCommandHandle = "TestCommand";

    public class TestCommand : Command
    {
        public Boolean ExecutedSuccessfully { get; private set; }

        public override Guid? Execute(Activity? activity = null , AccessKey? key = null)
        {
            if(AccessCheck(key) is false)
            {
                ExecutedSuccessfully = false;
                return null;
            }

            ExecutedSuccessfully = true;
            return activity!.ID;
        }

        public void Reset() { ExecutedSuccessfully = false; }
    }

    [SetUp]
    public void Setup()
    {
        _tool = new Tool();
        _ownerKey = _tool.CreateOwnerKey("Owner");
        Check.That(_tool.TakeOwnership(_ownerKey)).IsTrue();
        _executiveKey = _tool.RequestAccess(new ManagementRequest(_ownerKey!)) as ExecutiveKey;
        _clientKey = _tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        _testCommand = new TestCommand();

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
        Check.That(_tool.GetLifeCycleState()).IsEqualTo(LifeCycleState.UnInitialized);

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
}
