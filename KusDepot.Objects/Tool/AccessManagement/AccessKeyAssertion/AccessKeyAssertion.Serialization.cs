namespace KusDepot.Security.Assertions;

public abstract partial record class AccessKeyAssertion
{
    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/method[@name="Deserialize"]/*'/>*/
    public static AccessKeyAssertion? Deserialize(Byte[]? input)
    {
        return input is null ? null : TryDeserialize(input.AsMemory(),out AccessKeyAssertion? assertion) ? assertion : null;
    }

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/method[@name="DeserializeMemory"]/*'/>*/
    public static AccessKeyAssertion? Deserialize(ReadOnlyMemory<Byte> input)
    {
        return TryDeserialize(input,out AccessKeyAssertion? assertion) ? assertion : null;
    }

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/method[@name="Serialize"]/*'/>*/
    public Byte[] Serialize()
    {
        Byte[] payload = SerializePayload();

        if(String.IsNullOrWhiteSpace(SerializationIdentifier) || payload.Length == 0) { return Array.Empty<Byte>(); }

        return WriteEnvelope(EnvelopeVersion,SerializationIdentifier,payload);
    }

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/method[@name="SerializePayload"]/*'/>*/
    protected abstract Byte[] SerializePayload();

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/method[@name="ToSummary"]/*'/>*/
    public abstract AccessKeyAssertionSummary ToSummary();

    ///<inheritdoc/>
    public sealed override String ToString() => Convert.ToBase64String(this.Serialize());

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/method[@name="TryDeserializeMemory"]/*'/>*/
    public static Boolean TryDeserialize(ReadOnlyMemory<Byte> input , [MaybeNullWhen(false)] out AccessKeyAssertion? assertion)
    {
        assertion = null;

        try
        {
            if(!TryReadEnvelope(input,out AccessKeyAssertionEnvelope envelope)) { return false; }

            if(!Deserializers.TryGetValue(envelope.SerializationIdentifier,out Func<ReadOnlyMemory<Byte>,AccessKeyAssertion?>? deserializer)) { return false; }

            assertion = deserializer(envelope.Payload);

            return assertion is not null;
        }
        catch { assertion = null; return false; }
    }

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/method[@name="TryDeserialize"]/*'/>*/
    public static Boolean TryDeserialize(ReadOnlySpan<Byte> input , [MaybeNullWhen(false)] out AccessKeyAssertion? assertion)
    {
        return TryDeserialize(input.ToArray().AsMemory(),out assertion);
    }

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/method[@name="TryReadEnvelope"]/*'/>*/
    private static Boolean TryReadEnvelope(ReadOnlyMemory<Byte> input , out AccessKeyAssertionEnvelope envelope)
    {
        envelope = default;

        ReadOnlySpan<Byte> span = input.Span;

        if(span.Length < sizeof(Byte) + SerializationIdentifierLengthSize + PayloadLengthSize) { return false; }

        Byte version = span[0]; if(version != EnvelopeVersion) { return false; }

        UInt16 identifierlength = ReadUInt16BigEndian(span.Slice(sizeof(Byte),SerializationIdentifierLengthSize));

        Int32 identifieroffset = sizeof(Byte) + SerializationIdentifierLengthSize;

        Int32 payloadoffset = identifieroffset + identifierlength;

        if(span.Length < payloadoffset + PayloadLengthSize) { return false; }

        UInt32 payloadlength = ReadUInt32BigEndian(span.Slice(payloadoffset,PayloadLengthSize));

        Int32 payloadstart = payloadoffset + PayloadLengthSize;

        if(payloadlength > Int32.MaxValue || span.Length != payloadstart + (Int32)payloadlength) { return false; }

        ReadOnlySpan<Byte> identifierbytes = span.Slice(identifieroffset,identifierlength);

        if(!Utf8.IsValid(identifierbytes)) { return false; }

        envelope = new(version,Encoding.UTF8.GetString(identifierbytes),input.Slice(payloadstart,(Int32)payloadlength));

        return true;
    }

    /**<include file='AccessKeyAssertion.xml' path='AccessKeyAssertion/record[@name="AccessKeyAssertion"]/method[@name="WriteEnvelope"]/*'/>*/
    private static Byte[] WriteEnvelope(Byte version , String serializationidentifier , ReadOnlySpan<Byte> payload)
    {
        Byte[] identifierbytes = Encoding.UTF8.GetBytes(serializationidentifier);

        if(identifierbytes.Length > UInt16.MaxValue) { return Array.Empty<Byte>(); }

        Int32 length = sizeof(Byte) + SerializationIdentifierLengthSize + identifierbytes.Length + PayloadLengthSize + payload.Length;

        Byte[] buffer = new Byte[length]; buffer[0] = version;

        WriteUInt16BigEndian(buffer.AsSpan(sizeof(Byte),SerializationIdentifierLengthSize),(UInt16)identifierbytes.Length);

        identifierbytes.CopyTo(buffer.AsSpan(sizeof(Byte) + SerializationIdentifierLengthSize,identifierbytes.Length));

        Int32 payloadoffset = sizeof(Byte) + SerializationIdentifierLengthSize + identifierbytes.Length;

        WriteUInt32BigEndian(buffer.AsSpan(payloadoffset,PayloadLengthSize),(UInt32)payload.Length);

        payload.CopyTo(buffer.AsSpan(payloadoffset + PayloadLengthSize,payload.Length));

        return buffer;
    }
}
