namespace KusDepot.Security;

/**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.HostKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "HostKey" , Namespace = "KusDepot.Security")]

public sealed record class HostKey : AccessKey , IParsable<HostKey>
{
    /**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/constructor[@name="ParameterlessConstructor"]/*'/>*/    
    public HostKey() {}

    /**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/constructor[@name="Constructor"]/*'/>*/
    public HostKey(Byte[] key , Guid? id = null) { ID = id ?? Guid.NewGuid(); Key = key.CloneByteArray(); }

    /**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(HostKey? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/method[@name="IParsable{HostKey}.Parse"]/*'/>*/
    static HostKey IParsable<HostKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/method[@name="Parse"]/*'/>*/
    public static new HostKey? Parse(String input , IFormatProvider? format = null)
    {
        return SecurityKey.Parse<HostKey>(input,format);
    }

    /**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out HostKey key)
    {
        return SecurityKey.TryParse(input,format,out key);
    }
}