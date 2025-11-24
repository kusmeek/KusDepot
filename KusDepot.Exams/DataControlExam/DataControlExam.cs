namespace KusDepot.FabricExams.Data;

[TestFixture]
public class DataControlExam
{
    public String? ReadUserName;
    public String? ReadUserPass;
    public String? ReadToken;
    public String? WriteUserName;
    public String? WriteUserPass;
    public String? WriteToken;

    public String? ClientID;
    public String? TenantID;
    public String? Authority;
    public String[]? Scope;

    public Uri? URL;
    public KeySet? Keys;
    public DataItem? Item;
    public String? FilePath;
    public String? EndpointLocator;
    public BinaryItem? BinaryStream;
    public X509Certificate2? Certificate;
    public X509Certificate2? CatalogCertificate;

    [OneTimeSetUp]
    public async Task Calibrate()
    {
        Byte[] a = new Byte[13631480]; RandomNumberGenerator.Create().GetBytes(a); BinaryItem b = new();

        this.FilePath = Path.Combine(Path.GetTempPath(),"test",b.GetID().ToString()!);

        b.SetFILE(this.FilePath); await File.WriteAllBytesAsync(this.FilePath,a);

        Check.That(b.SetContentStreamed(true)).IsTrue(); ManagementKey? k = b.CreateManagementKey("Test"); Check.That(await b.EncryptData(k)).IsTrue(); this.BinaryStream = b;

        this.Item = new DataSetItem(content:new[]{new BinaryItem(DataGenerator.GenerateUnicodeString(100000).ToByteArrayFromUTF16String())});

        this.Keys = new KeySet([new ClientKey(Guid.NewGuid().ToByteArray()),new OwnerKey(Guid.NewGuid().ToByteArray())]);

        Check.That(this.LoadSetup()).IsTrue(); this.EndpointLocator = EndpointLocators.DataControlService;

        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();

        this.ReadToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync()).AccessToken;

        this.WriteToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync()).AccessToken;

        using X509Store s = new X509Store(StoreName.My,StoreLocation.CurrentUser); s.Open(OpenFlags.ReadOnly);
        this.Certificate = s.Certificates.Find(X509FindType.FindBySubjectName,"DataControlUser",true).First();
        this.CatalogCertificate = s.Certificates.Find(X509FindType.FindBySubjectName,"CatalogUser",true).First();
    }

    [OneTimeTearDown]
    public void CleanUp() { if(File.Exists(this.FilePath)) { File.Delete(this.FilePath); } }

    [Test] [Order(4)]
    public async Task Delete()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!); RestResponse<Guid?> _r; Guid? id = this.Item!.GetID();

        _r = await _dc.Delete(id);                 Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        _r = await _dc.Delete(id,this.ReadToken);  Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        _r = await _dc.Delete(id,this.WriteToken); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(Guid.Equals(this.Item!.GetID(),_r.Data)).IsTrue();

        _r = await _dc.Delete(this.Keys!.GetID(),this.WriteToken); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(Guid.Equals(this.Keys!.GetID(),_r.Data)).IsTrue();
    }

    [Test] [Order(10)]
    public async Task DeleteStream()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!); RestResponse<Guid?> _r; Guid? id = this.BinaryStream!.GetID();

        _r = await _dc.Delete(id);                 Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        _r = await _dc.Delete(id,this.ReadToken);  Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        _r = await _dc.Delete(id,this.WriteToken); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(Guid.Equals(this.BinaryStream!.GetID(),_r.Data)).IsTrue();
    }

    [Test] [Order(3)]
    public async Task Get()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!); RestResponse<DataControlDownload?> _r; Guid? id = this.Item!.GetID(); Guid? id2 = Guid.NewGuid();

        _r = await _dc.Get(id);                 Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        _r = await _dc.Get(id2,this.ReadToken); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status404NotFound); Check.That(_r.Content).Equals(JsonSerializer.Serialize(id2));

        _r = await _dc.Get(id,this.ReadToken);  Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlDownload? _d = _r.Data;      Check.That(_d.Verify()).IsTrue(); DataItem? _i = DataItem.Parse(_d!.Object,null); Check.That(this.Item.Equals(_i)).IsTrue();

        _r = await _dc.Get(this.Keys!.GetID(),this.ReadToken);  Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlDownload? _d2 = _r.Data;      Check.That(_d2.Verify()).IsTrue(); DataItem? _k = DataItem.Parse(_d2!.Object,null); Check.That(this.Keys.Equals(_k)).IsTrue();
    }

    [Test] [Order(9)]
    public async Task GetStream()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        Guid? id = this.BinaryStream!.GetID();

        String fp = Path.Combine(Path.GetTempPath(),this.BinaryStream?.GetID().ToString()!);

        String ip = Path.Combine(Path.GetTempPath(),this.BinaryStream?.GetID().ToString()!+".kdit");

        var di = await _dc.GetStream(id,fp,ip,this.ReadToken); Check.That(di).IsNotNull();

        Check.That(String.Equals(di!.FilePath,fp,StringComparison.Ordinal)).IsTrue();

        DataItem? bi = DataItem.Parse(await File.ReadAllTextAsync(ip),null); Check.That(bi).IsInstanceOf<BinaryItem>();

        Stream fs = File.OpenRead(fp); Stream? bs = this.BinaryStream?.GetContentStream();

        Byte[] fsh = await SHA512.HashDataAsync(fs); Byte[] bsh = await SHA512.HashDataAsync(bs!); Byte[]? bih = SHA512.HashData(bi!.ToString().ToByteArrayFromBase64());

        Check.That(fsh.SequenceEqual(bsh)).IsTrue(); Check.That(bih).IsNotNull(); Check.That(di.DataControlDownload.ObjectSHA512.ToByteArrayFromBase64()!.SequenceEqual(bih!)).IsTrue();

        await fs.DisposeAsync(); await bs!.DisposeAsync(); File.Delete(fp); File.Delete(ip); File.Delete(this.BinaryStream?.GetFILE()!);
    }

    [Test] [Order(1)]
    public async Task Health()
    {
        RestClientOptions o = new(EndpointLocators.DataControlService){ ClientCertificates = new(){this.Certificate!}};

        Check.That((await new RestClient(o).ExecuteAsync(new("/DataControl/Health",Method.Get))).Content).Equals("Healthy");
    }

    [Test] [Order(2)]
    public async Task Store()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        RestResponse<Guid?> _r; DataControlUpload? _dcu = this.Item!.MakeDataControlUpload();

        _r = await _dc.Store(_dcu!);                                   Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        _r = await _dc.Store(_dcu!,this.ReadToken);                    Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        _r = await _dc.Store(new DataControlUpload(),this.WriteToken); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status422UnprocessableEntity);

        _r = await _dc.Store(_dcu!,this.WriteToken);                   Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_r.Data).Equals(_dcu!.Descriptor.ID);

        var _ksu = this.Keys!.MakeDataControlUpload();

        _r = await _dc.Store(_ksu!,this.WriteToken);                   Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_r.Data).Equals(_ksu!.Descriptor.ID);
    }

    [Test] [Order(8)]
    public async Task StoreStream()
    {
        if(this.BinaryStream is null) { return; } using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!); HttpResponseMessage? _h;

        _h = await _dc.StoreStream(this.BinaryStream,this.WriteToken)!; Check.That((Int32)_h!.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(await _h.Content.ReadAsStringAsync()).Equals(JsonSerializer.Serialize(this.BinaryStream.GetID()));
    }

    [Test] [Order(5)]
    public async Task QueryCommands()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!,this.WriteToken); RestResponse<Guid?>? _dcr;

        BinaryItem? _b = BinaryItem.FromFile(Path.Join(AppContext.BaseDirectory,"ContainerAssembly.bin")); _b?.SetID(Guid.NewGuid());

        using CatalogClient _cc = new(EndpointLocators.CatalogService,this.CatalogCertificate!,this.ReadToken); RestResponse<CommandResponse?> _cr; CommandResponse? _c;

        _dcr = await _dc.Store(_b.MakeDataControlUpload()!);

        Check.That((Int32)_dcr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(Guid.Equals(_b!.GetID(),_dcr.Data)).IsTrue();

        _cr = await _cc.SearchCommands(new CommandQuery()); _c = _cr.Data;

        Check.That((Int32)_cr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(4);

        _cr = await _cc.SearchCommands(new CommandQuery() { CommandSpecifications = "sheet" }); _c = _cr.Data;

        Check.That((Int32)_cr.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(2);

        Check.That(_c!.Commands.All(_=>String.Equals(_.CommandHandle,"Handle001",StringComparison.Ordinal)));

        Check.That(_c!.Commands.Any(_=>String.Equals(_.CommandType,typeof(KusDepot.FabricExams.CommandExam0).FullName,StringComparison.Ordinal)));

        _cr = await _cc.SearchCommands(new CommandQuery() { CommandHandle = "002" }); _c = _cr.Data;

        Check.That((Int32)_cr.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(2);

        Check.That(_c!.Commands.All(_=>_.CommandSpecifications!.Contains("usage",StringComparison.Ordinal)));

        Check.That(_c!.Commands.Any(_=>String.Equals(_.CommandType,typeof(KusDepot.FabricExams.CommandExam1).FullName,StringComparison.Ordinal)));

        _dcr = await _dc.Delete(_b!.GetID());

        Check.That((Int32)_dcr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(Guid.Equals(_b!.GetID(),_dcr.Data)).IsTrue();
    }

    [Test] [Order(6)]
    public async Task QueryServices()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!,this.WriteToken); RestResponse<Guid?>? _dcr;

        BinaryItem? _b = BinaryItem.FromFile(Path.Join(AppContext.BaseDirectory,"ContainerAssembly.bin")); _b?.SetID(Guid.NewGuid());

        using CatalogClient _cc = new(EndpointLocators.CatalogService,this.CatalogCertificate!,this.ReadToken); RestResponse<ServiceResponse?> _sr; ServiceResponse? _s;

        _dcr = await _dc.Store(_b.MakeDataControlUpload()!);

        Check.That((Int32)_dcr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(Guid.Equals(_b!.GetID(),_dcr.Data)).IsTrue();

        _sr = await _cc.SearchServices(new ServiceQuery() { }); _s = _sr.Data;

        Check.That((Int32)_sr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_s).IsNotNull(); Check.That(_s!.Services.Length).IsEqualTo(2);

        _sr = await _cc.SearchServices(new ServiceQuery() { ServiceInterfaces = "IDeviceManager" }); _s = _sr.Data;

        Check.That((Int32)_sr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_s).IsNotNull(); Check.That(_s!.Services.Length).IsEqualTo(1);

        _sr = await _cc.SearchServices(new ServiceQuery() { ServiceType = "ServiceExam1" }); _s = _sr.Data;

        Check.That((Int32)_sr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_s).IsNotNull(); Check.That(_s!.Services.Length).IsEqualTo(1);

        _sr = await _cc.SearchServices(new ServiceQuery(),"null"); Check.That((Int32)_sr.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        _dcr = await _dc.Delete(_b!.GetID());

        Check.That((Int32)_dcr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(Guid.Equals(_b!.GetID(),_dcr.Data)).IsTrue();
    }

    [Test] [Order(7)]
    public async Task X509()
    {
        #pragma warning disable SYSLIB0026
        DataControlClient _dc = new(this.EndpointLocator!,new()); RestResponse? _r; 

        _r = await _dc.Store(new BinaryItem().MakeDataControlUpload()!); Check.That((Int32)_r.StatusCode).IsEqualTo(0);

        _dc = new(this.EndpointLocator!,this.CatalogCertificate!);

        _r = await _dc.Store(new BinaryItem().MakeDataControlUpload()!); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status403Forbidden);
    }

    private Boolean LoadSetup()
    {
        try
        {
            XmlDocument d = new XmlDocument(); d.Load(Environment.CurrentDirectory + @"\EntraSetup.xml"); XmlNode? n = d.SelectSingleNode("EntraSetup");

            this.Authority     = n!.SelectSingleNode("Authority")!.InnerText;
            this.TenantID      = n.SelectSingleNode("TenantID")!.InnerText;
            this.ClientID      = n.SelectSingleNode("ClientID")!.InnerText;
            this.Scope         = new List<String>(){n.SelectSingleNode("Scope")!.InnerText}.ToArray();
            this.ReadUserName  = n.SelectSingleNode("ReadUserName")!.InnerText;
            this.ReadUserPass  = n.SelectSingleNode("ReadUserPass")!.InnerText;
            this.WriteUserName = n.SelectSingleNode("WriteUserName")!.InnerText;
            this.WriteUserPass = n.SelectSingleNode("WriteUserPass")!.InnerText;

            return true;
        }
        catch (Exception) { return false; }
    }
}