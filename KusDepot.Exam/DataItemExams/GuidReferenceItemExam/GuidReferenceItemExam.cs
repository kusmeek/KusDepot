namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class GuidReferenceItemExam
{
    private ManagerKey? KeyM;

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
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }

        KeyM = new( Utility.SerializeCertificate(new CertificateRequest("CN=Management",RSA.Create(4096),HashAlgorithmName.SHA512,RSASignaturePadding.Pss).CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1)))! );        

        if(KeyM is null) { throw new InvalidOperationException(); }
    }

    [Test]
    public void Clone()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _1 = new GuidReferenceItem(Guid.NewGuid());

        Check.That(_0.Equals(_0.Clone())).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsFalse();
    }

    [Test]
    public void CompareTo()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        Thread.Sleep(100);
        GuidReferenceItem _1 = new GuidReferenceItem();

        Check.That(new GuidReferenceItem().CompareTo(null)).IsEqualTo(1);
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
        GuidReferenceItem _0 = new GuidReferenceItem(content:Guid.NewGuid(),id:Guid.NewGuid(),name:String.Empty,notes:new HashSet<String>(),tags:new HashSet<String>{});

        Check.That(_0).IsInstanceOfType(typeof(GuidReferenceItem));
    }

    [Test]
    public void opEquality()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"));
        GuidReferenceItem _2 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"));
        Guid _3 = Guid.NewGuid();
        GuidReferenceItem _4 = new GuidReferenceItem(content:new Guid("00021401-0000-0000-C000-000000000046"),id:_3);
        GuidReferenceItem _5 = new GuidReferenceItem(content:new Guid("00021401-0000-0000-C000-000000000046"),id:_3);
        GuidReferenceItem _8 = new GuidReferenceItem(content:new Guid("00021401-0000-0000-C000-000000000046"),id:_3);
        GuidReferenceItem _6 = new GuidReferenceItem(content:null,id:_3);
        GuidReferenceItem _7 = new GuidReferenceItem(content:null,id:_3);
        GuidReferenceItem _9 = new GuidReferenceItem();

        Check.That(new GuidReferenceItem() == null).IsFalse();
        Check.That(new GuidReferenceItem() == _9).IsFalse();
        Check.That(_0 == _1).IsFalse();
        Check.That(_1 == _0).IsFalse();
        Check.That(_1 == _1).IsTrue();
        Check.That(_2 == _4).IsTrue();
        Check.That(_4 == _5).IsTrue();
        Check.That(_6 == _4).IsFalse();
        Check.That(_6 == _7).IsFalse();
        Check.That(_6 == _1).IsFalse();
        Check.That(_6 == _8).IsFalse();
        Check.That(_6 == _9).IsFalse();
        Check.That(_1 == _9).IsFalse();
    }

    [Test]
    public void opInequality()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        GuidReferenceItem _1 = new GuidReferenceItem(new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"));
        GuidReferenceItem _2 = new GuidReferenceItem(new Guid("00021401-0000-0000-C000-000000000046"));
        Guid _3 = Guid.NewGuid();
        GuidReferenceItem _4 = new GuidReferenceItem(content:new Guid("00021401-0000-0000-C000-000000000046"),id:_3);
        GuidReferenceItem _5 = new GuidReferenceItem(content:new Guid("00021401-0000-0000-C000-000000000046"),id:_3);
        GuidReferenceItem _8 = new GuidReferenceItem(content:new Guid("00021401-0000-0000-C000-000000000046"),id:_3);
        GuidReferenceItem _6 = new GuidReferenceItem(content:null,id:_3);
        GuidReferenceItem _7 = new GuidReferenceItem(content:null,id:_3);
        GuidReferenceItem _9 = new GuidReferenceItem();

        Check.That(new GuidReferenceItem() != null).IsTrue();
        Check.That(new GuidReferenceItem() != _9).IsTrue();
        Check.That(_0 != _1).IsTrue();
        Check.That(_1 != _0).IsTrue();
        Check.That(_1 != _1).IsFalse();
        Check.That(_2 != _4).IsFalse();
        Check.That(_4 != _5).IsFalse();
        Check.That(_6 != _4).IsTrue();
        Check.That(_6 != _7).IsTrue();
        Check.That(_6 != _1).IsTrue();
        Check.That(_6 != _8).IsTrue();
        Check.That(_6 != _9).IsTrue();
        Check.That(_1 != _9).IsTrue();
    }

    [Test]
    public void EqualsObject()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _1 = new GuidReferenceItem(Guid.NewGuid());

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsFalse();
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
        GuidReferenceItem _4 = new GuidReferenceItem(content:new Guid("00021401-0000-0000-C000-000000000046"),id:_3);
        GuidReferenceItem _5 = new GuidReferenceItem(content:new Guid("00021401-0000-0000-C000-000000000046"),id:_3);
        GuidReferenceItem _8 = new GuidReferenceItem(content:new Guid("00021401-0000-0000-C000-000000000046"),id:_3);
        GuidReferenceItem _6 = new GuidReferenceItem(content:null,id:_3);
        GuidReferenceItem _7 = new GuidReferenceItem(content:null,id:_3);
        GuidReferenceItem _9 = new GuidReferenceItem();

        Check.That(new GuidReferenceItem().Equals(null)).IsFalse();
        Check.That(new GuidReferenceItem().Equals(new Object())).IsFalse();
        Check.That(new GuidReferenceItem().Equals(_9)).IsFalse();
        Check.That(_0.Equals(_1)).IsFalse();
        Check.That(_1.Equals(_0)).IsFalse();
        Check.That(_1.Equals(_1)).IsTrue();
        Check.That(_2.Equals(_4)).IsTrue();
        Check.That(_4.Equals(_5)).IsTrue();
        Check.That(_6.Equals(_4)).IsFalse();
        Check.That(_6.Equals(_7)).IsFalse();
        Check.That(_6.Equals(_1)).IsFalse();
        Check.That(_6.Equals(_8)).IsFalse();
        Check.That(_6.Equals(_9)).IsFalse();
        Check.That(_1.Equals(_9)).IsFalse();
    }

    [Test]
    public void FromFile()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(content:Guid.NewGuid(),id:Guid.NewGuid());

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
    public void GetApplication()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        String _1 = "GetApplication";

        Check.That(_0.SetApplication(String.Empty)).IsTrue();
        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
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
    public void GetBornOn()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public async Task GetContent()
    {
        Guid _2 = Guid.NewGuid();
        GuidReferenceItem _0 = new GuidReferenceItem(_2);

        Check.That(_0.GetContent()).HasAValue();
        var dc1 = await _0.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.GuidReference).IsNotNull();
        Check.That(dc1.GuidReference!.ContainsKey("Content")).IsTrue();
        Check.That(dc1.GuidReference["Content"]).Equals(_2);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetContent()).HasAValue();
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        var dc2 = await _0.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _0.GetDataContent(KeyM);
        Check.That(dc3).IsNotNull();
        Check.That(dc3!.GuidReference).IsNotNull();
        Check.That(dc3.GuidReference!.ContainsKey("Content")).IsTrue();
        Check.That(dc3.GuidReference["Content"]).Equals(_2);
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
    }

    [Test]
    public async Task GetDataContent()
    {
        var content = Guid.NewGuid();
        var next = new GuidReferenceItem(Guid.NewGuid());
        var previous = new GuidReferenceItem(Guid.NewGuid());
        var undirected = new HashSet<GuidReferenceItem> { new(Guid.NewGuid()), new(Guid.NewGuid()) };
        var item = new GuidReferenceItem(content);
        Check.That(item.RegisterManager(KeyM)).IsTrue();
        Check.That(item.SetNext(next)).IsTrue();
        Check.That(item.SetPrevious(previous)).IsTrue();
        Check.That(item.SetUndirectedLinks(undirected)).IsTrue();

        var dc1 = await item.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.GuidReference).IsNotNull();
        Check.That(dc1.GuidReference!["Content"]).IsEqualTo(content);

        var next1 = dc1.GuidReference["Next"] as GuidReferenceItem;
        Check.That(next1).IsNotNull().And.Not.IsSameReferenceAs(next);
        Check.That(next1!.Equals(next)).IsTrue();

        var prev1 = dc1.GuidReference["Previous"] as GuidReferenceItem;
        Check.That(prev1).IsNotNull().And.Not.IsSameReferenceAs(previous);
        Check.That(prev1!.Equals(previous)).IsTrue();

        var undirected1 = dc1.GuidReference["UndirectedLinks"] as HashSet<GuidReferenceItem>;
        Check.That(undirected1).IsNotNull().And.Not.IsSameReferenceAs(undirected);
        Check.That(undirected1).IsEquivalentTo(undirected);
        Check.That(undirected1!.First()).Not.IsSameReferenceAs(undirected.First());

        Check.That(item.MaskData(true, KeyM)).IsTrue();
        Check.That(await item.GetDataContent()).IsNull();

        var dc2 = await item.GetDataContent(KeyM);
        Check.That(dc2).IsNotNull();
        Check.That(dc2!.GuidReference).IsNotNull();
        Check.That(dc2.GuidReference!["Content"]).IsEqualTo(content);
        Check.That(dc2.GuidReference["Next"] as GuidReferenceItem).IsNotNull().And.IsEqualTo(next);
        Check.That(dc2.GuidReference["Previous"] as GuidReferenceItem).IsNotNull().And.IsEqualTo(previous);
        Check.That(dc2.GuidReference["UndirectedLinks"] as IEnumerable<GuidReferenceItem>).IsNotNull().And.IsEquivalentTo(undirected);
        Check.That(item.MaskData(false, KeyM)).IsTrue();

        Check.That(await item.EncryptData(KeyM)).IsTrue();
        Check.That(await item.GetDataContent()).IsNull();

        var dc3 = await item.GetDataContent(KeyM);
        Check.That(dc3).IsNotNull();
        Check.That(dc3!.GuidReference).IsNotNull();
        Check.That(dc3.GuidReference!["Content"]).IsEqualTo(content);
        Check.That(dc3.GuidReference["Next"] as GuidReferenceItem).IsNotNull().And.IsEqualTo(next);
        Check.That(dc3.GuidReference["Previous"] as GuidReferenceItem).IsNotNull().And.IsEqualTo(previous);
        Check.That(dc3.GuidReference["UndirectedLinks"] as IEnumerable<GuidReferenceItem>).IsNotNull().And.IsEquivalentTo(undirected);
    }

    [Test]
    public async Task MaskData()
    {
        Guid _2 = Guid.NewGuid();
        GuidReferenceItem _0 = new GuidReferenceItem(_2);

        Check.That(_0.GetContent()).HasAValue();
        var dc1 = await _0.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.GuidReference).IsNotNull();
        Check.That(dc1.GuidReference!.ContainsKey("Content")).IsTrue();
        Check.That(dc1.GuidReference["Content"]).Equals(_2);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetContent()).HasAValue();
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        var dc2 = await _0.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _0.GetDataContent(KeyM);
        Check.That(dc3).IsNotNull();
        Check.That(dc3!.GuidReference).IsNotNull();
        Check.That(dc3.GuidReference!.ContainsKey("Content")).IsTrue();
        Check.That(dc3.GuidReference["Content"]).Equals(_2);
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
    }

    [Test]
    public void GetDescriptor()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

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
        Check.That(_.ObjectType!.Split(" -> ")).Contains("KusDepot.GuidReferenceItem");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Type).Equals(DataType.GUID);
        Check.That(_.Version).IsEqualTo(_v.ToString());

        Check.That(_0.SetID(Guid.Empty)).IsTrue();
        Check.That(_0.GetID()).IsNull();
        Check.That(_0.GetDescriptor()).IsNull();
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
    public void GetHash()
    {
        Guid _ = Guid.NewGuid();
        GuidReferenceItem _0 = new GuidReferenceItem(content:null,id:Guid.NewGuid());
        GuidReferenceItem _1 = new GuidReferenceItem(content:null,id:new Guid("00021401-0000-0000-C000-000000000046"));
        GuidReferenceItem _2 = new GuidReferenceItem(content:_,id:new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"));
        GuidReferenceItem _3 = new(_);

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_0.GetHashCode()).IsNotEqualTo(_1.GetHashCode());
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        Check.That(_2.GetHashCode()).IsNotEqualTo(_3.GetHashCode());
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
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetModified()
    {
        GuidReferenceItem _0 = new();

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
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _1 = new GuidReferenceItem(Guid.NewGuid());

        Check.That(_0.SetNext(_1)).IsTrue();
        Check.That(_0.GetNext()).IsEqualTo(_1);
        Check.That(_0.SetNext(null)).IsTrue();
        Check.That(_0.GetNext()).IsNull();
        Check.That(_0.SetNext(_1)).IsTrue();
        Check.That(_0.GetNext()).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
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
        Check.That(_0.Lock(KeyM)).IsTrue();
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
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetPrevious()!.GetContent()).Equals(_1.GetContent());
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
    public void GetUndirectedLinks()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _1 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _2 = new GuidReferenceItem(Guid.NewGuid());
        List<GuidReferenceItem> _3 = new List<GuidReferenceItem>(){_1,_2};

        Check.That(_0.SetUndirectedLinks(_3.ToHashSet())).IsTrue();
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

        Check.That(_1.GetID()).HasAValue();
    }

    [Test]
    public void Lock()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

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
        Check.That(_0.GetLocked()).IsFalse();
        Check.That(_0.GetModified()).IsNotNull();
        Check.That(_0.GetName()).IsNull();
        Check.That(_0.GetNext()).IsNull();
        Check.That(_0.GetNotes()).IsNull();
        Check.That(_0.GetPrevious()).IsNull();
        Check.That(_0.GetSecurityDescriptor()).IsNull();
        Check.That(_0.GetServiceVersion()).IsNull();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.GetType()).Equals(DataType.GUID);
        Check.That(_0.GetUndirectedLinks()).IsNull();
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Parse()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(content:Guid.NewGuid(),id:Guid.NewGuid());

        GuidReferenceItem? _1 = GuidReferenceItem.Parse(_0.ToString(),null);

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
    public void SetContent()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetContent(_1)).IsTrue();
        Check.That(_0.GetContent()!).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetContent(_1)).IsFalse();
        Check.That(_0.UnLock(KeyM)).IsTrue();
        Check.That(_0.SetContent(Guid.Empty)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
    }

    [Test]
    public void SetContentStreamed()
    {
        GuidReferenceItem _0 = new();

        Byte[] d = new Byte[2194304];

        using RandomNumberGenerator r = RandomNumberGenerator.Create(); r.GetBytes(d);

        String p = Path.GetTempFileName()+Guid.NewGuid().ToString()+@".temp";

        if(File.Exists(p)) { throw new Exception(); } File.WriteAllBytes(p,d);

        Check.That(File.Exists(p)).IsTrue();

        Check.That(_0.SetFILE(p)).IsTrue();

        Check.That(_0.GetContentStreamed()).IsFalse();

        Check.That(_0.SetContentStreamed(true)).IsFalse();

        Check.That(_0.GetContentStreamed()).IsFalse();

        Stream? c = _0.GetContentStream(); Check.That(c).IsNull();

        Check.That(_0.SetContentStreamed(false)).IsTrue();

        Check.That(_0.GetContentStreamed()).IsFalse();

        File.Delete(p);
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
        Check.That(_0.Lock(KeyM)).IsTrue();
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
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
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
        GuidReferenceItem _0 = new();

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
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetPrevious(_1)).IsFalse();
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

        Check.That(_0.SetType(DataType.EVTX)).IsFalse();
        Check.That(_0.GetType()).Equals(DataType.GUID);
    }

    [Test]
    public void SetUndirectedLinks()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _1 = new GuidReferenceItem(Guid.NewGuid());
        GuidReferenceItem _2 = new GuidReferenceItem(Guid.NewGuid());
        List<GuidReferenceItem> _3 = new List<GuidReferenceItem>(){_1,_2};

        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetUndirectedLinks(_3.ToHashSet())).IsFalse();
        Check.That(_0.UnLock(KeyM)).IsTrue();
        Check.That(_0.SetUndirectedLinks(_3.ToHashSet())).IsTrue();
        Check.That(_0.GetUndirectedLinks()).ContainsExactly(_3);
        _3.Clear();
        Check.That(_0.SetUndirectedLinks(_3.ToHashSet())).IsTrue();
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
        GuidReferenceItem _0 = new GuidReferenceItem(content:Guid.NewGuid(),id:Guid.NewGuid());

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
        GuidReferenceItem _0 = new GuidReferenceItem(content:Guid.NewGuid(),id:Guid.NewGuid());

        GuidReferenceItem? _1 = GuidReferenceItem.Parse(_0.ToString(),null);

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void TryParse()
    {
        GuidReferenceItem _0 = new GuidReferenceItem(content:Guid.NewGuid(),id:Guid.NewGuid());

        GuidReferenceItem? _1;

        GuidReferenceItem.TryParse(_0.ToString(),null,out _1);

        Check.That(_1).IsInstanceOfType(typeof(GuidReferenceItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        GuidReferenceItem _0 = new GuidReferenceItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }
}