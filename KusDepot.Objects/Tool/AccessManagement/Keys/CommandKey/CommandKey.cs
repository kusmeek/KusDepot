namespace KusDepot.Security;

/**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.CommandKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "CommandKey" , Namespace = "KusDepot.Security")]

public sealed record class CommandKey : AccessKey , IParsable<CommandKey>
{
    /**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/constructor[@name="ParameterlessConstructor"]/*'/>*/    
    public CommandKey() {}

    /**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/constructor[@name="Constructor"]/*'/>*/
    public CommandKey(Byte[] key , Guid? id = null)
    {
        ID = id ?? Guid.NewGuid(); Key = key.CloneByteArray();
    }

    /**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(CommandKey? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/method[@name="IParsable{CommandKey}.Parse"]/*'/>*/
    static CommandKey IParsable<CommandKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/method[@name="Parse"]/*'/>*/
    public static new CommandKey? Parse(String input , IFormatProvider? format = null)
    {
        return SecurityKey.Parse<CommandKey>(input,format);
    }

    /**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out CommandKey key)
    {
        return SecurityKey.TryParse(input,format,out key);
    }
}