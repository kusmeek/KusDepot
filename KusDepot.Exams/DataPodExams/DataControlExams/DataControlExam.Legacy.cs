namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test] [Order(53)]
    public async Task DownloadSerializedOnlyBinaryItem()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        Byte[] payload = CreateSegmentedTransferPayload(1024,0x91);
        BinaryItem item = new(payload);
        String? modernStreamPath = null;

        try
        {
            DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken);

            Check.That(uploaded).IsNotNull();
            Check.That(uploaded!.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
            Check.That(uploaded.Segmented!.Published).IsTrue();
            Check.That(uploaded.Session.State.StreamLength).IsEqualTo(0);

            DataControlDownloadCompletionResult? modern = await _dc.Download(item.GetID()!.Value,this.ReadToken);

            Check.That(modern).IsNotNull();
            Check.That(modern!.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
            Check.That(modern.Item).IsInstanceOf<BinaryItem>();
            Check.That(modern.Item!.GetContentStreamed()).IsFalse();
            Check.That(((BinaryItem)modern.Item).GetContent()!.SequenceEqual(payload)).IsTrue();
            Check.That(File.Exists(modern.StreamPath)).IsTrue();
            Check.That(new FileInfo(modern.StreamPath).Length).IsEqualTo(0);

            modernStreamPath = modern.StreamPath;

            RestResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            DeleteWorkingDirectoryForPath(modernStreamPath);
        }
    }

    [Test] [Order(54)]
    public async Task DownloadPublishedStreamedBinaryItem()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        String filePath = Path.Combine(Path.GetTempPath(),$"KusDepot.LegacyReplacement.StreamedBinary.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(1024 * 1024,0x92);
        String? modernStreamPath = null;

        try
        {
            BinaryItem item = await CreateClientBinaryItem(filePath,payload);

            DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken);

            Check.That(uploaded).IsNotNull();
            Check.That(uploaded!.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
            Check.That(uploaded.Segmented!.Published).IsTrue();
            Check.That(uploaded.Session.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
            Check.That(uploaded.Session.State.StreamLength).IsEqualTo(payload.LongLength);

            DataControlDownloadCompletionResult? modern = await _dc.Download(item.GetID()!.Value,this.ReadToken);

            Check.That(modern).IsNotNull();
            Check.That(modern!.Item).IsInstanceOf<BinaryItem>();
            Check.That(modern.Item!.GetContentStreamed()).IsTrue();
            Check.That(File.Exists(modern.StreamPath)).IsTrue();
            Check.That((await File.ReadAllBytesAsync(modern.StreamPath)).SequenceEqual(payload)).IsTrue();

            modernStreamPath = modern.StreamPath;

            RestResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            if(File.Exists(filePath)) { File.Delete(filePath); }
            DeleteWorkingDirectoryForPath(modernStreamPath);
        }
    }

    [Test] [Order(55)]
    public async Task UploadDataSetItemRoundTrip()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        DataSetItem item = new(new DataItem[]
        {
            new BinaryItem(CreateSegmentedTransferPayload(128,0x93)),
            new TextItem("LegacyReplacement.DataSetItem.Text"),
            new GuidReferenceItem(Guid.NewGuid()),
        });
        String? modernStreamPath = null;

        try
        {
            DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken);

            Check.That(uploaded).IsNotNull();
            Check.That(uploaded!.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
            Check.That(uploaded.Segmented!.Published).IsTrue();
            Check.That(uploaded.Session.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
            Check.That(uploaded.Session.State.StreamLength).IsEqualTo(0);

            DataControlDownloadCompletionResult? modern = await _dc.Download(item.GetID()!.Value,this.ReadToken);

            Check.That(modern).IsNotNull();
            Check.That(modern!.Item).IsInstanceOf<DataSetItem>();
            Check.That(modern.Item!.GetContentStreamed()).IsFalse();
            Check.That(modern.Item.Equals(item)).IsTrue();

            DataSetItem downloadedSet = (DataSetItem)modern.Item;
            HashSet<DataItem>? downloadedContent = downloadedSet.GetContent();
            HashSet<DataItem>? originalContent = item.GetContent();

            Check.That(downloadedContent).IsNotNull();
            Check.That(originalContent).IsNotNull();
            Check.That(downloadedContent!.Count).IsEqualTo(originalContent!.Count);

            modernStreamPath = modern.StreamPath;

            RestResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            DeleteWorkingDirectoryForPath(modernStreamPath);
        }
    }

    [Test] [Order(56)]
    public async Task UploadKeySetRoundTrip()
    {
        using DataControlClient _dc = new(this.EndpointLocator!,this.Certificate!);

        KeySet item = new(new SecurityKey[]
        {
            new ClientKey(Guid.NewGuid().ToByteArray()),
            new OwnerKey(Guid.NewGuid().ToByteArray()),
        });
        String? modernStreamPath = null;

        try
        {
            DataControlUploadCompletionResult? uploaded = await _dc.Upload(item,this.WriteToken);

            Check.That(uploaded).IsNotNull();
            Check.That(uploaded!.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
            Check.That(uploaded.Segmented!.Published).IsTrue();
            Check.That(uploaded.Session.State.Status).IsEqualTo(DataItemTransferStatus.Committed);
            Check.That(uploaded.Session.State.StreamLength).IsEqualTo(0);

            DataControlDownloadCompletionResult? modern = await _dc.Download(item.GetID()!.Value,this.ReadToken);

            Check.That(modern).IsNotNull();
            Check.That(modern!.Item).IsInstanceOf<KeySet>();
            Check.That(modern.Item!.GetContentStreamed()).IsFalse();
            Check.That(modern.Item.Equals(item)).IsTrue();

            KeySet downloadedKeySet = (KeySet)modern.Item;
            Dictionary<String,SecurityKey>? originalKeys = item.GetAllKeys();
            Dictionary<String,SecurityKey>? downloadedKeys = downloadedKeySet.GetAllKeys();

            Check.That(originalKeys).IsNotNull();
            Check.That(downloadedKeys).IsNotNull();
            Check.That(downloadedKeys!.Count).IsEqualTo(originalKeys!.Count);

            modernStreamPath = modern.StreamPath;

            RestResponse<Guid> deleted = await _dc.Delete(item.GetID(),this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
        }
        finally
        {
            DeleteWorkingDirectoryForPath(modernStreamPath);
        }
    }
}
