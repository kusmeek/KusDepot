namespace KusDepot.Data;

internal sealed partial class Secure
{
    private IConfigurationRoot? Config;

    private IDisposable? UpdateCall;

    public void SetupConfiguration()
    {
        Config = new ConfigurationBuilder().AddJsonFile(ConfigFilePath,true,true).Build();
    }

    private void UpdateConfig(Object? o)
    {
        LoggingLevelSwitch? s = o as LoggingLevelSwitch;

        UpdateCall?.Dispose(); UpdateCall = Config?.GetReloadToken().RegisterChangeCallback(UpdateConfig,s);

        if(Enum.TryParse(Config?["Serilog:MinimumLevel"] ?? "Verbose",out LogEventLevel l)) { s!.MinimumLevel = l; }

        if(Boolean.TryParse(Config?["KusDepotExceptions:Enabled"] ?? "False",out Boolean e)) { Settings.NoExceptions = !e; }
    }
}