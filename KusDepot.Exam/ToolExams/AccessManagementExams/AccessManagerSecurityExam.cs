namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AccessManagerExam
{
    private sealed class CapturingSignatureProcessor : IAccessKeySignatureProcessor
    {
        private readonly ImmutableArray<Byte> signerKeyId = SHA256.HashData(Encoding.UTF8.GetBytes("access-manager-signature-processor")).ToImmutableArray();

        public ILogger? LastSigningLogger { get; private set; }
        public ILogger? LastValidationLogger { get; private set; }
        public Int32 SignCalls { get; private set; }
        public Int32 VerifyCalls { get; private set; }

        public AccessKeySignatureInfo GetSigningInfo(AccessKeySignatureSigningContext context)
        {
            LastSigningLogger = context.Logger;
            return new()
            {
                Algorithm = AccessKeySignatureAlgorithm.RsaPssSha512,
                SignerKeyID = signerKeyId,
                SignerKeyIDFormat = AccessKeySignerKeyIdFormat.Thumbprint,
                SignatureLength = 64
            };
        }

        public Boolean TrySign(AccessKeySignatureSigningContext context , Span<Byte> signature , out Int32 written)
        {
            SignCalls++;
            LastSigningLogger = context.Logger;
            Byte[] digest = SHA512.HashData(context.SignedRegion.Span).AsSpan(0,64).ToArray();
            if(signature.Length < digest.Length) { written = 0; return false; }
            digest.CopyTo(signature);
            written = digest.Length;
            return true;
        }

        public SignatureValidation Verify(AccessKeySignatureValidationContext context)
        {
            VerifyCalls++;
            LastValidationLogger = context.Logger;
            return new()
            {
                Algorithm = context.Signature.Algorithm,
                CertificateResolved = true,
                CertificateThumbprint = Convert.ToHexString(context.Signature.SignerKeyID.AsSpan()),
                ChainTrusted = true,
                SignatureValid = true,
                Signed = context.Signature.Signed,
                SignerAccessManagerID = context.Signature.SignerAccessManagerID,
                SignerKeyID = context.Signature.SignerKeyID,
                SignerKeyIDFormat = context.Signature.SignerKeyIDFormat,
                SignerToolID = context.Signature.SignerToolID,
                SignerTrusted = true,
                VerificationAttempted = true
            };
        }
    }

    private sealed class SignOnlySignatureProcessor : IAccessKeySigner
    {
        private readonly ImmutableArray<Byte> signerKeyId = SHA256.HashData(Encoding.UTF8.GetBytes("access-manager-sign-only-processor")).ToImmutableArray();

        public Int32 SignCalls { get; private set; }
        public ILogger? LastSigningLogger { get; private set; }

        public AccessKeySignatureInfo GetSigningInfo(AccessKeySignatureSigningContext context)
        {
            LastSigningLogger = context.Logger;
            return new()
            {
                Algorithm = AccessKeySignatureAlgorithm.RsaPssSha512,
                SignerKeyID = signerKeyId,
                SignerKeyIDFormat = AccessKeySignerKeyIdFormat.Thumbprint,
                SignatureLength = 64
            };
        }

        public Boolean TrySign(AccessKeySignatureSigningContext context , Span<Byte> signature , out Int32 written)
        {
            SignCalls++;
            LastSigningLogger = context.Logger;
            Byte[] digest = SHA512.HashData(context.SignedRegion.Span).AsSpan(0,64).ToArray();
            if(signature.Length < digest.Length) { written = 0; return false; }
            digest.CopyTo(signature);
            written = digest.Length;
            return true;
        }
    }

    private sealed class CaptureAuthorizeSignaturePolicyEngine : IAccessPolicyEngine
    {
        public SignatureValidation AuthorizeSignatureValidation { get; private set; }

        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            if(context.Mode == AccessPolicyMode.Authorize)
            {
                AuthorizeSignatureValidation = context.SignatureValidation;
            }

            return AccessPolicyDecision.Allow();
        }
    }

    private static Object? GetManifestIdentity(AccessManager manager)
    {
        FieldInfo field = typeof(AccessManager).GetField("ManifestIdentity",BindingFlags.NonPublic | BindingFlags.Instance)!;
        return field.GetValue(manager);
    }

    private static Dictionary<AccessKeyToken,MembershipToken> GetMembershipsByToken(AccessManager manager)
    {
        FieldInfo field = typeof(AccessManager).GetField("MembershipsByToken",BindingFlags.NonPublic | BindingFlags.Instance)!;
        return (Dictionary<AccessKeyToken,MembershipToken>)field.GetValue(manager)!;
    }

    private static Object? GetPrivateField(Object target , String name)
    {
        return target.GetType().GetField(name,BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(target);
    }

    private static CustomIssuedKey? CreateCustomIssuedKey(KeyMaterial material)
    {
        return material.Key is null || material.ID is null ? null : new CustomIssuedKey(material.Key,material.ID);
    }

    private static void SetManifestIdentity(AccessManager manager , String toolschemaid , String accesskeyrealmid , String manifesthash)
    {
        Type identityType = typeof(ToolManifest).Assembly.GetType("KusDepot.ToolManifestIdentity",throwOnError: true)!;
        Object identity = Activator.CreateInstance(identityType)!;
        identityType.GetProperty("AccessKeyRealmID")!.SetValue(identity,accesskeyrealmid);
        identityType.GetProperty("ToolSchemaID")!.SetValue(identity,toolschemaid);
        identityType.GetProperty("ManifestHash")!.SetValue(identity,manifesthash);

        FieldInfo field = typeof(AccessManager).GetField("ManifestIdentity",BindingFlags.NonPublic | BindingFlags.Instance)!;
        field.SetValue(manager,identity);
    }

    private static (String ToolSchemaID,String AccessKeyRealmID,ImmutableArray<String> Audiences,ImmutableArray<String> Scopes,String ManifestHash) ReadClaims(Byte[] secret , Byte[] realmKey , String toolschemaid , String accesskeyrealmid)
    {
        if(!AccessKeySecret.TryReadClaims(secret,realmKey,toolschemaid,accesskeyrealmid,out AccessKeyClaims claims))
        {
            throw new InvalidOperationException("Failed to read access-key claims from the issued secret.");
        }

        return (claims.ToolSchemaID,claims.AccessKeyRealmID,claims.Audiences,claims.Scopes,claims.ManifestHash);
    }

    [Test]
    public void AccessCheck()
    {
        Tool _0 = new();

        var KeyO = new OwnerKey(SerializeCertificate(CreateCertificate(Guid.NewGuid(),"Owner"))!);

        Check.That(KeyO.ID).IsNotNull(); Check.That(_0.TakeOwnership(KeyO)).IsTrue();

        ClientKey? k1 = _0.RequestAccess(new StandardRequest("AccessCheck")) as ClientKey; Check.That(k1).IsNotNull();

        ExecutiveKey? k2 = _0.RequestAccess(new ManagementRequest(KeyO!) { Operations = [.. ProtectedOperationSets.Executive, ProtectedOperation.GetInputs] }) as ExecutiveKey; Check.That(k2).IsNotNull();

        HostKey? k3 = _0.RequestAccess(new HostRequest(null,true)) as HostKey; Check.That(k3).IsNotNull();

        Check.That(_0.Lock(KeyO)).IsTrue();

        Check.That(_0.AddOutput(Guid.NewGuid(),"AccessCheck",k2)).IsTrue();

        Check.That(_0.AddInput("AccessCheck",k2)).IsTrue();

        Check.That(_0.GetWorkingSet(k1)).IsNull();

        Check.That(_0.GetWorkingSet(k2)).IsNotNull();

        Check.That(_0.GetWorkingSet(k3)).IsNull();

        Check.That(_0.GetInputs(k1)).IsNull();

        Check.That(_0.GetInputs(k2)).IsNotNull();

        Check.That(_0.GetInputs(k3)).IsNull();

        Check.That(_0.GetOutputIDs(k1)).IsNotNull();

        Check.That(_0.GetOutputIDs(k2)).IsNotNull();

        Check.That(_0.GetOutputIDs(k3)).IsNull();
    }

    [Test]
    public void GenerateAccessKey_Generic_Generates_ClientKey()
    {
        var manager = new AccessManager(new Tool());

        var key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: Guid.NewGuid().ToString(),operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(key).IsNotNull();
        Check.That(key).IsInstanceOf<ClientKey>();
    }

    [Test]
    public void GenerateAccessKey_Generic_Options_Uses_Lifetime_Audiences_And_Scopes()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var manager = tool.GetAccessManager(executiveKey) as AccessManager;

        Check.That(manager).IsNotNull();

        var key = manager!.GenerateAccessKey<ClientKey>(
            AccessKeyIssueOptions.Create(subject: Guid.NewGuid().ToString())
                .AddOperation(ProtectedOperation.GetOutputIDs)
                .SetLifetime(TimeSpan.FromMinutes(3))
                .AddAudience("aud-b")
                .AddAudience("aud-a")
                .AddAudience("aud-a")
                .AddScope("scope.z")
                .AddScope("scope.a")
                .AddScope("scope.a"),
            executiveKey);

        Check.That(key).IsNotNull();

        FieldInfo realmKeyField = typeof(AccessManager).GetField("RealmKey",BindingFlags.NonPublic | BindingFlags.Instance)!;
        Byte[] realmKey = (Byte[])realmKeyField.GetValue(manager)!;
        Object identity = GetManifestIdentity(manager)!;
        String toolschemaid = identity.GetType().GetProperty("ToolSchemaID")!.GetValue(identity) as String ?? String.Empty;
        String accesskeyrealmid = identity.GetType().GetProperty("AccessKeyRealmID")!.GetValue(identity) as String ?? String.Empty;

        var claims = ReadClaims(key!.Key!,realmKey,toolschemaid,accesskeyrealmid);

        Check.That(claims.ToolSchemaID).IsEqualTo(toolschemaid);
        Check.That(claims.AccessKeyRealmID).IsEqualTo(accesskeyrealmid);
        Check.That(claims.Audiences.SequenceEqual(["aud-a","aud-b"])).IsTrue();
        Check.That(claims.Scopes.SequenceEqual(["scope.a","scope.z"])).IsTrue();
    }

    [Test]
    public void GenerateAccessKey_Generic_Uses_Configured_SignatureProcessor_For_Signed_Issuance()
    {
        CapturingSignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(signatureprocessor: signatureprocessor),new Tool());

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "signed-subject",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(key).IsNotNull();
        Check.That(signatureprocessor.SignCalls).IsEqualTo(1);
        Check.That(AccessKeySecret.IsSigned(key!.Key!)).IsTrue();
    }

    [Test]
    public void Authorize_Populates_SignatureValidation_When_CustomPolicyEngine_And_SignatureProcessor_Are_Configured()
    {
        CaptureAuthorizeSignaturePolicyEngine policyengine = new();
        CapturingSignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: policyengine,signatureprocessor: signatureprocessor),new Tool());

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "signed-authorize",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsTrue();
        Check.That(signatureprocessor.VerifyCalls).IsEqualTo(1);
        Check.That(signatureprocessor.LastSigningLogger).IsNotNull();
        Check.That(signatureprocessor.LastValidationLogger).IsNotNull();
        Check.That(policyengine.AuthorizeSignatureValidation.Signed).IsTrue();
        Check.That(policyengine.AuthorizeSignatureValidation.VerificationAttempted).IsTrue();
        Check.That(policyengine.AuthorizeSignatureValidation.SignatureValid).IsTrue();
    }

    [Test]
    public void Authorize_Populates_Unsigned_Default_SignatureValidation_When_No_Processor_Is_Configured()
    {
        CaptureAuthorizeSignaturePolicyEngine policyengine = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: policyengine),new Tool());

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "unsigned-authorize",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsTrue();
        Check.That(policyengine.AuthorizeSignatureValidation.Signed).IsFalse();
        Check.That(policyengine.AuthorizeSignatureValidation.VerificationAttempted).IsFalse();
        Check.That(policyengine.AuthorizeSignatureValidation.FailureReason).IsNull();
    }

    [Test]
    public void Authorize_Populates_Signed_Unverified_Default_SignatureValidation_When_Signer_Only_Is_Configured()
    {
        CaptureAuthorizeSignaturePolicyEngine policyengine = new();
        SignOnlySignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: policyengine,signatureprocessor: signatureprocessor),new Tool());

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "signed-unverified-authorize",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsTrue();
        Check.That(signatureprocessor.SignCalls).IsEqualTo(1);
        Check.That(signatureprocessor.LastSigningLogger).IsNotNull();
        Check.That(policyengine.AuthorizeSignatureValidation.Signed).IsTrue();
        Check.That(policyengine.AuthorizeSignatureValidation.VerificationAttempted).IsFalse();
        Check.That(policyengine.AuthorizeSignatureValidation.SignatureValid).IsFalse();
        Check.That(policyengine.AuthorizeSignatureValidation.FailureReason).IsEqualTo("Signature verification was not attempted.");
    }

    [Test]
    public void Authorize_DefaultPolicyEngine_Still_Verifies_Signed_Key_When_SignatureProcessor_Is_Configured()
    {
        CapturingSignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(signatureprocessor: signatureprocessor),new Tool());

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "signed-default-authorize",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsTrue();
        Check.That(signatureprocessor.VerifyCalls).IsEqualTo(1);
    }

    [Test]
    public void AccessCheck_DefaultPolicyEngine_Still_Verifies_Signed_Key_When_SignatureProcessor_Is_Configured()
    {
        CapturingSignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(signatureprocessor: signatureprocessor),new Tool());

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "signed-default-accesscheck",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.AccessCheck(key,ProtectedOperation.GetOutputIDs);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsTrue();
        Check.That(signatureprocessor.VerifyCalls).IsEqualTo(1);
    }

    [Test]
    public void Authorize_Denies_Unsigned_Key_When_Policy_Requires_Signed_Key()
    {
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: new RequireSignedPolicyEngine()),new Tool());

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "unsigned-required-signed",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsFalse();
    }

    [Test]
    public void Authorize_Denies_Signed_Unverified_Key_When_Policy_Requires_Verified_Signature()
    {
        SignOnlySignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: new RequireVerifiedSignaturePolicyEngine(),signatureprocessor: signatureprocessor),new Tool());

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "signed-required-verified",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsFalse();
    }

    [Test]
    public void Authorize_Allows_Verified_Key_When_Policy_Requires_Verified_Signature()
    {
        CapturingSignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: new RequireVerifiedSignaturePolicyEngine(),signatureprocessor: signatureprocessor),new Tool());

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "verified-required-verified",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsTrue();
        Check.That(signatureprocessor.VerifyCalls).IsEqualTo(1);
    }

    [Test]
    public void Authorize_Allows_Verified_Trusted_Key_When_Policy_Requires_Trusted_Signature()
    {
        CapturingSignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: new RequireTrustedSignaturePolicyEngine(),signatureprocessor: signatureprocessor),new Tool());

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "trusted-required-trusted",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsTrue();
        Check.That(signatureprocessor.VerifyCalls).IsEqualTo(1);
    }

    [Test]
    public void AccessCheck_Denies_Unsigned_Key_When_Policy_Requires_Signed_Key()
    {
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: new RequireSignedPolicyEngine()),new Tool());

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "unsigned-access-signed",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.AccessCheck(key,ProtectedOperation.GetOutputIDs);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsFalse();
    }

    [Test]
    public void GenerateAccessKey_Generic_NullPermissions_Uses_Empty_Set()
    {
        var manager = new AccessManager(new Tool());

        var key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: Guid.NewGuid().ToString()));

        Check.That(key).IsNotNull();
        Check.That(key).IsInstanceOf<ClientKey>();
    }

    [Test]
    public void GenerateAccessKey_Generic_Uses_AccessCheck_When_Locked()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();

        var executiveKey = tool.CreateExecutiveKey(ownerKey);
        Check.That(executiveKey).IsNotNull();
        Check.That(tool.Lock(ownerKey)).IsTrue();

        var manager = tool.GetAccessManager(executiveKey) as AccessManager;
        Check.That(manager).IsNotNull();

        var denied = manager!.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: Guid.NewGuid().ToString(),operations: ImmutableArray<Int32>.Empty),null);
        var allowed = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: Guid.NewGuid().ToString(),operations: ImmutableArray<Int32>.Empty),executiveKey);

        Check.That(denied).IsNull();
        Check.That(allowed).IsNotNull();
        Check.That(allowed).IsInstanceOf<ClientKey>();
    }

    public class DenyingAccessManager : AccessManager
    {
        public DenyingAccessManager(ITool? tool = null, ILogger? logger = null)
            : base(tool, logger) { }

        public override Boolean AccessCheck(AccessKey? key = null, [CallerMemberName] String? operationname = null)
        {
            return false;
        }

        public override Task<Boolean> AccessCheckAsync(AccessKey? key = null , String? operationname = null , CancellationToken cancel = default)
        {
            return cancel.IsCancellationRequested ? Task.FromCanceled<Boolean>(cancel) : Task.FromResult(false);
        }

        public override Task<Boolean> AccessCheckAsync(AccessKey? key , Int32 operation , CancellationToken cancel = default)
        {
            return cancel.IsCancellationRequested ? Task.FromCanceled<Boolean>(cancel) : Task.FromResult(false);
        }
    }

    public sealed class DenyAuthorizationPolicyEngine : IAccessPolicyEngine
    {
        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            return context.Mode is AccessPolicyMode.Authorize or AccessPolicyMode.Access
                ? AccessPolicyDecision.Deny("authorization denied by engine")
                : AccessPolicyDecision.Allow();
        }
    }

    public sealed class RequireSignedPolicyEngine : IAccessPolicyEngine
    {
        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            return context.Mode is AccessPolicyMode.Authorize or AccessPolicyMode.Access
                ? context.SignatureValidation.Signed
                    ? AccessPolicyDecision.Allow()
                    : AccessPolicyDecision.Deny("signed key required")
                : AccessPolicyDecision.Allow();
        }
    }

    public sealed class RequireVerifiedSignaturePolicyEngine : IAccessPolicyEngine
    {
        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            return context.Mode is AccessPolicyMode.Authorize or AccessPolicyMode.Access
                ? context.SignatureValidation.VerificationAttempted && context.SignatureValidation.SignatureValid
                    ? AccessPolicyDecision.Allow()
                    : AccessPolicyDecision.Deny("verified signature required")
                : AccessPolicyDecision.Allow();
        }
    }

    public sealed class RequireTrustedSignaturePolicyEngine : IAccessPolicyEngine
    {
        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            return context.Mode is AccessPolicyMode.Authorize or AccessPolicyMode.Access
                ? context.SignatureValidation.SignatureValid && context.SignatureValidation.SignerTrusted && context.SignatureValidation.ChainTrusted
                    ? AccessPolicyDecision.Allow()
                    : AccessPolicyDecision.Deny("trusted signature required")
                : AccessPolicyDecision.Allow();
        }
    }

    public sealed class DenyRequestPolicyEngine : IAccessPolicyEngine
    {
        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            return context.Mode == AccessPolicyMode.Request
                ? AccessPolicyDecision.Deny("request denied by engine")
                : AccessPolicyDecision.Allow();
        }
    }

    public sealed class DenyIssuancePolicyEngine : IAccessPolicyEngine
    {
        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            return context.Mode == AccessPolicyMode.Issue
                ? AccessPolicyDecision.Deny("issuance denied by engine")
                : AccessPolicyDecision.Allow();
        }
    }

    public sealed class DenyRevocationPolicyEngine : IAccessPolicyEngine
    {
        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            return context.Mode == AccessPolicyMode.Revoke
                ? AccessPolicyDecision.Deny("revocation denied by engine")
                : AccessPolicyDecision.Allow();
        }
    }

    public sealed class PolicyEngineAccessManager : AccessManager
    {
        public PolicyEngineAccessManager(ITool? tool = null, ILogger? logger = null)
            : base(AccessManagerOptions.Create(policyengine: new DenyAuthorizationPolicyEngine()), tool, logger) { }
    }

    public sealed class RequestPolicyEngineAccessManager : AccessManager
    {
        public RequestPolicyEngineAccessManager(ITool? tool = null, ILogger? logger = null)
            : base(AccessManagerOptions.Create(policyengine: new DenyRequestPolicyEngine()), tool, logger) { }
    }

    public sealed class RequestedKeyTypeAccessManager : AccessManager
    {
        public RequestedKeyTypeAccessManager(ITool? tool = null, ILogger? logger = null)
            : base(tool, logger) { }

        public String? NormalizeRequestedKeyTypePublic(String? requestedkeytype) => NormalizeRequestedKeyType(requestedkeytype);
    }

    public sealed record class CustomIssuedKey : AccessKey
    {
        public CustomIssuedKey() {}

        public CustomIssuedKey(Byte[] key , Guid? id = null)
        {
            ID = id ?? Guid.NewGuid(); Key = key.ToArray();
        }
    }

    public sealed class CustomRequestAccessManager : AccessManager
    {
        public CustomRequestAccessManager(AccessManagerOptions? options = null , ITool? tool = null , ILogger? logger = null)
            : base(options,tool,logger) { }

        protected override AccessPolicyRequestContext CreateRequestPolicyContext(AccessRequest request , ITool tool)
        {
            return new()
            {
                Audiences = request.Audiences,
                Credential = request.Credential,
                Lifetime = request.Lifetime,
                Properties = request.Properties,
                Request = request,
                RequestedAssertions = request.RequestedAssertions,
                RequestedKeyType = NormalizeRequestedKeyType(typeof(CustomIssuedKey).FullName)!,
                RequestValidated = request.Lifetime is null || request.Lifetime > TimeSpan.Zero,
                Scopes = request.Scopes,
                Subject = request.Subject ?? String.Empty
            };
        }
    }

    public sealed class OverriddenPolicyEngineAccessManager : AccessManager
    {
        public OverriddenPolicyEngineAccessManager(AccessManagerOptions? options = null , ITool? tool = null , ILogger? logger = null)
            : base(AccessManagerOptions.Create(policyengine: new DenyIssuancePolicyEngine()), tool, logger) { }
    }

    public sealed class IssuancePolicyEngineAccessManager : AccessManager
    {
        public IssuancePolicyEngineAccessManager(ITool? tool = null, ILogger? logger = null)
            : base(AccessManagerOptions.Create(policyengine: new DenyIssuancePolicyEngine()), tool, logger) { }
    }

    public sealed class RevocationPolicyEngineAccessManager : AccessManager
    {
        public RevocationPolicyEngineAccessManager(ITool? tool = null, ILogger? logger = null)
            : base(AccessManagerOptions.Create(policyengine: new DenyRevocationPolicyEngine()), tool, logger) { }
    }

    public sealed class PipelinePolicyEngineAccessManager : AccessManager
    {
        private sealed class AllowStage : IAccessPolicyStage
        {
            public AccessPolicyDecision Evaluate(in AccessPolicyContext context) => AccessPolicyDecision.Allow();
        }

        private sealed class DenyAuthorizeStage : IAccessPolicyStage
        {
            public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
            {
                return context.Mode == AccessPolicyMode.Authorize ? AccessPolicyDecision.Deny("authorize denied by stage") : AccessPolicyDecision.Allow();
            }
        }

        public PipelinePolicyEngineAccessManager(ITool? tool = null, ILogger? logger = null)
            : base(AccessManagerOptions.Create(policyengine: new PipelineAccessPolicyEngine([new AllowStage(),new DenyAuthorizeStage()])), tool, logger) { }
    }

    [Test]
    public async Task Denying_AccessManager()
    {
        var denyingManager = new DenyingAccessManager();
        var tool = new Tool(accessmanager: denyingManager);
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;

        Check.That(tool.Lock(ownerKey)).IsTrue();

        Boolean activationResult = await tool.Activate(key: executiveKey);

        Check.That(activationResult).IsFalse();
        Check.That(tool.GetLifeCycleState()).IsEqualTo(LifeCycleState.Initialized);
    }

    [Test]
    public void RequestAccess_Denies_StandardRequest_When_CustomPolicyEngine_Denies_Request()
    {
        var manager = new RequestPolicyEngineAccessManager();
        var tool = new Tool(accessmanager: manager);

        AccessKey? key = tool.RequestAccess(new StandardRequest("subject-a"));

        Check.That(tool).IsNotNull();
        Check.That(key).IsNull();
    }

    [Test]
    public void RequestAccess_Denies_HostRequest_When_CustomPolicyEngine_Denies_Request()
    {
        var manager = new RequestPolicyEngineAccessManager();
        var tool = new Tool(accessmanager: manager);

        AccessKey? key = tool.RequestAccess(new HostRequest(null,true));

        Check.That(tool).IsNotNull();
        Check.That(key).IsNull();
    }

    [Test]
    public void RequestAccess_Denies_ServiceRequest_When_CustomPolicyEngine_Denies_Request()
    {
        var manager = new RequestPolicyEngineAccessManager();
        var tool = new Tool(accessmanager: manager);

        AccessKey? key = tool.RequestAccess(new ServiceRequest(new Tool(),String.Empty));

        Check.That(tool).IsNotNull();
        Check.That(key).IsNull();
    }

    [Test]
    public void RequestAccess_Denies_ManagementRequest_When_CustomPolicyEngine_Denies_Request()
    {
        var manager = new RequestPolicyEngineAccessManager();
        var tool = new Tool(accessmanager: manager);
        OwnerKey? ownerKey = tool.CreateOwnerKey("Owner");

        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();

        AccessKey? key = tool.RequestAccess(new ManagementRequest(ownerKey!));

        Check.That(key).IsNull();
    }

    [Test]
    public void NormalizeRequestedKeyType_Returns_Canonical_Simple_Name_For_BuiltIn_Fully_Qualified_Type()
    {
        var manager = new RequestedKeyTypeAccessManager();

        String? normalized = manager.NormalizeRequestedKeyTypePublic("KusDepot.Security.ClientKey");

        Check.That(normalized).IsEqualTo(nameof(ClientKey));
    }

    [Test]
    public void GenerateAccessKey_Generic_Generates_Custom_Key_Using_Configured_KeyMaker()
    {
        var keymaker = new KeyMaker().Register(typeof(CustomIssuedKey).FullName,CreateCustomIssuedKey);
        var options = AccessManagerOptions.Create(keymaker: keymaker);
        var manager = new AccessManager(options,new Tool());

        CustomIssuedKey? key = manager.GenerateAccessKey<CustomIssuedKey>(AccessKeyIssueOptions.Create(subject: Guid.NewGuid().ToString(),operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(key).IsNotNull();
        Check.That(key).IsInstanceOf<CustomIssuedKey>();
        Check.That(GetPrivateField(keymaker,"Key")).IsNull();
        Check.That(GetPrivateField(keymaker,"ID")).IsNull();
    }

    [Test]
    public void RequestAccess_Generates_Custom_Key_When_Request_Normalization_Uses_Custom_RequestedKeyType()
    {
        var keymaker = new KeyMaker().Register(typeof(CustomIssuedKey).FullName,CreateCustomIssuedKey);
        var options = AccessManagerOptions.Create(keymaker: keymaker);
        var manager = new CustomRequestAccessManager(options);
        var tool = new Tool(accessmanager: manager);

        AccessKey? key = tool.RequestAccess(new StandardRequest("custom-subject"));

        Check.That(key).IsNotNull();
        Check.That(key).IsInstanceOf<CustomIssuedKey>();
        Check.That(GetPrivateField(keymaker,"Key")).IsNull();
        Check.That(GetPrivateField(keymaker,"ID")).IsNull();
    }

    [Test]
    public async Task RevokeAccess_PreventsFurtherUse()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        Check.That(tool.Lock(ownerKey)).IsTrue();

        Check.That(clientKey).IsNotNull();
        Check.That(await tool.Activate(key:executiveKey)).IsTrue();

        var command = new SecurityCommand();
        Check.That(await tool.RegisterCommand("test", command, key: executiveKey)).IsTrue();

        var result = tool.ExecuteCommand(new CommandDetails().SetHandle("test"), key: clientKey);
        Check.That(command.ExecutedSuccessfully).IsTrue();
        command.Reset();

        Check.That(tool.RevokeAccess(clientKey)).IsTrue();

        tool.ExecuteCommand(new CommandDetails().SetHandle("test"), key: clientKey);
        Check.That(command.ExecutedSuccessfully).IsFalse();

        Check.That(await tool.UnRegisterCommand("test", key: executiveKey)).IsTrue();
    }

    [Test]
    public void GenerateAccessKey_Populates_Authoritative_Membership_Metadata()
    {
        var tool = new Tool();
        var manager = new AccessManager(tool);

        ClientKey? clientKey = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-a"));
        Dictionary<AccessKeyToken,MembershipToken> membershipsByToken = GetMembershipsByToken(manager);

        Check.That(clientKey).IsNotNull();
        Check.That(membershipsByToken.Count).IsEqualTo(1);
        Check.That(membershipsByToken.Values.Single().Subject).IsEqualTo("subject-a");
    }

    [Test]
    public void RevokeAccess_Removes_Matching_MembershipCache_Entry()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        var manager = tool.GetAccessManager(executiveKey) as AccessManager;

        Check.That(manager).IsNotNull();
        Check.That(clientKey).IsNotNull();
        Check.That(MembershipCacheKey.TryCreate(clientKey!.Key!,out MembershipCacheKey cachekey)).IsTrue();

        FieldInfo cacheField = typeof(AccessManager).GetField("MembershipCache",BindingFlags.NonPublic | BindingFlags.Instance)!;
        MembershipCache cache = (MembershipCache)cacheField.GetValue(manager)!;
        Dictionary<AccessKeyToken,MembershipToken> membershipsByToken = GetMembershipsByToken(manager!);

        Check.That(cache).IsNotNull();
        Check.That(cache.TryGet(cachekey,out AccessKeyToken cachedToken)).IsTrue();
        Check.That(membershipsByToken.ContainsKey(cachedToken)).IsTrue();
        Check.That(membershipsByToken[cachedToken].Subject).IsNotEmpty();
        Check.That(tool.RevokeAccess(clientKey)).IsTrue();
        Check.That(cache.TryGet(cachekey,out _)).IsFalse();
        Check.That(membershipsByToken.ContainsKey(cachedToken)).IsFalse();
    }

    [Test]
    public void Clearing_MembershipCache_Does_Not_Affect_Authoritative_Queries()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        var manager = tool.GetAccessManager(executiveKey) as AccessManager;

        Check.That(manager).IsNotNull();
        Check.That(clientKey).IsNotNull();

        FieldInfo cacheField = typeof(AccessManager).GetField("MembershipCache",BindingFlags.NonPublic | BindingFlags.Instance)!;
        MembershipCache cache = (MembershipCache)cacheField.GetValue(manager)!;

        Check.That(cache).IsNotNull();
        cache.Clear();

        AccessManagerSnapshot? snapshot = manager!.QuerySnapshot(key: executiveKey);
        AccessManagerMembershipCatalog? catalog = manager.QueryMembershipCatalog(key: executiveKey);

        Check.That(snapshot).IsNotNull();
        Check.That(snapshot!.AccessKeys).IsNotNull();
        Check.That(snapshot.AccessKeys!.Count).IsStrictlyGreaterThan(0);
        Check.That(catalog).IsNotNull();
        Check.That(catalog!.Entries).IsNotNull();
        Check.That(catalog.Entries!.Count).IsStrictlyGreaterThan(0);
    }

    [Test]
    public void DestroySecrets()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        Check.That(tool.AddOutput(Guid.NewGuid(), "output1", executiveKey)).IsTrue();
        Check.That(tool.Lock(ownerKey)).IsTrue();

        Check.That(clientKey).IsNotNull();
        Check.That(tool.GetOutputIDs(clientKey)).IsNotNull();

        var manager = tool.GetAccessManager(executiveKey) as AccessManager;
        Check.That(manager).IsNotNull();

        Check.That(manager!.DestroySecrets(clientKey)).IsFalse();
        Check.That(manager.DestroySecrets(executiveKey)).IsTrue();

        Check.That(tool.GetOutputIDs(clientKey)).IsNull();
        Check.That(tool.GetAccessManager(executiveKey)).IsNull();

        var accessKeysField = typeof(AccessManager).GetField("AccessKeys", BindingFlags.NonPublic | BindingFlags.Instance);
        Check.That(accessKeysField).IsNotNull();
        var accessKeysValue = accessKeysField!.GetValue(manager) as System.Collections.IDictionary;
        Check.That(accessKeysValue).IsNotNull();
        Check.That(accessKeysValue!.Count).IsEqualTo(0);

        Dictionary<AccessKeyToken,MembershipToken> membershipsByToken = GetMembershipsByToken(manager);
        Check.That(membershipsByToken.Count).IsEqualTo(0);

        var realmKeyField = typeof(AccessManager).GetField("RealmKey", BindingFlags.NonPublic | BindingFlags.Instance);
        Check.That(realmKeyField).IsNotNull();
        Check.That(realmKeyField!.GetValue(manager)).IsNull();
    }

    [Test]
    public void Initialize_Initializes_ManifestIdentity()
    {
        var tool = new Tool();
        var manager = new AccessManager(tool);

        Object? identity = GetManifestIdentity(manager);
        ToolManifest manifest = tool.GetToolManifest()!.ComputeManifestHash();

        Check.That(identity).IsNotNull();
        Check.That(identity!.GetType().GetProperty("AccessKeyRealmID")!.GetValue(identity) as String).IsEqualTo(manifest.ToolSchemaID);
        Check.That(identity!.GetType().GetProperty("ToolSchemaID")!.GetValue(identity) as String).IsEqualTo(manifest.ToolSchemaID);
        Check.That(identity.GetType().GetProperty("ManifestHash")!.GetValue(identity) as String).IsEqualTo(manifest.ManifestHash);
    }

    [Test]
    public void Initialize_Fails_After_First_Successful_Initialization()
    {
        var manager = new AccessManager();
        var tool1 = new Tool();
        var tool2 = new Tool();

        Check.That(manager.Initialize(tool1)).IsTrue();
        Check.That(manager.Initialize(tool1)).IsFalse();
        Check.That(manager.Initialize(tool2)).IsFalse();
    }

    [Test]
    public void Issued_AccessKey_Embeds_Current_Manifest_Identity()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        var manager = tool.GetAccessManager(executiveKey) as AccessManager;

        Check.That(clientKey).IsNotNull();
        Check.That(manager).IsNotNull();

        FieldInfo realmKeyField = typeof(AccessManager).GetField("RealmKey",BindingFlags.NonPublic | BindingFlags.Instance)!;
        Byte[] realmKey = (Byte[])realmKeyField.GetValue(manager!)!;
        Object? identity = GetManifestIdentity(manager!);
        String toolschemaid = identity!.GetType().GetProperty("ToolSchemaID")!.GetValue(identity) as String ?? String.Empty;
        String accesskeyrealmid = identity.GetType().GetProperty("AccessKeyRealmID")!.GetValue(identity) as String ?? String.Empty;
        var claims = ReadClaims(clientKey!.Key!,realmKey,toolschemaid,accesskeyrealmid);

        Check.That(claims.ToolSchemaID).IsEqualTo(toolschemaid);
        Check.That(claims.AccessKeyRealmID).IsEqualTo(accesskeyrealmid);
        Check.That(claims.ManifestHash).IsEqualTo(identity.GetType().GetProperty("ManifestHash")!.GetValue(identity) as String);
    }

    [Test]
    public void AccessCheck_Denies_When_ManifestIdentity_Changes()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;
        var manager = tool.GetAccessManager(executiveKey) as AccessManager;

        Check.That(clientKey).IsNotNull();
        Check.That(manager).IsNotNull();
        Check.That(tool.AddOutput(Guid.NewGuid(),"output",executiveKey)).IsTrue();
        Check.That(tool.Lock(ownerKey)).IsTrue();
        Check.That(tool.GetOutputIDs(clientKey)).IsNotNull();

        SetManifestIdentity(manager!,"KusDepot.Tools/OtherSchema","Other-Realm","OTHER-HASH");

        Check.That(tool.GetOutputIDs(clientKey)).IsNull();
    }

    [Test]
    public void GenerateAccessKey_Denies_When_CustomPolicyEngine_Denies_Issuance()
    {
        var manager = new IssuancePolicyEngineAccessManager();
        var tool = new Tool(accessmanager: manager);

        ClientKey? clientKey = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-a",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(tool).IsNotNull();
        Check.That(clientKey).IsNull();
    }

    [Test]
    public void Authorize_Allows_Valid_Key_Without_Operation()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var manager = tool.GetAccessManager(executiveKey) as AccessManager;
        var nopkey = manager!.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "authorize"));

        Check.That(nopkey).IsNotNull();
        Check.That(manager!.Authorize(nopkey)).IsTrue();
    }

    [Test]
    public void AccessCheck_Denies_When_CustomPolicyEngine_Denies()
    {
        var engineManager = new PolicyEngineAccessManager();
        var tool = new Tool(accessmanager: engineManager);
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;

        Check.That(tool.AddOutput(Guid.NewGuid(),"output",executiveKey)).IsTrue();
        Check.That(tool.Lock(ownerKey)).IsTrue();
        Check.That(clientKey).IsNotNull();
        Check.That(tool.GetOutputIDs(clientKey)).IsNull();
    }

    [Test]
    public void Authorize_Denies_When_CustomPolicyEngine_Denies()
    {
        var engineManager = new PolicyEngineAccessManager();
        var tool = new Tool(accessmanager: engineManager);
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var manager = tool.GetAccessManager(executiveKey) as AccessManager;
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;

        Check.That(manager).IsNotNull();
        Check.That(clientKey).IsNotNull();
        Check.That(manager!.Authorize(clientKey)).IsFalse();
    }

    [Test]
    public void Authorize_Denies_When_PipelinePolicyEngine_Denies()
    {
        var engineManager = new PipelinePolicyEngineAccessManager();
        var tool = new Tool(accessmanager: engineManager);
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var manager = tool.GetAccessManager(executiveKey) as AccessManager;
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;

        Check.That(manager).IsNotNull();
        Check.That(clientKey).IsNotNull();
        Check.That(manager!.Authorize(clientKey)).IsFalse();
    }

    [Test]
    public void RevokeAccess_Denies_When_CustomPolicyEngine_Denies()
    {
        var engineManager = new RevocationPolicyEngineAccessManager();
        var tool = new Tool(accessmanager: engineManager);
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var clientKey = tool.RequestAccess(new StandardRequest("Client")) as ClientKey;

        Check.That(clientKey).IsNotNull();
        Check.That(tool.RevokeAccess(clientKey)).IsFalse();
    }

    [Test]
    public void UpdateMembershipCatalog_Add_Transfers_Queried_Memberships()
    {
        Byte[] sharedRealmKey = new Byte[AccessKeySecret.SymmetricKeySize];
        RandomNumberGenerator.Fill(sharedRealmKey);
        Guid sharedManagerId = Guid.NewGuid();
        Guid sharedToolId = Guid.NewGuid();

        var manager1 = new AccessManager(new AccessManagerOptions(id:sharedManagerId,realmkey:sharedRealmKey));
        var tool1 = new Tool(id:sharedToolId,accessmanager:manager1);
        var ownerKey1 = tool1.CreateOwnerKey("Owner1");
        Check.That(tool1.TakeOwnership(ownerKey1)).IsTrue();
        var executiveKey1 = tool1.RequestAccess(new ManagementRequest(ownerKey1!)) as ExecutiveKey;
        var clientKey1 = tool1.RequestAccess(new StandardRequest("Client1")) as ClientKey;
        Check.That(tool1.AddOutput(Guid.NewGuid(),"output1",executiveKey1)).IsTrue();
        Check.That(tool1.Lock(ownerKey1)).IsTrue();
        Check.That(clientKey1).IsNotNull();
        Check.That(tool1.GetOutputIDs(clientKey1)).IsNotNull();

        var manager2 = new AccessManager(new AccessManagerOptions(id:sharedManagerId,realmkey:sharedRealmKey));
        var tool2 = new Tool(id:sharedToolId,accessmanager:manager2);
        var ownerKey2 = tool2.CreateOwnerKey("Owner2");
        Check.That(tool2.TakeOwnership(ownerKey2)).IsTrue();
        var executiveKey2 = tool2.RequestAccess(new ManagementRequest(ownerKey2!)) as ExecutiveKey;
        Check.That(tool2.AddOutput(Guid.NewGuid(),"output2",executiveKey2)).IsTrue();
        Check.That(tool2.Lock(ownerKey2)).IsTrue();

        AccessManagerMembershipCatalog? memberships = manager1.QueryMembershipCatalog(key:executiveKey1);
        Int32 metadataCountBeforeImport = GetMembershipsByToken(manager2).Count;
        Int32 importedMembershipCount = memberships!.Entries!.Sum(_ => _.Value.Length);

        Check.That(memberships).IsNotNull();
        Check.That(tool2.GetOutputIDs(clientKey1)).IsNull();
        Check.That(GetMembershipsByToken(manager2).Count).IsEqualTo(metadataCountBeforeImport);
        Check.That(manager2.UpdateMembershipCatalog(memberships,MembershipUpdateOperation.Add,executiveKey2)).IsTrue();

        AccessManagerMembershipCatalog? importedMemberships = manager2.QueryMembershipCatalog(key:executiveKey2);

        Check.That(importedMemberships).IsNotNull();
        Check.That(importedMemberships!.Entries).IsNotNull();
        foreach(var subjectentries in memberships.Entries!)
        {
            Boolean containsSubject = importedMemberships.Entries!.ContainsKey(subjectentries.Key);
            Check.That(containsSubject).IsTrue();

            HashSet<String> importedTokens = importedMemberships.Entries[subjectentries.Key].Select(_ => _.Token.ToString()).ToHashSet(StringComparer.Ordinal);
            Boolean importedContainsSourceTokens = importedTokens.IsSupersetOf(subjectentries.Value.Select(_ => _.Token.ToString()));
            Check.That(importedContainsSourceTokens).IsTrue();
        }
        Check.That(GetMembershipsByToken(manager2).Count).IsEqualTo(metadataCountBeforeImport + importedMembershipCount);

        Boolean importedKeyHasAccess = tool2.GetOutputIDs(clientKey1) is not null;
        Check.That(importedKeyHasAccess).IsTrue();
    }

    [Test]
    public void UpdateMembershipCatalog_Remove_Revokes_Matching_Memberships()
    {
        var tool = new Tool();
        var ownerKey = tool.CreateOwnerKey("Owner");
        Check.That(tool.TakeOwnership(ownerKey)).IsTrue();
        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
        var manager = tool.GetAccessManager(executiveKey) as AccessManager;
        Check.That(manager).IsNotNull();
        Check.That(tool.AddOutput(Guid.NewGuid(),"output",executiveKey)).IsTrue();
        Check.That(tool.Lock(ownerKey)).IsTrue();

        ClientKey? clientKey = manager!.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-remove",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)),executiveKey);
        AccessManagerMembershipCatalog? memberships = manager.QueryMembershipCatalog(AccessManagerQuery.Create(subjects:["subject-remove"]),executiveKey);

        Check.That(clientKey).IsNotNull();
        Check.That(memberships).IsNotNull();
        Check.That(tool.GetOutputIDs(clientKey)).IsNotNull();
        Check.That(manager.UpdateMembershipCatalog(memberships,MembershipUpdateOperation.Remove,executiveKey)).IsTrue();
        Check.That(tool.GetOutputIDs(clientKey)).IsNull();
    }
}