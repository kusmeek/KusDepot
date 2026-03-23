namespace KusDepot.Builders;

internal static class ToolBuilderExtensions
{
    extension(IToolWebHostBuilder builder)
    {
        internal IToolWebHostBuilder UseMcpServer(ServerCapabilities? capabilities = null,String? instructions = null)
        {
            try
            {
                builder.Builder.Services.AddMcpServer((o) => { o.Capabilities = capabilities; o.ServerInstructions = instructions ?? String.Empty; }).WithHttpTransport();

                builder.Builder.Services.AddRouting(); builder.ConfigureWebApplication((a) => { a.MapMcp(); });

                return builder;
            }
            catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseMcpServerFail); return builder; }
        }
    }

    public const String BuilderUseMcpServerFail = @"ToolBuilder Use MCP Server Failed.";
}