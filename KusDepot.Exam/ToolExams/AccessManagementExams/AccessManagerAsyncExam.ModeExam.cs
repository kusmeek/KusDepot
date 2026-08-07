namespace KusDepot.Exams.Security;

public partial class AccessManagerAsyncExam
{
    private sealed class AsyncOnlySignatureProcessor : IAccessKeySigner , IAsyncAccessKeySignatureProcessor
    {
        private readonly ImmutableArray<Byte> signerKeyId = SHA256.HashData(Encoding.UTF8.GetBytes("async-only-signature-processor")).ToImmutableArray();

        public Int32 AsyncVerifyCalls { get; private set; }
        public ILogger? LastSigningLogger { get; private set; }
        public ILogger? LastValidationLogger { get; private set; }
        public Int32 SignCalls { get; private set; }

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

        public async ValueTask<SignatureValidation> VerifyAsync(AccessKeySignatureValidationContext context , CancellationToken cancel = default)
        {
            await Task.Yield();
            cancel.ThrowIfCancellationRequested();
            AsyncVerifyCalls++;
            LastValidationLogger = context.Logger;
            return new()
            {
                Algorithm = context.Signature.Algorithm,
                Signed = context.Signature.Signed,
                SignatureValid = true,
                SignerAccessManagerID = context.Signature.SignerAccessManagerID,
                SignerKeyID = context.Signature.SignerKeyID,
                SignerKeyIDFormat = context.Signature.SignerKeyIDFormat,
                SignerToolID = context.Signature.SignerToolID,
                VerificationAttempted = true
            };
        }
    }

    private sealed class PreferAsyncSignatureProcessor : IAccessKeySignatureProcessor , IAsyncAccessKeySignatureProcessor
    {
        private readonly ImmutableArray<Byte> signerKeyId = SHA256.HashData(Encoding.UTF8.GetBytes("prefer-async-signature-processor")).ToImmutableArray();

        public Int32 AsyncVerifyCalls { get; private set; }
        public ILogger? LastSigningLogger { get; private set; }
        public ILogger? LastValidationLogger { get; private set; }
        public Int32 SignCalls { get; private set; }
        public Int32 SyncVerifyCalls { get; private set; }

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
            SyncVerifyCalls++;
            LastValidationLogger = context.Logger;
            return new()
            {
                Algorithm = context.Signature.Algorithm,
                Signed = context.Signature.Signed,
                SignatureValid = true,
                SignerAccessManagerID = context.Signature.SignerAccessManagerID,
                SignerKeyID = context.Signature.SignerKeyID,
                SignerKeyIDFormat = context.Signature.SignerKeyIDFormat,
                SignerToolID = context.Signature.SignerToolID,
                VerificationAttempted = true
            };
        }

        public async ValueTask<SignatureValidation> VerifyAsync(AccessKeySignatureValidationContext context , CancellationToken cancel = default)
        {
            await Task.Yield();
            cancel.ThrowIfCancellationRequested();
            AsyncVerifyCalls++;
            LastValidationLogger = context.Logger;
            return new()
            {
                Algorithm = context.Signature.Algorithm,
                Signed = context.Signature.Signed,
                SignatureValid = true,
                SignerAccessManagerID = context.Signature.SignerAccessManagerID,
                SignerKeyID = context.Signature.SignerKeyID,
                SignerKeyIDFormat = context.Signature.SignerKeyIDFormat,
                SignerToolID = context.Signature.SignerToolID,
                VerificationAttempted = true
            };
        }
    }

    private sealed class SyncOnlyAllowPolicyEngine : IAccessPolicyEngine
    {
        public Int32 SyncCalls { get; private set; }

        public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
        {
            SyncCalls++;
            return AccessPolicyDecision.Allow();
        }
    }

    private sealed class SyncCapturingAssertionGenerator(params AccessKeyAssertion[] assertions) : IAccessKeyAssertionGenerator
    {
        public Int32 SyncCalls { get; private set; }

        public ImmutableArray<AccessKeyAssertion> Generate(in AccessKeyAssertionGeneratorContext context)
        {
            SyncCalls++;
            return assertions.ToImmutableArray();
        }
    }

    private sealed class AsyncOnlyAssertionGenerator(params AccessKeyAssertion[] assertions) : IAsyncAccessKeyAssertionGenerator
    {
        public async ValueTask<ImmutableArray<AccessKeyAssertion>> GenerateAsync(AccessKeyAssertionGeneratorContext context , CancellationToken cancel = default)
        {
            await Task.Yield();
            cancel.ThrowIfCancellationRequested();
            return assertions.ToImmutableArray();
        }
    }

    [Test]
    public void RequestAccess_Throws_When_Sync_Path_Uses_AsyncOnlyPolicyEngine()
    {
        AsyncOnlyDenyRequestPolicyEngine engine = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: engine));
        Tool tool = new(accessmanager: manager);
        Check.That(tool.EnableMyExceptions()).IsTrue();

        Assert.Throws<InvalidOperationException>(() => manager.RequestAccess(new StandardRequest("subject-sync")));
    }

    [Test]
    public void GenerateAccessKey_Throws_When_Sync_Path_Uses_AsyncOnlyAssertionGenerator()
    {
        TokenAssertion assertion = new()
        {
            Format = "jwt/v1",
            Issuer = "https://sync-reject.example",
            Subject = "subject-sync-reject",
            Token = Encoding.UTF8.GetBytes("sync-reject-jwt"),
            TokenType = TokenKeyType.Jwt
        };
        AsyncOnlyAssertionGenerator generator = new(assertion);
        AccessManager manager = new(new AccessManagerOptions(keyassertiongenerator: generator));
        Tool tool = new(accessmanager: manager);
        Check.That(tool.EnableMyExceptions()).IsTrue();

        Assert.Throws<InvalidOperationException>(() => manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-sync-reject")));
    }

    [Test]
    public async Task RequestAccessAsync_Uses_SyncOnlyPolicyEngine_When_No_AsyncEngine_Is_Configured()
    {
        SyncOnlyAllowPolicyEngine engine = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: engine));
        Tool tool = new(accessmanager: manager);

        AccessKey? key = await manager.RequestAccessAsync(new StandardRequest("subject-sync-policy"));

        Check.That(tool).IsNotNull();
        Check.That(key).IsNotNull();
        Assert.That(engine.SyncCalls,Is.GreaterThan(0));
    }

    [Test]
    public async Task GenerateAccessKeyAsync_Uses_SyncOnlyAssertionGenerator_When_No_AsyncGenerator_Is_Configured()
    {
        TokenAssertion assertion = new()
        {
            Format = "jwt/v1",
            Issuer = "https://sync-generator.example",
            Subject = "subject-sync-generator",
            Token = Encoding.UTF8.GetBytes("sync-generator-jwt"),
            TokenType = TokenKeyType.Jwt
        };
        SyncCapturingAssertionGenerator generator = new(assertion);
        AccessManager manager = new(new AccessManagerOptions(keyassertiongenerator: generator),new Tool());

        ClientKey? key = await manager.GenerateAccessKeyAsync<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-sync-generator"));

        Check.That(key).IsNotNull();
        Check.That(generator.SyncCalls).IsEqualTo(1);
    }

    [Test]
    public void Authorize_Throws_When_Sync_Path_Uses_AsyncOnlySignatureProcessor()
    {
        SyncOnlyAllowPolicyEngine engine = new();
        AsyncOnlySignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: engine,signatureprocessor: signatureprocessor));
        Tool tool = new(accessmanager: manager);
        Check.That(tool.EnableMyExceptions()).IsTrue();

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-sync-signature",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));

        Check.That(key).IsNotNull();
        Assert.Throws<InvalidOperationException>(() => manager.Authorize(key));
    }

    [Test]
    public async Task AuthorizeAsync_Uses_AsyncSignatureProcessor_When_DualModeProcessor_Is_Configured()
    {
        SyncOnlyAllowPolicyEngine engine = new();
        PreferAsyncSignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(policyengine: engine,signatureprocessor: signatureprocessor));
        Tool tool = new(accessmanager: manager);

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-async-signature",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean authorized = await manager.AuthorizeAsync(key);

        Check.That(tool).IsNotNull();
        Check.That(key).IsNotNull();
        Check.That(authorized).IsTrue();
        Check.That(signatureprocessor.LastSigningLogger).IsNotNull();
        Check.That(signatureprocessor.LastValidationLogger).IsNotNull();
        Check.That(signatureprocessor.AsyncVerifyCalls).IsEqualTo(1);
        Check.That(signatureprocessor.SyncVerifyCalls).IsEqualTo(0);
    }

    [Test]
    public async Task AuthorizeAsync_DefaultPolicyEngine_Still_Verifies_Signed_Key_When_SignatureProcessor_Is_Configured()
    {
        PreferAsyncSignatureProcessor signatureprocessor = new();
        AccessManager manager = new(AccessManagerOptions.Create(signatureprocessor: signatureprocessor),new Tool());

        ClientKey? key = await manager.GenerateAccessKeyAsync<ClientKey>(AccessKeyIssueOptions.Create(subject: "subject-default-async-signature",operations: ImmutableArray.Create(ProtectedOperation.GetOutputIDs)));
        Boolean authorized = await manager.AuthorizeAsync(key);

        Check.That(key).IsNotNull();
        Check.That(authorized).IsTrue();
        Check.That(signatureprocessor.AsyncVerifyCalls).IsEqualTo(1);
        Check.That(signatureprocessor.SyncVerifyCalls).IsEqualTo(0);
    }
}
