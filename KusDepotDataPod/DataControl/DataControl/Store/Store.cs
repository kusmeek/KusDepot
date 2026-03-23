namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapStore(WebApplication application)
    {
        application.MapPost("Store",
                   ([FromBody] DataControlUpload? dcu,
                   [FromServices] IBlob bl,
                   [FromServices] ICacheManager cm,
                   [FromServices] IGrainFactory gf,
                   HttpContext hc) => {return Store(dcu,bl,cm,gf,hc);})
                   .WithName("Store").RequireAuthorization(X509Policy);
    }

    private async Task<IResult> Store(DataControlUpload? dcu , IBlob bl , ICacheManager cm , IGrainFactory gf , HttpContext hc)
    {
        try
        {
            String? id = dcu?.Descriptor.ID.ToString(); ETW.Log.StoreStart(id);

            using DiagnosticActivity? _ = StartDiagnostic(hc)?.AddTag("id",dcu?.Descriptor.ID);

            String t = GetToken(hc); _?.AddTag("enduser.id",GetUPN(t)); String? dt = _?.Context.TraceId.ToString();

            String? ds = _?.Context.SpanId.ToString(); Descriptor? od = dcu?.Descriptor; String? it = dcu?.Object;

            if(String.IsNullOrEmpty(t)) { Log.Error(StoreUnAuthID,id); SetErr(_); ETW.Log.StoreError(StoreUnAuth,id); return Unauthorized(); }

            if(dcu is null || dcu.Descriptor is null) { Log.Error(VerificationFailID,id); SetErr(_); ETW.Log.StoreError(VerificationFail,id); return Unprocessable($"{VerificationFail} {id}"); }

            if(await dcu.VerifyAsync(hc.RequestAborted) is false)
            {
                Log.Error(VerificationFailID,dcu?.Descriptor.ID.ToString()); SetErr(_); ETW.Log.StoreError(VerificationFail,dcu?.Descriptor.ID.ToString());

                return Unprocessable($"{VerificationFail} {dcu?.Descriptor.ID.ToString()}");
            }

            var dc = gf.GetGrain<IDataConfigs>(Guid.NewGuid().ToString());

            StorageSilo? s = await dc.GetAuthorizedWriteSilo(t,dt,ds); if(s is null) { Log.Error(StoreUnAuthID,id); SetErr(_); ETW.Log.StoreError(StoreUnAuth,id); return Unauthorized(); }

            var c = gf.GetGrain<ICatalogDB>(s.CatalogName);

            if(await c.Exists(od,dt,ds) is not false)                          { Log.Error(StoreFailConflictID,id); SetErr(_); ETW.Log.StoreError(StoreFailConflict,id); return Conflict(id); }

            if(await cm.Exists(id,dt,ds) is not false)                         { Log.Error(StoreFailConflictID,id); SetErr(_); ETW.Log.StoreError(StoreFailConflict,id); return Conflict(id); }

            if(bl.Initialize(s.ConnectionString,id,null) is false)             { Log.Error(BlobInitFailID,id);      SetErr(_); ETW.Log.StoreError(BlobInitFailID,id); return InternalError(); }

            if(await bl.Exists(hc.RequestAborted) is not false)                { Log.Error(StoreFailConflictID,id); SetErr(_); ETW.Log.StoreError(StoreFailConflict,id); return Conflict(id); }

            cm.EnqueueItem(id,it,dt,ds); _?.AddEvent(new ActivityEvent("AddCacheDCU"));

            if(await bl.Store(it.ToByteArrayFromBase64(),hc.RequestAborted))
            {
                if(await c.AddUpdate(dcu?.Descriptor,dt,ds) is false) { Log.Error(StoreCatalogFailID,id); ETW.Log.StoreIssue(StoreCatalogFail,id); }

                Log.Information(StoreSuccessID,id); SetOk(_); ETW.Log.StoreSuccess(id); return Results.Ok(id);
            }
            Log.Error(StoreBlobFailID,id); SetErr(_); ETW.Log.StoreError(StoreBlobFail,id); return InternalError();
        }
        catch ( Exception _ ) { Log.Error(_,StoreFailDescriptor,dcu?.Descriptor); ETW.Log.StoreError(_.Message,dcu?.Descriptor.ID?.ToString()); return InternalError(); }
    }
}