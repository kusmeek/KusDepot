namespace KusDepot.Performance.Benchmarks;

[MemoryDiagnoser]

public class ToolConstructionBenchmarks
{
    private Tool ActivateTool = null!;

    [IterationSetup(Target = nameof(Activate))]
    public void SetupActivate()
    {
        ActivateTool = new Tool();
    }

    [Benchmark(Description = "Tool Constructor")]
    public Tool Construction_Parameterless()
    {
        return new Tool();
    }

    [Benchmark(Description = "Tool Activate")]
    public async Task<Boolean> Activate()
    {
        return await ActivateTool.Activate().ConfigureAwait(false);
    }
}
