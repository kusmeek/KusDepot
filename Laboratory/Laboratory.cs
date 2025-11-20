using KusDepot.AI;

namespace Laboratory;

internal static class Space
{
    private static async Task Main()
    {
        var b = ToolBuilderFactory.CreateGenericHostBuilder().UseMcpServerDefaults(Assembly.GetAssembly(typeof(ConsoleCalculator)));

        b.Seal().AutoStart(); var t = await b.BuildGenericHostAsync(); await Task.Delay(Timeout.Infinite);
    }
}