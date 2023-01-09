namespace KusDepot.Exams
{
    /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]'/> */
    [Parallelizable,TestFixture]
    public class ToolExam
    {
        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/event[@Name = "Alert"]'/> */
        [Test]
        public void Alert()
        {
            Tool _0 = new Tool();
            String _1 = "AlertExam";
            _0.Alert += new EventHandler<EventArgs>(this.HandleAlert);
            _0.RaiseAlert(this,new UnhandledExceptionEventArgs(new Exception(_1),false));
        }

        private void HandleAlert(Object? sender , EventArgs e)
        {
            UnhandledExceptionEventArgs _0 = (UnhandledExceptionEventArgs)e;
            Exception _1 = (Exception)_0.ExceptionObject;
            String _2 = _1.Message;

            Check.That(_2).IsEqualTo("AlertExam");
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name =  "ToolExam"]/method[@Name = "Activate"]'/> */
        [Test]
        public void Activate()
        {
            Tool _0 = new Tool();
            
            Check.That(_0.Activate()).IsTrue();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "AddDataItems"]'/> */
        [Test]
        public void AddDataItems()
        {
            List<DataItem> _0    = new List<DataItem>();
            GuidReferenceItem _1 = new GuidReferenceItem();
            GenericItem _2       = new GenericItem();
            TextItem _3          = new TextItem();
            MSBuildItem _4       = new MSBuildItem();
            BinaryItem _5        = new BinaryItem();
            MultiMediaItem _6    = new MultiMediaItem();
            Tool _7              = new Tool();
            List<Guid?> _8       = new List<Guid?>();
            List<Guid?> _10      = new List<Guid?>();
            List<DataItem> _11   = new List<DataItem>();
            List<Guid?> _12      = new List<Guid?>();
            List<Guid?> _13      = new List<Guid?>();

            _0.Add(_1) ;_8.Add(_1.ID); _12.Add(_1.ID);
            _0.Add(_2) ;_8.Add(_2.ID); _12.Add(_2.ID);
            _0.Add(_3) ;_8.Add(_3.ID); _12.Add(_3.ID);
            _0.Add(_4) ;_8.Add(_4.ID); _12.Add(_4.ID);
            _0.Add(_5) ;_8.Add(_5.ID); _12.Add(_5.ID);
            _7.AddDataItems(_0);
            List<DataItem>? _9   = _7.Data;
            if(_9 is not null)
            {
                foreach(DataItem item in _9) { _10!.Add(item!.ID); }
            }
            _11.Add(_6); _12.Add(_6.ID);
            _7.AddDataItems(_11);
            List<DataItem>? _14   = _7.Data;
            if(_14 is not null)
            {
                foreach(DataItem item in _14) { _13!.Add(item!.ID); }
            }

            Check.That(_7.AddDataItems(null)).IsFalse();
            Check.That(_10).ContainsExactly(_8.ToArray());
            Check.That(_13).ContainsExactly(_12.ToArray());
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "AddNotes"]'/> */
        [Test]
        public void AddNotes()
        {
            Tool _0 = new Tool();
            List<String> _1 = new List<String>(){"AddNotesExam"};
            List<String> _2 = new List<String>(){"Pass"};
            List<String> _3 = new List<String>(); _3.AddRange(_1); _3.AddRange(_2);

            Check.That(_0.AddNotes(null)).IsFalse();
            Check.That(_0.Notes).IsEmpty();
            Check.That(_0!.AddNotes(_1)).IsTrue();
            Check.That(_0.Notes).ContainsExactly(_1);
            Check.That(_0!.AddNotes(_2)).IsTrue();
            Check.That(_0.Notes).ContainsExactly(_3);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "AddTags"]'/> */
        [Test]
        public void AddTags()
        {
            Tool _0 = new Tool();
            String _1 = "AddTagsExam";
            String _2 = "Pass";
            List<String> _3 = new List<String>(); _3.Add(_1);
            List<String> _4 = new List<String>(); _4.Add(_2);

            Check.That(_0.AddTags(null)).IsFalse();
            Check.That(_0.Tags).IsEmpty();
            Check.That(_0.AddTags(_3)).IsTrue();
            Check.That(_0.Tags).ContainsExactly(_1);
            Check.That(_0.AddTags(_4)).IsTrue();
            Check.That(_0.Tags).ContainsExactly(_1,_2);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "Calibrate"]'/> */
        [OneTimeSetUp]
        public void Calibrate() { }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "Command"]'/> */
        [Test]
        public void Command()
        {
            Tool _0 = new Tool();
            Object[] _1 = new Object[4]{Int16.MinValue,String.Empty,Single.Pi,Double.Epsilon};

            Check.That(_0.Command(_1)).IsTrue();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "CommandTasks"]'/> */
        [Test]
        public void CommandTasks()
        {
            Tool _0 = new Tool();
            Task<Object?> _1 = new Task<Object?>( () => { return new String(String.Empty);} );
            List<Task<Object?>> _2 = new List<Task<Object?>>() {_1};

            Check.That(_0.Command(_2)).IsTrue();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "Constructor"]'/> */
        [Test]
        public void Constructor()
        {
            List<DataItem> _0    = new List<DataItem>();
            GuidReferenceItem _1 = new GuidReferenceItem();
            GenericItem _2       = new GenericItem();
            TextItem _3          = new TextItem();
            MSBuildItem _4       = new MSBuildItem();
            BinaryItem _5        = new BinaryItem();
            MultiMediaItem _6    = new MultiMediaItem();
            List<Guid?> _8       = new List<Guid?>();
            List<Guid?> _10      = new List<Guid?>();
            _0.Add(_1); _8.Add(_1.ID);
            _0.Add(_2); _8.Add(_2.ID);
            _0.Add(_3); _8.Add(_3.ID);
            _0.Add(_4); _8.Add(_4.ID);
            _0.Add(_5); _8.Add(_5.ID);
            _0.Add(_6); _8.Add(_6.ID);
            String _11 = "ToolConstructorExamInputs";
            Queue<Object> _22 = new Queue<Object>();
            _22.Enqueue(_11);
            Guid _15 = new Guid();
            String _17 = "My Tool";
            String _18 = "ToolConstructorExamNotes";
            String _19 = "ToolConstructorExamObjectives";
            String _21 = "ToolConstructorExamPolicies";;
            List<String> _24 = new List<String>(){_18};
            List<Object> _25 = new List<Object>(){_19};
            List<Object> _26 = new List<Object>(){_21};
            String _28 = "ToolConstructorExamPurpose";
            String _32 = "ToolConstructorExamTags";
            List<String> _38 = new List<String>(){_32};
            Tool _99 = new Tool(_0,_15,_22,_17,_24,_25,_26,_28,_38);
            List<DataItem>? _9   = _99.Data;
            if(_9 is not null)
            {
                foreach(DataItem item in _9) { _10!.Add(item!.ID); }
            }

            Check.That(_99).IsInstanceOfType(typeof(Tool));
            Check.That(_10).ContainsExactly(_8.ToArray());
            Check.That(_99.ID).IsEqualTo(_15);
            Check.That(_99.Inputs!.Dequeue()).IsEqualTo(_11);
            Check.That(_99.Name).IsEqualTo(_17);
            Check.That(_99.Notes).ContainsExactly(_18);
            Check.That(_99.Objectives).ContainsExactly(_19);
            Check.That(_99.Policies).ContainsExactly(_21);
            Check.That(_99.Purpose).IsEqualTo(_28);
            Check.That(_99.Tags).ContainsExactly(_32);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "Deactivate"]'/> */
        [Test]
        public void Deactivate()
        {
            Tool _0 = new Tool();

            Check.That(_0.Deactivate()).IsTrue();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetActivities"]'/> */
        [Test]
        public void GetActivities()
        {
            Tool _0 = new Tool();
            Task<Object?> _1 = new Task<Object?>( () => { return new String(String.Empty); });
            List<Task<Object?>> _2 = new List<Task<Object?>>() {_1};
            _0.Activities = _2;

            Check.That(_0.GetActivities()).IsEqualTo(_2);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetAppDomainID"]'/> */
        [Test]
        public void GetAppDomainID()
        {
            Tool _0 = new Tool();

            Check.That(_0.GetAppDomainID()).HasAValue();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetAppDomainUID"]'/> */
        [Test]
        public void GetAppDomainUID() 
        {
            Tool _0 = new Tool();
            _0.AppDomainUID = Int128.MaxValue;

            Check.That(_0.GetAppDomainUID()).IsEqualTo(Int128.MaxValue);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetAssemblyVersion"]'/> */
        [Test]
        public void GetAssemblyVersion() 
        {
            Tool _0 = new Tool();

            Check.That(_0.GetAssemblyVersion()).IsNotNull();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetBornOn"]'/> */
        [Test]
        public void GetBornOn()
        {
            Tool _0 = new Tool();

            Check.That(_0.GetBornOn()).HasAValue();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetControls"]'/> */
        [Test]
        public void GetControls()
        {
            Tool _0 = new Tool();
            IPEndPoint _1 = new IPEndPoint(IPAddress.Loopback,443);
            _0.Controls = new List<IPEndPoint>(){_1};

            Check.That(_0.GetControls()).Contains(_1);
            Check.That(_0.GetControls()!.ToArray().First().Address).IsEqualTo(IPAddress.Loopback);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetCPUID"]'/> */
        [Test]
        public void GetCPUID()
        {
            Tool _0 = new Tool();
            _0.CPUID = Int64.MaxValue;

            Check.That(_0.GetCPUID()).IsEqualTo(Int64.MaxValue);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetDataItems"]'/> */
        [Test]
        public void GetDataItems()
        {
            List<DataItem>? _0   = new List<DataItem>();
            GuidReferenceItem _1 = new GuidReferenceItem();
            GenericItem _2       = new GenericItem();
            TextItem _3          = new TextItem();
            MSBuildItem _4       = new MSBuildItem();
            BinaryItem _5        = new BinaryItem();
            MultiMediaItem _6    = new MultiMediaItem();
            Tool _7              = new Tool();
            List<Guid?> _8       = new List<Guid?>();
            List<Guid?> _10      = new List<Guid?>();
            _0.Add(_1) ;_8.Add(_1.ID);
            _0.Add(_2) ;_8.Add(_2.ID);
            _0.Add(_3) ;_8.Add(_3.ID);
            _0.Add(_4) ;_8.Add(_4.ID);
            _0.Add(_5) ;_8.Add(_5.ID);
            _0.Add(_6) ;_8.Add(_6.ID);
            _7.Data = _0;
            List<DataItem>? _9   = _7.GetDataItems();
            if(_9 is not null)
            {
                foreach(DataItem item in _9) { _10!.Add(item!.ID); }
            }

            Check.That(_10).ContainsExactly(_8.ToArray());
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetDomainID"]'/> */
        [Test]
        public void GetDomainID()
        {
            Tool _0 = new Tool();
            String _1 = "GetDomainIDExam";
            _0.DomainID = _1;

            Check.That(_0.GetDomainID()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetEventLogs"]'/> */
        [Test]
        public void GetEventLogs()
        {
            Tool _0 = new Tool();
            String _1 = "GetEventLogsExam";
            LinkedList<String> _2 = new LinkedList<String>();
            _2.AddFirst(_1);

            _0.EventLogs = _2;

            Check.That(_0.GetEventLogs()).Contains(_2);
            Check.That(_0.GetEventLogs()!.ToArray().First()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetExtension"]'/> */
        [Test]
        public void GetExtension()
        {
            Tool _0 = new Tool();
            dynamic _1 = new ExpandoObject();
            String _2 = "Pass";
            _1.GetExtensionExam = _2;
            _0.Extension = _1;

            Check.That(((Object?)(_0.GetExtension()!.GetExtensionExam))).IsEqualTo(_2);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetGPS"]'/> */
        [Test]
        public void GetGPS()
        {
            Tool _0 = new Tool();
            String _1 = "GetGPSExam";
            _0.GPS = _1;

            Check.That(_0.GetGPS()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetHash"]'/> */
        [Test]
        public void GetHash()
        {
            Tool _0 = new Tool();
            Tool _1 = new Tool();

            Check.That(_0.GetHashCode()).IsNotZero();
            Check.That(_0.GetHashCode()).IsNotEqualTo(_1.GetHashCode());
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetID"]'/> */
        [Test]
        public void GetID()
        {
            Tool _0 = new Tool();

            Check.That(_0.GetID()).HasAValue();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetInputs"]'/> */
        [Test]
        public void GetInputs()
        {            
            Tool _0 = new Tool();
            String _1 = "GetInputsExam";
            Queue<Object> _2 = new Queue<Object>();
            _2.Enqueue(_1);
            _0.Inputs = _2;

            Check.That(_0.GetInputs()).Contains(_2);
            Check.That(_0.GetInputs()!.Dequeue()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetMachineID"]'/> */
        [Test]
        public void GetMachineID()
        {
            Tool _0 = new Tool();

            Check.That(_0.GetMachineID()).HasContent();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetModified"]'/> */
        [Test]
        public void GetModified()
        {
            Tool _0 = new Tool();
            Tool _3 = new Tool();
            DateTime _1 = DateTime.Today;
            _3.Modified = _1;

            Check.That(_0.GetModified()).IsNull();
            Check.That(_3.GetModified()).Equals(DateTime.Today);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetName"]'/> */
        [Test]
        public void GetName()
        {
            Tool _0 = new Tool();
            String _1 = "GetNameExam";
            _0.Name = _1;

            Check.That(_0.GetName()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetNotes"]'/> */
        [Test]
        public void GetNotes()
        {
            Tool _0 = new Tool();
            List<String> _1 = new List<String>(){"GetNotesExam","Pass"};
            _0!.Notes!.Add(_1.First()); _0.Notes.Add(_1.Last());

            Check.That(_0.GetNotes()).ContainsExactly(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetObjectives"]'/> */
        [Test]
        public void GetObjectives()
        {
            Tool _0 = new Tool();
            String _1 = "GetObjectivesExam";
            List<Object> _2 = new List<Object>();
            _2.Add(_1);
            _0.Objectives = _2;

            Check.That(_0.GetObjectives()).Contains(_2);
            Check.That(_0.GetObjectives()!.ToArray().First()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetOutputs"]'/> */
        [Test]
        public void GetOutputs()
        {
            Tool _0 = new Tool();
            String _1 = "GetOutputsExam";
            List<Object> _2 = new List<Object>();
            _2.Add(_1);
            _0.Outputs = _2;

            Check.That(_0.GetOutputs()).Contains(_2);
            Check.That(_0.GetOutputs()!.ToArray().First()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetPolicies"]'/> */
        [Test]
        public void GetPolicies()
        {
            Tool _0 = new Tool();
            String _1 = "GetPoliciesExam";
            List<Object> _2 = new List<Object>();
            _2.Add(_1);
            _0.Policies = _2;

            Check.That(_0.GetPolicies()).Contains(_2);
            Check.That(_0.GetPolicies()!.ToArray().First()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetProcessID"]'/> */
        [Test]
        public void GetProcessID()
        {
            Tool _0 = new Tool();

            Check.That(_0.GetProcessID()).HasAValue();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetSecurityDescriptor"]'/> */
        [Test]
        public void GetSecurityDescriptor()
        {
            Tool _0 = new Tool();
            String _1 = "GetSecurityDescriptorExam";
            _0.SecurityDescriptor = _1;

            Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetStatus"]'/> */
        [Test]
        public void GetStatus()
        {
            Tool _0 = new Tool();
            String _1 = "GetStatusExam";
            Stack<Object> _2 = new Stack<Object>();
            _2.Push(_1);
            _0.Status = _2;

            Check.That(_0.GetStatus()).Contains(_2);
            Check.That(_0.GetStatus()!.Pop()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetSubordinates"]'/> */
        [Test]
        public void GetSubordinates()
        {
            Tool _0 = new Tool();
            Tool _1 = new Tool();
            Tool _2 = new Tool();
            List<Tool> _3 = new List<Tool>();
            _0.Subordinates = _3;

            Check.That(_0.GetSubordinates()).IsEmpty();

            _3.Add(_1);

            Check.That(_0.GetSubordinates()).ContainsExactly(_1);

            _3.Add(_2);

            Check.That(_0.GetSubordinates()).ContainsExactly(_1,_2);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetSuperior"]'/> */
        [Test]
        public void GetSuperior()
        {
            Tool _0 = new Tool();
            Tool _1 = new Tool();
            _0.Superior = _1;

            Check.That(_0.GetSuperior()).IsSameReferenceAs(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetTags"]'/> */
        [Test]
        public void GetTags()
        {
            Tool _0 = new Tool();
            String _1 = "GetTagsExam";
            _0!.Tags!.Add(_1);

            Check.That(_0.GetTags()).Contains(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetTelemetry"]'/> */
        [Test]
        public void GetTelemetry()
        {
            Tool _0 = new Tool();
            IPEndPoint _1 = new IPEndPoint(IPAddress.Loopback,443);
            _0.Telemetry = new List<IPEndPoint>(){_1};

            Check.That(_0.GetTelemetry()).Contains(_1);
            Check.That(_0.GetTelemetry()!.ToArray().First().Address).IsEqualTo(IPAddress.Loopback);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "GetURI"]'/> */
        [Test]
        public void GetURI()
        {
            Tool _0 = new Tool();
            String _1 = "GetURIExam";
            _0.URI = _1;

            Check.That(_0.GetURI()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "Initialize"]'/> */
        [Test]
        public void Initialize()
        {
            Guid _0 = new Guid();
            Tool _1 = new Tool();

            _1.Initialize(_0);

            Check.That(_1.AppDomainID).IsNotNull();
            Check.That(_1.AssemblyVersion).IsNotNull();
            Check.That(_1.BornOn).IsNotNull();
            Check.That(_1.ID).IsEqualTo(_0);
            Check.That(_1.MachineID).IsNotNull();
            Check.That(_1.ProcessID).IsNotNull();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "InitializeTool"]'/> */
        [Test]
        public void InitializeTool()
        {            
            Tool _0 = new Tool();
            
            Check.That(_0.Data).IsNotNull();
            Check.That(_0.Notes).IsNotNull();
            Check.That(_0.Tags).IsNotNull();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "ParameterlessConstructor"]'/> */
        [Test]
        public void ParameterlessConstructor()
        {        
            Tool _0 = new Tool();

            Check.That(_0.Activities).IsNull();
            Check.That(_0.AppDomainID).IsNotNull();
            Check.That(_0.AppDomainUID).IsNull();
            Check.That(_0.AssemblyVersion).IsNotNull();
            Check.That(_0.BornOn).IsNotNull();
            Check.That(_0.Controls).IsNull();
            Check.That(_0.CPUID).IsNull();
            Check.That(_0.Data).IsNotNull();
            Check.That(_0.DomainID).IsNull();
            Check.That(_0.EventLogs).IsNull();
            Check.That(((Object?)(_0.Extension))).IsNull();
            Check.That(_0.GPS).IsNull();
            Check.That(_0.ID).IsNotNull();
            Check.That(_0.Inputs).IsNull();
            Check.That(_0.MachineID).IsNotNull();
            Check.That(_0.Modified).IsNull();
            Check.That(_0.Name).IsNull();
            Check.That(_0.Notes).IsNotNull();
            Check.That(_0.Objectives).IsNull();
            Check.That(_0.Outputs).IsNull();
            Check.That(_0.Policies).IsNull();
            Check.That(_0.Purpose).IsNull();
            Check.That(_0.ProcessID).IsNotNull();
            Check.That(_0.Status).IsNull();
            Check.That(_0.Subordinates).IsNull();
            Check.That(_0.Superior).IsNull();
            Check.That(_0.Tags).IsNotNull();
            Check.That(_0.Telemetry).IsNull();
            Check.That(_0.URI).IsNull();
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "SetExtension"]'/> */
        [Test]
        public void SetExtension()
        {
            Tool _0 = new Tool();
            dynamic _1 = new ExpandoObject();
            String _2 = "Pass";
            _1.SetExtensionExam = _2;
            _0.SetExtension(_1);

            Check.That(((Object?)(_0.Extension!.SetExtensionExam))).IsEqualTo(_2);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "SetModified"]'/> */
        [Test]
        public void SetModified()
        {
            Tool _0 = new Tool();
            Tool _3 = new Tool();
            DateTime _1 = DateTime.Today;

            Check.That(_0.SetModified(null)).IsFalse();
            Check.That(_3.SetModified(_1)).IsTrue();
            Check.That(_3.Modified).Equals(DateTime.Today);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "SetName"]'/> */
        [Test]
        public void SetName()
        {
            Tool _0 = new Tool();
            String _1 = "SetNameExam";
            
            Check.That(_0.SetName(null)).IsFalse();
            Check.That(_0.SetName(_1)).IsTrue();
            Check.That(_0.Name).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "SetObjectives"]'/> */
        [Test]
        public void SetObjectives()
        {
            Tool _0 = new Tool();
            String _1 = "SetObjectivesExam";
            List<Object> _2 = new List<Object>();
            _2.Add(_1);

            Check.That(_0.SetObjectives(null)).IsFalse();
            Check.That(_0.SetObjectives(_2)).IsTrue();
            Check.That(_0.Objectives).Contains(_2);
            Check.That(_0.Objectives!.ToArray().First()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "SetPolicies"]'/> */
        [Test]
        public void SetPolicies()
        {
            Tool _0 = new Tool();
            String _1 = "SetPoliciesExam";
            List<Object> _2 = new List<Object>();
            _2.Add(_1);

            Check.That(_0.SetPolicies(null)).IsFalse();
            Check.That(_0.SetPolicies(_2)).IsTrue();
            Check.That(_0.Policies).Contains(_2);
            Check.That(_0.Policies!.ToArray().First()).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "SetSecurityDescriptor"]'/> */
        [Test]
        public void SetSecurityDescriptor()
        {
            Tool _0 = new Tool();
            String _1 = "SetSecurityDescriptorExam";

            Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
            Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
            Check.That(_0.SecurityDescriptor).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "SetSubordinates"]'/> */
        [Test]
        public void SetSubordinates()
        {
            Tool _0 = new Tool();
            Tool _1 = new Tool();
            Tool _2 = new Tool();
            List<Tool> _3 = new List<Tool>();
            _3.Add(_1); _3.Add(_2);
            
            Check.That(_0.SetSubordinates(null)).IsFalse();
            Check.That(_0.SetSubordinates(_3)).IsTrue();
            Check.That(_0.Subordinates).ContainsExactly(_1,_2);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "SetSuperior"]'/> */
        [Test]
        public void SetSuperior()
        {
            Tool _0 = new Tool();
            Tool _1 = new Tool();

            Check.That(_0.SetSuperior(null)).IsFalse();
            Check.That(_0.SetSuperior(_1)).IsTrue();
            Check.That(_0.Superior).IsSameReferenceAs(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "SetURI"]'/> */
        [Test]
        public void SetURI()
        {
            Tool _0 = new Tool();
            String _1 = "SetURIExam";

            Check.That(_0.SetURI(null)).IsFalse();
            Check.That(_0.SetURI(_1)).IsTrue();
            Check.That(_0.URI).IsEqualTo(_1);
        }

        /** <include file = 'ToolExam.xml' path = 'ToolExam/class[@Name = "ToolExam"]/method[@Name = "UpdateInputs"]'/> */
        [Test]
        public void UpdateInputs()
        {
            Tool _0 = new Tool();
            String _1 = "UpdateInputsExam";
            String _3 = "Pass";
            Queue<Object> _2 = new Queue<Object>();
            Queue<Object> _4 = new Queue<Object>();

            _2.Enqueue(_1);
            _0.Inputs = _2;

            Check.That(_0.UpdateInputs(null)).IsFalse();
            Check.That(_0.Inputs).Contains(_2);
            Check.That(_0.Inputs!.Peek()).IsEqualTo(_1);

            _4.Enqueue(_1); _4.Enqueue(_3);

            Check.That(_0.UpdateInputs(_4)).IsTrue();
            Check.That(_0.Inputs.Dequeue()).IsEqualTo(_1);
            Check.That(_0.Inputs.Dequeue()).IsEqualTo(_3);
        }

    }

}