namespace KusDepot.Constants;

/**<include file='BufferSizes.xml' path='BufferSizes/class[@name="BufferSizes"]/main/*'/>*/
public static class BufferSizes
{
    /**<include file='BufferSizes.xml' path='BufferSizes/class[@name="BufferSizes"]/field[@name="CompressionBufferSize"]/*'/>*/
    public const Int32 CompressionBufferSize = 64 * KiB;

    /**<include file='BufferSizes.xml' path='BufferSizes/class[@name="BufferSizes"]/field[@name="ConversionBufferSize"]/*'/>*/
    public const Int32 ConversionBufferSize = 4 * KiB;

    /**<include file='BufferSizes.xml' path='BufferSizes/class[@name="BufferSizes"]/field[@name="DataHashingBufferSize"]/*'/>*/
    public const Int32 DataHashingBufferSize = 16 * KiB;

    /**<include file='BufferSizes.xml' path='BufferSizes/class[@name="BufferSizes"]/field[@name="ZeroMemoryBufferSize"]/*'/>*/
    public const Int32 ZeroMemoryBufferSize = 80 * KiB;
}
