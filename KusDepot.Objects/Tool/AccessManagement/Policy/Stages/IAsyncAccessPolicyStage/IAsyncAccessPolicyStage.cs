namespace KusDepot.Security.Policy;

/**<include file='IAsyncAccessPolicyStage.xml' path='IAsyncAccessPolicyStage/interface[@name="IAsyncAccessPolicyStage"]/main/*'/>*/
public interface IAsyncAccessPolicyStage : IAccessPolicyStageMode
{
    /**<include file='IAsyncAccessPolicyStage.xml' path='IAsyncAccessPolicyStage/interface[@name="IAsyncAccessPolicyStage"]/method[@name="EvaluateAsync"]/*'/>*/
    ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default);
}