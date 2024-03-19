namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class MSBuildItemExam
{
    [Test]
    public void AddNotes()
    {
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
        MSBuildItem _1 = new MSBuildItem();

        Check.That(_0.Equals(_0.Clone())).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsTrue();
        Check.That(_1.SetContent("CloneExam")).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsFalse();
    }

    [Test]
    public void CompareTo()
    {
        MSBuildItem _0 = new MSBuildItem();
        Thread.Sleep(100);
        MSBuildItem _1 = new MSBuildItem();

        Check.That(new MSBuildItem().CompareTo(null)).IsEqualTo(1);
        Check.That(_0.CompareTo(_0)).IsEqualTo(0);
        Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
        Check.That(_1.CompareTo(_0)).IsStrictlyPositive();
    }

    [Test]
    public void Constructor()
    {
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        MSBuildItem _3 = new MSBuildItem(_0,Guid.NewGuid());

        Check.That(_3).IsInstanceOfType(typeof(MSBuildItem));
        Check.That(_3.GetContent()).IsEqualTo(_0.ToString());
    }

    [Test]
    public void EqualsObject()
    {
        MSBuildItem _0 = new MSBuildItem();
        MSBuildItem _1 = new MSBuildItem();

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsTrue();
        Check.That(((Object)new MSBuildItem()).Equals(new MSBuildItemSync())).IsFalse();
        Check.That(((Object)new MSBuildItem()).Equals(null)).IsFalse();
        Check.That(((Object)new MSBuildItem()).Equals(new Object())).IsFalse();
    }

    [Test]
    public void EqualsInterface()
    {
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        String _6 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        MSBuildItem _3 = new MSBuildItem(_0,_11,String.Empty); _3.SetName("_3");
        MSBuildItem _15 = new MSBuildItem(_0,_11,String.Empty); _15.SetName("_15");
        MSBuildItem _4 = new MSBuildItem(_0,_14,String.Empty); _4.SetName("_4");
        MSBuildItem _5 = new MSBuildItem(_0,null,String.Empty); _5.SetName("_5");
        MSBuildItem _9 = new MSBuildItem(_6,_11,String.Empty); _9.SetName("_9");
        MSBuildItem _10 = new MSBuildItem(_6,_14,String.Empty); _10.SetName("_10");
        MSBuildItem _16 = new MSBuildItem(null,_14,String.Empty); _16.SetName("_16");
        MSBuildItem _17 = new MSBuildItem(null,null,String.Empty); _17.SetName("_17");
        MSBuildItem _18 = new MSBuildItem(null,_14,String.Empty); _18.SetName("_18");
        MSBuildItem _19 = new MSBuildItem(String.Empty,null); _19.SetName("_19");
        MSBuildItem _20 = new MSBuildItem(String.Empty,null); _20.SetName("_20");

        SortedList<Int32,MSBuildItem> _13 = new SortedList<Int32,MSBuildItem>(); _13.Add(0,_10); _13.Add(1,_5);
        HashSet<DataItem> _12 = new HashSet<DataItem>(); _12.Add(_3);
        MSBuildItem _21 = new MSBuildItem(); _21.SetName("_21");
        MSBuildItem _22 = new MSBuildItem(); _22.SetName("_22");
        MSBuildItem _23 = new MSBuildItem(); _23.SetName("_23");

        Check.That(new MSBuildItem().Equals(null)).IsFalse();
        Check.That(new MSBuildItem().Equals(new Object())).IsFalse();
        Check.That(new MSBuildItem().Equals(_17)).IsTrue();
        Check.That(_3.Equals(_4)).IsTrue();
        Check.That(_3.Equals(_5)).IsTrue();
        Check.That(_3.Equals(_9)).IsFalse();
        Check.That(_3.Equals(_15)).IsTrue();
        Check.That(_16.Equals(_10)).IsFalse();
        Check.That(_16.Equals(_17)).IsTrue();
        Check.That(_16.Equals(_18)).IsTrue();
        Check.That(_5.Equals(_17)).IsFalse();
        Check.That(_19.Equals(_20)).IsTrue();
        Check.That(_21.SetRequirements(_12)).IsTrue();
        Check.That(_21.SetSequence(_13)).IsTrue();
        Check.That(_22.SetSequence(_13)).IsTrue();
        Check.That(_22.SetRequirements(_12)).IsTrue();
        Check.That(_21.Equals(_22)).IsTrue();
        Check.That(_23.SetRequirements(_12)).IsTrue();
        Check.That(_21.Equals(_23)).IsFalse();
    }

    [Test]
    public void FromFile()
    {
        MSBuildItem _0 = new MSBuildItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        String _1 = Environment.GetEnvironmentVariable("TEMP") + "\\MSBuildItem.bld";

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();

        Check.That(_0.ToFile(_1)).IsTrue();

        MSBuildItem? _2 = MSBuildItem.FromFile(_1);

        Check.That(_2).IsNotNull();

        Check.That(_0.Equals(_2)).IsTrue();

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();
    }

    [Test]
    public void GetAppDomainID()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetAppDomainID()).HasAValue();
    }

    [Test]
    public void GetAppDomainUID() 
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetAppDomainUID()).IsNull();
    }

    [Test]
    public void GetApplication()
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetAssemblyVersion()
    {
        MSBuildItem _0 = new MSBuildItem();

        Version? _1 = Assembly.GetExecutingAssembly().GetName().Version;

        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
    }

    [Test]
    public void GetBornOn()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public void GetCertificates()
    {
        MSBuildItem _0 = new MSBuildItem();
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
        String _1 = "GetContentExam";
        MSBuildItem _0 = new MSBuildItem(_1);

        Check.That(_0.GetContent()).Equals(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetContent()).Equals(_1);
        Check.That(ReferenceEquals(_0.GetContent()!,_1)).IsFalse();
    }

    [Test]
    public void GetCPUID()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetCPUID()).IsNull();
    }

    [Test]
    public void GetDescriptor()
    {
        MSBuildItem _0 = new MSBuildItem();

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
        Check.That(_.ObjectType).Equals("MSBuildItem");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Type).Equals(DataType.MSB);
        Check.That(_.Version).IsEqualTo(_v.ToString());
    }

    [Test]
    public void GetDistinguishedName()
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "CN=ServiceN,OU=Engineering,DC=TailSpinToys,DC=COM";

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void GetDomainID()
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "GetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
    }

    [Test]
    public void GetExtension()
    {
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "GetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.SetFILE(String.Empty)).IsTrue();
        Check.That(_0.GetFILE()).IsNull();
    }

    [Test]
    public void GetGPS()
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "GetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
    }

    [Test]
    public void GetHash()
    {
        MSBuildItem _0 = new MSBuildItem(null,Guid.NewGuid(),String.Empty,null,null);
        MSBuildItem _1 = new MSBuildItem(null,new Guid("00021401-0000-0000-C000-000000000046"),String.Empty,null,null);
        MSBuildItem _2 = new MSBuildItem(null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),String.Empty,null,null);

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
    }

    [Test]
    public void GetID()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetID()).HasAValue();
    }

    [Test]
    public void GetLinks()
    {
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(String.Empty)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetMachineID()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetMachineID()).HasContent();
    }

    [Test]
    public void GetModified()
    {
        MSBuildItem _0 = new MSBuildItem();
        MSBuildItem _3 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "GetNameExam";

        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void GetNotes()
    {
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetProcessID()).HasAValue();
    }

    [Test]
    public void GetRequirements()
    {
        String _2 = "GetRequirementsExam";
        MSBuildItem _0 = new MSBuildItem();
        MSBuildItem _1 = new MSBuildItem(_2);
        HashSet<DataItem> _3 = new HashSet<DataItem>();
        _3.Add(_1);

        Check.That(_0.SetRequirements(_3)).IsTrue();
        Check.That(_0.GetRequirements()).Contains(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetRequirements()).Contains(_1);
    }

    [Test]
    public void GetSecureHashes()
    {
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "GetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void GetSequence()
    {
        String _2 = "GetSequenceExam";
        MSBuildItem _0 = new MSBuildItem();
        MSBuildItem _1 = new MSBuildItem(_2);
        SortedList<Int32,MSBuildItem> _3 = new SortedList<Int32,MSBuildItem>();
        _3.Add(0,_1);

        Check.That(_0.SetSequence(_3)).IsTrue();
        Check.That(_0.GetSequence()![0]).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetSequence()![0]).IsEqualTo(_1);
    }

    [Test]
    public void GetServiceVersion() 
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void GetTags()
    {
        MSBuildItem _0 = new MSBuildItem();
        List<String> _1 = new List<String>(){"GetTagsExam", "Pass"};

        Check.That(_0.AddTags(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
    }

    [Test]
    public new void GetType()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.SetType(DataType.MSB)).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.MSB);
    }

    [Test]
    public void GetThreadID()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetThreadID()).HasAValue();
    }

    [Test]
    public void GetVersion()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Initialize()
    {
        MSBuildItem _1 = new MSBuildItem();

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
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(String.Empty)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        MSBuildItem _0 = new MSBuildItem();

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
        Check.That(_0.GetRequirements()).IsNull();
        Check.That(_0.GetSecureHashes()).IsNull();
        Check.That(_0.GetSecurityDescriptor()).IsNull();
        Check.That(_0.GetSequence()).IsNull();
        Check.That(_0.GetServiceVersion()).IsNull();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.GetThreadID()).IsNotNull();
        Check.That(_0.GetType()).Equals(DataType.MSB);
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Parse()
    {
        MSBuildItem _0 = new MSBuildItem(null,Guid.NewGuid(),null,null,null);

        MSBuildItem _1 = MSBuildItem.Parse(_0.ToString(),null);

        Check.That(_1).IsInstanceOfType(typeof(MSBuildItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void RemoveNote()
    {
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion()
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetBornOn()
    {
        MSBuildItem _0 = new MSBuildItem();
        DateTimeOffset _1 = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);

        Check.That(_0.GetBornOn()).HasAValue().And.IsNotEqualTo(_1);
        Check.That(_0.SetBornOn(_1)).IsTrue();
        Check.That(_0.GetBornOn()).Equals(_1);
    }

    [Test]
    public void SetCertificates()
    {
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
        String _1 = Guid.NewGuid().ToString();

        Check.That(_0.SetContent(_1)).IsTrue();
        Check.That(_0.GetContent()!).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.SetContent(_1)).IsFalse();
    }

    [Test]
    public void SetDistinguishedName()
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "CN=ServiceN,OU=Operations,DC=Fabrikam,DC=NET";

        Check.That(_0.SetDistinguishedName(null)).IsFalse();
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void SetDomainID()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.SetDomainID(_1)).IsFalse();
    }

    [Test]
    public void SetExtension()
    {
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
    }

    [Test]
    public void SetGPS()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "SetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
    }

    [Test]
    public void SetID()
    {
        MSBuildItem _0 = new MSBuildItem();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void SetLinks()
    {
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "app://server/SetLocatorExam";
        Uri _2 = new Uri(_1);

        Check.That(_0.SetLocator(null)).IsFalse();
        Check.That(_0.SetLocator(_2)).IsTrue();
        Check.That(_0.GetLocator()!.AbsoluteUri).IsEqualTo(_1);
    }

    [Test]
    public void SetModified()
    {
        MSBuildItem _0 = new MSBuildItem();
        MSBuildItem _3 = new MSBuildItem();
        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_0.SetModified(null)).IsFalse();
        Check.That(_3.SetModified(_1)).IsTrue();
        Check.That(_3.GetModified()).Equals(_1);
    }

    [Test]
    public void SetName()
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void SetRequirements()
    {
        String _2 = "GetRequirementsExam";
        MSBuildItem _0 = new MSBuildItem();
        MSBuildItem _1 = new MSBuildItem(_2);
        HashSet<DataItem> _3 = new HashSet<DataItem>();
        _3.Add(_1);

        Check.That(_0.SetRequirements(_3)).IsTrue();
        Check.That(_0.GetRequirements()!.Contains(_1));
    }

    [Test]
    public void SetSecureHashes()
    {
        MSBuildItem _0 = new MSBuildItem();
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
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void SetServiceVersion()
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetSequence()
    {
        String _2 = "SetSequenceExam";
        MSBuildItem _0 = new MSBuildItem();
        MSBuildItem _1 = new MSBuildItem(_2);
        SortedList<Int32,MSBuildItem> _3 = new SortedList<Int32,MSBuildItem>();
        _3.Add(0,_1);

        Check.That(_0.SetSequence(_3)).IsTrue();
        Check.That(_0.GetSequence()![0]).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetSequence()![0]).IsEqualTo(_1);
    }

    [Test]
    public void SetType()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.SetType(DataType.MSB)).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.MSB);
        Check.That(_0.SetType(DataType.MSB)).IsFalse();
    }

    [Test]
    public void SetVersion()
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void ToFile()
    {
        MSBuildItem _0 = new MSBuildItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        String _1 = Environment.GetEnvironmentVariable("TEMP") + "\\MSBuildItem8.bld";

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();

        Check.That(_0.ToFile(_1)).IsTrue();

        MSBuildItem? _2 = MSBuildItem.FromFile(_1);

        Check.That(_2).IsNotNull();

        Check.That(_0.Equals(_2)).IsTrue();

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();
    }

    [Test]
    public new void ToString()
    {
        MSBuildItem _0 = new MSBuildItem(TestCaseDataGenerator.GenerateUnicodeString(8192),null,String.Empty,null,null);

        Check.That(_0.SetRequirements(new HashSet<DataItem>(){new TextItem(TestCaseDataGenerator.GenerateUnicodeString(8192))})).IsTrue();

        MSBuildItem? _2 = MSBuildItem.Parse(_0.ToString(),null);

        Check.That(_0.Equals(_2)).IsTrue();
    }

    [Test]
    public void TryParse()
    {
        MSBuildItem _0 = new MSBuildItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        MSBuildItem? _1; MSBuildItem.TryParse(new MSBuildItem(new MSBuildItem(_0.ToString()).ToString()).ToString(),null,out _1);

        MSBuildItem? _2; MSBuildItem.TryParse(_1!.GetContent()!,null,out _2);

        MSBuildItem? _3; MSBuildItem.TryParse(_2!.GetContent()!,null,out _3);

        Check.That(_0.Equals(_3)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        MSBuildItem _0 = new MSBuildItem();

        String _1 = "UnLockExam";

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(_1)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock("False")).IsFalse();

        Check.That(_0.UnLock(_1)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }

    [Test]
    public void Validate()
    {
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);

        MSBuildItem _3 = new MSBuildItem(_0);

        Check.That(MSBuildItem.Validate(_3)).IsTrue();
    }
}