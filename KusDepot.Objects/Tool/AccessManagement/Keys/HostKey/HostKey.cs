namespace KusDepot;

/**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/main/*'/>*/
[GenerateSerializer] [Alias("HostKey")]
[DataContract(Name = "HostKey" , Namespace = "KusDepot")]
public class HostKey : AccessKey , IParsable<HostKey>
{
    /**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/constructor[@name="Constructor"]/*'/>*/
    public HostKey(Byte[] key , Guid? id = null) { ID = id is null ? ID : id; Key = key.CloneByteArray(); }

    /**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/method[@name="IParsable{HostKey}.Parse"]/*'/>*/
    static HostKey IParsable<HostKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/method[@name="Parse"]/*'/>*/
    public static new HostKey? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(HostKey)); HostKey? _2 = _1.ReadObject(_0) as HostKey; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='HostKey.xml' path='HostKey/class[@name="HostKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out HostKey key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(HostKey)); HostKey? _2 = _1.ReadObject(_0) as HostKey; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}