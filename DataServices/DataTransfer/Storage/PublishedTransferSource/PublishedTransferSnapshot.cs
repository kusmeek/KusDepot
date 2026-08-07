namespace KusDepot.Data.Services.DataTransfer;

/**<include file='PublishedTransferSnapshot.xml' path='PublishedTransferSnapshot/record[@name="PublishedTransferSnapshot"]/main/*'/>*/
public sealed record PublishedTransferSnapshot
{
    /**<include file='PublishedTransferSnapshot.xml' path='PublishedTransferSnapshot/record[@name="PublishedTransferSnapshot"]/property[@name="ItemID"]/*'/>*/
    public Guid ItemId { get; init; }

    /**<include file='PublishedTransferSnapshot.xml' path='PublishedTransferSnapshot/record[@name="PublishedTransferSnapshot"]/property[@name="ObjectInfo"]/*'/>*/
    public DataItemTransferObjectInfo? ObjectInfo { get; init; }

    /**<include file='PublishedTransferSnapshot.xml' path='PublishedTransferSnapshot/record[@name="PublishedTransferSnapshot"]/property[@name="ObjectPayload"]/*'/>*/
    public Byte[] ObjectPayload { get; init; } = Array.Empty<Byte>();

    /**<include file='PublishedTransferSnapshot.xml' path='PublishedTransferSnapshot/record[@name="PublishedTransferSnapshot"]/property[@name="ObjectSHA512"]/*'/>*/
    public Byte[] ObjectSHA512 { get; init; } = Array.Empty<Byte>();

    /**<include file='PublishedTransferSnapshot.xml' path='PublishedTransferSnapshot/record[@name="PublishedTransferSnapshot"]/property[@name="RealizedRanges"]/*'/>*/
    public DataItemTransferRange[] RealizedRanges { get; init; } = Array.Empty<DataItemTransferRange>();

    /**<include file='PublishedTransferSnapshot.xml' path='PublishedTransferSnapshot/record[@name="PublishedTransferSnapshot"]/property[@name="StreamLength"]/*'/>*/
    public Int64 StreamLength { get; init; }

    /**<include file='PublishedTransferSnapshot.xml' path='PublishedTransferSnapshot/record[@name="PublishedTransferSnapshot"]/property[@name="StreamSHA512"]/*'/>*/
    public Byte[] StreamSHA512 { get; init; } = Array.Empty<Byte>();
}