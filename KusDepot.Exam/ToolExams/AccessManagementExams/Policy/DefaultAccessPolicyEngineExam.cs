namespace KusDepot.Exams.Security;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class DefaultAccessPolicyEngineExam
{
    private static AccessPolicyContext CreateContext(AccessPolicyMode mode)
    {
        return new()
        {
            AccessKeyRealmID = "Realm-A",
            Claims = new AccessKeyClaims { Subject = "subject-a", ToolSchemaID = "KusDepot.Tools/Schema", AccessKeyRealmID = "Realm-A", ManifestHash = "HASH-A" },
            EvaluatedAt = DateTimeOffset.UtcNow,
            Expired = false,
            ManifestHash = "HASH-A",
            ManifestMatched = true,
            MembershipActive = true,
            Mode = mode,
            Subject = "subject-a",
            ToolSchemaID = "KusDepot.Tools/Schema"
        };
    }

    private static AccessPolicyContext CopyContext(AccessPolicyContext source , AccessKeyClaims? claims = null , Boolean replaceclaims = false , Boolean? expired = null , Boolean? manifestmatched = null , Boolean? membershipactive = null , Int32? operation = null , Boolean? operationallowed = null , String? subject = null)
    {
        return new()
        {
            AccessKeyRealmID = source.AccessKeyRealmID,
            Claims = replaceclaims ? claims : source.Claims,
            EvaluatedAt = source.EvaluatedAt,
            Expired = expired ?? source.Expired,
            IssueOptions = source.IssueOptions,
            Key = source.Key,
            ManifestHash = source.ManifestHash,
            ManifestMatched = manifestmatched ?? source.ManifestMatched,
            MembershipActive = membershipactive ?? source.MembershipActive,
            Mode = source.Mode,
            Operation = operation ?? source.Operation,
            OperationAllowed = operationallowed ?? source.OperationAllowed,
            Operations = source.Operations,
            RequestContext = source.RequestContext,
            Subject = subject ?? source.Subject,
            Tool = source.Tool,
            ToolSchemaID = source.ToolSchemaID
        };
    }

    [Test]
    public void Evaluate_Authorize_Allows_When_Facts_Are_Valid()
    {
        AccessPolicyContext context = CreateContext(AccessPolicyMode.Authorize);

        AccessPolicyDecision decision = DefaultAccessPolicyEngine.Instance.Evaluate(in context);

        Check.That(decision.Allowed).IsTrue();
    }

    [Test]
    public void Evaluate_Authorize_Denies_When_Membership_Is_Inactive()
    {
        AccessPolicyContext context = CopyContext(CreateContext(AccessPolicyMode.Authorize),membershipactive: false);

        AccessPolicyDecision decision = DefaultAccessPolicyEngine.Instance.Evaluate(in context);

        Check.That(decision.Allowed).IsFalse();
    }

    [Test]
    public void Evaluate_Access_Denies_When_Operation_Is_Not_Allowed()
    {
        AccessPolicyContext context = CopyContext(CreateContext(AccessPolicyMode.Access),operation: ProtectedOperation.GetOutputIDs,operationallowed: false);

        AccessPolicyDecision decision = DefaultAccessPolicyEngine.Instance.Evaluate(in context);

        Check.That(decision.Allowed).IsFalse();
    }

    [Test]
    public void Evaluate_Issue_Denies_When_Subject_Is_Empty()
    {
        AccessPolicyContext context = CopyContext(CreateContext(AccessPolicyMode.Issue),claims: null,replaceclaims: true,membershipactive: false,manifestmatched: true,subject: String.Empty);

        AccessPolicyDecision decision = DefaultAccessPolicyEngine.Instance.Evaluate(in context);

        Check.That(decision.Allowed).IsFalse();
    }

    [Test]
    public void Evaluate_Revoke_Allows_When_Claims_Are_Present_And_Manifest_Matches()
    {
        AccessPolicyContext context = CopyContext(CreateContext(AccessPolicyMode.Revoke),expired: true,membershipactive: false);

        AccessPolicyDecision decision = DefaultAccessPolicyEngine.Instance.Evaluate(in context);

        Check.That(decision.Allowed).IsTrue();
    }

    [Test]
    public void Evaluate_Request_Allows_When_Request_Context_Is_Valid()
    {
        AccessPolicyContext context = new()
        {
            AccessKeyRealmID = "Realm-A",
            EvaluatedAt = DateTimeOffset.UtcNow,
            ManifestHash = "HASH-A",
            ManifestMatched = true,
            Mode = AccessPolicyMode.Request,
            RequestContext = new AccessPolicyRequestContext
            {
                Request = new StandardRequest("request-data"),
                RequestedKeyType = nameof(ClientKey),
                RequestValidated = true
            },
            ToolSchemaID = "KusDepot.Tools/Schema"
        };

        AccessPolicyDecision decision = DefaultAccessPolicyEngine.Instance.Evaluate(in context);

        Check.That(decision.Allowed).IsTrue();
    }

    [Test]
    public void Evaluate_Request_Denies_When_Request_Context_Is_Invalid()
    {
        AccessPolicyContext context = new()
        {
            AccessKeyRealmID = "Realm-A",
            EvaluatedAt = DateTimeOffset.UtcNow,
            ManifestHash = "HASH-A",
            ManifestMatched = true,
            Mode = AccessPolicyMode.Request,
            RequestContext = new AccessPolicyRequestContext
            {
                Request = new StandardRequest("request-data"),
                RequestedKeyType = nameof(ClientKey),
                RequestValidated = false
            },
            ToolSchemaID = "KusDepot.Tools/Schema"
        };

        AccessPolicyDecision decision = DefaultAccessPolicyEngine.Instance.Evaluate(in context);

        Check.That(decision.Allowed).IsFalse();
    }

    [Test]
    public async Task EvaluateAsync_Authorize_Allows_When_Facts_Are_Valid()
    {
        AccessPolicyContext context = CreateContext(AccessPolicyMode.Authorize);

        AccessPolicyDecision decision = await DefaultAccessPolicyEngine.Instance.EvaluateAsync(context);

        Check.That(decision.Allowed).IsTrue();
    }
}
