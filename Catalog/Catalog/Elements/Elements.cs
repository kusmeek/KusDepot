namespace KusDepot.Data;

public sealed partial class Catalog
{
    private void MapSearchElements(WebApplication application)
    {
        application.MapPost("Catalog/Elements",
                   ([FromBody] ElementQuery? search,
                   [FromServices] IDataConfigs dc,
                   [FromServices] ICatalogDBFactory cdb,
                   HttpContext hc) => {return SearchElements(search,dc,cdb,hc);})
                   .Produces<ElementResponse>(StatusCodes.Status200OK)
                   .Produces<ElementResponse>(StatusCodes.Status404NotFound)
                   .WithName("SearchElements").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> SearchElements(ElementQuery? search , IDataConfigs dc , ICatalogDBFactory cdb , HttpContext hc)
    {
        try
        {
            ETW.Log.SELStart();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",search?.ID);;

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); if(String.IsNullOrEmpty(t)) { Log.Error(SELUnAuth); SetErr(_); ETW.Log.SELError(SELUnAuth); return Unauthorized(); }

            StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds); if(s is null) { Log.Error(SELUnAuth); SetErr(_); ETW.Log.SELError(SELUnAuth); return Unauthorized(); }

            ICatalogDB c = cdb.Create(s.CatalogName); var f = await c.SearchElements(search,dt,ds);

            if(Equals(f.Elements.Length,0)) { SetOk(_); ETW.Log.SELSuccess(0); return Results.NotFound(new ElementResponse()); }

            SetOk(_); ETW.Log.SELSuccess(f.Elements.Length); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,SELFail); ETW.Log.SELError(_.Message); return InternalError(); }
    }
}