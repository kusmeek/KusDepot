namespace KusDepot.Cryptography;

internal static partial class EncryptionEnvelopeV1
{
    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/record[@name="DecryptionChunk"]/*'/>*/
    private record DecryptionChunk(Byte[] Cipher , Byte[] Tag , UInt32 PlainLength , UInt32 CipherLength , UInt32 ChunkIndex);

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/record[@name="DecryptedChunk"]/*'/>*/
    private record DecryptedChunk(Byte[] Plain , UInt32 PlainLength , UInt32 ChunkIndex);

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/record[@name="EncryptionChunk"]/*'/>*/
    private record EncryptionChunk(Byte[] Plain , Int32 ReadLength , UInt32 ChunkIndex);

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/record[@name="EncryptedChunk"]/*'/>*/
    private record EncryptedChunk(Byte[] Cipher , Byte[] Tag , Byte[] Lengths , Int32 CipherLength , UInt32 ChunkIndex);
}