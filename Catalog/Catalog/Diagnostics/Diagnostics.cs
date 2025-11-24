namespace KusDepot.Data;

public sealed partial class Catalog
{
    private ActivitySource? Trace;

    private DiagnosticActivity? StartDiagnostic(HttpContext hc , [CallerMemberName] String name = "")
    {
        try
        {
            if(!(hc.RequestServices.GetService<IConfiguration>()?.GetValue<Boolean>("Tracing:Enabled") ?? false)) { return null; }

            ActivityContext? ac = hc.Features.Get<IHttpActivityFeature>()?.Activity.Context;

            if(ac is null) { return Trace?.StartActivity(name,ActivityKind.Server); }

            return Trace?.StartActivity(name,ActivityKind.Server,ac.Value);
        }
        catch ( Exception _ ) { Log.Error(_,StartDiagnosticFail,name); return null; }
    }

    private void SetupDiagnostics() { Trace = new ActivitySource(TraceSourceName,this.Context.CodePackageActivationContext.CodePackageVersion); }

    private static void SetErr(DiagnosticActivity? da) { da?.SetTag("otel.status_code","ERROR"); }

    private static void SetOk(DiagnosticActivity? da) { da?.SetTag("otel.status_code","OK"); }
}