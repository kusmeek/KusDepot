namespace KusDepot.Security.Signatures;

/**<include file='IAccessKeySigner.xml' path='IAccessKeySigner/interface[@name="IAccessKeySigner"]/main/*'/>*/
public interface IAccessKeySigner : IAccessKeySignatureProcessorMode
{
    /**<include file='IAccessKeySigner.xml' path='IAccessKeySigner/interface[@name="IAccessKeySigner"]/method[@name="GetSigningInfo"]/*'/>*/
    AccessKeySignatureInfo GetSigningInfo(AccessKeySignatureSigningContext context);

    /**<include file='IAccessKeySigner.xml' path='IAccessKeySigner/interface[@name="IAccessKeySigner"]/method[@name="TrySign"]/*'/>*/
    Boolean TrySign(AccessKeySignatureSigningContext context , Span<Byte> signature , out Int32 written);
}