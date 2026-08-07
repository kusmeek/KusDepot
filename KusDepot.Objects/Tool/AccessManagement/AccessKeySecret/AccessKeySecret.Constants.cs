namespace KusDepot.Security;

public static partial class AccessKeySecret
{
    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="AccessKeyRealmLengthSize"]/*'/>*/
    private const Int32 AccessKeyRealmLengthSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="AssertionCountSize"]/*'/>*/
    private const Int32 AssertionCountSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="AssertionLengthSize"]/*'/>*/
    private const Int32 AssertionLengthSize = 4;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="AudienceCountSize"]/*'/>*/
    private const Int32 AudienceCountSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="AudienceLengthSize"]/*'/>*/
    private const Int32 AudienceLengthSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="BitsPerBlock"]/*'/>*/
    internal const Int32 BitsPerBlock = 128;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="BytesPerBlock"]/*'/>*/
    internal const Int32 BytesPerBlock = BitsPerBlock / 8;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="CiphertextLengthSize"]/*'/>*/
    private const Int32 CiphertextLengthSize = 4;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="FixedPrefixBeforeSubject"]/*'/>*/
    private const Int32 FixedPrefixBeforeSubject = TimeStampSize + TimeStampSize + GuidSize + GuidSize + SubjectLengthSize;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="FlagsSize"]/*'/>*/
    private const Int32 FlagsSize = 1;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="GuidSize"]/*'/>*/
    private const Int32 GuidSize = 16;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="MaxOperations"]/*'/>*/
    internal const Int32 MaxOperations = MaxPermissionBlocks * BitsPerBlock;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="MaxPermissionBlocks"]/*'/>*/
    internal const Int32 MaxPermissionBlocks = 128;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="MinPlaintextLength"]/*'/>*/
    private const Int32 MinPlaintextLength = FixedPrefixBeforeSubject + ToolSchemaLengthSize + AccessKeyRealmLengthSize + ManifestHashSize + TokenIdSize + PermissionBlockCountSize + AudienceCountSize + ScopeCountSize;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="ManifestHashSize"]/*'/>*/
    private const Int32 ManifestHashSize = 64;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="NonceSize"]/*'/>*/
    private const Int32 NonceSize = 12;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="OuterHeaderFixedSize"]/*'/>*/
    private const Int32 OuterHeaderFixedSize = VersionSize + FlagsSize + SignatureAlgorithmSize + SignerKeyIdFormatSize + ReservedSize + SignerToolIdSize + SignerAccessManagerIdSize + SignerKeyIdLengthSize + SignatureLengthSize + CiphertextLengthSize + NonceSize;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="PermissionBlockCountSize"]/*'/>*/
    private const Int32 PermissionBlockCountSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="ReservedSize"]/*'/>*/
    private const Int32 ReservedSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="ScopeCountSize"]/*'/>*/
    private const Int32 ScopeCountSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="ScopeLengthSize"]/*'/>*/
    private const Int32 ScopeLengthSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="SignedFlag"]/*'/>*/
    internal const Byte SignedFlag = 0x01;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="SignatureAlgorithmSize"]/*'/>*/
    private const Int32 SignatureAlgorithmSize = 1;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="SignatureLengthSize"]/*'/>*/
    private const Int32 SignatureLengthSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="SignerAccessManagerIdSize"]/*'/>*/
    private const Int32 SignerAccessManagerIdSize = GuidSize;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="SignerKeyIdFormatSize"]/*'/>*/
    private const Int32 SignerKeyIdFormatSize = 1;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="SignerKeyIdLengthSize"]/*'/>*/
    private const Int32 SignerKeyIdLengthSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="SignerToolIdSize"]/*'/>*/
    private const Int32 SignerToolIdSize = GuidSize;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="SymmetricKeySize"]/*'/>*/
    public const Int32 SymmetricKeySize = 32;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="SubjectLengthSize"]/*'/>*/
    private const Int32 SubjectLengthSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="ToolSchemaLengthSize"]/*'/>*/
    private const Int32 ToolSchemaLengthSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="TimeStampSize"]/*'/>*/
    private const Int32 TimeStampSize = 8;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="TagSize"]/*'/>*/
    private const Int32 TagSize = 16;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="TokenIdSize"]/*'/>*/
    internal const Int32 TokenIdSize = 64;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="Version"]/*'/>*/
    private const Byte Version = 0x01;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="VersionSize"]/*'/>*/
    private const Int32 VersionSize = 1;
}