namespace KusDepot.Exams.Security;

public partial class AccessManagerAsyncExam
{
    private sealed class CancelAwarePolicyEngine : IAsyncAccessPolicyEngine
    {
        public CancellationToken LastToken { get; private set; }
        public Int32 AsyncCalls { get; private set; }

        public async ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
        {
            LastToken = cancel;
            AsyncCalls++;
            await Task.Delay(TimeSpan.FromSeconds(5),cancel);
            return AccessPolicyDecision.Allow();
        }
    }

    private sealed class CancelAwareAssertionGenerator : IAsyncAccessKeyAssertionGenerator
    {
        public CancellationToken LastToken { get; private set; }
        public Int32 AsyncCalls { get; private set; }

        public async ValueTask<ImmutableArray<AccessKeyAssertion>> GenerateAsync(AccessKeyAssertionGeneratorContext context , CancellationToken cancel = default)
        {
            LastToken = cancel;
            AsyncCalls++;
            await Task.Delay(TimeSpan.FromSeconds(5),cancel);
            return [];
        }
    }

    [Test]
    public void RequestAccessAsync_Propagates_Cancellation_To_AsyncPolicyEngine()
    {
        CancelAwarePolicyEngine engine = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: engine));
        Tool tool = new(accessmanager: manager);
        Check.That(tool.EnableMyExceptions()).IsTrue();
        using CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromSeconds(2));

        Assert.That(async () => await manager.RequestAccessAsync(new StandardRequest("subject-cancel"),cts.Token),Throws.InstanceOf<OperationCanceledException>());
        Check.That(tool).IsNotNull();
        Check.That(engine.AsyncCalls).IsEqualTo(1);
        Check.That(engine.LastToken.CanBeCanceled).IsTrue();
    }

    [Test]
    public void GenerateAccessKeyAsync_Propagates_Cancellation_To_AsyncAssertionGenerator()
    {
        CancelAwareAssertionGenerator generator = new();
        Tool tool = new();
        AccessManager manager = new(new AccessManagerOptions(keyassertiongenerator: generator),tool);
        Check.That(tool.EnableMyExceptions()).IsTrue();
        using CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromSeconds(2));

        Assert.That(async () => await manager.GenerateAccessKeyAsync<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-cancel-generator"),cancel: cts.Token),Throws.InstanceOf<OperationCanceledException>());
        Check.That(generator.AsyncCalls).IsEqualTo(1);
        Check.That(generator.LastToken.CanBeCanceled).IsTrue();
    }

    [Test]
    public void AccessCheckAsync_Returns_Canceled_Task_When_Token_Is_PreCanceled()
    {
        AccessManager manager = new();
        using CancellationTokenSource cts = new();
        cts.Cancel();

        Assert.That(async () => await manager.AccessCheckAsync(null,ProtectedOperation.GetOutputIDs,cts.Token),Throws.InstanceOf<OperationCanceledException>());
    }
}
