namespace KusDepot.Security.Policy;

/**<include file='ManifestMatchStage.xml' path='ManifestMatchStage/class[@name="ManifestMatchStage"]/main/*'/>*/
public sealed class ManifestMatchStage : IAccessPolicyStage , IAsyncAccessPolicyStage
{
    /**<include file='ManifestMatchStage.xml' path='ManifestMatchStage/class[@name="ManifestMatchStage"]/constructor[@name="Constructor"]/*'/>*/
    public ManifestMatchStage() {}

    /**<include file='ManifestMatchStage.xml' path='ManifestMatchStage/class[@name="ManifestMatchStage"]/property[@name="Instance"]/*'/>*/
    public static ManifestMatchStage Instance { get; } = new();

    /**<include file='ManifestMatchStage.xml' path='ManifestMatchStage/class[@name="ManifestMatchStage"]/method[@name="Evaluate"]/*'/>*/
    public AccessPolicyDecision Evaluate(in AccessPolicyContext context)
    {
        return !context.ManifestMatched ? AccessPolicyDecision.Deny(ManifestIdentityDoesNotMatchCurrentToolState) : AccessPolicyDecision.Allow();
    }

    /**<include file='ManifestMatchStage.xml' path='ManifestMatchStage/class[@name="ManifestMatchStage"]/method[@name="EvaluateAsync"]/*'/>*/
    public ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default)
    {
        return cancel.IsCancellationRequested
            ? ValueTask.FromCanceled<AccessPolicyDecision>(cancel)
            : ValueTask.FromResult(Evaluate(in context));
    }
}