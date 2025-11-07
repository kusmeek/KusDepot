namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class DataSetItemExam
{
    private ManagerKey? KeyM;

    [Test]
    public void AddDataItems()
    {
        HashSet<DataItem> _ = new() { DataItemGenerator.CreateDataSet(20) , DataItemGenerator.CreateDataSet(20) };

        DataSetItem _0 = new DataSetItem();

        Int32 h = _0.GetHashCode();

        Check.That(_0.AddDataItems(_)).IsTrue();

        Check.That(_0.GetContent()).ContainsExactly(_);

        Check.That(_0.GetHashCode()).IsNotEqualTo(h);
    }

    [Test]
    public void AddNotes()
    {
        DataSetItem _0 = new DataSetItem();
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
        DataSetItem _0 = new DataSetItem();
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
        DataSetItem _0 = new DataSetItem();
        DataSetItem _1 = new DataSetItem(content:[DataItemGenerator.CreateDataSet(2)]);

        Check.That(_0.Equals(_0.Clone())).IsFalse();
        Check.That(_0.Equals(_1.Clone())).IsFalse();
        Check.That(_1.Equals(_1.Clone())).IsTrue();
    }

    [Test]
    public void CompareTo()
    {
        DataSetItem _0 = new DataSetItem();
        Thread.Sleep(100);
        DataSetItem _1 = new DataSetItem();

        Check.That(new DataSetItem().CompareTo(null)).IsEqualTo(1);
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
        DataSetItem _0 = new DataSetItem(content:new HashSet<DataItem>(),id:Guid.NewGuid(),name:String.Empty,notes:new HashSet<String>(),tags:new HashSet<String>(),type:DataType.SGML);

        Check.That(_0).IsInstanceOfType(typeof(DataSetItem));
    }

    [Test]
    public void opEquality()
    {
        GuidReferenceItem _7 = new GuidReferenceItem(Guid.NewGuid()); BinaryItem b = new(); TextItem t = new(); DataSetItem d = new([new GenericItem(),new DataSetItem([_7])]);
        HashSet<DataItem>? _4 = new HashSet<DataItem>([b,t,d]);
        HashSet<DataItem>? _5 = new HashSet<DataItem>([d,t,b]);
        HashSet<DataItem>? _8 = new HashSet<DataItem>([b,t,d,new CodeItem()]);
        HashSet<DataItem>? _10 = new HashSet<DataItem>([t,d]);

        DataSetItem _0 = new(_4);
        DataSetItem _6 = new(_4);
        DataSetItem _1 = new(_5);
        DataSetItem _9 = new(_8);
        DataSetItem _11 = new(_8);
        DataSetItem _2 = new();
        DataSetItem _3 = new(content:null,id:null,name:String.Empty);

        Check.That(new DataSetItem() == null).IsFalse();
        Check.That(new DataSetItem() == _3).IsFalse();
        Check.That(_0 == _0).IsTrue();
        Check.That(_0 == _1).IsTrue();
        Check.That(_0 == _2).IsFalse();
        Check.That(_0 == _3).IsFalse();
        Check.That(_0 == _6).IsTrue();
        Check.That(_9 == _6).IsFalse();
        Check.That(_11 == _9).IsTrue();
    }

    [Test]
    public void opInequality()
    {
        GuidReferenceItem _7 = new GuidReferenceItem(Guid.NewGuid()); BinaryItem b = new(); TextItem t = new(); DataSetItem d = new([new GenericItem(),new DataSetItem([_7])]);
        HashSet<DataItem>? _4 = new HashSet<DataItem>([b,t,d]);
        HashSet<DataItem>? _5 = new HashSet<DataItem>([d,t,b]);
        HashSet<DataItem>? _8 = new HashSet<DataItem>([b,t,d,new CodeItem()]);
        HashSet<DataItem>? _10 = new HashSet<DataItem>([t,d]);

        DataSetItem _0 = new(_4);
        DataSetItem _6 = new(_4);
        DataSetItem _1 = new(_5);
        DataSetItem _9 = new(_8);
        DataSetItem _11 = new(_8);
        DataSetItem _2 = new();
        DataSetItem _3 = new(content:null,id:null,name:String.Empty);

        Check.That(new DataSetItem() != null).IsTrue();
        Check.That(new DataSetItem() != _3).IsTrue();
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
        DataSetItem _0 = new DataSetItem();
        DataSetItem _1 = new DataSetItem();

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsFalse();
        Check.That(((Object)new DataSetItem()).Equals(null)).IsFalse();
        Check.That(((Object)new DataSetItem()).Equals(new Object())).IsFalse();
    }


    [Test]
    public void EqualsInterface()
    {
        GuidReferenceItem _7 = new GuidReferenceItem(Guid.NewGuid()); BinaryItem b = new(); TextItem t = new(); DataSetItem d = new([new GenericItem(),new DataSetItem([_7])]);
        HashSet<DataItem>? _4 = new HashSet<DataItem>([b,t,d]);
        HashSet<DataItem>? _5 = new HashSet<DataItem>([d,t,b]);
        HashSet<DataItem>? _8 = new HashSet<DataItem>([b,t,d,new CodeItem()]);
        HashSet<DataItem>? _10 = new HashSet<DataItem>([t,d]);

        DataSetItem _0 = new(_4);
        DataSetItem _6 = new(_4);
        DataSetItem _1 = new(_5);
        DataSetItem _9 = new(_8);
        DataSetItem _11 = new(_8);
        DataSetItem _2 = new();
        DataSetItem _3 = new(content:null,id:null,name:String.Empty);

        Check.That(new DataSetItem().Equals(null)).IsFalse();
        Check.That(new DataSetItem().Equals(new Object())).IsFalse();
        Check.That(new DataSetItem().Equals(_3)).IsFalse();
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

        DataSetItem _2 = new DataSetItem([new MultiMediaItem(_1)]);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\DataSetItem.kdi";

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();

        Check.That(_2.ToFile(_3)).IsTrue();

        DataSetItem? _4 = DataSetItem.FromFile(_3);

        Check.That(_4).IsNotNull();

        Check.That(_2.Equals(_4)).IsTrue();

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();
    }

    [Test]
    public void GetApplication()
    {
        DataSetItem _0 = new DataSetItem();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        DataSetItem _0 = new DataSetItem();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetBornOn()
    {
        DataSetItem _0 = new DataSetItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public async Task GetContent()
    {
        DataSetItem _0 = new DataSetItem(); Guid _1 = Guid.NewGuid();
        HashSet<DataItem> _2 = [new GenericItem([new DataSetItem([new GuidReferenceItem(_1),new MSBuildItem()])])];

        Check.That(_0.SetContent(_2)).IsTrue();
        var dc1 = await _0.GetDataContent();
        Check.That(dc1!.DataSet!.First()).IsInstanceOf<GenericItem>();
        Check.That(_0.GetContent()!.First()!).IsInstanceOf<GenericItem>();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetContent(_2)).IsFalse();
        Check.That(_0.GetContent()!.SetEquals(_2)).IsTrue();
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        var dc2 = await _0.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _0.GetDataContent(KeyM);
        Check.That(dc3!.DataSet!.SetEquals(_2)).IsTrue();
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()!.SetEquals(_2)).IsTrue();
    }

    [Test]
    public async Task GetDataContent()
    {
        DataSetItem _i = new DataSetItem();
        Check.That(_i.RegisterManager(KeyM)).IsTrue();
        HashSet<DataItem> _c = [new GenericItem([new DataSetItem([new GuidReferenceItem(Guid.NewGuid()),new MSBuildItem()])])];

        Check.That(_i.SetContent(_c)).IsTrue();

        var dc1 = await _i.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.DataSet).IsNotNull().And.Not.IsSameReferenceAs(_c);
        Check.That(dc1.DataSet!.First()).Not.IsSameReferenceAs(_c.First());
        Check.That(dc1.DataSet).IsEquivalentTo(_c);

        Check.That(_i.MaskData(true,KeyM)).IsTrue();
        Check.That(await _i.GetDataContent()).IsNull();

        var dc2 = await _i.GetDataContent(KeyM);
        Check.That(dc2).IsNotNull();
        Check.That(dc2!.DataSet).IsNotNull().And.IsEquivalentTo(_c);
        Check.That(_i.MaskData(false,KeyM)).IsTrue();

        Check.That(await _i.EncryptData(KeyM)).IsTrue();
        Check.That(await _i.GetDataContent()).IsNull();

        var dc3 = await _i.GetDataContent(KeyM);
        Check.That(dc3).IsNotNull();
        Check.That(dc3!.DataSet).IsNotNull().And.IsEquivalentTo(_c);
    }

    [Test]
    public async Task MaskData()
    {
        DataSetItem _0 = new DataSetItem(); Guid _1 = Guid.NewGuid();
        HashSet<DataItem> _2 = [new GenericItem([new DataSetItem([new GuidReferenceItem(_1),new MSBuildItem()])])];

        Check.That(_0.SetContent(_2)).IsTrue();
        var dc1 = await _0.GetDataContent();
        Check.That(dc1!.DataSet!.First()).IsInstanceOf<GenericItem>();
        Check.That(_0.GetContent()!.First()!).IsInstanceOf<GenericItem>();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetContent(_2)).IsFalse();
        Check.That(_0.GetContent()!.SetEquals(_2)).IsTrue();
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        var dc2 = await _0.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _0.GetDataContent(KeyM);
        Check.That(dc3!.DataSet!.SetEquals(_2)).IsTrue();
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()!.SetEquals(_2)).IsTrue();
    }

    [Test]
    public void ParameterlessConstructor()
    {
        DataSetItem _0 = new DataSetItem();

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
        var _ = DataItemGenerator.CreateDataSet(1000);

        var __ = DataSetItem.Parse(_.ToString());

        Check.That(_).IsEqualTo(__);
    }

    [Test]
    public void ParseinGeneric()
    {
        var _ = DataItemGenerator.CreateDataSet(1000);

        GenericItem g = new(new DataItem[]{_});

        Check.That(GenericItem.Parse(g.ToString())!.Equals(g)).IsTrue();
    }

    [Test]
    public void ParseinMSBuild()
    {
        var _ = DataItemGenerator.CreateDataSet(1000);

        MSBuildItem b = new(); b.SetRequirements(new DataItem[]{ _ });

        Check.That(MSBuildItem.Parse(b.ToString())!.Equals(b)).IsTrue();
    }

    [Test]
    public void RemoveDataItem()
    {
        HashSet<DataItem> _ = new() { DataItemGenerator.CreateDataSet(20) , DataItemGenerator.CreateDataSet(20) };

        DataSetItem _0 = new DataSetItem();

        Check.That(_0.AddDataItems(_)).IsTrue();

        Int32 h = _0.GetHashCode();

        Check.That(_0.RemoveDataItem(_.First().GetID())).IsTrue();

        Check.That(_0.GetHashCode()).IsNotEqualTo(h);
    }

    [Test]
    public void RemoveNote()
    {
        DataSetItem _0 = new DataSetItem();
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
        DataSetItem _0 = new DataSetItem();
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
        DataSetItem _0 = new DataSetItem();
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion()
    {
        DataSetItem _0 = new DataSetItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetBornOn()
    {
        DataSetItem _0 = new DataSetItem();
        DateTimeOffset _1 = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);

        Check.That(_0.GetBornOn()).HasAValue().And.IsNotEqualTo(_1);
        Check.That(_0.SetBornOn(_1)).IsTrue();
        Check.That(_0.GetBornOn()).Equals(_1);
    }

    [Test]
    public void SetContent()
    {
        DataSetItem _0 = new DataSetItem();
        Guid _1 = Guid.NewGuid();
        HashSet<DataItem> _2 = [new GenericItem([new DataSetItem([new GuidReferenceItem(_1),new MSBuildItem()])])];

        Check.That(_0.SetContent(_2)).IsTrue();
        Check.That(_0.GetContent()!.First()!).IsInstanceOf<GenericItem>();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetContent(_2)).IsFalse();
        Check.That(_0.GetContent()!.SetEquals(_2)).IsTrue();
    }

    [Test]
    public void SetContentStreamed()
    {
        DataSetItem _0 = new();

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
        DataSetItem _0 = new DataSetItem();
        String _1 = "CN=ServiceN,OU=Operations,DC=Fabrikam,DC=NET";

        Check.That(_0.SetDistinguishedName(null)).IsFalse();
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void SetDomainID()
    {
        DataSetItem _0 = new DataSetItem();
        String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetDomainID(_1)).IsFalse();
    }

    [Test]
    public void SetExtension()
    {
        DataSetItem _0 = new DataSetItem();
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
        DataSetItem _0 = new DataSetItem();
        String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
    }

    [Test]
    public void SetID()
    {
        DataSetItem _0 = new DataSetItem();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void SetLinks()
    {
        DataSetItem _0 = new DataSetItem();
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
        DataSetItem _0 = new();

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
        DataSetItem _0 = new DataSetItem();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void SetSecurityDescriptor()
    {
        DataSetItem _0 = new DataSetItem();
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void SetServiceVersion()
    {
        DataSetItem _0 = new DataSetItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetType()
    {
        DataSetItem _0 = new DataSetItem();

        Check.That(_0.SetType(DataType.NFO)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.NFO);
        Check.That(_0.SetType(DataType.ADMX)).IsFalse();
    }

    [Test]
    public void SetVersion()
    {
        DataSetItem _0 = new DataSetItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void ToFile()
    {
        Byte[] _1 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_1);

        DataSetItem _2 = new DataSetItem([new MultiMediaItem(_1)]);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\DataSetItem04.kdi";

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();

        Check.That(_2.ToFile(_3)).IsTrue();

        DataSetItem? _4 = DataSetItem.FromFile(_3);

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

        DataSetItem _3 = new DataSetItem(content:[new BinaryItem(_1),new MultiMediaItem(_2)],id:null,name:String.Empty);

        DataSetItem? _4 = DataSetItem.Parse(_3.ToString(),null);

        Check.That(_3.Equals(_4)).IsTrue();
    }

    [Test]
    public void TryParse()
    {
        DataSetItem _0 = new DataSetItem(content:[DataItemGenerator.CreateDataSet(2)],id:Guid.NewGuid());

        DataSetItem? _1;

        DataSetItem.TryParse(_0.ToString(),null,out _1);

        Check.That(_1).IsInstanceOfType(typeof(DataSetItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        DataSetItem _0 = new DataSetItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }
}