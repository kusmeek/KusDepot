namespace KusDepot.Data;

public sealed partial class Catalog
{
    private void MapSearchTags(WebApplication application)
    {
        application.MapPost("Catalog/Tags",
                   ([FromBody] TagQuery? search,
                   [FromServices] IDataConfigs dc,
                   [FromServices] ICatalogDBFactory cdb,
                   HttpContext hc) => {return SearchTags(search,dc,cdb,hc);})
                   .Produces<TagResponse>(StatusCodes.Status200OK)
                   .Produces<TagResponse>(StatusCodes.Status404NotFound)
                   .WithName("SearchTags").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> SearchTags(TagQuery? search , IDataConfigs dc , ICatalogDBFactory cdb , HttpContext hc)
    {
        try
        {
            ETW.Log.STGStart();

            using DiagnosticActivity? _ = StartDiagnostic(hc); String tk = GetToken(hc);

            _?.AddTag("enduser.id",GetUPN(tk)); String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(tk)) { Log.Error(STGUnAuth); SetErr(_); ETW.Log.STGError(STGUnAuth); return Unauthorized(); }

            if(search is null || search.Tags is null || Equals(search.Tags.Length,0)) { Log.Error(BadArg); SetErr(_); ETW.Log.STGError(BadArg); return BadRequest(BadArg); }

            StorageSilo? s = await dc.GetAuthorizedReadSilo(tk,dt,ds); if(s is null) { Log.Error(STGUnAuth); SetErr(_); ETW.Log.STGError(STGUnAuth); return Unauthorized(); }

            ICatalogDB c = cdb.Create(s.CatalogName); var f = await c.SearchTags(search,dt,ds);

            if(Equals(f.IDs.Count,0)) { SetOk(_); ETW.Log.STGSuccess(); return Results.NotFound(new TagResponse()); }

            SetOk(_); ETW.Log.STGSuccess(); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,STGFail); ETW.Log.STGError(_.Message); return InternalError(); }
    }
}