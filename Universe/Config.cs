namespace KusDepot.Data;

internal partial class Universe
{
    private static IConfiguration? Config;

    public static void SetupConfiguration()
    {
        Config = new ConfigurationBuilder().AddXmlFile(ConfigFilePath,true,true).Build();
    }
}