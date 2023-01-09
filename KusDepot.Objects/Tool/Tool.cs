namespace KusDepot
{
    /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]'/> */
    [DataContract(Name = "Tool" , Namespace = "KusDepot")]
    public class Tool : Context , ITool
    {
        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Activities"]'/> */
        [IgnoreDataMember]
        public List<Task<Object?>>? Activities;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Controls"]'/> */
        [DataMember(Name = "Controls" , EmitDefaultValue = false , IsRequired = false)]
        public List<IPEndPoint>? Controls;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Data"]'/> */
        [DataMember(Name = "Data" , EmitDefaultValue = false , IsRequired = true)]
        public List<DataItem>? Data;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "EventLogs"]'/> */
        [DataMember(Name = "EventLogs" , EmitDefaultValue = false , IsRequired = false)]
        public LinkedList<String>? EventLogs;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Inputs"]'/> */
        [DataMember(Name = "Inputs" , EmitDefaultValue = false , IsRequired = false)]
        public Queue<Object>? Inputs;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Name"]'/> */
        [DataMember(Name = "Name" , EmitDefaultValue = false , IsRequired = false)]
        public String? Name;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Notes"]'/> */
        [DataMember(Name = "Notes" , EmitDefaultValue = false , IsRequired = false)]
        public List<String>? Notes;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Objectives"]'/> */
        [DataMember(Name = "Objectives" , EmitDefaultValue = false , IsRequired = false)]
        public List<Object>? Objectives;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Outputs"]'/> */
        [DataMember(Name = "Outputs" , EmitDefaultValue = false , IsRequired = false)]
        public List<Object>? Outputs;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Policies"]'/> */
        [DataMember(Name = "Policies" , EmitDefaultValue = false , IsRequired = false)]
        public List<Object>? Policies;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Purpose"]'/> */
        [DataMember(Name = "Purpose" , EmitDefaultValue = false , IsRequired = false)]
        public String? Purpose;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Status"]'/> */
        [DataMember(Name = "Status" , EmitDefaultValue = false , IsRequired = false)]
        public Stack<Object>? Status;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Subordinates"]'/> */
        [IgnoreDataMember]
        public List<Tool>? Subordinates;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Superior"]'/> */
        [IgnoreDataMember]
        public Tool? Superior;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Tags"]'/> */
        [DataMember(Name = "Tags" , EmitDefaultValue = false , IsRequired = false)]
        public List<String>? Tags;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/field[@Name = "Telemetry"]'/> */
        [DataMember(Name = "Telemetry" , EmitDefaultValue = false , IsRequired = false)]
        public List<IPEndPoint>? Telemetry;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/constructor[@Name = "ParameterlessConstructor"]'/> */
        public Tool() { this.Initialize(new Guid()); }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/constructor[@Name = "Constructor"]'/> */
        public Tool(List<DataItem>? data , Guid? id , Queue<Object>? inputs , String? name , List<String>? notes , List<Object>? objectives , List<Object>? policies , String? purpose , List<String>? tags)
        {
            try
            {
                this.Initialize(id);
                if(data is not null)        { this.Data!.AddRange(data); }
                if(inputs is not null)      { this.Inputs  = inputs; }
                if(name is not null)        { this.Name    = name; }
                if(notes is not null)       { this.Notes!.AddRange(notes); }
                if(objectives is not null)  { this.Objectives = objectives; }
                if(policies is not null)    { this.Policies = policies; }
                if(purpose is not null)     { this.Purpose = purpose; }
                if(tags is not null)        { this.Tags!.AddRange(tags); }
            }
            catch ( Exception ie ) { throw new Exception("KusDepot.Tool.Constructor",ie); }
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/event[@Name = "Alert"]'/> */
        public event EventHandler<EventArgs>? Alert;

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "Activate"]'/> */
        public Boolean Activate() { return true; }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "AddDataItems"]'/> */
        public Boolean AddDataItems(List<DataItem>? data)
        {
            if(data is null) { return false; }

            try
            {
                this.Data!.AddRange(data);
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "AddNotes"]'/> */
        public Boolean AddNotes(List<String>? notes)
        {
            if(notes is null) { return false; }

            try
            {
                this.Notes!.AddRange(notes);
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "AddTags"]'/> */
        public Boolean AddTags(List<String>? tags)
        {
            if(tags is null) { return false; }

            try
            {
                this.Tags!.AddRange(tags);
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "Command"]'/> */
        public Boolean Command(Object[]? details) { return true; }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "CommandTasks"]'/> */
        public Boolean Command(List<Task<Object?>>? tasks) { return true; }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "Deactivate"]'/> */
        public Boolean Deactivate() { return true; }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetActivities"]'/> */
        public List<Task<Object?>>? GetActivities()
        {
            return this.Activities;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetAppDomainID"]'/> */
        public Int128? GetAppDomainID()
        {
            return this.AppDomainID;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetAppDomainUID"]'/> */
        public Int128? GetAppDomainUID()
        {
            return this.AppDomainUID;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetAssemblyVersion"]'/> */
        public Version? GetAssemblyVersion()
        {
            return this.AssemblyVersion;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetBornOn"]'/> */
        public DateTime? GetBornOn()
        {
            return this.BornOn;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetControls"]'/> */
        public List<IPEndPoint>? GetControls()
        {
            return this.Controls;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetCPUID"]'/> */
        public Int64? GetCPUID()
        {
            return this.CPUID;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetDataItems"]'/> */
        public List<DataItem>? GetDataItems()
        {
            return this.Data;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetDomainID"]'/> */
        public String? GetDomainID()
        {
            return this.DomainID;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetEventLogs"]'/> */
        public LinkedList<String>? GetEventLogs()
        {
            return this.EventLogs;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetExtension"]'/> */
        public dynamic? GetExtension()
        {
            return this.Extension;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetGPS"]'/> */
        public String? GetGPS()
        {
            return this.GPS;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetHashCode"]'/> */
        public override Int32 GetHashCode()
        {
            return HashCode.Combine<DateTime?,Guid?,String?>(BornOn,ID,Name);
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetID"]'/> */
        public Guid? GetID()
        {
            return this.ID;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetInputs"]'/> */
        public Queue<Object>? GetInputs()
        {
            return this.Inputs;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetMachineID"]'/> */
        public String? GetMachineID()
        {
            return this.MachineID;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetModified"]'/> */
        public DateTime? GetModified()
        {
            return this.Modified;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetName"]'/> */
        public String? GetName()
        {
            return this.Name;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetNotes"]'/> */
        public List<String>? GetNotes()
        {
            return this.Notes;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetObjectives"]'/> */
        public List<Object>? GetObjectives()
        {
            return this.Objectives;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetOutputs"]'/> */
        public List<Object>? GetOutputs()
        {
            return this.Outputs;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetPolicies"]'/> */
        public List<Object>? GetPolicies()
        {
            return this.Policies;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetProcessID"]'/> */
        public Int128? GetProcessID()
        {
            return this.ProcessID;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetSecurityDescriptor"]'/> */
        public String? GetSecurityDescriptor()
        {
            return this.SecurityDescriptor;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetStatus"]'/> */
        public Stack<Object>? GetStatus()
        {
            return this.Status;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetSubordinates"]'/> */
        public List<Tool>? GetSubordinates()
        {
            return this.Subordinates;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetSuperior"]'/> */
        public Tool? GetSuperior()
        {
            return this.Superior;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetTags"]'/> */
        public List<String>? GetTags()
        {
            return this.Tags;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetTelemetry"]'/> */
        public List<IPEndPoint>? GetTelemetry()
        {
            return this.Telemetry;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "GetURI"]'/> */
        public String? GetURI()
        {
            return this.URI;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "InitializeContext"]'/> */
        public override void Initialize(Guid? id)
        {
            try
            {
                this.AppDomainID     = AppDomain.CurrentDomain.Id;

                this.AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

                this.BornOn          = DateTime.Now;

                this.ID              = id ?? new Guid();

                Process me           = Process.GetCurrentProcess();

                this.MachineID       = me.MachineName;

                this.ProcessID       = me.Id;

                ((ITool)this).Initialize();
            }
            catch( Exception ) { return; }
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "InitializeTool"]'/> */
        void ITool.Initialize()
        {
            this.Data  = new List<DataItem>{ };
            this.Notes = new List<String>{ };
            this.Tags  = new List<String>{ };
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "RaiseAlert"]'/> */
        public void RaiseAlert(Object? sender, EventArgs eventargs)
        {
            if( (this.Alert is null) || (sender is null) || (eventargs is null) ) { return; }

            Alert(sender, eventargs);
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "SetExtension"]'/> */
        public Boolean SetExtension(dynamic? extension)
        {
            if(extension is null) { return false; }

            try
            {
                this.Extension = extension;
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "SetModified"]'/> */
        public Boolean SetModified(DateTime? datetime)
        {
            if(datetime is null) { return false; }

            try
            {
                this.Modified = datetime;
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "SetName"]'/> */
        public Boolean SetName(String? name)
        {
            if(name is null) { return false; }

            try
            {
                this.Name = name;
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "SetObjectives"]'/> */
        public Boolean SetObjectives(List<Object>? objectives)
        {
            if(objectives is null) { return false; }

            try
            {
                this.Objectives = objectives;
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "SetPolicies"]'/> */
        public Boolean SetPolicies(List<Object>? policies)
        {
            if(policies is null) { return false; }

            try
            {
                this.Policies = policies;
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "SetSecurityDescriptor"]'/> */
        public Boolean SetSecurityDescriptor(String? securitydescriptor)
        {
            if(securitydescriptor is null) { return false; }

            try
            {
                this.SecurityDescriptor = securitydescriptor;
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "SetSubordinates"]'/> */
        public Boolean SetSubordinates(List<Tool>? subordinates)
        {
            if(subordinates is null) { return false; }

            try
            {
                this.Subordinates = subordinates;
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "SetSuperior"]'/> */
        public Boolean SetSuperior(Tool? superior)
        {
            if(superior is null) { return false; }

            try
            {
                this.Superior = superior;
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "SetURI"]'/> */
        public Boolean SetURI(String? uri)
        {
            if(uri is null) { return false; }

            try
            {
                this.URI = uri;
            }
            catch ( Exception ) { return false; }

            return true;
        }

        /** <include file = 'Tool.xml' path = 'Tool/class[@Name = "Tool"]/method[@Name = "UpdateInputs"]'/> */
        public Boolean UpdateInputs(Queue<Object>? inputs)
        {
            if(inputs is null) { return false; }

            try
            {
                this.Inputs = inputs;
            }
            catch ( Exception ) { return false; }

            return true;
        }

    }

}