namespace KusDepot;

public sealed partial class CommonSecret
{
    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="AesKeySize"]/*'/>*/
    private const Int32 AesKeySize = 32;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="GuidSize"]/*'/>*/
    private const Int32 GuidSize = 16;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="HashSize"]/*'/>*/
    private const Int32 HashSize = 64;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="HeaderPrefixSize"]/*'/>*/
    private const Int32 HeaderPrefixSize = VersionSize + RsaLengthSize;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="MaxPlaintext"]/*'/>*/
    private const Int32 MaxPlaintext = 4096;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="MaxSerialNumberLength"]/*'/>*/
    private const Int32 MaxSerialNumberLength = 255;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="NonceSize"]/*'/>*/
    private const Int32 NonceSize = 12;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="PlaintextPrefixSize"]/*'/>*/
    private const Int32 PlaintextPrefixSize = 2;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="RsaKeyLimit"]/*'/>*/
    private const Int32 RsaKeyLimit = 1024;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="RsaLengthSize"]/*'/>*/
    private const Int32 RsaLengthSize = 2;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="SaltSize"]/*'/>*/
    private const Int32 SaltSize = 16;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="TagSize"]/*'/>*/
    private const Int32 TagSize = 16;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="V1"]/*'/>*/
    private const Byte V1 = 0x01;

    /**<include file='CommonSecret.xml' path='CommonSecret/class[@name="CommonSecret"]/field[@name="VersionSize"]/*'/>*/
    private const Int32 VersionSize = 1;
}