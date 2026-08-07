using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<Boolean> Remove(Descriptor? descriptor , String? traceid = null , String? spanid = null , CancellationToken cancel = default)
    {
        try
        {
            String? id = descriptor?.ID.ToString(); String ct = GetActorID();

            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(descriptor is null || descriptor.ID is null) { Logger.Error(BadArg); SetErr(__); return false; }

            cancel.ThrowIfCancellationRequested();

            await InitializeReadyDatabase(cancel); using var ctx = ctxfactory.Create(BuildConnectionString(ct));

            var existing = await ctx.Elements.FirstOrDefaultAsync(e => e.ID == descriptor.ID.Value,cancel);

            Boolean ok;

            if(existing is null) { ok = true; }

            else
            {
                ctx.Elements.Remove(existing);

                ok = (await ctx.SaveChangesAsync(cancel)) > 0;
            }

            if(ok) { Logger.Information(RemoveSuccessDescriptor,ct,descriptor); SetOk(__); return true; }

            Logger.Error(RemoveFailDescriptor,ct,descriptor); SetErr(__); return false;
        }
        catch ( Exception _ ) { Logger.Error(_,RemoveFailDescriptor,GetActorID(),descriptor); return false; }
    }

    public async Task<Boolean> RemoveID(Guid? id , String? traceid = null , String? spanid = null , CancellationToken cancel = default)
    {
        try
        {
            String? ids = id?.ToString(); String ct = GetActorID();

            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",ids);

            if(id is null) { Logger.Error(BadArg); SetErr(__); return false; }

            cancel.ThrowIfCancellationRequested();

            await InitializeReadyDatabase(cancel);

            using var ctx = ctxfactory.Create(BuildConnectionString(ct));

            var existing = await ctx.Elements.FirstOrDefaultAsync(e => e.ID == id.Value,cancel);

            Boolean ok;

            if(existing is null) { ok = true; }

            else
            {
                ctx.Elements.Remove(existing);

                ok = (await ctx.SaveChangesAsync(cancel)) > 0;
            }

            if(ok) { Logger.Information(RemoveSuccessID,ct,ids); SetOk(__); return true; }

            Logger.Error(RemoveFailID,ct,ids); SetErr(__); return false;
        }
        catch ( Exception _ ) { Logger.Error(_,RemoveFailID,GetActorID(),id?.ToString()); return false; }
    }
}