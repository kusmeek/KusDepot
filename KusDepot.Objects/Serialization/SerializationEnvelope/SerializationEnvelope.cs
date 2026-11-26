namespace KusDepot.Serialization;

/**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/main/*'/>*/
public static partial class SerializationEnvelope
{
    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="HasEnvelope"]/*'/>*/
    public static Boolean HasEnvelope(Byte[] input)
    {
        return input is not null && TryGetPayloadOffset(input,out _);
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="ParseHeader"]/*'/>*/
    public static ParsedSerializationHeader? ParseHeader(Byte[] input)
    {
        if(input is null || !TryGetPayloadOffset(input,out Int32 headerLength)) { return null; }

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

        return new ParsedSerializationHeader(formatVersion,kind,serializerVersion,libraryVersion,headerLength);
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="Unwrap"]/*'/>*/
    public static ReadOnlyMemory<Byte> Unwrap(Byte[] input)
    {
        if(input is null || !TryGetPayloadOffset(input,out Int32 payloadOffset)) { return input; }

        return new ReadOnlyMemory<Byte>(input,payloadOffset,input.Length - payloadOffset);
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="UnwrapMemory"]/*'/>*/
    public static ReadOnlyMemory<Byte> Unwrap(ReadOnlyMemory<Byte> input)
    {
        ReadOnlySpan<Byte> span = input.Span;

        if(!TryGetPayloadOffset(span,out Int32 payloadOffset)) { return input; }

        return input[payloadOffset..];
    }
}