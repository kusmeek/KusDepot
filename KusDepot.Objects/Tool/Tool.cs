namespace KusDepot;

/**<include file='Tool.xml' path='Tool/class[@name="Tool"]/main/*'/>*/
public partial class Tool : Common , IHostedLifecycleService , ITool
{
    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/constructor[@name="StaticConstructor"]/*'/>*/
    static Tool() { Instances = new(); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public Tool() : this(null,null,null,null,null,null,null) {}

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/constructor[@name="Constructor"]/*'/>*/
    public Tool(IAccessManager? accessmanager = null , IEnumerable<DataItem>? data = null , Guid? id = null , ToolServiceProvider? services = null,
           Dictionary<String,ICommand>? commands = null , IConfiguration? configuration = null , ILoggerFactory? logger = null)
    {
        try
        {
            this.LifeState = null!; this.Sync = null!; this.ID = id;

            this.Initialize(); this.InitializeToolServices(services); this.ResolveLogger(logger);

            this.SetConfiguration(configuration); this.InitializeAccessManagement(accessmanager);

            this.RegisterCommands(commands); this.ResolveHostedServices(); this.AddDataItems(data);
        }
        catch ( Exception _ ) { Logger?.Error(_,ConstructorFail,MyTypeName,MyID); throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="AccessCheck"]/*'/>*/
    protected Boolean AccessCheck(AccessKey? key = null , [CallerMemberName] String? operationname = null)
    {
        if(this.Locked is false) { return true; } if(key is null || String.IsNullOrEmpty(operationname)) { return false; }

        return this.AccessManager?.AccessCheck(key,operationname) ?? false;
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="AccessCheckIndex"]/*'/>*/
    protected Boolean AccessCheck(AccessKey? key , Int32 operation)
    {
        if(this.Locked is false) { return true; } if(key is null) { return false; }

        return this.AccessManager?.AccessCheck(key,operation) ?? false;
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="AccessCheckAsync"]/*'/>*/
    protected Task<Boolean> AccessCheckAsync(AccessKey? key = null , [CallerMemberName] String? operationname = null , CancellationToken cancel = default)
    {
        if(cancel.IsCancellationRequested) { return Task.FromCanceled<Boolean>(cancel); }

        if(this.Locked is false) { return Task.FromResult(true); }

        if(key is null || String.IsNullOrEmpty(operationname)) { return Task.FromResult(false); }

        return this.AccessManager?.AccessCheckAsync(key,operationname,cancel) ?? Task.FromResult(false);
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="AccessCheckIndexAsync"]/*'/>*/
    protected Task<Boolean> AccessCheckAsync(AccessKey? key , Int32 operation , CancellationToken cancel = default)
    {
        if(cancel.IsCancellationRequested) { return Task.FromCanceled<Boolean>(cancel); }

        if(this.Locked is false) { return Task.FromResult(true); }

        if(key is null) { return Task.FromResult(false); }

        return this.AccessManager?.AccessCheckAsync(key,operation,cancel) ?? Task.FromResult(false);
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.Activate)]
    public virtual async Task<Boolean> Activate(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;
        try
        {
            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            return await this.Activate_NoSync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,ActivateFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Activate_NoSync"]/*'/>*/
    [AccessCheck(ProtectedOperation.Activate)]
    protected virtual async Task<Boolean> Activate_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None;
        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false || this.LifeState.ActivateOK() is false) { return false; }

            this.LifeState.ToActive(); return true;
        }
        catch ( Exception _ )
        {
            Logger?.Error(_,ActivateFail,MyTypeName,MyID);

            if(NoExceptions||MyNoExceptions) { this.LifeState.ToError(); return false; } throw;
        }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AddActivity)]
    public virtual Boolean AddActivity(Activity? activity , AccessKey? key = null)
    {
        DC(); Boolean sa = false;
        try
        {
            if(activity is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            sa = TryEnter(this.Sync.Activities,SyncTime); if(!sa) { throw SyncException; }

            if(this.Activities is not null && this.Activities.Contains(activity)) { return true; }

            if(this.Activities is null) { this.Activities = new(); } this.Activities.Add(activity);

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,AddActivityFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(sa) { Exit(this.Sync.Activities); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AddDataItems)]
    public virtual Boolean AddDataItems(IEnumerable<DataItem>? data , AccessKey? key = null)
    {
        DC(); Boolean ds = false;
        try
        {
            if(data is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            HashSet<DataItem> _ = new(data,new IDEquality()); if(_.Count == 0) { return false; }

            HashSet<Guid> i = new();

            foreach(var d in _)
            {
                Guid? id = d.GetID(); if( id is null || id == Guid.Empty || !i.Add(id.Value) ) { return false; }
            }

            ds = TryEnter(this.Sync.Data,SyncTime); if(!ds) { throw SyncException; }

            if(this.Data is null)
            {
                this.Data = new(new IDEquality()); this.DataIndex = new();
            }

            foreach(var id in i)
            {
                if(this.DataIndex!.ContainsKey(id)) { return false; }
            }

            foreach(var d in _)
            {
                this.Data.Add(d); this.DataIndex![d.GetID()!.Value] = d;
            }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,AddDataItemsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ds) { Exit(this.Sync.Data); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AddHostedService)]
    public virtual async Task<Boolean> AddHostedService(IHostedService? service , String? name = null , ImmutableArray<Int32>? permissions = null , HostRequest? request = null , Boolean start = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; Boolean hs = false;
        try
        {
            if(service is null) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(!await this.Sync.HostedServices.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } hs = true;

            if(this.LifeState.HostedServicesChangeOK() is false) { return false; }

            return await this.AddHostedService_NoSync(service,name,permissions,request,start,cancel.Value).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,AddHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> AddHostedService(Func<IHostedService?>? factory , String? name = null , ImmutableArray<Int32>? permissions = null , HostRequest? request = null , Boolean start = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; Boolean hs = false;
        try
        {
            if(factory is null) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            IHostedService? service = factory(); if(service is null) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(!await this.Sync.HostedServices.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } hs = true;

            if(this.LifeState.HostedServicesChangeOK() is false) { return false; }

            return await this.AddHostedService_NoSync(service,name,permissions,request,start,cancel.Value).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,AddHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> AddHostedService(Type? servicetype , String? name = null , Object?[]? arguments = null , ImmutableArray<Int32>? permissions = null , HostRequest? request = null , Boolean start = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; Boolean hs = false;
        try
        {
            if(servicetype is null || servicetype.IsAssignableTo(typeof(IHostedService)) is false) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(!await this.Sync.HostedServices.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } hs = true;

            if(this.LifeState.HostedServicesChangeOK() is false) { return false; }

            ObjectBuilder b = ObjectBuilder.Create(servicetype);

            if(arguments is not null)
            {
                for(Int32 i = 0; i < arguments.Length; i++)
                {
                    if(b.SetArgument(i,arguments[i]) is false) { return false; }
                }
            }

            if(b.Build() is false || b.Value is not IHostedService s) { return false; }

            this.HostedServices ??= new(StringComparer.Ordinal); this.EnsureHostedServiceNameIndex_NoSync();

            if(String.IsNullOrEmpty(name) && this.HostedServices.Any(_ => servicetype.IsAssignableFrom(_.Value.GetType()))) { return false; }

            return await this.AddHostedService_NoSync(s,name,permissions,request,start,cancel.Value).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,AddHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="AddHostedService_NoSync"]/*'/>*/
    protected virtual async Task<Boolean> AddHostedService_NoSync(IHostedService? service , String? name = null , ImmutableArray<Int32>? permissions = null , HostRequest? request = null , Boolean start = false , CancellationToken cancel = default)
    {
        try
        {
            if(service is null) { return false; }

            this.HostedServices ??= new(StringComparer.Ordinal); this.EnsureHostedServiceNameIndex_NoSync();

            if(this.HostedServiceNamesByInstance is not null && this.HostedServiceNamesByInstance.TryGetValue(service,out String? existingname))
            {
                if(String.IsNullOrEmpty(name)) { return true; }

                return String.Equals(existingname,name,Ordinal);
            }

            String servicename = String.IsNullOrEmpty(name) ? this.GenerateHostedServiceName_NoSync() : new(name);

            if(this.HostedServices.TryGetValue(servicename,out IHostedService? named) && named is not null)
            {
                return ReferenceEquals(named,service);
            }

            HostKey? hk = null; MyHostKey? mhk = null; ManagerKey? hmk = null; Boolean waslocked = false;

            if(service is ITool t)
            {
                this.HostingKeys ??= new(ReferenceEqualityComparer.Instance); this.HostingManagerKeys ??= new(ReferenceEqualityComparer.Instance);
                this.HostingMyHostKeys ??= new(ReferenceEqualityComparer.Instance); this.HostedServiceLockState ??= new(ReferenceEqualityComparer.Instance);

                if(this.HostingKeys.ContainsKey(t) || this.HostingManagerKeys.ContainsKey(t) || this.HostingMyHostKeys.ContainsKey(t) || this.HostedServiceLockState.ContainsKey(t))
                {
                    return false;
                }

                waslocked = t.GetLocked();

                hk = t.RequestAccess(request ?? new HostRequest(this,true)) as HostKey; if(hk is null) { return false; }

                var c = CreateCertificate(t,"HostingManagerKey"); if(c is null) { _ = ClearKeysUnLock(t,hk,hmk,waslocked is false); return false; } hmk = new ManagerKey(c);

                mhk = this.AccessManager!.GenerateAccessKey<MyHostKey>(AccessKeyIssueOptions.Create(subject: t.GetID()!.Value.ToString(),operations: permissions ?? ProtectedOperationSets.MyHost),key:this.SelfKey);

                if(mhk is null) { _ = ClearKeysUnLock(t,hk,hmk,waslocked is false); return false; }

                if(waslocked is false && t.CheckManager(hmk) is false && t.RegisterManager(hmk,hk) is false)
                {
                    this.RevokeAccess(mhk); _ = ClearKeysUnLock(t,hk,hmk,waslocked is false); return false;
                }

                if(t.SetMyHostKey(hk,mhk) && ( waslocked || t.Lock(hmk) ) )
                {
                    this.HostingKeys[t] = hk.Copy<HostKey>()!; this.HostingManagerKeys[t] = hmk.Copy<ManagerKey>()!; this.HostingMyHostKeys[t] = mhk.Copy<MyHostKey>()!;

                    this.HostedServiceLockState[t] = waslocked;

                    if(this.HostedServices.TryAdd(servicename,service) is false)
                    {
                        this.RevokeAccess(mhk); _ = ClearKeysUnLock(t,hk,hmk,waslocked is false); return false;
                    }

                    this.HostedServiceNamesByInstance ??= new(ReferenceEqualityComparer.Instance); this.HostedServiceNamesByInstance[service] = servicename;
                }
                else { this.RevokeAccess(mhk); _ = ClearKeysUnLock(t,hk,hmk,waslocked is false); return false; }
            }
            else
            {
                if(this.HostedServices.TryAdd(servicename,service) is false) { return false; }

                this.HostedServiceNamesByInstance ??= new(ReferenceEqualityComparer.Instance); this.HostedServiceNamesByInstance[service] = servicename;
            }

            if(start)
            {
                try
                {
                    if(await this.StartHostedServiceCore(service,cancel).WaitAsync(this.HostingOptions!.StartupTimeout,cancel).ConfigureAwait(false) is false)
                    {
                        this.HostedServices.Remove(servicename); _ = this.HostedServiceNamesByInstance?.Remove(service);

                        if(service is ITool s)
                        {
                            this.RevokeAccess(mhk);

                            this.HostingKeys!.Remove(s); this.HostingManagerKeys!.Remove(s); this.HostingMyHostKeys!.Remove(s); this.HostedServiceLockState!.Remove(s);

                            _ = ClearKeysUnLock(s,hk,hmk,waslocked is false);
                        }

                        return false;
                    }
                }
                catch
                {
                    this.HostedServices.Remove(servicename); _ = this.HostedServiceNamesByInstance?.Remove(service);

                    if(service is ITool s)
                    {
                        this.RevokeAccess(mhk);

                        this.HostingKeys!.Remove(s); this.HostingManagerKeys!.Remove(s); this.HostingMyHostKeys!.Remove(s); this.HostedServiceLockState!.Remove(s);

                        _ = ClearKeysUnLock(s,hk,hmk,waslocked is false);
                    }

                    throw;
                }
            }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,AddHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        async Task ClearKeysUnLock(ITool? tool , HostKey? hostkey , ManagerKey? managerkey , Boolean unlock)
        {
            try
            {
                if(unlock) { if(tool!.UnLock(managerkey) && tool.UnRegisterManager(managerkey,hostkey) is false) { return; } }

                tool?.ClearMyHostKey(hostkey); hostkey?.ClearKey(); managerkey?.ClearKey(); await Task.CompletedTask.ConfigureAwait(false);
            }
            catch ( Exception _ ) { Logger?.Error(_,AddHostedServiceClearFail,MyTypeName,MyID); }
        }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="EnsureHostedServiceNameIndex_NoSync"]/*'/>*/
    private void EnsureHostedServiceNameIndex_NoSync()
    {
        if(this.HostedServices is null || this.HostedServices.Count == 0)
        {
            this.HostedServiceNamesByInstance = null; return;
        }

        if(this.HostedServiceNamesByInstance is not null && this.HostedServiceNamesByInstance.Count == this.HostedServices.Count) { return; }

        Dictionary<IHostedService,String> index = new(ReferenceEqualityComparer.Instance);

        foreach(var service in this.HostedServices)
        {
            _ = index.TryAdd(service.Value,service.Key);
        }

        this.HostedServiceNamesByInstance = index;
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AddInput)]
    public virtual Boolean AddInput(Object? input , AccessKey? key = null)
    {
        DC(); Boolean si = false;
        try
        {
            if(input is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            si = TryEnter(this.Sync.Inputs,SyncTime); if(!si) { throw SyncException; }

            if(this.Inputs is null) { this.Inputs = new(); } this.Inputs.Enqueue(input);

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,AddInputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(si) { Exit(this.Sync.Inputs); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="AddInstance"]/*'/>*/
    protected static Boolean AddInstance(ITool? tool)
    {
        try
        {
            if( tool is null || tool.GetID().HasValue is false ) { return false; }

            Guid id = tool.GetID()!.Value; WeakReference<ITool> w = new(tool);

            while(true)
            {
                if(Instances.TryGetValue(id,out WeakReference<ITool>? e))
                {
                    if(e.TryGetTarget(out ITool? t) && t.GetDisposed() is false) { return ReferenceEquals(t,tool); }

                    if(Instances.TryUpdate(id,w,e)) { return true; }

                    continue;
                }

                if(Instances.TryAdd(id,w)) { return true; }
            }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,AddInstanceFail); if(NoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AddInstance)]
    public Boolean AddInstance(AccessKey? key = null) { if(this.AccessCheck(key) is false) { return false; } return AddInstance(this); }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AddOutput)]
    public virtual Boolean AddOutput(Guid? id , Object? output , AccessKey? key = null)
    {
        DC(); Boolean os = false;
        List<TaskCompletionSource<Object?>>? waiters = null;
        try
        {
            if( id is null || id.Value == Guid.Empty ) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!this.Sync.Outputs.Wait(SyncTime)) { throw SyncException; } else { os = true; }

            if(this.Outputs is null) { this.Outputs = new(); }

            if(this.Outputs.TryAdd(id.Value,output) is false) { return false; }

            if(this.OutputWaiters is not null && this.OutputWaiters.Remove(id.Value,out List<TaskCompletionSource<Object?>>? pending))
            {
                if(this.OutputWaiters.Count == 0) { this.OutputWaiters = null; }

                waiters = pending;
            }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,AddOutputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally
        {
            if(os) { this.Sync.Outputs.Release(); }

            if(waiters is not null)
            {
                foreach(TaskCompletionSource<Object?> waiter in waiters)
                {
                    waiter.TrySetResult(output);
                }
            }
        }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.AttachCommand)]
    public virtual async Task<Boolean> AttachCommand(ICommand? command , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None;
        Boolean cs = false; Boolean ls = false;
        try
        {
            if(command is null) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            return await this.AttachCommand_NoSync(command,permissions,enable,cancel.Value,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,AttachCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> AttachCommand(Func<ICommand?>? factory , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None;
        Boolean cs = false; Boolean ls = false;
        try
        {
            if(factory is null) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            ICommand? command = factory(); if(command is null) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            return await this.AttachCommand_NoSync(command,permissions,enable,cancel.Value,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,AttachCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> AttachCommand(Type? commandtype , Object?[]? arguments = null , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None;
        Boolean cs = false; Boolean ls = false;
        try
        {
            if(commandtype is null || commandtype.IsAssignableTo(typeof(ICommand)) is false) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            ObjectBuilder b = ObjectBuilder.Create(commandtype);

            if(arguments is not null)
            {
                for(Int32 i = 0; i < arguments.Length; i++)
                {
                    if(b.SetArgument(i,arguments[i]) is false) { return false; }
                }
            }

            if(b.Build() is false || b.Value is not ICommand command) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            return await this.AttachCommand_NoSync(command,permissions,enable,cancel.Value,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,AttachCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="AttachCommand_NoSync"]/*'/>*/
    protected virtual async Task<Boolean> AttachCommand_NoSync(ICommand command , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken cancel = default , AccessKey? key = null)
    {
        this.CommandKeys ??= new(ReferenceEqualityComparer.Instance); this.CommandManagerKeys ??= new(ReferenceEqualityComparer.Instance); this.AttachedCommands ??= new(ReferenceEqualityComparer.Instance);

        if(this.CommandKeys.TryGetValue(command,out _) || this.CommandManagerKeys.TryGetValue(command,out _)) { return false; }

        if(command.GetLocked()) { return false; } Boolean enabled = false;

        X509Certificate2? c = CreateCertificate(command,"CommandManagerKey");

        CommandKey? ck = this.AccessManager!.GenerateAccessKey<CommandKey>(AccessKeyIssueOptions.Create(subject: command.GetID()!.Value.ToString(),operations: permissions ?? ProtectedOperationSets.Command),key:key);

        if(c is null || ck is null)
        {
            if(ck is not null) { this.RevokeAccess(ck); }

            return false;
        }

        ManagerKey mk = new(c); this.CommandKeys[command] = ck.Copy<CommandKey>()!; this.CommandManagerKeys[command] = mk.Copy<ManagerKey>()!;

        if(command.Attach(this,ck) is false)
        {
            this.RevokeAccess(ck);

            this.CommandKeys.Remove(command);

            this.CommandManagerKeys.Remove(command);

            return false;
        }

        if(enable && command.Enabled is false)
        {
            if(await command.Enable(cancel,ck).ConfigureAwait(false) is false)
            {
                this.RevokeAccess(ck);

                this.CommandKeys.Remove(command);

                this.CommandManagerKeys.Remove(command);

                command.Detach(ck);

                return false;
            }

            enabled = true;
        }

        if(command.RegisterManager(mk) is false || command.Lock(mk) is false)
        {
            this.RevokeAccess(ck);

            this.CommandKeys.Remove(command);

            this.CommandManagerKeys.Remove(command);

            if(enabled) { await command.Disable(cancel,ck).ConfigureAwait(false); }

            command.UnRegisterManager(mk);

            command.Detach(ck);

            return false;
        }

        this.AttachedCommands.Add(command);

        return true;
    }

    ///<inheritdoc/>
    public override Boolean CheckManager(ManagementKey? key)
    {
        DC(); Boolean ss = false;
        try
        {
            if( this.Secrets is null || this.Secrets.Count == 0 ) { return false; }

            ss = TryEnter(this.Sync.Secrets,SyncTime); if(!ss) { throw SyncException; }

            return this.Secrets.Any(_ => CheckSecret(key?.Key,_));
        }
        catch ( Exception _ ) { Logger?.Error(_,CheckManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ss) { Exit(this.Sync.Secrets); } }
    }

    ///<inheritdoc/>
    public Boolean CheckOwner(ManagementKey? managementkey)
    {
        DC(); Boolean os = false;
        try
        {
            if(this.OwnerSecret is null) { return false; }

            os = TryEnter(this.Sync.OwnerSecret,SyncTime); if(!os) { throw SyncException; }

            return CheckSecret(managementkey?.Key,this.OwnerSecret);
        }
        catch ( Exception _ ) { Logger?.Error(_,CheckOwnershipFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(os) { Exit(this.Sync.OwnerSecret); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.ClearMyHostKey)]
    public virtual Boolean ClearMyHostKey(HostKey? hostkey = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(hostkey) is false || this.MyHostKey is null) { return false; }

            if(this.RevokeAccess(hostkey) is false) { return false; }

            this.MyHostKey.ClearKey(); this.MyHostKey = null; return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,ClearMyHostKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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
    public virtual ExecutiveKey? CreateExecutiveKey(ManagementKey? managementkey = null)
    {
        DC(); if(this.CheckManager(managementkey) is false) { return null; }

        try { return this.AccessManager!.RequestAccess(new ManagementRequest(managementkey!)) as ExecutiveKey; }

        catch ( Exception _ ) { Logger?.Error(_,CreateExecutiveKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual async Task<ExecutiveKey?> CreateExecutiveKeyAsync(ManagementKey? managementkey = null , CancellationToken? cancel = null)
    {
        DC(); cancel ??= CancellationToken.None;

        if(cancel.Value.IsCancellationRequested) { return null; }

        try { return await this.AccessManager!.RequestAccessAsync(new ManagementRequest(managementkey!)) as ExecutiveKey; }

        catch ( Exception _ ) { Logger?.Error(_,CreateExecutiveKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
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
    public virtual async Task<Boolean> Deactivate(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false;
        try
        {
            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            return await this.Deactivate_NoSync(cancel,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,DeactivateFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Deactivate_NoSync"]/*'/>*/
    [AccessCheck(ProtectedOperation.Deactivate)]
    protected virtual async Task<Boolean> Deactivate_NoSync(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None;
        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false || this.LifeState.DeactivateOK() is false) { return false; }

            this.LifeState.ToInActive(); return true;
        }
        catch ( Exception _ )
        {
            Logger?.Error(_,DeactivateFail,MyTypeName,MyID);

            if(NoExceptions||MyNoExceptions) { this.LifeState.ToError(); return false; } throw;
        }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DestroySecrets_AccessKey"]/*'/>*/
    protected virtual Boolean DestroySecrets(AccessKey? accesskey)
    {
        Boolean ss = false; Boolean os = false;
        try
        {
            if(accesskey is null || Equals(this.SelfKey,accesskey) is false) { return false; }

            ss = TryEnter(this.Sync.Secrets,SyncTime); if(!ss) { throw SyncException; }

            os = TryEnter(this.Sync.OwnerSecret,SyncTime); if(!os) { throw SyncException; }

            this.AccessManager?.DestroySecrets(this.SelfKey);

            if(this.HostingKeys is not null)
            {
                foreach(var k in this.HostingKeys.Values) { k.ClearKey(); } this.HostingKeys.Clear();
            }

            if(this.HostingManagerKeys is not null)
            {
                foreach(var k in this.HostingManagerKeys.Values) { k.ClearKey(); } this.HostingManagerKeys.Clear(); this.HostingManagerKeys = null;
            }

            if(this.HostingMyHostKeys is not null)
            {
                foreach(var k in this.HostingMyHostKeys.Values) { k.ClearKey(); } this.HostingMyHostKeys.Clear(); this.HostingMyHostKeys = null;
            }

            if(this.CommandKeys is not null)
            {
                foreach(var k in this.CommandKeys.Values) { k.ClearKey(); } this.CommandKeys.Clear();
            }

            if(this.CommandManagerKeys is not null)
            {
                foreach(var k in this.CommandManagerKeys.Values) { k.ClearKey(); } this.CommandManagerKeys.Clear(); this.CommandManagerKeys = null;
            }

            if(this.OwnerSecret is not null) { ZeroMemory(this.OwnerSecret); this.OwnerSecret = null; }

            if(this.Secrets is not null)
            {
                foreach(var s in this.Secrets) { ZeroMemory(s); } this.Secrets.Clear();
            }

            this.MyHostKey?.ClearKey(); this.MyHostKey = null;

            this.SelfKey?.ClearKey(); this.SelfKey = null;

            return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DestroySecretsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(os) { Exit(this.Sync.OwnerSecret); } if(ss) { Exit(this.Sync.Secrets); } }
    }

   ///<inheritdoc/>
    protected override Boolean DestroySecrets(ManagementKey? managementkey)
    {
        try
        {
            if(this.CheckManager(managementkey) is false) { return false; }

            return this.DestroySecrets(this.SelfKey);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,DestroySecretsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.DetachCommand)]
    public virtual async Task<Boolean> DetachCommand(ICommand? command , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean cs = false; Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None;

            if(command is null) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            return await this.DetachCommand_NoSync(command,cancel.Value).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,DetachCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DetachCommand_NoSync"]/*'/>*/
    protected virtual async Task<Boolean> DetachCommand_NoSync(ICommand command , CancellationToken cancel = default)
    {
        if(this.CommandKeys is null || this.CommandManagerKeys is null) { return false; }

        if(this.CommandKeys.TryGetValue(command,out CommandKey? ck) is false || ck is null || this.CommandManagerKeys.TryGetValue(command,out ManagerKey? mk) is false || mk is null)
        {
            return false;
        }

        if(this.CommandHandles is not null && this.CommandHandles.TryGetValue(command,out HashSet<String>? handles))
        {
            if(handles.Count != 0) { return false; }
        }

        ck = ck.Copy<CommandKey>()!;

        if(command.Enabled && await command.Disable(cancel,ck).ConfigureAwait(false) is false) { return false; }

        mk = mk.Copy<ManagerKey>()!;

        if(command.GetLocked()) { if(command.UnLock(mk) is false) { return false; } }

        if(command.CheckManager(mk) && command.UnRegisterManager(mk) is false) { return false; }

        if(command.Detach(ck) is false) { return false; }

        if(this.RevokeAccess(ck) is false) { return false; }

        this.CommandKeys.Remove(command);

        this.CommandManagerKeys.Remove(command);

        this.AttachedCommands?.Remove(command);

        this.CommandHandles?.Remove(command);

        return true;
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
    [AccessCheck(ProtectedOperation.DisableAllCommands)]
    public virtual async Task<Boolean> DisableAllCommands(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean cs = false; Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None;

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            if(this.AttachedCommands is null || this.CommandKeys is null) { return true; }

            Task<Boolean>[] disable = new Task<Boolean>[this.AttachedCommands.Count]; Int32 index = 0;

            foreach(ICommand cmd in this.AttachedCommands)
            {
                try { disable[index] = cmd.Disable(cancel.Value,this.CommandKeys[cmd].Copy<CommandKey>()!); }

                catch { disable[index] = Task.FromResult(false); }

                index++;
            }

            Boolean[] r = await Task.WhenAll(disable).ConfigureAwait(false);

            for(Int32 i = 0; i < r.Length; i++)
            {
                if(r[i] is false) { return false; }
            }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,DisableAllCommandsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.DisableCommand)]
    public virtual async Task<Boolean> DisableCommand(String? handle , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean cs = false; Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None;

            if(String.IsNullOrEmpty(handle)) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            if(this.Commands is null || this.CommandKeys is null) { return false; }

            if(this.Commands.TryGetValue(handle,out ICommand? cmd) is false || cmd is null) { return false; }

            var ck = this.CommandKeys[cmd].Copy<CommandKey>()!;

            return await cmd.Disable(cancel.Value,ck).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,DisableCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.EnableAllCommands)]
    public virtual async Task<Boolean> EnableAllCommands(CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean cs = false; Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None;

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            if(this.AttachedCommands is null || this.CommandKeys is null) { return true; }

            Task<Boolean>[] enable = new Task<Boolean>[this.AttachedCommands.Count]; Int32 index = 0;

            foreach(ICommand cmd in this.AttachedCommands)
            {
                try { enable[index] = cmd.Enable(cancel.Value,this.CommandKeys[cmd].Copy<CommandKey>()!); }

                catch { enable[index] = Task.FromResult(false); }

                index++;
            }

            Boolean[] r = await Task.WhenAll(enable).ConfigureAwait(false);

            for(Int32 i = 0; i < r.Length; i++)
            {
                if(r[i] is false) { return false; }
            }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,EnableAllCommandsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.EnableCommand)]
    public virtual async Task<Boolean> EnableCommand(String? handle , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean cs = false; Boolean ls = false;
        try
        {
            cancel ??= CancellationToken.None;

            if(String.IsNullOrEmpty(handle)) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            if(this.Commands is null || this.CommandKeys is null) { return false; }

            if(this.Commands.TryGetValue(handle,out ICommand? cmd) is false || cmd is null) { return false; }

            var ck = this.CommandKeys[cmd].Copy<CommandKey>()!;

            return await cmd.Enable(cancel.Value,ck).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,EnableCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
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
        DC(); Boolean cs = false;
        try
        {
            if(this.GetLifeCycleState() is not Active) { return null; }

            if(details is null || this.Commands is null || details.MakeReady(this) is false) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(details.GetLogger() is null) { details.SetLogger(this.GetLogger()); }

            if(!this.Sync.Commands.Wait(SyncTime)) { return null; } else { cs = true; }

            if(this.Commands.TryGetValue(details.Handle!,out ICommand? cmd) is false) { return null; }

            CommandKey? ck = null; _ = this.CommandKeys?.TryGetValue(cmd,out ck); this.Sync.Commands.Release(); cs = false;

            var a = Activity.CreateActivity(details,cmd,asynchronous:false); if(a is null) { return null; }

            try
            {
                Guid? o = cmd.Execute(a,ck); if(o.HasValue) { this.SetActivitySuccess(a); } else { SetActivityFailed(a); } return o;
            }
            catch ( Exception _ ) { this.SetActivityFaulted(a,_); if(NoExceptions||MyNoExceptions) { return null; } throw; }
        }
        catch ( Exception _ ) { Logger?.Error(_,ExecuteCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.ExecuteCommand)]
    public virtual async Task<Guid?> ExecuteCommandAsync(CommandDetails? details = null , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); Boolean cs = false;
        try
        {
            cancel ??= CancellationToken.None;

            if(this.GetLifeCycleState() is not Active) { return null; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return null; }

            if(details is null || this.Commands is null || details.MakeReady(this) is false) { return null; }

            details.SetToolCancel(cancel.Value);

            if(details.GetLogger() is null) { details.SetLogger(this.GetLogger()); }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return null; } cs = true;

            if(this.Commands.TryGetValue(details.Handle!,out ICommand? cmd) is false) { return null; }

            CommandKey? ck = null; _ = this.CommandKeys?.TryGetValue(cmd,out ck); this.Sync.Commands.Release(); cs = false;

            Activity? a = Activity.CreateActivity(details,cmd,asynchronous:true); if(a is null) { return null; }

            try
            {
                Guid? o;

                if(a.FreeMode is false)
                {
                    o = await cmd.ExecuteAsync(a,ck).WaitAsync(a.Details.GetCancel()).ConfigureAwait(false);

                    if(o.HasValue) { this.SetActivitySuccess(a); } else { this.SetActivityFailed(a); }
                }
                else { _ = cmd.ExecuteAsync(a,ck); o = a.ID; }

                return o;
            }
            catch ( Exception _ )
            {
                if(a.FreeMode is false)
                {
                    if( _ is OperationCanceledException ) { this.SetActivityCanceled(a); }

                    else { this.SetActivityFaulted(a,_); }
                }

                if(NoExceptions||MyNoExceptions) { return null; } throw;
            }
        }
        catch ( Exception _ )
        {
            if( _ is not OperationCanceledException )

            { Logger?.Error(_,ExecuteCommandAsyncFail,MyTypeName,MyID); }

            if(NoExceptions||MyNoExceptions) { return null; } throw;
        }
        finally { if(cs) { this.Sync.Commands.Release(); } }
    }

    ///<inheritdoc/>
    public virtual Guid? ExecuteCommandCab(KusDepotCab? cab = null , AccessKey? key = null)
    {
        try { return this.ExecuteCommand(cab?.GetCommandDetails(),key); }

        catch ( Exception _ ) { Logger?.Error(_,ExecuteCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Task<Guid?> ExecuteCommandCabAsync(KusDepotCab? cab = null , CancellationToken? cancel = null , AccessKey? key = null)
    {
        try { return this.ExecuteCommandAsync(cab?.GetCommandDetails(),cancel,key); }

        catch ( Exception _ ) { Logger?.Error(_,ExecuteCommandCabAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return Task.FromResult<Guid?>(null); } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GenerateHostedServiceName_NoSync"]/*'/>*/
    protected virtual String GenerateHostedServiceName_NoSync()
    {
        String k;

        do { k = Guid.NewGuid().ToStringInvariant()!; } while(this.HostedServices?.ContainsKey(k) ?? false);

        return k;
    }

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
        DC(); Boolean sa = false;
        try
        {
            if(this.Activities is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            sa = TryEnter(this.Sync.Activities,SyncTime); if(!sa) { throw SyncException; }

            return this.Activities.ToList();
        }
        catch ( Exception _ ) { Logger?.Error(_,GetActivitiesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(sa) { Exit(this.Sync.Activities); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetCommands)]
    public virtual IList<ICommand>? GetAttachedCommands(AccessKey? key = null)
    {
        DC(); Boolean cs = false;
        try
        {
            if(this.AttachedCommands is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.Commands.Wait(SyncTime)) { throw SyncException; } else { cs = true; }

            return this.AttachedCommands.ToList();
        }
        catch ( Exception _ ) { Logger?.Error(_,GetCommandsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetCommand)]
    public virtual ICommand? GetCommand(String? handle , AccessKey? key = null)
    {
        DC(); Boolean cs = false;
        try
        {
            if(String.IsNullOrEmpty(handle) || this.Commands is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.Commands.Wait(SyncTime)) { throw SyncException; } else { cs = true; }

            return this.Commands.TryGetValue(handle,out ICommand? command) ? command : null;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetCommandDescriptor)]
    public virtual CommandDescriptor? GetCommandDescriptor(String? handle , AccessKey? key = null)
    {
        DC(); Boolean cs = false; Boolean ls = false;
        try
        {
            if(String.IsNullOrEmpty(handle) || this.AccessCheck(key) is false) { return null; }

            if(this.Commands is null || this.CommandTypesMasked) { return null; }

            if(!this.Sync.Life.Wait(SyncTime)) { return null; } ls = true;

            if(!this.Sync.Commands.Wait(SyncTime)) { return null; } else { cs = true; }

            if(this.Commands.TryGetValue(handle,out ICommand? cmd) is false || cmd is null) { return null; }

            var ck = this.CommandKeys?.GetValueOrDefault(cmd);

            CommandDescriptor? d = cmd.GetCommandDescriptor(ck);

            if(d is null) { return null; }

            if(this.CommandHandles?.TryGetValue(cmd,out HashSet<String>? handles) ?? false)
            {
                return d with { RegisteredHandles = handles.Select(_ => new String(_)).ToList() };
            }

            return d;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetCommandDescriptorFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetCommandDescriptors)]
    public virtual IList<CommandDescriptor>? GetCommandDescriptors(AccessKey? key = null)
    {
        DC(); Boolean cs = false; Boolean ls = false;
        try
        {
            if(this.AccessCheck(key) is false) { return null; }

            if(this.AttachedCommands is null || this.CommandTypesMasked) { return null; }

            if(!this.Sync.Life.Wait(SyncTime)) { return null; } ls = true;

            if(!this.Sync.Commands.Wait(SyncTime)) { return null; } else { cs = true; }

            List<CommandDescriptor> descriptors = new();

            foreach(var cmd in this.AttachedCommands)
            {
                var ck = this.CommandKeys?.GetValueOrDefault(cmd);

                CommandDescriptor? d = cmd.GetCommandDescriptor(ck);

                if(d is null) { continue; }

                if(this.CommandHandles?.TryGetValue(cmd,out HashSet<String>? handles) ?? false)
                {
                    d = d with { RegisteredHandles = handles.Select(_ => new String(_)).ToList() };
                }

                descriptors.Add(d);
            }

            return descriptors.Count == 0 ? null : descriptors;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetCommandDescriptorsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetCommandHandles)]
    public virtual IList<String>? GetCommandHandles(ICommand? command , AccessKey? key = null)
    {
        DC(); Boolean cs = false;
        try
        {
            if(command is null || this.CommandHandles is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.Commands.Wait(SyncTime)) { throw SyncException; } else { cs = true; }

            if(this.CommandHandles.TryGetValue(command,out HashSet<String>? handles) is false || handles is null || handles.Count == 0)
            {
                return null;
            }

            return handles.Select(_ => new String(_)).OrderBy(_ => _,StringComparer.Ordinal).ToImmutableList();
        }
        catch ( Exception _ ) { Logger?.Error(_,GetCommandsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetCommands)]
    public virtual Dictionary<String,ICommand>? GetCommands(AccessKey? key = null)
    {
        DC(); Boolean cs = false;
        try
        {
            if(this.Commands is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.Commands.Wait(SyncTime)) { throw SyncException; } else { cs = true; }

            return this.Commands.ToDictionary(_=>new String(_.Key),_=>_.Value);
        }
        catch ( Exception _ ) { Logger?.Error(_,GetCommandsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } }
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
        DC(); Boolean ds = false;
        try
        {
            if(this.Data is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            ds = TryEnter(this.Sync.Data,SyncTime); if(!ds) { throw SyncException; }

            List<Descriptor> _ = this.Data.Select(_=>_.GetDescriptor()!).ToList();

            if(this.Data.Count != _.Count) { return null; } return _;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetDataDescriptorsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(ds) { Exit(this.Sync.Data); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetDataItem)]
    public virtual DataItem? GetDataItem(Guid? id , AccessKey? key = null)
    {
        DC(); Boolean ds = false;
        try
        {
            if( this.Data is null || id is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            ds = TryEnter(this.Sync.Data,SyncTime); if(!ds) { throw SyncException; }

            return this.DataIndex is not null && this.DataIndex.TryGetValue(id.Value,out DataItem? dataitem) ? dataitem : null;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetDataItemFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(ds) { Exit(this.Sync.Data); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetDataItems)]
    public virtual HashSet<DataItem>? GetDataItems(AccessKey? key = null)
    {
        DC(); Boolean ds = false;
        try
        {
            if(this.Data is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            ds = TryEnter(this.Sync.Data,SyncTime); if(!ds) { throw SyncException; }

            HashSet<DataItem> _ = new(this.Data,new IDEquality());

            if(this.Data.Count != _.Count) { return null; } return _;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetDataItemsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(ds) { Exit(this.Sync.Data); } }
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
        IList<Guid>? ids = GetInstanceIDs(); if(ids is null) { return null; }

        foreach(Guid id in ids)
        {
            ITool? tool = GetInstance(id);

            try
            {
                if(tool?.IsHosting(service) ?? false) { return tool; }
            }
            catch ( ObjectDisposedException ) { continue; }
        }

        return null;
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetHostedServiceNames)]
    public virtual IList<String>? GetHostedServiceNames(AccessKey? key = null)
    {
        DC(); Boolean hs = false;
        try
        {
            if(this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.HostedServices.Wait(SyncTime)) { throw SyncException; } hs = true;

            return this.HostedServices?.Keys.OrderBy(_ => _,StringComparer.Ordinal).ToImmutableList();
        }
        catch ( Exception _ ) { Logger?.Error(_,GetHostedServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetHostedServices)]
    public virtual IHostedService? GetHostedService(String? name , AccessKey? key = null)
    {
        DC(); Boolean hs = false;
        try
        {
            if(String.IsNullOrEmpty(name) || this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.HostedServices.Wait(SyncTime)) { throw SyncException; } hs = true;

            IHostedService? service = null; _ = this.HostedServices?.TryGetValue(name,out service);

            return service;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetHostedServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetHostedServices)]
    public virtual IList<IHostedService>? GetHostedServices(AccessKey? key = null)
    {
        DC(); Boolean hs = false;
        try
        {
            if(this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.HostedServices.Wait(SyncTime)) { throw SyncException; } hs = true;

            if(this.HostedServices is not null)
            {
                List<String>? r = null;

                foreach(var _ in this.HostedServices)
                {
                    if(_.Value is ITool t && t.GetDisposed())
                    {
                        r ??= new(); r.Add(_.Key);
                    }
                }

                if(r is not null)
                {
                    foreach(var name in r)
                    {
                        if(this.HostedServices.Remove(name,out IHostedService? removed) && removed is not null) { this.HostedServiceNamesByInstance?.Remove(removed); }
                    }
                }
            }

            return this.HostedServices?.Values.ToImmutableList();
        }
        catch ( Exception _ ) { Logger?.Error(_,GetHostedServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetHostedServicesSnapshot"]/*'/>*/
    private IHostedService[] GetHostedServicesSnapshot()
    {
        Boolean hs = false;
        try
        {
            if(!this.Sync.HostedServices.Wait(SyncTime)) { throw SyncException; } hs = true;

            if(this.HostedServices is not null)
            {
                List<String>? remove = null;

                foreach(var service in this.HostedServices)
                {
                    if(service.Value is ITool tool && tool.GetDisposed())
                    {
                        remove ??= new(); remove.Add(service.Key);
                    }
                }

                if(remove is not null)
                {
                    foreach(var name in remove)
                    {
                        if(this.HostedServices.Remove(name,out IHostedService? removed) && removed is not null) { this.HostedServiceNamesByInstance?.Remove(removed); }
                    }
                }
            }

            if(this.HostedServices is null || this.HostedServices.Count == 0) { return []; }

            IHostedService[] snapshot = new IHostedService[this.HostedServices.Count]; Int32 i = 0;

            foreach(IHostedService service in this.HostedServices.Values) { snapshot[i++] = service; }

            return snapshot;
        }
        finally { if(hs) { this.Sync.HostedServices.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetHostedLifecycleServicesSnapshot"]/*'/>*/
    private static IHostedLifecycleService[] GetHostedLifecycleServicesSnapshot(IHostedService[]? services)
    {
        if(services is null || services.Length == 0) { return []; }

        Int32 count = 0;

        foreach(IHostedService service in services)
        {
            if(service is IHostedLifecycleService) { count++; }
        }

        if(count == 0) { return []; }

        IHostedLifecycleService[] snapshot = new IHostedLifecycleService[count]; Int32 i = 0;

        foreach(IHostedService service in services)
        {
            if(service is IHostedLifecycleService lifecycle) { snapshot[i++] = lifecycle; }
        }

        return snapshot;
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
        DC(); Boolean si = false;
        try
        {
            if(this.Inputs is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            si = TryEnter(this.Sync.Inputs,SyncTime); if(!si) { throw SyncException; }

            if(this.Inputs.TryDequeue(out Object? i)) { if(this.Inputs.Count == 0) { this.Inputs = null; } }

            return i;
        }
        catch ( Exception _ ) { Logger?.Error(_,GetInputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(si) { Exit(this.Sync.Inputs); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetInput)]
    public virtual Queue<Object>? GetInputs(AccessKey? key = null)
    {
        DC(); Boolean si = false;
        try
        {
            if(this.Inputs is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            si = TryEnter(this.Sync.Inputs,SyncTime); if(!si) { throw SyncException; }

            return new(this.Inputs);
        }
        catch ( Exception _ ) { Logger?.Error(_,GetInputsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(si) { Exit(this.Sync.Inputs); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetInstance"]/*'/>*/
    public static ITool? GetInstance(Guid? id)
    {
        try
        {
            if( id is null || id.HasValue is false ) { return null; }

            if(Instances.TryGetValue(id!.Value,out WeakReference<ITool>? w) is false) { return null; }

            if(w.TryGetTarget(out ITool? tool) is false || tool.GetDisposed()) { Instances.TryRemove(id.Value,out _); return null; }

            return tool;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetInstanceFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetInstanceIDs"]/*'/>*/
    public static IList<Guid>? GetInstanceIDs()
    {
        try
        {
            if(Instances.IsEmpty) { return null; }

            List<Guid> i = new(); List<Guid> r = new();

            foreach(var kv in Instances)
            {
                if(kv.Value.TryGetTarget(out ITool? t) && t.GetDisposed() is false) { i.Add(kv.Key); }

                else { r.Add(kv.Key); }
            }

            foreach(var id in r) { Instances.TryRemove(id,out _); }

            return i.Count == 0 ? null : i;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetInstanceIDsFail); if(NoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public LifeCycleState GetLifeCycleState()
    {
        DC(); try { return this.LifeState.State; }

        catch ( Exception _ ) { Logger?.Error(_,GetLifeCycleStateFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return UnInitialized; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean GetLocked()
    {
        DC(); try { return this.Locked; }

        catch ( Exception _ ) { Logger?.Error(_,GetLockedFail,MyTypeName,MyID); throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetLogger"]/*'/>*/
    protected virtual ILogger? GetLogger()
    {
        DC(); try { return this.Logger; }

        catch ( Exception _ ) { Logger?.Error(_,GetLoggerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetOutput)]
    public virtual Object? GetOutput(Guid? id , AccessKey? key = null)
    {
        DC(); Boolean os = false;
        try
        {
            if(this.Outputs is null || id is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.Outputs.Wait(SyncTime)) { throw SyncException; } else { os = true; }

            return GetOutput_NoSync(id);
        }
        catch ( Exception _ ) { Logger?.Error(_,GetOutputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(os) { this.Sync.Outputs.Release(); } }
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
        DC(); Boolean os = false;

        TaskCompletionSource<Object?>? waiter = null;
        TimeSpan effectivetimeout = timeout ?? GetOutputTimeout;
        DateTimeOffset started = DateTimeOffset.Now;

        try
        {
            if(id is null) { return null; }

            cancel ??= CancellationToken.None;

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return null; }

            TimeSpan remaining = GetRemainingTimeout(started,effectivetimeout); if(remaining <= TimeSpan.Zero) { return null; }

            if(!await this.Sync.Outputs.WaitAsync(remaining,cancel.Value).ConfigureAwait(false)) { return null; } os = true;

            if(this.Outputs is not null && this.Outputs.TryGetValue(id.Value,out Object? output)) { return output; }

            waiter = new(TaskCreationOptions.RunContinuationsAsynchronously); this.OutputWaiters ??= new();

            if(this.OutputWaiters.TryGetValue(id.Value,out List<TaskCompletionSource<Object?>>? waiters) is false)
            {
                waiters = new(); this.OutputWaiters.Add(id.Value,waiters);
            }

            waiters.Add(waiter); this.Sync.Outputs.Release(); os = false; remaining = GetRemainingTimeout(started,effectivetimeout);

            if(remaining <= TimeSpan.Zero) { return null; }

            return await waiter.Task.WaitAsync(remaining,cancel.Value).ConfigureAwait(false);
        }
        catch ( OperationCanceledException ) { return null; }

        catch ( TimeoutException ) { return null; }

        catch ( Exception _ ) { Logger?.Error(_,GetOutputAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally
        {
            if(os) { this.Sync.Outputs.Release(); }

            if(waiter is not null && waiter.Task.IsCompleted is false)
            {
                Boolean ws = false;

                try
                {
                    if(this.Sync.Outputs.Wait(SyncTime))
                    {
                        ws = true; this.RemoveOutputWaiter_NoSync(id.HasValue ? id.Value : Guid.Empty,waiter);
                    }
                }
                finally { if(ws) { this.Sync.Outputs.Release(); } }
            }
        }

        static TimeSpan GetRemainingTimeout(DateTimeOffset started , TimeSpan timeout)
        {
            if(timeout == InfiniteTimeSpan) { return timeout; }

            TimeSpan remaining = timeout - ( DateTimeOffset.Now - started ); return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="RemoveOutputWaiter_NoSync"]/*'/>*/
    private void RemoveOutputWaiter_NoSync(Guid id , TaskCompletionSource<Object?> waiter)
    {
        if(this.OutputWaiters is null || this.OutputWaiters.TryGetValue(id,out List<TaskCompletionSource<Object?>>? waiters) is false || waiters.Remove(waiter) is false)
        {
            return;
        }

        if(waiters.Count == 0)
        {
            this.OutputWaiters.Remove(id);

            if(this.OutputWaiters.Count == 0) { this.OutputWaiters = null; }
        }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetOutputIDs)]
    public virtual IList<Guid>? GetOutputIDs(AccessKey? key = null)
    {
        DC(); Boolean os = false;
        try
        {
            if(this.Outputs is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            if(!this.Sync.Outputs.Wait(SyncTime)) { throw SyncException; } else { os = true; }

            return GetOutputIDs_NoSync();
        }
        catch ( Exception _ ) { Logger?.Error(_,GetOutputIDsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(os) { this.Sync.Outputs.Release(); } }
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
        DC();
        try
        {
            cancel ??= CancellationToken.None;

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return null; }

            Object? _ = await this.GetOutputAsync(id,cancel,timeout,interval,key).ConfigureAwait(false); if(_ is not null) { this.RemoveOutput(id,key); } return _;
        }

        catch ( Exception _ ) { Logger?.Error(_,GetRemoveOutputAsyncFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetStatus)]
    public virtual Dictionary<String,Object?>? GetStatus(AccessKey? key = null)
    {
        DC(); Boolean ss = false;
        try
        {
            if(this.Status is null) { return null; }

            if(this.AccessCheck(key) is false) { return null; }

            ss = TryEnter(this.Sync.Status,SyncTime); if(!ss) { throw SyncException; }

            return this.Status.ToDictionary(_=>new String(_.Key),_=>_.Value);
        }
        catch ( Exception _ ) { Logger?.Error(_,GetStatusFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }

        finally { if(ss) { Exit(this.Sync.Status); } }
    }

    ///<inheritdoc/>
    protected override SyncNode GetSyncNode()
    {
        try { this.Sync ??= new(); return this.Sync; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetSyncNodeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetToolDescriptor)]
    public virtual ToolDescriptor? GetToolDescriptor(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return null; }

            return ToolDescriptor.Create(this,this.GetToolManifest(key));
        }
        catch ( Exception _ ) { Logger?.Error(_,GetToolDescriptorFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetToolHost"]/*'/>*/
    public static IToolHost? GetToolHost(IHostedService? service)
    {
        IList<Guid>? ids = GetInstanceIDs(); if(ids is null) { return null; }

        foreach(Guid id in ids)
        {
            ITool? tool = GetInstance(id);

            try
            {
                if(tool?.IsHosting(service) ?? false) { return tool as IToolHost; }
            }
            catch ( ObjectDisposedException ) { continue; }
        }

        return null;
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.GetToolManifest)]
    public virtual ToolManifest? GetToolManifest(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return null; }

            if(ToolManifestRegistry.TryGetManifest(this.GetType(),out ToolManifest manifest)) { return manifest; }

            return ToolManifest.Create(this);
        }
        catch ( Exception _ ) { Logger?.Error(_,GetToolManifestFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
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
            this.GetSyncNode(); base.Initialize(); this.LifeState = new(this.ID); this.LifeState.ToInitialized();

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
            if(this.AccessManager is not null) { return false; }

            if(accessmanager is null) { this.AccessManager = new AccessManager(this,this.GetLogger()); }
            else
            {
                _ = accessmanager.Initialize(this,this.GetLogger()); this.AccessManager = accessmanager;
            }

            this.SelfKey = this.AccessManager.RequestAccess(new ServiceRequest(this,this.GetID()!.ToStringInvariant()!)) as ExecutiveKey ;

            return this.AccessManager is not null && this.SelfKey is not null ? true : throw new SecurityException(InitializeAccessManagementFail);
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

            this.ToolServiceProvider = services;

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

            IList<IHostedService>? services = this.GetHostedServices(this.SelfKey); if(services is null) { return false; }

            if(this.HostedServicesMasked is true)
            {
                Boolean ok = false;

                foreach(IHostedService service in services)
                {
                    if(ReferenceEquals(service,caller)) { ok = true; break; }
                }

                if(ok is false) { return false; }
            }

            foreach(IHostedService service in services)
            {
                if(ReferenceEquals(service,serviceinstance)) { return true; }
            }

            return false;
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

            IList<IHostedService>? services = this.GetHostedServices(this.SelfKey); if(services is null) { return false; }

            if(this.HostedServicesMasked is true)
            {
                Boolean ok = false;

                foreach(IHostedService service in services)
                {
                    if(ReferenceEquals(service,caller)) { ok = true; break; }
                }

                if(ok is false) { return false; }
            }

            foreach(IHostedService service in services)
            {
                if(servicetype.IsAssignableFrom(service.GetType())) { return true; }
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,IsHostingFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean IsHosting<TService>(ITool? caller = null) { return this.IsHosting(typeof(TService),caller); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="IssueMyHostKeys"]/*'/>*/
    protected virtual Boolean IssueMyHostKeys()
    {
        DC();
        try
        {
            IEnumerable<ITool>? s = this.GetHostedServices(this.SelfKey)?.OfType<ITool>(); if(s is null || !s.Any()) { return true; }

            this.HostingKeys ??= new(ReferenceEqualityComparer.Instance); this.HostingMyHostKeys ??= new(ReferenceEqualityComparer.Instance);

            foreach(ITool t in s)
            {
                HostKey? hk = this.HostingKeys.TryGetValue(t,out HostKey? existinghostkey) ? existinghostkey?.Copy<HostKey>() : null;

                MyHostKey? mhk = this.AccessManager!.GenerateAccessKey<MyHostKey>(AccessKeyIssueOptions.Create(subject: t.GetID()!.Value.ToString(),operations: ProtectedOperationSets.MyHost),key:this.SelfKey);

                if(t.SetMyHostKey(hk,mhk) is false) { throw new OperationFailedException(); }

                this.HostingMyHostKeys[t] = mhk!.Copy<MyHostKey>()!;
            }
            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,IssueMyHostKeysFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean Lock(ManagementKey? key)
    {
        DC(); Boolean ls = false;
        try
        {
            if(this.Locked || this.CheckManager(key) is false) { return false; }

            ls = TryEnter(this.Sync.Locked,SyncTime); if(!ls) { throw SyncException; }

            this.Locked = true; return true;
        }
        catch ( Exception _ )
        {
            this.Locked = false; Logger?.Error(_,LockFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw;
        }
        finally { if(ls) { Exit(this.Sync.Locked); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.Lock)]
    public virtual Boolean Lock(AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            ls = TryEnter(this.Sync.Locked,SyncTime); if(!ls) { throw SyncException; }

            this.Locked = true; return true;
        }
        catch ( Exception _ )
        {
            this.Locked = false; Logger?.Error(_,LockFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw;
        }
        finally { if(ls) { Exit(this.Sync.Locked); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="LockHostedServices"]/*'/>*/
    protected virtual Boolean LockHostedServices()
    {
        DC();
        try
        {
            IEnumerable<ITool>? s = this.GetHostedServices(this.SelfKey)?.OfType<ITool>(); if(s is null || !s.Any()) { return true; }

            if(this.HostingManagerKeys is null) { this.HostingManagerKeys = new(ReferenceEqualityComparer.Instance); } if(this.HostedServiceLockState is null) { this.HostedServiceLockState = new(ReferenceEqualityComparer.Instance); }

            foreach(var tool in s)
            {
                Boolean waslocked = tool.GetLocked(); this.HostedServiceLockState[tool] = waslocked;

                if(this.HostingManagerKeys.TryGetValue(tool,out ManagerKey? mk) && mk is not null) { throw new SecurityException(LockHostedServicesDuplicateFail); }

                var c = CreateCertificate(tool,"HostingManagerKey"); if(c is null) { return false; } mk = new ManagerKey(c);

                if(this.HostingManagerKeys.TryAdd(tool,mk.Copy<ManagerKey>()!) is false) { throw new SecurityException(LockHostedServicesFail); }

                if(waslocked is false && tool.RegisterManager(mk) is false)
                {
                    this.HostingManagerKeys.Remove(tool); throw new SecurityException(LockHostedServicesFail);
                }

                if(waslocked is false && tool.Lock(mk) is false) { this.HostingManagerKeys.Remove(tool); throw new SecurityException(LockHostedServicesFail); }
            }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,LockHostedServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.MaskCommandTypes)]
    public Boolean MaskCommandTypes(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            this.CommandTypesMasked = true; return this.CommandTypesMasked;
        }
        catch ( Exception _ ) { Logger?.Error(_,MaskCommandTypesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.MaskHostedServices)]
    public Boolean MaskHostedServices(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            this.HostedServicesMasked = true; return this.HostedServicesMasked;
        }
        catch ( Exception _ ) { Logger?.Error(_,MaskHostedServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean MyExceptionsEnabled() { DC(); return this.ExceptionsEnabled; }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RegisterCommand)]
    public virtual async Task<Boolean> RegisterCommand(String? handle , ICommand? command , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None;
        Boolean cs = false; Boolean ls = false;
        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(String.IsNullOrEmpty(handle) || command is null) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            return await this.RegisterCommand_NoSync(handle,command,permissions,enable,cancel.Value,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,RegisterCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> RegisterCommand(String? handle , Func<ICommand?>? factory , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None;
        Boolean cs = false; Boolean ls = false;
        try
        {
            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(String.IsNullOrEmpty(handle) || factory is null) { return false; }

            ICommand? command = factory(); if(command is null) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            return await this.RegisterCommand_NoSync(handle,command,permissions,enable,cancel.Value,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,RegisterCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> RegisterCommand(String? handle , Type? commandtype , Object?[]? arguments = null , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None;
        Boolean cs = false; Boolean ls = false;
        try
        {
            if(commandtype is null || commandtype.IsAssignableTo(typeof(ICommand)) is false) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            ObjectBuilder b = ObjectBuilder.Create(commandtype);

            if(arguments is not null)
            {
                for(Int32 i = 0; i < arguments.Length; i++)
                {
                    if(b.SetArgument(i,arguments[i]) is false) { return false; }
                }
            }

            if(b.Build() is false || b.Value is not ICommand command) { return false; }

            if(String.IsNullOrEmpty(handle)) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            return await this.RegisterCommand_NoSync(handle,command,permissions,enable,cancel.Value,key).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,RegisterCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="RegisterCommand_NoSync"]/*'/>*/
    protected virtual async Task<Boolean> RegisterCommand_NoSync(String handle , ICommand command , ImmutableArray<Int32>? permissions = null , Boolean enable = false , CancellationToken cancel = default , AccessKey? key = null)
    {
        try
        {
            if((this.Commands?.ContainsKey(handle) ?? false)) { return false; }

            this.Commands ??= new(); this.CommandHandles ??= new(ReferenceEqualityComparer.Instance); this.AttachedCommands ??= new(ReferenceEqualityComparer.Instance);
            this.CommandKeys ??= new(ReferenceEqualityComparer.Instance); this.CommandManagerKeys ??= new(ReferenceEqualityComparer.Instance);

            Boolean attached = this.AttachedCommands?.Contains(command) ?? false;

            if(attached is false)
            {
                attached = await this.AttachCommand_NoSync(command,permissions,enable,cancel,key).ConfigureAwait(false);

                if(attached is false) { return false; }
            }

            if(this.Commands.TryAdd(new(handle),command) is false)
            {
                if(attached) { await this.DetachCommand_NoSync(command,cancel).ConfigureAwait(false); }

                return false;
            }

            if(this.CommandHandles.TryGetValue(command,out HashSet<String>? handles) is false)
            {
                handles = new(); this.CommandHandles.Add(command,handles);
            }

            if(handles.Add(handle) is false)
            {
                this.Commands.Remove(handle);

                if(attached) { await this.DetachCommand_NoSync(command,cancel).ConfigureAwait(false); }

                if(handles.Count == 0) { this.CommandHandles.Remove(command); }

                return false;
            }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,RegisterCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="RegisterCommands"]/*'/>*/
    protected Boolean RegisterCommands(Dictionary<String,ICommand>? commands = null)
    {
        DC(); Boolean cs = false; Boolean ls = false;
        try
        {
            if(commands is null || commands.Count == 0) { return true; }

            if(!this.Sync.Life.Wait(SyncTime)) { throw SyncException; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!this.Sync.Commands.Wait(SyncTime)) { throw SyncException; } cs = true;

            this.Commands ??= new();

            this.CommandHandles ??= new(ReferenceEqualityComparer.Instance); this.CommandKeys ??= new(ReferenceEqualityComparer.Instance);
            this.CommandManagerKeys ??= new(ReferenceEqualityComparer.Instance); this.AttachedCommands ??= new(ReferenceEqualityComparer.Instance);

            foreach(var commandpair in commands)
            {
                String handle = commandpair.Key; ICommand cmd = commandpair.Value;

                if(this.Commands.ContainsKey(handle)) { throw new OperationFailedException(); }

                Boolean attached = this.AttachedCommands?.Contains(cmd) ?? false;

                if(attached is false)
                {
                    attached = this.AttachCommand_NoSync(cmd,null,false,CancellationToken.None,this.SelfKey).GetAwaiter().GetResult();

                    if(attached is false) { throw new OperationFailedException(); }
                }

                if(this.Commands.TryAdd(new(handle),cmd))
                {
                    if(this.CommandHandles.TryGetValue(cmd,out HashSet<String>? handles) is false)
                    {
                        handles = new(); this.CommandHandles.Add(cmd,handles);
                    }

                    if(handles.Add(handle)) { continue; }
                }

                throw new OperationFailedException();
            }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,RegisterCommandsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RegisterManager)]
    public virtual Boolean RegisterManager(ManagementKey? managementkey = null , AccessKey? key = null)
    {
        DC(); Boolean ss = false;
        try
        {
            if(managementkey is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            ss = TryEnter(this.Sync.Secrets,SyncTime); if(!ss) { throw SyncException; }

            this.Secrets ??= new(); Byte[]? _ = CreateSecret(managementkey.Key); if(_ is null) { return false; }

            this.Secrets.Add(_); if(this.Secrets.Contains(_)) { return true; } return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ss) { Exit(this.Sync.Secrets); } }
    }

    ///<inheritdoc/>
    public override Boolean RegisterManager(ManagementKey? managementkey) { return this.RegisterManager(managementkey,null); }

    ///<inheritdoc/>
    public Boolean ReleaseOwnership(ManagementKey? managementkey)
    {
        DC(); Boolean os = false;
        try
        {
            os = TryEnter(this.Sync.OwnerSecret,SyncTime); if(!os) { throw SyncException; }

            if(CheckOwner(managementkey) is true) { ZeroMemory(this.OwnerSecret); this.OwnerSecret = null; return true; }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,ReleaseOwnershipFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(os) { Exit(this.Sync.OwnerSecret); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RemoveActivity)]
    public virtual Boolean RemoveActivity(Activity? activity , AccessKey? key = null)
    {
        DC(); Boolean sa = false;
        try
        {
            if( activity is null || this.Activities is null ) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            sa = TryEnter(this.Sync.Activities,SyncTime); if(!sa) { throw SyncException; }

            if(this.Activities.Remove(activity))
            {
                if(this.Activities.Count == 0) { this.Activities = null; }

                return true;
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveActivityFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(sa) { Exit(this.Sync.Activities); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RemoveDataItem)]
    public virtual Boolean RemoveDataItem(Guid? id , AccessKey? key = null)
    {
        DC(); Boolean ds = false;
        try
        {
            if( id is null || this.Data is null ) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            ds = TryEnter(this.Sync.Data,SyncTime); if(!ds) { throw SyncException; }

            if(this.DataIndex is not null && this.DataIndex.TryGetValue(id.Value,out DataItem? dataitem) && this.Data.Remove(dataitem))
            {
                _ = this.DataIndex.Remove(id.Value);

                if(this.Data.Count == 0)
                {
                    this.Data = null; this.DataIndex = null;
                }

                return true;
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveDataItemFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ds) { Exit(this.Sync.Data); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RemoveHostedService)]
    public virtual async Task<Boolean> RemoveHostedService(IHostedService? service , Boolean stop = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; Boolean hs = false;
        try
        {
            if(this.HostedServices is null) { return false; }

            if(service is null) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(!await this.Sync.HostedServices.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } hs = true;

            if(this.LifeState.HostedServicesChangeOK() is false) { return false; }

            this.EnsureHostedServiceNameIndex_NoSync();
            
            if(this.HostedServiceNamesByInstance is null || this.HostedServiceNamesByInstance.TryGetValue(service,out String? name) is false ||
               String.IsNullOrEmpty(name) || this.HostedServices.TryGetValue(name,out IHostedService? namedservice) is false || namedservice is null)
            { return false; }

            return await this.RemoveHostedService_NoSync(name,namedservice,stop,cancel.Value).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> RemoveHostedService(String? name , Boolean stop = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; Boolean hs = false;
        try
        {
            if(this.HostedServices is null) { return false; }

            if(String.IsNullOrEmpty(name)) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(!await this.Sync.HostedServices.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } hs = true;

            if(this.LifeState.HostedServicesChangeOK() is false) { return false; }

            if(this.HostedServices.TryGetValue(name,out IHostedService? service) is false || service is null) { return false; }

            return await this.RemoveHostedService_NoSync(name,service,stop,cancel.Value).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="RemoveHostedService_NoSync"]/*'/>*/
    protected virtual async Task<Boolean> RemoveHostedService_NoSync(String name , IHostedService service , Boolean stop = false , CancellationToken cancel = default)
    {
        try
        {
            if(String.IsNullOrEmpty(name) || service is null || this.HostedServices is null || this.HostedServices.TryGetValue(name,out IHostedService? s) is false || !ReferenceEquals(service,s)) { return false; }

            if(stop)
            {
                if(await this.StopHostedServiceCore(service,cancel).WaitAsync(this.HostingOptions!.ShutdownTimeout,cancel).ConfigureAwait(false) is false) { return false; }
            }

            if(service is ITool t)
            {
                if(this.HostingKeys is null || this.HostingManagerKeys is null || this.HostingMyHostKeys is null || this.HostedServiceLockState is null) { return false; }

                if(this.HostingKeys.ContainsKey(t) is false || this.HostingManagerKeys.ContainsKey(t) is false || this.HostingMyHostKeys.ContainsKey(t) is false || this.HostedServiceLockState.ContainsKey(t) is false)
                {
                    return false;
                }

                HostKey hk = this.HostingKeys[t].Copy<HostKey>()!; MyHostKey mhk = this.HostingMyHostKeys[t].Copy<MyHostKey>()!;

                ManagerKey hmk = this.HostingManagerKeys[t].Copy<ManagerKey>()!; Boolean waslocked = this.HostedServiceLockState[t];

                this.HostingKeys.Remove(t); this.HostingManagerKeys.Remove(t); this.HostingMyHostKeys.Remove(t); this.HostedServiceLockState.Remove(t);

                if(this.RevokeAccess(mhk) is false) { return false; } _ = ClearKeysUnLock(t,hk,hmk,waslocked is false);
            }

            if(this.HostedServices.Remove(name) is false) { return false; }

            _ = this.HostedServiceNamesByInstance?.Remove(service);

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        async Task ClearKeysUnLock(ITool? tool , HostKey? hostkey , ManagerKey? managerkey , Boolean unlock)
        {
            try
            {
                if(unlock) { if(tool!.UnLock(managerkey) && tool.UnRegisterManager(managerkey,hostkey) is false) { return; } }

                tool?.ClearMyHostKey(hostkey); hostkey?.ClearKey(); managerkey?.ClearKey(); await Task.CompletedTask.ConfigureAwait(false);
            }
            catch ( Exception _ ) { Logger?.Error(_,RemoveHostedServiceClearFail,MyTypeName,MyID); }
        }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="RemoveInstance"]/*'/>*/
    protected static Boolean RemoveInstance(Tool? tool)
    {
        try
        {
            if( tool is null || tool.GetID().HasValue is false ) { return false; }

            return Instances.TryRemove(tool.GetID()!.Value,out _);
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RemoveInstanceFail); if(NoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RemoveOutput)]
    public virtual Boolean RemoveOutput(Guid? id , AccessKey? key = null)
    {
        DC(); Boolean os = false;
        try
        {
            if( id is null || this.Outputs is null ) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            if(!this.Sync.Outputs.Wait(SyncTime)) { throw SyncException; } else { os = true; }

            if(this.Outputs.Remove(id.Value))
            {
                if(this.Outputs.Count == 0) { this.Outputs = null; }

                return true;
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveOutputFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(os) { this.Sync.Outputs.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.RemoveStatus)]
    public virtual Boolean RemoveStatus(String? index , AccessKey? key = null)
    {
        DC(); Boolean ss = false;
        try
        {
            if( index is null || this.Status is null ) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            ss = TryEnter(this.Sync.Status,SyncTime); if(!ss) { throw SyncException; }

            if(this.Status.Remove(index))
            {
                if(this.Status.Count == 0) { this.Status = null; }

                return true;
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,RemoveStatusFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ss) { Exit(this.Sync.Status); } }
    }

    ///<inheritdoc/>
    public virtual AccessKey? RequestAccess(AccessRequest? request = null)
    {
        DC(); try { return request is null ? null : (this.AccessManager?.RequestAccess(request)); }

        catch ( Exception _ ) { Logger?.Error(_,RequestAccessFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual async Task<AccessKey?> RequestAccessAsync(AccessRequest? request = null , CancellationToken? cancel = null)
    {
        DC(); cancel ??= CancellationToken.None;

        try { return request is null ? null : await ( this.AccessManager?.RequestAccessAsync(request,cancel.Value) ?? Task.FromResult<AccessKey?>(null) ).ConfigureAwait(false); }

        catch ( Exception _ ) { Logger?.Error(_,RequestAccessFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="RequestHostingKeys"]/*'/>*/
    protected virtual Boolean RequestHostingKeys()
    {
        DC();
        try
        {
            if(this.HostingKeys is not null) { return false; }

            IEnumerable<ITool>? s = this.GetHostedServices(this.SelfKey)?.OfType<ITool>(); if( s is null || !s.Any() ) { return true; }

            var k = s.Select(tool => new { Tool = tool , HostKey = tool.RequestAccess(new HostRequest(this)) as HostKey }).Where(_ => _.HostKey is not null).ToList();

            if(k.Count != s.Count()) { return false; } this.HostingKeys = new(k.Count,ReferenceEqualityComparer.Instance);

            foreach(var pair in k) { this.HostingKeys[pair.Tool] = pair.HostKey!.Copy<HostKey>()!; }

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

            this.HostedServices = new(StringComparer.Ordinal); this.HostedServiceNamesByInstance = new(ReferenceEqualityComparer.Instance);

            foreach(IHostedService s in this.ToolServices?.GetServices<IHostedService>() ?? Enumerable.Empty<IHostedService>())
            {
                if(this.HostedServiceNamesByInstance.ContainsKey(s)) { continue; }

                String servicename = this.GenerateHostedServiceName_NoSync(); this.HostedServices[servicename] = s; this.HostedServiceNamesByInstance[s] = servicename;
            }

            if(this.HostedServices.Values.OfType<ITool>().Any() is false) { return true; }

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

            this.Logger = (logger ?? this.ToolServices?.GetService<ILoggerFactory>())?
                .CreateLogger(String.Format(ToolLoggerNameFormat,this.GetType().FullName,this.GetID()));

            return this.Logger is not null;
        }
        catch ( Exception _ ) { Logger?.Error(_,ResolveLoggerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public Boolean RevokeAccess(AccessKey? key = null)
    {
        DC(); try { return key is null ? false : this.AccessManager?.RevokeAccess(key) ?? false;  }

        catch ( Exception _ ) { Logger?.Error(_,RevokeAccessFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> RevokeAccessAsync(AccessKey? key = null , CancellationToken? cancel = null)
    {
        DC(); cancel ??= CancellationToken.None;

        try { return key is not null && await ( this.AccessManager?.RevokeAccessAsync(key,cancel.Value) ?? Task.FromResult(false) ).ConfigureAwait(false); }

        catch ( Exception _ ) { Logger?.Error(_,RevokeAccessFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="SetActivityCanceled"]/*'/>*/
    protected virtual void SetActivityCanceled(Activity? activity , String? message = null)
    {
        if(activity?.IsTimedOut() ?? false) { this.SetActivityTimedOut(activity,message); }

        else { activity?.SetRecordByTool(ActivityRecordCode.Canceled,message); }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="SetActivityFailed"]/*'/>*/
    protected virtual void SetActivityFailed(Activity? activity , String? message = null)
    {
        activity?.SetRecordByTool(ActivityRecordCode.Failed,message);
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="SetActivityFaulted"]/*'/>*/
    protected virtual void SetActivityFaulted(Activity? activity , String? message = null)
    {
        activity?.SetRecordByTool(ActivityRecordCode.Faulted,message);
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="SetActivityFaultedException"]/*'/>*/
    protected virtual void SetActivityFaulted(Activity? activity , Exception exception)
    {
        activity?.SetRecordByTool(ActivityRecordCode.Faulted,exception.Message);
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="SetActivitySuccess"]/*'/>*/
    protected virtual void SetActivitySuccess(Activity? activity , String? message = null)
    {
        activity?.SetRecordByTool(ActivityRecordCode.Success,message);
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="SetActivityTimedOut"]/*'/>*/
    protected virtual void SetActivityTimedOut(Activity? activity , String? message = null)
    {
        activity?.SetRecordByTool(ActivityRecordCode.TimedOut,message);
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
                                   ReadAllText(Environment.GetEnvironmentVariable(ConfigurationPathEnvironmentVariable)));

            if(this.Configuration is not null) { return true; } return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,SetConfigurationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetID"]/*'/>*/
    [AccessCheck(ProtectedOperation.SetID)]
    public virtual Boolean SetID(Guid? id , AccessKey? key = null)
    {
        DC(); Boolean si = false;
        try
        {
            if(id is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            si = TryEnter(this.Sync.ID,SyncTime); if(!si) { throw SyncException; }

            if(id == Guid.Empty) { this.ID = null; } else { this.ID = id; }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,SetIDFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(si) { Exit(this.Sync.ID); } }
    }

    ///<inheritdoc/>
    public override Boolean SetID(Guid? id) { return this.SetID(id,null); }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.SetMyHostKey)]
    public virtual Boolean SetMyHostKey(HostKey? hostkey = null , MyHostKey? myhostkey = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(hostkey) is false) { return false; }

            if( myhostkey is null || this.MyHostKey is not null ) { return false; }

            this.MyHostKey = myhostkey.Copy() as MyHostKey; return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,SetMyHostKeyFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.SetStatus)]
    public virtual Boolean SetStatus(String? index , Object? status , AccessKey? key = null)
    {
        DC(); Boolean ss = false;
        try
        {
            if(String.IsNullOrEmpty(index)) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            ss = TryEnter(this.Sync.Status,SyncTime); if(!ss) { throw SyncException; }

            if(this.Status is null) { this.Status = new(); } this.Status[index] = status;

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,SetStatusFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ss) { Exit(this.Sync.Status); } }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> StartHostedService(IHostedService? service , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; Boolean hs = false;
        try
        {
            if(service is null) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(!await this.Sync.HostedServices.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } hs = true;

            this.EnsureHostedServiceNameIndex_NoSync(); if(this.HostedServices is null || this.HostedServiceNamesByInstance is null || this.HostedServiceNamesByInstance.ContainsKey(service) is false) { return false; }

            return await this.StartHostedServiceCore(service,cancel.Value).WaitAsync(this.HostingOptions?.StartupTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> StartHostedService(String? name , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; Boolean hs = false;
        try
        {
            if(String.IsNullOrEmpty(name)) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(!await this.Sync.HostedServices.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } hs = true;

            if(this.HostedServices is null || this.HostedServices.TryGetValue(name,out IHostedService? service) is false || service is null) { return false; }

            return await this.StartHostedServiceCore(service,cancel.Value).WaitAsync(this.HostingOptions?.StartupTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StartHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> StopHostedService(IHostedService? service , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; Boolean hs = false;
        try
        {
            if(service is null) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(!await this.Sync.HostedServices.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } hs = true;

            this.EnsureHostedServiceNameIndex_NoSync(); if(this.HostedServices is null || this.HostedServiceNamesByInstance is null || this.HostedServiceNamesByInstance.ContainsKey(service) is false) { return false; }

            return await this.StopHostedServiceCore(service,cancel.Value).WaitAsync(this.HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    public virtual async Task<Boolean> StopHostedService(String? name , CancellationToken? cancel = null , AccessKey? key = null)
    {
        DC(); cancel ??= CancellationToken.None; Boolean ls = false; Boolean hs = false;
        try
        {
            if(String.IsNullOrEmpty(name)) { return false; }

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(!await this.Sync.HostedServices.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } hs = true;

            if(this.HostedServices is null || this.HostedServices.TryGetValue(name,out IHostedService? service) is false || service is null) { return false; }

            return await this.StopHostedServiceCore(service,cancel.Value).WaitAsync(this.HostingOptions?.ShutdownTimeout ?? InfiniteTimeSpan,cancel.Value).ConfigureAwait(false);
        }
        catch ( Exception _ ) { Logger?.Error(_,StopHostedServiceFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(hs) { this.Sync.HostedServices.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    public Boolean TakeOwnership(ManagementKey? managementkey = null)
    {
        DC(); Boolean os = false;
        if( this.Locked || managementkey is null ) { return false; }
        try
        {
            os = TryEnter(this.Sync.OwnerSecret,SyncTime); if(!os) { throw SyncException; }

            if(this.OwnerSecret is null || CheckOwner(managementkey))
            {
                this.OwnerSecret = CreateSecret(managementkey.Key); if(this.OwnerSecret is null) { return false; }

                return CheckManager(managementkey) ? true : RegisterManager(managementkey);
            }

            return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,TakeOwnershipFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(os) { Exit(this.Sync.OwnerSecret); } }
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
        DC(); Boolean ls = false;
        try
        {
            if(this.Locked is false || this.Secrets is null || this.Secrets.Count == 0) { return false; }

            if(this.CheckOwner(key) is false && this.CheckManager(key) is false) { return false; }

            ls = TryEnter(this.Sync.Locked,SyncTime); if(!ls) { throw SyncException; }

            this.Locked = false; return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,UnLockFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { Exit(this.Sync.Locked); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.UnLock)]
    public virtual Boolean UnLock(AccessKey? key = null)
    {
        DC(); Boolean ls = false;
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            ls = TryEnter(this.Sync.Locked,SyncTime); if(!ls) { throw SyncException; }

            this.Locked = false; return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,UnLockFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ls) { Exit(this.Sync.Locked); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.UnMaskCommandTypes)]
    public Boolean UnMaskCommandTypes(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            this.CommandTypesMasked = false; return !this.CommandTypesMasked;
        }
        catch ( Exception _ ) { Logger?.Error(_,UnMaskCommandTypesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.UnMaskHostedServices)]
    public Boolean UnMaskHostedServices(AccessKey? key = null)
    {
        DC();
        try
        {
            if(this.AccessCheck(key) is false) { return false; }

            this.HostedServicesMasked = false; return !this.HostedServicesMasked;
        }
        catch ( Exception _ ) { Logger?.Error(_,UnMaskHostedServicesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.UnRegisterCommand)]
    public virtual async Task<Boolean> UnRegisterCommand(String? handle , Boolean detach = false , CancellationToken? cancel = null , AccessKey? key = null)
    {
        Boolean cs = false; Boolean ls = false;
        DC();
        try
        {
            if(handle is null || this.Commands is null || this.CommandHandles is null) { return false; }

            cancel ??= CancellationToken.None;

            if(await this.AccessCheckAsync(key,cancel:cancel.Value).ConfigureAwait(false) is false) { return false; }

            if(!await this.Sync.Life.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } ls = true;

            if(this.LifeState.CommandChangeOK() is false) { return false; }

            if(!await this.Sync.Commands.WaitAsync(SyncTime,cancel.Value).ConfigureAwait(false)) { return false; } cs = true;

            if(this.Commands.TryGetValue(handle,out ICommand? c) is false || c is null) { return false; }

            if(this.CommandHandles.TryGetValue(c,out HashSet<String>? handles) is false) { return false; }

            if(handles is null || handles.Contains(handle) is false) { return false; }

            Boolean lasthandle = handles.Count == 1;

            if(detach && lasthandle)
            {
                handles.Remove(handle);

                if(await this.DetachCommand_NoSync(c,cancel.Value).ConfigureAwait(false) is false)
                {
                    handles.Add(handle);

                    return false;
                }
            }
            else { handles.Remove(handle); }

            if(handles.Count == 0) { this.CommandHandles.Remove(c); }

            if((this.Commands.Remove(handle) is false)) { throw new OperationFailedException(); }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,UnRegisterCommandFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(cs) { this.Sync.Commands.Release(); } if(ls) { this.Sync.Life.Release(); } }
    }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.UnRegisterManager)]
    public virtual Boolean UnRegisterManager(ManagementKey? managementkey = null , AccessKey? key = null)
    {
        DC(); Boolean ss = false;
        try
        {
            if(managementkey is null) { return false; }

            if(this.AccessCheck(key) is false) { return false; }

            ss = TryEnter(this.Sync.Secrets,SyncTime); if(!ss) { throw SyncException; }

            if(CheckRemoveSecret(managementkey.Key,this.Secrets)) { return true; } return false;
        }
        catch ( Exception _ ) { Logger?.Error(_,UnRegisterManagerFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(ss) { Exit(this.Sync.Secrets); } }
    }

    ///<inheritdoc/>
    public override Boolean UnRegisterManager(ManagementKey? managementkey) { return this.UnRegisterManager(managementkey,null); }

    ///<inheritdoc/>
    [AccessCheck(ProtectedOperation.UpdateInputs)]
    public virtual Boolean UpdateInputs(IEnumerable<Object>? inputs , AccessKey? key = null)
    {
        DC(); Boolean si = false;
        try
        {
            if(inputs is null) { return false; } Queue<Object> _ = new(inputs);

            if(this.AccessCheck(key) is false) { return false; }

            si = TryEnter(this.Sync.Inputs,SyncTime); if(!si) { throw SyncException; }

            if(_.Count == 0) { this.Inputs = null; } else { this.Inputs = _; }

            return true;
        }
        catch ( Exception _ ) { Logger?.Error(_,UpdateInputsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }

        finally { if(si) { Exit(this.Sync.Inputs); } }
    }
}