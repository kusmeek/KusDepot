using KusDepot.Data.Models;
using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<TagResponse> SearchTags(TagQuery? search , TraceContext? tracecontext = null , CancellationToken cancel = default)
    {
        DiagnosticActivity? da = null;

        try
        {
            da = StartDiagnostic(tracecontext);

            CatalogDBPageRequest pageRequest = CatalogDBQueryPaging.ResolveRequest(search?.CatalogPageSize,search?.CatalogPageToken);

            cancel.ThrowIfCancellationRequested();

            await InitializeReadyDatabase(cancel);

            if(search?.Tags is null || search.Tags.Length == 0)
            {
                SetOk(da);

                return new TagResponse { NextCatalogPageToken = null };
            }

            using var ctx = CreateContext(GetActorID());

            IQueryable<Guid> idsQuery = Enumerable.Empty<Guid>().AsQueryable();

            Boolean first = true;

            foreach(var raw in search.Tags.Where(s => !String.IsNullOrWhiteSpace(s)))
            {
                String term = raw!;

                var termIds = ctx.Tags
                    .Where(t => t.ID != null && EF.Functions.ILike(t.Value,$"%{term}%"))
                    .Select(t => t.ID!.Value)
                    .Distinct();

                idsQuery = first ? termIds : idsQuery.Intersect(termIds);

                first = false;
            }

            idsQuery = idsQuery.Where(id => ctx.Elements.Any(element => element.ID == id && !element.IsPendingDelete));

            if(first) { SetOk(da); return new TagResponse(); }

            if(pageRequest.AfterKey is Guid afterKey)
            {
                idsQuery = idsQuery.Where(id => EF.Functions.GreaterThan(
                    ValueTuple.Create(id),
                    ValueTuple.Create(afterKey)));
            }

            var page = await CatalogDBQueryPaging.FetchAsync(idsQuery,id => id,pageRequest.PageSize,cancel);

            SetOk(da);

            return new TagResponse { IDs = page.Items.ToArray(), NextCatalogPageToken = page.NextPageToken };
        }
        catch ( CatalogDBInitializationException ) { SetErr(da); throw; }
        catch ( CatalogPageRequestException ) { SetErr(da); throw; }
        catch ( Exception _ )
        {
            Logger.Error(_,STGFail); SetErr(da); throw;
        }
        finally { da?.Dispose(); }
    }
}