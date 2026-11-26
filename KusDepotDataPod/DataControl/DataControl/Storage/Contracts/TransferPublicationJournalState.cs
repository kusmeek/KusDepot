namespace DataPodServices.DataControl;

/**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/main/*'/>*/
public sealed record TransferPublicationJournalState
{
    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="SessionID"]/*'/>*/
    public Guid SessionID { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="ItemID"]/*'/>*/
    public Guid ItemID { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="ItemInstanceToken"]/*'/>*/
    public Guid ItemInstanceToken { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="OperationID"]/*'/>*/
    public Guid OperationID { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="Family"]/*'/>*/
    public TransferPublicationFamily Family { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="Phase"]/*'/>*/
    public TransferPublicationPhase Phase { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="ExpectedStateVersion"]/*'/>*/
    public Int64 ExpectedStateVersion { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="CatalogName"]/*'/>*/
    public String CatalogName { get; init; } = String.Empty;

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="FailureCount"]/*'/>*/
    public Int32 FailureCount { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="LastError"]/*'/>*/
    public String? LastError { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="FinalObject"]/*'/>*/
    public TransferBlobReference? FinalObject { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="FinalStreamLength"]/*'/>*/
    public Int64? FinalStreamLength { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="Integrity"]/*'/>*/
    public TransferContentIntegrity? Integrity { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="JournalJson"]/*'/>*/
    public String JournalJson { get; init; } = String.Empty;

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="Created"]/*'/>*/
    public DateTimeOffset Created { get; init; }

    /**<include file='TransferPublicationJournalState.xml' path='TransferPublicationJournalState/record[@name="TransferPublicationJournalState"]/property[@name="Updated"]/*'/>*/
    public DateTimeOffset Updated { get; init; }
}
