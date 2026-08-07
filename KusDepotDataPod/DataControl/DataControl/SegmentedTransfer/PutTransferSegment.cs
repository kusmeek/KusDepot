namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapPutTransferSegment(WebApplication application)
    {
        application.MapPut("PutTransferSegment",
                   [DisableRequestSizeLimit]
                   ([FromServices] IGrainFactory gf,
                   [FromServices] IDataItemSegmentedTransferService transfer,
                   [FromServices] IDataControlNotificationPublisher notifications,
                   HttpContext hc) => { return PutTransferSegment(gf,transfer,notifications,hc); })            
                   .Produces<PutTransferSegmentResponse>(StatusCodes.Status200OK)
                   .WithName("PutTransferSegment").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> PutTransferSegment(IGrainFactory gf , IDataItemSegmentedTransferService transfer , IDataControlNotificationPublisher notifications , HttpContext hc)
    {
        PutTransferSegmentRequest? request = null;

        String? id = null;
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(PutTransferSegmentUnAuthID,null); SetErr(_); return Unauthorized(); }

            TransferEnvelopeHeader? header = await TransferEnvelope.ReadHeaderAsync(hc.Request.Body,hc.RequestAborted);

            if(!header.HasValue) { Log.Error(PutTransferSegmentBadArg); SetErr(_); return BadRequest(PutTransferSegmentBadArg); }

            ReadOnlyMemory<Byte> metadataBytes = await TransferEnvelope.ReadMetadataBytesAsync(hc.Request.Body,header.Value,hc.RequestAborted);

            request = metadataBytes.IsEmpty && header.Value.MetadataLength > 0 ? null : JsonUtility.Deserialize<PutTransferSegmentRequest>(metadataBytes);

            if(request is null) { Log.Error(PutTransferSegmentBadArg); SetErr(_); return BadRequest(PutTransferSegmentBadArg); }

            id = request.ItemID.ToString(); _?.AddTag("id",request.ItemID);

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(PutTransferSegmentUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(header.Value.PayloadLength != request.Length) { Log.Error(PutTransferSegmentBadArgID,id); SetErr(_); return BadRequest(PutTransferSegmentBadArg); }

            using Stream payload = TransferEnvelope.OpenPayloadStream(hc.Request.Body,header.Value);

            PutTransferSegmentResponse response = await transfer.PutTransferSegment(request,payload,hc.RequestAborted);

            if(response.Accepted is false)
            {
                Log.Error(PutTransferSegmentFailConflictID,id); SetErr(_); return Conflict(id);
            }

            await PublishNotificationAsync(notifications,CreateSegmentRealizedNotification(request,response),hc.RequestAborted);

            Log.Information(PutTransferSegmentSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,PutTransferSegmentBadArgID,id); return BadRequest(PutTransferSegmentBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,PutTransferSegmentFailID,id); return MapSegmentedFailure(_); }

        catch ( Exception _ ) { Log.Error(_,PutTransferSegmentFailID,id); return InternalError(); }
    }
}
