namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapOpenUploadTransfer(WebApplication application)
    {
        application.MapPost("OpenUploadTransfer",
                   ([FromBody] OpenUploadTransferRequest? request,
                   [FromServices] IDataItemSegmentedTransferService transfer,
                   HttpContext hc) => { return OpenUploadTransfer(request,transfer,hc); })
                   .WithMetadata(new TransferRequestBodyRateLimitMetadata())
                   .Produces<OpenUploadTransferResponse>(StatusCodes.Status200OK)
                   .WithName("OpenUploadTransfer");
    }

    private async Task<IResult> OpenUploadTransfer(OpenUploadTransferRequest? request , IDataItemSegmentedTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tc = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(OpenUploadTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(OpenUploadTransferBadArgID,id); SetErr(_); return BadRequest(OpenUploadTransferBadArg); }

            var dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? s = await dc.ResolveStorageAsync(t,CatalogAccess.Write,tc,hc.RequestAborted);

            if(s is null) { Log.Error(OpenUploadTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(s);

            OpenUploadTransferResponse response = await transfer.OpenUploadTransfer(request,hc.RequestAborted);

            Log.Information(OpenUploadTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,OpenUploadTransferBadArgID,id); return BadRequest(OpenUploadTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,OpenUploadTransferFailID,id); return MapSegmentedFailure(_); }

        catch ( Exception _ ) { Log.Error(_,OpenUploadTransferFailID,id); return InternalError(); }
    }
}
