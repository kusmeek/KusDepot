namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapOpenStreamTransfer(WebApplication application)
    {
        application.MapPost("OpenStreamTransfer",
                   ([FromBody] OpenStreamTransferRequest? request,
                   [FromServices] IDataStreamTransferService transfer,
                   HttpContext hc) => { return OpenStreamTransfer(request,transfer,hc); })
                   .WithMetadata(new TransferRequestBodyRateLimitMetadata())
                   .Produces<OpenStreamTransferResponse>(StatusCodes.Status200OK)
                   .WithName("OpenStreamTransfer");
    }

    private async Task<IResult> OpenStreamTransfer(OpenStreamTransferRequest? request , IDataStreamTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tc = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(OpenStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(OpenStreamTransferBadArgID,id); SetErr(_); return BadRequest(OpenStreamTransferBadArg); }

            var dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? s = await dc.ResolveStorageAsync(t,CatalogAccess.Write,tc,hc.RequestAborted);

            if(s is null) { Log.Error(OpenStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(s);

            OpenStreamTransferResponse response = await transfer.OpenStreamTransfer(request,hc.RequestAborted);

            Log.Information(OpenStreamTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,OpenStreamTransferBadArgID,id); return BadRequest(OpenStreamTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,OpenStreamTransferFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,OpenStreamTransferFailID,id); return InternalError(); }
    }
}
