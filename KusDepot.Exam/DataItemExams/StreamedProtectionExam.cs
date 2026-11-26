namespace KusDepot.Exams.DataItems;

[TestFixture]
public class StreamedProtectionExam
{
    private static readonly String[] FileBackedTypes = ["TextItem","CodeItem","BinaryItem","MultiMediaItem","GenericItem","DataSetItem","MSBuildItem"];

    private static readonly (String Type, String Field)[] ResidentFields =
    [
        ("TextItem","Content"), ("CodeItem","Content"), ("MultiMediaItem","Content"),
        ("BinaryItem","Content"), ("BinaryItem","Commands"), ("BinaryItem","Services"),
        ("GenericItem","Content"), ("GenericItem","CachedContent"),
        ("DataSetItem","Content"), ("DataSetItem","ContentIndex"),
        ("MSBuildItem","Content"), ("MSBuildItem","Requirements"), ("MSBuildItem","Sequence"),
        ("DataStreamItem","Content")
    ];

    private static IEnumerable<TestCaseData> ResidentFieldCases() => ResidentFields.Select(value => new TestCaseData(value.Type,value.Field));

    private static DataItem CreateItem(String type) => type switch
    {
        "TextItem" => new TextItem(),
        "CodeItem" => new CodeItem(),
        "BinaryItem" => new BinaryItem(),
        "MultiMediaItem" => new MultiMediaItem(),
        "GenericItem" => new GenericItem(),
        "DataSetItem" => new DataSetItem(),
        "MSBuildItem" => new MSBuildItem(),
        "DataStreamItem" => new DataStreamItem(),
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };

    private static FieldInfo GetField(DataItem item , String name) => item.GetType().GetField(name,BindingFlags.Instance|BindingFlags.NonPublic) ?? throw new InvalidOperationException($"{item.GetType().Name}.{name} not found.");

    private static Object ResidentValue(String type , String field) => (type,field) switch
    {
        ("TextItem" or "CodeItem" or "MSBuildItem","Content") => "resident content",
        ("BinaryItem" or "MultiMediaItem","Content") => new Byte[] { 9,8,7 },
        ("BinaryItem","Commands") => new HashSet<CommandDescriptor>(),
        ("BinaryItem","Services") => new HashSet<ToolDescriptor>(),
        ("GenericItem","Content") => new List<Object> { "resident" },
        ("GenericItem","CachedContent") => ImmutableList.Create<Object>("cached"),
        ("DataSetItem","Content") => new HashSet<DataItem> { new TextItem("resident") },
        ("DataSetItem","ContentIndex") => new Dictionary<Guid,DataItem> { [Guid.NewGuid()] = new TextItem("cached") },
        ("MSBuildItem","Requirements") => new HashSet<DataItem>(),
        ("MSBuildItem","Sequence") => new SortedList<Int32,MSBuildItem>(),
        ("DataStreamItem","Content") => new MemoryStream([9,8,7]),
        _ => throw new ArgumentOutOfRangeException(nameof(field))
    };

    [TestCaseSource(nameof(ResidentFieldCases))]
    public async Task StreamedEncryption_ResidentField_RejectsWithoutMutation(String type , String fieldName)
    {
        DataItem item = CreateItem(type);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        String directory = Path.Combine(Path.GetTempPath(),$"KusDepot-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        String file = Path.Combine(directory,"content.bin");
        Byte[] bytes = [1,2,3,4];
        await File.WriteAllBytesAsync(file,bytes);
        Object resident = ResidentValue(type,fieldName);

        try
        {
            Assert.That(item.SetFILE(file),Is.True);
            Assert.That(await item.SetContentStreamed(true),Is.True);
            FieldInfo field = GetField(item,fieldName);
            field.SetValue(item,resident);
            Assert.That(await item.SignData("Content",context),Is.Not.Null);
            Assert.That(await item.VerifyData("Content",context),Is.True);

            Assert.That(await item.EncryptData(context),Is.False);
            Assert.That(item.GetDataEncrypted(),Is.False);
            Assert.That(item.GetDataProtectionInfo(),Is.Null);
            Assert.That(item.GetContentStreamed(),Is.True);
            Assert.That(field.GetValue(item),Is.SameAs(resident));
            Assert.That(File.ReadAllBytes(file),Is.EqualTo(bytes));
            Assert.That(Directory.GetFiles(directory),Has.Length.EqualTo(1));
            Assert.That(await item.VerifyData("Content",context),Is.True);
        }
        finally
        {
            (resident as IDisposable)?.Dispose();
            Directory.Delete(directory,true);
        }
    }

    [TestCaseSource(nameof(FileBackedTypes))]
    [TestCase("DataStreamItem")]
    public async Task StreamedEncryption_FileOnly_RoundTrips(String type)
    {
        DataItem item = CreateItem(type);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        String file = Path.GetTempFileName();
        Byte[] bytes = [1,2,3,4];
        await File.WriteAllBytesAsync(file,bytes);
        try
        {
            Assert.That(item.SetFILE(file),Is.True);
            Assert.That(await item.SetContentStreamed(true),Is.True);
            Assert.That(await item.EncryptData(context),Is.True);
            Assert.That(item.GetDataEncrypted(),Is.True);
            Assert.That(File.ReadAllBytes(file),Is.Not.EqualTo(bytes));
            Assert.That(await item.SetContentStreamed(false),Is.False);
            Assert.That(item.GetContentStreamed(),Is.True);
            Assert.That(await item.DecryptData(context),Is.True);
            Assert.That(item.GetContentStreamed(),Is.True);
            Assert.That(File.ReadAllBytes(file),Is.EqualTo(bytes));
        }
        finally { File.Delete(file); }
    }

    [TestCaseSource(nameof(FileBackedTypes))]
    public async Task SwitchBack_SelectsExistingResidentWithoutReadingFile(String type)
    {
        DataItem item = CreateItem(type);
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        String file = Path.GetTempFileName();
        Byte[] bytes = [1,2,3,4];
        await File.WriteAllBytesAsync(file,bytes);
        try
        {
            Assert.That(item.SetFILE(file),Is.True);
            Assert.That(await item.SetContentStreamed(true,context),Is.True);
            Assert.That(await item.VerifyData("Content",context),Is.True);
            FieldInfo field = GetField(item,"Content");
            Assert.That(field.GetValue(item),Is.Null);
            Assert.That(await item.SetContentStreamed(false,context),Is.True);
            Assert.That(item.GetContentStreamed(),Is.False);
            Assert.That(field.GetValue(item),Is.Null);
            Assert.That(File.ReadAllBytes(file),Is.EqualTo(bytes));
            FieldInfo assertionsField = typeof(DataItem).GetField("FieldAssertions",BindingFlags.Instance|BindingFlags.NonPublic) ?? throw new InvalidOperationException("FieldAssertions not found.");
            Dictionary<String,Byte[]>? assertions = (Dictionary<String,Byte[]>?)assertionsField.GetValue(item);
            Assert.That(assertions is null || !assertions.ContainsKey("Content"),Is.True);
            Assert.That(await item.SetContentStreamed(true),Is.True);
            Object resident = ResidentValue(type,"Content");
            field.SetValue(item,resident);
            Assert.That(await item.SetContentStreamed(false,context),Is.True);
            Assert.That(field.GetValue(item),Is.SameAs(resident));
            Assert.That(await item.VerifyData("Content",context),Is.True);
            Assert.That(File.ReadAllBytes(file),Is.EqualTo(bytes));
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task SwitchBack_AssertionFailure_RestoresStreamedState()
    {
        TextItem item = new("resident");
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        String file = Path.GetTempFileName();
        Byte[] bytes = [1,2,3,4];
        await File.WriteAllBytesAsync(file,bytes);
        try
        {
            Assert.That(item.SetFILE(file),Is.True);
            Assert.That(await item.SetContentStreamed(true,context),Is.True);
            using X509Certificate2 certificate = X509CertificateLoader.LoadCertificate(context.SigningObject.Certificate.Export(X509ContentType.Cert));
            DataItemSecurityContext invalid = context with { SigningObject = new DataSecurityObject(context.SigningObject.ObjectId,certificate) };
            Assert.That(await item.SetContentStreamed(false,invalid),Is.False);
            Assert.That(item.GetContentStreamed(),Is.True);
            Assert.That(item.GetContent(),Is.EqualTo("resident"));
            Assert.That(File.ReadAllBytes(file),Is.EqualTo(bytes));
            Assert.That(await item.VerifyData("Content",context),Is.True);
        }
        finally { File.Delete(file); }
    }

    [Test]
    public async Task EncryptedFile_RejectsDataStreamMutations()
    {
        DataStreamItem item = new();
        DataItemSecurityContext context = DataItemSecurityContextExamHelpers.CreateContext(item);
        String file = Path.GetTempFileName();
        Byte[] bytes = [1,2,3,4];
        await File.WriteAllBytesAsync(file,bytes);
        using MemoryStream stream = new([9,8,7]);
        try
        {
            Assert.That(item.SetFILE(file),Is.True);
            Assert.That(await item.EncryptData(context),Is.True);
            Byte[] encrypted = File.ReadAllBytes(file);
            Assert.That(item.SetContent(stream),Is.False);
            Assert.That(item.SetContent(null),Is.False);
            Assert.That(item.BindLiveContent(stream),Is.False);
            Assert.That(await item.SetContentStreamed(false),Is.False);
            Assert.That(await item.SetContentStreamed(true),Is.False);
            Assert.That(stream.CanRead,Is.True);
            Assert.That(item.GetContentStreamed(),Is.True);
            Assert.That(File.ReadAllBytes(file),Is.EqualTo(encrypted));
            Assert.That(await item.DecryptData(context),Is.True);
            Assert.That(File.ReadAllBytes(file),Is.EqualTo(bytes));
        }
        finally { File.Delete(file); }
    }
}
