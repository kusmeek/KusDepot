namespace KusDepot.Dapr;

public class DaprSupervisorFactory
{
    public static async Task<DaprSupervisor> BuildSupervisorAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .ConfigureContainer(async (c,b) =>
            {
                b.RegisterInstance(await BuildPlacementManagerAsync(cancel)).AsImplementedInterfaces();

                b.RegisterInstance(await BuildSchedulerManagerAsync(cancel)).AsImplementedInterfaces();

                b.RegisterInstance(await BuildSidecarManagerAsync(cancel)).AsImplementedInterfaces();
            })

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .ConfigureContainer((c,b) => b.RegisterInstance(Log.Logger))

            .BuildAsync<DaprSupervisor>(cancel);
    }

    private static async Task<PlacementManager> BuildPlacementManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .RegisterCommand<RestartService>("RestartService")

            .RegisterCommand<TerminateService>("TerminateService")

            .RegisterCommand<CheckServiceHealth>("CheckServiceHealth")

            .ConfigureContainer((c,b) => b.RegisterInstance(Log.Logger))

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .BuildAsync<PlacementManager>(cancel);
    }

    private static async Task<SchedulerManager> BuildSchedulerManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .RegisterCommand<RestartService>("RestartService")

            .RegisterCommand<TerminateService>("TerminateService")

            .RegisterCommand<CheckServiceHealth>("CheckServiceHealth")

            .ConfigureContainer((c,b) => b.RegisterInstance(Log.Logger))

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .BuildAsync<SchedulerManager>(cancel);
    }

    private static async Task<SidecarManager> BuildSidecarManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .RegisterCommand<RestartSidecar>("RestartService")

            .RegisterCommand<TerminateSidecar>("TerminateService")

            .RegisterCommand<CheckSidecarHealth>("CheckServiceHealth")

            .ConfigureContainer((c,b) => b.RegisterInstance(Log.Logger))

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .BuildAsync<SidecarManager>(cancel);
    }
}