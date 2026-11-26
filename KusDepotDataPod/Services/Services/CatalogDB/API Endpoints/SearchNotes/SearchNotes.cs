using KusDepot.Data.Models;
using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<NoteResponse> SearchNotes(NoteQuery? search , TraceContext? tracecontext = null , CancellationToken cancel = default)
    {
        DiagnosticActivity? da = null;

        try
        {
            da = StartDiagnostic(tracecontext);

            CatalogDBPageRequest pageRequest = CatalogDBQueryPaging.ResolveRequest(search?.CatalogPageSize,search?.CatalogPageToken);

            cancel.ThrowIfCancellationRequested();

            await InitializeReadyDatabase(cancel);

            if(search?.Notes is null || search.Notes.Length == 0)
            {
                SetOk(da);

                return new NoteResponse { NextCatalogPageToken = null };
            }

            using var ctx = CreateContext(GetActorID());

            IQueryable<Guid> idsQuery = Enumerable.Empty<Guid>().AsQueryable();

            Boolean first = true;

            foreach(var raw in search.Notes.Where(s => !String.IsNullOrWhiteSpace(s)))
            {
                String term = raw!;

                var termIds = ctx.Notes
                    .Where(n => n.ID != null && EF.Functions.ILike(n.Value,$"%{term}%"))
                    .Select(n => n.ID!.Value)
                    .Distinct();

                idsQuery = first ? termIds : idsQuery.Intersect(termIds);

                first = false;
            }

            idsQuery = idsQuery.Where(id => ctx.Elements.Any(element => element.ID == id && !element.IsPendingDelete));

            if(first) { SetOk(da); return new NoteResponse(); }

            if(pageRequest.AfterKey is Guid afterKey)
            {
                idsQuery = idsQuery.Where(id => EF.Functions.GreaterThan(
                    ValueTuple.Create(id),
                    ValueTuple.Create(afterKey)));
            }

            var page = await CatalogDBQueryPaging.FetchAsync(idsQuery,id => id,pageRequest.PageSize,cancel);

            SetOk(da);

            return new NoteResponse { IDs = page.Items.ToArray(), NextCatalogPageToken = page.NextPageToken };
        }
        catch ( CatalogDBInitializationException ) { SetErr(da); throw; }
        catch ( CatalogPageRequestException ) { SetErr(da); throw; }
        catch ( Exception _ )
        {
            Logger.Error(_,SNFail); SetErr(da); throw;
        }
        finally { da?.Dispose(); }
    }
}