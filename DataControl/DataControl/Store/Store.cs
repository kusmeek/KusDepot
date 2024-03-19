namespace KusDepot.Data;

internal sealed partial class DataControl
{
    private static void MapStore(WebApplication application)
    {
        application.MapPost("DataControl",
                   ([FromBody] DataControlUpload? dcu,
                   [FromServices] IArkKeeperFactory af,
                   [FromServices] IBlob bl,
                   [FromServices] ICoreCache cc,
                   [FromServices] IDataConfigs dc,
                   [FromServices] IUniverse un,
                   HttpContext hc) => {return Store(dcu,af,bl,cc,dc,un,hc);})
                   .Produces<String>(StatusCodes.Status200OK)
                   .Produces(StatusCodes.Status400BadRequest)
                   .Produces(StatusCodes.Status401Unauthorized)
                   .Produces<String>(StatusCodes.Status409Conflict)
                   .Produces<String>(StatusCodes.Status422UnprocessableEntity)
                   .Produces<String?>(StatusCodes.Status500InternalServerError)
                   .WithName("Store")
                   .WithOpenApi(o=>{o.OperationId = "Store";return o;});
    }

    private static IResult Store(DataControlUpload? dcu , IArkKeeperFactory af , IBlob bl , ICoreCache cc , IDataConfigs dc , IUniverse un , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc,"Store")?.AddTag("id",dcu?.Descriptor.ID); String t = GetToken(hc);

            _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(dcu is null || dcu.Descriptor is null || dcu.Descriptor.ID is null || String.IsNullOrEmpty(dcu.Object)) { Log.Error(BadArg); SetErr(_); return Results.BadRequest(); }

            if(!dcu.Verify())                                                             { Log.Error(StoreFailHash); SetErr(_); return Results.UnprocessableEntity(StoreFailHash); }

            if(String.IsNullOrEmpty(t))                                                   { Log.Error(StoreUnAuth);   SetErr(_); return Results.Unauthorized(); }

            StorageSilo? s = dc.GetAuthorizedWriteSilo(t,dt,ds).Result; if(s is null)     { Log.Error(StoreUnAuth);   SetErr(_); return Results.Unauthorized(); }

            Descriptor od = dcu.Descriptor; String it = dcu.Object; String id = dcu.Descriptor.ID.ToString()!; IArkKeeper k = af.Create(s.CatalogName);

            if(un.Exists(Guid.Parse(id),dt,ds).Result is not false)                       { Log.Information(StoreFailConflictID,id); SetErr(_); return Results.Conflict(id); }

            if(k.Exists(od,dt,ds).Result is not false)                                    { Log.Information(StoreFailConflictID,id); SetErr(_); return Results.Conflict(id); }

            if(bl.Exists(s.ConnectionString,id,null,dt,ds).Result is not false)           { Log.Information(StoreFailConflictID,id); SetErr(_); return Results.Conflict(id); }

            if(cc.Exists(id,dt,ds).Result is not false)                                   { Log.Information(StoreFailConflictID,id); SetErr(_); return Results.Conflict(id); }

            new Thread(AddCacheDCM).Start(new Object?[]{dcu,dt,ds}); _?.AddEvent(new ActivityEvent("AddCacheDCM"));

            if(bl.Store(s.ConnectionString,id,it.ToByteArrayFromBase64(),dt,ds).Result)
            {
                if(!k.AddUpdate(dcu.Descriptor,dt,ds).Result) { Log.Error(StoreArkFailID,id); }

                if(!un.Add(Guid.Parse(id),dt,ds).Result) { Log.Error(StoreUniFailID,id); }

                Log.Information(StoreSuccessID,id); SetOk(_); return Results.Ok(id);
            }
            Log.Error(StoreBlobFailID,id); SetErr(_); return Results.Problem(id);
        }
        catch ( Exception _ ) { Log.Error(_,StoreFailDescriptor,dcu?.Descriptor); return Results.Problem(dcu?.Descriptor.ID.ToString()); }
    }
}