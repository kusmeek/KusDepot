namespace KusDepot.Exams.DataItems;

[TestFixture] [Parallelizable]
public class DataPortExam
{
    [Test]
    public async Task Export()
    {
        using RandomNumberGenerator _0 = RandomNumberGenerator.Create();

        Byte[] _1 = new Byte[1024]; _0.GetBytes(_1);

        String _2 = TestCaseDataGenerator.GenerateUnicodeString(1024);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\TextItem.txt";

        File.Delete(_3); Check.That(File.Exists(_3)).IsFalse(); 

        Check.That(await DataPort.Export(new TextItem(_2),_3)).IsTrue();

        Check.That(File.Exists(_3)).IsTrue();
        Check.That(File.ReadAllBytes(_3)).IsEqualTo(new UTF8Encoding(false,true).GetBytes(_2));

        TextItem? _t = (TextItem?)await DataPort.Import(_3,i => i.SetName("Name"));

        Check.That(_t?.GetContent()).IsEqualTo(_2); Check.That(_t?.GetName()).IsEqualTo("Name");

        File.Delete(_3); Check.That(File.Exists(_3)).IsFalse();

        String _4 = Environment.GetEnvironmentVariable("TEMP") + "\\CodeItem.cpp";

        File.Delete(_4); Check.That(File.Exists(_4)).IsFalse(); 

        Check.That(await DataPort.Export(new CodeItem(_2),_4)).IsTrue();

        Check.That(File.Exists(_4)).IsTrue();
        Check.That(File.ReadAllBytes(_4)).IsEqualTo(new UTF8Encoding(false,true).GetBytes(_2));

        CodeItem? _c = (CodeItem?)await DataPort.Import(_4);

        Check.That(_c?.GetContent()).IsEqualTo(_2);

        File.Delete(_4); Check.That(File.Exists(_4)).IsFalse();

        String _5 = Environment.GetEnvironmentVariable("TEMP") + "\\MultiMediaItem.mp4";

        File.Delete(_5); Check.That(File.Exists(_5)).IsFalse(); 

        Check.That(await DataPort.Export(new MultiMediaItem(_1),_5)).IsTrue();

        Check.That(File.Exists(_5)).IsTrue();

        MultiMediaItem? _m = (MultiMediaItem?)await DataPort.Import(_5);

        Check.That(_m?.GetContent()).IsEqualTo(_1);

        File.Delete(_5); Check.That(File.Exists(_5)).IsFalse();

        String _6 = Environment.GetEnvironmentVariable("TEMP") + "\\BinaryItem.iso";

        File.Delete(_6); Check.That(File.Exists(_6)).IsFalse(); 

        Check.That(await DataPort.Export(new BinaryItem(_1),_6)).IsTrue();

        Check.That(File.Exists(_6)).IsTrue();

        BinaryItem? _b = (BinaryItem?)await DataPort.Import(_6);

        Check.That(_b?.GetContent()).IsEqualTo(_1);

        File.Delete(_6); Check.That(File.Exists(_6)).IsFalse();

        String _7 = Environment.GetEnvironmentVariable("TEMP") + "\\GuidReferenceItem2.guid";

        File.Delete(_7); Check.That(File.Exists(_7)).IsFalse(); 

        Guid _g = Guid.NewGuid();

        Check.That(await DataPort.Export(new GuidReferenceItem(_g),_7)).IsTrue();

        Check.That(File.Exists(_7)).IsTrue();

        GuidReferenceItem? _gri = (GuidReferenceItem?)await DataPort.Import(_7);

        Check.That(_gri?.GetContent()).IsEqualTo(_g);

        File.Delete(_7); Check.That(File.Exists(_7)).IsFalse();
    }

    [Test]
    public async Task Import()
    {
        using RandomNumberGenerator _0 = RandomNumberGenerator.Create();

        Byte[] _1 = new Byte[1024]; _0.GetBytes(_1);

        String _2 = TestCaseDataGenerator.GenerateUnicodeString(1024);

        String _3 = Environment.GetEnvironmentVariable("TEMP") + "\\TextItem2.txt";

        File.Delete(_3); Check.That(File.Exists(_3)).IsFalse(); File.WriteAllBytes(_3,Encoding.UTF8.GetBytes(_2));

        TextItem? _t = (TextItem?)await DataPort.Import(_3);

        Check.That(_t?.GetContent()).IsEqualTo(_2);

        File.Delete(_3); Check.That(File.Exists(_3)).IsFalse();

        String _4 = Environment.GetEnvironmentVariable("TEMP") + "\\CodeItem.c";

        File.Delete(_4); Check.That(File.Exists(_4)).IsFalse(); File.WriteAllBytes(_4,Encoding.UTF8.GetBytes(_2));

        CodeItem ? _c = (CodeItem?)await DataPort.Import(_4);

        Check.That(_c?.GetContent()).IsEqualTo(_2);

        File.Delete(_4); Check.That(File.Exists(_4)).IsFalse();

        String _5 = Environment.GetEnvironmentVariable("TEMP") + "\\MultiMediaItem.mp3";

        File.Delete(_5); Check.That(File.Exists(_5)).IsFalse(); File.WriteAllBytes(_5,_1);

        MultiMediaItem? _m = (MultiMediaItem?)await DataPort.Import(_5);

        Check.That(_m?.GetContent()).IsEqualTo(_1);

        File.Delete(_5); Check.That(File.Exists(_5)).IsFalse();

        String _6 = Environment.GetEnvironmentVariable("TEMP") + "\\BLOB197346825";

        File.Delete(_6); Check.That(File.Exists(_6)).IsFalse(); File.WriteAllBytes(_6,_1);

        BinaryItem? _b = (BinaryItem?)await DataPort.Import(_6);

        Check.That(_b?.GetContent()).IsEqualTo(_1);

        File.Delete(_6); Check.That(File.Exists(_6)).IsFalse();

        String _7 = Environment.GetEnvironmentVariable("TEMP") + "\\GuidReferenceItem4.guid";

        Guid _g = Guid.NewGuid();

        File.Delete(_7); Check.That(File.Exists(_7)).IsFalse(); File.WriteAllBytes(_7,_g.ToByteArray());

        GuidReferenceItem? _gr = (GuidReferenceItem?)await DataPort.Import(_7);

        Check.That(_gr?.GetContent()).IsEqualTo(_g);

        File.Delete(_7); Check.That(File.Exists(_7)).IsFalse();
    }

    [Test]
    public async Task Import_WithoutAuditPrivilegeStillCapturesAvailableSecurityDescriptor()
    {
        String path = Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".txt");
        String content = "Import without audit privilege";

        try
        {
            await File.WriteAllTextAsync(path,content).ConfigureAwait(false);
            TextItem? item = (TextItem?)await DataPort.Import(path).ConfigureAwait(false);

            Check.That(item?.GetContent()).IsEqualTo(content);
            if(OperatingSystem.IsWindows()) { Check.That(item?.GetSecurityDescriptor()).IsNotNull(); }
        }
        finally { File.Delete(path); }
    }

    [Test] [NonParallelizable]
    public async Task Import_CodeAndText_DetectBomEncodings()
    {
        Int64 previousLimit = Settings.DataImportBufferLimit;
        Settings.DataImportBufferLimit = Int64.MaxValue;
        String content = "BOM-aware text Ω 🚀";
        Encoding[] encodings = [new UTF8Encoding(true),Encoding.Unicode,Encoding.BigEndianUnicode,Encoding.UTF32,new UTF32Encoding(true,true,true)];

        try
        {
            foreach(String extension in new[] { ".txt", ".c" })
            {
                foreach(Encoding encoding in encodings)
                {
                    String path = Path.Combine(Path.GetTempPath(),Guid.NewGuid()+extension);
                    try
                    {
                        Byte[] bytes = encoding.GetPreamble().Concat(encoding.GetBytes(content)).ToArray();
                        await File.WriteAllBytesAsync(path,bytes).ConfigureAwait(false);
                        DataItem? item = await DataPort.Import(path).ConfigureAwait(false);
                        String? imported = item switch { TextItem text => text.GetContent(), CodeItem code => code.GetContent(), _ => null };
                        Check.That(imported).IsEqualTo(content);
                    }
                    finally { File.Delete(path); }
                }
            }
        }
        finally { Settings.DataImportBufferLimit = previousLimit; }
    }

    [Test] [NonParallelizable]
    public async Task Import_InvalidCodeAndTextEncoding_ThrowsFormatException()
    {
        Int64 previousLimit = Settings.DataImportBufferLimit;

        try
        {
            foreach(Int64 bufferLimit in new[] { Int64.MaxValue,0L })
            {
                Settings.DataImportBufferLimit = bufferLimit;
                foreach(String extension in new[] { ".txt", ".c" })
                {
                    String path = Path.Combine(Path.GetTempPath(),Guid.NewGuid()+extension);
                    try
                    {
                        await File.WriteAllBytesAsync(path,new Byte[] { 0xC3,0x28 }).ConfigureAwait(false);
                        Check.ThatCode(async () => await DataPort.Import(path).ConfigureAwait(false)).Throws<FormatException>();
                    }
                    finally { File.Delete(path); }
                }
            }
        }
        finally { Settings.DataImportBufferLimit = previousLimit; }
    }

    [Test] [NonParallelizable]
    public async Task Import_OverBufferLimit_UsesFileBeforeStreaming()
    {
        Int64 previousLimit = Settings.DataImportBufferLimit;
        String path = Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".txt");

        try
        {
            Settings.DataImportBufferLimit = 0;
            Byte[] content = Encoding.UTF8.GetBytes(new String('a',8191)+"€ streaming content"); File.WriteAllBytes(path,content);

            TextItem? item = (TextItem?)await DataPort.Import(path);

            Check.That(item).IsNotNull();
            Check.That(item!.GetFILE()).IsEqualTo(path);
            Check.That(item.GetContentStreamed()).IsTrue();

            Stream? stream = item.GetContentStream(); Check.That(stream).IsNotNull();
            Check.That(stream!.Length).IsEqualTo(content.Length); stream.Dispose();
        }
        finally
        {
            Settings.DataImportBufferLimit = previousLimit; File.Delete(path);
        }
    }

    [Test] [NonParallelizable]
    public async Task Export_OverBufferLimit_BinaryItem_PreservesFileContent()
    {
        Int64 previousLimit = Settings.DataImportBufferLimit;
        String sourcePath = Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".bin");
        String exportPath = Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".bin");
        Byte[] content = RandomNumberGenerator.GetBytes(16*1024);

        try
        {
            Settings.DataImportBufferLimit = 0;
            await File.WriteAllBytesAsync(sourcePath,content).ConfigureAwait(false);
            BinaryItem? item = (BinaryItem?)await DataPort.Import(sourcePath).ConfigureAwait(false);

            Check.That(item).IsNotNull();
            Check.That(item!.GetContentStreamed()).IsTrue();
            Check.That(await DataPort.Export(item,exportPath).ConfigureAwait(false)).IsTrue();
            Check.That(await File.ReadAllBytesAsync(exportPath).ConfigureAwait(false)).IsEqualTo(content);
        }
        finally
        {
            Settings.DataImportBufferLimit = previousLimit;
            File.Delete(sourcePath); File.Delete(exportPath);
        }
    }

    [Test]
    public async Task Export_EncryptedBufferedContent_RequiresContext()
    {
        String content = "protected export content Ω";
        String exportPath = Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".txt");
        TextItem item = new(content);
        DataItemSecurityContext security = DataItemSecurityContextExamHelpers.CreateContext(item);

        try
        {
            Check.That(await item.EncryptData(security).ConfigureAwait(false)).IsTrue();
            Check.That(await DataPort.Export(item,exportPath).ConfigureAwait(false)).IsFalse();
            Check.That(File.Exists(exportPath)).IsFalse();
            Check.That(await DataPort.Export(item,exportPath,security:security).ConfigureAwait(false)).IsTrue();
            Check.That(await File.ReadAllBytesAsync(exportPath).ConfigureAwait(false)).IsEqualTo(new UTF8Encoding(false,true).GetBytes(content));
            Check.That(item.GetDataEncrypted()).IsTrue();
        }
        finally { File.Delete(exportPath); }
    }

    [Test]
    public async Task Export_EncryptedStreamedContent_DecryptsStagingCopy()
    {
        Byte[] content = RandomNumberGenerator.GetBytes(16*1024);
        String sourcePath = Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".bin");
        String exportPath = Path.Combine(Path.GetTempPath(),Guid.NewGuid()+".bin");
        BinaryItem item = new();

        try
        {
            await File.WriteAllBytesAsync(sourcePath,content).ConfigureAwait(false);
            Check.That(item.SetFILE(sourcePath)).IsTrue();
            Check.That(await item.SetContentStreamed(true).ConfigureAwait(false)).IsTrue();
            DataItemSecurityContext security = DataItemSecurityContextExamHelpers.CreateContext(item);
            Check.That(await item.EncryptData(security).ConfigureAwait(false)).IsTrue();
            Byte[] encryptedSource = await File.ReadAllBytesAsync(sourcePath).ConfigureAwait(false);

            Check.That(await DataPort.Export(item,exportPath).ConfigureAwait(false)).IsFalse();
            Check.That(File.Exists(exportPath)).IsFalse();
            Check.That(await DataPort.Export(item,exportPath,security:security).ConfigureAwait(false)).IsTrue();
            Check.That(await File.ReadAllBytesAsync(exportPath).ConfigureAwait(false)).IsEqualTo(content);
            Check.That(await File.ReadAllBytesAsync(sourcePath).ConfigureAwait(false)).IsEqualTo(encryptedSource);
            Check.That(item.GetDataEncrypted()).IsTrue();
        }
        finally
        {
            File.Delete(sourcePath); File.Delete(exportPath);
        }
    }
}