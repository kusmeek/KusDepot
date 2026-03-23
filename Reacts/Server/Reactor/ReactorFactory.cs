namespace KusDepot.Reacts;

public static class ReactorFactory
{
    public static IToolWebHost CreateReactor()
    {
        try
        {
            var port = Int32.TryParse(Environment.GetEnvironmentVariable("HTTP_PORT"),out var p) ? p : 8080;

            var b = ToolBuilderFactory.CreateWebHostBuilder(); b.Builder.WebHost.ConfigureKestrel(o => { o.ListenAnyIP(port); });

            b.Builder.Services.AddSerilog(Log.Logger);

            b.Builder.Services.AddSignalR( (o) =>
            {
                o.EnableDetailedErrors = false;
            });

            b.Builder.Services.AddSignalR()

                .AddJsonProtocol(o =>
                {
                    o.PayloadSerializerOptions.PropertyNamingPolicy = null;

                    o.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            b.ConfigureWebApplication(a =>
            {
                a.UseDefaultFiles(new DefaultFilesOptions {DefaultFileNames = ["Client.html"]});

                a.UseStaticFiles(); a.UseWebSockets(); a.MapHub<ReactorCore>("/reactor");
            })

            .UseConsoleLifetime().Seal();

            return b.BuildWebHost();
        }
        catch ( Exception _ ) { Log.Logger.Fatal(_,ReactorFactoryFail); return new ToolWebHost(); }
    }
}