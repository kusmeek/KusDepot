namespace KusDepot.Data;

internal sealed partial class CatalogDB
{
    public async Task<Boolean?> Exists(Descriptor? descriptor , String? traceid = null , String? spanid = null)
    {
        try
        {
            String? id = descriptor?.ID.ToString(); String ct = GetActorID(); ETW.Log.ExistsStart(ct,id);

            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",id);

            String? dt = __?.Context.TraceId.ToString(); String? ds = __?.Context.SpanId.ToString();

            if(descriptor is null || descriptor.ID is null) { Log.Error(BadArg); SetErr(__); ETW.Log.ExistsError(BadArg,ct,id); return null; }

            await InitializeReadyDatabase(); using var ctx = ctxfactory.Create(BuildConnectionString(ct));

            Boolean e = await ctx.Elements.AnyAsync(e => e.ID == descriptor.ID);

            Log.Information(ExistsSuccessID,ct,id); SetOk(__); ETW.Log.ExistsSuccess(ct,id); return e;
        }
        catch ( Exception _ ) { Log.Error(_,ExistsFailDescriptor,GetActorID(),descriptor); ETW.Log.ExistsError(_.Message,GetActorID(),descriptor?.ID.ToString()); return null; }
    }

    public async Task<Boolean?> ExistsID(Guid? id , String? traceid = null , String? spanid = null)
    {
        try
        {
            String? ids = id?.ToString(); String ct = GetActorID(); ETW.Log.ExistsIDStart(ct,ids);

            using DiagnosticActivity? __ = StartDiagnostic(traceid,spanid)?.AddTag("id",ids);

            String? dt = __?.Context.TraceId.ToString(); String? ds = __?.Context.SpanId.ToString();

            if(id is null) { Log.Error(BadArg); SetErr(__); ETW.Log.ExistsIDError(BadArg,ct,ids); return null; }

            await InitializeReadyDatabase();

            using var ctx = ctxfactory.Create(BuildConnectionString(ct));

            Boolean e = await ctx.Elements.AnyAsync(e => e.ID == id.Value);

            Log.Information(ExistsSuccessID,ct,ids); SetOk(__); ETW.Log.ExistsIDSuccess(ct,ids); return e;
        }
        catch ( Exception _ ) { Log.Error(_,ExistsFailID,GetActorID(),id?.ToString()); ETW.Log.ExistsIDError(_.Message,GetActorID(),id?.ToString()); return null; }
    }
}