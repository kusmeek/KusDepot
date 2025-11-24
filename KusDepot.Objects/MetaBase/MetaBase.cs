namespace KusDepot;

/**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/main/*'/>*/
[GenerateSerializer] [Alias("KusDepot.MetaBase")]
[DataContract(Name = "MetaBase" , Namespace = "KusDepot")]
public abstract class MetaBase : Common , IMetaBase
{
    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Application"]/*'/>*/
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)] [Id(0)]
    protected String? Application;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="ApplicationVersion"]/*'/>*/
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    protected Version? ApplicationVersion;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="BornOn"]/*'/>*/
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)] [Id(2)]
    protected DateTimeOffset? BornOn;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="DistinguishedName"]/*'/>*/
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)] [Id(3)]
    protected String? DistinguishedName;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="DomainID"]/*'/>*/
    [DataMember(Name = "DomainID" , EmitDefaultValue = true , IsRequired = true)] [Id(4)]
    protected String? DomainID;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Extension"]/*'/>*/
    [DataMember(Name = "Extension" , EmitDefaultValue = true , IsRequired = true)] [Id(5)]
    protected Dictionary<String,Object?>? Extension;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="ExtnData"]/*'/>*/
    [NonSerialized]
    protected ExtensionDataObject? ExtnData;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="FILE"]/*'/>*/
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)] [Id(6)]
    protected String? FILE;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Links"]/*'/>*/
    [DataMember(Name = "Links" , EmitDefaultValue = true , IsRequired = true)] [Id(7)]
    protected Dictionary<String,GuidReferenceItem>? Links;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Modified"]/*'/>*/
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)] [Id(8)]
    protected DateTimeOffset? Modified;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Name"]/*'/>*/
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)] [Id(9)]
    protected String? Name;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)] [Id(10)]
    protected HashSet<String>? Notes;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="SecurityDescriptor"]/*'/>*/
    [DataMember(Name = "SecurityDescriptor" , EmitDefaultValue = true , IsRequired = true)] [Id(11)]
    protected String? SecurityDescriptor;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="ServiceVersion"]/*'/>*/
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)] [Id(12)]
    protected Version? ServiceVersion;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Sync"]/*'/>*/
    [IgnoreDataMember] [NonSerialized]
    protected new MetaSync Sync;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)] [Id(13)]
    protected HashSet<String>? Tags;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/field[@name="Version"]/*'/>*/
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)] [Id(14)]
    protected Version? Version;

    /**<include file='MetaBase.xml' path='MetaBase/class[@name="MetaBase"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    protected MetaBase() { this.Sync = (this.GetSyncNode() as MetaSync)!; }

    ///<inheritdoc/>
    public virtual Boolean AddNotes(IEnumerable<String>? notes)
    {
        try
        {
            if( notes is null || this.Locked ) { return false; }

            HashSet<String> _ = new(notes.Select(_=>new String(_))); if(Equals(_.Count,0)) { return true; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.Notes is null) { this.Notes = _; MN(); return true; }

                this.Notes.UnionWith(_); MN(); return true;
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

            HashSet<String> _ = new(tags.Select(_=>new String(_))); if(Equals(_.Count,0)) { return true; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(this.Tags is null) { this.Tags = _; MN(); return true; }

                this.Tags.UnionWith(_); MN(); return true;
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

            try { return new(this.Application); }

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

            try { return new(this.ApplicationVersion.ToString()); }

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
            return this.GetID() is null ? null :

            new()
            {
                Application        = this.GetApplication(),
                ApplicationVersion = this.GetApplicationVersion()?.ToString(),
                BornOn             = this.GetBornOn()?.ToString("O"),
                DistinguishedName  = this.GetDistinguishedName(),
                FILE               = this.GetFILE(),
                ID                 = this.GetID(),
                Modified           = this.GetModified()?.ToString("O"),
                Name               = this.GetName(),
                Notes              = this.GetNotes(),
                ObjectType         = GetInheritanceList(this),
                ServiceVersion     = this.GetServiceVersion()?.ToString(),
                Tags               = this.GetTags(),
                Version            = this.GetVersion()?.ToString()
            };
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

            try { return new(this.DistinguishedName); }

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

            try { return new(this.DomainID); }

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

            try { return this.Extension.ToDictionary(_=>new String(_.Key),_=>_.Value); }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetExtensionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual String? GetFILE()
    {
        try
        {
            if(this.FILE is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return new(this.FILE); }

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

            try { return this.Links.ToDictionary(_=>new String(_.Key),_=>(_.Value.Clone() as GuidReferenceItem)!); }

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

            try { return new(this.Name); }

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

            try { return this.Notes.Select(_=>new String(_)).ToHashSet(); }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetNotesFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    public virtual String? GetSecurityDescriptor()
    {
        try
        {
            if(this.SecurityDescriptor is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return new(this.SecurityDescriptor); }

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

            try { return new(this.ServiceVersion.ToString()); }

            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,GetServiceVersionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null; } throw; }
    }

    ///<inheritdoc/>
    protected override SyncNode GetSyncNode()
    {
        try { this.Sync ??= new(); return this.Sync; }

        catch ( Exception _ ) { KusDepotLog.Error(_,GetSyncNodeFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return null!; } throw; }
    }

    ///<inheritdoc/>
    public virtual HashSet<String>? GetTags()
    {
        try
        {
            if(this.Tags is null) { return null; }

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try { return this.Tags.Select(_=>new String(_)).ToHashSet(); }

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

            try { return new(this.Version.ToString()); }

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
                if(this.Notes.Remove(note)) { if(Equals(this.Notes.Count,0)) { this.Notes = null; } MN(); return true; }

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
                if(this.Tags.Remove(tag)) { if(Equals(this.Tags.Count,0)) { this.Tags = null; } MN(); return true; }

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
                if(String.IsNullOrEmpty(application)) { this.Application = null; MN(); return true; }

                this.Application = new(application); MN(); return true;
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
                if(Version.Equals(applicationversion,Version.Parse("0.0.0.0"))) { this.ApplicationVersion = null; MN(); return true; }

                this.ApplicationVersion = new(applicationversion.ToString()); MN(); return true;
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
                if(Equals(bornon,DateTimeOffset.MinValue)) { this.BornOn = null; return true; }

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
                if(String.IsNullOrEmpty(distinguishedname)) { this.DistinguishedName = null; MN(); return true; }

                this.DistinguishedName = new(distinguishedname); MN(); return true;
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
                if(String.IsNullOrEmpty(domainid)) { this.DomainID = null; MN(); return true; }

                this.DomainID = new(domainid); MN(); return true;
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

            Dictionary<String,Object?> _ = extension.ToDictionary(_=>new String(_.Key),_=>_.Value);

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(Equals(_.Count,0)) { this.Extension = null; MN(); return true; }

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
                if(String.IsNullOrEmpty(file)) { this.FILE = null; MN(); return true; }

                this.FILE = new(file); MN(); return true;
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

            Dictionary<String,GuidReferenceItem> _ = links.ToDictionary(_=>new String(_.Key),_=>(_.Value.Clone() as GuidReferenceItem)!);

            if(!this.Sync.Meta.Wait(SyncTime)) { throw SyncException; }

            try
            {
                if(Equals(_.Count,0)) { this.Links = null; MN(); return true; }

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
                if(Equals(modified,DateTimeOffset.MinValue)) { this.Modified = null; return true; }

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

            if(Equals(modified,DateTimeOffset.MinValue)) { this.Modified = null; return true; }

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
                if(String.IsNullOrEmpty(name)) { this.Name = null; MN(); return true; }

                this.Name = new(name); MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetNameFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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
                if(String.IsNullOrEmpty(securitydescriptor)) { this.SecurityDescriptor = null; MN(); return true; }

                this.SecurityDescriptor = new(securitydescriptor); MN(); return true;
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
                if(Version.Equals(serviceversion,Version.Parse("0.0.0.0"))) { this.ServiceVersion = null; MN(); return true; }

                this.ServiceVersion = new(serviceversion.ToString()); MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetServiceVersionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
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
                if(Version.Equals(version,Version.Parse("0.0.0.0"))) { this.Version = null; MN(); return true; }

                this.Version = new(version.ToString()); MN(); return true;
            }
            finally { this.Sync.Meta.Release(); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,SetVersionFail,MyTypeName,MyID); if(NoExceptions||MyNoExceptions) { return false; } throw; }
    }
}