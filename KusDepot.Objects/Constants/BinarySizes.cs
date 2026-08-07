namespace KusDepot.Constants;

/**<include file='BinarySizes.xml' path='BinarySizes/class[@name="BinarySizes"]/main/*'/>*/
public static class BinarySizes
{
    /**<include file='BinarySizes.xml' path='BinarySizes/class[@name="BinarySizes"]/field[@name="KiB"]/*'/>*/
    public const Int32 KiB = 1024;

    /**<include file='BinarySizes.xml' path='BinarySizes/class[@name="BinarySizes"]/field[@name="MiB"]/*'/>*/
    public const Int32 MiB = 1024 * KiB;

    /**<include file='BinarySizes.xml' path='BinarySizes/class[@name="BinarySizes"]/field[@name="GiB"]/*'/>*/
    public const Int32 GiB = 1024 * MiB;

    /**<include file='BinarySizes.xml' path='BinarySizes/class[@name="BinarySizes"]/field[@name="TiB"]/*'/>*/
    public const Int64 TiB = 1024L * GiB;

    /**<include file='BinarySizes.xml' path='BinarySizes/class[@name="BinarySizes"]/field[@name="PiB"]/*'/>*/
    public const Int64 PiB = 1024L * TiB;
}
