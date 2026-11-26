namespace KusDepot.Security.Policy;

/**<include file='IAccessPolicyStage.xml' path='IAccessPolicyStage/interface[@name="IAccessPolicyStage"]/main/*'/>*/
public interface IAccessPolicyStage : IAccessPolicyStageMode
{
    /**<include file='IAccessPolicyStage.xml' path='IAccessPolicyStage/interface[@name="IAccessPolicyStage"]/method[@name="Evaluate"]/*'/>*/
    AccessPolicyDecision Evaluate(in AccessPolicyContext context);
}