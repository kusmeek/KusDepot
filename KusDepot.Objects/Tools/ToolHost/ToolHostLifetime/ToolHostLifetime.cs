namespace KusDepot;

/**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/main/*'/>*/
public class ToolHostLifetime : IToolHostLifetime
{
    private readonly Guid                    LifeID;

    private readonly CancellationTokenSource StartingSource = new();

    private readonly CancellationTokenSource StartedSource  = new();

    private readonly CancellationTokenSource StoppingSource = new();

    private readonly CancellationTokenSource StoppedSource  = new();

    ///<inheritdoc/>
    public CancellationToken ApplicationStarting => StartingSource.Token;

    ///<inheritdoc/>
    public CancellationToken ApplicationStarted  => StartedSource.Token;

    ///<inheritdoc/>
    public CancellationToken ApplicationStopping => StoppingSource.Token;

    ///<inheritdoc/>
    public CancellationToken ApplicationStopped  => StoppedSource.Token;

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/constructor[@name="Constructor"]/*'/>*/
    public ToolHostLifetime(Guid lifeid) { this.LifeID = lifeid; }

    ///<inheritdoc/>
    public void NotifyStarting(Guid lifeid)
    {
        if(Equals(this.LifeID,lifeid) is false || Disposed) { return; }

        try { this.StartingSource?.Cancel(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,NotifyStartingFail,this.LifeID.ToString()); if(NoExceptions) { return; } throw; }
    }

    ///<inheritdoc/>
    public void NotifyStarted(Guid lifeid)
    {
        if(Equals(this.LifeID,lifeid) is false || Disposed) { return; }

        try { this.StartedSource?.Cancel(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,NotifyStartedFail,this.LifeID.ToString()); if(NoExceptions) { return; } throw; }
    }

    ///<inheritdoc/>
    public void NotifyStopping(Guid lifeid)
    {
        if(Equals(this.LifeID,lifeid) is false || Disposed) { return; }

        try { this.StoppingSource?.Cancel(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,NotifyStoppingFail,this.LifeID.ToString()); if(NoExceptions) { return; } throw; }
    }

    ///<inheritdoc/>
    public void NotifyStopped(Guid lifeid)
    {
        if(Equals(this.LifeID,lifeid) is false || Disposed) { return; }

        try { this.StoppedSource?.Cancel(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,NotifyStoppedFail,this.LifeID.ToString()); if(NoExceptions) { return; } throw; }
    }

    ///<inheritdoc/>
    public void StopApplication() { return; }

    ///<inheritdoc/>
    public virtual void Dispose()
    {
        try
        {
            if(Disposed) { return; }

            this.StartingSource?.Dispose(); this.StartedSource?.Dispose(); this.StoppingSource?.Dispose(); this.StoppedSource?.Dispose();

            GC.SuppressFinalize(this); this.Disposed = true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DisposeFail,this.GetType().Name,this.LifeID.ToString()); }
    }

    private Boolean Disposed;
}