namespace KusDepot.Security.Data;

internal static partial class DataItemSecurityEnvelope
{
    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/field[@name="EnvelopeVersion1"]/*'/>*/
    private const Byte EnvelopeVersion1 = 0x01;

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="GetEnvelopeVersionArray"]/*'/>*/
    private static Byte? GetEnvelopeVersion(Byte[]? input)
    {
        return input is null || input.Length == 0 ? null : input[0];
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="GetEnvelopeVersionSpan"]/*'/>*/
    private static Byte? GetEnvelopeVersion(ReadOnlySpan<Byte> input)
    {
        return input.IsEmpty ? null : input[0];
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="GetEnvelopeVersionStream"]/*'/>*/
    private static Byte? GetEnvelopeVersion(Stream? input)
    {
        if(input is null || input.CanRead is false) { return null; }

        Int32 b = input.ReadByte();

        return b < 0 ? null : (Byte)b;
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="GetEnvelopeVersionStreamAsync"]/*'/>*/
    private static async Task<Byte?> GetEnvelopeVersionAsync(Stream? input , CancellationToken cancel = default)
    {
        if(input is null || input.CanRead is false || cancel.IsCancellationRequested) { return null; }

        Byte[] buffer = new Byte[1];
        Int32 read = await input.ReadAsync(buffer.AsMemory(0,1),cancel).ConfigureAwait(false);

        return read == 1 ? buffer[0] : null;
    }

    /**<include file='DataItemSecurityEnvelope.xml' path='DataItemSecurityEnvelope/class[@name="DataItemSecurityEnvelope"]/method[@name="IsSupportedVersion"]/*'/>*/
    private static Boolean IsSupportedVersion(Byte version)
    {
        return version == EnvelopeVersion1;
    }
}
