namespace DataPodServices;

internal class StartUp
{
    static async Task Main()
    {
        var builder = DataPodSilo.ConfigureDataPodSilo();

        var host = await builder.BuildGenericHostAsync();

        await host.RunAsync();
    }
}