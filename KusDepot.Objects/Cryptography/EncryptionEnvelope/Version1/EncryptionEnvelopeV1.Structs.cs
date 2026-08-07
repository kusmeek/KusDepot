namespace KusDepot.Cryptography;

/**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/main/*'/>*/
internal static partial class EncryptionEnvelopeV1
{
    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/struct[@name="Header"]/*'/>*/
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

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/struct[@name="Footer"]/*'/>*/
    private struct Footer { internal UInt32 ChunkCount; }
}