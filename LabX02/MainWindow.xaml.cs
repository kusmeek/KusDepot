namespace LabX02;

internal sealed partial class MainWindow : Window
{
    private readonly ProducerController controller;

    public MainWindow(ProducerViewModel viewModel , ProducerController controller)
    {
        this.ViewModel = viewModel;
        this.controller = controller;
        this.InitializeComponent();
        this.TryEnableDarkSystemTitleBar();
        this.controller.StateChanged += this.Controller_StateChanged;
        this.Closed += this.MainWindow_Closed;
        this.ResizeWindow(widthDip:1360,heightDip:980);
    }

    internal ProducerViewModel ViewModel { get; }

    private async void StartButton_Click(Object sender , RoutedEventArgs e)
    {
        await this.RunActionAsync(() => this.ViewModel.StartAsync()).ConfigureAwait(false);
    }

    private void PauseButton_Click(Object sender , RoutedEventArgs e)
    {
        this.RunViewAction(() => this.ViewModel.TogglePause());
    }

    private void TogglePayloadButton_Click(Object sender , RoutedEventArgs e)
    {
        this.RunViewAction(() => this.ViewModel.TogglePayloadMode());
    }

    private void RandomPauseButton_Click(Object sender , RoutedEventArgs e)
    {
        this.RunViewAction(() => this.ViewModel.ToggleRandomPause());
    }

    private async void CompleteButton_Click(Object sender , RoutedEventArgs e)
    {
        await this.RunActionAsync(() => this.ViewModel.CompleteAsync()).ConfigureAwait(false);
    }

    private async void AbortButton_Click(Object sender , RoutedEventArgs e)
    {
        await this.RunActionAsync(() => this.ViewModel.AbortAsync()).ConfigureAwait(false);
    }

    private void ResetButton_Click(Object sender , RoutedEventArgs e)
    {
        this.RunViewAction(() => this.ViewModel.ResetDisplay());
    }

    private void CloseButton_Click(Object sender , RoutedEventArgs e)
    {
        this.Close();
    }

    private void CopyItemIdButton_Click(Object sender , RoutedEventArgs e)
    {
        CopyToClipboard(this.ViewModel.ItemIdText);
    }

    private void CopySessionIdButton_Click(Object sender , RoutedEventArgs e)
    {
        CopyToClipboard(this.ViewModel.SessionIdText);
    }

    private void Controller_StateChanged(ProducerRuntimeState snapshot)
    {
        _ = this.DispatcherQueue.TryEnqueue(() => this.ViewModel.ApplySnapshot(snapshot));
    }

    private async void MainWindow_Closed(Object sender , WindowEventArgs args)
    {
        this.controller.StateChanged -= this.Controller_StateChanged;
        await this.ViewModel.StopAsync().ConfigureAwait(false);
    }

    private async Task RunActionAsync(Func<Task> action)
    {
        try
        {
            await action().ConfigureAwait(true);
        }
        catch ( Exception exception )
        {
            Log.Error(exception,"LabX02 WinUI action failed.");
            await this.ShowErrorAsync(exception.Message).ConfigureAwait(true);
        }
    }

    private void RunViewAction(Action action)
    {
        try
        {
            action();
        }
        catch ( Exception exception )
        {
            Log.Error(exception,"LabX02 WinUI action failed.");
            _ = this.ShowErrorAsync(exception.Message);
        }
    }

    private async Task ShowErrorAsync(String message)
    {
        ContentDialog dialog = new()
        {
            Title = "LabX02",
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = this.Content.XamlRoot,
        };

        _ = await dialog.ShowAsync();
    }

    private void ResizeWindow(Int32 widthDip , Int32 heightDip)
    {
        IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        Double scale = GetDpiForWindow(hwnd) / 96.0;
        this.AppWindow.Resize(new Windows.Graphics.SizeInt32((Int32)(widthDip * scale),(Int32)(heightDip * scale)));
    }

    private void TryEnableDarkSystemTitleBar()
    {
        const Int32 DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        if(hwnd == IntPtr.Zero) { return; }

        Int32 enabled = 1;
        _ = DwmSetWindowAttribute(hwnd,DWMWA_USE_IMMERSIVE_DARK_MODE,ref enabled,sizeof(Int32));
    }

    private static void CopyToClipboard(String? value)
    {
        if(String.IsNullOrWhiteSpace(value) || value == "-") { return; }

        DataPackage package = new();
        package.SetText(value);
        Clipboard.SetContent(package);
        Clipboard.Flush();
    }

    [LibraryImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static partial UInt32 GetDpiForWindow(IntPtr hWnd);

    [LibraryImport("dwmapi.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static partial Int32 DwmSetWindowAttribute(IntPtr hwnd , Int32 dwAttribute , ref Int32 pvAttribute , Int32 cbAttribute);
}
