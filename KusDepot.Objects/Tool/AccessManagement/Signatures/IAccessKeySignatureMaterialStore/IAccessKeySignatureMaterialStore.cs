namespace KusDepot.Security.Signatures;

/**<include file='IAccessKeySignatureMaterialStore.xml' path='IAccessKeySignatureMaterialStore/interface[@name="IAccessKeySignatureMaterialStore"]/main/*'/>*/
public interface IAccessKeySignatureMaterialStore : IAccessKeySignatureMaterialResolver
{
    /**<include file='IAccessKeySignatureMaterialStore.xml' path='IAccessKeySignatureMaterialStore/interface[@name="IAccessKeySignatureMaterialStore"]/method[@name="TryAddVerificationMaterial"]/*'/>*/
    Boolean TryAddVerificationMaterial(AccessKeySignatureVerificationMaterial material);

    /**<include file='IAccessKeySignatureMaterialStore.xml' path='IAccessKeySignatureMaterialStore/interface[@name="IAccessKeySignatureMaterialStore"]/method[@name="TryClearCurrentSigningMaterial"]/*'/>*/
    Boolean TryClearCurrentSigningMaterial();

    /**<include file='IAccessKeySignatureMaterialStore.xml' path='IAccessKeySignatureMaterialStore/interface[@name="IAccessKeySignatureMaterialStore"]/method[@name="TryGetCurrentSigningMaterial"]/*'/>*/
    Boolean TryGetCurrentSigningMaterial(out AccessKeySignatureSigningMaterial material);

    /**<include file='IAccessKeySignatureMaterialStore.xml' path='IAccessKeySignatureMaterialStore/interface[@name="IAccessKeySignatureMaterialStore"]/method[@name="TryRemoveVerificationMaterial"]/*'/>*/
    Boolean TryRemoveVerificationMaterial(AccessKeySignerKeyIdFormat signerkeyidformat , ReadOnlyMemory<Byte> signerkeyid);

    /**<include file='IAccessKeySignatureMaterialStore.xml' path='IAccessKeySignatureMaterialStore/interface[@name="IAccessKeySignatureMaterialStore"]/method[@name="TrySetCurrentSigningMaterial"]/*'/>*/
    Boolean TrySetCurrentSigningMaterial(AccessKeySignatureSigningMaterial material);
}