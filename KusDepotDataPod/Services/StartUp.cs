namespace DataPodServices;

internal class StartUp
{
    static async Task Main()
    {
        var builder = new DataPodSilo().ConfigureDataPodSilo();

        var host = await builder.BuildGenericHostAsync();

        await host.RunAsync();
    }
}