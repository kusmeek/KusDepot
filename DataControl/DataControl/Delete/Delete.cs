namespace KusDepot.Data;

public sealed partial class DataControl
{
    private void MapDelete(WebApplication application)
    {
        application.MapDelete("Delete/{id}",
                   ([FromRoute] String? id,
                   [FromServices] IBlob bl,
                   [FromServices] ICoreCache cc,
                   [FromServices] IDataConfigs dc,
                   [FromServices] ICatalogDBFactory cdb,
                   HttpContext hc) => {return Delete(id,cdb,bl,cc,dc,hc);})
                   .WithName("Delete").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> Delete(String? id , ICatalogDBFactory cdb , IBlob bl , ICoreCache cc , IDataConfigs dc , HttpContext hc)
    {
        try
        {
            ETW.Log.DeleteStart(id);

            using DiagnosticActivity? _ = StartDiagnostic(hc);

            String t = GetToken(hc); _?.AddTag("id",id)?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(DeleteUnAuthID,id); SetErr(_); ETW.Log.DeleteError(DeleteUnAuth,id); return Unauthorized(); }

            if(id is null || Equals(id,String.Empty)) { Log.Error(BadArgID,id); SetErr(_); ETW.Log.DeleteError(BadArg,id); return BadRequest(BadArg); }

            StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds); if(s is null) { Log.Error(DeleteUnAuthID,id); SetErr(_); ETW.Log.DeleteError(DeleteUnAuth,id); return Unauthorized(); }

            ICatalogDB c = cdb.Create(s.CatalogName);

            if(await c.ExistsID(Guid.Parse(id),dt,ds) is not true) { Log.Error(DeleteNotFoundID,id); SetErr(_); ETW.Log.DeleteError(DeleteNotFound,id); return Results.NotFound(id); }

            if(await bl.Exists(s.ConnectionString,id,null,dt,ds) is true)
            {
                if(await bl.Delete(s.ConnectionString,id,null,dt,ds) is true)
                {
                    if(await c.RemoveID(Guid.Parse(id),dt,ds) is false)
                    {
                        Log.Error(DeleteCatalogFailID,id); SetErr(_); ETW.Log.DeleteIssue(DeleteCatalogFail,id);
                    }

                    if(await cc.Exists(id,dt,ds) is true) { await cc.Delete(id,dt,ds); }

                    Log.Information(DeleteSuccessID,id); SetOk(_); ETW.Log.DeleteSuccess(id); return Results.Ok(id);
                }
                Log.Error(DeleteBlobFailID,id); SetErr(_); ETW.Log.DeleteError(DeleteBlobFail,id); return InternalError();
            }
            Log.Error(DeleteNotFoundID,id); SetErr(_); ETW.Log.DeleteError(DeleteNotFound,id); return Results.NotFound(id);
        }
        catch ( Exception _ ) { Log.Error(_,DeleteFailID,id); ETW.Log.DeleteError(DeleteFail,id); return InternalError(); }

        finally { DeleteBlobActor(bl.GetActorId()); }
    }
}