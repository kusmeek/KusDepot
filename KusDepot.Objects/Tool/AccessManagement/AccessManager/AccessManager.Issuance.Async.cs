namespace KusDepot.Security;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public partial class AccessManager
{
    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="CreateIssuedAccessKeyMaterialAsync"]/*'/>*/
    private async Task<IssuedAccessKeyMaterial?> CreateIssuedAccessKeyMaterialAsync(AccessKeyIssueOptions? options = null , AccessPolicyRequestContext? requestcontext = null , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        AccessKeyIssuanceContext? issuancecontext = CreateIssuanceContext(options,requestcontext); if(issuancecontext is null) { return null; }

        AccessPolicyContext issuepolicycontext = CreateIssuePolicyContext(issuancecontext.ManifestIdentity,issuancecontext.IssueOptions,issuancecontext.Tool,issuancecontext.Logger);

        if(!UsesDefaultPolicyEngine)
        {
            AccessPolicyDecision issuance = await EvaluatePolicyAsync(this.PolicyEngine,issuepolicycontext,cancel).ConfigureAwait(false);

            if(!issuance.Allowed) { return null; }
        }

        AccessKeyAssertionGeneratorContext assertiongeneratorcontext = CreateAssertionGeneratorContext(issuepolicycontext,issuancecontext.ManifestIdentity,issuancecontext.IssueOptions.RequestedKeyType ?? String.Empty,issuancecontext.RequestContext);

        ImmutableArray<AccessKeyAssertion> assertions = await GenerateKeyAssertionsAsync(assertiongeneratorcontext,cancel).ConfigureAwait(false); issuancecontext = issuancecontext with { Assertions = assertions };

        cancel.ThrowIfCancellationRequested();

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

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="GenerateKeyAssertionsAsync"]/*'/>*/
    protected virtual async Task<ImmutableArray<AccessKeyAssertion>> GenerateKeyAssertionsAsync(AccessKeyAssertionGeneratorContext context , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        IAccessKeyAssertionGeneratorMode? keyassertiongenerator = this.KeyAssertionGenerator;

        if(keyassertiongenerator is null) { return []; }

        ImmutableArray<AccessKeyAssertion> assertions = keyassertiongenerator switch
        {
            IAsyncAccessKeyAssertionGenerator asynchronous => await asynchronous.GenerateAsync(context,cancel).ConfigureAwait(false),

            IAccessKeyAssertionGenerator synchronous => synchronous.Generate(in context),

            _ => throw new InvalidOperationException(ConfiguredAssertionGeneratorModeInvalid)
        };

        if(assertions.IsDefaultOrEmpty) { return []; }

        var normalized = ImmutableArray.CreateBuilder<AccessKeyAssertion>();

        foreach(AccessKeyAssertion? assertion in assertions)
        {
            cancel.ThrowIfCancellationRequested();

            if(assertion is not null) { normalized.Add(assertion); }
        }

        return normalized.ToImmutable();
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="IssueAccessKeyAsync"]/*'/>*/
    protected virtual async Task<TKey?> IssueAccessKeyAsync<TKey>(AccessKeyIssueOptions? options = null , AccessPolicyRequestContext? requestcontext = null , CancellationToken cancel = default) where TKey : AccessKey
    {
        cancel.ThrowIfCancellationRequested();

        try
        {
            String? requestedkeytype = NormalizeRequestedKeyType(typeof(TKey).FullName ?? typeof(TKey).Name) ?? typeof(TKey).Name;

            return await IssueAccessKeyAsync(CreateIssueOptionsForRequestedKeyType(requestedkeytype,options: options),requestcontext,cancel).ConfigureAwait(false) as TKey;
        }
        catch ( Exception _ ) { Logger?.Error(_,IssueAccessKeyFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="IssueAccessKeyAsyncDispatch"]/*'/>*/
    private async Task<AccessKey?> IssueAccessKeyAsync(AccessKeyIssueOptions? options = null , AccessPolicyRequestContext? requestcontext = null , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        String? requestedkeytype = NormalizeRequestedKeyType(options?.RequestedKeyType); if(String.IsNullOrWhiteSpace(requestedkeytype)) { return null; }

        IssuedAccessKeyMaterial? material = await CreateIssuedAccessKeyMaterialAsync(options,requestcontext,cancel).ConfigureAwait(false); if(material is null) { return null; }

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

    /**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/method[@name="IssueRequestedAccessKeyAsync"]/*'/>*/
    protected virtual async Task<AccessKey?> IssueRequestedAccessKeyAsync(AccessPolicyRequestContext requestcontext , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        if(!requestcontext.RequestValidated) { return null; }

        return await IssueAccessKeyAsync(CreateIssueOptions(in requestcontext),requestcontext,cancel).ConfigureAwait(false);
    }
}