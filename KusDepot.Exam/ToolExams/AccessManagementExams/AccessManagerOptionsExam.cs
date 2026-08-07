namespace KusDepot.Exams.Security;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class AccessManagerOptionsExam
{
    private sealed class DummySignatureProcessor : IAccessKeySignatureProcessor
    {
        public AccessKeySignatureInfo GetSigningInfo(AccessKeySignatureSigningContext context)
        {
            return new()
            {
                Algorithm = AccessKeySignatureAlgorithm.RsaPssSha512,
                SignerKeyID = [1,2,3],
                SignerKeyIDFormat = AccessKeySignerKeyIdFormat.Thumbprint,
                SignatureLength = 64
            };
        }

        public Boolean TrySign(AccessKeySignatureSigningContext context , Span<Byte> signature , out Int32 written)
        {
            Byte[] digest = SHA512.HashData(context.SignedRegion.Span).AsSpan(0,64).ToArray();
            if(signature.Length < digest.Length) { written = 0; return false; }
            digest.CopyTo(signature);
            written = digest.Length;
            return true;
        }

        public SignatureValidation Verify(AccessKeySignatureValidationContext context)
        {
            return new()
            {
                Algorithm = context.Signature.Algorithm,
                Signed = context.Signature.Signed,
                SignatureValid = true,
                VerificationAttempted = true,
                SignerAccessManagerID = context.Signature.SignerAccessManagerID,
                SignerKeyID = context.Signature.SignerKeyID,
                SignerKeyIDFormat = context.Signature.SignerKeyIDFormat,
                SignerToolID = context.Signature.SignerToolID
            };
        }
    }

    [Test]
    public void ConstructorOptions_ManifestOverride_Takes_Precedence_When_Initializing_ManifestIdentity()
    {
        var tool = new Tool();
        ToolManifest manifest = ToolManifest.Create("KusDepot.Tools/OverrideSchema","Override-Realm","1.0.0",typeof(Tool).FullName,[]).ComputeManifestHash();
        var manager = new AccessManager(new AccessManagerOptions(manifest:manifest),tool);

        FieldInfo field = typeof(AccessManager).GetField("ManifestIdentity",BindingFlags.NonPublic | BindingFlags.Instance)!;
        Object? identity = field.GetValue(manager);

        Check.That(tool).IsNotNull();
        Check.That(identity).IsNotNull();
        Check.That(identity!.GetType().GetProperty("ToolSchemaID")!.GetValue(identity) as String).IsEqualTo(manifest.ToolSchemaID);
        Check.That(identity.GetType().GetProperty("AccessKeyRealmID")!.GetValue(identity) as String).IsEqualTo(manifest.AccessKeyRealmID);
        Check.That(identity.GetType().GetProperty("ManifestHash")!.GetValue(identity) as String).IsEqualTo(manifest.ManifestHash);
    }

    [Test]
    public void ConstructorOptions_Sets_InitialMembershipCatalog()
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
        AccessManagerMembershipCatalog? memberships = manager1.QueryMembershipCatalog(key:executiveKey1);

        var manager2 = new AccessManager(new AccessManagerOptions(id:sharedManagerId,realmkey:sharedRealmKey,initialmembershipcatalog:memberships));
        var tool2 = new Tool(id:sharedToolId,accessmanager:manager2);
        var ownerKey2 = tool2.CreateOwnerKey("Owner2");
        Check.That(tool2.TakeOwnership(ownerKey2)).IsTrue();
        var executiveKey2 = tool2.RequestAccess(new ManagementRequest(ownerKey2!)) as ExecutiveKey;
        Check.That(tool2.AddOutput(Guid.NewGuid(),"output2",executiveKey2)).IsTrue();
        Check.That(tool2.Lock(ownerKey2)).IsTrue();

        Check.That(clientKey1).IsNotNull();
        Check.That(memberships).IsNotNull();
        Check.That(tool2.GetOutputIDs(clientKey1)).IsNotNull();
    }

    [Test]
    public void AccessManagerOptions_Create_Configures_All_Values()
    {
        Byte[] realmkey = new Byte[AccessKeySecret.SymmetricKeySize];
        RandomNumberGenerator.Fill(realmkey);
        Guid id = Guid.NewGuid();
        ToolManifest manifest = ToolManifest.Create("KusDepot.Tools/Options","Options-Realm","1.0.0",typeof(Tool).FullName,[]).ComputeManifestHash();
        IAccessPolicyEngine policyengine = new AccessManagerExam.DenyAuthorizationPolicyEngine();
        DummySignatureProcessor signatureprocessor = new();
        AccessKeyToken token = new(SHA512.HashData(Encoding.UTF8.GetBytes("subject-a")));
        AccessManagerMembershipCatalog memberships = new()
        {
            ToolManifestIdentity = ToolManifestIdentity.Create(manifest),
            Entries = new Dictionary<String,ImmutableArray<AccessManagerMembershipEntry>>
            {
                ["subject-a"] = [new AccessManagerMembershipEntry
                {
                    AccessKeyRealmID = manifest!.AccessKeyRealmID!,
                    IssuedAt = DateTimeOffset.UtcNow,
                    IssuerAccessManagerID = Guid.NewGuid(),
                    IssuerToolID = Guid.NewGuid(),
                    ManifestHash = manifest!.ManifestHash!,
                    Permissions = [1,2,3],
                    Subject = "subject-a",
                    Token = token,
                    ToolSchemaID = manifest!.ToolSchemaID!
                }]
            }
        };

        AccessManagerOptions options = AccessManagerOptions.Create(id,realmkey,manifest,policyengine,32,memberships,null,null,signatureprocessor);

        Check.That(options.ID).IsEqualTo(id);
        Check.That(options.RealmKey).IsNotNull();
        Check.That(options.RealmKey!.SequenceEqual(realmkey)).IsTrue();
        Check.That(options.Manifest).IsNotNull();
        Check.That(options.Manifest!.ToolSchemaID).IsEqualTo(manifest.ToolSchemaID);
        Check.That(options.Manifest.AccessKeyRealmID).IsEqualTo(manifest.AccessKeyRealmID);
        Check.That(options.Manifest.ManifestHash).IsEqualTo(manifest.ManifestHash);
        Check.That(options.PolicyEngine).IsSameReferenceAs(policyengine);
        Check.That(options.SignatureProcessor).IsSameReferenceAs(signatureprocessor);
        Check.That(options.MembershipCacheSize).IsEqualTo(32);
        Check.That(options.InitialMembershipCatalog).IsNotNull();
        Check.That(options.InitialMembershipCatalog!.Entries).IsNotNull();
        Check.That(options.InitialMembershipCatalog.Entries!["subject-a"][0].Token).IsEqualTo(token);
    }

    [Test]
    public void AccessManagerOptions_SetID_Updates_Current_Instance()
    {
        AccessManagerOptions options = AccessManagerOptions.Create();
        Guid id = Guid.NewGuid();

        AccessManagerOptions updated = options.SetID(id);

        Check.That(updated).IsSameReferenceAs(options);
        Check.That(updated.ID).IsEqualTo(id);
    }

    [Test]
    public void AccessManagerOptions_SetManifest_Updates_Current_Instance_With_Computed_Hash()
    {
        AccessManagerOptions options = AccessManagerOptions.Create();
        ToolManifest manifest = ToolManifest.Create("KusDepot.Tools/OptionsManifest","OptionsManifest-Realm","1.0.0",typeof(Tool).FullName,[]);

        manifest.ComputeManifestHash();
        AccessManagerOptions updated = options.SetManifest(manifest);

        Check.That(updated).IsSameReferenceAs(options);
        Check.That(updated.Manifest).IsNotNull();
        Check.That(updated.Manifest!.ToolSchemaID).IsEqualTo(manifest.ToolSchemaID);
        Check.That(updated.Manifest.AccessKeyRealmID).IsEqualTo(manifest.AccessKeyRealmID);
        Check.That(updated.Manifest.ManifestHash).IsNotNull().And.Not.IsEmpty();
    }

    [Test]
    public void AccessManagerOptions_SetMembershipCacheSize_Normalizes_NonPositive_Value()
    {
        AccessManagerOptions options = AccessManagerOptions.Create();
        AccessManagerOptions updated = options.SetMembershipCacheSize(0);

        Check.That(updated).IsSameReferenceAs(options);
        Check.That(updated.MembershipCacheSize).IsEqualTo(MembershipCache.DefaultMaximumCount);
    }

    [Test]
    public void AccessManagerOptions_SetInitialMembershipCatalog_Clones_Source_Memberships()
    {
        AccessKeyToken token = new(SHA512.HashData(Encoding.UTF8.GetBytes("subject-b")));
        ToolManifest manifest = ToolManifest.Create("KusDepot.Tools/OptionsMembership","OptionsMembership-Realm","1.0.0",typeof(Tool).FullName,[]).ComputeManifestHash();
        AccessManagerMembershipCatalog source = new()
        {
            ToolManifestIdentity = ToolManifestIdentity.Create(manifest),
            Entries = new Dictionary<String,ImmutableArray<AccessManagerMembershipEntry>>
            {
                ["subject-b"] = [new AccessManagerMembershipEntry
                {
                    AccessKeyRealmID = manifest!.AccessKeyRealmID!,
                    IssuedAt = DateTimeOffset.UtcNow,
                    IssuerAccessManagerID = Guid.NewGuid(),
                    IssuerToolID = Guid.NewGuid(),
                    ManifestHash = manifest!.ManifestHash!,
                    Permissions = [1,2,3],
                    Subject = "subject-b",
                    Token = token,
                    ToolSchemaID = manifest!.ToolSchemaID!
                }]
            }
        };

        AccessManagerOptions options = AccessManagerOptions.Create();
        AccessManagerOptions updated = options.SetInitialMembershipCatalog(source);
        source.Entries = new Dictionary<String,ImmutableArray<AccessManagerMembershipEntry>>();
        source.ToolManifestIdentity = ToolManifestIdentity.Create(ToolManifest.Create("KusDepot.Tools/Changed","Changed-Realm","1.0.0",typeof(Tool).FullName,[]).ComputeManifestHash());

        Check.That(updated).IsSameReferenceAs(options);
        Check.That(updated.InitialMembershipCatalog).IsNotNull();
        Check.That(updated.InitialMembershipCatalog!.Entries).IsNotNull();
        Check.That(updated.InitialMembershipCatalog.Entries!["subject-b"][0].Token).IsEqualTo(token);
        Check.That(updated.InitialMembershipCatalog.ToolManifestIdentity).IsNotNull();
        Check.That(updated.InitialMembershipCatalog.ToolManifestIdentity!.ToolSchemaID).IsEqualTo(manifest.ToolSchemaID);
    }

    [Test]
    public void AccessManagerOptions_SetPolicyEngine_Updates_Current_Instance()
    {
        AccessManagerOptions options = AccessManagerOptions.Create();
        IAccessPolicyEngine policyengine = new AccessManagerExam.DenyAuthorizationPolicyEngine();

        AccessManagerOptions updated = options.SetPolicyEngine(policyengine);

        Check.That(updated).IsSameReferenceAs(options);
        Check.That(updated.PolicyEngine).IsSameReferenceAs(policyengine);
    }

    [Test]
    public void AccessManagerOptions_SetRealmKey_Updates_Current_Instance_And_Clones_Source_KeyMaterial()
    {
        Byte[] realmkey = new Byte[AccessKeySecret.SymmetricKeySize];
        RandomNumberGenerator.Fill(realmkey);

        AccessManagerOptions options = AccessManagerOptions.Create();
        AccessManagerOptions updated = options.SetRealmKey(realmkey);
        Byte[] beforeMutation = updated.RealmKey!;
        realmkey[0] ^= 0xFF;

        Check.That(updated).IsSameReferenceAs(options);
        Check.That(updated.RealmKey).IsNotNull();
        Check.That(updated.RealmKey!.SequenceEqual(beforeMutation)).IsTrue();
        Check.That(updated.RealmKey![0]).IsEqualTo(beforeMutation[0]);
    }

    [Test]
    public void AccessManagerOptions_SetSignatureProcessor_Updates_Current_Instance()
    {
        AccessManagerOptions options = AccessManagerOptions.Create();
        DummySignatureProcessor signatureprocessor = new();

        AccessManagerOptions updated = options.SetSignatureProcessor(signatureprocessor);

        Check.That(updated).IsSameReferenceAs(options);
        Check.That(updated.SignatureProcessor).IsSameReferenceAs(signatureprocessor);
    }
}
