namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapGetStreamTransferState(WebApplication application)
    {
        application.MapPost("GetStreamTransferState",
                   ([FromBody] DataItemTransferIdentity? identity,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataStreamTransferService transfer,
                   HttpContext hc) => { return GetStreamTransferState(identity,gf,transfer,hc); })
                   .Produces<GetStreamTransferStateResponse>(StatusCodes.Status200OK)
                   .WithName("GetStreamTransferState").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> GetStreamTransferState(DataItemTransferIdentity? identity , IGrainFactory gf , IDataStreamTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = identity?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(GetStreamTransferStateUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(identity is null) { Log.Error(GetStreamTransferStateBadArgID,id); SetErr(_); return BadRequest(GetStreamTransferStateBadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString()); StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds,hc.RequestAborted);

            if(s is null) { Log.Error(GetStreamTransferStateUnAuthID,id); SetErr(_); return Unauthorized(); }

            GetStreamTransferStateResponse response = await transfer.GetStreamTransferState(identity,hc.RequestAborted);

            Log.Information(GetStreamTransferStateSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,GetStreamTransferStateBadArgID,id); return BadRequest(GetStreamTransferStateBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,GetStreamTransferStateFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,GetStreamTransferStateFailID,id); return InternalError(); }
    }
}
