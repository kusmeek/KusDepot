namespace KusDepot.Data;

internal sealed partial class DataControl
{
    private static void MapDelete(WebApplication application)
    {
        application.MapDelete("DataControl/{id}",
                   ([FromRoute] String? id,
                   [FromServices] IArkKeeperFactory af,
                   [FromServices] IBlob bl,
                   [FromServices] ICoreCache cc,
                   [FromServices] IDataConfigs dc,
                   [FromServices] IUniverse un,
                   HttpContext hc) => {return Delete(id,af,bl,cc,dc,un,hc);})
                   .Produces<String>(StatusCodes.Status200OK)
                   .Produces(StatusCodes.Status400BadRequest)
                   .Produces(StatusCodes.Status401Unauthorized)
                   .Produces<String>(StatusCodes.Status404NotFound)
                   .Produces<String>(StatusCodes.Status500InternalServerError)
                   .WithName("Delete")
                   .WithOpenApi(o=>{o.OperationId = "Delete";return o;});
    }

    private static IResult Delete(String? id , IArkKeeperFactory af , IBlob bl , ICoreCache cc , IDataConfigs dc , IUniverse un , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc,"Delete");

            String t = GetToken(hc); _?.AddTag("id",id)?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(DeleteUnAuth); SetErr(_); return Results.Unauthorized(); }

            if(id is null || Equals(id,String.Empty)) { Log.Error(BadArg); SetErr(_); return Results.BadRequest(); }

            StorageSilo? s = dc.GetAuthorizedWriteSilo(t,dt,ds).Result; if(s is null) { Log.Error(DeleteUnAuth); SetErr(_); return Results.Unauthorized(); }

            if(un.Exists(Guid.Parse(id),dt,ds).Result is not true) { Log.Information(DeleteNotFoundID,id); SetErr(_); return Results.NotFound(id); }

            if(bl.Exists(s.ConnectionString,id,null,dt,ds).Result is true)
            {
                if(bl.Delete(s.ConnectionString,id,null,dt,ds).Result)
                {
                    if(!af.Create(s.CatalogName).RemoveID(Guid.Parse(id),dt,ds).Result) { Log.Error(DeleteArkFailID,id); }

                    if(!un.Remove(Guid.Parse(id),dt,ds).Result) { Log.Error(DeleteUniFailID,id); }

                    if(cc.Exists(id,dt,ds).Result is true) { cc.Delete(id,dt,ds).Wait(); }

                    Log.Information(DeleteSuccessID,id); SetOk(_); return Results.Ok(id);
                }
                Log.Error(DeleteBlobFailID,id); SetErr(_); return Results.Problem(id);
            }
            Log.Information(DeleteNotFoundID,id); SetErr(_); return Results.NotFound(id);
        }
        catch ( Exception _ ) { Log.Error(_,DeleteFailID,id); return Results.Problem(id); }
    }
}