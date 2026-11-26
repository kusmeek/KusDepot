namespace KusDepot.Constants;

/**<include file='SecurityConstants.xml' path='SecurityConstants/class[@name="SecurityConstants"]/main/*'/>*/
public static class SecurityConstants
{
    /**<include file='SecurityConstants.xml' path='SecurityConstants/class[@name="SecurityConstants"]/field[@name="DataSecurityCNKey"]/*'/>*/
    public const String DataSecurityCNKey = "Content";

    /**<include file='SecurityConstants.xml' path='SecurityConstants/class[@name="SecurityConstants"]/field[@name="DataSecurityEDKey"]/*'/>*/
    public const String DataSecurityEDKey = "EncryptedData";

    /**<include file='SecurityConstants.xml' path='SecurityConstants/class[@name="SecurityConstants"]/field[@name="DataSecurityHCKey"]/*'/>*/
    public const String DataSecurityHCKey = "HashCode";

    /**<include file='SecurityConstants.xml' path='SecurityConstants/class[@name="SecurityConstants"]/field[@name="DataSecurityMDKey"]/*'/>*/
    public const String DataSecurityMDKey = "MetaData";
}
