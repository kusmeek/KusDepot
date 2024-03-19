namespace KusDepot.Exams;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ArkExam
{
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
        MultiMediaItem _9 = new MultiMediaItem(); _9.SetID(Guid.Empty);

        Check.That(_0.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_8.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_9.ToString())).IsFalse();

        Check.That(_0.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_8.ToString())).IsTrue();

        Check.That(_0.Tables[Ark.ElementsTableName]!.Rows.Count).IsEqualTo(8);
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
        MultiMediaItem _9 = new MultiMediaItem(); _9.SetID(Guid.Empty);

        Check.That(_0.AddUpdate(_1.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_2.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_3.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_4.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_5.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_6.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_7.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_8.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_9.GetDescriptor())).IsFalse();

        Check.That(_0.AddUpdate(_1.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_2.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_3.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_4.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_5.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_6.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_7.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_8.GetDescriptor())).IsTrue();

        Check.That(_0.Tables[Ark.ElementsTableName]!.Rows.Count).IsEqualTo(8);
    }

    [OneTimeSetUp]
    public void Calibrate() { }

    [Test]
    public void Constructor()
    {
        Ark _0 = new Ark(0);
        Ark _10 = new Ark(10);
        Ark _11 = new Ark(1);
        Ark __ = new Ark(-1);
        Tool _1 = new Tool();
        GuidReferenceItem _2 = new GuidReferenceItem();
        CodeItem _3 = new CodeItem();
        TextItem _4 = new TextItem();
        GenericItem _5 = new GenericItem();
        BinaryItem _6 = new BinaryItem();
        MSBuildItem _7 = new MSBuildItem();
        MultiMediaItem _8 = new MultiMediaItem();

        Check.That(_0.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_8.ToString())).IsTrue();

        Check.That(_0.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_8.ToString())).IsTrue();

        Check.That(_0.Tables[Ark.ElementsTableName]!.Rows.Count).IsEqualTo(8);

        Check.That(_10.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_8.ToString())).IsTrue();

        Check.That(_10.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_10.AddUpdate(_8.ToString())).IsTrue();

        Check.That(_10.Tables[Ark.ElementsTableName]!.Rows.Count).IsEqualTo(8);

        Check.That(_11.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_8.ToString())).IsTrue();

        Check.That(_11.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_11.AddUpdate(_8.ToString())).IsTrue();

        Check.That(_11.Tables[Ark.ElementsTableName]!.Rows.Count).IsEqualTo(8);

        Check.That(__.AddUpdate(_1.ToString())).IsTrue();
        Check.That(__.AddUpdate(_2.ToString())).IsTrue();
        Check.That(__.AddUpdate(_3.ToString())).IsTrue();
        Check.That(__.AddUpdate(_4.ToString())).IsTrue();
        Check.That(__.AddUpdate(_5.ToString())).IsTrue();
        Check.That(__.AddUpdate(_6.ToString())).IsTrue();
        Check.That(__.AddUpdate(_7.ToString())).IsTrue();
        Check.That(__.AddUpdate(_8.ToString())).IsTrue();

        Check.That(__.AddUpdate(_1.ToString())).IsTrue();
        Check.That(__.AddUpdate(_2.ToString())).IsTrue();
        Check.That(__.AddUpdate(_3.ToString())).IsTrue();
        Check.That(__.AddUpdate(_4.ToString())).IsTrue();
        Check.That(__.AddUpdate(_5.ToString())).IsTrue();
        Check.That(__.AddUpdate(_6.ToString())).IsTrue();
        Check.That(__.AddUpdate(_7.ToString())).IsTrue();
        Check.That(__.AddUpdate(_8.ToString())).IsTrue();

        Check.That(__.Tables[Ark.ElementsTableName]!.Rows.Count).IsEqualTo(8);
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
    public void GetBytes()
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
        _9.Add(DataType.PPTX);

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

        Byte[] _12 = Ark.GetBytes(_0);

        MemoryStream _13 = new MemoryStream(_12);

        DataSet _10 = new DataSet();

        _10.ReadXml(_13);

        Check.That(_10.Tables[Ark.ElementsTableName]!.Rows.Count).IsEqualTo(8);
    }

    [Test]
    public void Parse()
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
        _9.Add(DataType.PPTX);

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

        Ark? _12 = Ark.Parse(Ark.GetBytes(_0));

        Check.That(_12!.Tables[Ark.ElementsTableName]!.Rows.Count).IsEqualTo(8);

        Check.That(Ark.GetBytes(_0).SequenceEqual(Ark.GetBytes(_12))).IsTrue();
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

        Check.That(_0.AddUpdate(_1.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_2.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_3.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_4.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_5.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_6.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_7.ToString())).IsTrue();
        Check.That(_0.AddUpdate(_8.ToString())).IsTrue();

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

        Check.That(_0.Tables[Ark.ElementsTableName]!.Rows.Count).IsEqualTo(0);
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

        Check.That(_0.AddUpdate(_1.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_2.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_3.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_4.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_5.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_6.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_7.GetDescriptor())).IsTrue();
        Check.That(_0.AddUpdate(_8.GetDescriptor())).IsTrue();

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

        Check.That(_0.Tables[Ark.ElementsTableName]!.Rows.Count).IsEqualTo(0);
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

    [Test]
    public void TryParse()
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
        _9.Add(DataType.PPTX);

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

        Check.That(Ark.TryParse(Ark.GetBytes(_0), out Ark? _12)).IsTrue();

        Check.That(_12!.Tables[Ark.ElementsTableName]!.Rows.Count).IsEqualTo(8);

        Check.That(Ark.GetBytes(_0).SequenceEqual(Ark.GetBytes(_12))).IsTrue();
    }
}