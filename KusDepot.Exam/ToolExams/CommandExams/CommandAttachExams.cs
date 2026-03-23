namespace KusDepot.Exams.Tools;

[TestFixture]
public partial class CommandExams
{
    [Test]
    public async Task RegisterCommand_AttachFails()
    {
        Tool _0 = new Tool();
        var cmd = new CommandAttachFail();

        Check.That(await _0.RegisterCommand("AttachFail",cmd)).IsFalse();
        Check.That(_0.GetCommands()?.ContainsKey("AttachFail") ?? false).IsFalse();
        Check.That(cmd.GetLocked()).IsFalse();
    }

    [Test]
    public async Task RegisterCommand_EnableFails()
    {
        Tool _0 = new Tool();
        var cmd = new CommandEnableFail();

        Check.That(await _0.RegisterCommand("EnableFail",cmd,enable:true)).IsFalse();
        Check.That(_0.GetCommands()?.ContainsKey("EnableFail") ?? false).IsFalse();
        Check.That(cmd.GetLocked()).IsFalse();
    }

    [Test]
    public async Task RegisterCommand_LockFails()
    {
        Tool _0 = new Tool();
        var cmd = new CommandLockFail();

        Check.That(await _0.RegisterCommand("LockFail",cmd)).IsFalse();
        Check.That(_0.GetCommands()?.ContainsKey("LockFail") ?? false).IsFalse();
        Check.That(cmd.GetLocked()).IsFalse();
    }

    [Test]
    public async Task DetachCommand_FailsWithHandles()
    {
        Tool _0 = new Tool();
        var cmd = new CommandTestE();

        Check.That(await _0.RegisterCommand("DetachFail",cmd,null,true)).IsTrue();
        Check.That(await _0.DetachCommand(cmd)).IsFalse();

        Check.That(await _0.UnRegisterCommand("DetachFail",detach:true)).IsTrue();
    }

    [Test]
    public async Task UnRegisterCommand_DetachLastHandle()
    {
        Tool _0 = new Tool();
        var cmd = new CommandTestE();

        Check.That(await _0.RegisterCommand("Detach1",cmd,null,true)).IsTrue();
        Check.That(await _0.RegisterCommand("Detach2",cmd)).IsTrue();
        Check.That(cmd.GetLocked()).IsTrue();

        Check.That(await _0.UnRegisterCommand("Detach1",detach:true)).IsTrue();
        Check.That(cmd.GetLocked()).IsTrue();

        Check.That(await _0.UnRegisterCommand("Detach2",detach:true)).IsTrue();
        Check.That(cmd.GetLocked()).IsFalse();
    }

    [Test]
    public async Task RegisterCommand_AttachThrows()
    {
        Tool _0 = new Tool();
        _0.EnableMyExceptions();
        var cmd = new CommandAttachFault();

        Check.ThatCode(async () => await _0.RegisterCommand("AttachThrow",cmd)).Throws<InvalidOperationException>();
        Check.That(_0.GetCommands()?.ContainsKey("AttachThrow") ?? false).IsFalse();
    }

    [Test]
    public async Task RegisterCommand_EnableThrows()
    {
        Tool _0 = new Tool();
        _0.EnableMyExceptions();
        var cmd = new CommandEnableFault();

        Check.ThatCode(async () => await _0.RegisterCommand("EnableThrow",cmd,enable:true)).Throws<InvalidOperationException>();
        Check.That(_0.GetCommands()?.ContainsKey("EnableThrow") ?? false).IsFalse();
    }

    [Test]
    public async Task RegisterCommand_LockThrows()
    {
        Tool _0 = new Tool();
        _0.EnableMyExceptions();
        var cmd = new CommandLockFault();

        Check.ThatCode(async () => await _0.RegisterCommand("LockThrow",cmd)).Throws<InvalidOperationException>();
        Check.That(_0.GetCommands()?.ContainsKey("LockThrow") ?? false).IsFalse();
    }

    [Test]
    public async Task UnRegisterCommand_DetachThrows()
    {
        Tool _0 = new Tool();
        _0.EnableMyExceptions();
        var cmd = new CommandDetachFault();

        Check.That(await _0.RegisterCommand("DetachThrow",cmd)).IsTrue();

        Check.ThatCode(async () => await _0.UnRegisterCommand("DetachThrow",detach:true)).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AttachCommand_Unlocked_EnableFalse()
    {
        Tool _0 = new Tool();
        var cmd = new CommandTestE();

        Check.That(await _0.AttachCommand(cmd,enable:false)).IsTrue();
        Check.That(cmd.Enabled).IsFalse();
        Check.That(cmd.GetLocked()).IsTrue();

        Check.That(await _0.DetachCommand(cmd)).IsTrue();
        Check.That(cmd.GetLocked()).IsFalse();
    }

    [Test]
    public async Task AttachCommand_Unlocked_EnableTrue()
    {
        Tool _0 = new Tool();
        var cmd = new CommandTestE();

        Check.That(await _0.AttachCommand(cmd,enable:true)).IsTrue();
        Check.That(cmd.Enabled).IsTrue();
        Check.That(cmd.GetLocked()).IsTrue();

        Check.That(await _0.DetachCommand(cmd)).IsTrue();
        Check.That(cmd.GetLocked()).IsFalse();
    }

    [Test]
    public async Task AttachCommand_Locked_EnableFalse_Fails()
    {
        Tool _0 = new Tool();
        var cmd = new CommandTestE();

        var mk = cmd.CreateManagementKey("cmd");
        Check.That(cmd.RegisterManager(mk)).IsTrue();
        Check.That(cmd.Lock(mk)).IsTrue();

        Check.That(await _0.AttachCommand(cmd,enable:false)).IsFalse();
        Check.That(cmd.GetLocked()).IsTrue();

        Check.That(cmd.UnLock(mk)).IsTrue();
        Check.That(cmd.UnRegisterManager(mk)).IsTrue();
    }

    [Test]
    public async Task AttachCommand_Locked_EnableTrue_Fails()
    {
        Tool _0 = new Tool();
        var cmd = new CommandTestE();

        var mk = cmd.CreateManagementKey("cmd");
        Check.That(cmd.RegisterManager(mk)).IsTrue();
        Check.That(cmd.Lock(mk)).IsTrue();

        Check.That(await _0.AttachCommand(cmd,enable:true)).IsFalse();
        Check.That(cmd.GetLocked()).IsTrue();

        Check.That(cmd.UnLock(mk)).IsTrue();
        Check.That(cmd.UnRegisterManager(mk)).IsTrue();
    }

    [Test]
    public async Task DetachCommand_Attached_EnableTrue()
    {
        Tool _0 = new Tool();
        var cmd = new CommandTestE();

        Check.That(await _0.AttachCommand(cmd,enable:true)).IsTrue();
        Check.That(cmd.Enabled).IsTrue();

        Check.That(await _0.DetachCommand(cmd)).IsTrue();
        Check.That(cmd.Enabled).IsFalse();
        Check.That(cmd.GetLocked()).IsFalse();
    }

    [Test]
    public async Task AttachCommand_AttachThrows()
    {
        Tool _0 = new Tool();
        _0.EnableMyExceptions();
        var cmd = new CommandAttachFault();

        Check.ThatCode(async () => await _0.AttachCommand(cmd)).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task DetachCommand_DetachThrows()
    {
        Tool _0 = new Tool();
        _0.EnableMyExceptions();
        var cmd = new CommandDetachFault();

        Check.That(await _0.AttachCommand(cmd)).IsTrue();

        Check.ThatCode(async () => await _0.DetachCommand(cmd)).Throws<InvalidOperationException>();
    }

    [Test]
    public async Task AttachCommand_Type_WithArguments()
    {
        Tool _0 = new Tool();

        Check.That(await _0.AttachCommand(typeof(CommandArgsProbe),new Object?[] { 7 , "ok" , true },enable:false)).IsTrue();

        var attached = _0.GetAttachedCommands();
        Check.That(attached).IsNotNull();
        Check.That(attached!.Any(_ => _ is CommandArgsProbe)).IsTrue();

        var cmd = attached!.OfType<CommandArgsProbe>().First();
        Check.That(await _0.DetachCommand(cmd)).IsTrue();
    }

    [Test]
    public async Task RegisterCommand_Type_WithArguments()
    {
        Tool _0 = new Tool();

        Check.That(await _0.RegisterCommand("ArgsType",typeof(CommandArgsProbe),new Object?[] { 7 , "ok" , true })).IsTrue();

        Check.That(await _0.Activate()).IsTrue();
        Check.That(_0.ExecuteCommand(new(){ Handle = "ArgsType" })).HasAValue();
    }

    [Test]
    public async Task AttachAndRegisterCommand_Type_InvalidType_ReturnsFalse()
    {
        Tool _0 = new Tool();

        Check.That(await _0.AttachCommand(typeof(String),new Object?[] { "x" })).IsFalse();
        Check.That(await _0.RegisterCommand("BadType",typeof(String),new Object?[] { "x" })).IsFalse();
        Check.That(_0.GetCommands()?.ContainsKey("BadType") ?? false).IsFalse();
    }

    [Test]
    public async Task RegisterCommand_PermissionsEmpty_CommandKey_CannotExecute()
    {
        Tool _0 = new Tool();
        var cmd = new CommandTestE();
        var k = _0.CreateManagementKey("KM");

        Check.That(await _0.RegisterCommand("P0",cmd,ImmutableArray<Int32>.Empty,enable:true)).IsTrue();
        Check.That(await _0.Activate()).IsTrue();
        Check.That(_0.Lock(k)).IsTrue();

        var ck = GetCommandKey(cmd);
        Check.That(ck).IsNotNull();

        Check.That(_0.ExecuteCommand(new(){ Handle = "P0" },ck)).IsNull();
    }

    [Test]
    public async Task RegisterCommand_DefaultPermissions_CommandKey_NoExecute()
    {
        Tool _0 = new Tool();
        var cmd = new CommandTestE();
        var k = _0.CreateManagementKey("KM");

        Check.That(await _0.RegisterCommand("P1",cmd,null,enable:true)).IsTrue();
        Check.That(await _0.Activate()).IsTrue();
        Check.That(_0.Lock(k)).IsTrue();

        var ck = GetCommandKey(cmd);
        Check.That(ck).IsNotNull();

        Check.That(_0.ExecuteCommand(new(){ Handle = "P1" },ck)).IsNull();
    }

    [Test]
    public async Task AttachCommand_Factory()
    {
        Tool _0 = new Tool();

        Check.That(await _0.AttachCommand(() => new CommandTestE(),enable:true)).IsTrue();

        var attached = _0.GetAttachedCommands();
        Check.That(attached).IsNotNull();
        Check.That(attached!.Any(_ => _ is CommandTestE)).IsTrue();

        var cmd = attached!.OfType<CommandTestE>().First();
        Check.That(await _0.DetachCommand(cmd)).IsTrue();
    }

    [Test]
    public async Task RegisterCommand_Factory()
    {
        Tool _0 = new Tool();

        Check.That(await _0.RegisterCommand("FactoryCmd",() => new CommandTestE(),enable:true)).IsTrue();

        Check.That(await _0.Activate()).IsTrue();
        Check.That(_0.ExecuteCommand(new(){ Handle = "FactoryCmd" })).HasAValue();
    }

    private static CommandKey? GetCommandKey(Command command)
        => typeof(Command).GetField("Key", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(command) as CommandKey;
}
