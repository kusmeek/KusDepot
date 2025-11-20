namespace KusDepot.Data;

internal sealed partial class CatalogDB
{
    public async Task<NoteResponse> SearchNotes(NoteQuery? search , String? traceid = null , String? spanid = null)
    {
        DiagnosticActivity? da = null;

        try
        {
            ETW.Log.SNStart(); da = StartDiagnostic(traceid,spanid);

            if(search?.Notes is null || search.Notes.Length == 0)
            {
                SetOk(da); ETW.Log.SNSuccess();

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

            var ids = await idsQuery.ToListAsync();

            if(ids.Count == 0)
            {
                SetOk(da); ETW.Log.SNSuccess();

                return new NoteResponse();
            }

            SetOk(da); ETW.Log.SNSuccess();

            return new NoteResponse { IDs = ids.ToHashSet() };
        }
        catch ( Exception _ )
        {
            Log.Error(_,SNFail); SetErr(da); ETW.Log.SNError(_.Message);

            return new NoteResponse();
        }
        finally { da?.Dispose(); }
    }
}