namespace KusDepot.Builders;

/**<include file='ToolHostBuilder.xml' path='ToolHostBuilder/class[@name="ToolHostBuilder"]/main/*'/>*/
public partial class ToolHostBuilder : ToolBuilder , IToolHostBuilder
{
    private Boolean Console;

    ///<inheritdoc/>
    public IToolHost BuildHost()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            IToolHostLifetime? l; Guid i = Guid.NewGuid(); if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); }

            this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l)); InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            var _ = new ToolHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration,lifeid:i,lifetime:l);

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; }

            if(Start) { ConfigureToolHost_NoSync(StartAction); } if(seal) { ConfigureToolHost_NoSync(SealAction); } configuretool(_); configuretoolhost(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public IToolHost BuildHost<TToolHost>() where TToolHost : notnull , IToolHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!Sync.Wait(SyncTime)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            IToolHostLifetime? l; Guid i = Guid.NewGuid(); if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); }

            this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l)); InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,i).WithArgument(7,l);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolHost)ob.Value;

            if(this.RegisterCommands(_) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(this.AddRegisteredTools(_) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(this.AddRegisteredServices(_) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; }

            if(Start) { ConfigureToolHost_NoSync(StartAction); } if(seal) { ConfigureToolHost_NoSync(SealAction); } configuretool(_); configuretoolhost(_); return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolHost> BuildHostAsync(CancellationToken cancel = default)
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            IToolHostLifetime? l; Guid i = Guid.NewGuid(); if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); }

            this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l)); InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            var _ = new ToolHost(accessmanager:AccessManager,services:BuildServiceProvider(),commands:null,configuration:Context!.Configuration,lifeid:i,lifetime:l);

            if(await this.RegisterCommandsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; }

            configuretool(_); configuretoolhost(_); if(Start) { await StartTool(_,cancel).ConfigureAwait(false); } if(seal) { SealTool(_); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new ToolHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    ///<inheritdoc/>
    public async Task<IToolHost> BuildHostAsync<TToolHost>(CancellationToken cancel = default) where TToolHost : notnull , IToolHost , new()
    {
        Boolean sb = false;
        try
        {
            if(!await Sync.WaitAsync(SyncTime,cancel).ConfigureAwait(false)) { throw SyncException; } sb = true;

            if(ToolBuilt) { throw new OperationFailedException(BuilderSingleFail); }

            IToolHostLifetime? l; Guid i = Guid.NewGuid(); if(Console) { l = new ToolHostConsoleLifetime(i); } else { l = new ToolHostLifetime(i); }

            this.ConfigureServices_NoSync((c,s) => s.AddSingletonWithInterfaces(l)); InitializeToolBuilderContext(); InitializeConfiguration(); ToolBuilt = true;

            var sp = BuildServiceProvider(); var ob = ObjectBuilder.Create(typeof(TToolHost)).WithArgument(0,AccessManager).WithArgument(3,sp).WithArgument(5,Context!.Configuration).WithArgument(6,i).WithArgument(7,l);

            if(ob.Build() is false || ob.Value is null) { throw new OperationFailedException(BuilderCreateInstanceFail); } var _ = (IToolHost)ob.Value;

            if(await this.RegisterCommandsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderRegisterCommandFail); }

            if(await this.AddRegisteredToolsAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredToolsFail); }

            if(await this.AddRegisteredServicesAsync(_!,cancel).ConfigureAwait(false) is false) { throw new OperationFailedException(BuilderAddRegisteredServicesFail); }

            if(Console) { var cl = l as ToolHostConsoleLifetime; cl!.ToolHost = _; cl.ManagerKey = _!.CreateManagementKey("Lifetime") as ManagerKey; }

            configuretool(_); configuretoolhost(_); if(Start) { await StartTool(_!,cancel).ConfigureAwait(false); } if(seal) { SealTool(_!); } return _;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderFail); return new TToolHost(); }

        finally { if(sb) { Sync.Release(); } }
    }

    /**<include file='ToolHostBuilder.xml' path='ToolHostBuilder/class[@name="ToolHostBuilder"]/method[@name="configuretoolhost"]/*'/>*/
    protected void configuretoolhost(IToolHost toolhost)
    {
        try
        {
            foreach(Action<ToolBuilderContext,IToolHost> a in this.ConfigureToolHostActions) { a(this.Context!,toolhost); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,BuilderConfigureToolHostFail); }
    }

    private readonly List<Action<ToolBuilderContext,IToolHost>> ConfigureToolHostActions = new();

    ///<inheritdoc/>
    public IToolHostBuilder ConfigureToolHost(Action<ToolBuilderContext,IToolHost> action)
    {
        Boolean sb = false;
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
        if(action is not null) { this.ConfigureToolHostActions.Add(action); }
    }

    ///<inheritdoc/>
    public IToolHostBuilder UseConsoleLifetime() { Console = true; return this; }
}