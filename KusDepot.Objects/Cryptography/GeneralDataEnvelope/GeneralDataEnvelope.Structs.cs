namespace KusDepot.Cryptography;

/**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/main/*'/>*/
internal static partial class GeneralDataEnvelope
{
    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/struct[@name="Header"]/*'/>*/
    private struct Header
    {
        internal Byte    VersionByte;
        internal Byte    Flags;
        internal UInt16  RsaWrappedLen;
        internal Byte    ChunkSizePower;
        internal Boolean HasOriginalLength;
        internal Boolean HasAadHash;
        internal UInt64  OriginalLength;
        internal Byte[]? AadHash;
        internal Int32   HeaderTotalLength;
    }

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/struct[@name="Footer"]/*'/>*/
    private struct Footer { internal UInt32 ChunkCount; }
}