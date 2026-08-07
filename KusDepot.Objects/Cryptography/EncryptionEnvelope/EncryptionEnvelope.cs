namespace KusDepot.Cryptography;

/**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/main/*'/>*/
internal static partial class EncryptionEnvelope
{
    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="DecryptArray"]/*'/>*/
    internal static Byte[]? DecryptArray(Byte[]? input , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default)
    {
        Byte? v = GetEnvelopeVersion(input); if(v is null || !IsSupportedVersion(v.Value)) { return null; }

        return v.Value switch
        {
            EnvelopeVersion1 => EncryptionEnvelopeV1.DecryptArray(input,certificate,aad),

            _ => null
        };
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="DecryptArrayAsync"]/*'/>*/
    internal static async Task<Byte[]?> DecryptArrayAsync(Byte[]? input , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , CancellationToken cancel = default)
    {
        Byte? v = GetEnvelopeVersion(input); if(v is null || !IsSupportedVersion(v.Value)) { return null; }

        return v.Value switch
        {
            EnvelopeVersion1 => await EncryptionEnvelopeV1.DecryptArrayAsync(input,certificate,aad,cancel).ConfigureAwait(false),

            _ => null
        };
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="DecryptSpan"]/*'/>*/
    internal static Boolean DecryptSpan(ReadOnlySpan<Byte> input , Span<Byte> output , X509Certificate2? certificate , out Int32 byteswritten , ReadOnlySpan<Byte> aad = default)
    {
        Byte? v = GetEnvelopeVersion(input); if(v is null || !IsSupportedVersion(v.Value)) { byteswritten = 0; return false; }

        return v.Value switch
        {
            EnvelopeVersion1 => EncryptionEnvelopeV1.DecryptSpan(input,output,certificate,out byteswritten,aad),

            _ => ZeroFalse(out byteswritten)
        };

        static Boolean ZeroFalse(out Int32 byteswritten) { byteswritten = 0; return false; }
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="DecryptStream"]/*'/>*/
    internal static Boolean DecryptStream(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default)
    {
        Byte? v = GetEnvelopeVersion(input); if(v is null || !IsSupportedVersion(v.Value)) { return false; }

        using DecryptionStream s = new(input!,v.Value);

        return v.Value switch
        {
            EnvelopeVersion1 => EncryptionEnvelopeV1.DecryptStream(s,output,certificate,aad),

            _ => false
        };
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="DecryptStreamAsync"]/*'/>*/
    internal static async Task<Boolean> DecryptStreamAsync(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , CancellationToken cancel = default)
    {
        Byte? v = await GetEnvelopeVersionAsync(input,cancel).ConfigureAwait(false); if(v is null || !IsSupportedVersion(v.Value)) { return false; }

        using DecryptionStream s = new(input!,v.Value);

        return v.Value switch
        {
            EnvelopeVersion1 => await EncryptionEnvelopeV1.DecryptStreamAsync(s,output,certificate,aad,cancel).ConfigureAwait(false),

            _ => false
        };
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="GetEncryptedCapacity"]/*'/>*/
    internal static Int32? GetEncryptedCapacity(X509Certificate2? certificate , Int64 plaintextlength , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        return certificate is null ? null : EncryptionEnvelopeV1.GetEncryptedArrayCapacity(certificate,plaintextlength,includeaadhash,includeoriginallength,chunksizepower);
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="EncryptArray"]/*'/>*/
    internal static Byte[]? EncryptArray(Byte[]? input , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        return EncryptionEnvelopeV1.EncryptArray(input,certificate,aad,includeaadhash,includeoriginallength,chunksizepower);
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="EncryptArrayAsync"]/*'/>*/
    internal static async Task<Byte[]?> EncryptArrayAsync(Byte[]? input , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower , CancellationToken cancel = default)
    {
        return await EncryptionEnvelopeV1.EncryptArrayAsync(input,certificate,aad,includeaadhash,includeoriginallength,chunksizepower,cancel).ConfigureAwait(false);
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="EncryptSpan"]/*'/>*/
    internal static Boolean EncryptSpan(ReadOnlySpan<Byte> input , Span<Byte> output , X509Certificate2? certificate , out Int32 byteswritten , ReadOnlySpan<Byte> aad = default , Boolean includeaadhash = false)
    {
        return EncryptionEnvelopeV1.EncryptSpan(input,output,certificate,out byteswritten,aad,includeaadhash);
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="EncryptStream"]/*'/>*/
    internal static Boolean EncryptStream(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        return EncryptionEnvelopeV1.EncryptStream(input,output,certificate,aad,includeaadhash,includeoriginallength,chunksizepower);
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="EncryptStreamAsync"]/*'/>*/
    internal static async Task<Boolean> EncryptStreamAsync(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower , CancellationToken cancel = default)
    {
        return await EncryptionEnvelopeV1.EncryptStreamAsync(input,output,certificate,aad,includeaadhash,includeoriginallength,chunksizepower,cancel).ConfigureAwait(false);
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="EncryptUtf16"]/*'/>*/
    internal static Boolean EncryptUtf16(String? input , Stream? output , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        return EncryptionEnvelopeV1.EncryptUtf16(input,output,certificate,aad,includeaadhash,includeoriginallength,chunksizepower);
    }

    /**<include file='EncryptionEnvelope.xml' path='EncryptionEnvelope/class[@name="EncryptionEnvelope"]/method[@name="EncryptUtf16Async"]/*'/>*/
    internal static async Task<Boolean> EncryptUtf16Async(String? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultCryptoChunkPower , CancellationToken cancel = default)
    {
        return await EncryptionEnvelopeV1.EncryptUtf16Async(input,output,certificate,aad,includeaadhash,includeoriginallength,chunksizepower,cancel).ConfigureAwait(false);
    }
}