namespace KusDepot.ToolServiceExam;

[TestFixture] [SingleThreaded]
public class ToolServiceExam
{
    private static CustomBinding Binding = new CustomBinding(new BindingElement[]{
                    new BinaryMessageEncodingBindingElement(){CompressionFormat = CompressionFormat.GZip,
                    MessageVersion = MessageVersion.Soap12WSAddressing10 , ReaderQuotas = XmlDictionaryReaderQuotas.Max},
                    new HttpTransportBindingElement(){MaxBufferPoolSize = Int32.MaxValue , MaxReceivedMessageSize = Int32.MaxValue}})
                    {Name = "ToolService" , Namespace = "KusDepot"};

    private static String URL = "http://localhost/ToolService";

    private static ToolClient _0 = new ToolClient(Binding,new EndpointAddress(URL));

    [Test] [Order(1)]
    public void Activate()
    {
        Check.That(_0.Activate()).IsTrue();
    }

    [Test] [Order(2)]
    public void AddDataItems()
    {
        HashSet<DataItem> _0  = new HashSet<DataItem>();
        GuidReferenceItem _1  = new GuidReferenceItem(); _1.ID = Guid.NewGuid();
        GuidReferenceItem _21  = new GuidReferenceItem(); _21.ID = Guid.NewGuid();
        GenericItem _2        = new GenericItem();       _2.ID = Guid.NewGuid();
        TextItem _3           = new TextItem();          _3.ID = Guid.NewGuid();
        MSBuildItem _4        = new MSBuildItem();       _4.ID = Guid.NewGuid();
        BinaryItem _5         = new BinaryItem();        _5.ID = Guid.NewGuid();
        Byte[] _15            = new Byte[1948576]; RandomNumberGenerator.Create().GetBytes(_15); _5.Content = _15;
        _5.SecureHashes       = new Dictionary<String,Byte[]>();
        _5.SecureHashes.Add("Content",SHA512.HashData(_15));
        CodeItem _16          = new CodeItem();          _16.ID = Guid.NewGuid();
        MultiMediaItem _6     = new MultiMediaItem();    _6.ID = Guid.NewGuid();

        ToolClient _7         = ToolServiceExam._0;
        List<Guid?> _8        = new List<Guid?>();
        List<Guid?> _10       = new List<Guid?>();
        HashSet<DataItem> _11 = new HashSet<DataItem>();
        List<Guid?> _12       = new List<Guid?>();
        List<Guid?> _13       = new List<Guid?>();

        _0.Add(_1); _8.Add(_1.ID); _12.Add(_1.ID);
        _0.Add(_2); _8.Add(_2.ID); _12.Add(_2.ID);
        _0.Add(_3); _8.Add(_3.ID); _12.Add(_3.ID);
        _0.Add(_4); _8.Add(_4.ID); _12.Add(_4.ID);
        _0.Add(_5); _8.Add(_5.ID); _12.Add(_5.ID);
        _0.Add(_16); _8.Add(_16.ID); _12.Add(_16.ID);
        _0.Add(_21); _8.Add(_21.ID); _12.Add(_21.ID);

        Check.That(_7.AddDataItems(_0.ToList())).IsTrue();
        HashSet<DataItem>? _9   = _7.GetDataItems().ToHashSet();
        if( _9 is not null )
        {
            foreach(DataItem item in _9)
            {
                _10!.Add(item!.ID);
            }
        }
        _11.Add(_6);
        _12.Add(_6.ID);
        Check.That(_7.AddDataItems(_11.ToList())).IsTrue();
        HashSet<DataItem>? _14   = _7.GetDataItems().ToHashSet();
        if( _14 is not null )
        {
            foreach(DataItem item in _14)
            {
                _13!.Add(item!.ID);
            }
        }

        Check.That(_7.AddDataItems(null)).IsFalse();
        Check.That(_10).ContainsExactly(_8.ToArray());
        Check.That(_13).ContainsExactly(_12.ToArray());
    }

    [Test] [Order(3)]
    public void AddInput()
    {
        String _1 = "AddInputExam";
        String _2 = "Pass";

        Check.That(_0.GetInputs()).IsNull();
        Check.That(_0.AddInput(_1)).IsTrue();
        Check.That(_0.GetInputs()?.Count).Equals(1);
        Check.That(_0.AddInput(_2)).IsTrue();
        Check.That(_0.GetInputs()?.Count).Equals(2);
        Check.That(_0.GetInput()).Equals(_1);
        Check.That(_0.GetInputs()?.Count).Equals(1);
        Check.That(_0.GetInput()).Equals(_2);
        Check.That(_0.GetInputs()).IsNull();
    }

    [Test] [Order(4)]
    public void AddNotes()
    {
        List<String> _1 = new List<String>(){"AddNotesExam"};
        List<String> _2 = new List<String>(){"Pass"};
        List<String> _3 = new List<String>();
        _3.AddRange(_1);
        _3.AddRange(_2);

        Check.That(_0.AddNotes(null)).IsFalse();
        Check.That(_0.GetNotes()).IsNull();
        Check.That(_0.AddNotes(_1)).IsTrue();
        Check.That(_0.GetNotes()).ContainsExactly(_1);
        Check.That(_0.AddNotes(_2)).IsTrue();
        Check.That(_0.GetNotes()).ContainsExactly(_3);
    }

    [Test] [Order(5)]
    public void AddOutput()
    {
        String _1 = "AddOutputExam"; Guid _11 = Guid.NewGuid();
        String _2 = "Pass";          Guid _12 = Guid.NewGuid();

        Check.That(_0.GetOutputIDs()).IsNull();
        Check.That(_0.AddOutput(_11,_1)).IsTrue();
        Check.That(_0.GetOutputIDs()).Contains(_11);
        Check.That(_0.AddOutput(_12,_2)).IsTrue();
        Check.That(_0.GetOutputIDs()).Contains(_11,_12);
        Check.That(_0.GetOutput(_12)).Equals(_2);
        Check.That(_0.AddOutput(Guid.NewGuid(),null)).IsTrue();
    }

    [Test] [Order(6)]
    public void AddTags()
    {
        String _1 = "AddTagsExam";
        String _2 = "Pass";
        List<String> _3 = new List<String>();
        _3.Add(_1);
        List<String> _4 = new List<String>();
        _4.Add(_2);

        Check.That(_0.AddTags(null)).IsFalse();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.AddTags(_3)).IsTrue();
        Check.That(_0.GetTags()).ContainsExactly(_1);
        Check.That(_0.AddTags(_4)).IsTrue();
        Check.That(_0.GetTags()).ContainsExactly(_1 , _2);
    }

    [OneTimeSetUp]
    public void Calibrate()
    {
        Process.Start(new ProcessStartInfo(){ CreateNoWindow = false , FileName = Environment.GetEnvironmentVariable("KusDepotSolution") + @"\KusDepot.ToolService\bin\x64\Debug\net7.0\ToolService.exe", UseShellExecute = true, WindowStyle = ProcessWindowStyle.Maximized});
        Thread.Sleep(4000);
    }

    [Test] [Order(7)]
    public void ClearEventLogs()
    {
        String _1 = "ClearEventLogs";
        String _2 = "Pass";

        Check.That(_0.LogEvent(_1)).IsTrue();
        Check.That(_0.LogEvent(_2)).IsTrue();
        Check.That(_0.GetEventLogs()!.Keys.Count).Equals(2);
        Check.That(_0.GetEvent(0)).Equals(_1);
        Check.That(_0.GetEvent(1)).Equals(_2);
        Check.That(_0.ClearEventLogs()).IsTrue();
        Check.That(_0.GetEventLogs()).IsNull();
    }

    [OneTimeTearDown]
    public void Complete() { }

    [Test] [Order(8)]
    public void Deactivate()
    {
        Check.That(_0.Deactivate()).IsTrue();
    }

    [Test] [Order(9)]
    public void ExecuteCommand()
    {
        Object[] _1 = new Object[4]{"handle",String.Empty,Single.Pi,Double.Epsilon};

        Check.That(_0.ExecuteCommand(_1.ToList())).IsNull();
    }

    [Test] [Order(10)]
    public void GetAppDomainID()
    {
        Check.That(_0.GetAppDomainID()).HasAValue();
    }

    [Test] [Order(11)]
    public void GetAppDomainUID()
    {
        Check.That(_0.GetAppDomainUID()).IsNull();
    }

    [Test] [Order(12)]
    public void GetApplication()
    {
        Check.That(_0.GetApplication()).IsNull();
    }

    [Test] [Order(13)]
    public void GetApplicationVersion()
    {
        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test] [Order(14)]
    public void GetAssemblyVersion()
    {
        Version? _1 = Assembly.GetExecutingAssembly().GetName().Version;

        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
    }

    [Test] [Order(15)]
    public void GetBornOn()
    {
        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test] [Order(16)]
    public void GetCertificates()
    {
        Dictionary<String,String> _1 = new Dictionary<String,String>();
        _1.Add("Content 0","6131f9c300030000000f");
        _1.Add("Content 1","611e23c200030000000e");
        _1.Add("Extension ExtKey InnerKey1 DeeperKeyN 8","473d29a500030000000f");

        Check.That(_0.SetCertificates(_1)).IsTrue();
        Check.That(_0.GetCertificates()).ContainsExactly(_1);
        Check.That(_0.SetCertificates(new Dictionary<String,String>())).IsTrue();
        Check.That(_0.GetCertificates()).IsNull();
    }

    [Test] [Order(17)]
    public void GetControls()
    {
        Check.That(_0.GetControls()).IsNull();;
    }

    [Test] [Order(18)]
    public void GetCPUID()
    {
        Check.That(_0.GetCPUID()).IsNull();
    }

    [Test] [Order(19)]
    public void GetDataItems()
    {
        Check.That(_0.GetDataItems()).HasSize(8);

        foreach(DataItem i in _0.GetDataItems())
        {
            if(((Object)i).GetType() == typeof(BinaryItem))
            {
                BinaryItem d = (BinaryItem)i;

                Check.That(d.Content.Length).IsEqualTo(1948576);
                Check.That(d.SecureHashes["Content"]).IsEqualTo(SHA512.HashData(d.Content));
            }
        }
    }

    [Test] [Order(20)]
    public void GetDistinguishedName()
    {
        String _1 = "CN=ServiceN,DC=Journey International,DC=ORG";

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test] [Order(21)]
    public void GetDomainID()
    {
        Check.That(_0.GetDomainID()).IsNull();
    }

    [Test] [Order(22)]
    public void GetEvent()
    {
        String _1 = "GetEventExam";
        String _2 = "Pass";

        Check.That(_0.LogEvent(_1)).IsTrue();
        Check.That(_0.LogEvent(_2)).IsTrue();
        Check.That(_0.GetEventLogs()!.Keys.Count).Equals(2);
        Check.That(_0.GetEvent(0)).Equals(_1);
        Check.That(_0.GetEvent(1)).Equals(_2);
        Check.That(_0.ClearEventLogs()).IsTrue();
    }

    [Test] [Order(23)]
    public void GetEventLogs()
    {
        Check.That(_0.GetEventLogs()).IsNull();
    }

    [Test] [Order(24)]
    public void GetExtension()
    {
        String _1 = "GetExtensionExam";

        Dictionary<String,Object?> _2 = new Dictionary<String,Object?>();

        _2.Add("1",_1);
            
        Check.That(_0.SetExtension(_2)).IsTrue();

        Check.That((String?)((Dictionary<String,Object?>)_0.GetExtension())["1"]).IsEqualTo(_1);
    }

    [Test] [Order(25)]
    public void GetFILE()
    {
        Check.That(_0.GetFILE()).IsNull();
    }

    [Test] [Order(26)]
    public void GetGPS()
    {
        Check.That(_0.GetGPS()).IsNull();
    }

    [Test] [Order(27)]
    public void GetID()
    {
        Check.That(_0.GetID()).HasAValue();
    }

    [Test] [Order(28)]
    public void GetInput()
    {
        String _1 = "GetInputExam";
        String _2 = "Pass";

        Check.That(_0.GetInputs()).IsNull();
        Check.That(_0.AddInput(_1)).IsTrue();
        Check.That(_0.GetInputs()?.Count).Equals(1);
        Check.That(_0.AddInput(_2)).IsTrue();
        Check.That(_0.GetInputs()?.Count).Equals(2);
        Check.That(_0.GetInput()).Equals(_1);
        Check.That(_0.GetInputs()?.Count).Equals(1);
        Check.That(_0.GetInput()).Equals(_2);
        Check.That(_0.GetInputs()).IsNull();
    }

    [Test] [Order(29)]
    public void GetInputs()
    {
        Check.That(_0.GetInputs()).IsNull();
    }

    [Test] [Order(30)]
    public void GetLinks()
    {
        Dictionary<String,GuidReferenceItem> _1 = new Dictionary<String,GuidReferenceItem>();
        GuidReferenceItem _2 = new GuidReferenceItem();
        GuidReferenceItem _3 = new GuidReferenceItem();
        GuidReferenceItem _4 = new GuidReferenceItem();
        _1.Add("_2",_2); _1.Add("_3",_3); _1.Add("_4",_4);

        Check.That(_0.SetLinks(null)).IsFalse();
        Check.That(_0.SetLinks(_1)).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetLinks()).HasSize(3);
        Check.That(_0.GetLinks()?["_2"].ID).IsEqualTo(_2.ID);
        Check.That(_0.GetLinks()?["_3"].ID).IsEqualTo(_3.ID);
        Check.That(_0.GetLinks()?["_4"].ID).IsEqualTo(_4.ID);
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsFalse();
        Check.That(_0.UnLock("key")).IsTrue();
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsTrue();
        Check.That(_0.GetLinks()).IsNull();
    }

    [Test] [Order(31)]
    public void GetLocator()
    {
        Check.That(_0.GetLocator()).IsNull();
    }

    [Test] [Order(32)]
    public void GetMachineID()
    {
        Check.That(_0.GetMachineID()).HasContent();
    }

    [Test] [Order(33)]
    public void GetModified()
    {
        Check.That(_0.GetModified()).IsNull();
    }

    [Test] [Order(34)]
    public void GetName()
    {
        Check.That(_0.GetName()).IsNull();
    }

    [Test] [Order(35)]
    public void GetNotes()
    {
        Check.That(_0.GetNotes()).HasSize(2);
    }

    [Test] [Order(36)]
    public void GetObjectives()
    {
        String _1 = "GetObjectivesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(_0.SetObjectives(_2)).IsTrue();
        Check.That(_0.GetObjectives()).Contains(_2);
        Check.That(_0.GetObjectives()!.ToArray().First()).IsEqualTo(_1);
    }

    [Test] [Order(37)]
    public void GetOutput()
    {
        Check.That(_0.GetOutputIDs().Any(_=>_0.GetOutput(_).Equals("AddOutputExam"))).IsTrue();
    }

    [Test] [Order(38)]
    public void GetOutputIDs()
    {
        Check.That(_0.GetOutputIDs().Count).Equals(3);
    }

    [Test] [Order(39)]
    public void GetPolicies()
    {
        String _1 = "GetPoliciesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(_0.SetPolicies(_2)).IsTrue();
        Check.That(_0.GetPolicies()).Contains(_2);
        Check.That(_0.GetPolicies()!.ToArray().First()).IsEqualTo(_1);
    }

    [Test] [Order(40)]
    public void GetProcessID()
    {
        Check.That(_0.GetProcessID()).HasAValue();
    }

    [Test] [Order(41)]
    public void GetPurpose()
    {
        Check.That(_0.GetPurpose()).IsNull();
    }

    [Test] [Order(42)]
    public void GetSecurityDescriptor()
    {
        String _1 = "GetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test] [Order(43)]
    public void GetServiceVersion()
    {
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test] [Order(44)]
    public void GetStatus()
    {
        Check.That(_0.GetStatus()).IsNull();
    }

    [Test] [Order(45)]
    public void GetSubordinates()
    {
        HashSet<Tool> _1 = new HashSet<Tool>();

        _1.Add(new Tool()); _1.Add(new Tool()); _1.Add(new Tool());

        Check.That(_0.SetSubordinates(_1.ToList())).IsTrue();

        Check.That(_0.GetSubordinates().Count).IsEqualTo(3);

        Tool _2 = new Tool(); _2.ID = Guid.NewGuid();
        Tool _3 = new Tool(); _3.ID = Guid.NewGuid();

        _1.Add(_2); _1.Add(_3);

        Check.That(_0.SetSubordinates(_1.ToList())).IsTrue();

        Check.That(_0.GetSubordinates().Count).IsEqualTo(5);
    }

    [Test] [Order(46)]
    public void GetSuperior()
    {
        Tool _1 = new Tool();
        Check.That(_0.SetSuperior(_1)).IsTrue();

        Check.That(_0.GetSuperior()).IsInstanceOfType(typeof(Tool));
    }

    [Test] [Order(47)]
    public void GetTags()
    {
        Check.That(_0.GetTags().Count).IsEqualTo(2);
    }

    [Test] [Order(48)]
    public void GetTelemetry()
    {
        Check.That(_0.GetTelemetry()).IsNull();
    }

    [Test] [Order(49)]
    public void GetThreadID()
    {
        Check.That(_0.GetThreadID()).HasAValue();
    }

    [Test] [Order(50)]
    public void GetVersion()
    {
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test] [Order(51)]
    public void Initialize()
    {
        Check.That(_0.Initialize()).IsTrue();
        Check.That(_0.GetAppDomainID()).HasAValue();
        Check.That(_0.GetAssemblyVersion()).IsNotNull();
        Check.That(_0.GetBornOn()).HasAValue();
        Check.That(_0.GetID()).HasAValue();
        Check.That(_0.GetMachineID()).HasContent();
        Check.That(_0.GetProcessID()).HasAValue();
        Check.That(_0.GetThreadID()).HasAValue();
    }

    [Test] [Order(52)]
    public void Lock()
    {
        String _1 = "LockExam";

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(_1)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor("null")).IsEqualTo(false);

        Check.That(_0.UnLock(_1)).IsTrue();
    }

    [Test] [Order(53)]
    public void LogEvent()
    {
        String _1 = "LogEventExam";
        String _2 = "Pass";

        Check.That(_0.LogEvent(_1)).IsTrue();
        Check.That(_0.LogEvent(_2)).IsTrue();
        Check.That(_0.GetEventLogs()!.Keys.Count).Equals(2);
        Check.That(_0.GetEvent(0)).Equals(_1);
        Check.That(_0.GetEvent(1)).Equals(_2);
        Check.That(_0.ClearEventLogs()).IsTrue();
    }

    [Test] [Order(54)]
    public void RemoveDataItem()
    {
        Check.That(_0.GetDataItems()).HasSize(8);
        Check.That(_0.RemoveDataItem(null)).IsFalse();
        Check.That(_0.RemoveDataItem(_0.GetDataItems().First().ID)).IsTrue();
        Check.That(_0.GetDataItems()).HasSize(7);

        foreach(DataItem _8 in _0.GetDataItems())
        {
            Check.That(_0.RemoveDataItem(_8.ID)).IsTrue();
        }
        Check.That(_0.GetDataItems()).IsNull();
    }

    [Test] [Order(55)]
    public void RemoveNote()
    {
        Check.That(_0.RemoveNote(null)).IsFalse();
        Check.That(_0.GetNotes().Count).IsEqualTo(2);
        Check.That(_0.RemoveNote(_0.GetNotes().First())).IsTrue();
        Check.That(_0.GetNotes().Count).IsEqualTo(1);

        foreach(String _1 in _0.GetNotes())
        {
            Check.That(_0.RemoveNote(_1)).IsTrue();
        }
        Check.That(_0.GetNotes()).IsNull();
    }

    [Test] [Order(56)]
    public void RemoveOutput()
    {
        Check.That(_0.GetOutputIDs().All(_=>_0.RemoveOutput(_))).IsTrue();
    }

    [Test] [Order(57)]
    public void RemoveStatus()
    {
        String _1 = "S_OK";
        String _2 = "E_FAIL";
        String _3 = "OP_23";
        String _4 = "OP_97";

        Check.That(_0.GetStatus()).IsNull();
        Check.That(_0.SetStatus(_3,_2)).IsTrue();
        Check.That(_0.SetStatus(_4,_1)).IsTrue();
        Check.That(_0.GetStatus()?.Count).Equals(2);
        Check.That(_0.GetStatus()?[_3]).Equals(_2);
        Check.That(_0.RemoveStatus(_3)).IsTrue();
        Check.That(_0.GetStatus()?[_4]).Equals(_1);
        Check.That(_0.RemoveStatus(_4)).IsTrue();
        Check.That(_0.GetStatus()).IsNull();
    }

    [Test] [Order(58)]
    public void RemoveTag()
    {
        Check.That(_0.RemoveTag(null)).IsFalse();
        Check.That(_0.GetTags().Count).IsEqualTo(2);
        Check.That(_0.RemoveTag(_0.GetTags().First())).IsTrue();
        Check.That(_0.GetTags().Count).IsEqualTo(1);

        foreach(String _1 in _0.GetTags())
        {
            Check.That(_0.RemoveTag(_1)).IsTrue();
        }
        Check.That(_0.GetTags()).IsNull();
    }

    [Test] [Order(59)]
    public void SetApplication()
    {
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
        Check.That(_0.SetApplication(String.Empty)).IsTrue();
        Check.That(_0.GetApplication()).IsNull();
    }

    [Test] [Order(60)]
    public void SetApplicationVersion()
    {
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);
        
        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()).IsEqualTo(_2);
        Check.That(_0.SetApplicationVersion(new Version("0.0.0.0"))).IsTrue();
        Check.That(_0.GetApplicationVersion()).IsNull();

    }

    [Test] [Order(61)]
    public void SetBornOn()
    {
        DateTimeOffset _2 = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);
        DateTimeOffset? _1 = _0.GetBornOn();

        Check.That(_1).IsNotNull().And.IsNotEqualTo(_2);
        Check.That(_0.SetBornOn(_2)).IsTrue();
        Check.That(_0.GetBornOn()).Equals(_2);
        Check.That(_0.SetBornOn(_1)).IsTrue();
    }

    [Test] [Order(62)]
    public void SetCertificates()
    {
        Dictionary<String,String> _1 = new Dictionary<String,String>();
        _1.Add("Content 0","6131f9c300030000000f");
        _1.Add("Content 1","611e23c200030000000e");
        _1.Add("Extension ExtKey InnerKey1 DeeperKeyN 8","473d29a500030000000f");

        Check.That(_0.SetCertificates(_1)).IsTrue();
        Check.That(_0.GetCertificates()).ContainsExactly(_1);
        Check.That(_0.SetCertificates(new Dictionary<String,String>())).IsTrue();
        Check.That(_0.GetCertificates()).IsNull();
    }

    [Test] [Order(63)]
    public void SetDistinguishedName()
    {
        String _1 = "CN=ServiceN,OU=BoardofDirectors,DC=Corporate,DC=BIZ";

        Check.That(_0.SetDistinguishedName(null)).IsFalse();
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test] [Order(64)]
    public void SetDomainID()
    {
        String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.UnLock("key")).IsTrue();
        Check.That(_0.SetDomainID(String.Empty)).IsTrue();
        Check.That(_0.GetDomainID()).IsNull();
    }

    [Test] [Order(65)]
    public void SetExtension()
    {
        String _1 = "SetExtensionExam"; Guid _3 = Guid.NewGuid();

        Dictionary<String,Object?> _2 = new();

        _2.Add("1",_1); _2.Add("3",_3);

        Check.That(_0.SetExtension(_2)).IsTrue();

        Check.That((String?)((Dictionary<String,Object?>)_0.GetExtension())["1"]).IsEqualTo(_1);
        Check.That((Guid?)((Dictionary<String,Object?>)_0.GetExtension())["3"]).IsEqualTo(_3);
    }

    [Test] [Order(66)]
    public void SetFILE()
    {
        String _1     = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.UnLock("key")).IsTrue();
        Check.That(_0.SetFILE(String.Empty)).IsTrue();
        Check.That(_0.GetFILE()).IsNull();
    }

    [Test] [Order(67)]
    public void SetGPS()
    {
        String _1     = "SetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
        Check.That(_0.UnLock("key")).IsTrue();
        Check.That(_0.SetGPS(String.Empty)).IsTrue();
        Check.That(_0.GetGPS()).IsNull();
    }

    [Test] [Order(68)]
    public void SetID()
    {
        Guid _1       = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.SetID(_1)).IsFalse();
        Check.That(_0.GetID()).Equals(_1);
        Check.That(_0.UnLock("key")).IsTrue();
    }

    [Test] [Order(69)]
    public void SetLinks()
    {
        Dictionary<String,GuidReferenceItem> _1 = new Dictionary<String,GuidReferenceItem>();
        GuidReferenceItem _2 = new GuidReferenceItem();
        GuidReferenceItem _3 = new GuidReferenceItem();
        GuidReferenceItem _4 = new GuidReferenceItem();
        _1.Add("_2",_2); _1.Add("_3",_3); _1.Add("_4",_4);

        Check.That(_0.SetLinks(null)).IsFalse();
        Check.That(_0.SetLinks(_1)).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetLinks()).HasSize(3);
        Check.That(_0.GetLinks()?["_2"].ID).IsEqualTo(_2.ID);
        Check.That(_0.GetLinks()?["_3"].ID).IsEqualTo(_3.ID);
        Check.That(_0.GetLinks()?["_4"].ID).IsEqualTo(_4.ID);
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsFalse();
        Check.That(_0.UnLock("key")).IsTrue();
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsTrue();
        Check.That(_0.GetLinks()).IsNull();
    }

    [Test] [Order(70)]
    public void SetLocator()
    {
        String _1 = "app://server/SetLocatorExam";
        Uri _2 = new Uri(_1);

        Check.That(_0.SetLocator(null)).IsFalse();
        Check.That(_0.SetLocator(_2)).IsTrue();
        Check.That(_0.GetLocator().AbsoluteUri).IsEqualTo(_1);
        Check.That(_0.SetLocator(new Uri("null:"))).IsTrue();
        Check.That(_0.GetLocator()).IsNull();
    }

    [Test] [Order(71)]
    public void SetModified()
    {
        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_0.SetModified(null)).IsFalse();
        Check.That(_0.SetModified(_1)).IsTrue();
        Check.That(_0.GetModified()).Equals(_1);
        Check.That(_0.SetModified(DateTimeOffset.MinValue)).IsTrue();
        Check.That(_0.GetModified()).IsNull();
    }

    [Test] [Order(72)]
    public void SetName()
    {
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
        Check.That(_0.SetName(String.Empty)).IsTrue();
        Check.That(_0.GetName()).IsNull();
    }

    [Test] [Order(73)]
    public void SetObjectives()
    {
        String _1 = "SetObjectivesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(_0.SetObjectives(null)).IsFalse();
        Check.That(_0.SetObjectives(_2)).IsTrue();
        Check.That(_0.GetObjectives()).Contains(_2);
        Check.That(_0.GetObjectives().ToArray().First()).IsEqualTo(_1);
    }

    [Test] [Order(74)]
    public void SetPolicies()
    {
        String _1 = "SetPoliciesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(_0.SetPolicies(null)).IsFalse();
        Check.That(_0.SetPolicies(_2)).IsTrue();
        Check.That(_0.GetPolicies()).Contains(_2);
        Check.That(_0.GetPolicies().ToArray().First()).IsEqualTo(_1);
    }

    [Test] [Order(75)]
    public void SetPurpose()
    {
        String _1 = "SetPurposeExam";

        Check.That(_0.SetPurpose(null)).IsFalse();
        Check.That(_0.SetPurpose(_1)).IsTrue();
        Check.That(_0.GetPurpose()).IsEqualTo(_1);
        Check.That(_0.SetPurpose(String.Empty)).IsTrue();
        Check.That(_0.GetPurpose()).IsNull();
    }

    [Test] [Order(76)]
    public void SetSecurityDescriptor()
    {
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test] [Order(77)]
    public void SetServiceVersion()
    {
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion().ToString()).IsEqualTo(_1);
    }

    [Test] [Order(78)]
    public void SetStatus()
    {
        String _1 = "S_OK";
        String _2 = "E_FAIL";
        String _3 = "OP_23";
        String _4 = "OP_97";

        Check.That(_0.GetStatus()).IsNull();
        Check.That(_0.SetStatus(_3,_2)).IsTrue();
        Check.That(_0.SetStatus(_4,_1)).IsTrue();
        Check.That(_0.GetStatus()?.Count).Equals(2);
        Check.That(_0.GetStatus()?[_3]).Equals(_2);
        Check.That(_0.RemoveStatus(_3)).IsTrue();
        Check.That(_0.GetStatus()?[_4]).Equals(_1);
        Check.That(_0.RemoveStatus(_4)).IsTrue();
        Check.That(_0.GetStatus()).IsNull();
    }

    [Test] [Order(79)]
    public void SetSubordinates()
    {
        Tool _1 = new Tool(); _1.ID = Guid.NewGuid();
        Tool _2 = new Tool(); _2.ID = Guid.NewGuid();
        HashSet<Tool> _3 = new HashSet<Tool>();

        Check.That(_0.SetSubordinates(_3.ToList())).IsTrue();

        Check.That(_0.GetSubordinates()).IsNull();

        _3.Add(_1); _3.Add(_2);

        Check.That(_0.SetSubordinates(_3.ToList())).IsTrue();

        Check.That(_0.GetSubordinates().Count).IsEqualTo(2);
    }

    [Test] [Order(80)]
    public void SetSuperior()
    {
        Tool _1 = new Tool();

        Check.That(_0.SetSuperior(_1)).IsTrue();
        Check.That(_0.GetSuperior()).IsInstanceOfType(typeof(Tool));
    }

    [Test] [Order(81)]
    public void SetVersion()
    {
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion().ToString()).IsEqualTo(_1);
        Check.That(_0.SetVersion(new Version("0.0.0.0"))).IsTrue();
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test] [Order(82)]
    public void UnLock()
    {
        String _1 = "UnLockExam";

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(_1)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock("False")).IsFalse();

        Check.That(_0.UnLock(_1)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }

    [Test] [Order(83)]
    public void UpdateInputs()
    {
        String _1 = "UpdateInputsExam";
        String _3 = "Pass";
        Queue<Object> _2 = new Queue<Object>();
        Queue<Object> _4 = new Queue<Object>();
        Queue<Object> _5;

        _2.Enqueue(_1);
        Check.That(_0.UpdateInputs(_2)).IsTrue();

        Check.That(_0.UpdateInputs(null)).IsFalse();
        Check.That(_0.GetInputs()).Contains(_2.ToArray());
        Check.That(_0.GetInputs().Peek()).IsEqualTo(_1);

        _4.Enqueue(_1);
        _4.Enqueue(_3);

        Check.That(_0.UpdateInputs(_4)).IsTrue();

        _5 = _0.GetInputs();

        Check.That(_5.Dequeue()).IsEqualTo(_1);
        Check.That(_5.Dequeue()).IsEqualTo(_3);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.UpdateInputs(_5)).IsFalse();
        Check.That(_0.UnLock("key")).IsTrue();
        Check.That(_0.UpdateInputs(new Queue<Object>())).IsTrue();
        Check.That(_0.GetInputs()).IsNull();
    }
}