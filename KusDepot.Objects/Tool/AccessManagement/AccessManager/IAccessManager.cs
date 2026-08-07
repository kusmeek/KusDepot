namespace KusDepot.Security;

/**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/main/*'/>*/
public interface IAccessManager
{
    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="AccessCheckIndex"]/*'/>*/
    Boolean AccessCheck(AccessKey? key , Int32 operation);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="AccessCheckIndexAsync"]/*'/>*/
    Task<Boolean> AccessCheckAsync(AccessKey? key , Int32 operation , CancellationToken cancel = default);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="AccessCheck"]/*'/>*/
    Boolean AccessCheck(AccessKey? key = null , String? operationname = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="AccessCheckAsync"]/*'/>*/
    Task<Boolean> AccessCheckAsync(AccessKey? key = null , String? operationname = null , CancellationToken cancel = default);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="Authorize"]/*'/>*/
    Boolean Authorize(AccessKey? key = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="AuthorizeAsync"]/*'/>*/
    Task<Boolean> AuthorizeAsync(AccessKey? key = null , CancellationToken cancel = default);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="DestroySecrets"]/*'/>*/
    Boolean DestroySecrets(AccessKey? key = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="GenerateAccessKey"]/*'/>*/
    TKey? GenerateAccessKey<TKey>(AccessKeyIssueOptions? options = null , AccessKey? key = null) where TKey : AccessKey;

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="GenerateAccessKeyAsync"]/*'/>*/
    Task<TKey?> GenerateAccessKeyAsync<TKey>(AccessKeyIssueOptions? options = null , AccessKey? key = null , CancellationToken cancel = default) where TKey : AccessKey;

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="Initialize"]/*'/>*/
    Boolean Initialize(ITool? tool = null , ILogger? logger = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="QueryMembershipCatalog"]/*'/>*/
    AccessManagerMembershipCatalog? QueryMembershipCatalog(AccessManagerQuery? query = null , AccessKey? key = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="QuerySnapshot"]/*'/>*/
    AccessManagerSnapshot? QuerySnapshot(AccessManagerQuery? query = null , AccessKey? key = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="RequestAccess"]/*'/>*/
    AccessKey? RequestAccess(AccessRequest? request = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="RequestAccessAsync"]/*'/>*/
    Task<AccessKey?> RequestAccessAsync(AccessRequest? request = null , CancellationToken cancel = default);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="RevokeAccess"]/*'/>*/
    Boolean RevokeAccess(AccessKey? key = null);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="RevokeAccessAsync"]/*'/>*/
    Task<Boolean> RevokeAccessAsync(AccessKey? key = null , CancellationToken cancel = default);

    /**<include file='IAccessManager.xml' path='IAccessManager/interface[@name="IAccessManager"]/method[@name="UpdateMembershipCatalog"]/*'/>*/
    Boolean UpdateMembershipCatalog(AccessManagerMembershipCatalog? memberships , MembershipUpdateOperation operation , AccessKey? key = null);
}