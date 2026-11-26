using DataPodServices.CatalogDB;
namespace KusDepot.DataPodExams;


[TestFixture] [NonParallelizable]
public class CatalogExam
{
    private IToolGenericHost? Host;

    private String? AdminUserName;
    private String? AdminUserPass;
    private String? AdminToken;
    private String? ReadUserName;
    private String? ReadUserPass;
    private String? ReadToken;
    private String? WriteUserName;
    private String? WriteUserPass;
    private String? WriteToken;

    private String? ClientID;
    private String? TenantID;
    private String? Authority;
    private String[]? Scope;

    private DataPodServices.CatalogDB.ICatalogDB? CatalogDB;
    private String? EndpointLocator;
    private static String? ItemHash {get;set;}
    private IEnumerable<Descriptor>? Descriptors;

    [OneTimeSetUp]
    public async Task Calibrate()
    {
        await this.LoadSetup();

        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();

        this.AdminToken = _.AcquireTokenByUsernamePassword(this.Scope,this.AdminUserName,this.AdminUserPass).ExecuteAsync().Result.AccessToken;

        this.ReadToken = _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync().Result.AccessToken;

        this.WriteToken = _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync().Result.AccessToken;

        IToolGenericHostBuilder tb = ToolBuilderFactory.CreateGenericHostBuilder();

        tb.Builder.UseOrleansClient(OrleansExamClient.Configure);

        Host = await tb.BuildGenericHostAsync(); await Host.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token);

        this.EndpointLocator = "http://localhost:5006";

        IGrainFactory? gf = Host.Services.GetService<IGrainFactory>();

        this.CatalogDB = gf!.GetGrain<DataPodServices.CatalogDB.ICatalogDB>("PublicCatalog");

        this.Descriptors = CatalogExamData.Build(); foreach(var d in this.Descriptors) { Check.That(await this.CatalogDB!.AddUpdate(d,Guid.NewGuid(),null)).IsTrue(); }
    }

    [OneTimeTearDown]
    public async Task Complete()
    {
        foreach(var d in this.Descriptors!)
        {
            CatalogItemIdentity? identity = await this.CatalogDB!.BeginDelete(d.ID!.Value,null);
            if(identity is not null) { await this.CatalogDB.FinalizeDelete(d.ID!.Value,identity.ItemInstanceToken,null); }
        }

        await Host!.DisposeAsync();
    }

    [Test]
    public async Task Health()
    {
        using HttpClient client = new() { BaseAddress = new Uri(this.EndpointLocator!,UriKind.Absolute) };
        using HttpResponseMessage response = await client.GetAsync("/Catalog/Health");
        String content = await response.Content.ReadAsStringAsync();
        Check.That(response.StatusCode).Equals(HttpStatusCode.OK);
        Check.That(content).Equals(String.Empty);
    }

    [Test]
    public async Task SearchCommands()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.ReadToken); CatalogResponse<CommandResponse> _r; CommandResponse? _c;

        _r = await _cc.SearchCommands(new CommandQuery()); _c = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(4);

        _r = await _cc.SearchCommands(new CommandQuery() { CommandSpecifications = "sheet" }); _c = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(2);

        Check.That(_c!.Commands.All(_=>String.Equals(_.CommandHandle,"Handle001",StringComparison.Ordinal))).IsTrue();

        Check.That(_c!.Commands.Any(_=>String.Equals(_.CommandType,typeof(CommandExam0).FullName,StringComparison.Ordinal))).IsTrue();

        _r = await _cc.SearchCommands(new CommandQuery() { CommandHandle = "002" }); _c = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(2);

        Check.That(_c!.Commands.All(_=>_.CommandSpecifications!.Contains("Usage",StringComparison.Ordinal))).IsTrue();

        Check.That(_c!.Commands.Any(_=>String.Equals(_.CommandType,typeof(CommandExam1).FullName,StringComparison.Ordinal))).IsTrue();

        _r = await _cc.SearchCommands(new CommandQuery(),"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized); 
    }

    [Test]
    public async Task SearchElements()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.ReadToken); CatalogResponse<ElementResponse> _r; ElementResponse? _e;

        _r = await _cc.SearchElements(new ElementQuery()); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(20);

        _r = await _cc.SearchElements(new ElementQuery(){ Name = "deploy" , DataType = DataTypes.PS1 }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ ObjectType = nameof(GenericItem) }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ ObjectType = nameof(KeySet) }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ DataType = DataTypes.KEYSET }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ Version = "1.0.1" }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ Name = "fig2" , DataType = DataTypes.XML }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ DataType = DataTypes.DAT }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(0);

        _r = await _cc.SearchElements(new ElementQuery(){ ContentStreamed = true }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ ObjectFILE = "G18" }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery(){ Language = DataTags.Language.tha }); _e = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = await _cc.SearchElements(new ElementQuery { CatalogPageSize = 0 }); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = await _cc.SearchElements(new ElementQuery { CatalogPageToken = "invalid" }); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = await _cc.SearchElements(new ElementQuery(),"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);
    }

    [Test]
    public async Task SearchMedia()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.ReadToken); CatalogResponse<MediaResponse> _r; MediaResponse? _m;

        _r = await _cc.SearchMedia(new MediaQuery(){ Title = "cosmo" }); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(3);

        _r = await _cc.SearchMedia(new MediaQuery(){ Artist = "Tyson" }); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(2);

        _r = await _cc.SearchMedia(new MediaQuery(){ Artist = "Tyson" , Year = "020" }); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(1);

        _r = await _cc.SearchMedia(new MediaQuery(){ Name = "Naruto" }); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(0);

        _r = await _cc.SearchMedia(new MediaQuery(){ Language = DataTags.Language.eng }); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(1);

        Check.That(_m.Media[0].Language).IsEqualTo(DataTags.Language.eng);

        _r = await _cc.SearchMedia(new MediaQuery(){ Album = "group" }); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(1);

        _r = await _cc.SearchMedia(new MediaQuery()); _m = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(4);

        _r = await _cc.SearchMedia(new MediaQuery(),"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized); 
    }

    [Test]
    public async Task SearchNotes()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.ReadToken); CatalogResponse<NoteResponse> _r; NoteResponse? _n;

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new List<String>(){"te10"}.ToArray() }); _n = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_n).IsNotNull(); Check.That(_n!.IDs!.Length).IsEqualTo(1);

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new List<String>(){"otE20"}.ToArray() }); _n = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_n).IsNotNull(); Check.That(_n!.IDs!.Length).IsEqualTo(2);

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new List<String>(){"300"}.ToArray() }); _n = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_n).IsNotNull(); Check.That(_n!.IDs!.Length).IsEqualTo(3);

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new List<String>(){"300","&"}.ToArray() }); _n = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_n).IsNotNull(); Check.That(_n!.IDs!.Length).IsEqualTo(0);

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new List<String>(){}.ToArray() }); _n = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = await _cc.SearchNotes(new NoteQuery()); _n = _r.Data; Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = await _cc.SearchNotes(new NoteQuery(){ Notes = new String[]{"!"}},"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized); 
    }

    [Test]
    public async Task SearchServices()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.ReadToken); CatalogResponse<ServiceResponse> _r; ServiceResponse? _a;

        _r = await _cc.SearchServices(new ServiceQuery() { }); _a = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_a).IsNotNull(); Check.That(_a!.Services.Length).IsEqualTo(2);
        Check.That(_a.Services.All(_ => !String.IsNullOrWhiteSpace(_.ServiceSpecifications))).IsTrue();

        _r = await _cc.SearchServices(new ServiceQuery() { ServiceInterfaces = "IDeviceManager" }); _a = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_a).IsNotNull(); Check.That(_a!.Services.Length).IsEqualTo(1);
        Check.That(_a.Services.All(_ => _.ServiceSpecifications!.Contains("Specification",StringComparison.Ordinal))).IsTrue();

        _r = await _cc.SearchServices(new ServiceQuery() { ServiceSpecifications = "sage" }); _a = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_a).IsNotNull(); Check.That(_a!.Services.Length).IsEqualTo(1);
        Check.That(_a.Services.All(_ => _.ServiceSpecifications!.Contains("...",StringComparison.Ordinal))).IsTrue();
        Check.That(_a.Services.Any(_ => String.Equals(_.ServiceType,typeof(ServiceExam1).FullName,StringComparison.Ordinal))).IsTrue();

        _r = await _cc.SearchServices(new ServiceQuery() { ServiceType = "ServiceExam1" }); _a = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_a).IsNotNull(); Check.That(_a!.Services.Length).IsEqualTo(1);
        Check.That(_a.Services.All(_ => _.ServiceSpecifications!.Contains("Usage",StringComparison.Ordinal))).IsTrue();

        _r = await _cc.SearchServices(new ServiceQuery(),"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);
    }

    [Test]
    public async Task SearchTags()
    {
        using CatalogClient _cc = new(this.EndpointLocator!,this.ReadToken); CatalogResponse<TagResponse> _r; TagResponse? _t;

        _r = await _cc.SearchTags(new TagQuery(){ Tags = new List<String>(){"tag10"}.ToArray() }); _t = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_t).IsNotNull(); Check.That(_t!.IDs!.Length).IsEqualTo(1);

        _r = await _cc.SearchTags(new TagQuery(){ Tags = new List<String>(){"G20"}.ToArray() }); _t = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_t).IsNotNull(); Check.That(_t!.IDs!.Length).IsEqualTo(2);

        _r = await _cc.SearchTags(new TagQuery(){ Tags = new List<String>(){"300"}.ToArray() }); _t = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_t).IsNotNull(); Check.That(_t!.IDs!.Length).IsEqualTo(3);

        _r = await _cc.SearchTags(new TagQuery(){ Tags = new List<String>(){"300","!"}.ToArray() }); _t = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_t).IsNotNull(); Check.That(_t!.IDs!.Length).IsEqualTo(0);

        _r = await _cc.SearchTags(new TagQuery() { Tags = new List<String>(){}.ToArray() }); _t = _r.Data;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = await _cc.SearchTags(new TagQuery()); _t = _r.Data; Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = await _cc.SearchTags(new TagQuery(){ Tags = new String[]{"!"}},"null"); Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);
    }

    private async Task LoadSetup()
    {
        EntraSetupRecord setup = await EntraSetupProvider.LoadAsync().ConfigureAwait(false);

        this.Authority = setup.Authority;
        this.TenantID = setup.TenantID;
        this.ClientID = setup.ClientID;
        this.Scope = [setup.Scope];
        this.AdminUserName = setup.AdminUserName;
        this.AdminUserPass = setup.AdminUserPass;
        this.ReadUserName = setup.ReadUserName;
        this.ReadUserPass = setup.ReadUserPass;
        this.WriteUserName = setup.WriteUserName;
        this.WriteUserPass = setup.WriteUserPass;
    }
}