using KusDepot.Data.Models;
using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<NoteResponse> SearchNotes(NoteQuery? search , String? traceid = null , String? spanid = null , CancellationToken cancel = default)
    {
        DiagnosticActivity? da = null;

        try
        {
            da = StartDiagnostic(traceid,spanid);

            cancel.ThrowIfCancellationRequested();

            if(search?.Notes is null || search.Notes.Length == 0)
            {
                SetOk(da);

                return new NoteResponse();
            }

            using var ctx = ctxfactory.Create(BuildConnectionString(GetActorID()));

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

            if(first) { SetOk(da); return new NoteResponse(); }

            var ids = await idsQuery.ToListAsync(cancel);

            if(ids.Count == 0)
            {
                SetOk(da);

                return new NoteResponse();
            }

            SetOk(da);

            return new NoteResponse { IDs = ids.ToHashSet() };
        }
        catch ( Exception _ )
        {
            Logger.Error(_,SNFail); SetErr(da);

            return new NoteResponse();
        }
        finally { da?.Dispose(); }
    }
}