namespace KusDepot;

/**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/main/*'/>*/
[GenerateSerializer] [Alias("MyHostKey")]
[DataContract(Name = "MyHostKey" , Namespace = "KusDepot")]
public class MyHostKey : AccessKey , IParsable<MyHostKey>
{
    /**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/constructor[@name="Constructor"]/*'/>*/
    public MyHostKey(Byte[] key , Guid? id = null) { ID = id is null ? ID : id; Key = key.CloneByteArray(); }

    /**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/method[@name="IParsable{MyHostKey}.Parse"]/*'/>*/
    static MyHostKey IParsable<MyHostKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/method[@name="Parse"]/*'/>*/
    public static new MyHostKey? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(MyHostKey)); MyHostKey? _2 = _1.ReadObject(_0) as MyHostKey; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='MyHostKey.xml' path='MyHostKey/class[@name="MyHostKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out MyHostKey key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(MyHostKey)); MyHostKey? _2 = _1.ReadObject(_0) as MyHostKey; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}