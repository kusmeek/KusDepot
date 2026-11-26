namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test]
    public async Task LiveFollowReadBlocksUntilFollowerDownloadsMoreBytes()
    {
        String workingRoot = CreateClientWorkingRoot();
        Byte[]? payload = null;

        try
        {
            using DataControlClient uploader = CreateClient(workingRoot);
            using DataControlClient followerClient = CreateClient(workingRoot);
            using ControllableProducerStream producer = new();

            DataStreamItem item = new(producer);
            Guid itemId = item.GetID()!.Value;

            DataControlUploadSession? uploadSession = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(uploadSession).IsNotNull();
            Check.That(uploadSession!.Session.StreamState).IsNotNull();

            Int32 chunkLength = ResolveLiveFollowChunkLength(uploadSession.Session.StreamState!);
            payload = CreateSegmentedTransferPayload(chunkLength,0x93);

            DataControlDownloadSession? follower = await followerClient.OpenDownload(itemId,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = uploadSession.Session.SessionId,
                BindLiveStreamItem = true,
            });

            Check.That(follower).IsNotNull();
            Check.That(follower!.LiveItem).IsNotNull();

            using Stream? liveStream = follower.LiveItem!.GetContentStream();

            Check.That(liveStream).IsNotNull();

            Task<Byte[]> firstRead = ReadExactlyAsync(liveStream!,chunkLength);

            Check.That(await RemainsIncompleteAsync(firstRead)).IsTrue();

            await producer.PublishAsync(payload);

            Check.That(await RemainsIncompleteAsync(firstRead)).IsTrue();

            Check.That(await uploadSession.UploadNext()).IsTrue();

            Check.That(await RemainsIncompleteAsync(firstRead)).IsTrue();

            Check.That(await follower.DownloadNext()).IsTrue();

            Check.That((await firstRead).SequenceEqual(payload)).IsTrue();

            producer.Complete();
            Check.That(await uploadSession.Abort()).IsTrue();
        }
        finally
        {
            DeleteWorkingDirectory(workingRoot);
        }
    }

    [Test]
    public async Task DownloadToCurrentReturnsWhenOpenStreamFollowHasNoMoreCurrentlyAvailableBytes()
    {
        String workingRoot = CreateClientWorkingRoot();
        String followerRoot = CreateClientWorkingRoot();

        try
        {
            using DataControlClient uploader = CreateClient(workingRoot);
            using DataControlClient followerClient = CreateClient(followerRoot);
            using ControllableProducerStream producer = new();

            DataStreamItem item = new(producer);
            Guid itemId = item.GetID()!.Value;

            DataControlUploadSession? uploadSession = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(uploadSession).IsNotNull();
            Check.That(uploadSession!.Session.StreamState).IsNotNull();

            Int32 chunkLength = ResolveLiveFollowChunkLength(uploadSession.Session.StreamState!);
            Byte[] segment = CreateSegmentedTransferPayload(chunkLength,0x97);

            DataControlDownloadSession? follower = await followerClient.OpenDownload(itemId,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = uploadSession.Session.SessionId,
                BindLiveStreamItem = true,
            });

            Check.That(follower).IsNotNull();

            await producer.PublishAsync(segment);
            Check.That(await uploadSession.UploadNext()).IsTrue();

            Int32 downloaded = await follower!.DownloadToCurrent();

            Check.That(downloaded).IsStrictlyGreaterThan(0);

            DataControlDownloadCompletionResult? materialized = await follower.Materialize();

            Check.That(materialized).IsNotNull();
            Check.That(new FileInfo(materialized!.StreamPath).Length).IsEqualTo(segment.LongLength);

            producer.Complete();
            Check.That(await uploadSession.Abort()).IsTrue();
        }
        finally
        {
            DeleteWorkingDirectory(workingRoot);
            DeleteWorkingDirectory(followerRoot);
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task DisposingDownloadSessionReleasesInfiniteFollowWait(Boolean asyncDispose)
    {
        String workingRoot = CreateClientWorkingRoot();
        String followerRoot = CreateClientWorkingRoot();
        using CancellationTokenSource fallbackCancellation = new();
        Task<Int32>? download = null;
        Task? disposal = null;

        try
        {
            using DataControlClient uploader = CreateClient(workingRoot);
            using DataControlClient followerClient = CreateClient(followerRoot);
            using ControllableProducerStream producer = new();

            DataStreamItem item = new(producer);
            Guid itemId = item.GetID()!.Value;

            DataControlUploadSession? uploadSession = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(uploadSession).IsNotNull();

            DataControlDownloadSession? follower = null;

            try
            {
                follower = await followerClient.OpenDownload(itemId,this.ReadToken,new DataControlDownloadOptions()
                {
                    Mode = DataControlDownloadMode.StreamFollow,
                    SourceSessionID = uploadSession!.Session.SessionId,
                });

                Check.That(follower).IsNotNull();

                download = follower!.DownloadToCompletion(cancel:fallbackCancellation.Token);

                Check.That(await RemainsIncompleteAsync(download,1000)).IsTrue();

                disposal = asyncDispose ? follower.DisposeAsync().AsTask() : Task.Run(follower.Dispose);

                await disposal.WaitAsync(TimeSpan.FromSeconds(5));

                await Assert.CatchAsync<OperationCanceledException>(async () => await download.WaitAsync(TimeSpan.FromSeconds(5)));
            }
            finally
            {
                fallbackCancellation.Cancel();

                try
                {
                    if(download is not null)
                    {
                        try { await download.WaitAsync(TimeSpan.FromSeconds(5)); } catch(OperationCanceledException) {}
                    }

                    if(disposal is null && follower is not null) { disposal = follower.DisposeAsync().AsTask(); }

                    if(disposal is not null) { await disposal.WaitAsync(TimeSpan.FromSeconds(15)); }
                }
                finally
                {
                    producer.Complete();

                    Check.That(await uploadSession!.Abort()).IsTrue();
                }
            }
        }
        finally
        {
            DeleteWorkingDirectory(workingRoot);
            DeleteWorkingDirectory(followerRoot);
        }
    }

    [Test]
    public async Task LiveFollowRepeatedReadsBlockAndUnblockAcrossMultipleUploads()
    {
        String workingRoot = CreateClientWorkingRoot();
        String followerRoot = CreateClientWorkingRoot();

        try
        {
            using DataControlClient uploader = CreateClient(workingRoot);
            using DataControlClient followerClient = CreateClient(followerRoot);
            using ControllableProducerStream producer = new();

            DataStreamItem item = new(producer);
            Guid itemId = item.GetID()!.Value;

            DataControlUploadSession? uploadSession = await uploader.OpenUpload(item,this.WriteToken);

            Check.That(uploadSession).IsNotNull();
            Check.That(uploadSession!.Session.StreamState).IsNotNull();

            Int32 chunkLength = ResolveLiveFollowChunkLength(uploadSession.Session.StreamState!);

            Byte[][] segments =
            [
                CreateSegmentedTransferPayload(chunkLength,0x94),
                CreateSegmentedTransferPayload(chunkLength,0x95),
                CreateSegmentedTransferPayload(chunkLength,0x96),
            ];

            DataControlDownloadSession? follower = await followerClient.OpenDownload(itemId,this.ReadToken,new DataControlDownloadOptions()
            {
                Mode = DataControlDownloadMode.StreamFollow,
                SourceSessionID = uploadSession.Session.SessionId,
                BindLiveStreamItem = true,
            });

            Check.That(follower).IsNotNull();
            Check.That(follower!.LiveItem).IsNotNull();

            using Stream? liveStream = follower.LiveItem!.GetContentStream();

            Check.That(liveStream).IsNotNull();

            foreach(Byte[] segment in segments)
            {
                Task<Byte[]> readTask = ReadExactlyAsync(liveStream!,chunkLength);

                Check.That(await RemainsIncompleteAsync(readTask)).IsTrue();

                await producer.PublishAsync(segment);

                Check.That(await uploadSession.UploadNext()).IsTrue();
                Check.That(await RemainsIncompleteAsync(readTask)).IsTrue();
                Check.That(await follower.DownloadNext()).IsTrue();
                Check.That((await readTask).SequenceEqual(segment)).IsTrue();
            }

            producer.Complete();
            Check.That(await uploadSession.Abort()).IsTrue();
        }
        finally
        {
            DeleteWorkingDirectory(workingRoot);
        }
    }

    private static Int32 ResolveLiveFollowChunkLength(DataStreamTransferState state)
    {
        Int64 preferred = state.SegmentSizePolicy.Validate() ? state.SegmentSizePolicy.PreferredSegmentBytes : 81920;

        preferred = Math.Max(1,preferred);

        return checked((Int32)preferred);
    }
}
