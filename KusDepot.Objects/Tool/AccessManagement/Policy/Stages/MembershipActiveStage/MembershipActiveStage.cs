namespace KusDepot.Security.Policy;

/**<include file='MembershipActiveStage.xml' path='MembershipActiveStage/class[@name="MembershipActiveStage"]/main/*'/>*/
public sealed class MembershipActiveStage : IAccessPolicyStage , IAsyncAccessPolicyStage
{
    /**<include file='MembershipActiveStage.xml' path='MembershipActiveStage/class[@name="MembershipActiveStage"]/constructor[@name="Constructor"]/*'/>*/
    public MembershipActiveStage() {}

    /**<include file='MembershipActiveStage.xml' path='MembershipActiveStage/class[@name="MembershipActiveStage"]/property[@name="Instance"]/*'/>*/
    public static MembershipActiveStage Instance { get; } = new();

    /**<include file='MembershipActiveStage.xml' path='MembershipActiveStage/class[@name="MembershipActiveStage"]/method[@name="Evaluate"]/*'/>*/
    public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
    {
        return !context.MembershipActive ? AccessPolicyDecision.Deny(AccessKeyNotActiveMembership) : AccessPolicyDecision.Allow();
    }

    /**<include file='MembershipActiveStage.xml' path='MembershipActiveStage/class[@name="MembershipActiveStage"]/method[@name="EvaluateAsync"]/*'/>*/
    public ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        return cancel.IsCancellationRequested
            ? ValueTask.FromCanceled<AccessPolicyDecision>(cancel)
            : ValueTask.FromResult(Evaluate(in context));
    }
}