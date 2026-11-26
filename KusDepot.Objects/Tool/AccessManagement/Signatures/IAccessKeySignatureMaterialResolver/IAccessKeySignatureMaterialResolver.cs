namespace KusDepot.Security.Signatures;

/**<include file='IAccessKeySignatureMaterialResolver.xml' path='IAccessKeySignatureMaterialResolver/interface[@name="IAccessKeySignatureMaterialResolver"]/main/*'/>*/
public interface IAccessKeySignatureMaterialResolver
{
    /**<include file='IAccessKeySignatureMaterialResolver.xml' path='IAccessKeySignatureMaterialResolver/interface[@name="IAccessKeySignatureMaterialResolver"]/method[@name="TryResolveSigningMaterial"]/*'/>*/
    Boolean TryResolveSigningMaterial(AccessKeySignatureSigningContext context , out AccessKeySignatureSigningMaterial material);

    /**<include file='IAccessKeySignatureMaterialResolver.xml' path='IAccessKeySignatureMaterialResolver/interface[@name="IAccessKeySignatureMaterialResolver"]/method[@name="TryResolveVerificationMaterial"]/*'/>*/
    Boolean TryResolveVerificationMaterial(AccessKeySignatureValidationContext context , out AccessKeySignatureVerificationMaterial material);
}