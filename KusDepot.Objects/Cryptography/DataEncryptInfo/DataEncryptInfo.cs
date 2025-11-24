namespace KusDepot;

/**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.DataEncryptInfo")]
[DataContract(Name = "DataEncryptInfo" , Namespace = "KusDepot")]
public sealed class DataEncryptInfo
{
    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/field[@name="managementkeyid"]/*'/>*/
    [NonSerialized]
    private Guid?   managementkeyid;

    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/field[@name="publickey"]/*'/>*/
    [NonSerialized]
    private String? publickey;

    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/field[@name="serial"]/*'/>*/
    [NonSerialized]
    private String? serial;

    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/field[@name="thumbprint"]/*'/>*/
    [NonSerialized]
    private String? thumbprint;

    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/property[@name="ManagementKeyID"]/*'/>*/
    [DataMember(Name = "ManagementKeyID" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public Guid?   ManagementKeyID { get { return this.managementkeyid; } set { this.managementkeyid ??= value; } }

    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/property[@name="PublicKey"]/*'/>*/
    [DataMember(Name = "PublicKey" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    public String? PublicKey       { get { return this.publickey;       } set { this.publickey ??= value; } }

    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/property[@name="Serial"]/*'/>*/
    [DataMember(Name = "Serial" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    public String? Serial          { get { return this.serial;          } set { this.serial ??= value; } }

    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/property[@name="ThumbPrint"]/*'/>*/
    [DataMember(Name = "ThumbPrint" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    public String? ThumbPrint      { get { return this.thumbprint;      } set { this.thumbprint ??= value; } }

    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/constructor[@name="Constructor"]/*'/>*/
    public DataEncryptInfo(Guid? managementkeyid = null , String? publickey = null , String? serial = null , String? thumbprint = null)
    {
        this.managementkeyid = managementkeyid; this.publickey = publickey; this.serial = serial; this.thumbprint = thumbprint;
    }

    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/method[@name="Clone"]/*'/>*/
    public DataEncryptInfo Clone() { return new DataEncryptInfo(this.managementkeyid,this.publickey,this.serial,this.thumbprint); }

    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/constructor[@name="ConstructorKey"]/*'/>*/
    public DataEncryptInfo(ManagementKey key)
    {
        var _ = DeserializeCertificate(key.Key); if(_ is null) { return; }

        this.managementkeyid = key.ID; this.publickey = _.GetPublicKey().ToBase64FromByteArray(); this.serial = _.SerialNumber; this.thumbprint = _.Thumbprint;
    }

    /**<include file='DataEncryptInfo.xml' path='DataEncryptInfo/class[@name="DataEncryptInfo"]/constructor[@name="ConstructorCertificate"]/*'/>*/
    public DataEncryptInfo(X509Certificate2 certificate) : this(null,certificate?.GetPublicKey().ToBase64FromByteArray(),certificate?.SerialNumber,certificate?.Thumbprint) {}
}