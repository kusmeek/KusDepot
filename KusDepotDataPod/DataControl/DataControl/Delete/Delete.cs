namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapDelete(WebApplication application)
    {
        application.MapDelete("Delete/{id}",
                   ([FromRoute] String? id,
                   [FromServices] IBlob bl,
                   [FromServices] IGrainFactory gf,
                   HttpContext hc) => {return Delete(id,bl,gf,hc);})
                   .WithName("Delete").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> Delete(String? id , IBlob bl , IGrainFactory gf , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc);

            String t = GetToken(hc); _?.AddTag("id",id)?.AddTag("enduser.id",GetUPN(t));

            String? dt = _?.Context.TraceId.ToString(); String? ds = _?.Context.SpanId.ToString();

            if(String.IsNullOrEmpty(t)) { Log.Error(DeleteUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(id is null || Equals(id,String.Empty)) { Log.Error(BadArgID,id); SetErr(_); return BadRequest(BadArg); }

            if(Guid.TryParse(id,out Guid itemId) is false) { Log.Error(BadArgID,id); SetErr(_); return BadRequest(BadArg); }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString());

            StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds,hc.RequestAborted); if(s is null) { Log.Error(DeleteUnAuthID,id); SetErr(_); return Unauthorized(); }

            var c = gf.GetGrain<ICatalogDB>(s.CatalogName);

            if(await c.ExistsID(itemId,dt,ds,hc.RequestAborted) is not true) { Log.Error(DeleteNotFoundID,id); SetErr(_); return NotFound(id); }

            if(bl.Initialize(s.ConnectionString,id,null) is false) { Log.Error(BlobInitFailID,id); SetErr(_); return InternalError(); }

            if(await bl.Exists(hc.RequestAborted) is true)
            {
                if(await bl.Delete(hc.RequestAborted) is true)
                {
                    if(await c.RemoveID(itemId,dt,ds,hc.RequestAborted) is false)
                    {
                        Log.Error(DeleteCatalogFailID,id); SetErr(_); 
                    }

                    Log.Information(DeleteSuccessID,id); SetOk(_); return Results.Ok(id);
                }
                Log.Error(DeleteBlobFailID,id); SetErr(_); return InternalError();
            }
            Log.Error(DeleteNotFoundID,id); SetErr(_); return NotFound(id);
        }
        catch ( Exception _ ) { Log.Error(_,DeleteFailID,id); return InternalError(); }
    }
}