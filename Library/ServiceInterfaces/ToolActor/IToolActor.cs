namespace ToolActor;

/**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/main/*'/>*/
public interface IToolActor : IActor
{
    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="Activate"]/*'/>*/
    public Task<Boolean> Activate();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="AddDataItems"]/*'/>*/
    public Task<Boolean> AddDataItems(IEnumerable<DataItem>? data);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="AddInput"]/*'/>*/
    public Task<Boolean> AddInput(Object? input);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="AddNotes"]/*'/>*/
    public Task<Boolean> AddNotes(IEnumerable<String>? notes);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="AddOutput"]/*'/>*/
    public Task<Boolean> AddOutput(Guid id , Object? output);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="AddTags"]/*'/>*/
    public Task<Boolean> AddTags(IEnumerable<String>? tags);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="ClearEventLogs"]/*'/>*/
    public Task<Boolean> ClearEventLogs();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="Deactivate"]/*'/>*/
    public Task<Boolean> Deactivate();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="ExecuteCommand"]/*'/>*/
    public Task<Guid?> ExecuteCommand(Object[]? details);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetActorID"]/*'/>*/
    public Task<Guid?> GetActorID();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetAppDomainID"]/*'/>*/
    public Task<Int64?> GetAppDomainID();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetAppDomainUID"]/*'/>*/
    public Task<Int64?> GetAppDomainUID();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetApplication"]/*'/>*/
    public Task<String?> GetApplication();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetApplicationVersion"]/*'/>*/
    public Task<Version?> GetApplicationVersion();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetAssemblyVersion"]/*'/>*/
    public Task<Version?> GetAssemblyVersion();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetBornOn"]/*'/>*/
    public Task<DateTimeOffset?> GetBornOn();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetCertificates"]/*'/>*/
    public Task<Dictionary<String,String>?> GetCertificates();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetCPUID"]/*'/>*/
    public Task<String?> GetCPUID();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetDataItems"]/*'/>*/
    public Task<HashSet<DataItem>?> GetDataItems();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetDistinguishedName"]/*'/>*/
    public Task<String?> GetDistinguishedName();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetDomainID"]/*'/>*/
    public Task<String?> GetDomainID();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetEvent"]/*'/>*/
    public Task<String?> GetEvent(Int32? key);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetEventLogs"]/*'/>*/
    public Task<Dictionary<Int32,String>?> GetEventLogs();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetExtension"]/*'/>*/
    public Task<Dictionary<String,Object?>?> GetExtension();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetGPS"]/*'/>*/
    public Task<String?> GetGPS();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetID"]/*'/>*/
    public Task<Guid?> GetID();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetInput"]/*'/>*/
    public Task<Object?> GetInput();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetInputs"]/*'/>*/
    public Task<Queue<Object>?> GetInputs();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetLinks"]/*'/>*/
    public Task<Dictionary<String,GuidReferenceItem>?> GetLinks();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetLocator"]/*'/>*/
    public Task<Uri?> GetLocator();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetLocked"]/*'/>*/
    public Task<Boolean> GetLocked();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetMachineID"]/*'/>*/
    public Task<String?> GetMachineID();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetModified"]/*'/>*/
    public Task<DateTimeOffset?> GetModified();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetName"]/*'/>*/
    public Task<String?> GetName();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetNotes"]/*'/>*/
    public Task<HashSet<String>?> GetNotes();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetObjectives"]/*'/>*/
    public Task<List<Object>?> GetObjectives();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetOutput"]/*'/>*/
    public Task<Object?> GetOutput(Guid? id);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetOutputIDs"]/*'/>*/
    public Task<List<Guid>?> GetOutputIDs();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetPolicies"]/*'/>*/
    public Task<List<Object>?> GetPolicies();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetProcessID"]/*'/>*/
    public Task<Int64?> GetProcessID();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetPurpose"]/*'/>*/
    public Task<String?> GetPurpose();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetSecurityDescriptor"]/*'/>*/
    public Task<String?> GetSecurityDescriptor();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetServiceVersion"]/*'/>*/
    public Task<Version?> GetServiceVersion();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetStatus"]/*'/>*/
    public Task<Dictionary<String,Object?>?> GetStatus();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetStringID"]/*'/>*/
    public Task<String?> GetStringID();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetTags"]/*'/>*/
    public Task<HashSet<String>?> GetTags();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetThreadID"]/*'/>*/
    public Task<Int32?> GetThreadID();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="GetVersion"]/*'/>*/
    public Task<Version?> GetVersion();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="Initialize"]/*'/>*/
    public Task<Boolean> Initialize();

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="Lock"]/*'/>*/
    public Task<Boolean> Lock(String? secret);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="LogEvent"]/*'/>*/
    public Task<Boolean> LogEvent(String? eventdata);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="RemoveDataItem"]/*'/>*/
    public Task<Boolean> RemoveDataItem(Guid? id);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="RemoveNote"]/*'/>*/
    public Task<Boolean> RemoveNote(String? note);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="RemoveOutput"]/*'/>*/
    public Task<Boolean> RemoveOutput(Guid? id);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="RemoveStatus"]/*'/>*/
    public Task<Boolean> RemoveStatus(String? key);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="RemoveTag"]/*'/>*/
    public Task<Boolean> RemoveTag(String? tag);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetApplication"]/*'/>*/
    public Task<Boolean> SetApplication(String? application);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetApplicationVersion"]/*'/>*/
    public Task<Boolean> SetApplicationVersion(Version? applicationversion);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetBornOn"]/*'/>*/
    public Task<Boolean> SetBornOn(DateTimeOffset? bornon);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetCertificates"]/*'/>*/
    public Task<Boolean> SetCertificates(Dictionary<String,String>? certificates);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetDistinguishedName"]/*'/>*/
    public Task<Boolean> SetDistinguishedName(String? distinguishedname);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetDomainID"]/*'/>*/
    public Task<Boolean> SetDomainID(String? domainid);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetExtension"]/*'/>*/
    public Task<Boolean> SetExtension(Dictionary<String,Object?>? extension);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetGPS"]/*'/>*/
    public Task<Boolean> SetGPS(String? gps);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetID"]/*'/>*/
    public Task<Boolean> SetID(Guid? id);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetLinks"]/*'/>*/
    public Task<Boolean> SetLinks(Dictionary<String,GuidReferenceItem>? links);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetLocator"]/*'/>*/
    public Task<Boolean> SetLocator(Uri? locator);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetModified"]/*'/>*/
    public Task<Boolean> SetModified(DateTimeOffset? modified);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetName"]/*'/>*/
    public Task<Boolean> SetName(String? name);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetObjectives"]/*'/>*/
    public Task<Boolean> SetObjectives(IEnumerable<Object>? objectives);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetPolicies"]/*'/>*/
    public Task<Boolean> SetPolicies(IEnumerable<Object>? policies);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetPurpose"]/*'/>*/
    public Task<Boolean> SetPurpose(String? purpose);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetSecurityDescriptor"]/*'/>*/
    public Task<Boolean> SetSecurityDescriptor(String? securitydescriptor);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetServiceVersion"]/*'/>*/
    public Task<Boolean> SetServiceVersion(Version? serviceversion);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetStatus"]/*'/>*/
    public Task<Boolean> SetStatus(String? key , Object? status);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="SetVersion"]/*'/>*/
    public Task<Boolean> SetVersion(Version? version);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="UnLock"]/*'/>*/
    public Task<Boolean> UnLock(String? secret);

    /**<include file='IToolActor.xml' path='IToolActor/interface[@name="IToolActor"]/method[@name="UpdateInputs"]/*'/>*/
    public Task<Boolean> UpdateInputs(Queue<Object>? inputs);
}