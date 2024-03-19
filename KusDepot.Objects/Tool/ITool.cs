namespace KusDepot;

/**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/main/*'/>*/
[ServiceContract(Name = "ITool" , Namespace = "KusDepot")]
public interface ITool
{
    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Activate"]/*'/>*/
    [OperationContract(Name = "Activate")]
    public Boolean Activate();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddActivity"]/*'/>*/
    public Boolean AddActivity(Activity? activity);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddDataItems"]/*'/>*/
    [OperationContract(Name = "AddDataItems")]
    public Boolean AddDataItems(IEnumerable<DataItem>? data);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddInput"]/*'/>*/
    [OperationContract(Name = "AddInput")]
    public Boolean AddInput(Object? input);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="AddNotes"]/*'/>*/
    [OperationContract(Name = "AddNotes")]
    public Boolean AddNotes(IEnumerable<String>? notes);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="AddOutput"]/*'/>*/
    [OperationContract(Name = "AddOutput")]
    public Boolean AddOutput(Guid id , Object? output);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="AddTags"]/*'/>*/
    [OperationContract(Name = "AddTags")]
    public Boolean AddTags(IEnumerable<String>? tags);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/event[@name="Alert"]/*'/>*/
    public event EventHandler<AlertEventArgs>? Alert;

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ClearEventLogs"]/*'/>*/
    [OperationContract(Name = "ClearEventLogs")]
    public Boolean ClearEventLogs();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Clone"]/*'/>*/
    public Object Clone();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="CompareTo"]/*'/>*/
    public Int32 CompareTo(Tool? other);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Deactivate"]/*'/>*/
    [OperationContract(Name = "Deactivate")]
    public Boolean Deactivate();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Dispose"]/*'/>*/
    public void Dispose();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(Object? other);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ExecuteCommand"]/*'/>*/
    [OperationContract(Name = "ExecuteCommand")]
    public Guid? ExecuteCommand(Object[]? details);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetActivities"]/*'/>*/
    public List<Activity>? GetActivities();

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetAppDomainID"]/*'/>*/
    [OperationContract(Name = "GetAppDomainID")]
    public Int64? GetAppDomainID();

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetAppDomainUID"]/*'/>*/
    [OperationContract(Name = "GetAppDomainUID")]
    public Int64? GetAppDomainUID();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetApplication"]/*'/>*/
    [OperationContract(Name = "GetApplication")]
    public String? GetApplication();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetApplicationVersion"]/*'/>*/
    [OperationContract(Name = "GetApplicationVersion")]
    public Version? GetApplicationVersion();

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetAssemblyVersion"]/*'/>*/
    [OperationContract(Name = "GetAssemblyVersion")]
    public Version? GetAssemblyVersion();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetBornOn"]/*'/>*/
    [OperationContract(Name = "GetBornOn")]
    public DateTimeOffset? GetBornOn();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetCertificates"]/*'/>*/
    [OperationContract(Name = "GetCertificates")]
    public Dictionary<String,String>? GetCertificates();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetCommands"]/*'/>*/
    public Dictionary<String,Command>? GetCommands();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetContainer"]/*'/>*/
    public IContainer? GetContainer();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetControls"]/*'/>*/
    [OperationContract(Name = "GetControls")]
    public HashSet<String>? GetControls();

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetCPUID"]/*'/>*/
    [OperationContract(Name = "GetCPUID")]
    public String? GetCPUID();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetDataItems"]/*'/>*/
    [OperationContract(Name = "GetDataItems")]
    public HashSet<DataItem>? GetDataItems();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetDescriptor"]/*'/>*/
    public Descriptor? GetDescriptor();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetDisposed"]/*'/>*/
    public Boolean GetDisposed();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetDistinguishedName"]/*'/>*/
    [OperationContract(Name = "GetDistinguishedName")]
    public String? GetDistinguishedName();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetDomainID"]/*'/>*/
    [OperationContract(Name = "GetDomainID")]
    public String? GetDomainID();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetEvent"]/*'/>*/
    [OperationContract(Name = "GetEvent")]
    public String? GetEvent(Int32? key);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetEventLogs"]/*'/>*/
    [OperationContract(Name = "GetEventLogs")]
    public Dictionary<Int32,String>? GetEventLogs();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetExtension"]/*'/>*/
    [OperationContract(Name = "GetExtension")]
    public Dictionary<String,Object?>? GetExtension();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetFILE"]/*'/>*/
    [OperationContract(Name = "GetFILE")]
    public String? GetFILE();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetGPS"]/*'/>*/
    [OperationContract(Name = "GetGPS")]
    public String? GetGPS();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetID"]/*'/>*/
    [OperationContract(Name = "GetID")]
    public Guid? GetID();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetInput"]/*'/>*/
    [OperationContract(Name = "GetInput")]
    public Object? GetInput();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetInputs"]/*'/>*/
    [OperationContract(Name = "GetInputs")]
    public Queue<Object>? GetInputs();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetLinks"]/*'/>*/
    [OperationContract(Name = "GetLinks")]
    public Dictionary<String,GuidReferenceItem>? GetLinks();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetLocator"]/*'/>*/
    [OperationContract(Name = "GetLocator")]
    public Uri? GetLocator();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetLocked"]/*'/>*/
    [OperationContract(Name = "GetLocked")]
    public Boolean GetLocked();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetLocked"]/*'/>*/
    [OperationContract(Name = "GetMachineID")]
    public String? GetMachineID();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetModified"]/*'/>*/
    [OperationContract(Name = "GetModified")]
    public DateTimeOffset? GetModified();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetName"]/*'/>*/
    [OperationContract(Name = "GetName")]
    public String? GetName();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetNotes"]/*'/>*/
    [OperationContract(Name = "GetNotes")]
    public HashSet<String>? GetNotes();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetObjectives"]/*'/>*/
    [OperationContract(Name = "GetObjectives")]
    public IList<Object>? GetObjectives();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetOutput"]/*'/>*/
    [OperationContract(Name = "GetOutput")]
    public Object? GetOutput(Guid? id);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetOutputIDs"]/*'/>*/
    [OperationContract(Name = "GetOutputIDs")]
    public IList<Guid>? GetOutputIDs();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetPolicies"]/*'/>*/
    [OperationContract(Name = "GetPolicies")]
    public IList<Object>? GetPolicies();

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetProcessID"]/*'/>*/
    [OperationContract(Name = "GetProcessID")]
    public Int64? GetProcessID();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetPurpose"]/*'/>*/
    [OperationContract(Name = "GetPurpose")]
    public String? GetPurpose();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetSecurityDescriptor"]/*'/>*/
    [OperationContract(Name = "GetSecurityDescriptor")]
    public String? GetSecurityDescriptor();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetServiceVersion"]/*'/>*/
    [OperationContract(Name = "GetServiceVersion")]
    public Version? GetServiceVersion();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetStatus"]/*'/>*/
    [OperationContract(Name = "GetStatus")]
    public Dictionary<String,Object?>? GetStatus();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetSubordinates"]/*'/>*/
    [OperationContract(Name = "GetSubordinates")]
    public HashSet<Tool>? GetSubordinates();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetSuperior"]/*'/>*/
    [OperationContract(Name = "GetSuperior")]
    public Tool? GetSuperior();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetTags"]/*'/>*/
    [OperationContract(Name = "GetTags")]
    public HashSet<String>? GetTags();

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="GetTelemetry"]/*'/>*/
    [OperationContract(Name = "GetTelemetry")]
    public HashSet<String>? GetTelemetry();

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="GetThreadID"]/*'/>*/
    [OperationContract(Name = "GetThreadID")]
    public Int32? GetThreadID();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="GetVersion"]/*'/>*/
    [OperationContract(Name = "GetVersion")]
    public Version? GetVersion();

    /**<include file='../Context/Context.xml' path='Context/class[@name="Context"]/method[@name="Initialize"]/*'/>*/
    [OperationContract(Name = "Initialize")]
    public Boolean Initialize();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="Lock"]/*'/>*/
    [OperationContract(Name = "Lock")]
    public Boolean Lock(String? secret);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="LogEvent"]/*'/>*/
    [OperationContract(Name = "LogEvent")]
    public Boolean LogEvent(String? eventdata);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RaiseAlert"]/*'/>*/
    public void RaiseAlert(Object? sender , AlertEventArgs eventargs , Boolean synchronous = false);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RegisterCommand"]/*'/>*/
    public Boolean RegisterCommand(Command command , String handle);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveActivity"]/*'/>*/
    public Boolean RemoveActivity(Activity? activity);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveDataItem"]/*'/>*/
    [OperationContract(Name = "RemoveDataItem")]
    public Boolean RemoveDataItem(Guid? id);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="RemoveNote"]/*'/>*/
    [OperationContract(Name = "RemoveNote")]
    public Boolean RemoveNote(String? note);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveOutput"]/*'/>*/
    [OperationContract(Name = "RemoveOutput")]
    public Boolean RemoveOutput(Guid? id);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="RemoveStatus"]/*'/>*/
    [OperationContract(Name = "RemoveStatus")]
    public Boolean RemoveStatus(String? key);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="RemoveTag"]/*'/>*/
    [OperationContract(Name = "RemoveTag")]
    public Boolean RemoveTag(String? tag);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="Serialize"]/*'/>*/
    [OperationContract(Name = "Serialize")]
    public String? Serialize();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetApplication"]/*'/>*/
    [OperationContract(Name = "SetApplication")]
    public Boolean SetApplication(String? application);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetApplicationVersion"]/*'/>*/
    [OperationContract(Name = "SetApplicationVersion")]
    public Boolean SetApplicationVersion(Version? applicationversion);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetBornOn"]/*'/>*/
    [OperationContract(Name = "SetBornOn")]
    public Boolean SetBornOn(DateTimeOffset? bornon);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetCertificates"]/*'/>*/
    [OperationContract(Name = "SetCertificates")]
    public Boolean SetCertificates(IDictionary<String,String>? certificates);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetContainer"]/*'/>*/
    public Boolean SetContainer(Autofac.IContainer? container);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetControls"]/*'/>*/
    public Boolean SetControls(IEnumerable<String>? controls);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetDistinguishedName"]/*'/>*/
    [OperationContract(Name = "SetDistinguishedName")]
    public Boolean SetDistinguishedName(String? distinguishedname);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetDomainID"]/*'/>*/
    [OperationContract(Name = "SetDomainID")]
    public Boolean SetDomainID(String? domainid);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetExtension"]/*'/>*/
    [OperationContract(Name = "SetExtension")]
    public Boolean SetExtension(IDictionary<String,Object?>? extension);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetFILE"]/*'/>*/
    [OperationContract(Name = "SetFILE")]
    public Boolean SetFILE(String? file);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetGPS"]/*'/>*/
    [OperationContract(Name = "SetGPS")]
    public Boolean SetGPS(String? gps);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetID"]/*'/>*/
    [OperationContract(Name = "SetID")]
    public Boolean SetID(Guid? id);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetLinks"]/*'/>*/
    [OperationContract(Name = "SetLinks")]
    public Boolean SetLinks(IDictionary<String,GuidReferenceItem>? links);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetLocator"]/*'/>*/
    [OperationContract(Name = "SetLocator")]
    public Boolean SetLocator(Uri? locator);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetModified"]/*'/>*/
    [OperationContract(Name = "SetModified")]
    public Boolean SetModified(DateTimeOffset? modified);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetName"]/*'/>*/
    [OperationContract(Name = "SetName")]
    public Boolean SetName(String? name);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetObjectives"]/*'/>*/
    [OperationContract(Name = "SetObjectives")]
    public Boolean SetObjectives(IEnumerable<Object>? objectives);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetPolicies"]/*'/>*/
    [OperationContract(Name = "SetPolicies")]
    public Boolean SetPolicies(IEnumerable<Object>? policies);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetPurpose"]/*'/>*/
    [OperationContract(Name = "SetPurpose")]
    public Boolean SetPurpose(String? purpose);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetSecurityDescriptor"]/*'/>*/
    [OperationContract(Name = "SetSecurityDescriptor")]
    public Boolean SetSecurityDescriptor(String? securitydescriptor);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetServiceVersion"]/*'/>*/
    [OperationContract(Name = "SetServiceVersion")]
    public Boolean SetServiceVersion(Version? serviceversion);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetStatus"]/*'/>*/
    [OperationContract(Name = "SetStatus")]
    public Boolean SetStatus(String? key , Object? status);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetSubordinates"]/*'/>*/
    [OperationContract(Name = "SetSubordinates")]
    public Boolean SetSubordinates(HashSet<Tool>? subordinates);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetSuperior"]/*'/>*/
    [OperationContract(Name = "SetSuperior")]
    public Boolean SetSuperior(Tool? superior);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="SetTelemetry"]/*'/>*/
    public Boolean SetTelemetry(IEnumerable<String>? telemetry);

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="SetVersion"]/*'/>*/
    [OperationContract(Name = "SetVersion")]
    public Boolean SetVersion(Version? version);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StartAsync"]/*'/>*/
    public Task StartAsync(CancellationToken token);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="StopAsync"]/*'/>*/
    public Task StopAsync(CancellationToken token);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="ToString"]/*'/>*/
    public String? ToString();

    /**<include file='../Common/Common.xml' path='Common/class[@name="Common"]/method[@name="UnLock"]/*'/>*/
    [OperationContract(Name = "UnLock")]
    public Boolean UnLock(String? secret);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="UnRegisterCommand"]/*'/>*/
    public Boolean UnRegisterCommand(String? handle);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="UpdateInputs"]/*'/>*/
    [OperationContract(Name = "UpdateInputs")]
    public Boolean UpdateInputs(Queue<Object>? inputs);

    /**<include file='ITool.xml' path='ITool/interface[@name="ITool"]/method[@name="WriteEventLogsToFile"]/*'/>*/
    [OperationContract(Name = "WriteEventLogsToFile")]
    public Boolean WriteEventLogsToFile(String path);
}