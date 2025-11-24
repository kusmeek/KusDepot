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

            config.GetChildren().ToList().ForEach(_ => SerializeSection(_,s));

            return JsonSerializer.Serialize(s);
        }
        catch { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ToolConfiguration.xml' path='ToolConfiguration/class[@name="ToolConfiguration"]/method[@name="SerializeSection"]/*'/>*/
    private static void SerializeSection(IConfigurationSection section , Dictionary<String,String> settings)
    {
        try
        {
            if(section.Value is not null) { settings[section.Path] = section.Value; }

            section.GetChildren().ToList().ForEach(child => SerializeSection(child,settings));
        }
        catch { if(NoExceptions) { return; } throw; }
    }

    /**<include file='ToolConfiguration.xml' path='ToolConfiguration/class[@name="ToolConfiguration"]/method[@name="Deserialize"]/*'/>*/
    public static IConfiguration? Deserialize(String? data)
    {
        try
        {
            if(data is null) { return null; }

            Dictionary<String,String>? s = JsonSerializer.Deserialize<Dictionary<String,String>>(data);

            ConfigurationManager _ = new(); if(s is not null) { _.AddInMemoryCollection(s!); } return _;
        }
        catch { if(NoExceptions) { return null; } throw; }
    }
}