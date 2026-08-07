namespace LabX02;

public partial class App : Application
{
    private Window? _window;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        LabDemoOptions options = LabDemoConfigurationLoader.Load();
        String token = LabTokenProvider.GetToken(options.Mode).GetAwaiter().GetResult();
        DataControlClient client = LabDataControlClientFactory.Create(options,token);
        ProducerController controller = new(options,client);
        ProducerViewModel viewModel = new(options,controller);

        _window = new MainWindow(viewModel,controller);
        _window.Activate();
    }
}
