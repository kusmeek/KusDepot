namespace KusDepot.Exams.Tools;

public partial class ToolExam
{
    private static readonly TimeSpan ConcurrencyTestTimeout = TimeSpan.FromSeconds(5);

    [Test]
    public async Task LifeCycleStateMachine_ConcurrentStartAdmission_TransitionsOnce()
    {
        LifeCycleStateMachine state = new(); Check.That(state.TryInitialize(out _)).IsTrue();
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<(LifeCycleOperationResult Result,LifeCycleState Previous)>[] operations = Enumerable.Range(0,16).Select(_ => Task.Run(async () =>
        {
            await release.Task.ConfigureAwait(false);
            LifeCycleOperationResult result = state.TryAdmitOperation(LifeCycleOperation.Starting,out LifeCycleState previous);
            return (result,previous);
        })).ToArray();

        release.TrySetResult(true); var results = await Task.WhenAll(operations).WaitAsync(ConcurrencyTestTimeout);

        Check.That(results.Count(_ => _.Result is LifeCycleOperationResult.Entered)).IsEqualTo(1);
        Check.That(results.Count(_ => _.Result is LifeCycleOperationResult.AlreadySatisfied)).IsEqualTo(15);
        Check.That(results.Single(_ => _.Result is LifeCycleOperationResult.Entered).Previous).IsEqualTo(LifeCycleState.Initialized);
        Check.That(results.Where(_ => _.Result is LifeCycleOperationResult.AlreadySatisfied).All(_ => _.Previous is LifeCycleState.Starting)).IsTrue();
        Check.That(state.State).IsEqualTo(LifeCycleState.Starting);
    }

    [Test]
    public async Task LifeCycleStateMachine_ConcurrentStopAdmission_TransitionsOnce()
    {
        LifeCycleStateMachine state = new();
        Check.That(state.TryInitialize(out _)).IsTrue(); Check.That(state.TryActivate(out _)).IsTrue();
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<(LifeCycleOperationResult Result,LifeCycleState Previous)>[] operations = Enumerable.Range(0,16).Select(_ => Task.Run(async () =>
        {
            await release.Task.ConfigureAwait(false);
            LifeCycleOperationResult result = state.TryAdmitOperation(LifeCycleOperation.Stopping,out LifeCycleState previous);
            return (result,previous);
        })).ToArray();

        release.TrySetResult(true); var results = await Task.WhenAll(operations).WaitAsync(ConcurrencyTestTimeout);

        Check.That(results.Count(_ => _.Result is LifeCycleOperationResult.Entered)).IsEqualTo(1);
        Check.That(results.Count(_ => _.Result is LifeCycleOperationResult.AlreadySatisfied)).IsEqualTo(15);
        Check.That(results.Single(_ => _.Result is LifeCycleOperationResult.Entered).Previous).IsEqualTo(LifeCycleState.Active);
        Check.That(results.Where(_ => _.Result is LifeCycleOperationResult.AlreadySatisfied).All(_ => _.Previous is LifeCycleState.Stopping)).IsTrue();
        Check.That(state.State).IsEqualTo(LifeCycleState.Stopping);
    }

    [Test]
    public async Task LifeCycleStateMachine_ToErrorRacingTransitions_IsTerminal()
    {
        LifeCycleStateMachine state = new(); Check.That(state.TryInitialize(out _)).IsTrue();
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Task error = Task.Run(async () => { await release.Task.ConfigureAwait(false); Check.That(state.ToError()).IsTrue(); });
        Task[] transitions = Enumerable.Range(0,16).Select(index => Task.Run(async () =>
        {
            await release.Task.ConfigureAwait(false); _ = state.TryAdmitOperation(LifeCycleOperation.Starting,out _); _ = state.TryActivate(out _);
        })).ToArray();

        release.TrySetResult(true); await Task.WhenAll(transitions.Append(error)).WaitAsync(ConcurrencyTestTimeout);

        Check.That(state.State).IsEqualTo(LifeCycleState.Error);
        Check.That(state.TryAdmitOperation(LifeCycleOperation.Starting,out LifeCycleState previous)).IsEqualTo(LifeCycleOperationResult.Rejected);
        Check.That(previous).IsEqualTo(LifeCycleState.Error);
        Check.That(state.TryActivate(out previous)).IsFalse(); Check.That(previous).IsEqualTo(LifeCycleState.Error);
        Check.That(state.TryDeactivate(out previous)).IsFalse(); Check.That(previous).IsEqualTo(LifeCycleState.Error);
        Check.That(state.TryInitialize(out previous)).IsFalse(); Check.That(previous).IsEqualTo(LifeCycleState.Error);
    }
}
