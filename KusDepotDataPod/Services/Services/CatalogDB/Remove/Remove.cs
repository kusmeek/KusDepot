using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<Boolean> Remove(Descriptor? descriptor , String? traceid = null , String? spanid = null)
    {
        try
        {
            String? id = descriptor?.ID.ToString(); String ct = GetActorID(); ETW.Log.RemoveStart(ct,id);

            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(descriptor is null || descriptor.ID is null) { Logger.Error(BadArg); SetErr(__); ETW.Log.RemoveError(BadArg,ct,id); return false; }

            await InitializeReadyDatabase(); using var ctx = ctxfactory.Create(BuildConnectionString(ct));

            var existing = await ctx.Elements.FirstOrDefaultAsync(e => e.ID == descriptor.ID.Value, CancellationToken.None);

            Boolean ok;

            if(existing is null) { ok = true; }

            else
            {
                ctx.Elements.Remove(existing);

                ok = (await ctx.SaveChangesAsync(CancellationToken.None)) >0;
            }

            if(ok) { Logger.Information(RemoveSuccessDescriptor,ct,descriptor); SetOk(__); ETW.Log.RemoveSuccess(ct,id); return true; }

            Logger.Error(RemoveFailDescriptor,ct,descriptor); SetErr(__); ETW.Log.RemoveError(RemoveFail,ct,id); return false;
        }
        catch ( Exception _ ) { Logger.Error(_,RemoveFailDescriptor,GetActorID(),descriptor); ETW.Log.RemoveError(_.Message,GetActorID(),descriptor?.ID.ToString()); return false; }
    }

    public async Task<Boolean> RemoveID(Guid? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            String? ids = id?.ToString(); String ct = GetActorID(); ETW.Log.RemoveIDStart(ct,ids);

            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",ids);

            if(id is null) { Logger.Error(BadArg); SetErr(__); ETW.Log.RemoveIDError(BadArg,ct,ids); return false; }

            await InitializeReadyDatabase();

            using var ctx = ctxfactory.Create(BuildConnectionString(ct));

            var existing = await ctx.Elements.FirstOrDefaultAsync(e => e.ID == id.Value, CancellationToken.None);

            Boolean ok;

            if(existing is null) { ok = true; }

            else
            {
                ctx.Elements.Remove(existing);

                ok = (await ctx.SaveChangesAsync(CancellationToken.None)) >0;
            }

            if(ok) { Logger.Information(RemoveSuccessID,ct,ids); SetOk(__); ETW.Log.RemoveIDSuccess(ct,ids); return true; }

            Logger.Error(RemoveFailID,ct,ids); SetErr(__); ETW.Log.RemoveIDError(RemoveFail,ct,ids); return false;
        }
        catch ( Exception _ ) { Logger.Error(_,RemoveFailID,GetActorID(),id?.ToString()); ETW.Log.RemoveIDError(_.Message,GetActorID(),id?.ToString()); return false; }
    }
}