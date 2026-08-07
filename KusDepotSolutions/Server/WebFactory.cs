namespace KusDepotSolutions;

internal static class WebFactory
{
    public static IToolWebHost CreateWeb()
    {
        try
        {
            var port = Int32.TryParse(Environment.GetEnvironmentVariable("HTTP_PORT"),out var p) ? p : 8080;

            var b = ToolBuilderFactory.CreateWebHostBuilder(); b.Builder.WebHost.ConfigureKestrel(o => { o.ListenAnyIP(port); });

            b.Builder.Services.AddSerilog(Log.Logger);

            b.Builder.Services.AddResponseCompression(o =>
            {
                o.EnableForHttps = true;
                o.Providers.Add<BrotliCompressionProvider>();
                o.Providers.Add<GzipCompressionProvider>();
            });

            b.Builder.Services.Configure<BrotliCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);
            b.Builder.Services.Configure<GzipCompressionProviderOptions>(o => o.Level = CompressionLevel.Fastest);

            b.ConfigureWebApplication(a =>
            {
                a.UseResponseCompression();

                a.UseDefaultFiles(new DefaultFilesOptions {DefaultFileNames = ["index.html"]});

                a.UseStaticFiles();
            })

            .UseConsoleLifetime().Seal();

            return b.BuildWebHost();
        }
        catch ( Exception _ ) { Log.Logger.Fatal(_,WebFactoryFail); throw; }
    }
}