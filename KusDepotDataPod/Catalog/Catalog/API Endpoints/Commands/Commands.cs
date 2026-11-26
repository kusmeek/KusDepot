namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    private void MapSearchCommands(WebApplication application)
    {
        application.MapPost("Catalog/Commands",
                   ([FromBody] CommandQuery? search,
                   [FromServices] IGrainFactory gf,
                   [FromServices] ICatalogDataConfigsClient dc,
                   HttpContext hc) => {return SearchCommands(search,gf,dc,hc);})
                   .Produces<CommandResponse>(StatusCodes.Status200OK)
                   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                   .WithName("SearchCommands");
    }

    private async Task<IResult> SearchCommands(CommandQuery? search , IGrainFactory gf , ICatalogDataConfigsClient dc , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",search?.ID);;

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tracecontext = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(SCUnAuth); SetErr(_); return Unauthorized(); }

            CatalogResolution resolution = await dc.ResolveCatalogAsync(t,CatalogAccess.Read,tracecontext,hc.RequestAborted);

            if(resolution.Status is not CatalogResolutionStatus.Authorized || String.IsNullOrWhiteSpace(resolution.CatalogName)) { Log.Error(SCUnAuth); SetErr(_); return Unauthorized(); }

            IResult? pagingError = ValidateCatalogPaging(search?.CatalogPageSize,search?.CatalogPageToken); if(pagingError is not null) { SetErr(_); return pagingError; }

            var c = gf.GetGrain<ICatalogDB>(resolution.CatalogName);

            var f = await c.SearchCommands(search,tracecontext,hc.RequestAborted);

            SetOk(_); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,SCFail); return InternalError(); }
    }
}