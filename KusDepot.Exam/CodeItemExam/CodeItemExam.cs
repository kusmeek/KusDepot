namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class CodeItemExam
{
    [Test]
    public void AddNotes()
    {
        CodeItem _0 = new CodeItem();
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
        CodeItem _0 = new CodeItem();
        String _1 = "AddTagsExam";
        String _2 = "Pass";
        HashSet<String> _3 = new(); _3.Add(_1);
        HashSet<String> _4 = new(); _4.Add(_2); _4.Add(String.Empty);

        Check.That(_0.AddTags(null)).IsFalse();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.AddTags(_3.ToArray())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
        Check.That(_0.AddTags(_4.ToList())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1,_2,String.Empty);
        Check.That(_0.AddTags(new HashSet<String>())).IsTrue();
    }

    [OneTimeSetUp]
    public void Calibrate() { if(Settings.NoExceptions is true) { throw new InvalidOperationException(); } }

    [Test]
    public void Clone()
    {
        CodeItem _0 = new CodeItem();
        CodeItem _1 = new CodeItem();

        Check.That(_0.Equals(_0.Clone())).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsTrue();
        Check.That(_1.SetContent("CloneExam")).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsFalse();
    }

    [Test]
    public void CompareTo()
    {
        CodeItem _0 = new CodeItem();
        Thread.Sleep(100);
        CodeItem _1 = new CodeItem();

        Check.That(new CodeItem().CompareTo(null)).IsEqualTo(1);
        Check.That(_0.CompareTo(_0)).IsEqualTo(0);
        Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
        Check.That(_1.CompareTo(_0)).IsStrictlyPositive();
    }

    [Test]
    public void Constructor()
    {
        RandomNumberGenerator _1 = RandomNumberGenerator.Create();
        Byte[] _2 = new Byte[8192];
        _1.GetBytes(_2);
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        String? _3 = Settings.CodeItemValidDataTypes[RandomNumberGenerator.GetInt32(0,Settings.CodeItemValidDataTypes.Length)];
        CodeItem _4 = new CodeItem(_0.ToString(),Guid.NewGuid(),String.Empty,new HashSet<String>{},new HashSet<String>{},_3!.ToString());

        Check.That(_4).IsInstanceOfType(typeof(CodeItem));
        Check.That(_4.GetContent()).IsEqualTo(_0.ToString());
    }

    [Test]
    public void EqualsObject()
    {
        CodeItem _0 = new CodeItem();
        CodeItem _1 = new CodeItem();

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsTrue();
        Check.That(((Object)new CodeItem()).Equals(new ItemSync())).IsFalse();
        Check.That(((Object)new CodeItem()).Equals(null)).IsFalse();
        Check.That(((Object)new CodeItem()).Equals(new Object())).IsFalse();
    }

    [Test]
    public void EqualsInterface()
    {
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        String _6 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        CodeItem _3 = new CodeItem(_0,_11);
        CodeItem _15 = new CodeItem(_0,_11);
        CodeItem _4 = new CodeItem(_0,_14);
        CodeItem _5 = new CodeItem(_0);
        CodeItem _9 = new CodeItem(_6,_11);
        CodeItem _10 = new CodeItem(_6,_14);
        CodeItem _16 = new CodeItem(null,_14);
        CodeItem _17 = new CodeItem(null,null,null,null,null,null);
        CodeItem _18 = new CodeItem(null,_14);
        CodeItem _19 = new CodeItem(String.Empty,null);
        CodeItem _20 = new CodeItem(String.Empty,null);

        Check.That(new CodeItem().Equals(null)).IsFalse();
        Check.That(new CodeItem().Equals(new Object())).IsFalse();
        Check.That(new CodeItem().Equals(_17)).IsTrue();
        Check.That(_3.Equals(_4)).IsTrue();
        Check.That(_3.Equals(_5)).IsTrue();
        Check.That(_3.Equals(_9)).IsFalse();
        Check.That(_3.Equals(_15)).IsTrue();
        Check.That(_16.Equals(_10)).IsFalse();
        Check.That(_16.Equals(_17)).IsTrue();
        Check.That(_16.Equals(_18)).IsTrue();
        Check.That(_5.Equals(_17)).IsFalse();
        Check.That(_19.Equals(_20)).IsTrue();
    }

    [Test]
    public void FromFile()
    {
        CodeItem _0 = new CodeItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        String _1 = Environment.GetEnvironmentVariable("TEMP") + "\\CodeItem.ts";

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();

        Check.That(_0.ToFile(_1)).IsTrue();

        CodeItem? _2 = CodeItem.FromFile(_1);

        Check.That(_2).IsNotNull();

        Check.That(_0.Equals(_2)).IsTrue();

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();
    }

    [Test]
    public void GetAppDomainID()
    {
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetAppDomainID()).HasAValue();
    }

    [Test]
    public void GetAppDomainUID() 
    {
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetAppDomainUID()).IsNull();
    }

    [Test]
    public void GetApplication()
    {
        CodeItem _0 = new CodeItem();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetAssemblyVersion()
    {
        CodeItem _0 = new CodeItem();

        Version? _1 = Assembly.GetExecutingAssembly().GetName().Version;

        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
    }

    [Test]
    public void GetBornOn()
    {
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public void GetCertificates()
    {
        CodeItem _0 = new CodeItem();
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
        String _2 = Guid.NewGuid().ToString();
        CodeItem _0 = new CodeItem(_2);

        Check.That(_0.GetContent()).Equals(_2);
        Check.That(_0.Lock("key")).IsTrue();;
        Check.That(_0.GetContent()).Equals(_2);
        Check.That(ReferenceEquals(_0.GetContent()!,_2)).IsFalse();
    }

    [Test]
    public void GetCPUID()
    {
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetCPUID()).IsNull();
    }

    [Test]
    public void GetDescriptor()
    {
        CodeItem _0 = new CodeItem();

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
        Check.That(_0.SetType(DataType.KQL)).IsTrue();
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
        Check.That(_.ObjectType).Equals("CodeItem");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Type).Equals(DataType.KQL);
        Check.That(_.Version).IsEqualTo(_v.ToString());
    }

    [Test]
    public void GetDistinguishedName()
    {
        CodeItem _0 = new CodeItem();
        String _1 = "CN=ServiceN,OU=Engineering,DC=TailSpinToys,DC=COM";

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void GetDomainID()
    {
        CodeItem _0 = new CodeItem();
        String _1 = "GetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
    }

    [Test]
    public void GetExtension()
    {
        CodeItem _0 = new CodeItem();
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
        CodeItem _0 = new CodeItem();
        String _1 = "GetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.SetFILE(String.Empty)).IsTrue();
        Check.That(_0.GetFILE()).IsNull();
    }

    [Test]
    public void GetGPS()
    {
        CodeItem _0 = new CodeItem();
        String _1 = "GetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
    }

    [Test]
    public void GetHash()
    {
        CodeItem _0 = new CodeItem(null,Guid.NewGuid(),null,null,null,null);
        CodeItem _1 = new CodeItem(null,new Guid("00021401-0000-0000-C000-000000000046"),null,null,null,null);
        CodeItem _2 = new CodeItem(null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null,null);

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
    }

    [Test]
    public void GetID()
    {
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetID()).HasAValue();
    }

    [Test]
    public void GetLinks()
    {
        CodeItem _0 = new CodeItem();
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
        CodeItem _0 = new CodeItem();
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
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(String.Empty)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetMachineID()
    {
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetMachineID()).HasContent();
    }

    [Test]
    public void GetModified()
    {
        CodeItem _0 = new CodeItem();
        CodeItem _3 = new CodeItem();
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
        CodeItem _0 = new CodeItem();
        String _1 = "GetNameExam";

        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void GetNotes()
    {
        CodeItem _0 = new CodeItem();
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
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetProcessID()).HasAValue();
    }

    [Test]
    public void GetSecureHashes()
    {
        CodeItem _0 = new CodeItem();
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
        CodeItem _0 = new CodeItem();
        String _1 = "GetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void GetServiceVersion() 
    {
        CodeItem _0 = new CodeItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void GetTags()
    {
        CodeItem _0 = new CodeItem();
        List<String> _1 = new List<String>(){"GetTagsExam", "Pass"};

        Check.That(_0.AddTags(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
    }

    [Test]
    public new void GetType()
    {
        CodeItem _0 = new CodeItem();

        Check.That(_0.SetType(DataType.C)).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.C);
    }

    [Test]
    public void GetThreadID()
    {
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetThreadID()).HasAValue();
    }

    [Test]
    public void GetVersion()
    {
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Initialize()
    {
        CodeItem _1 = new CodeItem();

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
        CodeItem _0 = new CodeItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(String.Empty)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        CodeItem _0 = new CodeItem();

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
        CodeItem _0 = new CodeItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        CodeItem _1 = CodeItem.Parse(new CodeItem(new CodeItem(_0.ToString()).ToString()).ToString(),null);

        CodeItem _2 = CodeItem.Parse(_1.GetContent()!,null);

        CodeItem _3 = CodeItem.Parse(_2.GetContent()!,null);

        Check.That(_0.Equals(_3)).IsTrue();
    }

    [Test]
    public void RemoveNote()
    {
        CodeItem _0 = new CodeItem();
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
        CodeItem _0 = new CodeItem();
        String _4 = "RemoveTagExam"; String _5 = "Pass";
        HashSet<String> _1 = new(){_4};
        HashSet<String> _2 = new(){_5};
        HashSet<String> _3 = new(); _3.UnionWith(_1); _3.UnionWith(_2);

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
        CodeItem _0 = new CodeItem();
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion()
    {
        CodeItem _0 = new CodeItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetBornOn()
    {
        CodeItem _0 = new CodeItem();
        DateTimeOffset _1 = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);

        Check.That(_0.GetBornOn()).HasAValue().And.IsNotEqualTo(_1);
        Check.That(_0.SetBornOn(_1)).IsTrue();
        Check.That(_0.GetBornOn()).Equals(_1);
    }

    [Test]
    public void SetCertificates()
    {
        CodeItem _0 = new CodeItem();
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
        String _2 = Guid.NewGuid().ToString();
        CodeItem _0 = new CodeItem();

        Check.That(_0.SetContent(_2)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
        Check.That(_0.Lock("key")).IsTrue();;
        Check.That(_0.GetContent()).Equals(_2);
    }

    [Test]
    public void SetDistinguishedName()
    {
        CodeItem _0 = new CodeItem();
        String _1 = "CN=ServiceN,OU=Operations,DC=Fabrikam,DC=NET";

        Check.That(_0.SetDistinguishedName(null)).IsFalse();
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void SetDomainID()
    {
        CodeItem _0 = new CodeItem();
        String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.SetDomainID(_1)).IsFalse();
    }

    [Test]
    public void SetExtension()
    {
        CodeItem _0 = new CodeItem();
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
        CodeItem _0 = new CodeItem();
        String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
    }

    [Test]
    public void SetGPS()
    {
        CodeItem _0 = new CodeItem();
        String _1 = "SetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
    }

    [Test]
    public void SetID()
    {
        CodeItem _0 = new CodeItem();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void SetLinks()
    {
        CodeItem _0 = new CodeItem();
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
        CodeItem _0 = new CodeItem();
        String _1 = "app://server/SetLocatorExam";
        Uri _2 = new Uri(_1);

        Check.That(_0.SetLocator(null)).IsFalse();
        Check.That(_0.SetLocator(_2)).IsTrue();
        Check.That(_0.GetLocator()!.AbsoluteUri).IsEqualTo(_1);
    }

    [Test]
    public void SetModified()
    {
        CodeItem _0 = new CodeItem();
        CodeItem _3 = new CodeItem();
        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_0.SetModified(null)).IsFalse();
        Check.That(_3.SetModified(_1)).IsTrue();
        Check.That(_3.GetModified()).Equals(_1);
    }

    [Test]
    public void SetName()
    {
        CodeItem _0 = new CodeItem();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void SetSecureHashes()
    {
        CodeItem _0 = new CodeItem();
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
        CodeItem _0 = new CodeItem();
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void SetServiceVersion()
    {
        CodeItem _0 = new CodeItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetType()
    {
        CodeItem _0 = new CodeItem();

        Check.That(_0.SetType(DataType.RAZOR)).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.RAZOR);
        Check.That(_0.SetType(DataType.BAT)).IsFalse();
    }

    [Test]
    public void SetVersion()
    {
        CodeItem _0 = new CodeItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void ToFile()
    {
        CodeItem _0 = new CodeItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        String _1 = Environment.GetEnvironmentVariable("TEMP") + "\\CodeItem8.fs";

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();

        Check.That(_0.ToFile(_1)).IsTrue();

        CodeItem? _2 = CodeItem.FromFile(_1);

        Check.That(_2).IsNotNull();

        Check.That(_0.Equals(_2)).IsTrue();

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();
    }

    [Test]
    public new void ToString()
    {
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);

        CodeItem _1 = new CodeItem(_0); 

        Check.That(_1.SetContent(_0)).IsTrue();

        CodeItem? _2 = CodeItem.Parse(_1.ToString(),null);

        Check.That(_1.Equals(_2)).IsTrue();
    }

    [Test]
    public void TryParse()
    {
        CodeItem _0 = new CodeItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        CodeItem? _1; CodeItem.TryParse(new CodeItem(new CodeItem(_0.ToString()).ToString()).ToString(),null,out _1);

        CodeItem? _2; CodeItem.TryParse(_1!.GetContent()!,null,out _2);

        CodeItem? _3; CodeItem.TryParse(_2!.GetContent()!,null,out _3);

        Check.That(_0.Equals(_3)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        CodeItem _0 = new CodeItem();

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
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);

        CodeItem _3 = new CodeItem(_0,null,null,null,null,type);

        if(Settings.CodeItemValidDataTypes.Contains(type))
        {
            Check.That(CodeItem.Validate(_3)).IsTrue();
        }

        else
        {
            Check.That(CodeItem.Validate(_3)).IsFalse();
        }
    }
}