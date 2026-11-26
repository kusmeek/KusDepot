namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class LifeCycleStateMachineExam
{
    [Test]
    public void UnInitialized()
    {
        var _ = new LifeCycleStateMachine();

        Check.That(_.State).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(_.HostedServicesChangeOK()).IsFalse();
        Check.That(_.CommandChangeOK()).IsFalse();
        Check.That(_.InitializeOK()).IsTrue();
    }

    [Test]
    public void Initialized()
    {
        var _ = new LifeCycleStateMachine();

        Check.That(_.TryInitialize(out LifeCycleState previous)).IsTrue();
        Check.That(previous).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(_.State).IsEqualTo(LifeCycleState.Initialized);

        Check.That(_.TryInitialize(out previous)).IsTrue();
        Check.That(previous).IsEqualTo(LifeCycleState.Initialized);
        Check.That(_.HostedServicesChangeOK()).IsTrue();
        Check.That(_.CommandChangeOK()).IsTrue();
        Check.That(_.InitializeOK()).IsTrue();
    }

    [Test]
    public void Starting()
    {
        var _ = new LifeCycleStateMachine();
        LifeCycleState previous;

        Check.That(_.TryInitialize(out previous)).IsTrue();
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Starting,out previous)).IsEqualTo(LifeCycleOperationResult.Entered);

        Check.That(_.State).IsEqualTo(LifeCycleState.Starting);

        Check.That(_.HostedServicesChangeOK()).IsFalse();
        Check.That(_.CommandChangeOK()).IsFalse();
        Check.That(_.InitializeOK()).IsFalse();
    }

    [Test]
    public void Stopping()
    {
        var _ = new LifeCycleStateMachine();
        LifeCycleState previous;

        Check.That(_.TryInitialize(out previous)).IsTrue();
        Check.That(_.TryActivate(out previous)).IsTrue();
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Stopping,out previous)).IsEqualTo(LifeCycleOperationResult.Entered);

        Check.That(_.State).IsEqualTo(LifeCycleState.Stopping);

        Check.That(_.HostedServicesChangeOK()).IsFalse();
        Check.That(_.CommandChangeOK()).IsFalse();
        Check.That(_.InitializeOK()).IsFalse();
    }

    [Test]
    public void Active()
    {
        var _ = new LifeCycleStateMachine();
        LifeCycleState previous;

        Check.That(_.TryInitialize(out previous)).IsTrue();
        Check.That(_.TryActivate(out previous)).IsTrue();
        Check.That(previous).IsEqualTo(LifeCycleState.Initialized);

        Check.That(_.State).IsEqualTo(LifeCycleState.Active);

        Check.That(_.HostedServicesChangeOK()).IsTrue();
        Check.That(_.CommandChangeOK()).IsTrue();
        Check.That(_.InitializeOK()).IsFalse();
    }

    [Test]
    public void InActive()
    {
        var _ = new LifeCycleStateMachine();
        LifeCycleState previous;

        Check.That(_.TryInitialize(out previous)).IsTrue();
        Check.That(_.TryActivate(out previous)).IsTrue();
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Stopping,out previous)).IsEqualTo(LifeCycleOperationResult.Entered);
        Check.That(_.TryDeactivate(out previous)).IsTrue();

        Check.That(_.State).IsEqualTo(LifeCycleState.InActive);

        Check.That(_.HostedServicesChangeOK()).IsTrue();
        Check.That(_.CommandChangeOK()).IsTrue();
        Check.That(_.InitializeOK()).IsTrue();
    }

    [Test]
    public void Error()
    {
        var _ = new LifeCycleStateMachine();

        Check.That(_.ToError()).IsTrue();

        Check.That(_.State).IsEqualTo(LifeCycleState.Error);

        Check.That(_.HostedServicesChangeOK()).IsFalse();
        Check.That(_.CommandChangeOK()).IsFalse();
        Check.That(_.InitializeOK()).IsFalse();
    }

    [Test]
    public void AtomicPhaseEntry()
    {
        var _ = new LifeCycleStateMachine();
        LifeCycleState previous;

        Check.That(_.TryInitialize(out previous)).IsTrue();

        Check.That(_.TryAdmitOperation(LifeCycleOperation.Start,out previous)).IsEqualTo(LifeCycleOperationResult.Entered);
        Check.That(previous).IsEqualTo(LifeCycleState.Initialized);
        Check.That(_.State).IsEqualTo(LifeCycleState.Starting);

        Check.That(_.TryAdmitOperation(LifeCycleOperation.Start,out previous)).IsEqualTo(LifeCycleOperationResult.Entered);
        Check.That(previous).IsEqualTo(LifeCycleState.Starting);

        Check.That(_.TryAdmitOperation(LifeCycleOperation.Starting,out previous)).IsEqualTo(LifeCycleOperationResult.AlreadySatisfied);
        Check.That(previous).IsEqualTo(LifeCycleState.Starting);

        Check.That(_.TryActivate(out previous)).IsTrue();
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Stop,out previous)).IsEqualTo(LifeCycleOperationResult.Entered);
        Check.That(previous).IsEqualTo(LifeCycleState.Active);
        Check.That(_.State).IsEqualTo(LifeCycleState.Stopping);

        Check.That(_.TryAdmitOperation(LifeCycleOperation.Stop,out previous)).IsEqualTo(LifeCycleOperationResult.Entered);
        Check.That(previous).IsEqualTo(LifeCycleState.Stopping);

        Check.That(_.TryAdmitOperation(LifeCycleOperation.Stopping,out previous)).IsEqualTo(LifeCycleOperationResult.AlreadySatisfied);
        Check.That(previous).IsEqualTo(LifeCycleState.Stopping);
    }

    [Test]
    public void OperationAdmission()
    {
        var _ = new LifeCycleStateMachine(); LifeCycleState previous;

        Check.That(_.TryAdmitOperation(LifeCycleOperation.Starting,out previous)).IsEqualTo(LifeCycleOperationResult.Rejected);
        Check.That(previous).IsEqualTo(LifeCycleState.UnInitialized);

        Check.That(_.TryInitialize(out previous)).IsTrue();
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Starting,out previous)).IsEqualTo(LifeCycleOperationResult.Entered);
        Check.That(previous).IsEqualTo(LifeCycleState.Initialized);
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Starting,out previous)).IsEqualTo(LifeCycleOperationResult.AlreadySatisfied);
        Check.That(previous).IsEqualTo(LifeCycleState.Starting);

        Check.That(_.TryAdmitOperation(LifeCycleOperation.Start,out previous)).IsEqualTo(LifeCycleOperationResult.Entered);
        Check.That(_.State).IsEqualTo(LifeCycleState.Starting);
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Started,out previous)).IsEqualTo(LifeCycleOperationResult.Rejected);

        Check.That(_.TryActivate(out previous)).IsTrue();
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Started,out previous)).IsEqualTo(LifeCycleOperationResult.Entered);
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Started,out previous)).IsEqualTo(LifeCycleOperationResult.AlreadySatisfied);
    }

    [Test]
    public void OperationAdmission_StopIsIdempotentForInitializedAndInactive()
    {
        var _ = new LifeCycleStateMachine(); LifeCycleState previous;

        Check.That(_.TryInitialize(out previous)).IsTrue();
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Stop,out previous)).IsEqualTo(LifeCycleOperationResult.AlreadySatisfied);
        Check.That(_.State).IsEqualTo(LifeCycleState.Initialized);

        Check.That(_.TryActivate(out previous)).IsTrue();
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Stopping,out previous)).IsEqualTo(LifeCycleOperationResult.Entered);
        Check.That(_.TryDeactivate(out previous)).IsTrue();
        Check.That(_.TryAdmitOperation(LifeCycleOperation.Stop,out previous)).IsEqualTo(LifeCycleOperationResult.AlreadySatisfied);
        Check.That(_.State).IsEqualTo(LifeCycleState.InActive);
    }

    private static IEnumerable<TestCaseData> OperationCases()
    {
        LifeCycleState[] states = Enum.GetValues<LifeCycleState>();

        foreach(LifeCycleState state in states)
        {
            yield return new TestCaseData(LifeCycleOperation.Starting,state,state is LifeCycleState.Starting or LifeCycleState.Active ? LifeCycleOperationResult.AlreadySatisfied : state is LifeCycleState.Initialized or LifeCycleState.InActive ? LifeCycleOperationResult.Entered : LifeCycleOperationResult.Rejected,state is LifeCycleState.Initialized or LifeCycleState.InActive ? LifeCycleState.Starting : state);
            yield return new TestCaseData(LifeCycleOperation.Start,state,state is LifeCycleState.Active ? LifeCycleOperationResult.AlreadySatisfied : state is LifeCycleState.Initialized or LifeCycleState.InActive or LifeCycleState.Starting ? LifeCycleOperationResult.Entered : LifeCycleOperationResult.Rejected,state is LifeCycleState.Initialized or LifeCycleState.InActive ? LifeCycleState.Starting : state);
            yield return new TestCaseData(LifeCycleOperation.Started,state,state is LifeCycleState.Active ? LifeCycleOperationResult.Entered : LifeCycleOperationResult.Rejected,state);
            yield return new TestCaseData(LifeCycleOperation.Stopping,state,state is LifeCycleState.Stopping ? LifeCycleOperationResult.AlreadySatisfied : state is LifeCycleState.Active ? LifeCycleOperationResult.Entered : state is LifeCycleState.Initialized or LifeCycleState.InActive ? LifeCycleOperationResult.AlreadySatisfied : LifeCycleOperationResult.Rejected,state is LifeCycleState.Active ? LifeCycleState.Stopping : state);
            yield return new TestCaseData(LifeCycleOperation.Stop,state,state is LifeCycleState.Initialized or LifeCycleState.InActive ? LifeCycleOperationResult.AlreadySatisfied : state is LifeCycleState.Active or LifeCycleState.Stopping ? LifeCycleOperationResult.Entered : LifeCycleOperationResult.Rejected,state is LifeCycleState.Active ? LifeCycleState.Stopping : state);
            yield return new TestCaseData(LifeCycleOperation.Stopped,state,state is LifeCycleState.Initialized ? LifeCycleOperationResult.AlreadySatisfied : state is LifeCycleState.InActive ? LifeCycleOperationResult.Entered : LifeCycleOperationResult.Rejected,state);
        }
    }

    [Test, TestCaseSource(nameof(OperationCases))]
    public void OperationMatrix(LifeCycleOperation operation , LifeCycleState source , LifeCycleOperationResult expected , LifeCycleState target)
    {
        var _ = CreateStateMachine(source); LifeCycleState previous;

        LifeCycleOperationResult result = _.TryAdmitOperation(operation,out previous);

        Check.That(result).IsEqualTo(expected);
        Check.That(previous).IsEqualTo(source);
        Check.That(_.State).IsEqualTo(target);
    }

    private static LifeCycleStateMachine CreateStateMachine(LifeCycleState state)
    {
        var _ = new LifeCycleStateMachine(); LifeCycleState previous;

        switch(state)
        {
            case LifeCycleState.Initialized:
            { _.TryInitialize(out previous); break; }

            case LifeCycleState.Starting:
            { _.TryInitialize(out previous); _.TryAdmitOperation(LifeCycleOperation.Starting,out previous); break; }

            case LifeCycleState.Active:
            { _.TryInitialize(out previous); _.TryActivate(out previous); break; }

            case LifeCycleState.Stopping:
            { _.TryInitialize(out previous); _.TryActivate(out previous); _.TryAdmitOperation(LifeCycleOperation.Stopping,out previous); break; }

            case LifeCycleState.InActive:
            { _.TryInitialize(out previous); _.TryActivate(out previous); _.TryAdmitOperation(LifeCycleOperation.Stopping,out previous); _.TryDeactivate(out previous); break; }

            case LifeCycleState.Error:
            { _.ToError(); break; }
        }

        return _;
    }

    [Test, TestCase(LifeCycleState.UnInitialized,false,false), TestCase(LifeCycleState.Initialized,false,false), TestCase(LifeCycleState.Starting,false,false),
           TestCase(LifeCycleState.Stopping,false,false), TestCase(LifeCycleState.Active,true,false), TestCase(LifeCycleState.InActive,false,true), TestCase(LifeCycleState.Error,false,false)]
    public void CompletionStateMatrix(LifeCycleState state , Boolean started , Boolean stopped)
    {
        var _ = CreateStateMachine(state);

        Check.That(_.State is LifeCycleState.Active).IsEqualTo(started);
        Check.That(_.State is LifeCycleState.InActive).IsEqualTo(stopped);
    }
}