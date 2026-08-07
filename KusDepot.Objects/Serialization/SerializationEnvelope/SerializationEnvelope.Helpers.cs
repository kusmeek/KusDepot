namespace KusDepot.Serialization;

public static partial class SerializationEnvelope
{
    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="GetSerializerVersionLength"]/*'/>*/
    private static Int32 GetSerializerVersionLength(Byte[] input)
    {
        return input[SerializerVersionLengthOffset];
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="GetSerializerVersionOffset"]/*'/>*/
    private static Int32 GetSerializerVersionOffset()
    {
        return SerializerVersionOffset;
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="GetLibraryVersionLengthOffset"]/*'/>*/
    private static Int32 GetLibraryVersionLengthOffset(Byte[] input)
    {
        return SerializerVersionOffset + GetSerializerVersionLength(input);
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="GetLibraryVersionLength"]/*'/>*/
    private static Int32 GetLibraryVersionLength(Byte[] input)
    {
        return input[GetLibraryVersionLengthOffset(input)];
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="GetLibraryVersionOffset"]/*'/>*/
    private static Int32 GetLibraryVersionOffset(Byte[] input)
    {
        return GetLibraryVersionLengthOffset(input) + LibraryVersionLengthSize;
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="GetPayloadOffset"]/*'/>*/
    private static Int32 GetPayloadOffset(Byte[] input)
    {
        return GetLibraryVersionOffset(input) + GetLibraryVersionLength(input);
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="ComputeHeaderSize"]/*'/>*/
    private static Int32 ComputeHeaderSize(SerializationIdentity identity)
    {
        return MagicSize + FormatVersionSize + SerializerKindSize + SerializerVersionLengthSize + identity.SerializerVersion.Length + LibraryVersionLengthSize + identity.LibraryVersion.Length;
    }

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/method[@name="BuildHeader"]/*'/>*/
    internal static Byte[] BuildHeader(SerializationIdentity identity)
    {
        Int32 headerSize = ComputeHeaderSize(identity);

        Byte[] header = new Byte[headerSize]; Int32 offset = 0;

        Magic.CopyTo(header,offset); offset += MagicSize;

        header[offset++] = FormatVersion;
        header[offset++] = (Byte)identity.Kind;
        header[offset++] = (Byte)identity.SerializerVersion.Length;

        identity.SerializerVersion.CopyTo(header,offset); offset += identity.SerializerVersion.Length;

        header[offset++] = (Byte)identity.LibraryVersion.Length;

        identity.LibraryVersion.CopyTo(header,offset);

        return header;
    }
}