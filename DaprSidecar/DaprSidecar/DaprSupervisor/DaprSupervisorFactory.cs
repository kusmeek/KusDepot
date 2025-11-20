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

                s.AddSingletonWithInterfaces(Log.Logger);
            })

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .BuildAsync<DaprSupervisor>(cancel);
    }

    private static async Task<PlacementManager> BuildPlacementManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .RegisterCommand<RestartService>("RestartService")

            .RegisterCommand<TerminateService>("TerminateService")

            .RegisterCommand<CheckServiceHealth>("CheckServiceHealth")

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .ConfigureServices((c,s) => { s.AddSingletonWithInterfaces(Log.Logger); })

            .BuildAsync<PlacementManager>(cancel);
    }

    private static async Task<SchedulerManager> BuildSchedulerManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .RegisterCommand<RestartService>("RestartService")

            .RegisterCommand<TerminateService>("TerminateService")

            .RegisterCommand<CheckServiceHealth>("CheckServiceHealth")

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .ConfigureServices((c,s) => { s.AddSingletonWithInterfaces(Log.Logger); })

            .BuildAsync<SchedulerManager>(cancel);
    }

    private static async Task<SidecarManager> BuildSidecarManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .RegisterCommand<RestartSidecar>("RestartService")

            .RegisterCommand<TerminateSidecar>("TerminateService")

            .RegisterCommand<CheckSidecarHealth>("CheckServiceHealth")

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .ConfigureServices((c,s) => { s.AddSingletonWithInterfaces(Log.Logger); })

            .BuildAsync<SidecarManager>(cancel);
    }
}