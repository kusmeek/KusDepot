namespace KusDepot.Security;

/**<include file='KeyMaterial.xml' path='KeyMaterial/struct[@name="KeyMaterial"]/main/*'/>*/
public readonly struct KeyMaterial
{
    /**<include file='KeyMaterial.xml' path='KeyMaterial/struct[@name="KeyMaterial"]/property[@name="Certificate"]/*'/>*/
    public X509Certificate2? Certificate { get; init; }

    /**<include file='KeyMaterial.xml' path='KeyMaterial/struct[@name="KeyMaterial"]/property[@name="ID"]/*'/>*/
    public Guid? ID { get; init; }

    /**<include file='KeyMaterial.xml' path='KeyMaterial/struct[@name="KeyMaterial"]/property[@name="Key"]/*'/>*/
    public Byte[]? Key { get; init; }

    /**<include file='KeyMaterial.xml' path='KeyMaterial/struct[@name="KeyMaterial"]/constructor[@name="Constructor"]/*'/>*/
    public KeyMaterial(Guid? id = null , Byte[]? key = null , X509Certificate2? certificate = null)
    {
        this.ID = id;
        this.Key = key;
        this.Certificate = certificate;
    }
}