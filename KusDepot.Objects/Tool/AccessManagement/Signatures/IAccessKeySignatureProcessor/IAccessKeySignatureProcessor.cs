namespace KusDepot.Security.Signatures;

/**<include file='IAccessKeySignatureProcessor.xml' path='IAccessKeySignatureProcessor/interface[@name="IAccessKeySignatureProcessor"]/main/*'/>*/
public interface IAccessKeySignatureProcessor : IAccessKeySigner
{
    /**<include file='IAccessKeySignatureProcessor.xml' path='IAccessKeySignatureProcessor/interface[@name="IAccessKeySignatureProcessor"]/method[@name="Verify"]/*'/>*/
    SignatureValidation Verify(AccessKeySignatureValidationContext context);
}