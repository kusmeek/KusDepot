namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapReOpenGetTransfer(WebApplication application)
    {
        application.MapPost("ReOpenGetTransfer",
                   ([FromBody] ReOpenGetTransferRequest? request,
                   [FromServices] IDataItemSegmentedTransferService transfer,
                   HttpContext hc) => { return ReOpenGetTransfer(request,transfer,hc); })
                   .WithMetadata(new PublishedTransferRequestContextMetadata())
                   .Produces<ReOpenGetTransferResponse>(StatusCodes.Status200OK)
                   .WithName("ReOpenGetTransfer");
    }

    private async Task<IResult> ReOpenGetTransfer(ReOpenGetTransferRequest? request , IDataItemSegmentedTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tc = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(ReOpenGetTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(ReOpenGetTransferBadArgID,id); SetErr(_); return BadRequest(ReOpenGetTransferBadArg); }

            var dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? s = await dc.ResolveStorageAsync(t,CatalogAccess.Read,tc,hc.RequestAborted);

            if(s is null) { Log.Error(ReOpenGetTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(s);

            ReOpenGetTransferResponse response = await transfer.ReOpenGetTransfer(request,hc.RequestAborted);

            Log.Information(ReOpenGetTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,ReOpenGetTransferBadArgID,id); return BadRequest(ReOpenGetTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,ReOpenGetTransferFailID,id); return MapSegmentedFailure(_); }

        catch ( Exception _ ) { Log.Error(_,ReOpenGetTransferFailID,id); return InternalError(); }
    }
}
