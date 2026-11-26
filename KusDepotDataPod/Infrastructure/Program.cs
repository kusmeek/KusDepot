namespace KusDepotDataPod.Infrastructure;

internal static class Program
{
    private static Task<Int32> Main() => Pulumi.Deployment.RunAsync(() =>
    {
        var config = DataPodConfiguration.Load(new Config());

        return DataPodStack.Create(config);
    });
}
