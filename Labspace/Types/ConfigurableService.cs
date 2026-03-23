namespace KusDepot.Test;

public sealed class ConfigurableService : Tool
{
    public ConfigurableService(LabConfig config) : base() { Config = config ?? new LabConfig(); }

    public LabConfig Config { get; }

    public String ConfigName => Config.Name;

    public Int32 ConfigMaxItems => Config.MaxItems;

    public Boolean ConfigVerbose => Config.Verbose;

    public override ToolDescriptor? GetToolDescriptor(AccessKey? key = null)
    {
        if(this.AccessCheck(key) is false) { return null; }

        return new ToolDescriptor
        {
            ID = GetID(),
            Type = GetType().FullName,
            Specifications = "Service configured via LabConfig reference. Exposes scalar projections ConfigName, ConfigMaxItems, ConfigVerbose and the full Config object.",
            ExtendedData = ["Role: Service", "Constructor: LabConfig (Reference)", "State: Config (LabConfig), ConfigName, ConfigMaxItems, ConfigVerbose"]
        };
    }
}
