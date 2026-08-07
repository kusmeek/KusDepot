namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public partial class MetaBaseExam
{
    private ManagerKey? KeyM; private ManagerKey? KeyX;

    private static void PauseClock() { Thread.Sleep(20); }

    [Test]
    public void AddNotes()
    {
        MetaBaseTest _0 = new();
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
    public void AddNotes_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new();
        HashSet<String> _1 = new(){"AddNotesExam"};

        Check.That(_0.AddNotes(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.AddNotes(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void AddTags()
    {
        MetaBaseTest _0 = new();
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

    [Test]
    public void AddTags_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new();
        HashSet<String> _1 = new(){"AddTagsExam"};

        Check.That(_0.AddTags(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.AddTags(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [OneTimeSetUp]
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }

        KeyM = new( Utility.SerializeCertificate(new CertificateRequest("CN=Management",RSA.Create(4096),HashAlgorithmName.SHA512,RSASignaturePadding.Pss).CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1)))! );
        KeyX = new( Utility.SerializeCertificate(new CertificateRequest("CN=debug",RSA.Create(4096),HashAlgorithmName.SHA512,RSASignaturePadding.Pss).CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1)))! );

        if(KeyM is null) { throw new InvalidOperationException(); } if(KeyX is null) { throw new InvalidOperationException(); }
    }

    [Test]
    public void Constructor()
    {
        MetaBaseTest _1 = new();

        Check.That(_1.GetID()).HasAValue();
    }

    [Test]
    public void opEquality()
    {
        MetaBaseTest _0 = new MetaBaseTest();
        MetaBaseTest _1 = new MetaBaseTest();

        Check.That(_1.SetID(Guid.Empty)).IsTrue();
        Check.That(_1.GetID()).IsNull();

        Check.That(new MetaBaseTest() == null).IsFalse();
        Check.That(new MetaBaseTest() == _1).IsFalse();
        Check.That(_1.SetID(_0.GetID())).IsTrue();
        Check.That(_0 == _1).IsTrue();
    }

    [Test]
    public void opInequality()
    {
        MetaBaseTest _0 = new MetaBaseTest();
        MetaBaseTest _1 = new MetaBaseTest();

        Check.That(_1.SetID(Guid.Empty)).IsTrue();
        Check.That(_1.GetID()).IsNull();

        Check.That(new MetaBaseTest() != null).IsTrue();
        Check.That(new MetaBaseTest() != _1).IsTrue();
        Check.That(_1.SetID(_0.GetID())).IsTrue();
        Check.That(_0 != _1).IsFalse();
    }

    [Test]
    public void EqualsObject()
    {
        MetaBaseTest _ = new();

        Check.That((Object)new MetaBaseTest().Equals(null)).IsEqualTo(false);
        Check.That((Object)new MetaBaseTest().Equals(new Object())).IsEqualTo(false);
        Check.That((Object)_.Equals(_)).IsEqualTo(true);
    }

    [Test]
    public void EqualsInterface()
    {
        MetaBaseTest _0 = new MetaBaseTest();
        MetaBaseTest _1 = new MetaBaseTest();

        Check.That(_1.SetID(Guid.Empty)).IsTrue();
        Check.That(_1.GetID()).IsNull();

        Check.That(new MetaBaseTest().Equals(null)).IsFalse();
        Check.That(new MetaBaseTest().Equals(new Object())).IsFalse();
        Check.That(new MetaBaseTest().Equals(_1)).IsFalse();
        Check.That(_1.SetID(_0.GetID())).IsTrue();
        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void GetApplication()
    {
        MetaBaseTest _0 = new();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        MetaBaseTest _0 = new();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetBornOn()
    {
        MetaBaseTest _0 = new();

        Check.That(_0.GetBornOn()).HasNoValue();
    }

    [Test]
    public void GetDescriptor()
    {
        MetaBaseTest _0 = new();

        String _s = "GetDescriptorExam";
        Version _v = new Version("1.2.3.4");
        DateTimeOffset? _d = DateTimeOffset.Now;
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
        Check.That(_.Modified).IsNotEqualTo(_d?.ToString("O"));
        Check.That(_.Name).IsEqualTo(_s);
        Check.That(_.Notes).Contains(_n);
        Check.That(_.ObjectType!.Split(" -> ")).Contains("KusDepot.Exams.MetaBaseTest");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Version).IsEqualTo(_v.ToString());

        Check.That(_0.SetID(Guid.Empty)).IsTrue();
        Check.That(_0.GetID()).IsNull();
        Check.That(_0.GetDescriptor()).IsNull();
    }

    [Test]
    public void GetDistinguishedName()
    {
        MetaBaseTest _0 = new();
        String _1 = "CN=ServiceN,OU=Engineering,DC=TailSpinToys,DC=COM";

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void GetDomainID()
    {
        MetaBaseTest _0 = new();
        String _1 = "GetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
    }

    [Test]
    public void GetExtension()
    {
        MetaBaseTest _0 = new();
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
        MetaBaseTest _0 = new();
        String _1 = "GetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.SetFILE(String.Empty)).IsTrue();
        Check.That(_0.GetFILE()).IsNull();
    }

    [Test]
    public void GetHash()
    {
        Check.That((Int32?)new MetaBaseTest().GetHashCode()).HasAValue();
    }

    [Test]
    public void GetMetaBaseIntegrityHash()
    {
        MetaBaseTest _0 = new();
        MetaBaseTest _1 = new();

        Version _v = new("1.2.3.4");
        DateTimeOffset _d = DateTimeOffset.Parse("2025-01-01T00:00:00+00:00");
        Dictionary<String,Object?> _e = new() { ["Mode"] = "A" , ["Count"] = 2 };

        Check.That(_0.SetApplication("App")).IsTrue();
        Check.That(_0.SetApplicationVersion(_v)).IsTrue();
        Check.That(_0.SetBornOn(_d)).IsTrue();
        Check.That(_0.SetDistinguishedName("CN=Meta,DC=example,DC=org")).IsTrue();
        Check.That(_0.SetDomainID("example.org")).IsTrue();
        Check.That(_0.SetExtension(_e)).IsTrue();
        Check.That(_0.SetName("MetaName")).IsTrue();
        Check.That(_0.SetNotes(new[] { "n1" , "n2" })).IsTrue();
        Check.That(_0.SetSecurityDescriptor("O:BAG:BAD:(A;;FA;;;SY)")).IsTrue();
        Check.That(_0.SetServiceVersion(_v)).IsTrue();
        Check.That(_0.SetTags(new[] { "t1" , "t2" })).IsTrue();
        Check.That(_0.SetVersion(_v)).IsTrue();
        Check.That(_0.SetFILE("c:\\temp\\a.bin")).IsTrue();
        Check.That(_0.SetObjectFILE("c:\\temp\\a.kd")).IsTrue();
        Check.That(_0.SetModified(_d.AddMinutes(1))).IsTrue();

        Check.That(_1.SetApplication("App")).IsTrue();
        Check.That(_1.SetApplicationVersion(_v)).IsTrue();
        Check.That(_1.SetBornOn(_d)).IsTrue();
        Check.That(_1.SetDistinguishedName("CN=Meta,DC=example,DC=org")).IsTrue();
        Check.That(_1.SetDomainID("example.org")).IsTrue();
        Check.That(_1.SetExtension(new Dictionary<String,Object?> { ["Count"] = 2 , ["Mode"] = "A" })).IsTrue();
        Check.That(_1.SetName("MetaName")).IsTrue();
        Check.That(_1.SetNotes(new[] { "n2" , "n1" })).IsTrue();
        Check.That(_1.SetSecurityDescriptor("O:BAG:BAD:(A;;FA;;;SY)")).IsTrue();
        Check.That(_1.SetServiceVersion(_v)).IsTrue();
        Check.That(_1.SetTags(new[] { "t2" , "t1" })).IsTrue();
        Check.That(_1.SetVersion(_v)).IsTrue();
        Check.That(_1.SetFILE("d:\\other\\b.bin")).IsTrue();
        Check.That(_1.SetObjectFILE("d:\\other\\b.kd")).IsTrue();
        Check.That(_1.SetModified(_d.AddMinutes(2))).IsTrue();

        Check.That(_0.GetMetaBaseIntegrityHash()).IsEqualTo(_1.GetMetaBaseIntegrityHash());

        Check.That(_1.SetServiceVersion(new Version("1.2.3.5"))).IsTrue();

        Check.That(_0.GetMetaBaseIntegrityHash()).IsNotEqualTo(_1.GetMetaBaseIntegrityHash());
    }

    [Test]
    public void GetID()
    {
        MetaBaseTest _0 = new();

        Check.That(_0.GetID()).HasAValue();
    }

    [Test]
    public void GetLinks()
    {
        MetaBaseTest _0 = new();
        Dictionary<String,GuidReferenceItem> _1 = new();
        GuidReferenceItem _2 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _3 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _4 = new GuidReferenceItem(Guid.NewGuid());
        _1.Add("_2",_2); _1.Add("_3",_3); _1.Add("_4",_4);

        Check.That(_0.SetLinks(null)).IsFalse();
        Check.That(_0.SetLinks(_1)).IsTrue();
        Check.That(_0.RegisterManager(KeyM)).IsTrue();
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
    public void SetLinks_Reassignment_UpdatesModified()
    {
        MetaBaseTest _0 = new();
        Dictionary<String,GuidReferenceItem> _1 = new()
        {
            {"_2",new GuidReferenceItem(Guid.NewGuid())},
            {"_3",new GuidReferenceItem(Guid.NewGuid())}
        };

        Check.That(_0.SetLinks(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Dictionary<String,GuidReferenceItem> _3 = new()
        {
            {"_2",new GuidReferenceItem(_1["_2"].GetContent())},
            {"_3",new GuidReferenceItem(_1["_3"].GetContent())}
        };

        Check.That(_0.SetLinks(_3)).IsTrue();
        Check.That(_0.GetModified()).IsNotEqualTo(_2);
    }

    [Test]
    public void GetLocked()
    {
        MetaBaseTest _0 = new MetaBaseTest();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.RegisterManager(KeyM)).IsTrue();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyX)).IsFalse();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }

    [Test]
    public void GetModified()
    {
        MetaBaseTest _0 = new();

        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_0.SetModified(_1)).IsTrue();
        Check.That(_0.GetModified()).Equals(_1);
        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetModified(DateTimeOffset.MaxValue)).IsFalse();
        Check.That(_0.GetModified()).Equals(_1);
    }

    [Test]
    public void GetName()
    {
        MetaBaseTest _0 = new();
        String _1 = "GetNameExam";

        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void GetNotes()
    {
        MetaBaseTest _0 = new();
        String _2 = "GetNotesExam";
        List<String> _1 = new(){_2,"Pass"};

        Check.That(_0.AddNotes(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
    }

    [Test]
    public void GetSecurityDescriptor()
    {
        MetaBaseTest _0 = new();
        String _1 = "GetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void GetServiceVersion() 
    {
        MetaBaseTest _0 = new();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void GetTags()
    {
        MetaBaseTest _0 = new();
        List<String> _1 = new List<String>(){"GetTagsExam", "Pass"};

        Check.That(_0.AddTags(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
    }

    [Test]
    public void GetVersion()
    {
        MetaBaseTest _0 = new();

        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Initialize()
    {
        MetaBaseTest _1 = new();

        Check.That(_1.Initialize()).IsTrue();

        Check.That(_1.GetID()).HasAValue();
    }

    [Test]
    public void Lock()
    {
        MetaBaseTest _0 = new();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.RegisterManager(KeyM)).IsTrue();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.UnLock(KeyX)).IsFalse();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsFalse();
    }

    [Test]
    public void RemoveNote()
    {
        MetaBaseTest _0 = new();
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
        MetaBaseTest _0 = new();
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
        MetaBaseTest _0 = new();
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplication_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new(); String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void SetApplicationVersion()
    {
        MetaBaseTest _0 = new();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new(); Version _1 = new("0.1.0.1");

        Check.That(_0.SetApplicationVersion(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.SetApplicationVersion(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void SetBornOn()
    {
        MetaBaseTest _0 = new();
        DateTimeOffset _1 = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);

        Check.That(_0.SetBornOn(_1)).IsTrue();
        Check.That(_0.GetBornOn()).Equals(_1);
    }

    [Test]
    public void SetDistinguishedName()
    {
        MetaBaseTest _0 = new();
        String _1 = "CN=ServiceN,OU=Operations,DC=Fabrikam,DC=NET";

        Check.That(_0.SetDistinguishedName(null)).IsFalse();
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void SetDistinguishedName_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new(); String _1 = "CN=ServiceN,OU=Operations,DC=Fabrikam,DC=NET";

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void SetDomainID()
    {
        MetaBaseTest _0 = new();
        String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetDomainID(_1)).IsFalse();
    }

    [Test]
    public void SetDomainID_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new(); String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void SetExtension()
    {
        MetaBaseTest _0 = new();
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
        MetaBaseTest _0 = new();
        String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
    }

    [Test]
    public void SetFILE_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new(); String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void SetID()
    {
        MetaBaseTest _0 = new();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void SetLinks()
    {
        MetaBaseTest _0 = new();
        Dictionary<String,GuidReferenceItem> _1 = new Dictionary<String,GuidReferenceItem>();
        GuidReferenceItem _2 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _3 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _4 = new GuidReferenceItem(Guid.NewGuid());
        _1.Add("_2",_2); _1.Add("_3",_3); _1.Add("_4",_4);

        Check.That(_0.SetLinks(null)).IsFalse();
        Check.That(_0.SetLinks(_1)).IsTrue();
        Check.That(_0.RegisterManager(KeyM)).IsTrue();
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
        MetaBaseTest _0 = new();

        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_0.SetModified(_1)).IsTrue();
        Check.That(_0.GetModified()).Equals(_1);
        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetModified(DateTimeOffset.MaxValue)).IsFalse();
        Check.That(_0.GetModified()).Equals(_1);
    }

    [Test]
    public void SetName()
    {
        MetaBaseTest _0 = new();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void SetName_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new(); String _1 = "SetNameExam";

        Check.That(_0.SetName(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void SetNotes_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new(); HashSet<String> _1 = new(){"SetNotesExam","Pass"};

        Check.That(_0.SetNotes(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.SetNotes(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void SetSecurityDescriptor()
    {
        MetaBaseTest _0 = new();
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void SetSecurityDescriptor_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new(); String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void SetServiceVersion()
    {
        MetaBaseTest _0 = new();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetServiceVersion_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new(); Version _1 = new("0.1.0.1");

        Check.That(_0.SetServiceVersion(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.SetServiceVersion(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void SetTags_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new(); HashSet<String> _1 = new(){"SetTagsExam","Pass"};

        Check.That(_0.SetTags(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.SetTags(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void SetVersion()
    {
        MetaBaseTest _0 = new();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetVersion_Idempotent_DoesNotUpdateModified()
    {
        MetaBaseTest _0 = new(); Version _1 = new("0.1.0.1");

        Check.That(_0.SetVersion(_1)).IsTrue();

        DateTimeOffset? _2 = _0.GetModified(); PauseClock();

        Check.That(_0.SetVersion(_1)).IsTrue();
        Check.That(_0.GetModified()).IsEqualTo(_2);
    }

    [Test]
    public void UnLock()
    {
        MetaBaseTest _0 = new();

        Check.That(_0.GetLocked()).IsFalse();

        Check.That(_0.RegisterManager(KeyM)).IsTrue();

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.UnLock(KeyX)).IsFalse();

        Check.That(_0.GetLocked()).IsTrue();

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsFalse();
    }
}