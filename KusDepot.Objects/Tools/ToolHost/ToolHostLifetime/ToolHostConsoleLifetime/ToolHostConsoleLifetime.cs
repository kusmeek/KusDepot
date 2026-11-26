namespace KusDepot;

/**<include file='ToolHostConsoleLifetime.xml' path='ToolHostConsoleLifetime/class[@name="ToolHostConsoleLifetime"]/main/*'/>*/
public class ToolHostConsoleLifetime : ToolHostLifetime
{
    /**<include file='ToolHostConsoleLifetime.xml' path='ToolHostConsoleLifetime/class[@name="ToolHostConsoleLifetime"]/field[@name="hostkey"]/*'/>*/
    private HostKey? hostkey;

    /**<include file='ToolHostConsoleLifetime.xml' path='ToolHostConsoleLifetime/class[@name="ToolHostConsoleLifetime"]/field[@name="Quit"]/*'/>*/
    private PosixSignalRegistration? Quit;

    /**<include file='ToolHostConsoleLifetime.xml' path='ToolHostConsoleLifetime/class[@name="ToolHostConsoleLifetime"]/field[@name="Interrupt"]/*'/>*/
    private PosixSignalRegistration? Interrupt;

    /**<include file='ToolHostConsoleLifetime.xml' path='ToolHostConsoleLifetime/class[@name="ToolHostConsoleLifetime"]/field[@name="Terminate"]/*'/>*/
    private PosixSignalRegistration? Terminate;

    /**<include file='ToolHostConsoleLifetime.xml' path='ToolHostConsoleLifetime/class[@name="ToolHostConsoleLifetime"]/constructor[@name="Constructor"]/*'/>*/
    public ToolHostConsoleLifetime()
    {
        Quit      = PosixSignalRegistration.Create(PosixSignal.SIGQUIT,StopToolHosting);
        Interrupt = PosixSignalRegistration.Create(PosixSignal.SIGINT,StopToolHosting);
        Terminate = PosixSignalRegistration.Create(PosixSignal.SIGTERM,StopToolHosting);
    }

    ///<inheritdoc/>
    public override Guid Bind(IToolHost toolhost)
    {
        HostKey key = toolhost.RequestAccess(new HostRequest(null,true)
        {
            Operations = ImmutableArray.Create(ProtectedOperation.StopHostAsync,ProtectedOperation.Dispose)
        })
            as HostKey ?? throw new SecurityException();

        try
        {
            Guid lifekey = base.Bind(toolhost); this.hostkey = key; return lifekey;
        }
        catch { key.ClearKey(); throw; }
    }

    [SuppressMessage("Callback","IDE0060")]
    private void StopToolHosting(PosixSignalContext signal)
    {
        try
        {
            this.ToolHost?.StopHostAsync(key:this.hostkey).GetAwaiter().GetResult();

            this.ToolHost?.Dispose(this.hostkey);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopToolHostingFail,this.ToolHost?.GetID()?.ToString()); }

        finally { Process.GetCurrentProcess().Kill(); }
    }

    ///<inheritdoc/>
    protected override void DisposeLifetimeResources()
    {
        this.hostkey?.ClearKey(); this.hostkey = null;

        this.Quit?.Dispose(); this.Interrupt?.Dispose(); this.Terminate?.Dispose();
    }
}