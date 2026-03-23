namespace DataPodServices.DataControl;

internal static class StartUp
{
    private static async Task Main()
    {
        await new DataControlHost().RunAsync(CancellationToken.None);
    }
}