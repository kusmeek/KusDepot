namespace KusDepot.Security.Policy;

/**<include file='IssuerMatchStage.xml' path='IssuerMatchStage/class[@name="IssuerMatchStage"]/main/*'/>*/
public sealed class IssuerMatchStage : IAccessPolicyStage , IAsyncAccessPolicyStage
{
    /**<include file='IssuerMatchStage.xml' path='IssuerMatchStage/class[@name="IssuerMatchStage"]/constructor[@name="Constructor"]/*'/>*/
    public IssuerMatchStage() {}

    /**<include file='IssuerMatchStage.xml' path='IssuerMatchStage/class[@name="IssuerMatchStage"]/property[@name="Instance"]/*'/>*/
    public static IssuerMatchStage Instance { get; } = new();

    /**<include file='IssuerMatchStage.xml' path='IssuerMatchStage/class[@name="IssuerMatchStage"]/method[@name="Evaluate"]/*'/>*/
    public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
    {
        return context.Claims?.IssuerAccessOnly is true && (!context.IssuerAccessManagerMatched || !context.IssuerToolMatched)
            ? AccessPolicyDecision.Deny(IssuerIdentityDoesNotMatchCurrentTool)
            : AccessPolicyDecision.Allow();
    }

    /**<include file='IssuerMatchStage.xml' path='IssuerMatchStage/class[@name="IssuerMatchStage"]/method[@name="EvaluateAsync"]/*'/>*/
    public ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        return cancel.IsCancellationRequested
            ? ValueTask.FromCanceled<AccessPolicyDecision>(cancel)
            : ValueTask.FromResult(Evaluate(in context));
    }
}
