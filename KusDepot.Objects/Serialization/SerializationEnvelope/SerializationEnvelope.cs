namespace KusDepot.Serialization;

/**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/main/*'/>*/
public static partial class SerializationEnvelope
{
    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="HasEnvelope"]/*'/>*/
    public static Boolean HasEnvelope(Byte[] input)
    {
        if(input is null || input.Length < MagicSize) { return false; }

        return input.AsSpan(MagicOffset,MagicSize).SequenceEqual(Magic.AsSpan());
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="ParseHeader"]/*'/>*/
    public static ParsedSerializationHeader? ParseHeader(Byte[] input)
    {
        if(!HasEnvelope(input)) { return null; }

        Byte formatVersion = input[FormatVersionOffset];

        SerializationKind kind = (SerializationKind)input[SerializerKindOffset];

        Int32 serializerVersionLength = GetSerializerVersionLength(input);

        ReadOnlySpan<Byte> serializerVersionBytes = input.AsSpan(GetSerializerVersionOffset(),serializerVersionLength);

        if(!Utf8.IsValid(serializerVersionBytes)) { return null; }

        String serializerVersion = Encoding.UTF8.GetString(serializerVersionBytes);

        Int32 libraryVersionLength = GetLibraryVersionLength(input);

        ReadOnlySpan<Byte> libraryVersionBytes = input.AsSpan(GetLibraryVersionOffset(input),libraryVersionLength);

        if(!Utf8.IsValid(libraryVersionBytes)) { return null; }

        String libraryVersion = Encoding.UTF8.GetString(libraryVersionBytes);

        Int32 headerLength = GetPayloadOffset(input);

        return new ParsedSerializationHeader(formatVersion,kind,serializerVersion,libraryVersion,headerLength);
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="Unwrap"]/*'/>*/
    public static ReadOnlyMemory<Byte> Unwrap(Byte[] input)
    {
        if(!HasEnvelope(input)) { return input; }

        Int32 payloadOffset = GetPayloadOffset(input);

        return new ReadOnlyMemory<Byte>(input,payloadOffset,input.Length - payloadOffset);
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="UnwrapMemory"]/*'/>*/
    public static ReadOnlyMemory<Byte> Unwrap(ReadOnlyMemory<Byte> input)
    {
        ReadOnlySpan<Byte> span = input.Span;

        if(span.Length < MagicSize || !span[MagicOffset..(MagicOffset + MagicSize)].SequenceEqual(Magic.AsSpan())) { return input; }

        Int32 serializerVersionLength = span[SerializerVersionLengthOffset];

        Int32 libraryVersionLengthOffset = SerializerVersionOffset + serializerVersionLength;

        Int32 payloadOffset = libraryVersionLengthOffset + LibraryVersionLengthSize + span[libraryVersionLengthOffset];

        return input[payloadOffset..];
    }
}