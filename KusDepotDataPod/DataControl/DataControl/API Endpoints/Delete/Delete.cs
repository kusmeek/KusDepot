namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapDelete(WebApplication application)
    {
        application.MapDelete("Delete/{id}",
                   ([FromRoute] String? id,
                   [FromServices] IGrainFactory gf,
                   [FromServices] IDataItemTransferCoordinator coordinator,
                   [FromServices] DataControlDeletionProcessor deletionProcessor,
                   HttpContext hc) => {return Delete(id,gf,coordinator,deletionProcessor,hc);})
                   .Produces(StatusCodes.Status200OK)
                   .WithName("Delete");
    }

    /**<include file='Delete.xml' path='Delete/class[@name="DataControl"]/method[@name="Delete"]/*'/>*/
    private async Task<IResult> Delete(String? id , IGrainFactory gf , IDataItemTransferCoordinator coordinator , DataControlDeletionProcessor deletionProcessor , HttpContext hc)
    {
        try
        {
            using DiagnosticActivity? _ = StartDiagnostic(hc);

            String t = GetToken(hc); _?.AddTag("id",id)?.AddTag("enduser.id",GetUPN(t));

            TraceContext? tc = GetTraceContext(hc,_);

            if(String.IsNullOrEmpty(t)) { Log.Error(DeleteUnAuthID,id); SetErr(_); return Unauthorized(); }

            if(id is null || Equals(id,String.Empty)) { Log.Error(BadArgID,id); SetErr(_); return BadRequest(BadArg); }

            if(Guid.TryParse(id,out Guid itemId) is false) { Log.Error(BadArgID,id); SetErr(_); return BadRequest(BadArg); }

            IDataControlDataConfigsClient dc = hc.RequestServices.GetRequiredService<IDataControlDataConfigsClient>();

            StorageSilo? s = await dc.ResolveStorageAsync(t,CatalogAccess.Write,tc,hc.RequestAborted); if(s is null) { Log.Error(DeleteUnAuthID,id); SetErr(_); return Unauthorized(); }

            var c = gf.GetGrain<ICatalogDB>(s.CatalogName);

            IDataControlDeletionJobStore jobs = hc.RequestServices.GetRequiredService<IDataControlDeletionJobStore>();

            IAsyncDisposable? itemLock = await coordinator.TryAcquireItemLockAsync(itemId,hc.RequestAborted);

            if(itemLock is not null)
            {
                await using(itemLock)
                {
                    CatalogItemIdentity? deletionIdentity = await c.BeginDelete(itemId,tc,hc.RequestAborted);

                    if(deletionIdentity is null) { Log.Error(DeleteNotFoundID,id); SetErr(_); return NotFound(id); }

                    DataControlDeletionJobState deletion = await jobs.EnsureDeletionJobAsync(s.CatalogName,itemId,deletionIdentity.ItemInstanceToken,hc.RequestAborted);

                    DataControlDeletionJobState? completed = await deletionProcessor.ProcessWithLockAsync(deletion,hc.RequestAborted);

                    if(completed is null) { Log.Information(DeleteSuccessID,id); SetOk(_); return Results.Ok(id); }

                    return Results.Accepted(value:id);
                }
            }

            CatalogItemIdentity? queuedIdentity = await c.BeginDelete(itemId,tc,hc.RequestAborted);

            if(queuedIdentity is null) { Log.Error(DeleteNotFoundID,id); SetErr(_); return NotFound(id); }

            await jobs.EnsureDeletionJobAsync(s.CatalogName,itemId,queuedIdentity.ItemInstanceToken,hc.RequestAborted);

            return Results.Accepted(value:id);
        }
        catch ( OperationCanceledException ) when(hc.RequestAborted.IsCancellationRequested) { throw; }

        catch ( Exception _ ) { Log.Error(_,DeleteFailID,id); return InternalError(); }
    }
}