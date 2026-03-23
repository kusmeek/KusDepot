using KusDepot.CatalogDb;

using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<Boolean> AddUpdate(Descriptor? descriptor , String? traceid = null , String? spanid = null)
    {
        try
        {
            String? id = descriptor?.ID.ToString(); String ct = GetActorID(); ETW.Log.AddUpdateStart(ct,id);

            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(descriptor is null) { Logger.Error(BadArg); SetErr(__); ETW.Log.AddUpdateError(BadArg,ct,id); return false; }

            await InitializeReadyDatabase(); Boolean ok = await AddUpdateCore(descriptor);

            if(ok) { Logger.Information(AddUpdateSuccess,ct,descriptor); SetOk(__); ETW.Log.AddUpdateSuccess(ct,id); return true; }

            Logger.Error(AddUpdateFailDescriptor,ct,descriptor); SetErr(__); ETW.Log.AddUpdateError(AddUpdateFailDescriptor,ct,id); return false;
        }
        catch ( Exception _ ) { Logger.Error(_,AddUpdateFailDescriptor,GetActorID(),descriptor); ETW.Log.AddUpdateError(_.Message,GetActorID(),descriptor?.ID.ToString()); return false; }
    }

    private async Task<Boolean> AddUpdateCore(Descriptor d , CancellationToken cancel = default)
    {
        if(d?.ID is null) { return false; } Guid id = d.ID.Value;

        var core = new KusDepot.CatalogDb.Element
        {
            ID                 = d.ID,
            Application        = d.Application,
            ApplicationVersion = d.ApplicationVersion,
            BornOn             = d.BornOn,
            ContentStreamed    = d.ContentStreamed,
            DataType           = d.DataType,
            DistinguishedName  = d.DistinguishedName,
            FilePath           = d.FILE,
            Modified           = d.Modified,
            Name               = d.Name,
            ObjectType         = d.ObjectType,
            ServiceVersion     = d.ServiceVersion,
            Size               = d.Size,
            Version            = d.Version
        };

        var tags     = d.Tags;
        var notes    = d.Notes;
        var services = d.Services?.Select(t => (ServiceType: t.Type , ServiceInterfaces: t.ExtendedData?.FirstOrDefault() , ServiceSpecifications: t.Specifications));
        var commands = d.Commands?.Select(t => (CommandType: t.Type , CommandHandle: t.RegisteredHandles?.FirstOrDefault() , CommandSpecifications: t.Specifications));

        using var ctx = ctxfactory.Create(BuildConnectionString(GetActorID())); await using var tx = await ctx.Database.BeginTransactionAsync(cancel);

        try
        {
            var e = await ctx.Elements.FirstOrDefaultAsync(x => x.ID == id, cancel);

            if(e is null)
            {
                e = new KusDepot.CatalogDb.Element { ID = id };
                ctx.Elements.Add(e);
            }

            e.Application        = core.Application;
            e.ApplicationVersion = core.ApplicationVersion;
            e.BornOn             = core.BornOn;
            e.ContentStreamed    = core.ContentStreamed;
            e.DataType           = core.DataType;
            e.DistinguishedName  = core.DistinguishedName;
            e.FilePath           = core.FilePath;
            e.Modified           = core.Modified;
            e.Name               = core.Name;
            e.ObjectType         = core.ObjectType;
            e.ServiceVersion     = core.ServiceVersion;
            e.Size               = core.Size;
            e.Version            = core.Version;

            ctx.Tags.RemoveRange(ctx.Tags.Where(t => t.ID == id));

            if(tags is not null)
            {
                foreach(var tag in tags)
                {
                    if(!String.IsNullOrWhiteSpace(tag)) { ctx.Tags.Add(new Tag { ID = id , Value = tag }); }
                }
            }

            ctx.Notes.RemoveRange(ctx.Notes.Where(n => n.ID == id));

            if(notes is not null)
            {
                foreach(var note in notes)
                {
                    if(!String.IsNullOrWhiteSpace(note)) { ctx.Notes.Add(new Note { ID = id , Value = note }); }
                }
            }

            ctx.Services.RemoveRange(ctx.Services.Where(s => s.ID == id));

            if(services is not null)
            {
                foreach(var svc in services)
                {
                    ctx.Services.Add(new KusDepot.CatalogDb.Service
                    {
                        ID                    = id,
                        ServiceType           = svc.ServiceType,
                        ServiceInterfaces     = svc.ServiceInterfaces,
                        ServiceSpecifications = svc.ServiceSpecifications
                    });
                }
            }

            ctx.Commands.RemoveRange(ctx.Commands.Where(c => c.ID == id));

            if(commands is not null)
            {
                foreach(var cmd in commands)
                {
                    ctx.Commands.Add(new KusDepot.CatalogDb.Command
                    {
                        ID                     = id,
                        CommandType            = cmd.CommandType,
                        CommandHandle          = cmd.CommandHandle,
                        CommandSpecifications  = cmd.CommandSpecifications
                    });
                }
            }

            ctx.MediaLibrary.RemoveRange(ctx.MediaLibrary.Where(m => m.ID == id));

            if(d.IsMultiMediaItem())
            {
                ctx.MediaLibrary.Add(new MultiMedia
                {
                    ID     = id,
                    Artist = d.Artist,
                    Title  = d.Title,
                    Year   = d.Year,
                });
            }

            await ctx.SaveChangesAsync(cancel); await tx.CommitAsync(cancel);

            return true;
        }
        catch { await tx.RollbackAsync(cancel); throw; }
    }
}