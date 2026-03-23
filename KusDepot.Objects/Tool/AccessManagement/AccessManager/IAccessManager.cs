namespace KusDepot.Security;

/**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/main/*'/>*/
public interface IAccessManager
{
    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="AccessCheck"]/*'/>*/
    Boolean AccessCheck(AccessKey? key = null , String? operationname = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="DestroySecrets"]/*'/>*/
    Boolean DestroySecrets(AccessKey? key = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="ExportAccessManagerState"]/*'/>*/
    AccessManagerState? ExportAccessManagerState(AccessKey? key = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="GenerateAccessKey"]/*'/>*/
    TKey? GenerateAccessKey<TKey>(String? subject , ImmutableArray<Int32>? operationset = null , AccessKey? key = null) where TKey : AccessKey;

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="ImportAccessManagementKeys"]/*'/>*/
    Boolean ImportAccessManagementKeys(AccessManagerState? state , AccessKey? key = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="Initialize"]/*'/>*/
    Boolean Initialize(ITool? tool = null , ILogger? logger = null , X509Certificate2? certificate = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="RequestAccess"]/*'/>*/
    AccessKey? RequestAccess(AccessRequest? request = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="RevokeAccess"]/*'/>*/
    Boolean RevokeAccess(AccessKey? key = null);
}