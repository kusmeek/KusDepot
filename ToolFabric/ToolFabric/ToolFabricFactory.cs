namespace KusDepot.ToolFabric;

public static class ToolFabricFactory
{
    public static async Task<ToolFabric> Create(StatefulServiceContext context)
    {
        return new(context,
            await ToolBuilderFactory.CreateBuilder()
            .UseAccessManager<TestAccessManager>()
            .RegisterCommand<CommandTest>("CommandTest")
            .EnableAllCommands().Seal().AutoStart().BuildAsync()!);
    }
}