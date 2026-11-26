namespace DataPodServices.DataControl;

/**<include file='TransferSessionQuery.xml' path='TransferSessionQuery/record[@name="TransferSessionQuery"]/main/*'/>*/
public sealed record TransferSessionQuery
{
    /**<include file='TransferSessionQuery.xml' path='TransferSessionQuery/record[@name="TransferSessionQuery"]/property[@name="ItemID"]/*'/>*/
    public Guid? ItemID { get; init; }

    /**<include file='TransferSessionQuery.xml' path='TransferSessionQuery/record[@name="TransferSessionQuery"]/property[@name="SessionID"]/*'/>*/
    public Guid? SessionID { get; init; }

    /**<include file='TransferSessionQuery.xml' path='TransferSessionQuery/record[@name="TransferSessionQuery"]/property[@name="CatalogName"]/*'/>*/
    public String? CatalogName { get; init; }

    /**<include file='TransferSessionQuery.xml' path='TransferSessionQuery/record[@name="TransferSessionQuery"]/property[@name="SourceSessionID"]/*'/>*/
    public Guid? SourceSessionID { get; init; }

    /**<include file='TransferSessionQuery.xml' path='TransferSessionQuery/record[@name="TransferSessionQuery"]/property[@name="Status"]/*'/>*/
    public DataItemTransferStatus? Status { get; init; }

    /**<include file='TransferSessionQuery.xml' path='TransferSessionQuery/record[@name="TransferSessionQuery"]/property[@name="Family"]/*'/>*/
    public DataControlTransferFamily? Family { get; init; }
}
