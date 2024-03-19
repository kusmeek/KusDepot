namespace KusDepot.Data;

internal sealed partial class Catalog
{
    private static ActivitySource? Trace;

    public static void SetupDiagnostics() { Trace = new ActivitySource("KusDepot.Data.Catalog","0.0.1"); }

    public static void SetErr(DiagnosticActivity? da) { da?.SetTag("otel.status_code","ERROR"); }

    public static void SetOk(DiagnosticActivity? da) { da?.SetTag("otel.status_code","OK"); }

    private static DiagnosticActivity? StartDiagnostic(HttpContext hc , String an)
    {
        if(!(hc.RequestServices.GetService<IConfiguration>()?.GetValue<Boolean>("Trace:Enabled") ?? false)) { return null; }

        ActivityContext? _ = hc.Features.Get<IHttpActivityFeature>()?.Activity.Context;

        if(_ is null) { return Trace?.StartActivity(an); } else { return Trace?.StartActivity(an,ActivityKind.Server,_.Value); }
    }
}