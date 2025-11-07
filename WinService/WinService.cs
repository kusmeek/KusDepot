namespace WinService;

public class WinService : BackgroundService
{
    private ITool? Tool;

    protected HostKey? HostKey;

    public WinService()
    {
        Tool = ITool.CreateBuilder()

            .ConfigureTool((x,t) =>
            {
                HostKey = t.RequestAccess(new HostRequest(null,true)) as HostKey;
            })

            .Seal().Build();
    }

    public override async Task StartAsync(CancellationToken cancel = default)
    {
        await base.StartAsync(cancel); await (Tool?.StartHostAsync(cancel) ?? Task.CompletedTask);
    }

    public override async Task StopAsync(CancellationToken cancel = default)
    {
        await (Tool?.StopHostAsync(cancel) ?? Task.CompletedTask); await base.StopAsync(cancel);
    }

    public override void Dispose()
    {
        Tool?.Dispose(key:HostKey); base.Dispose(); GC.SuppressFinalize(this);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.CompletedTask;
    }
}