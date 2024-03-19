namespace View;

public partial class ToolWindowWeb : Window
{
    public ToolWindowWeb() { this.InitializeComponent(); this.Web.WebMessageReceived += this.Dispatch; }

    public void Dispatch(Object? webview , CoreWebView2WebMessageReceivedEventArgs ea) { return; }

    public void DisplayTool(Tool tool) { this.Web.NavigateToString(tool?.ToString()); }
}