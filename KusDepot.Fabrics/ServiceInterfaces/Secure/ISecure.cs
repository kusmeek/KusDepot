namespace KusDepot.Data;

/**<include file='ISecure.xml' path='ISecure/interface[@name="ISecure"]/main/*'/>*/
public interface ISecure : IActor
{
    /**<include file='ISecure.xml' path='ISecure/interface[@name="ISecure"]/method[@name="IsAdmin"]/*'/>*/
    public Task<Boolean> IsAdmin(String token , String? traceid , String? spanid);

    /**<include file='ISecure.xml' path='ISecure/interface[@name="ISecure"]/method[@name="SetAdmin"]/*'/>*/
    public Task<Boolean> SetAdmin(String token , String tenantid , String clientid , String? traceid , String? spanid);

    /**<include file='ISecure.xml' path='ISecure/interface[@name="ISecure"]/method[@name="ValidateTokenVerifyRole"]/*'/>*/
    public Task<Boolean> ValidateTokenVerifyRole(String token , String role , String tenantid , String clientid , String? traceid , String? spanid);
}