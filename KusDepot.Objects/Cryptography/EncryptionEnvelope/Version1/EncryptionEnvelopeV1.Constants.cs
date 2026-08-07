namespace KusDepot.Cryptography;

internal static partial class EncryptionEnvelopeV1
{
    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="AadHashSize"]/*'/>*/
    private const Int32 AadHashSize = 64;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="AesKeySize"]/*'/>*/
    private const Int32 AesKeySize = 32;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="BaseNonceSeedSize"]/*'/>*/
    private const Int32 BaseNonceSeedSize = 16;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="ChunkLengthsBlockSize"]/*'/>*/
    private const Int32 ChunkLengthsBlockSize = 2 * IntSize;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="ChunkSizePowerSize"]/*'/>*/
    private const Int32 ChunkSizePowerSize = 1;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="DefaultChunkSizePower"]/*'/>*/
    private const Int32 DefaultChunkSizePower = 20;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="FixedHeaderSize"]/*'/>*/
    private const Int32 FixedHeaderSize = VersionSize + FlagsSize + RsaWrappedLenSize + ChunkSizePowerSize;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="Flag_HasAadHash"]/*'/>*/
    private const Byte Flag_HasAadHash = 1 << 1;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="Flag_HasOriginalLength"]/*'/>*/
    private const Byte Flag_HasOriginalLength = 1 << 0;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="FlagsSize"]/*'/>*/
    private const Int32 FlagsSize = 1;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="FooterMarker"]/*'/>*/
    private const Byte FooterMarker = 0xF0;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="FooterSize"]/*'/>*/
    private const Int32 FooterSize = 1 + IntSize;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="IntSize"]/*'/>*/
    private const Int32 IntSize = 4;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="LongSize"]/*'/>*/
    private const Int32 LongSize = 8;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="MaxChunkSizePower"]/*'/>*/
    private const Int32 MaxChunkSizePower = 26;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="MinChunkSizePower"]/*'/>*/
    private const Int32 MinChunkSizePower = 12;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="NonceSize"]/*'/>*/
    private const Int32 NonceSize = 12;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="PerChunkAadHashSize"]/*'/>*/
    private const Int32 PerChunkAadHashSize = 64;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="RsaWrappedLenSize"]/*'/>*/
    private const Int32 RsaWrappedLenSize = 2;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="TagSize"]/*'/>*/
    private const Int32 TagSize = 16;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="TagSizeLengthFieldSize"]/*'/>*/
    private const Int32 TagSizeLengthFieldSize = 1;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="Version"]/*'/>*/
    private const Byte Version = 0x01;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="VersionSize"]/*'/>*/
    private const Int32 VersionSize = 1;

    /**<include file='EncryptionEnvelopeV1.xml' path='EncryptionEnvelopeV1/class[@name="EncryptionEnvelopeV1"]/field[@name="WrappedMaterialSize"]/*'/>*/
    private const Int32 WrappedMaterialSize = AesKeySize + BaseNonceSeedSize;
}