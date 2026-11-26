using KusDepot.CatalogDb;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    /**<include file='Delete.xml' path='Delete/class[@name="CatalogDBService"]/method[@name="BeginDelete"]/*'/>*/
    public async Task<CatalogItemIdentity?> BeginDelete(Guid itemID , TraceContext? tracecontext = null , CancellationToken cancel = default)
    {
        using DiagnosticActivity? activity = StartDiagnostic(tracecontext)?.AddTag("id",itemID.ToString());

        try
        {
            ArgumentOutOfRangeException.ThrowIfEqual(itemID,Guid.Empty);

            await InitializeReadyDatabase(cancel);

            using var context = CreateContext(GetActorID());

            await using var transaction = await context.Database.BeginTransactionAsync(cancel);

            Element? item = await context.Elements
                .FromSql($"SELECT * FROM \"Elements\" WHERE \"ID\" = {itemID} FOR UPDATE")
                .SingleOrDefaultAsync(cancel);

            if(item is null)
            {
                await transaction.CommitAsync(cancel);

                SetOk(activity);

                return null;
            }

            if(!item.IsPendingDelete)
            {
                item.IsPendingDelete = true;

                await context.SaveChangesAsync(cancel);
            }

            await transaction.CommitAsync(cancel);

            SetOk(activity);

            return new CatalogItemIdentity(itemID,item.ItemInstanceToken);
        }
        catch { SetErr(activity); throw; }
    }

    /**<include file='Delete.xml' path='Delete/class[@name="CatalogDBService"]/method[@name="FinalizeDelete"]/*'/>*/
    public async Task<CatalogDeleteFinalizationResult> FinalizeDelete(Guid itemID , Guid itemInstanceToken , TraceContext? tracecontext = null , CancellationToken cancel = default)
    {
        using DiagnosticActivity? activity = StartDiagnostic(tracecontext)?.AddTag("id",itemID.ToString());

        try
        {
            ArgumentOutOfRangeException.ThrowIfEqual(itemID,Guid.Empty);

            ArgumentOutOfRangeException.ThrowIfEqual(itemInstanceToken,Guid.Empty);

            await InitializeReadyDatabase(cancel);

            using var context = CreateContext(GetActorID());

            await using var transaction = await context.Database.BeginTransactionAsync(cancel);

            Element? item = await context.Elements
                .FromSql($"SELECT * FROM \"Elements\" WHERE \"ID\" = {itemID} FOR UPDATE")
                .SingleOrDefaultAsync(cancel);

            if(item is null)
            {
                await transaction.CommitAsync(cancel);

                SetOk(activity);

                return CatalogDeleteFinalizationResult.AlreadyAbsent;
            }

            if(item.ItemInstanceToken != itemInstanceToken)
            {
                await transaction.CommitAsync(cancel);

                SetOk(activity);

                return CatalogDeleteFinalizationResult.TokenMismatch;
            }

            if(!item.IsPendingDelete)
            {
                await transaction.CommitAsync(cancel);

                SetOk(activity);

                return CatalogDeleteFinalizationResult.NotPending;
            }

            context.Elements.Remove(item);

            await context.SaveChangesAsync(cancel);

            await transaction.CommitAsync(cancel);

            SetOk(activity);

            return CatalogDeleteFinalizationResult.Removed;
        }
        catch { SetErr(activity); throw; }
    }

    /**<include file='Delete.xml' path='Delete/class[@name="CatalogDBService"]/method[@name="GetPendingDeletes"]/*'/>*/
    public async Task<CatalogPendingDeletePage> GetPendingDeletes(Guid? afterItemID , Int32 pageSize , CancellationToken cancel = default)
    {
        if(pageSize <= 0 || pageSize > 100) { throw new ArgumentOutOfRangeException(nameof(pageSize)); }

        await InitializeReadyDatabase(cancel);

        using var context = CreateContext(GetActorID());

        IQueryable<Element> query = context.Elements.AsNoTracking().Where(row => row.IsPendingDelete);

        if(afterItemID is Guid after)
        {
            query = query.Where(row => EF.Functions.GreaterThan(ValueTuple.Create(row.ID!.Value),ValueTuple.Create(after)));
        }

        List<Element> candidates = await query.OrderBy(row => row.ID).Take(pageSize + 1).ToListAsync(cancel);

        Boolean hasMore = candidates.Count > pageSize;

        Element[] page = candidates.Take(pageSize).ToArray();

        CatalogItemIdentity[] items = page.Select(row => new CatalogItemIdentity(row.ID!.Value,row.ItemInstanceToken)).ToArray();

        return new CatalogPendingDeletePage(items,hasMore ? page[^1].ID : null);
    }
}