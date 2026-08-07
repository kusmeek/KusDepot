namespace KusDepot.Data.Transfer;

public static partial class TransferEnvelope
{
    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="BuildHeader"]/*'/>*/
    internal static Byte[] BuildHeader(Int32 metadatalength , Int64 payloadlength , Int32 trailerlength = 0 , Byte flags = FlagsNone , Int32 reserved = 0)
    {
        Byte[] header = new Byte[FixedHeaderLength]; BufferWriter writer = new(header);

        if(!writer.TryWriteUInt32BigEndian(Magic) ||
           !writer.TryWriteByte(FormatVersion) ||
           !writer.TryWriteByte(flags) ||
           !writer.TryWriteUInt16BigEndian(FixedHeaderLength) ||
           !writer.TryWriteInt32BigEndian(metadatalength) ||
           !writer.TryWriteInt64BigEndian(payloadlength) ||
           !writer.TryWriteInt32BigEndian(trailerlength) ||
           !writer.TryWriteInt32BigEndian(reserved)) { return Array.Empty<Byte>(); }

        return header;
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="ComputeEnvelopeLength"]/*'/>*/
    public static Int64 ComputeEnvelopeLength(Int32 metadatalength , Int64 payloadlength , Int32 trailerlength = 0)
    {
        return FixedHeaderLength + metadatalength + payloadlength + trailerlength;
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="GetMetadataOffset"]/*'/>*/
    private static Int32 GetMetadataOffset() { return FixedHeaderLength; }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="GetPayloadOffset"]/*'/>*/
    private static Int32 GetPayloadOffset(TransferEnvelopeHeader header)
    {
        Int64 offset = header.HeaderLength + header.MetadataLength;

        return offset > Int32.MaxValue ? Int32.MaxValue : (Int32)offset;
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="GetTrailerOffset"]/*'/>*/
    private static Int32 GetTrailerOffset(TransferEnvelopeHeader header)
    {
        Int64 offset = header.HeaderLength + header.MetadataLength + header.PayloadLength;

        return offset > Int32.MaxValue ? Int32.MaxValue : (Int32)offset;
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="ReadExactlyAsync"]/*'/>*/
    private static async ValueTask<Boolean> ReadExactlyAsync(Stream input , Memory<Byte> buffer , CancellationToken cancel)
    {
        Int32 offset = 0;

        while(offset < buffer.Length)
        {
            Int32 read = await input.ReadAsync(buffer[offset..],cancel).ConfigureAwait(false);

            if(read <= 0) { return false; }

            offset += read;
        }

        return true;
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="TryParseHeaderCore"]/*'/>*/
    private static Boolean TryParseHeaderCore(Byte[] input , Boolean requirefullenvelopelength , out TransferEnvelopeHeader header)
    {
        header = default;

        if(input is null || input.Length < FixedHeaderLength) { return false; }

        BufferReader reader = new(input);

        if(!reader.TryReadUInt32BigEndian(out UInt32 magic) ||
           !reader.TryReadByte(out Byte formatVersion) ||
           !reader.TryReadByte(out Byte flags) ||
           !reader.TryReadUInt16BigEndian(out UInt16 headerLength) ||
           !reader.TryReadInt32BigEndian(out Int32 metadataLength) ||
           !reader.TryReadInt64BigEndian(out Int64 payloadLength) ||
           !reader.TryReadInt32BigEndian(out Int32 trailerLength) ||
           !reader.TryReadInt32BigEndian(out Int32 reserved)) { return false; }

        if(magic != Magic || formatVersion != FormatVersion || flags != FlagsNone || headerLength != FixedHeaderLength || metadataLength < 0 || payloadLength < 0 || trailerLength < 0 || reserved < 0) { return false; }

        if(requirefullenvelopelength)
        {
            Int64 totalLength = ComputeEnvelopeLength(metadataLength,payloadLength,trailerLength); if(totalLength > input.LongLength) { return false; }
        }

        header = new(magic,formatVersion,flags,headerLength,metadataLength,payloadLength,trailerLength,reserved); return true;
    }
}