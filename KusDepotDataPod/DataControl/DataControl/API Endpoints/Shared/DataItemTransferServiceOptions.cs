namespace DataPodServices.DataControl;

/**<include file='DataItemTransferServiceOptions.xml' path='DataItemTransferServiceOptions/class[@name="DataItemTransferServiceOptions"]/main/*'/>*/
public sealed class DataItemTransferServiceOptions
{
    /**<include file='DataItemTransferServiceOptions.xml' path='DataItemTransferServiceOptions/class[@name="DataItemTransferServiceOptions"]/property[@name="DefaultSegmentSizePolicy"]/*'/>*/
    public DataItemTransferSegmentSizePolicy DefaultSegmentSizePolicy { get; set; } = new()
    {
        MinSegmentBytes = 1 , PreferredSegmentBytes = 64 * MiB , MaxSegmentBytes = 4000L * MiB
    };

    /**<include file='DataItemTransferServiceOptions.xml' path='DataItemTransferServiceOptions/class[@name="DataItemTransferServiceOptions"]/property[@name="EnforceMaximumSegmentBytes"]/*'/>*/
    public Boolean EnforceMaximumSegmentBytes { get; set; } = false;

    /**<include file='DataItemTransferServiceOptions.xml' path='DataItemTransferServiceOptions/class[@name="DataItemTransferServiceOptions"]/property[@name="EnforceMinimumSegmentBytes"]/*'/>*/
    public Boolean EnforceMinimumSegmentBytes { get; set; } = false;
}