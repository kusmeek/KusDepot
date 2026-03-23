namespace KusDepot.Security;

/**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.TokenKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "TokenKey" , Namespace = "KusDepot.Security")]

public sealed record class TokenKey : AccessKey , IParsable<TokenKey>
{
    /**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public TokenKey() {}

    /**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/constructor[@name="Constructor"]/*'/>*/
    public TokenKey(ReadOnlySpan<Byte> token , Guid? id = null , TokenKeyType type = TokenKeyType.Unknown)
    {
        ID = id ?? Guid.NewGuid(); Key = token.ToArray(); TokenType = type;
    }

    /**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/property[@name="TokenType"]/*'/>*/
    [JsonPropertyName("TokenType")] [JsonRequired] [Id(0)]
    [DataMember(Name = "TokenType" , EmitDefaultValue = true , IsRequired = true)]
    public TokenKeyType TokenType { get; init; }

    /**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(TokenKey? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    ///<inheritdoc/>
    static TokenKey IParsable<TokenKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/method[@name="Parse"]/*'/>*/
    public static new TokenKey? Parse(String input , IFormatProvider? format = null)
    {
        return SecurityKey.Parse<TokenKey>(input,format);
    }

    /**<include file='TokenKey.xml' path='TokenKey/class[@name="TokenKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TokenKey key)
    {
        return SecurityKey.TryParse(input,format,out key);
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