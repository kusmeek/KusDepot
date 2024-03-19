namespace KusDepot;

/**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/main/*'/>*/
[DataContract(Name = "DataItem" , Namespace = "KusDepot")]
[KnownType(typeof(TextItem))]
[KnownType(typeof(CodeItem))]
[KnownType(typeof(BinaryItem))]
[KnownType(typeof(MSBuildItem))]
[KnownType(typeof(GenericItem))]
[KnownType(typeof(MultiMediaItem))]
[KnownType(typeof(GuidReferenceItem))]
public abstract class DataItem : Common , ICloneable
{
    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="SecureHashes"]/*'/>*/
    [DataMember(Name = "SecureHashes" , EmitDefaultValue = true , IsRequired = true)]
    protected Dictionary<String,Byte[]>? SecureHashes;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/field[@name="Type"]/*'/>*/
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)]
    protected String? Type;

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Clone"]/*'/>*/
    public abstract Object Clone();

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetSecureHashes"]/*'/>*/
    public abstract Dictionary<String,Byte[]>? GetSecureHashes();

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetType"]/*'/>*/
    public new abstract String? GetType();

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="SetSecureHashes"]/*'/>*/
    public abstract Boolean SetSecureHashes(IDictionary<String,Byte[]>? securehashes);

    /**<include file='DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="SetType"]/*'/>*/
    public abstract Boolean SetType(String? type);
}