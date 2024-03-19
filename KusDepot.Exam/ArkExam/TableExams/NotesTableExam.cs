namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class NotesTableExam
{
    [OneTimeSetUp]
    public void Calibrate() { }

    [Test]
    public void AddUpdate()
    {
        Ark _0 = new Ark();
        Tool _1 = new Tool();
        GuidReferenceItem _2 = new GuidReferenceItem();
        CodeItem _3 = new CodeItem();
        TextItem _4 = new TextItem();
        GenericItem _5 = new GenericItem();
        BinaryItem _6 = new BinaryItem();
        MSBuildItem _7 = new MSBuildItem();
        MultiMediaItem _8 = new MultiMediaItem();
        HashSet<String> _9 = new HashSet<String>();

        _9.Add(UsageType.Architecture);
        _9.Add(UsageType.Model);

        _1.AddNotes(_9); _1.AddTags(_9);
        _2.AddNotes(_9); _2.AddTags(_9);
        _3.AddNotes(_9); _3.AddTags(_9);
        _4.AddNotes(_9); _4.AddTags(_9);
        _5.AddNotes(_9); _5.AddTags(_9);
        _6.AddNotes(_9); _6.AddTags(_9);
        _7.AddNotes(_9); _7.AddTags(_9);
        _8.AddNotes(_9); _8.AddTags(_9);

        Check.That(_0.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_8.ToString())).IsTrue();

        _9.Add(UsageType.Network);

        _1.AddNotes(_9); _1.AddTags(_9);
        _2.AddNotes(_9); _2.AddTags(_9);
        _3.AddNotes(_9); _3.AddTags(_9);
        _4.AddNotes(_9); _4.AddTags(_9);
        _5.AddNotes(_9); _5.AddTags(_9);
        _6.AddNotes(_9); _6.AddTags(_9);
        _7.AddNotes(_9); _7.AddTags(_9);
        _8.AddNotes(_9); _8.AddTags(_9);

        Check.That(_0.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_8.ToString())).IsTrue();

        Check.That(_0.Tables[Ark.NotesTableName]!.Rows.Count).IsEqualTo(8);
    }

    [Test]
    public void AddUpdateDescriptor()
    {
        Ark _0 = new Ark();
        Tool _1 = new Tool();
        GuidReferenceItem _2 = new GuidReferenceItem();
        CodeItem _3 = new CodeItem();
        TextItem _4 = new TextItem();
        GenericItem _5 = new GenericItem();
        BinaryItem _6 = new BinaryItem();
        MSBuildItem _7 = new MSBuildItem();
        MultiMediaItem _8 = new MultiMediaItem();
        HashSet<String> _9 = new HashSet<String>();

        _9.Add(UsageType.Architecture);
        _9.Add(UsageType.Model);

        _1.AddNotes(_9); _1.AddTags(_9);
        _2.AddNotes(_9); _2.AddTags(_9);
        _3.AddNotes(_9); _3.AddTags(_9);
        _4.AddNotes(_9); _4.AddTags(_9);
        _5.AddNotes(_9); _5.AddTags(_9);
        _6.AddNotes(_9); _6.AddTags(_9);
        _7.AddNotes(_9); _7.AddTags(_9);
        _8.AddNotes(_9); _8.AddTags(_9);

        Check.That(_0.AddUpdate(_1.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_2.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_3.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_4.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_5.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_6.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_7.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_8.GetDescriptor())).IsTrue();

        _9.Add(UsageType.Network);

        _1.AddNotes(_9); _1.AddTags(_9);
        _2.AddNotes(_9); _2.AddTags(_9);
        _3.AddNotes(_9); _3.AddTags(_9);
        _4.AddNotes(_9); _4.AddTags(_9);
        _5.AddNotes(_9); _5.AddTags(_9);
        _6.AddNotes(_9); _6.AddTags(_9);
        _7.AddNotes(_9); _7.AddTags(_9);
        _8.AddNotes(_9); _8.AddTags(_9);

        Check.That(_0.AddUpdate(_1.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_2.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_3.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_4.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_5.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_6.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_7.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_8.GetDescriptor())).IsTrue();

        Check.That(_0.Tables[Ark.NotesTableName]!.Rows.Count).IsEqualTo(8);
    }

    [Test]
    public void Exists()
    {
        Ark _0 = new Ark();
        Tool _1 = new Tool();
        CodeItem _2 = new CodeItem();
        MultiMediaItem _3 = new MultiMediaItem();

        Check.That(_0.AddUpdate(_1.GetDescriptor())).IsTrue();
        Check.That(_0.Exists(_1.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(Guid.NewGuid())).IsEqualTo(false);
        Check.That(_0.Remove(_1.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(_1.GetID())).IsEqualTo(false);

        Check.That(_0.AddUpdate(_2.GetDescriptor())).IsTrue();
        Check.That(_0.Exists(_2.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(Guid.NewGuid())).IsEqualTo(false);
        Check.That(_0.Remove(_2.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(_2.GetID())).IsEqualTo(false);

        Check.That(_0.AddUpdate(_3.GetDescriptor())).IsTrue();
        Check.That(_0.Exists(_3.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(Guid.NewGuid())).IsEqualTo(false);
        Check.That(_0.Remove(_3.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(_3.GetID())).IsEqualTo(false);
    }

    [Test]
    public void Remove()
    {
        Ark _0 = new Ark();
        Tool _1 = new Tool();
        GuidReferenceItem _2 = new GuidReferenceItem();
        CodeItem _3 = new CodeItem();
        TextItem _4 = new TextItem();
        GenericItem _5 = new GenericItem();
        BinaryItem _6 = new BinaryItem();
        MSBuildItem _7 = new MSBuildItem();
        MultiMediaItem _8 = new MultiMediaItem();
        HashSet<String> _9 = new HashSet<String>();

        _9.Add(UsageType.Architecture);
        _9.Add(UsageType.Model);

        _1.AddNotes(_9); _1.AddTags(_9);
        _2.AddNotes(_9); _2.AddTags(_9);
        _3.AddNotes(_9); _3.AddTags(_9);
        _4.AddNotes(_9); _4.AddTags(_9);
        _5.AddNotes(_9); _5.AddTags(_9);
        _6.AddNotes(_9); _6.AddTags(_9);
        _7.AddNotes(_9); _7.AddTags(_9);
        _8.AddNotes(_9); _8.AddTags(_9);

        Check.That(_0.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_8.ToString())).IsTrue();

        _9.Add(UsageType.Network);

        _1.AddNotes(_9); _1.AddTags(_9);
        _2.AddNotes(_9); _2.AddTags(_9);
        _3.AddNotes(_9); _3.AddTags(_9);
        _4.AddNotes(_9); _4.AddTags(_9);
        _5.AddNotes(_9); _5.AddTags(_9);
        _6.AddNotes(_9); _6.AddTags(_9);
        _7.AddNotes(_9); _7.AddTags(_9);
        _8.AddNotes(_9); _8.AddTags(_9);

        Check.That(_0.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_8.ToString())).IsTrue();

        Check.That(_0.Remove(_1.ToString())).IsTrue();
        Check.That(_0.Remove(_2.ToString())).IsTrue();
        Check.That(_0.Remove(_3.ToString())).IsTrue();
        Check.That(_0.Remove(_4.ToString())).IsTrue();
        Check.That(_0.Remove(_5.ToString())).IsTrue();
        Check.That(_0.Remove(_6.ToString())).IsTrue();
        Check.That(_0.Remove(_7.ToString())).IsTrue();
        Check.That(_0.Remove(_8.ToString())).IsTrue();

        Check.That(_0.Remove(_1.ToString())).IsTrue();
        Check.That(_0.Remove(_2.ToString())).IsTrue();
        Check.That(_0.Remove(_3.ToString())).IsTrue();
        Check.That(_0.Remove(_4.ToString())).IsTrue();
        Check.That(_0.Remove(_5.ToString())).IsTrue();
        Check.That(_0.Remove(_6.ToString())).IsTrue();
        Check.That(_0.Remove(_7.ToString())).IsTrue();
        Check.That(_0.Remove(_8.ToString())).IsTrue();

        Check.That(_0.Tables[Ark.NotesTableName]!.Rows.Count).IsEqualTo(0);
    }

    [Test]
    public void RemoveDescriptor()
    {
        Ark _0 = new Ark();
        Tool _1 = new Tool();
        GuidReferenceItem _2 = new GuidReferenceItem();
        CodeItem _3 = new CodeItem();
        TextItem _4 = new TextItem();
        GenericItem _5 = new GenericItem();
        BinaryItem _6 = new BinaryItem();
        MSBuildItem _7 = new MSBuildItem();
        MultiMediaItem _8 = new MultiMediaItem();
        HashSet<String> _9 = new HashSet<String>();

        _9.Add(UsageType.Architecture);
        _9.Add(UsageType.Model);

        _1.AddNotes(_9); _1.AddTags(_9);
        _2.AddNotes(_9); _2.AddTags(_9);
        _3.AddNotes(_9); _3.AddTags(_9);
        _4.AddNotes(_9); _4.AddTags(_9);
        _5.AddNotes(_9); _5.AddTags(_9);
        _6.AddNotes(_9); _6.AddTags(_9);
        _7.AddNotes(_9); _7.AddTags(_9);
        _8.AddNotes(_9); _8.AddTags(_9);

        Check.That(_0.AddUpdate(_1.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_2.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_3.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_4.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_5.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_6.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_7.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_8.GetDescriptor())).IsTrue();

        _9.Add(UsageType.Network);

        _1.AddNotes(_9); _1.AddTags(_9);
        _2.AddNotes(_9); _2.AddTags(_9);
        _3.AddNotes(_9); _3.AddTags(_9);
        _4.AddNotes(_9); _4.AddTags(_9);
        _5.AddNotes(_9); _5.AddTags(_9);
        _6.AddNotes(_9); _6.AddTags(_9);
        _7.AddNotes(_9); _7.AddTags(_9);
        _8.AddNotes(_9); _8.AddTags(_9);

        Check.That(_0.AddUpdate(_1.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_2.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_3.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_4.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_5.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_6.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_7.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_8.GetDescriptor())).IsTrue();

        Check.That(_0.Remove(_1.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_2.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_3.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_4.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_5.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_6.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_7.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_8.GetDescriptor())).IsTrue();

        Check.That(_0.Remove(_1.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_2.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_3.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_4.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_5.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_6.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_7.GetDescriptor())).IsTrue();
        Check.That(_0.Remove(_8.GetDescriptor())).IsTrue();

        Check.That(_0.Tables[Ark.NotesTableName]!.Rows.Count).IsEqualTo(0);
    }

    [Test]
    public void RemoveGuid()
    {
        Ark _0 = new Ark();
        Tool _1 = new Tool();
        CodeItem _2 = new CodeItem();
        MultiMediaItem _3 = new MultiMediaItem();

        Check.That(_0.AddUpdate(_1.GetDescriptor())).IsTrue();
        Check.That(_0.Exists(_1.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(Guid.NewGuid())).IsEqualTo(false);
        Check.That(_0.Remove(_1.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(_1.GetID())).IsEqualTo(false);

        Check.That(_0.AddUpdate(_2.GetDescriptor())).IsTrue();
        Check.That(_0.Exists(_2.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(Guid.NewGuid())).IsEqualTo(false);
        Check.That(_0.Remove(_2.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(_2.GetID())).IsEqualTo(false);

        Check.That(_0.AddUpdate(_3.GetDescriptor())).IsTrue();
        Check.That(_0.Exists(_3.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(Guid.NewGuid())).IsEqualTo(false);
        Check.That(_0.Remove(_3.GetID())).IsEqualTo(true);
        Check.That(_0.Exists(_3.GetID())).IsEqualTo(false);
    }
}