namespace KusDepot;

/**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/main/*'/>*/
[GenerateSerializer] [Alias("ClientKey")]
[DataContract(Name = "ClientKey" , Namespace = "KusDepot")]
public class ClientKey : AccessKey , IParsable<ClientKey>
{
    /**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/constructor[@name="Constructor"]/*'/>*/
    public ClientKey(Byte[] key , Guid? id = null) { ID = id is null ? ID : id; Key = key.CloneByteArray(); }

    /**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/method[@name="IParsable{ClientKey}.Parse"]/*'/>*/
    static ClientKey IParsable<ClientKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/method[@name="Parse"]/*'/>*/
    public static new ClientKey? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ClientKey)); ClientKey? _2 = _1.ReadObject(_0) as ClientKey; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/method[@name="ToClientKeyWeb"]/*'/>*/
    public ClientKeyWeb ToClientKeyWeb() { return new() { ID = this.ID , Key = this.Key.CloneByteArray() }; }

    /**<include file='ClientKey.xml' path='ClientKey/class[@name="ClientKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ClientKey key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ClientKey)); ClientKey? _2 = _1.ReadObject(_0) as ClientKey; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='ClientKey.xml' path='ClientKey/record[@name="ClientKeyWeb"]/main/*'/>*/
public record ClientKeyWeb : SecurityKeyWeb
{
    /**<include file='ClientKey.xml' path='ClientKey/record[@name="ClientKeyWeb"]/method[@name="ToClientKey"]/*'/>*/
    public ClientKey? ToClientKey() { return Key is null ? null : new ClientKey(Key.CloneByteArray(),ID); }

    ///<inheritdoc/>
    public override SecurityKey? ToSecurityKey() => ToClientKey();
}