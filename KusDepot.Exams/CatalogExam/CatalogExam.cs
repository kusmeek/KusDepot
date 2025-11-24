namespace KusDepot.FabricExams.Data;

[TestFixture]
public class CatalogExam
{
    public String? AdminUserName;
    public String? AdminUserPass;
    public String? AdminToken;
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

    public ICatalogDB? CatalogDB;
    public String? EndpointLocator;
    public X509Certificate2? Certificate;
    public static String? ItemHash {get;set;}
    public X509Certificate2? DataControlCertificate;
    public IEnumerable<Descriptor>? Descriptors;

    [OneTimeSetUp]
    public async Task Calibrate()
    {
        Check.That(this.LoadSetup()).IsTrue();

        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();

        this.ReadToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync()).AccessToken;

        this.WriteToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync()).AccessToken;

        this.EndpointLocator = EndpointLocators.CatalogService;

        StorageSilo? s = await ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService).GetAuthorizedWriteSilo(this.WriteToken,null,null); Check.That(s).IsNotNull();

        this.CatalogDB = ActorProxy.Create<ICatalogDB>(new(s!.CatalogName),ServiceLocators.CatalogDBService);

        this.Descriptors = CatalogExamData.Build(); foreach(var d in this.Descriptors) { Check.That(await this.CatalogDB!.AddUpdate(d,null,null)).IsTrue(); }

        using X509Store x = new X509Store(StoreName.My,StoreLocation.CurrentUser); x.Open(OpenFlags.ReadOnly);

        this.Certificate = x.Certificates.Find(X509FindType.FindBySubjectName,"CatalogUser",true).First();

        this.DataControlCertificate = x.Certificates.Find(X509FindType.FindBySubjectName,"DataControlUser",true).First();
    }

    [OneTimeTearDown]
    public async Task Complete()
    {
        foreach(var d in this.Descriptors!) { Check.That( await this.CatalogDB!.RemoveID(d.ID,null,null) ).IsTrue(); }
    }

    [Test]
    public async Task Health()
    {
        RestClientOptions o = new(EndpointLocators.CatalogService){ ClientCertificates = new(){this.Certificate!}};

        Check.That((await new RestClient(o).ExecuteAsync(new("/Catalog/Health",Method.Get))).Content).Equals("Healthy");
    }

    [Test]
    public async Task SearchCommands()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.Certificate!,this.ReadToken); RestResponse<CommandResponse?> _r; CommandResponse? _c;

        _r = await _cc.SearchCommands(new CommandQuery()); _c = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(4);

        _r = await _cc.SearchCommands(new CommandQuery() { CommandSpecifications = "sheet" }); _c = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(2);

        Check.That(_c!.Commands.All(_=>String.Equals(_.CommandHandle,"Handle001",StringComparison.Ordinal))).IsTrue();

        Check.That(_c!.Commands.Any(_=>String.Equals(_.CommandType,typeof(KusDepot.FabricExams.CommandExam0).FullName,StringComparison.Ordinal))).IsTrue();

        _r = await _cc.SearchCommands(new CommandQuery() { CommandHandle = "002" }); _c = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(2);

        Check.That(_c!.Commands.All(_=>_.CommandSpecifications!.Contains("Usage",StringComparison.Ordinal))).IsTrue();

        Check.That(_c!.Commands.Any(_=>String.Equals(_.CommandType,typeof(KusDepot.FabricExams.CommandExam1).FullName,StringComparison.Ordinal))).IsTrue();

        _r = await _cc.SearchCommands(new CommandQuery(),"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized); 
    }

    [Test]
    public async Task SearchElements()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.Certificate!,this.ReadToken); RestResponse<ElementResponse?> _r; ElementResponse? _e;

        _r = await _cc.SearchElements(new ElementQuery()); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(20);

        _r = await _cc.SearchElements(new ElementQuery(){ Name = "deploy" , Type = DataType.PS1 }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ ObjectType = nameof(GenericItem) }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ ObjectType = nameof(KeySet) }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ Type = DataType.KEYSET }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ Version = "1.0.1" }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ Name = "fig2" , Type = DataType.XML }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ Type = DataType.DAT }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status404NotFound); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(0);

        _r = await _cc.SearchElements(new ElementQuery(){ ContentStreamed = true }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(),"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);
    }

    [Test]
    public async Task SearchMedia()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.Certificate!,this.ReadToken); RestResponse<MediaResponse?> _r; MediaResponse? _m;

        _r = await _cc.SearchMedia(new MediaQuery(){ Title = "cosmo" }); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(3);

        _r = await _cc.SearchMedia(new MediaQuery(){ Artist = "Tyson" }); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(2);

        _r = await _cc.SearchMedia(new MediaQuery(){ Artist = "Tyson" , Year = "020" }); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(1);

        _r = await _cc.SearchMedia(new MediaQuery(){ Name = "Naruto" }); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status404NotFound); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(0);

        _r = await _cc.SearchMedia(new MediaQuery()); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(4);

        _r = await _cc.SearchMedia(new MediaQuery(),"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized); 
    }

    [Test]
    public async Task SearchNotes()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.Certificate!,this.ReadToken); RestResponse<NoteResponse?> _r; NoteResponse? _n;

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new List<String>(){"te10"}.ToArray() }); _n = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_n).IsNotNull(); Check.That(_n!.IDs!.Count).IsEqualTo(1);

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new List<String>(){"otE20"}.ToArray() }); _n = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_n).IsNotNull(); Check.That(_n!.IDs!.Count).IsEqualTo(2);

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new List<String>(){"300"}.ToArray() }); _n = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_n).IsNotNull(); Check.That(_n!.IDs!.Count).IsEqualTo(3);

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new List<String>(){"300","&"}.ToArray() }); _n = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status404NotFound); Check.That(_n).IsNotNull(); Check.That(_n!.IDs!.Count).IsEqualTo(0);

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new List<String>(){}.ToArray() }); _n = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = await _cc.SearchNotes(new NoteQuery()); _n = _r.Data; Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new String[]{"!"}},"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized); 
    }

    [Test]
    public async Task SearchServices()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.Certificate!,this.ReadToken); RestResponse<ServiceResponse?> _r; ServiceResponse? _a;

        _r = await _cc.SearchServices(new ServiceQuery() { }); _a = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_a).IsNotNull(); Check.That(_a!.Services.Length).IsEqualTo(2);

        _r = await _cc.SearchServices(new ServiceQuery() { ServiceInterfaces = "IDeviceManager" }); _a = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_a).IsNotNull(); Check.That(_a!.Services.Length).IsEqualTo(1);

        _r = await _cc.SearchServices(new ServiceQuery() { ServiceType = "ServiceExam1" }); _a = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_a).IsNotNull(); Check.That(_a!.Services.Length).IsEqualTo(1);

        _r = await _cc.SearchServices(new ServiceQuery(),"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);
    }

    [Test]
    public async Task SearchTags()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.Certificate!,this.ReadToken); RestResponse<TagResponse?> _r; TagResponse? _t;

        _r = await _cc.SearchTags(new TagQuery(){ Tags = new List<String>(){"tag10"}.ToArray() }); _t = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_t).IsNotNull(); Check.That(_t!.IDs!.Count).IsEqualTo(1);

        _r = await _cc.SearchTags(new TagQuery(){ Tags = new List<String>(){"G20"}.ToArray() }); _t = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_t).IsNotNull(); Check.That(_t!.IDs!.Count).IsEqualTo(2);

        _r = await _cc.SearchTags(new TagQuery(){ Tags = new List<String>(){"300"}.ToArray() }); _t = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_t).IsNotNull(); Check.That(_t!.IDs!.Count).IsEqualTo(3);

        _r = await _cc.SearchTags(new TagQuery(){ Tags = new List<String>(){"300","!"}.ToArray() }); _t = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status404NotFound); Check.That(_t).IsNotNull(); Check.That(_t!.IDs!.Count).IsEqualTo(0);

        _r = await _cc.SearchTags(new TagQuery() { Tags = new List<String>(){}.ToArray() }); _t = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = await _cc.SearchTags(new TagQuery()); _t = _r.Data; Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = await _cc.SearchTags(new TagQuery(){ Tags = new String[]{"!"}},"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);
    }

    [Test]
    public async Task X509()
    {
        #pragma warning disable SYSLIB0026
        CatalogClient _cc = new(this.EndpointLocator!,new()); RestResponse? _r; 

        _r = await _cc.SearchElements(new()); Check.That((Int32)_r.StatusCode).IsEqualTo(0);

        _cc = new(this.EndpointLocator!,this.DataControlCertificate!);

        _r = await _cc.SearchElements(new()); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status403Forbidden);
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
            this.AdminUserName = n.SelectSingleNode("AdminUserName")!.InnerText;
            this.AdminUserPass = n.SelectSingleNode("AdminUserPass")!.InnerText;
            this.ReadUserName  = n.SelectSingleNode("ReadUserName")!.InnerText;
            this.ReadUserPass  = n.SelectSingleNode("ReadUserPass")!.InnerText;
            this.WriteUserName = n.SelectSingleNode("WriteUserName")!.InnerText;
            this.WriteUserPass = n.SelectSingleNode("WriteUserPass")!.InnerText;

            return true;
        }
        catch (Exception) { return false; }
    }
}