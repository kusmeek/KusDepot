namespace KusDepot.Security.Policy;

/**<include file='ExpiryStage.xml' path='ExpiryStage/class[@name="ExpiryStage"]/main/*'/>*/
public sealed class ExpiryStage : IAccessPolicyStage , IAsyncAccessPolicyStage
{
    /**<include file='ExpiryStage.xml' path='ExpiryStage/class[@name="ExpiryStage"]/constructor[@name="Constructor"]/*'/>*/
    public ExpiryStage() {}

    /**<include file='ExpiryStage.xml' path='ExpiryStage/class[@name="ExpiryStage"]/property[@name="Instance"]/*'/>*/
    public static ExpiryStage Instance { get; } = new();

    /**<include file='ExpiryStage.xml' path='ExpiryStage/class[@name="ExpiryStage"]/method[@name="Evaluate"]/*'/>*/
    public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
    {
        return context.Expired ? AccessPolicyDecision.Deny(AccessKeyExpired) : AccessPolicyDecision.Allow();
    }

    /**<include file='ExpiryStage.xml' path='ExpiryStage/class[@name="ExpiryStage"]/method[@name="EvaluateAsync"]/*'/>*/
    public ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        return cancel.IsCancellationRequested
            ? ValueTask.FromCanceled<AccessPolicyDecision>(cancel)
            : ValueTask.FromResult(Evaluate(in context));
    }
}