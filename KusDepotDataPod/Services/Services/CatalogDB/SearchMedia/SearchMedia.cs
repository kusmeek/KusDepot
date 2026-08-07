using KusDepot.Data.Models;
using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<MediaResponse> SearchMedia(MediaQuery? search , String? traceid = null , String? spanid = null , CancellationToken cancel = default)
    {
        DiagnosticActivity? da = null;

        try
        {
            da = StartDiagnostic(traceid,spanid);

            cancel.ThrowIfCancellationRequested();

            using var ctx = ctxfactory.Create(BuildConnectionString(GetActorID()));

            IQueryable<KusDepot.CatalogDb.Element> elements = ctx.Elements.AsQueryable();

            if(!String.IsNullOrWhiteSpace(search?.Application))
            {
                elements = elements.Where(e => e.Application != null && EF.Functions.ILike(e.Application,$"%{search!.Application!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.BornOn))
            {
                elements = elements.Where(e => e.BornOn != null && EF.Functions.ILike(e.BornOn,$"%{search!.BornOn!}%"));
            }

            if(search?.ContentStreamed is not null)
            {
                elements = elements.Where(e => e.ContentStreamed == search.ContentStreamed);
            }

            if(!String.IsNullOrWhiteSpace(search?.DistinguishedName))
            {
                elements = elements.Where(e => e.DistinguishedName != null && EF.Functions.ILike(e.DistinguishedName,$"%{search!.DistinguishedName!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.FILE))
            {
                elements = elements.Where(e => e.FilePath != null && EF.Functions.ILike(e.FilePath,$"%{search!.FILE!}%"));
            }

            if(search?.ID is not null)
            {
                elements = elements.Where(e => e.ID == search.ID);
            }

            if(!String.IsNullOrWhiteSpace(search?.Modified))
            {
                elements = elements.Where(e => e.Modified != null && EF.Functions.ILike(e.Modified,$"%{search!.Modified!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Name))
            {
                elements = elements.Where(e => e.Name != null && EF.Functions.ILike(e.Name,$"%{search!.Name!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.ObjectFILE))
            {
                elements = elements.Where(e => e.ObjectFilePath != null && EF.Functions.ILike(e.ObjectFilePath,$"%{search!.ObjectFILE!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Size))
            {
                elements = elements.Where(e => e.Size == search.Size);
            }

            if(!String.IsNullOrWhiteSpace(search?.DataType))
            {
                elements = elements.Where(e => e.DataType != null && EF.Functions.ILike(e.DataType,$"%{search!.DataType!}%"));
            }

            if(search?.Notes is not null && search.Notes.Length > 0)
            {
                foreach(var note in search.Notes.Where(s => !String.IsNullOrWhiteSpace(s)))
                {
                    String n = note!;
                    elements = elements.Where(e => ctx.Notes.Any(x => x.ID == e.ID && EF.Functions.ILike(x.Value,$"%{n}%")));
                }
            }

            if(search?.Tags is not null && search.Tags.Length > 0)
            {
                foreach(var tag in search.Tags.Where(s => !String.IsNullOrWhiteSpace(s)))
                {
                    String t = tag!;
                    elements = elements.Where(e => ctx.Tags.Any(x => x.ID == e.ID && EF.Functions.ILike(x.Value,$"%{t}%")));
                }
            }

            var query =
                from m in ctx.MediaLibrary
                join e in elements on m.ID equals e.ID
                select new { Media = m, Element = e };

            if(!String.IsNullOrWhiteSpace(search?.Album))
            {
                query = query.Where(x =>
                    x.Media.Album != null &&
                    EF.Functions.ILike(x.Media.Album,$"%{search.Album!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Artist))
            {
                query = query.Where(x =>
                    x.Media.Artist != null &&
                    EF.Functions.ILike(x.Media.Artist,$"%{search.Artist!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Title))
            {
                query = query.Where(x =>
                    x.Media.Title != null &&
                    EF.Functions.ILike(x.Media.Title,$"%{search.Title!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Year))
            {
                query = query.Where(x =>
                    x.Media.Year != null &&
                    EF.Functions.ILike(x.Media.Year,$"%{search.Year!}%"));
            }

            var results = await query.ToListAsync(cancel);

            if(results.Count == 0) { SetOk(da); return new MediaResponse(); }

            var ids = results.Select(r => r.Element.ID!.Value).Distinct().ToList();

            var notesLookup = await ctx.Notes
                .Where(n => n.ID != null && ids.Contains(n.ID.Value))
                .GroupBy(n => n.ID)
                .ToDictionaryAsync(g => g.Key!.Value,g => g.Select(n => n.Value).ToArray(),cancel);

            var tagsLookup = await ctx.Tags
                .Where(t => t.ID != null && ids.Contains(t.ID.Value))
                .GroupBy(t => t.ID)
                .ToDictionaryAsync(g => g.Key!.Value,g => g.Select(t => t.Value).ToArray(),cancel);

            var output = results.Select(r =>
            {
                var m   = r.Media;
                var e   = r.Element;
                var idv = e.ID!.Value;

                return new Media
                {
                    Album             = m.Album,
                    Application       = e.Application,
                    Artist            = m.Artist,
                    BornOn            = e.BornOn,
                    ContentStreamed   = e.ContentStreamed,
                    DataType          = e.DataType,
                    DistinguishedName = e.DistinguishedName,
                    FILE              = e.FilePath,
                    ID                = e.ID,
                    Modified          = e.Modified,
                    Name              = e.Name,
                    Notes             = notesLookup.TryGetValue(idv,out var n) ? n : null,
                    ObjectFILE        = e.ObjectFilePath,
                    Size              = e.Size,
                    Tags              = tagsLookup.TryGetValue(idv,out var tg) ? tg : null,
                    Title             = m.Title,
                    Year              = m.Year
                };
            }).ToArray();

            SetOk(da);

            return new MediaResponse { Media = output };
        }
        catch ( Exception _ )
        {
            Logger.Error(_,SMFail); SetErr(da);

            return new MediaResponse();
        }
        finally { da?.Dispose(); }
    }
}