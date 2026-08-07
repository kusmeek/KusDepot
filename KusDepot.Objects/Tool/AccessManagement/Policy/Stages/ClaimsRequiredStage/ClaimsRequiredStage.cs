namespace KusDepot.Security.Policy;

/**<include file='ClaimsRequiredStage.xml' path='ClaimsRequiredStage/class[@name="ClaimsRequiredStage"]/main/*'/>*/
public sealed class ClaimsRequiredStage : IAccessPolicyStage , IAsyncAccessPolicyStage
{
    /**<include file='ClaimsRequiredStage.xml' path='ClaimsRequiredStage/class[@name="ClaimsRequiredStage"]/constructor[@name="Constructor"]/*'/>*/
    public ClaimsRequiredStage() {}

    /**<include file='ClaimsRequiredStage.xml' path='ClaimsRequiredStage/class[@name="ClaimsRequiredStage"]/property[@name="Instance"]/*'/>*/
    public static ClaimsRequiredStage Instance { get; } = new();

    /**<include file='ClaimsRequiredStage.xml' path='ClaimsRequiredStage/class[@name="ClaimsRequiredStage"]/method[@name="Evaluate"]/*'/>*/
    public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
    {
        return context.Claims is null ? AccessPolicyDecision.Deny(ValidatedClaimsAreRequired) : AccessPolicyDecision.Allow();
    }

    /**<include file='ClaimsRequiredStage.xml' path='ClaimsRequiredStage/class[@name="ClaimsRequiredStage"]/method[@name="EvaluateAsync"]/*'/>*/
    public ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        return cancel.IsCancellationRequested
            ? ValueTask.FromCanceled<AccessPolicyDecision>(cancel)
            : ValueTask.FromResult(Evaluate(in context));
    }
}