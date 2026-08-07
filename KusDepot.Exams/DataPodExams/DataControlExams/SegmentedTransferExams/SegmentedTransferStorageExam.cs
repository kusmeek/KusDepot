namespace KusDepot.DataServices;

[TestFixture] [NonParallelizable]
public class SegmentedTransferStorageExam
{
    private String? RootPath;
    private ISegmentedTransferStorage? Storage;

    [OneTimeSetUp]
    public void Calibrate()
    {
        if(OperatingSystem.IsWindows() is false && OperatingSystem.IsLinux() is false) { Assert.Ignore("SegmentedTransferStorage is supported only on Windows and Linux."); }

        this.RootPath = Path.Combine(Path.GetTempPath(),"KusDepot","Exam","SegmentedTransfer",Guid.NewGuid().ToString("N",InvariantCulture));

        this.Storage = SegmentedTransferExamHelpers.CreateStorage(this.RootPath);
    }

    [OneTimeTearDown]
    public void Complete()
    {
        if(String.IsNullOrWhiteSpace(this.RootPath) is false && Directory.Exists(this.RootPath)) { Directory.Delete(this.RootPath,true); }
    }

    [Test] [Order(1)]
    public async Task CreateSessionPersistsManifestAndObjectPayload()
    {
        DataItemTransferManifest manifest = SegmentedTransferExamHelpers.CreateManifest();
        Byte[] objectPayload = SegmentedTransferExamHelpers.CreatePayload(48,0x21);

        SegmentedTransferStorageSessionPaths? paths = await this.Storage!.CreateSession(manifest,objectPayload);

        Check.That(paths).IsNotNull();
        Check.That(Directory.Exists(paths!.SessionDirectoryPath)).IsTrue();
        Check.That(File.Exists(paths.ManifestPath)).IsTrue();
        Check.That(File.Exists(paths.ObjectPath)).IsTrue();
        Check.That(File.Exists(paths.StreamPath)).IsTrue();
        Check.That(new FileInfo(paths.StreamPath).Length).IsEqualTo(manifest.StreamLength);

        DataItemTransferManifest? loaded = await this.Storage.LoadManifest(manifest.SessionID);
        Byte[]? loadedObjectPayload = await this.Storage.ReadObjectPayload(manifest.SessionID);

        Check.That(loaded).IsNotNull();
        Check.That(loaded!.SessionID).IsEqualTo(manifest.SessionID);
        Check.That(loaded.ItemID).IsEqualTo(manifest.ItemID);
        Check.That(loaded.StreamLength).IsEqualTo(manifest.StreamLength);
        Check.That(loaded.ObjectSHA512.SequenceEqual(manifest.ObjectSHA512)).IsTrue();
        Check.That(loaded.StreamSHA512.SequenceEqual(manifest.StreamSHA512)).IsTrue();
        Check.That(loadedObjectPayload).IsNotNull();
        Check.That(loadedObjectPayload!.SequenceEqual(objectPayload)).IsTrue();
    }

    [Test] [Order(2)]
    public async Task SaveManifestPersistsUpdatedRanges()
    {
        DataItemTransferManifest manifest = SegmentedTransferExamHelpers.CreateManifest();
        Byte[] objectPayload = SegmentedTransferExamHelpers.CreatePayload(32,0x31);

        await this.Storage!.CreateSession(manifest,objectPayload);

        DataItemTransferManifest updated = manifest.WithAddedRange(new DataItemTransferRange(){ Offset = 8 , Length = 16 });

        Check.That(await this.Storage.SaveManifest(updated)).IsTrue();

        DataItemTransferManifest? loaded = await this.Storage.LoadManifest(updated.SessionID);

        Check.That(loaded).IsNotNull();
        Check.That(loaded!.RealizedRanges.Length).IsEqualTo(1);
        Check.That(loaded.RealizedRanges[0].Offset).IsEqualTo(8);
        Check.That(loaded.RealizedRanges[0].Length).IsEqualTo(16);
    }

    [Test] [Order(3)]
    public async Task WriteRangeAndReadRangeRoundTrip()
    {
        DataItemTransferManifest manifest = SegmentedTransferExamHelpers.CreateManifest(streamLength:64);
        Byte[] objectPayload = SegmentedTransferExamHelpers.CreatePayload(24,0x41);
        DataItemTransferRange range = new(){ Offset = 4 , Length = 12 };
        Byte[] payload = SegmentedTransferExamHelpers.CreatePayload((Int32)range.Length,0x51);

        await this.Storage!.CreateSession(manifest,objectPayload);
        using MemoryStream payloadStream = SegmentedTransferExamHelpers.CreatePayloadStream(payload);
        SegmentedTransferWriteResult result = await this.Storage.WriteRange(manifest.SessionID,range,payloadStream);

        Check.That(result.Accepted).IsTrue();
        Check.That(result.Range).IsEqualTo(range);

        DataItemTransferManifest updated = manifest.WithAddedRange(range);
        Check.That(await this.Storage.SaveManifest(updated)).IsTrue();

        using Stream? loaded = await this.Storage.ReadRange(manifest.SessionID,range);
        using MemoryStream copy = new();

        Check.That(loaded).IsNotNull();

        await loaded!.CopyToAsync(copy);

        Check.That(copy.ToArray().SequenceEqual(payload)).IsTrue();
    }

    [Test] [Order(4)]
    public async Task WriteRangeRejectsAnyOverlap()
    {
        DataItemTransferManifest manifest = SegmentedTransferExamHelpers.CreateManifest(streamLength:64);
        Byte[] objectPayload = SegmentedTransferExamHelpers.CreatePayload(24,0x61);
        DataItemTransferRange first = new(){ Offset = 8 , Length = 16 };
        DataItemTransferRange overlap = new(){ Offset = 12 , Length = 8 };
        Byte[] firstPayload = SegmentedTransferExamHelpers.CreatePayload((Int32)first.Length,0x71);
        Byte[] overlapPayload = firstPayload.AsSpan(4,8).ToArray();

        await this.Storage!.CreateSession(manifest,objectPayload);
        using MemoryStream firstStream = SegmentedTransferExamHelpers.CreatePayloadStream(firstPayload);
        Check.That((await this.Storage.WriteRange(manifest.SessionID,first,firstStream)).Accepted).IsTrue();
        Check.That(await this.Storage.SaveManifest(manifest.WithAddedRange(first))).IsTrue();

        using MemoryStream overlapStream = SegmentedTransferExamHelpers.CreatePayloadStream(overlapPayload);
        SegmentedTransferWriteResult result = await this.Storage.WriteRange(manifest.SessionID,overlap,overlapStream);

        Check.That(result.Accepted).IsFalse();
        Check.That(result.Range).IsEqualTo(overlap);
    }

    [Test] [Order(5)]
    public async Task WriteRangeRejectsConflictingOverlap()
    {
        DataItemTransferManifest manifest = SegmentedTransferExamHelpers.CreateManifest(streamLength:64);
        Byte[] objectPayload = SegmentedTransferExamHelpers.CreatePayload(24,0x81);
        DataItemTransferRange first = new(){ Offset = 8 , Length = 16 };
        DataItemTransferRange overlap = new(){ Offset = 12 , Length = 8 };
        Byte[] firstPayload = SegmentedTransferExamHelpers.CreatePayload((Int32)first.Length,0x91);
        Byte[] overlapPayload = SegmentedTransferExamHelpers.CreatePayload((Int32)overlap.Length,0xA1);

        await this.Storage!.CreateSession(manifest,objectPayload);
        using MemoryStream firstStream = SegmentedTransferExamHelpers.CreatePayloadStream(firstPayload);
        Check.That((await this.Storage.WriteRange(manifest.SessionID,first,firstStream)).Accepted).IsTrue();
        Check.That(await this.Storage.SaveManifest(manifest.WithAddedRange(first))).IsTrue();

        using MemoryStream overlapStream = SegmentedTransferExamHelpers.CreatePayloadStream(overlapPayload);
        SegmentedTransferWriteResult result = await this.Storage.WriteRange(manifest.SessionID,overlap,overlapStream);

        Check.That(result.Accepted).IsFalse();
        Check.That(result.Range).IsEqualTo(overlap);
    }

    [Test] [Order(6)]
    public async Task DeleteSessionRemovesSessionDirectory()
    {
        DataItemTransferManifest manifest = SegmentedTransferExamHelpers.CreateManifest();
        Byte[] objectPayload = SegmentedTransferExamHelpers.CreatePayload(40,0xB1);

        SegmentedTransferStorageSessionPaths? paths = await this.Storage!.CreateSession(manifest,objectPayload);

        Check.That(paths).IsNotNull();
        Check.That(await this.Storage.DeleteSession(manifest.SessionID)).IsTrue();
        Check.That(Directory.Exists(paths!.SessionDirectoryPath)).IsFalse();
        Check.That(await this.Storage.LoadManifest(manifest.SessionID)).IsNull();
    }

}
