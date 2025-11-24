namespace KusDepot.Data;

internal sealed partial class DataConfigs
{
    private ActivitySource? Trace;

    public DiagnosticActivity? StartDiagnostic(String? traceid = null , String? spanid = null , [CallerMemberName] String name = "")
    {
        try
        {
            if(!(Config?.GetValue<Boolean>("Tracing:Enabled") ?? false)) { return null; }

            if(!String.IsNullOrEmpty(traceid) && !String.IsNullOrEmpty(spanid))
            {
                return Trace?.StartActivity(name,ActivityKind.Server,
                    new ActivityContext(ActivityTraceId.CreateFromString(traceid),
                        ActivitySpanId.CreateFromString(spanid),ActivityTraceFlags.Recorded));
            }

            return Trace?.StartActivity(name,ActivityKind.Server);
        }
        catch ( Exception _ ) { Log.Error(_,StartDiagnosticFail,name); return Trace?.StartActivity(name,ActivityKind.Server); }
    }

    public void SetupDiagnostics()
    {
        Trace = new ActivitySource(TraceSourceName,this.ActorService.Context.CodePackageActivationContext.CodePackageVersion);
    }

    public static void SetErr(DiagnosticActivity? da) { da?.SetTag("otel.status_code","ERROR"); }

    public static void SetOk(DiagnosticActivity? da) { da?.SetTag("otel.status_code","OK"); }
}