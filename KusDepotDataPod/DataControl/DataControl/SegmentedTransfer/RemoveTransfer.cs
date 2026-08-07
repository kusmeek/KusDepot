namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapRemoveTransfer(WebApplication application)
    {
        application.MapPost("RemoveTransfer",
                   ([FromBody] RemoveTransferRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataItemSegmentedTransferService segmentedTransfer,
                   [FromServices] IDataStreamTransferService streamTransfer,
                   HttpContext hc) => { return RemoveTransfer(request,gf,segmentedTransfer,streamTransfer,hc); })
                   .Produces<RemoveTransferResponse>(StatusCodes.Status200OK)
                   .WithName("RemoveTransfer").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> RemoveTransfer(RemoveTransferRequest? request , IGrainFactory gf , IDataItemSegmentedTransferService segmentedTransfer , IDataStreamTransferService streamTransfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(RemoveTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(RemoveTransferBadArgID,id); SetErr(_); return BadRequest(RemoveTransferBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(RemoveTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            DataItemTransferIdentity identity = new() { SessionID = request.SessionID , ItemID = request.ItemID };

            Boolean segmentedExists = await segmentedTransfer.SessionExists(identity,hc.RequestAborted);

            Boolean streamExists = await streamTransfer.SessionExists(identity,hc.RequestAborted);

            if(segmentedExists is false && streamExists is false)
            {
                throw new OperationFailedException($"SessionNotFound SessionID: {request.SessionID} ItemID: {request.ItemID}",SegmentedTransferFailureCode.SessionNotFound);
            }

            RemoveTransferResponse? response = null;

            if(segmentedExists)
            {
                response = await segmentedTransfer.RemoveTransfer(request,hc.RequestAborted);
            }

            if(streamExists)
            {
                response = await streamTransfer.RemoveTransfer(request,hc.RequestAborted);
            }

            if(response?.Removed is not true) { Log.Error(RemoveTransferFailID,id); SetErr(_); return InternalError(); }

            Log.Information(RemoveTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,RemoveTransferBadArgID,id); return BadRequest(RemoveTransferBadArg); }

        catch ( OperationFailedException _ )
        {
            Log.Error(_,RemoveTransferFailID,id);

            return _.FailureCode is StreamTransferFailureCode ? MapStreamTransferFailure(_) : MapSegmentedFailure(_);
        }

        catch ( Exception _ ) { Log.Error(_,RemoveTransferFailID,id); return InternalError(); }
    }
}
