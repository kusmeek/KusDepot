namespace KusDepot.Data;

public sealed partial class Catalog
{
    private void MapSearchServices(WebApplication application)
    {
        application.MapPost("Catalog/Services",
                   ([FromBody] ServiceQuery? search,
                   [FromServices] IDataConfigs dc,
                   [FromServices] ICatalogDBFactory cdb,
                   HttpContext hc) => {return SearchServices(search,dc,cdb,hc);})
                   .Produces<ServiceResponse>(StatusCodes.Status200OK)
                   .Produces<ServiceResponse>(StatusCodes.Status404NotFound)
                   .WithName("SearchServices").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> SearchServices(ServiceQuery? search , IDataConfigs dc , ICatalogDBFactory cdb , HttpContext hc)
    {
        try
        {
            ETW.Log.SSVStart();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",search?.ID);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); if(String.IsNullOrEmpty(t)) { Log.Error(SSVUnAuth); SetErr(_); ETW.Log.SSVError(SSVUnAuth); return Unauthorized(); }

            StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds); if(s is null) { Log.Error(SSVUnAuth); SetErr(_); ETW.Log.SSVError(SSVUnAuth); return Unauthorized(); }

            ICatalogDB c = cdb.Create(s.CatalogName); var f = await c.SearchServices(search,dt,ds);

            if(Equals(f.Services.Length,0)) { SetOk(_); ETW.Log.SSVSuccess(0); return Results.NotFound(new ServiceResponse()); }

            SetOk(_); ETW.Log.SSVSuccess(f.Services.Length); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,SSVFail); ETW.Log.SSVError(_.Message); return InternalError(); }
    }
}