namespace KusDepot.Data.Services.DataTransfer;

/**<include file='DataItemTransferServiceOptions.xml' path='DataItemTransferServiceOptions/class[@name="DataItemTransferServiceOptions"]/main/*'/>*/
public sealed class DataItemTransferServiceOptions
{
    /**<include file='DataItemTransferServiceOptions.xml' path='DataItemTransferServiceOptions/class[@name="DataItemTransferServiceOptions"]/property[@name="DefaultSegmentSizePolicy"]/*'/>*/
    public DataItemTransferSegmentSizePolicy DefaultSegmentSizePolicy { get; set; } = new()
    {
        MinSegmentBytes = 1 , PreferredSegmentBytes = 64 * MiB , MaxSegmentBytes = Int64.MaxValue
    };

    /**<include file='DataItemTransferServiceOptions.xml' path='DataItemTransferServiceOptions/class[@name="DataItemTransferServiceOptions"]/property[@name="EnforceMaximumSegmentBytes"]/*'/>*/
    public Boolean EnforceMaximumSegmentBytes { get; set; } = false;

    /**<include file='DataItemTransferServiceOptions.xml' path='DataItemTransferServiceOptions/class[@name="DataItemTransferServiceOptions"]/property[@name="EnforceMinimumSegmentBytes"]/*'/>*/
    public Boolean EnforceMinimumSegmentBytes { get; set; } = false;

    /**<include file='DataItemTransferServiceOptions.xml' path='DataItemTransferServiceOptions/class[@name="DataItemTransferServiceOptions"]/property[@name="TemporaryUploadPath"]/*'/>*/
    public String? TemporaryUploadPath { get; set; } = null;
}