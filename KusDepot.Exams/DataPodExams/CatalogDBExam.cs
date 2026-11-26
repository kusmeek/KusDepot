using DataPodServices.CatalogDB;

namespace KusDepot.DataPodExams;

[TestFixture] [NonParallelizable]
public class CatalogDBExam
{
    public String? ID;
    public DataItem? Item;
    public ManagementKey? Key;
    private IToolGenericHost? Host;
    private DataPodServices.CatalogDB.ICatalogDB? CatalogDB;
    private Guid ItemInstanceToken;
    private static IEnumerable<Descriptor>? Data {get;} = CatalogExamData.Build();

    [OneTimeSetUp]
    public async Task Calibrate()
    {
        IToolGenericHostBuilder tb = ToolBuilderFactory.CreateGenericHostBuilder();

        tb.Builder.UseOrleansClient(OrleansExamClient.Configure);

        Host = await tb.BuildGenericHostAsync(); await Host.StartHostAsync(new CancellationTokenSource(TimeSpan.FromMinutes(5)).Token);

        IGrainFactory? gf = Host.Services.GetService<IGrainFactory>();

        DataPodServices.CatalogDB.ICatalogDB? tg = gf?.GetGrain<DataPodServices.CatalogDB.ICatalogDB>("CatalogDBPodExam");

        Check.That(tg).IsNotNull(); CatalogDB = tg;

        foreach (var d in Data!)
        {
            Check.That(await this.CatalogDB!.AddUpdate(d,Guid.NewGuid(),null)).IsTrue();
        }

        this.Item = new BinaryItem();
    }

    [OneTimeTearDown]
    public async Task Complete()
    {
        foreach (var d in Data!)
        {
            CatalogItemIdentity? identity = await this.CatalogDB!.BeginDelete(d.ID!.Value,null);
            if(identity is not null) { await this.CatalogDB.FinalizeDelete(d.ID!.Value,identity.ItemInstanceToken,null); }
        }

        await Host!.DisposeAsync();
    }

    [Test]
    public async Task SearchElements()
    {
        var r = await this.CatalogDB!.SearchElements(new ElementQuery(), null);
        Check.That(r.Elements.Length).IsEqualTo(20);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { Name = "deploy", DataType = DataTypes.PS1 }, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { ObjectType = nameof(GenericItem) }, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { ObjectType = nameof(KeySet) }, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { DataType = DataTypes.KEYSET }, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { Version = "1.0.1" }, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { Name = "fig2", DataType = DataTypes.XML }, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { DataType = DataTypes.DAT }, null);
        Check.That(r.Elements.Length).IsEqualTo(0);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { ContentStreamed = true }, null);
        Check.That(r.Elements.Length).IsEqualTo(1);
    }

    [Test]
    public async Task SearchPagesFollowContinuationTokens()
    {
        ElementQuery elementQuery = new() { CatalogPageSize = 3 };
        List<Guid> elementIDs = new();
        do
        {
            ElementResponse page = await this.CatalogDB!.SearchElements(elementQuery,null);
            elementIDs.AddRange(page.Elements.Select(element => element.ID!.Value));
            elementQuery = elementQuery with { CatalogPageToken = page.NextCatalogPageToken };
        }
        while(elementQuery.CatalogPageToken is not null);
        Check.That(elementIDs.Count).IsEqualTo(20);
        Check.That(elementIDs.Distinct().Count()).IsEqualTo(20);

        MediaQuery mediaQuery = new() { CatalogPageSize = 2 };
        List<Guid> mediaIDs = new();
        do
        {
            MediaResponse page = await this.CatalogDB!.SearchMedia(mediaQuery,null);
            mediaIDs.AddRange(page.Media.Select(media => media.ID!.Value));
            mediaQuery = mediaQuery with { CatalogPageToken = page.NextCatalogPageToken };
        }
        while(mediaQuery.CatalogPageToken is not null);
        Check.That(mediaIDs.Count).IsEqualTo(4);
        Check.That(mediaIDs.Distinct().Count()).IsEqualTo(4);

        CommandQuery commandQuery = new() { CatalogPageSize = 2 };
        List<Guid> commandIDs = new();
        List<(Guid? ID,String? Type,String? Handle,String? Specifications)> commandRows = new();
        do
        {
            CommandResponse page = await this.CatalogDB!.SearchCommands(commandQuery,null);
            commandIDs.AddRange(page.Commands.Select(command => command.ID!.Value));
            commandRows.AddRange(page.Commands.Select(command => (command.ID,command.CommandType,command.CommandHandle,command.CommandSpecifications)));
            commandQuery = commandQuery with { CatalogPageToken = page.NextCatalogPageToken };
        }
        while(commandQuery.CatalogPageToken is not null);
        Check.That(commandIDs.Count).IsEqualTo(4);
        Check.That(commandIDs.Distinct().Count()).IsEqualTo(1);
        Check.That(commandRows.Distinct().Count()).IsEqualTo(4);

        ServiceQuery serviceQuery = new() { CatalogPageSize = 1 };
        List<Guid> serviceIDs = new();
        List<(Guid? ID,String? Type,String? Specifications)> serviceRows = new();
        do
        {
            ServiceResponse page = await this.CatalogDB!.SearchServices(serviceQuery,null);
            serviceIDs.AddRange(page.Services.Select(service => service.ID!.Value));
            serviceRows.AddRange(page.Services.Select(service => (service.ID,service.ServiceType,service.ServiceSpecifications)));
            serviceQuery = serviceQuery with { CatalogPageToken = page.NextCatalogPageToken };
        }
        while(serviceQuery.CatalogPageToken is not null);
        Check.That(serviceIDs.Count).IsEqualTo(2);
        Check.That(serviceIDs.Distinct().Count()).IsEqualTo(1);
        Check.That(serviceRows.Distinct().Count()).IsEqualTo(2);

        NoteQuery noteQuery = new() { CatalogPageSize = 1, Notes = new[] { "Note" } };
        List<Guid> noteIDs = new();
        do
        {
            NoteResponse page = await this.CatalogDB!.SearchNotes(noteQuery,null);
            noteIDs.AddRange(page.IDs);
            noteQuery = noteQuery with { CatalogPageToken = page.NextCatalogPageToken };
        }
        while(noteQuery.CatalogPageToken is not null);
        Check.That(noteIDs.Count).IsEqualTo(3);
        Check.That(noteIDs.Distinct().Count()).IsEqualTo(3);
        Check.That(noteIDs.SequenceEqual(noteIDs.OrderBy(id => id))).IsTrue();

        TagQuery tagQuery = new() { CatalogPageSize = 1, Tags = new[] { "Tag" } };
        List<Guid> tagIDs = new();
        do
        {
            TagResponse page = await this.CatalogDB!.SearchTags(tagQuery,null);
            tagIDs.AddRange(page.IDs);
            tagQuery = tagQuery with { CatalogPageToken = page.NextCatalogPageToken };
        }
        while(tagQuery.CatalogPageToken is not null);
        Check.That(tagIDs.Count).IsEqualTo(3);
        Check.That(tagIDs.Distinct().Count()).IsEqualTo(3);
        Check.That(tagIDs.SequenceEqual(tagIDs.OrderBy(id => id))).IsTrue();
    }

    [Test]
    public async Task SearchCommands()
    {
        var r = await this.CatalogDB!.SearchCommands(new CommandQuery(), null);
        Check.That(r.Commands.Length).IsEqualTo(4);

        r = await this.CatalogDB!.SearchCommands(new CommandQuery { CommandSpecifications = "sheet" }, null);
        Check.That(r.Commands.Length).IsEqualTo(2);
        Check.That(r.Commands.All(_ => String.Equals(_.CommandHandle, "Handle001", StringComparison.Ordinal))).IsTrue();
        Check.That(r.Commands.Any(_ => String.Equals(_.CommandType, typeof(CommandExam0).FullName, StringComparison.Ordinal))).IsTrue();

        r = await this.CatalogDB!.SearchCommands(new CommandQuery { CommandHandle = "002" }, null);
        Check.That(r.Commands.Length).IsEqualTo(2);
        Check.That(r.Commands.All(_ => _.CommandSpecifications!.Contains("Usage", StringComparison.Ordinal))).IsTrue();
        Check.That(r.Commands.Any(_ => String.Equals(_.CommandType, typeof(CommandExam1).FullName, StringComparison.Ordinal))).IsTrue();
    }

    [Test]
    public async Task SearchMedia()
    {
        var r = await this.CatalogDB!.SearchMedia(new MediaQuery { Title = "cosmo" }, null);
        Check.That(r.Media.Length).IsEqualTo(3);

        r = await this.CatalogDB!.SearchMedia(new MediaQuery { Artist = "Tyson" }, null);
        Check.That(r.Media.Length).IsEqualTo(2);

        r = await this.CatalogDB!.SearchMedia(new MediaQuery { Artist = "Tyson", Year = "020" }, null);
        Check.That(r.Media.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchMedia(new MediaQuery { Name = "Naruto" }, null);
        Check.That(r.Media.Length).IsEqualTo(0);

        r = await this.CatalogDB!.SearchMedia(new MediaQuery(), null);
        Check.That(r.Media.Length).IsEqualTo(4);
    }

    [Test]
    public async Task SearchNotes()
    {
        var r = await this.CatalogDB!.SearchNotes(new NoteQuery { Notes = new List<String> { "te10" }.ToArray() }, null);
        Check.That(r.IDs.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchNotes(new NoteQuery { Notes = new List<String> { "otE20" }.ToArray() }, null);
        Check.That(r.IDs.Length).IsEqualTo(2);

        r = await this.CatalogDB!.SearchNotes(new NoteQuery { Notes = new List<String> { "300" }.ToArray() }, null);
        Check.That(r.IDs.Length).IsEqualTo(3);

        r = await this.CatalogDB!.SearchNotes(new NoteQuery { Notes = new List<String> { "300", "&" }.ToArray() }, null);
        Check.That(r.IDs.Length).IsEqualTo(0);

        r = await this.CatalogDB!.SearchNotes(new NoteQuery { Notes = Array.Empty<String>() }, null);
        Check.That(r.IDs.Length).IsEqualTo(0);

        r = await this.CatalogDB!.SearchNotes(new NoteQuery(), null);
        Check.That(r.IDs.Length).IsEqualTo(0);
    }

    [Test]
    public async Task SearchServices()
    {
        var r = await this.CatalogDB!.SearchServices(new ServiceQuery { }, null);
        Check.That(r.Services.Length).IsEqualTo(2);
        Check.That(r.Services.All(_ => !String.IsNullOrWhiteSpace(_.ServiceSpecifications))).IsTrue();

        r = await this.CatalogDB!.SearchServices(new ServiceQuery { ServiceInterfaces = "IDeviceManager" }, null);
        Check.That(r.Services.Length).IsEqualTo(1);
        Check.That(r.Services.All(_ => _.ServiceSpecifications!.Contains("Specification", StringComparison.Ordinal))).IsTrue();

        r = await this.CatalogDB!.SearchServices(new ServiceQuery { ServiceSpecifications = "sage" }, null);
        Check.That(r.Services.Length).IsEqualTo(1);
        Check.That(r.Services.All(_ => _.ServiceSpecifications!.Contains("...", StringComparison.Ordinal))).IsTrue();
        Check.That(r.Services.Any(_ => String.Equals(_.ServiceType, typeof(ServiceExam1).FullName, StringComparison.Ordinal))).IsTrue();

        r = await this.CatalogDB!.SearchServices(new ServiceQuery { ServiceType = "ServiceExam1" }, null);
        Check.That(r.Services.Length).IsEqualTo(1);
        Check.That(r.Services.All(_ => _.ServiceSpecifications!.Contains("Usage", StringComparison.Ordinal))).IsTrue();
    }

    [Test]
    public async Task SearchTags()
    {
        var r = await this.CatalogDB!.SearchTags(new TagQuery { Tags = new List<String> { "tag10" }.ToArray() }, null);
        Check.That(r.IDs.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchTags(new TagQuery { Tags = new List<String> { "G20" }.ToArray() }, null);
        Check.That(r.IDs.Length).IsEqualTo(2);

        r = await this.CatalogDB!.SearchTags(new TagQuery { Tags = new List<String> { "300" }.ToArray() }, null);
        Check.That(r.IDs.Length).IsEqualTo(3);

        r = await this.CatalogDB!.SearchTags(new TagQuery { Tags = new List<String> { "300", "!" }.ToArray() }, null);
        Check.That(r.IDs.Length).IsEqualTo(0);

        r = await this.CatalogDB!.SearchTags(new TagQuery { Tags = Array.Empty<String>() }, null);
        Check.That(r.IDs.Length).IsEqualTo(0);

        r = await this.CatalogDB!.SearchTags(new TagQuery(), null);
        Check.That(r.IDs.Length).IsEqualTo(0);
    }

    [Test] [NonParallelizable] [Order(100)]
    public async Task AddUpdate()
    {
        this.ItemInstanceToken = Guid.NewGuid();

        Check.That(await this.CatalogDB!.AddUpdate(this.Item!.GetDescriptor(),this.ItemInstanceToken,null)).IsTrue();
    }

    [Test] [NonParallelizable] [Order(101)]
    public async Task Exists()
    {
        var r = await this.CatalogDB!.Exists(this.Item!.GetDescriptor()!.ID, null);

        Check.That(r.HasValue).IsTrue();
        Check.That(r!.Value).IsTrue();
    }

    [Test] [NonParallelizable] [Order(102)]
    public async Task Delete()
    {
        Guid itemID = this.Item!.GetDescriptor()!.ID!.Value;
        CatalogItemIdentity? identity = await this.CatalogDB!.BeginDelete(itemID,null);

        Check.That(identity).IsNotNull();
        Check.That(await this.CatalogDB.FinalizeDelete(itemID,this.ItemInstanceToken,null)).IsEqualTo(CatalogDeleteFinalizationResult.Removed);
        var r = await this.CatalogDB!.Exists(this.Item!.GetDescriptor()!.ID, null);
        Check.That(r.HasValue).IsTrue();
        Check.That(r!.Value).IsFalse();
    }

    [Test]
    public async Task ItemInstanceLifecycleFencesPendingDeleteAndReusedItemID()
    {
        Descriptor descriptor = new BinaryItem().GetDescriptor()!;
        Guid itemID = descriptor.ID!.Value;
        Guid firstToken = Guid.NewGuid();
        Guid replacementToken = Guid.NewGuid();

        try
        {
            Check.That(await this.CatalogDB!.AddUpdate(descriptor,firstToken,null)).IsTrue();
            Check.That((await this.CatalogDB.GetItemIdentity(itemID,null))?.ItemInstanceToken).IsEqualTo(firstToken);
            Check.That(await this.CatalogDB.AddUpdate(descriptor,Guid.NewGuid(),null)).IsFalse();

            CatalogItemIdentity? deleting = await this.CatalogDB.BeginDelete(itemID,null);
            CatalogItemIdentity? repeated = await this.CatalogDB.BeginDelete(itemID,null);
            Check.That(deleting?.ItemInstanceToken).IsEqualTo(firstToken);
            Check.That(repeated?.ItemInstanceToken).IsEqualTo(firstToken);
            Check.That(await this.CatalogDB.GetItemIdentity(itemID,null)).IsNull();
            Check.That((await this.CatalogDB.GetItemState(itemID,null))?.IsPendingDelete).IsEqualTo(true);

            CatalogPendingDeletePage pending = await this.CatalogDB.GetPendingDeletes(null,100);
            Check.That(pending.Items.Any(item => item.ItemID == itemID && item.ItemInstanceToken == firstToken)).IsTrue();
            Check.That(await this.CatalogDB.FinalizeDelete(itemID,firstToken,null)).IsEqualTo(CatalogDeleteFinalizationResult.Removed);

            Check.That(await this.CatalogDB.AddUpdate(descriptor,replacementToken,null)).IsTrue();
            Check.That(await this.CatalogDB.FinalizeDelete(itemID,firstToken,null)).IsEqualTo(CatalogDeleteFinalizationResult.TokenMismatch);
            Check.That((await this.CatalogDB.GetItemIdentity(itemID,null))?.ItemInstanceToken).IsEqualTo(replacementToken);
        }
        finally
        {
            CatalogItemIdentity? remaining = await this.CatalogDB!.BeginDelete(itemID,null);

            if(remaining is not null) { await this.CatalogDB.FinalizeDelete(itemID,remaining.ItemInstanceToken,null); }
        }
    }
}