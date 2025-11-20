namespace KusDepot.ToolFabric;

public partial class ToolFabric
{
    public static Int32 GetPort()
    {
        try
        {
            return JsonDocument.Parse(File.ReadAllText(ConfigFilePath)).RootElement.GetProperty("Port").GetInt32();
        }
        catch { return 443; }
    }

    private String GetURL() { return $"https://{this.Context.NodeContext.IPAddressOrFQDN}:{GetPort()}"; }
}