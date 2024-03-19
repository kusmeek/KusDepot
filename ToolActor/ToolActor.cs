namespace ToolActor;

[StatePersistence(StatePersistence.Persisted)]
public class ToolActor : Actor , IToolActor
{
    protected List<Activity>? Activities;

    protected Guid? ActorID;

    protected Int64? AppDomainID;

    protected Int64? AppDomainUID;

    protected String? Application;

    protected Version? ApplicationVersion;

    protected Version? AssemblyVersion;

    protected DateTimeOffset? BornOn;

    protected Dictionary<String,String>? Certificates;

    protected Dictionary<String,ActorCommand>? Commands;

    protected Autofac.IContainer? Container;

    protected String? CPUID;

    protected HashSet<DataItem>? Data;

    protected String? DistinguishedName;

    protected String? DomainID;

    protected Dictionary<Int32,String>? EventLogs;

    protected Dictionary<String,Object?>? Extension;

    protected String? GPS;

    protected Guid? ID;

    protected Queue<Object>? Inputs;

    protected Dictionary<String,GuidReferenceItem>? Links;

    protected Uri? Locator;

    protected Boolean Locked;

    protected String? MachineID;

    protected DateTimeOffset? Modified;

    protected String? Name;

    protected HashSet<String>? Notes;

    protected List<Object>? Objectives;

    protected Dictionary<Guid,Object?>? Outputs;

    protected List<Object>? Policies;

    protected Int64? ProcessID;

    protected String? Purpose;

    protected String? Secret;

    protected String? SecurityDescriptor;

    protected Version? ServiceVersion;

    protected Dictionary<String,Object?>? Status;

    protected String? StringID;

    protected HashSet<String>? Tags;

    protected Int32? ThreadID;

    protected HashSet<ActorTrait>? Traits;

    protected Version? Version;

    public ToolActor(ActorService actor , ActorId id) : base(actor,id)
    {
        try
        {
            this.Activities         = new List<Activity>();
            if(Equals(id?.Kind,ActorIdKind.Guid)) { this.ActorID = id.GetGuidId(); }
            this.AppDomainID        = 0;
            this.AppDomainUID       = 0;
            this.Application        = String.Empty;
            this.ApplicationVersion = new Version();
            this.AssemblyVersion    = new Version();
            this.BornOn             = DateTimeOffset.Now;
            this.Certificates       = new Dictionary<String,String>();
            this.Commands           = new Dictionary<String,ActorCommand>();
            this.Container          = null;
            this.CPUID              = String.Empty;
            this.Data               = new HashSet<DataItem>();
            this.DistinguishedName  = String.Empty;
            this.DomainID           = String.Empty;
            this.EventLogs          = new Dictionary<Int32,String>();
            this.Extension          = new Dictionary<String,Object?>();
            this.GPS                = String.Empty;
            this.ID                 = Guid.Empty;
            this.Inputs             = new Queue<Object>();
            this.Links              = new Dictionary<String,GuidReferenceItem>();
            this.Locator            = new Uri("myenterprise:/domain/ToolActorService");
            this.Locked             = false;
            this.MachineID          = String.Empty;
            this.Modified           = new DateTimeOffset();
            this.Name               = String.Empty;
            this.Notes              = new HashSet<String>();
            this.Objectives         = new List<Object>();
            this.Outputs            = new Dictionary<Guid,Object?>();
            this.Policies           = new List<Object>();
            this.ProcessID          = 0;
            this.Purpose            = String.Empty;
            this.SecurityDescriptor = String.Empty;
            this.ServiceVersion     = new Version();
            this.Status             = new Dictionary<String,Object?>();
            if(Equals(id?.Kind,ActorIdKind.String)) { this.StringID = id.GetStringId(); }
            this.Tags               = new HashSet<String>();
            this.Traits             = new HashSet<ActorTrait>();
            this.ThreadID           = 0;
            this.Version            = new Version();
        }
        catch ( Exception ) { throw; }
    }

    public Task<Boolean> Activate()
    {
        return Task.FromResult( (this.Commands?.Values.All((_)=>{ return _.Attach(this.ActorID,this.StringID) && _.Enable(); }) ?? true) && (this.Traits?.All((_)=>{ return _.Bind(this.ActorID,this.StringID).Result && _.Activate().Result; }) ?? true) );
    }

    public Boolean AddActivity(Activity? activity)
    {
        try
        {
            if(activity is null) { return false; } if(this.Activities is not null && this.Activities.Contains(activity)) { return true; }

            if(this.Activities is null) { this.Activities = new List<Activity>(); }

            this.Activities.Add(activity); this.RaiseAlert(this,new ActivityEventArgs(AlertCode.ActivityAdded,activity)); return true;
        }
        catch ( Exception ) { return false; }
    }

    public Task<Boolean> AddDataItems(IEnumerable<DataItem>? data)
    {
        try
        {
            if( data is null || this.Locked ) { return Task.FromResult(false); }

            HashSet<DataItem> ___ = new(data); if(Equals(___.Count,0)) { return Task.FromResult(true); }

            if(this.Data is null) { this.Data = new(); } HashSet<DataItem> __ = new HashSet<DataItem>();

            foreach(DataItem _ in ___)
            {
                if( _ is GuidReferenceItem) { if(__.Add((GuidReferenceItem)_.Clone())) { continue; } else { return Task.FromResult(false); } }
                if( _ is GenericItem)       { if(__.Add((GenericItem)_.Clone()))       { continue; } else { return Task.FromResult(false); } }
                if( _ is TextItem)          { if(__.Add((TextItem)_.Clone()))          { continue; } else { return Task.FromResult(false); } }
                if( _ is CodeItem)          { if(__.Add((CodeItem)_.Clone()))          { continue; } else { return Task.FromResult(false); } }
                if( _ is MSBuildItem)       { if(__.Add((MSBuildItem)_.Clone()))       { continue; } else { return Task.FromResult(false); } }
                if( _ is BinaryItem)        { if(__.Add((BinaryItem)_.Clone()))        { continue; } else { return Task.FromResult(false); } }
                if( _ is MultiMediaItem)    { if(__.Add((MultiMediaItem)_.Clone()))    { continue; } else { return Task.FromResult(false); } }
            }

            if(!Equals(__.Count,___.Count)) { return Task.FromResult(false); }

            this.Data.UnionWith(__); __.ToList().ForEach((_)=>{this.RaiseAlert(this,new DataItemEventArgs(AlertCode.DataItemAdded,_.GetID()));});
            
            return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> AddInput(Object? input)
    {
        try
        {
            if( input is null || this.Locked ) { return Task.FromResult(false); }

            if(this.Inputs is null) { this.Inputs = new Queue<Object>(); }

            this.Inputs.Enqueue(input);  this.RaiseAlert(this,new AlertEventArgs(AlertCode.InputAdded));

            return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> AddNotes(IEnumerable<String>? notes)
    {
        try
        {
            if( notes is null || this.Locked ) { return Task.FromResult(false); }

            HashSet<String> _ = notes.Select(_=>new String(_)).ToHashSet();

            if(Equals(_.Count,0)) { return Task.FromResult(true); }

            if(this.Notes is null) { this.Notes = _; }

            this.Notes.UnionWith(_); this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Notes"));

            return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> AddOutput(Guid id , Object? output)
    {
        try
        {
            if( output is null || this.Locked || Equals(id,Guid.Empty) ) { return Task.FromResult(false); }

            if(this.Outputs is null) { this.Outputs = new Dictionary<Guid,Object?>(); }

            if(this.Outputs.TryAdd(id,output))
            {
                this.RaiseAlert(this,new OutputEventArgs(AlertCode.OutputAdded,id)); return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> AddTags(IEnumerable<String>? tags)
    {
        try
        {
            if( tags is null || this.Locked ) { return Task.FromResult(false); }

            HashSet<String> _ = tags.Select(_=>new String(_)).ToHashSet();

            if(Equals(_.Count,0)) { return Task.FromResult(true); }

            if(this.Tags is null) { this.Tags = _; }

            this.Tags.UnionWith(_); this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Tags"));

            return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public event EventHandler<AlertEventArgs>? Alert;

    public Task<Boolean> ClearEventLogs()
    {
        try
        {
            this.EventLogs = null; return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> Deactivate()
    {
        return Task.FromResult( (this.Commands?.Values.All((_)=>{ return _.Disable() && _.Detach(); }) ?? true) && (this.Traits?.All((_)=>{ return _.Deactivate().Result; }) ?? true) );
    }

    public Task<Guid?> ExecuteCommand(Object[]? details)
    {
        try
        {
            if(details is null || this.Commands is null) { return Task.FromResult<Guid?>(null);; }

            String? handle = details[0] as String; if(handle is null) { return Task.FromResult<Guid?>(null);; }

            if(!this.Commands.TryGetValue(handle,out ActorCommand? cmd) ) { return Task.FromResult<Guid?>(null);; }

            return Task.FromResult(cmd.ExecuteAsync(details)?.ID);
        }
        catch ( Exception ) { return Task.FromResult<Guid?>(null); }
    }

    public List<Activity>? GetActivities()
    {
        try { return this.Activities?.ToList(); }
        catch ( Exception ) { return null; }
    }

    public Task<Guid?> GetActorID() { return Task.FromResult(this.ActorID); }

    public Task<Int64?> GetAppDomainID() { return Task.FromResult(this.AppDomainID); }

    public Task<Int64?> GetAppDomainUID() { return Task.FromResult(this.AppDomainUID); }

    public Task<String?> GetApplication() { return Task.FromResult(this.Application is null ? null : new String(this.Application)); }

    public Task<Version?> GetApplicationVersion() { return Task.FromResult(this.ApplicationVersion is null ? null : new Version(this.ApplicationVersion.ToString())); }

    public Task<Version?> GetAssemblyVersion() { return Task.FromResult(this.AssemblyVersion is null ? null : new Version(this.AssemblyVersion.ToString())); }

    public Task<DateTimeOffset?> GetBornOn() { return Task.FromResult(this.BornOn); }

    public Task<Dictionary<String,String>?> GetCertificates()
    {
        try { return Task.FromResult(this.Certificates?.ToDictionary(_=>new String(_.Key),_=>new String(_.Value))); }
        catch ( Exception ) { return Task.FromResult<Dictionary<String,String>?>(null); }
    }

    public Task<Dictionary<String,ActorCommand>?> GetCommands()
    {
        try { return Task.FromResult(this.Commands?.ToDictionary(_=>new String(_.Key),_=>_.Value)); }
        catch ( Exception ) { return Task.FromResult<Dictionary<String,ActorCommand>?>(null); }
    }

    public Task<Autofac.IContainer?> GetContainer() { return Task.FromResult(this.Container); }

    public Task<String?> GetCPUID() { return Task.FromResult(this.CPUID is null ? null : new String(this.CPUID)); }

    public Task<HashSet<DataItem>?> GetDataItems()
    {
        try
        {
            if(this.Data is null) { return Task.FromResult((HashSet<DataItem>?)null); }

            HashSet<DataItem> __ = new HashSet<DataItem>();

            foreach(DataItem _ in this.Data)
            {
                if( _ is GuidReferenceItem) { if(__.Add((GuidReferenceItem)_.Clone())) { continue; } else { return Task.FromResult((HashSet<DataItem>?)null); } }
                if( _ is GenericItem)       { if(__.Add((GenericItem)_.Clone()))       { continue; } else { return Task.FromResult((HashSet<DataItem>?)null); } }
                if( _ is TextItem)          { if(__.Add((TextItem)_.Clone()))          { continue; } else { return Task.FromResult((HashSet<DataItem>?)null); } }
                if( _ is CodeItem)          { if(__.Add((CodeItem)_.Clone()))          { continue; } else { return Task.FromResult((HashSet<DataItem>?)null); } }
                if( _ is MSBuildItem)       { if(__.Add((MSBuildItem)_.Clone()))       { continue; } else { return Task.FromResult((HashSet<DataItem>?)null); } }
                if( _ is BinaryItem)        { if(__.Add((BinaryItem)_.Clone()))        { continue; } else { return Task.FromResult((HashSet<DataItem>?)null); } }
                if( _ is MultiMediaItem)    { if(__.Add((MultiMediaItem)_.Clone()))    { continue; } else { return Task.FromResult((HashSet<DataItem>?)null); } }
            }

            if(!Equals(this.Data.Count,__.Count)) { return Task.FromResult((HashSet<DataItem>?)null); }

            return Task.FromResult((HashSet<DataItem>?)__);
        }
        catch ( Exception ) { return Task.FromResult((HashSet<DataItem>?)null); }
    }

    public Task<String?> GetDistinguishedName() { return Task.FromResult(this.DistinguishedName is null ? null : new String(this.DistinguishedName)); }

    public Task<String?> GetDomainID() { return Task.FromResult(this.DomainID is null ? null : new String(this.DomainID)); }

    public Task<String?> GetEvent(Int32? key)
    {
        try
        {
            if(key is null || (!this.EventLogs?.ContainsKey(key.Value) ?? true)) { return Task.FromResult<String?>(null); }
        
            return Task.FromResult<String?>(this.EventLogs?[key.Value]);
        }
        catch ( Exception ) { return Task.FromResult<String?>(null); }
    }

    public Task<Dictionary<Int32,String>?> GetEventLogs()
    {
        try { return Task.FromResult(this.EventLogs?.ToDictionary(_=>_.Key,_=>new String(_.Value))); }
        catch ( Exception ) { return Task.FromResult<Dictionary<Int32,String>?>(null); }
    }

    public Task<Dictionary<String,Object?>?> GetExtension() { return Task.FromResult(this.Extension); }

    public Task<String?> GetGPS() { return Task.FromResult(this.GPS is null ? null : new String(this.GPS)); }

    public Task<Guid?> GetID() { return Task.FromResult(this.ID); }

    public Task<Object?> GetInput()
    {
        try { return Task.FromResult(this.Inputs?.TryDequeue(out Object? i) is true ? i : null); }
        catch ( Exception ) { return Task.FromResult<Object?>(null); }
    }

    public Task<Queue<Object>?> GetInputs() { return Task.FromResult(this.Inputs); }

    public Task<Dictionary<String,GuidReferenceItem>?> GetLinks()
    {
        try { return Task.FromResult(this.Links?.ToDictionary(_=>new String(_.Key),_=>(GuidReferenceItem)_.Value.Clone())); }
        catch ( Exception ) { return Task.FromResult<Dictionary<String,GuidReferenceItem>?>(null); }
    }

    public Task<Uri?> GetLocator() { return Task.FromResult(this.Locator is null ? null : new Uri(this.Locator.ToString())); }

    public Task<Boolean> GetLocked() { return Task.FromResult(this.Locked); }

    public Task<String?> GetMachineID() { return Task.FromResult(this.MachineID is null ? null : new String(this.MachineID)); }

    public Task<DateTimeOffset?> GetModified() { return Task.FromResult(this.Modified); }

    public Task<String?> GetName() { return Task.FromResult(this.Name is null ? null : new String(this.Name)); }

    public Task<HashSet<String>?> GetNotes()
    {
        try { return Task.FromResult(this.Notes?.Select(_=>new String(_)).ToHashSet()); }
        catch ( Exception ) { return Task.FromResult((HashSet<String>?)null); }
    }

    public Task<List<Object>?> GetObjectives() { return Task.FromResult(this.Objectives); }

    public Task<Object?> GetOutput(Guid? id)
    {
        try { return id is null ? Task.FromResult<Object?>(null) : Task.FromResult(this.Outputs?.TryGetValue(id.Value,out Object? o) is true ? o : null); }
        catch ( Exception ) { return Task.FromResult<Object?>(null); }
    }

    public Task<List<Guid>?> GetOutputIDs()
    {
        try { return Task.FromResult(this.Outputs?.Keys.ToList()); }
        catch ( Exception ) { return Task.FromResult<List<Guid>?>(null); }
    }

    public Task<List<Object>?> GetPolicies() { return Task.FromResult(this.Policies); }

    public Task<Int64?> GetProcessID() { return Task.FromResult(this.ProcessID); }

    public Task<String?> GetPurpose() { return Task.FromResult(this.Purpose is null ? null : new String(this.Purpose)); }

    public Task<String?> GetSecurityDescriptor() { return Task.FromResult(this.SecurityDescriptor is null ? null : new String(this.SecurityDescriptor)); }

    public Task<Version?> GetServiceVersion() { return Task.FromResult(this.ServiceVersion is null ? null : new Version(this.ServiceVersion.ToString())); }

    public Task<Dictionary<String,Object?>?> GetStatus()
    {
        try { return Task.FromResult(this.Status?.ToDictionary(_=>new String(_.Key),_=>_.Value)); }
        catch ( Exception ) { return Task.FromResult<Dictionary<String,Object?>?>(null); }
    }

    public Task<String?> GetStringID() { return Task.FromResult(this.StringID); }

    public Task<HashSet<String>?> GetTags()
    {
        try { return Task.FromResult(this.Tags?.Select(_=>new String(_)).ToHashSet()); }
        catch ( Exception ) { return Task.FromResult((HashSet<String>?)null); }
    }

    public Task<Int32?> GetThreadID() { return Task.FromResult(this.ThreadID); }

    public Task<Version?> GetVersion() { return Task.FromResult(this.Version is null ? null : new Version(this.Version.ToString())); }

    public Task<Boolean> Initialize()
    {
        try
        {
                this.AppDomainID     = AppDomain.CurrentDomain.Id;
                this.AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
                this.BornOn          = this.BornOn ?? DateTimeOffset.Now;
                this.ID              = this.ID ?? Guid.NewGuid();
                this.MachineID       = Environment.MachineName;
                this.ProcessID       = Environment.ProcessId;
                this.ThreadID        = Native.GetCurrentThreadId() ?? Environment.CurrentManagedThreadId;

                return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> Lock(String? secret)
    {
        try
        {
            if( secret is null || this.Locked ) { return Task.FromResult(false); }

            this.Secret = new String(secret); this.Locked = true;

            this.RaiseAlert(this,new AlertEventArgs(AlertCode.Locked)); return Task.FromResult(true);
        }
        catch ( Exception ) { this.Secret = null; this.Locked = false; return Task.FromResult(false); }
    }

    protected override Task OnActivateAsync() { if(this.Initialize().Result && this.Activate().Result) { Log.Information(Activated); return Task.FromResult(true); } else { return Task.FromResult(false); } }

    protected override Task OnDeactivateAsync() { if(this.Deactivate().Result) { Log.Information(Deactivated); return Task.FromResult(true); } else { return Task.FromResult(false); } }

    public Task<Boolean> LogEvent(String? eventdata)
    {
        try
        {
            if(eventdata is null) { return Task.FromResult(false); }

            if(this.EventLogs is null) { this.EventLogs = new(); }

            if(this.EventLogs.TryAdd(this.EventLogs.Count,eventdata))
            {
                this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"EventLogs")); return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public void RaiseAlert(Object? sender , AlertEventArgs eventargs , Boolean synchronous = false)
    {
        try
        {
            if(this.Alert is null) { return; }

            if(synchronous) { this.Alert(sender,eventargs); }

            else { Task.Run( () => { this.Alert(sender,eventargs); }); }
        }
        catch ( Exception ) { }
    }

    public Boolean RegisterCommand(ActorCommand command, String handle)
    {
        try
        {
            if( command is null || handle is null || this.Locked ) { return false; }

            if(this.Commands is null) { this.Commands = new(); }

            if(this.Commands.ContainsKey(handle)) { return false; }

            if(this.Commands.TryAdd(new String(handle),command))
            {
                if(command.Attach(this.ActorID,this.StringID) is false) { this.Commands.Remove(handle); return false; }

                this.RaiseAlert(this,new CommandEventArgs(AlertCode.CommandRegistered,handle,command.GetID())); return true;
            }
            return false;
        }
        catch ( Exception ) { return false; }
    }

    public Boolean RemoveActivity(Activity? activity)
    {
        try
        {
            if( activity is null || this.Activities is null ) { return false; }

            if(this.Activities.Remove(activity))
            {
                if(Equals(this.Activities.Count,0)) { this.Activities = null; }

                this.RaiseAlert(this,new ActivityEventArgs(AlertCode.ActivityRemoved,activity)); return true;
            }
            return false;
        }
        catch ( Exception ) { return false; }
    }

    public Task<Boolean> RemoveDataItem(Guid? id)
    {
        try
        {
            if( id is null || this.Data is null || this.Locked ) { return Task.FromResult(false); }

            DataItem? _ = this.Data.FirstOrDefault(_=>_.GetID().Equals(id));

            if( _ is not null)
            {
                if(this.Data.Remove(_))
                {
                    if(Equals(this.Data.Count,0)) { this.Data = null; }

                    this.RaiseAlert(this,new DataItemEventArgs(AlertCode.DataItemRemoved,id)); return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> RemoveNote(String? note)
    {
        try
        {
            if( note is null || this.Notes is null || this.Locked ) { return Task.FromResult(false); }

            if(this.Notes.Remove(note))
            {
                if(Equals(this.Notes.Count,0)) { this.Notes = null; }

                this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Notes")); return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> RemoveOutput(Guid? id)
    {
        try
        {
            if( id is null || this.Outputs is null || this.Locked ) { return Task.FromResult(false); }

            if(this.Outputs.Remove(id.Value))
            {
                if(Equals(this.Outputs.Count,0)) { this.Outputs = null; }

                this.RaiseAlert(this,new OutputEventArgs(AlertCode.OutputRemoved,id.Value)); return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> RemoveStatus(String? key)
    {
        try
        {
            if( key is null || this.Status is null || this.Locked) { return Task.FromResult(false); }

            if(this.Status.Remove(key))
            {
                if(Equals(this.Status.Count,0)) { this.Status = null; }

                this.RaiseAlert(this,new AlertEventArgs(AlertCode.StatusUpdated)); return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> RemoveTag(String? tag)
    {
        try
        {
            if( tag is null || this.Tags is null || this.Locked ) { return Task.FromResult(false); }

            if(this.Tags.Remove(tag))
            {
                if(Equals(this.Tags.Count,0)) { this.Tags = null; }

                this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Tags")); return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetApplication(String? application)
    {
        try
        {
            if( application is null || this.Locked ) { return Task.FromResult(false); }

            if(String.IsNullOrEmpty(application)) { this.Application = null; }

            else { this.Application = new String(application); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Application")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetApplicationVersion(Version? applicationversion)
    {
        try
        {
            if( applicationversion is null || this.Locked ) { return Task.FromResult(false); }

            if(Version.Equals(applicationversion,Version.Parse("0.0.0.0"))) { this.ApplicationVersion = null; }

            else { this.ApplicationVersion = new Version(applicationversion.ToString()); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"ApplicationVersion")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetBornOn(DateTimeOffset? bornon)
    {
        try
        {
            if( bornon is null || this.Locked ) { return Task.FromResult(false);}

            if(Equals(bornon,DateTimeOffset.MinValue)) { this.BornOn = null; } else { this.BornOn = bornon; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"BornOn")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetCertificates(Dictionary<String,String>? certificates)
    {
        try
        {
            if( certificates is null || this.Locked ) { return Task.FromResult(false); }

            if(Equals(certificates.Count,0)) { this.Certificates = null; }

            else { this.Certificates = certificates.ToDictionary(_=>new String(_.Key),_=>new String(_.Value)); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Certificates")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetContainer(Autofac.IContainer? container)
    {
        try
        {
            if( container is null || this.Locked ) { return Task.FromResult(false); }

            this.Container = container; this.RaiseAlert(this,new AlertEventArgs(AlertCode.ContainerChanged));

            return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetDistinguishedName(String? distinguishedname)
    {
        try
        {
            if( distinguishedname is null || this.Locked ) { return Task.FromResult(false); }

            if(String.IsNullOrEmpty(distinguishedname)) { this.DistinguishedName = null; }

            else { this.DistinguishedName = new String(distinguishedname); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"DistinguishedName")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetDomainID(String? domainid)
    {
        try
        {
            if( domainid is null || this.Locked ) { return Task.FromResult(false); }

            if(String.IsNullOrEmpty(domainid)) { this.DomainID = null; } else { this.DomainID = new String(domainid); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"DomainID")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetExtension(Dictionary<String,Object?>? extension)
    {
        try
        {
            if( extension is null || this.Locked ) { return Task.FromResult(false); }

            if(Equals(extension.Count,0)) { this.Extension = null; }

            else { this.Extension = extension.ToDictionary(_=>new String(_.Key),_=>_.Value); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Extension")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetGPS(String? gps)
    {
        try
        {
            if( gps is null || this.Locked ) { return Task.FromResult(false); }

            if(String.IsNullOrEmpty(gps)) { this.GPS = null; } else { this.GPS = new String(gps); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"GPS")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetID(Guid? id)
    {
        try
        {
            if( id is null || this.Locked ) { return Task.FromResult(false); }

            if(Guid.Equals(id,Guid.Empty)) { this.ID = null; } else { this.ID = id; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"ID")); 

            return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetLinks(Dictionary<String,GuidReferenceItem>? links)
    {
        try
        {
            if( links is null || this.Locked ) { return Task.FromResult(false); }

            if(Equals(links.Count,0)) { this.Links = null; }

            else { this.Links = links.ToDictionary(_=>new String(_.Key),_=>(GuidReferenceItem)_.Value.Clone()); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Links")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetLocator(Uri? locator)
    {
        try
        {
            if( locator is null || this.Locked ) { return Task.FromResult(false); }

            if(Uri.Equals(locator,new Uri("null:"))) { this.Locator = null; } else { this.Locator = new Uri(locator.ToString()); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Locator")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetModified(DateTimeOffset? modified)
    {
        try
        {
            if( modified is null || this.Locked ) { return Task.FromResult(false); }

            if(DateTimeOffset.Equals(modified,DateTimeOffset.MinValue)) { this.Modified = null; } else { this.Modified = modified; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Modified")); 

            return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetName(String? name)
    {
        try
        {
            if( name is null || this.Locked ) { return Task.FromResult(false); }

            if(String.IsNullOrEmpty(name)) { this.Name = null; } else { this.Name = new String(name); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Name")); 

            return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetObjectives(IEnumerable<Object>? objectives)
    {
        try
        {
            if( objectives is null || this.Locked ) { return Task.FromResult(false); } List<Object> _ = objectives.ToList();

            if(Equals(_.Count,0)) { this.Objectives = null; } else { this.Objectives = _; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Objectives")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetPolicies(IEnumerable<Object>? policies)
    {
        try
        {
            if( policies is null || this.Locked ) { return Task.FromResult(false); } List<Object> _ = policies.ToList();

            if(Equals(_.Count,0)) { this.Policies = null; } else { this.Policies = _; }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Policies")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetPurpose(String? purpose)
    {
        try
        {
            if( purpose is null || this.Locked ) { return Task.FromResult(false); }

            if(String.IsNullOrEmpty(purpose)) { this.Purpose = null; } else { this.Purpose = new String(purpose); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Purpose")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetSecurityDescriptor(String? securitydescriptor)
    {
        try
        {
            if( securitydescriptor is null || this.Locked ) { return Task.FromResult(false); }

            if(String.IsNullOrEmpty(securitydescriptor)) { this.SecurityDescriptor = null; }

            else { this.SecurityDescriptor = new String(securitydescriptor); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"SecurityDescriptor")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetServiceVersion(Version? serviceversion)
    {
        try
        {
            if( serviceversion is null || this.Locked ) { return Task.FromResult(false); }

            if(Version.Equals(serviceversion,Version.Parse("0.0.0.0"))) { this.ServiceVersion = null; }

            else { this.ServiceVersion = new Version(serviceversion.ToString()); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"ServiceVersion")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetStatus(String? key , Object? status)
    {
        try
        {
            if( key is null || this.Locked ) { return Task.FromResult(false); }

            if(this.Status is null) { this.Status = new Dictionary<String,Object?>(); }

            if(this.Status.TryAdd(key,status)) { this.RaiseAlert(this,new AlertEventArgs(AlertCode.StatusUpdated)); return Task.FromResult(true); }

            return Task.FromResult(false);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> SetVersion(Version? version)
    {
        try
        {
            if( version is null || this.Locked ) { return Task.FromResult(false); }

            if(Version.Equals(version,Version.Parse("0.0.0.0"))) { this.Version = null; } else { this.Version = new Version(version.ToString()); }

            this.RaiseAlert(this,new FieldChangedEventArgs(AlertCode.FieldChanged,"Version")); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> UnLock(String? secret)
    {
        try
        {
            if( secret is null || !this.Locked ) { return Task.FromResult(false); }

            if(String.Equals(this.Secret,secret,StringComparison.Ordinal))
            {
                this.Secret = null; this.Locked = false;

                this.RaiseAlert(this,new AlertEventArgs(AlertCode.UnLocked)); return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Boolean UnRegisterCommand(String? handle)
    {
        try
        {
            if( handle is null || this.Commands is null || this.Locked ) { return false; }

            if(this.Commands.TryGetValue(handle,out ActorCommand? c))
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
        catch ( Exception ) { return false; }
    }

    public Task<Boolean> UpdateInputs(Queue<Object>? inputs)
    {
        try
        {
            if( inputs is null || this.Locked ) { return Task.FromResult(false); }

            this.Inputs = new Queue<Object>(inputs);

            this.RaiseAlert(this,new AlertEventArgs(AlertCode.InputUpdated)); return Task.FromResult(true);
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }

    public Task<Boolean> WriteEventLogsToFile(String path)
    {
        try
        {
            if(path is null) { return Task.FromResult(false);}

            if(File.Exists(path)) { return Task.FromResult(false); }

            if(this.EventLogs is null) { return Task.FromResult(false); }

            using(FileStream _0 = new FileStream(path,new FileStreamOptions(){Access = FileAccess.Write , Mode = FileMode.CreateNew , Share = FileShare.None}))
            {
                using(XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateTextWriter(_0))
                {
                    DataContractSerializer _2 = new DataContractSerializer(typeof(Dictionary<Int32,String>),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                    _2.WriteObject(_1,this.EventLogs); _1.Flush(); return Task.FromResult(true);
                }
            }
        }
        catch ( Exception ) { return Task.FromResult(false); }
    }
}