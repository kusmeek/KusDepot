namespace KusDepot.Data.Transfer;

public static partial class TransferEnvelope
{
    private const Int32 MagicOffset = 0;

    private const Int32 FormatVersionOffset = MagicOffset + sizeof(UInt32);

    private const Int32 FlagsOffset = FormatVersionOffset + sizeof(Byte);

    private const Int32 HeaderLengthOffset = FlagsOffset + sizeof(Byte);

    private const Int32 MetadataLengthOffset = HeaderLengthOffset + sizeof(UInt16);

    private const Int32 PayloadLengthOffset = MetadataLengthOffset + sizeof(Int32);

    private const Int32 TrailerLengthOffset = PayloadLengthOffset + sizeof(Int64);

    private const Int32 ReservedOffset = TrailerLengthOffset + sizeof(Int32);

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/field[@name="MediaType"]/*'/>*/
    public const String MediaType = "application/vnd.kusdepot.segment";

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/field[@name="FixedHeaderLength"]/*'/>*/
    private const UInt16 FixedHeaderLength = 28;

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/field[@name="FlagsNone"]/*'/>*/
    private const Byte FlagsNone = 0x00;

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/field[@name="FormatVersion"]/*'/>*/
    private const Byte FormatVersion = 0x01;

    /**<include file='TransferEnvelope.xml' path='TransferEnvelope/class[@name="TransferEnvelope"]/field[@name="Magic"]/*'/>*/
    private const UInt32 Magic = 0x4B445347;
}