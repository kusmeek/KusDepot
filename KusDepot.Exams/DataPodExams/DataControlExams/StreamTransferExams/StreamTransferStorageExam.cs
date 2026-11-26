namespace KusDepot.DataServices;

[TestFixture] [NonParallelizable]
public class StreamTransferStorageExam
{
    private String? RootPath;
    private IStreamTransferStorage? Storage;

    [OneTimeSetUp]
    public void Calibrate()
    {
        if(OperatingSystem.IsWindows() is false) { Assert.Ignore("StreamTransferStorage is currently Windows-only."); }

        this.RootPath = Path.Combine(Path.GetTempPath(),"KusDepot","Exam","StreamTransfer",Guid.NewGuid().ToString("N",InvariantCulture));

        this.Storage = StreamTransferExamHelpers.CreateStorage(this.RootPath);
    }

    [OneTimeTearDown]
    public void Complete()
    {
        if(String.IsNullOrWhiteSpace(this.RootPath) is false && Directory.Exists(this.RootPath)) { Directory.Delete(this.RootPath,true); }
    }

    [Test] [Order(1)]
    public async Task CreateSessionPersistsManifestObjectPayloadAndStreamFile()
    {
        DataStreamTransferManifest manifest = StreamTransferExamHelpers.CreateManifest(appendedLength:24);

        StreamTransferStorageSessionPaths? paths = await this.Storage!.CreateSession(manifest);

        Check.That(paths).IsNotNull();
        Check.That(Directory.Exists(paths!.SessionDirectoryPath)).IsTrue();
        Check.That(File.Exists(paths.ManifestPath)).IsTrue();
        Check.That(File.Exists(paths.ObjectPath)).IsTrue();
        Check.That(File.Exists(paths.StreamPath)).IsTrue();
        Check.That(new FileInfo(paths.StreamPath).Length).IsEqualTo(manifest.AppendedLength);

        DataStreamTransferManifest? loaded = await this.Storage.LoadManifest(manifest.SessionID);
        using Stream? payload = await this.Storage.ReadObjectPayload(manifest.SessionID);
        using MemoryStream copy = new();

        Check.That(loaded).IsNotNull();
        Check.That(loaded!.SessionID).IsEqualTo(manifest.SessionID);
        Check.That(loaded.ItemID).IsEqualTo(manifest.ItemID);
        Check.That(loaded.ObjectSHA512.SequenceEqual(manifest.ObjectSHA512)).IsTrue();
        Check.That(loaded.AppendedLength).IsEqualTo(manifest.AppendedLength);
        Check.That(payload).IsNotNull();

        await payload!.CopyToAsync(copy);

        Check.That(copy.ToArray().SequenceEqual(manifest.ObjectPayload)).IsTrue();
    }

    [Test] [Order(2)]
    public async Task SaveManifestPersistsUpdatedAppendedLength()
    {
        DataStreamTransferManifest manifest = StreamTransferExamHelpers.CreateManifest();

        await this.Storage!.CreateSession(manifest);

        DataStreamTransferManifest updated = manifest.WithAppendedLength(16);

        Check.That(await this.Storage.SaveManifest(updated)).IsTrue();

        DataStreamTransferManifest? loaded = await this.Storage.LoadManifest(updated.SessionID);

        Check.That(loaded).IsNotNull();
        Check.That(loaded!.AppendedLength).IsEqualTo(16);
        Check.That(loaded.StateVersion).IsEqualTo(updated.StateVersion);
    }

    [Test] [Order(3)]
    public async Task SaveObjectPayloadPersistsReplacementBytes()
    {
        DataStreamTransferManifest manifest = StreamTransferExamHelpers.CreateManifest();
        Byte[] replacement = StreamTransferExamHelpers.CreatePayload(40,0x41);

        await this.Storage!.CreateSession(manifest);

        Check.That(await this.Storage.SaveObjectPayload(manifest.SessionID,replacement)).IsTrue();

        using Stream? payload = await this.Storage.ReadObjectPayload(manifest.SessionID);
        using MemoryStream copy = new();

        Check.That(payload).IsNotNull();

        await payload!.CopyToAsync(copy);

        Check.That(copy.ToArray().SequenceEqual(replacement)).IsTrue();
    }

    [Test] [Order(4)]
    public async Task WriteTailAndReadRangeRoundTrip()
    {
        DataStreamTransferManifest manifest = StreamTransferExamHelpers.CreateManifest();
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(16,0x51);
        DataItemTransferRange range = new(){ Offset = 0 , Length = payload.Length };

        await this.Storage!.CreateSession(manifest);
        using MemoryStream append = StreamTransferExamHelpers.CreatePayloadStream(payload);
        StreamTransferAppendResult written = await this.Storage.WriteTail(manifest.SessionID,range,append);

        Check.That(written.Accepted).IsTrue();
        Check.That(written.Range).IsEqualTo(range);
        Check.That(written.AppendedLength).IsEqualTo(payload.Length);

        Check.That(await this.Storage.SaveManifest(manifest.WithAppendedLength(payload.Length))).IsTrue();

        using Stream? loaded = await this.Storage.ReadRange(manifest.SessionID,range);
        using MemoryStream copy = new();

        Check.That(loaded).IsNotNull();

        await loaded!.CopyToAsync(copy);

        Check.That(copy.ToArray().SequenceEqual(payload)).IsTrue();
    }

    [Test] [Order(5)]
    public async Task WriteTailRejectsNonTailAppend()
    {
        DataStreamTransferManifest manifest = StreamTransferExamHelpers.CreateManifest(appendedLength:8);
        Byte[] payload = StreamTransferExamHelpers.CreatePayload(8,0x61);
        DataItemTransferRange invalid = new(){ Offset = 4 , Length = payload.Length };

        await this.Storage!.CreateSession(manifest);
        using MemoryStream append = StreamTransferExamHelpers.CreatePayloadStream(payload);
        StreamTransferAppendResult written = await this.Storage.WriteTail(manifest.SessionID,invalid,append);

        Check.That(written.Accepted).IsFalse();
        Check.That(written.Range).IsEqualTo(invalid);
        Check.That(written.AppendedLength).IsEqualTo(manifest.AppendedLength);
    }

    [Test] [Order(6)]
    public async Task DeleteSessionRemovesSessionDirectory()
    {
        DataStreamTransferManifest manifest = StreamTransferExamHelpers.CreateManifest();

        StreamTransferStorageSessionPaths? paths = await this.Storage!.CreateSession(manifest);

        Check.That(paths).IsNotNull();
        Check.That(await this.Storage.DeleteSession(manifest.SessionID)).IsTrue();
        Check.That(Directory.Exists(paths!.SessionDirectoryPath)).IsFalse();
        Check.That(await this.Storage.LoadManifest(manifest.SessionID)).IsNull();
    }
}
