namespace DataPodServices.Catalog;

internal static class StartUp
{
    private static async Task Main()
    {
        await new CatalogHost().RunAsync(CancellationToken.None);
    }
}