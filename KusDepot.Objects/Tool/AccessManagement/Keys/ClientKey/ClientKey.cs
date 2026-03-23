namespace KusDepot.Security;

/**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.ClientKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ClientKey" , Namespace = "KusDepot.Security")]

public sealed record class ClientKey : AccessKey , IParsable<ClientKey>
{
    /**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ClientKey() {}

    /**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/constructor[@name="Constructor"]/*'/>*/
    public ClientKey(Byte[] key , Guid? id = null)
    {
        ID = id ?? Guid.NewGuid(); Key = key.CloneByteArray();
    }

    /**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(ClientKey? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/method[@name="IParsable{ClientKey}.Parse"]/*'/>*/
    static ClientKey IParsable<ClientKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/method[@name="Parse"]/*'/>*/
    public static new ClientKey? Parse(String input , IFormatProvider? format = null)
    {
        return SecurityKey.Parse<ClientKey>(input,format);
    }

    /**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ClientKey key)
    {
        return SecurityKey.TryParse(input,format,out key);
    }
}