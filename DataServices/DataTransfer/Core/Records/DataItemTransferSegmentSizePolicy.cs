namespace KusDepot.Data.Transfer;

/**<include file='DataItemTransferSegmentSizePolicy.xml' path='DataItemTransferSegmentSizePolicy/record[@name="DataItemTransferSegmentSizePolicy"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record DataItemTransferSegmentSizePolicy
{
    /**<include file='DataItemTransferSegmentSizePolicy.xml' path='DataItemTransferSegmentSizePolicy/record[@name="DataItemTransferSegmentSizePolicy"]/property[@name="MinSegmentBytes"]/*'/>*/
    [JsonPropertyName("MinSegmentBytes")] [JsonInclude]
    public Int64 MinSegmentBytes { get; init; } = 1;

    /**<include file='DataItemTransferSegmentSizePolicy.xml' path='DataItemTransferSegmentSizePolicy/record[@name="DataItemTransferSegmentSizePolicy"]/property[@name="PreferredSegmentBytes"]/*'/>*/
    [JsonPropertyName("PreferredSegmentBytes")] [JsonInclude]
    public Int64 PreferredSegmentBytes { get; init; } = 64 * MiB;

    /**<include file='DataItemTransferSegmentSizePolicy.xml' path='DataItemTransferSegmentSizePolicy/record[@name="DataItemTransferSegmentSizePolicy"]/property[@name="MaxSegmentBytes"]/*'/>*/
    [JsonPropertyName("MaxSegmentBytes")] [JsonInclude]
    public Int64 MaxSegmentBytes { get; init; } = Int64.MaxValue;

    /**<include file='DataItemTransferSegmentSizePolicy.xml' path='DataItemTransferSegmentSizePolicy/record[@name="DataItemTransferSegmentSizePolicy"]/method[@name="Validate"]/*'/>*/
    public Boolean Validate() { return this.MinSegmentBytes > 0 && this.PreferredSegmentBytes >= this.MinSegmentBytes && this.MaxSegmentBytes >= this.PreferredSegmentBytes; }

    /**<include file='DataItemTransferSegmentSizePolicy.xml' path='DataItemTransferSegmentSizePolicy/record[@name="DataItemTransferSegmentSizePolicy"]/method[@name="Allows"]/*'/>*/
    public Boolean Allows(Int64 length) { return Validate() && length >= this.MinSegmentBytes && length <= this.MaxSegmentBytes; }
}
