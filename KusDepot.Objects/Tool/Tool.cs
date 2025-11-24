namespace KusDepot;

/**<include file='Tool.xml' path='Tool/class[@name="Tool"]/main/*'/>*/
public partial class Tool : Common , IHostedLifecycleService , ITool
{
    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="AccessManager"]/*'/>*/
    protected IAccessManager? AccessManager;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Activities"]/*'/>*/
    protected List<Activity>? Activities;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="CommandManagerKey"]/*'/>*/
    protected ManagerKey? CommandManagerKey;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="CommandKeys"]/*'/>*/
    protected Dictionary<ICommand,AccessKey>? CommandKeys;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Commands"]/*'/>*/
    protected Dictionary<String,ICommand>? Commands;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="CommandTypes"]/*'/>*/
    protected Dictionary<String,String>? CommandTypes;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="CommandTypesMasked"]/*'/>*/
    protected Boolean CommandTypesMasked = true;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Configuration"]/*'/>*/
    protected IConfiguration? Configuration;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/property[@name="ConfigurationData"]/*'/>*/
    protected String? ConfigurationData { get => ToolConfiguration.Serialize(this.Configuration);
                                          set => this.Configuration = ToolConfiguration.Deserialize(value); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Data"]/*'/>*/
    protected HashSet<DataItem>? Data;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Disposed"]/*'/>*/
    protected Boolean Disposed;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostedServices"]/*'/>*/
    protected List<IHostedService>? HostedServices;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostedServicesMasked"]/*'/>*/
    protected Boolean HostedServicesMasked = true;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostingManagerKey"]/*'/>*/
    protected ManagerKey? HostingManagerKey;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostingKeys"]/*'/>*/
    protected Dictionary<ITool,AccessKey>? HostingKeys;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="HostingOptions"]/*'/>*/
    protected ToolHostOptions? HostingOptions;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Inputs"]/*'/>*/
    protected Queue<Object>? Inputs;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/property[@name="Instances"]/*'/>*/
    protected static Dictionary<Guid,ITool> Instances { get; }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="LifeState"]/*'/>*/
    [NotNull]
    protected LifeCycleStateMachine? LifeState;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Logger"]/*'/>*/
    protected ILogger? Logger;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="MyHostKey"]/*'/>*/
    protected AccessKey? MyHostKey;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Outputs"]/*'/>*/
    protected Dictionary<Guid,Object?>? Outputs;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="OwnerSecret"]/*'/>*/
    protected Byte[]? OwnerSecret;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="SelfAccessKey"]/*'/>*/
    protected AccessKey? SelfAccessKey;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="SelfManagerKey"]/*'/>*/
    protected ManagementKey? SelfManagerKey;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Status"]/*'/>*/
    protected Dictionary<String,Object?>? Status;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Sync"]/*'/>*/
    protected new ToolSync Sync;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="ToolServices"]/*'/>*/
    protected IServiceProvider? ToolServices;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="ToolServiceScope"]/*'/>*/
    protected AsyncServiceScope? ToolServiceScope;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="WorkingSet"]/*'/>*/
    protected ConcurrentDictionary<String,Object?>? WorkingSet;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/constructor[@name="StaticConstructor"]/*'/>*/
    static Tool() { Instances = new(); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/constructor[@name="Constructor"]/*'/>*/
    public Tool(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null,
           Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null , ILoggerFactory? logger = null)
    {
        try
        {
            this.LifeState = null!; this.Sync = null!; this.Initialize(); this.SetID(id); this.InitializeToolServices(services);

            this.ResolveLogger(logger); this.SetConfiguration(configuration); this.InitializeAccessManagement(accessmanager);

            this.RegisterCommands(commands); this.ResolveHostedServices(); this.AddDataItems(data);
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public Tool() : this(null,null,null,null,null,null,null) {}

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/finalizer[@name="Finalizer"]/*'/>*/
    ~Tool() { this.DisposeCore(false); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="AccessCheck"]/*'/>*/    
    protected Boolean AccessCheck(AccessKey? key = null , [CallerMemberName] String? operationname = null)
    {
        if(this.Locked is false) { return true; } if(key is null || String.IsNullOrEmpty(operationname)) { return false; }

        return this.AccessManager?.AccessCheck(key,operationname) ?? false;
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.Activate)]
    public virtual async Task<Boolean> Activate(CancellationToken? cancel = default , AccessKey? key = null)
    {
        Boolean CS = false;
        DC();
        try
        {
            using CancellationTokenSource __ = new();

            __.CancelAfter(SyncTime); cancel ??= __.Token;

            if(this.AccessCheck(key) is false) { return false; }

            if(this.LifeState.ActivateOK() is false) { return false; }

            await this.Sync.Commands.WaitAsync(cancel!.Value).ConfigureAwait(false); CS = true;

            if(this.Commands is null ? true : await this.Commands.Values.ToAsyncEnumerableRx().SelectAwaitRx(async _ => await _.Enable(cancel!.Value,this.CommandKeys?.GetValueOrDefault(_)).ConfigureAwait(false)).AllAsyncRx(_ => _).ConfigureAwait(false))
            { this.LifeState.ToActive(); return true; } else { if(NoExceptions||MyNoExceptions) { return false; } throw new OperationFailedException(); }
        }
        catch ( Exception _ ) { Logger?.Error(_,ActivateFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { this.LifeState.ToError(); return false; } throw; }

        finally { if(CS) { this.Sync.Commands.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AddActivity)]
    public virtual Boolean AddActivity(Activity? activity , AccessKey? key = null)
    {
        DC();
        try
        {
            if(activity is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(this.Activities is not null && this.Activities.Contains(activity)) { return true; }

            if(!TryEnter(this.Sync.Activities,SyncTime)) { throw SyncException; }

            if(this.Activities is null) { this.Activities = new(); } this.Activities.Add(activity);

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,AddActivityFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Activities)) { Exit(this.Sync.Activities); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AddDataItems)]
    public virtual Boolean AddDataItems(IEnumerable<DataItem>? data , AccessKey? key = null)
    {
        DC();
        try
        {
            if(data is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            HashSet<DataItem> _ = new(data,new IDEquality()); if(Equals(_.Count,0)) { return false; }

            if(_.Any( _ => { return _.GetID() is null || Equals(_.GetID(),Guid.Empty); })) { return false; }

            HashSet<DataItem> __ = _.Select(_=>_.Clone()!).ToHashSet(new IDEquality()); if(!Equals(_.Count,__.Count)) { return false; }

            if(!TryEnter(this.Sync.Data,SyncTime)) { throw SyncException; } if(this.Data is null) { this.Data = new(new IDEquality()); }

            if(_.Any( _ => this.Data.Any( d => Guid.Equals(d.GetID(),_.GetID())))) { return false; } this.Data.UnionWith(__);

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,AddDataItemsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Data)) { Exit(this.Sync.Data); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AddInput)]
    public virtual Boolean AddInput(Object? input , AccessKey? key = null)
    {
        DC();
        try
        {
            if(input is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!TryEnter(this.Sync.Inputs,SyncTime)) { throw SyncException; }

            if(this.Inputs is null) { this.Inputs = new(); } this.Inputs.Enqueue(input);

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,AddInputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Inputs)) { Exit(this.Sync.Inputs); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AddInstance)]
    public Boolean AddInstance(AccessKey? key = null) { if(this.AccessCheck(key) is false) { return false; } return AddInstance(this); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="AddInstance"]/*'/>*/
    protected static Boolean AddInstance(Tool? tool)
    {
        try
        {
            if( tool is null || tool.GetID().HasValue is false ) { return false; }

            if(!TryEnter(Instances,SyncTime)) { throw SyncException; }

            if(Instances.TryAdd(tool.GetID()!.Value,tool)) { return true; }

            return false;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,AddInstanceFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Instances)) { Exit(Instances); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AddOutput)]
    public virtual Boolean AddOutput(Guid? id , Object? output , AccessKey? key = null)
    {
        Boolean OS = false;
        DC();
        try
        {
            if( id is null || Equals(id,Guid.Empty) ) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!this.Sync.Outputs.Wait(SyncTime)) { throw SyncException; } else { OS = true; }

            if(this.Outputs is null) { this.Outputs = new(); }

            if(this.Outputs.TryAdd(id.Value,output) is false) { return false; }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,AddOutputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(OS) { this.Sync.Outputs.Release(); } }
    }

    ///<inheritdoc/>
    public override Boolean CheckManager(ManagementKey? key)
    {
        DC();
        try
        {
            if( this.Secrets is null || Equals(this.Secrets.Count,0) ) { return false; }

            if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            return this.Secrets.Any(_ => CheckSecret(key?.Key,_));
        }
        catch ( Exception _ ) { Logger?.Error(_,CheckManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } }
    }

    ///<inheritdoc/>
    public Boolean CheckOwner(ManagementKey? managementkey)
    {
        DC();
        try
        {
            if(this.OwnerSecret is null) { return false; }

            if(!TryEnter(this.Sync.OwnerSecret,SyncTime)) { throw SyncException; }

            return CheckSecret(managementkey?.Key,this.OwnerSecret);
        }
        catch ( Exception _ ) { Logger?.Error(_,CheckOwnershipFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.OwnerSecret)) { Exit(this.Sync.OwnerSecret); } }
    }

    ///<inheritdoc/>
    public override Int32 CompareTo(ICommon? other) { return other is Tool t ? this.CompareTo(t) : base.CompareTo(other); }

    ///<inheritdoc/>
    public override Int32 CompareTo(Common? other) { return other is Tool t ? this.CompareTo(t) : base.CompareTo(other); }

    ///<inheritdoc/>
    public virtual Int32 CompareTo(ITool? other) { return this.CompareTo(other as Tool); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="CompareTo"]/*'/>*/
    public virtual Int32 CompareTo(Tool? other)
    {
        try
        {
            if(ReferenceEquals(this,other))          { return 0; }

            Guid? _ = other?.GetID();

            if(this.ID is null     && _ is null)     { return 0; }
            if(this.ID is not null && _ is null)     { return 1; }
            if(this.ID is null     && _ is not null) { return -1; }

            return this.ID!.Value.CompareTo(_!.Value);
        }
        catch ( Exception _ ) { Logger?.Error(_,CompareToFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return Int32.MinValue; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.CreateManagementKey)]
    public virtual ManagementKey? CreateManagementKey(String? subject = null , AccessKey? key = null)
    {
        DC(); if(this.AccessCheck(key) is false) { return null; }

        try { var _ = new ManagerKey(CreateCertificate(this,subject)!); if(this.RegisterManager(_,key)) { return _; } return null; }

        catch ( Exception _ ) { Logger?.Error(_,CreateManagementKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override ManagementKey? CreateManagementKey(String? subject) { return this.CreateManagementKey(subject,null); }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.CreateOwnerKey)]
    public virtual OwnerKey? CreateOwnerKey(String? subject = null , AccessKey? key = null)
    {
        DC(); if(this.AccessCheck(key) is false) { return null; }

        try { var _ = new OwnerKey(CreateCertificate(this,subject)!); if(this.TakeOwnership(_)) { return _; } return null; }

        catch ( Exception _ ) { Logger?.Error(_,CreateOwnerKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.Deactivate)]
    public virtual async Task<Boolean> Deactivate(CancellationToken? cancel = default , AccessKey? key = null)
    {
        Boolean CS = false;
        DC();
        try
        {
            using CancellationTokenSource __ = new();

            __.CancelAfter(SyncTime); cancel ??= __.Token;

            if(this.AccessCheck(key) is false) { return false; }

            if(this.LifeState.DeactivateOK() is false) { return false; }

            await this.Sync.Commands.WaitAsync(cancel!.Value).ConfigureAwait(false); CS = true;

            if(this.Commands is null ? true : await this.Commands.Values.ToAsyncEnumerableRx().SelectAwaitRx(async _ => await _.Disable(cancel!.Value,this.CommandKeys?.GetValueOrDefault(_)).ConfigureAwait(false)).AllAsyncRx(_ => _).ConfigureAwait(false))
            { this.LifeState.ToInActive(); return true; } else { if(NoExceptions||MyNoExceptions) { return false; } throw new OperationFailedException(); }
        }
        catch ( Exception _ ) { Logger?.Error(_,DeactivateFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { this.LifeState.ToError(); return false; } throw; }

        finally { if(CS) { this.Sync?.Commands.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.KusDepotExceptions)]
    public Boolean DisableKusDepotExceptions(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            NoExceptions = true; return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisableKusDepotExceptionsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.MyExceptions)]
    public virtual Boolean DisableMyExceptions(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            this.MyNoExceptions = true; return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisableMyExceptionsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean DisableMyExceptions() { return this.DisableMyExceptions(null); }

    ///<inheritdoc/>
    protected override Boolean DestroySecrets(ManagementKey? managementkey)
    {
        try
        {
            if(this.CheckManager(managementkey) is false) { return false; }

            if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            if(!TryEnter(this.Sync.OwnerSecret,SyncTime)) { throw SyncException; }

            this.AccessManager?.DestroySecrets(this.SelfAccessKey);

            if(this.HostingKeys is not null)
            {
                foreach(var k in this.HostingKeys.Values) { k.ClearKey(); } this.HostingKeys.Clear();
            }

            this.HostingManagerKey?.ClearKey();

            if(this.CommandKeys is not null)
            {
                foreach(var k in this.CommandKeys.Values) { k.ClearKey(); } this.CommandKeys.Clear();
            }

            this.CommandManagerKey?.ClearKey();

            this.MyHostKey?.ClearKey(); this.MyHostKey = null;

            this.SelfAccessKey?.ClearKey(); this.SelfAccessKey = null;

            this.SelfManagerKey?.ClearKey(); this.SelfManagerKey = null;

            if(this.OwnerSecret is not null) { ZeroMemory(this.OwnerSecret); this.OwnerSecret = null; }

            base.DestroySecrets(managementkey);

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DestroySecretsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.OwnerSecret)) { Exit(this.Sync.OwnerSecret); } if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DisposedCheck"]/*'/>*/
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    protected void DC() { ObjectDisposedException.ThrowIf(this.Disposed,this); }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.KusDepotExceptions)]
    public Boolean EnableKusDepotExceptions(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            NoExceptions = false; return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,EnableKusDepotExceptionsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="EnableMyExceptions"]/*'/>*/
    [AccessCheck(ProtectedOperation.MyExceptions)]
    public virtual Boolean EnableMyExceptions(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            this.MyNoExceptions = false; return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,EnableMyExceptionsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean EnableMyExceptions() { return this.EnableMyExceptions(null); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="op_Equality"]/*'/>*/
    public static Boolean operator ==(Tool? a , Tool? b) { return a is null ? b is null : a.Equals(b); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="op_Inequality"]/*'/>*/
    public static Boolean operator !=(Tool? a , Tool? b) { return !(a == b); }

    ///<inheritdoc/>
    public override Boolean Equals(ICommon? other) { return this.Equals(other as Tool); }

    ///<inheritdoc/>
    public override Boolean Equals(Object? other) { return this.Equals(other as Tool); }

    ///<inheritdoc/>
    public override Boolean Equals(Common? other) { return this.Equals(other as Tool); }

    ///<inheritdoc/>
    public virtual Boolean Equals(ITool? other) { return this.Equals(other as Tool); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Equals"]/*'/>*/
    public virtual Boolean Equals(Tool? other)
    {
        try
        {
            if(other is null) { return false; }

            if(ReferenceEquals(this,other)) { return true; }

            return Guid.Equals(this.ID,other.GetID());
        }
        catch ( Exception _ ) { Logger?.Error(_,EqualsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.ExecuteCommand)]
    public virtual Guid? ExecuteCommand(CommandDetails? details = null , AccessKey? key = null)
    {
        Boolean CS = false;
        DC();
        try
        {
            if(this.GetLifeCycleState() is not Active) { return null; }

            if(details is null || this.Commands is null || details.MakeReady(this) is false) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(details.GetArgument("Logger") is null) { details.SetLogger(this.GetLogger()); }

            if(!this.Sync.Commands.Wait(SyncTime)) { throw SyncException; } else { CS = true; }

            if(this.Commands.TryGetValue(details.Handle!,out ICommand? cmd) is false) { return null; }

            var ck = this.CommandKeys!.GetValueOrDefault(cmd); this.Sync.Commands.Release(); CS = false;

            if(cmd is Command c && c.ExecutionMode.AllowSynchronous is false) { return null; }

            var a = Activity.CreateActivity(details); if(a is null) { return null; }

            return cmd.Execute(a,ck);
        }
        catch ( Exception _ ) { Logger?.Error(_,ExecuteCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(CS) { this.Sync.Commands.Release(); } }
    }

    ///<inheritdoc/>
    public virtual Guid? ExecuteCommandCab(KusDepotCab? cab = null , AccessKey? key = null)
    {
        try { return this.ExecuteCommand(cab?.GetCommandDetails(),key); }

        catch ( Exception _ ) { Logger?.Error(_,ExecuteCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.ExecuteCommand)]
    public virtual async Task<Guid?> ExecuteCommandAsync(CommandDetails? details = null , CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean CS = false;
        DC();
        try
        {
            if(this.GetLifeCycleState() is not Active) { return null; }

            if(details is null || this.Commands is null || details.MakeReady(this) is false) { return null; }

            if(this.AccessCheck(key) is false) { return null; } cancel ??= CancellationToken.None;

            if(details.GetArgument("Logger") is null) { details.SetLogger(this.GetLogger()); }

            await this.Sync.Commands.WaitAsync(cancel.Value).ConfigureAwait(false); CS = true;

            if(this.Commands.TryGetValue(details.Handle!,out ICommand? cmd) is false) { return null; }

            var ck = this.CommandKeys!.GetValueOrDefault(cmd); this.Sync.Commands.Release(); CS = false;

            if(cmd is Command c && c.ExecutionMode.AllowAsynchronous is false) { return null; }

            Activity? a = Activity.CreateActivity(details); if(a is null) { return null; }

            if(a.Cancel is null && cancel.HasValue)
            {
                if(details.GetArgument("Cancel") is null) { details.SetArgument("Cancel",cancel.Value); }

                TimeSpan? timeout = details.GetArgument<TimeSpan?>("Timeout") ?? DefaultCommandTimeout;

                var cts = CancellationTokenSource.CreateLinkedTokenSource(cancel.Value);

                cts.CancelAfter(timeout!.Value); a.Cancel = cts;
            }

            return await cmd.ExecuteAsync(a,ck).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,ExecuteCommandAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(CS) { this.Sync.Commands.Release(); } }
    }

    ///<inheritdoc/>
    public virtual Task<Guid?> ExecuteCommandCabAsync(KusDepotCab? cab = null , CancellationToken? cancel = null , AccessKey? key = null)
    {
        try { return this.ExecuteCommandAsync(cab?.GetCommandDetails(),cancel,key); }

        catch ( Exception _ ) { Logger?.Error(_,ExecuteCommandCabAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return Task.FromResult<Guid?>(null); } throw; }
    }

    ///<inheritdoc/>
    public override Boolean MyExceptionsEnabled() { DC(); return this.ExceptionsEnabled; }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetAccessManager)]
    public virtual IAccessManager? GetAccessManager(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.AccessManager; }

        catch ( Exception _ ) { Logger?.Error(_,GetAccessManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetActivities)]
    public virtual IList<Activity>? GetActivities(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.Activities is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!TryEnter(this.Sync.Activities,SyncTime)) { throw SyncException; }

            return this.Activities.ToList();
        }
        catch ( Exception _ ) { Logger?.Error(_,GetActivitiesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Activities)) { Exit(this.Sync.Activities); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetCommands)]
    public virtual Dictionary<String,ICommand>? GetCommands(AccessKey? key = null)
    {
        Boolean CS = false;
        DC();
        try
        {
            if(this.Commands is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.Commands.Wait(SyncTime)) { throw SyncException; } else { CS = true; }

            return this.Commands.ToDictionary(_=>new String(_.Key),_=>_.Value);
        }
        catch ( Exception _ ) { Logger?.Error(_,GetCommandsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(CS) { this.Sync.Commands.Release(); } }
    }

    ///<inheritdoc/>
    public virtual Dictionary<String,String>? GetCommandTypes()
    {
        DC();
        try
        {
            if(this.CommandTypes is null || this.CommandTypesMasked is true) { return null; }

            if(!TryEnter(this.Sync.CommandTypes,SyncTime)) { throw SyncException; }

            return this.CommandTypes.ToDictionary(_=>new String(_.Key),_=>new String(_.Value));
        }
        catch ( Exception _ ) { Logger?.Error(_,GetCommandTypesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.CommandTypes)) { Exit(this.Sync.CommandTypes); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetConfiguration)]
    public virtual IConfiguration? GetConfiguration(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.Configuration is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            return ToolConfiguration.Deserialize(ToolConfiguration.Serialize(this.Configuration));
        }
        catch ( Exception _ ) { Logger?.Error(_,GetConfigurationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetDataDescriptors)]
    public virtual IList<Descriptor>? GetDataDescriptors(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.Data is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!TryEnter(this.Sync.Data,SyncTime)) { throw SyncException; }

            List<Descriptor> _ = this.Data.Select(_=>_.GetDescriptor()!).ToList();

            if(Equals(this.Data.Count,_.Count) is false) { return null; } return _;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetDataDescriptorsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Data)) { Exit(this.Sync.Data); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetDataItem)]
    public virtual DataItem? GetDataItem(Guid? id , AccessKey? key = null)
    {
        DC();
        try
        {
            if( this.Data is null || id is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!TryEnter(this.Sync.Data,SyncTime)) { throw SyncException; }

            return this.Data.FirstOrDefault(_=>_.GetID().Equals(id))?.Clone();
        }
        catch ( Exception _ ) { Logger?.Error(_,GetDataItemFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Data)) { Exit(this.Sync.Data); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetDataItems)]
    public virtual HashSet<DataItem>? GetDataItems(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.Data is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!TryEnter(this.Sync.Data,SyncTime)) { throw SyncException; }

            HashSet<DataItem> _ = this.Data.Select(_=>_.Clone()!).ToHashSet(new IDEquality());

            if(Equals(this.Data.Count,_.Count) is false) { return null; } return _;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetDataItemsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Data)) { Exit(this.Sync.Data); } }
    }

    ///<inheritdoc/>
    public Boolean GetDisposed()
    {
        try { return this.Disposed; }

        catch ( Exception _ ) { Logger?.Error(_,GetDisposedFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public override Int32 GetHashCode()
    {
        DC(); try { return HashCode.Combine(this.ID); }

        catch ( Exception _ ) { Logger?.Error(_,GetHashCodeFail,MyTypeName,MyID); throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetHost"]/*'/>*/
    public static ITool? GetHost(IHostedService? service)
    {
        return GetInstanceIDs()?.Select(_ => GetInstance(_)).FirstOrDefault(_ => _?.IsHosting(service) ?? false);
    }

    ///<inheritdoc/>
    public LifeCycleState GetLifeCycleState()
    {
        DC(); try { return this.LifeState.State; }

        catch ( Exception _ ) { Logger?.Error(_,GetLifeCycleStateFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return UnInitialized; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetLogger"]/*'/>*/
    protected virtual ILogger? GetLogger()
    {
        DC(); try { return this.Logger; }

        catch ( Exception _ ) { Logger?.Error(_,GetLoggerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetHostedServices)]
    public virtual IList<IHostedService>? GetHostedServices(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return null; }

            if(!TryEnter(this.Sync.HostedServices,SyncTime)) { throw SyncException; }

            if(this.HostedServices is not null && this.HostedServices.Any(s => s is ITool t && t.GetDisposed()))
            {
                this.HostedServices = this.HostedServices.Where(s => s is not ITool t || t.GetDisposed() is false).ToList();
            }

            return this.HostedServices?.ToImmutableList();
        }
        catch ( Exception _ ) { Logger?.Error(_,GetHostedServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.HostedServices)) { Exit(this.Sync.HostedServices); } }
    }

    ///<inheritdoc/>
    public override Guid? GetID()
    {
        DC(); try { return this.ID; }

        catch ( Exception _ ) { Logger?.Error(_,GetIDFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetInput)]
    public virtual Object? GetInput(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.Inputs is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!TryEnter(this.Sync.Inputs,SyncTime)) { throw SyncException; }

            if(this.Inputs.TryDequeue(out Object? i)) { if(Equals(this.Inputs.Count,0)) { this.Inputs = null; } }

            return i;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetInputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Inputs)) { Exit(this.Sync.Inputs); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetInput)]
    public virtual Queue<Object>? GetInputs(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.Inputs is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!TryEnter(this.Sync.Inputs,SyncTime)) { throw SyncException; }

            return new(this.Inputs);
        }
        catch ( Exception _ ) { Logger?.Error(_,GetInputsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Inputs)) { Exit(this.Sync.Inputs); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetInstance"]/*'/>*/
    public static ITool? GetInstance(Guid? id)
    {
        try
        {
            if( id is null || id.HasValue is false ) { return null; }

            if(!TryEnter(Instances,SyncTime)) { throw SyncException; }

            Instances.Where(_ => _.Value.GetDisposed() is true).ToList().ForEach(_ => Instances.Remove(_.Key));

            return Instances.GetValueOrDefault(id!.Value);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetInstanceFail); if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(Instances)) { Exit(Instances); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetInstanceIDs"]/*'/>*/
    public static IList<Guid>? GetInstanceIDs()
    {
        try
        {
            if(Equals(Instances.Count,0)) { return null; }

            if(!TryEnter(Instances,SyncTime)) { throw SyncException; }

            Instances.Where(_ => _.Value.GetDisposed() is true).ToList().ForEach(_ => Instances.Remove(_.Key));

            return Instances.Keys.ToList();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetInstanceIDsFail); if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(Instances)) { Exit(Instances); } }
    }

    ///<inheritdoc/>
    public override Boolean GetLocked()
    {
        DC(); try { return this.Locked; }

        catch ( Exception _ ) { Logger?.Error(_,GetLockedFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetOutput)]
    public virtual Object? GetOutput(Guid? id , AccessKey? key = null)
    {
        Boolean OS = false;
        DC();
        try
        {
            if(this.Outputs is null || id is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.Outputs.Wait(SyncTime)) { throw SyncException; } else { OS = true; }

            return GetOutput_NoSync(id);
        }
        catch ( Exception _ ) { Logger?.Error(_,GetOutputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(OS) { this.Sync.Outputs.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetOutput_NoSync"]/*'/>*/
    protected Object? GetOutput_NoSync(Guid? id)
    {
        DC();
        try
        {
            if(this.Outputs is null || id is null) { return null; }

            return this.Outputs.TryGetValue(id.Value,out Object? o) is true ? o : null;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetOutputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetOutput)]
    public virtual async Task<Object?> GetOutputAsync(Guid? id , CancellationToken? cancel = null , TimeSpan? timeout = null , Int32? interval = null , AccessKey? key = null)
    {
        DC(); Boolean OS = false;

        CancellationTokenSource? s = null;

        try
        {
            if(id is null) { return null; }

            cancel ??= new CancellationToken();

            if(this.AccessCheck(key) is false) { return null; }

            s = new CancellationTokenSource(timeout ?? DefaultGetOutputTimeout);

            using CancellationTokenSource l = CancellationTokenSource.CreateLinkedTokenSource(cancel!.Value,s.Token);

            while(l.IsCancellationRequested is false)
            {
                try { await this.Sync.Outputs.WaitAsync(l.Token).ConfigureAwait(false); OS = true; }

                catch ( OperationCanceledException ) { await Task.Delay(interval.GetValueOrDefault(DefaultGetOutputInterval),l.Token).ConfigureAwait(false); continue; }

                if(this.GetOutputIDs_NoSync()?.Contains(id.Value) is true) { return this.GetOutput_NoSync(id); }

                if(OS) { this.Sync.Outputs.Release(); OS = false; }

                await Task.Delay(interval.GetValueOrDefault(DefaultGetOutputInterval),l.Token).ConfigureAwait(false);
            }

            return null;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetOutputAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(OS) { this.Sync.Outputs.Release(); } s?.Dispose(); }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetOutputIDs)]
    public virtual IList<Guid>? GetOutputIDs(AccessKey? key = null)
    {
        Boolean OS = false;
        DC();
        try
        {
            if(this.Outputs is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.Outputs.Wait(SyncTime)) { throw SyncException; } else { OS = true; }

            return GetOutputIDs_NoSync();
        }
        catch ( Exception _ ) { Logger?.Error(_,GetOutputIDsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(OS) { this.Sync.Outputs.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetOutputIDs_NoSync"]/*'/>*/
    protected List<Guid>? GetOutputIDs_NoSync()
    {
        DC();
        try
        {
            if(this.Outputs is null) { return null; }

            return this.Outputs.Keys.ToList();
        }
        catch ( Exception _ ) { Logger?.Error(_,GetOutputIDsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetRemoveOutput)]
    public virtual Object? GetRemoveOutput(Guid? id , AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; }

        Object? _ = this.GetOutput(id,key); if(_ is not null) { this.RemoveOutput(id,key); } return _; }

        catch ( Exception _ ) { Logger?.Error(_,GetRemoveOutputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetRemoveOutput)]
    public virtual async Task<Object?> GetRemoveOutputAsync(Guid? id , CancellationToken? cancel = null , TimeSpan? timeout = null , Int32? interval = null , AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; }

        Object? _ = await this.GetOutputAsync(id,cancel,timeout,interval,key).ConfigureAwait(false); if(_ is not null) { this.RemoveOutput(id,key); } return _; }

        catch ( Exception _ ) { Logger?.Error(_,GetRemoveOutputAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetStatus)]
    public virtual Dictionary<String,Object?>? GetStatus(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.Status is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!TryEnter(this.Sync.Status,SyncTime)) { throw SyncException; }

            return this.Status.ToDictionary(_=>new String(_.Key),_=>_.Value);
        }
        catch ( Exception _ ) { Logger?.Error(_,GetStatusFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Status)) { Exit(this.Sync.Status); } }
    }

    ///<inheritdoc/>
    protected override SyncNode GetSyncNode()
    {
        try { this.Sync ??= new(); return this.Sync; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetSyncNodeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null!; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetToolHost"]/*'/>*/
    public static IToolHost? GetToolHost(IHostedService? service)
    {
        return GetInstanceIDs()?.Select(_ => GetInstance(_)).FirstOrDefault(_ => _?.IsHosting(service) ?? false) as IToolHost;
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetToolServices)]
    public virtual IServiceProvider? GetToolServices(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.ToolServices; }

        catch ( Exception _ ) { Logger?.Error(_,GetToolServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetToolServiceScope)]
    public virtual IServiceScope? GetToolServiceScope(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } return this.ToolServiceScope; }

        catch ( Exception _ ) { Logger?.Error(_,GetToolServiceScopeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetWorkingSet)]
    public virtual ConcurrentDictionary<String,Object?>? GetWorkingSet(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return null; } this.WorkingSet ??= new(); return this.WorkingSet; }

        catch ( Exception _ ) { Logger?.Error(_,GetWorkingSetFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.Initialize)]
    public virtual Boolean Initialize(AccessKey? key = null)
    {
        DC(); if( (this.LifeState?.InitializeOK() ?? true) is false || this.AccessCheck(key) is false) { return false; }

        try
        {
            this.GetSyncNode(); base.Initialize(); this.LifeState = new(this.ID);

            if(this.ID is null || this.LifeState is null) { throw new InitializationException(); }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,InitializeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean Initialize() { return this.Initialize(null); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InitializeAccessManagement"]/*'/>*/
    protected virtual Boolean InitializeAccessManagement(IAccessManager? accessmanager = null)
    {
        DC();
        try
        {
            this.AccessManager ??= accessmanager?.Initialize(this,this.GetLogger()) ?? false ? accessmanager : new AccessManager(this,this.GetLogger());

            this.SelfManagerKey = this.CreateManagementKey(this.GetID()!.ToString()) as ManagerKey;

            this.SelfAccessKey = this.AccessManager.RequestAccess(new ServiceRequest(this,this.GetID()!.ToString()!));

            return this.AccessManager is not null && this.SelfAccessKey is not null && this.SelfManagerKey is not null ? true : throw new SecurityException(InitializeAccessManagementFail);
        }

        catch ( Exception _ ) { Logger?.Error(_,InitializeAccessManagementFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="InitializeToolServices"]/*'/>*/
    protected virtual Boolean InitializeToolServices(ToolServiceProvider? services = null)
    {
        DC();
        try
        {
            if(services is null) { return true; }

            this.ToolServiceScope = services.ServiceProvider.CreateAsyncScope();

            this.ToolServices = this.ToolServiceScope.Value.ServiceProvider;

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,InitializeToolServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean IsHosting(IHostedService? serviceinstance , ITool? caller = null)
    {
        DC();
        try
        {
            if(serviceinstance is null) { return false; }

            if(this.HostedServicesMasked is true) { if(this.GetHostedServices(this.SelfAccessKey)?.Any(_ => Object.ReferenceEquals(_,caller)) ?? false) {} else { return false; } }

            return this.GetHostedServices(this.SelfAccessKey)?.Any(_ => Object.ReferenceEquals(_,serviceinstance)) ?? false;
        }
        catch ( Exception _ ) { Logger?.Error(_,IsHostingFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean IsHosting(Type? servicetype , ITool? caller = null)
    {
        DC();
        try
        {
            if(servicetype is null) { return false; }

            if(this.HostedServicesMasked is true) { if(this.GetHostedServices(this.SelfAccessKey)?.Any(_ => Object.ReferenceEquals(_,caller)) ?? false) {} else { return false; } }

            return this.GetHostedServices(this.SelfAccessKey)?.Any(_ => servicetype.IsAssignableFrom(_.GetType())) ?? false;
        }
        catch ( Exception _ ) { Logger?.Error(_,IsHostingFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean IsHosting<TService>() { return this.IsHosting(typeof(TService)); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="IssueMyHostKeys"]/*'/>*/
    protected virtual Boolean IssueMyHostKeys()
    {
        DC();
        try
        {
            IEnumerable<ITool>? s = this.GetHostedServices(this.SelfAccessKey)?.OfType<ITool>(); if(s is null || Equals(s.Count(),0)) { return true; }

            foreach(ITool t in s)
            {
                if(t.SetMyHostKey(this.AccessManager!.GenerateHostedServiceKey(t,this.SelfAccessKey)) is false) { throw new OperationFailedException(); }
            }
            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,IssueMyHostKeysFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public Boolean KusDepotExceptionsEnabled() { DC(); return !NoExceptions; }

    ///<inheritdoc/>
    public override Boolean Lock(ManagementKey? key)
    {
        Byte[]? s = default;
        DC();
        try
        {
            if(this.Locked) { return false; } s = CreateSecret(key?.Key); if(s is null) { return false; }

            if(!TryEnter(this.Sync.Locked,SyncTime)) { throw SyncException; } if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            this.Secrets ??= new(); this.Secrets.Add(s); if(this.Secrets.Contains(s)) { this.Locked = true; return true; } return false;
        }
        catch ( Exception _ )
        {
            if(s is not null) { this.Secrets?.Remove(s); }

            this.Locked = false; Logger?.Error(_,LockFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw;
        }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } if(IsEntered(this.Sync.Locked)) { Exit(this.Sync.Locked); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="LockHostedServices"]/*'/>*/
    protected virtual Boolean LockHostedServices()
    {
        DC();
        try
        {
            IEnumerable<ITool>? s = this.GetHostedServices(this.SelfAccessKey)?.OfType<ITool>(); if(s is null || Equals(s.Count(),0)) { return true; }

            this.HostingManagerKey = this.CreateManagementKey("HostingManagerKey") as ManagerKey;

            return s.All(tool => tool.Lock(this.HostingManagerKey)) ? true : throw new SecurityException(LockHostedServicesFail);
        }
        catch ( Exception _ ) { Logger?.Error(_,LockHostedServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.MaskCommandTypes)]
    public Boolean MaskCommandTypes(Boolean mask = true , AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            this.CommandTypesMasked = mask; return Equals(this.CommandTypesMasked,mask);
        }
        catch ( Exception _ ) { Logger?.Error(_,mask?MaskCommandTypesFail:UnMaskCommandTypesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.MaskHostedServices)]
    public Boolean MaskHostedServices(Boolean mask = true , AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            this.HostedServicesMasked = mask; return Equals(this.HostedServicesMasked,mask);
        }
        catch ( Exception _ ) { Logger?.Error(_,mask?MaskHostedServicesFail:UnMaskHostedServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RegisterCommand)]
    public virtual async Task<Boolean> RegisterCommand(String? handle , ICommand? command , Boolean dynamic = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean CS = false;
        DC();
        try
        {
            if( command is null || handle is null ) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            cancel ??= CancellationToken.None;

            await this.Sync.Commands.WaitAsync(cancel.Value).ConfigureAwait(false); CS = true;

            if(!TryEnter(this.Sync.CommandTypes,SyncTime)) { throw SyncException; }

            if( (this.Commands?.ContainsKey(handle) ?? false ) || (this.CommandTypes?.ContainsKey(handle) ?? false) ) { return false; }

            if(this.Commands is null) { this.Commands = new(); } if(this.CommandTypes is null) { this.CommandTypes = new(); } if(this.CommandKeys is null) { this.CommandKeys = new(); }

            CommandKey? k = this.CommandKeys!.GetValueOrDefault(command) as CommandKey; if(k is null) { k = this.AccessManager!.GenerateCommandKey(command,key); if( k is null || this.CommandKeys!.TryAdd(command,k) is false) { return false; } }

            if( command.Attach(this,k) && ( dynamic ? await command.Enable(cancel.Value,key:k).ConfigureAwait(false) : true ) && ( command.GetLocked() ? true : command.Lock(this.CommandManagerKey) ) && this.Commands.TryAdd(new(handle),command) && this.CommandTypes.TryAdd(new(handle),command.GetType().FullName!) )
            {
                return true;
            }

            command.UnLock(this.HostingManagerKey); if(dynamic) { await command.Disable(cancel.Value,key:k).ConfigureAwait(false); } command.Detach(k); this.Commands.Remove(handle); if(Equals(this.Commands.Count,0)) { this.Commands = null; }

            this.CommandTypes.Remove(handle); if(Equals(this.CommandTypes.Count,0)) { this.CommandTypes = null; }

            return false;

        }
        catch ( Exception _ ) { Logger?.Error(_,RegisterCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.CommandTypes)) { Exit(this.Sync.CommandTypes); } if(CS) { this.Sync.Commands.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="RegisterCommands"]/*'/>*/
    protected Boolean RegisterCommands(Dictionary<String,ICommand>? commands = null)
    {
        Boolean CS = false;
        DC();
        try
        {
            this.CommandManagerKey = this.CreateManagementKey("CommandManagerKey") as ManagerKey;

            if(commands is null || Equals(commands.Count,0)) { return true; }

            if(!this.Sync.Commands.Wait(SyncTime)) { throw SyncException; } else { CS = true; }

            if(!TryEnter(this.Sync.CommandTypes,SyncTime)) { throw SyncException; }

            this.Commands ??= new(); this.CommandTypes ??= new(); this.CommandKeys ??= new();

            foreach(var _ in commands)
            {
                String handle = _.Key; ICommand cmd = _.Value;

                if(this.Commands.ContainsKey(handle) || this.CommandTypes.ContainsKey(handle)) { throw new OperationFailedException(); }

                CommandKey? k = this.CommandKeys!.GetValueOrDefault(cmd) as CommandKey; 
                
                if(k is null) { k = this.AccessManager!.GenerateCommandKey(cmd,this.SelfAccessKey); if( k is null || this.CommandKeys!.TryAdd(cmd,k) is false ) { throw new OperationFailedException(); } }

                if( cmd.Attach(this,k) && ( cmd.GetLocked() ? true : cmd.Lock(this.CommandManagerKey) ) && this.Commands.TryAdd(new(handle),cmd) && this.CommandTypes.TryAdd(new(handle),cmd.GetType().FullName!) )
                {
                    continue;
                }
                else
                {
                    cmd.UnLock(this.HostingManagerKey); cmd.Detach(k);

                    this.Commands.Remove(handle); if(Equals(this.Commands.Count,0)) { this.Commands = null; }

                    this.CommandTypes.Remove(handle); if(Equals(this.CommandTypes.Count,0)) { this.CommandTypes = null; }

                    throw new OperationFailedException();
                }
            }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,RegisterCommandsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.CommandTypes)) { Exit(this.Sync.CommandTypes); } if(CS) { this.Sync.Commands.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RegisterManager)]
    public virtual Boolean RegisterManager(ManagementKey? managementkey = null , AccessKey? key = null)
    {
        DC();
        try
        {
            if(managementkey is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            this.Secrets ??= new(); Byte[]? _ = CreateSecret(managementkey.Key); if(_ is null) { return false; }

            this.Secrets.Add(_); if(this.Secrets.Contains(_)) { return true; } return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } }
    }

    ///<inheritdoc/>
    public override Boolean RegisterManager(ManagementKey? managementkey) { return this.RegisterManager(managementkey,null); }

    ///<inheritdoc/>
    public Boolean ReleaseOwnership(ManagementKey? managementkey)
    {
        DC();
        try
        {
            if(!TryEnter(this.Sync.OwnerSecret,SyncTime)) { throw SyncException; }

            if(CheckOwner(managementkey) is true) { ZeroMemory(this.OwnerSecret); this.OwnerSecret = null; return true; }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,ReleaseOwnershipFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.OwnerSecret)) { Exit(this.Sync.OwnerSecret); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RemoveActivity)]
    public virtual Boolean RemoveActivity(Activity? activity , AccessKey? key = null)
    {
        DC();
        try
        {
            if( activity is null || this.Activities is null ) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!TryEnter(this.Sync.Activities,SyncTime)) { throw SyncException; }

            if(this.Activities.Remove(activity))
            {
                if(Equals(this.Activities.Count,0)) { this.Activities = null; }

                return true;
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveActivityFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Activities)) { Exit(this.Sync.Activities); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RemoveDataItem)]
    public virtual Boolean RemoveDataItem(Guid? id , AccessKey? key = null)
    {
        DC();
        try
        {
            if( id is null || this.Data is null ) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!TryEnter(this.Sync.Data,SyncTime)) { throw SyncException; }

            DataItem? _ = this.Data.FirstOrDefault(_=>_.GetID().Equals(id));

            if(_ is not null)
            {
                if(this.Data.Remove(_))
                {
                    if(Equals(this.Data.Count,0)) { this.Data = null; }

                    return true;
                }
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveDataItemFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Data)) { Exit(this.Sync.Data); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="RemoveInstance"]/*'/>*/
    protected static Boolean RemoveInstance(Tool? tool)
    {
        try
        {
            if( tool is null || tool.GetID().HasValue is false ) { return false; }

            if(!TryEnter(Instances,SyncTime)) { throw SyncException; }

            if(Instances.Remove(tool.GetID()!.Value)) { return true; }

            return false;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RemoveInstanceFail); if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(Instances)) { Exit(Instances); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RemoveOutput)]
    public virtual Boolean RemoveOutput(Guid? id , AccessKey? key = null)
    {
        Boolean OS = false;
        DC();
        try
        {
            if( id is null || this.Outputs is null ) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!this.Sync.Outputs.Wait(SyncTime)) { throw SyncException; } else { OS = true; }

            if(this.Outputs.Remove(id.Value))
            {
                if(Equals(this.Outputs.Count,0)) { this.Outputs = null; }

                return true;
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveOutputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(OS) { this.Sync.Outputs.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RemoveStatus)]
    public virtual Boolean RemoveStatus(String? index , AccessKey? key = null)
    {
        DC();
        try
        {
            if( index is null || this.Status is null ) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!TryEnter(this.Sync.Status,SyncTime)) { throw SyncException; }

            if(this.Status.Remove(index))
            {
                if(Equals(this.Status.Count,0)) { this.Status = null; }

                return true;
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveStatusFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Status)) { Exit(this.Sync.Status); } }
    }

    ///<inheritdoc/>
    public AccessKey? RequestAccess(AccessRequest? request = null)
    {
        DC(); try { if(request is null) { return null; } return this.AccessManager?.RequestAccess(request); }

        catch ( Exception _ ) { Logger?.Error(_,RequestAccessFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="RequestHostingKeys"]/*'/>*/
    protected virtual Boolean RequestHostingKeys()
    {
        DC();
        try
        {
            if(this.HostingKeys is not null) { return false; }

            IEnumerable<ITool>? s = this.GetHostedServices(this.SelfAccessKey)?.OfType<ITool>(); if( s is null || Equals(s.Count(),0) ) { return true; }

            var k = s.Select(tool => new { Tool = tool , AccessKey = tool.RequestAccess(new HostRequest(this)) }).Where(_ => _.AccessKey is not null).ToList();

            if(Equals(k.Count,s.Count()) is false) { return false; } this.HostingKeys = k.ToDictionary(_=> _.Tool , _=> _.AccessKey!);

            return this.HostingKeys is not null;
        }
        catch ( Exception _ ) { Logger?.Error(_,RequestHostingKeysFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="ResolveHostedServices"]/*'/>*/
    protected virtual Boolean ResolveHostedServices()
    {
        DC();
        try
        {
            if(this.HostedServices is not null) { return false; }

            this.HostingOptions = this.ToolServices?.GetService<IOptions<ToolHostOptions>>()?.Value ?? new();

            this.HostedServices = this.ToolServices?.GetServices<IHostedService>()?.ToList() ?? new List<IHostedService>();

            if( this.RequestHostingKeys() is true && this.IssueMyHostKeys() is true && this.LockHostedServices() is true ) { return true; }

            throw new OperationFailedException();
        }
        catch ( Exception _ ) { Logger?.Error(_,ResolveHostedServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="ResolveLogger"]/*'/>*/
    protected virtual Boolean ResolveLogger(ILoggerFactory? logger = null)
    {
        DC();
        try
        {
            if(this.Logger is not null) { return false; }

            if(!TryEnter(this.Sync.Logger,SyncTime)) { throw SyncException; }

            this.Logger = (logger ?? this.ToolServices?.GetService<ILoggerFactory>())?
                .CreateLogger(String.Format(ToolLoggerNameFormat,this.GetType().FullName,this.GetID()));

            return this.Logger is not null;
        }
        catch ( Exception _ ) { Logger?.Error(_,ResolveLoggerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Logger)) { Exit(this.Sync.Logger); } }
    }

    ///<inheritdoc/>
    public Boolean RevokeAccess(AccessKey? key = null)
    {
        DC(); try { if(key is null) { return false; } return this.AccessManager?.RevokeAccess(key) ?? false; }

        catch ( Exception _ ) { Logger?.Error(_,RevokeAccessFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="SetConfiguration"]/*'/>*/
    protected virtual Boolean SetConfiguration(IConfiguration? configuration = null)
    {
        DC();
        try
        {
            this.Configuration ??= ToolConfiguration.Deserialize(ToolConfiguration.Serialize(configuration    ??
                                   this.GetToolServiceScope()?.ServiceProvider?.GetService<IConfiguration>()) ??
                                   Environment.GetEnvironmentVariable(ConfigurationDataEnvironmentVariable)   ??
                                   ReadAllText(ConfigurationPathEnvironmentVariable));

            if(this.Configuration is not null) { return true; } return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,SetConfigurationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetID"]/*'/>*/
    [AccessCheck(ProtectedOperation.SetID)]
    public virtual Boolean SetID(Guid? id , AccessKey? key = null)
    {
        DC();
        try
        {
            if(id is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!TryEnter(this.Sync.ID,SyncTime)) { throw SyncException; }

            if(Guid.Equals(id,Guid.Empty)) { this.ID = null; } else { this.ID = id; }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,SetIDFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.ID)) { Exit(this.Sync.ID); } }
    }

    ///<inheritdoc/>
    public override Boolean SetID(Guid? id) { return this.SetID(id,null); }

    ///<inheritdoc/>
    public virtual Boolean SetMyHostKey(AccessKey? key = null)
    {
        DC();
        try
        {
            if( this.Locked || key is null || this.MyHostKey is not null ) { return false; } this.MyHostKey = key; return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,SetMyHostKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.SetStatus)]
    public virtual Boolean SetStatus(String? index , Object? status , AccessKey? key = null)
    {
        DC();
        try
        {
            if(String.IsNullOrEmpty(index)) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!TryEnter(this.Sync.Status,SyncTime)) { throw SyncException; }

            if(this.Status is null) { this.Status = new(); } if(this.Status.TryAdd(index,status) is false) { return false; }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,SetStatusFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Status)) { Exit(this.Sync.Status); } }
    }

    ///<inheritdoc/>
    public Boolean TakeOwnership(ManagementKey? managementkey = null)
    {
        DC(); if( this.Locked || managementkey is null ) { return false; }
        try
        {
            if(!TryEnter(this.Sync.OwnerSecret,SyncTime)) { throw SyncException; }

            if(this.OwnerSecret is null || CheckOwner(managementkey))
            {
                this.OwnerSecret = CreateSecret(managementkey.Key); if(this.OwnerSecret is null) { return false; }

                return CheckManager(managementkey) ? true : RegisterManager(managementkey);
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,TakeOwnershipFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.OwnerSecret)) { Exit(this.Sync.OwnerSecret); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="ToString"]/*'/>*/
    [AccessCheck(ProtectedOperation.ToStringOp)]
    public virtual String ToString(AccessKey? key = null)
    {
        DC(); try { if(this.AccessCheck(key) is false) { return String.Empty; } return $"{this.GetType().FullName} - [{this.GetID()}]"; }

        catch ( Exception _ ) { Logger?.Error(_,ToStringFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return String.Empty; } throw; }
    }

    ///<inheritdoc/>
    public override String ToString() { return this.ToString(null); }

    ///<inheritdoc/>
    public override Boolean UnLock(ManagementKey? key)
    {
        DC();
        try
        {
            if( this.Locked is false || this.Secrets is null || Equals(this.Secrets.Count,0) || key is null ) { return false; }

            if(!TryEnter(this.Sync.Locked,SyncTime)) { throw SyncException; } if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            if( CheckSecret(key.Key,this.OwnerSecret) || CheckSecrets(key.Key,this.Secrets) ) { this.Locked = false; return true; }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,UnLockFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } if(IsEntered(this.Sync.Locked)) { Exit(this.Sync.Locked); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.UnRegisterCommand)]
    public virtual async Task<Boolean> UnRegisterCommand(String? handle , CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean CS = false;
        DC();
        try
        {
            if( handle is null || this.Commands is null || this.CommandTypes is null || this.CommandKeys is null || (this.AccessCheck(key) is false) ) { return false; }

            cancel ??= CancellationToken.None;

            await this.Sync.Commands.WaitAsync(cancel.Value).ConfigureAwait(false); CS = true;

            if(!TryEnter(this.Sync.CommandTypes,SyncTime)) { throw SyncException; }

            if(this.Commands.TryGetValue(handle,out ICommand? c))
            {
                if(c.Enabled is false || await c.Disable(cancel.Value,key:this.CommandKeys[c]).ConfigureAwait(false) is true)
                {
                    if( (this.Commands.Remove(handle) is false) || (this.CommandTypes.Remove(handle) is false) ) { throw new OperationFailedException(); }

                    if(this.Commands.ContainsValue(c) is false)
                    {
                        if( (c.UnLock(this.CommandManagerKey) is false) || (c.Detach(this.CommandKeys.GetValueOrDefault(c)) is false) || (this.CommandKeys.Remove(c) is false) )
                        {
                            throw new OperationFailedException();
                        }
                    }

                    if(Equals(this.CommandTypes.Count,0)) { this.CommandTypes = null; }

                    if(Equals(this.CommandKeys.Count,0)) { this.CommandKeys = null; }

                    if(Equals(this.Commands.Count,0)) { this.Commands = null; }

                    return true;
                }
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,UnRegisterCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.CommandTypes)) { Exit(this.Sync.CommandTypes); } if(CS) { this.Sync.Commands.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.UnRegisterManager)]
    public virtual Boolean UnRegisterManager(ManagementKey? managementkey = null , AccessKey? key = null)
    {
        DC();
        try
        {
            if(managementkey is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!TryEnter(this.Sync.Secrets,SyncTime)) { throw SyncException; }

            if(CheckRemoveSecret(managementkey.Key,this.Secrets)) { return true; } return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,UnRegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secrets)) { Exit(this.Sync.Secrets); } }
    }

    ///<inheritdoc/>
    public override Boolean UnRegisterManager(ManagementKey? managementkey) { return this.UnRegisterManager(managementkey,null); }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.UpdateInputs)]
    public virtual Boolean UpdateInputs(IEnumerable<Object>? inputs , AccessKey? key = null)
    {
        DC();
        try
        {
            if(inputs is null) { return false; } Queue<Object> _ = new(inputs);

            if(this.AccessCheck(key) is false) { return false; }

            if(!TryEnter(this.Sync.Inputs,SyncTime)) { throw SyncException; }

            if(Equals(_.Count,0)) { this.Inputs = null; } else { this.Inputs = _; }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,UpdateInputsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Inputs)) { Exit(this.Sync.Inputs); } }
    }
}