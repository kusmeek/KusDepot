
namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolExam
{
    [Test]
    public void Alert()
    {
        Tool _0 = new Tool(); this.DataID = Guid.NewGuid();
        _0.Alert += new EventHandler<AlertEventArgs>(this.HandleAlert);
        _0.RaiseAlert(this,new DataItemEventArgs(AlertCode.DataItemAdded,this.DataID));
    }

    private Guid? DataID;

    private void HandleAlert(Object? sender , AlertEventArgs args)
    {
        DataItemEventArgs _0 = (DataItemEventArgs)args;

        Check.That(_0.Code).Equals(AlertCode.DataItemAdded);
        Check.That(_0.ID).Equals(this.DataID);
    }

    [Test]
    public void Activate()
    {
        Tool _0 = new Tool();

        Check.That(_0.Activate()).IsTrue();
    }

    [Test]
    public void AddActivity()
    {
        Tool _0 = new Tool();
        Activity _1 = new Activity(Task.FromResult<Object?>(true));
        Activity _2 = new Activity(Task.FromResult<Object?>(false));

        Check.That(_0.GetActivities()).IsNull();
        Check.That(_0.AddActivity(_1)).IsTrue();
        Check.That(_0.AddActivity(_2)).IsTrue();
        Check.That(_0.GetActivities()).Contains(_1,_2);
        Check.That(_0.AddActivity(_2)).IsTrue();
        Check.That(_0.GetActivities()).HasSize(2);
        Check.That(_0.RemoveActivity(_2)).IsTrue();
        Check.That(_0.GetActivities()).HasSize(1);
        Check.That(_0.RemoveActivity(_2)).IsFalse();
        Check.That(_0.GetActivities()).Not.Contains(_2);
        Check.That(_0.RemoveActivity(_0.GetActivities()!.Single())).IsTrue();
        Check.That(_0.GetActivities()).IsNull();
    }

    [Test]
    public void AddDataItems()
    {
        HashSet<DataItem> _0 = new HashSet<DataItem>();
        GuidReferenceItem _1 = new GuidReferenceItem();
        GenericItem _2       = new GenericItem();
        TextItem _3          = new TextItem();
        MSBuildItem _4       = new MSBuildItem();
        BinaryItem _5        = new BinaryItem();
        MultiMediaItem _6    = new MultiMediaItem();
        CodeItem _16         = new CodeItem();
        Tool _7              = new Tool();
        List<Guid?> _8       = new List<Guid?>();
        List<Guid?> _10      = new List<Guid?>();
        HashSet<DataItem> _11   = new HashSet<DataItem>();
        List<Guid?> _12      = new List<Guid?>();
        List<Guid?> _13      = new List<Guid?>();

        _0.Add(_1) ;_8.Add(_1.GetID()); _12.Add(_1.GetID());
        _0.Add(_2) ;_8.Add(_2.GetID()); _12.Add(_2.GetID());
        _0.Add(_3) ;_8.Add(_3.GetID()); _12.Add(_3.GetID());
        _0.Add(_4) ;_8.Add(_4.GetID()); _12.Add(_4.GetID());
        _0.Add(_5) ;_8.Add(_5.GetID()); _12.Add(_5.GetID());
        _0.Add(_16) ;_8.Add(_16.GetID()); _12.Add(_16.GetID());
        _7.AddDataItems(_0);
        HashSet<DataItem>? _9   = _7.GetDataItems();
        if(_9 is not null)
        {
            foreach(DataItem item in _9) { _10!.Add(item!.GetID()); }
        }
        _11.Add(_6); _12.Add(_6.GetID());
        _7.AddDataItems(_11);
        HashSet<DataItem>? _14   = _7.GetDataItems();
        if(_14 is not null)
        {
            foreach(DataItem item in _14) { _13!.Add(item!.GetID()); }
        }

        Check.That(_7.AddDataItems(null)).IsFalse();
        Check.That(_10).ContainsExactly(_8.ToArray());
        Check.That(_13).ContainsExactly(_12.ToArray());
    }

    [Test]
    public void AddInput()
    {
        Tool _0 = new Tool();
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

    [Test]
    public void AddNotes()
    {
        Tool _0 = new Tool();
        HashSet<String> _1 = new(){"AddNotesExam"};
        HashSet<String> _2 = new(){"Pass",String.Empty};
        HashSet<String> _3 = new(); _3.UnionWith(_1); _3.UnionWith(_2);

        Check.That(_0.AddNotes(null)).IsFalse();
        Check.That(_0.GetNotes()).IsNull();
        Check.That(_0.AddNotes(_1)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
        Check.That(_0.AddNotes(_2)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_3);
    }

    [Test]
    public void AddOutput()
    {
        Tool _0 = new Tool();
        String _1 = "AddOutputExam"; Guid _11 = Guid.NewGuid();
        String _2 = "Pass";          Guid _12 = Guid.NewGuid();
        Dictionary<Int32,String> _3 = new Dictionary<Int32,String>(){{0,_2}}; Guid _13 = Guid.NewGuid();

        Check.That(_0.GetOutputIDs()).IsNull();
        Check.That(_0.AddOutput(_11,_1)).IsTrue();
        Check.That(_0.GetOutputIDs()).Contains(_11);
        Check.That(_0.AddOutput(_12,_2)).IsTrue();
        Check.That(_0.GetOutputIDs()).Contains(_11,_12);
        Check.That(_0.AddOutput(_13,_3)).IsTrue();
        Check.That(_0.GetOutputIDs()).Contains(_11,_12,_13);
        Check.That(_0.AddOutput(Guid.NewGuid(),null)).IsTrue();
        Check.That(((Dictionary<Int32,String>)(_0.GetOutput(_13)!))[0]).Equals(_2);
    }

    [Test]
    public void AddTags()
    {
        Tool _0 = new Tool();
        String _1 = "AddTagsExam";
        String _2 = "Pass";
        HashSet<String> _3 = new(); _3.Add(_1);
        HashSet<String> _4 = new(); _4.Add(_2); _4.Add(String.Empty);

        Check.That(_0.AddTags(null)).IsFalse();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.AddTags(_3)).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
        Check.That(_0.AddTags(_4)).IsTrue();
        Check.That(_0.GetTags()).Contains(_1,_2,String.Empty);
        Check.That(_0.AddTags(new HashSet<String>())).IsTrue();
    }

    [OneTimeSetUp]
    public void Calibrate() { if(Settings.NoExceptions is true) { throw new InvalidOperationException(); } }

    [Test]
    public void ClearEventLogs()
    {
        Tool _0 = new();
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

    [Test]
    public void Clone()
    {
        Tool _0 = new Tool(null,Guid.NewGuid());

        Check.That(_0.Clone()!.Equals(_0)).IsTrue();
    }

    [Test]
    public void CompareTo()
    {
        Tool _0 = new Tool();
        Thread.Sleep(100);
        Tool _1 = new Tool();

        Check.That(new Tool().CompareTo(null)).IsEqualTo(1);
        Check.That(_0.CompareTo(_0)).IsEqualTo(0);
        Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
        Check.That(_1.CompareTo(_0)).IsStrictlyPositive();
    }

    [Test]
    public void Constructor()
    {
        HashSet<DataItem> _0 = new HashSet<DataItem>();
        GuidReferenceItem _1 = new GuidReferenceItem();
        GenericItem _2       = new GenericItem();
        TextItem _3          = new TextItem();
        MSBuildItem _4       = new MSBuildItem();
        BinaryItem _5        = new BinaryItem();
        MultiMediaItem _6    = new MultiMediaItem();
        CodeItem _7          = new CodeItem();
        List<Guid?> _8       = new List<Guid?>();
        List<Guid?> _10      = new List<Guid?>();
        _0.Add(_1); _8.Add(_1.GetID());
        _0.Add(_2); _8.Add(_2.GetID());
        _0.Add(_3); _8.Add(_3.GetID());
        _0.Add(_4); _8.Add(_4.GetID());
        _0.Add(_5); _8.Add(_5.GetID());
        _0.Add(_6); _8.Add(_6.GetID());
        _0.Add(_7); _8.Add(_7.GetID());
        String _11 = "ToolConstructorExamInputs";
        Queue<Object> _22 = new Queue<Object>();
        _22.Enqueue(_11);
        Guid _15 = Guid.NewGuid();
        String _17 = "My Tool";
        String _18 = "ToolConstructorExamNotes";
        HashSet<String> _24 = new HashSet<String>(){_18};
        String _32 = "ToolConstructorExamTags";
        HashSet<String> _38 = new HashSet<String>(){_32};
        HashSet<Trait> _40 = new HashSet<Trait>(){ new TraitTest() };
        Tool _99 = new Tool(_0,_15,_17,null,_22,_24,_38,_40);
        HashSet<DataItem>? _9   = _99.GetDataItems();
        if(_9 is not null)
        {
            foreach(DataItem item in _9) { _10!.Add(item!.GetID()); }
        }

        Check.That(_99).IsInstanceOfType(typeof(Tool));
        Check.That(_10).ContainsExactly(_8.ToArray());
        Check.That(_99.GetID()).IsEqualTo(_15);
        Check.That(_99.GetName()).IsEqualTo(_17);
        Check.That(_99.GetNotes()).ContainsExactly(_18);
        Check.That(_99.GetTags()).ContainsExactly(_32);
        Check.That(_99.GetDataItems()).ContainsExactly(_0);
        Check.That(_99.GetTags()).ContainsExactly(_38);
    }

    [Test]
    public void Deactivate()
    {
        Tool _0 = new Tool();

        Check.That(_0.Deactivate()).IsTrue();
    }

    [Test]
    public void Dispose()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetID()).HasAValue();
        Check.That(_0.GetDisposed()).IsFalse();

        _0.Dispose();

        Check.That(_0.GetDisposed()).IsTrue();
        Check.That(_0.GetID()).IsNull();
    }

    [Test]
    public void EqualsObject()
    {
        Check.That((Object)new Tool().Equals(null)).IsEqualTo(false);
        Check.That((Object)new Tool().Equals(new Object())).IsEqualTo(false);
    }

    [Test]
    public void EqualsInterface()
    {
        Tool _0 = new Tool(); _0.SetID(Guid.Empty);
        Tool _1 = new Tool(); _1.SetID(Guid.Empty);
        String _2 = "ToolEqualsExam";
        Guid _3 = Guid.NewGuid();
        Tool _4 = new Tool(); _4.SetApplication(_2);
        Tool _5 = new Tool(); _5.SetApplication(_2); _5.SetID(_3);
        Tool _6 = new Tool(); _6.SetApplication(_2); _6.SetID(_3);
        String _7 = "ToolEqualsExam2";
        Tool _8 = new Tool(); _8.SetApplication(_7); _8.SetID(_3);
        Guid _9 = Guid.NewGuid();
        Tool _10 = new Tool(); _10.SetApplication(_2); _10.SetID(_9);
        Tool _11 = new Tool(); _11.SetApplication(_7); _11.SetID(Guid.Empty);
        String _12 = "ToolEqualsExam3";
        Tool _13 = new Tool();_13.SetDistinguishedName(_12); _13.SetID(_3);
        Tool _14 = new Tool();_14.SetDistinguishedName(_12); _14.SetApplication(_2); _14.SetID(_3);
        Tool _15 = new Tool();_15.SetDistinguishedName(_12); _15.SetApplication(_2); _15.SetID(Guid.Empty);
        Tool _16 = new Tool(null,null,null,null,null,null,null,null);

        Check.That(new Tool().Equals(null)).IsFalse();
        Check.That(new Tool().Equals(new Object())).IsFalse();
        Check.That(new Tool().Equals(_16)).IsFalse();
        Check.That(_0.Equals(_4)).IsFalse();
        Check.That(_4.Equals(_1)).IsFalse();
        Check.That(_4.Equals(_5)).IsFalse();
        Check.That(_4.Equals(_11)).IsFalse();
        Check.That(_11.Equals(_0)).IsFalse();
        Check.That(_11.Equals(_4)).IsFalse();
        Check.That(_5.Equals(_6)).IsTrue();
        Check.That(_6.Equals(_8)).IsFalse();
        Check.That(_6.Equals(_10)).IsFalse();
        Check.That(_0.Equals(_1)).IsTrue();
        Check.That(_13.Equals(_1)).IsFalse();
        Check.That(_13.Equals(_14)).IsFalse();
        Check.That(_14.Equals(_15)).IsFalse();
    }

    [Test]
    public void ExecuteCommand()
    {
        Tool _0 = new Tool(); _0.Alert += new EventHandler<AlertEventArgs>(this.CheckOutput); Guid _4 = Guid.NewGuid(); this.OutputID = _4;
        Object[] _1 = new Object[4]{"Multiply",Guid.Empty,Double.Parse("483.660",null),Double.Parse("660.483",null)};
        Object[] _2 = new Object[4]{"Multiply",_4,Double.Parse("483.660",null),Double.Parse("660.483",null)};
        Double _3 = Double.Parse("483.660",null) * Double.Parse("660.483",null); this.Output_3 = _3;
        CommandTest _5 = new CommandTest();

        Check.That(_0.ExecuteCommand(new[]{"NoCommand"})).IsNull();
        Check.That(_0.RegisterCommand(_5,"Multiply")).IsTrue();        
        Check.That(_0.Activate()).IsTrue();
        Check.That(_0.ExecuteCommand(_1)).IsInstanceOf<Guid?>().And.IsNotEqualTo<Guid?>(Guid.Empty);
        Check.That(_0.Deactivate()).IsTrue();
        Check.That(_0.ExecuteCommand(_2)).IsNull();
        Check.That(_0.Activate()).IsTrue();
        Check.That(_0.ExecuteCommand(_2)).IsInstanceOf<Guid?>().And.IsEqualTo<Guid?>(_4);
    }

    private Guid? OutputID; private Double? Output_3;

    private void CheckOutput(Object? sender , EventArgs args)
    {
        Check.That(((Tool)sender!).GetOutputIDs()!.Count).Equals(2);
        Check.That(((Tool)sender!).GetOutput(this.OutputID!.Value)).Equals(this.Output_3!);
    }

    [Test]
    public void FromFile()
    {
        Byte[] _1 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_1);

        BinaryItem _2 = new BinaryItem(_1,null,String.Empty);

        Tool _3 = new Tool(new HashSet<DataItem>(){_2},Guid.NewGuid());

        String _4 = Environment.GetEnvironmentVariable("TEMP") + "\\Tool.ktool";

        File.Delete(_4);

        Check.That(File.Exists(_4)).IsFalse();

        Check.That(_3.ToFile(_4)).IsTrue();

        Tool? _5 = Tool.FromFile(_4);

        Check.That(_5).IsNotNull();

        Check.That(_3.Equals(_5)).IsTrue();

        File.Delete(_4);

        Check.That(File.Exists(_4)).IsFalse();
    }

    [Test]
    public void GetActivities()
    {
        Tool _0 = new Tool();
        String _1 = "GetActivities";

        Check.That(_0.GetActivities()).IsNull();
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.GetActivities()).IsNull();
    }

    [Test]
    public void GetAppDomainID()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetAppDomainID()).HasAValue();
    }

    [Test]
    public void GetAppDomainUID() 
    {
        Tool _0 = new Tool();

        Check.That(_0.GetAppDomainUID()).IsNull();
    }

    [Test]
    public void GetApplication()
    {
        Tool _0 = new Tool();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetAssemblyVersion()
    {
        Tool _0 = new Tool();

        Version? _1 = Assembly.GetExecutingAssembly().GetName().Version;

        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
    }

    [Test]
    public void GetBornOn()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public void GetCertificates()
    {
        Tool _0 = new Tool();
        Dictionary<String,String> _1 = new();
        _1.Add("Content 0","6131f9c300030000000f");
        _1.Add("Content 1","611e23c200030000000e");
        _1.Add("Extension ExtKey InnerKey1 DeeperKeyN 8","473d29a500030000000f");

        Check.That(_0.SetCertificates(_1.ToImmutableDictionary())).IsTrue();
        Check.That(_0.GetCertificates()).Contains(_1);
        Check.That(_0.SetCertificates(new Dictionary<String,String>())).IsTrue();
        Check.That(_0.GetCertificates()).IsNull();
    }

    [Test]
    public void GetCommands()
    {
        Tool _0 = new Tool();
        CommandTest _1 = new CommandTest();

        Check.That(_0.GetCommands()).IsNull();
        Check.That(_0.RegisterCommand(_1,"GetCommandsExam")).IsTrue();
        Check.That(_0.GetCommands()?.Count).Equals(1);
        Check.That(_0.GetCommands()!["GetCommandsExam"]).Equals(_1);
    }

    [Test]
    public void GetContainer()
    {
        Tool _0 = new Tool();
        IContainer _1 = new ContainerBuilder().Build();

        Check.That(_0.GetContainer()).IsNull();
        Check.That(_0.SetContainer(_1)).IsTrue();
        Check.That(_0.GetContainer()).Equals(_1);
    }

    [Test]
    public void GetControls()
    {
        Tool _0 = new Tool();
        List<String> _1 = new(){"GetControls","Exam","Pass","Exam"};

        Check.That(_0.SetControls(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetControls()).Contains(_1.Distinct());
        Check.That(_0.SetControls(new HashSet<String>())).IsTrue();
        Check.That(_0.GetControls()).IsNull();
    }

    [Test]
    public void GetCPUID()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetCPUID()).IsNull();
    }

    [Test]
    public void GetDataItems()
    {
        HashSet<DataItem>? _0   = new HashSet<DataItem>();
        GuidReferenceItem _1 = new GuidReferenceItem();
        GenericItem _2       = new GenericItem();
        TextItem _3          = new TextItem();
        MSBuildItem _4       = new MSBuildItem();
        BinaryItem _5        = new BinaryItem();
        MultiMediaItem _6    = new MultiMediaItem();
        Tool _7              = new Tool();
        List<Guid?> _8       = new List<Guid?>();
        List<Guid?> _10      = new List<Guid?>();
        List<Guid?> _11      = new List<Guid?>();
        _0.Add(_1) ;_8.Add(_1.GetID());
        _0.Add(_2) ;_8.Add(_2.GetID());
        _0.Add(_3) ;_8.Add(_3.GetID());
        _0.Add(_4) ;_8.Add(_4.GetID());
        _0.Add(_5) ;_8.Add(_5.GetID());
        _0.Add(_6) ;_8.Add(_6.GetID());
        _7.AddDataItems(_0);
        HashSet<DataItem>? _9   = _7.GetDataItems();
        if(_9 is not null)
        {
            foreach(DataItem item in _9) { _10!.Add(item!.GetID()); }
        }

        Check.That(_10).ContainsExactly(_8.ToArray());

        Check.That(_7.Lock("Secret")).IsTrue();

        HashSet<DataItem>? _12   = _7.GetDataItems();
        if(_12 is not null)
        {
            foreach(DataItem item in _12) { _11!.Add(item!.GetID()); }
        }

        Check.That(_11).ContainsExactly(_8.ToArray());
    }

    [Test]
    public void GetDescriptor()
    {
        Tool _0 = new Tool();

        String _s = "GetDescriptorExam";
        Version _v = new Version("1.2.3.4");
        DateTimeOffset _d = DateTimeOffset.Now;
        HashSet<String> _n = new(){"Get","Descriptor","Exam","Notes"};
        HashSet<String> _t = new(){"Get","Descriptor","Exam","Tags"};
        Uri _l = new Uri("over://here"); String _p = "Purpose";

        Check.That(_0.SetApplication(_s)).IsTrue();
        Check.That(_0.SetApplicationVersion(_v)).IsTrue();
        Check.That(_0.SetDistinguishedName(_s)).IsTrue();
        Check.That(_0.SetLocator(_l)).IsTrue();
        Check.That(_0.SetModified(_d)).IsTrue();
        Check.That(_0.SetName(_s)).IsTrue();
        Check.That(_0.AddNotes(_n)).IsTrue();
        Check.That(_0.SetPurpose(_p)).IsTrue();
        Check.That(_0.SetServiceVersion(_v)).IsTrue();
        Check.That(_0.AddTags(_t)).IsTrue();
        Check.That(_0.SetVersion(_v)).IsTrue();

        Descriptor? _ = _0.GetDescriptor();

        Check.That(_!.Application).IsEqualTo(_s);
        Check.That(_.ApplicationVersion).IsEqualTo(_v.ToString());
        Check.That(_.BornOn).Equals(_0.GetBornOn()?.ToString("O"));
        Check.That(_.DistinguishedName).IsEqualTo(_s);
        Check.That(_.ID).Equals(_0.GetID());
        Check.That(_.Locator).IsEqualTo(_l.ToString());
        Check.That(_.Modified).IsEqualTo(_d.ToString("O"));
        Check.That(_.Name).IsEqualTo(_s);
        Check.That(_.Notes).Contains(_n);
        Check.That(_.Purpose).IsEqualTo(_p);
        Check.That(_.ObjectType).Equals("Tool");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Version).IsEqualTo(_v.ToString());
    }

    [Test]
    public void GetDisposed()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetDisposed()).IsFalse();

        _0.Dispose();

        Check.That(_0.GetDisposed()).IsTrue();
    }

    [Test]
    public void GetDistinguishedName()
    {
        Tool _0 = new Tool();
        String _1 = "CN=ServiceN,OU=Engineering,DC=TailSpinToys,DC=COM";

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void GetDomainID()
    {
        Tool _0 = new Tool();
        String _1 = "GetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
    }

    [Test]
    public void GetEvent()
    {
        Tool _0 = new();
        String _1 = "GetEventExam";
        String _2 = "Pass";

        Check.That(_0.LogEvent(_1)).IsTrue();
        Check.That(_0.LogEvent(_2)).IsTrue();
        Check.That(_0.GetEventLogs()!.Keys.Count).Equals(2);
        Check.That(_0.GetEvent(0)).Equals(_1);
        Check.That(_0.GetEvent(1)).Equals(_2);
        Check.That(_0.ClearEventLogs()).IsTrue();
        Check.That(_0.GetEventLogs()).IsNull();
    }

    [Test]
    public void GetEventLogs()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetEventLogs()).IsNull();
    }

    [Test]
    public void GetExtension()
    {
        Tool _0 = new Tool();
        Dictionary<String,Object?> _1 = new Dictionary<String,Object?>();
        String _2 = "Pass";
        String _3 = "OK";
        Func<String> _4 = () => { return _3; };

        _1.Add("1",_2);
        _1.Add("2",_4);

        Check.That(_0.SetExtension(_1)).IsTrue();
        Check.That((String?)_0.GetExtension()!["1"]).IsEqualTo(_2);
        Check.That(((Func<String>?)_0.GetExtension()!["2"])!.DynamicInvoke()).IsEqualTo(_3);
    }

    [Test]
    public void GetFILE()
    {
        Tool _0 = new Tool();
        String _1 = "GetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.SetFILE(String.Empty)).IsTrue();
        Check.That(_0.GetFILE()).IsNull();
    }

    [Test]
    public void GetGPS()
    {
        Tool _0 = new Tool();
        String _1 = "GetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
    }

    [Test]
    public void GetHash()
    {
        Tool _0 = new Tool();
        Tool _1 = new Tool();

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_0.GetHashCode()).IsNotEqualTo(_1.GetHashCode());
    }

    [Test]
    public void GetID()
    {
        Tool _0 = new Tool();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void GetInput()
    {
        Tool _0 = new Tool();
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

    [Test]
    public void GetInputs()
    {
        Tool _0 = new Tool();
        String _1 = "GetInputsExam";
        Queue<Object> _2 = new Queue<Object>();
        _2.Enqueue(_1);

        Check.That(_0.UpdateInputs(_2)).IsTrue();
        Check.That(_0.GetInputs()).Contains(_2);
        Check.That(_0.GetInputs()!.Dequeue()).IsEqualTo(_1);
    }

    [Test]
    public void GetLinks()
    {
        Tool _0 = new Tool();
        Dictionary<String,GuidReferenceItem> _1 = new();
        GuidReferenceItem _2 = new GuidReferenceItem();
        GuidReferenceItem _3 = new GuidReferenceItem();
        GuidReferenceItem _4 = new GuidReferenceItem();
        _1.Add("_2",_2); _1.Add("_3",_3); _1.Add("_4",_4);

        Check.That(_0.SetLinks(null)).IsFalse();
        Check.That(_0.SetLinks(_1.ToImmutableDictionary())).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetLinks()).HasSize(3);
        Check.That(_0.GetLinks()?["_2"]).IsEqualTo(_2);
        Check.That(_0.GetLinks()?["_3"]).IsEqualTo(_3);
        Check.That(_0.GetLinks()?["_4"]).IsEqualTo(_4);
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsFalse();
        Check.That(_0.UnLock("key")).IsTrue();
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsTrue();
        Check.That(_0.GetLinks()).IsNull();
    }

    [Test]
    public void GetLocator()
    {
        Tool _0 = new Tool();
        String _1 = "app://server/GetLocatorExam";
        Uri _2 = new Uri(_1);

        Check.That(_0.SetLocator(_2)).IsTrue();
        Check.That(_0.GetLocator()!.AbsoluteUri).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetLocator()!.AbsoluteUri).IsEqualTo(_1);
    }

    [Test]
    public void GetLocked()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(String.Empty)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetMachineID()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetMachineID()).HasContent();
    }

    [Test]
    public void GetModified()
    {
        Tool _0 = new Tool();
        Tool _3 = new Tool();
        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_3.SetModified(_1)).IsTrue();
        Check.That(_0.GetModified()).IsNull();
        Check.That(_3.GetModified()).Equals(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_3.GetModified()).Equals(_1);
    }

    [Test]
    public void GetName()
    {
        Tool _0 = new Tool();
        String _1 = "GetNameExam";

        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void GetNotes()
    {
        Tool _0 = new Tool();
        String _2 = "GetNotesExam";
        List<String> _1 = new List<String>(){_2,"Pass"};

        Check.That(_0.AddNotes(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
        Check.That(_0.Lock(_2)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
    }

    [Test]
    public void GetObjectives()
    {
        Tool _0 = new Tool();
        String _1 = "GetObjectivesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(_0.SetObjectives(_2)).IsTrue();
        Check.That(_0.GetObjectives()).Contains(_2);
        Check.That(_0.GetObjectives()!.ToArray().First()).IsEqualTo(_1);
    }

    [Test]
    public void GetOutput()
    {
        Tool _0 = new Tool();
        String _1 = "GetOutputExam";
        Guid _2 = Guid.NewGuid();
        
        Check.That(_0.GetOutput(Guid.Empty)).IsNull();
        Check.That(_0.AddOutput(_2,_1)).IsTrue();
        Check.That(_0.Lock("Key")).IsTrue();
        Check.That(_0.GetOutput(_2)).Equals(_1);
    }

    [Test]
    public void GetOutputIDs()
    {
        Tool _0 = new Tool();
        String _1 = "GetOutputsExam"; Guid _11 = Guid.NewGuid();
        
        Check.That(_0.GetOutputIDs()).IsNull();
        Check.That(_0.AddOutput(_11,_1)).IsTrue();
        Check.That(_0.Lock("Key")).IsTrue();
        Check.That(_0.GetOutputIDs()).Contains(_11);
    }

    [Test]
    public void GetPolicies()
    {
        Tool _0 = new Tool();
        String _1 = "GetPoliciesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(_0.SetPolicies(_2)).IsTrue();
        Check.That(_0.GetPolicies()).Contains(_2);
        Check.That(_0.GetPolicies()!.ToArray().First()).IsEqualTo(_1);
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.GetPolicies()).Contains(_2);
    }

    [Test]
    public void GetProcessID()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetProcessID()).HasAValue();
    }

    [Test]
    public void GetPurpose()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetPurpose()).IsNull();
    }

    [Test]
    public void GetSecurityDescriptor()
    {
        Tool _0 = new Tool();
        String _1 = "GetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void GetServiceVersion() 
    {
        Tool _0 = new Tool();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void GetStatus()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetStatus()).IsNull();
        Check.That(_0.Lock("Key")).IsTrue();
        Check.That(_0.GetStatus()).IsNull();
    }

    [Test]
    public void GetSubordinates()
    {
        Tool _0 = new Tool();
        Tool _1 = new Tool();
        Tool _2 = new Tool();
        HashSet<Tool> _3 = new HashSet<Tool>();
        Check.That(_0.SetSubordinates(_3)).IsTrue();

        Check.That(_0.GetSubordinates()).IsNull();

        _3.Add(_1);

        Check.That(_0.SetSubordinates(_3)).IsTrue();
        Check.That(_0.GetSubordinates()).Contains(_1);

        _3.Add(_2);

        Check.That(_0.SetSubordinates(_3)).IsTrue();
        Check.That(_0.GetSubordinates()).Contains(_1,_2);
    }

    [Test]
    public void GetSuperior()
    {
        Tool _0 = new Tool();
        Tool _1 = new Tool();

        Check.That(_0.SetSuperior(_1)).IsTrue();
        Check.That(_0.GetSuperior()).IsSameReferenceAs(_1);
    }

    [Test]
    public void GetTags()
    {
        Tool _0 = new Tool();
        List<String> _1 = new List<String>(){"GetTagsExam", "Pass"};

        Check.That(_0.AddTags(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
    }

    [Test]
    public void GetTelemetry()
    {
        Tool _0 = new Tool();
        List<String> _1 = new(){"GetTelemetry","Exam","Pass","Exam"};

        Check.That(_0.SetTelemetry(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTelemetry()).Contains(_1.Distinct());
        Check.That(_0.SetTelemetry(new HashSet<String>())).IsTrue();
        Check.That(_0.GetTelemetry()).IsNull();
    }

    [Test]
    public void GetThreadID()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetThreadID()).HasAValue();
    }

    [Test]
    public void GetVersion()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Initialize()
    {
        Tool _0 = new Tool();
        Version? _1 = Assembly.GetExecutingAssembly().GetName().Version;

        Check.That(_0.Initialize()).IsTrue();
        Check.That(_0.GetAppDomainID()).HasAValue();
        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
        Check.That(_0.GetBornOn()).HasAValue();
        Check.That(_0.GetID()).HasAValue();
        Check.That(_0.GetMachineID()).HasContent();
        Check.That(_0.GetProcessID()).HasAValue();
        Check.That(_0.GetThreadID()).HasAValue();
    }

    [Test]
    public void Lock()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(String.Empty)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public void LogEvent()
    {
        Tool _0 = new();
        String _1 = "LogEventExam";
        String _2 = "Pass";

        Check.That(_0.LogEvent(_1)).IsTrue();
        Check.That(_0.LogEvent(_2)).IsTrue();
        Check.That(_0.GetEventLogs()!.Keys.Count).Equals(2);
        Check.That(_0.GetEvent(0)).Equals(_1);
        Check.That(_0.GetEvent(1)).Equals(_2);
        Check.That(_0.ClearEventLogs()).IsTrue();
        Check.That(_0.GetEventLogs()).IsNull();
    }

    [Test]
    public void ParameterlessConstructor()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetActivities()).IsNull();
        Check.That(_0.GetAppDomainID()).IsNotNull();
        Check.That(_0.GetAppDomainUID()).IsNull();
        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.GetApplicationVersion()).IsNull();
        Check.That(_0.GetAssemblyVersion()).IsNotNull();
        Check.That(_0.GetBornOn()).IsNotNull();
        Check.That(_0.GetCommands()).IsNull();
        Check.That(_0.GetContainer()).IsNull();
        Check.That(_0.GetControls()).IsNull();
        Check.That(_0.GetCPUID()).IsNull();
        Check.That(_0.GetDataItems()).IsNull();
        Check.That(_0.GetDisposed()).IsFalse();
        Check.That(_0.GetDistinguishedName()).IsNull();
        Check.That(_0.GetDomainID()).IsNull();
        Check.That(_0.GetEventLogs()).IsNull();
        Check.That(_0.GetExtension()).IsNull();
        Check.That(_0.GetFILE()).IsNull();
        Check.That(_0.GetGPS()).IsNull();
        Check.That(_0.GetID()).IsNotNull();
        Check.That(_0.GetInputs()).IsNull();
        Check.That(_0.GetLinks()).IsNull();
        Check.That(_0.GetLocator()).IsNull();
        Check.That(_0.GetLocked()).IsFalse();
        Check.That(_0.GetMachineID()).IsNotNull();
        Check.That(_0.GetModified()).IsNull();
        Check.That(_0.GetName()).IsNull();
        Check.That(_0.GetNotes()).IsNull();
        Check.That(_0.GetObjectives()).IsNull();
        Check.That(_0.GetOutputIDs()).IsNull();
        Check.That(_0.GetPolicies()).IsNull();
        Check.That(_0.GetProcessID()).IsNotNull();
        Check.That(_0.GetPurpose()).IsNull();
        Check.That(_0.GetSecurityDescriptor()).IsNull();
        Check.That(_0.GetServiceVersion()).IsNull();
        Check.That(_0.GetStatus()).IsNull();
        Check.That(_0.GetSubordinates()).IsNull();
        Check.That(_0.GetSuperior()).IsNull();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.GetTelemetry()).IsNull();
        Check.That(_0.GetThreadID()).IsNotNull();
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Parse()
    {
        Tool _0 = new Tool(); _0.AddDataItems(new HashSet<DataItem>() { new MSBuildItem() });

        Tool _1 = Tool.Parse(_0.ToString(),null);

        Check.That(_1).IsInstanceOfType(typeof(Tool));
        Check.That(_0.Equals(_1)).IsTrue();
        Check.That(_0.GetDataItems()!.Count).IsEqualTo(1);
        Check.That(_1.GetDataItems()!.Count).IsEqualTo(1);
    }

    [Test]
    public void RegisterCommand()
    {
        Tool _0 = new Tool();
        CommandTest _1 = new CommandTest();
        CommandTest _2 = new CommandTest();
        String _4 = "PlayVideo";
        String _5 = "StreamVideo";
        String _6 = "Vendor2,StreamVideo";
        String _7 = "VendorN,StreamVideoTo";

        Check.That(_0.RegisterCommand(_1,_4)).IsTrue();
        Check.That(_0.RegisterCommand(_1,_5)).IsTrue();
        Check.That(_0.RegisterCommand(_1,_6)).IsTrue();
        Check.That(_0.RegisterCommand(_2,_4)).IsFalse();
        Check.That(_0.RegisterCommand(_2,_7)).IsTrue();

        Check.That(_0.GetCommands()!.Count).IsEqualTo(4);
        Check.That(_0.GetCommands()!.Keys).ContainsExactly(new String[]{_4,_5,_6,_7});
        Check.That(_0.GetCommands()!.Values.Distinct()).ContainsExactly(new CommandTest[]{_1,_2});
    }

    [Test]
    public void RemoveActivity()
    {
        Tool _0 = new Tool();
        Activity _1 = new Activity(Task.FromResult<Object?>(true));
        Activity _2 = new Activity(Task.FromResult<Object?>(false));

        Check.That(_0.GetActivities()).IsNull();
        Check.That(_0.AddActivity(_1)).IsTrue();
        Check.That(_0.AddActivity(_2)).IsTrue();
        Check.That(_0.GetActivities()).Contains(_1,_2);
        Check.That(_0.AddActivity(_2)).IsTrue();
        Check.That(_0.GetActivities()).HasSize(2);
        Check.That(_0.RemoveActivity(_2)).IsTrue();
        Check.That(_0.GetActivities()).HasSize(1);
        Check.That(_0.RemoveActivity(_2)).IsFalse();
        Check.That(_0.GetActivities()).Not.Contains(_2);
        Check.That(_0.RemoveActivity(_0.GetActivities()!.Single())).IsTrue();
        Check.That(_0.GetActivities()).IsNull();
    }

    [Test]
    public void RemoveDataItem()
    {
        HashSet<DataItem> _0 = new HashSet<DataItem>();
        GuidReferenceItem _1 = new GuidReferenceItem();
        GenericItem _2       = new GenericItem();
        TextItem _3          = new TextItem();
        MSBuildItem _4       = new MSBuildItem();
        BinaryItem _5        = new BinaryItem();
        MultiMediaItem _6    = new MultiMediaItem();
        CodeItem _16         = new CodeItem();
        Tool _7              = new Tool();
        List<Guid?> _8       = new List<Guid?>();
        List<Guid?> _10      = new List<Guid?>();
        HashSet<DataItem> _11   = new HashSet<DataItem>();
        List<Guid?> _12      = new List<Guid?>();
        List<Guid?> _13      = new List<Guid?>();

        _0.Add(_1) ;_8.Add(_1.GetID()); _12.Add(_1.GetID());
        _0.Add(_2) ;_8.Add(_2.GetID()); _12.Add(_2.GetID());
        _0.Add(_3) ;_8.Add(_3.GetID()); _12.Add(_3.GetID());
        _0.Add(_4) ;_8.Add(_4.GetID()); _12.Add(_4.GetID());
        _0.Add(_5) ;_8.Add(_5.GetID()); _12.Add(_5.GetID());
        _0.Add(_16) ;_8.Add(_16.GetID()); _12.Add(_16.GetID());
        _0.Add(_6) ;_8.Add(_6.GetID());
        _7.AddDataItems(_0);
        HashSet<DataItem>? _9   = _7.GetDataItems();
        if(_9 is not null)
        {
            foreach(DataItem item in _9) { _10!.Add(item!.GetID()); }
        }

        _7.RemoveDataItem(_6.GetID());
        HashSet<DataItem>? _14   = _7.GetDataItems();
        if(_14 is not null)
        {
            foreach(DataItem item in _14) { _13!.Add(item!.GetID()); }
        }

        Check.That(_7.RemoveDataItem(null)).IsFalse();
        Check.That(_7.RemoveDataItem(Guid.NewGuid())).IsFalse();
        Check.That(_10).ContainsExactly(_8.ToArray());
        Check.That(_13).ContainsExactly(_12.ToArray());

        foreach(DataItem _ in _7.GetDataItems()!)
        {
            Check.That(_7.RemoveDataItem(_.GetID())).IsTrue();
        }

        Check.That(_7.GetDataItems()).IsNull();
    }

    [Test]
    public void RemoveNote()
    {
        Tool _0 = new Tool();
        String _4 = "RemoveNoteExam"; String _5 = "Pass";
        HashSet<String> _1 = new(){_4};
        HashSet<String> _2 = new(){_5};
        HashSet<String> _3 = new(); _3.UnionWith(_1); _3.UnionWith(_2);

        Check.That(_0.RemoveNote(null)).IsFalse();
        Check.That(_0.GetNotes()).IsNull();
        Check.That(_0.AddNotes(_1)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
        Check.That(_0.AddNotes(_2)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_3);
        Check.That(_0.RemoveNote(_4)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_2);
        Check.That(_0.RemoveNote(_5)).IsTrue();
        Check.That(_0.GetNotes()).IsNull();
    }

    [Test]
    public void RemoveOutput()
    {
        Tool _0 = new Tool();
        String _1 = "RemoveOutputExam";
        Guid _2 = Guid.NewGuid();

        Check.That(_0.GetOutput(Guid.Empty)).IsNull();
        Check.That(_0.AddOutput(_2,_1)).IsTrue();
        Check.That(_0.RemoveOutput(Guid.Empty)).IsFalse();
        Check.That(_0.Lock("Key")).IsTrue();
        Check.That(_0.RemoveOutput(_2)).IsFalse();
        Check.That(_0.GetOutput(_2)).Equals(_1);
        Check.That(_0.UnLock("Key")).IsTrue();
        Check.That(_0.RemoveOutput(_2)).IsTrue();
        Check.That(_0.GetOutputIDs()).IsNull();
    }

    [Test]
    public void RemoveStatus()
    {
        Tool _0 = new();
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

    [Test]
    public void RemoveTag()
    {
        Tool _0 = new Tool();
        String _4 = "RemoveTagExam"; String _5 = "Pass";
        HashSet<String> _1 = new HashSet<String>(){_4};
        HashSet<String> _2 = new HashSet<String>(){_5};
        HashSet<String> _3 = new HashSet<String>(); _3.UnionWith(_1); _3.UnionWith(_2);

        Check.That(_0.RemoveTag(null)).IsFalse();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.AddTags(_1)).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
        Check.That(_0.AddTags(_2)).IsTrue();
        Check.That(_0.GetTags()).Contains(_3);
        Check.That(_0.RemoveTag(_4)).IsTrue();
        Check.That(_0.GetTags()).Contains(_2);
        Check.That(_0.RemoveTag(_5)).IsTrue();
        Check.That(_0.GetTags()).IsNull();
    }

    [Test]
    public void Serialize()
    {
        Tool _0 = new Tool();
        Check.That(_0.AddDataItems(new HashSet<DataItem>{new GuidReferenceItem(),new CodeItem(),
        new GenericItem(),new TextItem(),new MSBuildItem(),new BinaryItem(),new MultiMediaItem()})).IsTrue();

        Tool _1 = Tool.Parse(_0.Serialize(),null);

        Check.That(_0.Equals(_1));
    }

    [Test]
    public void SetApplication()
    {
        Tool _0 = new Tool();
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion()
    {
        Tool _0 = new Tool();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetBornOn()
    {
        Tool _0 = new Tool();
        DateTimeOffset _1 = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);

        Check.That(_0.GetBornOn()).HasAValue().And.IsNotEqualTo(_1);
        Check.That(_0.SetBornOn(_1)).IsTrue();
        Check.That(_0.GetBornOn()).Equals(_1);
    }

    [Test]
    public void SetCertificates()
    {
        Tool _0 = new Tool();
        Dictionary<String,String> _1 = new();
        _1.Add("Content 0","6131f9c300030000000f");
        _1.Add("Content 1","611e23c200030000000e");
        _1.Add("Extension ExtKey InnerKey1 DeeperKeyN 8","473d29a500030000000f");

        Check.That(_0.SetCertificates(_1)).IsTrue();
        Check.That(_0.GetCertificates()).Contains(_1);
        Check.That(_0.SetCertificates(new Dictionary<String,String>())).IsTrue();
        Check.That(_0.GetCertificates()).IsNull();
    }

    [Test]
    public void SetContainer()
    {
        Tool _0 = new Tool();
        IContainer _1 = new ContainerBuilder().Build();

        Check.That(_0.GetContainer()).IsNull();
        Check.That(_0.SetContainer(_1)).IsTrue();
        Check.That(_0.GetContainer()).Equals(_1);
    }

    [Test]
    public void SetControls()
    {
        Tool _0 = new Tool();
        List<String> _1 = new(){"SetControls","Exam","Pass","Exam"};

        Check.That(_0.SetControls(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetControls()).Contains(_1.Distinct());
        Check.That(_0.SetControls(new HashSet<String>())).IsTrue();
        Check.That(_0.GetControls()).IsNull();
    }

    [Test]
    public void SetDistinguishedName()
    {
        Tool _0 = new Tool();
        String _1 = "CN=ServiceN,OU=Operations,DC=Fabrikam,DC=NET";

        Check.That(_0.SetDistinguishedName(null)).IsFalse();
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void SetDomainID()
    {
        Tool _0 = new Tool();
        String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
    }

    [Test]
    public void SetExtension()
    {
        Tool _0 = new Tool();
        Dictionary<String,Object?> _1 = new Dictionary<String,Object?>();
        String _2 = "Pass";
        String _3 = "OK";
        Func<String> _4 = () => { return _3; };

        _1.Add("1",_2);
        _1.Add("2",_4);
        
        Check.That(_0.SetExtension(_1.ToImmutableDictionary())).IsTrue();
        Check.That((String?)_0.GetExtension()!["1"]).IsEqualTo(_2);
        Check.That(((Func<String>?)_0.GetExtension()!["2"])!.DynamicInvoke()).IsEqualTo(_3);
    }

    [Test]
    public void SetFILE()
    {
        Tool _0 = new Tool();
        String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
    }

    [Test]
    public void SetGPS()
    {
        Tool _0 = new Tool();
        String _1 = "SetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
    }

    [Test]
    public void SetID()
    {
        Tool _0 = new Tool();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.SetID(_1)).IsFalse();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void SetLinks()
    {
        Tool _0 = new Tool();
        Dictionary<String,GuidReferenceItem> _1 = new();
        GuidReferenceItem _2 = new GuidReferenceItem();
        GuidReferenceItem _3 = new GuidReferenceItem();
        GuidReferenceItem _4 = new GuidReferenceItem();
        _1.Add("_2",_2); _1.Add("_3",_3); _1.Add("_4",_4);

        Check.That(_0.SetLinks(null)).IsFalse();
        Check.That(_0.SetLinks(_1.ToImmutableDictionary())).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetLinks()).HasSize(3);
        Check.That(_0.GetLinks()?["_2"]).IsEqualTo(_2);
        Check.That(_0.GetLinks()?["_3"]).IsEqualTo(_3);
        Check.That(_0.GetLinks()?["_4"]).IsEqualTo(_4);
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsFalse();
        Check.That(_0.UnLock("key")).IsTrue();
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsTrue();
        Check.That(_0.GetLinks()).IsNull();
    }

    [Test]
    public void SetLocator()
    {
        Tool _0 = new Tool();
        String _1 = "app://server/SetLocatorExam";
        Uri _2 = new Uri(_1);

        Check.That(_0.SetLocator(null)).IsFalse();
        Check.That(_0.SetLocator(_2)).IsTrue();
        Check.That(_0.GetLocator()!.AbsoluteUri).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.SetLocator(_2)).IsFalse();
        Check.That(_0.UnLock("key")).IsTrue();
        Check.That(_0.SetLocator(new Uri("null:"))).IsTrue();
        Check.That(_0.GetLocator()).IsNull();
    }

    [Test]
    public void SetModified()
    {
        Tool _0 = new Tool();
        Tool _3 = new Tool();
        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_3.SetModified(_1)).IsTrue();
        Check.That(_0.GetModified()).IsNull();
        Check.That(_3.GetModified()).Equals(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_3.GetModified()).Equals(_1);
    }

    [Test]
    public void SetName()
    {
        Tool _0 = new Tool();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void SetObjectives()
    {
        Tool _0 = new Tool();
        String _1 = "SetObjectivesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(_0.SetObjectives(null)).IsFalse();
        Check.That(_0.SetObjectives(_2)).IsTrue();
        Check.That(_0.GetObjectives()).Contains(_2);
        Check.That(_0.GetObjectives()!.ToArray().First()).IsEqualTo(_1);
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.SetObjectives(_2)).IsFalse();
    }

    [Test]
    public void SetPolicies()
    {
        Tool _0 = new Tool();
        String _1 = "SetPoliciesExam";
        List<Object> _2 = new List<Object>();
        _2.Add(_1);

        Check.That(_0.SetPolicies(null)).IsFalse();
        Check.That(_0.SetPolicies(_2)).IsTrue();
        Check.That(_0.GetPolicies()).Contains(_2);
        Check.That(_0.GetPolicies()!.ToArray().First()).IsEqualTo(_1);
    }

    [Test]
    public void SetPurpose()
    {
        Tool _0 = new Tool();
        String _1 = "SetPurposeExam";

        Check.That(_0.SetPurpose(null)).IsFalse();
        Check.That(_0.SetPurpose(_1)).IsTrue();
        Check.That(_0.GetPurpose()).IsEqualTo(_1);
    }

    [Test]
    public void SetSecurityDescriptor()
    {
        Tool _0 = new Tool();
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void SetServiceVersion()
    {
        Tool _0 = new Tool();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetStatus()
    {
        Tool _0 = new();
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

    [Test]
    public void SetSubordinates()
    {
        Tool _0 = new Tool();
        Tool _1 = new Tool();
        Tool _2 = new Tool();
        HashSet<Tool> _3 = new HashSet<Tool>();
        _3.Add(_1); _3.Add(_2);

        Check.That(_0.SetSubordinates(null)).IsFalse();
        Check.That(_0.SetSubordinates(_3)).IsTrue();
        Check.That(_0.GetSubordinates()).Contains(_1,_2);
    }

    [Test]
    public void SetSuperior()
    {
        Tool _0 = new Tool();
        Tool _1 = new Tool();

        Check.That(_0.SetSuperior(null)).IsTrue();
        Check.That(_0.SetSuperior(_1)).IsTrue();
        Check.That(_0.GetSuperior()).IsSameReferenceAs(_1);
    }

    [Test]
    public void SetTelemetry()
    {
        Tool _0 = new Tool();
        List<String> _1 = new(){"SetTelemetry","Exam","Pass","Exam"};

        Check.That(_0.SetTelemetry(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTelemetry()).Contains(_1.Distinct());
        Check.That(_0.SetTelemetry(new HashSet<String>())).IsTrue();
        Check.That(_0.GetTelemetry()).IsNull();
    }

    [Test]
    public void SetVersion()
    {
        Tool _0 = new Tool();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void ToFile()
    {
        Byte[] _1 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_1);

        BinaryItem _2 = new BinaryItem(_1,null,String.Empty);

        Tool _3 = new Tool(new HashSet<DataItem>(){_2},Guid.NewGuid());

        String _4 = Environment.GetEnvironmentVariable("TEMP") + "\\Tool003.ktool";

        File.Delete(_4);

        Check.That(File.Exists(_4)).IsFalse();

        Check.That(_3.ToFile(_4)).IsTrue();

        Tool? _5 = Tool.FromFile(_4);

        Check.That(_5).IsNotNull();

        Check.That(_3.Equals(_5)).IsTrue();

        File.Delete(_4);

        Check.That(File.Exists(_4)).IsFalse();
    }

    [Test]
    public new void ToString()
    {
        Tool _0 = new Tool();
        Check.That(_0.AddDataItems(new HashSet<DataItem>{new GuidReferenceItem(),new CodeItem(),
        new GenericItem(),new TextItem(),new MSBuildItem(),new BinaryItem(),new MultiMediaItem()})).IsTrue();

        Tool _1 = Tool.Parse(_0.ToString(),null);

        Check.That(_0.Equals(_1));
    }

    [Test]
    public void TryParse()
    {
        TextItem _2 = new TextItem("TryParseExam",Guid.NewGuid());

        Tool _0 = new Tool(new HashSet<DataItem>(){_2},Guid.NewGuid());

        Tool? _1;

        Tool.TryParse(_0.ToString(),null,out _1);

        Check.That(_1).IsInstanceOfType(typeof(Tool));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        Tool _0 = new Tool();

        String _1 = "UnLockExam";

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(_1)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock("False")).IsFalse();

        Check.That(_0.UnLock(_1)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }

    [Test]
    public void UnRegisterCommand()
    {
        Tool _0 = new Tool();
        CommandTest _1 = new CommandTest();
        CommandTest _2 = new CommandTest();
        String _4 = "PlayVideo";
        String _5 = "StreamVideo";
        String _6 = "Vendor2,StreamVideo";
        String _7 = "VendorN,StreamVideoTo";

        Check.That(_0.RegisterCommand(_1,_4)).IsTrue();
        Check.That(_0.RegisterCommand(_1,_5)).IsTrue();
        Check.That(_0.RegisterCommand(_1,_6)).IsTrue();
        Check.That(_0.RegisterCommand(_2,_4)).IsFalse();
        Check.That(_0.RegisterCommand(_2,_7)).IsTrue();

        Check.That(_0.GetCommands()!.Count).IsEqualTo(4);
        Check.That(_0.GetCommands()!.Keys).ContainsExactly(new String[]{_4,_5,_6,_7});
        Check.That(_0.GetCommands()!.Values.Distinct()).ContainsExactly(new CommandTest[]{_1,_2});

        Check.That(_0.UnRegisterCommand(_5)).IsTrue();

        Check.That(_0.GetCommands()!.Keys).ContainsExactly(new String[]{_4,_6,_7});
        Check.That(_0.GetCommands()!.Values.Distinct()).ContainsExactly(new CommandTest[]{_1,_2});
    }

    [Test]
    public void UpdateInputs()
    {
        Tool _0 = new Tool();
        String _1 = "UpdateInputsExam";
        String _3 = "Pass";
        Queue<Object> _2 = new Queue<Object>();
        Queue<Object> _4 = new Queue<Object>();

        _2.Enqueue(_1);

        Check.That(_0.UpdateInputs(_2)).IsTrue();

        Check.That(_0.UpdateInputs(null)).IsFalse();
        Check.That(_0.GetInputs()).Contains(_2);
        Check.That(_0.GetInputs()!.Peek()).IsEqualTo(_1);

        _4.Enqueue(_1); _4.Enqueue(_3);

        Check.That(_0.UpdateInputs(_4)).IsTrue();
        Queue<Object>? _5 = _0.GetInputs();
        Check.That(_5?.Dequeue()).IsEqualTo(_1);
        Check.That(_0.UpdateInputs(_5)).IsTrue();
        Check.That(_0.GetInputs()!.Dequeue()).IsEqualTo(_3);
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.UpdateInputs(_4)).IsFalse();
        Check.That(_0.UnLock(_1)).IsTrue();
        Check.That(_0.UpdateInputs(new Queue<Object>())).IsTrue();
        Check.That(_0.GetInputs()).IsNull();
    }

    [Test]
    public void WriteEventLogsToFile()
    {
        Tool _0 = new(); String path = Environment.CurrentDirectory + @"\WriteEventLogsToFileTest.txt"; if(File.Exists(path)) { File.Delete(path); }

        Check.That(_0.LogEvent("EventData1")).IsTrue(); Check.That(_0.LogEvent("EventData2")).IsTrue(); Check.That(_0.LogEvent("EventData3")).IsTrue();
        Check.That(_0.LogEvent("EventData4")).IsTrue(); Check.That(_0.LogEvent("EventData5")).IsTrue(); Check.That(_0.LogEvent("EventData6")).IsTrue();

        Check.That(_0.WriteEventLogsToFile(path)).IsTrue();

        using(FileStream _1 = new FileStream(path,new FileStreamOptions(){Access = FileAccess.Read , Mode = FileMode.Open , Options = FileOptions.SequentialScan , Share = FileShare.Read}))
        {
            using(XmlDictionaryReader _2 = XmlDictionaryReader.CreateTextReader(_1,XmlDictionaryReaderQuotas.Max))
            {
                DataContractSerializer _3 = new DataContractSerializer(typeof(Dictionary<Int32,String>),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

                Check.That((Dictionary<Int32,String>?)_3.ReadObject(_2)).Contains(_0.GetEventLogs());
            }
        }

        File.Delete(path);
    }

    internal sealed class CommandTest : Command
    {
        ITool? AttachedTool; List<Activity> Activities = new List<Activity>(); Boolean Enabled;

        public override Boolean Attach(ITool tool) { this.AttachedTool = tool; return true; }

        public override Boolean Detach() { this.AttachedTool = null; return true; }

        public override Boolean Disable() { this.Enabled = false; return true; }

        public override Boolean Enable() { this.Enabled = true; return true; }

        public override Activity? ExecuteAsync(Object[]? parameter)
        {
            try
            {
                if( parameter is null || !this.Enabled ) { throw new InvalidOperationException(); }

                Guid? id = parameter[1] as Guid?; id = id is null || id.Equals(Guid.Empty) ? Guid.NewGuid() : id;

                if( !Double.TryParse(parameter[2].ToString()!,out Double x) || !Double.TryParse(parameter[3].ToString()!,out Double y) ) { throw new InvalidOperationException(); }

                Activity a = new Activity(); a.ID = id; a.Task = Task.Run<Object?>( () => { return this.Multiply(a,x,y); } ); this.Activities.Add(a); return a;
            }
            catch ( Exception ) { this.AttachedTool?.AddOutput(parameter?[1] as Guid? ?? Guid.NewGuid(),null); return null; }
        }

        private Double? Multiply(Activity a , Double x , Double y) { Double result = x * y; this.AttachedTool?.AddOutput(a.ID ?? Guid.NewGuid(),result); this.CleanUp(a); return result; }

        private void CleanUp(Activity a) { this.Activities.Remove(a); this.AttachedTool?.RemoveActivity(a); }

        public override Dictionary<String , String> GetSpecifications()
        {
            throw new NotImplementedException();
        }
    }

    internal sealed class TraitTest : Trait
    {
        public override Task<Boolean> Activate(CancellationToken? token)
        {
            throw new NotImplementedException();
        }

        public override Task<Boolean> Bind(Tool tool)
        {
            throw new NotImplementedException();
        }

        public override Task<Boolean> Deactivate(CancellationToken? token)
        {
            throw new NotImplementedException();
        }
    }
}