namespace KusDepot.Dapr;

internal sealed partial class SupervisorHost
{
    private static HostKey? HostKey;

    private static IToolHostBuilder ConfigureDaprSupervisorHost(IToolHostBuilder builder , DaprSupervisor supervisor)
    {
        builder.ConfigureServices((c,s) => s.AddHostedService((s) => supervisor));

        builder.ConfigureTool((c,t) => { HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey; })

        .UseLogger(new SerilogLoggerFactory()).Seal();

        return builder;
    }
}