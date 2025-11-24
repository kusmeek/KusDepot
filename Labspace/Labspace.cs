using KusDepot.AI;
using Serilog.Extensions.Logging;

namespace Labspace;

internal static class Space
{
    private static async Task Main()
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console(formatProvider:CultureInfo.InvariantCulture).CreateLogger();

        var b = ToolBuilderFactory.CreateWebHostBuilder(); McpServerPrimitiveCollection<McpServerTool> tools = new(); tools.Add(new ToolCalculator());

        b.Builder.WebHost.UseUrls("http://localhost:48086"); b.UseMcpServer(new ServerCapabilities { Tools = new ToolsCapability { ToolCollection = tools } });

        b.UseConsoleLifetime().UseLogger(new SerilogLoggerFactory()).Seal().AutoStart(); b.Builder.Logging.AddSerilog(Log.Logger);

        var t = await b.BuildWebHostAsync(); await Task.Delay(Timeout.Infinite);
    }
}