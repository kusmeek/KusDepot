namespace WinViewsRT;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        try
        {
            InitializeComponent(); this.ExtendsContentIntoTitleBar = true; this.SetTitleBar(null);

            this.Activated += MainWindow_Activated;
        }
        catch {}
    }

    private void MainWindow_Activated(Object sender, WindowActivatedEventArgs args)
    {
        try
        {

            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            if(appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.Maximize();
            }
            this.Activated -= MainWindow_Activated;
        }
        catch {}
    }
}