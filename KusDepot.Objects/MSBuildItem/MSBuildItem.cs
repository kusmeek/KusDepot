namespace KusDepot;

/**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/main/*'/>*/
[DataContract(Name = "MSBuildItem" , Namespace = "KusDepot")]
public sealed class MSBuildItem : DataItem , ICloneable , IComparable<MSBuildItem> , IEquatable<MSBuildItem> , IExtensibleDataObject , IParsable<MSBuildItem>
{
    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/content[@name="Content"]/*'/>*/
    [DataMember(Name = "Content" , EmitDefaultValue = true , IsRequired = true)]
    private String? Content;

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/field[@name="Requirements"]/*'/>*/
    [DataMember(Name = "Requirements" , EmitDefaultValue = true , IsRequired = true)]
    private HashSet<DataItem>? Requirements;

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/field[@name="Sequence"]/*'/>*/
    [DataMember(Name = "Sequence" , EmitDefaultValue = true , IsRequired = true)]
    private SortedList<Int32,MSBuildItem>? Sequence;

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/field[@name="Sync"]/*'/>*/
    [DataMember(Name = "MSBuildItemSync" , EmitDefaultValue = true , IsRequired = true)]
    private MSBuildItemSync Sync;

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public MSBuildItem()
    {
        try
        {
            this.Sync = new MSBuildItemSync(); this.Type = DataType.MSB;

            this.Initialize();
        }
        catch ( Exception ) { throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/constructor[@name="Constructor"]/*'/>*/
    public MSBuildItem(String? content = null , Guid? id = null , String? name = null , HashSet<String>? notes = null , HashSet<String>? tags = null)
    {
        try
        {
            this.Sync = new MSBuildItemSync(); this.Type = DataType.MSB;

            this.SetContent(content); this.SetID(id); this.SetName(name); this.AddNotes(notes); this.AddTags(tags);

            this.Initialize();
        }
        catch ( Exception ) { throw; }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="AddNotes"]/*'/>*/
    public override Boolean AddNotes(IEnumerable<String>? notes)
    {
        try
        {
            if( notes is null || this.Locked ) { return false; }

            HashSet<String> _ = new(notes.Select(_=>new String(_))); if(Equals(_.Count,0)) { return true; }

            if(!TryEnter(this.Sync.Notes,SyncTime)) { throw NotesSyncException; }

            if(this.Notes is null) { this.Notes = _; return true; }

            this.Notes.UnionWith(_); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Notes)) { Exit(this.Sync.Notes); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="AddTags"]/*'/>*/
    public override Boolean AddTags(IEnumerable<String>? tags)
    {
        try
        {
            if( tags is null || this.Locked ) { return false; }

            HashSet<String> _ = new(tags.Select(_=>new String(_))); if(Equals(_.Count,0)) { return true; }

            if(!TryEnter(this.Sync.Tags,SyncTime)) { throw TagsSyncException; }

            if(this.Tags is null) { this.Tags = _; return true; }

            this.Tags.UnionWith(_); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Tags)) { Exit(this.Sync.Tags); } }
    }

    /**<include file='../DataItem/DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="Clone"]/*'/>*/
    public override Object Clone()
    {
        try
        {
            MSBuildItem? _ = MSBuildItem.Parse(this.ToString(),null);

            if( _ is not null ) { return _; }

            return new Object();
        }
        catch ( Exception ) { if(NoExceptions) { return new Object(); } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="IComparable{MSBuildItem}.CompareTo"]/*'/>*/
    public Int32 CompareTo(MSBuildItem? other)
    {
        try
        {
            if(other is null)               { return 1; }
            if(ReferenceEquals(this,other)) { return 0; }

            DateTimeOffset? _ = other.GetBornOn();

            if(this.BornOn == _ )            { return 0; }
            if(this.BornOn <  _ )            { return -1; }
            else                             { return 1; }
        }
        catch ( Exception ) { throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="EqualsObject"]/*'/>*/
    public override Boolean Equals(Object? other) { return this.Equals(other as MSBuildItem); }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="IEquatable{MSBuildItem}.Equals"]/*'/>*/
    public Boolean Equals(MSBuildItem? other)
    {
        try
        {
            if(other is null) { return false; } if(ReferenceEquals(this,other))                        { return true; }

            if(!TryEnter(this.Sync.Requirements,SyncTime))                                             { throw RequirementsSyncException; }
            if(!TryEnter(this.Sync.Sequence,SyncTime))                                                 { throw SequenceSyncException; }

            String? _ = this.GetContent(); String? _c = other.GetContent();
            ImmutableHashSet<DataItem>? _r = other.GetRequirements()?.ToImmutableHashSet();
            ImmutableSortedDictionary<Int32,MSBuildItem>? _s = other.GetSequence()?.ToImmutableSortedDictionary();

            if(this.Requirements is null ^ _r is null || this.Requirements?.Count != _r?.Count)        { return false; }
            if(this.Sequence     is null ^ _s is null || this.Sequence?.Count     != _s?.Count)        { return false; }
            if(!Equals(_?.Length,_c?.Length))                                                          { return false; }
            if(!String.Equals(_,_c,StringComparison.Ordinal))                                          { return false; }
            if(this.Requirements is null && _r is null && this.Sequence is null && _s is null)         { return true; }
            if(this.Sequence is not null     && _s is not null) { if(!this.Sequence.SequenceEqual(_s)) { return false; } }
            if(this.Requirements is not null && _r is not null) { if(!this.Requirements.SetEquals(_r)) { return false; } }

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally
        {
            if(IsEntered(this.Sync.Sequence))     { Exit(this.Sync.Sequence); }
            if(IsEntered(this.Sync.Requirements)) { Exit(this.Sync.Requirements); }
        }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="FromFile"]/*'/>*/
    public static MSBuildItem? FromFile(String path)
    {
        try
        {
            if(path is null) { return null; }

            if(!File.Exists(path)) { return null; }

            using(FileStream _0 = new FileStream(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read}))
            {
                using(XmlDictionaryReader _1 = XmlDictionaryReader.CreateBinaryReader(_0,XmlDictionaryReaderQuotas.Max))
                {
                    DataContractSerializer _2 = new DataContractSerializer(typeof(MSBuildItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                    return (MSBuildItem?)_2.ReadObject(_1);
                }
            }
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetAppDomainID"]/*'/>*/
    public override Int64? GetAppDomainID() { return this.AppDomainID; }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetAppDomainUID"]/*'/>*/
    public override Int64? GetAppDomainUID() { return this.AppDomainUID; }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetApplication"]/*'/>*/
    public override String? GetApplication() { return this.Application is null ? null : new String(this.Application); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetApplicationVersion"]/*'/>*/
    public override Version? GetApplicationVersion() { return this.ApplicationVersion is null ? null : new Version(this.ApplicationVersion.ToString()); }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetAssemblyVersion"]/*'/>*/
    public override Version? GetAssemblyVersion() { return this.AssemblyVersion is null ? null : new Version(this.AssemblyVersion.ToString()); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetBornOn"]/*'/>*/
    public override DateTimeOffset? GetBornOn() { return this.BornOn; }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetCertificates"]/*'/>*/
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

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="GetContent"]/*'/>*/
    public String? GetContent()
    {
        try
        {
            if(this.Content is null) { return null; }

            if(!TryEnter(this.Sync.Content,SyncTime)) { throw ContentSyncException; }

            return new String(this.Content);
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Content)) { Exit(this.Sync.Content); } }
    }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetCPUID"]/*'/>*/
    public override String? GetCPUID() { return this.CPUID is null ? null : new String(this.CPUID); }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="GetDescriptor"]/*'/>*/
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
                Modified           = this.GetModified()?.ToString("O"),
                Name               = this.GetName(),
                Notes              = this.GetNotes(),
                ObjectType         = typeof(MSBuildItem).Name,
                ServiceVersion     = this.GetServiceVersion()?.ToString(),
                Tags               = this.GetTags(),
                Type               = this.GetType(),
                Version            = this.GetVersion()?.ToString()
            };
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetDistinguishedName"]/*'/>*/
    public override String? GetDistinguishedName() { return this.DistinguishedName is null ? null : new String(this.DistinguishedName); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetDomainID"]/*'/>*/
    public override String? GetDomainID() { return this.DomainID is null ? null : new String(this.DomainID); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetExtension"]/*'/>*/
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
    public override String? GetFILE() { return this.FILE is null ? null : new String(this.FILE); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetGPS"]/*'/>*/
    public override String? GetGPS() { return this.GPS is null ? null : new String(this.GPS); }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="GetHashCode"]/*'/>*/
    public override Int32 GetHashCode() { return HashCode.Combine(this.Application,this.BornOn,this.ID,this.Name); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetID"]/*'/>*/
    public override Guid? GetID() { return this.ID; }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetLinks"]/*'/>*/
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
    public override Uri? GetLocator() { return this.Locator is null ? null : new Uri(this.Locator.ToString()); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetLocked"]/*'/>*/
    public override Boolean GetLocked() { return this.Locked; }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetMachineID"]/*'/>*/
    public override String? GetMachineID() { return this.MachineID is null ? null : new String(this.MachineID); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetModified"]/*'/>*/
    public override DateTimeOffset? GetModified() { return this.Modified; }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetName"]/*'/>*/
    public override String? GetName() { return this.Name is null ? null : new String(this.Name); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetNotes"]/*'/>*/
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

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetProcessID"]/*'/>*/
    public override Int64? GetProcessID() { return this.ProcessID; }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="GetRequirements"]/*'/>*/
    public HashSet<DataItem>? GetRequirements()
    {
        try
        {
            if(this.Requirements is null) { return null; }

            if(!TryEnter(this.Sync.Requirements,SyncTime)) { throw RequirementsSyncException; }

            HashSet<DataItem> __ = new HashSet<DataItem>();

            foreach(DataItem _ in this.Requirements)
            {
                if( _ is GuidReferenceItem) { if(__.Add((GuidReferenceItem)_.Clone())) { continue; } else { return null; } }
                if( _ is TextItem)          { if(__.Add((TextItem)_.Clone()))          { continue; } else { return null; } }
                if( _ is CodeItem)          { if(__.Add((CodeItem)_.Clone()))          { continue; } else { return null; } }
                if( _ is GenericItem)       { if(__.Add((GenericItem)_.Clone()))       { continue; } else { return null; } }
                if( _ is BinaryItem)        { if(__.Add((BinaryItem)_.Clone()))        { continue; } else { return null; } }
                if( _ is MultiMediaItem)    { if(__.Add((MultiMediaItem)_.Clone()))    { continue; } else { return null; } }
                if( _ is MSBuildItem)       { if(__.Add((MSBuildItem)_.Clone()))       { continue; } else { return null; } }
            }

            if(__.Count != this.Requirements.Count) { return null; } return __;
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Requirements)) { Exit(this.Sync.Requirements); } }
    }

    /**<include file='../DataItem/DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetSecureHashes"]/*'/>*/
    public override Dictionary<String,Byte[]>? GetSecureHashes()
    {
        try
        {
            if(this.SecureHashes is null) { return null; }

            if(!TryEnter(this.Sync.SecureHashes,SyncTime)) { throw SecureHashesSyncException; }

            return this.SecureHashes.ToDictionary(_=>new String(_.Key),_=>(Byte[])_.Value.Clone());
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.SecureHashes)) { Exit(this.Sync.SecureHashes); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetSecurityDescriptor"]/*'/>*/
    public override String? GetSecurityDescriptor() { return this.SecurityDescriptor is null ? null : new String(this.SecurityDescriptor); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetServiceVersion"]/*'/>*/
    public override Version? GetServiceVersion() { return this.ServiceVersion is null ? null : new Version(this.ServiceVersion.ToString()); }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="GetSequence"]/*'/>*/
    public SortedList<Int32,MSBuildItem>? GetSequence()
    {
        try
        {
            if(this.Sequence is null) { return null; }

            if(!TryEnter(this.Sync.Sequence,SyncTime)) { throw SequenceSyncException; }

            SortedList<Int32,MSBuildItem> __ = new SortedList<Int32,MSBuildItem>();

            foreach(KeyValuePair<Int32,MSBuildItem> _ in this.Sequence) { __.Add(_.Key,(MSBuildItem)_.Value.Clone()); }

            if(!Equals(__.Count,this.Sequence.Count)) { return null; } return __;
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }

        finally { if(IsEntered(this.Sync.Sequence)) { Exit(this.Sync.Sequence); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetTags"]/*'/>*/
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

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetThreadID"]/*'/>*/
    public override Int32? GetThreadID() { return this.ThreadID; }

    /**<include file='../DataItem/DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="GetType"]/*'/>*/
    public override String? GetType() { return this.Type is null ? null : new String(this.Type); }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetVersion"]/*'/>*/
    public override Version? GetVersion() { return this.Version is null ? null : new Version(this.Version.ToString()); }

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="Initialize"]/*'/>*/
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

            return true;
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
    public override Boolean Lock(String? secret)
    {
        try
        {
            if( secret is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Locked,SyncTime)) { throw LockedSyncException; }

            if(!TryEnter(this.Sync.Secret,SyncTime)) { throw SecretSyncException; }

            this.Secret = new String(secret); this.Locked = true; return true;
        }
        catch ( Exception ) { this.Secret = null; this.Locked = false; if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secret)) { Exit(this.Sync.Secret); }
                  if(IsEntered(this.Sync.Locked)) { Exit(this.Sync.Locked); } }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="OnDeserialized"]/*'/>*/
    [OnDeserialized]
    public void OnDeserialized(StreamingContext context) { this.Sync = new MSBuildItemSync(); }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="Parse"]/*'/>*/
    public static MSBuildItem Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            using(XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max))
            {
                DataContractSerializer _1 = new DataContractSerializer(typeof(MSBuildItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                MSBuildItem? _2 = (MSBuildItem?)_1.ReadObject(_0); if( _2 is not null ) { return _2; }

                throw new FormatException();
            }
        }
        catch ( Exception ) { if(NoExceptions) { return new MSBuildItem(); } throw; }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="RemoveNote"]/*'/>*/
    public override Boolean RemoveNote(String? note)
    {
        try
        {
            if( note is null || this.Notes is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Notes,SyncTime)) { throw NotesSyncException; }

            if(this.Notes.Remove(note)) { if(Equals(this.Notes.Count,0)) { this.Notes = null; } return true; }

            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Notes)) { Exit(this.Sync.Notes); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="RemoveTag"]/*'/>*/
    public override Boolean RemoveTag(String? tag)
    {
        try
        {
            if( tag is null || this.Tags is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Tags,SyncTime)) { throw TagsSyncException; }

            if(this.Tags.Remove(tag)) { if(Equals(this.Tags.Count,0)) { this.Tags = null; } return true; }

            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Tags)) { Exit(this.Sync.Tags); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetApplication"]/*'/>*/
    public override Boolean SetApplication(String? application)
    {
        try
        {
            if( application is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Application,SyncTime)) { throw ApplicationSyncException; }

            if(String.IsNullOrEmpty(application)) { this.Application = null; return true; }

            this.Application = new String(application); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Application)) { Exit(this.Sync.Application); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetApplicationVersion"]/*'/>*/
    public override Boolean SetApplicationVersion(Version? applicationversion)
    {
        try
        {
            if( applicationversion is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.ApplicationVersion,SyncTime)) { throw ApplicationVersionSyncException; }

            if(Version.Equals(applicationversion,Version.Parse("0.0.0.0"))) { this.ApplicationVersion = null; return true; }

            this.ApplicationVersion = new Version(applicationversion.ToString()); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.ApplicationVersion)) { Exit(this.Sync.ApplicationVersion); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetBornOn"]/*'/>*/
    public override Boolean SetBornOn(DateTimeOffset? bornon)
    {
        try
        {
            if( bornon is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.BornOn,SyncTime)) { throw BornOnSyncException; }

            if(Equals(bornon,DateTimeOffset.MinValue)) { this.BornOn = null; return true; }

            this.BornOn = bornon; return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.BornOn)) { Exit(this.Sync.BornOn); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetCertificates"]/*'/>*/
    public override Boolean SetCertificates(IDictionary<String,String>? certificates)
    {
        try
        {
            if( certificates is null || this.Locked ) { return false; }

            Dictionary<String,String> _ = certificates.ToDictionary(_=>new String(_.Key),_=>new String(_.Value));

            if(!TryEnter(this.Sync.Certificates,SyncTime)) { throw CertificatesSyncException; }

            if(Equals(_.Count,0)) { this.Certificates = null; return true; }

            this.Certificates = _; return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Certificates)) { Exit(this.Sync.Certificates); } }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="SetContent"]/*'/>*/
    public Boolean SetContent(String? content)
    {
        try
        {
            if( content is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Content,SyncTime)) { throw ContentSyncException; }

            if(String.IsNullOrEmpty(content)) { this.Content = null; return true; }

            this.Content = new String(content); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Content)) { Exit(this.Sync.Content); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetDistinguishedName"]/*'/>*/
    public override Boolean SetDistinguishedName(String? distinguishedname)
    {
        try
        {
            if( distinguishedname is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.DistinguishedName,SyncTime)) { throw DistinguishedNameSyncException; }

            if(String.IsNullOrEmpty(distinguishedname)) { this.DistinguishedName = null; return true; }

            this.DistinguishedName = new String(distinguishedname); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.DistinguishedName)) { Exit(this.Sync.DistinguishedName); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetDomainID"]/*'/>*/
    public override Boolean SetDomainID(String? domainid)
    {
        try
        {
            if( domainid is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.DomainID,SyncTime)) { throw DomainIDSyncException; }

            if(String.IsNullOrEmpty(domainid)) { this.DomainID = null; return true; }

            this.DomainID = new String(domainid); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.DomainID)) { Exit(this.Sync.DomainID); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetExtension"]/*'/>*/
    public override Boolean SetExtension(IDictionary<String,Object?>? extension)
    {
        try
        {
            if( extension is null || this.Locked ) { return false; }

            Dictionary<String,Object?> _ = extension.ToDictionary(_=>new String(_.Key),_=>_.Value);

            if(!TryEnter(this.Sync.Extension,SyncTime)) { throw ExtensionSyncException; }

            if(Equals(_.Count,0)) { this.Extension = null; return true; }

            this.Extension = _; return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Extension)) { Exit(this.Sync.Extension); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetFILE"]/*'/>*/
    public override Boolean SetFILE(String? file)
    {
        try
        {
            if( file is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.FILE,SyncTime)) { throw FILESyncException; }

            if(String.IsNullOrEmpty(file)) { this.FILE = null; return true; }

            this.FILE = new String(file); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.FILE)) { Exit(this.Sync.FILE); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetGPS"]/*'/>*/
    public override Boolean SetGPS(String? gps)
    {
        try
        {
            if( gps is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.GPS,SyncTime)) { throw GPSSyncException; }

            if(String.IsNullOrEmpty(gps)) { this.GPS = null; return true; }

            this.GPS = new String(gps); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.GPS)) { Exit(this.Sync.GPS); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetID"]/*'/>*/
    public override Boolean SetID(Guid? id)
    {
        try
        {
            if( id is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.ID,SyncTime)) { throw IDSyncException; }

            if(Guid.Equals(id,Guid.Empty)) { this.ID = null; return true; }

            this.ID = id; return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.ID)) { Exit(this.Sync.ID); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetLinks"]/*'/>*/
    public override Boolean SetLinks(IDictionary<String,GuidReferenceItem>? links)
    {
        try
        {
            if( links is null || this.Locked ) { return false; }

            Dictionary<String,GuidReferenceItem> _ = links.ToDictionary(_=>new String(_.Key),_=>(GuidReferenceItem)_.Value.Clone());

            if(!TryEnter(this.Sync.Links,SyncTime)) { throw LinksSyncException; }

            if(Equals(_.Count,0)) { this.Links = null; return true; }

            this.Links = _; return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Links)) { Exit(this.Sync.Links); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetLocator"]/*'/>*/
    public override Boolean SetLocator(Uri? locator)
    {
        try
        {
            if( locator is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Locator,SyncTime)) { throw LocatorSyncException; }

            if(Uri.Equals(locator,new Uri("null:"))) { this.Locator = null; return true; }

            this.Locator = new Uri(locator.ToString()); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Locator)) { Exit(this.Sync.Locator); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetModified"]/*'/>*/
    public override Boolean SetModified(DateTimeOffset? modified)
    {
        try
        {
            if( modified is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Modified,SyncTime)) { throw ModifiedSyncException; }

            if(Equals(modified,DateTimeOffset.MinValue)) { this.Modified = null; return true; }

            this.Modified = modified; return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Modified)) { Exit(this.Sync.Modified); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetName"]/*'/>*/
    public override Boolean SetName(String? name)
    {
        try
        {
            if( name is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Name,SyncTime)) { throw NameSyncException; }

            if(String.IsNullOrEmpty(name)) { this.Name = null; return true; }

            this.Name = new String(name); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Name)) { Exit(this.Sync.Name); } }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="SetRequirements"]/*'/>*/
    public Boolean SetRequirements(IEnumerable<DataItem>? requirements)
    {
        try
        {
            if( requirements is null || this.Locked ) { return false; }

            HashSet<DataItem> ___ = new(requirements);

            HashSet<DataItem> __ = new HashSet<DataItem>();

            foreach(DataItem _ in ___)
            {
                if( _ is GuidReferenceItem) { if(__.Add((GuidReferenceItem)_.Clone())) { continue; } else { return false; } }
                if( _ is TextItem)          { if(__.Add((TextItem)_.Clone()))          { continue; } else { return false; } }
                if( _ is CodeItem)          { if(__.Add((CodeItem)_.Clone()))          { continue; } else { return false; } }
                if( _ is GenericItem)       { if(__.Add((GenericItem)_.Clone()))       { continue; } else { return false; } }
                if( _ is BinaryItem)        { if(__.Add((BinaryItem)_.Clone()))        { continue; } else { return false; } }
                if( _ is MultiMediaItem)    { if(__.Add((MultiMediaItem)_.Clone()))    { continue; } else { return false; } }
                if( _ is MSBuildItem)       { if(__.Add((MSBuildItem)_.Clone()))       { continue; } else { return false; } }
            }

            if(!Equals(__.Count,___.Count)) { return false; }

            if(!TryEnter(this.Sync.Requirements,SyncTime)) { throw RequirementsSyncException; }

            if(Equals(___.Count,0)) { this.Requirements = null; return true; }

            this.Requirements = __; return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Requirements)) { Exit(this.Sync.Requirements); } }
    }

    /**<include file='../DataItem/DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="SetSecureHashes"]/*'/>*/
    public override Boolean SetSecureHashes(IDictionary<String,Byte[]>? securehashes)
    {
        try
        {
            if( securehashes is null || this.Locked ) { return false; }

            Dictionary<String,Byte[]> _ = securehashes.ToDictionary(_=>new String(_.Key),_=>(Byte[])_.Value.Clone());

            if(!TryEnter(this.Sync.SecureHashes,SyncTime)) { throw SecureHashesSyncException; }

            if(Equals(_.Count,0)) { this.SecureHashes = null; return true; }

            this.SecureHashes = _; return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.SecureHashes)) { Exit(this.Sync.SecureHashes); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetSecurityDescriptor"]/*'/>*/
    public override Boolean SetSecurityDescriptor(String? securitydescriptor)
    {
        try
        {
            if( securitydescriptor is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.SecurityDescriptor,SyncTime)) { throw SecurityDescriptorSyncException; }

            if(String.IsNullOrEmpty(securitydescriptor)) { this.SecurityDescriptor = null; return true; }

            this.SecurityDescriptor = new String(securitydescriptor); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.SecurityDescriptor)) { Exit(this.Sync.SecurityDescriptor); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetServiceVersion"]/*'/>*/
    public override Boolean SetServiceVersion(Version? serviceversion)
    {
        try
        {
            if( serviceversion is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.ServiceVersion,SyncTime)) { throw ServiceVersionSyncException; }

            if(Version.Equals(serviceversion,Version.Parse("0.0.0.0"))) { this.ServiceVersion = null; return true; }

            this.ServiceVersion = new Version(serviceversion.ToString()); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.ServiceVersion)) { Exit(this.Sync.ServiceVersion); } }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="SetSequence"]/*'/>*/
    public Boolean SetSequence(IDictionary<Int32,MSBuildItem>? sequence)
    {
        try
        {
            if( sequence is null || this.Locked ) { return false; }

            SortedList<Int32,MSBuildItem> _ = new(sequence.ToDictionary(_=>_.Key,_=>(MSBuildItem)_.Value.Clone(),null));

            if(!TryEnter(this.Sync.Sequence,SyncTime)) { throw SequenceSyncException; }

            if(Equals(_.Count,0)) { this.Sequence = null; return true; }
            
            this.Sequence = _; return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Sequence)) { Exit(this.Sync.Sequence); } }
    }

    /**<include file='../DataItem/DataItem.xml' path='DataItem/class[@name="DataItem"]/method[@name="SetType"]/*'/>*/
    public override Boolean SetType(String? type)
    {
        try
        {
            if( type is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Type,SyncTime)) { throw TypeSyncException; }

            if(String.IsNullOrEmpty(type)) { this.Type = null; return true; }

            this.Type = new String(type); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Type)) { Exit(this.Sync.Type); } }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetVersion"]/*'/>*/
    public override Boolean SetVersion(Version? version)
    {
        try
        {
            if( version is null || this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Version,SyncTime)) { throw VersionSyncException; }

            if(Version.Equals(version,Version.Parse("0.0.0.0"))) { this.Version = null; return true; }

            this.Version = new Version(version.ToString()); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Version)) { Exit(this.Sync.Version); } }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="ToFile"]/*'/>*/
    public Boolean ToFile(String path)
    {
        try
        {
            if(path is null) { return false; } if(File.Exists(path)) { return false; } this.AcquireLocks();

            using(FileStream _0 = new FileStream(path,new FileStreamOptions(){Access = FileAccess.Write , Mode = FileMode.CreateNew , Share = FileShare.None}))
            {
                using(XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0))
                {
                    DataContractSerializer _2 = new DataContractSerializer(typeof(MSBuildItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                    _2.WriteObject(_1,this); _1.Flush(); return true;
                }
            }
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { this.ReleaseLocks(); }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="ToString"]/*'/>*/
    public override String ToString()
    {
        try
        {
            this.AcquireLocks(); MemoryStream _0 = new MemoryStream();

            using(XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0))
            {
                DataContractSerializer _2 = new DataContractSerializer(typeof(MSBuildItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

                return _0.ToArray().ToBase64FromByteArray();
            }
        }
        catch ( Exception ) { if(NoExceptions) { return String.Empty; } throw; }

        finally { this.ReleaseLocks(); }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out MSBuildItem item)
    {
        item = null; if(input is null) { return false; }

        try
        {
            using(XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max))
            {
                DataContractSerializer _1 = new DataContractSerializer(typeof(MSBuildItem),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                MSBuildItem? _2 = (MSBuildItem?)_1.ReadObject(_0); if( _2 is not null ) { item = _2; return true; }

                return false;
            }
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="UnLock"]/*'/>*/
    public override Boolean UnLock(String? secret)
    {
        try
        {
            if( secret is null || !this.Locked ) { return false; }

            if(!TryEnter(this.Sync.Locked,SyncTime)) { throw LockedSyncException; }

            if(!TryEnter(this.Sync.Secret,SyncTime)) { throw SecretSyncException; }

            if(String.Equals(this.Secret,secret,StringComparison.Ordinal)) { this.Secret = null; this.Locked = false; return true; }

            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }

        finally { if(IsEntered(this.Sync.Secret)) { Exit(this.Sync.Secret); }
                  if(IsEntered(this.Sync.Locked)) { Exit(this.Sync.Locked); } }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="Validate"]/*'/>*/
    public static Boolean Validate(MSBuildItem? item)
    {
        if(item is null) { return false; }

        try { return new MSBuildItemValidator().Validate(item).IsValid; }

        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="AcquireLocks"]/*'/>*/
    private void AcquireLocks()
    {
        if(!TryEnter(this.Sync.AppDomainID,SyncTime))        { throw AppDomainIDSyncException; }
        if(!TryEnter(this.Sync.AppDomainUID,SyncTime))       { throw AppDomainUIDSyncException; }
        if(!TryEnter(this.Sync.Application,SyncTime))        { throw ApplicationSyncException; }
        if(!TryEnter(this.Sync.ApplicationVersion,SyncTime)) { throw ApplicationVersionSyncException; }
        if(!TryEnter(this.Sync.AssemblyVersion,SyncTime))    { throw AssemblyVersionSyncException; }
        if(!TryEnter(this.Sync.BornOn,SyncTime))             { throw BornOnSyncException; }
        if(!TryEnter(this.Sync.Certificates,SyncTime))       { throw CertificatesSyncException; }
        if(!TryEnter(this.Sync.Content,SyncTime))            { throw ContentSyncException; }
        if(!TryEnter(this.Sync.CPUID,SyncTime))              { throw CPUIDSyncException; }
        if(!TryEnter(this.Sync.DistinguishedName,SyncTime))  { throw DistinguishedNameSyncException; }
        if(!TryEnter(this.Sync.DomainID,SyncTime))           { throw DomainIDSyncException; }
        if(!TryEnter(this.Sync.Extension,SyncTime))          { throw ExtensionSyncException; }
        if(!TryEnter(this.Sync.FILE,SyncTime))               { throw FILESyncException; }
        if(!TryEnter(this.Sync.GPS,SyncTime))                { throw GPSSyncException; }
        if(!TryEnter(this.Sync.ID,SyncTime))                 { throw IDSyncException; }
        if(!TryEnter(this.Sync.Links,SyncTime))              { throw LinksSyncException; }
        if(!TryEnter(this.Sync.Locator,SyncTime))            { throw LocatorSyncException; }
        if(!TryEnter(this.Sync.Locked,SyncTime))             { throw LockedSyncException; }
        if(!TryEnter(this.Sync.MachineID,SyncTime))          { throw MachineIDSyncException; }
        if(!TryEnter(this.Sync.Modified,SyncTime))           { throw ModifiedSyncException; }
        if(!TryEnter(this.Sync.Name,SyncTime))               { throw NameSyncException; }
        if(!TryEnter(this.Sync.Notes,SyncTime))              { throw NotesSyncException; }
        if(!TryEnter(this.Sync.ProcessID,SyncTime))          { throw ProcessIDSyncException; }
        if(!TryEnter(this.Sync.Requirements,SyncTime))       { throw RequirementsSyncException; }
        if(!TryEnter(this.Sync.Secret,SyncTime))             { throw SecretSyncException; }
        if(!TryEnter(this.Sync.SecureHashes,SyncTime))       { throw SecureHashesSyncException; }
        if(!TryEnter(this.Sync.SecurityDescriptor,SyncTime)) { throw SecurityDescriptorSyncException; }
        if(!TryEnter(this.Sync.Sequence,SyncTime))           { throw SequenceSyncException; }
        if(!TryEnter(this.Sync.ServiceVersion,SyncTime))     { throw ServiceVersionSyncException; }
        if(!TryEnter(this.Sync.Tags,SyncTime))               { throw TagsSyncException; }
        if(!TryEnter(this.Sync.ThreadID,SyncTime))           { throw ThreadIDSyncException; }
        if(!TryEnter(this.Sync.Type,SyncTime))               { throw TypeSyncException; }
        if(!TryEnter(this.Sync.Version,SyncTime))            { throw VersionSyncException; }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/method[@name="ReleaseLocks"]/*'/>*/
    private void ReleaseLocks()
    {
        if(IsEntered(this.Sync.Version))            { Exit(this.Sync.Version); }
        if(IsEntered(this.Sync.Type))               { Exit(this.Sync.Type); }
        if(IsEntered(this.Sync.ThreadID))           { Exit(this.Sync.ThreadID); }
        if(IsEntered(this.Sync.Tags))               { Exit(this.Sync.Tags); }
        if(IsEntered(this.Sync.ServiceVersion))     { Exit(this.Sync.ServiceVersion); }
        if(IsEntered(this.Sync.Sequence))           { Exit(this.Sync.Sequence); }
        if(IsEntered(this.Sync.SecurityDescriptor)) { Exit(this.Sync.SecurityDescriptor); }
        if(IsEntered(this.Sync.SecureHashes))       { Exit(this.Sync.SecureHashes); }
        if(IsEntered(this.Sync.Secret))             { Exit(this.Sync.Secret); }
        if(IsEntered(this.Sync.Requirements))       { Exit(this.Sync.Requirements); }
        if(IsEntered(this.Sync.ProcessID))          { Exit(this.Sync.ProcessID); }
        if(IsEntered(this.Sync.Notes))              { Exit(this.Sync.Notes); }
        if(IsEntered(this.Sync.Name))               { Exit(this.Sync.Name); }
        if(IsEntered(this.Sync.Modified))           { Exit(this.Sync.Modified); }
        if(IsEntered(this.Sync.MachineID))          { Exit(this.Sync.MachineID); }
        if(IsEntered(this.Sync.Locked))             { Exit(this.Sync.Locked); }
        if(IsEntered(this.Sync.Locator))            { Exit(this.Sync.Locator); }
        if(IsEntered(this.Sync.Links))              { Exit(this.Sync.Links); }
        if(IsEntered(this.Sync.ID))                 { Exit(this.Sync.ID); }
        if(IsEntered(this.Sync.GPS))                { Exit(this.Sync.GPS); }
        if(IsEntered(this.Sync.FILE))               { Exit(this.Sync.FILE); }
        if(IsEntered(this.Sync.Extension))          { Exit(this.Sync.Extension); }
        if(IsEntered(this.Sync.DomainID))           { Exit(this.Sync.DomainID); }
        if(IsEntered(this.Sync.DistinguishedName))  { Exit(this.Sync.DistinguishedName); }
        if(IsEntered(this.Sync.CPUID))              { Exit(this.Sync.CPUID); }
        if(IsEntered(this.Sync.Content))            { Exit(this.Sync.Content); }
        if(IsEntered(this.Sync.Certificates))       { Exit(this.Sync.Certificates); }
        if(IsEntered(this.Sync.BornOn))             { Exit(this.Sync.BornOn); }
        if(IsEntered(this.Sync.AssemblyVersion))    { Exit(this.Sync.AssemblyVersion); }
        if(IsEntered(this.Sync.ApplicationVersion)) { Exit(this.Sync.ApplicationVersion); }
        if(IsEntered(this.Sync.Application))        { Exit(this.Sync.Application); }
        if(IsEntered(this.Sync.AppDomainUID))       { Exit(this.Sync.AppDomainUID); }
        if(IsEntered(this.Sync.AppDomainID))        { Exit(this.Sync.AppDomainID); }
    }

    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItem"]/property[@name="ExtensionData"]/*'/>*/
    public ExtensionDataObject? ExtensionData { get { return this.ExtnData!; } set { this.ExtnData = value; } }
}

/**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItemValidator"]/*'/>*/
public class MSBuildItemValidator : AbstractValidator<MSBuildItem>
{
    /**<include file='MSBuildItem.xml' path='MSBuildItem/class[@name="MSBuildItemValidator"]/constructor[@name="Constructor"]/*'/>*/
    public MSBuildItemValidator()
    {
        this.RuleFor( (item) => item.GetContent()).NotNull().NotEmpty();
        this.RuleFor( (item) => item.GetType()).NotNull().Equal(DataType.MSB);
    }
}