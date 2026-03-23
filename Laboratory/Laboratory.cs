namespace Laboratory;

internal static class Space
{
    private static async Task Main()
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console(formatProvider:CultureInfo.InvariantCulture).MinimumLevel.Verbose().CreateLogger();

        var b = ToolBuilderFactory.CreateWebHostBuilder(); b.Builder.Logging.AddSerilog(Log.Logger); b.UseMcpServer();

        b.Builder.Services.AddMcpServer().WithToolsFromAssembly(typeof(ToolPod).Assembly).WithResourcesFromAssembly(typeof(ToolPod).Assembly).WithPromptsFromAssembly(typeof(ToolPod).Assembly);

        b.Builder.WebHost.UseUrls("http://localhost:48086"); b.UseConsoleLifetime().UseLogger(new SerilogLoggerFactory()).Seal().AutoStart();

        var t = await b.BuildWebHostAsync(); await Task.Delay(Timeout.Infinite);
    }
}