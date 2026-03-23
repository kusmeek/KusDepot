namespace KusDepot.Security;

/**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.OwnerKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "OwnerKey" , Namespace = "KusDepot.Security")]

public sealed record class OwnerKey : ManagementKey , IParsable<OwnerKey>
{
    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public OwnerKey() {}

    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/constructor[@name="Constructor"]/*'/>*/
    public OwnerKey(Byte[] certificate , Guid? id = null)
    {
        ID = id ?? Guid.NewGuid(); Key = certificate.CloneByteArray();

        Thumbprint = DeserializeCertificate(certificate)?.Thumbprint;
    }

    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/constructor[@name="ConstructorCertificate"]/*'/>*/
    public OwnerKey(X509Certificate2 certificate , Guid? id = null) : this(SerializeCertificate(certificate)!,id) {}

    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(OwnerKey? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/method[@name="IParsable{OwnerKey}.Parse"]/*'/>*/
    static OwnerKey IParsable<OwnerKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/method[@name="Parse"]/*'/>*/
    public static new OwnerKey? Parse(String input , IFormatProvider? format = null)
    {
        return SecurityKey.Parse<OwnerKey>(input,format);
    }

    /**<include file='OwnerKey.xml' path='OwnerKey/class[@name="OwnerKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out OwnerKey key)
    {
        return SecurityKey.TryParse(input,format,out key);
    }
}