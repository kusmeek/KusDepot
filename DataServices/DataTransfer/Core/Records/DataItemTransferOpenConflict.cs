namespace KusDepot.Data.Services.DataTransfer;

/**<include file='DataItemTransferOpenConflict.xml' path='DataItemTransferOpenConflict/class[@name="DataItemTransferOpenConflict"]/main/*'/>*/
public sealed record DataItemTransferOpenConflict
{
    /**<include file='DataItemTransferOpenConflict.xml' path='DataItemTransferOpenConflict/class[@name="DataItemTransferOpenConflict"]/property[@name="SessionID"]/*'/>*/
    public required Guid SessionID { get; init; }

    /**<include file='DataItemTransferOpenConflict.xml' path='DataItemTransferOpenConflict/class[@name="DataItemTransferOpenConflict"]/property[@name="ItemID"]/*'/>*/
    public required Guid ItemID { get; init; }

    /**<include file='DataItemTransferOpenConflict.xml' path='DataItemTransferOpenConflict/class[@name="DataItemTransferOpenConflict"]/property[@name="TransferFamily"]/*'/>*/
    public required DataControlTransferFamily TransferFamily { get; init; }

    /**<include file='DataItemTransferOpenConflict.xml' path='DataItemTransferOpenConflict/class[@name="DataItemTransferOpenConflict"]/property[@name="Mode"]/*'/>*/
    public required DataItemTransferMode Mode { get; init; }

    /**<include file='DataItemTransferOpenConflict.xml' path='DataItemTransferOpenConflict/class[@name="DataItemTransferOpenConflict"]/property[@name="Status"]/*'/>*/
    public required DataItemTransferStatus Status { get; init; }

    /**<include file='DataItemTransferOpenConflict.xml' path='DataItemTransferOpenConflict/class[@name="DataItemTransferOpenConflict"]/property[@name="Updated"]/*'/>*/
    public required DateTimeOffset Updated { get; init; }
}