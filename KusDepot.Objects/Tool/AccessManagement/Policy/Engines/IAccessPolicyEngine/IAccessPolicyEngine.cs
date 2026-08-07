namespace KusDepot.Security.Policy;

/**<include file='IAccessPolicyEngine.xml' path='IAccessPolicyEngine/interface[@name="IAccessPolicyEngine"]/main/*'/>*/
public interface IAccessPolicyEngine : IAccessPolicyEngineMode
{
    /**<include file='IAccessPolicyEngine.xml' path='IAccessPolicyEngine/interface[@name="IAccessPolicyEngine"]/method[@name="Evaluate"]/*'/>*/
    AccessPolicyDecision Evaluate(in AccessPolicyContext context);
}