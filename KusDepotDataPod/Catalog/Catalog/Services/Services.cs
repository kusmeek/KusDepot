namespace DataPodServices.Catalog;

public sealed partial class Catalog
{
    private void MapSearchServices(WebApplication application)
    {
        application.MapPost("Catalog/Services",
                   ([FromBody] ServiceQuery? search,
                   [FromServices] IGrainFactory gf,
                   HttpContext hc) => {return SearchServices(search,gf,hc);})
                   .Produces<ServiceResponse>(StatusCodes.Status200OK)
                   .Produces<ServiceResponse>(StatusCodes.Status404NotFound)
                   .WithName("SearchServices").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> SearchServices(ServiceQuery? search , IGrainFactory gf , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",search?.ID);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); if(String.IsNullOrEmpty(t)) { Log.Error(SSVUnAuth); SetErr(_); return Unauthorized(); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToStringInvariant()!);

            StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds,hc.RequestAborted); if(s is null) { Log.Error(SSVUnAuth); SetErr(_); return Unauthorized(); }

            var c = gf.GetGrain<ICatalogDB>(s.CatalogName);

            var f = await c.SearchServices(search,dt,ds,hc.RequestAborted);

            if(Equals(f.Services.Length,0)) { SetOk(_); return Results.NotFound(new ServiceResponse()); }

            SetOk(_); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,SSVFail); return InternalError(); }
    }
}