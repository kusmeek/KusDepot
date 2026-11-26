namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapPublishSegmentedTransfer(WebApplication application)
    {
        application.MapPost("PublishSegmentedTransfer",
                   ([FromBody] PublishSegmentedTransferRequest? request,
                   [FromServices] IDataControlPublicationTransferStore store,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataItemSegmentedTransferService transfer,
                   [FromServices] IDataItemTransferCoordinator coordinator,
                   [FromServices] IDataControlNotificationPublisher notifications,
                   HttpContext hc) => { return PublishSegmentedTransfer(request,store,gf,transfer,coordinator,notifications,hc); })
                   .Produces<PublishSegmentedTransferResponse>(StatusCodes.Status200OK)
                   .WithName("PublishSegmentedTransfer");
    }

    private async Task<IResult> PublishSegmentedTransfer(PublishSegmentedTransferRequest? request , IDataControlPublicationTransferStore store , IGrainFactory gf , IDataItemSegmentedTransferService transfer , IDataItemTransferCoordinator coordinator , IDataControlNotificationPublisher notifications , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tc = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(PublishSegmentedTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(PublishSegmentedTransferBadArgID,id); SetErr(_); return BadRequest(PublishSegmentedTransferBadArg); }

            var dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? s = await dc.ResolveStorageAsync(t,CatalogAccess.Write,tc,hc.RequestAborted);

            if(s is null) { Log.Error(PublishSegmentedTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(s);

            ITransferMutationScope mutationScope = await coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,hc.RequestAborted);

            await using (mutationScope)
            {
                DataItemTransferManifest? manifest = await store.LoadSegmentedManifestAsync(request.SessionID,hc.RequestAborted);

                if(manifest is null || manifest.ItemID != request.ItemID || manifest.ItemInstanceToken == Guid.Empty) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                CatalogItemState? itemState = await gf.GetGrain<ICatalogDB>(s.CatalogName).GetItemState(request.ItemID,tc,hc.RequestAborted);

                if(itemState is not null && (itemState.IsPendingDelete || itemState.ItemInstanceToken != manifest.ItemInstanceToken))
                {
                    throw new OperationFailedException(PublishSegmentedTransferJournalMismatch,SegmentedTransferFailureCode.JournalMismatch);
                }

                TransferPublicationJournal? journal = await store.LoadJournalAsync(request.SessionID,s.CatalogName,hc.RequestAborted);

                if(journal is not null && (journal.ItemID != request.ItemID || journal.ItemInstanceToken != manifest.ItemInstanceToken || journal.Family is not TransferPublicationFamily.Segmented || !String.Equals(journal.CatalogName,s.CatalogName,Ordinal)))
                {
                    throw new OperationFailedException(PublishSegmentedTransferJournalMismatch,SegmentedTransferFailureCode.JournalMismatch);
                }

                if(journal is null)
                {
                    await transfer.ValidateBeginPublishSegmentedTransfer(request,mutationScope,hc.RequestAborted);

                    Byte[]? preflightPayload = await store.ReadObjectPayloadAsync(request.SessionID,hc.RequestAborted);

                    if(preflightPayload is null || preflightPayload.Length == 0) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                    DataItem? preflightItem = DataItem.Deserialize(preflightPayload); Descriptor? preflightDescriptor = preflightItem?.GetDescriptor();

                    if(preflightItem is null || preflightDescriptor is null) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                    DataItemTransferManifest? preflightManifest = await store.LoadSegmentedManifestAsync(request.SessionID,hc.RequestAborted);

                    if(preflightManifest is null || preflightManifest.ItemID != request.ItemID || preflightManifest.ItemInstanceToken != manifest.ItemInstanceToken) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                    if(preflightManifest.ObjectInfo?.ContentStreamed is true && await store.CreateSegmentedBlockPlanAsync(request.SessionID,hc.RequestAborted) is null) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                    if(await Publisher.HasPublishConflictAsync(gf,s,preflightDescriptor,preflightManifest.ItemInstanceToken,new PublishArtifactExpectation()
                    {
                        ObjectLength = preflightPayload.LongLength,
                        ObjectSHA512 = preflightManifest.ObjectSHA512,
                        PublishStream = preflightManifest.ObjectInfo?.ContentStreamed is true,
                        StreamLength = preflightManifest.StreamLength,
                        StreamIntegrity = preflightManifest.Integrity,
                    },id!,tc,hc.RequestAborted))
                    {
                        Log.Error(PublishSegmentedTransferFailConflictID,id); SetErr(_); return Conflict(id);
                    }

                    journal = await store.GetOrCreateJournalAsync(new TransferPublicationJournal()
                    {
                        CatalogName = s.CatalogName,
                        Created = DateTimeOffset.UtcNow,
                        ExpectedStateVersion = request.ExpectedStateVersion,
                        Family = TransferPublicationFamily.Segmented,
                        ItemID = request.ItemID,
                        ItemInstanceToken = manifest.ItemInstanceToken,
                        OperationID = Guid.NewGuid(),
                        Phase = TransferPublicationPhase.Prepared,
                        SessionID = request.SessionID,
                        Updated = DateTimeOffset.UtcNow,
                    },hc.RequestAborted);

                    if(journal.ItemID != request.ItemID || journal.ItemInstanceToken != manifest.ItemInstanceToken || journal.Family is not TransferPublicationFamily.Segmented || !String.Equals(journal.CatalogName,s.CatalogName,Ordinal))
                    {
                        throw new OperationFailedException(PublishSegmentedTransferJournalMismatch,SegmentedTransferFailureCode.JournalMismatch);
                    }
                }

                if(journal.Phase is TransferPublicationPhase.Failed)
                {
                    Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return Conflict(id);
                }

                if(journal is null)
                {
                    Byte[]? retryPayload = await store.ReadObjectPayloadAsync(request.SessionID,hc.RequestAborted);

                    if(retryPayload is null || retryPayload.Length == 0) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                    DataItem? retryItem = DataItem.Deserialize(retryPayload); Descriptor? retryDescriptor = retryItem?.GetDescriptor();

                    DataItemTransferManifest? retryManifest = await store.LoadSegmentedManifestAsync(request.SessionID,hc.RequestAborted);

                    if(retryItem is null || retryDescriptor is null || retryManifest is null || retryManifest.ItemID != request.ItemID || retryManifest.ItemInstanceToken != manifest.ItemInstanceToken || itemState is not null && retryManifest.ItemInstanceToken != itemState.ItemInstanceToken) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                    if(await Publisher.HasPublishConflictAsync(gf,s,retryDescriptor,retryManifest.ItemInstanceToken,new PublishArtifactExpectation()
                    {
                        ObjectLength = retryPayload.LongLength,
                        ObjectSHA512 = retryManifest.ObjectSHA512,
                        PublishStream = retryManifest.ObjectInfo?.ContentStreamed is true,
                        StreamLength = retryManifest.StreamLength,
                        StreamIntegrity = retryManifest.Integrity,
                    },id!,tc,hc.RequestAborted))
                    {
                        Log.Error(PublishSegmentedTransferFailConflictID,id); SetErr(_); return Conflict(id);
                    }

                    journal = await store.GetOrCreateJournalAsync(new TransferPublicationJournal()
                    {
                        CatalogName = s.CatalogName,
                        Created = DateTimeOffset.UtcNow,
                        ExpectedStateVersion = request.ExpectedStateVersion,
                        Family = TransferPublicationFamily.Segmented,
                        ItemID = request.ItemID,
                        ItemInstanceToken = manifest.ItemInstanceToken,
                        OperationID = Guid.NewGuid(),
                        Phase = TransferPublicationPhase.Prepared,
                        SessionID = request.SessionID,
                        Updated = DateTimeOffset.UtcNow,
                    },hc.RequestAborted);
                }

                PublishSegmentedTransferResponse response = new();

                Boolean advancedFromPrepared = false;

                if(journal.Phase is TransferPublicationPhase.Prepared)
                {
                    response = await transfer.BeginPublishSegmentedTransfer(request with { ExpectedStateVersion = journal.ExpectedStateVersion },mutationScope,hc.RequestAborted);

                    TransferContentIntegrity? integrity = response.State.Integrity ?? await store.RecomputeIntegrityAsync(request.SessionID,hc.RequestAborted);

                    journal = await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.Publishing,cancel:hc.RequestAborted,integrity:integrity);

                    advancedFromPrepared = true;
                }

                Byte[]? objectPayload = await store.ReadObjectPayloadAsync(request.SessionID,hc.RequestAborted);

                if(objectPayload is null || objectPayload.Length == 0) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                DataItem? item = DataItem.Deserialize(objectPayload); Descriptor? descriptor = item?.GetDescriptor();

                if(item is null || descriptor is null) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                manifest = await store.LoadSegmentedManifestAsync(request.SessionID,hc.RequestAborted);

                if(manifest is null || manifest.ItemID != request.ItemID || manifest.ItemInstanceToken != journal.ItemInstanceToken) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                manifest = manifest with { Integrity = journal.Integrity };

                Boolean publishStream = manifest.ObjectInfo?.ContentStreamed is true;

                IReadOnlyList<TransferContentIntegritySegment>? integritySegments = publishStream
                    ? await store.LoadIntegritySegmentsAsync(request.SessionID,hc.RequestAborted)
                    : null;

                if(journal.Phase is TransferPublicationPhase.OperatorActionRequired)
                {
                    if(manifest.Status is not DataItemTransferStatus.Publishing)
                    {
                        Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError();
                    }

                    if(await Publisher.HasPublishConflictAsync(gf,s,descriptor,manifest.ItemInstanceToken,new PublishArtifactExpectation()
                    {
                        ObjectLength = objectPayload.LongLength,
                        ObjectSHA512 = manifest.ObjectSHA512,
                        PublishStream = publishStream,
                        StreamLength = manifest.StreamLength,
                        StreamIntegrity = publishStream ? manifest.Integrity : null,
                        StreamSegments = publishStream ? integritySegments : null,
                    },id!,tc,hc.RequestAborted))
                    {
                        Log.Error(PublishSegmentedTransferFailConflictID,id); SetErr(_); return Conflict(id);
                    }

                    journal = await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.Publishing,null,null,hc.RequestAborted);
                }
                else if(advancedFromPrepared)
                {
                    if(await Publisher.HasPublishConflictAsync(gf,s,descriptor,manifest.ItemInstanceToken,new PublishArtifactExpectation()
                    {
                        ObjectLength = objectPayload.LongLength,
                        ObjectSHA512 = manifest.ObjectSHA512,
                        PublishStream = publishStream,
                        StreamLength = manifest.StreamLength,
                        StreamIntegrity = publishStream ? manifest.Integrity : null,
                        StreamSegments = publishStream ? integritySegments : null,
                    },id!,tc,hc.RequestAborted))
                    {
                        await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.OperatorActionRequired,PublishSegmentedTransferOperatorActionRequired,null,hc.RequestAborted);

                        Log.Error(PublishSegmentedTransferFailConflictID,id); SetErr(_); return Conflict(id);
                    }
                }

                if(journal.Phase is TransferPublicationPhase.Publishing)
                {
                    TenantTransferBlobReference? stagedObject = await store.LoadObjectReferenceAsync(request.SessionID,hc.RequestAborted);

                    TransferBlockPlan? streamPlan = publishStream ? await store.CreateSegmentedBlockPlanAsync(request.SessionID,hc.RequestAborted) : null;

                    if(stagedObject is null || publishStream && streamPlan is null) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                    ITenantTransferStorage storage = hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Get(request.SessionID,request.ItemID,manifest.ItemInstanceToken);

                    Boolean published = await Publisher.PublishArtifactsAsync(storage,stagedObject,manifest.ObjectSHA512,streamPlan,publishStream,manifest.StreamLength,publishStream ? manifest.Integrity : null,publishStream ? integritySegments : null,hc.RequestAborted);

                    if(published is false) { Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError(); }

                    journal = await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.Published,cancel:hc.RequestAborted);
                }

                if(journal.Phase is TransferPublicationPhase.Published)
                {
                    if(await Publisher.UpdateCatalogAsync(gf,s,descriptor,manifest.ItemInstanceToken,tc,hc.RequestAborted) is false)
                    {
                        Log.Error(PublishSegmentedTransferFailID,id); SetErr(_); return InternalError();
                    }

                    journal = await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.CatalogUpdated,cancel:hc.RequestAborted);
                }

                if(journal.Phase is TransferPublicationPhase.CatalogUpdated)
                {
                    response = await transfer.FinishPublishSegmentedTransfer(new DataItemTransferIdentity()
                    {
                        SessionID = request.SessionID,
                        ItemID = request.ItemID,
                    },mutationScope,hc.RequestAborted);

                    response = response with { Published = true };

                    journal = await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.Finalized,cancel:hc.RequestAborted);
                }
                else if(journal.Phase is TransferPublicationPhase.Finalized)
                {
                    response = new() { State = manifest.ToState(), Published = true };
                }

                await store.DeleteJournalAsync(request.SessionID,s.CatalogName,hc.RequestAborted);

                await PublishNotificationAsync(notifications,CreateTerminalSegmentedNotification(DataControlNotificationEventType.TransferPublished,response.State),hc.RequestAborted);

                Log.Information(PublishSegmentedTransferSuccessID,id); SetOk(_); return Results.Ok(response);
            }
        }
        catch ( ArgumentException _ ) { Log.Error(_,PublishSegmentedTransferBadArgID,id); return BadRequest(PublishSegmentedTransferBadArg); }

        catch ( PublicationBlockLimitException error ) { Log.Warning(error,PublishSegmentedTransferFailID,id); return BlockLimitExceeded(error,request!.SessionID); }

        catch ( OperationFailedException _ ) { Log.Error(_,PublishSegmentedTransferFailID,id); return MapSegmentedFailure(_); }

        catch ( Exception _ ) { Log.Error(_,PublishSegmentedTransferFailID,id); return InternalError(); }
    }
}
