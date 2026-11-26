namespace KusDepot.Builders;

/**<include file='ToolHostBuilder.xml' path='ToolHostBuilder/class[@name="ToolHostBuilder"]/main/*'/>*/
public partial class ToolHostBuilder : ToolBuilder , IToolHostBuilder
{
    /**<include file='ToolHostBuilder.xml' path='ToolHostBuilder/class[@name="ToolHostBuilder"]/field[@name="Console"]/*'/>*/
    private Boolean Console;

    /**<include file='ToolHostBuilder.xml' path='ToolHostBuilder/class[@name="ToolHostBuilder"]/field[@name="ConfigureToolHostActions"]/*'/>*/
    private readonly List<ConfigureToolHostAction> ConfigureToolHostActions = new();

    ///<inheritdoc/>
    public IToolHost BuildHost()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            CreateHostLifetime(Console); InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var _ = new ToolHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration); SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            configuretool(_); configuretoolhost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolHost BuildHost<TToolHost>() where TToolHost : notnull , IToolHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            CreateHostLifetime(Console); InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolHost)ob.Value; SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            configuretool(_); configuretoolhost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolHost BuildHost<TToolHost>(ToolHostFactory<TToolHost> factory) where TToolHost : notnull , IToolHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            CreateHostLifetime(Console); InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var _ = (IToolHost)factory(AccessManager,BuildServiceProvider(),Context!.Configuration);

            if(_ is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } SetBuildTool(_);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            configuretool(_); configuretoolhost(_); if(Start) { StartTool(_); } if(seal) { SealTool(_); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { DisposeBuildResources(); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolHost> BuildHostAsync(CancellationToken cancel = default)
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            CreateHostLifetime(Console); InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var _ = new ToolHost(accessmanager:AccessManager,services:await BuildServiceProviderAsync(cancel).ConfigureAwait(false),commands:null,configuration:Context!.Configuration); SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            await configuretoolasync(_,cancel).ConfigureAwait(false); await configuretoolhostasync(_,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_,cancel).ConfigureAwait(false); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolHost> BuildHostAsync<TToolHost>(CancellationToken cancel = default) where TToolHost : notnull , IToolHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            CreateHostLifetime(Console); InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var sp = await BuildServiceProviderAsync(cancel).ConfigureAwait(false); var ob = ObjectBuilder.Create(typeof(TToolHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolHost)ob.Value; SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            await configuretoolasync(_!,cancel).ConfigureAwait(false); await configuretoolhostasync(_!,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_!,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_!,cancel).ConfigureAwait(false); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolHost> BuildHostAsync<TToolHost>(ToolHostFactory<TToolHost> factory , CancellationToken cancel = default) where TToolHost : notnull , IToolHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            DC(); if(BuildStarted) { throw new OperationFailedException(BuilderSingleFail); }

            CreateHostLifetime(Console); InitializeToolBuilderContext(); InitializeConfiguration(); BuildStarted = true;

            var _ = (IToolHost)factory(AccessManager,await BuildServiceProviderAsync(cancel).ConfigureAwait(false),Context!.Configuration);

            if(_ is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } SetBuildTool(_);

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            await configuretoolasync(_!,cancel).ConfigureAwait(false); await configuretoolhostasync(_!,cancel).ConfigureAwait(false);

            if(Start) { await StartToolAsync(_!,cancel).ConfigureAwait(false); } if(seal) { await SealToolAsync(_!,cancel).ConfigureAwait(false); } ClearBuildResources(); return _;
        }
        catch ( Exception _ ) { if(sb) { await DisposeBuildResourcesAsync().ConfigureAwait(false); } KusDepotLog.Error(_,BuilderFail); throw; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolHostBuilder.xml' path='ToolHostBuilder/class[@name="ToolHostBuilder"]/method[@name="configuretoolhost"]/*'/>*/
    protected void configuretoolhost(IToolHost toolhost)
    {
        try
        {
            foreach(ConfigureToolHostAction a in this.ConfigureToolHostActions) { a.Execute(this.Context!,toolhost); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolHostFail); throw; }
    }

    /**<include file='ToolHostBuilder.xml' path='ToolHostBuilder/class[@name="ToolHostBuilder"]/method[@name="configuretoolhostasync"]/*'/>*/
    protected async Task configuretoolhostasync(IToolHost toolhost , CancellationToken cancel = default)
    {
        try
        {
            foreach(ConfigureToolHostAction a in this.ConfigureToolHostActions) { await a.ExecuteAsync(this.Context!,toolhost,cancel).ConfigureAwait(false); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolHostFail); throw; }
    }

    ///<inheritdoc/>
    public IToolHostBuilder ConfigureToolHost(Action<ToolBuilderContext,IToolHost> action)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureToolHost_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolHostFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolHostBuilder ConfigureToolHost(Func<ToolBuilderContext,IToolHost,CancellationToken,Task> action)
    {
        Boolean sb = false;
        DC();
        try
        {
            if(!Sync.Wait(SyncTime)) { return this; } sb = true;

            ConfigureToolHost_NoSync(action); return this;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolHostFail); return this; }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolHostBuilder.xml' path='ToolHostBuilder/class[@name="ToolHostBuilder"]/method[@name="ConfigureToolHost_NoSync"]/*'/>*/
    protected void ConfigureToolHost_NoSync(Action<ToolBuilderContext,IToolHost> action)
    {
        if(action is not null) { this.ConfigureToolHostActions.Add(new(action)); }
    }

    /**<include file='ToolHostBuilder.xml' path='ToolHostBuilder/class[@name="ToolHostBuilder"]/method[@name="ConfigureToolHostAsync_NoSync"]/*'/>*/
    protected void ConfigureToolHost_NoSync(Func<ToolBuilderContext,IToolHost,CancellationToken,Task> action)
    {
        if(action is not null) { this.ConfigureToolHostActions.Add(new(action)); }
    }

    ///<inheritdoc/>
    public IToolHostBuilder UseConsoleLifetime() { DC(); Console = true; return this; }
}