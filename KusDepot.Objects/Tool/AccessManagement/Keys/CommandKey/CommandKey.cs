namespace KusDepot;

/**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/main/*'/>*/
[GenerateSerializer] [Alias("CommandKey")]
[DataContract(Name = "CommandKey" , Namespace = "KusDepot")]
public class CommandKey : AccessKey , IParsable<CommandKey>
{
    /**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/constructor[@name="Constructor"]/*'/>*/
    public CommandKey(Byte[] key , Guid? id = null) { ID = id is null ? ID : id; Key = key.CloneByteArray(); }

    /**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/method[@name="IParsable{CommandKey}.Parse"]/*'/>*/
    static CommandKey IParsable<CommandKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/method[@name="Parse"]/*'/>*/
    public static new CommandKey? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(CommandKey)); CommandKey? _2 = _1.ReadObject(_0) as CommandKey; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/method[@name="ToCommandKeyWeb"]/*'/>*/
    public CommandKeyWeb ToCommandKeyWeb() { return new() { ID = this.ID , Key = this.Key.CloneByteArray() }; }

    /**<include file='CommandKey.xml' path='CommandKey/class[@name="CommandKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out CommandKey key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(CommandKey)); CommandKey? _2 = _1.ReadObject(_0) as CommandKey; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='CommandKey.xml' path='CommandKey/record[@name="CommandKeyWeb"]/main/*'/>*/
public record CommandKeyWeb : SecurityKeyWeb
{
    /**<include file='CommandKey.xml' path='CommandKey/record[@name="CommandKeyWeb"]/method[@name="ToCommandKey"]/*'/>*/
    public CommandKey? ToCommandKey() { return Key is null ? null : new CommandKey(Key.CloneByteArray(),ID); }

    ///<inheritdoc/>
    public override SecurityKey? ToSecurityKey() => ToCommandKey();
}