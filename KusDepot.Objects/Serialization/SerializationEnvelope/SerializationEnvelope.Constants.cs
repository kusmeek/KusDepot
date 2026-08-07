namespace KusDepot.Serialization;

public static partial class SerializationEnvelope
{
    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="ByteSize"]/*'/>*/
    private const Int32 ByteSize = 1;

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="FormatVersion"]/*'/>*/
    private const Byte FormatVersion = 0x01;

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="Magic"]/*'/>*/
    private static readonly Byte[] Magic = "KD"u8.ToArray();

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="MagicOffset"]/*'/>*/
    private static readonly Int32 MagicOffset = 0;

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="MagicSize"]/*'/>*/
    private static readonly Int32 MagicSize = Magic.Length;

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="FormatVersionOffset"]/*'/>*/
    private static readonly Int32 FormatVersionOffset = MagicOffset + MagicSize;

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="FormatVersionSize"]/*'/>*/
    private static readonly Int32 FormatVersionSize = ByteSize;

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="SerializerKindOffset"]/*'/>*/
    private static readonly Int32 SerializerKindOffset = FormatVersionOffset + FormatVersionSize;

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="SerializerKindSize"]/*'/>*/
    private static readonly Int32 SerializerKindSize = ByteSize;

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="SerializerVersionLengthOffset"]/*'/>*/
    private static readonly Int32 SerializerVersionLengthOffset = SerializerKindOffset + SerializerKindSize;

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="SerializerVersionLengthSize"]/*'/>*/
    private static readonly Int32 SerializerVersionLengthSize = ByteSize;

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="SerializerVersionOffset"]/*'/>*/
    private static readonly Int32 SerializerVersionOffset = SerializerVersionLengthOffset + SerializerVersionLengthSize;

    /**<include file='SerializationEnvelope.xml' path='SerializationEnvelope/class[@name="SerializationEnvelope"]/field[@name="LibraryVersionLengthSize"]/*'/>*/
    private static readonly Int32 LibraryVersionLengthSize = ByteSize;
}