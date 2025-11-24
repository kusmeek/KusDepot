namespace KusDepot;

/**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/main/*'/>*/
[GenerateSerializer] [Alias("ExecutiveKey")]
[DataContract(Name = "ExecutiveKey" , Namespace = "KusDepot")]
public class ExecutiveKey : AccessKey , IParsable<ExecutiveKey>
{
    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/constructor[@name="Constructor"]/*'/>*/
    public ExecutiveKey(Byte[] key , Guid? id = null) { ID = id is null ? ID : id; Key = key.CloneByteArray(); }

    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/method[@name="IParsable{ExecutiveKey}.Parse"]/*'/>*/
    static ExecutiveKey IParsable<ExecutiveKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/method[@name="Parse"]/*'/>*/
    public static new ExecutiveKey? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ExecutiveKey)); ExecutiveKey? _2 = _1.ReadObject(_0) as ExecutiveKey; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/method[@name="ToExecutiveKeyWeb"]/*'/>*/
    public ExecutiveKeyWeb ToExecutiveKeyWeb() { return new() { ID = this.ID , Key = this.Key.CloneByteArray() }; }

    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/class[@name="ExecutiveKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ExecutiveKey key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ExecutiveKey)); ExecutiveKey? _2 = _1.ReadObject(_0) as ExecutiveKey; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='ExecutiveKey.xml' path='ExecutiveKey/record[@name="ExecutiveKeyWeb"]/main/*'/>*/
public record ExecutiveKeyWeb : SecurityKeyWeb
{
    /**<include file='ExecutiveKey.xml' path='ExecutiveKey/record[@name="ExecutiveKeyWeb"]/method[@name="ToExecutiveKey"]/*'/>*/
    public ExecutiveKey? ToExecutiveKey() { return Key is null ? null : new ExecutiveKey(Key.CloneByteArray(),ID); }

    ///<inheritdoc/>
    public override SecurityKey? ToSecurityKey() => ToExecutiveKey();
}