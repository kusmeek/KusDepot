namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class TextItemExam
{
    [Test]
    public void AddNotes()
    {
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem();
        TextItem _1 = new TextItem();

        Check.That(_0.Equals(_0.Clone())).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsTrue();
        Check.That(_1.SetContent("CloneExam")).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsFalse();
    }

    [Test]
    public void CompareTo()
    {
        TextItem _0 = new TextItem();
        Thread.Sleep(100);
        TextItem _1 = new TextItem();

        Check.That(new TextItem().CompareTo(null)).IsEqualTo(1);
        Check.That(_0.CompareTo(_0)).IsEqualTo(0);
        Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
        Check.That(_1.CompareTo(_0)).IsStrictlyPositive();
    }

    [Test]
    public void Constructor()
    {
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        String? _3 = Settings.TextItemValidDataTypes[RandomNumberGenerator.GetInt32(0,Settings.TextItemValidDataTypes.Length)];
        TextItem _4 = new TextItem(_0,Guid.NewGuid(),String.Empty,new HashSet<String>{},new HashSet<String>{},_3!.ToString());

        Check.That(_4).IsInstanceOfType(typeof(TextItem));
        Check.That(_4.GetContent()).IsEqualTo(_0.ToString());
    }

    [Test]
    public void EqualsObject()
    {
        TextItem _0 = new TextItem();
        TextItem _1 = new TextItem();

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsTrue();
        Check.That(((Object)new TextItem()).Equals(new TextItemSync())).IsFalse();
        Check.That(((Object)new TextItem()).Equals(null)).IsFalse();
        Check.That(((Object)new TextItem()).Equals(new Object())).IsFalse();
    }

    [Test]
    public void EqualsInterface()
    {
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        String _6 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        TextItem _3 = new TextItem(_0,_11);
        TextItem _15 = new TextItem(_0,_11);
        TextItem _4 = new TextItem(_0,_14);
        TextItem _5 = new TextItem(_0);
        TextItem _9 = new TextItem(_6,_11);
        TextItem _10 = new TextItem(_6,_14);
        TextItem _16 = new TextItem(null,_14);
        TextItem _17 = new TextItem(null,null,null,null,null,null);
        TextItem _18 = new TextItem(null,_14);

        Check.That(new TextItem().Equals(null)).IsFalse();
        Check.That(new TextItem().Equals(new Object())).IsFalse();
        Check.That(new TextItem().Equals(_17)).IsTrue();
        Check.That(_3.Equals(_4)).IsTrue();
        Check.That(_3.Equals(_5)).IsTrue();
        Check.That(_3.Equals(_9)).IsFalse();
        Check.That(_3.Equals(_15)).IsTrue();
        Check.That(_16.Equals(_10)).IsFalse();
        Check.That(_16.Equals(_17)).IsTrue();
        Check.That(_16.Equals(_18)).IsTrue();
        Check.That(_5.Equals(_17)).IsFalse();
        Check.That(_5.Equals(_18)).IsFalse();
    }

    [Test]
    public void FromFile()
    {
        TextItem _0 = new TextItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        String _1 = Environment.GetEnvironmentVariable("TEMP") + "\\TextItem7.txt";

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();

        Check.That(_0.ToFile(_1)).IsTrue();

        TextItem? _2 = TextItem.FromFile(_1);

        Check.That(_2).IsNotNull();

        Check.That(_0.Equals(_2)).IsTrue();

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();
    }

    [Test]
    public void GetAppDomainID()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetAppDomainID()).HasAValue();
    }

    [Test]
    public void GetAppDomainUID() 
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetAppDomainUID()).IsNull();
    }

    [Test]
    public void GetApplication()
    {
        TextItem _0 = new TextItem();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetAssemblyVersion()
    {
        TextItem _0 = new TextItem();

        Version? _1 = Assembly.GetExecutingAssembly().GetName().Version;

        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
    }

    [Test]
    public void GetBornOn()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public void GetCertificates()
    {
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem(_2);

        Check.That(_0.GetContent()).Equals(_2);
        Check.That(_0.Lock("key")).IsTrue();;
        Check.That(_0.GetContent()).Equals(_2);
        Check.That(ReferenceEquals(_0.GetContent()!,_2)).IsFalse();
    }

    [Test]
    public void GetCPUID()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetCPUID()).IsNull();
    }

    [Test]
    public void GetDescriptor()
    {
        TextItem _0 = new TextItem();

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
        Check.That(_0.SetType(DataType.NFO)).IsTrue();
        Check.That(_0.SetVersion(_v)).IsTrue();

        Descriptor ? _ = _0.GetDescriptor();

        Check.That(_!.Application).IsEqualTo(_s);
        Check.That(_.ApplicationVersion).IsEqualTo(_v.ToString());
        Check.That(_.BornOn).Equals(_0.GetBornOn()?.ToString("O"));
        Check.That(_.DistinguishedName).IsEqualTo(_s);
        Check.That(_.ID).Equals(_0.GetID());
        Check.That(_.Modified).IsEqualTo(_d.ToString("O"));
        Check.That(_.Name).IsEqualTo(_s);
        Check.That(_.Notes).Contains(_n);
        Check.That(_.ObjectType).Equals("TextItem");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Type).Equals(DataType.NFO);
        Check.That(_.Version).IsEqualTo(_v.ToString());
    }

    [Test]
    public void GetDistinguishedName()
    {
        TextItem _0 = new TextItem();
        String _1 = "CN=ServiceN,OU=Engineering,DC=TailSpinToys,DC=COM";

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void GetDomainID()
    {
        TextItem _0 = new TextItem();
        String _1 = "GetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
    }

    [Test]
    public void GetExtension()
    {
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem();
        String _1 = "GetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.SetFILE(String.Empty)).IsTrue();
        Check.That(_0.GetFILE()).IsNull();
    }

    [Test]
    public void GetGPS()
    {
        TextItem _0 = new TextItem();
        String _1 = "GetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
    }

    [Test]
    public void GetHash()
    {
        TextItem _0 = new TextItem(null,Guid.NewGuid(),null,null,null,null);
        TextItem _1 = new TextItem(null,new Guid("00021401-0000-0000-C000-000000000046"),null,null,null,null);
        TextItem _2 = new TextItem(null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null,null);

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
    }

    [Test]
    public void GetID()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetID()).HasAValue();
    }

    [Test]
    public void GetLanguage()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.SetLanguage(KusDepot.Language.swa)).IsTrue();
        Check.That(_0.GetLanguage()).IsEqualTo(Language.swa);
    }

    [Test]
    public void GetLinks()
    {
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(String.Empty)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetMachineID()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetMachineID()).HasContent();
    }

    [Test]
    public void GetModified()
    {
        TextItem _0 = new TextItem();
        TextItem _3 = new TextItem();
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
        TextItem _0 = new TextItem();
        String _1 = "GetNameExam";

        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void GetNotes()
    {
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem();

        Check.That(_0.GetProcessID()).HasAValue();
    }

    [Test]
    public void GetSecureHashes()
    {
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem();
        String _1 = "GetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void GetServiceVersion() 
    {
        TextItem _0 = new TextItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void GetTags()
    {
        TextItem _0 = new TextItem();
        List<String> _1 = new List<String>(){"GetTagsExam", "Pass"};

        Check.That(_0.AddTags(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
    }

    [Test]
    public new void GetType()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.SetType(DataType.NFO)).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.NFO);
    }

    [Test]
    public void GetThreadID()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetThreadID()).HasAValue();
    }

    [Test]
    public void GetVersion()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Initialize()
    {
        TextItem _1 = new TextItem();

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
        TextItem _0 = new TextItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(String.Empty)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        TextItem _0 = new TextItem();

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
        Check.That(_0.GetLanguage()).IsNull();
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
        TextItem _0 = new TextItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        TextItem _1 = TextItem.Parse(new TextItem(new TextItem(_0.ToString()).ToString()).ToString(),null);

        TextItem _2 = TextItem.Parse(_1.GetContent()!,null);

        TextItem _3 = TextItem.Parse(_2.GetContent()!,null);

        Check.That(_0.Equals(_3)).IsTrue();
    }

    [Test]
    public void RemoveNote()
    {
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem();
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion()
    {
        TextItem _0 = new TextItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetBornOn()
    {
        TextItem _0 = new TextItem();
        DateTimeOffset _1 = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);

        Check.That(_0.GetBornOn()).HasAValue().And.IsNotEqualTo(_1);
        Check.That(_0.SetBornOn(_1)).IsTrue();
        Check.That(_0.GetBornOn()).Equals(_1);
    }

    [Test]
    public void SetCertificates()
    {
        TextItem _0 = new TextItem();
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
    public void SetContent()
    {
        String _2 = Guid.NewGuid().ToString();
        TextItem _0 = new TextItem();

        Check.That(_0.SetContent(_2)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
        Check.That(_0.Lock("key")).IsTrue();;
        Check.That(_0.GetContent()).Equals(_2);
    }

    [Test]
    public void SetDistinguishedName()
    {
        TextItem _0 = new TextItem();
        String _1 = "CN=ServiceN,OU=Operations,DC=Fabrikam,DC=NET";

        Check.That(_0.SetDistinguishedName(null)).IsFalse();
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void SetDomainID()
    {
        TextItem _0 = new TextItem();
        String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.SetDomainID(_1)).IsFalse();
    }

    [Test]
    public void SetFILE()
    {
        TextItem _0 = new TextItem();
        String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
    }

    [Test]
    public void SetExtension()
    {
        TextItem _0 = new TextItem();
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
    public void SetGPS()
    {
        TextItem _0 = new TextItem();
        String _1 = "SetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
    }

    [Test]
    public void SetID()
    {
        TextItem _0 = new TextItem();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void SetLanguage()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.SetLanguage(KusDepot.Language.swa)).IsTrue();
        Check.That(_0.GetLanguage()).IsEqualTo(Language.swa);
    }

    [Test]
    public void SetLinks()
    {
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem();
        String _1 = "app://server/SetLocatorExam";
        Uri _2 = new Uri(_1);

        Check.That(_0.SetLocator(null)).IsFalse();
        Check.That(_0.SetLocator(_2)).IsTrue();
        Check.That(_0.GetLocator()!.AbsoluteUri).IsEqualTo(_1);
    }

    [Test]
    public void SetModified()
    {
        TextItem _0 = new TextItem();
        TextItem _3 = new TextItem();
        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_0.SetModified(null)).IsFalse();
        Check.That(_3.SetModified(_1)).IsTrue();
        Check.That(_3.GetModified()).Equals(_1);
    }

    [Test]
    public void SetName()
    {
        TextItem _0 = new TextItem();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void SetSecureHashes()
    {
        TextItem _0 = new TextItem();
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
        TextItem _0 = new TextItem();
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void SetServiceVersion()
    {
        TextItem _0 = new TextItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetType()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.SetType(DataType.NFO)).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.NFO);
        Check.That(_0.SetType(DataType.TXT)).IsFalse();
    }

    [Test]
    public void SetVersion()
    {
        TextItem _0 = new TextItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void ToFile()
    {
        TextItem _0 = new TextItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        String _1 = Environment.GetEnvironmentVariable("TEMP") + "\\TextItem12.txt";

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();

        Check.That(_0.ToFile(_1)).IsTrue();

        TextItem? _2 = TextItem.FromFile(_1);

        Check.That(_2).IsNotNull();

        Check.That(_0.Equals(_2)).IsTrue();

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();
    }

    [Test]
    public new void ToString()
    {
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);

        TextItem _1 = new TextItem(_0); 

        Check.That(_1.SetContent(_0)).IsTrue();

        TextItem? _2 = TextItem.Parse(_1.ToString(),null);

        Check.That(_1.Equals(_2)).IsTrue();
    }

    [Test]
    public void TryParse()
    {
        TextItem _0 = new TextItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        TextItem? _1; TextItem.TryParse(new TextItem(new TextItem(_0.ToString()).ToString()).ToString(),null,out _1);

        TextItem? _2; TextItem.TryParse(_1!.GetContent()!,null,out _2);

        TextItem? _3; TextItem.TryParse(_2!.GetContent()!,null,out _3);

        Check.That(_0.Equals(_3)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        TextItem _0 = new TextItem();

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

        TextItem _3 = new TextItem(_0,null,null,null,null,type);

        if(Settings.TextItemValidDataTypes.Contains(type))
        {
            Check.That(TextItem.Validate(_3)).IsTrue();
        }

        else
        {
            Check.That(TextItem.Validate(_3)).IsFalse();
        }
    }
}