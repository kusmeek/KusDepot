namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapReOpenGetTransfer(WebApplication application)
    {
        application.MapPost("ReOpenGetTransfer",
                   ([FromBody] ReOpenGetTransferRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataItemSegmentedTransferService transfer,
                   HttpContext hc) => { return ReOpenGetTransfer(request,gf,transfer,hc); })
                   .Produces<ReOpenGetTransferResponse>(StatusCodes.Status200OK)
                   .WithName("ReOpenGetTransfer").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> ReOpenGetTransfer(ReOpenGetTransferRequest? request , IGrainFactory gf , IDataItemSegmentedTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(ReOpenGetTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(ReOpenGetTransferBadArgID,id); SetErr(_); return BadRequest(ReOpenGetTransferBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(ReOpenGetTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            ReOpenGetTransferResponse response = await transfer.ReOpenGetTransfer(request,hc.RequestAborted);

            Log.Information(ReOpenGetTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,ReOpenGetTransferBadArgID,id); return BadRequest(ReOpenGetTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,ReOpenGetTransferFailID,id); return MapSegmentedFailure(_); }

        catch ( Exception _ ) { Log.Error(_,ReOpenGetTransferFailID,id); return InternalError(); }
    }
}
