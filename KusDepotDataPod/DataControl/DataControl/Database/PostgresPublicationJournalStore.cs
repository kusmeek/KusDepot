namespace DataPodServices.DataControl;

/**<include file='PostgresPublicationJournalStore.xml' path='PostgresPublicationJournalStore/class[@name="PostgresPublicationJournalStore"]/main/*'/>*/
internal sealed class PostgresPublicationJournalStore : IDataControlPublicationJournalStore
{
    /**<include file='PostgresPublicationJournalStore.xml' path='PostgresPublicationJournalStore/class[@name="PostgresPublicationJournalStore"]/field[@name="state"]/*'/>*/
    private readonly ITransferStateStore state;

    /**<include file='PostgresPublicationJournalStore.xml' path='PostgresPublicationJournalStore/class[@name="PostgresPublicationJournalStore"]/field[@name="dataConfigs"]/*'/>*/
    private readonly IDataControlDataConfigsClient dataConfigs;

    /**<include file='PostgresPublicationJournalStore.xml' path='PostgresPublicationJournalStore/class[@name="PostgresPublicationJournalStore"]/field[@name="storageFactory"]/*'/>*/
    private readonly ITenantTransferStorageFactory storageFactory;

    /**<include file='PostgresPublicationJournalStore.xml' path='PostgresPublicationJournalStore/class[@name="PostgresPublicationJournalStore"]/constructor[@name="PostgresPublicationJournalStore"]/*'/>*/
    public PostgresPublicationJournalStore(ITransferStateStore state , IDataControlDataConfigsClient dataConfigs , ITenantTransferStorageFactory storageFactory)
    {
        this.state = state ?? throw new ArgumentNullException(nameof(state));

        this.dataConfigs = dataConfigs ?? throw new ArgumentNullException(nameof(dataConfigs));

        this.storageFactory = storageFactory ?? throw new ArgumentNullException(nameof(storageFactory));
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<TransferPublicationRecoveryCandidate>> LoadRecoverableCandidatesAsync(DateTimeOffset cutoff , TransferPublicationRecoveryCandidate? after , Int32 pageSize , CancellationToken cancel = default)
    {
        return this.state.LoadRecoverableJournalCandidatesAsync(cutoff,after,pageSize,cancel);
    }

    /// <inheritdoc/>
    public async Task<TransferPublicationJournal?> LoadAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        TransferPublicationJournalState? journal = await this.state.LoadJournalAsync(sessionid,catalogname,cancel);

        return journal is null ? null : await ToJournalAsync(journal,cancel);
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<TransferPublicationJournalState>> LoadForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default)
    {
        return this.state.LoadJournalsForInstanceAsync(itemid,itemInstanceToken,catalogname,cancel);
    }

    /// <inheritdoc/>
    public Task<Boolean> DeleteForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default)
    {
        return this.state.DeleteJournalsForInstanceAsync(itemid,itemInstanceToken,catalogname,cancel);
    }

    /// <inheritdoc/>
    public async Task<TransferPublicationJournal> GetOrCreateAsync(TransferPublicationJournal journal , String catalogname , CancellationToken cancel = default)
    {
        if(!String.Equals(journal.CatalogName,catalogname,Ordinal)) { throw new InvalidOperationException("The publication journal catalog does not match the authorized catalog."); }

        TransferPublicationJournal? existing = await LoadAsync(journal.SessionID,catalogname,cancel);

        if(existing is not null) { return existing; }

        TransferBlobReference? finalObject = null;

        if(journal.FinalObjectPayload.Length > 0)
        {
            await using MemoryStream payload = new(journal.FinalObjectPayload,false);

            finalObject = await SaveFinalObjectAsync(journal,payload,cancel);
        }

        await this.state.SaveJournalAsync(ToState(journal,finalObject),catalogname,cancel);

        return journal;
    }

    /// <inheritdoc/>
    public async Task<TransferPublicationJournal> AdvanceAsync(Guid sessionid , Guid operationid , String catalogname , TransferPublicationPhase phase , String? error = null , TransferContentIntegrity? integrity = null , CancellationToken cancel = default)
    {
        TransferPublicationJournal journal = await LoadAsync(sessionid,catalogname,cancel) ?? throw new InvalidOperationException("The transfer publication journal was not found.");

        EnsureOperation(journal,operationid);

        TransferPublicationJournal updated = journal with { Phase = phase , LastError = error , Integrity = integrity ?? journal.Integrity , Updated = DateTimeOffset.UtcNow };

        await SaveStateAsync(updated,catalogname,cancel);

        return updated;
    }

    /// <inheritdoc/>
    public async Task<TransferPublicationJournal> RecordFailureAsync(Guid sessionid , Guid operationid , String catalogname , String error , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(error);

        TransferPublicationJournal journal = await LoadAsync(sessionid,catalogname,cancel) ?? throw new InvalidOperationException("The transfer publication journal was not found.");

        EnsureOperation(journal,operationid);

        TransferPublicationJournal updated = journal with { FailureCount = journal.FailureCount + 1 , LastError = error , Updated = DateTimeOffset.UtcNow };

        await SaveStateAsync(updated,catalogname,cancel);

        return updated;
    }

    /// <inheritdoc/>
    public Task DeleteAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        return this.state.DeleteJournalAsync(sessionid,catalogname,cancel);
    }

    /**<include file='PostgresPublicationJournalStore.xml' path='PostgresPublicationJournalStore/class[@name="PostgresPublicationJournalStore"]/method[@name="EnsureOperation"]/*'/>*/
    private static void EnsureOperation(TransferPublicationJournal journal , Guid operationid)
    {
        if(journal.OperationID != operationid) { throw new InvalidOperationException("The transfer publication operation does not own the journal."); }
    }

    /**<include file='PostgresPublicationJournalStore.xml' path='PostgresPublicationJournalStore/class[@name="PostgresPublicationJournalStore"]/method[@name="SaveStateAsync"]/*'/>*/
    private async Task SaveStateAsync(TransferPublicationJournal journal , String catalogname , CancellationToken cancel)
    {
        TransferBlobReference? finalObject = null;

        if(journal.FinalObjectPayload.Length > 0)
        {
            await using MemoryStream payload = new(journal.FinalObjectPayload,false);

            finalObject = await SaveFinalObjectAsync(journal,payload,cancel);
        }

        await this.state.SaveJournalAsync(ToState(journal,finalObject),catalogname,cancel);
    }

    /**<include file='PostgresPublicationJournalStore.xml' path='PostgresPublicationJournalStore/class[@name="PostgresPublicationJournalStore"]/method[@name="ToState"]/*'/>*/
    private static TransferPublicationJournalState ToState(TransferPublicationJournal journal , TransferBlobReference? finalObject)
    {
        return new()
        {
            SessionID = journal.SessionID,
            ItemID = journal.ItemID,
            ItemInstanceToken = journal.ItemInstanceToken,
            OperationID = journal.OperationID,
            Family = journal.Family,
            Phase = journal.Phase,
            ExpectedStateVersion = journal.ExpectedStateVersion,
            CatalogName = journal.CatalogName,
            FailureCount = journal.FailureCount,
            LastError = journal.LastError,
            FinalObject = finalObject,
            FinalStreamLength = journal.FinalStreamLength,
            Integrity = journal.Integrity,
            JournalJson = DataControlTransferSnapshot.SanitizeJournal(Encoding.UTF8.GetString(JsonUtility.Serialize(journal))),
            Created = journal.Created,
            Updated = journal.Updated,
        };
    }

    /**<include file='PostgresPublicationJournalStore.xml' path='PostgresPublicationJournalStore/class[@name="PostgresPublicationJournalStore"]/method[@name="SaveFinalObjectAsync"]/*'/>*/
    private async Task<TransferBlobReference> SaveFinalObjectAsync(TransferPublicationJournal journal , Stream payload , CancellationToken cancel)
    {
        StorageSilo? silo = await this.dataConfigs.ResolveRecoveryStorageAsync(journal.CatalogName,cancel);

        if(silo is null) { throw new InvalidOperationException("The publication journal final object storage is unavailable."); }

        TenantTransferBlobReference? reference = await this.storageFactory.Create(silo,journal.SessionID,journal.ItemID,journal.ItemInstanceToken).SavePublicationObjectAsync(journal.OperationID,payload,cancel);

        TransferBlobReference? finalObject = ToReference(reference);

        if(finalObject is null || !String.Equals(finalObject.BlobName,TransferStoragePath.Create(journal.ItemInstanceToken,journal.SessionID).CreatePublicationObjectName(journal.OperationID),Ordinal) ||
            finalObject.Length != journal.FinalObjectPayload.LongLength || !SHA512.HashData(journal.FinalObjectPayload).AsSpan().SequenceEqual(finalObject.SHA512))
        {
            throw new InvalidOperationException("The publication journal final object could not be saved or verified.");
        }

        return finalObject;
    }

    /**<include file='PostgresPublicationJournalStore.xml' path='PostgresPublicationJournalStore/class[@name="PostgresPublicationJournalStore"]/method[@name="ToJournalAsync"]/*'/>*/
    private async Task<TransferPublicationJournal> ToJournalAsync(TransferPublicationJournalState state , CancellationToken cancel)
    {
        TransferPublicationJournal? journal = null;

        if(String.IsNullOrWhiteSpace(state.JournalJson) is false)
        {
            JsonNode? parsed = JsonNode.Parse(state.JournalJson);

            if(parsed is JsonObject snapshot)
            {
                snapshot.Remove("FinalObjectBlobName");

                journal = JsonUtility.Deserialize<TransferPublicationJournal>(Encoding.UTF8.GetBytes(snapshot.ToJsonString()));
            }
        }

        if(state.FinalObject is not null)
        {
            if(journal is null || !String.Equals(state.FinalObject.BlobName,TransferStoragePath.Create(state.ItemInstanceToken,state.SessionID).CreatePublicationObjectName(state.OperationID),Ordinal) ||
                state.FinalObject.Length <= 0 || state.FinalObject.SHA512.Length != 64)
            {
                throw new InvalidOperationException("The publication journal final object reference is invalid.");
            }

            StorageSilo? silo = await this.dataConfigs.ResolveRecoveryStorageAsync(state.CatalogName,cancel);

            if(silo is null) { throw new InvalidOperationException("The publication journal final object storage is unavailable."); }

            await using Stream? payload = await this.storageFactory.Create(silo,state.SessionID,state.ItemID,state.ItemInstanceToken).OpenPublicationObjectAsync(state.OperationID,cancel);

            if(payload is null) { throw new InvalidOperationException("The publication journal final object was not found."); }

            Byte[] bytes = await ObjectPayloadReader.ReadAsync(payload,cancel);

            if(bytes.LongLength != state.FinalObject.Length || !SHA512.HashData(bytes).AsSpan().SequenceEqual(state.FinalObject.SHA512))
            {
                throw new InvalidOperationException("The publication journal final object does not match its stored reference.");
            }

            journal = journal with { FinalObjectPayload = bytes };
        }

        journal ??= new TransferPublicationJournal
        {
            SessionID = state.SessionID,
            ItemID = state.ItemID,
            ItemInstanceToken = state.ItemInstanceToken,
            OperationID = state.OperationID,
            Family = state.Family,
            Phase = state.Phase,
            ExpectedStateVersion = state.ExpectedStateVersion,
            CatalogName = state.CatalogName,
            FailureCount = state.FailureCount,
            LastError = state.LastError,
            FinalStreamLength = state.FinalStreamLength,
            Integrity = state.Integrity,
            Created = state.Created,
            Updated = state.Updated,
        };

        if(journal.SessionID != state.SessionID || journal.ItemID != state.ItemID || journal.ItemInstanceToken != state.ItemInstanceToken || journal.OperationID != state.OperationID || journal.Family != state.Family || !String.Equals(journal.CatalogName,state.CatalogName,Ordinal))
        {
            throw new InvalidOperationException("The publication journal payload does not match its persisted identity and item instance token.");
        }

        return journal;
    }

    /**<include file='PostgresPublicationJournalStore.xml' path='PostgresPublicationJournalStore/class[@name="PostgresPublicationJournalStore"]/method[@name="ToReference"]/*'/>*/
    private static TransferBlobReference? ToReference(TenantTransferBlobReference? reference)
    {
        return reference is null ? null : new TransferBlobReference
        {
            BlobName = reference.BlobName,
            Length = reference.Length,
            SHA512 = reference.SHA512.ToArray(),
        };
    }
}
