namespace KusDepot.Data;

public sealed partial class DataControl
{
    private void MapGet(WebApplication application)
    {
        application.MapGet("Get/{id}",
                   ([FromRoute] String? id,
                   [FromServices] IBlob bl,
                   [FromServices] ICoreCache cc,
                   [FromServices] IDataConfigs dc,
                   HttpContext hc) => {return Get(id,bl,cc,dc,hc);})
                   .Produces<DataControlDownload>(StatusCodes.Status200OK)
                   .WithName("Get").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> Get(String? id , IBlob bl , ICoreCache cc , IDataConfigs dc , HttpContext hc)
    {
        try
        {
            ETW.Log.GetStart(id);

            using DiagnosticActivity? _ = StartDiagnostic(hc);

            String t = GetToken(hc); _?.AddTag("id",id)?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(GetUnAuthID,id); SetErr(_); ETW.Log.GetError(GetUnAuth,id); return Unauthorized(); }

            if(id is null || Equals(id,String.Empty)) { Log.Error(BadArgID,id); SetErr(_); ETW.Log.GetError(BadArgID,id); return BadRequest(BadArg); }

            StorageSilo? s = await dc.GetAuthorizedReadSilo(t,dt,ds); if(s is null) { Log.Error(GetUnAuth,id); SetErr(_); ETW.Log.GetError(GetUnAuth,id); return Unauthorized(); }

            if(await cc.Exists(id,dt,ds) is true) { Log.Information(GetSuccessCacheID,id); SetOk(_); ETW.Log.GetSuccess(id); return Results.Ok(MakeDataControlDownload(await cc.Get(id,dt,ds))); }

            if(await bl.Exists(s.ConnectionString,id,null,dt,ds) is true)
            {
                String? it = (await bl.Get(s.ConnectionString,id,null,dt,ds))?.ToBase64FromByteArray();

                new Thread(AddCacheIDIT).Start(new Tuple<String?,String?,String?,String?>(id,it,dt,ds)); _?.AddEvent(new ActivityEvent("AddCacheIDIT"));

                Log.Information(GetSuccessID,id); SetOk(_); ETW.Log.GetSuccess(id); return Results.Ok(MakeDataControlDownload(it));
            }
            Log.Error(GetNotFoundID,id); SetErr(_); ETW.Log.GetError(GetNotFound,id); return Results.NotFound(id);
        }
        catch ( Exception _ ) { Log.Error(_,GetFailID,id); ETW.Log.GetError(GetFail,id); return InternalError(); }

        finally { DeleteBlobActor(bl.GetActorId()); }
    }
}