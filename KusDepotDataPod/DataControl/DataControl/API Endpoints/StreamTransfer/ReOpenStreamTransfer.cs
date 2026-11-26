namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapReOpenStreamTransfer(WebApplication application)
    {
        application.MapPost("ReOpenStreamTransfer",
                   ([FromBody] ReOpenStreamTransferRequest? request,
                   [FromServices] IDataStreamTransferService transfer,
                   HttpContext hc) => { return ReOpenStreamTransfer(request,transfer,hc); })
                   .Produces<ReOpenStreamTransferResponse>(StatusCodes.Status200OK)
                   .WithName("ReOpenStreamTransfer");
    }

    private async Task<IResult> ReOpenStreamTransfer(ReOpenStreamTransferRequest? request , IDataStreamTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tc = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(ReOpenStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(ReOpenStreamTransferBadArgID,id); SetErr(_); return BadRequest(ReOpenStreamTransferBadArg); }

            var dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? s = await dc.ResolveStorageAsync(t,CatalogAccess.Write,tc,hc.RequestAborted);

            if(s is null) { Log.Error(ReOpenStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(s);

            ReOpenStreamTransferResponse response = await transfer.ReOpenStreamTransfer(request,hc.RequestAborted);

            Log.Information(ReOpenStreamTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,ReOpenStreamTransferBadArgID,id); return BadRequest(ReOpenStreamTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,ReOpenStreamTransferFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,ReOpenStreamTransferFailID,id); return InternalError(); }
    }
}
