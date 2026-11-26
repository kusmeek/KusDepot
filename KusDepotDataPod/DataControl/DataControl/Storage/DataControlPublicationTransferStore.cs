namespace DataPodServices.DataControl;

/**<include file='DataControlPublicationTransferStore.xml' path='DataControlPublicationTransferStore/class[@name="DataControlPublicationTransferStore"]/main/*'/>*/
internal sealed class DataControlPublicationTransferStore : IDataControlPublicationTransferStore
{
    /**<include file='DataControlPublicationTransferStore.xml' path='DataControlPublicationTransferStore/class[@name="DataControlPublicationTransferStore"]/field[@name="sessions"]/*'/>*/
    private readonly IDataControlTransferSessionStore sessions;

    /**<include file='DataControlPublicationTransferStore.xml' path='DataControlPublicationTransferStore/class[@name="DataControlPublicationTransferStore"]/field[@name="journals"]/*'/>*/
    private readonly IDataControlPublicationJournalStore journals;

    /**<include file='DataControlPublicationTransferStore.xml' path='DataControlPublicationTransferStore/class[@name="DataControlPublicationTransferStore"]/constructor[@name="DataControlPublicationTransferStore"]/*'/>*/
    public DataControlPublicationTransferStore(IDataControlTransferSessionStore sessions , IDataControlPublicationJournalStore journals)
    {
        this.sessions = sessions ?? throw new ArgumentNullException(nameof(sessions));

        this.journals = journals ?? throw new ArgumentNullException(nameof(journals));
    }

    /// <inheritdoc/>
    public Task<DataItemTransferManifest?> LoadSegmentedManifestAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return this.sessions.LoadSegmentedManifestAsync(sessionid,cancel);
    }

    /// <inheritdoc/>
    public Task<DataStreamTransferManifest?> LoadStreamManifestAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return this.sessions.LoadStreamManifestAsync(sessionid,cancel);
    }

    /// <inheritdoc/>
    public Task<Byte[]?> ReadObjectPayloadAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return this.sessions.ReadObjectPayloadAsync(sessionid,cancel);
    }

    /// <inheritdoc/>
    public Task<Stream?> OpenObjectAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return this.sessions.OpenObjectAsync(sessionid,cancel);
    }

    /// <inheritdoc/>
    public Task<Stream?> OpenSegmentedRangeAsync(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default)
    {
        return this.sessions.OpenSegmentedRangeAsync(sessionid,range,cancel);
    }

    /// <inheritdoc/>
    public Task<Stream?> OpenStreamRangeAsync(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default)
    {
        return this.sessions.OpenStreamRangeAsync(sessionid,range,cancel);
    }

    /// <inheritdoc/>
    public Task<TransferContentIntegrity?> RecomputeIntegrityAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return this.sessions.RecomputeIntegrityAsync(sessionid,cancel);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<TransferContentIntegritySegment>?> LoadIntegritySegmentsAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return this.sessions.LoadIntegritySegmentsAsync(sessionid,cancel);
    }

    public Task<TenantTransferBlobReference?> LoadObjectReferenceAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return this.sessions.LoadObjectReferenceAsync(sessionid,cancel);
    }

    public Task<TransferBlockPlan?> CreateSegmentedBlockPlanAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return this.sessions.CreateSegmentedBlockPlanAsync(sessionid,cancel);
    }

    public Task<TransferBlockPlan?> CreateStreamBlockPlanAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return this.sessions.CreateStreamBlockPlanAsync(sessionid,cancel);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<TransferPublicationRecoveryCandidate>> LoadRecoverableCandidatesAsync(DateTimeOffset cutoff , TransferPublicationRecoveryCandidate? after , Int32 pageSize , CancellationToken cancel = default)
    {
        return this.journals.LoadRecoverableCandidatesAsync(cutoff,after,pageSize,cancel);
    }

    /// <inheritdoc/>
    public Task<TransferPublicationJournal?> LoadJournalAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        return this.journals.LoadAsync(sessionid,catalogname,cancel);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<TransferPublicationJournalState>> LoadJournalsForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default)
    {
        return this.journals.LoadForInstanceAsync(itemid,itemInstanceToken,catalogname,cancel);
    }

    /// <inheritdoc/>
    public Task<TransferPublicationJournal> GetOrCreateJournalAsync(TransferPublicationJournal journal , CancellationToken cancel = default)
    {
        return this.journals.GetOrCreateAsync(journal,journal.CatalogName,cancel);
    }

    /// <inheritdoc/>
    public Task<TransferPublicationJournal> AdvanceJournalAsync(Guid sessionid , Guid operationid , String catalogname , TransferPublicationPhase phase , String? error = null , TransferContentIntegrity? integrity = null , CancellationToken cancel = default)
    {
        return this.journals.AdvanceAsync(sessionid: sessionid,operationid: operationid,catalogname: catalogname,phase: phase,error: error,integrity: integrity,cancel: cancel);
    }

    /// <inheritdoc/>
    public Task<TransferPublicationJournal> RecordJournalFailureAsync(Guid sessionid , Guid operationid , String catalogname , String error , CancellationToken cancel = default)
    {
        return this.journals.RecordFailureAsync(sessionid,operationid,catalogname,error,cancel);
    }

    /// <inheritdoc/>
    public Task DeleteJournalAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        return this.journals.DeleteAsync(sessionid,catalogname,cancel);
    }

    /// <inheritdoc/>
    public Task<Boolean> DeleteJournalsForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default)
    {
        return this.journals.DeleteForInstanceAsync(itemid,itemInstanceToken,catalogname,cancel);
    }
}
