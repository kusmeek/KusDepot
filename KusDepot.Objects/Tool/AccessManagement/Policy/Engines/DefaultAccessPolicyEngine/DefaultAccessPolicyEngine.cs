namespace KusDepot.Security.Policy;

/**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/main/*'/>*/
public sealed class DefaultAccessPolicyEngine : IAccessPolicyEngine , IAsyncAccessPolicyEngine
{
    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/constructor[@name="Constructor"]/*'/>*/
    private DefaultAccessPolicyEngine() {}

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/property[@name="Instance"]/*'/>*/
    public static DefaultAccessPolicyEngine Instance { get; } = new();

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/method[@name="Evaluate"]/*'/>*/
    public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
    {
        return context.Mode switch
        {
            AccessPolicyMode.Access => EvaluateAccess(in context),

            AccessPolicyMode.Authorize => EvaluateAuthorization(in context),

            AccessPolicyMode.Issue => EvaluateIssuance(in context),

            AccessPolicyMode.Request => EvaluateRequest(in context),

            AccessPolicyMode.Revoke => EvaluateRevocation(in context),

            _ => AccessPolicyDecision.Deny(UnknownPolicyMode)
        };
    }

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/method[@name="EvaluateAsync"]/*'/>*/
    public async ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        cancel.ThrowIfCancellationRequested();

        return context.Mode switch
        {
            AccessPolicyMode.Access => await EvaluateAccessAsync(context,cancel).ConfigureAwait(false),

            AccessPolicyMode.Authorize => await EvaluateAuthorizationAsync(context,cancel).ConfigureAwait(false),

            AccessPolicyMode.Issue => await EvaluateIssuanceAsync(context,cancel).ConfigureAwait(false),

            AccessPolicyMode.Request => EvaluateRequest(in context),

            AccessPolicyMode.Revoke => await EvaluateRevocationAsync(context,cancel).ConfigureAwait(false),

            _ => AccessPolicyDecision.Deny(UnknownPolicyMode)
        };
    }

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/method[@name="EvaluateAccess"]/*'/>*/
    private static AccessPolicyDecision EvaluateAccess(in AccessPolicyContext context)
    {
        AccessPolicyDecision decision = EvaluateAuthorization(in context); if(!decision.Allowed) { return decision; }

        decision = OperationRequired.Evaluate(in context); if(!decision.Allowed) { return decision; }

        decision = OperationAllowed.Evaluate(in context); if(!decision.Allowed) { return decision; }

        return AccessPolicyDecision.Allow();
    }

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/method[@name="EvaluateAccessAsync"]/*'/>*/
    private static async ValueTask<AccessPolicyDecision> EvaluateAccessAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        AccessPolicyDecision decision = await EvaluateAuthorizationAsync(context,cancel).ConfigureAwait(false); if(!decision.Allowed) { return decision; }

        decision = await OperationRequired.EvaluateAsync(context,cancel).ConfigureAwait(false); if(!decision.Allowed) { return decision; }

        decision = await OperationAllowed.EvaluateAsync(context,cancel).ConfigureAwait(false); if(!decision.Allowed) { return decision; }

        return AccessPolicyDecision.Allow();
    }

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/method[@name="EvaluateAuthorization"]/*'/>*/
    private static AccessPolicyDecision EvaluateAuthorization(in AccessPolicyContext context)
    {
        AccessPolicyDecision decision = ClaimsRequired.Evaluate(in context); if(!decision.Allowed) { return decision; }

        decision = ManifestMatch.Evaluate(in context); if(!decision.Allowed) { return decision; }

        decision = Expiry.Evaluate(in context); if(!decision.Allowed) { return decision; }

        decision = MembershipActive.Evaluate(in context); if(!decision.Allowed) { return decision; }

        return AccessPolicyDecision.Allow();
    }

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/method[@name="EvaluateAuthorizationAsync"]/*'/>*/
    private static async ValueTask<AccessPolicyDecision> EvaluateAuthorizationAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        AccessPolicyDecision decision = await ClaimsRequired.EvaluateAsync(context,cancel).ConfigureAwait(false); if(!decision.Allowed) { return decision; }

        decision = await ManifestMatch.EvaluateAsync(context,cancel).ConfigureAwait(false); if(!decision.Allowed) { return decision; }

        decision = await Expiry.EvaluateAsync(context,cancel).ConfigureAwait(false); if(!decision.Allowed) { return decision; }

        decision = await MembershipActive.EvaluateAsync(context,cancel).ConfigureAwait(false); if(!decision.Allowed) { return decision; }

        return AccessPolicyDecision.Allow();
    }

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/method[@name="EvaluateIssuance"]/*'/>*/
    private static AccessPolicyDecision EvaluateIssuance(in AccessPolicyContext context)
    {
        return SubjectRequired.Evaluate(in context);
    }

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/method[@name="EvaluateIssuanceAsync"]/*'/>*/
    private static ValueTask<AccessPolicyDecision> EvaluateIssuanceAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        return SubjectRequired.EvaluateAsync(context,cancel);
    }

    private static AccessPolicyDecision EvaluateRequest(in AccessPolicyContext context)
    {
        if(context.RequestContext is not AccessPolicyRequestContext requestcontext) { return AccessPolicyDecision.Deny(RequestContextIsRequired); }

        if(String.IsNullOrWhiteSpace(requestcontext.RequestedKeyType)) { return AccessPolicyDecision.Deny(RequestedKeyTypeIsRequired); }

        return requestcontext.RequestValidated ? AccessPolicyDecision.Allow() : AccessPolicyDecision.Deny(AccessRequestIsNotValid);
    }

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/method[@name="EvaluateRevocation"]/*'/>*/
    private static AccessPolicyDecision EvaluateRevocation(in AccessPolicyContext context)
    {
        AccessPolicyDecision decision = ClaimsRequired.Evaluate(in context); if(!decision.Allowed) { return decision; }

        decision = ManifestMatch.Evaluate(in context); if(!decision.Allowed) { return decision; }

        return AccessPolicyDecision.Allow();
    }

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/method[@name="EvaluateRevocationAsync"]/*'/>*/
    private static async ValueTask<AccessPolicyDecision> EvaluateRevocationAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        AccessPolicyDecision decision = await ClaimsRequired.EvaluateAsync(context,cancel).ConfigureAwait(false); if(!decision.Allowed) { return decision; }

        decision = await ManifestMatch.EvaluateAsync(context,cancel).ConfigureAwait(false); if(!decision.Allowed) { return decision; }

        return AccessPolicyDecision.Allow();
    }

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/field[@name="ClaimsRequired"]/*'/>*/
    private static readonly ClaimsRequiredStage ClaimsRequired = ClaimsRequiredStage.Instance;

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/field[@name="Expiry"]/*'/>*/
    private static readonly ExpiryStage Expiry = ExpiryStage.Instance;

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/field[@name="ManifestMatch"]/*'/>*/
    private static readonly ManifestMatchStage ManifestMatch = ManifestMatchStage.Instance;

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/field[@name="MembershipActive"]/*'/>*/
    private static readonly MembershipActiveStage MembershipActive = MembershipActiveStage.Instance;

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/field[@name="OperationAllowed"]/*'/>*/
    private static readonly OperationAllowedStage OperationAllowed = OperationAllowedStage.Instance;

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/field[@name="OperationRequired"]/*'/>*/
    private static readonly OperationRequiredStage OperationRequired = OperationRequiredStage.Instance;

    /**<include file='DefaultAccessPolicyEngine.xml' path='DefaultAccessPolicyEngine/class[@name="DefaultAccessPolicyEngine"]/field[@name="SubjectRequired"]/*'/>*/
    private static readonly SubjectRequiredStage SubjectRequired = SubjectRequiredStage.Instance;
}