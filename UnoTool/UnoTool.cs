namespace UnoTool;

public static class MultiTool
{
    public static ITool GetTool() => ToolBuilderFactory.CreateBuilder().Build();
}