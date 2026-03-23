namespace KusDepot.Security;

/**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.ExecutiveKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ExecutiveKey" , Namespace = "KusDepot.Security")]

public sealed record class ExecutiveKey : AccessKey , IParsable<ExecutiveKey>
{    
    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/constructor[@name="ParameterlessConstructor"]/*'/>*/    
    public ExecutiveKey() {}

    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/constructor[@name="Constructor"]/*'/>*/
    public ExecutiveKey(Byte[] key , Guid? id = null)
    {
        ID = id ?? Guid.NewGuid(); Key = key.CloneByteArray();
    }

    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(ExecutiveKey? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/method[@name="IParsable{ExecutiveKey}.Parse"]/*'/>*/
    static ExecutiveKey IParsable<ExecutiveKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/method[@name="Parse"]/*'/>*/
    public static new ExecutiveKey? Parse(String input , IFormatProvider? format = null)
    {
        return SecurityKey.Parse<ExecutiveKey>(input,format);
    }

    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ExecutiveKey key)
    {
        return SecurityKey.TryParse(input,format,out key);
    }
}