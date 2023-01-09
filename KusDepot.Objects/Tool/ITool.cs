namespace KusDepot
{
    /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]'/> */
    public interface ITool
    {
        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "Activate"]'/> */
        public Boolean Activate();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "AddDataItems"]'/> */
        public Boolean AddDataItems(List<DataItem>? data);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "AddNotes"]'/> */
        public Boolean AddNotes(List<String>? notes);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "AddTags"]'/> */
        public Boolean AddTags(List<String>? tags);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "Command"]'/> */
        public Boolean Command(Object[]? details);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "CommandTasks"]'/> */
        public Boolean Command(List<Task<Object?>>? tasks);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "Deactivate"]'/> */
        public Boolean Deactivate();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetActivities"]'/> */
        public List<Task<Object?>>? GetActivities();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetAppDomainID"]'/> */
        public Int128? GetAppDomainID();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetAppDomainUID"]'/> */
        public Int128? GetAppDomainUID();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetAssemblyVersion"]'/> */
        public Version? GetAssemblyVersion();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetBornOn"]'/> */
        public DateTime? GetBornOn();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetControls"]'/> */
        public List<IPEndPoint>? GetControls();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetCPUID"]'/> */
        public Int64? GetCPUID();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetDataItems"]'/> */
        public List<DataItem>? GetDataItems();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetDomainID"]'/> */
        public String? GetDomainID();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetEventLogs"]'/> */
        public LinkedList<String>? GetEventLogs();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetExtension"]'/> */
        public dynamic? GetExtension();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetGPS"]'/> */
        public String? GetGPS();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetID"]'/> */
        public Guid? GetID();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetInputs"]'/> */
        public Queue<Object>? GetInputs();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetMachineID"]'/> */
        public String? GetMachineID();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetModified"]'/> */
        public DateTime? GetModified();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetName"]'/> */
        public String? GetName();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetNotes"]'/> */
        public List<String>? GetNotes();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetObjectives"]'/> */
        public List<Object>? GetObjectives();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetOutputs"]'/> */
        public List<Object>? GetOutputs();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetPolicies"]'/> */
        public List<Object>? GetPolicies();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetProcessID"]'/> */
        public Int128? GetProcessID();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetSecurityDescriptor"]'/> */
        public String? GetSecurityDescriptor();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetStatus"]'/> */
        public Stack<Object>? GetStatus();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetSubordinates"]'/> */
        public List<Tool>? GetSubordinates();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetSuperior"]'/> */
        public Tool? GetSuperior();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetTags"]'/> */
        public List<String>? GetTags();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetTelemetry"]'/> */
        public List<IPEndPoint>? GetTelemetry();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "GetURI"]'/> */
        public String? GetURI();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "Initialize"]'/> */
        public void Initialize();

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "RaiseAlert"]'/> */
        public void RaiseAlert(Object sender, EventArgs eventargs);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "SetExtension"]'/> */
        public Boolean SetExtension(dynamic? extension);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "SetModified"]'/> */
        public Boolean SetModified(DateTime? datetime);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "SetName"]'/> */
        public Boolean SetName(String? name);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "SetObjectives"]'/> */
        public Boolean SetObjectives(List<Object>? objectives);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "SetPolicies"]'/> */
        public Boolean SetPolicies(List<Object>? policies);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "SetSecurityDescriptor"]'/> */
        public Boolean SetSecurityDescriptor(String? securitydescriptor);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "SetSubordinates"]'/> */
        public Boolean SetSubordinates(List<Tool>? subordinates);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "SetSuperior"]'/> */
        public Boolean SetSuperior(Tool? superior);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "SetURI"]'/> */
        public Boolean SetURI(String? name);

        /** <include file = 'ITool.xml' path = 'ITool/interface[@Name = "ITool"]/method[@Name = "UpdateInputs"]'/> */
        public Boolean UpdateInputs(Queue<Object>? inputs);
    }

}