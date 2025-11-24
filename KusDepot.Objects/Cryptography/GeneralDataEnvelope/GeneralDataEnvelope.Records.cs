namespace KusDepot.Cryptography;

internal static partial class GeneralDataEnvelope
{
    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/record[@name="DecryptionChunk"]/*'/>*/
    private record DecryptionChunk(Byte[] Cipher , Byte[] Tag , UInt32 PlainLength , UInt32 CipherLength , UInt32 ChunkIndex);

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/record[@name="DecryptedChunk"]/*'/>*/
    private record DecryptedChunk(Byte[] Plain , UInt32 PlainLength , UInt32 ChunkIndex);

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/record[@name="EncryptionChunk"]/*'/>*/
    private record EncryptionChunk(Byte[] Plain , Int32 ReadLength , UInt32 ChunkIndex);

    /**<include file='GeneralDataEnvelope.xml' path='GeneralDataEnvelope/class[@name="GeneralDataEnvelope"]/record[@name="EncryptedChunk"]/*'/>*/
    private record EncryptedChunk(Byte[] Cipher , Byte[] Tag , Byte[] Lengths , Int32 CipherLength , UInt32 ChunkIndex);
}