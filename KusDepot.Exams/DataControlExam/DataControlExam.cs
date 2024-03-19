namespace KusDepot.FabricExams;

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
    public Tool? Tool;
    public HttpClient? Client;
    public String? EndpointLocator;

    [OneTimeSetUp]
    public void Calibrate()
    {
        this.Tool = new Tool(new HashSet<DataItem>(){new TextItem(DataGenerator.GenerateUnicodeString(10000))});

        Check.That(this.LoadSetup()).IsTrue(); this.Client = new HttpClient(); this.EndpointLocator = EndpointLocators.DataControlService;

        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();

        this.ReadToken = _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync().Result.AccessToken;

        this.WriteToken = _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync().Result.AccessToken;
    }

    [Test] [Order(4)]
    public void Delete()
    {
        HttpResponseMessage _r;

        this.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",String.Empty);

        _r = this.Client.DeleteAsync(this.URL!).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",this.ReadToken);

        _r = this.Client.DeleteAsync(this.URL).Result;

        Check.That(( Int32 )_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",this.WriteToken);

        _r = this.Client.DeleteAsync(this.URL).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Guid _g = Guid.Parse(JsonSerializer.Deserialize<String>(_r.Content.ReadAsStringAsync().Result)!);

        Check.That(Guid.Equals(this.Tool!.GetID(),_g)).IsTrue();
    }

    [Test] [Order(3)]
    public void Get()
    {
        HttpResponseMessage _r; Tool? _t; this.URL = new Uri(this.EndpointLocator + @"/DataControl/" + this.Tool!.GetID());

        this.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",String.Empty);

        _r = this.Client.GetAsync(this.URL!).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",this.ReadToken);

        _r = this.Client.GetAsync(this.URL).Result;

        Check.That(( Int32 )_r.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        DataControlDownload? _d = JsonSerializer.Deserialize<DataControlDownload>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_d.Verify()).IsTrue();

        _t = Tool.Parse(_d!.Object,null);

        Check.That(this.Tool.Equals(_t)).IsTrue();
    }

    [Test] [Order(1)]
    public void Health()
    {
        Uri _u = new Uri(String.Concat(this.EndpointLocator,"/DataControl/Health"));

        Check.That(this.Client!.GetAsync(_u).Result.Content.ReadAsStringAsync().Result).Equals("Healthy");
    }

    [Test] [Order(2)]
    public void Store()
    {
        HttpResponseMessage _r; DataControlUpload? _dcu; Guid? _g;

        this.URL = new Uri(this.EndpointLocator + @"/DataControl");

        this.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",String.Empty);

        _r = this.Client.PostAsync(this.URL!,JsonContent.Create(String.Empty)).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status400BadRequest);

        this.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",String.Empty);

        _dcu = this.Tool!.MakeDataControlUpload();

        _r = this.Client.PostAsync(this.URL!,JsonContent.Create(_dcu)).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        this.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",this.ReadToken);

        _r = this.Client.PostAsync(this.URL!,JsonContent.Create(_dcu)).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        this.Client!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",this.WriteToken);

        _r = this.Client.PostAsync(this.URL!,JsonContent.Create(_dcu)).Result;

        Check.That((Int32)_r.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        _g = JsonSerializer.Deserialize<Guid>(_r.Content.ReadAsStringAsync().Result);

        Check.That(_g).Equals(_dcu!.Descriptor.ID);
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