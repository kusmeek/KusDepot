namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class BinaryItemExam
{
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
    public void Calibrate() { if(Settings.NoExceptions is true) { throw new InvalidOperationException(); } }

    [Test]
    public void Clone()
    {
        BinaryItem _0 = new BinaryItem();
        BinaryItem _1 = new BinaryItem();

        Check.That(_0.Equals(_0.Clone())).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsTrue();
        Check.That(_1.SetContent(new Byte[1])).IsTrue();
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
    }

    [Test]
    public void Constructor()
    {
        Byte[] _0 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_0);
        String? _1 = Settings.BinaryItemValidDataTypes[RandomNumberGenerator.GetInt32(0,Settings.BinaryItemValidDataTypes.Length)]?.ToString();
        BinaryItem _2 = new BinaryItem(_0,Guid.NewGuid(),String.Empty,new HashSet<String>{},new HashSet<String>{},_1);

        Check.That(_2).IsInstanceOfType(typeof(BinaryItem));
        Check.That(_2.GetContent()).IsEqualTo(_0);
    }

    [Test]
    public void EqualsObject()
    {
        BinaryItem _0 = new BinaryItem();
        BinaryItem _1 = new BinaryItem();

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsTrue();
        Check.That(((Object)new BinaryItem()).Equals(new ItemSync())).IsFalse();
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
        BinaryItem _3 = new BinaryItem(_2,_11,String.Empty);
        BinaryItem _15 = new BinaryItem(_2,_11,String.Empty);
        BinaryItem _4 = new BinaryItem(_2,_14,String.Empty);
        BinaryItem _5 = new BinaryItem(_2,null,String.Empty);
        BinaryItem _9 = new BinaryItem(_8,_11,String.Empty);
        BinaryItem _10 = new BinaryItem(null,null,String.Empty);
        BinaryItem _12 = new BinaryItem(null,_11,String.Empty);
        BinaryItem _13 = new BinaryItem(null,_11,String.Empty);

        Check.That(new BinaryItem().Equals(null)).IsFalse();
        Check.That(new BinaryItem().Equals(new Object())).IsFalse();
        Check.That(new BinaryItem().Equals(_10)).IsTrue();
        Check.That(_3.Equals(_4)).IsTrue();
        Check.That(_3.Equals(_5)).IsTrue();
        Check.That(_3.Equals(_9)).IsFalse();
        Check.That(_3.Equals(_15)).IsTrue();
        Check.That(_10.Equals(_5)).IsFalse();
        Check.That(_12.Equals(_9)).IsFalse();
        Check.That(_12.Equals(_13)).IsTrue();
    }

    [Test]
    public void FromFile()
    {
        Byte[] _1 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_1);

        BinaryItem _2 = new BinaryItem(_1,null,String.Empty);

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
    public void GetAppDomainID()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetAppDomainID()).HasAValue();
    }

    [Test]
    public void GetAppDomainUID() 
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetAppDomainUID()).IsNull();
    }

    [Test]
    public void GetApplication()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetAssemblyVersion()
    {
        BinaryItem _0 = new BinaryItem();

        Version? _1 = Assembly.GetExecutingAssembly().GetName().Version;

        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
    }

    [Test]
    public void GetBornOn()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public void GetCertificates()
    {
        BinaryItem _0 = new BinaryItem();
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
    public void GetContent()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();
        Byte[] _1 = new Byte[8192];
        _0.GetBytes(_1);
        BinaryItem _3 = new BinaryItem(_1);

        Check.That(_3.GetContent()!).IsEqualTo(_1);
        Check.That(_3.Lock("key")).IsTrue();
        Check.That(_3.SetContent(_1)).IsFalse();
        Check.That(_3.GetContent()!).IsEqualTo(_1);
        Check.That(ReferenceEquals(_3.GetContent()!,_1)).IsFalse();
    }

    [Test]
    public void GetCPUID()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetCPUID()).IsNull();
    }

    [Test]
    public void GetDescriptor()
    {
        BinaryItem _0 = new BinaryItem();

        String _s = "GetDescriptorExam";
        Version _v = new Version("1.2.3.4");
        DateTimeOffset _d = DateTimeOffset.Now;
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
        Check.That(_0.SetType(DataType.PDB)).IsTrue();
        Check.That(_0.SetVersion(_v)).IsTrue();

        Descriptor? _ = _0.GetDescriptor();

        Check.That(_!.Application).IsEqualTo(_s);
        Check.That(_.ApplicationVersion).IsEqualTo(_v.ToString());
        Check.That(_.BornOn).Equals(_0.GetBornOn()?.ToString("O"));
        Check.That(_.DistinguishedName).IsEqualTo(_s);
        Check.That(_.ID).Equals(_0.GetID());
        Check.That(_.Modified).IsEqualTo(_d.ToString("O"));
        Check.That(_.Name).IsEqualTo(_s);
        Check.That(_.Notes).Contains(_n);
        Check.That(_.ObjectType).Equals("BinaryItem");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Type).Equals(DataType.PDB);
        Check.That(_.Version).IsEqualTo(_v.ToString());
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
    public void GetGPS()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "GetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
    }

    [Test]
    public void GetHash()
    {
        BinaryItem _0 = new BinaryItem(null,Guid.NewGuid(),String.Empty,null,null,null);
        BinaryItem _1 = new BinaryItem(null,new Guid("00021401-0000-0000-C000-000000000046"),String.Empty,null,null,null);
        BinaryItem _2 = new BinaryItem(null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),String.Empty,null,null,null);

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
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
        GuidReferenceItem _2 = new GuidReferenceItem();
        GuidReferenceItem _3 = new GuidReferenceItem();
        GuidReferenceItem _4 = new GuidReferenceItem();
        _1.Add("_2",_2); _1.Add("_3",_3); _1.Add("_4",_4);

        Check.That(_0.SetLinks(null)).IsFalse();
        Check.That(_0.SetLinks(_1)).IsTrue();
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
        BinaryItem _0 = new BinaryItem();
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
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(String.Empty)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetMachineID()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetMachineID()).HasContent();
    }

    [Test]
    public void GetModified()
    {
        BinaryItem _0 = new BinaryItem();
        BinaryItem _3 = new BinaryItem();
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
        Check.That(_0.Lock(_2)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
    }

    [Test]
    public void GetProcessID()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetProcessID()).HasAValue();
    }

    [Test]
    public void GetSecureHashes()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "GetSecureHashesExam";
        Byte[] _2 = new Byte[384]; RandomNumberGenerator.Create().GetBytes(_2);
        Dictionary<String,Byte[]> _3 = new Dictionary<String,Byte[]>();
        _3.Add(_1,_2);

        Check.That(_0.SetSecureHashes(_3)).IsTrue();
        Check.That(_0.GetSecureHashes()![_1]).Equals(_2);
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.GetSecureHashes()![_1]).Equals(_2);
        Check.That(_0.SetSecureHashes(_3)).IsFalse();
        Check.That(_0.UnLock(_1)).IsTrue();
        Check.That(_0.SetSecureHashes(new Dictionary<String,Byte[]>())).IsTrue();
        Check.That(_0.GetSecureHashes()).IsNull();
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
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.ACCDB);
    }

    [Test]
    public void GetThreadID()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetThreadID()).HasAValue();
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

        Check.That(_1.GetAppDomainID()).IsNotNull();
        Check.That(_1.GetAssemblyVersion()).IsNotNull();
        Check.That(_1.GetBornOn()).IsNotNull();
        Check.That(_1.GetID()).HasAValue();
        Check.That(_1.GetMachineID()).IsNotNull();
        Check.That(_1.GetProcessID()).IsNotNull();
        Check.That(_1.GetThreadID()).IsNotNull();
    }

    [Test]
    public void Lock()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(String.Empty)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        BinaryItem _0 = new BinaryItem();

        Check.That(_0.GetAppDomainID()).IsNotNull();
        Check.That(_0.GetAppDomainUID()).IsNull();
        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.GetApplicationVersion()).IsNull();
        Check.That(_0.GetAssemblyVersion()).IsNotNull();
        Check.That(_0.GetBornOn()).IsNotNull();
        Check.That(_0.GetContent()).IsNull();
        Check.That(_0.GetCPUID()).IsNull();
        Check.That(_0.GetDistinguishedName()).IsNull();
        Check.That(_0.GetDomainID()).IsNull();
        Check.That(_0.GetExtension()).IsNull();
        Check.That(_0.GetFILE()).IsNull();
        Check.That(_0.GetGPS()).IsNull();
        Check.That(_0.GetID()).IsNotNull();
        Check.That(_0.GetLinks()).IsNull();
        Check.That(_0.GetLocator()).IsNull();
        Check.That(_0.GetMachineID()).IsNotNull();
        Check.That(_0.GetModified()).IsNull();
        Check.That(_0.GetName()).IsNull();
        Check.That(_0.GetNotes()).IsNull();
        Check.That(_0.GetProcessID()).IsNotNull();
        Check.That(_0.GetSecureHashes()).IsNull();
        Check.That(_0.GetSecurityDescriptor()).IsNull();
        Check.That(_0.GetServiceVersion()).IsNull();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.GetThreadID()).IsNotNull();
        Check.That(_0.GetType()).IsNull();
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Parse()
    {
        BinaryItem _0 = new BinaryItem(null,Guid.NewGuid(),null,null,null,null);

        BinaryItem _1 = BinaryItem.Parse(_0.ToString(),null);

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
    public void SetCertificates()
    {
        BinaryItem _0 = new BinaryItem();
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
    public void SetContent()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();
        Byte[] _1 = new Byte[8192];
        _0.GetBytes(_1);
        BinaryItem _3 = new BinaryItem();

        Check.That(_3.SetContent(_1)).IsTrue();
        Check.That(_3.GetContent()!).IsEqualTo(_1);
        Check.That(_3.Lock("key")).IsTrue();
        Check.That(_3.SetContent(_1)).IsFalse();
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
        Check.That(_0.Lock("key")).IsTrue();
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
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
    }

    [Test]
    public void SetGPS()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "SetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
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
        GuidReferenceItem _2 = new GuidReferenceItem();
        GuidReferenceItem _3 = new GuidReferenceItem();
        GuidReferenceItem _4 = new GuidReferenceItem();
        _1.Add("_2",_2); _1.Add("_3",_3); _1.Add("_4",_4);

        Check.That(_0.SetLinks(null)).IsFalse();
        Check.That(_0.SetLinks(_1)).IsTrue();
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
        BinaryItem _0 = new BinaryItem();
        String _1 = "app://server/SetLocatorExam";
        Uri _2 = new Uri(_1);

        Check.That(_0.SetLocator(null)).IsFalse();
        Check.That(_0.SetLocator(_2)).IsTrue();
        Check.That(_0.GetLocator()!.AbsoluteUri).IsEqualTo(_1);
    }

    [Test]
    public void SetModified()
    {
        BinaryItem _0 = new BinaryItem();
        BinaryItem _3 = new BinaryItem();
        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_3.SetModified(_1)).IsTrue();
        Check.That(_0.SetModified(null)).IsFalse();
        Check.That(_0.GetModified()).IsNull();
        Check.That(_3.GetModified()).Equals(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_3.GetModified()).Equals(_1);
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
    public void SetSecureHashes()
    {
        BinaryItem _0 = new BinaryItem();
        String _1 = "SetSecureHashesExam";
        Byte[] _2 = new Byte[384]; RandomNumberGenerator.Create().GetBytes(_2);
        Dictionary<String,Byte[]> _3 = new Dictionary<String,Byte[]>();
        _3.Add(_1,_2);

        Check.That(_0.SetSecureHashes(_3)).IsTrue();
        Check.That(_0.GetSecureHashes()![_1]).Equals(_2);
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.GetSecureHashes()![_1]).Equals(_2);
        Check.That(_0.SetSecureHashes(_3)).IsFalse();
        Check.That(_0.UnLock(_1)).IsTrue();
        Check.That(_0.SetSecureHashes(new Dictionary<String,Byte[]>())).IsTrue();
        Check.That(_0.GetSecureHashes()).IsNull();
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
        Check.That(_0.Lock("key")).IsTrue();
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

        BinaryItem _2 = new BinaryItem(_1,null,String.Empty);

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

        BinaryItem _2 = new BinaryItem(_1,null,String.Empty);

        BinaryItem? _3 = BinaryItem.Parse(_2.ToString(),null);

        Check.That(_2.Equals(_3)).IsTrue();
    }

    [Test]
    public void TryParse()
    {
        BinaryItem _0 = new BinaryItem(null,Guid.NewGuid(),null,null,null,null);

        BinaryItem? _1;

        BinaryItem.TryParse(_0.ToString(),null,out _1);

        Check.That(_1).IsInstanceOfType(typeof(BinaryItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        BinaryItem _0 = new BinaryItem();

        String _1 = "UnLockExam";

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(_1)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock("False")).IsFalse();

        Check.That(_0.UnLock(_1)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }

    [TestCaseSource(typeof(TestCaseDataGenerator),nameof(TestCaseDataGenerator.DataTypeTestCases))]
    public void Validate(String? type)
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();

        Byte[] _1 = new Byte[8192];

        _0.GetBytes(_1);

        BinaryItem _2 = new BinaryItem(_1,null,String.Empty,null,null,type);

        if(Settings.BinaryItemValidDataTypes.Contains(type))
        {
            Check.That(BinaryItem.Validate(_2)).IsTrue();
        }

        else
        {
            Check.That(BinaryItem.Validate(_2)).IsFalse();
        }
    }
}