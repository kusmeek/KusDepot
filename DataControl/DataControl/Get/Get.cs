namespace KusDepot.Data;

public sealed partial class DataControl
{
    private void MapGet(WebApplication application)
    {
        application.MapGet("Get/{id}",
                   ([FromRoute] String? id,
                   [FromServices] IBlob bl,
                   [FromServices] IDataConfigs dc,
                   [FromServices] ICacheManager cm,
                   HttpContext hc) => {return Get(id,bl,cm,dc,hc);})
                   .Produces<DataControlDownload>(StatusCodes.Status200OK)
                   .WithName("Get").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> Get(String? id , IBlob bl , ICacheManager cm , IDataConfigs dc , HttpContext hc)
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

            if(await cm.Exists(id,dt,ds) is true)
            {
                Log.Information(GetSuccessCacheID,id); SetOk(_); ETW.Log.GetSuccess(id); return Results.Ok(await MakeDataControlDownloadAsync(await cm.Get(id,dt,ds),null,hc.RequestAborted));
            }

            if(bl.Initialize(s.ConnectionString,id,null) is false) { Log.Error(BlobInitFailID,id); SetErr(_); ETW.Log.StoreError(BlobInitFailID,id); return InternalError(); }

            if(await bl.Exists(hc.RequestAborted) is true)
            {
                String? it = (await bl.Get(hc.RequestAborted))?.ToBase64FromByteArray();

                cm.EnqueueItem(id,it,dt,ds); _?.AddEvent(new ActivityEvent("AddCacheIDIT"));

                Log.Information(GetSuccessID,id); SetOk(_); ETW.Log.GetSuccess(id); return Results.Ok(await MakeDataControlDownloadAsync(it,null,hc.RequestAborted));
            }
            Log.Error(GetNotFoundID,id); SetErr(_); ETW.Log.GetError(GetNotFound,id); return Results.NotFound(id);
        }
        catch ( Exception _ ) { Log.Error(_,GetFailID,id); ETW.Log.GetError(GetFail,id); return InternalError(); }
    }
}