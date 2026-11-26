namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test]
    public async Task ClientDownloadRequestSizeControlsSegmentedReadAndPreservesEightMiBDefault()
    {
        String workingRoot = CreateClientWorkingRoot();
        String sourcePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.DownloadRequestSize.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(10 * 1024 * 1024,0xB1);
        Guid itemID = Guid.Empty;

        try
        {
            using DataControlClient client = CreateClient(workingRoot);
            BinaryItem item = await CreateClientBinaryItem(sourcePath,payload);
            itemID = item.GetID()!.Value;

            DataControlUploadCompletionResult? uploaded = await client.Upload(item,this.WriteToken);
            Check.That(uploaded).IsNotNull();

            DataControlDownloadSession? configured = await client.OpenDownload(itemID,this.ReadToken,new DataControlDownloadOptions
            {
                DownloadRequestSizeBytes = 256 * 1024,
            });
            Check.That(configured).IsNotNull();

            await using (configured!)
            {
                Check.That(await configured!.DownloadNext()).IsTrue();
                Check.That(configured.Session.State.RealizedBytes).IsEqualTo(256 * 1024);
            }

            DataControlDownloadSession? defaulted = await client.OpenDownload(itemID,this.ReadToken);
            Check.That(defaulted).IsNotNull();

            await using (defaulted!)
            {
                Check.That(await defaulted!.DownloadNext()).IsTrue();
                Check.That(defaulted.Session.State.RealizedBytes).IsEqualTo(8L * 1024 * 1024);
            }
        }
        finally
        {
            if(File.Exists(sourcePath)) { File.Delete(sourcePath); }
            if(itemID != Guid.Empty)
            {
                using DataControlClient cleanup = CreateClient(workingRoot);
                _ = await cleanup.Delete(itemID,this.WriteToken);
            }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientDownloadRequestSizeControlsStreamFollowRead()
    {
        String workingRoot = CreateClientWorkingRoot();
        String sourcePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.DownloadStreamFollowSize.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(512 * 1024,0xB2);
        Guid itemID = Guid.Empty;
        Guid followerSessionID = Guid.Empty;
        DataControlUploadSession? upload = null;
        using DataControlClient client = CreateClient(workingRoot);

        try
        {
            await using FileStream source = new(sourcePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await source.WriteAsync(payload);
            source.Position = 0;

            DataStreamItem item = new(source);
            itemID = item.GetID()!.Value;
            upload = await client.OpenUpload(item,this.WriteToken);
            Check.That(upload).IsNotNull();
            Check.That(await upload!.UploadNext()).IsTrue();

            DataControlDownloadSession? follower = await client.OpenDownload(itemID,this.ReadToken,new DataControlDownloadOptions
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = upload.Session.SessionId,
                DownloadRequestSizeBytes = 32 * 1024,
            });
            Check.That(follower).IsNotNull();
            followerSessionID = follower!.Session.SessionId;

            await using (follower!)
            {
                Check.That(await follower.DownloadNext()).IsTrue();
                Check.That(follower.Session.StreamState!.AppendedLength).IsEqualTo(32 * 1024);
            }

            DataControlDownloadSession? resumed = await client.ResumeDownload(followerSessionID,this.ReadToken,new DataControlDownloadOptions
            {
                DownloadRequestSizeBytes = 64 * 1024,
            });
            Check.That(resumed).IsNotNull();

            await using (resumed!)
            {
                Check.That(await resumed!.DownloadNext()).IsTrue();
                Check.That(resumed.Session.StreamState!.AppendedLength).IsEqualTo(96 * 1024);
            }
        }
        finally
        {
            if(followerSessionID != Guid.Empty)
            {
                using DataControlClient cleanup = CreateClient(workingRoot);
                _ = await cleanup.AbortTransfer(new AbortTransferRequest(){ SessionID = followerSessionID , ItemID = itemID },this.WriteToken);
            }
            if(upload is not null) { await upload.Abort(); await upload.DisposeAsync(); }
            if(File.Exists(sourcePath)) { File.Delete(sourcePath); }
            if(itemID != Guid.Empty)
            {
                using DataControlClient cleanup = CreateClient(workingRoot);
                _ = await cleanup.Delete(itemID,this.WriteToken);
            }
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task ClientDownloadRequestSizeRejectsNonPositiveValuesBeforeOpening()
    {
        using DataControlClient client = new(this.EndpointLocator!);

        DataControlDownloadSession? zero = await client.OpenDownload(Guid.NewGuid(),this.ReadToken,new DataControlDownloadOptions
        {
            DownloadRequestSizeBytes = 0,
        });

        DataControlDownloadSession? negative = await client.OpenDownload(Guid.NewGuid(),this.ReadToken,new DataControlDownloadOptions
        {
            DownloadRequestSizeBytes = -1,
        });

        Check.That(zero).IsNull();
        Check.That(negative).IsNull();
    }
}
