namespace KusDepot.Security.Policy;

/**<include file='OperationAllowedStage.xml' path='OperationAllowedStage/class[@name="OperationAllowedStage"]/main/*'/>*/
public sealed class OperationAllowedStage : IAccessPolicyStage , IAsyncAccessPolicyStage
{
    /**<include file='OperationAllowedStage.xml' path='OperationAllowedStage/class[@name="OperationAllowedStage"]/constructor[@name="Constructor"]/*'/>*/
    public OperationAllowedStage() {}

    /**<include file='OperationAllowedStage.xml' path='OperationAllowedStage/class[@name="OperationAllowedStage"]/property[@name="Instance"]/*'/>*/
    public static OperationAllowedStage Instance { get; } = new();

    /**<include file='OperationAllowedStage.xml' path='OperationAllowedStage/class[@name="OperationAllowedStage"]/method[@name="Evaluate"]/*'/>*/
    public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
    {
        return !context.OperationAllowed ? AccessPolicyDecision.Deny(ProtectedOperationIsNotAllowed) : AccessPolicyDecision.Allow();
    }

    /**<include file='OperationAllowedStage.xml' path='OperationAllowedStage/class[@name="OperationAllowedStage"]/method[@name="EvaluateAsync"]/*'/>*/
    public ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        return cancel.IsCancellationRequested
            ? ValueTask.FromCanceled<AccessPolicyDecision>(cancel)
            : ValueTask.FromResult(Evaluate(in context));
    }
}