using Microsoft.Extensions.Logging;

namespace DataPodServices;

internal sealed class SiloProbeHealthServer(SiloProbeHealthState healthState) : IHostedService , IAsyncDisposable
{
    public const Int32 Port = 8081;

    public const String StartupPath = "/startup";

    public const String LivenessPath = "/live";

    public const String ReadinessPath = "/ready";

    private WebApplication? Application;

    public async Task StartAsync(CancellationToken cancel)
    {
        WebApplicationBuilder builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());

        builder.WebHost.UseKestrelCore();

        builder.Services.AddRouting();

        builder.Logging.AddConsole();

        builder.Logging.SetMinimumLevel(LogLevel.Warning);

        builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(Port));

        Application = builder.Build();

        Application.MapGet(StartupPath, () => Results.StatusCode(healthState.HasStarted ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable));

        Application.MapGet(LivenessPath, () => Results.StatusCode(healthState.IsLive ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable));

        Application.MapGet(ReadinessPath, () => Results.StatusCode(healthState.IsReady ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable));

        await Application.StartAsync(cancel);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        healthState.MarkStopping();

        if(Application is not null)
        {
            await Application.StopAsync(cancellationToken);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if(Application is not null)
        {
            await Application.DisposeAsync();

            Application = null;
        }
    }
}
