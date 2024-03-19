namespace TempDB;

internal partial class TempDB
{
    private static IConfiguration? Config;

    public static void SetupConfiguration()
    {
        Config = new ConfigurationBuilder().AddXmlFile(ConfigFilePath,true,true).Build();
    }
}