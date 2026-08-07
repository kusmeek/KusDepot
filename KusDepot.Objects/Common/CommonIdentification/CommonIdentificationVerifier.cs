namespace KusDepot;

/**<include file='CommonIdentificationVerifier.xml' path='CommonIdentificationVerifier/class[@name="CommonIdentificationVerifier"]/main/*'/>*/
public static class CommonIdentificationVerifier
{
    /**<include file='CommonIdentificationVerifier.xml' path='CommonIdentificationVerifier/class[@name="CommonIdentificationVerifier"]/method[@name="TryValidateData"]/*'/>*/
    internal static Boolean TryValidateData(in CommonIdentificationData data)
    {
        if(data.Version != CommonIdentificationFactory.DataVersion || data.Id == Guid.Empty || data.IssuerObjectId == Guid.Empty ||
           data.SubjectObjectId == Guid.Empty || String.IsNullOrWhiteSpace(data.Purpose) || String.IsNullOrWhiteSpace(data.Thumbprint))
        {
            return false;
        }

        if(data.NotBefore < data.CreatedAt) { return false; }

        return data.ExpiresAt.HasValue is false || data.ExpiresAt.Value > data.NotBefore;
    }

    /**<include file='CommonIdentificationVerifier.xml' path='CommonIdentificationVerifier/class[@name="CommonIdentificationVerifier"]/method[@name="TryValidateEvaluation"]/*'/>*/
    internal static Boolean TryValidateEvaluation(in CommonIdentificationData data , DateTimeOffset evaluatedat)
    {
        if(evaluatedat < data.NotBefore) { return false; }

        return data.ExpiresAt.HasValue is false || evaluatedat <= data.ExpiresAt.Value;
    }

    /**<include file='CommonIdentificationVerifier.xml' path='CommonIdentificationVerifier/class[@name="CommonIdentificationVerifier"]/method[@name="TryVerifyArtifact"]/*'/>*/
    public static Boolean TryVerify(CommonIdentification? identification , X509Certificate2? certificate , DateTimeOffset? evaluatedat , out CommonIdentificationData data)
    {
        data = default;

        if(identification is null || certificate is null || identification.EnvelopeVersion != CommonIdentificationFactory.EnvelopeVersion || identification.SignatureAlgorithm != CommonIdentificationFactory.SignatureAlgorithmRsaSha512Pss)
        {
            return false;
        }

        CommonIdentificationData identificationdata = identification.Data;

        if(!TryValidateData(in identificationdata) || !TryValidateEvaluation(in identificationdata,evaluatedat ?? DateTimeOffset.UtcNow)) { return false; }

        if(!String.Equals(identificationdata.Thumbprint,certificate.Thumbprint,Ordinal)) { return false; }

        try
        {
            using RSA? rsa = certificate.GetRSAPublicKey(); if(rsa is null) { return false; }

            if(!CommonIdentificationFactory.TryWriteData(in identificationdata,out Byte[]? databytes)) { return false; }

            if(!rsa.VerifyData(databytes!,identification.Signature,HashAlgorithmName.SHA512,RSASignaturePadding.Pss)) { return false; }

            data = identificationdata;

            return true;
        }
        catch { data = default; return false; }
    }

    /**<include file='CommonIdentificationVerifier.xml' path='CommonIdentificationVerifier/class[@name="CommonIdentificationVerifier"]/method[@name="TryVerifyBytes"]/*'/>*/
    public static Boolean TryVerify(ReadOnlySpan<Byte> identification , X509Certificate2? certificate , DateTimeOffset? evaluatedat , out CommonIdentificationData data)
    {
        data = default;

        if(!CommonIdentificationFactory.TryRead(identification,out CommonIdentification? artifact)) { return false; }

        return TryVerify(artifact,certificate,evaluatedat,out data);
    }

    /**<include file='CommonIdentificationVerifier.xml' path='CommonIdentificationVerifier/class[@name="CommonIdentificationVerifier"]/method[@name="TryVerifyArtifactResolver"]/*'/>*/
    public static Boolean TryVerify(CommonIdentification? identification , Func<Guid,X509Certificate2?>? issuercertificateresolver , DateTimeOffset? evaluatedat , out CommonIdentificationData data)
    {
        data = default;

        if(identification is null || issuercertificateresolver is null) { return false; }

        return TryVerify(identification,issuercertificateresolver(identification.Data.IssuerObjectId),evaluatedat,out data);
    }

    /**<include file='CommonIdentificationVerifier.xml' path='CommonIdentificationVerifier/class[@name="CommonIdentificationVerifier"]/method[@name="TryVerifyBytesResolver"]/*'/>*/
    public static Boolean TryVerify(ReadOnlySpan<Byte> identification , Func<Guid,X509Certificate2?>? issuercertificateresolver , DateTimeOffset? evaluatedat , out CommonIdentificationData data)
    {
        data = default;

        if(!CommonIdentificationFactory.TryRead(identification,out CommonIdentification? artifact) || issuercertificateresolver is null) { return false; }

        return TryVerify(artifact,issuercertificateresolver,evaluatedat,out data);
    }
}