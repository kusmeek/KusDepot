namespace KusDepot.Security.Requests;

/**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[Alias("KusDepot.Security.Requests.ManagementRequest")] [GenerateSerializer] [Immutable]

public sealed record class ManagementRequest : AccessRequest , IParsable<ManagementRequest>
{
    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ManagementRequest()
    {
        RequestedKeyType = nameof(ExecutiveKey); Operations = ProtectedOperationSets.Executive;
    }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/constructor[@name="Constructor"]/*'/>*/
    public ManagementRequest(String key)
    {
        Credential = new ManagementKeyCredential() { Key = key }; RequestedKeyType = nameof(ExecutiveKey); Operations = ProtectedOperationSets.Executive;

        if(ManagementKey.Parse(key,null) is ManagementKey managementkey && managementkey.Key is Byte[] certificatekey)
        {
            Subject = DeserializeCertificate(certificatekey)?.Subject;
        }
    }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/constructor[@name="ConstructorKey"]/*'/>*/
    public ManagementRequest(ManagementKey key)
    {
        Credential = new ManagementKeyCredential() { Key = key.ToString() }; RequestedKeyType = nameof(ExecutiveKey); Operations = ProtectedOperationSets.Executive;

        Subject = key.Key is Byte[] certificatekey ? DeserializeCertificate(certificatekey)?.Subject : null;
    }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/constructor[@name="ConstructorCertificate"]/*'/>*/
    public ManagementRequest(X509Certificate2 certificate)
    {
        Credential = CertificateCredential.Create(certificate); RequestedKeyType = nameof(ExecutiveKey); Operations = ProtectedOperationSets.Executive; Subject = certificate.Subject;
    }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(ManagementRequest? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="IParsable{ManagementRequest}.Parse"]/*'/>*/
    static ManagementRequest IParsable<ManagementRequest>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="Parse"]/*'/>*/
    public static new ManagementRequest? Parse(String input , IFormatProvider? format = null)
    {
        return AccessRequest.Parse(input,format) as ManagementRequest;
    }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ManagementRequest request)
    {
        return AccessRequest.TryParse(input,format,out request);
    }
}