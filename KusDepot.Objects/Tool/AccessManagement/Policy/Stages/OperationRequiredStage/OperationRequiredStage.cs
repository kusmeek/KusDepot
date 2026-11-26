namespace KusDepot.Security.Policy;

/**<include file='OperationRequiredStage.xml' path='OperationRequiredStage/class[@name="OperationRequiredStage"]/main/*'/>*/
public sealed class OperationRequiredStage : IAccessPolicyStage , IAsyncAccessPolicyStage
{
    /**<include file='OperationRequiredStage.xml' path='OperationRequiredStage/class[@name="OperationRequiredStage"]/constructor[@name="Constructor"]/*'/>*/
    public OperationRequiredStage() {}

    /**<include file='OperationRequiredStage.xml' path='OperationRequiredStage/class[@name="OperationRequiredStage"]/property[@name="Instance"]/*'/>*/
    public static OperationRequiredStage Instance { get; } = new();

    /**<include file='OperationRequiredStage.xml' path='OperationRequiredStage/class[@name="OperationRequiredStage"]/method[@name="Evaluate"]/*'/>*/
    public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
    {
        return context.Operation.HasValue is false ? AccessPolicyDecision.Deny(ProtectedOperationIsRequired) : AccessPolicyDecision.Allow();
    }

    /**<include file='OperationRequiredStage.xml' path='OperationRequiredStage/class[@name="OperationRequiredStage"]/method[@name="EvaluateAsync"]/*'/>*/
    public ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        return cancel.IsCancellationRequested
            ? ValueTask.FromCanceled<AccessPolicyDecision>(cancel)
            : ValueTask.FromResult(Evaluate(in context));
    }
}