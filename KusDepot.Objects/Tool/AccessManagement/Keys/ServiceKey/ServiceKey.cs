namespace KusDepot.Security;

/**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.ServiceKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ServiceKey" , Namespace = "KusDepot.Security")]

public sealed record class ServiceKey : AccessKey , IParsable<ServiceKey>
{
    /**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/constructor[@name="ParameterlessConstructor"]/*'/>*/    
    public ServiceKey() {}

    /**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/constructor[@name="Constructor"]/*'/>*/
    public ServiceKey(Byte[] key , Guid? id = null)
    {
        ID = id ?? Guid.NewGuid(); Key = key.CloneByteArray();
    }

    /**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(ServiceKey? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/method[@name="IParsable{ServiceKey}.Parse"]/*'/>*/
    static ServiceKey IParsable<ServiceKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/method[@name="Parse"]/*'/>*/
    public static new ServiceKey? Parse(String input , IFormatProvider? format = null)
    {
        return SecurityKey.Parse<ServiceKey>(input,format);
    }

    /**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ServiceKey key)
    {
        return SecurityKey.TryParse(input,format,out key);
    }
}