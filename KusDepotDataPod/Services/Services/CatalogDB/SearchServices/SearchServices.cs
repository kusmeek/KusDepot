using KusDepot.Data.Models;
using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<ServiceResponse> SearchServices(ServiceQuery? search , String? traceid = null , String? spanid = null)
    {
        DiagnosticActivity? da = null;

        try
        {
            ETW.Log.SSVStart(); da = StartDiagnostic(traceid,spanid);

            using var ctx = ctxfactory.Create(BuildConnectionString(GetActorID()));

            IQueryable<KusDepot.CatalogDb.Element> elements = ctx.Elements.AsQueryable();

            if(!String.IsNullOrWhiteSpace(search?.Application))
            {
                elements = elements.Where(e => e.Application != null && EF.Functions.ILike(e.Application,$"%{search!.Application!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.ApplicationVersion))
            {
                elements = elements.Where(e => e.ApplicationVersion != null && EF.Functions.ILike(e.ApplicationVersion,$"%{search!.ApplicationVersion!}%"));
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

            if(!String.IsNullOrWhiteSpace(search?.ObjectType))
            {
                elements = elements.Where(e => e.ObjectType != null && EF.Functions.ILike(e.ObjectType,$"%{search!.ObjectType!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.ServiceVersion))
            {
                elements = elements.Where(e => e.ServiceVersion != null && EF.Functions.ILike(e.ServiceVersion,$"%{search!.ServiceVersion!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Size))
            {
                elements = elements.Where(e => e.Size == search.Size);
            }

            if(!String.IsNullOrWhiteSpace(search?.Version))
            {
                elements = elements.Where(e => e.Version != null && EF.Functions.ILike(e.Version,$"%{search!.Version!}%"));
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
                from s in ctx.Services
                join e in elements on s.ID equals e.ID
                select new { Service = s, Element = e };

            if(!String.IsNullOrWhiteSpace(search?.ServiceInterfaces))
            {
                query = query.Where(x =>
                    x.Service.ServiceInterfaces != null &&
                    EF.Functions.ILike(x.Service.ServiceInterfaces,$"%{search.ServiceInterfaces!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.ServiceSpecifications))
            {
                query = query.Where(x =>
                    x.Service.ServiceSpecifications != null &&
                    EF.Functions.ILike(x.Service.ServiceSpecifications,$"%{search.ServiceSpecifications!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.ServiceType))
            {
                query = query.Where(x =>
                    x.Service.ServiceType != null &&
                    EF.Functions.ILike(x.Service.ServiceType,$"%{search.ServiceType!}%"));
            }

            var results = await query.ToListAsync();

            if(results.Count == 0) { SetOk(da); ETW.Log.SSVSuccess(0); return new ServiceResponse(); }

            var ids = results.Select(r => r.Element.ID!.Value).Distinct().ToList();

            var notesLookup = await ctx.Notes
                .Where(n => n.ID != null && ids.Contains(n.ID.Value))
                .GroupBy(n => n.ID)
                .ToDictionaryAsync(g => g.Key!.Value,g => g.Select(n => n.Value).ToArray());

            var tagsLookup = await ctx.Tags
                .Where(tg => tg.ID != null && ids.Contains(tg.ID.Value))
                .GroupBy(tg => tg.ID)
                .ToDictionaryAsync(g => g.Key!.Value,g => g.Select(tg => tg.Value).ToArray());

            var output = results.Select(r =>
            {
                var svc = r.Service;
                var e   = r.Element;
                var idv = e.ID!.Value;

                return new Service
                {
                    Application           = e.Application,
                    ApplicationVersion    = e.ApplicationVersion,
                    BornOn                = e.BornOn,
                    ContentStreamed       = e.ContentStreamed,
                    DistinguishedName     = e.DistinguishedName,
                    FILE                  = e.FilePath,
                    ID                    = e.ID,
                    Modified              = e.Modified,
                    Name                  = e.Name,
                    Notes                 = notesLookup.TryGetValue(idv,out var n) ? n : null,
                    ObjectType            = e.ObjectType,
                    ServiceInterfaces     = svc.ServiceInterfaces,
                    ServiceSpecifications = svc.ServiceSpecifications,
                    ServiceType           = svc.ServiceType,
                    ServiceVersion        = e.ServiceVersion,
                    Size                  = e.Size,
                    Tags                  = tagsLookup.TryGetValue(idv,out var tg) ? tg : null,
                    Version               = e.Version
                };
            }).ToArray();

            SetOk(da); ETW.Log.SSVSuccess(output.Length);

            return new ServiceResponse { Services = output };
        }
        catch ( Exception _ )
        {
            Logger.Error(_,SSVFail); SetErr(da); ETW.Log.SSVError(_.Message);

            return new ServiceResponse();
        }
        finally { da?.Dispose(); }
    }
}