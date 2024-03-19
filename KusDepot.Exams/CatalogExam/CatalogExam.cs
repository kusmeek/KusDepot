namespace KusDepot.FabricExams;

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

    public HttpClient? Client;
    public IArkKeeper? Catalog;
    public String? EndpointLocator;

    [OneTimeSetUp]
    public void Calibrate()
    {
        Check.That(this.LoadSetup()).IsTrue();

        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();

        this.AdminToken = _.AcquireTokenByUsernamePassword(this.Scope,this.AdminUserName,this.AdminUserPass).ExecuteAsync().Result.AccessToken;

        this.ReadToken = _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync().Result.AccessToken;

        this.WriteToken = _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync().Result.AccessToken;

        this.Client = new HttpClient(); this.EndpointLocator = EndpointLocators.CatalogService;

        StorageSilo? s = ActorProxy.Create<IDataConfigs>(ActorIds.DataConfiguration,ServiceLocators.DataConfigsService).GetAuthorizedWriteSilo(this.WriteToken,null,null).Result; Check.That(s).IsNotNull();

        this.Catalog = ActorProxy.Create<IArkKeeper>(new ActorId(s!.CatalogName),ServiceLocators.ArkKeeperService);
        Check.That(this.Catalog!.StoreArk(Ark.GetBytes(CatalogExamArk.GenerateCatalogExamArk()),null,null).Result).IsTrue();
        Check.That(Ark.Parse(this.Catalog.GetArk(null,null).Result)).IsNotNull();

        this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",this.ReadToken);
    }

    [Test]
    public void Health()
    {
        Uri _u = new Uri(String.Concat(this.EndpointLocator,"/Catalog/Health"));

        Check.That(this.Client!.GetAsync(_u).Result.Content.ReadAsStringAsync().Result).Equals("Healthy");
    }

    [Test]
    public void SearchActiveServices()
    {
        Uri _u = new Uri(String.Concat(this.EndpointLocator,"/Catalog/ActiveServices")); HttpResponseMessage _r; ActiveServiceResponse? _a;

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ActiveServiceRequest() { Name = String.Concat(CatalogExamArk.ToolName.Skip(2).Take(6)) })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _a = JsonSerializer.Deserialize<ActiveServiceResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_a).IsNotNull(); Check.That(_a!.ActiveServices.Length).IsEqualTo(1); Check.That(_a!.ActiveServices[0].Application).IsEqualTo(CatalogExamArk.ToolApp); Check.That(_a!.ActiveServices[0].ID).IsEqualTo(CatalogExamArk.ToolID); Check.That(_a!.ActiveServices[0].Modified).IsEqualTo(CatalogExamArk.ToolMod.ToString("O")); Check.That(_a!.ActiveServices[0].Name).IsEqualTo(CatalogExamArk.ToolName); Check.That(_a!.ActiveServices[0].Version).IsEqualTo(CatalogExamArk.ToolVersion.ToString());

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ActiveServiceRequest() { Notes = new List<String>(){"01"}.ToArray() , Tags = new List<String>(){"04"}.ToArray() })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _a = JsonSerializer.Deserialize<ActiveServiceResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_a).IsNotNull(); Check.That(_a!.ActiveServices.Length).IsEqualTo(1);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ActiveServiceRequest() { ID = CatalogExamArk.ToolID })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _a = JsonSerializer.Deserialize<ActiveServiceResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_a).IsNotNull(); Check.That(_a!.ActiveServices.Length).IsEqualTo(1);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ActiveServiceRequest() { Notes = new List<String>(){"03"}.ToArray()})).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status404NotFound); _a = JsonSerializer.Deserialize<ActiveServiceResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_a).IsNotNull(); Check.That(_a!.ActiveServices.Length).IsEqualTo(0);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ActiveServiceRequest() { Modified = CatalogExamArk.ToolMod.Day.ToString(CultureInfo.InvariantCulture) })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _a = JsonSerializer.Deserialize<ActiveServiceResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_a).IsNotNull(); Check.That(_a!.ActiveServices.Length).IsEqualTo(1);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ActiveServiceRequest() { })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _a = JsonSerializer.Deserialize<ActiveServiceResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_a).IsNotNull(); Check.That(_a!.ActiveServices.Length).IsEqualTo(2);

        _r = this.Client!.PostAsync(_u,JsonContent.Create<Object?>(null)).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _a = JsonSerializer.Deserialize<ActiveServiceResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_a).IsNotNull(); Check.That(_a!.ActiveServices.Length).IsEqualTo(2);
    }

    [Test]
    public void SearchElements()
    {
        Uri _u = new Uri(String.Concat(this.EndpointLocator,"/Catalog/Elements")); HttpResponseMessage _r; ElementResponse? _e;

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ElementRequest(){ })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _e = JsonSerializer.Deserialize<ElementResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(19);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ElementRequest(){ Name = "deploy" , Type = DataType.PS1 })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _e = JsonSerializer.Deserialize<ElementResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ElementRequest(){ ObjectType = typeof(GenericItem).Name })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _e = JsonSerializer.Deserialize<ElementResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ElementRequest(){ Version = "1.0.1" })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _e = JsonSerializer.Deserialize<ElementResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(2);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ElementRequest(){ Name = "fig2" , Type = DataType.XML })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _e = JsonSerializer.Deserialize<ElementResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(1);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new ElementRequest(){ Type = DataType.DAT })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status404NotFound); _e = JsonSerializer.Deserialize<ElementResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(0);

        _r = this.Client!.PostAsync(_u,JsonContent.Create<Object?>(null)).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _e = JsonSerializer.Deserialize<ElementResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_e).IsNotNull(); Check.That(_e!.Elements.Length).IsEqualTo(19);
    }

    [Test]
    public void SearchMedia()
    {
        Uri _u = new Uri(String.Concat(this.EndpointLocator,"/Catalog/Media")); HttpResponseMessage _r; MediaResponse? _m;

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new MediaRequest(){ Title = "cosmo" })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _m = JsonSerializer.Deserialize<MediaResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(3);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new MediaRequest(){ Artist = "Tyson" })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _m = JsonSerializer.Deserialize<MediaResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(2);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new MediaRequest(){ Artist = "Tyson" , Year = "020" })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _m = JsonSerializer.Deserialize<MediaResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(1);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new MediaRequest(){ Name = "Naruto" })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status404NotFound); _m = JsonSerializer.Deserialize<MediaResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(0);

        _r = this.Client!.PostAsync(_u,JsonContent.Create<Object?>(null)).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _m = JsonSerializer.Deserialize<MediaResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.Media.Length).IsEqualTo(4);
    }

    [Test]
    public void SearchNotes()
    {
        Uri _u = new Uri(String.Concat(this.EndpointLocator,"/Catalog/Notes")); HttpResponseMessage _r; NoteResponse? _m;

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new NoteRequest(){ Notes = new List<String>(){"te10"}.ToArray() })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _m = JsonSerializer.Deserialize<NoteResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.IDs!.Count).IsEqualTo(1);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new NoteRequest(){ Notes = new List<String>(){"otE20"}.ToArray() })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _m = JsonSerializer.Deserialize<NoteResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.IDs!.Count).IsEqualTo(2);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new NoteRequest(){ Notes = new List<String>(){"300"}.ToArray() })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _m = JsonSerializer.Deserialize<NoteResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.IDs!.Count).IsEqualTo(3);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new NoteRequest(){ Notes = new List<String>(){"300","&"}.ToArray() })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status404NotFound); _m = JsonSerializer.Deserialize<NoteResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.IDs!.Count).IsEqualTo(0);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new NoteRequest(){ Notes = new List<String>(){}.ToArray() })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = this.Client!.PostAsync(_u,JsonContent.Create<Object?>(null)).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);
    }

    [Test]
    public void SearchTags()
    {
        Uri _u = new Uri(String.Concat(this.EndpointLocator,"/Catalog/Tags")); HttpResponseMessage _r; TagResponse? _m;

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new TagRequest(){ Tags = new List<String>(){"tag10"}.ToArray() })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _m = JsonSerializer.Deserialize<TagResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.IDs!.Count).IsEqualTo(1);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new TagRequest(){ Tags = new List<String>(){"G20"}.ToArray() })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _m = JsonSerializer.Deserialize<TagResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.IDs!.Count).IsEqualTo(2);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new TagRequest(){ Tags = new List<String>(){"300"}.ToArray() })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK); _m = JsonSerializer.Deserialize<TagResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.IDs!.Count).IsEqualTo(3);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new TagRequest(){ Tags = new List<String>(){"300","!"}.ToArray() })).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status404NotFound); _m = JsonSerializer.Deserialize<TagResponse>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_m).IsNotNull(); Check.That(_m!.IDs!.Count).IsEqualTo(0);

        _r = this.Client!.PostAsync(_u,JsonContent.Create(new TagRequest(){})).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        _r = this.Client!.PostAsync(_u,JsonContent.Create<Object?>(null)).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);
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