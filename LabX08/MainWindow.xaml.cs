namespace LabX08;

internal sealed partial class MainWindow : Window
{
    private readonly ConsumerController controller;

    public MainWindow(ConsumerViewModel viewModel , ConsumerController controller)
    {
        this.ViewModel = viewModel;
        this.controller = controller;
        this.InitializeComponent();
        this.TryEnableDarkSystemTitleBar();
        this.controller.StateChanged += this.Controller_StateChanged;
        this.Closed += this.MainWindow_Closed;
        this.ResizeWindow(widthDip:1440,heightDip:1020);
    }

    internal ConsumerViewModel ViewModel { get; }

    private async void OpenLiveButton_Click(Object sender , RoutedEventArgs e)
    {
        await this.RunActionAsync(() => this.ViewModel.OpenLiveAsync()).ConfigureAwait(false);
    }

    private async void ResumeButton_Click(Object sender , RoutedEventArgs e)
    {
        await this.RunActionAsync(() => this.ViewModel.ResumeAsync()).ConfigureAwait(false);
    }

    private async void MaterializeButton_Click(Object sender , RoutedEventArgs e)
    {
        await this.RunActionAsync(() => this.ViewModel.MaterializeAsync()).ConfigureAwait(false);
    }

    private void ToggleOutputButton_Click(Object sender , RoutedEventArgs e)
    {
        this.RunViewAction(() => this.ViewModel.ToggleOutputMode());
    }

    private void ClearOutputButton_Click(Object sender , RoutedEventArgs e)
    {
        this.RunViewAction(() => this.ViewModel.ClearOutput());
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

    private void CopySourceSessionIdButton_Click(Object sender , RoutedEventArgs e)
    {
        CopyToClipboard(this.ViewModel.SourceSessionIdText);
    }

    private void CopyLocalSessionIdButton_Click(Object sender , RoutedEventArgs e)
    {
        CopyToClipboard(this.ViewModel.LocalSessionIdText);
    }

    private async void PasteItemIdButton_Click(Object sender , RoutedEventArgs e)
    {
        await this.PasteIntoAsync(value => this.ViewModel.ItemIdInput = value).ConfigureAwait(false);
    }

    private async void PasteSourceSessionIdButton_Click(Object sender , RoutedEventArgs e)
    {
        await this.PasteIntoAsync(value => this.ViewModel.SourceSessionIdInput = value).ConfigureAwait(false);
    }

    private async void PasteLocalSessionIdButton_Click(Object sender , RoutedEventArgs e)
    {
        await this.PasteIntoAsync(value => this.ViewModel.LocalSessionIdInput = value).ConfigureAwait(false);
    }

    private void Controller_StateChanged(ConsumerRuntimeState snapshot)
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
            Log.Error(exception,"LabX08 WinUI action failed.");
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
            Log.Error(exception,"LabX08 WinUI action failed.");
            _ = this.ShowErrorAsync(exception.Message);
        }
    }

    private async Task ShowErrorAsync(String message)
    {
        ContentDialog dialog = new()
        {
            Title = "LabX08",
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

    private async Task PasteIntoAsync(Action<String> assign)
    {
        try
        {
            DataPackageView content = Clipboard.GetContent();
            if(content.Contains(StandardDataFormats.Text) is false) { return; }

            String value = await content.GetTextAsync();
            assign(value?.Trim() ?? String.Empty);
        }
        catch ( Exception exception )
        {
            Log.Error(exception,"LabX08 clipboard paste failed.");
            await this.ShowErrorAsync(exception.Message).ConfigureAwait(true);
        }
    }

    [LibraryImport("user32.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static partial UInt32 GetDpiForWindow(IntPtr hWnd);

    [LibraryImport("dwmapi.dll")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static partial Int32 DwmSetWindowAttribute(IntPtr hwnd , Int32 dwAttribute , ref Int32 pvAttribute , Int32 cbAttribute);
}
