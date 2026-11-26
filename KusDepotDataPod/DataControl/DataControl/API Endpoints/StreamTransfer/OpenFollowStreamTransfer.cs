namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapOpenFollowStreamTransfer(WebApplication application)
    {
        application.MapPost("OpenFollowStreamTransfer",
                   ([FromBody] OpenFollowStreamTransferRequest? request,
                   [FromServices] IDataStreamTransferService transfer,
                   HttpContext hc) => { return OpenFollowStreamTransfer(request,transfer,hc); })
                   .Produces<OpenFollowStreamTransferResponse>(StatusCodes.Status200OK)
                   .WithName("OpenFollowStreamTransfer");
    }

    private async Task<IResult> OpenFollowStreamTransfer(OpenFollowStreamTransferRequest? request , IDataStreamTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = request?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tc = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(OpenFollowStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(request is null) { Log.Error(OpenFollowStreamTransferBadArgID,id); SetErr(_); return BadRequest(OpenFollowStreamTransferBadArg); }

            var dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? s = await dc.ResolveStorageAsync(t,CatalogAccess.Read,tc,hc.RequestAborted);

            if(s is null) { Log.Error(OpenFollowStreamTransferUnAuthID,id); SetErr(_); return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(s);

            OpenFollowStreamTransferResponse response = await transfer.OpenFollowStreamTransfer(request,hc.RequestAborted);

            Log.Information(OpenFollowStreamTransferSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,OpenFollowStreamTransferBadArgID,id); return BadRequest(OpenFollowStreamTransferBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,OpenFollowStreamTransferFailID,id); return MapStreamTransferFailure(_); }

        catch ( Exception _ ) { Log.Error(_,OpenFollowStreamTransferFailID,id); return InternalError(); }
    }
}
