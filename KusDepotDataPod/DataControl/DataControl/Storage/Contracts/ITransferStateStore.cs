namespace DataPodServices.DataControl;

/**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/main/*'/>*/
public interface ITransferStateStore
{
    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="CommitSegmentedRangeAsync"]/*'/>*/
    Task<SegmentedTransferWriteResult> CommitSegmentedRangeAsync(SegmentedTransferRangeState range , TransferSessionState updatedsession , Int64 expectedstateversion , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="CommitStreamAppendAsync"]/*'/>*/
    Task<StreamTransferAppendResult> CommitStreamAppendAsync(StreamTransferChunkState chunk , TransferSessionState updatedsession , Int64 expectedstateversion , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="DeleteJournalAsync"]/*'/>*/
    Task<Boolean> DeleteJournalAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="DeleteJournalsForInstanceAsync"]/*'/>*/
    Task<Boolean> DeleteJournalsForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="DeleteSessionAsync"]/*'/>*/
    Task<Boolean> DeleteSessionAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="FindOpenConflictAsync"]/*'/>*/
    Task<DataItemTransferOpenConflict?> FindOpenConflictAsync(Guid itemid , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadJournalAsync"]/*'/>*/
    Task<TransferPublicationJournalState?> LoadJournalAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadJournalsForInstanceAsync"]/*'/>*/
    Task<IReadOnlyList<TransferPublicationJournalState>> LoadJournalsForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadRecoverableJournalCandidatesAsync"]/*'/>*/
    Task<IReadOnlyList<TransferPublicationRecoveryCandidate>> LoadRecoverableJournalCandidatesAsync(DateTimeOffset cutoff , TransferPublicationRecoveryCandidate? after , Int32 pageSize , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadRecoveryStateForAdminAsync"]/*'/>*/
    Task<TransferRecoveryStateSnapshot> LoadRecoveryStateForAdminAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadSegmentedRangesAsync"]/*'/>*/
    Task<IReadOnlyList<SegmentedTransferRangeState>> LoadSegmentedRangesAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadSegmentedCoverageAsync"]/*'/>*/
    Task<IReadOnlyList<DataItemTransferRange>> LoadSegmentedCoverageAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadSegmentedRangeAsync"]/*'/>*/
    Task<SegmentedTransferRangeState?> LoadSegmentedRangeAsync(Guid sessionid , Int64 offset , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="IsSegmentedRangeCoveredAsync"]/*'/>*/
    Task<Boolean> IsSegmentedRangeCoveredAsync(Guid sessionid , DataItemTransferRange range , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="HasSegmentedRangeOverlapAsync"]/*'/>*/
    Task<Boolean> HasSegmentedRangeOverlapAsync(Guid sessionid , DataItemTransferRange range , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadSessionAsync"]/*'/>*/
    Task<TransferSessionState?> LoadSessionAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadStaleTransferSessionCandidatesAsync"]/*'/>*/
    Task<IReadOnlyList<TransferSessionCleanupCandidate>> LoadStaleTransferSessionCandidatesAsync(String catalogname , DateTimeOffset cutoff , TransferSessionCleanupCandidate? after , Int32 pageSize , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="HasDependentSessionAsync"]/*'/>*/
    Task<Boolean> HasDependentSessionAsync(Guid sourcesessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="HasPublicationJournalAsync"]/*'/>*/
    Task<Boolean> HasPublicationJournalAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadSessionsAsync"]/*'/>*/
    Task<IReadOnlyList<TransferSessionState>> LoadSessionsAsync(TransferSessionQuery query , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadSessionsForAdminAsync"]/*'/>*/
    Task<IReadOnlyList<TransferSessionState>> LoadSessionsForAdminAsync(TransferSessionQuery? query = null , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadStreamChunksAsync"]/*'/>*/
    Task<IReadOnlyList<StreamTransferChunkState>> LoadStreamChunksAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="LoadStreamChunkAsync"]/*'/>*/
    Task<StreamTransferChunkState?> LoadStreamChunkAsync(Guid sessionid , Int64 offset , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="PromoteStagedReadSessionAsync"]/*'/>*/
    Task<Boolean> PromoteStagedReadSessionAsync(TransferSessionState session , Guid sourcesessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="PurgeRecoveryStateAsync"]/*'/>*/
    Task<Boolean> PurgeRecoveryStateAsync(Guid sessionid , String catalogname , Guid itemid , Int64 expectedsessionstateversion , Guid? expectedoperationid , TransferPublicationPhase? expectedphase , DateTimeOffset? expectedjournalupdated , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="SaveJournalAsync"]/*'/>*/
    Task SaveJournalAsync(TransferPublicationJournalState journal , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="SaveSessionAsync"]/*'/>*/
    Task SaveSessionAsync(TransferSessionState session , String catalogname , CancellationToken cancel = default);

    /**<include file='ITransferStateStore.xml' path='ITransferStateStore/interface[@name="ITransferStateStore"]/method[@name="TouchTransferSessionAsync"]/*'/>*/
    Task<Boolean> TouchTransferSessionAsync(Guid sessionid , Guid itemid , String catalogname , DateTimeOffset activity , CancellationToken cancel = default);
}