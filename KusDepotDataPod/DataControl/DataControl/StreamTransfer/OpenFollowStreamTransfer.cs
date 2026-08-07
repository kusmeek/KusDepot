namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapOpenFollowStreamTransfer(WebApplication application)
    {
        application.MapPost("OpenFollowStreamTransfer",
                   ([FromBody] OpenFollowStreamTransferRequest? request,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataStreamTransferService transfer,
                   HttpContext hc) => { return OpenFollowStreamTransfer(request,gf,transfer,hc); })
                   .Produces<OpenFollowStreamTransferResponse>(StatusCodes.Status200OK)
                   .WithName("OpenFollowStreamTransfer").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> OpenFollowStreamTransfer(OpenFollowStreamTransferRequest? request , IGrainFactory gf , IDataStreamTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(OpenFollowStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(OpenFollowStreamTransferBadArgID,id); SetErr(_); return BadRequest(OpenFollowStreamTransferBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(OpenFollowStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            OpenFollowStreamTransferResponse response = await transfer.OpenFollowStreamTransfer(request,hc.RequestAborted);

            Log.Information(OpenFollowStreamTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,OpenFollowStreamTransferBadArgID,id); return BadRequest(OpenFollowStreamTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,OpenFollowStreamTransferFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,OpenFollowStreamTransferFailID,id); return InternalError(); }
    }
}
