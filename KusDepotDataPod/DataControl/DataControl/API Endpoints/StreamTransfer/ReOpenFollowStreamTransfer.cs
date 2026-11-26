namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapReOpenFollowStreamTransfer(WebApplication application)
    {
        application.MapPost("ReOpenFollowStreamTransfer",
                   ([FromBody] ReOpenFollowStreamTransferRequest? request,
                   [FromServices] IDataStreamTransferService transfer,
                   HttpContext hc) => { return ReOpenFollowStreamTransfer(request,transfer,hc); })
                   .Produces<ReOpenFollowStreamTransferResponse>(StatusCodes.Status200OK)
                   .WithName("ReOpenFollowStreamTransfer");
    }

    private async Task<IResult> ReOpenFollowStreamTransfer(ReOpenFollowStreamTransferRequest? request , IDataStreamTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tc = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(ReOpenFollowStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(ReOpenFollowStreamTransferBadArgID,id); SetErr(_); return BadRequest(ReOpenFollowStreamTransferBadArg); }

            var dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? s = await dc.ResolveStorageAsync(t,CatalogAccess.Read,tc,hc.RequestAborted);

            if(s is null) { Log.Error(ReOpenFollowStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(s);

            ReOpenFollowStreamTransferResponse response = await transfer.ReOpenFollowStreamTransfer(request,hc.RequestAborted);

            Log.Information(ReOpenFollowStreamTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,ReOpenFollowStreamTransferBadArgID,id); return BadRequest(ReOpenFollowStreamTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,ReOpenFollowStreamTransferFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,ReOpenFollowStreamTransferFailID,id); return InternalError(); }
    }
}
