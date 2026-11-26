using KusDepot.CatalogDb;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    /**<include file='GetItemState.xml' path='GetItemState/class[@name="CatalogDBService"]/method[@name="GetItemState"]/*'/>*/
    public async Task<CatalogItemState?> GetItemState(Guid itemID , TraceContext? tracecontext = null , CancellationToken cancel = default)
    {
        using DiagnosticActivity? activity = StartDiagnostic(tracecontext)?.AddTag("id",itemID.ToString());

        try
        {
            ArgumentOutOfRangeException.ThrowIfEqual(itemID,Guid.Empty);

            await InitializeReadyDatabase(cancel);

            using var context = CreateContext(GetActorID());

            Element? item = await context.Elements.AsNoTracking().SingleOrDefaultAsync(row => row.ID == itemID,cancel);

            SetOk(activity);

            return item is null ? null : new CatalogItemState(itemID,item.ItemInstanceToken,item.IsPendingDelete);
        }
        catch { SetErr(activity); throw; }
    }
}
