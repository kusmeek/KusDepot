namespace KusDepot.Data;

public sealed partial class Catalog
{
    private void MapSearchCommands(WebApplication application)
    {
        application.MapPost("Catalog/Commands",
                   ([FromBody] CommandQuery? search,
                   [FromServices] IDataConfigs dc,
                   [FromServices] ICatalogDBFactory cdb,
                   HttpContext hc) => {return SearchCommands(search,dc,cdb,hc);})
                   .Produces<CommandResponse>(StatusCodes.Status200OK)
                   .Produces<CommandResponse>(StatusCodes.Status404NotFound)
                   .WithName("SearchCommands").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> SearchCommands(CommandQuery? search , IDataConfigs dc , ICatalogDBFactory cdb , HttpContext hc)
    {
        try
        {
            ETW.Log.SCStart();

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",search?.ID);;

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); if(String.IsNullOrEmpty(t)) { Log.Error(SCUnAuth); SetErr(_); ETW.Log.SCError(SCUnAuth); return Unauthorized(); }

            StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds); if(s is null) { Log.Error(SCUnAuth); SetErr(_); ETW.Log.SCError(SCUnAuth); return Unauthorized(); }

            ICatalogDB c = cdb.Create(s.CatalogName); var f = await c.SearchCommands(search,dt,ds);

            if(Equals(f.Commands.Length,0)) { SetOk(_); ETW.Log.SCSuccess(0); return Results.NotFound(new CommandResponse()); }

            SetOk(_); ETW.Log.SCSuccess(f.Commands.Length); return Results.Ok(f);
        }
        catch ( Exception _ ) { Log.Error(_,SCFail); ETW.Log.SCError(_.Message); return InternalError(); }
    }
}