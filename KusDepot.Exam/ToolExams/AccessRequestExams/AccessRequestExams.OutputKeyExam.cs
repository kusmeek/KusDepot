namespace KusDepot.Exams.Security;

public partial class AccessRequestExams
{
    private static AccessManager GetAccessManager(Tool tool)
    {
        return tool.GetAccessManager() as AccessManager ?? throw new InvalidOperationException("Failed to resolve the tool access manager for the request exam.");
    }

    private static (Byte[] RealmKey,String ToolSchemaID,String AccessKeyRealmID) GetReadContext(AccessManager manager)
    {
        FieldInfo realmkeyfield = typeof(AccessManager).GetField("RealmKey",BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("Failed to resolve the access-manager realm key field for the request exam.");
        FieldInfo manifestidentityfield = typeof(AccessManager).GetField("ManifestIdentity",BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("Failed to resolve the access-manager manifest identity field for the request exam.");

        Byte[] realmkey = (Byte[])realmkeyfield.GetValue(manager)!;
        Object manifestidentity = manifestidentityfield.GetValue(manager)
            ?? throw new InvalidOperationException("Failed to read the access-manager manifest identity for the request exam.");

        return (
            realmkey,
            manifestidentity.GetType().GetProperty("ToolSchemaID")!.GetValue(manifestidentity) as String ?? String.Empty,
            manifestidentity.GetType().GetProperty("AccessKeyRealmID")!.GetValue(manifestidentity) as String ?? String.Empty);
    }

    private static AccessKeyClaims ReadClaims(AccessManager manager , AccessKey key)
    {
        (Byte[] realmkey,String toolschemaid,String accesskeyrealmid) = GetReadContext(manager);

        if(!AccessKeySecret.TryReadClaims(key.Key!,realmkey,toolschemaid,accesskeyrealmid,out AccessKeyClaims claims))
        {
            throw new InvalidOperationException("Failed to read access-key claims from the issued request key.");
        }

        return claims;
    }

    private static ImmutableArray<AccessKeyAssertion> ReadAssertions(AccessManager manager , AccessKey key)
    {
        (Byte[] realmkey,String toolschemaid,String accesskeyrealmid) = GetReadContext(manager);

        if(!AccessKeySecret.TryReadAssertions(key.Key!,realmkey,toolschemaid,accesskeyrealmid,out ImmutableArray<AccessKeyAssertion> assertions))
        {
            throw new InvalidOperationException("Failed to read access-key assertions from the issued request key.");
        }

        return assertions;
    }

    private sealed class RequestDrivenAssertionGenerator : IAccessKeyAssertionGenerator
    {
        public ImmutableArray<AccessKeyAssertion> Generate(in AccessKeyAssertionGeneratorContext context)
        {
            if(context.RequestContext?.RequestedAssertions?.OfType<TokenAssertionRequest>().Any() is not true) { return []; }

            return
            [
                new TokenAssertion()
                {
                    Format = "jwt/v1",
                    IssuedAt = DateTimeOffset.UtcNow,
                    Subject = context.Subject,
                    Token = Encoding.UTF8.GetBytes("request-assertion-token"),
                    TokenType = TokenKeyType.Jwt
                }
            ];
        }
    }

    private sealed class CredentialDrivenAssertionGenerator : IAccessKeyAssertionGenerator
    {
        public ImmutableArray<AccessKeyAssertion> Generate(in AccessKeyAssertionGeneratorContext context)
        {
            if(context.RequestContext?.RequestedAssertions?.OfType<TokenAssertionRequest>().Any() is not true) { return []; }

            if(context.RequestContext?.Credential is not JwtCredential credential || String.IsNullOrWhiteSpace(credential.Token)) { return []; }

            return
            [
                new TokenAssertion()
                {
                    Format = "jwt/v1",
                    IssuedAt = DateTimeOffset.UtcNow,
                    Subject = context.Subject,
                    Token = Encoding.UTF8.GetBytes(credential.Token),
                    TokenType = TokenKeyType.Jwt
                }
            ];
        }
    }

    [Test]
    public void StandardRequest_RequestAccess_Projects_Subject_Audiences_Scopes_And_Lifetime_Into_OutputKey()
    {
        Tool tool = new();
        AccessManager manager = GetAccessManager(tool);
        TimeSpan lifetime = TimeSpan.FromMinutes(5);
        StandardRequest request = new("request-subject")
        {
            Audiences = ["aud-a","aud-b"],
            Lifetime = lifetime,
            Properties = ImmutableDictionary<String,String>.Empty.Add("request-property","request-value"),
            Scopes = ["scope.a","scope.b"]
        };

        ClientKey? key = tool.RequestAccess(request) as ClientKey;

        Check.That(key).IsNotNull();

        AccessKeyClaims claims = ReadClaims(manager,key!);

        Check.That(claims.Subject).IsEqualTo(request.Subject);
        Check.That(claims.Audiences.SequenceEqual(request.Audiences!.Value)).IsTrue();
        Check.That(claims.Scopes.SequenceEqual(request.Scopes!.Value)).IsTrue();
        Check.That(claims.NotAfter).IsNotNull();
        Check.That(Math.Abs(((claims.NotAfter!.Value - claims.IssuedAt) - lifetime).TotalSeconds) < 1).IsTrue();
    }

    [Test]
    public void StandardRequest_RequestAccess_Uses_RequestedKeyType_In_OutputKey()
    {
        Tool tool = new();
        AccessManager manager = GetAccessManager(tool);
        StandardRequest request = new("token-request-subject")
        {
            RequestedKeyType = typeof(TokenKey).FullName
        };

        AccessKey? key = tool.RequestAccess(request);

        Check.That(key).IsInstanceOf<TokenKey>();

        AccessKeyClaims claims = ReadClaims(manager,key!);

        Check.That(claims.Subject).IsEqualTo(request.Subject);
    }

    [Test]
    public void ManagementRequest_RequestAccess_Uses_Credential_By_Issuing_OutputKey()
    {
        Tool tool = new();
        OwnerKey ownerkey = tool.CreateOwnerKey("Owner")!;
        Check.That(tool.TakeOwnership(ownerkey)).IsTrue();
        AccessManager manager = GetAccessManager(tool);
        ManagementRequest request = new(ownerkey)
        {
            Subject = "management-request-subject"
        };

        AccessKey? key = tool.RequestAccess(request);

        Check.That(key).IsInstanceOf<ExecutiveKey>();

        AccessKeyClaims claims = ReadClaims(manager,key!);

        Check.That(claims.Subject).IsEqualTo(request.Subject);
    }

    [Test]
    public void StandardRequest_RequestAccess_Projects_RequestedAssertions_Into_OutputKey()
    {
        AccessManager manager = new(new AccessManagerOptions(keyassertiongenerator: new RequestDrivenAssertionGenerator()));
        Tool tool = new(accessmanager:manager);
        StandardRequest request = new("assertion-request-subject")
        {
            RequestedAssertions = [new TokenAssertionRequest()]
        };

        ClientKey? key = tool.RequestAccess(request) as ClientKey;

        Check.That(key).IsNotNull();

        ImmutableArray<AccessKeyAssertion> assertions = ReadAssertions(manager,key!);

        Check.That(assertions.Length).IsEqualTo(1);
        Check.That(assertions[0]).IsInstanceOf<TokenAssertion>();
        Check.That(((TokenAssertion)assertions[0]).Subject).IsEqualTo(request.Subject);
    }

    [Test]
    public async Task StandardRequest_RequestAccessAsync_WithShortLifetime_Works_Expires_And_Fails()
    {
        Tool tool = new();
        AccessManager manager = GetAccessManager(tool);
        StandardRequest request = new("short-lifetime-request")
        {
            Lifetime = TimeSpan.FromSeconds(1)
        };

        ClientKey? key = await manager.RequestAccessAsync(request).ConfigureAwait(false) as ClientKey;

        Check.That(key).IsNotNull();

        AccessKeyClaims claims = ReadClaims(manager,key!);
        Boolean authorizeBeforeExpiry = await manager.AuthorizeAsync(key).ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromMilliseconds(1500)).ConfigureAwait(false);

        Boolean authorizeAfterExpiry = await manager.AuthorizeAsync(key).ConfigureAwait(false);

        Check.That(claims.NotAfter).IsNotNull();
        Check.That(authorizeBeforeExpiry).IsTrue();
        Check.That(authorizeAfterExpiry).IsFalse();
    }

    [Test]
    public async Task ManagementRequest_RequestAccessAsync_Uses_Credential_And_Lifetime_In_OutputKey()
    {
        Tool tool = new();
        OwnerKey ownerkey = tool.CreateOwnerKey("Owner")!;
        Check.That(tool.TakeOwnership(ownerkey)).IsTrue();
        AccessManager manager = GetAccessManager(tool);
        ManagementRequest request = new(ownerkey)
        {
            Lifetime = TimeSpan.FromSeconds(1),
            Subject = "management-request-async-subject"
        };

        ExecutiveKey? key = await manager.RequestAccessAsync(request).ConfigureAwait(false) as ExecutiveKey;

        Check.That(key).IsNotNull();

        AccessKeyClaims claims = ReadClaims(manager,key!);
        Boolean authorizeBeforeExpiry = await manager.AuthorizeAsync(key).ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromMilliseconds(1500)).ConfigureAwait(false);

        Boolean authorizeAfterExpiry = await manager.AuthorizeAsync(key).ConfigureAwait(false);

        Check.That(claims.Subject).IsEqualTo(request.Subject);
        Check.That(claims.NotAfter).IsNotNull();
        Check.That(authorizeBeforeExpiry).IsTrue();
        Check.That(authorizeAfterExpiry).IsFalse();
    }

    [Test]
    public async Task StandardRequest_RequestAccessAsync_Projects_Credential_Into_OutputKey_Assertions()
    {
        AccessManager manager = new(new AccessManagerOptions(keyassertiongenerator: new CredentialDrivenAssertionGenerator()));
        Tool tool = new(accessmanager:manager);
        StandardRequest request = new("credential-assertion-request-subject")
        {
            Credential = new JwtCredential() { Token = "credential-jwt-token" },
            RequestedAssertions = [new TokenAssertionRequest()]
        };

        ClientKey? key = await manager.RequestAccessAsync(request).ConfigureAwait(false) as ClientKey;

        Check.That(key).IsNotNull();

        ImmutableArray<AccessKeyAssertion> assertions = ReadAssertions(manager,key!);

        Check.That(assertions.Length).IsEqualTo(1);
        Check.That(assertions[0]).IsInstanceOf<TokenAssertion>();
        Check.That(Encoding.UTF8.GetString(((TokenAssertion)assertions[0]).Token)).IsEqualTo("credential-jwt-token");
        Check.That(((TokenAssertion)assertions[0]).Subject).IsEqualTo(request.Subject);
    }
}
