namespace DataPodServices.DataControl;

/**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/main/*'/>*/
internal sealed class DataControlTransferSessionStore : IDataControlTransferSessionStore
{
    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/field[@name="state"]/*'/>*/
    private readonly ITransferStateStore state;

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/field[@name="storage"]/*'/>*/
    private readonly ITenantTransferStorageContext storage;

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/constructor[@name="DataControlTransferSessionStore"]/*'/>*/
    public DataControlTransferSessionStore(ITransferStateStore state , ITenantTransferStorageContext storage)
    {
        this.state = state ?? throw new ArgumentNullException(nameof(state));

        this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    /// <inheritdoc/>
    public async Task<DataItemTransferManifest?> LoadSegmentedManifestAsync(Guid sessionid , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is null || session.Family is not DataControlTransferFamily.Segmented) { return null; }

        DataItemTransferManifest? manifest = DataItemTransferManifest.Deserialize(Encoding.UTF8.GetBytes(session.ManifestJson));

        if(manifest is null) { return null; }

        IReadOnlyList<DataItemTransferRange> ranges = await this.state.LoadSegmentedCoverageAsync(sessionid,this.storage.CatalogName,cancel);

        return manifest.ItemInstanceToken == session.ItemInstanceToken
            ? manifest with { RealizedRanges = ranges.ToArray() , Integrity = null }
            : null;
    }

    /// <inheritdoc/>
    public async Task<DataItemTransferManifest?> LoadSegmentedWriteManifestAsync(Guid sessionid , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is null || session.Family is not DataControlTransferFamily.Segmented) { return null; }

        DataItemTransferManifest? manifest = DataItemTransferManifest.Parse(session.ManifestJson);

        return manifest?.ItemInstanceToken == session.ItemInstanceToken ? manifest with { RealizedRanges = Array.Empty<DataItemTransferRange>() , Integrity = null } : null;
    }

    /// <inheritdoc/>
    public Task<Boolean> IsSegmentedRangeCoveredAsync(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default)
    {
        return this.state.IsSegmentedRangeCoveredAsync(sessionid,range,this.storage.CatalogName,cancel);
    }

    /// <inheritdoc/>
    public async Task<DataStreamTransferManifest?> LoadStreamManifestAsync(Guid sessionid , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is null || session.Family is not DataControlTransferFamily.Stream) { return null; }

        DataStreamTransferManifest? manifest = DataStreamTransferManifest.Deserialize(Encoding.UTF8.GetBytes(session.ManifestJson));

        if(manifest is null) { return null; }

        Byte[]? objectPayload = await ReadObjectPayloadAsync(sessionid,cancel);

        return manifest.ItemInstanceToken == session.ItemInstanceToken
            ? manifest with { ObjectPayload = objectPayload ?? Array.Empty<Byte>() , Integrity = null }
            : null;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<TransferSessionState>> LoadSessionsAsync(TransferSessionQuery query , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if(String.IsNullOrWhiteSpace(query.CatalogName) || !String.Equals(query.CatalogName,this.storage.CatalogName,Ordinal)) { throw new InvalidOperationException("The transfer-session query catalog does not match the selected tenant catalog."); }

        return this.state.LoadSessionsAsync(query,cancel);
    }

    /// <inheritdoc/>
    public Task<TransferRecoveryStateSnapshot> LoadRecoveryStateForAdminAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return this.state.LoadRecoveryStateForAdminAsync(sessionid,cancel);
    }

    /// <inheritdoc/>
    public Task<Boolean> PurgeRecoveryStateAsync(Guid sessionid , Guid itemid , Int64 expectedsessionstateversion , Guid? expectedoperationid , TransferPublicationPhase? expectedphase , DateTimeOffset? expectedjournalupdated , CancellationToken cancel = default)
    {
        return this.state.PurgeRecoveryStateAsync(sessionid,this.storage.CatalogName,itemid,expectedsessionstateversion,expectedoperationid,expectedphase,expectedjournalupdated,cancel);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TransferSessionState>> LoadSessionsForAdminAsync(TransferSessionQuery? query = null , CancellationToken cancel = default)
    {
        IReadOnlyList<TransferSessionState> sessions = await this.state.LoadSessionsForAdminAsync(query,cancel);

        List<TransferSessionState> result = new(sessions.Count);

        foreach(TransferSessionState session in sessions)
        {
            DataItemTransferManifest? manifest = session.Family is DataControlTransferFamily.Segmented ? DataItemTransferManifest.Parse(session.ManifestJson) : null;

            if(manifest is not null && !String.IsNullOrWhiteSpace(manifest.CatalogName))
            {
                IReadOnlyList<DataItemTransferRange> coverage = await this.state.LoadSegmentedCoverageAsync(session.SessionID,manifest.CatalogName,cancel);

                result.Add(session with { ManifestJson = Encoding.UTF8.GetString((manifest with { RealizedRanges = coverage.ToArray() }).Serialize()) });
            }
            else { result.Add(session); }
        }

        return result;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<TransferSessionCleanupCandidate>> LoadStaleTransferSessionCandidatesAsync(DateTimeOffset cutoff , TransferSessionCleanupCandidate? after , Int32 pageSize , CancellationToken cancel = default)
    {
        return this.state.LoadStaleTransferSessionCandidatesAsync(this.storage.CatalogName,cutoff,after,pageSize,cancel);
    }

    /// <inheritdoc/>
    public Task<Boolean> TouchTransferSessionAsync(Guid sessionid , Guid itemid , DateTimeOffset activity , CancellationToken cancel = default)
    {
        return this.state.TouchTransferSessionAsync(sessionid,itemid,this.storage.CatalogName,activity,cancel);
    }

    /// <inheritdoc/>
    public async Task<TransferBlobReference?> SaveObjectAsync(Guid sessionid , Stream payload , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is null) { return null; }

        return ToReference(await this.storage.Get(sessionid,session.ItemID,session.ItemInstanceToken).SaveObjectAsync(payload,cancel));
    }

    /// <inheritdoc/>
    public async Task<TransferBlobReference?> ReplaceObjectAsync(Guid sessionid , Stream payload , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is null) { return null; }

        return ToReference(await this.storage.Get(sessionid,session.ItemID,session.ItemInstanceToken).ReplaceObjectAsync(payload,cancel));
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="SaveVerifiedSegmentedRangeAsync"]/*'/>*/
    private async Task<TransferBlobReference?> SaveVerifiedSegmentedRangeAsync(Guid sessionid , DataItemTransferRange range , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is null) { return null; }

        if(session.Mode is DataItemTransferMode.Upload)
        {
            TenantTransferStorageLimits.ValidateBudget(sessionid,session.PlannedBlockCount,TenantTransferStorageLimits.CountBlocks(range.Length));
        }

        try
        {
            return ToReference(await this.storage.Get(sessionid,session.ItemID,session.ItemInstanceToken).SaveVerifiedRangeAsync(range,payload,expectedLength,expectedSHA512,cancel));
        }
        catch ( OperationFailedException error ) when(error.FailureCode is StorageVerificationFailureCode code )
        {
            throw new OperationFailedException(error.Message,ToSegmentedFailureCode(code),error);
        }
    }

    /// <inheritdoc/>
    public Task<SegmentedTransferWriteResult> WriteSegmentedRangeAsync(Guid sessionid , DataItemTransferManifest manifest , DataItemTransferRange range , Stream payload , CancellationToken cancel = default)
    {
        return WriteSegmentedRangeCoreAsync(sessionid,manifest,range,payload,null,null,cancel);
    }

    /// <inheritdoc/>
    public Task<SegmentedTransferWriteResult> WriteVerifiedSegmentedRangeAsync(Guid sessionid , DataItemTransferManifest manifest , DataItemTransferRange range , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel = default)
    {
        return WriteSegmentedRangeCoreAsync(sessionid,manifest,range,payload,expectedLength,expectedSHA512,cancel);
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="WriteSegmentedRangeCoreAsync"]/*'/>*/
    private async Task<SegmentedTransferWriteResult> WriteSegmentedRangeCoreAsync(Guid sessionid , DataItemTransferManifest manifest , DataItemTransferRange range , Stream payload , Int64? expectedLength , Byte[]? expectedSHA512 , CancellationToken cancel)
    {
        if(manifest.CanWriteRange(range) is false) { return SegmentedTransferWriteResult.Create(range,accepted:false,stateversion:manifest.StateVersion); }

        SegmentedTransferRangeState? existingAtOffset = await this.state.LoadSegmentedRangeAsync(sessionid,range.Offset,this.storage.CatalogName,cancel);

        if(existingAtOffset is not null)
        {
            if(existingAtOffset.Range.Length != range.Length) { return SegmentedTransferWriteResult.Create(range,accepted:false,stateversion:manifest.StateVersion); }

            using HashingReadStream hashing = new(payload,leaveopen:true);

            await hashing.CopyToAsync(Stream.Null,cancel);

            Byte[] actualSHA512 = hashing.GetHashAndReset();

            Int64 actualLength = hashing.BytesRead;

            if(payload.CanSeek) { payload.Position = 0; }

            Boolean lengthMismatch = expectedLength.HasValue && actualLength != expectedLength.Value;

            Boolean hashMismatch = expectedSHA512 is not null && actualSHA512.AsSpan().SequenceEqual(expectedSHA512) is false;

            if(lengthMismatch || hashMismatch)
            {
                throw new OperationFailedException("The staged segmented range does not match its declared content.",expectedLength.HasValue && actualLength != expectedLength.Value ? SegmentedTransferFailureCode.InvalidSegmentLength : SegmentedTransferFailureCode.InvalidSegmentHash);
            }

            if(actualLength != existingAtOffset.Payload.Length || actualSHA512.AsSpan().SequenceEqual(existingAtOffset.Payload.SHA512) is false)
            {
                return SegmentedTransferWriteResult.Create(range,accepted:false,stateversion:manifest.StateVersion);
            }

            return await CommitSegmentedRangeAsync(existingAtOffset,manifest,cancel);
        }

        if(await this.state.HasSegmentedRangeOverlapAsync(sessionid,range,this.storage.CatalogName,cancel)) { return SegmentedTransferWriteResult.Create(range,accepted:false,stateversion:manifest.StateVersion); }

        TransferBlobReference? reference = expectedLength.HasValue && expectedSHA512 is not null
            ? await SaveVerifiedSegmentedRangeAsync(sessionid,range,payload,expectedLength.Value,expectedSHA512,cancel)
            : await SaveUnverifiedSegmentedRangeAsync(sessionid,range,payload,cancel);

        if(reference is null) { return SegmentedTransferWriteResult.Create(range,accepted:false,stateversion:manifest.StateVersion); }

        return await CommitSegmentedRangeAsync(new SegmentedTransferRangeState
        {
            SessionID = sessionid,
            Range = range,
            Payload = reference,
            Created = DateTimeOffset.UtcNow,
        },manifest,cancel);
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="CommitSegmentedRangeAsync"]/*'/>*/
    private async Task<SegmentedTransferWriteResult> CommitSegmentedRangeAsync(SegmentedTransferRangeState rangeState , DataItemTransferManifest manifest , CancellationToken cancel)
    {
        TransferSessionState? existing = await this.state.LoadSessionAsync(manifest.SessionID,this.storage.CatalogName,cancel);

        DataItemTransferManifest updatedManifest = manifest with { RealizedRanges = Array.Empty<DataItemTransferRange>() , Integrity = null , Updated = DateTimeOffset.UtcNow , StateVersion = checked(manifest.StateVersion + 1) , FaultMessage = null };

        return await this.state.CommitSegmentedRangeAsync(rangeState,new TransferSessionState
        {
            SessionID = manifest.SessionID,
            ItemID = manifest.ItemID,
            ItemInstanceToken = manifest.ItemInstanceToken,
            Family = DataControlTransferFamily.Segmented,
            Mode = manifest.Mode,
            Status = updatedManifest.Status,
            StateVersion = updatedManifest.StateVersion,
            Created = manifest.Created,
            Updated = updatedManifest.Updated,
            ManifestJson = DataControlTransferSnapshot.SanitizeManifest(Encoding.UTF8.GetString(updatedManifest.Serialize())),
            ObjectPayload = existing?.ObjectPayload,
        },manifest.StateVersion,this.storage.CatalogName,cancel);
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="SaveUnverifiedSegmentedRangeAsync"]/*'/>*/
    private async Task<TransferBlobReference?> SaveUnverifiedSegmentedRangeAsync(Guid sessionid , DataItemTransferRange range , Stream payload , CancellationToken cancel)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is null) { return null; }

        if(session.Mode is DataItemTransferMode.Upload)
        {
            TenantTransferStorageLimits.ValidateBudget(sessionid,session.PlannedBlockCount,TenantTransferStorageLimits.CountBlocks(range.Length));
        }

        return ToReference(await this.storage.Get(sessionid,session.ItemID,session.ItemInstanceToken).SaveRangeAsync(range,payload,cancel));
    }

    /// <inheritdoc/>
    public async Task<TransferBlobReference?> CreateSegmentedSessionAsync(DataItemTransferManifest manifest , Byte[] objectpayload , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(manifest); ArgumentNullException.ThrowIfNull(objectpayload);

        EnsureManifestCatalog(manifest.CatalogName);

        await using MemoryStream payload = new(objectpayload,false);

        TransferBlobReference? reference = ToReference(await this.storage.Get(manifest.SessionID,manifest.ItemID,manifest.ItemInstanceToken).SaveObjectAsync(payload,cancel));

        if(reference is null) { return null; }

        await SaveSegmentedManifestAsync(manifest,reference,cancel);

        return reference;
    }

    /// <inheritdoc/>
    public async Task<TransferBlobReference?> CreateStreamSessionAsync(DataStreamTransferManifest manifest , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        EnsureManifestCatalog(manifest.CatalogName);

        await using MemoryStream payload = new(manifest.ObjectPayload ?? Array.Empty<Byte>(),false);

        TransferBlobReference? reference = ToReference(await this.storage.Get(manifest.SessionID,manifest.ItemID,manifest.ItemInstanceToken).SaveObjectAsync(payload,cancel));

        if(reference is null) { return null; }

        await SaveStreamManifestAsync(manifest,reference,cancel);

        return reference;
    }

    /// <inheritdoc/>
    public async Task<StreamTransferAppendResult> WriteVerifiedStreamChunkAsync(Guid sessionid , Guid operationid , DataStreamTransferManifest manifest , DataItemTransferRange range , Stream payload , Int64 expectedLength , Byte[] expectedSHA512 , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(manifest); ArgumentNullException.ThrowIfNull(payload);

        if(operationid == Guid.Empty || range.Validate() is false)
        {
            return StreamTransferAppendResult.Create(range,accepted:false,manifest.AppendedLength,manifest.StateVersion);
        }

        if(range.Offset > manifest.AppendedLength)
        {
            return StreamTransferAppendResult.Create(range,accepted:false,manifest.AppendedLength,manifest.StateVersion);
        }

        StreamTransferChunkState? existingAtOffset = await this.state.LoadStreamChunkAsync(sessionid,range.Offset,this.storage.CatalogName,cancel);

        if(existingAtOffset is not null)
        {
            using HashingReadStream hashing = new(payload,leaveopen:true);

            await hashing.CopyToAsync(Stream.Null,cancel);

            Byte[] actualSHA512 = hashing.GetHashAndReset();

            Int64 actualLength = hashing.BytesRead;

            if(payload.CanSeek) { payload.Position = 0; }

            if(actualLength != expectedLength || actualSHA512.AsSpan().SequenceEqual(expectedSHA512) is false)
            {
                throw new OperationFailedException("The staged stream segment does not match its declared content.",actualLength != expectedLength ? StreamTransferFailureCode.InvalidSegmentLength : StreamTransferFailureCode.InvalidSegmentHash);
            }

            if(existingAtOffset.Range.Length != range.Length || existingAtOffset.Payload.Length != range.Length || !existingAtOffset.Payload.SHA512.AsSpan().SequenceEqual(actualSHA512))
            {
                return StreamTransferAppendResult.Create(range,accepted:false,manifest.AppendedLength,manifest.StateVersion);
            }

            DataStreamTransferManifest replayManifest = manifest.WithAppendedLength(Math.Max(manifest.AppendedLength,existingAtOffset.Range.EndOffsetExclusive)) with { Integrity = null };

            return await CommitStreamAppendAsync(existingAtOffset,replayManifest,manifest.StateVersion,cancel);
        }

        if(manifest.CanWriteRange(range) is false)
        {
            return StreamTransferAppendResult.Create(range,accepted:false,manifest.AppendedLength,manifest.StateVersion);
        }

        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is not null)
        {
            TenantTransferStorageLimits.ValidateBudget(sessionid,session.PlannedBlockCount,TenantTransferStorageLimits.CountBlocks(range.Length));
        }

        TransferBlobReference? reference;

        try
        {
            reference = session is null
                ? null
                : ToReference(await this.storage.Get(sessionid,session.ItemID,session.ItemInstanceToken).SaveVerifiedChunkAsync(operationid,range,payload,expectedLength,expectedSHA512,cancel));
        }
        catch ( OperationFailedException error ) when(error.FailureCode is StorageVerificationFailureCode code )
        {
            throw new OperationFailedException(error.Message,ToStreamFailureCode(code),error);
        }

        if(reference is null) { return StreamTransferAppendResult.Create(range,accepted:false,manifest.AppendedLength,manifest.StateVersion); }

        DataStreamTransferManifest updatedManifest = manifest.WithAppendedLength(range.EndOffsetExclusive) with { Integrity = null };

        return await CommitStreamAppendAsync(new StreamTransferChunkState
        {
            SessionID = sessionid,
            OperationID = operationid,
            Range = range,
            Payload = reference,
            Created = DateTimeOffset.UtcNow,
        },updatedManifest,manifest.StateVersion,cancel);
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="CommitStreamAppendAsync"]/*'/>*/
    private async Task<StreamTransferAppendResult> CommitStreamAppendAsync(StreamTransferChunkState chunk , DataStreamTransferManifest manifest , Int64 expectedStateVersion , CancellationToken cancel)
    {
        TransferSessionState? existing = await this.state.LoadSessionAsync(manifest.SessionID,this.storage.CatalogName,cancel);

        return await this.state.CommitStreamAppendAsync(chunk,new TransferSessionState
        {
            SessionID = manifest.SessionID,
            ItemID = manifest.ItemID,
            ItemInstanceToken = manifest.ItemInstanceToken,
            Family = DataControlTransferFamily.Stream,
            Mode = manifest.Mode,
            Status = manifest.Status,
            StateVersion = manifest.StateVersion,
            Created = manifest.Created,
            Updated = manifest.Updated,
            ManifestJson = DataControlTransferSnapshot.SanitizeManifest(Encoding.UTF8.GetString(manifest.Serialize())),
            ObjectPayload = existing?.ObjectPayload,
        },expectedStateVersion,this.storage.CatalogName,cancel);
    }

    /// <inheritdoc/>
    public async Task<TransferContentIntegrity?> RecomputeIntegrityAsync(Guid sessionid , CancellationToken cancel = default)
    {
        IReadOnlyList<TransferContentIntegritySegment>? segments = await LoadIntegritySegmentsAsync(sessionid,cancel);

        if(segments is null) { return null; }

        TransferContentIntegrity integrity;

        try { integrity = TransferContentIntegrityCalculator.Compute(segments); } catch ( ArgumentException ) { return null; }

        return integrity;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TransferContentIntegritySegment>?> LoadIntegritySegmentsAsync(Guid sessionid , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is null) { return null; }

        if(session.Family is DataControlTransferFamily.Segmented)
        {
            IReadOnlyList<SegmentedTransferRangeState> ranges = await this.state.LoadSegmentedRangesAsync(sessionid,this.storage.CatalogName,cancel);

            return ranges.OrderBy(item => item.Range.Offset).Select(item => new TransferContentIntegritySegment
            {
                Offset = item.Range.Offset,
                Length = item.Range.Length,
                SHA512 = item.Payload.SHA512.ToArray(),
            }).ToArray();
        }

        if(session.Family is DataControlTransferFamily.Stream)
        {
            IReadOnlyList<StreamTransferChunkState> chunks = await this.state.LoadStreamChunksAsync(sessionid,this.storage.CatalogName,cancel);

            return chunks.OrderBy(item => item.Range.Offset).Select(item => new TransferContentIntegritySegment
            {
                Offset = item.Range.Offset,
                Length = item.Range.Length,
                SHA512 = item.Payload.SHA512.ToArray(),
            }).ToArray();
        }

        return null;
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="LoadObjectReferenceAsync"]/*'/>*/
    public async Task<TenantTransferBlobReference?> LoadObjectReferenceAsync(Guid sessionid , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        return session?.ObjectPayload is TransferBlobReference reference ? ToTenantReference(reference) : null;
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="CreateSegmentedBlockPlanAsync"]/*'/>*/
    public async Task<TransferBlockPlan?> CreateSegmentedBlockPlanAsync(Guid sessionid , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is null || session.Family is not DataControlTransferFamily.Segmented) { return null; }

        IReadOnlyList<SegmentedTransferRangeState> ranges = await this.state.LoadSegmentedRangesAsync(sessionid,this.storage.CatalogName,cancel);

        return TransferBlockPlanner.Create(ranges.OrderBy(item => item.Ordinal).Select(item => new TransferRangeReference
        {
            Range = item.Range,
            Blob = ToTenantReference(item.Payload),
        }).ToArray());
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="CreateStreamBlockPlanAsync"]/*'/>*/
    public async Task<TransferBlockPlan?> CreateStreamBlockPlanAsync(Guid sessionid , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is null || session.Family is not DataControlTransferFamily.Stream) { return null; }

        IReadOnlyList<StreamTransferChunkState> chunks = await this.state.LoadStreamChunksAsync(sessionid,this.storage.CatalogName,cancel);

        return TransferBlockPlanner.Create(chunks.OrderBy(item => item.Ordinal).Select(item => new TransferRangeReference
        {
            Range = item.Range,
            Blob = ToTenantReference(item.Payload),
        }).ToArray());
    }

    /// <inheritdoc/>
    public async Task SaveSegmentedManifestAsync(DataItemTransferManifest manifest , TransferBlobReference? objectpayload = null , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        EnsureManifestCatalog(manifest.CatalogName);

        TransferSessionState? existing = await this.state.LoadSessionAsync(manifest.SessionID,this.storage.CatalogName,cancel);

        objectpayload ??= existing?.ObjectPayload;

        String json = DataControlTransferSnapshot.SanitizeManifest(Encoding.UTF8.GetString((manifest with { RealizedRanges = Array.Empty<DataItemTransferRange>() , Integrity = null }).Serialize()));

        await this.state.SaveSessionAsync(new TransferSessionState
        {
            SessionID = manifest.SessionID,
            ItemID = manifest.ItemID,
            ItemInstanceToken = manifest.ItemInstanceToken,
            Family = DataControlTransferFamily.Segmented,
            Mode = manifest.Mode,
            Status = manifest.Status,
            StateVersion = manifest.StateVersion,
            Created = manifest.Created,
            Updated = manifest.Updated,
            ManifestJson = json,
            ObjectPayload = objectpayload,
        },this.storage.CatalogName,cancel);
    }

    /// <inheritdoc/>
    public async Task<Boolean> PromoteStagedReadManifestAsync(DataItemTransferManifest manifest , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        EnsureManifestCatalog(manifest.CatalogName);

        if(manifest.Mode is not DataItemTransferMode.PublishedRead || manifest.SourceSessionID.HasValue)
        {
            throw new ArgumentException("The promoted manifest must be a published-read manifest without a source session.",nameof(manifest));
        }

        TransferSessionState? existing = await this.state.LoadSessionAsync(manifest.SessionID,this.storage.CatalogName,cancel);

        if(existing is null || existing.Family is not DataControlTransferFamily.Segmented || existing.Mode is not DataItemTransferMode.StagedRead || existing.ItemID != manifest.ItemID) { return false; }

        DataItemTransferManifest? existingManifest = DataItemTransferManifest.Deserialize(Encoding.UTF8.GetBytes(existing.ManifestJson));

        if(existingManifest is null
            || existingManifest.SessionID != existing.SessionID
            || existingManifest.ItemID != existing.ItemID
            || existingManifest.Mode is not DataItemTransferMode.StagedRead
            || String.Equals(existingManifest.CatalogName,this.storage.CatalogName,Ordinal) is false
            || existingManifest.SourceSessionID.HasValue is false) { return false; }

        DataItemTransferManifest promoted = manifest with { Created = existingManifest.Created };

        String json = DataControlTransferSnapshot.SanitizeManifest(Encoding.UTF8.GetString((promoted with { RealizedRanges = Array.Empty<DataItemTransferRange>() , Integrity = null }).Serialize()));

        return await this.state.PromoteStagedReadSessionAsync(new TransferSessionState
        {
            SessionID = manifest.SessionID,
            ItemID = manifest.ItemID,
            ItemInstanceToken = existing.ItemInstanceToken,
            Family = DataControlTransferFamily.Segmented,
            Mode = DataItemTransferMode.PublishedRead,
            Status = promoted.Status,
            StateVersion = promoted.StateVersion,
            Created = existingManifest.Created,
            Updated = promoted.Updated,
            ManifestJson = json,
            ObjectPayload = existing.ObjectPayload,
        },existingManifest.SourceSessionID.Value,this.storage.CatalogName,cancel);
    }

    /// <inheritdoc/>
    public async Task SaveStreamManifestAsync(DataStreamTransferManifest manifest , TransferBlobReference? objectpayload = null , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(manifest);

        EnsureManifestCatalog(manifest.CatalogName);

        TransferSessionState? existing = await this.state.LoadSessionAsync(manifest.SessionID,this.storage.CatalogName,cancel);

        objectpayload ??= existing?.ObjectPayload;

        String json = DataControlTransferSnapshot.SanitizeManifest(Encoding.UTF8.GetString((manifest with { Integrity = null }).Serialize()));

        await this.state.SaveSessionAsync(new TransferSessionState
        {
            SessionID = manifest.SessionID,
            ItemID = manifest.ItemID,
            ItemInstanceToken = manifest.ItemInstanceToken,
            Family = DataControlTransferFamily.Stream,
            Mode = manifest.Mode,
            Status = manifest.Status,
            StateVersion = manifest.StateVersion,
            Created = manifest.Created,
            Updated = manifest.Updated,
            ManifestJson = json,
            ObjectPayload = objectpayload,
        },this.storage.CatalogName,cancel);
    }

    /// <inheritdoc/>
    public async Task<Stream?> OpenObjectAsync(Guid sessionid , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        return session is null ? null : await this.storage.Get(sessionid,session.ItemID,session.ItemInstanceToken).OpenObjectAsync(cancel);
    }

    /// <inheritdoc/>
    public async Task<Byte[]?> ReadObjectPayloadAsync(Guid sessionid , CancellationToken cancel = default)
    {
        await using Stream? payload = await OpenObjectAsync(sessionid,cancel);

        if(payload is null) { return null; }

        return await ObjectPayloadReader.ReadAsync(payload,cancel);
    }

    /// <inheritdoc/>
    public async Task<Stream?> OpenSegmentedRangeAsync(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default)
    {
        IReadOnlyList<SegmentedTransferRangeState> states = await this.state.LoadSegmentedRangesAsync(sessionid,this.storage.CatalogName,cancel);

        if(range.Length <= 0 || states.Count == 0) { return null; }

        Int64 requestedEnd = checked(range.Offset + range.Length);

        Int64 covered = range.Offset;

        List<CompositeReadStreamPart> parts = new();

        foreach(SegmentedTransferRangeState state in states.OrderBy(item => item.Range.Offset))
        {
            if(state.Range.EndOffsetExclusive <= covered) { continue; }

            if(state.Range.Offset > covered) { return null; }

            Int64 overlapStart = covered;

            Int64 overlapEnd = Math.Min(state.Range.EndOffsetExclusive,requestedEnd);

            if(overlapEnd <= overlapStart) { continue; }

            parts.Add(new CompositeReadStreamPart
            {
                SourceOffset = overlapStart - state.Range.Offset,
                Length = overlapEnd - overlapStart,
                OpenAsync = token => OpenSegmentedRangePartAsync(sessionid,state.Range,token),
            });

            covered = overlapEnd;

            if(covered == requestedEnd) { break; }
        }

        if(covered != requestedEnd) { return null; }

        return new CompositeReadStream(parts);
    }

    /// <inheritdoc/>
    public async Task<Stream?> OpenStreamRangeAsync(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default)
    {
        IReadOnlyList<StreamTransferChunkState> chunks = await this.state.LoadStreamChunksAsync(sessionid,this.storage.CatalogName,cancel);

        if(range.Length <= 0 || chunks.Count == 0) { return null; }

        Int64 requestedEnd = checked(range.Offset + range.Length);

        Int64 covered = range.Offset;

        List<CompositeReadStreamPart> parts = new();

        foreach(StreamTransferChunkState chunk in chunks.OrderBy(item => item.Range.Offset))
        {
            if(chunk.Range.EndOffsetExclusive <= covered) { continue; }

            if(chunk.Range.Offset > covered) { return null; }

            Int64 overlapStart = covered;

            Int64 overlapEnd = Math.Min(chunk.Range.EndOffsetExclusive,requestedEnd);

            if(overlapEnd <= overlapStart) { continue; }

            parts.Add(new CompositeReadStreamPart
            {
                SourceOffset = overlapStart - chunk.Range.Offset,
                Length = overlapEnd - overlapStart,
                OpenAsync = token => OpenStreamChunkPartAsync(sessionid,chunk.OperationID,token),
            });

            covered = overlapEnd;

            if(covered == requestedEnd) { break; }
        }

        if(covered != requestedEnd) { return null; }

        return covered == requestedEnd ? new CompositeReadStream(parts) : null;
    }

    /// <inheritdoc/>
    public async Task<Boolean> DeleteSessionAsync(Guid sessionid , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        Boolean payload = session is not null && await this.storage.Get(sessionid,session.ItemID,session.ItemInstanceToken).DeleteTransferAsync(cancel);

        Boolean metadata = await this.state.DeleteSessionAsync(sessionid,this.storage.CatalogName,cancel);

        return metadata || payload;
    }

    /// <inheritdoc/>
    public async Task<Boolean> CloseReadSessionAsync(Guid sessionid , Guid itemid , CancellationToken cancel = default)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        if(session is null) { return true; }

        if(session.ItemID != itemid || session.Mode is not (DataItemTransferMode.PublishedRead or DataItemTransferMode.StagedRead or DataItemTransferMode.StreamFollow)) { return false; }

        if(session.Status is DataItemTransferStatus.Publishing) { return false; }

        if(await this.state.HasPublicationJournalAsync(sessionid,this.storage.CatalogName,cancel)) { return false; }

        if(await this.state.HasDependentSessionAsync(sessionid,this.storage.CatalogName,cancel)) { return false; }

        return await this.DeleteSessionAsync(sessionid,cancel);
    }

    /// <inheritdoc/>
    public Task<Boolean> HasDependentSessionAsync(Guid sourcesessionid , CancellationToken cancel = default)
    {
        return this.state.HasDependentSessionAsync(sourcesessionid,this.storage.CatalogName,cancel);
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="OpenSegmentedRangePartAsync"]/*'/>*/
    private async Task<Stream?> OpenSegmentedRangePartAsync(Guid sessionid , DataItemTransferRange range , CancellationToken cancel)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        return session is null ? null : await this.storage.Get(sessionid,session.ItemID,session.ItemInstanceToken).OpenRangeAsync(range,cancel);
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="OpenStreamChunkPartAsync"]/*'/>*/
    private async Task<Stream?> OpenStreamChunkPartAsync(Guid sessionid , Guid operationid , CancellationToken cancel)
    {
        TransferSessionState? session = await this.state.LoadSessionAsync(sessionid,this.storage.CatalogName,cancel);

        return session is null ? null : await this.storage.Get(sessionid,session.ItemID,session.ItemInstanceToken).OpenChunkAsync(operationid,cancel);
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="EnsureManifestCatalog"]/*'/>*/
    private void EnsureManifestCatalog(String catalogname)
    {
        if(String.IsNullOrWhiteSpace(catalogname) || !String.Equals(catalogname,this.storage.CatalogName,Ordinal))
        {
            throw new InvalidOperationException("The transfer manifest catalog does not match the selected tenant catalog.");
        }
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="ToReference"]/*'/>*/
    private static TransferBlobReference? ToReference(TenantTransferBlobReference? reference)
    {
        return reference is null ? null : new TransferBlobReference
        {
            BlobName = reference.BlobName,
            Length = reference.Length,
            SHA512 = reference.SHA512.ToArray(),
        };
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="ToSegmentedFailureCode"]/*'/>*/
    private static SegmentedTransferFailureCode ToSegmentedFailureCode(StorageVerificationFailureCode code)
    {
        return code switch
        {
            StorageVerificationFailureCode.InvalidLength => SegmentedTransferFailureCode.InvalidSegmentLength,
            StorageVerificationFailureCode.InvalidHash => SegmentedTransferFailureCode.InvalidSegmentHash,
            _ => throw new ArgumentOutOfRangeException(nameof(code),code,null),
        };
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="ToStreamFailureCode"]/*'/>*/
    private static StreamTransferFailureCode ToStreamFailureCode(StorageVerificationFailureCode code)
    {
        return code switch
        {
            StorageVerificationFailureCode.InvalidLength => StreamTransferFailureCode.InvalidSegmentLength,
            StorageVerificationFailureCode.InvalidHash => StreamTransferFailureCode.InvalidSegmentHash,
            _ => throw new ArgumentOutOfRangeException(nameof(code),code,null),
        };
    }

    /**<include file='DataControlTransferSessionStore.xml' path='DataControlTransferSessionStore/class[@name="DataControlTransferSessionStore"]/method[@name="ToTenantReference"]/*'/>*/
    private static TenantTransferBlobReference ToTenantReference(TransferBlobReference reference)
    {
        ArgumentNullException.ThrowIfNull(reference);

        return new TenantTransferBlobReference
        {
            BlobName = reference.BlobName,
            Length = reference.Length,
            SHA512 = reference.SHA512.ToArray(),
        };
    }
}
