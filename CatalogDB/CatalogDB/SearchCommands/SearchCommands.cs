namespace KusDepot.Data;

internal sealed partial class CatalogDB
{
    public async Task<CommandResponse> SearchCommands(CommandQuery? search , String? traceid = null , String? spanid = null)
    {
        DiagnosticActivity? da = null;

        try
        {
            ETW.Log.SCStart(); da = StartDiagnostic(traceid,spanid);

            using var ctx = ctxfactory.Create(BuildConnectionString(GetActorID()));

            IQueryable<CatalogDb.Command> q = ctx.Commands.AsQueryable();

            if(!String.IsNullOrWhiteSpace(search?.Application))
            {
                q = q.Where(e => e.Application != null && EF.Functions.ILike(e.Application,$"%{search!.Application!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.ApplicationVersion))
            {
                q = q.Where(e => e.ApplicationVersion != null && EF.Functions.ILike(e.ApplicationVersion,$"%{search!.ApplicationVersion!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.BornOn))
            {
                q = q.Where(e => e.BornOn != null && EF.Functions.ILike(e.BornOn,$"%{search!.BornOn!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.CommandHandle))
            {
                q = q.Where(e => e.CommandHandle != null && EF.Functions.ILike(e.CommandHandle,$"%{search!.CommandHandle!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.CommandSpecifications))
            {
                q = q.Where(e => e.CommandSpecifications != null && EF.Functions.ILike(e.CommandSpecifications,$"%{search!.CommandSpecifications!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.CommandType))
            {
                q = q.Where(e => e.CommandType != null && EF.Functions.ILike(e.CommandType,$"%{search!.CommandType!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.ContentHash))
            {
                q = q.Where(e => e.ContentHash != null && EF.Functions.ILike(e.ContentHash,$"%{search!.ContentHash!}%"));
            }

            if(search?.ContentStreamed is not null)
            {
                q = q.Where(e => e.ContentStreamed == search.ContentStreamed);
            }

            if(!String.IsNullOrWhiteSpace(search?.DistinguishedName))
            {
                q = q.Where(e => e.DistinguishedName != null && EF.Functions.ILike(e.DistinguishedName,$"%{search!.DistinguishedName!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.FILE))
            {
                q = q.Where(e => e.FilePath != null && EF.Functions.ILike(e.FilePath,$"%{search!.FILE!}%"));
            }

            if(search?.ID is not null)
            {
                q = q.Where(e => e.ID == search.ID);
            }

            if(!String.IsNullOrWhiteSpace(search?.Modified))
            {
                q = q.Where(e => e.Modified != null && EF.Functions.ILike(e.Modified,$"%{search!.Modified!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Name))
            {
                q = q.Where(e => e.Name != null && EF.Functions.ILike(e.Name,$"%{search!.Name!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.ObjectType))
            {
                q = q.Where(e => e.ObjectType != null && EF.Functions.ILike(e.ObjectType,$"%{search!.ObjectType!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.ServiceVersion))
            {
                q = q.Where(e => e.ServiceVersion != null && EF.Functions.ILike(e.ServiceVersion,$"%{search!.ServiceVersion!}%"));
            }

            if(!String.IsNullOrWhiteSpace(search?.Size))
            {
                q = q.Where(e => e.Size == search.Size);
            }

            if(!String.IsNullOrWhiteSpace(search?.Version))
            {
                q = q.Where(e => e.Version != null && EF.Functions.ILike(e.Version,$"%{search!.Version!}%"));
            }

            if(search?.Notes is not null && search.Notes.Length > 0)
            {
                foreach(var note in search.Notes.Where(s => !String.IsNullOrWhiteSpace(s)))
                {
                    String n = note!; q = q.Where(e => ctx.Notes.Any(x => x.ID == e.ID && EF.Functions.ILike(x.Value,$"%{n}%")));
                }
            }

            if(search?.Tags is not null && search.Tags.Length > 0)
            {
                foreach(var tag in search.Tags.Where(s => !String.IsNullOrWhiteSpace(s)))
                {
                    String t = tag!; q = q.Where(e => ctx.Tags.Any(x => x.ID == e.ID && EF.Functions.ILike(x.Value,$"%{t}%")));
                }
            }

            var results = await q.ToListAsync();

            if(results.Count == 0) { SetOk(da); ETW.Log.SCSuccess(0); return new CommandResponse(); }

            var ids = results.Select(r => r.ID!.Value).ToArray();

            var notesLookup = await ctx.Notes
                .Where(n => n.ID != null && ids.Contains(n.ID.Value))
                .GroupBy(n => n.ID)
                .ToDictionaryAsync(g => g.Key!.Value,g => g.Select(n => n.Value).ToArray());

            var tagsLookup = await ctx.Tags
                .Where(t => t.ID != null && ids.Contains(t.ID.Value))
                .GroupBy(t => t.ID)
                .ToDictionaryAsync(g => g.Key!.Value,g => g.Select(t => t.Value).ToArray());

            var output = results.Select(r => new KusDepot.Data.Models.Command
            {
                Application = r.Application,
                ApplicationVersion = r.ApplicationVersion,
                BornOn = r.BornOn,
                CommandHandle = r.CommandHandle,
                CommandSpecifications = r.CommandSpecifications,
                CommandType = r.CommandType,
                ContentHash = r.ContentHash,
                ContentStreamed = r.ContentStreamed,
                DistinguishedName = r.DistinguishedName,
                FILE = r.FilePath,
                ID = r.ID,
                Modified = r.Modified,
                Name = r.Name,
                Notes = notesLookup.TryGetValue(r.ID!.Value,out var n) ? n : null,
                ObjectType = r.ObjectType,
                ServiceVersion = r.ServiceVersion,
                Size = r.Size,
                Tags = tagsLookup.TryGetValue(r.ID!.Value,out var tg) ? tg : null,
                Version = r.Version
            }).ToArray();

            SetOk(da); ETW.Log.SCSuccess(output.Length);

            return new CommandResponse { Commands = output };
        }
        catch ( Exception _ )
        {
            Log.Error(_,SCFail); SetErr(da); ETW.Log.SCError(_.Message);

            return new CommandResponse();
        }
        finally { da?.Dispose(); }
    }
}