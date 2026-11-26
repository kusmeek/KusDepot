namespace KusDepot.Exams.Tools;

public partial class ToolExam
{
    [Test]
    public async Task ToolGenericHost_NestedToolHost_CallbacksFollowLayeredOrder()
    {
        ConcurrentQueue<String> events = new(); RecordingToolHost inner = new(events,"inner");
        await using RecordingToolGenericHost outer = new(events,"outer",inner);

        Check.That(await outer.StartHostAsync().WaitAsync(ConcurrencyTestTimeout)).IsTrue();
        Check.That(await outer.StopHostAsync().WaitAsync(ConcurrencyTestTimeout)).IsTrue();

        Check.That(events).ContainsExactly(
            "outer.Starting","inner.Starting",
            "outer.Start","inner.Start",
            "outer.Started","inner.Started",
            "outer.Stopping","inner.Stopping",
            "inner.Stop","outer.Stop",
            "outer.Stopped","inner.Stopped");
        Check.That(outer.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
        Check.That(inner.GetLifeCycleState()).IsEqualTo(LifeCycleState.InActive);
    }

    [Test]
    public async Task ToolGenericHost_InnerStartFailure_SkipsLaterCallbacks()
    {
        ConcurrentQueue<String> events = new(); RecordingToolHost inner = new(events,"inner",failingPhase:"Start");
        await using RecordingToolGenericHost outer = new(events,"outer",inner);

        Check.That(await outer.StartHostAsync().WaitAsync(ConcurrencyTestTimeout)).IsFalse();

        Check.That(events).ContainsExactly("outer.Starting","inner.Starting","outer.Start","inner.Start");
        Check.That(events).Not.Contains("outer.Started","inner.Started");
        Check.That(outer.GetLifeCycleState()).IsEqualTo(LifeCycleState.Active);
        Check.That(inner.GetLifeCycleState()).IsEqualTo(LifeCycleState.Starting);
    }

    [Test]
    public async Task ToolGenericHost_InnerStopFailure_SkipsStoppedCallbacks()
    {
        ConcurrentQueue<String> events = new(); RecordingToolHost inner = new(events,"inner",failingPhase:"Stop");
        await using RecordingToolGenericHost outer = new(events,"outer",inner);

        Check.That(await outer.StartHostAsync().WaitAsync(ConcurrencyTestTimeout)).IsTrue();
        Check.That(await outer.StopHostAsync().WaitAsync(ConcurrencyTestTimeout)).IsFalse();

        Check.That(events).ContainsExactly(
            "outer.Starting","inner.Starting",
            "outer.Start","inner.Start",
            "outer.Started","inner.Started",
            "outer.Stopping","inner.Stopping",
            "inner.Stop","outer.Stop");
        Check.That(outer.GetLifeCycleState()).IsEqualTo(LifeCycleState.Stopping);
        Check.That(inner.GetLifeCycleState()).IsEqualTo(LifeCycleState.Stopping);
    }
}
