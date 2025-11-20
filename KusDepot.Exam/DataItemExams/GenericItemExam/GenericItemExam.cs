namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class GenericItemExam
{
    private ManagerKey? KeyM;

    [Test]
    public void AddNotes()
    {
        GenericItem _0 = new GenericItem();
        HashSet<String> _1 = new(){"AddNotesExam"};
        HashSet<String> _2 = new(){"Pass",String.Empty};
        HashSet<String> _3 = new(); _3.UnionWith(_1); _3.UnionWith(_2);

        Check.That(_0.AddNotes(null)).IsFalse();
        Check.That(_0.GetNotes()).IsNull();
        Check.That(_0.AddNotes(_1)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
        Check.That(_0.AddNotes(_2)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_3);
        Check.That(_0.AddNotes(new HashSet<String>())).IsTrue();
    }

    [Test]
    public void AddTags()
    {
        GenericItem _0 = new GenericItem();
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
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }

        KeyM = new( Utility.SerializeCertificate(new CertificateRequest("CN=Management",RSA.Create(4096),HashAlgorithmName.SHA512,RSASignaturePadding.Pss).CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1)))! );        

        if(KeyM is null) { throw new InvalidOperationException(); }
    }

    [Test]
    public void Clone()
    {
        GenericItem _0 = new GenericItem();
        GenericItem _1 = new GenericItem(content:[Double.PositiveInfinity]);

        Check.That(_0.Equals(_0.Clone())).IsFalse();
        Check.That(_0.Equals(_1.Clone())).IsFalse();
        Check.That(_1.Equals(_1.Clone())).IsTrue();
    }

    [Test]
    public void CompareTo()
    {
        GenericItem _0 = new GenericItem();
        Thread.Sleep(100);
        GenericItem _1 = new GenericItem();

        Check.That(new GenericItem().CompareTo(null)).IsEqualTo(1);
        Check.That(_0.CompareTo(_0)).IsEqualTo(0);
        Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
        Check.That(_1.CompareTo(_0)).IsStrictlyPositive();

        Check.That(_1.SetBornOn(DateTimeOffset.MinValue)).IsTrue();
        Check.That(_1.GetBornOn()).IsNull();
        Check.That(_1.CompareTo(null)).IsEqualTo(0);
        Check.That(_1.CompareTo(_0)).IsStrictlyNegative();
        Check.That(_0.CompareTo(_1)).IsStrictlyPositive();
        Check.That(_0.SetBornOn(DateTimeOffset.MinValue)).IsTrue();
        Check.That(_0.GetBornOn()).IsNull();
        Check.That(_0.CompareTo(null)).IsEqualTo(0);
        Check.That(_1.CompareTo(_0)).IsEqualTo(0);
    }

    [Test]
    public void Constructor()
    {
        GenericItem _0 = new GenericItem(content:new List<Object>(),id:Guid.NewGuid(),name:String.Empty,notes:new HashSet<String>(),tags:new HashSet<String>(),type:DataType.SGML);

        Check.That(_0).IsInstanceOfType(typeof(GenericItem));
    }

    [Test]
    public void opEquality()
    {
        GuidReferenceItem _7 = new GuidReferenceItem(Guid.NewGuid());
        List<Object>? _4 = new List<Object>(){"EqualsInterfaceExam",new Byte[3]{0x5,0xA,0x4},_7,new Char[3]{'c','a','t'}};
        List<Object>? _5 = new List<Object>(){new Byte[3]{0x5,0xA,0x4},"EqualsInterfaceExam",new Char[3]{'c','a','t'},_7};
        List<Object>? _8 = new List<Object>(){new Byte[3]{0xF,0xF,0xF},"EqualsInterfaceExam",_7,new Char[3]{'r','a','t'}};
        List<Object>? _10 = new List<Object>(){new Byte[3]{0xF,0xF,0xF},"EqualsInterfaceExam",_7,new Char[3]{'r','a','t'}};

        GenericItem _0 = new(_4);
        GenericItem _6 = new(_4);
        GenericItem _1 = new(_5);
        GenericItem _9 = new(_8);
        GenericItem _11 = new(_8);
        GenericItem _2 = new();
        GenericItem _3 = new(content:null,id:null,name:String.Empty);

        Check.That(new GenericItem() == null).IsFalse();
        Check.That(new GenericItem() == _3).IsFalse();
        Check.That(_0 == _0).IsTrue();
        Check.That(_0 == _1).IsTrue();
        Check.That(_0 == _2).IsFalse();
        Check.That(_3 == _0).IsFalse();
        Check.That(_0 == _6).IsTrue();
        Check.That(_9 == _6).IsFalse();
        Check.That(_11 == _9).IsTrue();
    }

    [Test]
    public void opInequality()
    {
        GuidReferenceItem _7 = new GuidReferenceItem(Guid.NewGuid());
        List<Object>? _4 = new List<Object>(){"EqualsInterfaceExam",new Byte[3]{0x5,0xA,0x4},_7,new Char[3]{'c','a','t'}};
        List<Object>? _5 = new List<Object>(){new Byte[3]{0x5,0xA,0x4},"EqualsInterfaceExam",new Char[3]{'c','a','t'},_7};
        List<Object>? _8 = new List<Object>(){new Byte[3]{0xF,0xF,0xF},"EqualsInterfaceExam",_7,new Char[3]{'r','a','t'}};
        List<Object>? _10 = new List<Object>(){new Byte[3]{0xF,0xF,0xF},"EqualsInterfaceExam",_7,new Char[3]{'r','a','t'}};

        GenericItem _0 = new GenericItem(_4);
        GenericItem _6 = new GenericItem(_4);
        GenericItem _1 = new GenericItem(_5);
        GenericItem _9 = new GenericItem(_8);
        GenericItem _11 = new GenericItem(_8);
        GenericItem _2 = new GenericItem();
        GenericItem _3 = new GenericItem(content:null,id:null,name:String.Empty);

        Check.That(new GenericItem() != null).IsTrue();
        Check.That(new GenericItem() != _3).IsTrue();
        Check.That(_0 != _0).IsFalse();
        Check.That(_0 != _1).IsFalse();
        Check.That(_0 != _2).IsTrue();
        Check.That(_3 != _0).IsTrue();
        Check.That(_0 != _6).IsFalse();
        Check.That(_9 != _6).IsTrue();
        Check.That(_11 != _9).IsFalse();
    }

    [Test]
    public void EqualsObject()
    {
        GenericItem _0 = new GenericItem();
        GenericItem _1 = new GenericItem();

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsFalse();
        Check.That(((Object)new GenericItem()).Equals(null)).IsFalse();
        Check.That(((Object)new GenericItem()).Equals(new Object())).IsFalse();
    }


    [Test]
    public void EqualsInterface()
    {
        GuidReferenceItem _7 = new GuidReferenceItem(Guid.NewGuid());
        List<Object>? _4 = new List<Object>(){"EqualsInterfaceExam",new Byte[3]{0x5,0xA,0x4},_7,new Char[3]{'c','a','t'}};
        List<Object>? _5 = new List<Object>(){new Byte[3]{0x5,0xA,0x4},"EqualsInterfaceExam",new Char[3]{'c','a','t'},_7};
        List<Object>? _8 = new List<Object>(){new Byte[3]{0xF,0xF,0xF},"EqualsInterfaceExam",_7,new Char[3]{'r','a','t'}};
        List<Object>? _10 = new List<Object>(){new Byte[3]{0xF,0xF,0xF},"EqualsInterfaceExam",_7,new Char[3]{'r','a','t'}};

        GenericItem _0 = new(_4);
        GenericItem _6 = new(_4);
        GenericItem _1 = new(_5);
        GenericItem _9 = new(_8);
        GenericItem _11 = new(_8);
        GenericItem _2 = new();
        GenericItem _3 = new(content:null,id:null,name:String.Empty);

        Check.That(new GenericItem().Equals(null)).IsFalse();
        Check.That(new GenericItem().Equals(new Object())).IsFalse();
        Check.That(new GenericItem().Equals(_3)).IsFalse();
        Check.That(_0.Equals(_0)).IsTrue();
        Check.That(_0.Equals(_1)).IsTrue();
        Check.That(_0.Equals(_2)).IsFalse();
        Check.That(_3.Equals(_0)).IsFalse();
        Check.That(_0.Equals(_6)).IsTrue();
        Check.That(_9.Equals(_6)).IsFalse();
        Check.That(_11.Equals(_9)).IsTrue();
    }

    [Test]
    public void FromFile()
    {
        Byte[] _1 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_1);

        GenericItem _2 = new GenericItem(new List<Object>{_1});

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\GenericItem.kdi";

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();

        Check.That(_2.ToFile(_3)).IsTrue();

        GenericItem? _4 = GenericItem.FromFile(_3);

        Check.That(_4).IsNotNull();

        Check.That(_2.Equals(_4)).IsTrue();

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();
    }

    [Test]
    public void GetApplication()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        GenericItem _0 = new GenericItem();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetBornOn()
    {
        GenericItem _0 = new GenericItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public async Task GetContent()
    {
        GenericItem _0 = new GenericItem();
        Guid _1 = Guid.NewGuid();
        List<Object> _2 = new List<Object>(){_1};

        Check.That(_0.SetContent(_2)).IsTrue();
        Check.That(_0.GetContent()!.First()!).IsEqualTo(_1);
        var dc1 = await _0.GetDataContent();
        Check.That(dc1!.Generic!.First()!).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetContent(_2)).IsFalse();
        Check.That(_0.GetContent()!.First()!).IsEqualTo(_1);
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        var dc2 = await _0.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _0.GetDataContent(KeyM);
        Check.That(dc3!.Generic!.First()!).IsEqualTo(_1);
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()!.First()!).IsEqualTo(_1);
    }

    [Test]
    public async Task GetDataContent()
    {
        GenericItem _0 = new GenericItem();
        Guid _1 = Guid.NewGuid();
        List<Object> _2 = new List<Object>(){_1};

        Check.That(_0.SetContent(_2)).IsTrue();
        Check.That(_0.GetContent()!.First()!).IsEqualTo(_1);
        var dc1 = await _0.GetDataContent();
        Check.That(dc1!.Generic!.First()!).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetContent(_2)).IsFalse();
        Check.That(_0.GetContent()!.First()!).IsEqualTo(_1);
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        var dc2 = await _0.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _0.GetDataContent(KeyM);
        Check.That(dc3!.Generic!.First()!).IsEqualTo(_1);
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()!.First()!).IsEqualTo(_1);
    }

    [Test]
    public void GetDescriptor()
    {
        GenericItem _0 = new GenericItem();

        String _s = "GetDescriptorExam";
        Version _v = new Version("1.2.3.4");
        DateTimeOffset? _d = _0.GetModified();
        HashSet<String> _n = new(){"Get","Descriptor","Exam","Notes"};
        HashSet<String> _t = new(){"Get","Descriptor","Exam","Tags"};

        Check.That(_0.SetApplication(_s)).IsTrue();
        Check.That(_0.SetApplicationVersion(_v)).IsTrue();
        Check.That(_0.SetDistinguishedName(_s)).IsTrue();
        Check.That(_0.SetModified(_d)).IsTrue();
        Check.That(_0.SetName(_s)).IsTrue();
        Check.That(_0.AddNotes(_n)).IsTrue();
        Check.That(_0.SetServiceVersion(_v)).IsTrue();
        Check.That(_0.AddTags(_t)).IsTrue();
        Check.That(_0.SetType(DataType.DIT)).IsTrue();
        Check.That(_0.SetVersion(_v)).IsTrue();

        Descriptor? _ = _0.GetDescriptor();

        Check.That(_!.Application).IsEqualTo(_s);
        Check.That(_.ApplicationVersion).IsEqualTo(_v.ToString());
        Check.That(_.BornOn).Equals(_0.GetBornOn()?.ToString("O"));
        Check.That(_.DistinguishedName).IsEqualTo(_s);
        Check.That(_.ID).Equals(_0.GetID());
        Check.That(_.Modified).IsNotEqualTo(_d?.ToString("O"));
        Check.That(_.Name).IsEqualTo(_s);
        Check.That(_.Notes).Contains(_n);
        Check.That(_.ObjectType!.Split(" -> ")).Contains("KusDepot.GenericItem");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Type).Equals(DataType.DIT);
        Check.That(_.Version).IsEqualTo(_v.ToString());

        Check.That(_0.SetID(Guid.Empty)).IsTrue();
        Check.That(_0.GetID()).IsNull();
        Check.That(_0.GetDescriptor()).IsNull();
    }

    [Test]
    public void GetDistinguishedName()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "CN=ServiceN,OU=Engineering,DC=TailSpinToys,DC=COM";

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void GetDomainID()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "GetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
    }

    [Test]
    public void GetExtension()
    {
        GenericItem _0 = new GenericItem();
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
        GenericItem _0 = new GenericItem();
        String _1 = "GetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.SetFILE(String.Empty)).IsTrue();
        Check.That(_0.GetFILE()).IsNull();
    }

    [Test]
    public void GetHash()
    {
        Int32[] _ = Enumerable.Range(0,12).ToArray();
        GuidReferenceItem _7 = new(Guid.NewGuid());
        GenericItem _0 = new GenericItem(content:null,id:Guid.NewGuid(),name:String.Empty);
        GenericItem _1 = new GenericItem(content:null,id:new Guid("00021401-0000-0000-C000-000000000046"),name:String.Empty);
        GenericItem _2 = new GenericItem(content:new List<Object>(){_},id:new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),name:String.Empty);
        GenericItem _3 = new(new List<Object>(){_});
        List<Object>? _8 = new List<Object>(){new Byte[3]{0xF,0xF,0xF},"EqualsInterfaceExam",_7,new Char[3]{'r','a','t'}};
        List<Object>? _10 = new List<Object>(){new Byte[3]{0xF,0xF,0xF},"EqualsInterfaceExam",_7,new Char[3]{'r','a','t'}};
        GenericItem _4 = new(_8);
        GenericItem _5 = new(_10);

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_0.GetHashCode()).IsNotEqualTo(_1.GetHashCode());
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        Check.That(_2.GetHashCode()).IsNotEqualTo(_3.GetHashCode());
        Check.That(_4.GetHashCode()).IsNotEqualTo(_5.GetHashCode());
    }

    [Test]
    public void GetID()
    {
        GenericItem _0 = new GenericItem();

        Check.That(_0.GetID()).HasAValue();
    }

    [Test]
    public void GetLinks()
    {
        GenericItem _0 = new GenericItem();
        Dictionary<String,GuidReferenceItem> _1 = new Dictionary<String,GuidReferenceItem>();
        GuidReferenceItem _2 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _3 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _4 = new GuidReferenceItem(Guid.NewGuid());
        _1.Add("_2",_2); _1.Add("_3",_3); _1.Add("_4",_4);

        Check.That(_0.SetLinks(null)).IsFalse();
        Check.That(_0.SetLinks(_1)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetLinks()).HasSize(3);
        Check.That(_0.GetLinks()?["_2"]).IsEqualTo(_2);
        Check.That(_0.GetLinks()?["_3"]).IsEqualTo(_3);
        Check.That(_0.GetLinks()?["_4"]).IsEqualTo(_4);
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsFalse();
        Check.That(_0.UnLock(KeyM)).IsTrue();
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsTrue();
        Check.That(_0.GetLinks()).IsNull();
    }

    [Test]
    public void GetLocked()
    {
        GenericItem _0 = new GenericItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetModified()
    {
        GenericItem _0 = new();

        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_0.GetModified()).IsNotNull();
        Check.That(_0.SetModified(_1)).IsTrue();
        Check.That(_0.GetModified()).Equals(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetModified(DateTimeOffset.MaxValue)).IsFalse();
        Check.That(_0.GetModified()).Equals(_1);
    }

    [Test]
    public void GetName()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "GetNameExam";

        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void GetNotes()
    {
        GenericItem _0 = new GenericItem();
        String _2 = "GetNotesExam";
        List<String> _1 = new(){_2,"Pass"};

        Check.That(_0.AddNotes(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
    }

    [Test]
    public void GetSecurityDescriptor()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "GetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void GetServiceVersion() 
    {
        GenericItem _0 = new GenericItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void GetTags()
    {
        GenericItem _0 = new GenericItem();
        List<String> _1 = new List<String>(){"GetTagsExam", "Pass"};

        Check.That(_0.AddTags(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
    }

    [Test]
    public new void GetType()
    {
        GenericItem _0 = new GenericItem();

        Check.That(_0.SetType(DataType.NFO)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.NFO);
    }

    [Test]
    public void GetVersion()
    {
        GenericItem _0 = new GenericItem();

        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Initialize()
    {
        GenericItem _1 = new GenericItem();

        Check.That(_1.Initialize()).IsTrue();

        Check.That(_1.GetID()).HasAValue();
    }

    [Test]
    public void Lock()
    {
        GenericItem _0 = new GenericItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public async Task MaskData()
    {
        GenericItem _0 = new GenericItem();
        Guid _1 = Guid.NewGuid();
        List<Object> _2 = new List<Object>(){_1};

        Check.That(_0.SetContent(_2)).IsTrue();
        Check.That(_0.GetContent()!.First()!).IsEqualTo(_1);
        var dc1 = await _0.GetDataContent();
        Check.That(dc1!.Generic!.First()!).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetContent(_2)).IsFalse();
        Check.That(_0.GetContent()!.First()!).IsEqualTo(_1);
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        var dc2 = await _0.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _0.GetDataContent(KeyM);
        Check.That(dc3!.Generic!.First()!).IsEqualTo(_1);
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()!.First()!).IsEqualTo(_1);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        GenericItem _0 = new GenericItem();

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.GetApplicationVersion()).IsNull();
        Check.That(_0.GetBornOn()).IsNotNull();
        Check.That(_0.GetContent()).IsNull();
        Check.That(_0.GetDistinguishedName()).IsNull();
        Check.That(_0.GetDomainID()).IsNull();
        Check.That(_0.GetExtension()).IsNull();
        Check.That(_0.GetFILE()).IsNull();
        Check.That(_0.GetID()).IsNotNull();
        Check.That(_0.GetLinks()).IsNull();
        Check.That(_0.GetModified()).IsNotNull();
        Check.That(_0.GetName()).IsNull();
        Check.That(_0.GetNotes()).IsNull();
        Check.That(_0.GetSecurityDescriptor()).IsNull();
        Check.That(_0.GetServiceVersion()).IsNull();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.GetType()).IsNull();
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Parse()
    {
        GenericItem _0 = new GenericItem(content:["ParseExam"],id:Guid.NewGuid());

        GenericItem? _1 = GenericItem.Parse(_0.ToString(),null);

        Check.That(_1).IsInstanceOfType(typeof(GenericItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void RemoveNote()
    {
        GenericItem _0 = new GenericItem();
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
    public void RemoveTag()
    {
        GenericItem _0 = new GenericItem();
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
    public void SetApplication()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetBornOn()
    {
        GenericItem _0 = new GenericItem();
        DateTimeOffset _1 = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);

        Check.That(_0.GetBornOn()).HasAValue().And.IsNotEqualTo(_1);
        Check.That(_0.SetBornOn(_1)).IsTrue();
        Check.That(_0.GetBornOn()).Equals(_1);
    }

    [Test]
    public void SetContent()
    {
        GenericItem _0 = new GenericItem();
        Guid _1 = Guid.NewGuid();
        List<Object> _2 = new List<Object>(){_1};

        Check.That(_0.SetContent(_2)).IsTrue();
        Check.That(_0.GetContent()!.First()!).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetContent(_2)).IsFalse();
    }

    [Test]
    public void SetContentStreamed()
    {
        GenericItem _0 = new();

        Byte[] d = new Byte[2194304];

        using RandomNumberGenerator r = RandomNumberGenerator.Create(); r.GetBytes(d);

        String p = Path.GetTempFileName()+Guid.NewGuid().ToString()+@".temp";

        if(File.Exists(p)) { throw new Exception(); } File.WriteAllBytes(p,d);

        Check.That(File.Exists(p)).IsTrue();

        Check.That(_0.SetFILE(p)).IsTrue();

        Check.That(_0.GetContentStreamed()).IsFalse();

        Check.That(_0.SetContentStreamed(true)).IsTrue();

        Check.That(_0.GetContentStreamed()).IsTrue();

        Stream? c = _0.GetContentStream(); Check.That(c).IsNotNull();

        Check.That(c!.Length).IsEqualTo(d.Length);

        Check.That(SHA512.HashData(d).SequenceEqual(SHA512.HashData(c))).IsTrue();

        Check.That(_0.SetContentStreamed(false)).IsTrue();

        Check.That(_0.GetContentStreamed()).IsFalse();

        c.Dispose(); File.Delete(p);
    }

    [Test]
    public void SetDistinguishedName()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "CN=ServiceN,OU=Operations,DC=Fabrikam,DC=NET";

        Check.That(_0.SetDistinguishedName(null)).IsFalse();
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void SetDomainID()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetDomainID(_1)).IsFalse();
    }

    [Test]
    public void SetExtension()
    {
        GenericItem _0 = new GenericItem();
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
    public void SetFILE()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
    }

    [Test]
    public void SetID()
    {
        GenericItem _0 = new GenericItem();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void SetLinks()
    {
        GenericItem _0 = new GenericItem();
        Dictionary<String,GuidReferenceItem> _1 = new Dictionary<String,GuidReferenceItem>();
        GuidReferenceItem _2 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _3 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _4 = new GuidReferenceItem(Guid.NewGuid());
        _1.Add("_2",_2); _1.Add("_3",_3); _1.Add("_4",_4);

        Check.That(_0.SetLinks(null)).IsFalse();
        Check.That(_0.SetLinks(_1)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetLinks()).HasSize(3);
        Check.That(_0.GetLinks()?["_2"]).IsEqualTo(_2);
        Check.That(_0.GetLinks()?["_3"]).IsEqualTo(_3);
        Check.That(_0.GetLinks()?["_4"]).IsEqualTo(_4);
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsFalse();
        Check.That(_0.UnLock(KeyM)).IsTrue();
        Check.That(_0.SetLinks(new Dictionary<String,GuidReferenceItem>())).IsTrue();
        Check.That(_0.GetLinks()).IsNull();
    }

    [Test]
    public void SetModified()
    {
        GenericItem _0 = new();

        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_0.GetModified()).IsNotNull();
        Check.That(_0.SetModified(_1)).IsTrue();
        Check.That(_0.GetModified()).Equals(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetModified(DateTimeOffset.MaxValue)).IsFalse();
        Check.That(_0.GetModified()).Equals(_1);
    }

    [Test]
    public void SetName()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void SetSecurityDescriptor()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void SetServiceVersion()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetType()
    {
        GenericItem _0 = new GenericItem();

        Check.That(_0.SetType(DataType.NFO)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.NFO);
        Check.That(_0.SetType(DataType.ADMX)).IsFalse();
    }

    [Test]
    public void SetVersion()
    {
        GenericItem _0 = new GenericItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void ToFile()
    {
        Byte[] _1 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_1);

        GenericItem _2 = new GenericItem(new List<Object>{_1});

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\GenericItem04.kdi";

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();

        Check.That(_2.ToFile(_3)).IsTrue();

        GenericItem? _4 = GenericItem.FromFile(_3);

        Check.That(_4).IsNotNull();

        Check.That(_2.Equals(_4)).IsTrue();

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();
    }

    [Test]
    public new void ToString()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();

        Byte[] _1 = new Byte[8192]; _0.GetBytes(_1);

        Byte[] _2 = new Byte[32768]; _0.GetBytes(_2);

        GenericItem _3 = new GenericItem(content:new List<Object>(){_1,_2},id:null,name:String.Empty);

        GenericItem? _4 = GenericItem.Parse(_3.ToString(),null);

        Check.That(_3.Equals(_4)).IsTrue();
    }

    [Test]
    public void TryParse()
    {
        GenericItem _0 = new GenericItem(content:["TryParseExam".ToByteArrayFromUTF16String()],id:Guid.NewGuid());

        GenericItem? _1;

        GenericItem.TryParse(_0.ToString(),null,out _1);

        Check.That(_1).IsInstanceOfType(typeof(GenericItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        GenericItem _0 = new GenericItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }
}