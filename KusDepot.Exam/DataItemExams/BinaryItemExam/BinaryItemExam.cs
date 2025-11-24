namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class BinaryItemExam
{
    private ManagerKey? KeyM;

    [Test]
    public void AddNotes()
    {
        BinaryItem _0 = new BinaryItem();
        HashSet<String> _1 = new HashSet<String>(){"AddNotesExam"};
        HashSet<String> _2 = new HashSet<String>(){"Pass",String.Empty};
        HashSet<String> _3 = new HashSet<String>(); _3.UnionWith(_1); _3.UnionWith(_2);

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
        BinaryItem _0 = new BinaryItem();
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
        BinaryItem _0 = new BinaryItem();
        BinaryItem _1 = new BinaryItem();

        Check.That(_0.Equals(_0.Clone())).IsFalse();
        Check.That(_1.SetContent(new Byte[1])).IsTrue();
        Check.That(_1.Equals(_1.Clone())).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsFalse();
    }

    [Test]
    public void CompareTo()
    {
        BinaryItem _0 = new BinaryItem();
        Thread.Sleep(100);
        BinaryItem _1 = new BinaryItem();

        Check.That(new BinaryItem().CompareTo(null)).IsEqualTo(1);
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
        Byte[] _0 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_0);
        String? _1 = DataValidation.BinaryItemValidDataTypes[RandomNumberGenerator.GetInt32(0,DataValidation.BinaryItemValidDataTypes.Length)]?.ToString();
        BinaryItem _2 = new BinaryItem(content:_0,file:null,id:Guid.NewGuid(),name:String.Empty,notes:new HashSet<String>{},tags:new HashSet<String>{},type:_1);

        Check.That(_2).IsInstanceOfType(typeof(BinaryItem));
        Check.That(_2.GetContent()).IsEqualTo(_0);
    }

    [Test]
    public void opEquality()
    {
        RandomNumberGenerator _1 = RandomNumberGenerator.Create();
        RandomNumberGenerator _7 = RandomNumberGenerator.Create();
        Byte[] _2 = new Byte[8192];
        Byte[] _8 = new Byte[8192];
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        _1.GetBytes(_2);
        _7.GetBytes(_8);
        BinaryItem _3 = new BinaryItem(content:_2,id:_11,name:String.Empty);
        BinaryItem _15 = new BinaryItem(content:_2,id:_11,name:String.Empty);
        BinaryItem _4 = new BinaryItem(content:_2,id:_14,name:String.Empty);
        BinaryItem _5 = new BinaryItem(content:_2,id:null,name:String.Empty);
        BinaryItem _9 = new BinaryItem(content:_8,id:_11,name:String.Empty);
        BinaryItem _10 = new BinaryItem(content:null,id:null,name:String.Empty);
        BinaryItem _12 = new BinaryItem(content:null,id:_11,name:String.Empty);
        BinaryItem _13 = new BinaryItem(content:null,id:_11,name:String.Empty);

        Check.That(new BinaryItem() == null).IsFalse();
        Check.That(new BinaryItem() == _10).IsFalse();
        Check.That(_3 == _4).IsTrue();
        Check.That(_3 == _5).IsTrue();
        Check.That(_3 == _9).IsFalse();
        Check.That(_3 == _15).IsTrue();
        Check.That(_10 == _5).IsFalse();
        Check.That(_12 == _9).IsFalse();
        Check.That(_12 == _13).IsFalse();
    }

    [Test]
    public void opInequality()
    {
        RandomNumberGenerator _1 = RandomNumberGenerator.Create();
        RandomNumberGenerator _7 = RandomNumberGenerator.Create();
        Byte[] _2 = new Byte[8192];
        Byte[] _8 = new Byte[8192];
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        _1.GetBytes(_2);
        _7.GetBytes(_8);
        BinaryItem _3 = new BinaryItem(content:_2,id:_11,name:String.Empty);
        BinaryItem _15 = new BinaryItem(content:_2,id:_11,name:String.Empty);
        BinaryItem _4 = new BinaryItem(content:_2,id:_14,name:String.Empty);
        BinaryItem _5 = new BinaryItem(content:_2,id:null,name:String.Empty);
        BinaryItem _9 = new BinaryItem(content:_8,id:_11,name:String.Empty);
        BinaryItem _10 = new BinaryItem(content:null,id:null,name:String.Empty);
        BinaryItem _12 = new BinaryItem(content:null,id:_11,name:String.Empty);
        BinaryItem _13 = new BinaryItem(content:null,id:_11,name:String.Empty);

        Check.That(new BinaryItem() != null).IsTrue();
        Check.That(new BinaryItem() != _10).IsTrue();
        Check.That(_3 != _4).IsFalse();
        Check.That(_3 != _5).IsFalse();
        Check.That(_3 != _9).IsTrue();
        Check.That(_3 != _15).IsFalse();
        Check.That(_10 != _5).IsTrue();
        Check.That(_12 != _9).IsTrue();
        Check.That(_12 != _13).IsTrue();
    }

    [Test]
    public void EqualsObject()
    {
        BinaryItem _0 = new BinaryItem();
        BinaryItem _1 = new BinaryItem();

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsFalse();
        Check.That(((Object)new BinaryItem()).Equals(null)).IsFalse();
        Check.That(((Object)new BinaryItem()).Equals(new Object())).IsFalse();
    }

    [Test]
    public void EqualsInterface()
    {
        RandomNumberGenerator _1 = RandomNumberGenerator.Create();
        RandomNumberGenerator _7 = RandomNumberGenerator.Create();
        Byte[] _2 = new Byte[8192];
        Byte[] _8 = new Byte[8192];
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        _1.GetBytes(_2);
        _7.GetBytes(_8);
        BinaryItem _3 = new BinaryItem(content:_2,id:_11,name:String.Empty);
        BinaryItem _15 = new BinaryItem(content:_2,id:_11,name:String.Empty);
        BinaryItem _4 = new BinaryItem(content:_2,id:_14,name:String.Empty);
        BinaryItem _5 = new BinaryItem(content:_2,id:null,name:String.Empty);
        BinaryItem _9 = new BinaryItem(content:_8,id:_11,name:String.Empty);
        BinaryItem _10 = new BinaryItem(content:null,id:null,name:String.Empty);
        BinaryItem _12 = new BinaryItem(content:null,id:_11,name:String.Empty);
        BinaryItem _13 = new BinaryItem(content:null,id:_11,name:String.Empty);

        Check.That(new BinaryItem().Equals(null)).IsFalse();
        Check.That(new BinaryItem().Equals(new Object())).IsFalse();
        Check.That(new BinaryItem().Equals(_10)).IsFalse();
        Check.That(_3.Equals(_4)).IsTrue();
        Check.That(_3.Equals(_5)).IsTrue();
        Check.That(_3.Equals(_9)).IsFalse();
        Check.That(_3.Equals(_15)).IsTrue();
        Check.That(_10.Equals(_5)).IsFalse();
        Check.That(_12.Equals(_9)).IsFalse();
        Check.That(_12.Equals(_13)).IsFalse();
    }

    [Test]
    public void FromFile()
    {
        Byte[] _1 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_1);

        BinaryItem _2 = new BinaryItem(content:_1,id:null,name:String.Empty);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\BinaryItem.bin";

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();

        Check.That(_2.ToFile(_3)).IsTrue();

        BinaryItem? _4 = BinaryItem.FromFile(_3);

        Check.That(_4).IsNotNull();

        Check.That(_2.Equals(_4)).IsTrue();

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();
    }

    [Test]
    public void GetApplication()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetBornOn()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public async Task GetContent()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();
        Byte[] _1 = new Byte[8192]; _0.GetBytes(_1);
        BinaryItem _3 = new BinaryItem(_1);

        Check.That(_3.GetContent()!).IsEqualTo(_1);
        var dc1 = await _3.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.Binary).IsNotNull();
        Check.That(dc1.Binary).IsEqualTo(_1);
        Check.That(_3.Lock(KeyM)).IsTrue();
        Check.That(_3.SetContent(_1)).IsFalse();
        Check.That(_3.GetContent()!).IsEqualTo(_1);
        Check.That(ReferenceEquals(_3.GetContent()!,_1)).IsFalse();
        Check.That(_3.MaskData(true,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsNull();
        Check.That(await _3.GetDataContent()).IsNull();
        var dc2 = await _3.GetDataContent(KeyM);
        Check.That(dc2).IsNotNull();
        Check.That(dc2!.Binary).IsNotNull();
        Check.That(dc2.Binary).IsEqualTo(_1);
        Check.That(_3.MaskData(false,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsEqualTo(_1);
    }

    [Test]
    public async Task GetDataContent()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();
        Byte[] _1 = new Byte[8192]; _0.GetBytes(_1);
        BinaryItem _3 = new BinaryItem(_1);

        Check.That(_3.GetContent()!).IsEqualTo(_1);
        var dc1 = await _3.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.Binary).IsNotNull();
        Check.That(dc1.Binary).IsEqualTo(_1);
        Check.That(_3.Lock(KeyM)).IsTrue();
        Check.That(_3.SetContent(_1)).IsFalse();
        Check.That(_3.GetContent()!).IsEqualTo(_1);
        Check.That(ReferenceEquals(_3.GetContent()!,_1)).IsFalse();
        Check.That(_3.MaskData(true,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsNull();
        Check.That(await _3.GetDataContent()).IsNull();
        var dc2 = await _3.GetDataContent(KeyM);
        Check.That(dc2).IsNotNull();
        Check.That(dc2!.Binary).IsNotNull();
        Check.That(dc2.Binary).IsEqualTo(_1);
        Check.That(_3.MaskData(false,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsEqualTo(_1);
    }

    [Test]
    public void GetDescriptor()
    {
        BinaryItem _0 = BinaryItem.FromFile(Path.Join(AppContext.BaseDirectory,"ContainerAssembly.bin"))!;

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
        Check.That(_0.SetType(DataType.DLL)).IsTrue();
        Check.That(_0.SetVersion(_v)).IsTrue();

        Descriptor? _ = _0.GetDescriptor();

        Check.That(_!.Application).IsEqualTo(_s);
        Check.That(_.ApplicationVersion).IsEqualTo(_v.ToString());
        Check.That(_.BornOn).Equals(_0.GetBornOn()?.ToString("O"));
        Check.That(_.Commands!.Count).IsEqualTo(4);
        Check.That(_.DistinguishedName).IsEqualTo(_s);
        Check.That(_.ID).Equals(_0.GetID());
        Check.That(_.Modified).IsNotEqualTo(_d?.ToString("O"));
        Check.That(_.Name).IsEqualTo(_s);
        Check.That(_.Notes).Contains(_n);
        Check.That(_.ObjectType!.Split(" -> ")).Contains("KusDepot.BinaryItem");
        Check.That(_.Services!.Count).IsEqualTo(2);
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Type).Equals(DataType.DLL);
        Check.That(_.Version).IsEqualTo(_v.ToString());

        Check.That(_0.SetID(Guid.Empty)).IsTrue();
        Check.That(_0.GetID()).IsNull();
        Check.That(_0.GetDescriptor()).IsNull();
    }

    [Test]
    public void GetDistinguishedName()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "CN=ServiceN,OU=Engineering,DC=TailSpinToys,DC=COM";

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void GetDomainID()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "GetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
    }

    [Test]
    public void GetExtension()
    {
        BinaryItem _0 = new BinaryItem();
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
        BinaryItem _0 = new BinaryItem();
        String _1 = "GetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.SetFILE(String.Empty)).IsTrue();
        Check.That(_0.GetFILE()).IsNull();
    }

    [Test]
    public void GetHash()
    {
        Byte[] _ = new Byte[3]{0xA,0xB,0xE};
        BinaryItem _0 = new BinaryItem(content:null,id:Guid.NewGuid(),name:String.Empty);
        BinaryItem _1 = new BinaryItem(content:null,id:new Guid("00021401-0000-0000-C000-000000000046"),name:String.Empty);
        BinaryItem _2 = new BinaryItem(content:_,id:new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),name:String.Empty);
        BinaryItem _3 = new(_);

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_0.GetHashCode()).IsNotEqualTo(_1.GetHashCode());
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        Check.That(_2.GetHashCode()).IsNotEqualTo(_3.GetHashCode());
    }

    [Test]
    public void GetID()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetID()).HasAValue();
    }

    [Test]
    public void GetLinks()
    {
        BinaryItem _0 = new BinaryItem();
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
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetModified()
    {
        BinaryItem _0 = new();

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
        BinaryItem _0 = new BinaryItem();
        String _1 = "GetNameExam";

        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void GetNotes()
    {
        BinaryItem _0 = new BinaryItem();
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
        BinaryItem _0 = new BinaryItem();
        String _1 = "GetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void GetServiceVersion() 
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void GetTags()
    {
        BinaryItem _0 = new BinaryItem();
        List<String> _1 = new List<String>(){"GetTagsExam", "Pass"};

        Check.That(_0.AddTags(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
    }

    [Test]
    public new void GetType()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.SetType(DataType.ACCDB)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.ACCDB);
    }

    [Test]
    public void GetVersion()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Initialize()
    {
        BinaryItem _1 = new BinaryItem();

        Check.That(_1.Initialize()).IsTrue();

        Check.That(_1.GetID()).HasAValue();
    }

    [Test]
    public void Lock()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public async Task MaskData()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();
        Byte[] _1 = new Byte[8192]; _0.GetBytes(_1);
        BinaryItem _3 = new BinaryItem(_1);

        Check.That(_3.GetContent()!).IsEqualTo(_1);
        var dc1 = await _3.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.Binary).IsNotNull();
        Check.That(dc1.Binary).IsEqualTo(_1);
        Check.That(_3.Lock(KeyM)).IsTrue();
        Check.That(_3.SetContent(_1)).IsFalse();
        Check.That(_3.GetContent()!).IsEqualTo(_1);
        Check.That(ReferenceEquals(_3.GetContent()!,_1)).IsFalse();
        Check.That(_3.MaskData(true,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsNull();
        Check.That(await _3.GetDataContent()).IsNull();
        var dc2 = await _3.GetDataContent(KeyM);
        Check.That(dc2).IsNotNull();
        Check.That(dc2!.Binary).IsNotNull();
        Check.That(dc2.Binary).IsEqualTo(_1);
        Check.That(_3.MaskData(false,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsEqualTo(_1);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        BinaryItem _0 = new BinaryItem();

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
        BinaryItem _0 = new BinaryItem(content:"ParseExam".ToByteArrayFromUTF16String(),id:Guid.NewGuid());

        BinaryItem? _1 = BinaryItem.Parse(_0.ToString(),null);

        Check.That(_1).IsInstanceOfType(typeof(BinaryItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void RemoveNote()
    {
        BinaryItem _0 = new BinaryItem();
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
        BinaryItem _0 = new BinaryItem();
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
        BinaryItem _0 = new BinaryItem();
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetBornOn()
    {
        BinaryItem _0 = new BinaryItem();
        DateTimeOffset _1 = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);

        Check.That(_0.GetBornOn()).HasAValue().And.IsNotEqualTo(_1);
        Check.That(_0.SetBornOn(_1)).IsTrue();
        Check.That(_0.GetBornOn()).Equals(_1);
    }

    [Test]
    public void SetContent()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();
        Byte[] _1 = new Byte[8192]; _0.GetBytes(_1);
        BinaryItem _3 = new BinaryItem();

        Check.That(_3.SetContent(_1)).IsTrue();
        Check.That(_3.GetContent()!).IsEqualTo(_1);
        Check.That(_3.Lock(KeyM)).IsTrue();
        Check.That(_3.SetContent(_1)).IsFalse();
    }

    [Test]
    public void SetContentStreamed()
    {
        BinaryItem _0 = new();

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
        BinaryItem _0 = new BinaryItem();
        String _1 = "CN=ServiceN,OU=Operations,DC=Fabrikam,DC=NET";

        Check.That(_0.SetDistinguishedName(null)).IsFalse();
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void SetDomainID()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetDomainID(_1)).IsFalse();
    }

    [Test]
    public void SetExtension()
    {
        BinaryItem _0 = new BinaryItem();
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
        BinaryItem _0 = new BinaryItem();
        String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
    }

    [Test]
    public void SetID()
    {
        BinaryItem _0 = new BinaryItem();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void SetLinks()
    {
        BinaryItem _0 = new BinaryItem();
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
        BinaryItem _0 = new();

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
        BinaryItem _0 = new BinaryItem();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void SetSecurityDescriptor()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void SetServiceVersion()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetType()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.SetType(DataType.ACCDB)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.ACCDB);
        Check.That(_0.SetType(DataType.ISO)).IsFalse();
    }

    [Test]
    public void SetVersion()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void ToFile()
    {
        Byte[] _1 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_1);

        BinaryItem _2 = new BinaryItem(content:_1,id:null,name:String.Empty);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\BinaryItem008.bin";

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();

        Check.That(_2.ToFile(_3)).IsTrue();

        BinaryItem? _4 = BinaryItem.FromFile(_3);

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

        BinaryItem _2 = new BinaryItem(content:_1,id:null,name:String.Empty);

        BinaryItem? _3 = BinaryItem.Parse(_2.ToString(),null);

        Check.That(_2.Equals(_3)).IsTrue();
    }

    [Test]
    public void TryParse()
    {
        BinaryItem _0 = new BinaryItem(content:"TryParseExam".ToByteArrayFromUTF16String(),id:Guid.NewGuid());

        BinaryItem? _1;

        BinaryItem.TryParse(_0.ToString(),null,out _1);

        Check.That(_1).IsInstanceOfType(typeof(BinaryItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }
}
