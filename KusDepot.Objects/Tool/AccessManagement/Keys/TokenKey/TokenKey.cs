namespace KusDepot;

/**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/main/*'/>*/
[GenerateSerializer] [Alias("TokenKey")]
[DataContract(Name = "TokenKey" , Namespace = "KusDepot")]
public class TokenKey : AccessKey , IParsable<TokenKey>
{
    /**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/constructor[@name="Constructor"]/*'/>*/
    public TokenKey(ReadOnlySpan<Byte> token , Guid? id = null , TokenKeyType type = TokenKeyType.Unknown)
    {
        ID = id is null ? ID : id; Key = token.ToArray(); Type = type;
    }

    /**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/property[@name="Type"]/*'/>*/
    [DataMember(Name = "Type" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    public TokenKeyType Type { get; private set; }

    ///<inheritdoc/>
    static TokenKey IParsable<TokenKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/method[@name="Parse"]/*'/>*/
    public static new TokenKey? Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TokenKey)); TokenKey? _2 = _1.ReadObject(_0) as TokenKey; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }


    /**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TokenKey key)
    {
        key = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TokenKey)); TokenKey? _2 = _1.ReadObject(_0) as TokenKey; if( _2 is not null ) { key = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/main/*'/>*/
public enum TokenKeyType
{
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="Unknown"]/*'/>*/
    Unknown = 0,
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="Cwt"]/*'/>*/
    Cwt,
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="Jwt"]/*'/>*/
    Jwt,
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="Kerberos"]/*'/>*/
    Kerberos,
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="Opaque"]/*'/>*/
    Opaque,
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="Other"]/*'/>*/
    Other,
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="Paseto"]/*'/>*/
    Paseto,
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="Pop"]/*'/>*/
    Pop,
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="Saml2"]/*'/>*/
    Saml2,
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="Sas"]/*'/>*/
    Sas,
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="Spnego"]/*'/>*/
    Spnego,
    /**<include file='TokenKey.xml' path='TokenKey/enum[@name="TokenKeyType"]/field[@name="X509"]/*'/>*/
    X509
}