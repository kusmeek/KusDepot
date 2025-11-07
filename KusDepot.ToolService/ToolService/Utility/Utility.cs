namespace KusDepot.ToolService;

public partial class ToolService
{
    public static Int32 GetPort()
    {
        try
        {
            return JsonDocument.Parse(File.ReadAllText(ConfigFilePath)).RootElement.GetProperty("Port").GetInt32();
        }
        catch { return 443; }
    }
}