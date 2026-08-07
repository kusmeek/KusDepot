namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapCommitUploadTransfer(WebApplication application)
    {
        application.MapPost("CommitUploadTransfer",
                   ([FromBody] CommitUploadTransferRequest? request,
                   [FromServices] ISegmentedTransferStorage storage,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataItemSegmentedTransferService transfer,
                   [FromServices] IDataControlNotificationPublisher notifications,
                   HttpContext hc) => { return CommitUploadTransfer(request,storage,gf,transfer,notifications,hc); })
                   .Produces<CommitUploadTransferResponse>(StatusCodes.Status200OK)
                   .WithName("CommitUploadTransfer").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> CommitUploadTransfer(CommitUploadTransferRequest? request , ISegmentedTransferStorage storage , IGrainFactory gf , IDataItemSegmentedTransferService transfer , IDataControlNotificationPublisher notifications , HttpContext hc)
    {
        String? id = null;
        Boolean beganCommit = false;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(CommitUploadTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(CommitUploadTransferBadArgID,id); SetErr(_); return BadRequest(CommitUploadTransferBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(CommitUploadTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            CommitUploadTransferResponse response = await transfer.BeginCommitUploadTransfer(request,hc.RequestAborted);

            beganCommit = true;

            DataItemTransferManifest? manifest = await storage.LoadManifest(request.SessionID,hc.RequestAborted);

            if(manifest is null || manifest.ItemID != request.ItemID) { Log.Error(CommitUploadTransferFailID,id); SetErr(_); return InternalError(); }

            Byte[]? objectPayload = await storage.ReadObjectPayload(request.SessionID,hc.RequestAborted);

            if(objectPayload is null || objectPayload.Length == 0) { Log.Error(CommitUploadTransferFailID,id); SetErr(_); return InternalError(); }

            DataItem? item = DataItem.Deserialize(objectPayload);

            Descriptor? descriptor = item?.GetDescriptor();

            if(item is null || descriptor is null) { Log.Error(CommitUploadTransferFailID,id); SetErr(_); return InternalError(); }

            if(await Publisher.HasPublishConflict(gf,s,descriptor,id!,dt,ds,hc.RequestAborted))
            {
                await transfer.CancelCommitUploadTransfer(new DataItemTransferIdentity()
                {
                    SessionID = request.SessionID,
                    ItemID = request.ItemID,
                },hc.RequestAborted);

                beganCommit = false;

                Log.Error(CommitUploadTransferFailConflictID,id); SetErr(_); return Conflict(id);
            }

            Boolean published;

            if(manifest.StreamLength > 0)
            {
                using Stream? streamPayload = await storage.ReadRange(request.SessionID,new DataItemTransferRange(){ Offset = 0 , Length = manifest.StreamLength },hc.RequestAborted);

                if(streamPayload is null)
                {
                    await transfer.CancelCommitUploadTransfer(new DataItemTransferIdentity()
                    {
                        SessionID = request.SessionID,
                        ItemID = request.ItemID,
                    },hc.RequestAborted);

                    beganCommit = false;

                    Log.Error(CommitUploadTransferFailID,id); SetErr(_); return InternalError();
                }

                published = await Publisher.PublishAsync(gf,s,descriptor,objectPayload,manifest.ObjectSHA512,streamPayload,manifest.StreamSHA512,dt,ds,hc.RequestAborted);
            }
            else
            {
                published = await Publisher.PublishAsync(gf,s,descriptor,objectPayload,manifest.ObjectSHA512,null,manifest.StreamSHA512,dt,ds,hc.RequestAborted);
            }

            if(published is false)
            {
                await transfer.CancelCommitUploadTransfer(new DataItemTransferIdentity()
                {
                    SessionID = request.SessionID,
                    ItemID = request.ItemID,
                },hc.RequestAborted);

                beganCommit = false;

                Log.Error(CommitUploadTransferFailID,id); SetErr(_); return InternalError();
            }

            response = await transfer.CompleteCommitUploadTransfer(new DataItemTransferIdentity()
            {
                SessionID = request.SessionID,
                ItemID = request.ItemID,
            },hc.RequestAborted);

            response = response with { Published = true };

            await PublishNotificationAsync(notifications,CreateTerminalSegmentedNotification(DataControlNotificationEventType.TransferCommitted,response.State),hc.RequestAborted);

            Log.Information(CommitUploadTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,CommitUploadTransferBadArgID,id); return BadRequest(CommitUploadTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,CommitUploadTransferFailID,id); return MapSegmentedFailure(_); }

        catch ( Exception _ )
        {
            if(beganCommit && request is not null)
            {
                try
                {
                    await transfer.CancelCommitUploadTransfer(new DataItemTransferIdentity()
                    {
                        SessionID = request.SessionID,
                        ItemID = request.ItemID,
                    },hc.RequestAborted);

                    Log.Warning(_,CommitUploadTransferIssueID,id);
                }
                catch ( Exception cancelError )
                {
                    Log.Error(cancelError,CommitUploadTransferIssueID,id);
                }
            }

            Log.Error(_,CommitUploadTransferFailID,id); return InternalError();
        }
    }
}
