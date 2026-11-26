namespace KusDepot.Security.Policy;

/**<include file='IAsyncAccessPolicyEngine.xml' path='IAsyncAccessPolicyEngine/interface[@name="IAsyncAccessPolicyEngine"]/main/*'/>*/
public interface IAsyncAccessPolicyEngine : IAccessPolicyEngineMode
{
    /**<include file='IAsyncAccessPolicyEngine.xml' path='IAsyncAccessPolicyEngine/interface[@name="IAsyncAccessPolicyEngine"]/method[@name="EvaluateAsync"]/*'/>*/
    ValueTask<AccessPolicyDecision> EvaluateAsync(AccessPolicyContext context , CancellationToken cancel = default);
}