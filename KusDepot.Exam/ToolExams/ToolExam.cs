namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public partial class ToolExam
{
    private ManagerKey? KeyM;

    private OwnerKey? KeyO;

    [Test]
    public async Task Activate()
    {
        var _ = ToolBuilderFactory.CreateBuilder().RegisterCommand<CommandTestR>("Work").Build();

        Check.That(_.GetLifeCycleState()).Equals(LifeCycleState.UnInitialized);

        Check.That(await _.Activate()).IsTrue();

        Check.That(_.GetLifeCycleState()).Equals(LifeCycleState.Active);

        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).HasAValue();

        Check.That(await _.Deactivate()).IsTrue();

        Check.That(_.GetLifeCycleState()).Equals(LifeCycleState.InActive);

        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).HasNoValue();

        Check.That(await _.Activate()).IsTrue();

        Check.That(_.GetLifeCycleState()).Equals(LifeCycleState.Active);

        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).HasAValue();
    }

    [Test]
    public void AddActivity()
    {
        Tool _0 = new Tool();
        Activity _1 = new Activity(Task.FromResult<Object?>(true));
        Activity _2 = new Activity(Task.FromResult<Object?>(false));


        Check.That(_0.AddActivity(_1)).IsTrue();
        Check.That(_0.AddActivity(_2)).IsTrue();
        Check.That(_0.AddActivity(_2)).IsTrue();
        Check.That(_0.RemoveActivity(_2)).IsTrue();
        Check.That(_0.RemoveActivity(_2)).IsFalse();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.AddActivity(_2,new CommandKey(Array.Empty<Byte>()))).IsFalse();
        Check.That(_0.RemoveActivity(_2)).IsFalse();
    }

    [Test]
    public void AddDataItems()
    {
        HashSet<DataItem> _0  = new(new IDEquality());
        GuidReferenceItem _1  = new();
        GenericItem _2        = new();
        GenericItem _22       = new();
        TextItem _3           = new();
        MSBuildItem _4        = new();
        BinaryItem _5         = new();
        MultiMediaItem _6     = new();
        DataSetItem _18       = DataItemGenerator.CreateDataSet(40);
        CodeItem _16          = new();
        Tool _7               = new();
        List<Guid?> _8        = new();
        List<Guid?> _10       = new();
        HashSet<DataItem> _11 = new();
        List<Guid?> _12       = new();
        List<Guid?> _13       = new();

        _0.Add(_1);  _8.Add(_1.GetID());  _12.Add(_1.GetID());
        _0.Add(_2);  _8.Add(_2.GetID());  _12.Add(_2.GetID());
        _0.Add(_3);  _8.Add(_3.GetID());  _12.Add(_3.GetID());
        _0.Add(_4);  _8.Add(_4.GetID());  _12.Add(_4.GetID());
        _0.Add(_5);  _8.Add(_5.GetID());  _12.Add(_5.GetID());
        _0.Add(_16); _8.Add(_16.GetID()); _12.Add(_16.GetID());
        _0.Add(_18); _8.Add(_18.GetID()); _12.Add(_18.GetID());
        _0.Add(_22); _8.Add(_22.GetID()); _12.Add(_22.GetID());
        _7.AddDataItems(_0);

        HashSet<DataItem>? _9 = _7.GetDataItems();
        if(_9 is not null) { foreach(DataItem item in _9) { _10.Add(item.GetID()); } }

        _11.Add(_6); _12.Add(_6.GetID()); _7.AddDataItems(_11);

        HashSet<DataItem>? _14   = _7.GetDataItems();
        if(_14 is not null) { foreach(DataItem item in _14) { _13!.Add(item!.GetID()); } }

        Check.That(_7.AddDataItems(null)).IsFalse();
        Check.That(_10).ContainsExactly(_8.ToArray());
        Check.That(_13).ContainsExactly(_12.ToArray());
        Check.That(_7.AddDataItems(new DataItem[]{_5})).IsFalse();
        Check.That(_7.AddDataItems(new DataItem[]{new BinaryItem(id:Guid.Empty)})).IsFalse();
        GenericItem _15 = new();
        Check.That(_15.SetID(Guid.Empty)).IsTrue();
        Check.That(_15.GetID()).IsNull();
        Check.That(_7.AddDataItems(new DataItem[]{_15})).IsFalse();
        Check.That(_15.SetID(_16.GetID())).IsTrue();
        Check.That(_15.GetID()).Equals(_16.GetID());
        Check.That(_7.AddDataItems(new DataItem[]{_15})).IsFalse();
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
    public void AddInstance()
    {
        Tool _0 = new(); Guid? _1 = _0.GetID();

        Check.That(_0.AddInstance()).IsTrue();

        Check.That(Tool.GetInstanceIDs()).Contains(_1);
        Check.That(Tool.GetInstance(_1)).IsSameReferenceAs(_0);
        _0.Dispose();
        Check.That(Tool.GetInstanceIDs()).Not.Contains(_1);
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

    [OneTimeSetUp]
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }

        KeyM = new( new CertificateRequest("CN=Management",RSA.Create(4096),HashAlgorithmName.SHA512,RSASignaturePadding.Pss).CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1))! );

        KeyO = new( Utility.SerializeCertificate(new CertificateRequest("CN=Owner",RSA.Create(4096),HashAlgorithmName.SHA512,RSASignaturePadding.Pss).CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1)))! );

        if(KeyM is null || KeyO is null) { throw new InvalidOperationException(); }
    }

    [Test]
    public void CompareTo()
    {
        Tool _0 = new Tool(); Tool _1 = new Tool();

        Check.That(_0.CompareTo(_1)).IsNotZero();
    }

    [Test]
    public void Constructor()
    {
        HashSet<DataItem> _0 = new HashSet<DataItem>();
        GuidReferenceItem _1 = new GuidReferenceItem(Guid.NewGuid());
        GenericItem _2       = new GenericItem(content:[nameof(GenericItem)]);
        TextItem _3          = new TextItem(content:nameof(TextItem));
        MSBuildItem _4       = new MSBuildItem(content:nameof(MSBuildItem));
        BinaryItem _5        = new BinaryItem(content:nameof(BinaryItem).ToByteArrayFromUTF16String());
        MultiMediaItem _6    = new MultiMediaItem(content:nameof(MultiMediaItem).ToByteArrayFromUTF16String());
        CodeItem _7          = new CodeItem(content:nameof(CodeItem));
        DataSetItem _12      = DataItemGenerator.CreateDataSet(4);
        List<Guid?> _8       = new List<Guid?>();
        List<Guid?> _10      = new List<Guid?>();
        _0.Add(_1); _8.Add(_1.GetID());
        _0.Add(_2); _8.Add(_2.GetID());
        _0.Add(_3); _8.Add(_3.GetID());
        _0.Add(_4); _8.Add(_4.GetID());
        _0.Add(_5); _8.Add(_5.GetID());
        _0.Add(_6); _8.Add(_6.GetID());
        _0.Add(_7); _8.Add(_7.GetID());
        _0.Add(_12); _8.Add(_12.GetID());

        Guid _15 = Guid.NewGuid();
        Tool _99 = new Tool(data:_0,id:_15);
        HashSet<DataItem>? _9 = _99.GetDataItems();
        if(_9 is not null)
        {
            foreach(DataItem item in _9) { _10!.Add(item!.GetID()); }
        }

        Check.That(_99).IsInstanceOfType(typeof(Tool));
        Check.That(_10).ContainsExactly(_8.ToArray());
        Check.That(_99.GetID()).IsEqualTo(_15);
        Check.That(_99.GetDataItems()).ContainsExactly(_0);
    }

    [Test]
    public async Task Deactivate()
    {
        var _ = ToolBuilderFactory.CreateBuilder().RegisterCommand("Work",typeof(CommandTestR)).Build();

        Check.That(_.GetLifeCycleState()).Equals(LifeCycleState.UnInitialized);

        Check.That(await _.Deactivate()).IsFalse();

        Check.That(await _.Activate()).IsTrue();

        Check.That(_.GetLifeCycleState()).Equals(LifeCycleState.Active);

        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).HasAValue();

        Check.That(await _.Deactivate()).IsTrue();

        Check.That(_.GetLifeCycleState()).Equals(LifeCycleState.InActive);

        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).HasNoValue();

        Check.That(await _.Activate()).IsTrue();

        Check.That(_.GetLifeCycleState()).Equals(LifeCycleState.Active);

        Check.That(_.ExecuteCommand(new(){Handle = "Work"})).HasAValue();
    }

    [Test, Explicit, Category("Extended")]
    public void DisableKusDepotExceptions()
    {
        Tool _0 = new();

        Check.That(Settings.NoExceptions).IsFalse();

        Check.That(_0.DisableKusDepotExceptions()).IsTrue();

        Check.That(Settings.NoExceptions).IsTrue();

        Check.That(_0.EnableKusDepotExceptions()).IsTrue();

        Check.That(Settings.NoExceptions).IsFalse();
    }

    [Test]
    public void DisableMyExceptions()
    {
        Tool _ = new();

        Check.That(_.ExceptionsEnabled).IsFalse();

        Check.That(_.EnableMyExceptions()).IsTrue();

        Check.That(_.ExceptionsEnabled).IsTrue();

        Check.That(_.DisableMyExceptions()).IsTrue();

        Check.That(_.ExceptionsEnabled).IsFalse();
    }

    [Test]
    public void Dispose()
    {
        Tool _0 = new Tool(); var i = _0.GetID();

        Check.That(_0.GetID()).HasAValue();

        Check.That(_0.GetDisposed()).IsFalse();

        _0.Dispose();

        Check.That(_0.GetDisposed()).IsTrue();

        Check.That(Tool.GetInstanceIDs()).Not.Contains(i);
    }

    [Test]
    public async Task DisposeAsync()
    {
        Tool _0 = new Tool(); var i = _0.GetID();

        Check.That(_0.GetID()).HasAValue();

        Check.That(_0.GetDisposed()).IsFalse();

        await _0.DisposeAsync();

        Check.That(_0.GetDisposed()).IsTrue();

        Check.That(Tool.GetInstanceIDs()).Not.Contains(i);
    }

    [Test, Explicit, Category("Extended")]
    public void EnableKusDepotExceptions()
    {
        Tool _0 = new();

        Check.That(Settings.NoExceptions).IsFalse();

        Check.That(_0.DisableKusDepotExceptions()).IsTrue();

        Check.That(Settings.NoExceptions).IsTrue();

        Check.That(_0.EnableKusDepotExceptions()).IsTrue();

        Check.That(Settings.NoExceptions).IsFalse();
    }

    [Test]
    public void EnableMyExceptions()
    {
        Tool _ = new();

        Check.That(_.ExceptionsEnabled).IsFalse();

        Check.That(_.EnableMyExceptions()).IsTrue();

        Check.That(_.ExceptionsEnabled).IsTrue();

        Check.That(_.DisableMyExceptions()).IsTrue();

        Check.That(_.ExceptionsEnabled).IsFalse();
    }

    [Test]
    public void opEquality()
    {
        Tool _0 = new Tool();
        Tool _1 = new Tool();

        Check.That(_1.SetID(Guid.Empty)).IsTrue();
        Check.That(_1.GetID()).IsNull();

        Check.That(new Tool() == null).IsFalse();
        Check.That(new Tool() == _1).IsFalse();
        Check.That(_1.SetID(_0.GetID())).IsTrue();
        Check.That(_0 == _1).IsTrue();
    }

    [Test]
    public void opInequality()
    {
        Tool _0 = new Tool();
        Tool _1 = new Tool();

        Check.That(_1.SetID(Guid.Empty)).IsTrue();
        Check.That(_1.GetID()).IsNull();

        Check.That(new Tool() != null).IsTrue();
        Check.That(new Tool() != _1).IsTrue();
        Check.That(_1.SetID(_0.GetID())).IsTrue();
        Check.That(_0 != _1).IsFalse();
    }

    [Test]
    public void EqualsObject()
    {
        Tool _ = new();

        Check.That((Object)new Tool().Equals(null)).IsEqualTo(false);
        Check.That((Object)new Tool().Equals(new Object())).IsEqualTo(false);
        Check.That((Object)_.Equals(_)).IsEqualTo(true);
    }

    [Test]
    public void EqualsInterface()
    {
        Tool _0 = new Tool();
        Tool _1 = new Tool();

        Check.That(_1.SetID(Guid.Empty)).IsTrue();
        Check.That(_1.GetID()).IsNull();

        Check.That(new Tool().Equals(null)).IsFalse();
        Check.That(new Tool().Equals(new Object())).IsFalse();
        Check.That(new Tool().Equals(_1)).IsFalse();
        Check.That(_1.SetID(_0.GetID())).IsTrue();
        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void ExceptionsEnabled()
    {
        Tool _ = new();

        Check.That(_.ExceptionsEnabled).IsFalse();

        Check.That(_.EnableMyExceptions()).IsTrue();

        Check.That(_.ExceptionsEnabled).IsTrue();

        Check.That(_.DisableMyExceptions()).IsTrue();

        Check.That(_.ExceptionsEnabled).IsFalse();
    }

    [Test]
    public async Task GetCommandTypes()
    {
        Tool _0 = new Tool();
        CommandTest _1 = new CommandTest();
        String _2 = "GetCommandTypesExam";

        Check.That(_0.MaskCommandTypes(false)).IsTrue();
        Check.That(_0.GetCommandTypes()).IsNull();
        Check.That(await _0.RegisterCommand(_2,_1)).IsTrue();
        Check.That(_0.GetCommandTypes()?.Count).Equals(1);
        Check.That(_0.GetCommandTypes()?[_2]).Equals(_1.GetType().FullName!);;
        Check.That(await _0.UnRegisterCommand(_2)).IsTrue();
        Check.That(_0.GetCommandTypes()).IsNull();
    }

    [Test]
    public void GetConfiguration()
    {
        IToolBuilder tb = ToolBuilderFactory.CreateBuilder();

        IConfigurationRoot c = new ConfigurationBuilder().Build();

        tb.ConfigureToolConfiguration((x,b) => { b.AddConfiguration(c); });

        ITool _ = tb.Build(); Check.That(_.GetConfiguration()).IsNotNull();
    }

    [Test]
    public void GetDataDescriptors()
    {
        HashSet<DataItem>? _0 = new();
        GuidReferenceItem _1  = new();
        GenericItem _2        = new();
        TextItem _3           = new();
        MSBuildItem _4        = new();
        CodeItem _14          = new();
        BinaryItem _5         = new();
        MultiMediaItem _6     = new();
        Tool _7               = new();

        _0.Add(_1);
        _0.Add(_2);
        _0.Add(_3);
        _0.Add(_4);
        _0.Add(_14);
        _0.Add(_5);
        _0.Add(_6);

        List<Descriptor> _d1 = _0.Select(_ => _.GetDescriptor()!).ToList();

        Check.That(_7.AddDataItems(_0)).IsTrue();

        IList<Descriptor>? _d2 = _7.GetDataDescriptors();

        Check.That(_d1).ContainsExactly(_d2);
    }

    [Test]
    public void GetDataItem()
    {
        Guid _      = Guid.NewGuid();
        Tool _0     = new();
        CodeItem _1 = new(content:"GetDataItemExam",id:_);
        TextItem _2 = new(content:"GetDataItemExam");

        AccessKey? k = _0.RequestAccess(new ServiceRequest(new Tool(),""));

        Check.That(k).IsNotNull();

        _0.AddDataItems(new DataItem[]{_1,_2});

        Check.That(_0.GetDataItems()).ContainsExactly(new DataItem[]{_1,_2});

        Check.That(_0.GetDataItem(_)).Equals(_1);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetDataItems(k)).ContainsExactly(new DataItem[]{_1,_2});

        Check.That(_0.GetDataItem(_,k)).Equals(_1);
    }

    [Test]
    public void GetDataItems()
    {
        HashSet<DataItem>? _0 = new();
        GuidReferenceItem _1  = new();
        GenericItem _2        = new();
        TextItem _3           = new();
        MSBuildItem _4        = new();
        CodeItem _14          = new();
        BinaryItem _5         = new();
        MultiMediaItem _6     = new();
        Tool _7               = new();
        List<Guid?> _8        = new();
        List<Guid?> _10       = new();
        List<Guid?> _11       = new();
        _0.Add(_1);  _8.Add(_1.GetID());
        _0.Add(_2);  _8.Add(_2.GetID());
        _0.Add(_3);  _8.Add(_3.GetID());
        _0.Add(_4);  _8.Add(_4.GetID());
        _0.Add(_14); _8.Add(_14.GetID());
        _0.Add(_5);  _8.Add(_5.GetID());
        _0.Add(_6);  _8.Add(_6.GetID());

        _7.AddDataItems(_0); HashSet<DataItem>? _9 = _7.GetDataItems();

        if(_9 is not null) { foreach(DataItem item in _9) { _10!.Add(item!.GetID()); } }

        Check.That(_10).ContainsExactly(_8.ToArray());

        AccessKey? k = _7.RequestAccess(new ServiceRequest(new Tool(),""));

        Check.That(k).IsNotNull();

        Check.That(_7.Lock(KeyM)).IsTrue();

        HashSet<DataItem>? _12 = _7.GetDataItems(k);

        if(_12 is not null) { foreach(DataItem item in _12) { _11!.Add(item!.GetID()); } }

        Check.That(_11).ContainsExactly(_8.ToArray());
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
    public void GetHash()
    {
        Tool _0 = new Tool();
        Tool _1 = new Tool();

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_0.GetHashCode()).IsNotEqualTo(_1.GetHashCode());
        Check.That(_1.SetID(_0.GetID())).IsTrue();
        Check.That(_0.GetHashCode()).IsEqualTo(_1.GetHashCode());
    }

    [Test]
    public void GetHost()
    {
        var _ = new ToolHost3();
    }

    [Test]
    public void GetID()
    {
        Tool _0 = new Tool();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.GetID()).Not.Equals(_1);
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
    public void GetInstance()
    {
        Tool _0 = new(); Guid? _1 = _0.GetID();

        Check.That(_0.AddInstance()).IsTrue();

        Check.That(Tool.GetInstanceIDs()).Contains(_1);
        Check.That(Tool.GetInstance(_1)).IsSameReferenceAs(_0);
        _0.Dispose();
        Check.That(Tool.GetInstanceIDs()).Not.Contains(_1);
    }

    [Test]
    public async Task GetInstanceIDs()
    {
        Tool _0 = new(); Guid? _1 = _0.GetID();

        Tool _2 = new(); Guid? _8 = _2.GetID();

        Check.That(_0.AddInstance()).IsTrue();
        Check.That(_2.AddInstance()).IsTrue();

        Check.That(Tool.GetInstanceIDs()).Contains(_1);
        Check.That(Tool.GetInstance(_1)).IsSameReferenceAs(_0);
        _0.Dispose();
        Check.That(Tool.GetInstanceIDs()).Not.Contains(_1);

        Check.That(Tool.GetInstanceIDs()).Contains(_8);
        Check.That(Tool.GetInstance(_8)).IsSameReferenceAs(_2);
        await _2.DisposeAsync();
        Check.That(Tool.GetInstanceIDs()).Not.Contains(_8);

        Tool _3 = new();

        Check.That(Tool.GetInstanceIDs()).Not.Contains(_3.GetID());

        Check.That(_3.AddInstance()).IsTrue();

        Check.That(Tool.GetInstanceIDs()).Contains(_3.GetID());
    }

    [Test]
    public void GetOutput()
    {
        Tool _0 = new Tool();
        String _1 = "GetOutputExam";
        Guid _2 = Guid.NewGuid();

        AccessKey? k = _0.RequestAccess(new StandardRequest("GetOutput"));

        Check.That(_0.GetOutput(Guid.Empty)).IsNull();
        Check.That(_0.AddOutput(_2,_1)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetOutput(_2)).IsNull();
        Check.That(_0.GetOutput(_2,k)).Equals(_1);
    }

    [Test]
    public async Task GetOutputAsync()
    {
        Tool _0 = new Tool();
        String _1 = "GetOutputAsyncExam";
        Guid _2 = Guid.NewGuid();

        AccessKey? k = _0.RequestAccess(new StandardRequest("GetOutputAsync"));
        Check.That(k).IsNotNull();
        Check.That(k).IsInstanceOf<ClientKey>();

        Check.That(_0.GetOutput(Guid.Empty)).IsNull();
        Check.That(_0.AddOutput(_2,_1)).IsTrue();
        Check.That(_0.GetOutputIDs()!.Count).IsGreaterOrEqualThan(1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(await _0.GetOutputAsync(_2)).IsNull();
        Check.That(await _0.GetOutputAsync(_2,key:k)).Equals(_1);
    }

    [Test]
    public void GetRemoveOutput()
    {
        Tool _0 = new Tool();
        String _1 = "GetRemoveOutput";
        Guid _2 = Guid.NewGuid();

        AccessKey? kc = _0.RequestAccess(new StandardRequest("GetRemoveOutput"));
        Check.That(kc).IsNotNull();
        Check.That(kc).IsInstanceOf<ClientKey>();

        AccessKey? ks = _0.RequestAccess(new ServiceRequest(new Tool(),"GetRemoveOutput"));
        Check.That(ks).IsNotNull();
        Check.That(ks).IsInstanceOf<ServiceKey>();

        Check.That(_0.GetOutput(Guid.Empty)).IsNull();
        Check.That(_0.AddOutput(_2,_1)).IsTrue();
        Check.That(_0.GetOutputIDs()!.Count).IsGreaterOrEqualThan(1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetRemoveOutput(_2,key:kc)).IsNull();
        Check.That(_0.GetRemoveOutput(_2,key:ks)).Equals(_1);
        Check.That(_0.GetOutput(_2,kc)).IsNull();
        Check.That(_0.GetOutput(_2,ks)).IsNull();
    }

    [Test]
    public async Task GetRemoveOutputAsync()
    {
        Tool _0 = new Tool();
        String _1 = "GetRemoveOutputAsync";
        Guid _2 = Guid.NewGuid();

        AccessKey? kc = _0.RequestAccess(new StandardRequest("GetRemoveOutputAsync"));
        Check.That(kc).IsNotNull();
        Check.That(kc).IsInstanceOf<ClientKey>();

        AccessKey? ks = _0.RequestAccess(new ServiceRequest(new Tool(),"GetRemoveOutputAsync"));
        Check.That(ks).IsNotNull();
        Check.That(ks).IsInstanceOf<ServiceKey>();

        Check.That(_0.GetOutput(Guid.Empty)).IsNull();
        Check.That(_0.AddOutput(_2,_1)).IsTrue();
        Check.That(_0.GetOutputIDs()!.Count).IsGreaterOrEqualThan(1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(await _0.GetRemoveOutputAsync(_2,key:kc)).IsNull();
        Check.That(await _0.GetRemoveOutputAsync(_2,key:ks)).Equals(_1);
        Check.That(_0.GetOutput(_2,kc)).IsNull();
        Check.That(_0.GetOutput(_2,ks)).IsNull();
    }

    [Test]
    public void GetOutputIDs()
    {
        Tool _0 = new Tool();
        String _1 = "GetOutputIDs"; Guid _11 = Guid.NewGuid();
        
        AccessKey? k = _0.RequestAccess(new StandardRequest("GetOutputIDs"));
        Check.That(k).IsInstanceOf<ClientKey>();

        Check.That(_0.GetOutputIDs()).IsNull();
        Check.That(_0.AddOutput(_11,_1)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetOutputIDs(key:k)).Contains(_11);
    }

    [Test]
    public void GetStatus()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetStatus()).IsNull();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetStatus()).IsNull();
    }

    [Test]
    public void GetToolServiceScope()
    {
        IToolBuilder tb = ToolBuilderFactory.CreateBuilder();

        tb.ConfigureServices((_,s) => { s.AddSingleton<Tool001>(); });

        Tool000 _ = tb.Build<Tool000>();

        Check.That(_.GetToolServiceScope()!.GetType().IsAssignableTo(typeof(IServiceScope))).IsTrue();
    }

    [Test]
    public void GetWorkingSet()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetWorkingSet()).IsNotNull();
    }

    [Test]
    public void Initialize()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetID()).HasAValue();
        Check.That(_0.SetID(Guid.Empty)).IsTrue();
        Check.That(_0.GetID()).IsNull();
        Check.That(_0.Initialize()).IsTrue();
        Check.That(_0.GetID()).HasAValue();
    }

    [Test, Explicit, Category("Extended")]
    public void KusDepotExceptionsEnabled()
    {
        Tool _0 = new();

        Check.That(_0.KusDepotExceptionsEnabled()).IsTrue();

        Check.That(_0.DisableKusDepotExceptions()).IsTrue();

        Check.That(_0.KusDepotExceptionsEnabled()).IsFalse();

        Check.That(_0.EnableKusDepotExceptions()).IsTrue();

        Check.That(_0.KusDepotExceptionsEnabled()).IsTrue();
    }

    [Test]
    public void MyExceptionsEnabled()
    {
        Tool _ = new();

        Check.That(_.MyExceptionsEnabled()).IsFalse();

        Check.That(_.EnableMyExceptions()).IsTrue();

        Check.That(_.MyExceptionsEnabled()).IsTrue();

        Check.That(_.DisableMyExceptions()).IsTrue();

        Check.That(_.MyExceptionsEnabled()).IsFalse();
    }

    [Test]
    public void ParameterlessConstructor()
    {
        Tool _0 = new Tool();

        Check.That(_0.GetCommandTypes()).IsNull();
        Check.That(_0.GetToolServiceScope()).IsNull();
        Check.That(_0.GetDataItems()).IsNull();
        Check.That(_0.GetDisposed()).IsFalse();
        Check.That(_0.GetLifeCycleState().Equals(LifeCycleState.UnInitialized));
        Check.That(_0.GetID()).IsNotNull();
        Check.That(_0.GetInputs()).IsNull();
        Check.That(_0.GetLocked()).IsFalse();
        Check.That(_0.GetOutputIDs()).IsNull();
        Check.That(_0.GetStatus()).IsNull();
    }

    [Test]
    public async Task RegisterCommand()
    {
        Tool _0 = new Tool();
        CommandTest _1 = new CommandTest();
        CommandTest _2 = new CommandTest();
        String _4 = "PlayVideo";
        String _5 = "StreamVideo";
        String _6 = "Vendor2,StreamVideo";
        String _7 = "VendorN,StreamVideoTo";
        String _8 = "PlayVideoDynamic";
        String _9 = "StreamVideoDynamic";

        Check.That(await _0.RegisterCommand(_4,_1)).IsTrue();
        Check.That(await _0.RegisterCommand(_5,_1)).IsTrue();
        Check.That(await _0.RegisterCommand(_6,_1)).IsTrue();
        Check.That(await _0.RegisterCommand(_4,_2)).IsFalse();
        Check.That(await _0.RegisterCommand(_7,_2)).IsTrue();

        Check.That(_0.MaskCommandTypes(false)).IsTrue();
        Check.That(_0.GetCommandTypes()!.Count).IsEqualTo(4);
        Check.That(_0.GetCommandTypes()!.Keys).ContainsExactly(new String[]{_4,_5,_6,_7});
        Check.That(_0.GetCommandTypes()!.Values.Distinct()).ContainsExactly(_1.GetType().FullName!);

        Check.That(await _0.Activate()).IsTrue();
        Check.That(_0.GetLifeCycleState()).Equals(LifeCycleState.Active);

        var _10 = new CommandTest(); var _11 = new CommandTest();

        Check.That(await _0.RegisterCommand(_8,_10,true)).IsTrue();
        Check.That(await _0.RegisterCommand(_9,_11,true)).IsTrue();

        Check.That(_10.GetLocked()).IsTrue(); Check.That(_11.GetLocked()).IsTrue();

        Check.That(_0.GetCommandTypes()!.Count).IsEqualTo(6);
    }

    [Test]
    public void RemoveActivity()
    {
        Tool _0 = new Tool();
        Activity _1 = new Activity(Task.FromResult<Object?>(true));
        Activity _2 = new Activity(Task.FromResult<Object?>(false));

        
        Check.That(_0.AddActivity(_1)).IsTrue();
        Check.That(_0.AddActivity(_2)).IsTrue();        
        Check.That(_0.AddActivity(_2)).IsTrue();        
        Check.That(_0.RemoveActivity(_2)).IsTrue();        
        Check.That(_0.RemoveActivity(_2)).IsFalse();        
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
    public void RemoveInstance()
    {
        Tool _0 = new(); Guid? _1 = _0.GetID();

        Check.That(_0.AddInstance()).IsTrue();

        Check.That(Tool.GetInstanceIDs()).Contains(_1);
        Check.That(Tool.GetInstance(_1)).IsSameReferenceAs(_0);
        _0.Dispose();
        Check.That(Tool.GetInstanceIDs()).Not.Contains(_1);
    }

    [Test]
    public void RemoveOutput()
    {
        Tool _0 = new Tool();
        String _1 = "RemoveOutputExam";
        Guid _2 = Guid.NewGuid();

        AccessKey? kc = _0.RequestAccess(new StandardRequest("RemoveOutput"));

        AccessKey? ks = _0.RequestAccess(new ServiceRequest(new Tool(),"RemoveOutput"));

        Check.That(_0.GetOutput(Guid.Empty)).IsNull();
        Check.That(_0.AddOutput(_2,_1)).IsTrue();
        Check.That(_0.RemoveOutput(Guid.Empty)).IsFalse();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.RemoveOutput(_2)).IsFalse();
        Check.That(_0.RemoveOutput(_2,kc)).IsFalse();
        Check.That(_0.GetOutput(_2)).IsNull();
        Check.That(_0.GetOutput(_2,kc)).Equals(_1);
        Check.That(_0.UnLock(KeyM)).IsTrue();
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
    public void SetID()
    {
        Tool _0 = new Tool();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetID(_1)).IsFalse();
        Check.That(_0.GetID()).Equals(_1);
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
    public new void ToString()
    {
        Tool _0 = new Tool();

        Check.That(_0.ToString()).IsNotNull();
    }

    [Test]
    public async Task UnRegisterCommand()
    {
        Tool _0 = new Tool();
        CommandTest _1 = new CommandTest();
        CommandTest _2 = new CommandTest();
        String _4 = "PlayVideo";
        String _5 = "StreamVideo";
        String _6 = "Vendor2,StreamVideo";
        String _7 = "VendorN,StreamVideoTo";

        Check.That(await _0.RegisterCommand(_4,_1)).IsTrue();
        Check.That(await _0.RegisterCommand(_5,_1)).IsTrue();
        Check.That(await _0.RegisterCommand(_6,_1)).IsTrue();
        Check.That(await _0.RegisterCommand(_4,_2)).IsFalse();
        Check.That(await _0.RegisterCommand(_7,_2)).IsTrue();
        Check.That(_0.MaskCommandTypes(false)).IsTrue();

        Check.That(await _0.Activate()).IsTrue();

        Check.That(_0.GetCommandTypes()!.Count).IsEqualTo(4);
        Check.That(_0.GetCommandTypes()!.Keys).ContainsExactly(new String[]{_4,_5,_6,_7});
        Check.That(_0.GetCommandTypes()!.Values.Distinct()).ContainsExactly(_1.GetType().FullName!);

        Check.That(await _0.UnRegisterCommand(_5)).IsTrue();

        Check.That(_0.GetCommandTypes()!.Count).IsEqualTo(3);
        Check.That(_0.GetCommandTypes()!.Keys).ContainsExactly(new String[]{_4,_6,_7});
        Check.That(_0.GetCommandTypes()!.Values.Distinct()).ContainsExactly(_2.GetType().FullName!);
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
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.UpdateInputs(_4)).IsFalse();
        Check.That(_0.UnLock(KeyM)).IsTrue();
        Check.That(_0.UpdateInputs(new Queue<Object>())).IsTrue();
        Check.That(_0.GetInputs()).IsNull();
    }
}