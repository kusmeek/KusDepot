namespace KusDepot;

/**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/main/*'/>*/
[GenerateSerializer] [Alias("ManagerKey")]
[DataContract(Name = "ManagerKey" , Namespace = "KusDepot")]
public class ManagerKey : ManagementKey , IParsable<ManagerKey>
{
    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/constructor[@name="Constructor"]/*'/>*/
    public ManagerKey(Byte[] certificate , Guid? id = null , String? thumbprint = null)
    {
        ID = id is null ? ID : id; Key = certificate.CloneByteArray();

        Thumbprint = thumbprint is not null ? new(thumbprint) : DeserializeCertificate(certificate)?.Thumbprint;
    }

    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/constructor[@name="ConstructorCertificate"]/*'/>*/
    public ManagerKey(X509Certificate2 certificate , Guid? id = null) : this(SerializeCertificate(certificate)!,id,certificate.Thumbprint) {}

    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/method[@name="IParsable{ManagerKey}.Parse"]/*'/>*/
    static ManagerKey IParsable<ManagerKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/method[@name="Parse"]/*'/>*/
    public static new ManagerKey? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ManagerKey)); ManagerKey? _2 = _1.ReadObject(_0) as ManagerKey; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/method[@name="ToManagerKeyWeb"]/*'/>*/
    public ManagerKeyWeb ToManagerKeyWeb() { return new() { ID = this.ID , Key = this.Key.CloneByteArray() , Thumbprint = this.Thumbprint }; }

    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ManagerKey key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(ManagerKey)); ManagerKey? _2 = _1.ReadObject(_0) as ManagerKey; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='ManagerKey.xml' path='ManagerKey/record[@name="ManagerKeyWeb"]/main/*'/>*/
public record ManagerKeyWeb : ManagementKeyWeb
{
    /**<include file='ManagerKey.xml' path='ManagerKey/record[@name="ManagerKeyWeb"]/method[@name="ToManagerKey"]/*'/>*/
    public ManagerKey? ToManagerKey() { return Key is null ? null : new ManagerKey(Key.CloneByteArray(),ID,Thumbprint); }

    ///<inheritdoc/>
    public override SecurityKey? ToSecurityKey() => ToManagerKey();
}