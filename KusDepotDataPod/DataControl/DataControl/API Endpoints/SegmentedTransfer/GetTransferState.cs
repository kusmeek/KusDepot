namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapGetTransferState(WebApplication application)
    {
        application.MapPost("GetTransferState",
                   ([FromBody] DataItemTransferIdentity? identity,
                   [FromServices] IDataItemSegmentedTransferService transfer,
                    HttpContext hc) => {return GetTransferState(identity,transfer,hc);})
                   .Produces<GetTransferStateResponse>(StatusCodes.Status200OK)
                   .WithName("GetTransferState");
    }

    private async Task<IResult> GetTransferState(DataItemTransferIdentity? identity , IDataItemSegmentedTransferService transfer , HttpContext hc)
    {
        String? id = null;
        try
        {
            id = identity?.ItemID.ToString();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",id);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tc = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(GetTransferStateUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(identity is null) { Log.Error(GetTransferStateBadArgID,id); SetErr(_); return BadRequest(GetTransferStateBadArg); }

            var dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? s = await dc.ResolveStorageAsync(t,CatalogAccess.Read,tc,hc.RequestAborted);

            if(s is null) { Log.Error(GetTransferStateUnAuthID,id); SetErr(_); return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(s);

            GetTransferStateResponse response = await transfer.GetTransferState(identity,hc.RequestAborted);

            Log.Information(GetTransferStateSuccessID,id); SetOk(_); return Results.Ok(response);
        }
        catch ( ArgumentException _ ) { Log.Error(_,GetTransferStateBadArgID,id); return BadRequest(GetTransferStateBadArg); }

        catch ( OperationFailedException _ ) { Log.Error(_,GetTransferStateFailID,id); return MapSegmentedFailure(_); }

        catch ( Exception _ ) { Log.Error(_,GetTransferStateFailID,id); return InternalError(); }
    }
}
