namespace KusDepot;

public static partial class AccessKeySecret
{
    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="BitsPerBlock"]/*'/>*/
    internal const Int32 BitsPerBlock = 128;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="BytesPerBlock"]/*'/>*/
    internal const Int32 BytesPerBlock = BitsPerBlock / 8;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="FixedPrefixBeforeSubject"]/*'/>*/
    private const Int32 FixedPrefixBeforeSubject = VersionSize + NonceSize + GuidSize + GuidSize + TimeStampSize + TimeStampSize + SubjectLengthSize;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="FixedTailSize"]/*'/>*/
    private const Int32 FixedTailSize = SaltSize + TokenHashSize;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="GuidSize"]/*'/>*/
    private const Int32 GuidSize = 16;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="MaxOperations"]/*'/>*/
    internal const Int32 MaxOperations = MaxPermissionBlocks * BitsPerBlock;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="MaxPermissionBlocks"]/*'/>*/
    internal const Int32 MaxPermissionBlocks = 128;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="MinPlaintextLength"]/*'/>*/
    private const Int32 MinPlaintextLength = FixedPrefixBeforeSubject + PermissionBlockCountSize + FixedTailSize;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="NonceSize"]/*'/>*/
    private const Int32 NonceSize = 32;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="PermissionBlockCountSize"]/*'/>*/
    private const Int32 PermissionBlockCountSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="SaltSize"]/*'/>*/
    private const Int32 SaltSize = 16;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="SubjectLengthSize"]/*'/>*/
    private const Int32 SubjectLengthSize = 2;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="TimeStampSize"]/*'/>*/
    private const Int32 TimeStampSize = 8;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="TokenHashSize"]/*'/>*/
    private const Int32 TokenHashSize = 64;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="Version"]/*'/>*/
    private const Byte Version = 0x01;

    /**<include file='AccessKeySecret.xml' path='AccessKeySecret/class[@name="AccessKeySecret"]/field[@name="VersionSize"]/*'/>*/
    private const Int32 VersionSize = 1;
}