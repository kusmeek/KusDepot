namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable(ParallelScope.All)]
public partial class DataStreamItemExam
{
    private ManagerKey? KeyM;

    [Test]
    public void AddNotes()
    {
        DataStreamItem _0 = new DataStreamItem();
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
        DataStreamItem _0 = new DataStreamItem();
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

    [Test]
    public async Task BindLiveContent()
    {
        String _0 = Path.GetTempFileName();

        try
        {
            DataStreamItem _1 = new();
            MemoryStream _2 = new([1,2,3]);

            Check.That(_1.GetContentBindingMode()).IsEqualTo(DataStreamItemContentBindingMode.Static);
            Check.That(_1.GetLiveContentStatus()).IsEqualTo(DataStreamItemLiveContentStatus.None);
            Check.That(_1.GetLiveContentFaultMessage()).IsNull();

            Check.That(_1.BindLiveContent(_2)).IsTrue();
            Check.That(_1.GetContent()).IsSameReferenceAs(_2);
            Check.That(_1.GetContentStreamed()).IsTrue();
            Check.That(_1.GetContentBindingMode()).IsEqualTo(DataStreamItemContentBindingMode.LiveSessionManaged);
            Check.That(_1.GetLiveContentStatus()).IsEqualTo(DataStreamItemLiveContentStatus.Open);
            Check.That(_1.GetLiveContentFaultMessage()).IsNull();
            Check.That(_1.BindLiveContent(new MemoryStream([4,5,6]))).IsFalse();
            Check.That(_1.SetContent(new MemoryStream([7,8,9]))).IsFalse();
            Check.That(_1.SetFILE(_0)).IsFalse();
            Check.That(await _1.SetContentStreamed(false,security:null)).IsFalse();

            Check.That(_1.FaultLiveContent("BindLiveContentExam")).IsTrue();
            Check.That(_1.GetLiveContentStatus()).IsEqualTo(DataStreamItemLiveContentStatus.Faulted);
            Check.That(_1.GetLiveContentFaultMessage()).IsEqualTo("BindLiveContentExam");
            Check.That(_1.AbortLiveContent()).IsFalse();
            Check.That(_1.CompleteLiveContent()).IsFalse();

            DataStreamItem _3 = new();

            Check.That(_3.BindLiveContent(new MemoryStream([0xA]))).IsTrue();
            Check.That(_3.CompleteLiveContent()).IsTrue();
            Check.That(_3.GetLiveContentStatus()).IsEqualTo(DataStreamItemLiveContentStatus.Completed);
            Check.That(_3.GetLiveContentFaultMessage()).IsNull();

            DataStreamItem _4 = new();

            Check.That(_4.BindLiveContent(new MemoryStream([0xB]))).IsTrue();
            Check.That(_4.AbortLiveContent()).IsTrue();
            Check.That(_4.GetLiveContentStatus()).IsEqualTo(DataStreamItemLiveContentStatus.Aborted);
            Check.That(_4.GetLiveContentFaultMessage()).IsNull();
        }
        finally
        {
            if(File.Exists(_0)) { File.Delete(_0); }
        }
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
        DataStreamItem _0 = new DataStreamItem();
        DataStreamItem _1 = new DataStreamItem();

        Check.That(_0.Equals(_0.Clone())).IsTrue();
        Check.That(_1.SetContent(new MemoryStream([1]))).IsTrue();
        Check.That(_1.Equals(_1.Clone())).IsTrue();
        Check.That(_0.Equals(_1.Clone())).IsFalse();
    }

    [Test]
    public void CompareTo()
    {
        DataStreamItem _0 = new DataStreamItem();
        Thread.Sleep(100);
        DataStreamItem _1 = new DataStreamItem();

        Check.That(new DataStreamItem().CompareTo(null)).IsEqualTo(1);
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
        MemoryStream _2 = new MemoryStream();
        String _5 = "DataStreamItemExam";
        DataStreamItem _4 = new DataStreamItem(content:_2,id:Guid.NewGuid(),name:_5);

        Check.That(_4).IsInstanceOfType(typeof(DataStreamItem));
        Check.That(_4.GetContent()).IsSameReferenceAs(_2);
    }

    [Test]
    public void ExplicitOperator()
    {
        MemoryStream _0 = new();

        DataStreamItem? _1 = (DataStreamItem?)_0;
        DataStreamItem? _2 = (DataStreamItem?)null;

        Check.That(_1).IsNotNull();
        Check.That(_1!.GetContent()).IsSameReferenceAs(_0);
        Check.That(_2).IsNull();
    }

    [Test]
    public void opEquality()
    {
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        DataStreamItem _0 = new DataStreamItem(id:_11);
        DataStreamItem _3 = new DataStreamItem(content:new MemoryStream([1]),id:_11);
        DataStreamItem _15 = new DataStreamItem(content:new MemoryStream([2]),id:_11);
        DataStreamItem _4 = new DataStreamItem(content:new MemoryStream([1]),id:_14);
        DataStreamItem _5 = new DataStreamItem(content:new MemoryStream([1]));
        DataStreamItem _9 = new DataStreamItem(content:new MemoryStream([9]),id:_11);
        DataStreamItem _10 = new DataStreamItem();
        DataStreamItem _13 = new DataStreamItem(content:null,id:_11);

        Check.That(new DataStreamItem() == null).IsFalse();
        Check.That(_0 == _3).IsTrue();
        Check.That(_3 == _4).IsFalse();
        Check.That(_3 == _5).IsFalse();
        Check.That(_3 == _9).IsTrue();
        Check.That(_3 == _15).IsTrue();
        Check.That(_3 == _10).IsFalse();
        Check.That(_5 == _10).IsFalse();
        Check.That(_13 == _10).IsFalse();
    }

    [Test]
    public void opInequality()
    {
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        DataStreamItem _0 = new DataStreamItem(id:_11);
        DataStreamItem _3 = new DataStreamItem(content:new MemoryStream([1]),id:_11);
        DataStreamItem _15 = new DataStreamItem(content:new MemoryStream([2]),id:_11);
        DataStreamItem _4 = new DataStreamItem(content:new MemoryStream([1]),id:_14);
        DataStreamItem _5 = new DataStreamItem(content:new MemoryStream([1]));
        DataStreamItem _9 = new DataStreamItem(content:new MemoryStream([9]),id:_11);
        DataStreamItem _10 = new DataStreamItem();
        DataStreamItem _13 = new DataStreamItem(content:null,id:_11);

        Check.That(new DataStreamItem() != null).IsTrue();
        Check.That(_0 != _3).IsFalse();
        Check.That(_3 != _4).IsTrue();
        Check.That(_3 != _5).IsTrue();
        Check.That(_3 != _9).IsFalse();
        Check.That(_3 != _15).IsFalse();
        Check.That(_3 != _10).IsTrue();
        Check.That(_5 != _10).IsTrue();
        Check.That(_13 != _10).IsTrue();
    }

    [Test]
    public void EqualsObject()
    {
        Guid _0i = Guid.NewGuid();
        DataStreamItem _0 = new DataStreamItem(id:_0i);
        DataStreamItem _1 = new DataStreamItem(id:_0i);

        Check.That(((Object)_0).Equals(_0)).IsTrue();
        Check.That(((Object)_0).Equals(_1)).IsTrue();
        Check.That(((Object)new DataStreamItem()).Equals(null)).IsFalse();
        Check.That(((Object)new DataStreamItem()).Equals(new Object())).IsFalse();
    }

    [Test]
    public void EqualsInterface()
    {
        Guid _11 = new Guid("00021401-0000-0000-C000-000000000046");
        Guid _14 = new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511");
        DataStreamItem _0 = new DataStreamItem(id:_11);
        DataStreamItem _3 = new DataStreamItem(content:new MemoryStream([1]),id:_11);
        DataStreamItem _15 = new DataStreamItem(content:new MemoryStream([2]),id:_11);
        DataStreamItem _4 = new DataStreamItem(content:new MemoryStream([1]),id:_14);
        DataStreamItem _5 = new DataStreamItem(content:new MemoryStream([1]));
        DataStreamItem _9 = new DataStreamItem(content:new MemoryStream([9]),id:_11);
        DataStreamItem _10 = new DataStreamItem();
        DataStreamItem _13 = new DataStreamItem(content:null,id:_11);

        Check.That(new DataStreamItem().Equals(null)).IsFalse();
        Check.That(new DataStreamItem().Equals(new Object())).IsFalse();
        Check.That(_0.Equals(_3)).IsTrue();
        Check.That(_3.Equals(_4)).IsFalse();
        Check.That(_3.Equals(_5)).IsFalse();
        Check.That(_3.Equals(_9)).IsTrue();
        Check.That(_3.Equals(_15)).IsTrue();
        Check.That(_3.Equals(_10)).IsFalse();
        Check.That(_5.Equals(_10)).IsFalse();
        Check.That(_13.Equals(_10)).IsFalse();
    }

    [Test]
    public void FromFile()
    {
        DataStreamItem _2 = new DataStreamItem(content:null,id:Guid.NewGuid(),name:String.Empty);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\DataStreamItem03.bin";

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();

        Check.That(_2.ToFile(_3)).IsTrue();

        DataStreamItem? _4 = DataStreamItem.FromFile(_3);

        Check.That(_4).IsNotNull();

        Check.That(_2.Equals(_4)).IsTrue();

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();
    }

    [Test]
    public void GetApplication()
    {
        DataStreamItem _0 = new DataStreamItem();
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
        DataStreamItem _0 = new DataStreamItem();

        Check.That(_0.GetApplicationVersion()).IsNull();
    }

    [Test]
    public void GetBornOn()
    {
        DataStreamItem _0 = new DataStreamItem();

        Check.That(_0.GetBornOn()).HasAValue();
    }

    [Test]
    public async Task GetContent()
    {
        RandomNumberGenerator _0 = RandomNumberGenerator.Create();
        Byte[] _1 = new Byte[8192]; _0.GetBytes(_1);
        MemoryStream _2 = new MemoryStream(_1);
        DataStreamItem _3 = new DataStreamItem(_2);

        Check.That(_3.GetContent()).IsSameReferenceAs(_2);
        Check.That(_3.RegisterManager(KeyM)).IsTrue();
        Check.That(_3.Lock(KeyM)).IsTrue();
        Check.That(_3.SetContent(new MemoryStream(_1))).IsFalse();
        Check.That(_3.GetContent()).IsSameReferenceAs(_2);
    }

    [Test]
    public void GetContentStream()
    {
        Byte[] _0 = [1,2,3,4,5,6,7,8];
        String _1 = Path.GetTempFileName();
        DataStreamItem _2 = new();

        try
        {
            File.WriteAllBytes(_1,_0);

            Check.That(_2.GetContent()).IsNull();
            Check.That(_2.SetFILE(_1)).IsTrue();

            using Stream? _3 = _2.GetContentStream();

            Check.That(_3).IsNotNull();
            Check.That(_3!.CanRead).IsTrue();
            Check.That(_3.Length).IsEqualTo(_0.Length);
        }
        finally
        {
            if(File.Exists(_1)) { File.Delete(_1); }
        }
    }

    [Test]
    public void GetDataType()
    {
        DataStreamItem _0 = new DataStreamItem();

        Check.That(_0.SetDataType(DataTypes.MP4)).IsTrue();
        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetDataType()).Equals(DataTypes.MP4);
    }

    [Test]
    public void GetDescriptor()
    {
        DataStreamItem _0 = new DataStreamItem();

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
        Check.That(_0.SetDataType(DataTypes.MPG)).IsTrue();
        Check.That(_0.SetVersion(_v)).IsTrue();

        Descriptor? _ = _0.GetDescriptor();

        Check.That(_!.Application).IsEqualTo(_s);
        Check.That(_.ApplicationVersion).IsEqualTo(_v.ToString());
        Check.That(_.BornOn).Equals(_0.GetBornOn()?.ToString("O"));
        Check.That(_.ContentStreamed).IsEqualTo(false);
        Check.That(_.DataType).Equals(DataTypes.MPG);
        Check.That(_.DistinguishedName).IsEqualTo(_s);
        Check.That(_.FILE).IsNull();
        Check.That(_.ID).Equals(_0.GetID());
        Check.That(_.Modified).IsNotEqualTo(_d?.ToString("O"));
        Check.That(_.Name).IsEqualTo(_s);
        Check.That(_.Notes).Contains(_n);
        Check.That(_.ObjectType!.Split(" -> ")).Contains("KusDepot.DataStreamItem");
        Check.That(_.ServiceVersion).IsEqualTo(_v.ToString());
        Check.That(_.Tags).Contains(_t);
        Check.That(_.Version).IsEqualTo(_v.ToString());
        Check.That(_.Year).IsNull();

        Check.That(_0.SetID(Guid.Empty)).IsTrue();
        Check.That(_0.GetID()).IsNull();
        Check.That(_0.GetDescriptor()).IsNull();
    }

    [Test]
    public void GetDistinguishedName()
    {
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "CN=ServiceN,OU=Engineering,DC=TailSpinToys,DC=COM";

        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void GetDomainID()
    {
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "GetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
    }

    [Test]
    public void GetExtension()
    {
        DataStreamItem _0 = new DataStreamItem();
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
    public void GetHash()
    {
        DataStreamItem _0 = new DataStreamItem(content:null,id:new Guid("00021401-0000-0000-C000-000000000046"));
        DataStreamItem _1 = new DataStreamItem(content:null,id:new Guid("00021401-0000-0000-C000-000000000046"));
        DataStreamItem _2 = new DataStreamItem(content:new MemoryStream([0xA,0xB,0xE]),id:new Guid("FE3C02B0-B9FF-4A45-AE19-E9A8D4893511"));
        DataStreamItem _4 = new DataStreamItem(content:new MemoryStream([0xA,0xB,0xE]),id:new Guid("00021401-0000-0000-C000-000000000046"));
        DataStreamItem _3 = new(new MemoryStream([0x1]));

        Check.That(_0.GetHashCode()).IsNotZero();
        Check.That(_0.GetHashCode()).IsEqualTo(_1.GetHashCode());
        Check.That(_1.GetHashCode()).IsNotEqualTo(_2.GetHashCode());
        Check.That(_2.GetHashCode()).IsNotEqualTo(_3.GetHashCode());
        Check.That(_1.GetHashCode()).IsEqualTo(_4.GetHashCode());
    }

    [Test]
    public void GetID()
    {
        DataStreamItem _0 = new DataStreamItem();

        Check.That(_0.GetID()).HasAValue();
    }

    [Test]
    public void GetLinks()
    {
        DataStreamItem _0 = new DataStreamItem();
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
    public void GetLocked()
    {
        DataStreamItem _0 = new DataStreamItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);
    }

    [Test]
    public void GetModified()
    {
        DataStreamItem _0 = new();

        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_0.GetModified()).IsNotNull();
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
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "GetNameExam";

        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void GetNotes()
    {
        DataStreamItem _0 = new DataStreamItem();
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
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "GetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void GetServiceVersion() 
    {
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void GetTags()
    {
        DataStreamItem _0 = new DataStreamItem();
        List<String> _1 = new List<String>(){"GetTagsExam", "Pass"};

        Check.That(_0.AddTags(_1.ToHashSet())).IsTrue();
        Check.That(_0.GetTags()).Contains(_1);
    }

    [Test]
    public void GetVersion()
    {
        DataStreamItem _0 = new DataStreamItem();

        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Initialize()
    {
        DataStreamItem _1 = new DataStreamItem();

        Check.That(_1.Initialize()).IsTrue();

        Check.That(_1.GetID()).HasAValue();
    }

    [Test]
    public void IsMetadataOnly()
    {
        DataStreamItem _0 = new DataStreamItem();

        Check.That(_0.IsMetadataOnly()).IsTrue();
        Check.That(_0.SetContent(new MemoryStream([1]))).IsTrue();
        Check.That(_0.IsMetadataOnly()).IsFalse();
    }

    [Test]
    public async Task SignData_And_VerifyData_MetaData_ValidSignature()
    {
        DataStreamItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetName("DataStreamItemMetaDataSignExam"),Is.True);
        Assert.That(await item.SignData("MetaData",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(await item.VerifyData("MetaData",context).ConfigureAwait(false),Is.True);
    }

    [Test]
    public async Task SignData_And_VerifyData_MetaData_TamperedMetadata()
    {
        DataStreamItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(item.SetName("DataStreamItemMetaDataSignExam"),Is.True);
        Assert.That(await item.SignData("MetaData",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
        Assert.That(item.SetName("DataStreamItemMetaDataSignExam-Tampered"),Is.True);
        Assert.That(await item.VerifyData("MetaData",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_Content_NotSupported()
    {
        DataStreamItem item = new(new MemoryStream([1,2,3]));
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Null);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public async Task SignData_And_VerifyData_Content_FileBacked_ValidSignature()
    {
        String file = Path.GetTempFileName();

        try
        {
            File.WriteAllBytes(file,[1,2,3,4,5]);

            DataStreamItem item = new();
            DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

            Assert.That(item.SetFILE(file),Is.True);
            Assert.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false),Is.True);
            Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);
            Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
            Assert.That(await item.CheckDataHash().ConfigureAwait(false),Is.True);
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task SignData_And_VerifyData_Content_FileBacked_TamperedFile()
    {
        String file = Path.GetTempFileName();

        try
        {
            File.WriteAllBytes(file,[6,7,8,9,10]);

            DataStreamItem item = new();
            DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

            Assert.That(item.SetFILE(file),Is.True);
            Assert.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false),Is.True);
            Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);

            DataItemSecurityTestHelpers.TamperFile(file);

            Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
            Assert.That(await item.CheckDataHash().ConfigureAwait(false),Is.False);
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task EncryptData_And_DecryptData_FileBacked_Roundtrip()
    {
        String file = Path.GetTempFileName();

        try
        {
            Byte[] original = [11,12,13,14,15,16];
            File.WriteAllBytes(file,original);

            DataStreamItem item = new();
            DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);

            Assert.That(item.SetFILE(file),Is.True);
            Assert.That(await item.SetContentStreamed(true,(DataItemSecurityContext?)null).ConfigureAwait(false),Is.True);
            Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Not.Null.And.Not.Empty);

            Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.True);
            Assert.That(item.GetDataEncrypted(),Is.True);
            Assert.That(await item.VerifyData("EncryptedData",context).ConfigureAwait(false),Is.True);
            Assert.That(await item.CheckDataHash().ConfigureAwait(false),Is.True);

            Assert.That(await item.DecryptData(context).ConfigureAwait(false),Is.True);
            Assert.That(item.GetDataEncrypted(),Is.False);
            Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.True);
            Assert.That(await item.CheckDataHash().ConfigureAwait(false),Is.True);
            Assert.That(File.ReadAllBytes(file),Is.EqualTo(original));
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task EncryptData_FileBacked_LiveContentManaged_NotSupported()
    {
        DataStreamItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        using MemoryStream stream = new([1,2,3]);

        Assert.That(item.BindLiveContent(stream),Is.True);
        Assert.That(await item.SignData("Content",context).ConfigureAwait(false),Is.Null);
        Assert.That(await item.VerifyData("Content",context).ConfigureAwait(false),Is.False);
        Assert.That(await item.EncryptData(context).ConfigureAwait(false),Is.False);
        Assert.That(await item.DecryptData(context).ConfigureAwait(false),Is.False);
    }

    [Test]
    public void Lock()
    {
        DataStreamItem _0 = new DataStreamItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.SetSecurityDescriptor(null)).IsEqualTo(false);
    }

    [Test]
    public void ParameterlessConstructor()
    {
        DataStreamItem _0 = new DataStreamItem();

        Check.That(_0.GetApplication()).IsNull();
        Check.That(_0.GetApplicationVersion()).IsNull();
        Check.That(_0.GetBornOn()).IsNotNull();
        Check.That(_0.GetContent()).IsNull();
        Check.That(_0.GetDataType()).IsNull();
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
        Check.That(_0.GetVersion()).IsNull();
    }

    [Test]
    public void Parse()
    {
        DataStreamItem _0 = new DataStreamItem(content:new MemoryStream("ParseExam"u8.ToArray()),id:Guid.NewGuid());

        DataStreamItem? _1 = DataStreamItem.Parse(_0.ToString(),null);

        Check.That(_1).IsInstanceOfType(typeof(DataStreamItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void RemoveNote()
    {
        DataStreamItem _0 = new DataStreamItem();
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
        DataStreamItem _0 = new DataStreamItem();
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
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "SetApplicationExam";

        Check.That(_0.SetApplication(null)).IsFalse();
        Check.That(_0.SetApplication(_1)).IsTrue();
        Check.That(_0.GetApplication()).IsEqualTo(_1);
    }

    [Test]
    public void SetApplicationVersion()
    {
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetApplicationVersion(_2)).IsTrue();
        Check.That(_0.GetApplicationVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetBornOn()
    {
        DataStreamItem _0 = new DataStreamItem();
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
        DataStreamItem _3 = new DataStreamItem();
        MemoryStream _2 = new MemoryStream(_1);

        Check.That(_3.SetContent(_2)).IsTrue();
        Check.That(_3.GetContent()).IsSameReferenceAs(_2);
        Check.That(_3.RegisterManager(KeyM)).IsTrue();
        Check.That(_3.Lock(KeyM)).IsTrue();
        Check.That(_3.SetContent(new MemoryStream(_1))).IsFalse();
    }

    [Test]
    public async Task SetContentStreamed()
    {
        Byte[] _0b = [1,2,3,4,5,6,7,8];
        String _1 = Path.GetTempFileName();

        try
        {
            File.WriteAllBytes(_1,_0b);

            DataStreamItem _0 = new();
            DataStreamItem _2 = new();
            DataStreamItem _3 = new();

            Check.That(_0.GetContentStreamed()).IsFalse();
            Check.That(await _0.SetContentStreamed(true,security:null)).IsFalse();
            Check.That(_0.GetContentStreamed()).IsFalse();

            Check.That(_2.SetContent(new MemoryStream(_0b))).IsTrue();
            Check.That(_2.GetContentStreamed()).IsTrue();
            Check.That(await _2.SetContentStreamed(true,security:null)).IsTrue();
            Check.That(_2.GetContentStreamed()).IsTrue();
            Check.That(await _2.SetContentStreamed(false,security:null)).IsFalse();
            Check.That(_2.GetContentStreamed()).IsTrue();

            Check.That(_3.GetContent()).IsNull();
            Check.That(_3.SetFILE(_1)).IsTrue();
            Check.That(await _3.SetContentStreamed(true,security:null)).IsTrue();
            Check.That(_3.GetContentStreamed()).IsTrue();
            Check.That(await _3.SetContentStreamed(false,security:null)).IsTrue();
            Check.That(_3.GetContentStreamed()).IsFalse();
        }
        finally
        {
            if(File.Exists(_1)) { File.Delete(_1); }
        }
    }

    [Test]
    public void SetDataType()
    {
        DataStreamItem _0 = new DataStreamItem();

        Check.That(_0.SetDataType(DataTypes.MP4)).IsTrue();
        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.GetDataType()).Equals(DataTypes.MP4);
        Check.That(_0.SetDataType(DataTypes.MP3)).IsFalse();
    }

    [Test]
    public void SetDistinguishedName()
    {
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "CN=ServiceN,OU=Operations,DC=Fabrikam,DC=NET";

        Check.That(_0.SetDistinguishedName(null)).IsFalse();
        Check.That(_0.SetDistinguishedName(_1)).IsTrue();
        Check.That(_0.GetDistinguishedName()).IsEqualTo(_1);
    }

    [Test]
    public void SetDomainID()
    {
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "SetDomainIDExam";

        Check.That(_0.SetDomainID(_1)).IsTrue();
        Check.That(_0.GetDomainID()).IsEqualTo(_1);
        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();
        Check.That(_0.SetDomainID(_1)).IsFalse();
    }

    [Test]
    public void SetExtension()
    {
        DataStreamItem _0 = new DataStreamItem();
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
        DataStreamItem _0 = new DataStreamItem();
        Guid _1 = Guid.NewGuid();

        Check.That(_0.SetID(_1)).IsTrue();
        Check.That(_0.GetID()).Equals(_1);
    }

    [Test]
    public void SetLinks()
    {
        DataStreamItem _0 = new DataStreamItem();
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
        DataStreamItem _0 = new();

        DateTimeOffset _1 = DateTimeOffset.Now;

        Check.That(_0.GetModified()).IsNotNull();
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
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "SetNameExam";

        Check.That(_0.SetName(null)).IsFalse();
        Check.That(_0.SetName(_1)).IsTrue();
        Check.That(_0.GetName()).IsEqualTo(_1);
    }

    [Test]
    public void SetSecurityDescriptor()
    {
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "SetSecurityDescriptorExam";

        Check.That(_0.SetSecurityDescriptor(null)).IsFalse();
        Check.That(_0.SetSecurityDescriptor(_1)).IsTrue();
        Check.That(_0.GetSecurityDescriptor()).IsEqualTo(_1);
    }

    [Test]
    public void Serialize()
    {
        Byte[] _1 = new Byte[8192]; RandomNumberGenerator.Create().GetBytes(_1);

        var _0 = new DataStreamItem(content:new MemoryStream(_1),id:null,name:String.Empty);

        Byte[] _2 = _0.Serialize();

        DataStreamItem? _3 = DataItem.Deserialize<DataStreamItem>(_2);

        Check.That(_2.Length).IsStrictlyGreaterThan(0);
        Check.That(_3).IsNotNull();
        Check.That(((DataStreamItem)_0).Equals(_3)).IsTrue();
    }

    [Test]
    public void SetServiceVersion()
    {
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetServiceVersion(_2)).IsTrue();
        Check.That(_0.GetServiceVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void SetVersion()
    {
        DataStreamItem _0 = new DataStreamItem();
        String _1 = "0.1.0.1";
        Version _2 = new Version(_1);

        Check.That(_0.SetVersion(_2)).IsTrue();
        Check.That(_0.GetVersion()!.ToString()).IsEqualTo(_1);
    }

    [Test]
    public void ToFile()
    {
        DataStreamItem _2 = new DataStreamItem(content:null,id:Guid.NewGuid(),name:String.Empty);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\DataStreamItem0001.bin";

        File.Delete(_3);

        Check.That(File.Exists(_3)).IsFalse();

        Check.That(_2.ToFile(_3)).IsTrue();

        DataStreamItem? _4 = DataStreamItem.FromFile(_3);

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

        DataStreamItem _2 = new DataStreamItem(new MemoryStream(_1));

        DataStreamItem? _3 = DataStreamItem.Parse(_2.ToString(),null);

        Check.That(_2.Equals(_3)).IsTrue();
    }

    [Test]
    public void TryParse()
    {
        DataStreamItem _0 = new DataStreamItem(content:new MemoryStream("TryParseExam"u8.ToArray()),id:Guid.NewGuid());

        DataStreamItem? _1;

        DataStreamItem.TryParse(_0.ToString(),null,out _1);

        Check.That(_1).IsInstanceOfType(typeof(DataStreamItem));

        Check.That(_0.Equals(_1)).IsTrue();
    }

    [Test]
    public void UnLock()
    {
        DataStreamItem _0 = new DataStreamItem();

        Check.That(_0.GetLocked()).IsEqualTo(false);

        Check.That(_0.RegisterManager(KeyM)).IsTrue();
        Check.That(_0.Lock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(true);

        Check.That(_0.UnLock(KeyM)).IsTrue();

        Check.That(_0.GetLocked()).IsEqualTo(false);
    }

    [Test]
    public async Task GetHashCodeAsync()
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        Byte[] data = new Byte[8192]; rng.GetBytes(data);
        DataStreamItem item = new DataStreamItem(new MemoryStream(data),Guid.NewGuid());

        Int32 syncHash = item.GetHashCode();
        Int32 asyncHash = await item.GetHashCodeAsync();

        Check.That(asyncHash).IsEqualTo(syncHash);

        rng.GetBytes(data);
        Check.That(item.SetContent(new MemoryStream(data))).IsTrue();

        syncHash = item.GetHashCode();
        asyncHash = await item.GetHashCodeAsync();

        Check.That(asyncHash).IsEqualTo(syncHash);
    }
}