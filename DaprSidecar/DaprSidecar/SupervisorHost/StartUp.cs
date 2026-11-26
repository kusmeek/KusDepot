namespace KusDepot.Dapr;

internal static class StartUp
{
    private static async Task Main()
    {
        await new SupervisorHost().RunAsync(CancellationToken.None);
    }
}