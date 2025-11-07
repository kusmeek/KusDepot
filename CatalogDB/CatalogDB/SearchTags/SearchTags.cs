namespace KusDepot.Data;

internal sealed partial class CatalogDB
{
    public async Task<TagResponse> SearchTags(TagQuery? search , String? traceid = null , String? spanid = null)
    {
        DiagnosticActivity? da = null;

        try
        {
            ETW.Log.STGStart(); da = StartDiagnostic(traceid,spanid);

            if(search?.Tags is null || search.Tags.Length == 0)
            {
                SetOk(da); ETW.Log.STGSuccess();

                return new TagResponse();
            }

            using var ctx = ctxfactory.Create(BuildConnectionString(GetActorID()));

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

            var ids = await idsQuery.ToListAsync();

            if(ids.Count == 0)
            {
                SetOk(da); ETW.Log.STGSuccess();

                return new TagResponse();
            }

            SetOk(da); ETW.Log.STGSuccess();

            return new TagResponse { IDs = ids.ToHashSet() };
        }
        catch ( Exception _ )
        {
            Log.Error(_,STGFail); SetErr(da); ETW.Log.STGError(_.Message);

            return new TagResponse();
        }
        finally { da?.Dispose(); }
    }
}