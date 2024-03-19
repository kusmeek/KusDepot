namespace TempDB;

internal partial class TempDB
{
    private static ActivitySource? Trace;

    private static TracerProvider? Tracer;

    public static void SetupDiagnostics()
    {
        Trace = new ActivitySource("KusDepot.Data.TempDB","0.0.1");

        Tracer = Sdk.CreateTracerProviderBuilder().ConfigureResource(_=>{_.Clear();}).AddSource("KusDepot.Data.TempDB").AddOtlpExporter().AddConsoleExporter().Build();
    }

    public static void SetErr(DiagnosticActivity? da) { da?.SetTag("otel.status_code","ERROR"); }

    public static void SetOk(DiagnosticActivity? da) { da?.SetTag("otel.status_code","OK"); }

    public static DiagnosticActivity? StartDiagnostic(String an , String? traceid = null , String? spanid = null)
    {
        try
        {
            if(!(Config?.GetValue<Boolean>("Trace:Enabled") ?? false)) { return null; }

            return Trace?.StartActivity(an,ActivityKind.Server,
            new ActivityContext(ActivityTraceId.CreateFromString(traceid),ActivitySpanId.CreateFromString(spanid),ActivityTraceFlags.Recorded));
        }
        catch ( Exception ) { return Trace?.StartActivity(an); }
    }
}