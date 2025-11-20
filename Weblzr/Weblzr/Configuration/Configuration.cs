namespace Weblzr;

internal sealed partial class Server
{
    private static IToolWebHostBuilder ConfigureWeblzrBuilder(IToolWebHostBuilder builder)
    {
        builder.Builder.Services.AddRazorComponents().AddInteractiveServerComponents();

        builder.Builder.Configuration.AddJsonFile(ConfigFilePath);

        builder.Builder.Services.AddSerilog(null,true,null);

        builder.ConnectWithApplication().Seal();

        SetupLogging(builder.Builder);

        ConfigureTLS(builder.Builder);

        return builder;
    }

    private static WebApplication ConfigureWeblzrServer(WebApplication server)
    {
        server.UseStaticFiles(); server.UseAntiforgery(); server.MapRazorComponents<App>().AddInteractiveServerRenderMode(); return server;
    }

    private static IDisposable? UpdateCall;

    private static void UpdateConfig(Object? o)
    {
        var z = o as Tuple<IConfigurationRoot,LoggingLevelSwitch>;

        LoggingLevelSwitch? s = z!.Item2; IConfigurationRoot? c = z!.Item1;

        if(Enum.TryParse(c?["Serilog:MinimumLevel"] ?? "Verbose",out LogEventLevel l)) { s!.MinimumLevel = l; }

        if(Boolean.TryParse(c?["KusDepotExceptions:Enabled"] ?? "False",out Boolean e)) { Settings.NoExceptions = !e; }

        UpdateCall = c?.GetReloadToken().RegisterChangeCallback(UpdateConfig,new Tuple<IConfigurationRoot,LoggingLevelSwitch>(c,s));
    }

    private static WebApplicationOptions GetWeblzrOptions()
    {
        try
        {
            String? _ = JsonDocument.Parse(File.ReadAllText(ConfigFilePath)).RootElement.GetProperty("Environment").GetString();

            if(new[]{Environments.Development,Environments.Production,Environments.Staging}.Contains(_))
            {
                return new(){ ApplicationName = ServiceName , ContentRootPath = AppContext.BaseDirectory , WebRootPath = Path.Combine(AppContext.BaseDirectory,"wwwroot") , EnvironmentName = _ };
            }

            return new(){ ApplicationName = ServiceName , ContentRootPath = AppContext.BaseDirectory , WebRootPath = Path.Combine(AppContext.BaseDirectory,"wwwroot") };
        }
        catch { return new(){ ApplicationName = ServiceName , ContentRootPath = AppContext.BaseDirectory , WebRootPath = Path.Combine(AppContext.BaseDirectory,"wwwroot") }; }
    }

    private static String ConfigFilePath => AppContext.BaseDirectory + @"\appsettings.json";
}