using static DataPodServices.CatalogDB.CatalogDBStrings;

namespace DataPodServices.CatalogDB;

public sealed partial class CatalogDBService
{
    private ActivitySource? Trace;

    public DiagnosticActivity? StartDiagnostic(TraceContext? tracecontext = null , [CallerMemberName] String name = "")
    {
        try
        {
            if(!(Config?.GetValue<Boolean>("Tracing:Enabled") ?? false)) { return null; }

            name = String.Join(' ',"CatalogDB",name);

            DiagnosticActivity? activity = tracecontext is not null && tracecontext.TryToActivityContext(out ActivityContext parent)
                ? Trace?.StartActivity(name,ActivityKind.Server,parent)
                : Trace?.StartActivity(name,ActivityKind.Server);

            activity?.SetTag("orleans.actor.id",GetActorID());

            activity?.SetTag("catalog.name",GetActorID());

            return activity;
        }
        catch ( Exception _ ) { Logger.Error(_,StartDiagnosticFail,GetActorID(),name); return Trace?.StartActivity(name,ActivityKind.Server); }
    }

    public void SetupDiagnostics()
    {
        Trace = new ActivitySource(TraceSourceName);
    }

    public static void SetErr(DiagnosticActivity? da) { da?.SetTag("otel.status_code","ERROR"); }

    public static void SetOk(DiagnosticActivity? da) { da?.SetTag("otel.status_code","OK"); }
}