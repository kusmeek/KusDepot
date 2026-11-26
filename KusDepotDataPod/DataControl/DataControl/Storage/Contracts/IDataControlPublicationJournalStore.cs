namespace DataPodServices.DataControl;

/**<include file='IDataControlPublicationJournalStore.xml' path='IDataControlPublicationJournalStore/interface[@name="IDataControlPublicationJournalStore"]/main/*'/>*/
internal interface IDataControlPublicationJournalStore
{
    /**<include file='IDataControlPublicationJournalStore.xml' path='IDataControlPublicationJournalStore/interface[@name="IDataControlPublicationJournalStore"]/method[@name="AdvanceAsync"]/*'/>*/
    Task<TransferPublicationJournal> AdvanceAsync(Guid sessionid , Guid operationid , String catalogname , TransferPublicationPhase phase , String? error = null , TransferContentIntegrity? integrity = null , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationJournalStore.xml' path='IDataControlPublicationJournalStore/interface[@name="IDataControlPublicationJournalStore"]/method[@name="DeleteAsync"]/*'/>*/
    Task DeleteAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationJournalStore.xml' path='IDataControlPublicationJournalStore/interface[@name="IDataControlPublicationJournalStore"]/method[@name="DeleteForInstanceAsync"]/*'/>*/
    Task<Boolean> DeleteForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationJournalStore.xml' path='IDataControlPublicationJournalStore/interface[@name="IDataControlPublicationJournalStore"]/method[@name="GetOrCreateAsync"]/*'/>*/
    Task<TransferPublicationJournal> GetOrCreateAsync(TransferPublicationJournal journal , String catalogname , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationJournalStore.xml' path='IDataControlPublicationJournalStore/interface[@name="IDataControlPublicationJournalStore"]/method[@name="LoadAsync"]/*'/>*/
    Task<TransferPublicationJournal?> LoadAsync(Guid sessionid , String catalogname , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationJournalStore.xml' path='IDataControlPublicationJournalStore/interface[@name="IDataControlPublicationJournalStore"]/method[@name="LoadForInstanceAsync"]/*'/>*/
    Task<IReadOnlyList<TransferPublicationJournalState>> LoadForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationJournalStore.xml' path='IDataControlPublicationJournalStore/interface[@name="IDataControlPublicationJournalStore"]/method[@name="LoadRecoverableCandidatesAsync"]/*'/>*/
    Task<IReadOnlyList<TransferPublicationRecoveryCandidate>> LoadRecoverableCandidatesAsync(DateTimeOffset cutoff , TransferPublicationRecoveryCandidate? after , Int32 pageSize , CancellationToken cancel = default);

    /**<include file='IDataControlPublicationJournalStore.xml' path='IDataControlPublicationJournalStore/interface[@name="IDataControlPublicationJournalStore"]/method[@name="RecordFailureAsync"]/*'/>*/
    Task<TransferPublicationJournal> RecordFailureAsync(Guid sessionid , Guid operationid , String catalogname , String error , CancellationToken cancel = default);
}
