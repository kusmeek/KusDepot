namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    private void MapSearchNotes(WebApplication application)
    {
        application.MapPost("Catalog/Notes",
                   ([FromBody] NoteQuery? search,
                   [FromServices] IGrainFactory gf,
                   [FromServices] ICatalogDataConfigsClient dc,
                   HttpContext hc) => {return SearchNotes(search,gf,dc,hc);})
                   .Produces<NoteResponse>(StatusCodes.Status200OK)
                   .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                   .WithName("SearchNotes");
    }

    private async Task<IResult> SearchNotes(NoteQuery? search , IGrainFactory gf , ICatalogDataConfigsClient dc , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc); String t = GetToken(hc);

            _?.AddTag("enduser.id",GetUPN(t)); TraceContext? tracecontext = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(SNUnAuth); SetErr(_); return Unauthorized(); }

            if(search is null || search.Notes is null || Equals(search.Notes.Length,0)) { Log.Error(BadArg); SetErr(_); return BadRequest(BadArg); }

            CatalogResolution resolution = await dc.ResolveCatalogAsync(t,CatalogAccess.Read,tracecontext,hc.RequestAborted);

            if(resolution.Status is not CatalogResolutionStatus.Authorized || String.IsNullOrWhiteSpace(resolution.CatalogName)) { Log.Error(SNUnAuth); SetErr(_); return Unauthorized(); }

            IResult? pagingError = ValidateCatalogPaging(search.CatalogPageSize,search.CatalogPageToken); if(pagingError is not null) { SetErr(_); return pagingError; }

            var c = gf.GetGrain<ICatalogDB>(resolution.CatalogName);

            var f = await c.SearchNotes(search,tracecontext,hc.RequestAborted);

            SetOk(_); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,SNFail); return InternalError(); }
    }
}