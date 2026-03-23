namespace KusDepot.Cryptography;

internal static partial class GeneralDataEnvelope
{
    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="DecryptArray"]/*'/>*/
    internal static Byte[]? DecryptArray(Byte[]? input , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default)
    {
        if(input is null || certificate is null) { return null; }

        try
        {
            using var i = new MemoryStream(input,writable:false); using var o = new MemoryStream();

            return DecryptStream(i,o,certificate,aad) is false ? null : o.ToArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptArrayFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="DecryptArrayAsync"]/*'/>*/
    internal static async Task<Byte[]?> DecryptArrayAsync(Byte[]? input , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , CancellationToken cancel = default)
    {
        if(input is null || certificate is null || cancel.IsCancellationRequested) { return null; }

        try
        {
            using var i = new MemoryStream(input,writable:false); using var o = new MemoryStream();

            return await DecryptStreamAsync(i,o,certificate,aad,cancel).ConfigureAwait(false) is false ? null : o.ToArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DecryptArrayAsyncFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="EncryptArray"]/*'/>*/
    internal static Byte[]? EncryptArray(Byte[]? input , X509Certificate2? certificate , ReadOnlySpan<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower)
    {
        if(input is null || certificate is null) { return null; }

        try
        {
            using var i = new MemoryStream(input,writable:false); using var o = new MemoryStream();

            return EncryptStream(i,o,certificate,aad,includeaadhash,includeoriginallength,chunksizepower) is false ? null : o.ToArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptArrayFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/method[@name="EncryptArrayAsync"]/*'/>*/
    internal static async Task<Byte[]?> EncryptArrayAsync(Byte[]? input , X509Certificate2? certificate , ReadOnlyMemory<Byte> aad = default , Boolean includeaadhash = false , Boolean includeoriginallength = true , Int32 chunksizepower = DefaultChunkSizePower , CancellationToken cancel = default)
    {
        if(input is null || certificate is null || cancel.IsCancellationRequested) { return null; }

        try
        {
            using var i = new MemoryStream(input,writable:false); using var o = new MemoryStream();

            return await EncryptStreamAsync(i,o,certificate,aad,includeaadhash,includeoriginallength,chunksizepower,cancel).ConfigureAwait(false) is false ? null : o.ToArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,EncryptArrayAsyncFail); if(NoExceptions) { return null; } throw; }
    }
}