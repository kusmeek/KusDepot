namespace LabX08;

internal static class LabDemoConfigurationLoader
{
    public static LabDemoOptions Load()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json",optional:false,reloadOnChange:false)
            .Build();

        LabDemoOptions? options = configuration.GetSection("LabDemo").Get<LabDemoOptions>();

        if(options is null) { throw new InvalidOperationException("LabDemo configuration section is missing."); }

        if(String.IsNullOrWhiteSpace(options.Endpoint)) { throw new InvalidOperationException("LabDemo endpoint is required."); }

        if(String.IsNullOrWhiteSpace(options.WorkingDirectory)) { throw new InvalidOperationException("LabDemo working directory is required."); }

        Directory.CreateDirectory(options.WorkingDirectory);

        return options;
    }
}
