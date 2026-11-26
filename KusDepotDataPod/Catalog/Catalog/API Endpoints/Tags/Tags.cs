namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    private void MapSearchTags(WebApplication application)
    {
        application.MapPost("Catalog/Tags",
                   ([FromBody] TagQuery? search,
                   [FromServices] IGrainFactory gf,
                   [FromServices] ICatalogDataConfigsClient dc,
                   HttpContext hc) => {return SearchTags(search,gf,dc,hc);})
                   .Produces<TagResponse>(StatusCodes.Status200OK)
                   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                   .WithName("SearchTags");
    }

    private async Task<IResult> SearchTags(TagQuery? search , IGrainFactory gf , ICatalogDataConfigsClient dc , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc); String tk = GetToken(hc);

            _?.AddTag("enduser.id",GetUPN(tk)); TraceContext? tracecontext = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(tk)) { Log.Error(STGUnAuth); SetErr(_); return Unauthorized(); }

            if(search is null || search.Tags is null || Equals(search.Tags.Length,0)) { Log.Error(BadArg); SetErr(_); return BadRequest(BadArg); }

            CatalogResolution resolution = await dc.ResolveCatalogAsync(tk,CatalogAccess.Read,tracecontext,hc.RequestAborted);

            if(resolution.Status is not CatalogResolutionStatus.Authorized || String.IsNullOrWhiteSpace(resolution.CatalogName)) { Log.Error(STGUnAuth); SetErr(_); return Unauthorized(); }

            IResult? pagingError = ValidateCatalogPaging(search.CatalogPageSize,search.CatalogPageToken); if(pagingError is not null) { SetErr(_); return pagingError; }

            var c = gf.GetGrain<ICatalogDB>(resolution.CatalogName);

            var f = await c.SearchTags(search,tracecontext,hc.RequestAborted);

            SetOk(_); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,STGFail); return InternalError(); }
    }
}