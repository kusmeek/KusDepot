namespace KusDepot.Performance;

public static class Program
{
    private const String BenchmarkAllSwitch = "--benchmark-all";

    public static Int32 Main(String[] args)
    {
        if(args.Any(_ => String.Equals(_,BenchmarkAllSwitch,StringComparison.OrdinalIgnoreCase)))
        {
            String[] benchmarkargs = args.Where(_ => String.Equals(_,BenchmarkAllSwitch,StringComparison.OrdinalIgnoreCase) is false).ToArray();

            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).RunAllJoined(DefaultConfig.Instance,benchmarkargs);

            return 0;
        }

        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

        return 0;
    }
}
