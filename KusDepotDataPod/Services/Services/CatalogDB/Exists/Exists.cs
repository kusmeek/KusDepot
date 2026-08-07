using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<Boolean?> Exists(Descriptor? descriptor , String? traceid = null , String? spanid = null , CancellationToken cancel = default)
    {
        try
        {
            String? id = descriptor?.ID.ToString(); String ct = GetActorID();

            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            if(descriptor is null || descriptor.ID is null) { Logger.Error(BadArg); SetErr(__); return null; }

            cancel.ThrowIfCancellationRequested();

            await InitializeReadyDatabase(cancel); using var ctx = ctxfactory.Create(BuildConnectionString(ct));

            Boolean e = await ctx.Elements.AnyAsync(e => e.ID == descriptor.ID,cancel);

            Logger.Information(ExistsSuccessID,ct,id); SetOk(__); return e;
        }
        catch ( Exception _ ) { Logger.Error(_,ExistsFailDescriptor,GetActorID(),descriptor); return null; }
    }

    public async Task<Boolean?> ExistsID(Guid? id , String? traceid = null , String? spanid = null , CancellationToken cancel = default)
    {
        try
        {
            String? ids = id?.ToString(); String ct = GetActorID();

            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",ids);

            if(id is null) { Logger.Error(BadArg); SetErr(__); return null; }

            cancel.ThrowIfCancellationRequested();

            await InitializeReadyDatabase(cancel);

            using var ctx = ctxfactory.Create(BuildConnectionString(ct));

            Boolean e = await ctx.Elements.AnyAsync(e => e.ID == id.Value,cancel);

            Logger.Information(ExistsSuccessID,ct,ids); SetOk(__); return e;
        }
        catch ( Exception _ ) { Logger.Error(_,ExistsFailID,GetActorID(),id?.ToString()); return null; }
    }
}