namespace KusDepot.Security;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public partial class AccessManager
{
    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateAssertionGeneratorContext"]/*'/>*/
    protected virtual AccessKeyAssertionGeneratorContext CreateAssertionGeneratorContext(
        AccessPolicyContext accesspolicycontext , ToolManifestIdentity manifestidentity , String requestedkeytype , AccessPolicyRequestContext? requestcontext = null)
    {
        return new()
        {
            AccessPolicyContext = accesspolicycontext,
            IssueOptions = accesspolicycontext.IssueOptions,
            Logger = accesspolicycontext.Logger,
            ManifestIdentity = manifestidentity,
            Request = requestcontext?.Request,
            RequestedKeyType = requestedkeytype ?? String.Empty,
            RequestContext = requestcontext,
            Subject = accesspolicycontext.Subject,
            Tool = accesspolicycontext.Tool
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="GenerateKeyAssertions"]/*'/>*/
    protected virtual ImmutableArray<AccessKeyAssertion> GenerateKeyAssertions(in AccessKeyAssertionGeneratorContext context)
    {
        IAccessKeyAssertionGenerator? keyassertiongenerator = GetSynchronousKeyAssertionGenerator(this.KeyAssertionGenerator);

        if(keyassertiongenerator is null) { return []; }

        ImmutableArray<AccessKeyAssertion> assertions = keyassertiongenerator.Generate(in context);

        if(assertions.IsDefaultOrEmpty) { return []; }

        var normalized = ImmutableArray.CreateBuilder<AccessKeyAssertion>();

        foreach(AccessKeyAssertion? assertion in assertions)
        {
            if(assertion is not null) { normalized.Add(assertion); }
        }

        return normalized.ToImmutable();
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateIssuanceContext"]/*'/>*/
    private AccessKeyIssuanceContext? CreateIssuanceContext(AccessKeyIssueOptions? options = null , AccessPolicyRequestContext? requestcontext = null)
    {
        AccessKeyIssueOptions issueoptions = CreateIssueOptions(options: options); String subject = (issueoptions.Subject ?? String.Empty).Trim();
        String requestedkeytype = NormalizeRequestedKeyType(issueoptions.RequestedKeyType) ?? String.Empty;

        if(String.IsNullOrWhiteSpace(requestedkeytype) || String.IsNullOrWhiteSpace(subject)) { return null; }

        if(TryGetManifestIdentity(out ToolManifestIdentity? manifestidentity) is false ||
           Tool?.GetID() is not Guid toolid || toolid == Guid.Empty ||
           ID is not Guid accessmanagerid || accessmanagerid == Guid.Empty)
        {
            return null;
        }

        return new()
        {
            IssueOptions = issueoptions,
            IssuerAccessManagerID = accessmanagerid,
            IssuerToolID = toolid,
            Logger = Logger ?? NullLogger.Instance,
            ManifestIdentity = manifestidentity,
            RequestContext = requestcontext,
            Subject = subject,
            Tool = Tool
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateIssueOptionsForRequestedKeyType"]/*'/>*/
    private AccessKeyIssueOptions CreateIssueOptionsForRequestedKeyType(String requestedkeytype , AccessKeyIssueOptions? options = null)
    {
        return AccessKeyIssueOptions.Create(
            subject: options?.Subject,
            operations: options?.Operations ?? ImmutableArray<Int32>.Empty,
            lifetime: options?.Lifetime,
            audiences: options?.Audiences,
            scopes: options?.Scopes,
            credential: options?.Credential,
            properties: options?.Properties,
            requestedassertions: options?.RequestedAssertions,
            requestedkeytype: NormalizeRequestedKeyType(requestedkeytype));
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateIssueOptions"]/*'/>*/
    protected virtual AccessKeyIssueOptions CreateIssueOptions(AccessKeyIssueOptions? options = null)
    {
        return AccessKeyIssueOptions.Create(
            subject: options?.Subject,
            operations: options?.Operations ?? ImmutableArray<Int32>.Empty,
            lifetime: options?.Lifetime,
            audiences: options?.Audiences,
            scopes: options?.Scopes,
            credential: options?.Credential,
            properties: options?.Properties,
            requestedassertions: options?.RequestedAssertions,
            requestedkeytype: NormalizeRequestedKeyType(options?.RequestedKeyType));
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateIssueOptionsFromRequestContext"]/*'/>*/
    protected virtual AccessKeyIssueOptions CreateIssueOptions(in AccessPolicyRequestContext requestcontext , AccessKeyIssueOptions? options = null)
    {
        return AccessKeyIssueOptions.Create(
            subject: options?.Subject ?? requestcontext.Subject,
            operations: options?.Operations ?? requestcontext.Operations ?? ImmutableArray<Int32>.Empty,
            lifetime: options?.Lifetime ?? requestcontext.Lifetime,
            audiences: options?.Audiences ?? requestcontext.Audiences,
            scopes: options?.Scopes ?? requestcontext.Scopes,
            credential: options?.Credential ?? requestcontext.Credential,
            properties: options?.Properties ?? requestcontext.Properties,
            requestedassertions: options?.RequestedAssertions ?? requestcontext.RequestedAssertions,
            requestedkeytype: NormalizeRequestedKeyType(options?.RequestedKeyType) ?? requestcontext.RequestedKeyType);
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateIssuePolicyContext"]/*'/>*/
    protected virtual AccessPolicyContext CreateIssuePolicyContext(ToolManifestIdentity manifestidentity , AccessKeyIssueOptions options , ITool? tool , ILogger? logger)
    {
        ArgumentNullException.ThrowIfNull(options);

        return new()
        {
            AccessKeyRealmID = manifestidentity.AccessKeyRealmID,
            EvaluatedAt = DateTimeOffset.UtcNow,
            IssueOptions = options,
            Logger = logger,
            ManifestHash = manifestidentity.ManifestHash,
            ManifestMatched = true,
            Mode = AccessPolicyMode.Issue,
            Operations = options.Operations ?? ImmutableArray<Int32>.Empty,
            Subject = options.Subject ?? String.Empty,
            Tool = tool,
            ToolSchemaID = manifestidentity.ToolSchemaID
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateBuiltInAccessKey"]/*'/>*/
    private static AccessKey? CreateBuiltInAccessKey(String requestedkeytype , Byte[] ciphertext)
    {
        return requestedkeytype switch
        {
            nameof(ClientKey)    => new ClientKey(ciphertext),
            nameof(CommandKey)   => new CommandKey(ciphertext),
            nameof(ExecutiveKey) => new ExecutiveKey(ciphertext),
            nameof(HostKey)      => new HostKey(ciphertext),
            nameof(MyHostKey)    => new MyHostKey(ciphertext),
            nameof(ServiceKey)   => new ServiceKey(ciphertext),
            nameof(TokenKey)     => new TokenKey(ciphertext),
            _                    => null
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateIssuedAccessKeyMaterial"]/*'/>*/
    private IssuedAccessKeyMaterial? CreateIssuedAccessKeyMaterial(AccessKeyIssueOptions? options = null , AccessPolicyRequestContext? requestcontext = null)
    {
        AccessKeyIssuanceContext? issuancecontext = CreateIssuanceContext(options,requestcontext); if(issuancecontext is null) { return null; }

        AccessPolicyContext issuepolicycontext = CreateIssuePolicyContext(issuancecontext.ManifestIdentity,issuancecontext.IssueOptions,issuancecontext.Tool,issuancecontext.Logger);

        if(!UsesDefaultPolicyEngine)
        {
            AccessPolicyDecision issuance = EvaluatePolicy(this.PolicyEngine,in issuepolicycontext);

            if(!issuance.Allowed) { return null; }
        }

        AccessKeyAssertionGeneratorContext assertiongeneratorcontext = CreateAssertionGeneratorContext(issuepolicycontext,issuancecontext.ManifestIdentity,issuancecontext.IssueOptions.RequestedKeyType ?? String.Empty,issuancecontext.RequestContext);

        ImmutableArray<AccessKeyAssertion> assertions = GenerateKeyAssertions(in assertiongeneratorcontext); issuancecontext = issuancecontext with { Assertions = assertions };

        if(TryIssueToken(issuancecontext,out Byte[]? ciphertext,out AccessKeyToken token) is false) { return null; }

        if(AddMembership(issuancecontext.Subject,token) is false) { return null; }

        MembershipToken? membership = null; MembershipCacheKey? cachekey = null;

        if(AccessKeySecret.TryGetMembershipToken(ciphertext,RealmKey,issuancecontext.ManifestIdentity.ToolSchemaID,issuancecontext.ManifestIdentity.AccessKeyRealmID,out MembershipToken resolvedmembership))
        {
            membership = resolvedmembership; _ = SetMembershipMetadata(resolvedmembership);

            if(MembershipCacheKey.TryCreate(ciphertext,out MembershipCacheKey issuedcachekey))
            {
                cachekey = issuedcachekey; _ = this.MembershipCache.TrySet(issuedcachekey,resolvedmembership.Token);
            }
        }

        return new()
        {
            Assertions = issuancecontext.Assertions,
            CacheKey = cachekey,
            Ciphertext = ciphertext!,
            ManifestIdentity = issuancecontext.ManifestIdentity,
            Membership = membership,
            Subject = issuancecontext.Subject,
            Token = token
        };
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="IssueAccessKey"]/*'/>*/
    protected virtual TKey? IssueAccessKey<TKey>(AccessKeyIssueOptions? options = null , AccessPolicyRequestContext? requestcontext = null) where TKey : AccessKey
    {
        try
        {
            String? requestedkeytype = NormalizeRequestedKeyType(typeof(TKey).FullName ?? typeof(TKey).Name);

            return IssueAccessKey(CreateIssueOptionsForRequestedKeyType(requestedkeytype!,options:options),requestcontext) as TKey;
        }
        catch ( Exception _ ) { Logger?.Error(_,IssueAccessKeyFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="IssueAccessKeyDispatch"]/*'/>*/
    private AccessKey? IssueAccessKey(AccessKeyIssueOptions? options = null , AccessPolicyRequestContext? requestcontext = null)
    {
        String? requestedkeytype = NormalizeRequestedKeyType(options?.RequestedKeyType); if(String.IsNullOrWhiteSpace(requestedkeytype)) { return null; }

        IssuedAccessKeyMaterial? material = CreateIssuedAccessKeyMaterial(options,requestcontext); if(material is null) { return null; }

        try
        {
            AccessKey? generated = CreateBuiltInAccessKey(requestedkeytype,material.Ciphertext); if(generated is not null) { return generated; }

            if(TryCreateCustomTypeAccessKey(requestedkeytype,material.Ciphertext,out generated)) { return generated; }

            RollbackIssuedAccessKeyMaterial(material); return null;
        }
        catch
        {
            RollbackIssuedAccessKeyMaterial(material); throw;
        }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="IssueRequestedAccessKey"]/*'/>*/
    protected virtual AccessKey? IssueRequestedAccessKey(in AccessPolicyRequestContext requestcontext)
    {
        if(!requestcontext.RequestValidated) { return null; }

        return IssueAccessKey(CreateIssueOptions(in requestcontext),requestcontext);
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="RollbackIssuedAccessKeyMaterial"]/*'/>*/
    private void RollbackIssuedAccessKeyMaterial(IssuedAccessKeyMaterial material)
    {
        _ = RemoveMembership(material.Subject,material.Token); _ = RemoveMembershipMetadata(material.Token);

        if(material.CacheKey is MembershipCacheKey cachekey) { _ = this.MembershipCache.Remove(cachekey); }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryCreateCustomTypeAccessKey"]/*'/>*/
    private Boolean TryCreateCustomTypeAccessKey(String requestedkeytype , Byte[] ciphertext , out AccessKey? key)
    {
        key = null;

        try
        {
            KeyMaker? keymaker = this.KeyMaker; if(keymaker is null) { return false; }

            keymaker.AutoID().WithOwnedKey(ciphertext); return keymaker.TryMake(requestedkeytype,out key);
        }
        catch { key = null; return false; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="TryIssueToken"]/*'/>*/
    private Boolean TryIssueToken(AccessKeyIssuanceContext issuancecontext , out Byte[]? ciphertext , out AccessKeyToken token)
    {
        ciphertext = null; token = default;

        try
        {
            ArgumentNullException.ThrowIfNull(issuancecontext);

            if(RealmKey is null) { return false; }

            IAccessKeySigner? signer = TryGetSignatureSigner(this.SignatureProcessor);

            return AccessKeySecret.TryCreate(issuancecontext,RealmKey,signer,out ciphertext,out token);
        }

        catch ( Exception _ ) { Logger?.Error(_,TryIssueTokenFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="GetSynchronousKeyAssertionGeneratorStatic"]/*'/>*/
    private static IAccessKeyAssertionGenerator? GetSynchronousKeyAssertionGenerator(IAccessKeyAssertionGeneratorMode? generator)
    {
        if(generator is null) { return null; }

        if(generator is IAccessKeyAssertionGenerator synchronous) { return synchronous; }

        if(generator is IAsyncAccessKeyAssertionGenerator)
        {
            throw new InvalidOperationException(SynchronousAccessManagerRequiresSynchronousAssertionGenerator);
        }

        throw new InvalidOperationException(ConfiguredAssertionGeneratorModeInvalid);
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="ValidateAssertionGeneratorMode"]/*'/>*/
    private static void ValidateAssertionGeneratorMode(IAccessKeyAssertionGeneratorMode? generator)
    {
        if(generator is null) { return; }

        if(generator is IAccessKeyAssertionGenerator or IAsyncAccessKeyAssertionGenerator) { return; }

        throw new InvalidOperationException(ConfiguredAssertionGeneratorModeInvalid);
    }
}