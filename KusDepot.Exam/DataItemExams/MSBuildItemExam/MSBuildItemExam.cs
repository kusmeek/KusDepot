namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class MSBuildItemExam
{
    private ManagerKey? KeyM;

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
    public void Calibrate()
    {
        if(Settings.NoExceptions is true) { throw new InvalidOperationException(); }

        KeyM = new( Utility.SerializeCertificate(new CertificateRequest("CN=Management",RSA.Create(4096),HashAlgorithmName.SHA512,RSASignaturePadding.Pss).CreateSelfSigned(DateTimeOffset.Now,DateTimeOffset.Now.AddYears(1)))! );        

        if(KeyM is null) { throw new InvalidOperationException(); }
    }

    [Test]
    public void Clone()
    {
        MSBuildItem _0 = new MSBuildItem();
        MSBuildItem _1 = new MSBuildItem();

        Check.That(_0.Equals(_0.Clone())).IsFalse();
        Check.That(_1.SetContent("CloneExam")).IsTrue();
        Check.That(_1.Equals(_1.Clone())).IsTrue();
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
        MSBuildItem _3 = new MSBuildItem(content:_0,id:Guid.NewGuid());

        Check.That(_3).IsInstanceOfType(typeof(MSBuildItem));
        Check.That(_3.GetContent()).IsEqualTo(_0.ToString());
    }

    [Test]
    public void opEquality()
    {
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        String _6 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        MSBuildItem _3 = new MSBuildItem(content:_0,id:_11,name:"_3");
        MSBuildItem _15 = new MSBuildItem(content:_0,id:_11,name:"_15");
        MSBuildItem _4 = new MSBuildItem(content:_0,id:_14,name:"_4");
        MSBuildItem _5 = new MSBuildItem(content:_0,id:null,name:"_5");
        MSBuildItem _9 = new MSBuildItem(content:_6,id:_11,name:"_9");
        MSBuildItem _10 = new MSBuildItem(content:_6,id:_14,name:"_10");
        MSBuildItem _16 = new MSBuildItem(content:null,id:_14,name:"_16");
        MSBuildItem _17 = new MSBuildItem(content:null,id:null,name:"_17");
        MSBuildItem _18 = new MSBuildItem(content:null,id:_14,name:"_18");
        MSBuildItem _19 = new MSBuildItem(content:String.Empty,id:null,name:"_19");
        MSBuildItem _20 = new MSBuildItem(content:String.Empty,id:null,name:"_20");

        SortedList<Int32,MSBuildItem> _13 = new(); _13.Add(0,_10); _13.Add(1,_5);
        HashSet<DataItem> _12 = new(); _12.Add(_3);
        MSBuildItem _21 = new MSBuildItem(); _21.SetName("_21");
        MSBuildItem _22 = new MSBuildItem(); _22.SetName("_22");
        MSBuildItem _23 = new MSBuildItem(); _23.SetName("_23");

        Check.That(new MSBuildItem() == null).IsFalse();
        Check.That(new MSBuildItem() == _17).IsFalse();
        Check.That(_3 == _4).IsTrue();
        Check.That(_3 == _5).IsTrue();
        Check.That(_3 == _9).IsFalse();
        Check.That(_3 == _15).IsTrue();
        Check.That(_16 == _10).IsFalse();
        Check.That(_16 == _17).IsFalse();
        Check.That(_16 == _18).IsFalse();
        Check.That(_5 == _17).IsFalse();
        Check.That(_19 == _20).IsFalse();
        Check.That(_21.SetRequirements(_12)).IsTrue();
        Check.That(_21.SetSequence(_13)).IsTrue();
        Check.That(_22.SetSequence(_13)).IsTrue();
        Check.That(_22.SetRequirements(_12)).IsTrue();
        Check.That(_21 == _22).IsTrue();
        Check.That(_23.SetRequirements(_12)).IsTrue();
        Check.That(_21 == _23).IsFalse();
    }

    [Test]
    public void opInequality()
    {
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        String _0 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        String _6 = TestCaseDataGenerator.GenerateUnicodeString(8192);
        MSBuildItem _3 = new MSBuildItem(content:_0,id:_11,name:"_3");
        MSBuildItem _15 = new MSBuildItem(content:_0,id:_11,name:"_15");
        MSBuildItem _4 = new MSBuildItem(content:_0,id:_14,name:"_4");
        MSBuildItem _5 = new MSBuildItem(content:_0,id:null,name:"_5");
        MSBuildItem _9 = new MSBuildItem(content:_6,id:_11,name:"_9");
        MSBuildItem _10 = new MSBuildItem(content:_6,id:_14,name:"_10");
        MSBuildItem _16 = new MSBuildItem(content:null,id:_14,name:"_16");
        MSBuildItem _17 = new MSBuildItem(content:null,id:null,name:"_17");
        MSBuildItem _18 = new MSBuildItem(content:null,id:_14,name:"_18");
        MSBuildItem _19 = new MSBuildItem(content:String.Empty,id:null,name:"_19");
        MSBuildItem _20 = new MSBuildItem(content:String.Empty,id:null,name:"_20");

        SortedList<Int32,MSBuildItem> _13 = new(); _13.Add(0,_10); _13.Add(1,_5);
        HashSet<DataItem> _12 = new(); _12.Add(_3);
        MSBuildItem _21 = new MSBuildItem(); _21.SetName("_21");
        MSBuildItem _22 = new MSBuildItem(); _22.SetName("_22");
        MSBuildItem _23 = new MSBuildItem(); _23.SetName("_23");

        Check.That(new MSBuildItem() != null).IsTrue();
        Check.That(new MSBuildItem() != _17).IsTrue();
        Check.That(_3 != _4).IsFalse();
        Check.That(_3 != _5).IsFalse();
        Check.That(_3 != _9).IsTrue();
        Check.That(_3 != _15).IsFalse();
        Check.That(_16 != _10).IsTrue();
        Check.That(_16 != _17).IsTrue();
        Check.That(_16 != _18).IsTrue();
        Check.That(_5 != _17).IsTrue();
        Check.That(_19 != _20).IsTrue();
        Check.That(_21.SetRequirements(_12)).IsTrue();
        Check.That(_21.SetSequence(_13)).IsTrue();
        Check.That(_22.SetSequence(_13)).IsTrue();
        Check.That(_22.SetRequirements(_12)).IsTrue();
        Check.That(_21 != _22).IsFalse();
        Check.That(_23.SetRequirements(_12)).IsTrue();
        Check.That(_21 != _23).IsTrue();
    }

    [Test]
    public void EqualsObject()
    {
        MSBuildItem _0 = new MSBuildItem();
        MSBuildItem _1 = new MSBuildItem();

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsFalse();
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
        MSBuildItem _3 = new MSBuildItem(content:_0,id:_11,name:"_3");
        MSBuildItem _15 = new MSBuildItem(content:_0,id:_11,name:"_15");
        MSBuildItem _4 = new MSBuildItem(content:_0,id:_14,name:"_4");
        MSBuildItem _5 = new MSBuildItem(content:_0,id:null,name:"_5");
        MSBuildItem _9 = new MSBuildItem(content:_6,id:_11,name:"_9");
        MSBuildItem _10 = new MSBuildItem(content:_6,id:_14,name:"_10");
        MSBuildItem _16 = new MSBuildItem(content:null,id:_14,name:"_16");
        MSBuildItem _17 = new MSBuildItem(content:null,id:null,name:"_17");
        MSBuildItem _18 = new MSBuildItem(content:null,id:_14,name:"_18");
        MSBuildItem _19 = new MSBuildItem(content:String.Empty,id:null,name:"_19");
        MSBuildItem _20 = new MSBuildItem(content:String.Empty,id:null,name:"_20");

        SortedList<Int32,MSBuildItem> _13 = new SortedList<Int32,MSBuildItem>(); _13.Add(0,_10); _13.Add(1,_5);
        HashSet<DataItem> _12 = new HashSet<DataItem>(); _12.Add(_3);
        MSBuildItem _21 = new MSBuildItem(); _21.SetName("_21");
        MSBuildItem _22 = new MSBuildItem(); _22.SetName("_22");
        MSBuildItem _23 = new MSBuildItem(); _23.SetName("_23");

        Check.That(new MSBuildItem().Equals(null)).IsFalse();
        Check.That(new MSBuildItem().Equals(new Object())).IsFalse();
        Check.That(new MSBuildItem().Equals(_17)).IsFalse();
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
        Check.That(_19.Equals(_20)).IsFalse();
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
    public void GetApplication()
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetBornOn()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public async Task GetContent()
    {
        String _2 = Guid.NewGuid().ToString();
        MSBuildItem _0 = new MSBuildItem(_2);

        Check.That(_0.GetContent()).Equals(_2);
        var dc1 = await _0.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.MSBuild).IsNotNull();
        Check.That(dc1.MSBuild!.ContainsKey("Content")).IsTrue();
        Check.That(dc1.MSBuild["Content"]).Equals(_2);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
        Check.That(ReferenceEquals(_0.GetContent()!,_2)).IsFalse();
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        var dc2 = await _0.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _0.GetDataContent(KeyM);
        Check.That(dc3).IsNotNull();
        Check.That(dc3!.MSBuild).IsNotNull();
        Check.That(dc3.MSBuild!.ContainsKey("Content")).IsTrue();
        Check.That(dc3.MSBuild["Content"]).Equals(_2);
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
    }

    [Test]
    public async Task GetDataContent()
    {
        String _c = Guid.NewGuid().ToString();
        MSBuildItem _i = new MSBuildItem();
        Check.That(_i.RegisterManager(KeyM)).IsTrue();
        HashSet<DataItem> _r = new(){ new TextItem("requirement") };
        SortedList<Int32,MSBuildItem> _s = new(){ {0, new MSBuildItem("sequence")} };

        Check.That(_i.SetContent(_c)).IsTrue();
        Check.That(_i.SetRequirements(_r)).IsTrue();
        Check.That(_i.SetSequence(_s)).IsTrue();

        var dc1 = await _i.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.MSBuild).IsNotNull();
        Check.That(dc1.MSBuild!["Content"]).IsEqualTo(_c);
        Check.That(dc1.MSBuild["Requirements"] as HashSet<DataItem>).IsNotNull().And.Not.IsSameReferenceAs(_r);
        Check.That((dc1.MSBuild["Requirements"] as HashSet<DataItem>)!.First()).Not.IsSameReferenceAs(_r.First());
        Check.That(dc1.MSBuild["Sequence"] as SortedList<Int32,MSBuildItem>).IsNotNull().And.Not.IsSameReferenceAs(_s);
        Check.That((dc1.MSBuild["Sequence"] as SortedList<Int32,MSBuildItem>)![0]).Not.IsSameReferenceAs(_s[0]);

        Check.That(_i.MaskData(true,KeyM)).IsTrue();
        Check.That(await _i.GetDataContent()).IsNull();

        var dc2 = await _i.GetDataContent(KeyM);
        Check.That(dc2).IsNotNull();
        Check.That(dc2!.MSBuild).IsNotNull();
        Check.That(dc2.MSBuild!["Content"]).IsEqualTo(_c);
        Check.That(dc2.MSBuild["Requirements"] as IEnumerable<DataItem>).IsNotNull().And.IsEquivalentTo(_r);
        Check.That(dc2.MSBuild["Sequence"] as SortedList<Int32,MSBuildItem>).IsNotNull().And.IsEqualTo(_s);
        Check.That(_i.MaskData(false,KeyM)).IsTrue();

        Check.That(await _i.EncryptData(KeyM)).IsTrue();
        Check.That(await _i.GetDataContent()).IsNull();

        var dc3 = await _i.GetDataContent(KeyM);
        Check.That(dc3).IsNotNull();
        Check.That(dc3!.MSBuild).IsNotNull();
        Check.That(dc3.MSBuild!["Content"]).IsEqualTo(_c);
        Check.That(dc3.MSBuild["Requirements"] as IEnumerable<DataItem>).IsNotNull().And.IsEquivalentTo(_r);
        Check.That(dc3.MSBuild["Sequence"] as SortedList<Int32,MSBuildItem>).IsNotNull().And.IsEqualTo(_s);
    }

    [Test]
    public void GetDescriptor()
    {
        MSBuildItem _0 = new MSBuildItem();

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
        Check.That(_.ObjectType!.Split(" -> ")).Contains("KusDepot.MSBuildItem");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Type).Equals(DataType.MSB);
        Check.That(_.Version).IsEqualTo(_v.ToString());

        Check.That(_0.SetID(Guid.Empty)).IsTrue();
        Check.That(_0.GetID()).IsNull();
        Check.That(_0.GetDescriptor()).IsNull();
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
    public void GetHash()
    {
        String _ = "MSBuildItemExam";
        MSBuildItem _0 = new MSBuildItem(content:null,id:Guid.NewGuid(),name:String.Empty);
        MSBuildItem _1 = new MSBuildItem(content:null,id:new Guid("00021401-0000-0000-C000-000000000046"),name:String.Empty);
        MSBuildItem _2 = new MSBuildItem(content:_,id:new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"),name:String.Empty);
        MSBuildItem _3 = new(_);

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_0.GetHashCode()).IsNotEqualTo(_1.GetHashCode());
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        Check.That(_2.GetHashCode()).IsNotEqualTo(_3.GetHashCode());

        HashSet<DataItem> _4 = new(); _4.Add(_1);

        Check.That(_0.SetRequirements(_4)).IsTrue();
        Check.That(_0.GetHashCode()).IsNotEqualTo(_1.GetHashCode());

        SortedList<Int32,MSBuildItem> _5 = new(); _5.Add(0,_1);

        MSBuildItem _6 = new(content:null,id:Guid.NewGuid());

        Check.That(_6.SetRequirements(_4)).IsTrue();
        Check.That(_6.SetSequence(_5)).IsTrue();
        Check.That(_0.SetSequence(_5)).IsTrue();
        Check.That(_0.GetHashCode()).IsNotEqualTo(_6.GetHashCode());
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
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetModified()
    {
        MSBuildItem _0 = new();

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
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetNotes()).Contains(_1);
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
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetRequirements()).Contains(_1);
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
        Check.That(_0.Lock(KeyM)).IsTrue();
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

        Check.That(_0.SetType(DataType.SFPROJ)).IsFalse();
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

        Check.That(_1.GetID()).HasAValue();
    }

    [Test]
    public void Lock()
    {
        MSBuildItem _0 = new MSBuildItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public async Task MaskData()
    {
        String _2 = Guid.NewGuid().ToString();
        MSBuildItem _0 = new MSBuildItem(_2);

        Check.That(_0.GetContent()).Equals(_2);
        var dc1 = await _0.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.MSBuild).IsNotNull();
        Check.That(dc1.MSBuild!.ContainsKey("Content")).IsTrue();
        Check.That(dc1.MSBuild["Content"]).Equals(_2);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
        Check.That(ReferenceEquals(_0.GetContent()!,_2)).IsFalse();
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetContent()).IsNull();
        var dc2 = await _0.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _0.GetDataContent(KeyM);
        Check.That(dc3).IsNotNull();
        Check.That(dc3!.MSBuild).IsNotNull();
        Check.That(dc3.MSBuild!.ContainsKey("Content")).IsTrue();
        Check.That(dc3.MSBuild["Content"]).Equals(_2);
        Check.That(_0.MaskData(false,KeyM)).IsTrue();
        Check.That(_0.GetContent()).Equals(_2);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        MSBuildItem _0 = new MSBuildItem();

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
        Check.That(_0.GetRequirements()).IsNull();
        Check.That(_0.GetSecurityDescriptor()).IsNull();
        Check.That(_0.GetSequence()).IsNull();
        Check.That(_0.GetServiceVersion()).IsNull();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.GetType()).Equals(DataType.MSB);
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Parse()
    {
        MSBuildItem _0 = new MSBuildItem(content:"ParseExam",id:Guid.NewGuid());

        MSBuildItem? _1 = MSBuildItem.Parse(_0.ToString(),null);

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
    public void SetContent()
    {
        MSBuildItem _0 = new MSBuildItem();
        String _1 = Guid.NewGuid().ToString();

        Check.That(_0.SetContent(_1)).IsTrue();
        Check.That(_0.GetContent()!).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetContent(_1)).IsFalse();
    }

    [Test]
    public void SetContentStreamed()
    {
        MSBuildItem _0 = new();

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
        Check.That(_0.Lock(KeyM)).IsTrue();
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
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
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
        MSBuildItem _0 = new();

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
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetSequence()![0]).IsEqualTo(_1);
    }

    [Test]
    public void SetType()
    {
        MSBuildItem _0 = new MSBuildItem();

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
        MSBuildItem _0 = new MSBuildItem(content:TestCaseDataGenerator.GenerateUnicodeString(8192),id:null,name:String.Empty);

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

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }
}