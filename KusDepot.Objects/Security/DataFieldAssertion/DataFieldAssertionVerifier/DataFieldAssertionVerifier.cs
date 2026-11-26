namespace KusDepot.Security;

/**<include file='DataFieldAssertionVerifier.xml' path='DataFieldAssertionVerifier/class[@name="DataFieldAssertionVerifier"]/main/*'/>*/
public static class DataFieldAssertionVerifier
{
    /**<include file='DataFieldAssertionVerifier.xml' path='DataFieldAssertionVerifier/class[@name="DataFieldAssertionVerifier"]/method[@name="TryValidateData"]/*'/>*/
    internal static Boolean TryValidateData(in DataFieldAssertionData data)
    {
        return DataFieldAssertionFactory.TryValidateData(in data);
    }

    /**<include file='DataFieldAssertionVerifier.xml' path='DataFieldAssertionVerifier/class[@name="DataFieldAssertionVerifier"]/method[@name="TryValidateEvaluation"]/*'/>*/
    internal static Boolean TryValidateEvaluation(in DataFieldAssertionData data , DateTimeOffset evaluatedat)
    {
        if(data.NotBefore.HasValue && evaluatedat < data.NotBefore.Value) { return false; }

        return data.ExpiresAt.HasValue is false || evaluatedat <= data.ExpiresAt.Value;
    }

    /**<include file='DataFieldAssertionVerifier.xml' path='DataFieldAssertionVerifier/class[@name="DataFieldAssertionVerifier"]/method[@name="TryVerifyArtifact"]/*'/>*/
    public static Boolean TryVerify(DataFieldAssertion? assertion , X509Certificate2? certificate , DateTimeOffset? evaluatedat , out DataFieldAssertionData data)
    {
        data = default;

        if(assertion is null || certificate is null || assertion.EnvelopeVersion != DataFieldAssertionFactory.EnvelopeVersion || assertion.SignatureAlgorithm != DataFieldAssertionFactory.SignatureAlgorithmRsaSha512Pss)
        {
            return false;
        }

        DataFieldAssertionData assertiondata = assertion.Data;

        if(!TryValidateData(in assertiondata) || !TryValidateEvaluation(in assertiondata,evaluatedat ?? DateTimeOffset.UtcNow)) { return false; }

        if(!String.Equals(assertiondata.Thumbprint,certificate.Thumbprint,Ordinal)) { return false; }

        try
        {
            using RSA? rsa = certificate.GetRSAPublicKey(); if(rsa is null) { return false; }

            if(!DataFieldAssertionFactory.TryWriteData(in assertiondata,out Byte[]? databytes)) { return false; }

            if(!rsa.VerifyData(databytes!,assertion.Signature,HashAlgorithmName.SHA512,RSASignaturePadding.Pss)) { return false; }

            data = assertiondata;

            return true;
        }
        catch { data = default; return false; }
    }

    /**<include file='DataFieldAssertionVerifier.xml' path='DataFieldAssertionVerifier/class[@name="DataFieldAssertionVerifier"]/method[@name="TryVerifyBytes"]/*'/>*/
    public static Boolean TryVerify(ReadOnlySpan<Byte> assertion , X509Certificate2? certificate , DateTimeOffset? evaluatedat , out DataFieldAssertionData data)
    {
        data = default;

        if(!DataFieldAssertionFactory.TryRead(assertion,out DataFieldAssertion? artifact)) { return false; }

        return TryVerify(artifact,certificate,evaluatedat,out data);
    }

    /**<include file='DataFieldAssertionVerifier.xml' path='DataFieldAssertionVerifier/class[@name="DataFieldAssertionVerifier"]/method[@name="TryVerifyArtifactResolver"]/*'/>*/
    public static Boolean TryVerify(DataFieldAssertion? assertion , Func<Guid,X509Certificate2?>? issuercertificateresolver , DateTimeOffset? evaluatedat , out DataFieldAssertionData data)
    {
        data = default;

        if(assertion is null || issuercertificateresolver is null) { return false; }

        return TryVerify(assertion,issuercertificateresolver(assertion.Data.IssuerObjectId),evaluatedat,out data);
    }

    /**<include file='DataFieldAssertionVerifier.xml' path='DataFieldAssertionVerifier/class[@name="DataFieldAssertionVerifier"]/method[@name="TryVerifyBytesResolver"]/*'/>*/
    public static Boolean TryVerify(ReadOnlySpan<Byte> assertion , Func<Guid,X509Certificate2?>? issuercertificateresolver , DateTimeOffset? evaluatedat , out DataFieldAssertionData data)
    {
        data = default;

        if(!DataFieldAssertionFactory.TryRead(assertion,out DataFieldAssertion? artifact) || issuercertificateresolver is null) { return false; }

        return TryVerify(artifact,issuercertificateresolver,evaluatedat,out data);
    }
}
