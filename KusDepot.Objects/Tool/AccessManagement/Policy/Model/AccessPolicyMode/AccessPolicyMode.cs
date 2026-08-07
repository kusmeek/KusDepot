namespace KusDepot.Security.Policy;

/**<include file='AccessPolicyMode.xml' path='AccessPolicyMode/enum[@name="AccessPolicyMode"]/main/*'/>*/
public enum AccessPolicyMode
{
    /**<include file='AccessPolicyMode.xml' path='AccessPolicyMode/enum[@name="AccessPolicyMode"]/value[@name="Access"]/*'/>*/
    Access = 0,

    /**<include file='AccessPolicyMode.xml' path='AccessPolicyMode/enum[@name="AccessPolicyMode"]/value[@name="Authorize"]/*'/>*/
    Authorize = 1,

    /**<include file='AccessPolicyMode.xml' path='AccessPolicyMode/enum[@name="AccessPolicyMode"]/value[@name="Issue"]/*'/>*/
    Issue = 2,

    /**<include file='AccessPolicyMode.xml' path='AccessPolicyMode/enum[@name="AccessPolicyMode"]/value[@name="Revoke"]/*'/>*/
    Revoke = 3,

    /**<include file='AccessPolicyMode.xml' path='AccessPolicyMode/enum[@name="AccessPolicyMode"]/value[@name="Request"]/*'/>*/
    Request = 4
}