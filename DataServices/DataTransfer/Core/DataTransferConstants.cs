namespace KusDepot.Data.Services.DataTransfer;

/**<include file='DataTransferConstants.xml' path='DataTransferConstants/class[@name="DataTransferConstants"]/main/*'/>*/
public static class DataTransferConstants
{
    /**<include file='DataTransferConstants.xml' path='DataTransferConstants/class[@name="DataTransferConstants"]/field[@name="LargeObjectPayloadSize"]/*'/>*/
    public const Int32 LargeObjectPayloadSize = 10 * MiB;

    /**<include file='DataTransferConstants.xml' path='DataTransferConstants/class[@name="DataTransferConstants"]/field[@name="PublicationBlockLimitProblemType"]/*'/>*/
    public const String PublicationBlockLimitProblemType = "urn:kusdepot:transfer:publication-block-limit";
}
