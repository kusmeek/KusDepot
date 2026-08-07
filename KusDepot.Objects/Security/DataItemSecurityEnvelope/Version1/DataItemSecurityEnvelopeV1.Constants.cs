namespace KusDepot.Security.Data;

internal static partial class DataItemSecurityEnvelopeV1
{
    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="AesKeySize"]/*'/>*/
    private const Int32 AesKeySize = 32;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="AlgorithmSuiteRsaOaepSha512Aes256Gcm"]/*'/>*/
    private const Byte AlgorithmSuiteRsaOaepSha512Aes256Gcm = 0x01;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="BaseNonceSeedSize"]/*'/>*/
    private const Int32 BaseNonceSeedSize = 16;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="ChunkLengthBlockSize"]/*'/>*/
    private const Int32 ChunkLengthBlockSize = 8;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="DefaultChunkSizePower"]/*'/>*/
    private const Int32 DefaultChunkSizePower = 20;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="FixedHeaderSize"]/*'/>*/
    private const Int32 FixedHeaderSize = 146;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="EnvelopeMetadataHashSize"]/*'/>*/
    private const Int32 EnvelopeMetadataHashSize = 64;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="GuidSize"]/*'/>*/
    private const Int32 GuidSize = 16;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="MaxChunkSizePower"]/*'/>*/
    private const Int32 MaxChunkSizePower = 26;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="MinChunkSizePower"]/*'/>*/
    private const Int32 MinChunkSizePower = 12;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="PublicKeyHashSize"]/*'/>*/
    private const Int32 PublicKeyHashSize = 32;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="RecipientCountSize"]/*'/>*/
    private const Int32 RecipientCountSize = 2;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="RecipientObjectIdFlag"]/*'/>*/
    private const Byte RecipientObjectIdFlag = 1 << 0;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="RecipientPublicKeyHashFlag"]/*'/>*/
    private const Byte RecipientPublicKeyHashFlag = 1 << 1;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="RecipientTableLengthSize"]/*'/>*/
    private const Int32 RecipientTableLengthSize = 4;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="RootContextHashSize"]/*'/>*/
    private const Int32 RootContextHashSize = 64;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="TagSize"]/*'/>*/
    private const Int32 TagSize = 16;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="TagSizeLengthFieldSize"]/*'/>*/
    private const Int32 TagSizeLengthFieldSize = 1;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="Version"]/*'/>*/
    private const Byte Version = 0x01;

    /**<include file='DataItemSecurityEnvelopeV1.xml' path='DataItemSecurityEnvelopeV1/class[@name="DataItemSecurityEnvelopeV1"]/field[@name="WrappedMaterialSize"]/*'/>*/
    private const Int32 WrappedMaterialSize = AesKeySize + BaseNonceSeedSize;
}
