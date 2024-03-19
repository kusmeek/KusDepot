namespace View;

internal static class StartUp
{
    [STAThread]
    private static void Main()
    {
        Application _0 = new Application(); ToolWindowWeb _1 = new ToolWindowWeb();

        _1.KeyDown += new KeyEventHandler(CreateTool);

        _0.MainWindow = _1;

        _0.Run(_1);
    }

    private static void CreateTool(Object window , KeyEventArgs ea)
    {
        if(ea.Key == Key.F10) { ((ToolWindowWeb)window).DisplayTool(new Tool()); }
    }
}