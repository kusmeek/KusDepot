namespace DataPodServices.DataControl;

/**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/enum[@name="DataControlDeletionProgress"]/main/*'/>*/
public enum DataControlDeletionProgress
{
    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/enum[@name="DataControlDeletionProgress"]/field[@name="CatalogPending"]/*'/>*/
    CatalogPending,
    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/enum[@name="DataControlDeletionProgress"]/field[@name="PublishedArtifactsRemoved"]/*'/>*/
    PublishedArtifactsRemoved,
    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/enum[@name="DataControlDeletionProgress"]/field[@name="TransferStateRemoved"]/*'/>*/
    TransferStateRemoved
}

/**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/record[@name="DataControlDeletionJobState"]/main/*'/>*/
public sealed record DataControlDeletionJobState
{
    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/record[@name="DataControlDeletionJobState"]/property[@name="CatalogName"]/*'/>*/
    public String CatalogName { get; init; } = String.Empty;

    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/record[@name="DataControlDeletionJobState"]/property[@name="ItemID"]/*'/>*/
    public Guid ItemID { get; init; }

    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/record[@name="DataControlDeletionJobState"]/property[@name="ItemInstanceToken"]/*'/>*/
    public Guid ItemInstanceToken { get; init; }

    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/record[@name="DataControlDeletionJobState"]/property[@name="Progress"]/*'/>*/
    public DataControlDeletionProgress Progress { get; init; }

    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/record[@name="DataControlDeletionJobState"]/property[@name="LastError"]/*'/>*/
    public String? LastError { get; init; }

    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/record[@name="DataControlDeletionJobState"]/property[@name="AttemptCount"]/*'/>*/
    public Int32 AttemptCount { get; init; }

    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/record[@name="DataControlDeletionJobState"]/property[@name="LastAttempt"]/*'/>*/
    public DateTimeOffset? LastAttempt { get; init; }

    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/record[@name="DataControlDeletionJobState"]/property[@name="NextAttempt"]/*'/>*/
    public DateTimeOffset? NextAttempt { get; init; }

    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/record[@name="DataControlDeletionJobState"]/property[@name="Created"]/*'/>*/
    public DateTimeOffset Created { get; init; }

    /**<include file='DataControlDeletionJobState.xml' path='DataControlDeletionJobState/record[@name="DataControlDeletionJobState"]/property[@name="Updated"]/*'/>*/
    public DateTimeOffset Updated { get; init; }
}
