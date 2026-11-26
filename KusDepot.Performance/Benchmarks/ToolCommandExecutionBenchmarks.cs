namespace KusDepot.Performance.Benchmarks;

[MemoryDiagnoser]

public class ToolCommandExecutionBenchmarks
{
    private Tool UnlockedTool = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        UnlockedTool = new Tool();

        await UnlockedTool.RegisterCommand("NoOp" , new NoOpCommand());
        await UnlockedTool.RegisterCommand("Async" , new AsyncNoOpCommand());

        await UnlockedTool.Activate();
        await UnlockedTool.EnableAllCommands();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        UnlockedTool?.Dispose();
    }

    [Benchmark(Description = "ExecuteCommand — unlocked NoOp" , Baseline = true)]
    public Guid? ExecuteCommand_Unlocked_NoOp()
    {
        return UnlockedTool.ExecuteCommand(new() { Handle = "NoOp" });
    }

    [Benchmark(Description = "ExecuteCommandAsync — unlocked NoOp")]
    public Task<Guid?> ExecuteCommandAsync_Unlocked()
    {
        return UnlockedTool.ExecuteCommandAsync(new() { Handle = "Async" });
    }
}
