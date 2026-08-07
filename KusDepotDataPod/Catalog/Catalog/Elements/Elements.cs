namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    private void MapSearchElements(WebApplication application)
    {
        application.MapPost("Catalog/Elements",
                   ([FromBody] ElementQuery? search,
                   [FromServices] IGrainFactory gf,
                   HttpContext hc) => {return SearchElements(search,gf,hc);})
                   .Produces<ElementResponse>(StatusCodes.Status200OK)
                   .Produces<ElementResponse>(StatusCodes.Status404NotFound)
                   .WithName("SearchElements").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> SearchElements(ElementQuery? search , IGrainFactory gf , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",search?.ID);;

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); if(String.IsNullOrEmpty(t)) { Log.Error(SELUnAuth); SetErr(_); return Unauthorized(); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToStringInvariant()!);

            StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds,hc.RequestAborted); if(s is null) { Log.Error(SELUnAuth); SetErr(_); return Unauthorized(); }

            var c = gf.GetGrain<ICatalogDB>(s.CatalogName);

            var f = await c.SearchElements(search,dt,ds,hc.RequestAborted);

            if(Equals(f.Elements.Length,0)) { SetOk(_); return Results.NotFound(new ElementResponse()); }

            SetOk(_); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,SELFail); return InternalError(); }
    }
}