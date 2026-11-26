namespace DataPodServices.CatalogDB;

internal sealed record CatalogDBQueryPage<T>(IReadOnlyList<T> Items , String? NextPageToken);

internal readonly record struct CatalogDBPageRequest(Int32 PageSize , Guid? AfterKey);

internal static class CatalogDBQueryPaging
{
    internal static CatalogDBPageRequest ResolveRequest(Int32? pageSize , String? pageToken)
    {
        return new CatalogDBPageRequest(
            CatalogPageSize.Resolve(pageSize),
            CatalogPageTokenCodec.Decode(pageToken));
    }

    internal static async Task<CatalogDBQueryPage<T>> FetchAsync<T>(
        IQueryable<T> query,
        Expression<Func<T,Guid>> keySelector,
        Int32 pageSize,
        CancellationToken cancel)
    {
        var candidates = await query
            .OrderBy(keySelector)
            .Take(pageSize + 1)
            .ToListAsync(cancel);

        Boolean hasMore = candidates.Count > pageSize;

        T[] items = candidates.Take(pageSize).ToArray();

        String? nextPageToken = null;

        if(hasMore)
        {
            nextPageToken = CatalogPageTokenCodec.Encode(keySelector.Compile()(items[^1]));
        }

        return new CatalogDBQueryPage<T>(items,nextPageToken);
    }
}
