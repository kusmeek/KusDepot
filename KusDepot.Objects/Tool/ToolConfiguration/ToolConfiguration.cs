namespace KusDepot;

/**<include file='ToolConfiguration.xml' path='ToolConfiguration/class[@name="ToolConfiguration"]/main/*'/>*/
public static class ToolConfiguration
{
    /**<include file='ToolConfiguration.xml' path='ToolConfiguration/class[@name="ToolConfiguration"]/method[@name="Serialize"]/*'/>*/
    public static String? Serialize(IConfiguration? config)
    {
        try
        {
            if(config is null) { return null; }

            Dictionary<String,String> s = new();

            foreach(IConfigurationSection section in config.GetChildren()) { SerializeSection(section,s); }

            return JsonSerializer.Serialize(s,ToolConfigurationJsonContext.Default.DictionaryStringString);
        }
        catch { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ToolConfiguration.xml' path='ToolConfiguration/class[@name="ToolConfiguration"]/method[@name="SerializeSection"]/*'/>*/
    private static void SerializeSection(IConfigurationSection section , Dictionary<String,String> settings)
    {
        try
        {
            if(section.Value is not null) { settings[section.Path] = section.Value; }

            foreach(IConfigurationSection child in section.GetChildren()) { SerializeSection(child,settings); }
        }
        catch { if(NoExceptions) { return; } throw; }
    }

    /**<include file='ToolConfiguration.xml' path='ToolConfiguration/class[@name="ToolConfiguration"]/method[@name="Deserialize"]/*'/>*/
    public static IConfiguration? Deserialize(String? data)
    {
        try
        {
            if(data is null) { return null; }

            Dictionary<String,String>? s = JsonSerializer.Deserialize(data,ToolConfigurationJsonContext.Default.DictionaryStringString);

            ConfigurationManager _ = new(); if(s is not null) { _.AddInMemoryCollection(s!); } return _;
        }
        catch { if(NoExceptions) { return null; } throw; }
    }
}

/**<include file='ToolConfiguration.xml' path='ToolConfiguration/class[@name="ToolConfigurationJsonContext"]/main/*'/>*/
[JsonSerializable(typeof(Dictionary<String,String>))]
internal sealed partial class ToolConfigurationJsonContext : JsonSerializerContext {}