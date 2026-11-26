using Serilog.Extensions.Logging;

namespace KusDepot.Dapr;

public class DaprSupervisorFactory
{
    public static async Task<DaprSupervisor> BuildSupervisorAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .ConfigureToolServices(async (c,s,ct) =>
            {
                s.AddSingletonWithInterfaces(await BuildPlacementManagerAsync(ct));

                s.AddSingletonWithInterfaces(await BuildSchedulerManagerAsync(ct));

                s.AddSingletonWithInterfaces(await BuildSidecarManagerAsync(ct));
            })

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .UseLogger(new SerilogLoggerFactory())

            .BuildAsync<DaprSupervisor>(cancel);
    }

    private static async Task<PlacementManager> BuildPlacementManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .EnableAllCommands()

            .UseLogger(new SerilogLoggerFactory())

            .RegisterCommand<TerminateService>("TerminateService")

            .RegisterCommand<CheckServiceHealth>("CheckServiceHealth")

            .RegisterCommand(
                "RestartService",
                new ConsoleDelegator(
                    PlacementManager.BuildRestartServiceCommand(GetConfiguration()),
                    streamed:true,observer:DaprConsoleObserverFactory.Create()))

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .BuildAsync<PlacementManager>(cancel);
    }

    private static async Task<SchedulerManager> BuildSchedulerManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .EnableAllCommands()

            .UseLogger(new SerilogLoggerFactory())

            .RegisterCommand<TerminateService>("TerminateService")

            .RegisterCommand<CheckServiceHealth>("CheckServiceHealth")

            .RegisterCommand(
                "RestartService",
                new ConsoleDelegator(
                    SchedulerManager.BuildRestartServiceCommand(GetConfiguration()),
                    streamed:true,observer:DaprConsoleObserverFactory.Create()))

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .BuildAsync<SchedulerManager>(cancel);
    }

    private static async Task<SidecarManager> BuildSidecarManagerAsync(CancellationToken cancel = default)
    {
        return await ToolBuilderFactory.CreateBuilder()

            .EnableAllCommands()

            .UseLogger(new SerilogLoggerFactory())

            .RegisterCommand<TerminateSidecar>("TerminateService")

            .RegisterCommand<CheckSidecarHealth>("CheckServiceHealth")

            .RegisterCommand(
                "RestartService",
                new ConsoleDelegator(
                    SidecarManager.BuildRestartServiceCommand(GetConfiguration()),
                    streamed:true,observer:DaprConsoleObserverFactory.Create()))

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .BuildAsync<SidecarManager>(cancel);
    }

    private static IConfiguration GetConfiguration() { return new ConfigurationBuilder().AddJsonFile(ConfigFilePath).Build(); }
}