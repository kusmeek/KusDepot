namespace KusDepot.ToolServiceExam;

internal static class Utility
{
    public static X509Certificate2? DeserializeCertificate(String? certificate)
    {
        if(certificate is null) { return null; }

        #if NET9_0_OR_GREATER

        return X509CertificateLoader.LoadPkcs12(certificate.ToByteArrayFromBase64(),null,X509KeyStorageFlags.EphemeralKeySet);

        #else

        return new X509Certificate2(certificate.ToByteArrayFromBase64(),(String?)null,X509KeyStorageFlags.EphemeralKeySet);

        #endif
    }

    public static String? SerializeCertificate(X509Certificate2? certificate)
    {
        if(certificate is null) { return null; }

        return certificate.Export(X509ContentType.Pkcs12).ToBase64FromByteArray();
    }

    public static String ToBase64FromByteArray(this Byte[]? input) { if(input is null) { return String.Empty; } return Convert.ToBase64String(input); }

    public static Byte[] ToByteArrayFromBase64(this String? input) { if(input is null) { return Array.Empty<Byte>(); } return Convert.FromBase64String(input); }
}