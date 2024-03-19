namespace KusDepot.Data;

internal partial class Secure
{
    private static IConfiguration? Config;

    public static void SetupConfiguration()
    {
        Config = new ConfigurationBuilder().AddXmlFile(ConfigFilePath,true,true).Build();
    }
}