namespace DataPodServices.DataConfigs;

internal sealed class DataConfigsDiagnostics : IDisposable
{
    private readonly ActivitySource trace;
    private readonly Boolean enabled;

    public DataConfigsDiagnostics(IConfiguration configuration)
    {
        this.enabled = configuration.GetSection("DataConfiguration").GetValue<Boolean>("Tracing:Enabled");

        this.trace = new ActivitySource(DataConfigsStrings.TraceSourceName);
    }

    public DiagnosticActivity? Start(TraceContext? tracecontext , [CallerMemberName] String operation = "")
    {
        if(this.enabled is false)
        {
            return null;
        }

        String name = String.Join(' ',"DataConfigs",operation);

        try
        {
            if(tracecontext is not null && tracecontext.TryToActivityContext(out ActivityContext parent))
            {
                return this.trace.StartActivity(name,ActivityKind.Server,parent);
            }

            return this.trace.StartActivity(name,ActivityKind.Server);
        }
        catch ( Exception exception ) when(exception is ArgumentException or FormatException)
        {
            Log.Warning(exception,"Unable to use the supplied parent trace context for DataConfigs operation {Operation}.",operation);

            return this.trace.StartActivity(name,ActivityKind.Server);
        }
    }

    public static void SetErr(DiagnosticActivity? activity)
    {
        activity?.SetTag("otel.status_code","ERROR");
    }

    public static void SetOk(DiagnosticActivity? activity)
    {
        activity?.SetTag("otel.status_code","OK");
    }

    public void Dispose()
    {
        this.trace.Dispose();
    }
}
