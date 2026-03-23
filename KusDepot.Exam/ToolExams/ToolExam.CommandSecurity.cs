namespace KusDepot.Exams.Tools;

[TestFixture]
[Parallelizable(ParallelScope.None)]
public class ToolCommandSecurityExam
{
    private static T? GetProtectedField<T>(Object obj , String fieldName) where T : class
    {
        return obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(obj) as T;
    }

    [Test]
    public async Task CommandKeyClone()
    {
        Tool tool = new();
        var cmd = new CommandKeyCapture();

        Check.That(await tool.AttachCommand(cmd,enable:true)).IsTrue();

        var commandKeys = GetProtectedField<Dictionary<ICommand, CommandKey>>(tool, "CommandKeys");
        var commandManagerKeys = GetProtectedField<Dictionary<ICommand, ManagerKey>>(tool, "CommandManagerKeys");

        Check.That(commandKeys).IsNotNull();
        Check.That(commandManagerKeys).IsNotNull();

        var storedCommandKey = commandKeys![cmd];
        var storedManagerKey = commandManagerKeys![cmd];

        Check.That(cmd.LastAttachKey).IsNotNull();
        Check.That(cmd.LastEnableKey).IsNotNull();
        Check.That(cmd.LastRegisterManagerKey).IsNotNull();

        Check.That(ReferenceEquals(storedCommandKey, cmd.LastAttachKey)).IsFalse();
        Check.That(ReferenceEquals(storedCommandKey, cmd.LastEnableKey)).IsFalse();
        Check.That(storedCommandKey.Key).IsEqualTo(cmd.LastAttachKey!.Key);

        Check.That(ReferenceEquals(storedManagerKey, cmd.LastRegisterManagerKey)).IsFalse();
        Check.That(storedManagerKey.Key).IsEqualTo(cmd.LastRegisterManagerKey!.Key);

        Check.That(await tool.DetachCommand(cmd)).IsTrue();

        Check.That(cmd.LastDisableKey).IsNotNull();
        Check.That(cmd.LastDetachKey).IsNotNull();

        Check.That(ReferenceEquals(storedCommandKey, cmd.LastDisableKey)).IsFalse();
        Check.That(ReferenceEquals(storedCommandKey, cmd.LastDetachKey)).IsFalse();
        Check.That(storedCommandKey.Key).IsEqualTo(cmd.LastDetachKey!.Key);
    }
}

internal sealed class CommandKeyCapture : Command
{
    public CommandKey? LastAttachKey { get; private set; }
    public CommandKey? LastEnableKey { get; private set; }
    public CommandKey? LastDisableKey { get; private set; }
    public CommandKey? LastDetachKey { get; private set; }
    public ManagementKey? LastRegisterManagerKey { get; private set; }

    public override Boolean Attach(ITool tool , CommandKey? key = null)
    {
        LastAttachKey = key; return base.Attach(tool, key!);
    }

    public override Task<Boolean> Enable(CancellationToken cancel = default , CommandKey? key = null)
    {
        LastEnableKey = key; return base.Enable(cancel, key);
    }

    public override Task<Boolean> Disable(CancellationToken cancel = default , CommandKey? key = null)
    {
        LastDisableKey = key; return base.Disable(cancel, key);
    }

    public override Boolean Detach(CommandKey? key = null)
    {
        LastDetachKey = key; return base.Detach(key);
    }

    public override Boolean RegisterManager(ManagementKey? key = null)
    {
        LastRegisterManagerKey = key; return base.RegisterManager(key);
    }

    public override Boolean Lock(ManagementKey? key)
    {
        return base.Lock(key);
    }
}
