namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapOpenGetTransfer(WebApplication application)
    {
        application.MapPost("OpenGetTransfer",
                   ([FromBody] OpenGetTransferRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataItemSegmentedTransferService transfer,
                   HttpContext hc) => { return OpenGetTransfer(request,gf,transfer,hc); })            
                   .Produces<OpenGetTransferResponse>(StatusCodes.Status200OK)
                   .WithName("OpenGetTransfer").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> OpenGetTransfer(OpenGetTransferRequest? request , IGrainFactory gf , IDataItemSegmentedTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(OpenGetTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(OpenGetTransferBadArgID,id); SetErr(_); return BadRequest(OpenGetTransferBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(OpenGetTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            OpenGetTransferResponse response = await transfer.OpenGetTransfer(request,hc.RequestAborted);

            Log.Information(OpenGetTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,OpenGetTransferBadArgID,id); return BadRequest(OpenGetTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,OpenGetTransferFailID,id); return MapSegmentedFailure(_); }

        catch ( Exception _ ) { Log.Error(_,OpenGetTransferFailID,id); return InternalError(); }
    }
}
