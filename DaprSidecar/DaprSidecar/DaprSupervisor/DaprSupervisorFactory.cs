using Serilog;
using Serilog.Extensions.Logging;

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

            .EnableAllCommands()

            .UseLogger(new SerilogLoggerFactory())

            .RegisterCommand<TerminateService>("TerminateService")

            .RegisterCommand<CheckServiceHealth>("CheckServiceHealth")

            .RegisterCommand(
                "RestartService",
                new ConsoleDelegator(
                    PlacementManager.BuildRestartServiceCommand(GetConfiguration()),
                    streamed:true,observer:CreateConsoleObserver()))

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
                    streamed:true,observer:CreateConsoleObserver()))

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
                    streamed:true,observer:CreateConsoleObserver()))

            .ConfigureToolConfiguration((c,b) => b.AddJsonFile(ConfigFilePath))

            .BuildAsync<SidecarManager>(cancel);
    }

    private static ConsoleCommandObserver CreateConsoleObserver()
    {
        return new ConsoleCommandObserver
        {
            OnStarted = _ => { Log.Logger.Information("Process started. PID = {Pid}",_.ProcessId); return Task.CompletedTask; },

            OnStdOut  = _ => { Log.Logger.Information("{Line}",_.Text);                            return Task.CompletedTask; },

            OnStdErr  = _ => { Log.Logger.Error("{Line}",_.Text);                                  return Task.CompletedTask; },

            OnExited  = _ => { Log.Logger.Information("Process exited. Code = {Code}",_.ExitCode); return Task.CompletedTask; }
        };
    }

    private static IConfiguration GetConfiguration() { return new ConfigurationBuilder().AddJsonFile(ConfigFilePath).Build(); }
}