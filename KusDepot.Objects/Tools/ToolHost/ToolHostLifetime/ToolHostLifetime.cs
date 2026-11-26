namespace KusDepot;

/**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/main/*'/>*/
public class ToolHostLifetime : IToolHostLifetime
{
    ///<inheritdoc/>
    public CancellationToken ApplicationStarting => StartingSource.Token;

    ///<inheritdoc/>
    public CancellationToken ApplicationStarted  => StartedSource.Token;

    ///<inheritdoc/>
    public CancellationToken ApplicationStopping => StoppingSource.Token;

    ///<inheritdoc/>
    public CancellationToken ApplicationStopped  => StoppedSource.Token;

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/field[@name="StartingSource"]/*'/>*/
    private readonly CancellationTokenSource StartingSource = new();

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/field[@name="StartedSource"]/*'/>*/
    private readonly CancellationTokenSource StartedSource  = new();

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/field[@name="StoppingSource"]/*'/>*/
    private readonly CancellationTokenSource StoppingSource = new();

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/field[@name="StoppedSource"]/*'/>*/
    private readonly CancellationTokenSource StoppedSource  = new();

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/field[@name="bound"]/*'/>*/
    private Boolean bound;

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/field[@name="lifekey"]/*'/>*/
    private Guid? lifekey;

    ///<inheritdoc/>
    public Guid? LifeID => this.ToolHost?.GetID();

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/property[@name="Logger"]/*'/>*/
    internal ILogger? Logger { get; set { if(value is not null) { field = value; } } }

    ///<inheritdoc/>
    public IToolHost? ToolHost { get; private set; }

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/constructor[@name="Constructor"]/*'/>*/
    public ToolHostLifetime() {}

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/method[@name="Bind"]/*'/>*/
    public virtual Guid Bind(IToolHost toolhost)
    {
        ArgumentNullException.ThrowIfNull(toolhost);

        if(toolhost.GetID() is not Guid lifeid || lifeid == Guid.Empty) { throw new InvalidOperationException(); }

        if(this.bound || this.Disposed) { throw new InvalidOperationException(); }

        this.bound = true; this.ToolHost = toolhost; this.lifekey = Guid.NewGuid(); return this.lifekey.Value;
    }

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/method[@name="IsBoundTo"]/*'/>*/
    protected Boolean IsBoundTo(Guid lifekey)
    {
        return this.bound && this.ToolHost is not null && this.lifekey == lifekey && !this.Disposed;
    }

    ///<inheritdoc/>
    public void NotifyStarting(Guid lifekey)
    {
        if(this.IsBoundTo(lifekey) is false || Disposed) { return; }

        try { this.StartingSource?.Cancel(); }

        catch ( Exception _ ) { Logger?.Error(_,NotifyStartingFail,this.LifeID?.ToString()); if(NoExceptions) { return; } throw; }
    }

    ///<inheritdoc/>
    public void NotifyStarted(Guid lifekey)
    {
        if(this.IsBoundTo(lifekey) is false || Disposed) { return; }

        try { this.StartedSource?.Cancel(); }

        catch ( Exception _ ) { Logger?.Error(_,NotifyStartedFail,this.LifeID?.ToString()); if(NoExceptions) { return; } throw; }
    }

    ///<inheritdoc/>
    public void NotifyStopping(Guid lifekey)
    {
        if(this.IsBoundTo(lifekey) is false || Disposed) { return; }

        try { this.StoppingSource?.Cancel(); }

        catch ( Exception _ ) { Logger?.Error(_,NotifyStoppingFail,this.LifeID?.ToString()); if(NoExceptions) { return; } throw; }
    }

    ///<inheritdoc/>
    public void NotifyStopped(Guid lifekey)
    {
        if(this.IsBoundTo(lifekey) is false || Disposed) { return; }

        try { this.StoppedSource?.Cancel(); }

        catch ( Exception _ ) { Logger?.Error(_,NotifyStoppedFail,this.LifeID?.ToString()); if(NoExceptions) { return; } throw; }
    }

    ///<inheritdoc/>
    public void StopApplication() { return; }

    ///<inheritdoc/>
    public virtual void Dispose(Guid lifekey)
    {
        if(!this.TryBeginDispose(lifekey)) { return; }

        try
        {
            this.DisposeLifetimeResources();

            this.StartingSource?.Dispose(); this.StartedSource?.Dispose(); this.StoppingSource?.Dispose(); this.StoppedSource?.Dispose();

            GC.SuppressFinalize(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisposeFail,this.GetType().Name,this.LifeID?.ToString()); }

        finally { this.ToolHost = null; this.lifekey = null; }
    }

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/method[@name="TryBeginDispose"]/*'/>*/
    protected Boolean TryBeginDispose(Guid lifekey)
    {
        if(!this.bound || this.lifekey != lifekey || this.Disposed) { return false; }

        this.Disposed = true; return true;
    }

    /**<include file='ToolHostLifetime.xml' path='ToolHostLifetime/class[@name="ToolHostLifetime"]/method[@name="DisposeLifetimeResources"]/*'/>*/
    protected virtual void DisposeLifetimeResources() {}

    private Boolean Disposed;
}