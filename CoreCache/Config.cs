namespace KusDepot.Data;

internal partial class CoreCache
{
    private static IConfiguration? Config;

    public static void SetupConfiguration()
    {
        Config = new ConfigurationBuilder().AddXmlFile(ConfigFilePath,true,true).Build();
    }
}