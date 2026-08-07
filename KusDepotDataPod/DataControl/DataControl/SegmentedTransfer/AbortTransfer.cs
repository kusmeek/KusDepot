namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapAbortTransfer(WebApplication application)
    {
        application.MapPost("AbortTransfer",
                   ([FromBody] AbortTransferRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataItemSegmentedTransferService segmentedTransfer,
                   [FromServices] IDataStreamTransferService streamTransfer,
                   [FromServices] IDataControlNotificationPublisher notifications,
                   HttpContext hc) => {return AbortTransfer(request,gf,segmentedTransfer,streamTransfer,notifications,hc);})
                   .Produces<AbortTransferResponse>(StatusCodes.Status200OK)
                   .WithName("AbortTransfer").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> AbortTransfer(AbortTransferRequest? request , IGrainFactory gf , IDataItemSegmentedTransferService segmentedTransfer , IDataStreamTransferService streamTransfer , IDataControlNotificationPublisher notifications , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(AbortTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(AbortTransferBadArgID,id); SetErr(_); return BadRequest(AbortTransferBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(AbortTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            DataItemTransferIdentity identity = new() { SessionID = request.SessionID , ItemID = request.ItemID };

            Boolean segmentedExists = await segmentedTransfer.SessionExists(identity,hc.RequestAborted);

            Boolean streamExists = await streamTransfer.SessionExists(identity,hc.RequestAborted);

            if(segmentedExists is false && streamExists is false)
            {
                throw new OperationFailedException($"SessionNotFound SessionID: {request.SessionID} ItemID: {request.ItemID}",SegmentedTransferFailureCode.SessionNotFound);
            }

            AbortTransferResponse? response = null;

            if(segmentedExists)
            {
                response = await segmentedTransfer.AbortTransfer(request,hc.RequestAborted);
            }

            if(streamExists)
            {
                response = await streamTransfer.AbortTransfer(request,hc.RequestAborted);
            }

            if(response?.Aborted is not true) { Log.Error(AbortTransferFailID,id); SetErr(_); return InternalError(); }

            await PublishNotificationAsync(notifications,CreateTransferAbortedNotification(response),hc.RequestAborted);

            Log.Information(AbortTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,AbortTransferBadArgID,id); return BadRequest(AbortTransferBadArg); }

        catch ( OperationFailedException _ )
        {
            Log.Error(_,AbortTransferFailID,id);

            return _.FailureCode is StreamTransferFailureCode ? MapStreamTransferFailure(_) : MapSegmentedFailure(_);
        }

        catch ( Exception _ ) { Log.Error(_,AbortTransferFailID,id); return InternalError(); }
    }
}
