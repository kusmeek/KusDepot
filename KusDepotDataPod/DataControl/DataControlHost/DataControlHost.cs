namespace DataPodServices.DataControl;

internal sealed partial class DataControlHost
{
    public DataControlHost() {}

    public async Task RunAsync(CancellationToken cancel)
    {
        IToolWebHost? host = null;

        try
        {
            var datacontrol = DataControlFactory.Create();

            host = await ConfigureDataControlHost(datacontrol).BuildWebHostAsync(cancel:cancel); await host.StartHostAsync(key:HostKey,cancel:cancel);

            Log.Information(DataControlStartedURL,datacontrol.URL); await Task.Delay(Timeout.Infinite,cancel);
        }
        catch ( OperationCanceledException ) { Log.Information(DataControlStopped); }

        catch ( Exception _ ) { Log.Fatal(_,DataControlFail); await Log.CloseAndFlushAsync(); throw; }

        finally { if(host is not null) { await host.StopHostAsync(CancellationToken.None,key:HostKey); await host.DisposeAsync(HostKey); } }
    }

    private static readonly ProcessTelemetryResource TelemetryResource = new(TraceServiceName);
}