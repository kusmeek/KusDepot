namespace KusDepotDataPod.Build;

public static class Program
{
    public static Int32 Main(String[] args)
    {
        return new CakeHost()
            .UseContext<BuildContext>()
            .Run(args);
    }
}