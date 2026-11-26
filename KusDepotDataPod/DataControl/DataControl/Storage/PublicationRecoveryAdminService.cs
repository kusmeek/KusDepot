namespace DataPodServices.DataControl;

/**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/interface[@name="IPublicationRecoveryAdminService"]/main/*'/>*/
internal interface IPublicationRecoveryAdminService
{
    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/interface[@name="IPublicationRecoveryAdminService"]/method[@name="InspectAsync"]/*'/>*/
    Task<PublicationRecoveryFootprint?> InspectAsync(Guid sessionid , CancellationToken cancel = default);

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/interface[@name="IPublicationRecoveryAdminService"]/method[@name="PurgeAsync"]/*'/>*/
    Task<PurgePublicationResponse?> PurgeAsync(PurgePublicationRequest request , CancellationToken cancel = default);
}

/**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/main/*'/>*/
internal sealed partial class PublicationRecoveryAdminService : IPublicationRecoveryAdminService
{
    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/field[@name="sessions"]/*'/>*/
    private readonly IDataControlTransferSessionStore sessions;

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/field[@name="coordinator"]/*'/>*/
    private readonly IDataItemTransferCoordinator coordinator;

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/field[@name="storage"]/*'/>*/
    private readonly ITenantTransferStorageContext storage;

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/field[@name="dataConfigs"]/*'/>*/
    private readonly IDataControlDataConfigsClient dataConfigs;

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/constructor[@name="PublicationRecoveryAdminService"]/*'/>*/
    public PublicationRecoveryAdminService(IDataControlTransferSessionStore sessions , IDataItemTransferCoordinator coordinator , ITenantTransferStorageContext storage , IDataControlDataConfigsClient dataConfigs)
    {
        this.sessions = sessions ?? throw new ArgumentNullException(nameof(sessions));
        this.coordinator = coordinator ?? throw new ArgumentNullException(nameof(coordinator));
        this.storage = storage ?? throw new ArgumentNullException(nameof(storage));
        this.dataConfigs = dataConfigs ?? throw new ArgumentNullException(nameof(dataConfigs));
    }

    /// <inheritdoc/>
    public Task<PublicationRecoveryFootprint?> InspectAsync(Guid sessionid , CancellationToken cancel = default)
    {
        if(sessionid == Guid.Empty) { throw new ArgumentException("A transfer session identifier is required.",nameof(sessionid)); }

        return LoadFootprintAsync(sessionid,cancel);
    }

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/method[@name="LoadFootprintAsync"]/*'/>*/
    private async Task<PublicationRecoveryFootprint?> LoadFootprintAsync(Guid sessionid , CancellationToken cancel)
    {
        TransferRecoveryStateSnapshot snapshot = await this.sessions.LoadRecoveryStateForAdminAsync(sessionid,cancel);

        if(snapshot.Session is not TransferSessionState session) { return null; }

        return BuildFootprint(snapshot);
    }

    /// <inheritdoc/>
    public async Task<PurgePublicationResponse?> PurgeAsync(PurgePublicationRequest request , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if(request.SessionID == Guid.Empty || request.ItemID == Guid.Empty || String.IsNullOrWhiteSpace(request.CatalogName) || request.ExpectedSessionStateVersion < 0)
        {
            throw new ArgumentException("Session, item, catalog, and expected state version are required.",nameof(request));
        }

        if(!request.ConfirmPurge)
        {
            throw new ArgumentException("Explicit purge confirmation is required.",nameof(request));
        }

        if(request.ExpectedOperationID.HasValue != request.ExpectedPhase.HasValue || request.ExpectedOperationID.HasValue != request.ExpectedJournalUpdated.HasValue)
        {
            throw new ArgumentException("Expected operation, phase, and journal update time must either all be specified or all be absent.",nameof(request));
        }

        ITransferMutationScope scope = await this.coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,cancel);

        await using(scope)
        {
            TransferRecoveryStateSnapshot snapshot = await this.sessions.LoadRecoveryStateForAdminAsync(request.SessionID,cancel);

            if(snapshot.Session is not TransferSessionState session || session.ItemID != request.ItemID || session.ItemInstanceToken == Guid.Empty || session.StateVersion != request.ExpectedSessionStateVersion || !String.Equals(GetSessionCatalogName(session),request.CatalogName,Ordinal))
            {
                return null;
            }

            TransferPublicationJournalState? journal = snapshot.Journal;

            if((journal is null) != !request.ExpectedOperationID.HasValue) { return null; }

            if(journal is null)
            {
                if(request.ExpectedOperationID.HasValue) { return null; }
            }
            else if(journal.ItemID != request.ItemID || journal.ItemInstanceToken != session.ItemInstanceToken || !String.Equals(journal.CatalogName,request.CatalogName,Ordinal) || journal.OperationID != request.ExpectedOperationID || journal.Phase != request.ExpectedPhase || journal.Updated != request.ExpectedJournalUpdated)
            {
                return null;
            }

            StorageSilo? silo = await this.dataConfigs.ResolveRecoveryStorageAsync(request.CatalogName,cancel);

            if(silo is null || !String.Equals(silo.CatalogName,request.CatalogName,Ordinal)) { return null; }

            this.storage.Set(silo);

            Boolean stagingDeleted = await this.storage.Get(request.SessionID,request.ItemID,session.ItemInstanceToken).DeleteTransferAsync(cancel);

            if(!stagingDeleted) { return null; }

            Boolean databaseDeleted = await this.sessions.PurgeRecoveryStateAsync(request.SessionID,request.ItemID,request.ExpectedSessionStateVersion,request.ExpectedOperationID,request.ExpectedPhase,request.ExpectedJournalUpdated,cancel);

            if(!databaseDeleted) { return null; }

            PublicationRecoveryFootprint footprint = BuildFootprint(snapshot);

            return new PurgePublicationResponse
            {
                Purged = true,
                SessionID = request.SessionID,
                OperationID = journal?.OperationID,
                StagingArtifactsDeleted = true,
                DatabaseStateDeleted = true,
                FinalArtifactsDeleted = false,
                CatalogDataDeleted = false,
                FinalArtifactCandidatesLeftUntouched = footprint.FinalArtifacts,
                Message = "DataControl database state and transfer staging Blobs were removed. Final published artifacts and catalog data were left unchanged.",
            };
        }
    }

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/method[@name="BuildFootprint"]/*'/>*/
    private static PublicationRecoveryFootprint BuildFootprint(TransferRecoveryStateSnapshot snapshot)
    {
        TransferSessionState session = snapshot.Session ?? throw new InvalidOperationException("The transfer session is unavailable.");

        TransferPublicationJournalState? journal = snapshot.Journal;

        DataItemTransferManifest? segmented = session.Family is DataControlTransferFamily.Segmented ? DataItemTransferManifest.Parse(session.ManifestJson) : null;

        DataStreamTransferManifest? stream = session.Family is DataControlTransferFamily.Stream ? DataStreamTransferManifest.Parse(session.ManifestJson) : null;

        List<PublicationRecoveryArtifact> staging = new();

        if(session.ObjectPayload is TransferBlobReference objectPayload) { staging.Add(ToArtifact(PublicationRecoveryArtifactKinds.StagedObject,objectPayload)); }

        staging.AddRange(snapshot.SegmentedRanges.Select(item => ToArtifact(PublicationRecoveryArtifactKinds.SegmentedRange,item.Payload,offset:item.Range.Offset)));

        staging.AddRange(snapshot.StreamChunks.Select(item => ToArtifact(PublicationRecoveryArtifactKinds.StreamChunk,item.Payload,item.OperationID,item.Range.Offset)));

        if(journal?.FinalObject is TransferBlobReference publicationObject) { staging.Add(ToArtifact(PublicationRecoveryArtifactKinds.PublicationObjectRecoveryCopy,publicationObject)); }

        List<PublicationRecoveryArtifact> final = new();

        TransferInstanceStoragePath instancePaths = TransferStoragePath.ForInstance(session.ItemInstanceToken);

        Byte[]? objectSHA512 = journal?.FinalObject?.SHA512 ?? segmented?.ObjectSHA512 ?? stream?.ObjectSHA512;

        if(session.Mode is DataItemTransferMode.Upload or DataItemTransferMode.StreamUpload or DataItemTransferMode.PublishedRead ||
            session.Status is DataItemTransferStatus.Publishing or DataItemTransferStatus.Published || journal is not null)
        {
            final.Add(new PublicationRecoveryArtifact
            {
                Kind = PublicationRecoveryArtifactKinds.FinalObjectCandidate,
                BlobName = instancePaths.PublishedObjectName,
                Length = journal?.FinalObject?.Length ?? session.ObjectPayload?.Length,
                SHA512 = objectSHA512?.ToArray() ?? journal?.FinalObject?.SHA512.ToArray(),
            });
        }

        Int64? streamLength = journal?.FinalStreamLength ?? segmented?.StreamLength ?? stream?.AppendedLength;

        TransferContentIntegrity? integrity = journal?.Integrity ?? segmented?.Integrity ?? stream?.Integrity;

        Boolean hasStream = stream is not null || segmented?.ObjectInfo?.ContentStreamed is true ||
            segmented is null && stream is null && (journal?.FinalStreamLength.HasValue is true || journal?.Integrity is not null);

        if(hasStream)
        {
            final.Add(new PublicationRecoveryArtifact { Kind = PublicationRecoveryArtifactKinds.FinalStreamCandidate , BlobName = instancePaths.PublishedStreamName , Length = streamLength });
        }

        final.Add(new PublicationRecoveryArtifact { Kind = PublicationRecoveryArtifactKinds.FinalIntegrityCandidate , BlobName = instancePaths.PublishedIntegrityName });

        return new PublicationRecoveryFootprint
        {
            InspectedAt = DateTimeOffset.UtcNow,
            ExternalArtifactExistenceChecked = false,
            SessionID = session.SessionID,
            ItemID = session.ItemID,
            CatalogName = journal?.CatalogName ?? GetSessionCatalogName(session),
            Family = session.Family,
            Mode = session.Mode,
            Status = session.Status,
            SessionStateVersion = session.StateVersion,
            SessionUpdated = session.Updated,
            OperationID = journal?.OperationID,
            Phase = journal?.Phase,
            JournalUpdated = journal?.Updated,
            FailureCount = journal?.FailureCount,
            LastError = SanitizeLastError(journal?.LastError),
            ExpectedStateVersion = journal?.ExpectedStateVersion,
            ExpectedObjectLength = journal?.FinalObject?.Length ?? session.ObjectPayload?.Length,
            ExpectedObjectSHA512 = objectSHA512?.ToArray() ?? journal?.FinalObject?.SHA512.ToArray(),
            ExpectedStreamLength = streamLength,
            ExpectedStreamIntegrity = integrity,
            StagingPrefix = String.Concat(instancePaths.Prefix,"transfers/",session.SessionID.ToString("N",CultureInfo.InvariantCulture),"/"),
            StagingArtifacts = staging.ToArray(),
            FinalArtifacts = final.ToArray(),
            IntegrityEvidence = integrity,
        };
    }

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/method[@name="GetSessionCatalogName"]/*'/>*/
    private static String GetSessionCatalogName(TransferSessionState session)
    {
        return session.Family is DataControlTransferFamily.Segmented
            ? DataItemTransferManifest.Parse(session.ManifestJson)?.CatalogName ?? String.Empty
            : DataStreamTransferManifest.Parse(session.ManifestJson)?.CatalogName ?? String.Empty;
    }

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/method[@name="ToArtifact"]/*'/>*/
    private static PublicationRecoveryArtifact ToArtifact(String kind , TransferBlobReference reference , Guid? operationid = null , Int64? offset = null)
    {
        return new PublicationRecoveryArtifact { Kind = kind , BlobName = reference.BlobName , Length = reference.Length , SHA512 = reference.SHA512.ToArray() , OperationID = operationid , Offset = offset };
    }

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/method[@name="SanitizeLastError"]/*'/>*/
    private static String? SanitizeLastError(String? error)
    {
        if(String.IsNullOrWhiteSpace(error)) { return error; }

        String sanitized = CredentialAssignmentRegex().Replace(error,"$1=[REDACTED]");
        sanitized = SasParameterRegex().Replace(sanitized,"$1[REDACTED]");
        sanitized = BearerTokenRegex().Replace(sanitized,"Bearer [REDACTED]");
        sanitized = JwtTokenRegex().Replace(sanitized,"[REDACTED-TOKEN]");
        sanitized = UrlRegex().Replace(sanitized,"[REDACTED-URL]");

        return sanitized.Length <= 1024 ? sanitized : sanitized[..1024];
    }

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/method[@name="CredentialAssignmentRegex"]/*'/>*/
    [GeneratedRegex(@"\b(AccountKey|SharedAccessSignature|password|pwd|client_secret|access_token)\s*=\s*[^;\s&]+",RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex CredentialAssignmentRegex();

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/method[@name="SasParameterRegex"]/*'/>*/
    [GeneratedRegex(@"([?&](?:sig|se|sp|sv|sr|st|skoid|sktid|skt|ske|sks|skv)=)[^&\s]+",RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex SasParameterRegex();

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/method[@name="BearerTokenRegex"]/*'/>*/
    [GeneratedRegex(@"\bBearer\s+[A-Za-z0-9._~-]+",RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex BearerTokenRegex();

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/method[@name="JwtTokenRegex"]/*'/>*/
    [GeneratedRegex(@"\beyJ[A-Za-z0-9_-]{10,}\.[A-Za-z0-9_-]+\.[A-Za-z0-9_-]+\b",RegexOptions.CultureInvariant)]
    private static partial Regex JwtTokenRegex();

    /**<include file='PublicationRecoveryAdminService.xml' path='PublicationRecoveryAdminService/class[@name="PublicationRecoveryAdminService"]/method[@name="UrlRegex"]/*'/>*/
    [GeneratedRegex(@"https?://[^\s<>""']+",RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex UrlRegex();
}
