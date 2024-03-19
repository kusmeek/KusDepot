namespace KusDepot;

/**<include file='Common.xml' path='Common/class[@name="Common"]/main/*'/>*/
[DataContract(Name = "Common" , Namespace = "KusDepot")]
public abstract class Common : Context
{
    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Application"]/*'/>*/
    [DataMember(Name = "Application" , EmitDefaultValue = true , IsRequired = true)]
    protected String? Application;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="ApplicationVersion"]/*'/>*/
    [DataMember(Name = "ApplicationVersion" , EmitDefaultValue = true , IsRequired = true)]
    protected Version? ApplicationVersion;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="BornOn"]/*'/>*/
    [DataMember(Name = "BornOn" , EmitDefaultValue = true , IsRequired = true)]
    protected DateTimeOffset? BornOn;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Certificates"]/*'/>*/
    [DataMember(Name = "Certificates" , EmitDefaultValue = true , IsRequired = true)]
    protected Dictionary<String,String>? Certificates;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="DistinguishedName"]/*'/>*/
    [DataMember(Name = "DistinguishedName" , EmitDefaultValue = true , IsRequired = true)]
    protected String? DistinguishedName;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="DomainID"]/*'/>*/
    [DataMember(Name = "DomainID" , EmitDefaultValue = true , IsRequired = true)]
    protected String? DomainID;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Extension"]/*'/>*/
    [DataMember(Name = "Extension" , EmitDefaultValue = true , IsRequired = true)]
    protected Dictionary<String,Object?>? Extension;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="ExtnData"]/*'/>*/
    protected ExtensionDataObject? ExtnData;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="FILE"]/*'/>*/
    [DataMember(Name = "FILE" , EmitDefaultValue = true , IsRequired = true)]
    protected String? FILE;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="GPS"]/*'/>*/
    [DataMember(Name = "GPS" , EmitDefaultValue = true , IsRequired = true)]
    protected String? GPS;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    protected Guid? ID;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Links"]/*'/>*/
    [DataMember(Name = "Links" , EmitDefaultValue = true , IsRequired = true)]
    protected Dictionary<String,GuidReferenceItem>? Links;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Locator"]/*'/>*/
    [DataMember(Name = "Locator" , EmitDefaultValue = true , IsRequired = true)]
    protected Uri? Locator;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Locked"]/*'/>*/
    [DataMember(Name = "Locked" , EmitDefaultValue = true , IsRequired = true)]
    protected Boolean Locked;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Modified"]/*'/>*/
    [DataMember(Name = "Modified" , EmitDefaultValue = true , IsRequired = true)]
    protected DateTimeOffset? Modified;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Name"]/*'/>*/
    [DataMember(Name = "Name" , EmitDefaultValue = true , IsRequired = true)]
    protected String? Name;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Notes"]/*'/>*/
    [DataMember(Name = "Notes" , EmitDefaultValue = true , IsRequired = true)]
    protected HashSet<String>? Notes;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Secret"]/*'/>*/
    [DataMember(Name = "Secret" , EmitDefaultValue = true , IsRequired = true)]
    protected String? Secret;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="SecurityDescriptor"]/*'/>*/
    [DataMember(Name = "SecurityDescriptor" , EmitDefaultValue = true , IsRequired = true)]
    protected String? SecurityDescriptor;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="ServiceVersion"]/*'/>*/
    [DataMember(Name = "ServiceVersion" , EmitDefaultValue = true , IsRequired = true)]
    protected Version? ServiceVersion;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Tags"]/*'/>*/
    [DataMember(Name = "Tags" , EmitDefaultValue = true , IsRequired = true)]
    protected HashSet<String>? Tags;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/field[@name="Version"]/*'/>*/
    [DataMember(Name = "Version" , EmitDefaultValue = true , IsRequired = true)]
    protected Version? Version;

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="AddNotes"]/*'/>*/
    public virtual Boolean AddNotes(IEnumerable<String>? notes)
    {
        try
        {
            if( notes is null || this.Locked ) { return false; }

            HashSet<String> _ = new(notes.Select(_=>new String(_))); if(Equals(_.Count,0)) { return false; }

            if(this.Notes is null) { this.Notes = _; return true; }

            this.Notes.UnionWith(_); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="AddTags"]/*'/>*/
    public virtual Boolean AddTags(IEnumerable<String>? tags)
    {
        try
        {
            if( tags is null || this.Locked ) { return false; }

            HashSet<String> _ = new(tags.Select(_=>new String(_))); if(Equals(_.Count,0)) { return true; }

            if(this.Tags is null) { this.Tags = _; return true; }

            this.Tags.UnionWith(_); return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetApplication"]/*'/>*/
    public virtual String? GetApplication() { return this.Application is null ? null : new String(this.Application); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetApplicationVersion"]/*'/>*/
    public virtual Version? GetApplicationVersion() { return this.ApplicationVersion is null ? null : new Version(this.ApplicationVersion.ToString()); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetBornOn"]/*'/>*/
    public virtual DateTimeOffset? GetBornOn() { return this.BornOn; }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetCertificates"]/*'/>*/
    public virtual Dictionary<String,String>? GetCertificates()
    {
        try
        {
            return this.Certificates?.ToDictionary(_=>new String(_.Key),_=>new String(_.Value));
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetDistinguishedName"]/*'/>*/
    public virtual String? GetDistinguishedName() { return this.DistinguishedName is null ? null : new String(this.DistinguishedName); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetDomainID"]/*'/>*/
    public virtual String? GetDomainID() { return this.DomainID is null ? null : new String(this.DomainID); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetExtension"]/*'/>*/
    public virtual Dictionary<String,Object?>? GetExtension() { return this.Extension?.ToDictionary(_=>new String(_.Key),_=>_.Value); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetFILE"]/*'/>*/
    public virtual String? GetFILE() { return this.FILE is null ? null : new String(this.FILE); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetGPS"]/*'/>*/
    public virtual String? GetGPS() { return this.GPS is null ? null : new String(this.GPS); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetID"]/*'/>*/
    public virtual Guid? GetID() { return this.ID; }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetLinks"]/*'/>*/
    public virtual Dictionary<String,GuidReferenceItem>? GetLinks()
    {
        try
        {
            return this.Links?.ToDictionary(_=>new String(_.Key),_=>(GuidReferenceItem)_.Value.Clone());
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetLocator"]/*'/>*/
    public virtual Uri? GetLocator() { return this.Locator is null ? null : new Uri(this.Locator.ToString()); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetLocked"]/*'/>*/
    public virtual Boolean GetLocked() { return this.Locked; }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetModified"]/*'/>*/
    public virtual DateTimeOffset? GetModified() { return this.Modified; }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetName"]/*'/>*/
    public virtual String? GetName() { return this.Name is null ? null : new String(this.Name); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetNotes"]/*'/>*/
    public virtual HashSet<String>? GetNotes()
    {
        try
        {
            return this.Notes?.Select(_=>new String(_)).ToHashSet();
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetSecurityDescriptor"]/*'/>*/
    public virtual String? GetSecurityDescriptor() { return this.SecurityDescriptor is null ? null : new String(this.SecurityDescriptor); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetServiceVersion"]/*'/>*/
    public virtual Version? GetServiceVersion() { return this.ServiceVersion is null ? null : new Version(this.ServiceVersion.ToString()); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetTags"]/*'/>*/
    public virtual HashSet<String>? GetTags()
    {
        try
        {
            return this.Tags?.Select(_=>new String(_)).ToHashSet();
        }
        catch ( Exception ) { if(NoExceptions) { return null; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetVersion"]/*'/>*/
    public virtual Version? GetVersion() { return this.Version is null ? null : new Version(this.Version.ToString()); }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="Lock"]/*'/>*/
    public virtual Boolean Lock(String? secret)
    {
        try
        {
            if( secret is null || this.Locked ) { return false; }

            this.Secret = new String(secret);

            this.Locked = true;

            return true;
        }
        catch ( Exception ) { this.Secret = null; this.Locked = false; if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="RemoveNote"]/*'/>*/
    public virtual Boolean RemoveNote(String? note)
    {
        try
        {
            if( note is null || this.Notes is null || this.Locked ) { return false; }

            if(this.Notes.Remove(note)) { if(Equals(this.Notes.Count,0)) { this.Notes = null; } return true; }

            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="RemoveTag"]/*'/>*/
    public virtual Boolean RemoveTag(String? tag)
    {
        try
        {
            if( tag is null || this.Tags is null || this.Locked ) { return false; }

            if(this.Tags.Remove(tag)) { if(Equals(this.Tags.Count,0)) { this.Tags = null; } return true; }

            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetApplication"]/*'/>*/
    public virtual Boolean SetApplication(String? application)
    {
        try
        {
            if( application is null || this.Locked ) { return false; }

            if(String.IsNullOrEmpty(application)) { this.Application = null; return true; }

            this.Application = new String(application);

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="GetApplicationVersion"]/*'/>*/
    public virtual Boolean SetApplicationVersion(Version? applicationversion)
    {
        try
        {
            if( applicationversion is null || this.Locked ) { return false; }

            if(Version.Equals(applicationversion,Version.Parse("0.0.0.0"))) { this.ApplicationVersion = null; return true; }

            this.ApplicationVersion = new Version(applicationversion.ToString());

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetBornOn"]/*'/>*/
    public virtual Boolean SetBornOn(DateTimeOffset? bornon)
    {
        try
        {
            if( bornon is null || this.Locked ) { return false; }

            if(Equals(bornon,DateTimeOffset.MinValue)) { this.BornOn = null; return true; }

            this.BornOn = bornon;

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetCertificates"]/*'/>*/
    public virtual Boolean SetCertificates(IDictionary<String,String>? certificates)
    {
        try
        {
            if( certificates is null || this.Locked ) { return false; }

            this.Certificates = certificates.ToDictionary(_=>new String(_.Key),_=>new String(_.Value));

            if(Equals(this.Certificates.Count,0)) { this.Certificates = null; } return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetDistinguishedName"]/*'/>*/
    public virtual Boolean SetDistinguishedName(String? distinguishedname)
    {
        try
        {
            if( distinguishedname is null || this.Locked ) { return false; }

            if(String.IsNullOrEmpty(distinguishedname)) { this.DistinguishedName = null; return true; }

            this.DistinguishedName = new String(distinguishedname);

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetDomainID"]/*'/>*/
    public virtual Boolean SetDomainID(String? domainid)
    {
        try
        {
            if( domainid is null || this.Locked ) { return false; }

            if(String.IsNullOrEmpty(domainid)) { this.DomainID = null; return true; }

            this.DomainID = new String(domainid);

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetExtension"]/*'/>*/
    public virtual Boolean SetExtension(IDictionary<String,Object?>? extension)
    {
        try
        {
            if( extension is null || this.Locked ) { return false; }

            this.Extension = extension.ToDictionary(_=>new String(_.Key),_=>_.Value);

            if(Equals(this.Extension.Count,0)) { this.Extension = null; } return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetFILE"]/*'/>*/
    public virtual Boolean SetFILE(String? file)
    {
        try
        {
            if( file is null || this.Locked ) { return false; }

            if(String.IsNullOrEmpty(file)) { this.FILE = null; return true; }

            this.FILE = new String(file);

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetGPS"]/*'/>*/
    public virtual Boolean SetGPS(String? gps)
    {
        try
        {
            if( gps is null || this.Locked ) { return false; }

            if(String.IsNullOrEmpty(gps)) { this.GPS = null; return true; }

            this.GPS = new String(gps);

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetID"]/*'/>*/
    public virtual Boolean SetID(Guid? id)
    {
        try
        {
            if( id is null || this.Locked ) { return false; }

            if(Guid.Equals(id,Guid.Empty)) { this.ID = null; return true; }

            this.ID = id;

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetLinks"]/*'/>*/
    public virtual Boolean SetLinks(IDictionary<String,GuidReferenceItem>? links)
    {
        try
        {
            if( links is null || this.Locked ) { return false; }

            this.Links = links.ToDictionary(_=>new String(_.Key),_=>(GuidReferenceItem)_.Value.Clone());

            if(Equals(this.Links.Count,0)) { this.Links = null; } return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetLocator"]/*'/>*/
    public virtual Boolean SetLocator(Uri? locator)
    {
        try
        {
            if( locator is null || this.Locked ) { return false; }

            if(Uri.Equals(locator,new Uri("null:"))) { this.Locator = null; return true; }

            this.Locator = new Uri(locator.ToString());

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetModified"]/*'/>*/
    public virtual Boolean SetModified(DateTimeOffset? modified)
    {
        try
        {
            if( modified is null || this.Locked ) { return false; }

            if(Equals(modified,DateTimeOffset.MinValue)) { this.Modified = null; return true; }

            this.Modified = modified;

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetName"]/*'/>*/
    public virtual Boolean SetName(String? name)
    {
        try
        {
            if( name is null || this.Locked ) { return false; }

            if(String.IsNullOrEmpty(name)) { this.Name = null; return true; }

            this.Name = new String(name);

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetSecurityDescriptor"]/*'/>*/
    public virtual Boolean SetSecurityDescriptor(String? securitydescriptor)
    {
        try
        {
            if( securitydescriptor is null || this.Locked ) { return false; }

            if(String.IsNullOrEmpty(securitydescriptor)) { this.SecurityDescriptor = null; return true; }

            this.SecurityDescriptor = new String(securitydescriptor);

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetServiceVersion"]/*'/>*/
    public virtual Boolean SetServiceVersion(Version? serviceversion)
    {
        try
        {
            if( serviceversion is null || this.Locked ) { return false; }

            if(Version.Equals(serviceversion,Version.Parse("0.0.0.0"))) { this.ServiceVersion = null; return true; }

            this.ServiceVersion = new Version(serviceversion.ToString());

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="SetVersion"]/*'/>*/
    public virtual Boolean SetVersion(Version? version)
    {
        try
        {
            if( version is null || this.Locked ) { return false; }

            if(Version.Equals(version,Version.Parse("0.0.0.0"))) { this.Version = null; return true; }

            this.Version = new Version(version.ToString());

            return true;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }

    /**<include file='Common.xml' path='Common/class[@name="Common"]/method[@name="UnLock"]/*'/>*/
    public virtual Boolean UnLock(String? secret)
    {
        try
        {
            if( secret is null || !this.Locked ) { return false; }

            if(String.Equals(this.Secret,secret,StringComparison.Ordinal)) { this.Secret = null; this.Locked = false; return true; }

            return false;
        }
        catch ( Exception ) { if(NoExceptions) { return false; } throw; }
    }
}