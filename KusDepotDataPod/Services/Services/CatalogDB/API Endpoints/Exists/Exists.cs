using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    public async Task<Boolean?> Exists(Guid? id , TraceContext? tracecontext = null , CancellationToken cancel = default)
    {
        try
        {
            String? ids = id?.ToString(); String ct = GetActorID();

            using DiagnosticActivity? __ = StartDiagnostic(tracecontext)?.AddTag("id",ids);

            if(id is null) { Logger.Error(BadArg); SetErr(__); return null; }

            cancel.ThrowIfCancellationRequested();

            await InitializeReadyDatabase(cancel);

            using var ctx = CreateContext(ct);

            Boolean e = await ctx.Elements.AnyAsync(e => e.ID == id.Value && !e.IsPendingDelete,cancel);

            Logger.Information(ExistsSuccessID,ct,ids); SetOk(__); return e;
        }
        catch ( CatalogDBInitializationException ) { throw; }

        catch ( Exception _ ) { Logger.Error(_,ExistsFailID,GetActorID(),id?.ToString()); return null; }
    }
}