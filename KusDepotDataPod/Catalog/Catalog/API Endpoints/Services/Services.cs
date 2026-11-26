namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    private void MapSearchServices(WebApplication application)
    {
        application.MapPost("Catalog/Services",
                   ([FromBody] ServiceQuery? search,
                   [FromServices] IGrainFactory gf,
                   [FromServices] ICatalogDataConfigsClient dc,
                   HttpContext hc) => {return SearchServices(search,gf,dc,hc);})
                   .Produces<ServiceResponse>(StatusCodes.Status200OK)
                   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                   .WithName("SearchServices");
    }

    private async Task<IResult> SearchServices(ServiceQuery? search , IGrainFactory gf , ICatalogDataConfigsClient dc , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",search?.ID);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tracecontext = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(SSVUnAuth); SetErr(_); return Unauthorized(); }

            CatalogResolution resolution = await dc.ResolveCatalogAsync(t,CatalogAccess.Read,tracecontext,hc.RequestAborted);

            if(resolution.Status is not CatalogResolutionStatus.Authorized || String.IsNullOrWhiteSpace(resolution.CatalogName)) { Log.Error(SSVUnAuth); SetErr(_); return Unauthorized(); }

            IResult? pagingError = ValidateCatalogPaging(search?.CatalogPageSize,search?.CatalogPageToken); if(pagingError is not null) { SetErr(_); return pagingError; }

            var c = gf.GetGrain<ICatalogDB>(resolution.CatalogName);

            var f = await c.SearchServices(search,tracecontext,hc.RequestAborted);

            SetOk(_); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,SSVFail); return InternalError(); }
    }
}