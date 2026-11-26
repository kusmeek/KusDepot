namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapPublicationRecoveryAdmin(WebApplication application)
    {
        application.MapPost("InspectPublication",
            ([FromBody] InspectPublicationRequest? request,
            [FromServices] IDataControlDataConfigsClient dataConfigs,
            [FromServices] IPublicationRecoveryAdminService service,
            HttpContext context) => InspectPublication(request,dataConfigs,service,context))
            .Produces<PublicationRecoveryFootprint>(StatusCodes.Status200OK)
            .WithName("InspectPublication");

        application.MapPost("PurgePublication",
            ([FromBody] PurgePublicationRequest? request,
            [FromServices] IDataControlDataConfigsClient dataConfigs,
            [FromServices] IPublicationRecoveryAdminService service,
            HttpContext context) => PurgePublication(request,dataConfigs,service,context))
            .Produces<PurgePublicationResponse>(StatusCodes.Status200OK)
            .WithName("PurgePublication");
    }

    private async Task<IResult> InspectPublication(InspectPublicationRequest? request , IDataControlDataConfigsClient dataConfigs , IPublicationRecoveryAdminService service , HttpContext context)
    {
        try
        {
            String token = GetToken(context);

            using DiagnosticActivity? _ = StartDiagnostic(context)?.AddTag("session.id",request?.SessionID.ToString());

            if(String.IsNullOrWhiteSpace(token) || request is null) { SetErr(_); return request is null ? BadRequest() : Unauthorized(); }

            TraceContext? tc = GetTraceContext(context,_);

            if((await dataConfigs.CheckAdminAsync(token,tc,context.RequestAborted)).Status is not AdminCheckStatus.Authorized) { SetErr(_); return Unauthorized(); }

            PublicationRecoveryFootprint? result = await service.InspectAsync(request.SessionID,context.RequestAborted);

            if(result is null) { SetErr(_); return NotFound(); }

            SetOk(_); return Results.Ok(result);
        }
        catch ( ArgumentException ) { return BadRequest(); }

        catch ( Exception error ) { Log.Error(error,InspectPublicationFailID,request?.SessionID); return InternalError(); }
    }

    private async Task<IResult> PurgePublication(PurgePublicationRequest? request , IDataControlDataConfigsClient dataConfigs , IPublicationRecoveryAdminService service , HttpContext context)
    {
        try
        {
            String token = GetToken(context);

            using DiagnosticActivity? _ = StartDiagnostic(context)?.AddTag("session.id",request?.SessionID.ToString());

            if(String.IsNullOrWhiteSpace(token) || request is null) { SetErr(_); return request is null ? BadRequest() : Unauthorized(); }

            TraceContext? tc = GetTraceContext(context,_);

            if((await dataConfigs.CheckAdminAsync(token,tc,context.RequestAborted)).Status is not AdminCheckStatus.Authorized) { SetErr(_); return Unauthorized(); }

            StorageSilo? silo = await dataConfigs.ResolveRecoveryStorageAsync(request.CatalogName,context.RequestAborted);

            if(silo is null || !String.Equals(silo.CatalogName,request.CatalogName,Ordinal)) { SetErr(_); return Unauthorized(); }

            context.RequestServices.GetRequiredService<ITenantTransferStorageContext>().Set(silo);

            PurgePublicationResponse? result = await service.PurgeAsync(request,context.RequestAborted);

            if(result is null) { SetErr(_); return Conflict("Publication state changed after inspection."); }

            SetOk(_); return Results.Ok(result);
        }
        catch ( ArgumentException ) { return BadRequest(); }

        catch ( Exception error ) { Log.Error(error,PurgePublicationFailID,request?.SessionID); return InternalError(); }
    }
}
