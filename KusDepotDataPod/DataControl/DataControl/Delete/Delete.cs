namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapDelete(WebApplication application)
    {
        application.MapDelete("Delete/{id}",
                   ([FromRoute] String? id,
                   [FromServices] IBlob bl,
                   [FromServices] ICacheManager cm,
                   [FromServices] IGrainFactory gf,
                   HttpContext hc) => {return Delete(id,bl,cm,gf,hc);})
                   .WithName("Delete").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> Delete(String? id , IBlob bl , ICacheManager cm , IGrainFactory gf , HttpContext hc)
    {
        try
        {
            ETW.Log.DeleteStart(id);

            using DiagnosticActivity? _ = StartDiagnostic(hc);

            String t = GetToken(hc); _?.AddTag("id",id)?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(DeleteUnAuthID,id); SetErr(_); ETW.Log.DeleteError(DeleteUnAuth,id); return Unauthorized(); }

            if(id is null || Equals(id,String.Empty)) { Log.Error(BadArgID,id); SetErr(_); ETW.Log.DeleteError(BadArg,id); return BadRequest(BadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString());

            StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds); if(s is null) { Log.Error(DeleteUnAuthID,id); SetErr(_); ETW.Log.DeleteError(DeleteUnAuth,id); return Unauthorized(); }

            var c = gf.GetGrain<ICatalogDB>(s.CatalogName);

            if(await c.ExistsID(Guid.Parse(id),dt,ds) is not true) { Log.Error(DeleteNotFoundID,id); SetErr(_); ETW.Log.DeleteError(DeleteNotFound,id); return Results.NotFound(id); }

            if(bl.Initialize(s.ConnectionString,id,null) is false) { Log.Error(BlobInitFailID,id);   SetErr(_); ETW.Log.StoreError(BlobInitFailID,id); return InternalError(); }

            if(await bl.Exists(hc.RequestAborted) is true)
            {
                if(await bl.Delete(hc.RequestAborted) is true)
                {
                    if(await c.RemoveID(Guid.Parse(id),dt,ds) is false)
                    {
                        Log.Error(DeleteCatalogFailID,id); SetErr(_); ETW.Log.DeleteIssue(DeleteCatalogFail,id);
                    }

                    if(await cm.Exists(id,dt,ds) is true) { await cm.Delete(id,dt,ds); }

                    Log.Information(DeleteSuccessID,id); SetOk(_); ETW.Log.DeleteSuccess(id); return Results.Ok(id);
                }
                Log.Error(DeleteBlobFailID,id); SetErr(_); ETW.Log.DeleteError(DeleteBlobFail,id); return InternalError();
            }
            Log.Error(DeleteNotFoundID,id); SetErr(_); ETW.Log.DeleteError(DeleteNotFound,id); return Results.NotFound(id);
        }
        catch ( Exception _ ) { Log.Error(_,DeleteFailID,id); ETW.Log.DeleteError(DeleteFail,id); return InternalError(); }
    }
}