namespace KusDepot.Security;

/**<include file='ISecurityIdentityDirectory.xml' path='ISecurityIdentityDirectory/interface[@name="ISecurityIdentityDirectory"]/main/*'/>*/
public interface ISecurityIdentityDirectory
{
    /**<include file='ISecurityIdentityDirectory.xml' path='ISecurityIdentityDirectory/interface[@name="ISecurityIdentityDirectory"]/method[@name="ResolveIdentityObjectId"]/*'/>*/
    ValueTask<DataSecurityObject?> ResolveIdentity(Guid objectid , CancellationToken cancel = default);

    /**<include file='ISecurityIdentityDirectory.xml' path='ISecurityIdentityDirectory/interface[@name="ISecurityIdentityDirectory"]/method[@name="ResolveIdentityThumbprint"]/*'/>*/
    ValueTask<DataSecurityObject?> ResolveIdentity(String thumbprint , CancellationToken cancel = default);
}