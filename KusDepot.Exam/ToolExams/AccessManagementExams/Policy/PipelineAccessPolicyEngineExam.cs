namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class PipelineAccessPolicyEngineExam
{
    private sealed class AllowStage : IAccessPolicyStage
    {
        public Int32 Count { get; private set; }

        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            Count++;
            return AccessPolicyDecision.Allow();
        }
    }

    private sealed class AsyncAllowStage : IAsyncAccessPolicyStage
    {
        public Int32 Count { get; private set; }

        public async ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
        {
            await Task.Yield();
            cancel.ThrowIfCancellationRequested();
            Count++;
            return AccessPolicyDecision.Allow();
        }
    }

    private sealed class AsyncDenyStage : IAsyncAccessPolicyStage
    {
        public Int32 Count { get; private set; }

        public async ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
        {
            await Task.Yield();
            cancel.ThrowIfCancellationRequested();
            Count++;
            return AccessPolicyDecision.Deny("denied by async stage");
        }
    }

    private sealed class DenyStage : IAccessPolicyStage
    {
        public Int32 Count { get; private set; }

        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            Count++;
            return AccessPolicyDecision.Deny("denied by stage");
        }
    }

    [Test]
    public void Evaluate_Allows_When_All_Stages_Allow()
    {
        AllowStage first = new();
        AllowStage second = new();
        PipelineAccessPolicyEngine engine = new([first,second]);
        AccessPolicyContext context = new() { Mode = AccessPolicyMode.Authorize, Subject = "subject-a" };

        AccessPolicyDecision decision = engine.Evaluate(in context);

        Check.That(decision.Allowed).IsTrue();
        Check.That(first.Count).IsEqualTo(1);
        Check.That(second.Count).IsEqualTo(1);
    }

    [Test]
    public void Evaluate_Stops_On_First_Deny()
    {
        AllowStage first = new();
        DenyStage second = new();
        AllowStage third = new();
        PipelineAccessPolicyEngine engine = new([first,second,third]);
        AccessPolicyContext context = new() { Mode = AccessPolicyMode.Access, Subject = "subject-a" };

        AccessPolicyDecision decision = engine.Evaluate(in context);

        Check.That(decision.Allowed).IsFalse();
        Check.That(decision.Reason).IsEqualTo("denied by stage");
        Check.That(first.Count).IsEqualTo(1);
        Check.That(second.Count).IsEqualTo(1);
        Check.That(third.Count).IsEqualTo(0);
    }

    [Test]
    public async Task EvaluateAsync_Prefers_Async_Stages_And_Stops_On_First_Deny()
    {
        AsyncAllowStage first = new();
        AsyncDenyStage second = new();
        AllowStage third = new();
        PipelineAccessPolicyEngine engine = new([first,second,third]);
        AccessPolicyContext context = new() { Mode = AccessPolicyMode.Access, Subject = "subject-a" };

        AccessPolicyDecision decision = await engine.EvaluateAsync(context);

        Check.That(decision.Allowed).IsFalse();
        Check.That(decision.Reason).IsEqualTo("denied by async stage");
        Check.That(first.Count).IsEqualTo(1);
        Check.That(second.Count).IsEqualTo(1);
        Check.That(third.Count).IsEqualTo(0);
    }
}
