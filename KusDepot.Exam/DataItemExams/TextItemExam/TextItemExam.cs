namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class TextItemExam
{
    private ManagerKey? KeyM;

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
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }

        KeyM = new( Utility.SerializeCertificate(new CertificateRequest("CN=Management",RSA.Create(4096),HashAlgorithmName.SHA512,RSASignaturePadding.Pss).CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1)))! );        

        if(KeyM is null) { throw new InvalidOperationException(); }
    }

    [Test]
    public void Clone()
    {
        TextItem _0 = new TextItem();
        TextItem _1 = new TextItem();

        Check.That(_0.Equals(_0.Clone())).IsFalse();
        Check.That(_1.SetContent("CloneExam")).IsTrue();
        Check.That(_1.Equals(_1.Clone())).IsTrue();
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
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        String? _3 = DataValidation.TextItemValidDataTypes[RandomNumberGenerator.GetInt32(0,DataValidation.TextItemValidDataTypes.Length)];
        TextItem _4 = new TextItem(content:_0,id:Guid.NewGuid(),name:String.Empty,notes:new HashSet<String>{},tags:new HashSet<String>{},type:_3!.ToString());

        Check.That(_4).IsInstanceOfType(typeof(TextItem));
        Check.That(_4.GetContent()).IsEqualTo(_0.ToString());
    }

    [Test]
    public void opEquality()
    {
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        String _6 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        TextItem _3 = new TextItem(content:_0,id:_11);
        TextItem _15 = new TextItem(content:_0,id:_11);
        TextItem _4 = new TextItem(content:_0,id:_14);
        TextItem _5 = new TextItem(_0);
        TextItem _9 = new TextItem(content:_6,id:_11);
        TextItem _10 = new TextItem(content:_6,id:_14);
        TextItem _16 = new TextItem(content:null,id:_14);
        TextItem _17 = new TextItem();
        TextItem _18 = new TextItem(content:null,id:_14);

        Check.That(new TextItem() == null).IsFalse();
        Check.That(new TextItem() == _17).IsFalse();
        Check.That(_3 == _4).IsTrue();
        Check.That(_3 == _5).IsTrue();
        Check.That(_3 == _9).IsFalse();
        Check.That(_3 == _15).IsTrue();
        Check.That(_16 == _10).IsFalse();
        Check.That(_16 == _17).IsFalse();
        Check.That(_16 == _18).IsFalse();
        Check.That(_5 == _17).IsFalse();
        Check.That(_5 == _18).IsFalse();
    }

    [Test]
    public void opInequality()
    {
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        String _6 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        TextItem _3 = new TextItem(content:_0,id:_11);
        TextItem _15 = new TextItem(content:_0,id:_11);
        TextItem _4 = new TextItem(content:_0,id:_14);
        TextItem _5 = new TextItem(_0);
        TextItem _9 = new TextItem(content:_6,id:_11);
        TextItem _10 = new TextItem(content:_6,id:_14);
        TextItem _16 = new TextItem(content:null,id:_14);
        TextItem _17 = new TextItem();
        TextItem _18 = new TextItem(content:null,id:_14);

        Check.That(new TextItem() != null).IsTrue();
        Check.That(new TextItem() != _17).IsTrue();
        Check.That(_3 != _4).IsFalse();
        Check.That(_3 != _5).IsFalse();
        Check.That(_3 != _9).IsTrue();
        Check.That(_3 != _15).IsFalse();
        Check.That(_16 != _10).IsTrue();
        Check.That(_16 != _17).IsTrue();
        Check.That(_16 != _18).IsTrue();
        Check.That(_5 != _17).IsTrue();
        Check.That(_5 != _18).IsTrue();
    }

    [Test]
    public void EqualsObject()
    {
        TextItem _0 = new TextItem();
        TextItem _1 = new TextItem();

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsFalse();
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
        TextItem _3 = new TextItem(content:_0,id:_11);
        TextItem _15 = new TextItem(content:_0,id:_11);
        TextItem _4 = new TextItem(content:_0,id:_14);
        TextItem _5 = new TextItem(_0);
        TextItem _9 = new TextItem(content:_6,id:_11);
        TextItem _10 = new TextItem(content:_6,id:_14);
        TextItem _16 = new TextItem(content:null,id:_14);
        TextItem _17 = new TextItem();
        TextItem _18 = new TextItem(content:null,id:_14);

        Check.That(new TextItem().Equals(null)).IsFalse();
        Check.That(new TextItem().Equals(new Object())).IsFalse();
        Check.That(new TextItem().Equals(_17)).IsFalse();
        Check.That(_3.Equals(_4)).IsTrue();
        Check.That(_3.Equals(_5)).IsTrue();
        Check.That(_3.GetHashCode()).IsNotEqualTo(_5.GetHashCode());
        Check.That(_4.GetHashCode()).IsNotEqualTo(_5.GetHashCode());
        Check.That(_3.Equals(_9)).IsFalse();
        Check.That(_3.Equals(_15)).IsTrue();
        Check.That(_16.Equals(_10)).IsFalse();
        Check.That(_16.Equals(_17)).IsFalse();
        Check.That(_16.Equals(_18)).IsFalse();
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
    public void GetApplication()
    {
        TextItem _0 = new TextItem();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetBornOn()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public async Task GetContent()
    {
        String _2 = Guid.NewGuid().ToString();
        TextItem _0 = new TextItem(_2);

        Check.That(_0.GetContent()).Equals(_2);
        var dc1 = await _0.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.Text).Equals(_2);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
        Check.That(ReferenceEquals(_0.GetContent()!,_2)).IsFalse();
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        Check.That(await _0.GetDataContent()).IsNull();
        var dc2 = await _0.GetDataContent(KeyM);
        Check.That(dc2).IsNotNull();
        Check.That(dc2!.Text).Equals(_2);
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
    }

    [Test]
    public async Task GetDataContent()
    {
        String _2 = Guid.NewGuid().ToString();
        TextItem _0 = new TextItem(_2);

        Check.That(_0.GetContent()).Equals(_2);
        var dc1 = await _0.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.Text).Equals(_2);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
        Check.That(ReferenceEquals(_0.GetContent()!,_2)).IsFalse();
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        Check.That(await _0.GetDataContent()).IsNull();
        var dc2 = await _0.GetDataContent(KeyM);
        Check.That(dc2).IsNotNull();
        Check.That(dc2!.Text).Equals(_2);
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
    }

    [Test]
    public void GetDescriptor()
    {
        TextItem _0 = new TextItem();

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
        Check.That(_0.SetType(DataType.NFO)).IsTrue();
        Check.That(_0.SetVersion(_v)).IsTrue();

        Descriptor ? _ = _0.GetDescriptor();

        Check.That(_!.Application).IsEqualTo(_s);
        Check.That(_.ApplicationVersion).IsEqualTo(_v.ToString());
        Check.That(_.BornOn).Equals(_0.GetBornOn()?.ToString("O"));
        Check.That(_.DistinguishedName).IsEqualTo(_s);
        Check.That(_.ID).Equals(_0.GetID());
        Check.That(_.Modified).IsNotEqualTo(_d?.ToString("O"));
        Check.That(_.Name).IsEqualTo(_s);
        Check.That(_.Notes).Contains(_n);
        Check.That(_.ObjectType!.Split(" -> ")).Contains("KusDepot.TextItem");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Type).Equals(DataType.NFO);
        Check.That(_.Version).IsEqualTo(_v.ToString());

        Check.That(_0.SetID(Guid.Empty)).IsTrue();
        Check.That(_0.GetID()).IsNull();
        Check.That(_0.GetDescriptor()).IsNull();
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
    public void GetHash()
    {
        String _ = "TextItemExam";
        TextItem _0 = new TextItem(content:null,id:Guid.NewGuid());
        TextItem _1 = new TextItem(content:null,id:new Guid("00021401-0000-0000-C000-000000000046"));
        TextItem _2 = new TextItem(content:_,id:new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"));
        TextItem _3 = new TextItem(_);

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_0.GetHashCode()).IsNotEqualTo(_1.GetHashCode());
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        Check.That(_2.GetHashCode()).IsNotEqualTo(_3.GetHashCode());
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
        TextItem _0 = new TextItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetModified()
    {
        TextItem _0 = new();

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
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
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
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.NFO);
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

        Check.That(_1.GetID()).HasAValue();
    }

    [Test]
    public void Lock()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public async Task MaskData()
    {
        String _2 = Guid.NewGuid().ToString();
        TextItem _0 = new TextItem(_2);

        Check.That(_0.GetContent()).Equals(_2);
        var dc1 = await _0.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.Text).Equals(_2);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
        Check.That(ReferenceEquals(_0.GetContent()!,_2)).IsFalse();
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        Check.That(await _0.GetDataContent()).IsNull();
        var dc2 = await _0.GetDataContent(KeyM);
        Check.That(dc2).IsNotNull();
        Check.That(dc2!.Text).Equals(_2);
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        TextItem _0 = new TextItem();

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.GetApplicationVersion()).IsNull();
        Check.That(_0.GetBornOn()).IsNotNull();
        Check.That(_0.GetContent()).IsNull();
        Check.That(_0.GetDistinguishedName()).IsNull();
        Check.That(_0.GetDomainID()).IsNull();
        Check.That(_0.GetExtension()).IsNull();
        Check.That(_0.GetFILE()).IsNull();
        Check.That(_0.GetID()).IsNotNull();
        Check.That(_0.GetLanguage()).IsNull();
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
        TextItem _0 = new TextItem(TestCaseDataGenerator.GenerateUnicodeString(8192));

        TextItem? _1 = TextItem.Parse(new TextItem(new TextItem(_0.ToString()).ToString()).ToString(),null);

        TextItem? _2 = TextItem.Parse(_1!.GetContent()!,null);

        TextItem? _3 = TextItem.Parse(_2!.GetContent()!,null);

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
    public void SetContent()
    {
        String _2 = Guid.NewGuid().ToString();
        TextItem _0 = new TextItem();

        Check.That(_0.SetContent(_2)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
        Check.That(_0.Lock(KeyM)).IsTrue();;
        Check.That(_0.GetContent()).Equals(_2);
    }

    [Test]
    public void SetContentStreamed()
    {
        TextItem _0 = new();

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
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetDomainID(_1)).IsFalse();
    }

    [Test]
    public void SetFILE()
    {
        TextItem _0 = new TextItem();
        String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
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
        TextItem _0 = new();

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
        TextItem _0 = new TextItem();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
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
        Check.That(_0.Lock(KeyM)).IsTrue();
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

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }
}