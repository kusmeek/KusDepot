namespace KusDepot.DataPodExams;

[TestFixture] [NonParallelizable]
public partial class DataControlExam
{
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
    private String? Authority;
    private String[]? Scope;

    public String? EndpointLocator;
    public String? CatalogEndpointLocator;
    public X509Certificate2? Certificate;
    public X509Certificate2? CatalogCertificate;

    [OneTimeSetUp]
    public async Task Calibrate()
    {
        Check.That(this.LoadSetup()).IsTrue();

        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();

        this.ReadToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync()).AccessToken;

        this.WriteToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync()).AccessToken;

        this.AdminToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.AdminUserName,this.AdminUserPass).ExecuteAsync()).AccessToken;

        Check.That(this.LoadSetup()).IsTrue(); this.EndpointLocator = "https://localhost:5007"; this.CatalogEndpointLocator = "https://localhost:5006";

        using X509Store s = new X509Store(StoreName.My,StoreLocation.CurrentUser); s.Open(OpenFlags.ReadOnly);
        this.Certificate = s.Certificates.Find(X509FindType.FindBySubjectName,"DataControlUser",true).First();
        this.CatalogCertificate = s.Certificates.Find(X509FindType.FindBySubjectName,"CatalogUser",true).First();
    }

    [OneTimeTearDown]
    public async Task Complete()
    {
        if(this.EndpointLocator is null || this.Certificate is null || this.AdminToken is null) { return; }

        using DataControlClient client = new(this.EndpointLocator,this.Certificate,this.AdminToken);

        RestResponse<DataControlServerSessionInfo[]> sessions = await client.GetServerSessions(token:this.AdminToken);

        foreach(DataControlServerSessionInfo session in sessions.Data ?? Array.Empty<DataControlServerSessionInfo>())
        {
            try
            {
                RestResponse<RemoveTransferResponse> removed = await client.RemoveTransfer(new RemoveTransferRequest()
                {
                    SessionID = session.SessionID,
                    ItemID = session.ItemID,
                },this.AdminToken).ConfigureAwait(false);

                if(removed.StatusCode == HttpStatusCode.OK && removed.Data?.Removed is true) { continue; }

                RestResponse<AbortTransferResponse> aborted = await client.AbortTransfer(new AbortTransferRequest()
                {
                    SessionID = session.SessionID,
                    ItemID = session.ItemID,
                },this.AdminToken).ConfigureAwait(false);

                if(aborted.StatusCode == HttpStatusCode.OK && aborted.Data?.Aborted is true)
                {
                    _ = await client.RemoveTransfer(new RemoveTransferRequest()
                    {
                        SessionID = session.SessionID,
                        ItemID = session.ItemID,
                    },this.AdminToken).ConfigureAwait(false);
                }
            }
            catch { }
        }
    }

    [Test] [Order(1)]
    public async Task Health()
    {
        RestClientOptions o = new(this.EndpointLocator!){ ClientCertificates = new(){this.Certificate!}};

        Check.That((await new RestClient(o).ExecuteAsync(new("/DataControl/Health",Method.Get))).Content).Equals("Healthy");
    }

    [Test] [Order(2)]
    public async Task QueryCommands()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!,this.WriteToken); DataControlUploadCompletionResult? uploaded;

        BinaryItem? _b = BinaryItem.FromFile(Path.Join(AppContext.BaseDirectory,"ContainerAssembly.bin")); _b?.SetID(Guid.NewGuid());

        using CatalogClient _cc = new(this.CatalogEndpointLocator!,this.CatalogCertificate!,this.ReadToken); RestResponse<CommandResponse> _cr; CommandResponse? _c;

        uploaded = await _dc.Upload(_b!);

        Check.That(uploaded).IsNotNull(); Check.That(uploaded!.Session.ItemId).IsEqualTo(_b!.GetID());

        _cr = await _cc.SearchCommands(new CommandQuery()); _c = _cr.Data;

        Check.That((Int32)_cr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(4);

        _cr = await _cc.SearchCommands(new CommandQuery() { CommandSpecifications = "sheet" }); _c = _cr.Data;

        Check.That((Int32)_cr.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(2);

        Check.That(_c!.Commands.All(_=>String.Equals(_.CommandHandle,"Handle001",StringComparison.Ordinal)));

        Check.That(_c!.Commands.Any(_=>String.Equals(_.CommandType,typeof(CommandExam0).FullName,StringComparison.Ordinal)));

        _cr = await _cc.SearchCommands(new CommandQuery() { CommandHandle = "002" }); _c = _cr.Data;

        Check.That((Int32)_cr.StatusCode).IsEqualTo(StatusCodes.Status200OK);

        Check.That(_c).IsNotNull(); Check.That(_c!.Commands.Length).IsEqualTo(2);

        Check.That(_c!.Commands.All(_=>_.CommandSpecifications!.Contains("usage",StringComparison.Ordinal)));

        Check.That(_c!.Commands.Any(_=>String.Equals(_.CommandType,typeof(CommandExam1).FullName,StringComparison.Ordinal)));

        RestResponse<Guid> deleted = await _dc.Delete(_b!.GetID());

        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(Guid.Equals(_b!.GetID(),deleted.Data)).IsTrue();
    }

    [Test] [Order(3)]
    public async Task QueryServices()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!,this.WriteToken); DataControlUploadCompletionResult? uploaded;

        BinaryItem? _b = BinaryItem.FromFile(Path.Join(AppContext.BaseDirectory,"ContainerAssembly.bin")); _b?.SetID(Guid.NewGuid());

        using CatalogClient _cc = new(this.CatalogEndpointLocator!,this.CatalogCertificate!,this.ReadToken); RestResponse<ServiceResponse> _sr; ServiceResponse? _s;

        uploaded = await _dc.Upload(_b!);

        Check.That(uploaded).IsNotNull(); Check.That(uploaded!.Session.ItemId).IsEqualTo(_b!.GetID());

        _sr = await _cc.SearchServices(new ServiceQuery() { }); _s = _sr.Data;

        Check.That((Int32)_sr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_s).IsNotNull(); Check.That(_s!.Services.Length).IsEqualTo(2);
        Check.That(_s.Services.All(_ => !String.IsNullOrWhiteSpace(_.ServiceSpecifications))).IsTrue();

        _sr = await _cc.SearchServices(new ServiceQuery() { ServiceInterfaces = "IDeviceManager" }); _s = _sr.Data;

        Check.That((Int32)_sr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_s).IsNotNull(); Check.That(_s!.Services.Length).IsEqualTo(1);
        Check.That(_s.Services.All(_ => _.ServiceSpecifications!.Contains("Specification",StringComparison.Ordinal))).IsTrue();

        _sr = await _cc.SearchServices(new ServiceQuery() { ServiceSpecifications = "sage" }); _s = _sr.Data;

        Check.That((Int32)_sr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_s).IsNotNull(); Check.That(_s!.Services.Length).IsEqualTo(1);
        Check.That(_s.Services.All(_ => _.ServiceSpecifications!.Contains("...",StringComparison.Ordinal))).IsTrue();
        Check.That(_s.Services.Any(_ => String.Equals(_.ServiceType,typeof(ServiceExam1).FullName,StringComparison.Ordinal))).IsTrue();

        _sr = await _cc.SearchServices(new ServiceQuery() { ServiceType = "ServiceExam1" }); _s = _sr.Data;

        Check.That((Int32)_sr.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(_s).IsNotNull(); Check.That(_s!.Services.Length).IsEqualTo(1);
        Check.That(_s.Services.All(_ => _.ServiceSpecifications!.Contains("Usage",StringComparison.Ordinal))).IsTrue();

        _sr = await _cc.SearchServices(new ServiceQuery(),"null"); Check.That((Int32)_sr.StatusCode).IsEqualTo(StatusCodes.Status401Unauthorized);

        RestResponse<Guid> deleted = await _dc.Delete(_b!.GetID());

        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(Guid.Equals(_b!.GetID(),deleted.Data)).IsTrue();
    }

    [Test] [Order(4)]
    public async Task SearchElementsFindsUploadedDataStreamItemAndRoundTripsStreamContent()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!,this.WriteToken);
        using CatalogClient _cc = new(this.CatalogEndpointLocator!,this.CatalogCertificate!,this.ReadToken);

        Byte[] payload = CreateSegmentedTransferPayload(4096,0xA1);
        String itemName = $"SearchElements.Stream.{Guid.NewGuid():N}";
        DataStreamItem item = new(new MemoryStream(payload,writable:false));
        String? downloadedStreamPath = null;

        item.SetID(Guid.NewGuid());
        item.SetName(itemName);

        try
        {
            DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken);

            Check.That(uploaded).IsNotNull();
            Check.That(uploaded!.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(uploaded.Session.ItemId).IsEqualTo(item.GetID());

            RestResponse<ElementResponse> searched = await _cc.SearchElements(new ElementQuery() { ObjectType = nameof(DataStreamItem) },this.ReadToken);

            Check.That((Int32)searched.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(searched.Data).IsNotNull();
            Check.That(searched.Data!.Elements.Any(_ => _.ID == item.GetID() && String.Equals(_.Name,itemName,StringComparison.Ordinal))).IsTrue();

            DataControlDownloadCompletionResult? downloaded = await _dc.Download(item.GetID()!.Value,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsInstanceOf<DataStreamItem>();
            Check.That(downloaded.Item!.GetName()).IsEqualTo(itemName);

            using Stream? content = downloaded.Item.GetContentStream();
            using MemoryStream loaded = new();

            Check.That(content).IsNotNull();

            await content!.CopyToAsync(loaded);

            Check.That(loaded.ToArray().SequenceEqual(payload)).IsTrue();

            downloadedStreamPath = downloaded.StreamPath;

            RestResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);

            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }
    }

    [Test] [Order(5)]
    public async Task SearchElementsFindsStreamUploadCommittedAsBinaryItemAndRoundTripsRenamedFile()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!,this.WriteToken);
        using CatalogClient _cc = new(this.CatalogEndpointLocator!,this.CatalogCertificate!,this.ReadToken);

        Byte[] payload = CreateSegmentedTransferPayload(8192,0xA2);
        String finalName = $"SearchElements.Final.{Guid.NewGuid():N}";
        DataStreamItem item = new(new MemoryStream(payload,writable:false));
        String? downloadedStreamPath = null;

        item.SetID(Guid.NewGuid());
        item.SetName($"SearchElements.StreamSource.{Guid.NewGuid():N}");

        try
        {
            DataControlUploadSession? session = await _dc.OpenUpload(item,this.WriteToken);

            Check.That(session).IsNotNull();

            Int32 uploadedSegments = await session!.UploadToCompletion(cancel:default);

            Check.That(uploadedSegments > 0).IsTrue();

            BinaryItem finalItem = new();
            finalItem.SetID(item.GetID());
            finalItem.SetName(finalName);

            DataControlUploadCompletionResult? committed = await session.Commit(finalItem);

            Check.That(committed).IsNotNull();
            Check.That(committed!.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(committed.Session.ItemId).IsEqualTo(item.GetID());

            RestResponse<ElementResponse> searched = await _cc.SearchElements(new ElementQuery() { ObjectType = nameof(BinaryItem) },this.ReadToken);

            Check.That((Int32)searched.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(searched.Data).IsNotNull();
            Check.That(searched.Data!.Elements.Any(_ => _.ID == item.GetID() && String.Equals(_.Name,finalName,StringComparison.Ordinal))).IsTrue();

            DataControlDownloadCompletionResult? downloaded = await _dc.Download(item.GetID()!.Value,this.ReadToken);

            Check.That(downloaded).IsNotNull();
            Check.That(downloaded!.Item).IsInstanceOf<BinaryItem>();
            Check.That(downloaded.Item!.GetName()).IsEqualTo(finalName);

            using Stream? content = downloaded.Item.GetContentStream();
            using MemoryStream loaded = new();

            Check.That(content).IsNotNull();

            await content!.CopyToAsync(loaded);

            Check.That(loaded.ToArray().SequenceEqual(payload)).IsTrue();

            downloadedStreamPath = downloaded.StreamPath;

            RestResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);

            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }
    }

    [Test] [Order(6)]
    public async Task X509()
    {
        #pragma warning disable SYSLIB0026
        DataControlClient _dc = new(this.EndpointLocator!,new());
        BinaryItem item = new(new Byte[]{ 0x01 });

        Check.That(await _dc.Upload(item)).IsNull();

        _dc = new(this.EndpointLocator!,this.CatalogCertificate!);

        Check.That(await _dc.Upload(item)).IsNull();
    }

    private Boolean LoadSetup()
    {
        try
        {
            XmlDocument d = new XmlDocument(); d.Load(Environment.CurrentDirectory + @"\EntraSetup.xml"); XmlNode? n = d.SelectSingleNode("EntraSetup");

            this.Authority     = n!.SelectSingleNode("Authority")!.InnerText;
            this.ClientID      = n.SelectSingleNode("ClientID")!.InnerText;
            this.Scope         = new List<String>(){n.SelectSingleNode("Scope")!.InnerText}.ToArray();
            this.ReadUserName  = n.SelectSingleNode("ReadUserName")!.InnerText;
            this.ReadUserPass  = n.SelectSingleNode("ReadUserPass")!.InnerText;
            this.WriteUserName = n.SelectSingleNode("WriteUserName")!.InnerText;
            this.WriteUserPass = n.SelectSingleNode("WriteUserPass")!.InnerText;
            this.AdminUserName = n.SelectSingleNode("AdminUserName")!.InnerText;
            this.AdminUserPass = n.SelectSingleNode("AdminUserPass")!.InnerText;

            return true;
        }
        catch (Exception) { return false; }
    }
}