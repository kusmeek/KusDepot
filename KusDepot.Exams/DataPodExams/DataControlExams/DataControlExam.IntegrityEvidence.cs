namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test]
    public async Task ClientMaterializationRejectsSameLengthCorruptionForPublishedSegmentedDownload()
    {
        String workingRoot = CreateClientWorkingRoot();
        String sourcePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.IntegritySegmented.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(512 * 1024,0xA1);
        Guid itemID = Guid.Empty;

        try
        {
            using DataControlClient client = CreateClient(workingRoot);
            BinaryItem item = await CreateClientBinaryItem(sourcePath,payload);
            itemID = item.GetID()!.Value;
            DataControlUploadCompletionResult? uploaded = await client.Upload(item,this.WriteToken);
            Check.That(uploaded).IsNotNull();

            DataControlDownloadSession? download = await client.OpenDownload(itemID,this.ReadToken);
            Check.That(download).IsNotNull();
            await using DataControlDownloadSession materializationSession = download!;
            Check.That(await materializationSession.DownloadToCompletion()).IsGreaterOrEqualThan(0);

            String streamPath = Path.Combine(workingRoot,"Segments",materializationSession.Session.SessionId.ToString("N"),DataItemTransferManifest.StreamFileName);
            Byte[] corrupted = await File.ReadAllBytesAsync(streamPath);
            corrupted[corrupted.Length / 2] ^= 0xFF;
            await File.WriteAllBytesAsync(streamPath,corrupted);

            Check.That(await materializationSession.Materialize()).IsNull();

            DataControlResponse<Guid> deleted = await client.Delete(itemID,this.WriteToken);
            Check.That((Int32)deleted.StatusCode).IsEqualTo(StatusCodes.Status200OK);
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
    public async Task ClientMaterializationUsesSourceIntegrityBoundariesForSegmentedFollow()
    {
        String workingRoot = CreateClientWorkingRoot();
        String sourcePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.IntegritySourceBoundaries.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(128 * 1024,0xA2);
        Guid itemID = Guid.Empty;

        try
        {
            using DataControlClient client = CreateClient(workingRoot);
            BinaryItem item = await CreateClientBinaryItem(sourcePath,payload);
            itemID = item.GetID()!.Value;
            DataControlUploadSession? source = await client.OpenUpload(item,this.WriteToken,new DataControlUploadOptions
            {
                RequestedSegmentSizePolicy = new DataItemTransferSegmentSizePolicy { MinSegmentBytes = 1 , PreferredSegmentBytes = 64 * 1024 , MaxSegmentBytes = 64 * 1024 },
            });
            Check.That(source).IsNotNull();
            Check.That(await source!.UploadToCompletion()).IsGreaterOrEqualThan(0);

            DataControlDownloadSession? follower = await client.OpenDownload(itemID,this.ReadToken,new DataControlDownloadOptions
            {
                Mode = DataControlDownloadMode.StagedFollow,
                SourceSessionID = source.Session.SessionId,
            });
            Check.That(follower).IsNotNull();
            await using DataControlDownloadSession followerSession = follower!;
            Check.That(await followerSession.DownloadToCurrent()).IsGreaterOrEqualThan(0);

            DataControlResponse<GetTransferIntegrityEvidenceResponse> evidence = await client.GetTransferIntegrityEvidence(new GetTransferIntegrityEvidenceRequest
            {
                ItemID = itemID,
                SessionID = followerSession.Session.SessionId,
                TransferFamily = DataControlTransferFamily.Segmented,
                PrefixLength = payload.LongLength,
            },this.ReadToken);
            Check.That((Int32)evidence.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(evidence.Data).IsNotNull();
            Check.That(evidence.Data!.Segments.Length).IsEqualTo(2);
            Check.That(evidence.Data.Segments.All(segment => segment.Length == 64 * 1024)).IsTrue();
            Check.That(await followerSession.Materialize()).IsNotNull();

            Check.That(await source.Abort()).IsTrue();
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
    public async Task ClientStreamFollowMaterializationReportsProvisionalUntilSourcePublished()
    {
        String workingRoot = CreateClientWorkingRoot();
        String sourcePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.IntegrityStreamFollow.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(256 * 1024,0xA3);
        Guid itemID = Guid.Empty;
        DataControlUploadSession? source = null;

        try
        {
            using DataControlClient client = CreateClient(workingRoot);
            await using FileStream stream = new(sourcePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read,81920,FileOptions.Asynchronous);
            await stream.WriteAsync(payload);
            stream.Position = 0;
            DataStreamItem item = new(stream);
            itemID = item.GetID()!.Value;
            source = await client.OpenUpload(item,this.WriteToken);
            Check.That(source).IsNotNull();
            Check.That(await source!.UploadToCompletion()).IsGreaterOrEqualThan(0);

            DataControlDownloadSession? follower = await client.OpenDownload(itemID,this.ReadToken,new DataControlDownloadOptions
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = source.Session.SessionId,
                BindLiveStreamItem = true,
            });
            Check.That(follower).IsNotNull();
            await using DataControlDownloadSession followerSession = follower!;
            Check.That(await followerSession.DownloadToCurrent()).IsGreaterOrEqualThan(0);

            DataControlDownloadCompletionResult? provisional = await followerSession.Materialize();
            Check.That(provisional).IsNotNull();
            Check.That(provisional!.IsProvisional).IsTrue();

            Check.That(await source.Publish()).IsNotNull();
            DataControlDownloadCompletionResult? published = await followerSession.Materialize();
            Check.That(published).IsNotNull();
            Check.That(published!.IsProvisional).IsFalse();

            Check.That(await client.Delete(itemID,this.WriteToken)).IsNotNull();
        }
        finally
        {
            if(source is not null) { await source.DisposeAsync(); }
            if(File.Exists(sourcePath)) { File.Delete(sourcePath); }
            if(itemID != Guid.Empty)
            {
                using DataControlClient cleanup = CreateClient(workingRoot);
                _ = await cleanup.Delete(itemID,this.WriteToken);
            }
            DeleteWorkingDirectory(workingRoot);
        }
    }
}
