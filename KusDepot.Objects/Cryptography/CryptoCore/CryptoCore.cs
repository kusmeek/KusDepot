namespace KusDepot.Cryptography;

/**<include file='CryptoCore.xml' path='CryptoCore/class[@name="CryptoCore"]/main/*'/>*/
internal static partial class CryptoCore
{
    /**<include file='CryptoCore.xml' path='CryptoCore/class[@name="CryptoCore"]/method[@name="Decrypt"]/*'/>*/
    internal static Byte[]? Decrypt(Byte[]? input , X509Certificate2? certificate , Byte[]? aad = null)
    {
        return input is null || certificate is null ? null : GeneralDataEnvelope.DecryptArray(input,certificate,aad);
    }

    /**<include file='CryptoCore.xml' path='CryptoCore/class[@name="CryptoCore"]/method[@name="DecryptStream"]/*'/>*/
    internal static Boolean Decrypt(Stream? input , Stream? output , X509Certificate2? certificate , Byte[]? aad = null)
    {
        return input is null || output is null || certificate is null ? false : GeneralDataEnvelope.DecryptStream(input,output,certificate,aad);
    }

    /**<include file='CryptoCore.xml' path='CryptoCore/class[@name="CryptoCore"]/method[@name="DecryptSpan"]/*'/>*/
    internal static Boolean Decrypt(ReadOnlySpan<Byte> input , Span<Byte> output , X509Certificate2? certificate , out Int32 byteswritten , ReadOnlySpan<Byte> aad = default)
    {
        byteswritten = 0; return certificate is null ? false : GeneralDataEnvelope.DecryptSpan(input,output,certificate,out byteswritten,aad);
    }

    /**<include file='CryptoCore.xml' path='CryptoCore/class[@name="CryptoCore"]/method[@name="DecryptAsync"]/*'/>*/
    internal static Task<Byte[]?> DecryptAsync(Byte[]? input , X509Certificate2? certificate , Byte[]? aad = null , CancellationToken cancel = default)
    {
        return input is null || certificate is null ? Task.FromResult<Byte[]?>(null) : GeneralDataEnvelope.DecryptArrayAsync(input,certificate,aad,cancel);
    }

    /**<include file='CryptoCore.xml' path='CryptoCore/class[@name="CryptoCore"]/method[@name="DecryptAsyncStream"]/*'/>*/
    internal static Task<Boolean> DecryptAsync(Stream? input , Stream? output , X509Certificate2? certificate , Byte[]? aad = null , CancellationToken cancel = default)
    {
        return input is null || output is null || certificate is null ? Task.FromResult(false) : GeneralDataEnvelope.DecryptStreamAsync(input,output,certificate,aad,cancel);
    }

    /**<include file='CryptoCore.xml' path='CryptoCore/class[@name="CryptoCore"]/method[@name="Encrypt"]/*'/>*/
    internal static Byte[]? Encrypt(Byte[]? input , X509Certificate2? certificate , Byte[]? aad = null , Boolean includeaadhash = true , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        return input is null || certificate is null ? null : GeneralDataEnvelope.EncryptArray(input,certificate,aad,includeaadhash,includeoriginallength,chunksizepower);
    }

    /**<include file='CryptoCore.xml' path='CryptoCore/class[@name="CryptoCore"]/method[@name="EncryptStream"]/*'/>*/
    internal static Boolean Encrypt(Stream? input , Stream? output , X509Certificate2? certificate , Byte[]? aad = null , Boolean includeaadhash = true , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        return input is null || output is null || certificate is null ? false : GeneralDataEnvelope.EncryptStream(input,output,certificate,aad,includeaadhash,includeoriginallength,chunksizepower);
    }

    /**<include file='CryptoCore.xml' path='CryptoCore/class[@name="CryptoCore"]/method[@name="EncryptSpan"]/*'/>*/
    internal static Boolean Encrypt(ReadOnlySpan<Byte> input , Span<Byte> output , X509Certificate2? certificate , out Int32 byteswritten , ReadOnlySpan<Byte> aad = default , Boolean includeaadhash = true)
    {
        byteswritten = 0; return certificate is null ? false : GeneralDataEnvelope.EncryptSpan(input,output,certificate,out byteswritten,aad,includeaadhash);
    }

    /**<include file='CryptoCore.xml' path='CryptoCore/class[@name="CryptoCore"]/method[@name="EncryptAsync"]/*'/>*/
    internal static Task<Byte[]?> EncryptAsync(Byte[]? input , X509Certificate2? certificate , Byte[]? aad = null , Boolean includeaadhash = true , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower , CancellationToken cancel = default)
    {
        return input is null || certificate is null ? Task.FromResult<Byte[]?>(null) : GeneralDataEnvelope.EncryptArrayAsync(input,certificate,aad,includeaadhash,includeoriginallength,chunksizepower,cancel);
    }

    /**<include file='CryptoCore.xml' path='CryptoCore/class[@name="CryptoCore"]/method[@name="EncryptAsyncStream"]/*'/>*/
    internal static Task<Boolean> EncryptAsync(Stream? input , Stream? output , X509Certificate2? certificate , Byte[]? aad = null , Boolean includeaadhash = true , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower , CancellationToken cancel = default)
    {
        return input is null || output is null || certificate is null ? Task.FromResult(false) : GeneralDataEnvelope.EncryptStreamAsync(input,output,certificate,aad,includeaadhash,includeoriginallength,chunksizepower,cancel);
    }
}