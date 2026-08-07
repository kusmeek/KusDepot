namespace KusDepot.Exams;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public partial class JsonUtilityExam
{
    private static Byte[] Bytes(Byte seed)
    {
        var data = new Byte[32];
        for(Int32 i = 0; i < data.Length; i++) data[i] = (Byte)(seed + i);
        return data;
    }

    [Test]
    [TestCase(typeof(Uri))]
    [TestCase(typeof(Guid))]
    [TestCase(typeof(Guid?))]
    [TestCase(typeof(Version))]
    [TestCase(typeof(DateTime))]
    [TestCase(typeof(TimeSpan))]
    [TestCase(typeof(BigInteger))]
    [TestCase(typeof(DateTimeOffset))]
    [TestCase(typeof(Byte[]))]
    [TestCase(typeof(Int32[]))]
    [TestCase(typeof(HashSet<Guid>))]
    [TestCase(typeof(List<String>))]
    [TestCase(typeof(Dictionary<String,String>))]
    [TestCase(typeof(Dictionary<String,Int32>))]
    [TestCase(typeof(HashSet<DataItem>))]
    [TestCase(typeof(SortedList<Int32,MSBuildItem>))]
    [TestCase(typeof(KusDepot.Data.Models.Command))]
    [TestCase(typeof(KusDepot.Data.Models.CommandQuery))]
    [TestCase(typeof(KusDepot.Data.Models.CommandResponse))]
    [TestCase(typeof(KusDepot.Data.Models.Element))]
    [TestCase(typeof(KusDepot.Data.Models.ElementQuery))]
    [TestCase(typeof(KusDepot.Data.Models.ElementResponse))]
    [TestCase(typeof(KusDepot.Data.Models.Media))]
    [TestCase(typeof(KusDepot.Data.Models.MediaQuery))]
    [TestCase(typeof(KusDepot.Data.Models.MediaResponse))]
    [TestCase(typeof(KusDepot.Data.Models.NoteQuery))]
    [TestCase(typeof(KusDepot.Data.Models.NoteResponse))]
    [TestCase(typeof(KusDepot.Data.Models.Service))]
    [TestCase(typeof(KusDepot.Data.Models.ServiceQuery))]
    [TestCase(typeof(KusDepot.Data.Models.ServiceResponse))]
    [TestCase(typeof(KusDepot.Data.Models.TagQuery))]
    [TestCase(typeof(KusDepot.Data.Models.TagResponse))]
    [TestCase(typeof(AccessRequest))]
    [TestCase(typeof(BinaryItem))]
    [TestCase(typeof(CodeItem))]
    [TestCase(typeof(CommandDescriptor))]
    [TestCase(typeof(CommandWorkflow))]
    [TestCase(typeof(DataItem))]
    [TestCase(typeof(DataSetItem))]
    [TestCase(typeof(Descriptor))]
    [TestCase(typeof(GenericItem))]
    [TestCase(typeof(GuidReferenceItem))]
    [TestCase(typeof(KeySet))]
    [TestCase(typeof(KusDepotCab))]
    [TestCase(typeof(MSBuildItem))]
    [TestCase(typeof(MultiMediaItem))]
    [TestCase(typeof(SecurityKey))]
    [TestCase(typeof(TextItem))]
    [TestCase(typeof(ToolDescriptor))]
    public void IsSupported_Type_ReturnsTrue_ForTypesRegisteredInJsonContext(Type type)
    {
        Check.That(JsonUtility.IsSupported(type)).IsTrue();
    }

    [Test]
    public void IsSupported_Generic_ReturnsTrue_ForTypeRegisteredInJsonContext()
    {
        Check.That(JsonUtility.IsSupported<SecurityKey>()).IsTrue();
        Check.That(JsonUtility.IsSupported<Dictionary<String,String>>()).IsTrue();
    }

    [Test]
    [TestCase(typeof(ToolBuilder))]
    [TestCase(typeof(JsonUtilityExam))]
    public void IsSupported_Type_ReturnsFalse_ForTypesNotRegisteredInJsonContext(Type type)
    {
        Check.That(JsonUtility.IsSupported(type)).IsFalse();
    }

    [Test]
    public void Parse_And_ToJsonString_AccessRequest_Roundtrip()
    {
        using var cert = CreateCertificate(Guid.NewGuid(), "Mgr");
        var original = new ManagementRequest(new ManagerKey(cert!, Guid.NewGuid()));

        var json = JsonUtility.ToJsonString((AccessRequest)original, typeof(AccessRequest));
        var polymorphic = AccessRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"ManagementRequest\"");
        Check.That(polymorphic).IsInstanceOf<ManagementRequest>();
        Check.That(((ManagementRequest)polymorphic!).Credential).IsEqualTo(original.Credential);
        Check.That(((ManagementRequest)polymorphic!).Subject).IsEqualTo(original.Subject);
    }

    [Test]
    public void Parse_And_ToJsonString_StandardRequest_Roundtrip()
    {
        var original = new StandardRequest("standard-request");

        var json = JsonUtility.ToJsonString((AccessRequest)original, typeof(AccessRequest));
        var polymorphic = AccessRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"StandardRequest\"");
        Check.That(polymorphic).IsInstanceOf<StandardRequest>();
        Check.That(((StandardRequest)polymorphic!).Subject).IsEqualTo(original.Subject);
    }

    [Test]
    public void Parse_And_ToJsonString_ServiceRequest_Roundtrip()
    {
        var original = new ServiceRequest(null,"service-request");

        var json = JsonUtility.ToJsonString((AccessRequest)original, typeof(AccessRequest));
        var polymorphic = AccessRequest.Parse(json);

        Check.That(json).Contains("\"Type\":\"ServiceRequest\"");
        Check.That(polymorphic).IsInstanceOf<ServiceRequest>();
        Check.That(((ServiceRequest)polymorphic!).Subject).IsEqualTo(original.Subject);
        Check.That(((ServiceRequest)polymorphic!).Tool).IsNull();
    }

    [Test]
    public void Serialize_And_Deserialize_ManagementRequest_Roundtrip()
    {
        using var cert = CreateCertificate(Guid.NewGuid(), "Mgr");
        var original = new ManagementRequest(new ManagerKey(cert!, Guid.NewGuid()));

        var bytes = JsonUtility.Serialize(original);
        var roundtrip = JsonUtility.Deserialize<ManagementRequest>(bytes);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Credential).IsEqualTo(original.Credential);
        Check.That(roundtrip.Subject).IsEqualTo(original.Subject);
    }

    [Test]
    public void Parse_And_ToJsonString_AccessCredential_Roundtrip()
    {
        var original = new JwtCredential() { Token = "jwt-token-value" };

        var json = JsonUtility.ToJsonString((AccessCredential)original, typeof(AccessCredential));
        var polymorphic = JsonUtility.Parse<AccessCredential>(json);

        Check.That(json).Contains("\"Type\":\"JwtCredential\"");
        Check.That(polymorphic).IsInstanceOf<JwtCredential>();
        Check.That(((JwtCredential)polymorphic!).Token).IsEqualTo(original.Token);
    }

    [Test]
    public void Serialize_And_Deserialize_CertificateCredential_Roundtrip()
    {
        var original = new CertificateCredential() { Certificate = [1,2,3,4] };

        var bytes = JsonUtility.Serialize(original);
        var roundtrip = JsonUtility.Deserialize<CertificateCredential>(bytes);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Certificate).IsEqualTo(original.Certificate);
    }

    [Test]
    public void Parse_And_ToJsonString_AccessAssertionRequest_Roundtrip()
    {
        var original = new TokenAssertionRequest() { Required = false };

        var json = JsonUtility.ToJsonString((AccessAssertionRequest)original, typeof(AccessAssertionRequest));
        var polymorphic = JsonUtility.Parse<AccessAssertionRequest>(json);

        Check.That(json).Contains("\"Type\":\"TokenAssertionRequest\"");
        Check.That(polymorphic).IsInstanceOf<TokenAssertionRequest>();
        Check.That(((TokenAssertionRequest)polymorphic!).Required).IsEqualTo(original.Required);
    }

    [Test]
    public void Parse_And_ToJsonString_SamlAssertionRequest_Roundtrip()
    {
        var original = new SamlAssertionRequest() { Required = false };

        var json = JsonUtility.ToJsonString((AccessAssertionRequest)original, typeof(AccessAssertionRequest));
        var polymorphic = JsonUtility.Parse<AccessAssertionRequest>(json);

        Check.That(json).Contains("\"Type\":\"SamlAssertionRequest\"");
        Check.That(polymorphic).IsInstanceOf<SamlAssertionRequest>();
        Check.That(((SamlAssertionRequest)polymorphic!).Required).IsEqualTo(original.Required);
        Check.That(((SamlAssertionRequest)polymorphic).AssertionType).IsEqualTo("saml2");
    }

    [Test]
    public void Parse_And_ToJsonString_SecurityKey_Roundtrip()
    {
        var original = new ServiceKey(Bytes(4), Guid.NewGuid());

        var json = JsonUtility.ToJsonString(original);
        var roundtrip = JsonUtility.Parse<ServiceKey>(json);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip).IsEqualTo(original);
    }

    [Test]
    public void Serialize_And_Deserialize_SecurityKey_Roundtrip()
    {
        using var cert = CreateCertificate(Guid.NewGuid(), "Mgr");
        var original = new ManagerKey(cert!, Guid.NewGuid());

        var bytes = JsonUtility.Serialize(original);
        var roundtrip = JsonUtility.Deserialize<ManagerKey>(bytes);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Thumbprint).IsEqualTo(original.Thumbprint);
        Check.That(roundtrip).IsEqualTo(original);
    }

    [Test]
    public void Runtime_Type_ToJsonString_Preserves_Derived_Key_Members()
    {
        var original = new TokenKey(new Byte[] { 1, 2, 3 }, Guid.NewGuid(), TokenKeyType.Spnego);

        var json = JsonUtility.ToJsonString((Object)original, typeof(TokenKey));
        var roundtrip = TokenKey.Parse(json);

        Check.That(json).Contains("\"TokenType\":10");
        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.TokenType).IsEqualTo(TokenKeyType.Spnego);
        Check.That(roundtrip).IsEqualTo(original);
    }

    [Test]
    public void Runtime_Type_Serialize_Preserves_Derived_Key_Members()
    {
        var original = new TokenKey(new Byte[] { 9, 4, 9 }, Guid.NewGuid(), TokenKeyType.Jwt);

        var bytes = JsonUtility.Serialize((Object)original, typeof(TokenKey));
        var roundtrip = TokenKey.Parse(Encoding.UTF8.GetString(bytes));

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.TokenType).IsEqualTo(TokenKeyType.Jwt);
        Check.That(roundtrip).IsEqualTo(original);
    }

    [Test]
    public void ToFile_And_FromFile_AccessRequest_Roundtrip()
    {
        var path = Path.Combine(Path.GetTempPath(), $"JsonUtilityExam-Request-{Guid.NewGuid():N}.json");
        var original = new StandardRequest("json-utility-file");

        try
        {
            var written = JsonUtility.ToFile(path, (AccessRequest)original, typeof(AccessRequest));
            var polymorphic = AccessRequest.Parse(File.ReadAllText(path));

            Check.That(written).IsTrue();
            Check.That(polymorphic).IsInstanceOf<StandardRequest>();
            Check.That(((StandardRequest)polymorphic!).Subject).IsEqualTo(original.Subject);
        }
        finally
        {
            if(File.Exists(path)) { File.Delete(path); }
        }
    }

    [Test]
    public void ToFile_And_FromFile_SecurityKey_Roundtrip()
    {
        var path = Path.Combine(Path.GetTempPath(), $"JsonUtilityExam-Key-{Guid.NewGuid():N}.json");
        var original = new HostKey(Bytes(7), Guid.NewGuid());

        try
        {
            var written = JsonUtility.ToFile(path, (Object)original, typeof(HostKey));
            var roundtrip = JsonUtility.FromFile<HostKey>(path);

            Check.That(written).IsTrue();
            Check.That(roundtrip).IsNotNull();
            Check.That(roundtrip).IsEqualTo(original);
        }
        finally
        {
            if(File.Exists(path)) { File.Delete(path); }
        }
    }

    [Test]
    public void Serialize_And_Deserialize_AccessManagerQuery_Roundtrip()
    {
        var original = AccessManagerQuery.Create(
            subjects: ["subject-a","subject-b"],
            protectedoperations: [ProtectedOperation.GetOutput,ProtectedOperation.GetOutputIDs],
            audiences: ["aud-a","aud-b"],
            scopes: ["scope.a","scope.b"],
            includeexpired: false);

        var bytes = JsonUtility.Serialize(original);
        var roundtrip = JsonUtility.Deserialize<AccessManagerQuery>(bytes);

        Check.That(roundtrip).IsNotNull();
        Check.That(roundtrip!.Audiences.HasValue).IsTrue();
        Check.That(roundtrip.Audiences!.Value).IsEqualTo(original.Audiences!.Value);
        Check.That(roundtrip.IncludeExpired).IsEqualTo(original.IncludeExpired);
        Check.That(roundtrip.ProtectedOperations.HasValue).IsTrue();
        Check.That(roundtrip.ProtectedOperations!.Value).IsEqualTo(original.ProtectedOperations!.Value);
        Check.That(roundtrip.Scopes.HasValue).IsTrue();
        Check.That(roundtrip.Scopes!.Value).IsEqualTo(original.Scopes!.Value);
        Check.That(roundtrip.Subjects.HasValue).IsTrue();
        Check.That(roundtrip.Subjects!.Value).IsEqualTo(original.Subjects!.Value);
    }

    [Test]
    public void Serialize_And_Deserialize_AccessManagement_Assertion_And_Claims_Types_Roundtrip()
    {
        TokenAssertionSummary summary = CreateTokenAssertionSummary("json");
        SamlAssertionSummary samlSummary = CreateSamlAssertionSummary("json");
        AccessKeyToken token = CreateAccessKeyToken("json-token");
        AccessKeyClaims claims = CreateAccessKeyClaims(token: default);
        AccessKeyClaims populatedclaims = CreateAccessKeyClaims(summary,default);
        AccessKeyClaims populatedSamlClaims = CreateAccessKeyClaims(samlSummary,default);
        TokenAssertion assertion = CreateTokenAssertion("json");

        AssertJsonRoundTrip(summary,rt =>
        {
            Check.That(rt.AssertionType).IsEqualTo(summary.AssertionType);
            Check.That(rt.Audiences).IsEqualTo(summary.Audiences);
            Check.That(rt.AuthenticationMethods).IsEqualTo(summary.AuthenticationMethods);
            Check.That(rt.Format).IsEqualTo(summary.Format);
            Check.That(rt.Issuer).IsEqualTo(summary.Issuer);
            Check.That(rt.Scopes).IsEqualTo(summary.Scopes);
        });

        AssertJsonRoundTrip(samlSummary,rt =>
        {
            Check.That(rt.AssertionType).IsEqualTo(samlSummary.AssertionType);
            Check.That(rt.Audiences).IsEqualTo(samlSummary.Audiences);
            Check.That(rt.AssertionID).IsEqualTo(samlSummary.AssertionID);
            Check.That(rt.Format).IsEqualTo(samlSummary.Format);
            Check.That(rt.Issuer).IsEqualTo(samlSummary.Issuer);
            Check.That(rt.NameID).IsEqualTo(samlSummary.NameID);
            Check.That(rt.SessionIndex).IsEqualTo(samlSummary.SessionIndex);
        });

        AssertJsonRoundTrip(assertion,rt =>
        {
            Check.That(rt.AssertionType).IsEqualTo(assertion.AssertionType);
            Check.That(rt.Audiences).IsEqualTo(assertion.Audiences);
            Check.That(rt.AuthenticationMethods).IsEqualTo(assertion.AuthenticationMethods);
            Check.That(rt.Format).IsEqualTo(assertion.Format);
            Check.That(rt.Issuer).IsEqualTo(assertion.Issuer);
            Check.That(rt.Scopes).IsEqualTo(assertion.Scopes);
            Check.That(rt.Token).IsEqualTo(assertion.Token);
            Check.That(rt.TokenType).IsEqualTo(assertion.TokenType);
        });

        AccessKeyToken roundtriptoken = JsonUtility.Deserialize<AccessKeyToken>(JsonUtility.Serialize(token));
        Check.That(roundtriptoken).IsEqualTo(token);

        AssertJsonRoundTrip(claims,rt =>
        {
            Check.That(rt.AccessKeyRealmID).IsEqualTo(claims.AccessKeyRealmID);
            Check.That(rt.AssertionSummaries).IsEmpty();
            Check.That(rt.Audiences).IsEqualTo(claims.Audiences);
            Check.That(rt.NotAfter).IsEqualTo(claims.NotAfter);
            Check.That(rt.Permissions).IsEqualTo(claims.Permissions);
            Check.That(rt.Scopes).IsEqualTo(claims.Scopes);
            Check.That(rt.Token).IsEqualTo(claims.Token);
        });

        AssertJsonRoundTrip(populatedclaims,rt =>
        {
            Check.That(rt.AccessKeyRealmID).IsEqualTo(populatedclaims.AccessKeyRealmID);
            Check.That(rt.AssertionSummaries.Length).IsEqualTo(1);
            Check.That(rt.AssertionSummaries[0]).IsInstanceOf<TokenAssertionSummary>();
            Check.That(((TokenAssertionSummary)rt.AssertionSummaries[0]).Issuer).IsEqualTo(summary.Issuer);
            Check.That(rt.Audiences).IsEqualTo(populatedclaims.Audiences);
            Check.That(rt.NotAfter).IsEqualTo(populatedclaims.NotAfter);
            Check.That(rt.Permissions).IsEqualTo(populatedclaims.Permissions);
            Check.That(rt.Scopes).IsEqualTo(populatedclaims.Scopes);
            Check.That(rt.Token).IsEqualTo(populatedclaims.Token);
        });

        AssertJsonRoundTrip(populatedSamlClaims,rt =>
        {
            Check.That(rt.AccessKeyRealmID).IsEqualTo(populatedSamlClaims.AccessKeyRealmID);
            Check.That(rt.AssertionSummaries.Length).IsEqualTo(1);
            Check.That(rt.AssertionSummaries[0]).IsInstanceOf<SamlAssertionSummary>();
            Check.That(((SamlAssertionSummary)rt.AssertionSummaries[0]).Issuer).IsEqualTo(samlSummary.Issuer);
            Check.That(rt.Audiences).IsEqualTo(populatedSamlClaims.Audiences);
            Check.That(rt.NotAfter).IsEqualTo(populatedSamlClaims.NotAfter);
            Check.That(rt.Permissions).IsEqualTo(populatedSamlClaims.Permissions);
            Check.That(rt.Scopes).IsEqualTo(populatedSamlClaims.Scopes);
            Check.That(rt.Token).IsEqualTo(populatedSamlClaims.Token);
        });
    }

    [Test]
    public void Serialize_And_Deserialize_AccessManagement_Policy_And_Generator_Context_Types_Roundtrip()
    {
        ToolManifestIdentity manifestidentity = CreateAccessManagementManifestIdentity();
        AccessKeyIssueOptions options = AccessKeyIssueOptions.Create(
            operations: [ProtectedOperation.GetOutput,ProtectedOperation.GetOutputIDs],
            lifetime: TimeSpan.FromMinutes(5),
            audiences: ["aud-a","aud-b"],
            scopes: ["scope.a","scope.b"]);

        AccessPolicyRequestContext requestcontext = new()
        {
            ExternalRequest = true,
            HostMatched = false,
            ManagementCredentialValid = true,
            Request = new StandardRequest("policy-request"),
            RequestedKeyType = nameof(ClientKey),
            RequestValidated = true,
            ServiceMatched = false
        };

        AccessPolicyDecision decision = AccessPolicyDecision.Deny("policy-reason");

        AccessPolicyContext context = new()
        {
            AccessKeyRealmID = manifestidentity.AccessKeyRealmID,
            AssertionSummaries = ImmutableArray<AccessKeyAssertionSummary>.Empty,
            Claims = null,
            EvaluatedAt = DateTimeOffset.UtcNow,
            Expired = false,
            IssueOptions = options,
            ManifestHash = manifestidentity.ManifestHash,
            ManifestMatched = true,
            MembershipActive = true,
            Mode = AccessPolicyMode.Access,
            Operation = ProtectedOperation.GetOutputIDs,
            OperationAllowed = true,
            Operations = ImmutableArray.Create(ProtectedOperation.GetOutput,ProtectedOperation.GetOutputIDs),
            RequestContext = requestcontext,
            SignatureValidation = new() { SignerKeyID = ImmutableArray<Byte>.Empty },
            Subject = "policy-subject",
            ToolSchemaID = manifestidentity.ToolSchemaID
        };

        AccessKeyAssertionGeneratorContext generatorcontext = new()
        {
            AccessPolicyContext = context,
            IssueOptions = options,
            ManifestIdentity = manifestidentity,
            Request = new StandardRequest("generator-request"),
            RequestedKeyType = nameof(ClientKey),
            RequestContext = requestcontext,
            Subject = "policy-subject"
        };

        AssertJsonRoundTrip(options,rt =>
        {
            Check.That(rt.Audiences.HasValue).IsTrue();
            Check.That(rt.Audiences!.Value).IsEqualTo(options.Audiences!.Value);
            Check.That(rt.Lifetime).IsEqualTo(options.Lifetime);
            Check.That(rt.Operations.HasValue).IsTrue();
            Check.That(rt.Operations!.Value).IsEqualTo(options.Operations!.Value);
            Check.That(rt.Scopes.HasValue).IsTrue();
            Check.That(rt.Scopes!.Value).IsEqualTo(options.Scopes!.Value);
        });

        AssertJsonRoundTrip(requestcontext,rt =>
        {
            Check.That(rt.ExternalRequest).IsEqualTo(requestcontext.ExternalRequest);
            Check.That(rt.ManagementCredentialValid).IsEqualTo(requestcontext.ManagementCredentialValid);
            Check.That(rt.Request).IsInstanceOf<StandardRequest>();
            Check.That(rt.RequestedKeyType).IsEqualTo(requestcontext.RequestedKeyType);
            Check.That(rt.RequestValidated).IsEqualTo(requestcontext.RequestValidated);
        });

        AssertJsonRoundTrip(decision,rt =>
        {
            Check.That(rt.Allowed).IsFalse();
            Check.That(rt.Reason).IsEqualTo(decision.Reason);
        });

        AssertJsonRoundTrip(context,rt =>
        {
            Check.That(rt.AssertionSummaries.HasValue).IsTrue();
            Check.That(rt.AssertionSummaries!.Value).IsEmpty();
            Check.That(rt.Claims).IsNull();
            Check.That(rt.IssueOptions).IsNotNull();
            Check.That(rt.Key).IsNull();
            Check.That(rt.Mode).IsEqualTo(AccessPolicyMode.Access);
            Check.That(rt.Operations.HasValue).IsTrue();
            Check.That(rt.Operations!.Value).IsEqualTo(context.Operations!.Value);
            Check.That(rt.RequestContext).IsNotNull();
        });

        AssertJsonRoundTrip(generatorcontext,rt =>
        {
            Check.That(rt.AccessPolicyContext.Mode).IsEqualTo(AccessPolicyMode.Access);
            Check.That(rt.IssueOptions).IsNotNull();
            Check.That(rt.ManifestIdentity).IsNotNull();
            Check.That(rt.Request).IsInstanceOf<StandardRequest>();
            Check.That(rt.RequestedKeyType).IsEqualTo(nameof(ClientKey));
            Check.That(rt.RequestContext).IsNotNull();
            Check.That(rt.Subject).IsEqualTo("policy-subject");
        });
    }

    [Test]
    public void Serialize_And_Deserialize_AccessManagement_Snapshot_And_Catalog_Projection_Types_Roundtrip()
    {
        ToolManifestIdentity manifestidentity = CreateAccessManagementManifestIdentity();
        AccessManagerMembershipEntry membershipentry = new()
        {
            AccessKeyRealmID = manifestidentity.AccessKeyRealmID,
            AssertionSummaries = [],
            Audiences = ["aud-a"],
            IssuedAt = DateTimeOffset.UtcNow,
            IssuerAccessManagerID = Guid.NewGuid(),
            IssuerToolID = Guid.NewGuid(),
            ManifestHash = manifestidentity.ManifestHash,
            NotAfter = DateTimeOffset.UtcNow.AddMinutes(10),
            Permissions = Bytes(3),
            Scopes = ["scope.a"],
            Subject = "snapshot-subject",
            Token = default,
            ToolSchemaID = manifestidentity.ToolSchemaID
        };

        AccessManagerMembershipCatalog catalog = new()
        {
            Entries = new Dictionary<String,ImmutableArray<AccessManagerMembershipEntry>> { ["snapshot-subject"] = [membershipentry] },
            ToolManifestIdentity = manifestidentity
        };

        AccessManagerSnapshotEntry snapshotentry = new()
        {
            AccessKeyRealmID = manifestidentity.AccessKeyRealmID,
            AssertionSummaries = [],
            Audiences = ["aud-a"],
            Expired = false,
            IssuedAt = membershipentry.IssuedAt,
            IssuerAccessManagerID = membershipentry.IssuerAccessManagerID,
            IssuerToolID = membershipentry.IssuerToolID,
            ManifestHash = manifestidentity.ManifestHash,
            NotAfter = membershipentry.NotAfter,
            Permissions = Bytes(5),
            Scopes = ["scope.a"],
            Subject = "snapshot-subject",
            ToolSchemaID = manifestidentity.ToolSchemaID
        };

        AccessManagerSnapshot snapshot = new()
        {
            AccessKeys = new Dictionary<String,ImmutableArray<AccessManagerSnapshotEntry>> { ["snapshot-subject"] = [snapshotentry] },
            ToolManifestIdentity = manifestidentity
        };

        AssertJsonRoundTrip(membershipentry,rt =>
        {
            Check.That(rt.AssertionSummaries).IsEmpty();
            Check.That(rt.Audiences).IsEqualTo(membershipentry.Audiences);
            Check.That(rt.Permissions).IsEqualTo(membershipentry.Permissions);
            Check.That(rt.Scopes).IsEqualTo(membershipentry.Scopes);
            Check.That(rt.Token).IsEqualTo(membershipentry.Token);
        });

        AssertJsonRoundTrip(catalog,rt =>
        {
            Check.That(rt.Entries).IsNotNull();
            Check.That(rt.Entries!.ContainsKey("snapshot-subject")).IsTrue();
            Check.That(rt.Entries["snapshot-subject"][0].AssertionSummaries).IsEmpty();
            Check.That(rt.ToolManifestIdentity).IsNotNull();
        });

        AssertJsonRoundTrip(snapshotentry,rt =>
        {
            Check.That(rt.AssertionSummaries).IsEmpty();
            Check.That(rt.Permissions).IsEqualTo(snapshotentry.Permissions);
            Check.That(rt.Scopes).IsEqualTo(snapshotentry.Scopes);
        });

        AssertJsonRoundTrip(snapshot,rt =>
        {
            Check.That(rt.AccessKeys).IsNotNull();
            Check.That(rt.AccessKeys!["snapshot-subject"][0].AssertionSummaries).IsEmpty();
            Check.That(rt.ToolManifestIdentity).IsNotNull();
        });
    }

    private static void AssertJsonRoundTrip<T>(T original , Action<T> assert)
    {
        Byte[] bytes = JsonUtility.Serialize(original);
        T? roundtrip = JsonUtility.Deserialize<T>(bytes);

        Assert.That(roundtrip, Is.Not.Null);

        assert(roundtrip!);
    }

    private static AccessKeyClaims CreateAccessKeyClaims(AccessKeyAssertionSummary? summary = null , AccessKeyToken token = default)
    {
        return new()
        {
            AccessKeyRealmID = "json-realm",
            AssertionSummaries = summary is null ? [] : [summary],
            Audiences = ["aud-a","aud-b"],
            Expired = false,
            IssuedAt = DateTimeOffset.UtcNow.AddMinutes(-2),
            IssuerAccessManagerID = Guid.NewGuid(),
            IssuerToolID = Guid.NewGuid(),
            ManifestHash = "manifest-hash",
            NotAfter = DateTimeOffset.UtcNow.AddMinutes(10),
            Permissions = Bytes(1),
            Scopes = ["scope.a","scope.b"],
            Subject = "claims-subject",
            Token = token,
            ToolSchemaID = "KusDepot.Tools/Json"
        };
    }

    private static AccessKeyToken CreateAccessKeyToken(String value)
    {
        return new AccessKeyToken(SHA512.HashData(Encoding.UTF8.GetBytes(value)));
    }

    private static ToolManifestIdentity CreateAccessManagementManifestIdentity()
    {
        return ToolManifestIdentity.Create(ToolManifest.Create("KusDepot.Tools/Json","Json-Realm","1.0.0",typeof(Tool).FullName,[]).ComputeManifestHash());
    }

    private static TokenAssertion CreateTokenAssertion(String suffix)
    {
        return new()
        {
            AuthenticationContextClassReference = "urn:kdp:" + suffix,
            Audiences = ["aud-" + suffix],
            AuthenticationMethods = ["pwd","mfa"],
            AuthenticationTime = DateTimeOffset.UtcNow.AddMinutes(-1),
            AuthorizedParty = "azp-" + suffix,
            ClientID = "client-" + suffix,
            Format = "jwt/v1",
            IssuedAt = DateTimeOffset.UtcNow.AddMinutes(-2),
            Issuer = "https://issuer.example/" + suffix,
            JwtID = "jwt-" + suffix,
            NotAfter = DateTimeOffset.UtcNow.AddMinutes(10),
            NotBefore = DateTimeOffset.UtcNow.AddMinutes(-3),
            ObjectID = "object-" + suffix,
            Scopes = ["scope." + suffix],
            SessionID = "sid-" + suffix,
            Subject = "subject-" + suffix,
            TenantID = "tenant-" + suffix,
            Token = Encoding.UTF8.GetBytes("token-" + suffix),
            TokenType = TokenKeyType.Jwt
        };
    }

    private static SamlAssertionSummary CreateSamlAssertionSummary(String suffix)
    {
        return new()
        {
            AssertionID = "assertion-" + suffix,
            AssertionType = "saml2",
            Audiences = ["aud-" + suffix],
            AuthenticationContextClassReference = "urn:oasis:names:tc:SAML:2.0:ac:classes:PasswordProtectedTransport",
            AuthenticationTime = DateTimeOffset.UtcNow.AddMinutes(-1),
            Destination = "https://sp.example/acs/" + suffix,
            Format = "saml2/v1",
            InResponseTo = "request-" + suffix,
            Issuer = "https://issuer.example/" + suffix,
            NameID = "subject-" + suffix,
            NameIDFormat = "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress",
            NotAfter = DateTimeOffset.UtcNow.AddMinutes(10),
            NotBefore = DateTimeOffset.UtcNow.AddMinutes(-3),
            Recipient = "https://sp.example/recipient/" + suffix,
            SessionIndex = "session-" + suffix,
            SessionNotOnOrAfter = DateTimeOffset.UtcNow.AddMinutes(15),
            SubjectConfirmationMethod = "urn:oasis:names:tc:SAML:2.0:cm:bearer"
        };
    }

    private static TokenAssertionSummary CreateTokenAssertionSummary(String suffix)
    {
        return new()
        {
            AuthenticationContextClassReference = "urn:kdp:" + suffix,
            AssertionType = "token",
            Audiences = ["aud-" + suffix],
            AuthenticationMethods = ["pwd","mfa"],
            AuthenticationTime = DateTimeOffset.UtcNow.AddMinutes(-1),
            AuthorizedParty = "azp-" + suffix,
            ClientID = "client-" + suffix,
            Format = "jwt/v1",
            IssuedAt = DateTimeOffset.UtcNow.AddMinutes(-2),
            Issuer = "https://issuer.example/" + suffix,
            JwtID = "jwt-" + suffix,
            NotAfter = DateTimeOffset.UtcNow.AddMinutes(10),
            NotBefore = DateTimeOffset.UtcNow.AddMinutes(-3),
            ObjectID = "object-" + suffix,
            Scopes = ["scope." + suffix],
            SessionID = "sid-" + suffix,
            Subject = "subject-" + suffix,
            TenantID = "tenant-" + suffix
        };
    }
}