namespace KusDepot.Security;

/**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.Security.ManagerKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ManagerKey" , Namespace = "KusDepot.Security")]

public sealed record class ManagerKey : ManagementKey , IParsable<ManagerKey>
{
    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ManagerKey() {}

    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/constructor[@name="Constructor"]/*'/>*/
    public ManagerKey(Byte[] certificate , Guid? id = null)
    {
        ID = id ?? Guid.NewGuid(); Key = certificate.CloneByteArray();

        Thumbprint = DeserializeCertificate(certificate)?.Thumbprint;
    }

    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/constructor[@name="ConstructorCertificate"]/*'/>*/
    public ManagerKey(X509Certificate2 certificate , Guid? id = null) : this(SerializeCertificate(certificate)!,id) {}

    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(ManagerKey? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/method[@name="IParsable{ManagerKey}.Parse"]/*'/>*/
    static ManagerKey IParsable<ManagerKey>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/method[@name="Parse"]/*'/>*/
    public static new ManagerKey? Parse(String input , IFormatProvider? format = null)
    {
        return SecurityKey.Parse<ManagerKey>(input,format);
    }

    /**<include file='ManagerKey.xml' path='ManagerKey/class[@name="ManagerKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ManagerKey key)
    {
        return SecurityKey.TryParse(input,format,out key);
    }
}