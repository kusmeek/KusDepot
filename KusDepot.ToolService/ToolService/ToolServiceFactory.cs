namespace KusDepot.ToolService;

public static class ToolServiceFactory
{
    public static async Task<ToolService> Create()
    {
        return new(await ToolBuilderFactory.CreateBuilder()
            .UseAccessManager<TestAccessManager>()
            .RegisterCommand<CommandTest>("CommandTest")
            .Seal().AutoStart().BuildAsync()!);
    }
}