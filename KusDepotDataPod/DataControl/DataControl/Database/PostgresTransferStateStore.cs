namespace DataPodServices.DataControl;

/**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/main/*'/>*/
public sealed class PostgresTransferStateStore : ITransferStateStore
{
    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/field[@name="contexts"]/*'/>*/
    private readonly IDbContextFactory<DataControlDbContext> contexts;

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/constructor[@name="PostgresTransferStateStore"]/*'/>*/
    public PostgresTransferStateStore(IDbContextFactory<DataControlDbContext> contexts)
    {
        this.contexts = contexts ?? throw new ArgumentNullException(nameof(contexts));
    }

    /// <inheritdoc/>
    public async Task<TransferSessionState?> LoadSessionAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        DataControlTransferSessionRow? row = await context.TransferSessions
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.SessionID == sessionid && item.CatalogName == catalogname,cancel);

        return row is null ? null : ToState(row);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TransferSessionCleanupCandidate>> LoadStaleTransferSessionCandidatesAsync(String catalogname , DateTimeOffset cutoff , TransferSessionCleanupCandidate? after , Int32 pageSize , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        if(after is not null && !String.Equals(after.CatalogName,catalogname,Ordinal))
        {
            throw new ArgumentException("The cleanup cursor belongs to a different catalog.",nameof(after));
        }

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        IQueryable<DataControlTransferSessionRow> query = context.TransferSessions
            .AsNoTracking()
            .Where(item => item.CatalogName == catalogname && item.LastActivity <= cutoff);

        if(after is not null)
        {
            query = query.Where(item => item.LastActivity > after.LastActivity
                || (item.LastActivity == after.LastActivity && item.SessionID.CompareTo(after.SessionID) > 0));
        }

        return await query
            .OrderBy(item => item.LastActivity)
            .ThenBy(item => item.SessionID)
            .Select(item => new TransferSessionCleanupCandidate(item.LastActivity,item.SessionID,item.ItemID,item.CatalogName))
            .Take(pageSize)
            .ToListAsync(cancel);
    }

    /// <inheritdoc/>
    public async Task<Boolean> HasDependentSessionAsync(Guid sourcesessionid , String catalogname , CancellationToken cancel = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(sourcesessionid,Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        return await context.TransferSessions
            .AsNoTracking()
            .AnyAsync(item => item.SourceSessionID == sourcesessionid
                && item.CatalogName == catalogname
                && (item.Mode == (Int32)DataItemTransferMode.StagedRead || item.Mode == (Int32)DataItemTransferMode.StreamFollow),cancel);
    }

    /// <inheritdoc/>
    public async Task<Boolean> HasPublicationJournalAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(sessionid,Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        return await context.PublicationJournals
            .AsNoTracking()
            .AnyAsync(item => item.SessionID == sessionid && item.CatalogName == catalogname,cancel);
    }

    /// <inheritdoc/>
    public async Task<Boolean> TouchTransferSessionAsync(Guid sessionid , Guid itemid , String catalogname , DateTimeOffset activity , CancellationToken cancel = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(sessionid,Guid.Empty);
        ArgumentOutOfRangeException.ThrowIfEqual(itemid,Guid.Empty);
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        activity = activity.ToUniversalTime();

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        Int32 updated = await context.TransferSessions
            .Where(item => item.SessionID == sessionid && item.ItemID == itemid && item.CatalogName == catalogname)
            .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.LastActivity,item => item.LastActivity < activity ? activity : item.LastActivity),cancel);

        return updated != 0;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TransferPublicationJournalState>> LoadJournalsForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(itemid,Guid.Empty);

        ArgumentOutOfRangeException.ThrowIfEqual(itemInstanceToken,Guid.Empty);

        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        List<DataControlPublicationJournalRow> rows = await context.PublicationJournals
            .AsNoTracking()
            .Where(item => item.ItemID == itemid && item.ItemInstanceToken == itemInstanceToken && item.CatalogName == catalogname)
            .OrderBy(item => item.SessionID)
            .ToListAsync(cancel);

        return rows.Select(ToState).ToArray();
    }

    /// <inheritdoc/>
    public async Task<Boolean> DeleteJournalsForInstanceAsync(Guid itemid , Guid itemInstanceToken , String catalogname , CancellationToken cancel = default)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(itemid,Guid.Empty);

        ArgumentOutOfRangeException.ThrowIfEqual(itemInstanceToken,Guid.Empty);

        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        Int32 deleted = await context.PublicationJournals
            .Where(item => item.ItemID == itemid && item.ItemInstanceToken == itemInstanceToken && item.CatalogName == catalogname)
            .ExecuteDeleteAsync(cancel);

        return deleted > 0;
    }

    /// <inheritdoc/>
    public async Task<TransferRecoveryStateSnapshot> LoadRecoveryStateForAdminAsync(Guid sessionid , CancellationToken cancel = default)
    {
        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead,cancel);

        DataControlTransferSessionRow? sessionRow = await context.TransferSessions
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.SessionID == sessionid,cancel);

        if(sessionRow is null)
        {
            await transaction.CommitAsync(cancel);

            return new();
        }

        DataControlPublicationJournalRow? journalRow = await context.PublicationJournals
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.SessionID == sessionid,cancel);

        List<DataControlSegmentedRangeRow> ranges = await context.SegmentedRanges
            .AsNoTracking()
            .Where(item => item.SessionID == sessionid)
            .OrderBy(item => item.Offset)
            .ToListAsync(cancel);

        List<DataControlStreamChunkRow> chunks = await context.StreamChunks
            .AsNoTracking()
            .Where(item => item.SessionID == sessionid)
            .OrderBy(item => item.Offset)
            .ToListAsync(cancel);

        TransferRecoveryStateSnapshot snapshot = new()
        {
            Session = ToState(sessionRow),
            Journal = journalRow is null ? null : ToState(journalRow),
            SegmentedRanges = ranges.Select(ToState).ToArray(),
            StreamChunks = chunks.Select(ToState).ToArray(),
        };

        await transaction.CommitAsync(cancel);

        return snapshot;
    }

    /// <inheritdoc/>
    public async Task<Boolean> PurgeRecoveryStateAsync(Guid sessionid , String catalogname , Guid itemid , Int64 expectedsessionstateversion , Guid? expectedoperationid , TransferPublicationPhase? expectedphase , DateTimeOffset? expectedjournalupdated , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        await using var transaction = await context.Database.BeginTransactionAsync(cancel);

        DataControlTransferSessionRow? session = await context.TransferSessions
            .FromSql($"SELECT * FROM \"TransferSessions\" WHERE \"SessionID\" = {sessionid} AND \"CatalogName\" = {catalogname} FOR UPDATE")
            .SingleOrDefaultAsync(cancel);

        if(session is null || session.ItemID != itemid || session.StateVersion != expectedsessionstateversion) { return false; }

        DataControlPublicationJournalRow? journal = await context.PublicationJournals
            .FromSql($"SELECT * FROM \"PublicationJournals\" WHERE \"SessionID\" = {sessionid} FOR UPDATE")
            .SingleOrDefaultAsync(cancel);

        if(journal is null)
        {
            if(expectedoperationid.HasValue || expectedphase.HasValue || expectedjournalupdated.HasValue) { return false; }
        }
        else if(!String.Equals(journal.CatalogName,catalogname,Ordinal)
            || !expectedoperationid.HasValue || !expectedphase.HasValue || !expectedjournalupdated.HasValue
            || journal.OperationID != expectedoperationid.Value
            || journal.Phase != (Int32)expectedphase.Value
            || journal.Updated != expectedjournalupdated.Value)
        {
            return false;
        }

        await context.PublicationJournals
            .Where(item => item.SessionID == sessionid && item.CatalogName == catalogname)
            .ExecuteDeleteAsync(cancel);

        Int32 deleted = await context.TransferSessions
            .Where(item => item.SessionID == sessionid && item.CatalogName == catalogname && item.ItemID == itemid && item.StateVersion == expectedsessionstateversion)
            .ExecuteDeleteAsync(cancel);

        if(deleted != 1) { return false; }

        await transaction.CommitAsync(cancel);

        return true;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TransferSessionState>> LoadSessionsAsync(TransferSessionQuery query , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(query); ArgumentException.ThrowIfNullOrWhiteSpace(query.CatalogName);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        return await LoadSessionsCoreAsync(context.TransferSessions.AsNoTracking(),query,query.CatalogName,cancel);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TransferSessionState>> LoadSessionsForAdminAsync(TransferSessionQuery? query = null , CancellationToken cancel = default)
    {
        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        return await LoadSessionsCoreAsync(context.TransferSessions.AsNoTracking(),query,null,cancel);
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="LoadSessionsCoreAsync"]/*'/>*/
    private static async Task<IReadOnlyList<TransferSessionState>> LoadSessionsCoreAsync(IQueryable<DataControlTransferSessionRow> sessions , TransferSessionQuery? query , String? catalogname , CancellationToken cancel)
    {
        if(catalogname is not null) { sessions = sessions.Where(item => item.CatalogName == catalogname); }

        if(query?.ItemID.HasValue is true) { sessions = sessions.Where(item => item.ItemID == query.ItemID.Value); }
        if(query?.SessionID.HasValue is true) { sessions = sessions.Where(item => item.SessionID == query.SessionID.Value); }
        if(query?.CatalogName is not null && catalogname is null) { sessions = sessions.Where(item => item.CatalogName == query.CatalogName); }
        if(query?.SourceSessionID.HasValue is true) { sessions = sessions.Where(item => item.SourceSessionID == query.SourceSessionID.Value); }
        if(query?.Status.HasValue is true) { sessions = sessions.Where(item => item.Status == (Int32)query.Status.Value); }
        if(query?.Family.HasValue is true) { sessions = sessions.Where(item => item.Family == (Int32)query.Family.Value); }

        List<DataControlTransferSessionRow> rows = await sessions
            .OrderByDescending(item => item.Updated)
            .ToListAsync(cancel);

        return rows.Select(ToState).ToArray();
    }

    /// <inheritdoc/>
    public async Task<StreamTransferChunkState?> LoadStreamChunkAsync(Guid sessionid , Int64 offset , String catalogname , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        DataControlStreamChunkRow? row = await context.StreamChunks
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.SessionID == sessionid && item.Offset == offset && item.Session.CatalogName == catalogname,cancel);

        return row is null ? null : ToState(row);
    }

    /// <inheritdoc/>
    public async Task SaveSessionAsync(TransferSessionState session , String catalogname , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(session); ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        if(session.SessionID == Guid.Empty) { throw new ArgumentException("A transfer session identifier is required.",nameof(session)); }

        (String sessionCatalogName,_) = GetManifestProjection(session);

        if(String.Equals(sessionCatalogName,catalogname,Ordinal) is false) { throw new InvalidOperationException("The transfer session manifest does not belong to the authorized catalog."); }

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        DataControlTransferSessionRow? row = await context.TransferSessions
            .SingleOrDefaultAsync(item => item.SessionID == session.SessionID,cancel);

        if(row is null)
        {
            row = new DataControlTransferSessionRow { SessionID = session.SessionID };

            context.TransferSessions.Add(row);
        }
        else if(!String.Equals(row.CatalogName,catalogname,Ordinal)
            || row.ItemID != session.ItemID
            || row.ItemInstanceToken != session.ItemInstanceToken
            || row.Family != (Int32)session.Family
            || row.Mode != (Int32)session.Mode)
        {
            throw new InvalidOperationException("The transfer session catalog and identity are immutable.");
        }

        Apply(row,session);

        await context.SaveChangesAsync(cancel);
    }

    /// <inheritdoc/>
    public async Task<StreamTransferAppendResult> CommitStreamAppendAsync(StreamTransferChunkState chunk , TransferSessionState updatedsession , Int64 expectedstateversion , String catalogname , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(chunk); ArgumentNullException.ThrowIfNull(updatedsession); ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        ValidateRange(chunk.SessionID,chunk.Range,chunk.Payload);

        if(chunk.OperationID == Guid.Empty || chunk.SessionID != updatedsession.SessionID || chunk.SessionID == Guid.Empty || expectedstateversion < 0)
        {
            throw new ArgumentException("A valid stream append and updated session are required.");
        }

        (String updatedCatalog,_) = GetManifestProjection(updatedsession);

        DataStreamTransferManifest? proposedManifest = DataStreamTransferManifest.Parse(updatedsession.ManifestJson);

        if(!String.Equals(updatedCatalog,catalogname,Ordinal) || updatedsession.Family is not DataControlTransferFamily.Stream || proposedManifest is null ||
            proposedManifest.ItemInstanceToken != updatedsession.ItemInstanceToken ||
            proposedManifest.Mode is not DataItemTransferMode.StreamUpload || proposedManifest.AppendedLength < chunk.Range.EndOffsetExclusive ||
            proposedManifest.StateVersion != expectedstateversion && proposedManifest.StateVersion != expectedstateversion + 1)
        {
            throw new InvalidOperationException("The proposed stream append state does not match the verified chunk.");
        }

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        await using var transaction = await context.Database.BeginTransactionAsync(cancel);

        DataControlTransferSessionRow? row = await context.TransferSessions
            .FromSql($"SELECT * FROM \"TransferSessions\" WHERE \"SessionID\" = {chunk.SessionID} FOR UPDATE")
            .SingleOrDefaultAsync(cancel);

        if(row is null || !String.Equals(row.CatalogName,catalogname,Ordinal) || row.ItemID != updatedsession.ItemID ||
            row.ItemInstanceToken != updatedsession.ItemInstanceToken || row.Family != (Int32)DataControlTransferFamily.Stream || row.Mode != (Int32)DataItemTransferMode.StreamUpload)
        {
            throw new InvalidOperationException("The stream append session does not belong to the authorized catalog or identity.");
        }

        TransferSessionState currentState = ToState(row);

        DataStreamTransferManifest? currentManifest = DataStreamTransferManifest.Parse(currentState.ManifestJson);

        if(currentManifest is null || currentManifest.SessionID != chunk.SessionID || currentManifest.ItemID != updatedsession.ItemID || currentManifest.ItemInstanceToken != updatedsession.ItemInstanceToken ||
            currentManifest.Mode is not DataItemTransferMode.StreamUpload || currentManifest.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Publishing or DataItemTransferStatus.Published or DataItemTransferStatus.Faulted)
        {
            throw new InvalidOperationException("The stream append session is not writable.");
        }

        DataControlStreamChunkRow? existing = await context.StreamChunks
            .SingleOrDefaultAsync(item => item.SessionID == chunk.SessionID && item.Offset == chunk.Range.Offset,cancel);

        if(existing is not null)
        {
            Boolean sameContent = existing.Length == chunk.Range.Length && existing.SHA512.AsSpan().SequenceEqual(chunk.Payload.SHA512);

            if(!sameContent) { return StreamTransferAppendResult.Create(chunk.Range,accepted:false,currentManifest.AppendedLength,currentManifest.StateVersion); }

            if(currentManifest.AppendedLength == chunk.Range.Offset)
            {
                if(currentManifest.StateVersion != expectedstateversion || row.StateVersion != expectedstateversion || proposedManifest.AppendedLength != chunk.Range.EndOffsetExclusive || proposedManifest.StateVersion != expectedstateversion + 1)
                {
                    return StreamTransferAppendResult.Create(chunk.Range,accepted:false,currentManifest.AppendedLength,currentManifest.StateVersion);
                }

                Apply(row,updatedsession);

                await context.SaveChangesAsync(cancel);

                await transaction.CommitAsync(cancel);

                return StreamTransferAppendResult.Create(chunk.Range,accepted:true,proposedManifest.AppendedLength,proposedManifest.StateVersion,replayed:true);
            }

            if(existing.Offset + existing.Length <= currentManifest.AppendedLength)
            {
                await transaction.CommitAsync(cancel);

                return StreamTransferAppendResult.Create(chunk.Range,accepted:true,currentManifest.AppendedLength,currentManifest.StateVersion,replayed:true);
            }

            return StreamTransferAppendResult.Create(chunk.Range,accepted:false,currentManifest.AppendedLength,currentManifest.StateVersion);
        }

        Boolean operationInUse = await context.StreamChunks.AnyAsync(item => item.OperationID == chunk.OperationID,cancel);

        if(operationInUse || currentManifest.StateVersion != expectedstateversion || row.StateVersion != expectedstateversion || currentManifest.AppendedLength != chunk.Range.Offset || proposedManifest.AppendedLength != chunk.Range.EndOffsetExclusive || proposedManifest.StateVersion != expectedstateversion + 1)
        {
            return StreamTransferAppendResult.Create(chunk.Range,accepted:false,currentManifest.AppendedLength,currentManifest.StateVersion);
        }

        Int64 newBlocks = TenantTransferStorageLimits.CountBlocks(chunk.Range.Length);

        TenantTransferStorageLimits.ValidateBudget(chunk.SessionID,row.PlannedBlockCount,newBlocks);

        Int64 ordinal = await context.StreamChunks
            .Where(item => item.SessionID == chunk.SessionID)
            .Select(item => (Int64?)item.Ordinal)
            .MaxAsync(cancel)
             ?? -1;

        context.StreamChunks.Add(new DataControlStreamChunkRow
        {
            SessionID = chunk.SessionID,
            OperationID = chunk.OperationID,
            Offset = chunk.Range.Offset,
            Length = chunk.Range.Length,
            BlobName = chunk.Payload.BlobName,
            SHA512 = chunk.Payload.SHA512.ToArray(),
            Created = chunk.Created,
            Ordinal = ordinal + 1,
        });

        row.PlannedBlockCount += newBlocks;

        Apply(row,updatedsession);

        await context.SaveChangesAsync(cancel);

        await transaction.CommitAsync(cancel);

        return StreamTransferAppendResult.Create(chunk.Range,accepted:true,proposedManifest.AppendedLength,proposedManifest.StateVersion);
    }

    /// <inheritdoc/>
    public async Task<Boolean> PromoteStagedReadSessionAsync(TransferSessionState session , Guid sourcesessionid , String catalogname , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(session); ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        if(session.SessionID == Guid.Empty) { throw new ArgumentException("A transfer session identifier is required.",nameof(session)); }

        if(session.Family is not DataControlTransferFamily.Segmented || session.Mode is not DataItemTransferMode.PublishedRead)
        {
            throw new ArgumentException("Only segmented published-read state can promote a staged-read session.",nameof(session));
        }

        if(sourcesessionid == Guid.Empty) { throw new ArgumentException("A source session identifier is required.",nameof(sourcesessionid)); }

        (String sessionCatalogName,_) = GetManifestProjection(session);

        if(String.Equals(sessionCatalogName,catalogname,Ordinal) is false) { throw new InvalidOperationException("The transfer session manifest does not belong to the authorized catalog."); }

        DataItemTransferManifest? manifest = DataItemTransferManifest.Parse(session.ManifestJson);

        if(manifest is null || manifest.SessionID != session.SessionID || manifest.ItemID != session.ItemID || manifest.ItemInstanceToken != session.ItemInstanceToken || manifest.Mode is not DataItemTransferMode.PublishedRead || manifest.SourceSessionID.HasValue) { throw new InvalidOperationException("The promoted transfer manifest is not a published-read manifest."); }

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        Int32 updated = await context.TransferSessions
            .Where(item => item.SessionID == session.SessionID
                && item.ItemID == session.ItemID
                && item.CatalogName == catalogname
                && item.ItemInstanceToken == session.ItemInstanceToken
                && item.Family == (Int32)DataControlTransferFamily.Segmented
                && item.Mode == (Int32)DataItemTransferMode.StagedRead
                && item.SourceSessionID == sourcesessionid)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(item => item.Mode,(Int32)session.Mode)
                .SetProperty(item => item.Status,(Int32)session.Status)
                .SetProperty(item => item.StateVersion,session.StateVersion)
                .SetProperty(item => item.Updated,session.Updated)
                .SetProperty(item => item.ManifestJson,session.ManifestJson)
                .SetProperty(item => item.SourceSessionID,(Guid?)null)
                .SetProperty(item => item.LastActivity,DateTimeOffset.UtcNow)
                .SetProperty(item => item.ObjectBlobName,session.ObjectPayload?.BlobName)
                .SetProperty(item => item.ObjectLength,session.ObjectPayload?.Length)
                .SetProperty(item => item.ObjectSHA512,session.ObjectPayload?.SHA512.ToArray()),cancel);

        return updated != 0;
    }

    /// <inheritdoc/>
    public async Task<Boolean> DeleteSessionAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        Int32 deleted = await context.TransferSessions
            .Where(item => item.SessionID == sessionid && item.CatalogName == catalogname)
            .ExecuteDeleteAsync(cancel);

        return deleted != 0;
    }

    /// <inheritdoc/>
    public async Task<DataItemTransferOpenConflict?> FindOpenConflictAsync(Guid itemid , String catalogname , CancellationToken cancel = default)
    {
        if(itemid == Guid.Empty) { return null; }

        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        DataControlTransferSessionRow? row = await context.TransferSessions
            .AsNoTracking()
            .Where(item => item.ItemID == itemid
                && item.CatalogName == catalogname
                && (item.Mode == (Int32)DataItemTransferMode.Upload || item.Mode == (Int32)DataItemTransferMode.StreamUpload)
                && item.Status != (Int32)DataItemTransferStatus.Aborted
                && item.Status != (Int32)DataItemTransferStatus.Faulted)
            .OrderByDescending(item => item.Updated)
            .FirstOrDefaultAsync(cancel);

        return row is null ? null : new DataItemTransferOpenConflict
        {
            SessionID = row.SessionID,
            ItemID = row.ItemID,
            TransferFamily = (DataControlTransferFamily)row.Family,
            Mode = (DataItemTransferMode)row.Mode,
            Status = (DataItemTransferStatus)row.Status,
            Updated = row.Updated,
        };
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<SegmentedTransferRangeState>> LoadSegmentedRangesAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        List<DataControlSegmentedRangeRow> rows = await context.SegmentedRanges
            .AsNoTracking()
            .Where(item => item.SessionID == sessionid && item.Session.CatalogName == catalogname)
            .OrderBy(item => item.Offset)
            .ToListAsync(cancel);

        return rows.Select(ToState).ToArray();
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<DataItemTransferRange>> LoadSegmentedCoverageAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        return await context.SegmentedCoverage.AsNoTracking()
            .Where(item => item.SessionID == sessionid && item.Session.CatalogName == catalogname)
            .OrderBy(item => item.Offset)
            .Select(item => new DataItemTransferRange { Offset = item.Offset , Length = item.Length })
            .ToArrayAsync(cancel);
    }

    /// <inheritdoc/>
    public async Task<SegmentedTransferRangeState?> LoadSegmentedRangeAsync(Guid sessionid , Int64 offset , String catalogname , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        DataControlSegmentedRangeRow? row = await context.SegmentedRanges.AsNoTracking()
            .SingleOrDefaultAsync(item => item.SessionID == sessionid && item.Offset == offset && item.Session.CatalogName == catalogname,cancel);

        return row is null ? null : ToState(row);
    }

    /// <inheritdoc/>
    public async Task<Boolean> IsSegmentedRangeCoveredAsync(Guid sessionid , DataItemTransferRange range , String catalogname , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(range); ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);
        if(!range.Validate() || range.Length <= 0) { return false; }

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        DataControlSegmentedCoverageRow? preceding = await context.SegmentedCoverage.AsNoTracking()
            .Where(item => item.SessionID == sessionid && item.Session.CatalogName == catalogname && item.Offset <= range.Offset)
            .OrderByDescending(item => item.Offset).FirstOrDefaultAsync(cancel);

        return preceding is not null && preceding.Offset + preceding.Length >= range.EndOffsetExclusive;
    }

    /// <inheritdoc/>
    public async Task<Boolean> HasSegmentedRangeOverlapAsync(Guid sessionid , DataItemTransferRange range , String catalogname , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(range); ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);
        if(!range.Validate() || range.Length <= 0) { return false; }

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        DataControlSegmentedCoverageRow? preceding = await context.SegmentedCoverage.AsNoTracking()
            .Where(item => item.SessionID == sessionid && item.Session.CatalogName == catalogname && item.Offset <= range.Offset)
            .OrderByDescending(item => item.Offset).FirstOrDefaultAsync(cancel);

        if(preceding is not null && preceding.Offset + preceding.Length > range.Offset) { return true; }

        DataControlSegmentedCoverageRow? following = await context.SegmentedCoverage.AsNoTracking()
            .Where(item => item.SessionID == sessionid && item.Session.CatalogName == catalogname && item.Offset > range.Offset)
            .OrderBy(item => item.Offset).FirstOrDefaultAsync(cancel);

        return following is not null && following.Offset < range.EndOffsetExclusive;
    }

    /// <inheritdoc/>
    public async Task<SegmentedTransferWriteResult> CommitSegmentedRangeAsync(SegmentedTransferRangeState range , TransferSessionState updatedsession , Int64 expectedstateversion , String catalogname , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(range); ArgumentNullException.ThrowIfNull(updatedsession); ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        ValidateRange(range.SessionID,range.Range,range.Payload);

        if(range.SessionID != updatedsession.SessionID || expectedstateversion < 0)
        {
            throw new ArgumentException("A valid segmented range and updated session are required.");
        }

        (String updatedCatalog,_) = GetManifestProjection(updatedsession);

        DataItemTransferManifest? proposedManifest = DataItemTransferManifest.Deserialize(Encoding.UTF8.GetBytes(updatedsession.ManifestJson));

        if(!String.Equals(updatedCatalog,catalogname,Ordinal) || updatedsession.Family is not DataControlTransferFamily.Segmented || proposedManifest is null ||
            proposedManifest.ItemInstanceToken != updatedsession.ItemInstanceToken ||
            proposedManifest.Mode is not (DataItemTransferMode.Upload or DataItemTransferMode.PublishedRead) || !proposedManifest.CanWriteRange(range.Range) ||
            updatedsession.StateVersion != proposedManifest.StateVersion || updatedsession.Updated != proposedManifest.Updated ||
            proposedManifest.StateVersion != checked(expectedstateversion + 1))
        {
            throw new InvalidOperationException("The proposed segmented transfer state does not match the verified range.");
        }

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        await using var transaction = await context.Database.BeginTransactionAsync(cancel);

        DataControlTransferSessionRow? sessionRow = await context.TransferSessions
            .FromSql($"SELECT * FROM \"TransferSessions\" WHERE \"SessionID\" = {range.SessionID} FOR UPDATE")
            .SingleOrDefaultAsync(cancel);

        if(sessionRow is null || !String.Equals(sessionRow.CatalogName,catalogname,Ordinal) || sessionRow.ItemID != updatedsession.ItemID || sessionRow.ItemInstanceToken != updatedsession.ItemInstanceToken ||
            sessionRow.Family != (Int32)DataControlTransferFamily.Segmented || sessionRow.Mode != (Int32)updatedsession.Mode)
        {
            throw new InvalidOperationException("The segmented range parent session does not belong to the authorized catalog or identity.");
        }

        TransferSessionState currentState = ToState(sessionRow);

        DataItemTransferManifest? currentManifest = DataItemTransferManifest.Deserialize(Encoding.UTF8.GetBytes(currentState.ManifestJson));

        if(currentManifest is null || currentManifest.SessionID != range.SessionID || currentManifest.ItemID != updatedsession.ItemID || currentManifest.ItemInstanceToken != updatedsession.ItemInstanceToken || currentManifest.Mode != updatedsession.Mode ||
            currentManifest.Mode is not (DataItemTransferMode.Upload or DataItemTransferMode.PublishedRead) || currentManifest.Status is DataItemTransferStatus.Aborted or DataItemTransferStatus.Publishing or DataItemTransferStatus.Published or DataItemTransferStatus.Faulted)
        {
            throw new InvalidOperationException("The segmented transfer session is not writable.");
        }

        DataControlSegmentedRangeRow? existing = await context.SegmentedRanges
            .SingleOrDefaultAsync(item => item.SessionID == range.SessionID && item.Offset == range.Range.Offset,cancel);

        if(existing is not null)
        {
            Boolean sameContent = existing.Length == range.Range.Length && existing.SHA512.AsSpan().SequenceEqual(range.Payload.SHA512);

            if(!sameContent) { return SegmentedTransferWriteResult.Create(range.Range,accepted:false,currentManifest.StateVersion); }

            DataControlSegmentedCoverageRow? containing = await context.SegmentedCoverage
                .Where(item => item.SessionID == range.SessionID && item.Offset <= range.Range.Offset)
                .OrderByDescending(item => item.Offset)
                .FirstOrDefaultAsync(cancel);

            if(containing is null || containing.Offset + containing.Length < range.Range.EndOffsetExclusive)
            {
                throw new InvalidOperationException("The committed segmented range is missing from its coverage.");
            }

            await transaction.CommitAsync(cancel);

            return SegmentedTransferWriteResult.Create(range.Range,accepted:true,currentManifest.StateVersion,replayed:true) with { CompletedCoverage = sessionRow.CoveredBytes == currentManifest.StreamLength , Updated = currentManifest.Updated };
        }

        DataControlSegmentedRangeRow? preceding = await context.SegmentedRanges
            .Where(item => item.SessionID == range.SessionID && item.Offset < range.Range.Offset)
            .OrderByDescending(item => item.Offset)
            .FirstOrDefaultAsync(cancel);

        DataControlSegmentedRangeRow? following = await context.SegmentedRanges
            .Where(item => item.SessionID == range.SessionID && item.Offset > range.Range.Offset)
            .OrderBy(item => item.Offset)
            .FirstOrDefaultAsync(cancel);

        Boolean overlaps = preceding is not null && preceding.Offset + preceding.Length > range.Range.Offset ||
            following is not null && following.Offset < range.Range.EndOffsetExclusive;

        if(overlaps || sessionRow.StateVersion != expectedstateversion || currentManifest.StateVersion != expectedstateversion || proposedManifest.StateVersion != expectedstateversion + 1)
        {
            return SegmentedTransferWriteResult.Create(range.Range,accepted:false,currentManifest.StateVersion);
        }

        Int64 newBlocks = currentManifest.Mode is DataItemTransferMode.Upload ? TenantTransferStorageLimits.CountBlocks(range.Range.Length) : 0;

        TenantTransferStorageLimits.ValidateBudget(range.SessionID,sessionRow.PlannedBlockCount,newBlocks);

        Int64 coveredBytes = checked(sessionRow.CoveredBytes + range.Range.Length);

        if(coveredBytes > currentManifest.StreamLength) { throw new InvalidOperationException("The segmented coverage exceeds the stream length."); }

        DataControlSegmentedCoverageRow? coverageBefore = await context.SegmentedCoverage
            .Where(item => item.SessionID == range.SessionID && item.Offset < range.Range.Offset)
            .OrderByDescending(item => item.Offset)
            .FirstOrDefaultAsync(cancel);

        DataControlSegmentedCoverageRow? coverageAfter = await context.SegmentedCoverage
            .Where(item => item.SessionID == range.SessionID && item.Offset >= range.Range.Offset)
            .OrderBy(item => item.Offset)
            .FirstOrDefaultAsync(cancel);

        if(coverageBefore is not null && coverageBefore.Offset + coverageBefore.Length > range.Range.Offset ||
            coverageAfter is not null && coverageAfter.Offset < range.Range.EndOffsetExclusive)
        {
            throw new InvalidOperationException("The segmented range overlaps existing coverage.");
        }

        Boolean mergeBefore = coverageBefore is not null && coverageBefore.Offset + coverageBefore.Length == range.Range.Offset;

        Boolean mergeAfter = coverageAfter is not null && coverageAfter.Offset == range.Range.EndOffsetExclusive;

        if(mergeBefore)
        {
            coverageBefore!.Length = checked(coverageBefore.Length + range.Range.Length + (mergeAfter ? coverageAfter!.Length : 0));
        }
        else
        {
            context.SegmentedCoverage.Add(new DataControlSegmentedCoverageRow
            {
                SessionID = range.SessionID,
                Offset = range.Range.Offset,
                Length = checked(range.Range.Length + (mergeAfter ? coverageAfter!.Length : 0)),
            });
        }

        if(mergeAfter) { context.SegmentedCoverage.Remove(coverageAfter!); }

        Int64 ordinal = await context.SegmentedRanges
            .Where(item => item.SessionID == range.SessionID)
            .Select(item => (Int64?)item.Ordinal)
            .MaxAsync(cancel)
             ?? -1;

        context.SegmentedRanges.Add(new DataControlSegmentedRangeRow
        {
            SessionID = range.SessionID,
            Offset = range.Range.Offset,
            Length = range.Range.Length,
            BlobName = range.Payload.BlobName,
            SHA512 = range.Payload.SHA512.ToArray(),
            Created = range.Created,
            Ordinal = ordinal + 1,
        });

        sessionRow.PlannedBlockCount += newBlocks;

        sessionRow.CoveredBytes = coveredBytes;

        DataItemTransferManifest committedManifest = proposedManifest with
        {
            Status = coveredBytes == currentManifest.StreamLength ? DataItemTransferStatus.Complete : DataItemTransferStatus.Open,
            RealizedRanges = Array.Empty<DataItemTransferRange>(),
        };

        Apply(sessionRow,updatedsession with { Status = committedManifest.Status , ManifestJson = DataControlTransferSnapshot.SanitizeManifest(Encoding.UTF8.GetString(committedManifest.Serialize())) });

        await context.SaveChangesAsync(cancel);

        await transaction.CommitAsync(cancel);

        return SegmentedTransferWriteResult.Create(range.Range,accepted:true,proposedManifest.StateVersion) with { CompletedCoverage = coveredBytes == currentManifest.StreamLength , Updated = committedManifest.Updated };
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<StreamTransferChunkState>> LoadStreamChunksAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        List<DataControlStreamChunkRow> rows = await context.StreamChunks
            .AsNoTracking()
            .Where(item => item.SessionID == sessionid && item.Session.CatalogName == catalogname)
            .OrderBy(item => item.Offset)
            .ToListAsync(cancel);

        return rows.Select(ToState).ToArray();
    }

    /// <inheritdoc/>
    public async Task<TransferPublicationJournalState?> LoadJournalAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        DataControlPublicationJournalRow? row = await context.PublicationJournals
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.SessionID == sessionid && item.CatalogName == catalogname,cancel);

        return row is null ? null : ToState(row);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<TransferPublicationRecoveryCandidate>> LoadRecoverableJournalCandidatesAsync(DateTimeOffset cutoff , TransferPublicationRecoveryCandidate? after , Int32 pageSize , CancellationToken cancel = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        IQueryable<DataControlPublicationJournalRow> query = context.PublicationJournals
            .AsNoTracking()
            .Where(item => item.Updated <= cutoff
                && item.Phase != (Int32)TransferPublicationPhase.OperatorActionRequired
                && item.Phase != (Int32)TransferPublicationPhase.Failed);

        if(after is not null)
        {
            query = query.Where(item => item.Updated > after.Updated
                || (item.Updated == after.Updated && item.SessionID.CompareTo(after.SessionID) > 0));
        }

        return await query
            .OrderBy(item => item.Updated)
            .ThenBy(item => item.SessionID)
            .Select(item => new TransferPublicationRecoveryCandidate(item.Updated,item.SessionID,item.ItemID,item.OperationID,item.CatalogName))
            .Take(pageSize)
            .ToListAsync(cancel);
    }

    /// <inheritdoc/>
    public async Task SaveJournalAsync(TransferPublicationJournalState journal , String catalogname , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(journal); ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        if(journal.SessionID == Guid.Empty) { throw new ArgumentException("A transfer session identifier is required.",nameof(journal)); }

        if(journal.OperationID == Guid.Empty) { throw new ArgumentException("A publish operation identifier is required.",nameof(journal)); }

        ArgumentOutOfRangeException.ThrowIfEqual(journal.ItemInstanceToken,Guid.Empty);

        if(String.Equals(journal.CatalogName,catalogname,Ordinal) is false) { throw new InvalidOperationException("The publication journal does not belong to the authorized catalog."); }

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        await using var transaction = await context.Database.BeginTransactionAsync(cancel);

        DataControlTransferSessionRow? session = await context.TransferSessions
            .FromSql($"SELECT * FROM \"TransferSessions\" WHERE \"SessionID\" = {journal.SessionID} FOR UPDATE")
            .SingleOrDefaultAsync(cancel);

        if(session is null || !String.Equals(session.CatalogName,catalogname,Ordinal) || session.ItemID != journal.ItemID || session.ItemInstanceToken != journal.ItemInstanceToken ||
            (session.Family == (Int32)DataControlTransferFamily.Segmented && journal.Family is not TransferPublicationFamily.Segmented) ||
            (session.Family == (Int32)DataControlTransferFamily.Stream && journal.Family is not TransferPublicationFamily.Stream) ||
            session.Family != (Int32)DataControlTransferFamily.Segmented && session.Family != (Int32)DataControlTransferFamily.Stream)
        {
            throw new InvalidOperationException("The publication journal does not match an existing transfer session item instance and identity.");
        }

        DataControlPublicationJournalRow? row = await context.PublicationJournals
            .FromSql($"SELECT * FROM \"PublicationJournals\" WHERE \"SessionID\" = {journal.SessionID} FOR UPDATE")
            .SingleOrDefaultAsync(cancel);

        if(row is null)
        {
            row = new DataControlPublicationJournalRow { SessionID = journal.SessionID };

            context.PublicationJournals.Add(row);
        }
        else if (!String.Equals(row.CatalogName,catalogname,Ordinal)
            || row.ItemID != journal.ItemID
            || row.ItemInstanceToken != journal.ItemInstanceToken
            || row.OperationID != journal.OperationID)
        {
            throw new InvalidOperationException("The publication journal catalog, item instance token, and identity are immutable.");
        }

        Apply(row,journal);

        await context.SaveChangesAsync(cancel);

        await transaction.CommitAsync(cancel);
    }

    /// <inheritdoc/>
    public async Task<Boolean> DeleteJournalAsync(Guid sessionid , String catalogname , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(catalogname);

        await using DataControlDbContext context = await this.contexts.CreateDbContextAsync(cancel);

        Int32 deleted = await context.PublicationJournals
            .Where(item => item.SessionID == sessionid && item.CatalogName == catalogname)
            .ExecuteDeleteAsync(cancel);

        return deleted != 0;
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="ApplySessionState"]/*'/>*/
    private static void Apply(DataControlTransferSessionRow row , TransferSessionState state)
    {
        row.ItemID = state.ItemID;
        row.ItemInstanceToken = state.ItemInstanceToken;
        row.Family = (Int32)state.Family;
        row.Mode = (Int32)state.Mode;
        row.Status = (Int32)state.Status;
        row.StateVersion = state.StateVersion;
        row.Created = state.Created;
        row.Updated = state.Updated;
        DateTimeOffset now = DateTimeOffset.UtcNow;
        if(row.LastActivity < now) { row.LastActivity = now; }
        row.ManifestJson = state.ManifestJson;
        (row.CatalogName,row.SourceSessionID) = GetManifestProjection(state);
        row.ObjectBlobName = state.ObjectPayload?.BlobName;
        row.ObjectLength = state.ObjectPayload?.Length;
        row.ObjectSHA512 = state.ObjectPayload?.SHA512.ToArray();
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="GetManifestProjection"]/*'/>*/
    private static (String CatalogName, Guid? SourceSessionID) GetManifestProjection(TransferSessionState state)
    {
        if(state.Family is DataControlTransferFamily.Segmented)
        {
            DataItemTransferManifest? manifest = DataItemTransferManifest.Parse(state.ManifestJson);

            if(manifest is null || manifest.SessionID != state.SessionID || manifest.ItemID != state.ItemID || manifest.ItemInstanceToken != state.ItemInstanceToken || state.ItemInstanceToken == Guid.Empty || String.IsNullOrWhiteSpace(manifest.CatalogName))
            {
                throw new InvalidOperationException("The transfer session manifest does not contain a valid relational projection.");
            }

            return (manifest.CatalogName,manifest.SourceSessionID);
        }

        if(state.Family is DataControlTransferFamily.Stream)
        {
            DataStreamTransferManifest? manifest = DataStreamTransferManifest.Parse(state.ManifestJson);

            if(manifest is null || manifest.SessionID != state.SessionID || manifest.ItemID != state.ItemID || manifest.ItemInstanceToken != state.ItemInstanceToken || state.ItemInstanceToken == Guid.Empty || String.IsNullOrWhiteSpace(manifest.CatalogName))
            {
                throw new InvalidOperationException("The transfer session manifest does not contain a valid relational projection.");
            }

            return (manifest.CatalogName,manifest.SourceSessionID);
        }

        throw new InvalidOperationException("The transfer session family does not support a relational projection.");
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="ApplyJournalState"]/*'/>*/
    private static void Apply(DataControlPublicationJournalRow row , TransferPublicationJournalState state)
    {
        row.ItemID = state.ItemID;
        row.ItemInstanceToken = state.ItemInstanceToken;
        row.OperationID = state.OperationID;
        row.Family = (Int32)state.Family;
        row.Phase = (Int32)state.Phase;
        row.ExpectedStateVersion = state.ExpectedStateVersion;
        row.CatalogName = state.CatalogName;
        row.FailureCount = state.FailureCount;
        row.LastError = state.LastError;
        row.FinalObjectBlobName = state.FinalObject?.BlobName;
        row.FinalObjectLength = state.FinalObject?.Length;
        row.FinalObjectSHA512 = state.FinalObject?.SHA512.ToArray();
        row.FinalStreamLength = state.FinalStreamLength;
        row.IntegrityAlgorithm = state.Integrity?.Algorithm;
        row.IntegrityDigest = state.Integrity?.Digest.ToArray();
        row.IntegrityLength = state.Integrity?.Length;
        row.IntegritySegmentCount = state.Integrity?.SegmentCount;
        row.Created = state.Created;
        row.Updated = state.Updated;
        row.JournalJson = state.JournalJson;
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="ToSessionState"]/*'/>*/
    private static TransferSessionState ToState(DataControlTransferSessionRow row)
    {
        return new()
        {
            SessionID = row.SessionID,
            ItemID = row.ItemID,
            ItemInstanceToken = row.ItemInstanceToken,
            Family = (DataControlTransferFamily)row.Family,
            Mode = (DataItemTransferMode)row.Mode,
            Status = (DataItemTransferStatus)row.Status,
            StateVersion = row.StateVersion,
            PlannedBlockCount = row.PlannedBlockCount,
            CoveredBytes = row.CoveredBytes,
            Created = row.Created,
            Updated = row.Updated,
            LastActivity = row.LastActivity,
            ManifestJson = row.ManifestJson,
            ObjectPayload = ToBlob(row.ObjectBlobName,row.ObjectLength,row.ObjectSHA512),
        };
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="ToSegmentedRangeState"]/*'/>*/
    private static SegmentedTransferRangeState ToState(DataControlSegmentedRangeRow row)
    {
        return new()
        {
            SessionID = row.SessionID,
            Range = new() { Offset = row.Offset , Length = row.Length },
            Payload = new() { BlobName = row.BlobName , Length = row.Length , SHA512 = row.SHA512.ToArray() },
            Created = row.Created,
            Ordinal = row.Ordinal,
        };
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="ToStreamChunkState"]/*'/>*/
    private static StreamTransferChunkState ToState(DataControlStreamChunkRow row)
    {
        return new()
        {
            SessionID = row.SessionID,
            OperationID = row.OperationID,
            Range = new() { Offset = row.Offset , Length = row.Length },
            Payload = new() { BlobName = row.BlobName , Length = row.Length , SHA512 = row.SHA512.ToArray() },
            Created = row.Created,
            Ordinal = row.Ordinal,
        };
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="ToJournalState"]/*'/>*/
    private static TransferPublicationJournalState ToState(DataControlPublicationJournalRow row)
    {
        if((row.FinalObjectBlobName is not null || row.FinalObjectLength is not null || row.FinalObjectSHA512 is not null) &&
            (String.IsNullOrWhiteSpace(row.FinalObjectBlobName) || row.FinalObjectLength is null or <= 0 || row.FinalObjectSHA512 is null || row.FinalObjectSHA512.Length != 64))
        {
            throw new InvalidOperationException("The publication journal final object reference is incomplete or invalid.");
        }

        return new()
        {
            SessionID = row.SessionID,
            ItemID = row.ItemID,
            ItemInstanceToken = row.ItemInstanceToken,
            OperationID = row.OperationID,
            Family = (TransferPublicationFamily)row.Family,
            Phase = (TransferPublicationPhase)row.Phase,
            ExpectedStateVersion = row.ExpectedStateVersion,
            CatalogName = row.CatalogName,
            FailureCount = row.FailureCount,
            LastError = row.LastError,
            FinalObject = ToBlob(row.FinalObjectBlobName,row.FinalObjectLength,row.FinalObjectSHA512),
            FinalStreamLength = row.FinalStreamLength,
            Integrity = ToJournalIntegrity(row),
            JournalJson = row.JournalJson,
            Created = row.Created,
            Updated = row.Updated,
        };
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="ToJournalIntegrity"]/*'/>*/
    private static TransferContentIntegrity? ToJournalIntegrity(DataControlPublicationJournalRow row)
    {
        return row.IntegrityDigest is null || row.IntegrityLength is null || row.IntegritySegmentCount is null
            ? null
            : new TransferContentIntegrity
            {
                Algorithm = row.IntegrityAlgorithm ?? TransferContentIntegrityCalculator.Algorithm,
                Digest = row.IntegrityDigest.ToArray(),
                Length = row.IntegrityLength.Value,
                SegmentCount = row.IntegritySegmentCount.Value,
            };
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="ToBlob"]/*'/>*/
    private static TransferBlobReference? ToBlob(String? name , Int64? length , Byte[]? sha512)
    {
        return String.IsNullOrWhiteSpace(name) || length is null || sha512 is null
            ? null
            : new TransferBlobReference { BlobName = name , Length = length.Value , SHA512 = sha512.ToArray() };
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="ValidateRange"]/*'/>*/
    private static void ValidateRange(Guid sessionid , DataItemTransferRange range , TransferBlobReference payload)
    {
        if(sessionid == Guid.Empty) { throw new ArgumentException("A transfer session identifier is required."); }

        if(range is null || range.Validate() is false || range.Length == 0) { throw new ArgumentException("A non-empty valid transfer range is required.",nameof(range)); }

        ValidatePayload(payload);

        if(payload.Length != range.Length) { throw new ArgumentException("The Blob length must equal the transfer range length.",nameof(payload)); }
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="ValidatePayload"]/*'/>*/
    private static void ValidatePayload(TransferBlobReference payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        if(String.IsNullOrWhiteSpace(payload.BlobName)) { throw new ArgumentException("A Blob name is required.",nameof(payload)); }

        if(payload.Length <= 0) { throw new ArgumentOutOfRangeException(nameof(payload),"The Blob length must be greater than zero."); }

        if(payload.SHA512.Length != 64) { throw new ArgumentException("A SHA-512 digest is required.",nameof(payload)); }
    }

    /**<include file='PostgresTransferStateStore.xml' path='PostgresTransferStateStore/class[@name="PostgresTransferStateStore"]/method[@name="EnsureSamePayload"]/*'/>*/
    private static void EnsureSamePayload(String blobName , Int64 length , Byte[] sha512 , TransferBlobReference payload , String kind)
    {
        if(String.Equals(blobName,payload.BlobName,Ordinal) is false || length != payload.Length || sha512.SequenceEqual(payload.SHA512) is false)
        {
            throw new InvalidOperationException($"The existing {kind} has different immutable Blob metadata.");
        }
    }
}
