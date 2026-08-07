namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapOpenStreamTransfer(WebApplication application)
    {
        application.MapPost("OpenStreamTransfer",
                   ([FromBody] OpenStreamTransferRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataStreamTransferService transfer,
                   HttpContext hc) => { return OpenStreamTransfer(request,gf,transfer,hc); })
                   .Produces<OpenStreamTransferResponse>(StatusCodes.Status200OK)
                   .WithName("OpenStreamTransfer").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> OpenStreamTransfer(OpenStreamTransferRequest? request , IGrainFactory gf , IDataStreamTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(OpenStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(OpenStreamTransferBadArgID,id); SetErr(_); return BadRequest(OpenStreamTransferBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(OpenStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            OpenStreamTransferResponse response = await transfer.OpenStreamTransfer(request,hc.RequestAborted);

            Log.Information(OpenStreamTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,OpenStreamTransferBadArgID,id); return BadRequest(OpenStreamTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,OpenStreamTransferFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,OpenStreamTransferFailID,id); return InternalError(); }
    }
}
