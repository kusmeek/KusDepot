namespace KusDepot.Exams.Tools;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CommandExecutionModeExam
{
    [Test]
    public void Constructor_Defaults()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowSynchronous).IsTrue();
        Check.That(m.AllowAsynchronous).IsFalse();
    }

    [Test]
    public void AllowBoth_Unlocked()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowBoth()).IsTrue();
        Check.That(m.AllowSynchronous).IsTrue();
        Check.That(m.AllowAsynchronous).IsTrue();
    }

    [Test]
    public void AllowSynchronousOnly_Unlocked()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowSynchronousOnly()).IsTrue();
        Check.That(m.AllowSynchronous).IsTrue();
        Check.That(m.AllowAsynchronous).IsFalse();
    }

    [Test]
    public void AllowAsynchronousOnly_Unlocked()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowAsynchronousOnly()).IsTrue();
        Check.That(m.AllowSynchronous).IsFalse();
        Check.That(m.AllowAsynchronous).IsTrue();
    }

    [Test]
    public void Lock_Idempotent()
    {
        var m = new CommandExecutionMode();

        Check.That(m.Lock()).IsTrue();
        Check.That(m.Lock()).IsFalse();
    }

    [Test]
    public void Locked()
    {
        var m = new CommandExecutionMode();

        Check.That(m.Lock()).IsTrue();
        Check.That(m.AllowBoth()).IsFalse();
        Check.That(m.AllowSynchronousOnly()).IsFalse();
        Check.That(m.AllowAsynchronousOnly()).IsFalse();
    }

    [Test]
    public void MixedSequence_BeforeLock()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowAsynchronousOnly()).IsTrue();
        Check.That(m.AllowSynchronous).IsFalse();
        Check.That(m.AllowAsynchronous).IsTrue();

        Check.That(m.AllowBoth()).IsTrue();
        Check.That(m.AllowSynchronous).IsTrue();
        Check.That(m.AllowAsynchronous).IsTrue();
    }

    [Test]
    public async Task Model_Locked_By_Command_Enable()
    {
        var c = new MinimalCommand();

        Check.That(c.ExecutionMode.AllowAsynchronous).IsFalse();
        Check.That(c.ExecutionMode.AllowSynchronous).IsTrue();

        Check.That(await c.Enable()).IsTrue();

        Check.That(c.ExecutionMode.AllowAsynchronousOnly()).IsFalse();
        Check.That(c.ExecutionMode.AllowSynchronousOnly()).IsFalse();
        Check.That(c.ExecutionMode.AllowBoth()).IsFalse();
    }

    private sealed class MinimalCommand : Command { }
}