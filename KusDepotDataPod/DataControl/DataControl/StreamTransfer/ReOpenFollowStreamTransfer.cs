namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapReOpenFollowStreamTransfer(WebApplication application)
    {
        application.MapPost("ReOpenFollowStreamTransfer",
                   ([FromBody] ReOpenFollowStreamTransferRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataStreamTransferService transfer,
                   HttpContext hc) => { return ReOpenFollowStreamTransfer(request,gf,transfer,hc); })
                   .Produces<ReOpenFollowStreamTransferResponse>(StatusCodes.Status200OK)
                   .WithName("ReOpenFollowStreamTransfer").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> ReOpenFollowStreamTransfer(ReOpenFollowStreamTransferRequest? request , IGrainFactory gf , IDataStreamTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(ReOpenFollowStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(ReOpenFollowStreamTransferBadArgID,id); SetErr(_); return BadRequest(ReOpenFollowStreamTransferBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(ReOpenFollowStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            ReOpenFollowStreamTransferResponse response = await transfer.ReOpenFollowStreamTransfer(request,hc.RequestAborted);

            Log.Information(ReOpenFollowStreamTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,ReOpenFollowStreamTransferBadArgID,id); return BadRequest(ReOpenFollowStreamTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,ReOpenFollowStreamTransferFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,ReOpenFollowStreamTransferFailID,id); return InternalError(); }
    }
}
