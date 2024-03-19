namespace KusDepot.Data;

internal sealed partial class DataControl
{
    private static void MapGet(WebApplication application)
    {
        application.MapGet("DataControl/{id}",
                   ([FromRoute] String? id,
                   [FromServices] IBlob bl,
                   [FromServices] ICoreCache cc,
                   [FromServices] IDataConfigs dc,
                   HttpContext hc) => {return Get(id,bl,cc,dc,hc);})
                   .Produces<DataControlDownload>(StatusCodes.Status200OK)
                   .Produces(StatusCodes.Status400BadRequest)
                   .Produces(StatusCodes.Status401Unauthorized)
                   .Produces<String>(StatusCodes.Status404NotFound)
                   .Produces<String>(StatusCodes.Status500InternalServerError)
                   .WithName("Get")
                   .WithOpenApi(o=>{o.OperationId = "Get";return o;});
    }

    private static IResult Get(String? id , IBlob bl , ICoreCache cc , IDataConfigs dc , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc,"Get");

            String t = GetToken(hc); _?.AddTag("id",id)?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(GetUnAuth); SetErr(_); return Results.Unauthorized(); }

            if(id is null || Equals(id,String.Empty)) { Log.Error(BadArg); SetErr(_); return Results.BadRequest(); }

            StorageSilo? s = dc.GetAuthorizedReadSilo(t,dt,ds).Result; if(s is null) { Log.Error(GetUnAuth); SetErr(_); return Results.Unauthorized(); }

            if(cc.Exists(id,dt,ds).Result is true) { Log.Information(GetSuccessCacheID,id); SetOk(_); return Results.Ok(MakeDataControlDownload(cc.Get(id,dt,ds).Result)); }

            if(bl.Exists(s.ConnectionString,id,null,dt,ds).Result is true)
            {
                String? it = bl.Get(s.ConnectionString,id,null,dt,ds).Result?.ToBase64FromByteArray();

                new Thread(AddCacheIDIT).Start(new Object?[]{id,it,dt,ds}); _?.AddEvent(new ActivityEvent("AddCacheIDIT"));

                Log.Information(GetSuccessID,id); SetOk(_); return Results.Ok(MakeDataControlDownload(it));
            }
            Log.Information(GetNotFoundID,id); SetErr(_); return Results.NotFound(id);
        }
        catch ( Exception _ ) { Log.Error(_,GetFailID,id); return Results.Problem(id); }
    }
}