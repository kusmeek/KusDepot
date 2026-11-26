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

    private sealed class CaptureSelfAccessPolicyEngine : IAccessPolicyEngine , IAsyncAccessPolicyEngine
    {
        public AccessPolicyContext? LastContext { get; private set; }

        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            LastContext = context;
            return context.Mode == AccessPolicyMode.SelfAccess && context.Operation == ProtectedOperation.GetOutputIDs
                ? AccessPolicyDecision.Allow()
                : AccessPolicyDecision.Deny();
        }

        public ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
        {
            LastContext = context;
            return ValueTask.FromResult(context.Mode == AccessPolicyMode.SelfAccess && context.Operation == ProtectedOperation.GetOutputIDs
                ? AccessPolicyDecision.Allow()
                : AccessPolicyDecision.Deny());
        }
    }

    private sealed class CaptureAccessPolicyContextEngine : IAccessPolicyEngine
    {
        public AccessPolicyContext? LastContext { get; private set; }

        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            LastContext = context;
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

    private static MembershipCache GetMembershipCache(AccessManager manager)
    {
        FieldInfo field = typeof(AccessManager).GetField("MembershipCache",BindingFlags.NonPublic | BindingFlags.Instance)!;
        return (MembershipCache)field.GetValue(manager)!;
    }

    private static ClientKey TamperCredential(ClientKey key)
    {
        Byte[] credential = key.Key!;
        credential[credential.Length / 2] ^= 0x01;
        return new(credential);
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
        var manager = new AccessManager();
        _ = new Tool(accessmanager: manager);

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
        AccessManager manager = new(AccessManagerOptions.Create(signatureprocessor: signatureprocessor));
        _ = new Tool(accessmanager: manager);

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
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: policyengine,signatureprocessor: signatureprocessor));
        _ = new Tool(accessmanager: manager);

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
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: policyengine));
        _ = new Tool(accessmanager: manager);

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
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: policyengine,signatureprocessor: signatureprocessor));
        _ = new Tool(accessmanager: manager);

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
        AccessManager manager = new(AccessManagerOptions.Create(signatureprocessor: signatureprocessor));
        _ = new Tool(accessmanager: manager);

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
        AccessManager manager = new(AccessManagerOptions.Create(signatureprocessor: signatureprocessor));
        _ = new Tool(accessmanager: manager);

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "signed-default-accesscheck",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.AccessCheck(key,ProtectedOperation.GetOutputIDs);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsTrue();
        Check.That(signatureprocessor.VerifyCalls).IsEqualTo(1);
    }

    [Test]
    public void Authorize_Denies_Unsigned_Key_When_Policy_Requires_Signed_Key()
    {
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: new RequireSignedPolicyEngine()));
        _ = new Tool(accessmanager: manager);

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "unsigned-required-signed",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsFalse();
    }

    [Test]
    public void Authorize_Denies_Signed_Unverified_Key_When_Policy_Requires_Verified_Signature()
    {
        SignOnlySignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: new RequireVerifiedSignaturePolicyEngine(),signatureprocessor: signatureprocessor));
        _ = new Tool(accessmanager: manager);

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "signed-required-verified",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsFalse();
    }

    [Test]
    public void Authorize_Allows_Verified_Key_When_Policy_Requires_Verified_Signature()
    {
        CapturingSignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: new RequireVerifiedSignaturePolicyEngine(),signatureprocessor: signatureprocessor));
        _ = new Tool(accessmanager: manager);

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
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: new RequireTrustedSignaturePolicyEngine(),signatureprocessor: signatureprocessor));
        _ = new Tool(accessmanager: manager);

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "trusted-required-trusted",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.Authorize(key);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsTrue();
        Check.That(signatureprocessor.VerifyCalls).IsEqualTo(1);
    }

    [Test]
    public void AccessCheck_Denies_Unsigned_Key_When_Policy_Requires_Signed_Key()
    {
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: new RequireSignedPolicyEngine()));
        _ = new Tool(accessmanager: manager);

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "unsigned-access-signed",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean allowed = manager.AccessCheck(key,ProtectedOperation.GetOutputIDs);

        Check.That(key).IsNotNull();
        Check.That(allowed).IsFalse();
    }

    [Test]
    public void GenerateAccessKey_Generic_NullPermissions_Uses_Empty_Set()
    {
        var manager = new AccessManager();
        _ = new Tool(accessmanager: manager);

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

        var executiveKey = tool.RequestAccess(new ManagementRequest(ownerKey!)) as ExecutiveKey;
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
        public DenyingAccessManager() : base() { }

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

    private sealed class CaptureManagementRequestIssuerOnlyPolicyEngine : IAccessPolicyEngine
    {
        public Boolean? LastRequestIssuerAccessOnly { get; private set; }

        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            if(context.Mode == AccessPolicyMode.Request)
            {
                LastRequestIssuerAccessOnly = context.RequestContext?.IssuerAccessOnly;
            }

            return AccessPolicyDecision.Allow();
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
        public PolicyEngineAccessManager() : base(AccessManagerOptions.Create(policyengine: new DenyAuthorizationPolicyEngine())) { }
    }

    public sealed class RequestPolicyEngineAccessManager : AccessManager
    {
        public RequestPolicyEngineAccessManager() : base(AccessManagerOptions.Create(policyengine: new DenyRequestPolicyEngine())) { }
    }

    public sealed class RequestedKeyTypeAccessManager : AccessManager
    {
        public RequestedKeyTypeAccessManager() : base() { }

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
        public CustomRequestAccessManager(AccessManagerOptions? options = null) : base(options) { }

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
        public OverriddenPolicyEngineAccessManager() : base(AccessManagerOptions.Create(policyengine: new DenyIssuancePolicyEngine())) { }
    }

    public sealed class IssuancePolicyEngineAccessManager : AccessManager
    {
        public IssuancePolicyEngineAccessManager() : base(AccessManagerOptions.Create(policyengine: new DenyIssuancePolicyEngine())) { }
    }

    public sealed class RevocationPolicyEngineAccessManager : AccessManager
    {
        public RevocationPolicyEngineAccessManager() : base(AccessManagerOptions.Create(policyengine: new DenyRevocationPolicyEngine())) { }
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

        public PipelinePolicyEngineAccessManager() : base(AccessManagerOptions.Create(policyengine: new PipelineAccessPolicyEngine([new AllowStage(),new DenyAuthorizeStage()]))) { }
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
        var manager = new AccessManager(options);
        _ = new Tool(accessmanager: manager);

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
        var manager = new AccessManager();
        _ = new Tool(accessmanager: manager);

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
    public void Authorize_Rejects_TamperedCredential_WhenIssuanceWarmedCache()
    {
        AccessManager manager = new();
        _ = new Tool(accessmanager: manager);
        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject:"cache-tamper-sync",operations:ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(key).IsNotNull();
        ClientKey tampered = TamperCredential(key!);

        Check.That(manager.Authorize(tampered)).IsFalse();
        Check.That(manager.AccessCheck(tampered,ProtectedOperation.GetOutputIDs)).IsFalse();
        Check.That(manager.Authorize(key)).IsTrue();
    }

    [Test]
    public void Authorize_Rejects_TamperedCredential_WhenCacheIsCold()
    {
        AccessManager manager = new();
        _ = new Tool(accessmanager: manager);
        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject:"cache-tamper-cold",operations:ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(key).IsNotNull();
        GetMembershipCache(manager).Clear();

        ClientKey tampered = TamperCredential(key!);

        Check.That(manager.Authorize(tampered)).IsFalse();
        Check.That(manager.Authorize(key)).IsTrue();
        Check.That(manager.Authorize(tampered)).IsFalse();
    }

    [Test]
    public async Task AuthorizeAsync_Rejects_TamperedCredential_WhenAuthorizationWarmedCache()
    {
        AccessManager manager = new();
        _ = new Tool(accessmanager: manager);
        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject:"cache-tamper-async",operations:ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(key).IsNotNull();
        GetMembershipCache(manager).Clear();
        Check.That(await manager.AuthorizeAsync(key).ConfigureAwait(false)).IsTrue();

        ClientKey tampered = TamperCredential(key!);

        Check.That(await manager.AuthorizeAsync(tampered).ConfigureAwait(false)).IsFalse();
        Check.That(await manager.AccessCheckAsync(tampered,ProtectedOperation.GetOutputIDs).ConfigureAwait(false)).IsFalse();
        Check.That(await manager.AuthorizeAsync(key).ConfigureAwait(false)).IsTrue();
    }

    [Test]
    public async Task RevokeAccess_Rejects_TamperedCredential_AndPreservesOriginalMembership()
    {
        AccessManager manager = new();
        _ = new Tool(accessmanager: manager);
        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject:"cache-tamper-revoke",operations:ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(key).IsNotNull();
        ClientKey tampered = TamperCredential(key!);

        Check.That(manager.RevokeAccess(tampered)).IsFalse();
        Check.That(await manager.RevokeAccessAsync(tampered).ConfigureAwait(false)).IsFalse();
        Check.That(manager.Authorize(key)).IsTrue();
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

        var manager = typeof(Tool).GetField("AccessManager", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tool) as AccessManager;
        Check.That(manager).IsNotNull();
        Check.That(ReferenceEquals(tool.GetAccessManager(executiveKey),manager)).IsTrue();

        var destroySelfKey = typeof(Tool).GetField("SelfKey", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tool) as SelfKey;
        Check.That(destroySelfKey).IsNotNull();
        SelfKey invalidSelfKey = new();
        Check.That(manager!.DestroySecrets(invalidSelfKey)).IsFalse();
        Check.That(manager.DestroySecrets(destroySelfKey!)).IsTrue();

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
        var manager = new AccessManager();
        var tool = new Tool(accessmanager: manager);

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

        Check.That(manager.Initialize(tool1)).IsNotNull();
        Check.That(manager.Initialize(tool1)).IsNull();
        Check.That(manager.Initialize(tool2)).IsNull();
    }

    [Test]
    public void Initialize_Binds_SelfKey_Only_To_The_Initialized_Tool()
    {
        Tool tool = new();
        Tool otherTool = new();
        AccessManager manager = new();

        SelfKey? selfkey = manager.Initialize(tool);

        Check.That(selfkey).IsNotNull();
        Check.That(manager.Initialize(tool)).IsNull();
        Check.That(manager.Initialize(otherTool)).IsNull();
    }

    [Test]
    public async Task AccessCheck_SelfKey_Requires_Bound_Reference_And_Passes_Operation_To_SelfAccess_Policy()
    {
        CaptureSelfAccessPolicyEngine policyengine = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine:policyengine));
        Tool tool = new(accessmanager:manager);
        SelfKey selfkey = (SelfKey)typeof(AccessManager).GetField("ToolSelfKey",BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(manager)!;
        SelfKey otherSelfKey = new();

        Check.That(manager.AccessCheck(selfkey,ProtectedOperation.GetOutputIDs)).IsTrue();
        Check.That(policyengine.LastContext!.Value.Mode).IsEqualTo(AccessPolicyMode.SelfAccess);
        Check.That(policyengine.LastContext.Value.Operation).IsEqualTo(ProtectedOperation.GetOutputIDs);
        Check.That(policyengine.LastContext.Value.OperationAllowed).IsFalse();
        Check.That(manager.AccessCheck(selfkey,ProtectedOperation.GetInputs)).IsFalse();
        Check.That(manager.AccessCheck(otherSelfKey,ProtectedOperation.GetOutputIDs)).IsFalse();
        Check.That(await manager.AccessCheckAsync(selfkey,ProtectedOperation.GetOutputIDs).ConfigureAwait(false)).IsTrue();
        Check.That(policyengine.LastContext!.Value.Mode).IsEqualTo(AccessPolicyMode.SelfAccess);
        Check.That(policyengine.LastContext.Value.Operation).IsEqualTo(ProtectedOperation.GetOutputIDs);
        Check.That(await manager.AccessCheckAsync(selfkey,ProtectedOperation.GetInputs).ConfigureAwait(false)).IsFalse();
        Check.That(await manager.AccessCheckAsync(otherSelfKey,ProtectedOperation.GetOutputIDs).ConfigureAwait(false)).IsFalse();
        Check.That(tool).IsNotNull();
    }

    [Test]
    public async Task GenerateIssuerOnlyAccessKey_Requires_The_Exact_Bound_SelfKey()
    {
        AccessManager manager = new();
        Tool tool = new(accessmanager:manager);
        SelfKey selfkey = (SelfKey)typeof(AccessManager).GetField("ToolSelfKey",BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(manager)!;
        SelfKey otherSelfKey = new();
        AccessKeyIssueOptions options = AccessKeyIssueOptions.Create(subject:"issuer-only-self-key",operations:ImmutableArray.Create(ProtectedOperation.GetOutputIDs));

        Check.That(manager.GenerateIssuerOnlyAccessKey<ClientKey>(otherSelfKey,options)).IsNull();
        Check.That(await manager.GenerateIssuerOnlyAccessKeyAsync<ClientKey>(otherSelfKey,options,CancellationToken.None)).IsNull();

        Check.That(manager.GenerateIssuerOnlyAccessKey<ClientKey>(selfkey,options)).IsNotNull();
        Check.That(await manager.GenerateIssuerOnlyAccessKeyAsync<ClientKey>(selfkey,options,CancellationToken.None)).IsNotNull();
        Check.That(manager.GenerateAccessKey<SelfKey>(options)).IsNull();
        Check.That(await manager.GenerateAccessKeyAsync<SelfKey>(options,cancel:CancellationToken.None)).IsNull();
        Check.That(manager.GenerateIssuerOnlyAccessKey<SelfKey>(selfkey,options)).IsNull();
        Check.That(await manager.GenerateIssuerOnlyAccessKeyAsync<SelfKey>(selfkey,options,CancellationToken.None)).IsNull();
        Check.That(tool).IsNotNull();
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

    [TestCase(false,false,true)]
    [TestCase(true,false,false)]
    [TestCase(false,true,false)]
    public async Task IssuerAccessOnly_Requires_Both_Issuer_IDs_To_Match(Boolean accessmanagerdiffers , Boolean tooldiffers , Boolean expectedallowed)
    {
        Byte[] sharedRealmKey = new Byte[AccessKeySecret.SymmetricKeySize];
        RandomNumberGenerator.Fill(sharedRealmKey);
        Guid sourceManagerId = Guid.NewGuid();
        Guid sourceToolId = Guid.NewGuid();
        AccessManager sourceManager = new(new AccessManagerOptions(id:sourceManagerId,realmkey:sharedRealmKey));
        Tool sourceTool = new(id:sourceToolId,accessmanager:sourceManager);
        AccessManager targetManager = new(new AccessManagerOptions(id:accessmanagerdiffers ? Guid.NewGuid() : sourceManagerId,realmkey:sharedRealmKey));
        Tool targetTool = new(id:tooldiffers ? Guid.NewGuid() : sourceToolId,accessmanager:targetManager);
        FieldInfo selfKeyField = typeof(AccessManager).GetField("ToolSelfKey",BindingFlags.NonPublic | BindingFlags.Instance)!;
        SelfKey selfkey = (SelfKey)selfKeyField.GetValue(sourceManager)!;
        ClientKey? sourceKey = sourceManager.GenerateIssuerOnlyAccessKey<ClientKey>(selfkey,AccessKeyIssueOptions.Create(
            subject:"issuer-only",operations:ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(sourceKey).IsNotNull();
        Check.That(sourceManager.Authorize(sourceKey)).IsTrue();
        Check.That(sourceManager.AccessCheck(sourceKey,ProtectedOperation.GetOutputIDs)).IsTrue();

        Object identity = GetManifestIdentity(sourceManager)!;
        String toolschemaid = identity.GetType().GetProperty("ToolSchemaID")!.GetValue(identity) as String ?? String.Empty;
        String accesskeyrealmid = identity.GetType().GetProperty("AccessKeyRealmID")!.GetValue(identity) as String ?? String.Empty;
        Check.That(AccessKeySecret.TryReadClaims(sourceKey!.Key!,sharedRealmKey,toolschemaid,accesskeyrealmid,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.IssuerAccessOnly).IsTrue();
        Check.That(claims.IssuerAccessManagerID).IsEqualTo(sourceManagerId);
        Check.That(claims.IssuerToolID).IsEqualTo(sourceToolId);

        AccessManagerMembershipCatalog? catalog = sourceManager.QueryMembershipCatalog();
        Check.That(catalog).IsNotNull();
        Check.That(catalog!.Entries!["issuer-only"][0].IssuerAccessOnly).IsTrue();

        Check.That(targetManager.UpdateMembershipCatalog(catalog,MembershipUpdateOperation.Add)).IsTrue();
        Check.That(targetManager.Authorize(sourceKey)).IsEqualTo(expectedallowed);
        Check.That(targetManager.AccessCheck(sourceKey,ProtectedOperation.GetOutputIDs)).IsEqualTo(expectedallowed);
        Check.That(await targetManager.AuthorizeAsync(sourceKey).ConfigureAwait(false)).IsEqualTo(expectedallowed);
        Check.That(await targetManager.AccessCheckAsync(sourceKey,ProtectedOperation.GetOutputIDs).ConfigureAwait(false)).IsEqualTo(expectedallowed);

        Check.That(sourceTool).IsNotNull();
        Check.That(targetTool).IsNotNull();
    }

    [TestCase(false,false,false,true)]
    [TestCase(false,true,false,true)]
    [TestCase(false,false,true,true)]
    [TestCase(true,false,false,true)]
    [TestCase(true,true,false,false)]
    [TestCase(true,false,true,false)]
    public async Task Policy_Observes_Issuer_Matches_And_Cannot_Override_IssuerOnly_Denial(Boolean issueraccessonly , Boolean accessmanagerdiffers , Boolean tooldiffers , Boolean expectedallowed)
    {
        Byte[] sharedRealmKey = new Byte[AccessKeySecret.SymmetricKeySize];
        RandomNumberGenerator.Fill(sharedRealmKey);
        Guid sourceManagerId = Guid.NewGuid();
        Guid sourceToolId = Guid.NewGuid();
        AccessManager sourceManager = new(new AccessManagerOptions(id:sourceManagerId,realmkey:sharedRealmKey));
        Tool sourceTool = new(id:sourceToolId,accessmanager:sourceManager);
        FieldInfo selfKeyField = typeof(AccessManager).GetField("ToolSelfKey",BindingFlags.NonPublic | BindingFlags.Instance)!;
        SelfKey selfkey = (SelfKey)selfKeyField.GetValue(sourceManager)!;
        ClientKey? sourceKey = issueraccessonly
            ? sourceManager.GenerateIssuerOnlyAccessKey<ClientKey>(selfkey,AccessKeyIssueOptions.Create(subject:"policy-issuer-match",operations:ImmutableArray.Create(ProtectedOperation.GetOutputIDs)))
            : sourceManager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject:"policy-issuer-match",operations:ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        CaptureAccessPolicyContextEngine policyengine = new();
        AccessManager targetManager = new(AccessManagerOptions.Create(id:accessmanagerdiffers ? Guid.NewGuid() : sourceManagerId,realmkey:sharedRealmKey,policyengine:policyengine));
        Tool targetTool = new(id:tooldiffers ? Guid.NewGuid() : sourceToolId,accessmanager:targetManager);
        AccessManagerMembershipCatalog? catalog = sourceManager.QueryMembershipCatalog();

        Check.That(sourceTool).IsNotNull();
        Check.That(sourceKey).IsNotNull();
        Check.That(targetTool).IsNotNull();
        Check.That(catalog).IsNotNull();
        Check.That(targetManager.UpdateMembershipCatalog(catalog,MembershipUpdateOperation.Add)).IsTrue();

        Check.That(targetManager.Authorize(sourceKey)).IsEqualTo(expectedallowed);
        Check.That(policyengine.LastContext).IsNotNull();
        Check.That(policyengine.LastContext!.Value.Claims!.IssuerAccessOnly).IsEqualTo(issueraccessonly);
        Check.That(policyengine.LastContext.Value.IssuerAccessManagerMatched).IsEqualTo(!accessmanagerdiffers);
        Check.That(policyengine.LastContext.Value.IssuerToolMatched).IsEqualTo(!tooldiffers);

        Check.That(targetManager.AccessCheck(sourceKey,ProtectedOperation.GetOutputIDs)).IsEqualTo(expectedallowed);
        Check.That(policyengine.LastContext!.Value.IssuerAccessManagerMatched).IsEqualTo(!accessmanagerdiffers);
        Check.That(policyengine.LastContext.Value.IssuerToolMatched).IsEqualTo(!tooldiffers);

        Check.That(await targetManager.AuthorizeAsync(sourceKey).ConfigureAwait(false)).IsEqualTo(expectedallowed);
        Check.That(policyengine.LastContext!.Value.IssuerAccessManagerMatched).IsEqualTo(!accessmanagerdiffers);
        Check.That(policyengine.LastContext.Value.IssuerToolMatched).IsEqualTo(!tooldiffers);

        Check.That(await targetManager.AccessCheckAsync(sourceKey,ProtectedOperation.GetOutputIDs).ConfigureAwait(false)).IsEqualTo(expectedallowed);
        Check.That(policyengine.LastContext!.Value.IssuerAccessManagerMatched).IsEqualTo(!accessmanagerdiffers);
        Check.That(policyengine.LastContext.Value.IssuerToolMatched).IsEqualTo(!tooldiffers);

        if(issueraccessonly && !expectedallowed)
        {
            Check.That(targetManager.RevokeAccess(sourceKey)).IsFalse();
            Check.That(policyengine.LastContext!.Value.Mode).IsEqualTo(AccessPolicyMode.Revoke);
            Check.That(policyengine.LastContext.Value.IssuerAccessManagerMatched).IsEqualTo(!accessmanagerdiffers);
            Check.That(policyengine.LastContext.Value.IssuerToolMatched).IsEqualTo(!tooldiffers);

            Check.That(await targetManager.RevokeAccessAsync(sourceKey).ConfigureAwait(false)).IsFalse();
            Check.That(policyengine.LastContext!.Value.Mode).IsEqualTo(AccessPolicyMode.Revoke);
            Check.That(policyengine.LastContext.Value.IssuerAccessManagerMatched).IsEqualTo(!accessmanagerdiffers);
            Check.That(policyengine.LastContext.Value.IssuerToolMatched).IsEqualTo(!tooldiffers);
        }
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
    public async Task ManagementRequest_Forces_IssuerOnly_For_All_Key_Types()
    {
        var policyengine = new CaptureManagementRequestIssuerOnlyPolicyEngine();
        var manager = new AccessManager(AccessManagerOptions.Create(policyengine: policyengine));
        Tool tool = new(accessmanager: manager);
        OwnerKey? ownerKey = tool.CreateOwnerKey("management-request-issuer-only");
        Check.That(ownerKey).IsNotNull();

        ManagementRequest request = new(ownerKey!) { IssuerAccessOnly = false, RequestedKeyType = nameof(ClientKey) };
        ClientKey? key = tool.RequestAccess(request) as ClientKey;
        Check.That(key).IsNotNull();
        Check.That(policyengine.LastRequestIssuerAccessOnly == true).IsTrue();

        ManagementRequest asyncRequest = new(ownerKey!) { IssuerAccessOnly = false, RequestedKeyType = nameof(ClientKey) };
        ClientKey? asyncKey = await tool.RequestAccessAsync(asyncRequest).ConfigureAwait(false) as ClientKey;
        Check.That(asyncKey).IsNotNull();
        Check.That(policyengine.LastRequestIssuerAccessOnly == true).IsTrue();

        Object identity = GetManifestIdentity(manager)!;
        String toolschemaid = identity.GetType().GetProperty("ToolSchemaID")!.GetValue(identity) as String ?? String.Empty;
        String accesskeyrealmid = identity.GetType().GetProperty("AccessKeyRealmID")!.GetValue(identity) as String ?? String.Empty;
        FieldInfo realmKeyField = typeof(AccessManager).GetField("RealmKey",BindingFlags.NonPublic | BindingFlags.Instance)!;
        Byte[] realmkey = (Byte[])realmKeyField.GetValue(manager)!;

        Check.That(AccessKeySecret.TryReadClaims(key!.Key!,realmkey,toolschemaid,accesskeyrealmid,out AccessKeyClaims claims)).IsTrue();
        Check.That(claims.IssuerAccessOnly).IsTrue();
        Check.That(AccessKeySecret.TryReadClaims(asyncKey!.Key!,realmkey,toolschemaid,accesskeyrealmid,out AccessKeyClaims asyncclaims)).IsTrue();
        Check.That(asyncclaims.IssuerAccessOnly).IsTrue();
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
    public void UpdateMembershipCatalog_Add_Rejects_Identityless_Catalog_Without_Mutation()
    {
        var manager = new AccessManager();
        _ = new Tool(accessmanager: manager);
        ClientKey? sourceKey = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "identity-source"));
        AccessManagerMembershipCatalog? sourceCatalog = manager.QueryMembershipCatalog();
        Int32 membershipCount = GetMembershipsByToken(manager).Count;

        AccessManagerMembershipCatalog identitylessCatalog = new() { Entries = sourceCatalog!.Entries };

        Check.That(sourceKey).IsNotNull();
        Check.That(sourceCatalog).IsNotNull();
        Check.That(manager.UpdateMembershipCatalog(identitylessCatalog,MembershipUpdateOperation.Add)).IsFalse();
        Check.That(GetMembershipsByToken(manager).Count).IsEqualTo(membershipCount);
    }

    [Test]
    public void UpdateMembershipCatalog_Add_Rejects_Invalid_Later_Entry_Without_Partial_Import()
    {
        Byte[] sharedRealmKey = new Byte[AccessKeySecret.SymmetricKeySize];
        RandomNumberGenerator.Fill(sharedRealmKey);
        Guid sharedManagerId = Guid.NewGuid();
        Guid sharedToolId = Guid.NewGuid();

        var sourceManager = new AccessManager(new AccessManagerOptions(id:sharedManagerId,realmkey:sharedRealmKey));
        var sourceTool = new Tool(id:sharedToolId,accessmanager:sourceManager);
        ClientKey? sourceKey = sourceManager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "catalog-source"));
        AccessManagerMembershipCatalog? sourceCatalog = sourceManager.QueryMembershipCatalog();

        var targetManager = new AccessManager(new AccessManagerOptions(id:sharedManagerId,realmkey:sharedRealmKey));
        var targetTool = new Tool(id:sharedToolId,accessmanager:targetManager);
        _ = targetManager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "existing"));
        Int32 membershipCount = GetMembershipsByToken(targetManager).Count;

        AccessManagerMembershipEntry validEntry = sourceCatalog!.Entries!["catalog-source"][0].Clone();
        AccessManagerMembershipEntry invalidEntry = validEntry.Clone();
        invalidEntry.ManifestHash = "invalid-manifest-hash";

        AccessManagerMembershipCatalog invalidCatalog = new()
        {
            ToolManifestIdentity = sourceCatalog.ToolManifestIdentity,
            Entries = new Dictionary<String,ImmutableArray<AccessManagerMembershipEntry>>
            {
                ["catalog-source"] = [validEntry,invalidEntry]
            }
        };

        Check.That(sourceKey).IsNotNull();
        Check.That(sourceTool).IsNotNull();
        Check.That(targetTool).IsNotNull();
        Check.That(targetManager.UpdateMembershipCatalog(invalidCatalog,MembershipUpdateOperation.Add)).IsFalse();
        Check.That(GetMembershipsByToken(targetManager).Count).IsEqualTo(membershipCount);
        Check.That(targetManager.QueryMembershipCatalog()!.Entries!.ContainsKey("catalog-source")).IsFalse();
    }

    [Test]
    public void UpdateMembershipCatalog_Null_And_Empty_Catalogs_Are_NoOps()
    {
        var manager = new AccessManager();
        _ = new Tool(accessmanager: manager);
        _ = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "no-op"));
        Int32 membershipCount = GetMembershipsByToken(manager).Count;

        Check.That(manager.UpdateMembershipCatalog(null,MembershipUpdateOperation.Add)).IsTrue();
        Check.That(manager.UpdateMembershipCatalog(new AccessManagerMembershipCatalog(),MembershipUpdateOperation.Add)).IsTrue();
        Check.That(GetMembershipsByToken(manager).Count).IsEqualTo(membershipCount);
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