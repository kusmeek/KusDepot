namespace KusDepot.Data.Transfer;

/**<include file='TransferEnvelopeHeader.xml' path='TransferEnvelopeHeader/struct[@name="TransferEnvelopeHeader"]/main/*'/>*/
public readonly record struct TransferEnvelopeHeader
{
    /**<include file='TransferEnvelopeHeader.xml' path='TransferEnvelopeHeader/struct[@name="TransferEnvelopeHeader"]/field[@name="Magic"]/*'/>*/
    public UInt32 Magic { get; init; }

    /**<include file='TransferEnvelopeHeader.xml' path='TransferEnvelopeHeader/struct[@name="TransferEnvelopeHeader"]/field[@name="FormatVersion"]/*'/>*/
    public Byte FormatVersion { get; init; }

    /**<include file='TransferEnvelopeHeader.xml' path='TransferEnvelopeHeader/struct[@name="TransferEnvelopeHeader"]/field[@name="Flags"]/*'/>*/
    public Byte Flags { get; init; }

    /**<include file='TransferEnvelopeHeader.xml' path='TransferEnvelopeHeader/struct[@name="TransferEnvelopeHeader"]/field[@name="HeaderLength"]/*'/>*/
    public UInt16 HeaderLength { get; init; }

    /**<include file='TransferEnvelopeHeader.xml' path='TransferEnvelopeHeader/struct[@name="TransferEnvelopeHeader"]/field[@name="MetadataLength"]/*'/>*/
    public Int32 MetadataLength { get; init; }

    /**<include file='TransferEnvelopeHeader.xml' path='TransferEnvelopeHeader/struct[@name="TransferEnvelopeHeader"]/field[@name="PayloadLength"]/*'/>*/
    public Int64 PayloadLength { get; init; }

    /**<include file='TransferEnvelopeHeader.xml' path='TransferEnvelopeHeader/struct[@name="TransferEnvelopeHeader"]/field[@name="TrailerLength"]/*'/>*/
    public Int32 TrailerLength { get; init; }

    /**<include file='TransferEnvelopeHeader.xml' path='TransferEnvelopeHeader/struct[@name="TransferEnvelopeHeader"]/field[@name="Reserved"]/*'/>*/
    public Int32 Reserved { get; init; }

    /**<include file='TransferEnvelopeHeader.xml' path='TransferEnvelopeHeader/struct[@name="TransferEnvelopeHeader"]/constructor[@name="Constructor"]/*'/>*/
    public TransferEnvelopeHeader(UInt32 magic , Byte formatversion , Byte flags , UInt16 headerlength , Int32 metadatalength , Int64 payloadlength , Int32 trailerlength , Int32 reserved)
    {
        Magic = magic; FormatVersion = formatversion; Flags = flags; HeaderLength = headerlength;

        MetadataLength = metadatalength; PayloadLength = payloadlength; TrailerLength = trailerlength; Reserved = reserved;
    }
}