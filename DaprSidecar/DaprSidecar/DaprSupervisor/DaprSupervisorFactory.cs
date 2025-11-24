

namespace KusDepot.Dapr;

public class DaprSupervisorFactory
{
    public static async Task<DaprSupervisor> BuildSupervisorAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .ConfigureServices(async (c,s) =>
            {
                s.AddSingletonWithInterfaces(await BuildPlacementManagerAsync(cancel));

                s.AddSingletonWithInterfaces(await BuildSchedulerManagerAsync(cancel));

                s.AddSingletonWithInterfaces(await BuildSidecarManagerAsync(cancel));
            })

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .UseLogger(new SerilogLoggerFactory())

            .BuildAsync<DaprSupervisor>(cancel);
    }

    private static async Task<PlacementManager> BuildPlacementManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .UseLogger(new SerilogLoggerFactory())

            .RegisterCommand<RestartService>("RestartService")

            .RegisterCommand<TerminateService>("TerminateService")

            .RegisterCommand<CheckServiceHealth>("CheckServiceHealth")

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .BuildAsync<PlacementManager>(cancel);
    }

    private static async Task<SchedulerManager> BuildSchedulerManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .UseLogger(new SerilogLoggerFactory())

            .RegisterCommand<RestartService>("RestartService")

            .RegisterCommand<TerminateService>("TerminateService")

            .RegisterCommand<CheckServiceHealth>("CheckServiceHealth")

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .BuildAsync<SchedulerManager>(cancel);
    }

    private static async Task<SidecarManager> BuildSidecarManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .UseLogger(new SerilogLoggerFactory())

            .RegisterCommand<RestartSidecar>("RestartService")

            .RegisterCommand<TerminateSidecar>("TerminateService")

            .RegisterCommand<CheckSidecarHealth>("CheckServiceHealth")

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .BuildAsync<SidecarManager>(cancel);
    }
}