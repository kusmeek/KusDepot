namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    private void MapSearchMedia(WebApplication application)
    {
        application.MapPost("Catalog/Media",
                   ([FromBody] MediaQuery? search,
                   [FromServices] IGrainFactory gf,
                   HttpContext hc) => {return SearchMedia(search,gf,hc);})
                   .Produces<MediaResponse>(StatusCodes.Status200OK)
                   .Produces<MediaResponse>(StatusCodes.Status404NotFound)
                   .WithName("SearchMedia").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> SearchMedia(MediaQuery? search , IGrainFactory gf , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",search?.ID);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); if(String.IsNullOrEmpty(t)) { Log.Error(SMUnAuth); SetErr(_); return Unauthorized(); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToStringInvariant()!);

            StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds,hc.RequestAborted); if(s is null) { Log.Error(SMUnAuth); SetErr(_); return Unauthorized(); }

            var c = gf.GetGrain<ICatalogDB>(s.CatalogName);

            var f = await c.SearchMedia(search,dt,ds,hc.RequestAborted);

            if(Equals(f.Media.Length,0)) { SetOk(_); return Results.NotFound(new MediaResponse()); }

            SetOk(_); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,SMFail); return InternalError(); }
    }
}