namespace KusDepot;

/**<include file='ToolHostConsoleLifetime.xml' path='ToolHostConsoleLifetime/class[@name="ToolHostConsoleLifetime"]/main/*'/>*/
public class ToolHostConsoleLifetime : ToolHostLifetime
{
    private Boolean Disposed;

    private IToolHost? toolhost;

    private ManagerKey? managerkey;

    private PosixSignalRegistration? Quit;

    private PosixSignalRegistration? Interrupt;

    private PosixSignalRegistration? Terminate;

    /**<include file='ToolHostConsoleLifetime.xml' path='ToolHostConsoleLifetime/class[@name="ToolHostConsoleLifetime"]/constructor[@name="Constructor"]/*'/>*/
    public ToolHostConsoleLifetime(Guid lifeid , IToolHost? toolhost = null) : base(lifeid)
    {
        Quit      = PosixSignalRegistration.Create(PosixSignal.SIGQUIT,StopToolHosting);
        Interrupt = PosixSignalRegistration.Create(PosixSignal.SIGINT,StopToolHosting);
        Terminate = PosixSignalRegistration.Create(PosixSignal.SIGTERM,StopToolHosting);

        this.ToolHost = toolhost;
    }

    [SuppressMessage("Callback","IDE0060")]
    private void StopToolHosting(PosixSignalContext signal)
    {
        try
        {
            var key = toolhost?.RequestAccess(new ManagementRequest(managerkey!));

            toolhost?.Dispose(key); key?.ClearKey();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,StopToolHostingFail,toolhost?.GetID()?.ToString()); }

        finally { this.managerkey?.ClearKey(); Process.GetCurrentProcess().Kill(); }
    }

    ///<inheritdoc/>
    public override void Dispose() { if(Disposed) { return; } this.managerkey?.ClearKey(); Quit?.Dispose(); Interrupt?.Dispose(); Terminate?.Dispose(); base.Dispose(); this.Disposed = true; }

    /**<include file='ToolHostConsoleLifetime.xml' path='ToolHostConsoleLifetime/class[@name="ToolHostConsoleLifetime"]/property[@name="ToolHost"]/*'/>*/
    public ManagerKey? ManagerKey { set => managerkey ??= value; }

    /**<include file='ToolHostConsoleLifetime.xml' path='ToolHostConsoleLifetime/class[@name="ToolHostConsoleLifetime"]/property[@name="ToolHost"]/*'/>*/
    public IToolHost? ToolHost { set => toolhost ??= value; }
}