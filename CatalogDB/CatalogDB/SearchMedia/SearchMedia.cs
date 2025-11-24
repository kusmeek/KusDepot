namespace KusDepot.Data;

internal sealed partial class CatalogDB
{
    public async Task<MediaResponse> SearchMedia(MediaQuery? search , String? traceid = null , String? spanid = null)
    {
        DiagnosticActivity? da = null;

        try
        {
            ETW.Log.SMStart(); da = StartDiagnostic(traceid,spanid);

            using var ctx = ctxfactory.Create(BuildConnectionString(GetActorID()));

            IQueryable<CatalogDb.MultiMedia> q = ctx.MediaLibrary.AsQueryable();

            if(!String.IsNullOrWhiteSpace(search?.Album))
            {
                q = q.Where(m => m.Album != null && EF.Functions.ILike(m.Album,$"%{search!.Album!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Application))
            {
                q = q.Where(m => m.Application != null && EF.Functions.ILike(m.Application,$"%{search!.Application!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Artist))
            {
                q = q.Where(m => m.Artist != null && EF.Functions.ILike(m.Artist,$"%{search!.Artist!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.BornOn))
            {
                q = q.Where(m => m.BornOn != null && EF.Functions.ILike(m.BornOn,$"%{search!.BornOn!}%"));
            }

            if(search?.ContentStreamed is not null)
            {
                q = q.Where(m => m.ContentStreamed == search.ContentStreamed);
            }

            if(!String.IsNullOrWhiteSpace(search?.DistinguishedName))
            {
                q = q.Where(m => m.DistinguishedName != null && EF.Functions.ILike(m.DistinguishedName,$"%{search!.DistinguishedName!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.FILE))
            {
                q = q.Where(m => m.FilePath != null && EF.Functions.ILike(m.FilePath,$"%{search!.FILE!}%"));
            }

            if(search?.ID is not null)
            {
                q = q.Where(m => m.ID == search.ID);
            }

            if(!String.IsNullOrWhiteSpace(search?.Modified))
            {
                q = q.Where(m => m.Modified != null && EF.Functions.ILike(m.Modified,$"%{search!.Modified!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Name))
            {
                q = q.Where(m => m.Name != null && EF.Functions.ILike(m.Name,$"%{search!.Name!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Size))
            {
                q = q.Where(m => m.Size == search.Size);
            }

            if(!String.IsNullOrWhiteSpace(search?.Title))
            {
                q = q.Where(m => m.Title != null && EF.Functions.ILike(m.Title,$"%{search!.Title!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Type))
            {
                q = q.Where(m => m.Type != null && EF.Functions.ILike(m.Type,$"%{search!.Type!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Year))
            {
                q = q.Where(m => m.Year != null && EF.Functions.ILike(m.Year,$"%{search!.Year!}%"));
            }

            if(search?.Notes is not null && search.Notes.Length > 0)
            {
                foreach(var note in search.Notes.Where(s => !String.IsNullOrWhiteSpace(s)))
                {
                    String n = note!; q = q.Where(m => ctx.Notes.Any(x => x.ID == m.ID && EF.Functions.ILike(x.Value,$"%{n}%")));
                }
            }

            if(search?.Tags is not null && search.Tags.Length > 0)
            {
                foreach(var tag in search.Tags.Where(s => !String.IsNullOrWhiteSpace(s)))
                {
                    String t = tag!; q = q.Where(m => ctx.Tags.Any(x => x.ID == m.ID && EF.Functions.ILike(x.Value,$"%{t}%")));
                }
            }

            var results = await q.ToListAsync();

            if(results.Count == 0) { SetOk(da); ETW.Log.SMSuccess(0); return new MediaResponse(); }

            var ids = results.Select(r => r.ID!.Value).ToArray();

            var notesLookup = await ctx.Notes
                .Where(n => n.ID != null && ids.Contains(n.ID.Value))
                .GroupBy(n => n.ID)
                .ToDictionaryAsync(g => g.Key!.Value,g => g.Select(n => n.Value).ToArray());

            var tagsLookup = await ctx.Tags
                .Where(t => t.ID != null && ids.Contains(t.ID.Value))
                .GroupBy(t => t.ID)
                .ToDictionaryAsync(g => g.Key!.Value,g => g.Select(t => t.Value).ToArray());

            var output = results.Select(r => new KusDepot.Data.Models.Media
            {
                Album = r.Album,
                Application = r.Application,
                Artist = r.Artist,
                BornOn = r.BornOn,
                ContentStreamed = r.ContentStreamed,
                DistinguishedName = r.DistinguishedName,
                FILE = r.FilePath,
                ID = r.ID,
                Modified = r.Modified,
                Name = r.Name,
                Notes = notesLookup.TryGetValue(r.ID!.Value,out var n) ? n : null,
                Size = r.Size,
                Tags = tagsLookup.TryGetValue(r.ID!.Value,out var tg) ? tg : null,
                Title = r.Title,
                Type = r.Type,
                Year = r.Year
            }).ToArray();

            SetOk(da); ETW.Log.SMSuccess(output.Length);

            return new MediaResponse { Media = output };
        }
        catch ( Exception _ )
        {
            Log.Error(_,SMFail); SetErr(da); ETW.Log.SMError(_.Message);

            return new MediaResponse();
        }
        finally { da?.Dispose(); }
    }
}