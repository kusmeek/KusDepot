namespace DataPodServices.DataControl;

/**<include file='TransferSegmentSizePolicyResolver.xml' path='TransferSegmentSizePolicyResolver/class[@name="TransferSegmentSizePolicyResolver"]/main/*'/>*/
internal static class TransferSegmentSizePolicyResolver
{
    /**<include file='TransferSegmentSizePolicyResolver.xml' path='TransferSegmentSizePolicyResolver/class[@name="TransferSegmentSizePolicyResolver"]/method[@name="Resolve"]/*'/>*/
    public static DataItemTransferSegmentSizePolicy Resolve(DataItemTransferSegmentSizePolicy? requested , DataItemTransferSegmentSizePolicy serverDefault)
    {
        if(serverDefault is null || serverDefault.Validate() is false) { throw new InvalidOperationException("The configured transfer segment size policy is invalid."); }

        if(requested is null) { return serverDefault; }

        if(requested.Validate() is false) { throw new ArgumentException("The requested transfer segment size policy is invalid.",nameof(requested)); }

        Int64 min = Math.Max(requested.MinSegmentBytes,serverDefault.MinSegmentBytes);

        Int64 max = Math.Min(requested.MaxSegmentBytes,serverDefault.MaxSegmentBytes);

        if(max < min)
        {
            throw new ArgumentException(
                $"The requested transfer segment size policy ({requested.MinSegmentBytes}..{requested.MaxSegmentBytes} bytes) does not overlap the server policy ({serverDefault.MinSegmentBytes}..{serverDefault.MaxSegmentBytes} bytes).",
                nameof(requested));
        }

        return new()
        {
            MinSegmentBytes = min,
            MaxSegmentBytes = max,
            PreferredSegmentBytes = Math.Clamp(requested.PreferredSegmentBytes,min,max),
        };
    }
}
