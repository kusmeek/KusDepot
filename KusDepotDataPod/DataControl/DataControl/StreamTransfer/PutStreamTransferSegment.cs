namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapPutStreamTransferSegment(WebApplication application)
    {
        application.MapPut("PutStreamTransferSegment",
                   [DisableRequestSizeLimit]
                   ([FromServices] IGrainFactory gf,
                   [FromServices] IDataStreamTransferService transfer,
                   [FromServices] IDataControlNotificationPublisher notifications,
                   HttpContext hc) => { return PutStreamTransferSegment(gf,transfer,notifications,hc); })
                   .Produces<PutStreamTransferSegmentResponse>(StatusCodes.Status200OK)
                   .WithName("PutStreamTransferSegment").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> PutStreamTransferSegment(IGrainFactory gf , IDataStreamTransferService transfer , IDataControlNotificationPublisher notifications , HttpContext hc)
    {
        PutStreamTransferSegmentRequest? request = null;
        String? id = null;
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(PutStreamTransferSegmentUnAuthID,null); SetErr(_); return Unauthorized(); }

            TransferEnvelopeHeader? header = await TransferEnvelope.ReadHeaderAsync(hc.Request.Body,hc.RequestAborted);

            if(!header.HasValue) { Log.Error(PutStreamTransferSegmentBadArg); SetErr(_); return BadRequest(PutStreamTransferSegmentBadArg); }

            ReadOnlyMemory<Byte> metadataBytes = await TransferEnvelope.ReadMetadataBytesAsync(hc.Request.Body,header.Value,hc.RequestAborted);

            request = metadataBytes.IsEmpty && header.Value.MetadataLength > 0 ? null : JsonUtility.Deserialize<PutStreamTransferSegmentRequest>(metadataBytes);

            if(request is null) { Log.Error(PutStreamTransferSegmentBadArg); SetErr(_); return BadRequest(PutStreamTransferSegmentBadArg); }

            id = request.ItemID.ToString(); _?.AddTag("id",request.ItemID);

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(PutStreamTransferSegmentUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(header.Value.PayloadLength != request.Length) { Log.Error(PutStreamTransferSegmentBadArgID,id); SetErr(_); return BadRequest(PutStreamTransferSegmentBadArg); }

            using Stream payload = TransferEnvelope.OpenPayloadStream(hc.Request.Body,header.Value);

            PutStreamTransferSegmentResponse response = await transfer.PutStreamTransferSegment(request,payload,hc.RequestAborted);

            if(response.Accepted is false)
            {
                Log.Error(PutStreamTransferSegmentFailConflictID,id); SetErr(_); return Conflict(id);
            }

            await PublishNotificationAsync(notifications,CreateStreamAppendedNotification(request,response),hc.RequestAborted);

            Log.Information(PutStreamTransferSegmentSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,PutStreamTransferSegmentBadArgID,id); return BadRequest(PutStreamTransferSegmentBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,PutStreamTransferSegmentFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,PutStreamTransferSegmentFailID,id); return InternalError(); }
    }
}
