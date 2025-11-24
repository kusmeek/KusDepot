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
}