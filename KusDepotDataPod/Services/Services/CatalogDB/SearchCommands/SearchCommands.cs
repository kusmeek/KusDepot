using KusDepot.Data.Models;
using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<CommandResponse> SearchCommands(CommandQuery? search , String? traceid = null , String? spanid = null)
    {
        DiagnosticActivity? da = null;

        try
        {
            ETW.Log.SCStart(); da = StartDiagnostic(traceid,spanid);

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
                from c in ctx.Commands
                join e in elements on c.ID equals e.ID
                select new { Command = c, Element = e };

            if(!String.IsNullOrWhiteSpace(search?.CommandHandle))
            {
                query = query.Where(x =>
                    x.Command.CommandHandle != null &&
                    EF.Functions.ILike(x.Command.CommandHandle, $"%{search.CommandHandle!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.CommandSpecifications))
            {
                query = query.Where(x =>
                    x.Command.CommandSpecifications != null &&
                    EF.Functions.ILike(x.Command.CommandSpecifications, $"%{search.CommandSpecifications!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.CommandType))
            {
                query = query.Where(x =>
                    x.Command.CommandType != null &&
                    EF.Functions.ILike(x.Command.CommandType, $"%{search.CommandType!}%"));
            }

            var results = await query.ToListAsync();

            if(results.Count == 0) { SetOk(da); ETW.Log.SCSuccess(0); return new CommandResponse(); }

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
                var cmd = r.Command;
                var e   = r.Element;
                var idv = e.ID!.Value;

                return new KusDepot.Data.Models.Command
                {
                    Application           = e.Application,
                    ApplicationVersion    = e.ApplicationVersion,
                    BornOn                = e.BornOn,
                    CommandHandle         = cmd.CommandHandle,
                    CommandSpecifications = cmd.CommandSpecifications,
                    CommandType           = cmd.CommandType,
                    ContentStreamed       = e.ContentStreamed,
                    DistinguishedName     = e.DistinguishedName,
                    FILE                  = e.FilePath,
                    ID                    = e.ID,
                    Modified              = e.Modified,
                    Name                  = e.Name,
                    Notes                 = notesLookup.TryGetValue(idv,out var n) ? n : null,
                    ObjectType            = e.ObjectType,
                    ServiceVersion        = e.ServiceVersion,
                    Size                  = e.Size,
                    Tags                  = tagsLookup.TryGetValue(idv,out var tg) ? tg : null,
                    Version               = e.Version
                };
            }).ToArray();

            SetOk(da); ETW.Log.SCSuccess(output.Length);

            return new CommandResponse { Commands = output };
        }
        catch ( Exception _ )
        {
            Logger.Error(_,SCFail); SetErr(da); ETW.Log.SCError(_.Message);

            return new CommandResponse();
        }
        finally { da?.Dispose(); }
    }
}