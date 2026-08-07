namespace KusDepot.Security.Data;

/**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/main/*'/>*/
internal static partial class DataItemSecurityEnvelope
{
    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="DecryptArray"]/*'/>*/
    internal static Byte[]? DecryptArray(Byte[]? input , X509Certificate2? certificate , ReadOnlySpan<Byte> rootcontext = default)
    {
        Byte? v = GetEnvelopeVersion(input); if(v is null || !IsSupportedVersion(v.Value)) { return null; }

        return v.Value switch
        {
            EnvelopeVersion1 => DataItemSecurityEnvelopeV1.DecryptArray(input,certificate,rootcontext),

            _ => null
        };
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="DecryptArrayWithHash"]/*'/>*/
    internal static DataItemSecurityEnvelopeArrayHashResult? DecryptArrayWithHash(Byte[]? input , X509Certificate2? certificate , ReadOnlySpan<Byte> rootcontext = default)
    {
        Byte? v = GetEnvelopeVersion(input); if(v is null || !IsSupportedVersion(v.Value)) { return null; }

        return v.Value switch
        {
            EnvelopeVersion1 => DataItemSecurityEnvelopeV1.DecryptArrayWithHash(input,certificate,rootcontext),

            _ => null
        };
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="DecryptArrayWithHashAsync"]/*'/>*/
    internal static async Task<DataItemSecurityEnvelopeArrayHashResult?> DecryptArrayWithHashAsync(Byte[]? input , X509Certificate2? certificate , ReadOnlyMemory<Byte> rootcontext = default , CancellationToken cancel = default)
    {
        Byte? v = GetEnvelopeVersion(input); if(v is null || !IsSupportedVersion(v.Value)) { return null; }

        return v.Value switch
        {
            EnvelopeVersion1 => await DataItemSecurityEnvelopeV1.DecryptArrayWithHashAsync(input,certificate,rootcontext,cancel).ConfigureAwait(false),

            _ => null
        };
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="DecryptArrayAsync"]/*'/>*/
    internal static async Task<Byte[]?> DecryptArrayAsync(Byte[]? input , X509Certificate2? certificate , ReadOnlyMemory<Byte> rootcontext = default , CancellationToken cancel = default)
    {
        Byte? v = GetEnvelopeVersion(input); if(v is null || !IsSupportedVersion(v.Value)) { return null; }

        return v.Value switch
        {
            EnvelopeVersion1 => await DataItemSecurityEnvelopeV1.DecryptArrayAsync(input,certificate,rootcontext,cancel).ConfigureAwait(false),

            _ => null
        };
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="DecryptStream"]/*'/>*/
    internal static Boolean DecryptStream(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlySpan<Byte> rootcontext = default)
    {
        Byte? v = GetEnvelopeVersion(input); if(v is null || !IsSupportedVersion(v.Value)) { return false; }

        using DecryptionStream s = new(input!,v.Value);

        return v.Value switch
        {
            EnvelopeVersion1 => DataItemSecurityEnvelopeV1.DecryptStream(s,output,certificate,rootcontext),

            _ => false
        };
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="DecryptStreamAsync"]/*'/>*/
    internal static async Task<Boolean> DecryptStreamAsync(Stream? input , Stream? output , X509Certificate2? certificate , ReadOnlyMemory<Byte> rootcontext = default , CancellationToken cancel = default)
    {
        Byte? v = await GetEnvelopeVersionAsync(input,cancel).ConfigureAwait(false); if(v is null || !IsSupportedVersion(v.Value)) { return false; }

        using DecryptionStream s = new(input!,v.Value);

        return v.Value switch
        {
            EnvelopeVersion1 => await DataItemSecurityEnvelopeV1.DecryptStreamAsync(s,output,certificate,rootcontext,cancel).ConfigureAwait(false),

            _ => false
        };
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="EncryptArray"]/*'/>*/
    internal static Byte[]? EncryptArray(Byte[]? input , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlySpan<Byte> rootcontext = default , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        return DataItemSecurityEnvelopeV1.EncryptArray(input,recipients,issuer,rootcontext,chunksizepower);
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="EncryptArrayAsync"]/*'/>*/
    internal static async Task<Byte[]?> EncryptArrayAsync(Byte[]? input , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlyMemory<Byte> rootcontext = default , Int32 chunksizepower = DefaultCryptoChunkPower , CancellationToken cancel = default)
    {
        return await DataItemSecurityEnvelopeV1.EncryptArrayAsync(input,recipients,issuer,rootcontext,chunksizepower,cancel).ConfigureAwait(false);
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="EncryptArrayWithHash"]/*'/>*/
    internal static DataItemSecurityEnvelopeArrayHashResult? EncryptArrayWithHash(Byte[]? input , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlySpan<Byte> rootcontext = default , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        return DataItemSecurityEnvelopeV1.EncryptArrayWithHash(input,recipients,issuer,rootcontext,chunksizepower);
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="EncryptArrayWithHashAsync"]/*'/>*/
    internal static async Task<DataItemSecurityEnvelopeArrayHashResult?> EncryptArrayWithHashAsync(Byte[]? input , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlyMemory<Byte> rootcontext = default , Int32 chunksizepower = DefaultCryptoChunkPower , CancellationToken cancel = default)
    {
        return await DataItemSecurityEnvelopeV1.EncryptArrayWithHashAsync(input,recipients,issuer,rootcontext,chunksizepower,cancel).ConfigureAwait(false);
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="EncryptStream"]/*'/>*/
    internal static Boolean EncryptStream(Stream? input , Stream? output , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlySpan<Byte> rootcontext = default , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        return DataItemSecurityEnvelopeV1.EncryptStream(input,output,recipients,issuer,rootcontext,chunksizepower);
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="EncryptStreamAsync"]/*'/>*/
    internal static async Task<Boolean> EncryptStreamAsync(Stream? input , Stream? output , ImmutableArray<DataSecurityRecipient> recipients , DataSecurityObject? issuer , ReadOnlyMemory<Byte> rootcontext = default , Int32 chunksizepower = DefaultCryptoChunkPower , CancellationToken cancel = default)
    {
        return await DataItemSecurityEnvelopeV1.EncryptStreamAsync(input,output,recipients,issuer,rootcontext,chunksizepower,cancel).ConfigureAwait(false);
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="GetEncryptedCapacity"]/*'/>*/
    internal static Int32? GetEncryptedCapacity(ImmutableArray<DataSecurityRecipient> recipients , Int64 plaintextlength , Int32 chunksizepower = DefaultCryptoChunkPower)
    {
        return DataItemSecurityEnvelopeV1.GetEncryptedCapacity(recipients,plaintextlength,chunksizepower);
    }
}

/**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/record[@name="DataItemSecurityEnvelopeArrayHashResult"]/*'/>*/
internal readonly record struct DataItemSecurityEnvelopeArrayHashResult(Byte[] Buffer , Byte[] Hash);