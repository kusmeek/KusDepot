namespace KusDepot.Exams.Security;

public partial class AccessManagerAsyncExam
{
    private sealed class AsyncOnlyDenyRequestPolicyEngine : IAsyncAccessPolicyEngine
    {
        public Int32 AsyncCalls { get; private set; }

        public async ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
        {
            await Task.Yield();
            cancel.ThrowIfCancellationRequested();
            AsyncCalls++;
            return context.Mode == AccessPolicyMode.Request
                ? AccessPolicyDecision.Deny("request denied by async engine")
                : AccessPolicyDecision.Allow();
        }
    }

    private sealed class PreferAsyncDenyModePolicyEngine(AccessPolicyMode deniedmode) : IAccessPolicyEngine , IAsyncAccessPolicyEngine
    {
        public Int32 AsyncCalls { get; private set; }
        public Int32 SyncCalls { get; private set; }

        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            SyncCalls++;
            return AccessPolicyDecision.Allow();
        }

        public async ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
        {
            await Task.Yield();
            cancel.ThrowIfCancellationRequested();
            AsyncCalls++;
            return context.Mode == deniedmode
                ? AccessPolicyDecision.Deny($"{deniedmode} denied by async engine")
                : AccessPolicyDecision.Allow();
        }
    }

    [Test]
    public async Task RequestAccessAsync_Denies_StandardRequest_When_AsyncOnlyPolicyEngine_Denies_Request()
    {
        AsyncOnlyDenyRequestPolicyEngine engine = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: engine));
        Tool tool = new(accessmanager: manager);

        AccessKey? key = await manager.RequestAccessAsync(new StandardRequest("subject-async"));

        Check.That(tool).IsNotNull();
        Check.That(key).IsNull();
        Check.That(engine.AsyncCalls).IsEqualTo(1);
    }

    [Test]
    public async Task AccessCheckAsync_Uses_AsyncPolicyEngine_When_DualModeEngine_Is_Configured()
    {
        PreferAsyncDenyModePolicyEngine engine = new(AccessPolicyMode.Access);
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: engine));
        Tool tool = new(accessmanager: manager);
        OwnerKey? ownerkey = tool.CreateOwnerKey("Owner");

        Check.That(tool.TakeOwnership(ownerkey)).IsTrue();

        ClientKey? clientkey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        Boolean allowed = await manager.AccessCheckAsync(clientkey,ProtectedOperation.GetOutputIDs);

        Check.That(clientkey).IsNotNull();
        Check.That(allowed).IsFalse();
        Check.That(engine.AsyncCalls).IsEqualTo(1);
        Assert.That(engine.SyncCalls,Is.GreaterThan(0));
    }

    [Test]
    public async Task AuthorizeAsync_Uses_AsyncPolicyEngine_When_DualModeEngine_Is_Configured()
    {
        PreferAsyncDenyModePolicyEngine engine = new(AccessPolicyMode.Authorize);
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: engine));
        Tool tool = new(accessmanager: manager);
        OwnerKey? ownerkey = tool.CreateOwnerKey("Owner");

        Check.That(tool.TakeOwnership(ownerkey)).IsTrue();

        ClientKey? clientkey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        Boolean authorized = await manager.AuthorizeAsync(clientkey);

        Check.That(clientkey).IsNotNull();
        Check.That(authorized).IsFalse();
        Check.That(engine.AsyncCalls).IsEqualTo(1);
        Assert.That(engine.SyncCalls,Is.GreaterThan(0));
    }

    [Test]
    public async Task RevokeAccessAsync_Uses_AsyncPolicyEngine_When_DualModeEngine_Is_Configured()
    {
        PreferAsyncDenyModePolicyEngine engine = new(AccessPolicyMode.Revoke);
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: engine));
        Tool tool = new(accessmanager: manager);
        OwnerKey? ownerkey = tool.CreateOwnerKey("Owner");

        Check.That(tool.TakeOwnership(ownerkey)).IsTrue();

        ClientKey? clientkey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        Boolean revoked = await manager.RevokeAccessAsync(clientkey);

        Check.That(clientkey).IsNotNull();
        Check.That(revoked).IsFalse();
        Check.That(engine.AsyncCalls).IsEqualTo(1);
        Assert.That(engine.SyncCalls,Is.GreaterThan(0));
    }
}
