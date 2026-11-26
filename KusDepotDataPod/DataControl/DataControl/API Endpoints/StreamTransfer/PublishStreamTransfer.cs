namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapPublishStreamTransfer(WebApplication application)
    {
        application.MapPost("PublishStreamTransfer",
                   ([FromBody] PublishStreamTransferRequest? request,
                   [FromServices] IDataControlPublicationTransferStore store,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataStreamTransferService transfer,
                   [FromServices] IDataItemTransferCoordinator coordinator,
                   [FromServices] IDataControlNotificationPublisher notifications,
                   HttpContext hc) => { return PublishStreamTransfer(request,store,gf,transfer,coordinator,notifications,hc); })
                   .Produces<PublishStreamTransferResponse>(StatusCodes.Status200OK)
                   .WithName("PublishStreamTransfer");
    }

    private async Task<IResult> PublishStreamTransfer(PublishStreamTransferRequest? request , IDataControlPublicationTransferStore store , IGrainFactory gf , IDataStreamTransferService transfer , IDataItemTransferCoordinator coordinator , IDataControlNotificationPublisher notifications , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tc = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(PublishStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(PublishStreamTransferBadArgID,id); SetErr(_); return BadRequest(PublishStreamTransferBadArg); }

            var dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>(); StorageSilo? s = await dc.ResolveStorageAsync(t,CatalogAccess.Write,tc,hc.RequestAborted);

            if(s is null) { Log.Error(PublishStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(s);

            ITransferMutationScope mutationScope = await coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,hc.RequestAborted);

            await using (mutationScope)
            {
                DataStreamTransferManifest? manifest = await store.LoadStreamManifestAsync(request.SessionID,hc.RequestAborted);

                if(manifest is null || manifest.ItemID != request.ItemID || manifest.ItemInstanceToken == Guid.Empty) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                CatalogItemState? itemState = await gf.GetGrain<ICatalogDB>(s.CatalogName).GetItemState(request.ItemID,tc,hc.RequestAborted);

                if(itemState is not null && (itemState.IsPendingDelete || itemState.ItemInstanceToken != manifest.ItemInstanceToken))
                {
                    throw new OperationFailedException(PublishStreamTransferJournalMismatch,StreamTransferFailureCode.JournalMismatch);
                }

                TransferPublicationJournal? journal = await store.LoadJournalAsync(request.SessionID,s.CatalogName,hc.RequestAborted);

                if(journal is not null && (journal.ItemID != request.ItemID || journal.ItemInstanceToken != manifest.ItemInstanceToken || journal.Family is not TransferPublicationFamily.Stream || !String.Equals(journal.CatalogName,s.CatalogName,Ordinal)))
                {
                    throw new OperationFailedException(PublishStreamTransferJournalMismatch,StreamTransferFailureCode.JournalMismatch);
                }

                if(journal is null)
                {
                    await transfer.ValidateBeginPublishStreamTransfer(request,mutationScope,hc.RequestAborted);

                    if(await store.CreateStreamBlockPlanAsync(request.SessionID,hc.RequestAborted) is null) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                    Byte[]? preflightPayload = await store.ReadObjectPayloadAsync(request.SessionID,hc.RequestAborted);

                    if(preflightPayload is null || preflightPayload.Length == 0) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                    DataItem? preflightItem = DataItem.Deserialize(preflightPayload); Descriptor? preflightDescriptor = preflightItem?.GetDescriptor();

                    if(preflightItem is null || preflightDescriptor is null) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                    if(await Publisher.HasPublishConflictAsync(gf,s,preflightDescriptor,manifest.ItemInstanceToken,new PublishArtifactExpectation()
                    {
                        ObjectLength = preflightPayload.LongLength,
                        ObjectSHA512 = SHA512.HashData(preflightPayload),
                        PublishStream = true,
                        StreamLength = request.FinalStreamLength ?? 0,
                        StreamIntegrity = null,
                    },id!,tc,hc.RequestAborted))
                    {
                        Log.Error(PublishStreamTransferFailID,id); SetErr(_); return Conflict(id);
                    }

                    journal = await store.GetOrCreateJournalAsync(new TransferPublicationJournal()
                    {
                        CatalogName = s.CatalogName,
                        Created = DateTimeOffset.UtcNow,
                        ExpectedStateVersion = request.ExpectedStateVersion,
                        Family = TransferPublicationFamily.Stream,
                        FinalObjectInfo = request.FinalObjectInfo,
                        FinalObjectPayload = request.FinalObjectPayload,
                        FinalStreamLength = request.FinalStreamLength,
                        Integrity = null,
                        ItemID = request.ItemID,
                        ItemInstanceToken = manifest.ItemInstanceToken,
                        OperationID = Guid.NewGuid(),
                        Phase = TransferPublicationPhase.Prepared,
                        SessionID = request.SessionID,
                        Updated = DateTimeOffset.UtcNow,
                    },hc.RequestAborted);

                    if(journal.ItemID != request.ItemID || journal.ItemInstanceToken != manifest.ItemInstanceToken || journal.Family is not TransferPublicationFamily.Stream || !String.Equals(journal.CatalogName,s.CatalogName,Ordinal))
                    {
                        throw new OperationFailedException(PublishStreamTransferJournalMismatch,StreamTransferFailureCode.JournalMismatch);
                    }
                }

                if(journal.Phase is TransferPublicationPhase.Failed)
                {
                    Log.Error(PublishStreamTransferFailID,id); SetErr(_); return Conflict(id);
                }

                if(journal is null)
                {
                    Byte[]? retryPayload = await store.ReadObjectPayloadAsync(request.SessionID,hc.RequestAborted);

                    if(retryPayload is null || retryPayload.Length == 0) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                    DataItem? retryItem = DataItem.Deserialize(retryPayload); Descriptor? retryDescriptor = retryItem?.GetDescriptor();

                    if(retryItem is null || retryDescriptor is null) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                    if(await Publisher.HasPublishConflictAsync(gf,s,retryDescriptor,manifest.ItemInstanceToken,new PublishArtifactExpectation()
                    {
                        ObjectLength = retryPayload.LongLength,
                        ObjectSHA512 = SHA512.HashData(retryPayload),
                        PublishStream = true,
                        StreamLength = request.FinalStreamLength ?? 0,
                        StreamIntegrity = null,
                    },id!,tc,hc.RequestAborted))
                    {
                        Log.Error(PublishStreamTransferFailID,id); SetErr(_); return Conflict(id);
                    }

                    journal = await store.GetOrCreateJournalAsync(new TransferPublicationJournal()
                    {
                        CatalogName = s.CatalogName,
                        Created = DateTimeOffset.UtcNow,
                        ExpectedStateVersion = request.ExpectedStateVersion,
                        Family = TransferPublicationFamily.Stream,
                        FinalObjectInfo = request.FinalObjectInfo,
                        FinalObjectPayload = request.FinalObjectPayload,
                        FinalStreamLength = request.FinalStreamLength,
                        Integrity = null,
                        ItemID = request.ItemID,
                        ItemInstanceToken = manifest.ItemInstanceToken,
                        OperationID = Guid.NewGuid(),
                        Phase = TransferPublicationPhase.Prepared,
                        SessionID = request.SessionID,
                        Updated = DateTimeOffset.UtcNow,
                    },hc.RequestAborted);
                }

                PublishStreamTransferResponse response = new();
                Byte[] objectPayload; DataItem? item; Descriptor? descriptor;
                Boolean advancedFromPrepared = false;

                if(journal.Phase is TransferPublicationPhase.Prepared or TransferPublicationPhase.Publishing)
                {
                    response = await transfer.BeginPublishStreamTransfer(request with
                    {
                        ExpectedStateVersion = journal.ExpectedStateVersion,
                        FinalObjectInfo = journal.FinalObjectInfo,
                        FinalObjectPayload = journal.FinalObjectPayload,
                        FinalStreamLength = journal.FinalStreamLength,
                    },mutationScope,hc.RequestAborted);

                    if(journal.Phase is TransferPublicationPhase.Prepared)
                    {
                        TransferContentIntegrity? integrity = response.State.Integrity ?? await store.RecomputeIntegrityAsync(request.SessionID,hc.RequestAborted);

                        journal = await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.Publishing,cancel:hc.RequestAborted,integrity:integrity);

                        advancedFromPrepared = true;
                    }
                }

                manifest = await store.LoadStreamManifestAsync(request.SessionID,hc.RequestAborted);

                if(manifest is null || manifest.ItemID != request.ItemID || manifest.ItemInstanceToken != journal.ItemInstanceToken) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                manifest = manifest with { Integrity = journal.Integrity };

                IReadOnlyList<TransferContentIntegritySegment>? integritySegments = await store.LoadIntegritySegmentsAsync(request.SessionID,hc.RequestAborted);

                objectPayload = await store.ReadObjectPayloadAsync(request.SessionID,hc.RequestAborted) ?? Array.Empty<Byte>();

                if(objectPayload.Length == 0) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                item = DataItem.Deserialize(objectPayload); descriptor = item?.GetDescriptor();

                if(item is null || descriptor is null) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                if(journal.Phase is TransferPublicationPhase.OperatorActionRequired)
                {
                    if(manifest.Status is not DataItemTransferStatus.Publishing)
                    {
                        Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError();
                    }

                    if(await Publisher.HasPublishConflictAsync(gf,s,descriptor,manifest.ItemInstanceToken,new PublishArtifactExpectation()
                    {
                        ObjectLength = objectPayload.LongLength,
                        ObjectSHA512 = manifest.ObjectSHA512,
                        PublishStream = true,
                        StreamLength = manifest.AppendedLength,
                        StreamIntegrity = manifest.Integrity,
                        StreamSegments = integritySegments,
                    },id!,tc,hc.RequestAborted))
                    {
                        Log.Error(PublishStreamTransferFailID,id); SetErr(_); return Conflict(id);
                    }

                    await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.Publishing,null,null,hc.RequestAborted);
                }
                else if((advancedFromPrepared || journal.Phase is TransferPublicationPhase.Publishing) && await Publisher.HasPublishConflictAsync(gf,s,descriptor,manifest.ItemInstanceToken,new PublishArtifactExpectation()
                {
                    ObjectLength = objectPayload.LongLength,
                    ObjectSHA512 = manifest.ObjectSHA512,
                    PublishStream = true,
                    StreamLength = manifest.AppendedLength,
                    StreamIntegrity = manifest.Integrity,
                    StreamSegments = integritySegments,
                },id!,tc,hc.RequestAborted))
                {
                    await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.OperatorActionRequired,PublishStreamTransferOperatorActionRequired,null,hc.RequestAborted);

                    Log.Error(PublishStreamTransferFailID,id); SetErr(_); return Conflict(id);
                }

                if(journal.Phase is TransferPublicationPhase.Publishing)
                {
                    TenantTransferBlobReference? stagedObject = await store.LoadObjectReferenceAsync(request.SessionID,hc.RequestAborted);

                    TransferBlockPlan? streamPlan = await store.CreateStreamBlockPlanAsync(request.SessionID,hc.RequestAborted);

                    if(stagedObject is null || streamPlan is null) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                    ITenantTransferStorage storage = hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Get(request.SessionID,request.ItemID,manifest.ItemInstanceToken);

                    Boolean published = await Publisher.PublishArtifactsAsync(storage,stagedObject,manifest.ObjectSHA512,streamPlan,true,manifest.AppendedLength,manifest.Integrity,integritySegments,hc.RequestAborted);

                    if(published is false) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                    journal = await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.Published,cancel:hc.RequestAborted);
                }

                if(journal.Phase is TransferPublicationPhase.Published)
                {
                    if(await Publisher.UpdateCatalogAsync(gf,s,descriptor,manifest.ItemInstanceToken,tc,hc.RequestAborted) is false)
                    {
                        Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError();
                    }

                    journal = await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.CatalogUpdated,cancel:hc.RequestAborted);
                }

                if(journal.Phase is TransferPublicationPhase.CatalogUpdated)
                {
                    response = await transfer.FinishPublishStreamTransfer(new DataItemTransferIdentity()
                    {
                        SessionID = request.SessionID,
                        ItemID = request.ItemID,
                    },mutationScope,hc.RequestAborted);

                    response = response with { Published = true };

                    journal = await store.AdvanceJournalAsync(request.SessionID,journal.OperationID,s.CatalogName,TransferPublicationPhase.Finalized,cancel:hc.RequestAborted);
                }
                else if(journal.Phase is TransferPublicationPhase.Finalized)
                {
                    DataStreamTransferManifest? finalized = await store.LoadStreamManifestAsync(request.SessionID,hc.RequestAborted);

                    if(finalized is null || finalized.ItemID != request.ItemID) { Log.Error(PublishStreamTransferFailID,id); SetErr(_); return InternalError(); }

                    response = new() { State = finalized.ToState(), Published = true };
                }

                await store.DeleteJournalAsync(request.SessionID,s.CatalogName,hc.RequestAborted);

                await PublishNotificationAsync(notifications,CreateTerminalStreamNotification(DataControlNotificationEventType.TransferCompleted,response.State),hc.RequestAborted);

                Log.Information(PublishStreamTransferSuccessID,id); SetOk(_); return Results.Ok(response);
            }
        }
        catch ( ArgumentException _ ) { Log.Error(_,PublishStreamTransferBadArgID,id); return BadRequest(PublishStreamTransferBadArg); }

        catch ( PublicationBlockLimitException error ) { Log.Warning(error,PublishStreamTransferFailID,id); return BlockLimitExceeded(error,request!.SessionID); }

        catch ( OperationFailedException _ ) { Log.Error(_,PublishStreamTransferFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,PublishStreamTransferFailID,id); return InternalError(); }
    }

}
