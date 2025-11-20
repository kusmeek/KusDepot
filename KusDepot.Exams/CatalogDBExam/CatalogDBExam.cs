namespace KusDepot.FabricExams.Data;

[TestFixture] [Parallelizable]
public class CatalogDBExam
{
    private DataItem? Item;
    private ActorId? ActorId;
    private ICatalogDB? CatalogDB;
    private static IEnumerable<Descriptor>? Data {get;} = CatalogExamData.Build();

    [OneTimeSetUp]
    public async Task Calibrate()
    {
        this.ActorId = new ActorId("CatalogDBExam");
        this.CatalogDB = ActorProxy.Create<ICatalogDB>(ActorId, ServiceLocators.CatalogDBService);

        foreach (var d in Data!)
        {
            Check.That(await this.CatalogDB!.AddUpdate(d, null, null)).IsTrue();
        }

        this.Item = new BinaryItem();
    }

    [OneTimeTearDown]
    public async Task Complete()
    {
        foreach (var d in Data!)
        {
            await this.CatalogDB!.RemoveID(d.ID, null, null);
        }

        await ActorServiceProxy.Create(ServiceLocators.CatalogDBService, ActorId!).DeleteActorAsync(ActorId, CancellationToken.None);
    }

    [Test]
    public async Task SearchElements()
    {
        var r = await this.CatalogDB!.SearchElements(new ElementQuery(), null, null);
        Check.That(r.Elements.Length).IsEqualTo(20);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { Name = "deploy", Type = DataType.PS1 }, null, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { ObjectType = nameof(GenericItem) }, null, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { ObjectType = nameof(KeySet) }, null, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { Type = DataType.KEYSET }, null, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { Version = "1.0.1" }, null, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { Name = "fig2", Type = DataType.XML }, null, null);
        Check.That(r.Elements.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { Type = DataType.DAT }, null, null);
        Check.That(r.Elements.Length).IsEqualTo(0);

        r = await this.CatalogDB!.SearchElements(new ElementQuery { ContentStreamed = true }, null, null);
        Check.That(r.Elements.Length).IsEqualTo(1);
    }

    [Test]
    public async Task SearchCommands()
    {
        var r = await this.CatalogDB!.SearchCommands(new CommandQuery(), null, null);
        Check.That(r.Commands.Length).IsEqualTo(4);

        r = await this.CatalogDB!.SearchCommands(new CommandQuery { CommandSpecifications = "sheet" }, null, null);
        Check.That(r.Commands.Length).IsEqualTo(2);
        Check.That(r.Commands.All(_ => String.Equals(_.CommandHandle, "Handle001", StringComparison.Ordinal))).IsTrue();
        Check.That(r.Commands.Any(_ => String.Equals(_.CommandType, typeof(KusDepot.FabricExams.CommandExam0).FullName, StringComparison.Ordinal))).IsTrue();

        r = await this.CatalogDB!.SearchCommands(new CommandQuery { CommandHandle = "002" }, null, null);
        Check.That(r.Commands.Length).IsEqualTo(2);
        Check.That(r.Commands.All(_ => _.CommandSpecifications!.Contains("Usage", StringComparison.Ordinal))).IsTrue();
        Check.That(r.Commands.Any(_ => String.Equals(_.CommandType, typeof(KusDepot.FabricExams.CommandExam1).FullName, StringComparison.Ordinal))).IsTrue();
    }

    [Test]
    public async Task SearchMedia()
    {
        var r = await this.CatalogDB!.SearchMedia(new MediaQuery { Title = "cosmo" }, null, null);
        Check.That(r.Media.Length).IsEqualTo(3);

        r = await this.CatalogDB!.SearchMedia(new MediaQuery { Artist = "Tyson" }, null, null);
        Check.That(r.Media.Length).IsEqualTo(2);

        r = await this.CatalogDB!.SearchMedia(new MediaQuery { Artist = "Tyson", Year = "020" }, null, null);
        Check.That(r.Media.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchMedia(new MediaQuery { Name = "Naruto" }, null, null);
        Check.That(r.Media.Length).IsEqualTo(0);

        r = await this.CatalogDB!.SearchMedia(new MediaQuery(), null, null);
        Check.That(r.Media.Length).IsEqualTo(4);
    }

    [Test]
    public async Task SearchNotes()
    {
        var r = await this.CatalogDB!.SearchNotes(new NoteQuery { Notes = new List<String> { "te10" }.ToArray() }, null, null);
        Check.That(r.IDs.Count).IsEqualTo(1);

        r = await this.CatalogDB!.SearchNotes(new NoteQuery { Notes = new List<String> { "otE20" }.ToArray() }, null, null);
        Check.That(r.IDs.Count).IsEqualTo(2);

        r = await this.CatalogDB!.SearchNotes(new NoteQuery { Notes = new List<String> { "300" }.ToArray() }, null, null);
        Check.That(r.IDs.Count).IsEqualTo(3);

        r = await this.CatalogDB!.SearchNotes(new NoteQuery { Notes = new List<String> { "300", "&" }.ToArray() }, null, null);
        Check.That(r.IDs.Count).IsEqualTo(0);

        r = await this.CatalogDB!.SearchNotes(new NoteQuery { Notes = Array.Empty<String>() }, null, null);
        Check.That(r.IDs.Count).IsEqualTo(0);

        r = await this.CatalogDB!.SearchNotes(new NoteQuery(), null, null);
        Check.That(r.IDs.Count).IsEqualTo(0);
    }

    [Test]
    public async Task SearchServices()
    {
        var r = await this.CatalogDB!.SearchServices(new ServiceQuery { }, null, null);
        Check.That(r.Services.Length).IsEqualTo(2);

        r = await this.CatalogDB!.SearchServices(new ServiceQuery { ServiceInterfaces = "IDeviceManager" }, null, null);
        Check.That(r.Services.Length).IsEqualTo(1);

        r = await this.CatalogDB!.SearchServices(new ServiceQuery { ServiceType = "ServiceExam1" }, null, null);
        Check.That(r.Services.Length).IsEqualTo(1);
    }

    [Test]
    public async Task SearchTags()
    {
        var r = await this.CatalogDB!.SearchTags(new TagQuery { Tags = new List<String> { "tag10" }.ToArray() }, null, null);
        Check.That(r.IDs.Count).IsEqualTo(1);

        r = await this.CatalogDB!.SearchTags(new TagQuery { Tags = new List<String> { "G20" }.ToArray() }, null, null);
        Check.That(r.IDs.Count).IsEqualTo(2);

        r = await this.CatalogDB!.SearchTags(new TagQuery { Tags = new List<String> { "300" }.ToArray() }, null, null);
        Check.That(r.IDs.Count).IsEqualTo(3);

        r = await this.CatalogDB!.SearchTags(new TagQuery { Tags = new List<String> { "300", "!" }.ToArray() }, null, null);
        Check.That(r.IDs.Count).IsEqualTo(0);

        r = await this.CatalogDB!.SearchTags(new TagQuery { Tags = Array.Empty<String>() }, null, null);
        Check.That(r.IDs.Count).IsEqualTo(0);

        r = await this.CatalogDB!.SearchTags(new TagQuery(), null, null);
        Check.That(r.IDs.Count).IsEqualTo(0);
    }

    [Test] [NonParallelizable] [Order(100)]
    public async Task AddUpdate()
    {
        Check.That(await this.CatalogDB!.AddUpdate(this.Item!.GetDescriptor(), null, null)).IsTrue();
    }

    [Test] [NonParallelizable] [Order(101)]
    public async Task Exists()
    {
        var r = await this.CatalogDB!.Exists(this.Item!.GetDescriptor(), null, null);
        Check.That(r.HasValue).IsTrue();
        Check.That(r.Value).IsTrue();
    }

    [Test] [NonParallelizable] [Order(102)]
    public async Task ExistsID()
    {
        var r = await this.CatalogDB!.ExistsID(this.Item!.GetDescriptor()!.ID, null, null);
        Check.That(r.HasValue).IsTrue();
        Check.That(r.Value).IsTrue();
    }

    [Test] [NonParallelizable] [Order(103)]
    public async Task Remove()
    {
        Check.That(await this.CatalogDB!.Remove(this.Item!.GetDescriptor(), null, null)).IsTrue();
        var r = await this.CatalogDB!.Exists(this.Item!.GetDescriptor(), null, null);
        Check.That(r.HasValue).IsTrue();
        Check.That(r.Value).IsFalse();
    }

    [Test] [NonParallelizable] [Order(104)]
    public async Task RemoveID()
    {
        Check.That(this.Item!.SetID(Guid.NewGuid())).IsTrue();
        Check.That(await this.CatalogDB!.AddUpdate(this.Item!.GetDescriptor(), null, null)).IsTrue();
        Check.That(await this.CatalogDB!.RemoveID(this.Item!.GetDescriptor()!.ID, null, null)).IsTrue();
        var r = await this.CatalogDB!.ExistsID(this.Item!.GetDescriptor()!.ID, null, null);
        Check.That(r.HasValue).IsTrue();
        Check.That(r.Value).IsFalse();
    }
}