namespace DataPodServices.DataControl;

/**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/main/*'/>*/
internal interface IDataControlPublicationTransferStore
{
    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="AdvanceJournalAsync"]/*'/>*/
    Task<TransferPublicationJournal> AdvanceJournalAsync(Guid sessionid , Guid operationid , String catalogname , TransferPublicationPhase phase , String? error = null , TransferContentIntegrity? integrity = null , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="CreateSegmentedBlockPlanAsync"]/*'/>*/
    Task<TransferBlockPlan?> CreateSegmentedBlockPlanAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="CreateStreamBlockPlanAsync"]/*'/>*/
    Task<TransferBlockPlan?> CreateStreamBlockPlanAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="DeleteJournalAsync"]/*'/>*/
    Task DeleteJournalAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="DeleteJournalsForInstanceAsync"]/*'/>*/
    Task<Boolean> DeleteJournalsForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="GetOrCreateJournalAsync"]/*'/>*/
    Task<TransferPublicationJournal> GetOrCreateJournalAsync(TransferPublicationJournal journal , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="LoadIntegritySegmentsAsync"]/*'/>*/
    Task<IReadOnlyList<TransferContentIntegritySegment>?> LoadIntegritySegmentsAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="LoadJournalAsync"]/*'/>*/
    Task<TransferPublicationJournal?> LoadJournalAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="LoadJournalsForInstanceAsync"]/*'/>*/
    Task<IReadOnlyList<TransferPublicationJournalState>> LoadJournalsForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="LoadObjectReferenceAsync"]/*'/>*/
    Task<TenantTransferBlobReference?> LoadObjectReferenceAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="LoadRecoverableCandidatesAsync"]/*'/>*/
    Task<IReadOnlyList<TransferPublicationRecoveryCandidate>> LoadRecoverableCandidatesAsync(DateTimeOffset cutoff , TransferPublicationRecoveryCandidate? after , Int32 pageSize , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="LoadSegmentedManifestAsync"]/*'/>*/
    Task<DataItemTransferManifest?> LoadSegmentedManifestAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="LoadStreamManifestAsync"]/*'/>*/
    Task<DataStreamTransferManifest?> LoadStreamManifestAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="OpenObjectAsync"]/*'/>*/
    Task<Stream?> OpenObjectAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="OpenSegmentedRangeAsync"]/*'/>*/
    Task<Stream?> OpenSegmentedRangeAsync(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="OpenStreamRangeAsync"]/*'/>*/
    Task<Stream?> OpenStreamRangeAsync(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="ReadObjectPayloadAsync"]/*'/>*/
    Task<Byte[]?> ReadObjectPayloadAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="RecomputeIntegrityAsync"]/*'/>*/
    Task<TransferContentIntegrity?> RecomputeIntegrityAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationTransferStore.xml' path='IDataControlPublicationTransferStore/interface[@name="IDataControlPublicationTransferStore"]/method[@name="RecordJournalFailureAsync"]/*'/>*/
    Task<TransferPublicationJournal> RecordJournalFailureAsync(Guid sessionid , Guid operationid , String catalogname , String error , CancellationToken cancel = default);
}
