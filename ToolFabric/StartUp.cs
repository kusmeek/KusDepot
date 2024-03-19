namespace ToolFabric;

internal static class StartUp
{
    private static void Main()
    {
        try
        {
            ServiceRuntime.RegisterServiceAsync("ToolFabricType",context => new ToolFabric(context)).GetAwaiter().GetResult();

            Thread.Sleep(Timeout.Infinite);
        }
        catch ( Exception ) { throw; }
    }
}