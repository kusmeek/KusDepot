namespace KusDepot;

/**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/main/*'/>*/
[GenerateSerializer] [Alias("OwnerKey")]
[DataContract(Name = "OwnerKey" , Namespace = "KusDepot")]
public class OwnerKey : ManagementKey , IParsable<OwnerKey>
{
    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/constructor[@name="Constructor"]/*'/>*/
    public OwnerKey(Byte[] certificate , Guid? id = null , String? thumbprint = null)
    {
        ID = id is null ? ID : id; Key = certificate.CloneByteArray();

        Thumbprint = thumbprint is not null ? new(thumbprint) : DeserializeCertificate(certificate)?.Thumbprint;
    }

    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/constructor[@name="ConstructorCertificate"]/*'/>*/
    public OwnerKey(X509Certificate2 certificate , Guid? id = null) : this(SerializeCertificate(certificate)!,id,certificate.Thumbprint) {}

    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/method[@name="IParsable{OwnerKey}.Parse"]/*'/>*/
    static OwnerKey IParsable<OwnerKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/method[@name="Parse"]/*'/>*/
    public static new OwnerKey? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(OwnerKey)); OwnerKey? _2 = _1.ReadObject(_0) as OwnerKey; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/method[@name="ToOwnerKeyWeb"]/*'/>*/
    public OwnerKeyWeb ToOwnerKeyWeb() { return new() { ID = this.ID , Key = this.Key.CloneByteArray() , Thumbprint = this.Thumbprint }; }

    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out OwnerKey key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(OwnerKey)); OwnerKey? _2 = _1.ReadObject(_0) as OwnerKey; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='OwnerKey.xml' path='OwnerKey/record[@name="OwnerKeyWeb"]/main/*'/>*/
public record OwnerKeyWeb : ManagementKeyWeb
{
    /**<include file='OwnerKey.xml' path='OwnerKey/record[@name="OwnerKeyWeb"]/method[@name="ToOwnerKey"]/*'/>*/
    public OwnerKey? ToOwnerKey() { return Key is null ? null : new OwnerKey(Key.CloneByteArray(),ID,Thumbprint); }

    ///<inheritdoc/>
    public override SecurityKey? ToSecurityKey() => ToOwnerKey();
}