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

    private String? StorageConnectionString;
    private HashSet<String> ExistingStorageContainers = new(StringComparer.OrdinalIgnoreCase);

    [OneTimeSetUp]
    public async Task Calibrate()
    {
        await this.LoadSetup();

        BlobServiceClient storage = new(this.StorageConnectionString!);

        await foreach(BlobContainerItem container in storage.GetBlobContainersAsync())
        {
            this.ExistingStorageContainers.Add(container.Name);
        }

        IPublicClientApplication _ = PublicClientApplicationBuilder.Create(this.ClientID).WithAuthority(new Uri(this.Authority!)).Build();

        this.ReadToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.ReadUserName,this.ReadUserPass).ExecuteAsync()).AccessToken;

        this.WriteToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.WriteUserName,this.WriteUserPass).ExecuteAsync()).AccessToken;

        this.AdminToken = (await _.AcquireTokenByUsernamePassword(this.Scope,this.AdminUserName,this.AdminUserPass).ExecuteAsync()).AccessToken;

        if(DataPodAppConfiguration.IsAzureContainerProfile())
        {
            this.EndpointLocator = Environment.GetEnvironmentVariable("KUSDEPOT_DATACONTROL_ENDPOINT");
            this.CatalogEndpointLocator = Environment.GetEnvironmentVariable("KUSDEPOT_CATALOG_ENDPOINT");

            if(String.IsNullOrWhiteSpace(this.EndpointLocator)) { throw new InvalidOperationException("KUSDEPOT_DATACONTROL_ENDPOINT must be set for the AzureContainer deployment profile."); }
            if(String.IsNullOrWhiteSpace(this.CatalogEndpointLocator)) { throw new InvalidOperationException("KUSDEPOT_CATALOG_ENDPOINT must be set for the AzureContainer deployment profile."); }
        }
        else
        {
            this.EndpointLocator = "http://localhost:5007";
            this.CatalogEndpointLocator = "http://localhost:5006";
        }
    }

    [OneTimeTearDown]
    public async Task Complete()
    {
        if(this.EndpointLocator is null || this.AdminToken is null) { return; }

        using DataControlClient client = new(this.EndpointLocator,this.AdminToken);

        DataControlResponse<DataControlServerSessionInfo[]> sessions = await client.GetServerSessions(token:this.AdminToken);

        foreach(DataControlServerSessionInfo session in sessions.Data ?? Array.Empty<DataControlServerSessionInfo>())
        {
            try
            {
                DataItemTransferStatus status = session.SegmentedState?.Status ?? session.StreamState?.Status ?? DataItemTransferStatus.Open;

                if(status is not DataItemTransferStatus.Complete and not DataItemTransferStatus.Published)
                {
                    _ = await client.AbortTransfer(new AbortTransferRequest()
                    {
                        SessionID = session.SessionID,
                        ItemID = session.ItemID,
                    },this.AdminToken).ConfigureAwait(false);
                }
                else
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

        if(String.IsNullOrWhiteSpace(this.StorageConnectionString) is false)
        {
            BlobServiceClient storage = new(this.StorageConnectionString);

            await foreach(BlobContainerItem container in storage.GetBlobContainersAsync())
            {
                if(this.ExistingStorageContainers.Contains(container.Name) || Guid.TryParse(container.Name,out _) is false) { continue; }

                try { await storage.GetBlobContainerClient(container.Name).DeleteIfExistsAsync().ConfigureAwait(false); }
                catch { }
            }
        }
    }

    [Test]
    public async Task Health()
    {
        using HttpClient client = new() { BaseAddress = new Uri(this.EndpointLocator!,UriKind.Absolute) };
        using HttpResponseMessage response = await client.GetAsync("/DataControl/Health");
        String content = await response.Content.ReadAsStringAsync();
        Check.That(response.StatusCode).Equals(HttpStatusCode.OK);
        Check.That(content).Equals(String.Empty);
    }

    [Test]
    public async Task ConcurrentDistinctItemUploadsRemainSearchable()
    {
        const Int32 itemCount = 4;
        String namePrefix = $"ConcurrentItemMutation.{Guid.NewGuid():N}";
        String tag = $"{namePrefix}.tag";
        String note = $"{namePrefix}.note";
        BinaryItem[] items = Enumerable.Range(0,itemCount).Select(index => new BinaryItem(
            CreateSegmentedTransferPayload(256 * 1024,unchecked((Byte)(0x41 + index))),
            id:Guid.NewGuid(),
            name:$"{namePrefix}.{index}",
            notes:[note],
            tags:[tag])).ToArray();

        using CatalogClient catalog = new(this.CatalogEndpointLocator!,this.ReadToken);
        TaskCompletionSource start = new(TaskCreationOptions.RunContinuationsAsynchronously);

        Task[] uploads = items.Select(async item =>
        {
            await start.Task;

            using DataControlClient client = new(this.EndpointLocator!,this.WriteToken);
            DataControlUploadCompletionResult? result = await client.Upload(item,this.WriteToken);

            Check.That(result).IsNotNull();
            Check.That(result!.Session.ItemId).IsEqualTo(item.GetID());
        }).ToArray();

        Task searches = Task.Run(async () =>
        {
            await start.Task;

            do
            {
                CatalogResponse<ElementResponse> response = await catalog.SearchElements(new ElementQuery { Name = namePrefix },this.ReadToken);

                Check.That(new[] { StatusCodes.Status200OK,StatusCodes.Status404NotFound }.Contains((Int32)response.StatusCode)).IsTrue();
                Check.That(response.Data).IsNotNull();
                Check.That((response.Data?.Elements ?? Array.Empty<Element>()).All(element =>
                    items.Any(item => item.GetID() == element.ID) &&
                    element.Tags?.Contains(tag,StringComparer.Ordinal) is true &&
                    element.Notes?.Contains(note,StringComparer.Ordinal) is true)).IsTrue();

                await Task.Yield();
            }
            while(uploads.Any(upload => !upload.IsCompleted));
        });

        try
        {
            start.SetResult();

            await Task.WhenAll(uploads.Append(searches));

            CatalogResponse<ElementResponse> final = await catalog.SearchElements(new ElementQuery { Name = namePrefix },this.ReadToken);

            Check.That((Int32)final.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(final.Data).IsNotNull();
            Check.That(final.Data!.Elements.Length).IsEqualTo(itemCount);
            Check.That(final.Data.Elements.All(element => element.Tags?.Contains(tag,StringComparer.Ordinal) is true && element.Notes?.Contains(note,StringComparer.Ordinal) is true)).IsTrue();
        }
        finally
        {
            using DataControlClient cleanup = new(this.EndpointLocator!,this.WriteToken);

            foreach(BinaryItem item in items)
            {
                try { await cleanup.Delete(item.GetID(),this.WriteToken); }
                catch { }
            }
        }
    }

    [Test]
    public async Task QueryCommands()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.WriteToken); DataControlUploadCompletionResult? uploaded;

        BinaryItem? _b = BinaryItem.FromFile(Path.Join(AppContext.BaseDirectory,"ContainerAssembly.bin")); _b?.SetID(Guid.NewGuid());

        using CatalogClient _cc = new(this.CatalogEndpointLocator!,this.ReadToken); CatalogResponse<CommandResponse> _cr; CommandResponse? _c;

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

        DataControlResponse<Guid> deleted = await _dc.Delete(_b!.GetID());

        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(Guid.Equals(_b!.GetID(),deleted.Data)).IsTrue();
    }

    [Test]
    public async Task QueryServices()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.WriteToken); DataControlUploadCompletionResult? uploaded;

        BinaryItem? _b = BinaryItem.FromFile(Path.Join(AppContext.BaseDirectory,"ContainerAssembly.bin")); _b?.SetID(Guid.NewGuid());

        using CatalogClient _cc = new(this.CatalogEndpointLocator!,this.ReadToken); CatalogResponse<ServiceResponse> _sr; ServiceResponse? _s;

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

        DataControlResponse<Guid> deleted = await _dc.Delete(_b!.GetID());

        Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK); Check.That(Guid.Equals(_b!.GetID(),deleted.Data)).IsTrue();
    }

    [Test]
    public async Task SearchElementsFindsUploadedDataStreamItemAndRoundTripsStreamContent()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.WriteToken);
        using CatalogClient _cc = new(this.CatalogEndpointLocator!,this.ReadToken);

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

            CatalogResponse<ElementResponse> searched = await _cc.SearchElements(new ElementQuery() { ObjectType = nameof(DataStreamItem) },this.ReadToken);

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

            DataControlResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);

            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }
    }

    [Test]
    public async Task SearchElementsFindsStreamUploadPublishedAsBinaryItemAndRoundTripsRenamedFile()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.WriteToken);
        using CatalogClient _cc = new(this.CatalogEndpointLocator!,this.ReadToken);

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

            DataControlUploadCompletionResult? published = await session.Publish(finalItem);

            Check.That(published).IsNotNull();
            Check.That(published!.TransferFamily).IsEqualTo(DataControlTransferFamily.Stream);
            Check.That(published.Session.ItemId).IsEqualTo(item.GetID());

            CatalogResponse<ElementResponse> searched = await _cc.SearchElements(new ElementQuery() { ObjectType = nameof(BinaryItem) },this.ReadToken);

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

            DataControlResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);

            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }
    }

    private async Task LoadSetup()
    {
        EntraSetupRecord setup = await EntraSetupProvider.LoadAsync().ConfigureAwait(false);

        this.Authority = setup.Authority;
        this.ClientID = setup.ClientID;
        this.Scope = [setup.Scope];
        this.ReadUserName = setup.ReadUserName;
        this.ReadUserPass = setup.ReadUserPass;
        this.WriteUserName = setup.WriteUserName;
        this.WriteUserPass = setup.WriteUserPass;
        this.AdminUserName = setup.AdminUserName;
        this.AdminUserPass = setup.AdminUserPass;
        this.StorageConnectionString = setup.ConnectionString;
    }
}