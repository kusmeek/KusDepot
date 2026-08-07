namespace KusDepot.Data.Transfer;

/**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/main/*'/>*/
public static partial class TransferEnvelope
{
    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="HasEnvelope"]/*'/>*/
    public static Boolean HasEnvelope(Byte[] input)
    {
        if(input is null || input.Length < FixedHeaderLength) { return false; }

        return ReadUInt32BigEndian(input.AsSpan(MagicOffset,sizeof(UInt32))) == Magic;
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="ParseHeader"]/*'/>*/
    public static TransferEnvelopeHeader? ParseHeader(Byte[] input)
    {
        return TryParseHeaderCore(input,requirefullenvelopelength:true,out TransferEnvelopeHeader header) ? header : null;
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="GetMetadataBytes"]/*'/>*/
    public static ReadOnlyMemory<Byte> GetMetadataBytes(Byte[] input)
    {
        TransferEnvelopeHeader? header = ParseHeader(input); if(!header.HasValue) { return ReadOnlyMemory<Byte>.Empty; }

        return new ReadOnlyMemory<Byte>(input,GetMetadataOffset(),header.Value.MetadataLength);
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="GetTrailerBytes"]/*'/>*/
    public static ReadOnlyMemory<Byte> GetTrailerBytes(Byte[] input)
    {
        TransferEnvelopeHeader? header = ParseHeader(input); if(!header.HasValue) { return ReadOnlyMemory<Byte>.Empty; }

        Int32 trailerOffset = GetTrailerOffset(header.Value);

        return new ReadOnlyMemory<Byte>(input,trailerOffset,header.Value.TrailerLength);
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="ReadHeaderAsync"]/*'/>*/
    public static async ValueTask<TransferEnvelopeHeader?> ReadHeaderAsync(Stream input , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(input);

        Byte[] buffer = new Byte[FixedHeaderLength];

        if(await ReadExactlyAsync(input,buffer,cancel).ConfigureAwait(false) is false) { return null; }

        return TryParseHeaderCore(buffer,requirefullenvelopelength:false,out TransferEnvelopeHeader header) ? header : null;
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="ReadMetadataBytesAsync"]/*'/>*/
    public static async ValueTask<ReadOnlyMemory<Byte>> ReadMetadataBytesAsync(Stream input , TransferEnvelopeHeader header , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(input); ArgumentOutOfRangeException.ThrowIfNegative(header.MetadataLength);

        Byte[] buffer = new Byte[header.MetadataLength];

        if(await ReadExactlyAsync(input,buffer,cancel).ConfigureAwait(false) is false) { return ReadOnlyMemory<Byte>.Empty; }

        return buffer;
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="ReadTrailerBytesAsync"]/*'/>*/
    public static async ValueTask<ReadOnlyMemory<Byte>> ReadTrailerBytesAsync(Stream input , TransferEnvelopeHeader header , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(input); ArgumentOutOfRangeException.ThrowIfNegative(header.TrailerLength);

        Byte[] buffer = new Byte[header.TrailerLength];

        if(await ReadExactlyAsync(input,buffer,cancel).ConfigureAwait(false) is false) { return ReadOnlyMemory<Byte>.Empty; }

        return buffer;
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="OpenPayloadStream"]/*'/>*/
    public static Stream OpenPayloadStream(Stream input , TransferEnvelopeHeader header , Boolean leaveopen  = true)
    {
        ArgumentNullException.ThrowIfNull(input);

        return input.CanSeek
            ? new BoundedReadStream(input,input.Position,header.PayloadLength,leaveopen)
            : new NonSeekableBoundedReadStream(input,header.PayloadLength,leaveopen);
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="UnwrapPayload"]/*'/>*/
    public static ReadOnlyMemory<Byte> UnwrapPayload(Byte[] input)
    {
        TransferEnvelopeHeader? header = ParseHeader(input); if(!header.HasValue) { return ReadOnlyMemory<Byte>.Empty; }

        Int32 payloadOffset = GetPayloadOffset(header.Value);

        return new ReadOnlyMemory<Byte>(input,payloadOffset,(Int32)header.Value.PayloadLength);
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="WriteAsync"]/*'/>*/
    public static async ValueTask WriteAsync(Stream destination , ReadOnlyMemory<Byte> metadatabytes , Stream payload , Int64 payloadlength , Int32 trailerlength = 0 , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(destination); ArgumentNullException.ThrowIfNull(payload); ArgumentOutOfRangeException.ThrowIfNegative(payloadlength); ArgumentOutOfRangeException.ThrowIfNegative(trailerlength);

        Byte[] header = BuildHeader(metadatabytes.Length,payloadlength,trailerlength);

        await destination.WriteAsync(header,cancel).ConfigureAwait(false);

        if(metadatabytes.Length > 0) { await destination.WriteAsync(metadatabytes,cancel).ConfigureAwait(false); }

        using Stream bounded = new BoundedReadStream(payload,payload.CanSeek ? payload.Position : 0,payloadlength,leaveopen:true);

        await bounded.CopyToAsync(destination,cancel).ConfigureAwait(false);
    }

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/method[@name="WriteWithTrailer"]/*'/>*/
    public static async ValueTask WriteWithTrailer(Stream destination , ReadOnlyMemory<Byte> trailerbytes , Stream payload , Int64 payloadlength , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(destination); ArgumentNullException.ThrowIfNull(payload); ArgumentOutOfRangeException.ThrowIfNegative(payloadlength);

        Byte[] header = BuildHeader(0,payloadlength,trailerbytes.Length);

        await destination.WriteAsync(header,cancel).ConfigureAwait(false);

        using Stream bounded = new BoundedReadStream(payload,payload.CanSeek ? payload.Position : 0,payloadlength,leaveopen:true);

        await bounded.CopyToAsync(destination,cancel).ConfigureAwait(false);

        if(trailerbytes.Length > 0) { await destination.WriteAsync(trailerbytes,cancel).ConfigureAwait(false); }
    }
}