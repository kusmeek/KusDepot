namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapCompleteStreamTransfer(WebApplication application)
    {
        application.MapPost("CompleteStreamTransfer",
                   ([FromBody] CompleteStreamTransferRequest? request,
                   [FromServices] IStreamTransferStorage storage,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataStreamTransferService transfer,
                   [FromServices] IDataControlNotificationPublisher notifications,
                   HttpContext hc) => { return CompleteStreamTransfer(request,storage,gf,transfer,notifications,hc); })
                   .Produces<CompleteStreamTransferResponse>(StatusCodes.Status200OK)
                   .WithName("CompleteStreamTransfer").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> CompleteStreamTransfer(CompleteStreamTransferRequest? request , IStreamTransferStorage storage , IGrainFactory gf , IDataStreamTransferService transfer , IDataControlNotificationPublisher notifications , HttpContext hc)
    {
        String? id = null;
        Boolean beganComplete = false;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(CompleteStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(CompleteStreamTransferBadArgID,id); SetErr(_); return BadRequest(CompleteStreamTransferBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(CompleteStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            CompleteStreamTransferResponse response = await transfer.BeginCompleteStreamTransfer(request,hc.RequestAborted);

            beganComplete = true;

            DataStreamTransferManifest? manifest = await storage.LoadManifest(request.SessionID,hc.RequestAborted);

            if(manifest is null || manifest.ItemID != request.ItemID) { Log.Error(CompleteStreamTransferFailID,id); SetErr(_); return InternalError(); }

            await using Stream? objectPayloadStream = await storage.ReadObjectPayload(request.SessionID,hc.RequestAborted);

            if(objectPayloadStream is null) { Log.Error(CompleteStreamTransferFailID,id); SetErr(_); return InternalError(); }

            using MemoryStream objectPayloadBuffer = new();

            await objectPayloadStream.CopyToAsync(objectPayloadBuffer,hc.RequestAborted);

            Byte[] objectPayload = objectPayloadBuffer.ToArray();

            if(objectPayload.Length == 0) { Log.Error(CompleteStreamTransferFailID,id); SetErr(_); return InternalError(); }

            DataItem? item = DataItem.Deserialize(objectPayload);

            Descriptor? descriptor = item?.GetDescriptor();

            if(item is null || descriptor is null) { Log.Error(CompleteStreamTransferFailID,id); SetErr(_); return InternalError(); }

            if(await Publisher.HasPublishConflict(gf,s,descriptor,id!,dt,ds,hc.RequestAborted))
            {
                await transfer.CancelCompleteStreamTransfer(new DataItemTransferIdentity()
                {
                    SessionID = request.SessionID,
                    ItemID = request.ItemID,
                },hc.RequestAborted);

                beganComplete = false;

                Log.Error(CompleteStreamTransferFailID,id); SetErr(_); return Conflict(id);
            }

            Boolean published;

            if(manifest.AppendedLength > 0)
            {
                await using Stream? streamPayload = await storage.ReadRange(request.SessionID,new DataItemTransferRange(){ Offset = 0 , Length = manifest.AppendedLength },hc.RequestAborted);

                if(streamPayload is null)
                {
                    await transfer.CancelCompleteStreamTransfer(new DataItemTransferIdentity()
                    {
                        SessionID = request.SessionID,
                        ItemID = request.ItemID,
                    },hc.RequestAborted);

                    beganComplete = false;

                    Log.Error(CompleteStreamTransferFailID,id); SetErr(_); return InternalError();
                }

                published = await Publisher.PublishAsync(gf,s,descriptor,objectPayload,manifest.ObjectSHA512,streamPayload,manifest.StreamSHA512,dt,ds,hc.RequestAborted);
            }
            else
            {
                published = await Publisher.PublishAsync(gf,s,descriptor,objectPayload,manifest.ObjectSHA512,null,manifest.StreamSHA512,dt,ds,hc.RequestAborted);
            }

            if(published is false)
            {
                await transfer.CancelCompleteStreamTransfer(new DataItemTransferIdentity()
                {
                    SessionID = request.SessionID,
                    ItemID = request.ItemID,
                },hc.RequestAborted);

                beganComplete = false;

                Log.Error(CompleteStreamTransferFailID,id); SetErr(_); return InternalError();
            }

            response = await transfer.FinishCompleteStreamTransfer(new DataItemTransferIdentity()
            {
                SessionID = request.SessionID,
                ItemID = request.ItemID,
            },hc.RequestAborted);

            response = response with { Published = true };

            await PublishNotificationAsync(notifications,CreateTerminalStreamNotification(DataControlNotificationEventType.TransferCompleted,response.State),hc.RequestAborted);

            Log.Information(CompleteStreamTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,CompleteStreamTransferBadArgID,id); return BadRequest(CompleteStreamTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,CompleteStreamTransferFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ )
        {
            if(beganComplete && request is not null)
            {
                try
                {
                    await transfer.CancelCompleteStreamTransfer(new DataItemTransferIdentity()
                    {
                        SessionID = request.SessionID,
                        ItemID = request.ItemID,
                    },hc.RequestAborted);

                    Log.Warning(_,CompleteStreamTransferFailID,id);
                }
                catch ( Exception cancelError )
                {
                    Log.Error(cancelError,CompleteStreamTransferFailID,id);
                }
            }

            Log.Error(_,CompleteStreamTransferFailID,id); return InternalError();
        }
    }
}
