namespace KusDepot.Data;

internal sealed partial class DataControlHost : StatelessService
{
    public DataControlHost(StatelessServiceContext context) : base(context)
    {
        try { this.SetupLogging(); }

        catch (Exception _ ) { Log.Fatal(_,DataControlFail); Log.CloseAndFlush(); throw; }
    }

    protected override async Task RunAsync(CancellationToken token)
    {
        IToolWebHost? host = null;

        try
        {
            var datacontrol = DataControlFactory.Create(this.Context);

            using(var _ = new FabricClient()) { await _.PropertyManager.PutPropertyAsync(this.Context.ServiceName,"EndpointLocator",datacontrol.URL,TimeSpan.MaxValue,token); }

            host = await ConfigureDataControlHost(datacontrol).BuildWebHostAsync(cancel:token); await host.StartHostAsync(key:HostKey,cancel:token);

            Log.Information(DataControlStartedURL,datacontrol.URL); await Task.Delay(Timeout.Infinite,token);
        }
        catch ( OperationCanceledException ) { Log.Information(DataControlStopped); }

        catch ( Exception _ ) { Log.Fatal(_,DataControlFail); await Log.CloseAndFlushAsync(); throw; }

        finally { if(host is not null) { await host.StopHostAsync(CancellationToken.None,key:HostKey); await host.DisposeAsync(HostKey); } }
    }

    protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners() => Array.Empty<ServiceInstanceListener>();

    protected override async Task OnCloseAsync(CancellationToken token) { Log.Information(DataControlClosing); await Log.CloseAndFlushAsync(); await base.OnCloseAsync(token); }
}