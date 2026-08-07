namespace KusDepot.Exams.Security;

public partial class AccessKeyIssueOptionsExam
{
    private static (Byte[] RealmKey,String ToolSchemaID,String AccessKeyRealmID) GetReadContext(AccessManager manager)
    {
        FieldInfo realmkeyfield = typeof(AccessManager).GetField("RealmKey",BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("Failed to resolve the access-manager realm key field for the issue-options exam.");
        FieldInfo manifestidentityfield = typeof(AccessManager).GetField("ManifestIdentity",BindingFlags.NonPublic | BindingFlags.Instance)
            ?? throw new InvalidOperationException("Failed to resolve the access-manager manifest identity field for the issue-options exam.");

        Byte[] realmkey = (Byte[])realmkeyfield.GetValue(manager)!;
        Object manifestidentity = manifestidentityfield.GetValue(manager)
            ?? throw new InvalidOperationException("Failed to read the access-manager manifest identity for the issue-options exam.");

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
            throw new InvalidOperationException("Failed to read access-key claims from the issued secret.");
        }

        return claims;
    }

    private static ImmutableArray<AccessKeyAssertion> ReadAssertions(AccessManager manager , AccessKey key)
    {
        (Byte[] realmkey,String toolschemaid,String accesskeyrealmid) = GetReadContext(manager);

        if(!AccessKeySecret.TryReadAssertions(key.Key!,realmkey,toolschemaid,accesskeyrealmid,out ImmutableArray<AccessKeyAssertion> assertions))
        {
            throw new InvalidOperationException("Failed to read access-key assertions from the issued secret.");
        }

        return assertions;
    }

    private static AccessKey InvokeIssueAccessKey(AccessManager manager , AccessKeyIssueOptions options)
    {
        MethodInfo method = typeof(AccessManager).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Single(_ => _.Name == "IssueAccessKey" && _.IsGenericMethod is false && _.GetParameters().Length == 2 && _.GetParameters()[0].ParameterType == typeof(AccessKeyIssueOptions));

        return method.Invoke(manager,[options,null]) as AccessKey
            ?? throw new InvalidOperationException("Failed to issue the access key for the issue-options exam.");
    }

    private sealed class IssueOptionsAssertionGenerator : IAccessKeyAssertionGenerator
    {
        public ImmutableArray<AccessKeyAssertion> Generate(in AccessKeyAssertionGeneratorContext context)
        {
            if(context.IssueOptions?.RequestedAssertions?.OfType<TokenAssertionRequest>().Any() is not true) { return []; }
            if(context.IssueOptions.Credential is not JwtCredential credential) { return []; }

            String property = context.IssueOptions.Properties?.GetValueOrDefault("trace") ?? String.Empty;

            return
            [
                new TokenAssertion()
                {
                    Format = "jwt/v1",
                    IssuedAt = DateTimeOffset.UtcNow,
                    Subject = context.Subject,
                    Token = Encoding.UTF8.GetBytes($"{credential.Token}|{property}|{context.IssueOptions.RequestedKeyType}"),
                    TokenType = TokenKeyType.Jwt
                }
            ];
        }
    }

    [Test]
    public void GenerateAccessKey_Uses_Subject_Audiences_Scopes_Lifetime_And_Operations()
    {
        Tool tool = new();
        AccessManager manager = tool.GetAccessManager() as AccessManager ?? throw new InvalidOperationException();
        AccessKeyIssueOptions options = AccessKeyIssueOptions.Create(
            subject: "options-subject",
            operations: [ProtectedOperation.GetOutputIDs],
            lifetime: TimeSpan.FromMinutes(3),
            audiences: ["aud-a","aud-b"],
            scopes: ["scope.a","scope.b"]);

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(options);

        Check.That(key).IsNotNull();

        AccessKeyClaims claims = ReadClaims(manager,key!);

        Check.That(claims.Subject).IsEqualTo(options.Subject);
        Check.That(claims.Audiences.SequenceEqual(options.Audiences!.Value)).IsTrue();
        Check.That(claims.Scopes.SequenceEqual(options.Scopes!.Value)).IsTrue();
        Check.That(claims.NotAfter).IsNotNull();
        Check.That(Math.Abs(((claims.NotAfter!.Value - claims.IssuedAt) - options.Lifetime!.Value).TotalSeconds) < 1).IsTrue();
        Check.That(manager.AccessCheck(key,ProtectedOperation.GetOutputIDs)).IsTrue();
        Check.That(manager.AccessCheck(key,ProtectedOperation.AddOutput)).IsFalse();
    }

    [Test]
    public async Task GenerateAccessKeyAsync_Uses_Subject_Lifetime_And_Operations_When_Key_Expires()
    {
        Tool tool = new();
        AccessManager manager = tool.GetAccessManager() as AccessManager ?? throw new InvalidOperationException();
        AccessKeyIssueOptions options = AccessKeyIssueOptions.Create(
            subject: "async-options-subject",
            operations: [ProtectedOperation.GetOutputIDs],
            lifetime: TimeSpan.FromSeconds(1),
            audiences: ["aud-async"],
            scopes: ["scope.async"]);

        ClientKey? key = await manager.GenerateAccessKeyAsync<ClientKey>(options).ConfigureAwait(false);

        Check.That(key).IsNotNull();

        AccessKeyClaims claims = ReadClaims(manager,key!);
        Boolean allowedBeforeExpiry = await manager.AuthorizeAsync(key).ConfigureAwait(false);

        await Task.Delay(TimeSpan.FromMilliseconds(1500)).ConfigureAwait(false);

        Boolean allowedAfterExpiry = await manager.AuthorizeAsync(key).ConfigureAwait(false);

        Check.That(claims.Subject).IsEqualTo(options.Subject);
        Check.That(claims.NotAfter).IsNotNull();
        Check.That(claims.Audiences.SequenceEqual(options.Audiences!.Value)).IsTrue();
        Check.That(claims.Scopes.SequenceEqual(options.Scopes!.Value)).IsTrue();
        Check.That(allowedBeforeExpiry).IsTrue();
        Check.That(allowedAfterExpiry).IsFalse();
    }

    [Test]
    public void GenerateAccessKey_Uses_Credential_RequestedAssertions_And_Properties_In_OutputAssertions()
    {
        IssueOptionsAssertionGenerator generator = new();
        AccessManager manager = new(new AccessManagerOptions(keyassertiongenerator: generator),new Tool());
        AccessKeyIssueOptions options = AccessKeyIssueOptions.Create(
            subject: "assertion-options-subject",
            operations: [ProtectedOperation.GetOutputIDs],
            credential: new JwtCredential() { Token = "options-jwt-token" },
            properties: [new KeyValuePair<String,String>("trace","issue-options-property")],
            requestedassertions: [new TokenAssertionRequest()]);

        ClientKey? key = manager.GenerateAccessKey<ClientKey>(options);

        Check.That(key).IsNotNull();

        ImmutableArray<AccessKeyAssertion> assertions = ReadAssertions(manager,key!);

        Check.That(assertions.Length).IsEqualTo(1);
        Check.That(assertions[0]).IsInstanceOf<TokenAssertion>();
        Check.That(Encoding.UTF8.GetString(((TokenAssertion)assertions[0]).Token)).IsEqualTo($"options-jwt-token|issue-options-property|{nameof(ClientKey)}");
        Check.That(((TokenAssertion)assertions[0]).Subject).IsEqualTo(options.Subject);
    }

    [Test]
    public void IssueAccessKey_Uses_RequestedKeyType_From_IssueOptions()
    {
        AccessManager manager = new(tool:new Tool());
        AccessKeyIssueOptions options = AccessKeyIssueOptions.Create(
            subject: "requested-keytype-subject",
            operations: [ProtectedOperation.GetOutputIDs],
            requestedkeytype: typeof(TokenKey).FullName);

        AccessKey key = InvokeIssueAccessKey(manager,options);

        Check.That(key).IsInstanceOf<TokenKey>();

        AccessKeyClaims claims = ReadClaims(manager,key);

        Check.That(claims.Subject).IsEqualTo(options.Subject);
    }
}
