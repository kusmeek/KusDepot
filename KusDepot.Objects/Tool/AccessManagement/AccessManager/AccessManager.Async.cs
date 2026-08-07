namespace KusDepot.Security;

/**<include file='AccessManager.xml' path='AccessManager/class[@name="AccessManager"]/main/*'/>*/
public partial class AccessManager
{
    ///<inheritdoc/>
    public virtual async Task<Boolean> AccessCheckAsync(AccessKey? key , Int32 operation , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        try
        {
            if(operation < 0 || operation >= AccessKeySecret.MaxOperations) { return false; }

            if(TryGetPolicyMembership(key,out ToolManifestIdentity? manifestidentity,out MembershipToken membership,out DateTimeOffset evaluatedat,out Boolean manifestmatched,out Boolean expired,out Boolean membershipactive) is false) { return false; }

            Byte[]? secret = key?.GetKey();

            Boolean operationallowed = membership.Allows(operation);

            if(UsesDefaultPolicyEngine && !IsSignedAccessKeySecret(secret)) { return manifestmatched && !expired && membershipactive && operationallowed; }

            AccessKeyClaims claims = membership.ToClaims(evaluatedat);

            SignatureValidation? signaturevalidation = await CreateSignatureValidationAsync(manifestidentity,secret,cancel).ConfigureAwait(false);

            if(signaturevalidation is null) { return false; }

            AccessPolicyContext context = CreateAccessPolicyContext(manifestidentity,key,claims,evaluatedat,manifestmatched,expired,membershipactive,signaturevalidation.Value,operation,operationallowed,membership.Subject,Tool,Logger);

            AccessPolicyDecision decision = await EvaluatePolicyAsync(this.PolicyEngine,context,cancel).ConfigureAwait(false);

            return decision.Allowed;
        }
        catch ( Exception _ ) { Logger?.Error(_,AccessCheckFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(AccessKeys!)) { Exit(AccessKeys!); } }
    }

    ///<inheritdoc/>
    public virtual Task<Boolean> AccessCheckAsync(AccessKey? key = null , String? operationname = null , CancellationToken cancel = default)
    {
        if(cancel.IsCancellationRequested) { return Task.FromCanceled<Boolean>(cancel); }

        if(AccessKeys.Count == 0) { return Task.FromResult(false); }

        if(key is null || String.IsNullOrEmpty(operationname)) { return Task.FromResult(false); }

        try
        {
            Int32 op = ProtectedOperationResolver.ResolveIndex(operationname);

            if(op < 0 && Tool is not null) { _ = ToolManifestRegistry.TryResolveOperation(Tool.GetType(),operationname,out op); }

            return op < 0 ? Task.FromResult(false) : AccessCheckAsync(key,op,cancel);
        }
        catch ( Exception _ )
        {
            Logger?.Error(_,AccessCheckFail,this.GetType().Name,Tool?.GetID()?.ToString());

            return MyNoExceptions ? Task.FromResult(false) : Task.FromException<Boolean>(_);
        }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> AuthorizeAsync(AccessKey? key = null , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        try
        {
            if(TryGetPolicyMembership(key,out ToolManifestIdentity? manifestidentity,out MembershipToken membership,out DateTimeOffset evaluatedat,out Boolean manifestmatched,out Boolean expired,out Boolean membershipactive) is false) { return false; }

            Byte[]? secret = key?.GetKey();

            if(UsesDefaultPolicyEngine && !IsSignedAccessKeySecret(secret)) { return manifestmatched && !expired && membershipactive; }

            SignatureValidation? signaturevalidation = await CreateSignatureValidationAsync(manifestidentity,secret,cancel).ConfigureAwait(false);

            if(signaturevalidation is null) { return false; }

            AccessPolicyContext context = CreateAuthorizationPolicyContext(manifestidentity,key,membership.ToClaims(evaluatedat),evaluatedat,manifestmatched,expired,membershipactive,signaturevalidation.Value,membership.Subject,Tool,Logger);

            return (await EvaluatePolicyAsync(this.PolicyEngine,context,cancel).ConfigureAwait(false)).Allowed;
        }
        catch ( Exception _ ) { Logger?.Error(_,AuthorizeFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual async Task<AccessKey?> RequestAccessAsync(AccessRequest? request = null , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        try
        {
            if(request is null || Tool is null) { return null; }

            if(TryGetManifestIdentity(out ToolManifestIdentity? manifestidentity) is false) { return null; }

            AccessPolicyRequestContext requestcontext = CreateRequestPolicyContext(request,Tool);

            AccessPolicyContext context = CreateRequestEvaluationContext(manifestidentity,request,requestcontext,Tool,Logger);

            AccessPolicyDecision decision = await EvaluatePolicyAsync(this.PolicyEngine,context,cancel).ConfigureAwait(false);

            if(!decision.Allowed) { return null; }

            return await IssueRequestedAccessKeyAsync(requestcontext,cancel).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,RequestAccessFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> RevokeAccessAsync(AccessKey? key = null , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        try
        {
            if(TryGetPolicyMembership(key,out ToolManifestIdentity? manifestidentity,out MembershipToken membershiptoken,out DateTimeOffset evaluatedat,out Boolean manifestmatched,out Boolean expired,out Boolean membershipactive) is false) { return false; }

            Byte[]? secret = key?.GetKey();

            if(UsesDefaultPolicyEngine && !IsSignedAccessKeySecret(secret))
            {
                if(!manifestmatched) { return false; }
            }

            else
            {
                SignatureValidation? signaturevalidation = await CreateSignatureValidationAsync(manifestidentity,secret,cancel).ConfigureAwait(false);

                if(signaturevalidation is null) { return false; }

                AccessPolicyContext context = CreateRevocationPolicyContext(manifestidentity,key,membershiptoken.ToClaims(evaluatedat),evaluatedat,manifestmatched,expired,membershipactive,signaturevalidation.Value,membershiptoken.Subject,Tool,Logger);

                AccessPolicyDecision revocation = await EvaluatePolicyAsync(this.PolicyEngine,context,cancel).ConfigureAwait(false);

                if(!revocation.Allowed) { return false; }
            }

            Boolean removed = RemoveMembership(membershiptoken.Subject,membershiptoken.Token);

            if(removed && secret is not null && MembershipCacheKey.TryCreate(secret,out MembershipCacheKey cachekey))
            {
                _ = RemoveMembershipMetadata(membershiptoken.Token);

                _ = this.MembershipCache.Remove(cachekey);
            }

            return removed;
        }
        catch ( Exception _ ) { Logger?.Error(_,RevokeAccessFail,this.GetType().Name,Tool?.GetID()?.ToString()); if(MyNoExceptions) { return false; } throw; }
    }
}
