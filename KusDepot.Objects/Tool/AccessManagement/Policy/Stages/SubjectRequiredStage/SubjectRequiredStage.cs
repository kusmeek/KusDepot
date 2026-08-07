namespace KusDepot.Security.Policy;

/**<include file='SubjectRequiredStage.xml' path='SubjectRequiredStage/class[@name="SubjectRequiredStage"]/main/*'/>*/
public sealed class SubjectRequiredStage : IAccessPolicyStage , IAsyncAccessPolicyStage
{
    /**<include file='SubjectRequiredStage.xml' path='SubjectRequiredStage/class[@name="SubjectRequiredStage"]/constructor[@name="Constructor"]/*'/>*/
    public SubjectRequiredStage() {}

    /**<include file='SubjectRequiredStage.xml' path='SubjectRequiredStage/class[@name="SubjectRequiredStage"]/property[@name="Instance"]/*'/>*/
    public static SubjectRequiredStage Instance { get; } = new();

    /**<include file='SubjectRequiredStage.xml' path='SubjectRequiredStage/class[@name="SubjectRequiredStage"]/method[@name="Evaluate"]/*'/>*/
    public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
    {
        return String.IsNullOrWhiteSpace(context.Subject) ? AccessPolicyDecision.Deny(SubjectIsRequired) : AccessPolicyDecision.Allow();
    }

    /**<include file='SubjectRequiredStage.xml' path='SubjectRequiredStage/class[@name="SubjectRequiredStage"]/method[@name="EvaluateAsync"]/*'/>*/
    public ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        return cancel.IsCancellationRequested
            ? ValueTask.FromCanceled<AccessPolicyDecision>(cancel)
            : ValueTask.FromResult(Evaluate(in context));
    }
}