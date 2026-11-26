namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapCloseTransferSession(WebApplication application)
    {
        application.MapPost("CloseTransferSession",
                   ([FromBody] CloseTransferSessionRequest? request,
                   [FromServices] IDataControlTransferSessionStore sessions,
                   [FromServices] IDataItemTransferCoordinator coordinator,
                   HttpContext hc) => CloseTransferSession(request,sessions,coordinator,hc))
                   .Produces<CloseTransferSessionResponse>(StatusCodes.Status200OK)
                   .WithName("CloseTransferSession");
    }

    private async Task<IResult> CloseTransferSession(CloseTransferSessionRequest? request , IDataControlTransferSessionStore sessions , IDataItemTransferCoordinator coordinator , HttpContext hc)
    {
        String? id = request?.ItemID.ToString();
        try
        {
            using DiagnosticActivity? activity = StartDiagnostic(hc)?.AddTag("id",id);

            String token = GetToken(hc); activity?.AddTag("enduser.id",GetUPN(token));

            TraceContext? trace = GetTraceContext(hc,activity);

            if(String.IsNullOrWhiteSpace(token)) { return Unauthorized(); }

            if(request is null || request.ItemID == Guid.Empty || request.SessionID == Guid.Empty) { return BadRequest(); }

            IDataControlDataConfigsClient configs = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? silo = await configs.ResolveStorageAsync(token,CatalogAccess.Read,trace,hc.RequestAborted); if(silo is null) { return Unauthorized(); }

            hc.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(silo);

            await using ITransferMutationScope mutation = await coordinator.AcquireMutationScopeAsync(request.ItemID,request.SessionID,hc.RequestAborted);

            Boolean closed = await sessions.CloseReadSessionAsync(request.SessionID,request.ItemID,hc.RequestAborted); if(!closed) { return Conflict(); }

            return Results.Ok(new CloseTransferSessionResponse { Closed = true , ItemID = request.ItemID , SessionID = request.SessionID });
        }
        catch ( ArgumentException error )
        {
            Log.Warning(error,CloseTransferSessionBadArgID,id); return BadRequest();
        }
        catch ( Exception error ) when(error is not OperationCanceledException)
        {
            Log.Error(error,CloseTransferSessionFailID,id); return InternalError();
        }
    }
}
