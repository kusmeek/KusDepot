namespace KusDepot.Builders;

internal static class ToolBuilderExtensions
{
    extension(IToolGenericHostBuilder builder)
    {
        internal IToolGenericHostBuilder UseOrleans(Action<ISiloBuilder> configureorleans)
        {
            try
            {
                builder.Builder.UseOrleans(configureorleans); return builder;
            }
            catch ( Exception _ ) { KusDepotLog.Error(_,BuilderUseOrleansFail); return builder; }
        }
    }

    public const String BuilderUseOrleansFail = @"ToolBuilder Use Orleans Failed.";
}