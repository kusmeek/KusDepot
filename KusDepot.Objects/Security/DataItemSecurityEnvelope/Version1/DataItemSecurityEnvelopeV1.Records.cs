namespace KusDepot.Security.Data;

internal static partial class DataItemSecurityEnvelopeV1
{
    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/record[@name="RecipientEntry"]/*'/>*/ 
    private sealed record RecipientEntry(Byte RecipientFlags , String Thumbprint , Guid? ObjectId , ImmutableArray<Byte> PublicKeyHash , ImmutableArray<Byte> WrappedMaterial);

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/record[@name="Header"]/*'/>*/
    private sealed record Header(Byte VersionByte , Byte Flags , Byte AlgorithmSuite , Byte ChunkSizePower , UInt16 RecipientCount , UInt32 RecipientTableLength , UInt64 OriginalLength , Byte[] RootContextHash , Byte[] EnvelopeMetadataHash);

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/record[@name="DecryptionChunk"]/*'/>*/
    private sealed record DecryptionChunk(Byte[] Cipher , Byte[] Tag , UInt32 PlainLength , UInt32 CipherLength , UInt32 ChunkIndex);

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/record[@name="DecryptedChunk"]/*'/>*/
    private sealed record DecryptedChunk(Byte[] Plain , UInt32 PlainLength , UInt32 ChunkIndex);

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/record[@name="EncryptionChunk"]/*'/>*/
    private sealed record EncryptionChunk(Byte[] Plain , Int32 ReadLength , UInt32 ChunkIndex);

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/record[@name="EncryptedChunk"]/*'/>*/
    private sealed record EncryptedChunk(Byte[] Cipher , Byte[] Tag , Byte[] Lengths , Int32 CipherLength , UInt32 ChunkIndex);
}
