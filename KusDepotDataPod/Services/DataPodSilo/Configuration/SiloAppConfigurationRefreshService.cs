using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Logging;

namespace DataPodServices;

internal sealed class SiloAppConfigurationRefreshService(IConfigurationRefresherProvider refresherProvider,
    ILogger<SiloAppConfigurationRefreshService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMinutes(1));

        do
        {
            foreach(IConfigurationRefresher refresher in refresherProvider.Refreshers)
            {
                try
                {
                    await refresher.TryRefreshAsync(stoppingToken);
                }
                catch ( Exception exception ) when(stoppingToken.IsCancellationRequested is false)
                {
                    logger.LogWarning(exception,"Failed to refresh Silo configuration from Azure App Configuration.");
                }
            }
        }
        while ( await timer.WaitForNextTickAsync(stoppingToken) );
    }
}
