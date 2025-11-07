namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class MultiMediaItemExam
{
    private ManagerKey? KeyM;

    [Test]
    public void AddNotes()
    {
        MultiMediaItem _0 = new MultiMediaItem();
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
        MultiMediaItem _0 = new MultiMediaItem();
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
        MultiMediaItem _0 = new MultiMediaItem();
        MultiMediaItem _1 = new MultiMediaItem();

        Check.That(_0.Equals(_0.Clone())).IsFalse();
        Check.That(_1.SetContent(new Byte[1])).IsTrue();
        Check.That(_1.Equals(_1.Clone())).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsFalse();
    }

    [Test]
    public void CompareTo()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        Thread.Sleep(100);
        MultiMediaItem _1 = new MultiMediaItem();

        Check.That(new MultiMediaItem().CompareTo(null)).IsEqualTo(1);
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
        Byte[] _2 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_2);
        String _5 = "MultiMediaExam";
        String? _3 = DataValidation.MultiMediaItemValidDataTypes[RandomNumberGenerator.GetInt32(0,DataValidation.MultiMediaItemValidDataTypes.Length)]?.ToString();
        MultiMediaItem _4 = new MultiMediaItem(content:_2,id:Guid.NewGuid(),name:_5,artists:new HashSet<String>(),notes:new HashSet<String>(),tags:new HashSet<String>(),title:null,type:_3);

        Check.That(_4).IsInstanceOfType(typeof(MultiMediaItem));
        Check.That(_4.GetContent()).IsEqualTo(_2);
    }

    [Test]
    public void opEquality()
    {
        RandomNumberGenerator _1 = RandomNumberGenerator.Create();
        RandomNumberGenerator _7 = RandomNumberGenerator.Create();
        Byte[] _2 = new Byte[8192];
        Byte[] _8 = new Byte[8192];
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        _1.GetBytes(_2);
        _7.GetBytes(_8);
        MultiMediaItem _3 = new MultiMediaItem(content:_2,id:_11);
        MultiMediaItem _15 = new MultiMediaItem(content:_2,id:_11);
        MultiMediaItem _4 = new MultiMediaItem(content:_2,id:_14);
        MultiMediaItem _5 = new MultiMediaItem(content:_2);
        MultiMediaItem _9 = new MultiMediaItem(content:_8,id:_11);
        MultiMediaItem _10 = new MultiMediaItem();
        MultiMediaItem _13 = new MultiMediaItem(content:null,id:_11);

        Check.That(new MultiMediaItem() == null).IsFalse();
        Check.That(new MultiMediaItem() == _10).IsFalse();
        Check.That(_3 == _4).IsTrue();
        Check.That(_3 == _5).IsTrue();
        Check.That(_3 == _9).IsFalse();
        Check.That(_3 == _15).IsTrue();
        Check.That(_3 == _10).IsFalse();
        Check.That(_5 == _10).IsFalse();
        Check.That(_13 == _10).IsFalse();
    }

    [Test]
    public void opInequality()
    {
        RandomNumberGenerator _1 = RandomNumberGenerator.Create();
        RandomNumberGenerator _7 = RandomNumberGenerator.Create();
        Byte[] _2 = new Byte[8192];
        Byte[] _8 = new Byte[8192];
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        _1.GetBytes(_2);
        _7.GetBytes(_8);
        MultiMediaItem _3 = new MultiMediaItem(content:_2,id:_11);
        MultiMediaItem _15 = new MultiMediaItem(content:_2,id:_11);
        MultiMediaItem _4 = new MultiMediaItem(content:_2,id:_14);
        MultiMediaItem _5 = new MultiMediaItem(content:_2);
        MultiMediaItem _9 = new MultiMediaItem(content:_8,id:_11);
        MultiMediaItem _10 = new MultiMediaItem();
        MultiMediaItem _13 = new MultiMediaItem(content:null,id:_11);

        Check.That(new MultiMediaItem() != null).IsTrue();
        Check.That(new MultiMediaItem() != _10).IsTrue();
        Check.That(_3 != _4).IsFalse();
        Check.That(_3 != _5).IsFalse();
        Check.That(_3 != _9).IsTrue();
        Check.That(_3 != _15).IsFalse();
        Check.That(_3 != _10).IsTrue();
        Check.That(_5 != _10).IsTrue();
        Check.That(_13 != _10).IsTrue();
    }

    [Test]
    public void EqualsObject()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        MultiMediaItem _1 = new MultiMediaItem();

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsFalse();
        Check.That(((Object)new MultiMediaItem()).Equals(null)).IsFalse();
        Check.That(((Object)new MultiMediaItem()).Equals(new Object())).IsFalse();
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
        MultiMediaItem _3 = new MultiMediaItem(content:_2,id:_11);
        MultiMediaItem _15 = new MultiMediaItem(content:_2,id:_11);
        MultiMediaItem _4 = new MultiMediaItem(content:_2,id:_14);
        MultiMediaItem _5 = new MultiMediaItem(content:_2);
        MultiMediaItem _9 = new MultiMediaItem(content:_8,id:_11);
        MultiMediaItem _10 = new MultiMediaItem();
        MultiMediaItem _13 = new MultiMediaItem(content:null,id:_11);

        Check.That(new MultiMediaItem().Equals(null)).IsFalse();
        Check.That(new MultiMediaItem().Equals(new Object())).IsFalse();
        Check.That(new MultiMediaItem().Equals(_10)).IsFalse();
        Check.That(_3.Equals(_4)).IsTrue();
        Check.That(_3.Equals(_5)).IsTrue();
        Check.That(_3.Equals(_9)).IsFalse();
        Check.That(_3.Equals(_15)).IsTrue();
        Check.That(_3.Equals(_10)).IsFalse();
        Check.That(_5.Equals(_10)).IsFalse();
        Check.That(_13.Equals(_10)).IsFalse();
    }

    [Test]
    public void FromFile()
    {
        Byte[] _1 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_1);

        MultiMediaItem _2 = new MultiMediaItem(content:_1,id:null,name:String.Empty);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\MultiMediaItem03.bin";

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();

        Check.That(_2.ToFile(_3)).IsTrue();

        MultiMediaItem? _4 = MultiMediaItem.FromFile(_3);

        Check.That(_4).IsNotNull();

        Check.That(_2.Equals(_4)).IsTrue();

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();
    }

    [Test]
    public void GetApplication()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "GetApplication";

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void GetApplicationVersion()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetArtists()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _2 = "GetArtistsExam";
        HashSet<String> _1 = new HashSet<String>(){_2,"Pass"};

        Check.That(_0!.SetArtists(_1)).IsTrue();
        Check.That(_0.GetArtists()).ContainsExactly(_2,"Pass");
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetArtists()).ContainsExactly(_2,"Pass");
    }

    [Test]
    public void GetBitrate()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetBitrate(48000000)).IsTrue();
        Check.That(_0.GetBitrate()).HasAValue();
    }

    [Test]
    public void GetBornOn()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public async Task GetContent()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();
        Byte[] _1 = new Byte[8192]; _0.GetBytes(_1);
        MultiMediaItem _3 = new MultiMediaItem(_1);

        Check.That(_3.GetContent()!).IsEqualTo(_1);
        var dc1 = await _3.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.MultiMedia).IsNotNull();
        Check.That(dc1.MultiMedia!.ContainsKey("Content")).IsTrue();
        Check.That(dc1.MultiMedia["Content"]).IsEqualTo(_1);
        Check.That(_3.Lock(KeyM)).IsTrue();
        Check.That(_3.SetContent(_1)).IsFalse();
        Check.That(_3.GetContent()!).IsEqualTo(_1);
        Check.That(ReferenceEquals(_3.GetContent()!,_1)).IsFalse();
        Check.That(_3.MaskData(true,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsNull();
        var dc2 = await _3.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _3.GetDataContent(KeyM);
        Check.That(dc3).IsNotNull();
        Check.That(dc3!.MultiMedia).IsNotNull();
        Check.That(dc3.MultiMedia!.ContainsKey("Content")).IsTrue();
        Check.That(dc3.MultiMedia["Content"]).IsEqualTo(_1);
        Check.That(_3.MaskData(false,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsEqualTo(_1);
    }

    [Test]
    public async Task GetDataContent()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();
        Byte[] _1 = new Byte[8192]; _0.GetBytes(_1);
        MultiMediaItem _3 = new MultiMediaItem(_1);

        Check.That(_3.GetContent()!).IsEqualTo(_1);
        var dc1 = await _3.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.MultiMedia).IsNotNull();
        Check.That(dc1.MultiMedia!.ContainsKey("Content")).IsTrue();
        Check.That(dc1.MultiMedia["Content"]).IsEqualTo(_1);
        Check.That(_3.Lock(KeyM)).IsTrue();
        Check.That(_3.SetContent(_1)).IsFalse();
        Check.That(_3.GetContent()!).IsEqualTo(_1);
        Check.That(ReferenceEquals(_3.GetContent()!,_1)).IsFalse();
        Check.That(_3.MaskData(true,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsNull();
        var dc2 = await _3.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _3.GetDataContent(KeyM);
        Check.That(dc3).IsNotNull();
        Check.That(dc3!.MultiMedia).IsNotNull();
        Check.That(dc3.MultiMedia!.ContainsKey("Content")).IsTrue();
        Check.That(dc3.MultiMedia["Content"]).IsEqualTo(_1);
        Check.That(_3.MaskData(false,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsEqualTo(_1);
    }

    [Test]
    public void GetDescriptor()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        String _s = "GetDescriptorExam";
        Version _v = new Version("1.2.3.4");
        DateTimeOffset? _d = _0.GetModified();
        HashSet<String> _n = new(){"Get","Descriptor","Exam","Notes"};
        HashSet<String> _t = new(){"Get","Descriptor","Exam","Tags"};
        HashSet<String> _a = new(){"Artist"};
        Int32 _y = 1993;

        Check.That(_0.SetApplication(_s)).IsTrue();
        Check.That(_0.SetApplicationVersion(_v)).IsTrue();
        Check.That(_0.SetArtists(_a)).IsTrue();
        Check.That(_0.SetDistinguishedName(_s)).IsTrue();
        Check.That(_0.SetModified(_d)).IsTrue();
        Check.That(_0.SetName(_s)).IsTrue();
        Check.That(_0.AddNotes(_n)).IsTrue();
        Check.That(_0.SetServiceVersion(_v)).IsTrue();
        Check.That(_0.AddTags(_t)).IsTrue();
        Check.That(_0.SetType(DataType.MPG)).IsTrue();
        Check.That(_0.SetVersion(_v)).IsTrue();
        Check.That(_0.SetYear(_y)).IsTrue();

        Descriptor? _ = _0.GetDescriptor();

        Check.That(_!.Application).IsEqualTo(_s);
        Check.That(_.ApplicationVersion).IsEqualTo(_v.ToString());
        Check.That(_.Artist).Equals(_a.FirstOrDefault());
        Check.That(_.BornOn).Equals(_0.GetBornOn()?.ToString("O"));
        Check.That(_.DistinguishedName).IsEqualTo(_s);
        Check.That(_.ID).Equals(_0.GetID());
        Check.That(_.Modified).IsNotEqualTo(_d?.ToString("O"));
        Check.That(_.Name).IsEqualTo(_s);
        Check.That(_.Notes).Contains(_n);
        Check.That(_.ObjectType!.Split(" -> ")).Contains("KusDepot.MultiMediaItem");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Type).Equals(DataType.MPG);
        Check.That(_.Version).IsEqualTo(_v.ToString());
        Check.That(_.Year).Equals(_y.ToString(CultureInfo.InvariantCulture));

        Check.That(_0.SetID(Guid.Empty)).IsTrue();
        Check.That(_0.GetID()).IsNull();
        Check.That(_0.GetDescriptor()).IsNull();
    }

    [Test]
    public void GetDistinguishedName()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "CN=ServiceN,OU=Engineering,DC=TailSpinToys,DC=COM";

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void GetDomainID()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "GetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
    }

    [Test]
    public void GetDuration()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetDuration(Decimal.MaxValue)).IsTrue();
        Check.That(_0.GetDuration()).IsEqualTo(Decimal.MaxValue);
    }

    [Test]
    public void GetExtension()
    {
        MultiMediaItem _0 = new MultiMediaItem();
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
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "GetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.SetFILE(String.Empty)).IsTrue();
        Check.That(_0.GetFILE()).IsNull();
    }

    [Test]
    public void GetFramerate()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetFramerate(Single.MaxValue)).IsTrue();
        Check.That(_0.GetFramerate()).IsEqualTo(Single.MaxValue);
    }

    [Test]
    public void GetHash()
    {
        Byte[] _ = new Byte[3]{0xA,0xB,0xE};
        MultiMediaItem _0 = new MultiMediaItem(content:null,id:Guid.NewGuid());
        MultiMediaItem _1 = new MultiMediaItem(content:null,id:new Guid("00021401-0000-0000-C000-000000000046"));
        MultiMediaItem _2 = new MultiMediaItem(content:_,id:new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"));
        MultiMediaItem _3 = new(_);

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_0.GetHashCode()).IsNotEqualTo(_1.GetHashCode());
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        Check.That(_2.GetHashCode()).IsNotEqualTo(_3.GetHashCode());
    }

    [Test]
    public void GetID()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.GetID()).HasAValue();
    }

    [Test]
    public void GetLanguage()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetLanguage(KusDepot.Language.swa)).IsTrue();
        Check.That(_0.GetLanguage()).IsEqualTo(Language.swa);
    }

    [Test]
    public void GetLinks()
    {
        MultiMediaItem _0 = new MultiMediaItem();
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
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetModified()
    {
        MultiMediaItem _0 = new();

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
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "GetNameExam";

        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void GetNotes()
    {
        MultiMediaItem _0 = new MultiMediaItem();
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
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "GetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void GetServiceVersion() 
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void GetSource() 
    {
        MultiMediaItem _0 = new MultiMediaItem();
        MemoryStream _1 = new MemoryStream();

        Check.That(_0.SetSource(_1)).IsTrue();
        Check.That(ReferenceEquals(_0.GetSource(),_1)).IsTrue();
        var KeyM = _0.CreateManagementKey("GetSourceExam");
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetSource()).IsNull();
    }

    [Test]
    public void GetTags()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        List<String> _1 = new List<String>(){"GetTagsExam", "Pass"};

        Check.That(_0.AddTags(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
    }

    [Test]
    public new void GetType()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetType(DataType.MP4)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.MP4);
    }

    [Test]
    public void GetTitle()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "GetTitleExam";

        Check.That(_0.SetTitle(_1)).IsTrue();
        Check.That(_0.GetTitle()).Equals(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetTitle(_1)).IsFalse();
    }

    [Test]
    public void GetVersion()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void GetYear()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetYear(DateTimeOffset.Now.Year)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetYear()).Equals(DateTimeOffset.Now.Year);
        Check.That(_0.SetYear(DateTimeOffset.Now.AddYears(2023).Year)).IsFalse();
    }

    [Test]
    public void Initialize()
    {
        MultiMediaItem _1 = new MultiMediaItem();

        Check.That(_1.Initialize()).IsTrue();

        Check.That(_1.GetID()).HasAValue();
    }

    [Test]
    public void Lock()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public async Task MaskData()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();
        Byte[] _1 = new Byte[8192]; _0.GetBytes(_1);
        MultiMediaItem _3 = new MultiMediaItem(_1);

        Check.That(_3.GetContent()!).IsEqualTo(_1);
        var dc1 = await _3.GetDataContent();
        Check.That(dc1).IsNotNull();
        Check.That(dc1!.MultiMedia).IsNotNull();
        Check.That(dc1.MultiMedia!.ContainsKey("Content")).IsTrue();
        Check.That(dc1.MultiMedia["Content"]).IsEqualTo(_1);
        Check.That(_3.Lock(KeyM)).IsTrue();
        Check.That(_3.SetContent(_1)).IsFalse();
        Check.That(_3.GetContent()!).IsEqualTo(_1);
        Check.That(ReferenceEquals(_3.GetContent()!,_1)).IsFalse();
        Check.That(_3.MaskData(true,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsNull();
        var dc2 = await _3.GetDataContent();
        Check.That(dc2).IsNull();
        var dc3 = await _3.GetDataContent(KeyM);
        Check.That(dc3).IsNotNull();
        Check.That(dc3!.MultiMedia).IsNotNull();
        Check.That(dc3.MultiMedia!.ContainsKey("Content")).IsTrue();
        Check.That(dc3.MultiMedia["Content"]).IsEqualTo(_1);
        Check.That(_3.MaskData(false,KeyM)).IsTrue();
        Check.That(_3.GetContent()).IsEqualTo(_1);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.GetApplicationVersion()).IsNull();
        Check.That(_0.GetArtists()).IsNull();
        Check.That(_0.GetBitrate()).IsNull();
        Check.That(_0.GetBornOn()).IsNotNull();
        Check.That(_0.GetContent()).IsNull();
        Check.That(_0.GetDistinguishedName()).IsNull();
        Check.That(_0.GetDomainID()).IsNull();
        Check.That(_0.GetDuration()).IsNull();
        Check.That(_0.GetExtension()).IsNull();
        Check.That(_0.GetFILE()).IsNull();
        Check.That(_0.GetFramerate()).IsNull();
        Check.That(_0.GetID()).IsNotNull();
        Check.That(_0.GetLanguage()).IsNull();
        Check.That(_0.GetLinks()).IsNull();
        Check.That(_0.GetModified()).IsNotNull();
        Check.That(_0.GetName()).IsNull();
        Check.That(_0.GetNotes()).IsNull();
        Check.That(_0.GetSecurityDescriptor()).IsNull();
        Check.That(_0.GetServiceVersion()).IsNull();
        Check.That(_0.GetSource()).IsNull();
        Check.That(_0.GetTags()).IsNull();
        Check.That(_0.GetTitle()).IsNull();
        Check.That(_0.GetType()).IsNull();
        Check.That(_0.GetYear()).IsNull();
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Parse()
    {
        MultiMediaItem _0 = new MultiMediaItem(content:"ParseExam".ToByteArrayFromUTF16String(),id:Guid.NewGuid());

        MultiMediaItem? _1 = MultiMediaItem.Parse(_0.ToString(),null);

        Check.That(_1).IsInstanceOfType(typeof(MultiMediaItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void RemoveNote()
    {
        MultiMediaItem _0 = new MultiMediaItem();
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
        MultiMediaItem _0 = new MultiMediaItem();
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
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetArtists()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _2 = "SetArtistsExam";
        HashSet<String> _1 = new HashSet<String>(){_2,"Pass"};

        Check.That(_0!.SetArtists(_1)).IsTrue();
        Check.That(_0.GetArtists()).ContainsExactly(_2,"Pass");
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetArtists()).ContainsExactly(_2,"Pass");
    }

    [Test]
    public void SetBitrate()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetBitrate(48000000)).IsTrue();
        Check.That(_0.GetBitrate()).HasAValue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetBitrate()).Equals(48000000);
    }

    [Test]
    public void SetBornOn()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        DateTimeOffset _1 = new DateTimeOffset(1969,7,20,20,17,0,TimeSpan.Zero);

        Check.That(_0.GetBornOn()).HasAValue().And.IsNotEqualTo(_1);
        Check.That(_0.SetBornOn(_1)).IsTrue();
        Check.That(_0.GetBornOn()).Equals(_1);
    }

    [Test]
    public void SetContent()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();
        Byte[] _1 = new Byte[8192];
        _0.GetBytes(_1);
        MultiMediaItem _3 = new MultiMediaItem();

        Check.That(_3.SetContent(_1)).IsTrue();
        Check.That(_3.GetContent()!).IsEqualTo(_1);
        Check.That(_3.Lock(KeyM)).IsTrue();
        Check.That(_3.SetContent(_1)).IsFalse();
    }

    [Test]
    public void SetContentStreamed()
    {
        MultiMediaItem _0 = new();

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
        MultiMediaItem _0 = new MultiMediaItem();
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
    public void SetDuration()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetDuration(Decimal.MaxValue)).IsTrue();
        Check.That(_0.GetDuration()).IsEqualTo(Decimal.MaxValue);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetDuration(Decimal.MaxValue)).IsFalse();
    }

    [Test]
    public void SetExtension()
    {
        MultiMediaItem _0 = new MultiMediaItem();
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
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "SetFILEExam";

        Check.That(_0.SetFILE(_1)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetFILE()).IsEqualTo(_1);
    }

    [Test]
    public void SetFramerate()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetFramerate(Single.MaxValue)).IsTrue();
        Check.That(_0.GetFramerate()).IsEqualTo(Single.MaxValue);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetFramerate(Single.MaxValue)).IsFalse();
    }

    [Test]
    public void SetID()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void SetLanguage()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetLanguage(KusDepot.Language.swa)).IsTrue();
        Check.That(_0.GetLanguage()).IsEqualTo(Language.swa);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetLanguage(KusDepot.Language.nya)).IsFalse();
    }

    [Test]
    public void SetLinks()
    {
        MultiMediaItem _0 = new MultiMediaItem();
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
        MultiMediaItem _0 = new();

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
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void SetSecurityDescriptor()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void SetServiceVersion()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetSource() 
    {
        MultiMediaItem _0 = new MultiMediaItem();
        MemoryStream _1 = new MemoryStream();

        Check.That(_0.SetSource(_1)).IsTrue();
        Check.That(ReferenceEquals(_0.GetSource(),_1)).IsTrue();
        var KeyM = _0.CreateManagementKey("SetSourceExam");
        Check.That(_0.MaskData(true,KeyM)).IsTrue();
        Check.That(_0.GetSource()).IsNull();
    }

    [Test]
    public void SetTitle()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "GetTitleExam";

        Check.That(_0.SetTitle(_1)).IsTrue();
        Check.That(_0.GetTitle()).Equals(_1);
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetTitle(_1)).IsFalse();
    }

    [Test]
    public void SetType()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetType(DataType.MP4)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetType()).Equals(DataType.MP4);
        Check.That(_0.SetType(DataType.MP3)).IsFalse();
    }

    [Test]
    public void SetVersion()
    {
        MultiMediaItem _0 = new MultiMediaItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetYear()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.SetYear(DateTimeOffset.Now.Year)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetYear()).Equals(DateTimeOffset.Now.Year);
        Check.That(_0.SetYear(DateTimeOffset.Now.AddYears(2023).Year)).IsFalse();
    }

    [Test]
    public void ToFile()
    {
        Byte[] _1 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_1);

        MultiMediaItem _2 = new MultiMediaItem(content:_1,id:null,name:String.Empty);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\MultiMediaItem0001.bin";

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();

        Check.That(_2.ToFile(_3)).IsTrue();

        MultiMediaItem? _4 = MultiMediaItem.FromFile(_3);

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

        MultiMediaItem _2 = new MultiMediaItem(_1);

        MultiMediaItem? _3 = MultiMediaItem.Parse(_2.ToString(),null);

        Check.That(_2.Equals(_3)).IsTrue();
    }

    [Test]
    public void TryParse()
    {
        MultiMediaItem _0 = new MultiMediaItem(content:"TryParseExam".ToByteArrayFromUTF16String(),id:Guid.NewGuid());

        MultiMediaItem? _1;

        MultiMediaItem.TryParse(_0.ToString(),null,out _1);

        Check.That(_1).IsInstanceOfType(typeof(MultiMediaItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        MultiMediaItem _0 = new MultiMediaItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }
}