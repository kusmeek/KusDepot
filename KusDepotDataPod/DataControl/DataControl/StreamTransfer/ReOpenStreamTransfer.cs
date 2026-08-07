namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapReOpenStreamTransfer(WebApplication application)
    {
        application.MapPost("ReOpenStreamTransfer",
                   ([FromBody] ReOpenStreamTransferRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataStreamTransferService transfer,
                   HttpContext hc) => { return ReOpenStreamTransfer(request,gf,transfer,hc); })
                   .Produces<ReOpenStreamTransferResponse>(StatusCodes.Status200OK)
                   .WithName("ReOpenStreamTransfer").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> ReOpenStreamTransfer(ReOpenStreamTransferRequest? request , IGrainFactory gf , IDataStreamTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(ReOpenStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(ReOpenStreamTransferBadArgID,id); SetErr(_); return BadRequest(ReOpenStreamTransferBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(ReOpenStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            ReOpenStreamTransferResponse response = await transfer.ReOpenStreamTransfer(request,hc.RequestAborted);

            Log.Information(ReOpenStreamTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,ReOpenStreamTransferBadArgID,id); return BadRequest(ReOpenStreamTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,ReOpenStreamTransferFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,ReOpenStreamTransferFailID,id); return InternalError(); }
    }
}
