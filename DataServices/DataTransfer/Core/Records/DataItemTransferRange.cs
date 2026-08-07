namespace KusDepot.Data.Transfer;

/**<include file='DataItemTransferRange.xml' path='DataItemTransferRange/record[@name="DataItemTransferRange"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record DataItemTransferRange
{
    /**<include file='DataItemTransferRange.xml' path='DataItemTransferRange/record[@name="DataItemTransferRange"]/property[@name="Offset"]/*'/>*/
    [JsonPropertyName("Offset")] [JsonInclude]
    public Int64 Offset { get; init; }

    /**<include file='DataItemTransferRange.xml' path='DataItemTransferRange/record[@name="DataItemTransferRange"]/property[@name="Length"]/*'/>*/
    [JsonPropertyName("Length")] [JsonInclude]
    public Int64 Length { get; init; }

    /**<include file='DataItemTransferRange.xml' path='DataItemTransferRange/record[@name="DataItemTransferRange"]/property[@name="EndOffsetExclusive"]/*'/>*/
    public Int64 EndOffsetExclusive => this.Validate() ? this.Offset + this.Length : -1;

    /**<include file='DataItemTransferRange.xml' path='DataItemTransferRange/record[@name="DataItemTransferRange"]/property[@name="IsEmpty"]/*'/>*/
    public Boolean IsEmpty => this.Length == 0;

    /**<include file='DataItemTransferRange.xml' path='DataItemTransferRange/record[@name="DataItemTransferRange"]/method[@name="Validate"]/*'/>*/
    public Boolean Validate() { return this.Offset >= 0 && this.Length >= 0 && this.Offset <= Int64.MaxValue - this.Length; }

    /**<include file='DataItemTransferRange.xml' path='DataItemTransferRange/record[@name="DataItemTransferRange"]/method[@name="ContainsRange"]/*'/>*/
    public Boolean Contains(DataItemTransferRange? range)
    {
        return range is not null && this.Validate() && range.Validate() && range.Offset >= this.Offset && range.EndOffsetExclusive <= this.EndOffsetExclusive;
    }

    /**<include file='DataItemTransferRange.xml' path='DataItemTransferRange/record[@name="DataItemTransferRange"]/method[@name="Intersects"]/*'/>*/
    public Boolean Intersects(DataItemTransferRange? range)
    {
        return range is not null && this.Validate() && range.Validate() && this.Offset < range.EndOffsetExclusive && range.Offset < this.EndOffsetExclusive;
    }

    /**<include file='DataItemTransferRange.xml' path='DataItemTransferRange/record[@name="DataItemTransferRange"]/method[@name="CanMerge"]/*'/>*/
    public Boolean CanMerge(DataItemTransferRange? range)
    {
        return range is not null && this.Validate() && range.Validate() && this.EndOffsetExclusive >= range.Offset && range.EndOffsetExclusive >= this.Offset;
    }

    /**<include file='DataItemTransferRange.xml' path='DataItemTransferRange/record[@name="DataItemTransferRange"]/method[@name="Merge"]/*'/>*/
    public DataItemTransferRange Merge(DataItemTransferRange range)
    {
        if(!CanMerge(range)) { return this; }

        Int64 offset = Math.Min(this.Offset,range.Offset); Int64 end = Math.Max(this.EndOffsetExclusive,range.EndOffsetExclusive);

        return new() { Offset = offset , Length = end - offset };
    }
}
