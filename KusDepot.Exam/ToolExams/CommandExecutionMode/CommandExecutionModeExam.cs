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
        Check.That(m.AllowFreeMode).IsFalse();
    }

    [Test]
    public void AllowBoth()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowBoth()).IsTrue();
        Check.That(m.AllowSynchronous).IsTrue();
        Check.That(m.AllowAsynchronous).IsTrue();
        Check.That(m.AllowFreeMode).IsFalse();
    }

    [Test]
    public void AllowBothFreeMode()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowBothFreeMode()).IsTrue();
        Check.That(m.AllowSynchronous).IsTrue();
        Check.That(m.AllowAsynchronous).IsTrue();
        Check.That(m.AllowFreeMode).IsTrue();
    }

    [Test]
    public void AllowSynchronousOnly()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowSynchronousOnly()).IsTrue();
        Check.That(m.AllowSynchronous).IsTrue();
        Check.That(m.AllowAsynchronous).IsFalse();
        Check.That(m.AllowFreeMode).IsFalse();
    }

    [Test]
    public void AllowAsynchronousOnly()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowAsynchronousOnly()).IsTrue();
        Check.That(m.AllowSynchronous).IsFalse();
        Check.That(m.AllowAsynchronous).IsTrue();
        Check.That(m.AllowFreeMode).IsFalse();
    }

    [Test]
    public void AllowAsynchronousFreeMode()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowAsynchronousFreeMode()).IsTrue();
        Check.That(m.AllowSynchronous).IsFalse();
        Check.That(m.AllowAsynchronous).IsTrue();
        Check.That(m.AllowFreeMode).IsTrue();
    }

    [Test]
    public void AllowAll()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowAll()).IsTrue();
        Check.That(m.AllowSynchronous).IsTrue();
        Check.That(m.AllowAsynchronous).IsTrue();
        Check.That(m.AllowFreeMode).IsTrue();
    }

    [Test]
    public void Configure_Invalid_FreeModeWithoutAsync()
    {
        var m = new CommandExecutionMode();

        Check.That(m.Configure(allowsynchronous: true, allowasynchronous: false, allowfreemode: true)).IsFalse();

        Check.That(m.AllowSynchronous).IsTrue();
        Check.That(m.AllowAsynchronous).IsFalse();
        Check.That(m.AllowFreeMode).IsFalse();
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
        Check.That(m.AllowBothFreeMode()).IsFalse();
        Check.That(m.AllowAll()).IsFalse();
        Check.That(m.AllowAsynchronousFreeMode()).IsFalse();
        Check.That(m.Configure(true,true,true)).IsFalse();
    }

    [Test]
    public void MixedSequence_BeforeLock()
    {
        var m = new CommandExecutionMode();

        Check.That(m.AllowAsynchronousOnly()).IsTrue();
        Check.That(m.AllowSynchronous).IsFalse();
        Check.That(m.AllowAsynchronous).IsTrue();
        Check.That(m.AllowFreeMode).IsFalse();

        Check.That(m.AllowBothFreeMode()).IsTrue();
        Check.That(m.AllowSynchronous).IsTrue();
        Check.That(m.AllowAsynchronous).IsTrue();
        Check.That(m.AllowFreeMode).IsTrue();
    }

    [Test]
    public async Task Model_Locked_By_Command_Enable()
    {
        var c = new MinimalCommand();

        Check.That(c.ExecutionMode.AllowAsynchronous).IsFalse();
        Check.That(c.ExecutionMode.AllowSynchronous).IsTrue();
        Check.That(c.ExecutionMode.AllowFreeMode).IsFalse();

        Check.That(await c.Enable()).IsTrue();

        Check.That(c.ExecutionMode.AllowAsynchronousOnly()).IsFalse();
        Check.That(c.ExecutionMode.AllowSynchronousOnly()).IsFalse();
        Check.That(c.ExecutionMode.AllowBoth()).IsFalse();
        Check.That(c.ExecutionMode.AllowBothFreeMode()).IsFalse();
        Check.That(c.ExecutionMode.AllowAll()).IsFalse();
        Check.That(c.ExecutionMode.AllowAsynchronousFreeMode()).IsFalse();
    }

    private sealed class MinimalCommand : Command { }
}