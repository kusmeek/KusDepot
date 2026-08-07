namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public partial class AccessKeyAssertionRequestExam
{
    private static Byte[] GetRealmKey(AccessManager manager)
    {
        FieldInfo field = typeof(AccessManager).GetField("RealmKey",BindingFlags.NonPublic | BindingFlags.Instance)!;
        return (Byte[])field.GetValue(manager)!;
    }

    private static (String ToolSchemaID,String AccessKeyRealmID) GetManifestIdentity(AccessManager manager)
    {
        FieldInfo field = typeof(AccessManager).GetField("ManifestIdentity",BindingFlags.NonPublic | BindingFlags.Instance)!;
        Object identity = field.GetValue(manager)!;
        return (
            identity.GetType().GetProperty("ToolSchemaID")!.GetValue(identity) as String ?? String.Empty,
            identity.GetType().GetProperty("AccessKeyRealmID")!.GetValue(identity) as String ?? String.Empty);
    }

    private sealed class CapturingAssertionGenerator(params AccessKeyAssertion[] assertions) : IAccessKeyAssertionGenerator
    {
        public AccessKeyAssertionGeneratorContext? LastContext { get; private set; }

        public ImmutableArray<AccessKeyAssertion> Generate(in AccessKeyAssertionGeneratorContext context)
        {
            LastContext = context;
            return assertions.ToImmutableArray();
        }
    }

    private sealed class ExposedAssertionContextAccessManager : AccessManager
    {
        public ExposedAssertionContextAccessManager(AccessManagerOptions? options = null , ITool? tool = null , ILogger? logger = null)
            : base(options,tool,logger) {}

        public AccessKeyAssertionGeneratorContext CreateAssertionContextForExam(AccessPolicyContext context , String requestedkeytype , AccessPolicyRequestContext? requestcontext = null)
        {
            FieldInfo field = typeof(AccessManager).GetField("ManifestIdentity",BindingFlags.NonPublic | BindingFlags.Instance)!;
            Object manifestidentity = field.GetValue(this)!;
            MethodInfo method = typeof(AccessManager).GetMethod("CreateAssertionGeneratorContext",BindingFlags.Instance | BindingFlags.NonPublic)!;
            return (AccessKeyAssertionGeneratorContext)method.Invoke(this,[context,manifestidentity,requestedkeytype,requestcontext])!;
        }
    }

    [Test]
    public void GenerateAccessKey_WithKeyAssertionGenerator_PersistsAndRecoversAssertionSummaries()
    {
        Tool tool = new();
        TokenAssertion assertion = new()
        {
            AuthenticationContextClassReference = "urn:kdp:manager",
            Audiences = ["aud-manager"],
            AuthenticationMethods = ["pwd","mfa"],
            AuthenticationTime = DateTimeOffset.UtcNow.AddMinutes(-1),
            AuthorizedParty = "azp-manager",
            ClientID = "client-manager",
            Format = "jwt/v1",
            IssuedAt = DateTimeOffset.UtcNow.AddMinutes(-2),
            Issuer = "https://issuer.manager.example",
            JwtID = "jwt-manager",
            NotAfter = DateTimeOffset.UtcNow.AddMinutes(10),
            NotBefore = DateTimeOffset.UtcNow.AddMinutes(-3),
            ObjectID = "object-manager",
            Scopes = ["scope.manager"],
            SessionID = "sid-manager",
            Subject = "subject-manager",
            TenantID = "tenant-manager",
            Token = Encoding.UTF8.GetBytes("manager-jwt"),
            TokenType = TokenKeyType.Jwt
        };
        CapturingAssertionGenerator generator = new(assertion);
        AccessManagerOptions options = new(keyassertiongenerator: generator);
        AccessManager manager = new(options,tool);

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "manager-subject",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(key).IsNotNull();
        Check.That(generator.LastContext).IsNotNull();
        Check.That(generator.LastContext!.Value.Subject).IsEqualTo("manager-subject");
        Check.That(generator.LastContext!.Value.Request).IsNull();
        Check.That(generator.LastContext!.Value.RequestContext).IsNull();
        Check.That(generator.LastContext!.Value.RequestedKeyType).IsEqualTo(nameof(ClientKey));
        Check.That(generator.LastContext!.Value.AccessPolicyContext.Mode).IsEqualTo(AccessPolicyMode.Issue);

        Byte[] realmkey = GetRealmKey(manager);
        var identity = GetManifestIdentity(manager);
        Check.That(AccessKeySecret.TryReadClaims(key!.Key!,realmkey,identity.ToolSchemaID,identity.AccessKeyRealmID,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.AssertionSummaries.Length).IsEqualTo(1);
        Check.That(claims.AssertionSummaries[0]).IsInstanceOf<TokenAssertionSummary>();
        TokenAssertionSummary summary = (TokenAssertionSummary)claims.AssertionSummaries[0];
        Check.That(summary.AssertionType).IsEqualTo("token");
        Check.That(summary.AuthenticationContextClassReference).IsEqualTo("urn:kdp:manager");
        Check.That(summary.Format).IsEqualTo("jwt/v1");
        Check.That(summary.Issuer).IsEqualTo("https://issuer.manager.example");
        Check.That(summary.JwtID).IsEqualTo("jwt-manager");
        Check.That(summary.NotBefore).IsEqualTo(assertion.NotBefore);
        Check.That(summary.SessionID).IsEqualTo("sid-manager");
        Check.That(summary.Subject).IsEqualTo("subject-manager");
        Check.That(summary.Audiences.SequenceEqual(["aud-manager"])).IsTrue();
        Check.That(summary.AuthorizedParty).IsEqualTo("azp-manager");
        Check.That(summary.Scopes.SequenceEqual(["scope.manager"])).IsTrue();
        Check.That(summary.AuthenticationMethods.SequenceEqual(["pwd","mfa"])).IsTrue();
    }

    [Test]
    public void CreateAssertionGeneratorContext_WithRequestContext_UsesRequestContextRequest()
    {
        Tool tool = new();
        ExposedAssertionContextAccessManager manager = new(null,tool);
        StandardRequest request = new("request-subject");
        AccessPolicyRequestContext requestcontext = new()
        {
            Request = request,
            RequestedKeyType = nameof(ClientKey),
            RequestValidated = true
        };
        AccessPolicyContext accesspolicycontext = new()
        {
            Mode = AccessPolicyMode.Issue,
            Subject = "request-subject",
            Tool = tool,
            IssueOptions = AccessKeyIssueOptions.Create(operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs))
        };

        AccessKeyAssertionGeneratorContext context = manager.CreateAssertionContextForExam(accesspolicycontext,nameof(ClientKey),requestcontext);

        Check.That(context.RequestContext).IsEqualTo(requestcontext);
        Check.That(context.Request).IsSameReferenceAs(request);
        Check.That(context.Subject).IsEqualTo("request-subject");
        Check.That(context.RequestedKeyType).IsEqualTo(nameof(ClientKey));
    }
}
