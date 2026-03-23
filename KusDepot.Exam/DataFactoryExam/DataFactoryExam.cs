namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable]
public class DataFactoryExam
{
    [Test]
    public void CreateTextItem()
    {
        var text = TestCaseDataGenerator.GenerateUnicodeString(1024);
        var id = Guid.NewGuid();

        var item = DataFactory.CreateTextItem(text, null, id, "txt-name", new[] { "note-a" }, new[] { "tag-a" }, "TXT");

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(text);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public void CreateTextItemAction()
    {
        var text = TestCaseDataGenerator.GenerateUnicodeString(256);
        var id = Guid.NewGuid();
        var name = "txt-action";

        var item = DataFactory.CreateTextItem(t =>
        {
            Check.That(t.SetContent(text)).IsTrue();
            Check.That(t.SetID(id)).IsTrue();
            Check.That(t.SetName(name)).IsTrue();
            t.AddNotes(new[] { "action-note" });
            t.AddTags(new[] { "action-tag" });
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(text);
        Check.That(item.GetID()).IsEqualTo(id);
        Check.That(item.GetName()).IsEqualTo(name);
        Check.That(item.GetNotes()).Contains("action-note");
        Check.That(item.GetTags()).Contains("action-tag");
    }

    [Test]
    public async Task CreateTextItemActionAsync()
    {
        var text = TestCaseDataGenerator.GenerateUnicodeString(128);
        var id = Guid.NewGuid();
        var name = "txt-action-async";

        var item = await DataFactory.CreateTextItemAsync(async t =>
        {
            await Task.Yield();
            Check.That(t.SetContent(text)).IsTrue();
            Check.That(t.SetID(id)).IsTrue();
            Check.That(t.SetName(name)).IsTrue();
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(text);
        Check.That(item.GetID()).IsEqualTo(id);
        Check.That(item.GetName()).IsEqualTo(name);
    }

    [Test]
    public void CreateCodeItem()
    {
        var code = TestCaseDataGenerator.GenerateUnicodeString(1024);
        var id = Guid.NewGuid();

        var item = DataFactory.CreateCodeItem(code, null, id, "code-name", new[] { "note-b" }, new[] { "tag-b" }, "SRC");

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(code);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public void CreateCodeItemAction()
    {
        var code = TestCaseDataGenerator.GenerateUnicodeString(128);
        var id = Guid.NewGuid();

        var item = DataFactory.CreateCodeItem(c =>
        {
            Check.That(c.SetContent(code)).IsTrue();
            Check.That(c.SetID(id)).IsTrue();
            c.SetDataType(DataTypes.PY);
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(code);
        Check.That(item.GetID()).IsEqualTo(id);
        Check.That(item.GetDataType()).IsEqualTo(DataTypes.PY);
    }

    [Test]
    public async Task CreateCodeItemActionAsync()
    {
        var code = TestCaseDataGenerator.GenerateUnicodeString(64);
        var id = Guid.NewGuid();

        var item = await DataFactory.CreateCodeItemAsync(async c =>
        {
            await Task.Delay(1);
            Check.That(c.SetContent(code)).IsTrue();
            Check.That(c.SetID(id)).IsTrue();
            c.SetDataType(DataTypes.JSON);
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(code);
        Check.That(item.GetID()).IsEqualTo(id);
        Check.That(item.GetDataType()).IsEqualTo(DataTypes.JSON);
    }

    [Test]
    public void CreateBinaryItem()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new Byte[2048];
        rng.GetBytes(bytes);
        var id = Guid.NewGuid();

        var item = DataFactory.CreateBinaryItem(bytes, null, null, id, "bin-name", new[] { "note-c" }, new[] { "tag-c" }, "BIN");

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(bytes);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public void CreateBinaryItemAction()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new Byte[512];
        rng.GetBytes(bytes);
        var id = Guid.NewGuid();

        var item = DataFactory.CreateBinaryItem(b =>
        {
            Check.That(b.SetContent(bytes)).IsTrue();
            Check.That(b.SetID(id)).IsTrue();
            b.SetName("bin-action");
            b.AddTags(new[] { "bin-action-tag" });
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(bytes);
        Check.That(item.GetID()).IsEqualTo(id);
        Check.That(item.GetName()).IsEqualTo("bin-action");
        Check.That(item.GetTags()).Contains("bin-action-tag");
    }

    [Test]
    public async Task CreateBinaryItemActionAsync()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new Byte[256];
        rng.GetBytes(bytes);
        var id = Guid.NewGuid();

        var item = await DataFactory.CreateBinaryItemAsync(async b =>
        {
            await Task.Yield();
            Check.That(b.SetContent(bytes)).IsTrue();
            Check.That(b.SetID(id)).IsTrue();
            b.SetName("bin-action-async");
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(bytes);
        Check.That(item.GetID()).IsEqualTo(id);
        Check.That(item.GetName()).IsEqualTo("bin-action-async");
    }

    [Test]
    public void CreateMultiMediaItem()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new Byte[1024];
        rng.GetBytes(bytes);
        var id = Guid.NewGuid();

        var artists = new[] { "artist-1", "artist-2" };
        var title = "media-title";
        var year = DateTime.UtcNow.Year;

        var item = DataFactory.CreateMultiMediaItem(bytes, null, id, "mm-name", artists, new[] { "note-d" }, new[] { "tag-d" }, title, "MM", year);

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(bytes);
        Check.That(item.GetArtists()).ContainsExactly(artists);
        Check.That(item.GetTitle()).IsEqualTo(title);
        Check.That(item.GetYear()).IsEqualTo(year);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public void CreateMultiMediaItemAction()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new Byte[256];
        rng.GetBytes(bytes);
        var id = Guid.NewGuid();
        var artists = new[] { "a-1", "a-2" };
        var title = "mm-action";
        var year = 1999;

        var item = DataFactory.CreateMultiMediaItem(m =>
        {
            Check.That(m.SetContent(bytes)).IsTrue();
            Check.That(m.SetID(id)).IsTrue();
            Check.That(m.SetArtists(artists)).IsTrue();
            Check.That(m.SetTitle(title)).IsTrue();
            Check.That(m.SetYear(year)).IsTrue();
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(bytes);
        Check.That(item.GetArtists()).ContainsExactly(artists);
        Check.That(item.GetTitle()).IsEqualTo(title);
        Check.That(item.GetYear()).IsEqualTo(year);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public async Task CreateMultiMediaItemActionAsync()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new Byte[128];
        rng.GetBytes(bytes);
        var id = Guid.NewGuid();
        var artists = new[] { "async-a-1", "async-a-2" };
        var title = "mm-action-async";
        var year = 2001;

        var item = await DataFactory.CreateMultiMediaItemAsync(async m =>
        {
            await Task.Delay(1);
            Check.That(m.SetContent(bytes)).IsTrue();
            Check.That(m.SetID(id)).IsTrue();
            Check.That(m.SetArtists(artists)).IsTrue();
            Check.That(m.SetTitle(title)).IsTrue();
            Check.That(m.SetYear(year)).IsTrue();
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(bytes);
        Check.That(item.GetArtists()).ContainsExactly(artists);
        Check.That(item.GetTitle()).IsEqualTo(title);
        Check.That(item.GetYear()).IsEqualTo(year);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public void CreateGuidReferenceItem()
    {
        var guid = Guid.NewGuid();
        var id = Guid.NewGuid();

        var item = DataFactory.CreateGuidReferenceItem(guid, null, id, "guid-ref", new[] { "note-e" }, new[] { "tag-e" });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(guid);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public void CreateGuidReferenceItemAction()
    {
        var guid = Guid.NewGuid();
        var id = Guid.NewGuid();

        var item = DataFactory.CreateGuidReferenceItem(g =>
        {
            Check.That(g.SetContent(guid)).IsTrue();
            Check.That(g.SetID(id)).IsTrue();
            g.SetName("guid-action");
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(guid);
        Check.That(item.GetID()).IsEqualTo(id);
        Check.That(item.GetName()).IsEqualTo("guid-action");
    }

    [Test]
    public async Task CreateGuidReferenceItemActionAsync()
    {
        var guid = Guid.NewGuid();
        var id = Guid.NewGuid();

        var item = await DataFactory.CreateGuidReferenceItemAsync(async g =>
        {
            await Task.Yield();
            Check.That(g.SetContent(guid)).IsTrue();
            Check.That(g.SetID(id)).IsTrue();
            g.SetName("guid-action-async");
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(guid);
        Check.That(item.GetID()).IsEqualTo(id);
        Check.That(item.GetName()).IsEqualTo("guid-action-async");
    }

    [Test]
    public void CreateMSBuildItem()
    {
        var content = TestCaseDataGenerator.GenerateUnicodeString(512);
        var id = Guid.NewGuid();

        var item = DataFactory.CreateMSBuildItem(content, null, id, "msb", new[] { "note-f" }, new[] { "tag-f" });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(content);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public void CreateMSBuildItemAction()
    {
        var content = TestCaseDataGenerator.GenerateUnicodeString(64);
        var id = Guid.NewGuid();

        var item = DataFactory.CreateMSBuildItem(m =>
        {
            Check.That(m.SetContent(content)).IsTrue();
            Check.That(m.SetID(id)).IsTrue();
            m.SetName("msb-action");
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(content);
        Check.That(item.GetID()).IsEqualTo(id);
        Check.That(item.GetName()).IsEqualTo("msb-action");
    }

    [Test]
    public async Task CreateMSBuildItemActionAsync()
    {
        var content = TestCaseDataGenerator.GenerateUnicodeString(32);
        var id = Guid.NewGuid();

        var item = await DataFactory.CreateMSBuildItemAsync(async m =>
        {
            await Task.Delay(1);
            Check.That(m.SetContent(content)).IsTrue();
            Check.That(m.SetID(id)).IsTrue();
            m.SetName("msb-action-async");
        });

        Check.That(item).IsNotNull();
        Check.That(item!.GetContent()).IsEqualTo(content);
        Check.That(item.GetID()).IsEqualTo(id);
        Check.That(item.GetName()).IsEqualTo("msb-action-async");
    }

    [Test]
    public void CreateDataSetItem()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new Byte[128];
        rng.GetBytes(bytes);

        var t = DataFactory.CreateTextItem("alpha", null, Guid.NewGuid(), "t", null, null, "TXT")!;
        var b = DataFactory.CreateBinaryItem(bytes, null, null, Guid.NewGuid(), "b", null, null, "BIN")!;

        var id = Guid.NewGuid();
        var item = DataFactory.CreateDataSetItem(new DataItem[] { t, b }, null, id, "ds", new[] { "note-g" }, new[] { "tag-g" }, "SET");

        Check.That(item).IsNotNull();
        var set = item!.GetContent();
        Check.That(set).IsNotNull();
        Check.That(set!.Count).IsEqualTo(2);
        Check.That(set.Select(i => i.GetID()).ToHashSet())
            .IsEqualTo(new HashSet<Guid?> { t.GetID(), b.GetID() });
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public void CreateDataSetItemAction()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new Byte[64];
        rng.GetBytes(bytes);

        var t = DataFactory.CreateTextItem("beta", null, Guid.NewGuid(), "t2", null, null, "TXT")!;
        var b = DataFactory.CreateBinaryItem(bytes, null, null, Guid.NewGuid(), "b2", null, null, "BIN")!;
        var id = Guid.NewGuid();

        var item = DataFactory.CreateDataSetItem(ds =>
        {
            ds.SetContent(new DataItem[] { t, b });
            ds.SetID(id);
            ds.SetName("ds-action");
        });

        Check.That(item).IsNotNull();
        var set = item!.GetContent();
        Check.That(set).IsNotNull();
        Check.That(set!.Count).IsEqualTo(2);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public async Task CreateDataSetItemActionAsync()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new Byte[32];
        rng.GetBytes(bytes);

        var t = DataFactory.CreateTextItem("beta-async", null, Guid.NewGuid(), "t3", null, null, "TXT")!;
        var b = DataFactory.CreateBinaryItem(bytes, null, null, Guid.NewGuid(), "b3", null, null, "BIN")!;
        var id = Guid.NewGuid();

        var item = await DataFactory.CreateDataSetItemAsync(async ds =>
        {
            await Task.Delay(1);
            ds.SetContent(new DataItem[] { t, b });
            ds.SetID(id);
        });

        Check.That(item).IsNotNull();
        var set = item!.GetContent();
        Check.That(set).IsNotNull();
        Check.That(set!.Count).IsEqualTo(2);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public void CreateGenericItem()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new Byte[16];
        rng.GetBytes(bytes);

        var content = new Object[] {42, "str", bytes };
        var id = Guid.NewGuid();

        var item = DataFactory.CreateGenericItem(content, null, id, "gen", new[] { "note-h" }, new[] { "tag-h" }, "GEN");

        Check.That(item).IsNotNull();
        var list = item!.GetContent();
        Check.That(list).IsNotNull();
        Check.That(list!.Count).IsEqualTo(content.Length);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public void CreateGenericItemAction()
    {
        var content = new Object[] { 1, "two", 3.0m };
        var id = Guid.NewGuid();

        var item = DataFactory.CreateGenericItem(g =>
        {
            g.SetContent(content);
            g.SetID(id);
        });

        Check.That(item).IsNotNull();
        var list = item!.GetContent();
        Check.That(list).IsNotNull();
        Check.That(list!.Count).IsEqualTo(content.Length);
        Check.That(item.GetID()).IsEqualTo(id);
    }

    [Test]
    public async Task CreateGenericItemActionAsync()
    {
        var content = new Object[] { Guid.NewGuid(), 5, "value" };
        var id = Guid.NewGuid();

        var item = await DataFactory.CreateGenericItemAsync(async g =>
        {
            await Task.Yield();
            g.SetContent(content);
            g.SetID(id);
        });

        Check.That(item).IsNotNull();
        var list = item!.GetContent();
        Check.That(list).IsNotNull();
        Check.That(list!.Count).IsEqualTo(content.Length);
        Check.That(item.GetID()).IsEqualTo(id);
    }
}