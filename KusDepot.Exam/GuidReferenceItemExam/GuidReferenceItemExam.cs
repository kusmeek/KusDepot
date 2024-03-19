namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class GuidReferenceItemExam
{
    [Test]
    public void AddNotes()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
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
        GuidReferenceItem _0 = new GuidReferenceItem();
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
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem();

        Check.That(_0.Equals(_0.Clone())).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsTrue();
    }

    [Test]
    public void CompareTo()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),null,null,null,null);
        GuidReferenceItem _2 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null,null);
        GuidReferenceItem _3 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null,null);

        Check.That(new GuidReferenceItem().CompareTo(null)).IsEqualTo(1);
        Check.That(_1.CompareTo(_1)).IsEqualTo(0);
        Check.That(_0.CompareTo(_1)).IsStrictlyNegative();
        Check.That(_1.CompareTo(_0)).IsStrictlyPositive();
        Check.That(_1.CompareTo(_2)).IsStrictlyNegative();
        Check.That(_2.CompareTo(_1)).IsStrictlyPositive();
        Check.That(_2.CompareTo(_3)).IsEqualTo(0);
    }

    [Test]
    public void Constructor()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid(),Guid.NewGuid(),String.Empty,new HashSet<String>(),new HashSet<String>{});

        Check.That(_0).IsInstanceOfType(typeof(GuidReferenceItem));
    }

    [Test]
    public void EqualsObject()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem();

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsTrue();
        Check.That(new GuidReferenceItem().Equals(new GuidReferenceItemSync())).IsFalse();
        Check.That(((Object)new GuidReferenceItem()).Equals(null)).IsFalse();
        Check.That(new GuidReferenceItem().Equals(new Object())).IsFalse();
    }

    [Test]
    public void EqualsInterface()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"));
        GuidReferenceItem _2 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"));
        Guid _3 = Guid.NewGuid();
        GuidReferenceItem _4 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),_3);
        GuidReferenceItem _5 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),_3);
        GuidReferenceItem _8 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"),_3);
        GuidReferenceItem _6 = new GuidReferenceItem(null,_3);
        GuidReferenceItem _7 = new GuidReferenceItem(null,_3);
        GuidReferenceItem _9 = new GuidReferenceItem(null,null,null,null,null);

        Check.That(new GuidReferenceItem().Equals(null)).IsFalse();
        Check.That(new GuidReferenceItem().Equals(new Object())).IsFalse();
        Check.That(new GuidReferenceItem().Equals(_9)).IsTrue();
        Check.That(_0.Equals(_1)).IsFalse();
        Check.That(_1.Equals(_0)).IsFalse();
        Check.That(_1.Equals(_1)).IsTrue();
        Check.That(_2.Equals(_4)).IsTrue();
        Check.That(_4.Equals(_5)).IsTrue();
        Check.That(_6.Equals(_4)).IsFalse();
        Check.That(_6.Equals(_7)).IsTrue();
        Check.That(_6.Equals(_1)).IsFalse();
        Check.That(_6.Equals(_8)).IsFalse();
        Check.That(_6.Equals(_9)).IsTrue();
        Check.That(_1.Equals(_9)).IsFalse();
    }

    [Test]
    public void FromFile()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid(),Guid.NewGuid());

        String _1 = Environment.GetEnvironmentVariable("TEMP") + "\\GuidReferenceItem.guid";

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();

        Check.That(_0.ToFile(_1)).IsTrue();

        GuidReferenceItem? _2 = GuidReferenceItem.FromFile(_1);

        Check.That(_2).IsNotNull();

        Check.That(_0.Equals(_2)).IsTrue();

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();
    }

    [Test]
    public void GetAppDomainID()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetAppDomainID()).HasAValue();
    }

    [Test]
    public void GetAppDomainUID() 
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetAppDomainUID()).IsNull();
    }

    [Test]
    public void GetApplication()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "GetApplication";

        Check.That(_0.SetApplication(String.Empty)).IsTrue();
        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        Version _1 = new Version("0.0.0.0");
        Version _2 = new Version("20.0.0.23");

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()).Equals(_2);
        Check.That(_0.SetApplicationVersion(_1)).IsTrue();
        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetAssemblyVersion()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Version? _1 = Assembly.GetExecutingAssembly().GetName().Version;

        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetAssemblyVersion()).IsEqualTo(_1);
    }

    [Test]
    public void GetBornOn()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public void GetCertificates()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
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
    public void GetContent()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid());

        Check.That(_0.GetContent()).HasAValue();
        Check.That(_0.SetContent(Guid.Empty)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        Check.That(_0.SetContent(Guid.NewGuid())).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetContent()).HasAValue();
    }

    [Test]
    public void GetCPUID()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetCPUID()).IsNull();
    }

    [Test]
    public void GetDescriptor()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

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
        Check.That(_.ObjectType).Equals("GuidReferenceItem");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Type).Equals(DataType.GUID);
        Check.That(_.Version).IsEqualTo(_v.ToString());
    }

    [Test]
    public void GetDistinguishedName()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "CN=ServiceN,OU=Engineering,DC=TailSpinToys,DC=COM";
        
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
        Check.That(_0.SetDistinguishedName(String.Empty)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsNull();
    }

    [Test]
    public void GetDomainID()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "GetDomainIDExam";
        
        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.SetDomainID(String.Empty)).IsTrue();
        Check.That(_0.GetDomainID()).IsNull();
    }

    [Test]
    public void GetExtension()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
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
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "GetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.SetFILE(String.Empty)).IsTrue();
        Check.That(_0.GetFILE()).IsNull();
    }

    [Test]
    public void GetGPS()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "GetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
        Check.That(_0.SetGPS(String.Empty)).IsTrue();
        Check.That(_0.GetGPS()).IsNull();
    }

    [Test]
    public void GetHash()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(null,Guid.NewGuid(),null,null,null);
        GuidReferenceItem _1 = new GuidReferenceItem(null,new Guid("00021401-0000-0000-C000-000000000046"),null,null,null);
        GuidReferenceItem _2 = new GuidReferenceItem(null,new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),null,null,null);

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
    }

    [Test]
    public void GetID()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetID()).HasAValue();
        Check.That(_0.SetID(Guid.Empty)).IsTrue();
        Check.That(_0.GetID()).IsNull();
    }

    [Test]
    public void GetLinks()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
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
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "app://server/GetLocatorExam";
        Uri _2 = new Uri(_1);

        Check.That(_0.SetLocator(_2)).IsTrue();
        Check.That(_0.SetLocator(new Uri("null:"))).IsTrue();
        Check.That(_0.GetLocator()).IsNull();
        Check.That(_0.SetLocator(_2)).IsTrue();
        Check.That(_0.GetLocator()!.AbsoluteUri).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetLocator()!.AbsoluteUri).IsEqualTo(_1);
    }

    [Test]
    public void GetLocked()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        _0.Lock(String.Empty);

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetMachineID()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetMachineID()).HasContent();
    }

    [Test]
    public void GetModified()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _3 = new GuidReferenceItem();
        DateTimeOffset _1 = DateTimeOffset.Now;
                
        Check.That(_0.SetModified(DateTimeOffset.MinValue)).IsTrue();
        Check.That(_0.GetModified()).IsNull();
        Check.That(_3.SetModified(_1)).IsTrue();
        Check.That(_3.GetModified()).Equals(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_3.GetModified()).Equals(_1);
    }

    [Test]
    public void GetName()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "GetNameExam";

        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
        Check.That(_0.SetName(String.Empty)).IsTrue();
        Check.That(_0.GetName()).IsNull();
    }

    [Test]
    public void GetNext()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem();

        Check.That(_0.SetNext(_1)).IsTrue();
        Check.That(_0.GetNext()).IsEqualTo(_1);
        Check.That(_0.SetNext(null)).IsTrue();
        Check.That(_0.GetNext()).IsNull();
        Check.That(_0.SetNext(_1)).IsTrue();
        Check.That(_0.GetNext()).IsEqualTo(_1);
        Check.That(_0.Lock("Key")).IsTrue();
        Check.That(_0.GetNext()).IsEqualTo(_1);
    }

    [Test]
    public void GetNotes()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _2 = "GetNotesExam";
        List<String> _1 = new(){_2,"Pass"};

        Check.That(_0.AddNotes(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
        Check.That(_0.Lock(_2)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
    }

    [Test]
    public void GetPrevious()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem(Guid.NewGuid());

        Check.That(_0.GetPrevious()).IsNull();
        Check.That(_0.SetPrevious(_1)).IsTrue();
        Check.That(_0.GetPrevious()!.GetContent()).Equals(_1.GetContent());
        Check.That(_0.SetPrevious(null)).IsTrue();
        Check.That(_0.GetPrevious()).IsNull();
        Check.That(_0.SetPrevious(_1)).IsTrue();
        Check.That(_0.GetPrevious()!.GetContent()).Equals(_1.GetContent());
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetPrevious()!.GetContent()).Equals(_1.GetContent());
    }

    [Test]
    public void GetProcessID()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetProcessID()).HasAValue();
    }

    [Test]
    public void GetSecureHashes()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
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
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "GetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
        Check.That(_0.SetSecurityDescriptor(String.Empty)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsNull();
    }

    [Test]
    public void GetServiceVersion() 
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
        Check.That(_0.SetServiceVersion(new Version("0.0.0.0"))).IsTrue();
        Check.That(_0.GetServiceVersion()).IsNull();
    }

    [Test]
    public void GetTags()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        List<String> _1 = new List<String>(){"GetTagsExam", "Pass"};

        Check.That(_0.AddTags(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
    }

    [Test]
    public new void GetType()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetType()).Equals(DataType.GUID);
    }

    [Test]
    public void GetThreadID()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetThreadID()).HasAValue();
    }

    [Test]
    public void GetUndirectedLinks()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem();
        GuidReferenceItem _2 = new GuidReferenceItem();
        List<GuidReferenceItem> _3 = new List<GuidReferenceItem>(){_1,_2};

        Check.That(Check.That(_0.SetUndirectedLinks(_3.ToHashSet())).IsTrue());
        Check.That(_0.GetUndirectedLinks()).ContainsExactly(_3);
    }

    [Test]
    public void GetVersion()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Initialize()
    {
        GuidReferenceItem _1 = new GuidReferenceItem();

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
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        _0.Lock(String.Empty);

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

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
        Check.That(_0.GetLocked()).IsFalse();
        Check.That(_0.GetMachineID()).IsNotNull();
        Check.That(_0.GetModified()).IsNull();
        Check.That(_0.GetName()).IsNull();
        Check.That(_0.GetNext()).IsNull();
        Check.That(_0.GetNotes()).IsNull();
        Check.That(_0.GetPrevious()).IsNull();
        Check.That(_0.GetProcessID()).IsNotNull();
        Check.That(_0.GetSecureHashes()).IsNull();
        Check.That(_0.GetSecurityDescriptor()).IsNull();
        Check.That(_0.GetServiceVersion()).IsNull();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.GetThreadID()).IsNotNull();
        Check.That(_0.GetType()).Equals(DataType.GUID);
        Check.That(_0.GetUndirectedLinks()).IsNull();
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Parse()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid(),Guid.NewGuid(),null,null,null);

        GuidReferenceItem _1 = GuidReferenceItem.Parse(_0.ToString(),null);

        Check.That(_1).IsInstanceOfType(typeof(GuidReferenceItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void RemoveNote()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
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
        GuidReferenceItem _0 = new GuidReferenceItem();
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
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetBornOn()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        DateTimeOffset _1 = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);

        Check.That(_0.GetBornOn()).HasAValue().And.IsNotEqualTo(_1);
        Check.That(_0.SetBornOn(_1)).IsTrue();
        Check.That(_0.GetBornOn()).Equals(_1);
    }

    [Test]
    public void SetCertificates()
    {
        GuidReferenceItem _0 = new();
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
        GuidReferenceItem _0 = new GuidReferenceItem();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetContent(_1)).IsTrue();
        Check.That(_0.GetContent()!).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.SetContent(_1)).IsFalse();
    }

    [Test]
    public void SetDistinguishedName()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "CN=ServiceN,OU=Operations,DC=Fabrikam,DC=NET";

        Check.That(_0.SetDistinguishedName(null)).IsFalse();
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void SetDomainID()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.SetDomainID(_1)).IsFalse();
    }

    [Test]
    public void SetExtension()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
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
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
    }

    [Test]
    public void SetGPS()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "SetGPSExam";

        Check.That(_0.SetGPS(_1)).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetGPS()).IsEqualTo(_1);
    }

    [Test]
    public void SetID()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void SetLinks()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
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
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "app://server/SetLocatorExam";
        String _3 = "null:";
        Uri _2 = new Uri(_1);
        Uri _4 = new Uri(_3);

        Check.That(_0.SetLocator(null)).IsFalse();
        Check.That(_0.SetLocator(_2)).IsTrue();
        Check.That(_0.GetLocator()!.AbsoluteUri).IsEqualTo(_1);
        Check.That(_0.SetLocator(_4)).IsTrue();
        Check.That(_0.GetLocator()?.AbsoluteUri).IsNull();
    }

    [Test]
    public void SetModified()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _3 = new GuidReferenceItem();
        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_0.SetModified(null)).IsFalse();
        Check.That(_3.SetModified(_1)).IsTrue();
        Check.That(_3.GetModified()).Equals(_1);
    }

    [Test]
    public void SetName()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void SetNext()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem(Guid.NewGuid());

        Check.That(_0.GetNext()).IsNull();
        Check.That(_0.SetNext(_1)).IsTrue();
        Check.That(_0.GetNext()!.GetContent()).Equals(_1.GetContent());
    }

    [Test]
    public void SetPrevious()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem(Guid.NewGuid());

        Check.That(_0.GetPrevious()).IsNull();
        Check.That(_0.SetPrevious(_1)).IsTrue();
        Check.That(_0.GetPrevious()!.GetContent()).Equals(_1.GetContent());
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.SetPrevious(_1)).IsFalse();
    }

    [Test]
    public void SetSecureHashes()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
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
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void SetServiceVersion()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetType()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.SetType(DataType.GUID)).IsTrue();
        Check.That(_0.Lock("key")).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.GUID);
        Check.That(_0.SetType(DataType.GUID)).IsFalse();
    }

    [Test]
    public void SetUndirectedLinks()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem();
        GuidReferenceItem _2 = new GuidReferenceItem();
        List<GuidReferenceItem> _3 = new List<GuidReferenceItem>(){_1,_2};

        Check.That(_0.Lock("key")).IsTrue();
        Check.That(Check.That(_0.SetUndirectedLinks(_3.ToHashSet())).IsFalse());
        Check.That(_0.UnLock("key")).IsTrue();
        Check.That(Check.That(_0.SetUndirectedLinks(_3.ToHashSet())).IsTrue());
        Check.That(_0.GetUndirectedLinks()).ContainsExactly(_3);
        _3.Clear();
        Check.That(Check.That(_0.SetUndirectedLinks(_3.ToHashSet())).IsTrue());
        Check.That(_0.GetUndirectedLinks()).IsNull();
    }

    [Test]
    public void SetVersion()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion()!.ToString()).IsEqualTo(_1);
        Check.That(_0.SetVersion(new Version("0.0.0.0"))).IsTrue();
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void ToFile()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid(),Guid.NewGuid());

        String _1 = Environment.GetEnvironmentVariable("TEMP") + "\\GuidReferenceItem09.guid";

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();

        Check.That(_0.ToFile(_1)).IsTrue();

        GuidReferenceItem? _2 = GuidReferenceItem.FromFile(_1);

        Check.That(_2).IsNotNull();

        Check.That(_0.Equals(_2)).IsTrue();

        File.Delete(_1);

        Check.That(File.Exists(_1)).IsFalse();
    }

    [Test]
    public new void ToString()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid(),Guid.NewGuid());

        GuidReferenceItem? _1 = GuidReferenceItem.Parse(_0.ToString(),null);

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void TryParse()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid(),Guid.NewGuid(),null,null,null);

        GuidReferenceItem? _1;

        GuidReferenceItem.TryParse(_0.ToString(),null,out _1);

        Check.That(_1).IsInstanceOfType(typeof(GuidReferenceItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

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
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid(),Guid.NewGuid(),null,null,null);

        Check.That(GuidReferenceItem.Validate(_0)).IsTrue();

        Check.That(GuidReferenceItem.Validate(new GuidReferenceItem())).IsFalse();

        Check.That(_0.SetType(DataType.GIF)).IsTrue();

        Check.That(GuidReferenceItem.Validate(_0)).IsFalse();
    }
}