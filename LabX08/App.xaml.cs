namespace LabX08;

public partial class App : Application
{
    private Window? window;

    public App()
    {
        this.InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        SetupLog();

        LabDemoOptions options = LabDemoConfigurationLoader.Load();
        String token = LabTokenProvider.GetToken(options.Mode).GetAwaiter().GetResult();
        DataControlClient client = LabDataControlClientFactory.Create(options,token);
        ConsumerController controller = new(options,client);
        ConsumerViewModel viewModel = new(options,controller);

        this.window = new MainWindow(viewModel,controller);
        this.window.Activate();
    }

    private static void SetupLog()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
            .CreateLogger();
    }
}
