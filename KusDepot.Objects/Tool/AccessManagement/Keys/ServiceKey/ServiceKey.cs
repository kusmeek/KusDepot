namespace KusDepot;

/**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/main/*'/>*/
[GenerateSerializer] [Alias("ServiceKey")]
[DataContract(Name = "ServiceKey" , Namespace = "KusDepot")]
public class ServiceKey : AccessKey , IParsable<ServiceKey>
{
    /**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/constructor[@name="Constructor"]/*'/>*/
    public ServiceKey(Byte[] key , Guid? id = null) { ID = id is null ? ID : id; Key = key.CloneByteArray(); }

    /**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/method[@name="IParsable{ServiceKey}.Parse"]/*'/>*/
    static ServiceKey IParsable<ServiceKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/method[@name="Parse"]/*'/>*/
    public static new ServiceKey? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ServiceKey)); ServiceKey? _2 = _1.ReadObject(_0) as ServiceKey; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/method[@name="ToServiceKeyWeb"]/*'/>*/
    public ServiceKeyWeb ToServiceKeyWeb() { return new() { ID = this.ID , Key = this.Key.CloneByteArray() }; }

    /**<include file='ServiceKey.xml' path='ServiceKey/class[@name="ServiceKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ServiceKey key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ServiceKey)); ServiceKey? _2 = _1.ReadObject(_0) as ServiceKey; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='ServiceKey.xml' path='ServiceKey/record[@name="ServiceKeyWeb"]/main/*'/>*/
public record ServiceKeyWeb : SecurityKeyWeb
{
    /**<include file='ServiceKey.xml' path='ServiceKey/record[@name="ServiceKeyWeb"]/method[@name="ToServiceKey"]/*'/>*/
    public ServiceKey? ToServiceKey() { return Key is null ? null : new ServiceKey(Key.CloneByteArray(),ID); }

    ///<inheritdoc/>
    public override SecurityKey? ToSecurityKey() => ToServiceKey();
}