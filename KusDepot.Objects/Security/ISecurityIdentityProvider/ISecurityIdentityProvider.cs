namespace KusDepot.Security;

/**<include file='ISecurityIdentityProvider.xml' path='ISecurityIdentityProvider/interface[@name="ISecurityIdentityProvider"]/main/*'/>*/
public interface ISecurityIdentityProvider
{
    /**<include file='ISecurityIdentityProvider.xml' path='ISecurityIdentityProvider/interface[@name="ISecurityIdentityProvider"]/method[@name="TryGetLocalIdentity"]/*'/>*/
    Boolean TryGetLocalIdentity([NotNullWhen(true)] out DataSecurityObject? identity);

    /**<include file='ISecurityIdentityProvider.xml' path='ISecurityIdentityProvider/interface[@name="ISecurityIdentityProvider"]/method[@name="TryGetSigningIdentity"]/*'/>*/
    Boolean TryGetSigningIdentity([NotNullWhen(true)] out DataSecurityObject? identity);
}