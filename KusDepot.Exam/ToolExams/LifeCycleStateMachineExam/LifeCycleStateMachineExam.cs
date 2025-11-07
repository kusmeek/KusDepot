namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class LifeCycleStateMachineExam
{
    [Test]
    public void UnInitialized()
    {
        var _ = new LifeCycleStateMachine();

        Check.That(_.State).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(_.ActivateOK()).IsTrue();
        Check.That(_.DeactivateOK()).IsFalse();
        Check.That(_.InitializeOK()).IsTrue();
        Check.That(_.StartingOK()).IsTrue();
        Check.That(_.StartOK()).IsTrue();
        Check.That(_.StartedOK()).IsFalse();
        Check.That(_.StoppingOK()).IsFalse();
        Check.That(_.StopOK()).IsFalse();
        Check.That(_.StoppedOK()).IsFalse();
        Check.That(_.StartHostOK()).IsTrue();
        Check.That(_.StopHostOK()).IsFalse();
    }

    [Test]
    public void Initialized()
    {
        var _ = new LifeCycleStateMachine();

        Check.That(_.ToInitialized()).IsTrue();

        Check.That(_.State).IsEqualTo(LifeCycleState.Initialized);

        Check.That(_.ActivateOK()).IsTrue();
        Check.That(_.DeactivateOK()).IsFalse();
        Check.That(_.InitializeOK()).IsTrue();
        Check.That(_.StartingOK()).IsTrue();
        Check.That(_.StartOK()).IsTrue();
        Check.That(_.StartedOK()).IsFalse();
        Check.That(_.StoppingOK()).IsFalse();
        Check.That(_.StopOK()).IsFalse();
        Check.That(_.StoppedOK()).IsFalse();
        Check.That(_.StartHostOK()).IsTrue();
        Check.That(_.StopHostOK()).IsFalse();
    }

    [Test]
    public void Starting()
    {
        var _ = new LifeCycleStateMachine();

        Check.That(_.ToStarting()).IsTrue();

        Check.That(_.State).IsEqualTo(LifeCycleState.Starting);

        Check.That(_.ActivateOK()).IsTrue();
        Check.That(_.DeactivateOK()).IsFalse();
        Check.That(_.InitializeOK()).IsFalse();
        Check.That(_.StartingOK()).IsFalse();
        Check.That(_.StartOK()).IsTrue();
        Check.That(_.StartedOK()).IsFalse();
        Check.That(_.StoppingOK()).IsFalse();
        Check.That(_.StopOK()).IsFalse();
        Check.That(_.StoppedOK()).IsFalse();
        Check.That(_.StartHostOK()).IsFalse();
        Check.That(_.StopHostOK()).IsFalse();
    }

    [Test]
    public void Stopping()
    {
        var _ = new LifeCycleStateMachine();

        Check.That(_.ToStopping()).IsTrue();

        Check.That(_.State).IsEqualTo(LifeCycleState.Stopping);

        Check.That(_.ActivateOK()).IsFalse();
        Check.That(_.DeactivateOK()).IsTrue();
        Check.That(_.InitializeOK()).IsFalse();
        Check.That(_.StartingOK()).IsFalse();
        Check.That(_.StartOK()).IsFalse();
        Check.That(_.StartedOK()).IsFalse();
        Check.That(_.StoppingOK()).IsFalse();
        Check.That(_.StopOK()).IsTrue();
        Check.That(_.StoppedOK()).IsFalse();
        Check.That(_.StartHostOK()).IsFalse();
        Check.That(_.StopHostOK()).IsFalse();
    }

    [Test]
    public void Active()
    {
        var _ = new LifeCycleStateMachine();

        Check.That(_.ToActive()).IsTrue();

        Check.That(_.State).IsEqualTo(LifeCycleState.Active);

        Check.That(_.ActivateOK()).IsFalse();
        Check.That(_.DeactivateOK()).IsTrue();
        Check.That(_.InitializeOK()).IsFalse();
        Check.That(_.StartingOK()).IsFalse();
        Check.That(_.StartOK()).IsFalse();
        Check.That(_.StartedOK()).IsTrue();
        Check.That(_.StoppingOK()).IsTrue();
        Check.That(_.StopOK()).IsTrue();
        Check.That(_.StoppedOK()).IsFalse();
        Check.That(_.StartHostOK()).IsFalse();
        Check.That(_.StopHostOK()).IsTrue();
    }

    [Test]
    public void InActive()
    {
        var _ = new LifeCycleStateMachine();

        Check.That(_.ToInActive()).IsTrue();

        Check.That(_.State).IsEqualTo(LifeCycleState.InActive);

        Check.That(_.ActivateOK()).IsTrue();
        Check.That(_.DeactivateOK()).IsFalse();
        Check.That(_.InitializeOK()).IsTrue();
        Check.That(_.StartingOK()).IsTrue();
        Check.That(_.StartOK()).IsTrue();
        Check.That(_.StartedOK()).IsFalse();
        Check.That(_.StoppingOK()).IsFalse();
        Check.That(_.StopOK()).IsFalse();
        Check.That(_.StoppedOK()).IsTrue();
        Check.That(_.StartHostOK()).IsTrue();
        Check.That(_.StopHostOK()).IsFalse();
    }

    [Test]
    public void Error()
    {
        var _ = new LifeCycleStateMachine();

        Check.That(_.ToError()).IsTrue();

        Check.That(_.State).IsEqualTo(LifeCycleState.Error);

        Check.That(_.ActivateOK()).IsFalse();
        Check.That(_.DeactivateOK()).IsFalse();
        Check.That(_.InitializeOK()).IsFalse();
        Check.That(_.StartingOK()).IsFalse();
        Check.That(_.StartOK()).IsFalse();
        Check.That(_.StartedOK()).IsFalse();
        Check.That(_.StoppingOK()).IsFalse();
        Check.That(_.StopOK()).IsFalse();
        Check.That(_.StoppedOK()).IsFalse();
        Check.That(_.StartHostOK()).IsFalse();
        Check.That(_.StopHostOK()).IsFalse();
    }
}