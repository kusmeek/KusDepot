namespace KusDepot.Security;

/**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.MyHostKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "MyHostKey" , Namespace = "KusDepot.Security")]

public sealed record class MyHostKey : AccessKey , IParsable<MyHostKey>
{
    /**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/constructor[@name="ParameterlessConstructor"]/*'/>*/    
    public MyHostKey() {}

    /**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/constructor[@name="Constructor"]/*'/>*/
    public MyHostKey(Byte[] key , Guid? id = null) { ID = id ?? Guid.NewGuid(); Key = key.CloneByteArray(); }

    /**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(MyHostKey? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/method[@name="IParsable{MyHostKey}.Parse"]/*'/>*/
    static MyHostKey IParsable<MyHostKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/method[@name="Parse"]/*'/>*/
    public static new MyHostKey? Parse(String input , IFormatProvider? format = null)
    {
        return SecurityKey.Parse<MyHostKey>(input,format);
    }

    /**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out MyHostKey key)
    {
        return SecurityKey.TryParse(input,format,out key);
    }
}