namespace KusDepot;

/**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.MetaBase")]
[DataContract(Name = "MetaBase" , Namespace = "KusDepot")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]

public abstract partial class MetaBase : Common , IMetaBase
{
    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Application"]/*'/>*/
    [JsonPropertyName("Application")] [JsonInclude]
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = false)] [Id(0)]
    protected internal String? Application;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="ApplicationVersion"]/*'/>*/
    [JsonPropertyName("ApplicationVersion")] [JsonInclude]
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = false)] [Id(1)]
    protected internal Version? ApplicationVersion;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="BornOn"]/*'/>*/
    [JsonPropertyName("BornOn")] [JsonInclude]
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    protected internal DateTimeOffset? BornOn;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="DistinguishedName"]/*'/>*/
    [JsonPropertyName("DistinguishedName")] [JsonInclude]
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = false)] [Id(3)]
    protected internal String? DistinguishedName;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="DomainID"]/*'/>*/
    [JsonPropertyName("DomainID")] [JsonInclude]
    [DataMember(Name = "DomainID" , EmitDefaultValue = true , IsRequired = false)] [Id(4)]
    protected internal String? DomainID;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Extension"]/*'/>*/
    [DataMember(Name = "Extension" , EmitDefaultValue = true , IsRequired = false)] [Id(5)]
    protected internal Dictionary<String,Object?>? Extension;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="ExtnData"]/*'/>*/
    [NonSerialized]
    protected ExtensionDataObject? ExtnData;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="FILE"]/*'/>*/
    [JsonPropertyName("FILE")] [JsonInclude]
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = false)] [Id(6)]
    protected internal String? FILE;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="FileSizeCached"]/*'/>*/
    [IgnoreDataMember] [NonSerialized]
    protected Int64? FileSizeCached;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="InheritanceCache"]/*'/>*/
    private static readonly ConcurrentDictionary<Type,String> InheritanceCache;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Links"]/*'/>*/
    [JsonPropertyName("Links")] [JsonInclude]
    [DataMember(Name = "Links" , EmitDefaultValue = true , IsRequired = false)] [Id(7)]
    protected internal Dictionary<String,GuidReferenceItem>? Links;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Modified"]/*'/>*/
    [JsonPropertyName("Modified")] [JsonInclude]
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = false)] [Id(8)]
    protected internal DateTimeOffset? Modified;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Name"]/*'/>*/
    [JsonPropertyName("Name")] [JsonInclude]
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = false)] [Id(9)]
    protected internal String? Name;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Notes"]/*'/>*/
    [JsonPropertyName("Notes")] [JsonInclude]
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = false)] [Id(10)]
    protected internal HashSet<String>? Notes;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="ObjectFILE"]/*'/>*/
    [JsonPropertyName("ObjectFILE")] [JsonInclude]
    [DataMember(Name = "ObjectFILE" , EmitDefaultValue = true , IsRequired = false)] [Id(11)]
    protected internal String? ObjectFILE;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="SecurityDescriptor"]/*'/>*/
    [JsonPropertyName("SecurityDescriptor")] [JsonInclude]
    [DataMember(Name = "SecurityDescriptor" , EmitDefaultValue = true , IsRequired = false)] [Id(12)]
    protected internal String? SecurityDescriptor;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="ServiceVersion"]/*'/>*/
    [JsonPropertyName("ServiceVersion")] [JsonInclude]
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = false)] [Id(13)]
    protected internal Version? ServiceVersion;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Sync"]/*'/>*/
    [IgnoreDataMember] [NonSerialized]
    protected new MetaSync Sync;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Tags"]/*'/>*/
    [JsonPropertyName("Tags")] [JsonInclude]
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = false)] [Id(14)]
    protected internal HashSet<String>? Tags;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Version"]/*'/>*/
    [JsonPropertyName("Version")] [JsonInclude]
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = false)] [Id(15)]
    protected internal Version? Version;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/constructor[@name="StaticConstructor"]/*'/>*/
    static MetaBase() { InheritanceCache = new(); }

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    protected MetaBase() { this.Sync = (this.GetSyncNode() as MetaSync)!; }

    ///<inheritdoc/>
    public virtual Boolean AddNotes(IEnumerable<String>? notes)
    {
        try
        {
            if( notes is null || this.Locked ) { return false; }

            HashSet<String> _ = DataIsolation.IsEnabled() ? CreateIsolatedSet(notes) : notes as HashSet<String> ?? new(notes); if(_.Count == 0) { return true; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.Notes is null) { this.Notes = _; MN(); return true; }

                Int32 c = this.Notes.Count; this.Notes.UnionWith(_);

                if(Equals(c,this.Notes.Count)) { return true; }

                MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,AddNotesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean AddTags(IEnumerable<String>? tags)
    {
        try
        {
            if( tags is null || this.Locked ) { return false; }

            HashSet<String> _ = DataIsolation.IsEnabled() ? CreateIsolatedSet(tags) : tags as HashSet<String> ?? new(tags); if(_.Count == 0) { return true; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.Tags is null) { this.Tags = _; MN(); return true; }

                Int32 c = this.Tags.Count; this.Tags.UnionWith(_);

                if(Equals(c,this.Tags.Count)) { return true; }

                MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,AddTagsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual String? GetApplication()
    {
        try
        {
            if(this.Application is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? new(this.Application) : this.Application; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetApplicationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Version? GetApplicationVersion()
    {
        try
        {
            if(this.ApplicationVersion is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? new(this.ApplicationVersion.ToString()) : this.ApplicationVersion; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetApplicationVersionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual DateTimeOffset? GetBornOn()
    {
        try
        {
            if(this.BornOn is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return this.BornOn; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetBornOnFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Descriptor? GetDescriptor()
    {
        try
        {
            Guid? id = this.GetID(); if(id is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(DataIsolation.IsEnabled())
                {
                    return new()
                    {
                        Application        = this.Application is null ? null : new(this.Application),
                        ApplicationVersion = this.ApplicationVersion is null ? null : new(this.ApplicationVersion.ToString()),
                        BornOn             = this.BornOn?.ToString("O"),
                        DistinguishedName  = this.DistinguishedName is null ? null : new(this.DistinguishedName),
                        FILE               = this.FILE is null ? null : new(this.FILE),
                        ID                 = id,
                        Modified           = this.Modified?.ToString("O"),
                        Name               = this.Name is null ? null : new(this.Name),
                        Notes              = this.Notes?.Select(n => new String(n)).ToHashSet(),
                        ObjectFILE         = this.ObjectFILE is null ? null : new(this.ObjectFILE),
                        ObjectType         = this.GetCachedInheritanceList(),
                        ServiceVersion     = this.ServiceVersion?.ToString(),
                        Tags               = this.Tags?.Select(t => new String(t)).ToHashSet(),
                        Version            = this.Version?.ToString()
                    };
                }

                return new()
                {
                    Application        = this.Application,
                    ApplicationVersion = this.ApplicationVersion?.ToString(),
                    BornOn             = this.BornOn?.ToString("O"),
                    DistinguishedName  = this.DistinguishedName,
                    FILE               = this.FILE,
                    ID                 = id,
                    Modified           = this.Modified?.ToString("O"),
                    Name               = this.Name,
                    Notes              = this.Notes,
                    ObjectFILE         = this.ObjectFILE,
                    ObjectType         = this.GetCachedInheritanceList(),
                    ServiceVersion     = this.ServiceVersion?.ToString(),
                    Tags               = this.Tags,
                    Version            = this.Version?.ToString()
                };
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDescriptorFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual String? GetDistinguishedName()
    {
        try
        {
            if(this.DistinguishedName is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? new(this.DistinguishedName) : this.DistinguishedName; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDistinguishedNameFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual String? GetDomainID()
    {
        try
        {
            if(this.DomainID is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? new(this.DomainID) : this.DomainID; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetDomainIDFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Dictionary<String,Object?>? GetExtension()
    {
        try
        {
            if( this.Extension is null || this.Locked) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? new(this.Extension) : this.Extension; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetExtensionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Byte[] GetMetaBaseIntegrityHash()
    {
        try
        {
            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                Dictionary<String,Object?> projection = new(StringComparer.Ordinal)
                {
                    ["Application"] = this.Application,
                    ["ApplicationVersion"] = this.ApplicationVersion,
                    ["BornOn"] = this.BornOn,
                    ["DistinguishedName"] = this.DistinguishedName,
                    ["DomainID"] = this.DomainID,
                    ["Extension"] = this.Extension,
                    ["Links"] = this.Links,
                    ["MetaBaseType"] = this.GetType().FullName,
                    ["Name"] = this.Name,
                    ["Notes"] = this.Notes,
                    ["SecurityDescriptor"] = this.SecurityDescriptor,
                    ["ServiceVersion"] = this.ServiceVersion,
                    ["Tags"] = this.Tags,
                    ["Version"] = this.Version,
                };

                return GenerateDeterministicSHA512(projection,DeterministicHashingProfile.MetaBaseIntegrity);
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetMetaBaseIntegrityHashFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return Array.Empty<Byte>(); } throw; }
    }

    ///<inheritdoc/>
    public virtual String? GetFILE()
    {
        try
        {
            if(this.FILE is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? new(this.FILE) : this.FILE; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetFILEFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Dictionary<String,GuidReferenceItem>? GetLinks()
    {
        try
        {
            if(this.Links is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? this.Links.ToDictionary(_=>_.Key,_=>(_.Value.Clone() as GuidReferenceItem)!) : this.Links; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetLinksFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual DateTimeOffset? GetModified()
    {
        try
        {
            if(this.Modified is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return this.Modified; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetModifiedFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual String? GetName()
    {
        try
        {
            if(this.Name is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? new(this.Name) : this.Name; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetNameFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual HashSet<String>? GetNotes()
    {
        try
        {
            if(this.Notes is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? this.Notes.Select(n => new String(n)).ToHashSet() : this.Notes; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetNotesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual String? GetObjectFILE()
    {
        try
        {
            if(this.ObjectFILE is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? new(this.ObjectFILE) : this.ObjectFILE; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetObjectFILEFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual String? GetSecurityDescriptor()
    {
        try
        {
            if(this.SecurityDescriptor is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? new(this.SecurityDescriptor) : this.SecurityDescriptor; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetSecurityDescriptorFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Version? GetServiceVersion()
    {
        try
        {
            if(this.ServiceVersion is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? new(this.ServiceVersion.ToString()) : this.ServiceVersion; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetServiceVersionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    protected override SyncNode GetSyncNode() { this.Sync ??= new(); return this.Sync; }

    ///<inheritdoc/>
    public virtual HashSet<String>? GetTags()
    {
        try
        {
            if(this.Tags is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? this.Tags.Select(t => new String(t)).ToHashSet() : this.Tags; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetTagsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual Version? GetVersion()
    {
        try
        {
            if(this.Version is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return DataIsolation.IsEnabled() ? new(this.Version.ToString()) : this.Version; }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetVersionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public override Boolean Initialize() { this.GetSyncNode(); return base.Initialize(); }

    ///<inheritdoc/>
    protected override void RefreshSyncNodes()
    {
        try { this.Sync = (this.GetSyncNode() as MetaSync)!; base.RefreshSyncNodes(); }

        catch ( Exception _ ) { KusDepotLog.Error(_,RefreshSyncNodeFail,MyTypeName,MyID); throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean RemoveNote(String? note)
    {
        try
        {
            if( note is null || this.Notes is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.Notes.Remove(note)) { if(this.Notes.Count == 0) { this.Notes = null; } MN(); return true; }

                return false;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RemoveNoteFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean RemoveTag(String? tag)
    {
        try
        {
            if( tag is null || this.Tags is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.Tags.Remove(tag)) { if(this.Tags.Count == 0) { this.Tags = null; } MN(); return true; }

                return false;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,RemoveTagFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetApplication(String? application)
    {
        try
        {
            if( application is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                String? _ = String.IsNullOrEmpty(application) ? null : application;

                if(String.Equals(this.Application,_,Ordinal)) { return true; }

                this.Application = _ is null ? null : DataIsolation.IsEnabled() ? new(_) : _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetApplicationFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetApplicationVersion(Version? applicationversion)
    {
        try
        {
            if( applicationversion is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                Version? _ = Version.Equals(applicationversion,EmptyVersion) ? null : applicationversion;

                if(Version.Equals(this.ApplicationVersion,_)) { return true; }

                this.ApplicationVersion = _ is null ? null : DataIsolation.IsEnabled() ? new(_.ToString()) : _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetApplicationVersionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetBornOn(DateTimeOffset? bornon)
    {
        try
        {
            if( bornon is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(bornon == DateTimeOffset.MinValue) { this.BornOn = null; return true; }

                this.BornOn = bornon; return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetBornOnFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetDistinguishedName(String? distinguishedname)
    {
        try
        {
            if( distinguishedname is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                String? _ = String.IsNullOrEmpty(distinguishedname) ? null : distinguishedname;

                if(String.Equals(this.DistinguishedName,_,Ordinal)) { return true; }

                this.DistinguishedName = _ is null ? null : DataIsolation.IsEnabled() ? new(_) : _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetDistinguishedNameFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetDomainID(String? domainid)
    {
        try
        {
            if( domainid is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                String? _ = String.IsNullOrEmpty(domainid) ? null : domainid;

                if(String.Equals(this.DomainID,_,Ordinal)) { return true; }

                this.DomainID = _ is null ? null : DataIsolation.IsEnabled() ? new(_) : _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetDomainIDFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetExtension(IDictionary<String,Object?>? extension)
    {
        try
        {
            if( extension is null || this.Locked ) { return false; }

            Dictionary<String,Object?> _ = DataIsolation.IsEnabled() ? new(extension) : extension as Dictionary<String,Object?> ?? new(extension);

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(_.Count == 0) { this.Extension = null; MN(); return true; }

                this.Extension = _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetExtensionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetFILE(String? file)
    {
        try
        {
            if( file is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                String? _ = String.IsNullOrEmpty(file) ? null : file;

                if(String.Equals(this.FILE,_,Ordinal)) { return true; }

                this.FILE = _ is null ? null : DataIsolation.IsEnabled() ? new(_) : _; this.ClearCachedFileSize_NoSync(); MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetFILEFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetLinks(IDictionary<String,GuidReferenceItem>? links)
    {
        try
        {
            if( links is null || this.Locked ) { return false; }

            Dictionary<String,GuidReferenceItem> _ = DataIsolation.IsEnabled() ? links.ToDictionary(_=>_.Key,_=>(_.Value.Clone() as GuidReferenceItem)!) : links as Dictionary<String,GuidReferenceItem> ?? new(links);

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(_.Count == 0)
                {
                    if(this.Links is null) { return true; }

                    this.Links = null; MN(); return true;
                }

                this.Links = _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetLinksFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetModified(DateTimeOffset? modified)
    {
        try
        {
            if( modified is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(modified == DateTimeOffset.MinValue) { this.Modified = null; return true; }

                this.Modified = modified; return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetModifiedFail); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/method[@name="SetModified_NoSync"]/*'/>*/
    protected virtual Boolean SetModified_NoSync(DateTimeOffset? modified)
    {
        try
        {
            if( modified is null || this.Locked ) { return false; }

            if(modified == DateTimeOffset.MinValue) { this.Modified = null; return true; }

            this.Modified = modified; return true;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetModifiedFail); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/method[@name="ModifiedNow"]/*'/>*/
    protected virtual Boolean MN() => this.SetModified_NoSync(DateTimeOffset.Now);

    ///<inheritdoc/>
    public virtual Boolean SetName(String? name)
    {
        try
        {
            if( name is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                String? _ = String.IsNullOrEmpty(name) ? null : name;

                if(String.Equals(this.Name,_,Ordinal)) { return true; }

                this.Name = _ is null ? null : DataIsolation.IsEnabled() ? new(_) : _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetNameFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetNotes(IEnumerable<String>? notes)
    {
        try
        {
            if( notes is null || this.Locked ) { return false; }

            HashSet<String> _ = DataIsolation.IsEnabled() ? CreateIsolatedSet(notes) : notes as HashSet<String> ?? new(notes);

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(_.Count == 0)
                {
                    if(this.Notes is null) { return true; }

                    this.Notes = null; MN(); return true;
                }

                if(this.Notes?.SetEquals(_) is true) { return true; }

                this.Notes = _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetNotesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetObjectFILE(String? objectfile)
    {
        try
        {
            if( objectfile is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                String? _ = String.IsNullOrEmpty(objectfile) ? null : objectfile;

                if(String.Equals(this.ObjectFILE,_,Ordinal)) { return true; }

                this.ObjectFILE = _ is null ? null : DataIsolation.IsEnabled() ? new(_) : _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetObjectFILEFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetSecurityDescriptor(String? securitydescriptor)
    {
        try
        {
            if( securitydescriptor is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                String? _ = String.IsNullOrEmpty(securitydescriptor) ? null : securitydescriptor;

                if(String.Equals(this.SecurityDescriptor,_,Ordinal)) { return true; }

                this.SecurityDescriptor = _ is null ? null : DataIsolation.IsEnabled() ? new(_) : _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetSecurityDescriptorFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetServiceVersion(Version? serviceversion)
    {
        try
        {
            if( serviceversion is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                Version? _ = Version.Equals(serviceversion,EmptyVersion) ? null : serviceversion;

                if(Version.Equals(this.ServiceVersion,_)) { return true; }

                this.ServiceVersion = _ is null ? null : DataIsolation.IsEnabled() ? new(_.ToString()) : _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetServiceVersionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetTags(IEnumerable<String>? tags)
    {
        try
        {
            if( tags is null || this.Locked ) { return false; }

            HashSet<String> _ = DataIsolation.IsEnabled() ? CreateIsolatedSet(tags) : tags as HashSet<String> ?? new(tags);

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(_.Count == 0)
                {
                    if(this.Tags is null) { return true; }

                    this.Tags = null; MN(); return true;
                }

                if(this.Tags?.SetEquals(_) is true) { return true; }

                this.Tags = _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetTagsFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    public virtual Boolean SetVersion(Version? version)
    {
        try
        {
            if( version is null || this.Locked ) { return false; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                Version? _ = Version.Equals(version,EmptyVersion) ? null : version;

                if(Version.Equals(this.Version,_)) { return true; }

                this.Version = _ is null ? null : DataIsolation.IsEnabled() ? new(_.ToString()) : _; MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetVersionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }
}