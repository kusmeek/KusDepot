using KusDepot.CatalogDb;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    /**<include file='GetItemIdentity.xml' path='GetItemIdentity/class[@name="CatalogDBService"]/method[@name="GetItemIdentity"]/*'/>*/
    public async Task<CatalogItemIdentity?> GetItemIdentity(Guid itemID , TraceContext? tracecontext = null , CancellationToken cancel = default)
    {
        using DiagnosticActivity? activity = StartDiagnostic(tracecontext)?.AddTag("id",itemID.ToString());

        try
        {
            ArgumentOutOfRangeException.ThrowIfEqual(itemID,Guid.Empty);

            await InitializeReadyDatabase(cancel);

            using var context = CreateContext(GetActorID());

            Element? item = await context.Elements.AsNoTracking().SingleOrDefaultAsync(row => row.ID == itemID && !row.IsPendingDelete,cancel);

            SetOk(activity);

            return item is null ? null : new CatalogItemIdentity(itemID,item.ItemInstanceToken);
        }
        catch { SetErr(activity); throw; }
    }
}
