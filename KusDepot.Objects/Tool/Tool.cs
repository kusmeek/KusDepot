namespace KusDepot;

/**<include file='Tool.xml' path='Tool/class[@name="Tool"]/main/*'/>*/
[DataContract(Name = "Tool" , Namespace = "KusDepot")]
[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple , InstanceContextMode = InstanceContextMode.Single , Name = "Tool" , Namespace = "KusDepot")]
public class Tool : Common , ICloneable , IComparable<Tool> , IDisposable , IEquatable<Tool> , IExtensibleDataObject , IHostedService , IParsable<Tool> , ITool
{
    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Activities"]/*'/>*/
    [IgnoreDataMember]
    protected List<Activity>? Activities;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Commands"]/*'/>*/
    [DataMember(Name = "Commands" , EmitDefaultValue = true , IsRequired = true)]
    protected Dictionary<String,Command>? Commands;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Container"]/*'/>*/
    [IgnoreDataMember]
    protected Autofac.IContainer? Container;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Controls"]/*'/>*/
    [DataMember(Name = "Controls" , EmitDefaultValue = true , IsRequired = true)]
    protected HashSet<String>? Controls;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Data"]/*'/>*/
    [DataMember(Name = "Data" , EmitDefaultValue = true , IsRequired = true)]
    protected HashSet<DataItem>? Data;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Disposed"]/*'/>*/
    [DataMember(Name = "Disposed" , EmitDefaultValue = true , IsRequired = true)]
    protected Boolean Disposed;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="EventLogs"]/*'/>*/
    [DataMember(Name = "EventLogs" , EmitDefaultValue = true , IsRequired = true)]
    protected Dictionary<Int32,String>? EventLogs;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Inputs"]/*'/>*/
    [DataMember(Name = "Inputs" , EmitDefaultValue = true , IsRequired = true)]
    protected Queue<Object>? Inputs;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Objectives"]/*'/>*/
    [DataMember(Name = "Objectives" , EmitDefaultValue = true , IsRequired = true)]
    protected List<Object>? Objectives;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Outputs"]/*'/>*/
    [DataMember(Name = "Outputs" , EmitDefaultValue = true , IsRequired = true)]
    protected Dictionary<Guid,Object?>? Outputs;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Policies"]/*'/>*/
    [DataMember(Name = "Policies" , EmitDefaultValue = true , IsRequired = true)]
    protected List<Object>? Policies;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Purpose"]/*'/>*/
    [DataMember(Name = "Purpose" , EmitDefaultValue = true , IsRequired = true)]
    protected String? Purpose;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Status"]/*'/>*/
    [DataMember(Name = "Status" , EmitDefaultValue = true , IsRequired = true)]
    protected Dictionary<String,Object?>? Status;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Subordinates"]/*'/>*/
    [DataMember(Name = "Subordinates" , EmitDefaultValue = true , IsRequired = true)]
    protected HashSet<Tool>? Subordinates;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Superior"]/*'/>*/
    [DataMember(Name = "Superior" , EmitDefaultValue = true , IsRequired = true)]
    protected Tool? Superior;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Sync"]/*'/>*/
    [DataMember(Name = "ToolSync" , EmitDefaultValue = true , IsRequired = true)]
    protected ToolSync Sync;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Telemetry"]/*'/>*/
    [DataMember(Name = "Telemetry" , EmitDefaultValue = true , IsRequired = true)]
    protected HashSet<String>? Telemetry;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/field[@name="Traits"]/*'/>*/
    [DataMember(Name = "Traits" , EmitDefaultValue = true , IsRequired = true)]
    protected HashSet<Trait>? Traits;

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public Tool()
    {
        try
        {
            this.Sync = new ToolSync();

            this.Initialize();
        }
        catch ( Exception ) { throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/constructor[@name="Constructor"]/*'/>*/
    public Tool(HashSet<DataItem>? data = null , Guid? id = null , String? name = null , IContainer? container = null , Queue<Object>? inputs = null , HashSet<String>? notes = null , HashSet<String>? tags = null , HashSet<Trait>? traits = null)
    {
        try
        {
            this.Container = container; this.Sync = new ToolSync();

            if(traits is not null) { this.Traits = new HashSet<Trait>(traits); }

            this.AddDataItems(data); this.SetID(id); this.UpdateInputs(inputs); this.SetName(name); this.AddNotes(notes); this.AddTags(tags);

            this.Initialize();
        }
        catch ( Exception ) { throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/finalizer[@name="Finalizer"]/*'/>*/
    ~Tool() { this.Dispose(false); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/event[@name="Alert"]/*'/>*/
    public event EventHandler<AlertEventArgs>? Alert;

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Activate"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean Activate()
    {
        try
        {
            if(!TryEnter(this.Sync.Traits,SyncTime))   { throw TraitsSyncException; }
            if(!TryEnter(this.Sync.Commands,SyncTime)) { throw CommandsSyncException; }
    
            return (this.Commands?.Values.All((_)=>{ return _.Attach(this) && _.Enable(); }) ?? true) && (this.Traits?.All((_)=>{ return _.Bind(this).Result && _.Activate().Result; }) ?? true);
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Commands)) { Exit(this.Sync.Commands); }
                  if(IsEntered(this.Sync.Traits))   { Exit(this.Sync.Traits); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddActivity"]/*'/>*/
    public Boolean AddActivity(Activity? activity)
    {
        try
        {
            if(activity is null) { return false; } if(this.Activities is not null && this.Activities.Contains(activity)) { return true; }

            if(!TryEnter(this.Sync.Activities,SyncTime)) { throw ActivitiesSyncException; }

            if(this.Activities is null) { this.Activities = new List<Activity>(); }

            this.Activities.Add(activity); this.RaiseAlert(this,new ActivityEventArgs(AlertCode.ActivityAdded,activity)); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Activities)) { Exit(this.Sync.Activities); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddDataItems"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean AddDataItems(IEnumerable<DataItem>? data)
    {
        try
        {
            if( data is null || this.Locked ) { return false; }

            HashSet<DataItem> ___ = new(data); if(Equals(___.Count,0)) { return false; }

            HashSet<DataItem> __ = new HashSet<DataItem>();

            foreach(DataItem _ in ___)
            {
                if( _ is GuidReferenceItem) { if(__.Add((GuidReferenceItem)_.Clone())) { continue; } else { return false; } }
                if( _ is GenericItem)       { if(__.Add((GenericItem)_.Clone()))       { continue; } else { return false; } }
                if( _ is TextItem)          { if(__.Add((TextItem)_.Clone()))          { continue; } else { return false; } }
                if( _ is CodeItem)          { if(__.Add((CodeItem)_.Clone()))          { continue; } else { return false; } }
                if( _ is MSBuildItem)       { if(__.Add((MSBuildItem)_.Clone()))       { continue; } else { return false; } }
                if( _ is BinaryItem)        { if(__.Add((BinaryItem)_.Clone()))        { continue; } else { return false; } }
                if( _ is MultiMediaItem)    { if(__.Add((MultiMediaItem)_.Clone()))    { continue; } else { return false; } }
            }

            if(!Equals(__.Count,___.Count)) { return false; }

            if(!TryEnter(this.Sync.Data,SyncTime)) { throw DataSyncException; } if(this.Data is null) { this.Data = new HashSet<DataItem>(); }

            this.Data.UnionWith(__); __.ToList().ForEach((_)=>{this.RaiseAlert(this,new DataItemEventArgs(AlertCode.DataItemAdded,_.GetID()));});

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Data)) { Exit(this.Sync.Data); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddInput"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean AddInput(Object? input)
    {
        try
        {
            if( input is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Inputs,SyncTime)) { throw InputsSyncException; }

            if(this.Inputs is null) { this.Inputs = new Queue<Object>(); }

            this.Inputs.Enqueue(input); this.RaiseAlert(this,new AlertEventArgs(AlertCode.InputAdded));

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Inputs)) { Exit(this.Sync.Inputs); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="AddNotes"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean AddNotes(IEnumerable<String>? notes)
    {
        try
        {
            if( notes is null || this.Locked ) { return false; }

            HashSet<String> _ = new(notes.Select(_=>new String(_))); if(Equals(_.Count,0)) { return false; }

            if(!TryEnter(this.Sync.Notes,SyncTime)) { throw NotesSyncException; }

            if(this.Notes is null) { this.Notes = _; } else { this.Notes.UnionWith(_); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Notes")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Notes)) { Exit(this.Sync.Notes); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddOutput"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean AddOutput(Guid id , Object? output)
    {
        try
        {
            if( this.Locked || Equals(id,Guid.Empty) ) { return false; }

            if(!TryEnter(this.Sync.Outputs,SyncTime)) { throw OutputsSyncException; }

            if(this.Outputs is null) { this.Outputs = new Dictionary<Guid,Object?>(); }

            if(this.Outputs.TryAdd(id,output))
            {
                this.RaiseAlert(this,new OutputEventArgs(AlertCode.OutputAdded,id)); return true;
            }
            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Outputs)) { Exit(this.Sync.Outputs); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="AddTags"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean AddTags(IEnumerable<String>? tags)
    {
        try
        {
            if( tags is null || this.Locked ) { return false; }

            HashSet<String> _ = new(tags.Select(_=>new String(_))); if(Equals(_.Count,0)) { return true; }

            if(!TryEnter(this.Sync.Tags,SyncTime)) { throw TagsSyncException; }

            if(this.Tags is null) { this.Tags = _; } else { this.Tags.UnionWith(_); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Tags")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Tags)) { Exit(this.Sync.Tags); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ClearEventLogs"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean ClearEventLogs()
    {
        try
        {
            if(this.EventLogs is null) { return false; }

            if(!TryEnter(this.Sync.EventLogs,SyncTime)) { throw EventLogsSyncException; }

            this.EventLogs = null; return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.EventLogs) is true) { Exit(this.Sync.EventLogs); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Clone"]/*'/>*/
    public Object Clone()
    {
        try
        {
            #pragma warning disable CA2000

            Tool? _ = Tool.Parse(this.ToString(),null);

            if( _ is not null ) { return _; }

            return new Object();

            #pragma warning restore CA2000
        }
        catch ( Exception ) { if(NoExceptions) { return new Object(); } throw; }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="CompareTo"]/*'/>*/
    public Int32 CompareTo(Tool? other)
    {
        try
        {
            if(other is null)               { return 1; }
            if(ReferenceEquals(this,other)) { return 0; }

            DateTimeOffset? _ = other.GetBornOn();

            if(this.BornOn == _ )           { return 0; }
            if(this.BornOn <  _ )           { return -1; }
            else                            { return 1; }
        }
        catch ( Exception ) { throw; }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Deactivate"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean Deactivate()
    {
        try
        {
            if(!TryEnter(this.Sync.Traits,SyncTime))   { throw TraitsSyncException; }
            if(!TryEnter(this.Sync.Commands,SyncTime)) { throw CommandsSyncException; }

            return (this.Commands?.Values.All((_)=>{ return _.Disable() && _.Detach(); }) ?? true) && (this.Traits?.All((_)=>{ return _.Deactivate().Result; }) ?? true);
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Commands)) { Exit(this.Sync.Commands); }
                  if(IsEntered(this.Sync.Traits))   { Exit(this.Sync.Traits); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Dispose"]/*'/>*/
    public void Dispose() { this.Dispose(true); GC.SuppressFinalize(this); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="DisposeVirtual"]/*'/>*/
    protected virtual void Dispose(Boolean disposing)
    {
        try
        {
            if(this.Disposed) { return; }

            if(disposing)
            {
                this.AcquireLocks();

                this.RaiseAlert(this,new AlertEventArgs(AlertCode.Disposing),true); this.Deactivate(); this.Container?.Dispose();

                this.GetType().GetFields(BindingFlags.Instance|BindingFlags.NonPublic).ToList().ForEach((_)=>{if(!Equals(_.Name,"Sync")){_.SetValue(this,null);}});

                this.ReleaseLocks();
            }
            /*free*/ this.Disposed = true;
        }
        catch ( Exception ) { this.ReleaseLocks(); throw; }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Equals"]/*'/>*/
    public override Boolean Equals(Object? other) { return this.Equals(other as Tool); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(Tool? other)
    {
        try
        {
            if(other is null) { return false; }
                        
            if(ReferenceEquals(this,other)) { return true; }

            if(!TryEnter(this.Sync.Application,SyncTime))       { throw ApplicationSyncException; }

            if(!TryEnter(this.Sync.DistinguishedName,SyncTime)) { throw DistinguishedNameSyncException; }

            if(!String.Equals(this.Application,other.GetApplication(),StringComparison.Ordinal))             { return false; }

            if(!String.Equals(this.DistinguishedName,other.GetDistinguishedName(),StringComparison.Ordinal)) { return false; }

            return this.ID == other.GetID();
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally
        {
            if(IsEntered(this.Sync.DistinguishedName)) { Exit(this.Sync.DistinguishedName); }
            if(IsEntered(this.Sync.Application))       { Exit(this.Sync.Application); }
        }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ExecuteCommand"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Guid? ExecuteCommand(Object[]? details)
    {
        try
        {
            if(details is null || this.Commands is null) { return null; }

            String? handle = details.FirstOrDefault() as String; if(handle is null) { return null; }

            if(!this.Commands.TryGetValue(handle,out Command? cmd) ) { return null; }

            return cmd.ExecuteAsync(details)?.ID;
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="FromFile"]/*'/>*/
    public static Tool? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return null; }

            using(FileStream _0 = new FileStream(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read}))
            {
                using(XmlDictionaryReader _1 = XmlDictionaryReader.CreateBinaryReader(_0,XmlDictionaryReaderQuotas.Max))
                {
                    DataContractSerializer _2 = new DataContractSerializer(typeof(Tool),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                    return (Tool?)_2.ReadObject(_1);
                }
            }
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetActivities"]/*'/>*/
    public List<Activity>? GetActivities()
    {
        try
        {
            if(this.Activities is null) { return null; }

            if(!TryEnter(this.Sync.Activities,SyncTime)) { throw ActivitiesSyncException; }

            return this.Activities.ToList();
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Activities)) { Exit(this.Sync.Activities); } }
    }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetAppDomainID"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Int64? GetAppDomainID() { return this.AppDomainID; }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetAppDomainUID"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Int64? GetAppDomainUID() { return this.AppDomainUID; }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetApplication"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override String? GetApplication() { return this.Application is null ? null : new String(this.Application); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetApplicationVersion"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Version? GetApplicationVersion() { return this.ApplicationVersion is null ? null : new Version(this.ApplicationVersion.ToString()); }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetAssemblyVersion"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Version? GetAssemblyVersion() { return this.AssemblyVersion is null ? null : new Version(this.AssemblyVersion.ToString()); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetBornOn"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override DateTimeOffset? GetBornOn() { return this.BornOn; }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetCertificates"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Dictionary<String,String>? GetCertificates()
    {
        try
        {
            if(this.Certificates is null) { return null; }

            if(!TryEnter(this.Sync.Certificates,SyncTime)) { throw CertificatesSyncException; }

            return this.Certificates.ToDictionary(_=>new String(_.Key),_=>new String(_.Value));
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Certificates)) { Exit(this.Sync.Certificates); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetCommands"]/*'/>*/
    public Dictionary<String,Command>? GetCommands()
    {
        try
        {
            if(this.Commands is null) { return null; }

            if(!TryEnter(this.Sync.Commands,SyncTime)) { throw CommandsSyncException; }

            return this.Commands.ToDictionary(_=>new String(_.Key),_=>_.Value);
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Commands)) { Exit(this.Sync.Commands); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetContainer"]/*'/>*/
    public Autofac.IContainer? GetContainer() { return this.Container; }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetControls"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public HashSet<String>? GetControls()
    {
        try
        {
            if(this.Controls is null) { return null; }

            if(!TryEnter(this.Sync.Controls,SyncTime)) { throw ControlsSyncException; }

            return this.Controls.Select(_=>new String(_)).ToHashSet();
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Controls)) { Exit(this.Sync.Controls); } }
    }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetCPUID"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override String? GetCPUID() { return this.CPUID is null ? null : new String(this.CPUID); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetDataItems"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public HashSet<DataItem>? GetDataItems()
    {
        try
        {
            if(this.Data is null) { return null; }

            if(!TryEnter(this.Sync.Data,SyncTime)) { throw DataSyncException; }

            HashSet<DataItem> __ = new HashSet<DataItem>();

            foreach(DataItem _ in this.Data)
            {
                if( _ is GuidReferenceItem) { if(__.Add((GuidReferenceItem)_.Clone())) { continue; } else { return null; } }
                if( _ is GenericItem)       { if(__.Add((GenericItem)_.Clone()))       { continue; } else { return null; } }
                if( _ is TextItem)          { if(__.Add((TextItem)_.Clone()))          { continue; } else { return null; } }
                if( _ is CodeItem)          { if(__.Add((CodeItem)_.Clone()))          { continue; } else { return null; } }
                if( _ is MSBuildItem)       { if(__.Add((MSBuildItem)_.Clone()))       { continue; } else { return null; } }
                if( _ is BinaryItem)        { if(__.Add((BinaryItem)_.Clone()))        { continue; } else { return null; } }
                if( _ is MultiMediaItem)    { if(__.Add((MultiMediaItem)_.Clone()))    { continue; } else { return null; } }
            }

            if(!Equals(this.Data.Count,__.Count)) { return null; }

            return __;
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Data) is true) { Exit(this.Sync.Data); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetDescriptor"]/*'/>*/
    public Descriptor? GetDescriptor()
    {
        try
        {
            return new Descriptor()
            {
                Application        = this.GetApplication(),
                ApplicationVersion = this.GetApplicationVersion()?.ToString(),
                BornOn             = this.GetBornOn()?.ToString("O"),
                DistinguishedName  = this.GetDistinguishedName(),
                ID                 = this.GetID(),
                Locator            = this.GetLocator()?.ToString(),
                Modified           = this.GetModified()?.ToString("O"),
                Name               = this.GetName(),
                Notes              = this.GetNotes()?.ToHashSet(),
                ObjectType         = this.GetType().Name,
                Purpose            = this.GetPurpose(),
                ServiceVersion     = this.GetServiceVersion()?.ToString(),
                Tags               = this.GetTags()?.ToHashSet(),
                Version            = this.GetVersion()?.ToString()
            };
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetDisposed"]/*'/>*/
    public Boolean GetDisposed() { return this.Disposed; }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetDistinguishedName"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override String? GetDistinguishedName() { return this.DistinguishedName is null ? null : new String(this.DistinguishedName); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetDomainID"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override String? GetDomainID() { return this.DomainID is null ? null : new String(this.DomainID); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetEvent"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public String? GetEvent(Int32? key)
    {
        try
        {
            if(this.EventLogs is null || key is null) { return null; }

            if(!TryEnter(this.Sync.EventLogs,SyncTime)) { throw EventLogsSyncException; }

            this.EventLogs.TryGetValue(key.Value,out String? e); return e;
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.EventLogs) is true) { Exit(this.Sync.EventLogs); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetEventLogs"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Dictionary<Int32,String>? GetEventLogs()
    {
        try
        {
            if(this.EventLogs is null) { return null; }

            if(!TryEnter(this.Sync.EventLogs,SyncTime)) { throw EventLogsSyncException; }

            return this.EventLogs.ToDictionary(_=>_.Key,_=>new String(_.Value));
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.EventLogs) is true) { Exit(this.Sync.EventLogs); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetExtension"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Dictionary<String,Object?>? GetExtension()
    {
        try
        {
            if(this.Extension is null) { return null; }

            if(!TryEnter(this.Sync.Extension,SyncTime)) { throw ExtensionSyncException; }

            return this.Extension.ToDictionary(_=>new String(_.Key),_=>_.Value);
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Extension) is true) { Exit(this.Sync.Extension); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetFILE"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override String? GetFILE() { return this.FILE is null ? null : new String(this.FILE); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetGPS"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override String? GetGPS() { return this.GPS is null ? null : new String(this.GPS); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="GetHashCode"]/*'/>*/
    public override Int32 GetHashCode() { return HashCode.Combine(this.Application,this.BornOn,this.ID,this.Name); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetID"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Guid? GetID() { return this.ID; }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetInput"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Object? GetInput()
    {
        try
        {
            if(this.Inputs is null) { return null; }

            if(!TryEnter(this.Sync.Inputs,SyncTime)) { throw InputsSyncException; }

            if(this.Inputs.TryDequeue(out Object? i)) { if(Equals(this.Inputs.Count,0)) { this.Inputs = null; } }

            this.RaiseAlert(this,new AlertEventArgs(AlertCode.InputRemoved)); return i;
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Inputs) is true) { Exit(this.Sync.Inputs); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetInputs"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Queue<Object>? GetInputs()
    {
        try
        {
            if(this.Inputs is null) { return null; }

            if(!TryEnter(this.Sync.Inputs,SyncTime)) { throw InputsSyncException; }

            return new Queue<Object>(this.Inputs);
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Inputs) is true) { Exit(this.Sync.Inputs); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetLinks"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Dictionary<String,GuidReferenceItem>? GetLinks()
    {
        try
        {
            if(this.Links is null) { return null; }

            if(!TryEnter(this.Sync.Links,SyncTime)) { throw LinksSyncException; }

            return this.Links.ToDictionary(_=>new String(_.Key),_=>(GuidReferenceItem)_.Value.Clone());
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Links) is true) { Exit(this.Sync.Links); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetLocator"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Uri? GetLocator() { return this.Locator is null ? null : new Uri(this.Locator.ToString()); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetLocked"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean GetLocked() { return this.Locked; }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetMachineID"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override String? GetMachineID() { return this.MachineID is null ? null : new String(this.MachineID); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetModified"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override DateTimeOffset? GetModified() { return this.Modified; }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetName"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override String? GetName() { return this.Name is null ? null : new String(this.Name); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetNotes"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override HashSet<String>? GetNotes()
    {
        try
        {
            if(this.Notes is null) { return null; }

            if(!TryEnter(this.Sync.Notes,SyncTime)) { throw NotesSyncException; }

            return this.Notes.Select(_=>new String(_)).ToHashSet();
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Notes)) { Exit(this.Sync.Notes); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetObjectives"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public IList<Object>? GetObjectives()
    {
        try
        {
            if(this.Objectives is null) { return null; }

            if(!TryEnter(this.Sync.Objectives,SyncTime)) { throw ObjectivesSyncException; }

            return this.Objectives.ToList();
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Objectives)) { Exit(this.Sync.Objectives); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetOutput"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Object? GetOutput(Guid? id)
    {
        try
        {
            if(this.Outputs is null) { return null; }

            if(!TryEnter(this.Sync.Outputs,SyncTime)) { throw OutputsSyncException; }

            return id is null ? null : this.Outputs.TryGetValue(id.Value,out Object? o) is true ? o : null;
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Outputs)) { Exit(this.Sync.Outputs); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetOutputIDs"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public IList<Guid>? GetOutputIDs()
    {
        try
        {
            if(this.Outputs is null) { return null; }

            if(!TryEnter(this.Sync.Outputs,SyncTime)) { throw OutputsSyncException; }

            return this.Outputs.Keys.ToList();
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Outputs)) { Exit(this.Sync.Outputs); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetPolicies"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public IList<Object>? GetPolicies()
    {
        try
        {
            if(this.Policies is null) { return null; }

            if(!TryEnter(this.Sync.Policies,SyncTime)) { throw PoliciesSyncException; }

            return this.Policies.ToList();
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Policies)) { Exit(this.Sync.Policies); } }
    }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetProcessID"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Int64? GetProcessID() { return this.ProcessID; }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetPurpose"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public String? GetPurpose() { return this.Purpose is null ? null : new String(this.Purpose); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetSecurityDescriptor"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override String? GetSecurityDescriptor() { return this.SecurityDescriptor is null ? null : new String(this.SecurityDescriptor); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetServiceVersion"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Version? GetServiceVersion() { return this.ServiceVersion is null ? null : new Version(this.ServiceVersion.ToString()); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetStatus"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Dictionary<String,Object?>? GetStatus()
    {
        try
        {
            if(this.Status is null) { return null; }

            if(!TryEnter(this.Sync.Status,SyncTime)) { throw StatusSyncException; }

            return this.Status.ToDictionary(_=>new String(_.Key),_=>_.Value);
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Status)) { Exit(this.Sync.Status); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetSubordinates"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public HashSet<Tool>? GetSubordinates()
    {
        try
        {
            if(this.Subordinates is null) { return null; }

            if(!TryEnter(this.Sync.Subordinates,SyncTime)) { throw SubordinatesSyncException; }

            return this.Subordinates.ToHashSet();
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Subordinates)) { Exit(this.Sync.Subordinates); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetSuperior"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Tool? GetSuperior() { return this.Superior; }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetTags"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override HashSet<String>? GetTags()
    {
        try
        {
            if(this.Tags is null) { return null; }

            if(!TryEnter(this.Sync.Tags,SyncTime)) { throw TagsSyncException; }

            return this.Tags.Select(_=>new String(_)).ToHashSet();
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Tags)) { Exit(this.Sync.Tags); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetTelemetry"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public HashSet<String>? GetTelemetry()
    {
        try
        {
            if(this.Telemetry is null) { return null; }

            if(!TryEnter(this.Sync.Telemetry,SyncTime)) { throw TelemetrySyncException; }

            return this.Telemetry.Select(_=>new String(_)).ToHashSet();
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Telemetry)) { Exit(this.Sync.Telemetry); } }
    }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetThreadID"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Int32? GetThreadID() { return this.ThreadID; }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetVersion"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Version? GetVersion() { return this.Version is null ? null : new Version(this.Version.ToString()); }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="Initialize"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean Initialize()
    {
        try
        {
            if(!TryEnter(this.Sync.AppDomainID,SyncTime))     { throw AppDomainIDSyncException; }
            if(!TryEnter(this.Sync.AssemblyVersion,SyncTime)) { throw AssemblyVersionSyncException; }
            if(!TryEnter(this.Sync.BornOn,SyncTime))          { throw BornOnSyncException; }
            if(!TryEnter(this.Sync.ID,SyncTime))              { throw IDSyncException; }
            if(!TryEnter(this.Sync.MachineID,SyncTime))       { throw MachineIDSyncException; }
            if(!TryEnter(this.Sync.ProcessID,SyncTime))       { throw ProcessIDSyncException; }
            if(!TryEnter(this.Sync.ThreadID,SyncTime))        { throw ThreadIDSyncException; }

            this.AppDomainID     = AppDomain.CurrentDomain.Id;
            this.AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            this.BornOn          = this.BornOn ?? DateTimeOffset.Now;
            this.ID              = this.ID ?? Guid.NewGuid();
            this.MachineID       = Environment.MachineName;
            this.ProcessID       = Environment.ProcessId;
            this.ThreadID        = Native.GetCurrentThreadId() ?? Environment.CurrentManagedThreadId;

            if(new List<Object?>(){this.AppDomainID,this.AssemblyVersion,this.BornOn,this.ID,this.MachineID,this.ProcessID,this.ThreadID}.Any(_=>_ is null)) { throw new InitializationException(); }

            this.RaiseAlert(this,new AlertEventArgs(AlertCode.Initialized)); return true;
        }
        catch ( InitializationException ) { if(NoExceptions) { return false; } throw; }

        catch ( Exception _ ) { if(NoExceptions) { return false; } throw new InitializationException(_.Message,_); }

        finally
        {
            if(IsEntered(this.Sync.ThreadID))        { Exit(this.Sync.ThreadID); }
            if(IsEntered(this.Sync.ProcessID))       { Exit(this.Sync.ProcessID); }
            if(IsEntered(this.Sync.MachineID))       { Exit(this.Sync.MachineID); }
            if(IsEntered(this.Sync.ID))              { Exit(this.Sync.ID); }
            if(IsEntered(this.Sync.BornOn))          { Exit(this.Sync.BornOn); }
            if(IsEntered(this.Sync.AssemblyVersion)) { Exit(this.Sync.AssemblyVersion); }
            if(IsEntered(this.Sync.AppDomainID))     { Exit(this.Sync.AppDomainID); }
        }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="Lock"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean Lock(String? secret)
    {
        try
        {
            if( secret is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Locked,SyncTime)) { throw LockedSyncException; }

            if(!TryEnter(this.Sync.Secret,SyncTime)) { throw SecretSyncException; }

            this.Secret = new String(secret); this.Locked = true;

            this.RaiseAlert(this,new AlertEventArgs(AlertCode.Locked)); return true;
        }
        catch ( Exception ) { this.Secret = null; this.Locked = false; if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secret)) { Exit(this.Sync.Secret); }
                  if(IsEntered(this.Sync.Locked)) { Exit(this.Sync.Locked); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="LogEvent"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean LogEvent(String? eventdata)
    {
        try
        {
            if(eventdata is null) { return false; }

            if(!TryEnter(this.Sync.EventLogs,SyncTime)) { throw EventLogsSyncException; }

            if(this.EventLogs is null) { this.EventLogs = new Dictionary<Int32,String>(); }

            if(this.EventLogs.TryAdd(this.EventLogs.Count,eventdata))
            {
                this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"EventLogs")); return true;
            }
            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.EventLogs) is true) { Exit(this.Sync.EventLogs); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
        this.Sync = new ToolSync(); this.Initialize(); this.Activate();

        this.RaiseAlert(this,new AlertEventArgs(AlertCode.DeserializationComplete));
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="Parse"]/*'/>*/
    public static Tool Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using(XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max))
            {
                DataContractSerializer _1 = new DataContractSerializer(typeof(Tool),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                Tool? _2 = (Tool?)_1.ReadObject(_0); if( _2 is not null ) { return _2; }

                throw new FormatException();
            }
        }
        catch ( Exception ) { if(NoExceptions) { return new Tool(); } throw; }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RaiseAlert"]/*'/>*/
    public void RaiseAlert(Object? sender , AlertEventArgs eventargs , Boolean synchronous = false)
    {
        try
        {
            if(this.Alert is null) { return; }

            if(synchronous) { this.Alert(sender,eventargs); }

            else { Task.Run( () => { this.Alert(sender,eventargs); }); }
        }
        catch ( Exception ) { if(NoExceptions) { } throw; }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RegisterCommand"]/*'/>*/
    public Boolean RegisterCommand(Command command , String handle)
    {
        try
        {
            if( command is null || handle is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Commands,SyncTime)) { throw CommandsSyncException; }

            if(this.Commands is null) { this.Commands = new Dictionary<String,Command>(); }

            if(this.Commands.ContainsKey(handle)) { return false; }

            if(this.Commands.TryAdd(new String(handle),command))
            {
                if(command.Attach(this) is false) { this.Commands.Remove(handle); return false; }

                this.RaiseAlert(this,new CommandEventArgs(AlertCode.CommandRegistered,handle,command.GetID())); return true;
            }
            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Commands)) { Exit(this.Sync.Commands); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveActivity"]/*'/>*/
    public Boolean RemoveActivity(Activity? activity)
    {
        try
        {
            if( activity is null || this.Activities is null ) { return false; }

            if(!TryEnter(this.Sync.Activities,SyncTime)) { throw ActivitiesSyncException; }

            if(this.Activities.Remove(activity))
            {
                if(Equals(this.Activities.Count,0)) { this.Activities = null; }

                this.RaiseAlert(this,new ActivityEventArgs(AlertCode.ActivityRemoved,activity)); return true;
            }
            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Activities)) { Exit(this.Sync.Activities); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveDataItem"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean RemoveDataItem(Guid? id)
    {
        try
        {
            if( id is null || this.Data is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Data,SyncTime)) { throw DataSyncException; }

            DataItem? _ = this.Data.FirstOrDefault(_=>_.GetID().Equals(id));

            if( _ is not null)
            {
                if(this.Data.Remove(_))
                {
                    if(Equals(this.Data.Count,0)) { this.Data = null; }

                    this.RaiseAlert(this,new DataItemEventArgs(AlertCode.DataItemRemoved,id)); return true;
                }
            }
            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Data)) { Exit(this.Sync.Data); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="RemoveNote"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean RemoveNote(String? note)
    {
        try
        {
            if( note is null || this.Notes is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Notes,SyncTime)) { throw NotesSyncException; }

            if(this.Notes.Remove(note))
            {
                if(Equals(this.Notes.Count,0)) { this.Notes = null; }

                this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Notes")); return true;
            }
            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Notes)) { Exit(this.Sync.Notes); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveOutput"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean RemoveOutput(Guid? id)
    {
        try
        {
            if( id is null || this.Outputs is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Outputs,SyncTime)) { throw OutputsSyncException; }

            if(this.Outputs.Remove(id.Value))
            {
                if(Equals(this.Outputs.Count,0)) { this.Outputs = null; }

                this.RaiseAlert(this,new OutputEventArgs(AlertCode.OutputRemoved,id.Value)); return true;
            }
            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Outputs)) { Exit(this.Sync.Outputs); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveStatus"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean RemoveStatus(String? key)
    {
        try
        {
            if(key is null || this.Status is null) { return false; }

            if(!TryEnter(this.Sync.Status,SyncTime)) { throw StatusSyncException; }

            if(this.Status.Remove(key))
            {
                if(Equals(this.Status.Count,0)) { this.Status = null; }

                this.RaiseAlert(this,new AlertEventArgs(AlertCode.StatusUpdated)); return true;
            }
            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Status)) { Exit(this.Sync.Status); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="RemoveTag"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean RemoveTag(String? tag)
    {
        try
        {
            if( tag is null || this.Tags is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Tags,SyncTime)) { throw TagsSyncException; }

            if(this.Tags.Remove(tag))
            {
                if(Equals(this.Tags.Count,0)) { this.Tags = null; }

                this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Tags")); return true;
            }
            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Tags)) { Exit(this.Sync.Tags); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Serialize"]/*'/>*/
    [OperationBehavior(ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public String Serialize() { return this.ToString(); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetApplication"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetApplication(String? application)
    {
        try
        {
            if( application is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Application,SyncTime)) { throw ApplicationSyncException; }

            if(String.IsNullOrEmpty(application)) { this.Application = null; }

            else { this.Application = new String(application); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Application")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Application)) { Exit(this.Sync.Application); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetApplicationVersion"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetApplicationVersion(Version? applicationversion)
    {
        try
        {
            if( applicationversion is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.ApplicationVersion,SyncTime)) { throw ApplicationVersionSyncException; }

            if(Version.Equals(applicationversion,Version.Parse("0.0.0.0"))) { this.ApplicationVersion = null; }

            else { this.ApplicationVersion = new Version(applicationversion.ToString()); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"ApplicationVersion")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.ApplicationVersion)) { Exit(this.Sync.ApplicationVersion); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetBornOn"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetBornOn(DateTimeOffset? bornon)
    {
        try
        {
            if( bornon is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.BornOn,SyncTime)) { throw BornOnSyncException; }

            if(Equals(bornon,DateTimeOffset.MinValue)) { this.BornOn = null; } else { this.BornOn = bornon; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"BornOn")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.BornOn)) { Exit(this.Sync.BornOn); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetCertificates"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetCertificates(IDictionary<String,String>? certificates)
    {
        try
        {
            if( certificates is null || this.Locked ) { return false; }

            Dictionary<String,String> _ = certificates.ToDictionary(_=>new String(_.Key),_=>new String(_.Value));

            if(!TryEnter(this.Sync.Certificates,SyncTime)) { throw CertificatesSyncException; }

            if(Equals(_.Count,0)) { this.Certificates = null; } else { this.Certificates = _; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Certificates")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Certificates)) { Exit(this.Sync.Certificates); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetContainer"]/*'/>*/
    public Boolean SetContainer(Autofac.IContainer? container)
    {
        try
        {
            if( container is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Container,SyncTime)) { throw ContainerSyncException; }

            this.Container = container; this.RaiseAlert(this,new AlertEventArgs(AlertCode.ContainerChanged));

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Container)) { Exit(this.Sync.Container); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetControls"]/*'/>*/
    public Boolean SetControls(IEnumerable<String>? controls)
    {
        try
        {
            if( controls is null || this.Locked ) { return false; }

            HashSet<String> _ = new HashSet<String>(controls.Select(_=>new String(_)));

            if(!TryEnter(this.Sync.Controls,SyncTime)) { throw ControlsSyncException; }

            if(Equals(_.Count,0)) { this.Controls = null; } else { this.Controls = _; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Controls")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Controls)) { Exit(this.Sync.Controls); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetDistinguishedName"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetDistinguishedName(String? distinguishedname)
    {
        try
        {
            if( distinguishedname is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.DistinguishedName,SyncTime)) { throw DistinguishedNameSyncException; }

            if(String.IsNullOrEmpty(distinguishedname)) { this.DistinguishedName = null; }

            else { this.DistinguishedName = new String(distinguishedname); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"DistinguishedName")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.DistinguishedName)) { Exit(this.Sync.DistinguishedName); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetDomainID"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetDomainID(String? domainid)
    {
        try
        {
            if( domainid is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.DomainID,SyncTime)) { throw DomainIDSyncException; }

            if(String.IsNullOrEmpty(domainid)) { this.DomainID = null; } else { this.DomainID = new String(domainid); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"DomainID")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.DomainID)) { Exit(this.Sync.DomainID); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetExtension"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetExtension(IDictionary<String,Object?>? extension)
    {
        try
        {
            if( extension is null || this.Locked ) { return false; }

            Dictionary<String,Object?> _ = extension.ToDictionary(_=>new String(_.Key),_=>_.Value);

            if(!TryEnter(this.Sync.Extension,SyncTime)) { throw ExtensionSyncException; }

            if(Equals(_.Count,0)) { this.Extension = null; } else { this.Extension = _; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Extension")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Extension)) { Exit(this.Sync.Extension); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetFILE"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetFILE(String? file)
    {
        try
        {
            if( file is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.FILE,SyncTime)) { throw FILESyncException; }

            if(String.IsNullOrEmpty(file)) { this.FILE = null; } else { this.FILE = new String(file); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"FILE")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.FILE)) { Exit(this.Sync.FILE); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetGPS"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetGPS(String? gps)
    {
        try
        {
            if( gps is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.GPS,SyncTime)) { throw GPSSyncException; }

            if(String.IsNullOrEmpty(gps)) { this.GPS = null; } else { this.GPS = new String(gps); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"GPS")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.GPS)) { Exit(this.Sync.GPS); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetID"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetID(Guid? id)
    {
        try
        {
            if( id is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.ID,SyncTime)) { throw IDSyncException; }

            if(Guid.Equals(id,Guid.Empty)) { this.ID = null; } else { this.ID = id; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"ID")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.ID)) { Exit(this.Sync.ID); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetLinks"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetLinks(IDictionary<String,GuidReferenceItem>? links)
    {
        try
        {
            if( links is null || this.Locked ) { return false; }

            Dictionary<String,GuidReferenceItem> _ = links.ToDictionary(_=>new String(_.Key),_=>(GuidReferenceItem)_.Value.Clone());

            if(!TryEnter(this.Sync.Links,SyncTime)) { throw LinksSyncException; }

            if(Equals(_.Count,0)) { this.Links = null; } else { this.Links = _; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Links")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Links)) { Exit(this.Sync.Links); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetLocator"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetLocator(Uri? locator)
    {
        try
        {
            if( locator is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Locator,SyncTime)) { throw LocatorSyncException; }

            if(Uri.Equals(locator,new Uri("null:"))) { this.Locator = null; } else { this.Locator = new Uri(locator.ToString()); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Locator")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Locator)) { Exit(this.Sync.Locator); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetModified"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetModified(DateTimeOffset? modified)
    {
        try
        {
            if( modified is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Modified,SyncTime)) { throw ModifiedSyncException; }

            if(Equals(modified,DateTimeOffset.MinValue)) { this.Modified = null; } else { this.Modified = modified; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Modified")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Modified)) { Exit(this.Sync.Modified); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetName"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetName(String? name)
    {
        try
        {
            if( name is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Name,SyncTime)) { throw NameSyncException; }

            if(String.IsNullOrEmpty(name)) { this.Name = null; } else { this.Name = new String(name); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Name")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Name)) { Exit(this.Sync.Name); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetObjectives"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean SetObjectives(IEnumerable<Object>? objectives)
    {
        try
        {
            if( objectives is null || this.Locked ) { return false; } List<Object> _ = objectives.ToList();

            if(!TryEnter(this.Sync.Objectives,SyncTime)) { throw ObjectivesSyncException; }

            if(Equals(_.Count,0)) { this.Objectives = null; } else { this.Objectives = _; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Objectives")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Objectives)) { Exit(this.Sync.Objectives); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetPolicies"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean SetPolicies(IEnumerable<Object>? policies)
    {
        try
        {
            if( policies is null || this.Locked ) { return false; } List<Object> _ = policies.ToList();

            if(!TryEnter(this.Sync.Policies,SyncTime)) { throw PoliciesSyncException; }

            if(Equals(_.Count,0)) { this.Policies = null; } else { this.Policies = _; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Policies")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Policies)) { Exit(this.Sync.Policies); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetPurpose"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean SetPurpose(String? purpose)
    {
        try
        {
            if( purpose is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Purpose,SyncTime)) { throw PurposeSyncException; }

            if(String.IsNullOrEmpty(purpose)) { this.Purpose = null; } else { this.Purpose = new String(purpose); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Purpose")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Purpose)) { Exit(this.Sync.Purpose); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetSecurityDescriptor"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetSecurityDescriptor(String? securitydescriptor)
    {
        try
        {
            if( securitydescriptor is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.SecurityDescriptor,SyncTime)) { throw SecurityDescriptorSyncException; }

            if(String.IsNullOrEmpty(securitydescriptor)) { this.SecurityDescriptor = null; }

            else { this.SecurityDescriptor = new String(securitydescriptor); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"SecurityDescriptor")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.SecurityDescriptor)) { Exit(this.Sync.SecurityDescriptor); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetServiceVersion"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetServiceVersion(Version? serviceversion)
    {
        try
        {
            if( serviceversion is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.ServiceVersion,SyncTime)) { throw ServiceVersionSyncException; }

            if(Version.Equals(serviceversion,Version.Parse("0.0.0.0"))) { this.ServiceVersion = null; }

            else { this.ServiceVersion = new Version(serviceversion.ToString()); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"ServiceVersion")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.ServiceVersion)) { Exit(this.Sync.ServiceVersion); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetStatus"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean SetStatus(String? key , Object? status)
    {
        try
        {
            if(key is null) { return false; }

            if(!TryEnter(this.Sync.Status,SyncTime)) { throw StatusSyncException; }

            if(this.Status is null) { this.Status = new Dictionary<String,Object?>(); }

            if(this.Status.TryAdd(key,status)) { this.RaiseAlert(this,new AlertEventArgs(AlertCode.StatusUpdated)); return true; }

            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Status)) { Exit(this.Sync.Status); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetSubordinates"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean SetSubordinates(HashSet<Tool>? subordinates)
    {
        try
        {
            if( subordinates is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Subordinates,SyncTime)) { throw SubordinatesSyncException; }

            if(Equals(subordinates.Count,0)) { this.Subordinates = null; }

            else { this.Subordinates = new HashSet<Tool>(subordinates); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Subordinates")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Subordinates)) { Exit(this.Sync.Subordinates); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetSuperior"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean SetSuperior(Tool? superior)
    {
        try
        {
            if(this.Locked) { return false; }

            if(!TryEnter(this.Sync.Superior,SyncTime)) { throw SuperiorSyncException; }

            this.Superior = superior;

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Superior")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Superior)) { Exit(this.Sync.Superior); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetTelemetry"]/*'/>*/
    public Boolean SetTelemetry(IEnumerable<String>? telemetry)
    {
        try
        {
            if( telemetry is null || this.Locked ) { return false; }

            HashSet<String> _ = new HashSet<String>(telemetry.Select(_=>new String(_)));

            if(!TryEnter(this.Sync.Telemetry,SyncTime)) { throw TelemetrySyncException; }

            if(Equals(_.Count,0)) { this.Telemetry = null; } else { this.Telemetry = _; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Telemetry")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Telemetry)) { Exit(this.Sync.Telemetry); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetVersion"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean SetVersion(Version? version)
    {
        try
        {
            if( version is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Version,SyncTime)) { throw VersionSyncException; }

            if(Version.Equals(version,Version.Parse("0.0.0.0"))) { this.Version = null; }

            else { this.Version = new Version(version.ToString()); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Version")); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Version)) { Exit(this.Sync.Version); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartAsync"]/*'/>*/
    public Task StartAsync(CancellationToken token) { return Task.FromResult(this.Activate()); }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StopAsync"]/*'/>*/
    public Task StopAsync(CancellationToken token) { return Task.FromResult(this.Deactivate()); }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="ToFile"]/*'/>*/
    public Boolean ToFile(String path)
    {
        try
        {
            if(path is null) { return false; }

            if(File.Exists(path)) { return false; }

            this.AcquireLocks(); this.RaiseAlert(this,new AlertEventArgs(AlertCode.SerializationStarted),true);

            using(FileStream _0 = new FileStream(path,new FileStreamOptions(){Access = FileAccess.Write , Mode = FileMode.CreateNew , Share = FileShare.None}))
            {
                using(XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0))
                {
                    DataContractSerializer _2 = new DataContractSerializer(typeof(Tool),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                    _2.WriteObject(_1,this); _1.Flush(); this.RaiseAlert(this,new AlertEventArgs(AlertCode.SerializationComplete)); return true;
                }
            }
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { this.ReleaseLocks(); }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ToString"]/*'/>*/
    public override String ToString()
    {
        try
        {
            this.AcquireLocks(); this.RaiseAlert(this,new AlertEventArgs(AlertCode.SerializationStarted),true); MemoryStream _0 = new MemoryStream();

            using(XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0))
            {
                DataContractSerializer _2 = new DataContractSerializer(typeof(Tool),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

                this.RaiseAlert(this,new AlertEventArgs(AlertCode.SerializationComplete)); return _0.ToArray().ToBase64FromByteArray();
            }
        }
        catch ( Exception ) { if(NoExceptions) { return String.Empty; } throw; }

        finally { this.ReleaseLocks(); }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out Tool tool)
    {
        tool = null; if(input is null) { return false; }

        try
        {
            using(XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max))
            {
                DataContractSerializer _1 = new DataContractSerializer(typeof(Tool),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                Tool? _2 = (Tool?)_1.ReadObject(_0); if( _2 is not null ) { tool = _2; return true; }

                return false;
            }
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="UnLock"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public override Boolean UnLock(String? secret)
    {
        try
        {
            if( secret is null || !this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Locked,SyncTime)) { throw LockedSyncException; }

            if(!TryEnter(this.Sync.Secret,SyncTime)) { throw SecretSyncException; }

            if(String.Equals(this.Secret,secret,StringComparison.Ordinal))
            {
                this.Secret = null; this.Locked = false;

                this.RaiseAlert(this,new AlertEventArgs(AlertCode.UnLocked)); return true;
            }
             return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secret)) { Exit(this.Sync.Secret); }
                  if(IsEntered(this.Sync.Locked)) { Exit(this.Sync.Locked); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="UnRegisterCommand"]/*'/>*/
    public Boolean UnRegisterCommand(String? handle)
    {
        try
        {
            if( handle is null || this.Commands is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Commands,SyncTime)) { throw CommandsSyncException; }

            if(this.Commands.TryGetValue(handle,out Command? c))
            {
                if(c.Disable() && c.Detach())
                {
                    if(this.Commands.Remove(handle) is false) { return false; }

                    this.RaiseAlert(this,new CommandEventArgs(AlertCode.CommandUnregistered,handle,c?.GetID()));

                    if(Equals(this.Commands.Count,0)) { this.Commands = null; } return true;
                }
            }
            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Commands)) { Exit(this.Sync.Commands); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="UpdateInputs"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean UpdateInputs(Queue<Object>? inputs)
    {
        try
        {
            if( inputs is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Inputs,SyncTime)) { throw InputsSyncException; }

            if(Equals(inputs.Count,0)) { this.Inputs = null; } else { this.Inputs = new Queue<Object>(inputs); }

            this.RaiseAlert(this,new AlertEventArgs(AlertCode.InputUpdated)); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Inputs)) { Exit(this.Sync.Inputs); } }
    }

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="WriteEventLogsToFile"]/*'/>*/
    [OperationBehavior(AutoDisposeParameters = false , ReleaseInstanceMode = ReleaseInstanceMode.None)]
    public Boolean WriteEventLogsToFile(String path)
    {
        try
        {
            if(path is null) { return false; }

            if(File.Exists(path)) { return false; }

            if(this.EventLogs is null) { return false; }

            if(!TryEnter(this.Sync.EventLogs,SyncTime)) { throw EventLogsSyncException; }

            using(FileStream _0 = new FileStream(path,new FileStreamOptions(){Access = FileAccess.Write , Mode = FileMode.CreateNew , Share = FileShare.None}))
            {
                using(XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateTextWriter(_0))
                {
                    DataContractSerializer _2 = new DataContractSerializer(typeof(Dictionary<Int32,String>),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                    _2.WriteObject(_1,this.EventLogs); _1.Flush(); return true;
                }
            }
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.EventLogs) is true) { Exit(this.Sync.EventLogs); } }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="AcquireLocks"]/*'/>*/
    private void AcquireLocks()
    {
        if(!TryEnter(this.Sync.Activities,SyncTime))         { throw ActivitiesSyncException; }
        if(!TryEnter(this.Sync.AppDomainID,SyncTime))        { throw AppDomainIDSyncException; }
        if(!TryEnter(this.Sync.AppDomainUID,SyncTime))       { throw AppDomainUIDSyncException; }
        if(!TryEnter(this.Sync.Application,SyncTime))        { throw ApplicationSyncException; }
        if(!TryEnter(this.Sync.ApplicationVersion,SyncTime)) { throw ApplicationVersionSyncException; }
        if(!TryEnter(this.Sync.AssemblyVersion,SyncTime))    { throw AssemblyVersionSyncException; }
        if(!TryEnter(this.Sync.BornOn,SyncTime))             { throw BornOnSyncException; }
        if(!TryEnter(this.Sync.Certificates,SyncTime))       { throw CertificatesSyncException; }
        if(!TryEnter(this.Sync.Commands,SyncTime))           { throw CommandsSyncException; }
        if(!TryEnter(this.Sync.Container,SyncTime))          { throw ContainerSyncException; }
        if(!TryEnter(this.Sync.Controls,SyncTime))           { throw ControlsSyncException; }
        if(!TryEnter(this.Sync.CPUID,SyncTime))              { throw CPUIDSyncException; }
        if(!TryEnter(this.Sync.Data,SyncTime))               { throw DataSyncException; }
        if(!TryEnter(this.Sync.DistinguishedName,SyncTime))  { throw DistinguishedNameSyncException; }
        if(!TryEnter(this.Sync.DomainID,SyncTime))           { throw DomainIDSyncException; }
        if(!TryEnter(this.Sync.EventLogs,SyncTime))          { throw EventLogsSyncException; }
        if(!TryEnter(this.Sync.Extension,SyncTime))          { throw ExtensionSyncException; }
        if(!TryEnter(this.Sync.FILE,SyncTime))               { throw FILESyncException; }
        if(!TryEnter(this.Sync.GPS,SyncTime))                { throw GPSSyncException; }
        if(!TryEnter(this.Sync.ID,SyncTime))                 { throw IDSyncException; }
        if(!TryEnter(this.Sync.Inputs,SyncTime))             { throw InputsSyncException; }
        if(!TryEnter(this.Sync.Links,SyncTime))              { throw LinksSyncException; }
        if(!TryEnter(this.Sync.Locator,SyncTime))            { throw LocatorSyncException; }
        if(!TryEnter(this.Sync.Locked,SyncTime))             { throw LockedSyncException; }
        if(!TryEnter(this.Sync.MachineID,SyncTime))          { throw MachineIDSyncException; }
        if(!TryEnter(this.Sync.Modified,SyncTime))           { throw ModifiedSyncException; }
        if(!TryEnter(this.Sync.Name,SyncTime))               { throw NameSyncException; }
        if(!TryEnter(this.Sync.Notes,SyncTime))              { throw NotesSyncException; }
        if(!TryEnter(this.Sync.Objectives,SyncTime))         { throw ObjectivesSyncException; }
        if(!TryEnter(this.Sync.Outputs,SyncTime))            { throw OutputsSyncException; }
        if(!TryEnter(this.Sync.Policies,SyncTime))           { throw PoliciesSyncException; }
        if(!TryEnter(this.Sync.ProcessID,SyncTime))          { throw ProcessIDSyncException; }
        if(!TryEnter(this.Sync.Purpose,SyncTime))            { throw PurposeSyncException; }
        if(!TryEnter(this.Sync.Secret,SyncTime))             { throw SecretSyncException; }
        if(!TryEnter(this.Sync.SecurityDescriptor,SyncTime)) { throw SecurityDescriptorSyncException; }
        if(!TryEnter(this.Sync.ServiceVersion,SyncTime))     { throw ServiceVersionSyncException; }
        if(!TryEnter(this.Sync.Status,SyncTime))             { throw StatusSyncException; }
        if(!TryEnter(this.Sync.Subordinates,SyncTime))       { throw SubordinatesSyncException; }
        if(!TryEnter(this.Sync.Superior,SyncTime))           { throw SuperiorSyncException; }
        if(!TryEnter(this.Sync.Tags,SyncTime))               { throw TagsSyncException; }
        if(!TryEnter(this.Sync.Telemetry,SyncTime))          { throw TelemetrySyncException; }
        if(!TryEnter(this.Sync.ThreadID,SyncTime))           { throw ThreadIDSyncException; }
        if(!TryEnter(this.Sync.Traits,SyncTime))             { throw TraitsSyncException; }
        if(!TryEnter(this.Sync.Version,SyncTime))            { throw VersionSyncException; }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/method[@name="ReleaseLocks"]/*'/>*/
    private void ReleaseLocks()
    {
        if(IsEntered(this.Sync.Version))            { Exit(this.Sync.Version); }
        if(IsEntered(this.Sync.Traits))             { Exit(this.Sync.Traits); }
        if(IsEntered(this.Sync.ThreadID))           { Exit(this.Sync.ThreadID); }
        if(IsEntered(this.Sync.Telemetry))          { Exit(this.Sync.Telemetry); }
        if(IsEntered(this.Sync.Tags))               { Exit(this.Sync.Tags); }
        if(IsEntered(this.Sync.Superior))           { Exit(this.Sync.Superior); }
        if(IsEntered(this.Sync.Subordinates))       { Exit(this.Sync.Subordinates); }
        if(IsEntered(this.Sync.Status))             { Exit(this.Sync.Status); }
        if(IsEntered(this.Sync.ServiceVersion))     { Exit(this.Sync.ServiceVersion); }
        if(IsEntered(this.Sync.SecurityDescriptor)) { Exit(this.Sync.SecurityDescriptor); }
        if(IsEntered(this.Sync.Secret))             { Exit(this.Sync.Secret); }
        if(IsEntered(this.Sync.Purpose))            { Exit(this.Sync.Purpose); }
        if(IsEntered(this.Sync.ProcessID))          { Exit(this.Sync.ProcessID); }
        if(IsEntered(this.Sync.Policies))           { Exit(this.Sync.Policies); }
        if(IsEntered(this.Sync.Outputs))            { Exit(this.Sync.Outputs); }
        if(IsEntered(this.Sync.Objectives))         { Exit(this.Sync.Objectives); }
        if(IsEntered(this.Sync.Notes))              { Exit(this.Sync.Notes); }
        if(IsEntered(this.Sync.Name))               { Exit(this.Sync.Name); }
        if(IsEntered(this.Sync.Modified))           { Exit(this.Sync.Modified); }
        if(IsEntered(this.Sync.MachineID))          { Exit(this.Sync.MachineID); }
        if(IsEntered(this.Sync.Locked))             { Exit(this.Sync.Locked); }
        if(IsEntered(this.Sync.Locator))            { Exit(this.Sync.Locator); }
        if(IsEntered(this.Sync.Links))              { Exit(this.Sync.Links); }
        if(IsEntered(this.Sync.Inputs))             { Exit(this.Sync.Inputs); }
        if(IsEntered(this.Sync.ID))                 { Exit(this.Sync.ID); }
        if(IsEntered(this.Sync.GPS))                { Exit(this.Sync.GPS); }
        if(IsEntered(this.Sync.FILE))               { Exit(this.Sync.FILE); }
        if(IsEntered(this.Sync.Extension))          { Exit(this.Sync.Extension); }
        if(IsEntered(this.Sync.EventLogs))          { Exit(this.Sync.EventLogs); }
        if(IsEntered(this.Sync.DomainID))           { Exit(this.Sync.DomainID); }
        if(IsEntered(this.Sync.DistinguishedName))  { Exit(this.Sync.DistinguishedName); }
        if(IsEntered(this.Sync.Data))               { Exit(this.Sync.Data); }
        if(IsEntered(this.Sync.CPUID))              { Exit(this.Sync.CPUID); }
        if(IsEntered(this.Sync.Controls))           { Exit(this.Sync.Controls); }
        if(IsEntered(this.Sync.Container))          { Exit(this.Sync.Container); }
        if(IsEntered(this.Sync.Commands))           { Exit(this.Sync.Commands); }
        if(IsEntered(this.Sync.Certificates))       { Exit(this.Sync.Certificates); }
        if(IsEntered(this.Sync.BornOn))             { Exit(this.Sync.BornOn); }
        if(IsEntered(this.Sync.AssemblyVersion))    { Exit(this.Sync.AssemblyVersion); }
        if(IsEntered(this.Sync.ApplicationVersion)) { Exit(this.Sync.ApplicationVersion); }
        if(IsEntered(this.Sync.Application))        { Exit(this.Sync.Application); }
        if(IsEntered(this.Sync.AppDomainUID))       { Exit(this.Sync.AppDomainUID); }
        if(IsEntered(this.Sync.AppDomainID))        { Exit(this.Sync.AppDomainID); }
        if(IsEntered(this.Sync.Activities))         { Exit(this.Sync.Activities); }
    }

    /**<include file='Tool.xml' path='Tool/class[@name="Tool"]/property[@name="ExtensionData"]/*'/>*/
    public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}