namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapReOpenUploadTransfer(WebApplication application)
    {
        application.MapPost("ReOpenUploadTransfer",
                   ([FromBody] ReOpenUploadTransferRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataItemSegmentedTransferService transfer,
                   HttpContext hc) => { return ReOpenUploadTransfer(request,gf,transfer,hc); })
                   .Produces<ReOpenUploadTransferResponse>(StatusCodes.Status200OK)
                   .WithName("ReOpenUploadTransfer").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> ReOpenUploadTransfer(ReOpenUploadTransferRequest? request , IGrainFactory gf , IDataItemSegmentedTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(ReOpenUploadTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(ReOpenUploadTransferBadArgID,id); SetErr(_); return BadRequest(ReOpenUploadTransferBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(ReOpenUploadTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            ReOpenUploadTransferResponse response = await transfer.ReOpenUploadTransfer(request,hc.RequestAborted);

            Log.Information(ReOpenUploadTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,ReOpenUploadTransferBadArgID,id); return BadRequest(ReOpenUploadTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,ReOpenUploadTransferFailID,id); return MapSegmentedFailure(_); }

        catch ( Exception _ ) { Log.Error(_,ReOpenUploadTransferFailID,id); return InternalError(); }
    }
}
