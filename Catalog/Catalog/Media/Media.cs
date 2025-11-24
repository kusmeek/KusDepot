namespace KusDepot.Data;

public sealed partial class Catalog
{
    private void MapSearchMedia(WebApplication application)
    {
        application.MapPost("Catalog/Media",
                   ([FromBody] MediaQuery? search,
                   [FromServices] IDataConfigs dc,
                   [FromServices] ICatalogDBFactory cdb,
                   HttpContext hc) => {return SearchMedia(search,dc,cdb,hc);})
                   .Produces<MediaResponse>(StatusCodes.Status200OK)
                   .Produces<MediaResponse>(StatusCodes.Status404NotFound)
                   .WithName("SearchMedia").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> SearchMedia(MediaQuery? search , IDataConfigs dc , ICatalogDBFactory cdb , HttpContext hc)
    {
        try
        {
            ETW.Log.SMStart();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",search?.ID);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); if(String.IsNullOrEmpty(t)) { Log.Error(SMUnAuth); SetErr(_); ETW.Log.SMError(SMUnAuth); return Unauthorized(); }

            StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds); if(s is null) { Log.Error(SMUnAuth); SetErr(_); ETW.Log.SMError(SMUnAuth); return Unauthorized(); }

            ICatalogDB c = cdb.Create(s.CatalogName); var f = await c.SearchMedia(search,dt,ds);

            if(Equals(f.Media.Length,0)) { SetOk(_); ETW.Log.SMSuccess(0); return Results.NotFound(new MediaResponse()); }

            SetOk(_); ETW.Log.SMSuccess(f.Media.Length); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,SMFail); ETW.Log.SMError(_.Message); return InternalError(); }
    }
}