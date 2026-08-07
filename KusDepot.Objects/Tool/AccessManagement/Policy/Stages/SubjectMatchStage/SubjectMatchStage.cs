namespace KusDepot.Security.Policy;

/**<include file='SubjectMatchStage.xml' path='SubjectMatchStage/class[@name="SubjectMatchStage"]/main/*'/>*/
public sealed class SubjectMatchStage : IAccessPolicyStage , IAsyncAccessPolicyStage
{
    /**<include file='SubjectMatchStage.xml' path='SubjectMatchStage/class[@name="SubjectMatchStage"]/field[@name="Subject"]/*'/>*/
    private readonly String Subject;

    /**<include file='SubjectMatchStage.xml' path='SubjectMatchStage/class[@name="SubjectMatchStage"]/constructor[@name="Constructor"]/*'/>*/
    public SubjectMatchStage(String subject)
    {
        if(String.IsNullOrWhiteSpace(subject)) { throw new ArgumentException(SubjectIsRequired,nameof(subject)); }

        this.Subject = subject;
    }

    /**<include file='SubjectMatchStage.xml' path='SubjectMatchStage/class[@name="SubjectMatchStage"]/method[@name="Evaluate"]/*'/>*/
    public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
    {
        if(String.IsNullOrWhiteSpace(context.Subject)) { return AccessPolicyDecision.Deny(SubjectIsRequired); }

        return String.Equals(context.Subject,this.Subject,Ordinal)
            ? AccessPolicyDecision.Allow() : AccessPolicyDecision.Deny(SubjectDoesNotMatchRequiredSubject);
    }

    /**<include file='SubjectMatchStage.xml' path='SubjectMatchStage/class[@name="SubjectMatchStage"]/method[@name="EvaluateAsync"]/*'/>*/
    public ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        return cancel.IsCancellationRequested
            ? ValueTask.FromCanceled<AccessPolicyDecision>(cancel)
            : ValueTask.FromResult(Evaluate(in context));
    }
}