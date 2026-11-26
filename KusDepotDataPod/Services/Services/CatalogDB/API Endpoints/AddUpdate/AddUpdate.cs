using KusDepot.CatalogDb;

using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<Boolean> AddUpdate(Descriptor? descriptor , Guid itemInstanceToken , TraceContext? tracecontext = null , CancellationToken cancel = default)
    {
        try
        {
            String? id = descriptor?.ID.ToString(); String ct = GetActorID();

            using DiagnosticActivity? __ = StartDiagnostic(tracecontext)?.AddTag("id",id);

            if(descriptor is null || itemInstanceToken == Guid.Empty) { Logger.Error(BadArg); SetErr(__); return false; }

            cancel.ThrowIfCancellationRequested(); await InitializeReadyDatabase(cancel);

            Boolean ok = await AddUpdateCore(descriptor,itemInstanceToken,cancel);

            if(ok) { Logger.Information(AddUpdateSuccess,ct,id); SetOk(__);return true; }

            Logger.Error(AddUpdateFailDescriptor,ct,descriptor); SetErr(__); return false;
        }
        catch ( CatalogDBInitializationException ) { throw; }

        catch ( Exception _ ) { Logger.Error(_,AddUpdateFailDescriptor,GetActorID(),descriptor); return false; }
    }

    private async Task<Boolean> AddUpdateCore(Descriptor d , Guid itemInstanceToken , CancellationToken cancel = default)
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
            Language           = d.Language,
            Modified           = d.Modified,
            Name               = d.Name,
            ObjectFilePath     = d.ObjectFILE,
            ObjectType         = d.ObjectType,
            ServiceVersion     = d.ServiceVersion,
            Size               = d.Size,
            Version            = d.Version
        };

        var tags     = d.Tags;
        var notes    = d.Notes;
        var services = d.Services?.Select(t => (ServiceType: t.Type , ServiceInterfaces: t.ExtendedData?.FirstOrDefault() , ServiceSpecifications: t.Specifications));
        var commands = d.Commands?.Select(t => (CommandType: t.Type , CommandHandle: t.RegisteredHandles?.FirstOrDefault() , CommandSpecifications: t.Specifications));

        using var ctx = CreateContext(GetActorID()); await using var tx = await ctx.Database.BeginTransactionAsync(cancel);

        try
        {
            var e = await ctx.Elements.FromSql($"SELECT * FROM \"Elements\" WHERE \"ID\" = {id} FOR UPDATE").SingleOrDefaultAsync(cancel);

            if(e is null)
            {
                e = new KusDepot.CatalogDb.Element { ID = id , ItemInstanceToken = itemInstanceToken };

                ctx.Elements.Add(e);
            }
            else if(e.ItemInstanceToken != itemInstanceToken || e.IsPendingDelete)
            {
                await tx.RollbackAsync(cancel);

                return false;
            }

            e.Application        = core.Application;
            e.ApplicationVersion = core.ApplicationVersion;
            e.BornOn             = core.BornOn;
            e.ContentStreamed    = core.ContentStreamed;
            e.DataType           = core.DataType;
            e.DistinguishedName  = core.DistinguishedName;
            e.FilePath           = core.FilePath;
            e.Language           = core.Language;
            e.Modified           = core.Modified;
            e.Name               = core.Name;
            e.ObjectFilePath     = core.ObjectFilePath;
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
                    Album  = d.Album,
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