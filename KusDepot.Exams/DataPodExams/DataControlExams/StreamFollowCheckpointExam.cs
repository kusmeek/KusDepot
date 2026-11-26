using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;

#pragma warning disable IDE0230

namespace KusDepot.DataPodExams;

[TestFixture]
internal sealed class StreamFollowCheckpointExam
{
    private String root = null!;
    private WebApplication hub = null!;
    private String endpoint = null!;

    [SetUp]
    public async Task SetUp()
    {
        this.root = Path.Combine(Path.GetTempPath(),"KusDepot","Exam","StreamFollowCheckpoint",Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(this.root);
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddSignalR();
        this.hub = builder.Build();
        this.hub.MapHub<CheckpointHub>("/DataControl/Notifications");
        await this.hub.StartAsync();
        this.endpoint = this.hub.Urls.Single();
    }

    [TearDown]
    public async Task TearDown()
    {
        try
        {
            if(this.hub is not null) { await this.hub.DisposeAsync(); }
        }
        finally
        {
            if(Directory.Exists(this.root)) { Directory.Delete(this.root,true); }
        }
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task ResumeDiscardsUncheckpointedTail(Boolean bind)
    {
        FollowHandler handler = new();
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        Guid id;
        String streamPath;
        await using (DataControlDownloadSession first = await OpenSession(client,handler,bind))
        {
            Check.That(await first.DownloadNext()).IsTrue();
            id = first.Session.SessionId;
            streamPath = Path.Combine(first.Session.WorkingDirectory,DataStreamTransferManifest.StreamFileName);
        }
        await using (FileStream tail = new(streamPath,FileMode.Append,FileAccess.Write,FileShare.Read)) { await tail.WriteAsync(new Byte[] { 99,99,99 }); }

        await using DataControlDownloadSession? resumed = await client.ResumeDownload(id,options:new DataControlDownloadOptions { BindLiveStreamItem = bind , DownloadRequestSizeBytes = 4 });
        Check.That(resumed).IsNotNull();
        Check.That(new FileInfo(streamPath).Length).IsEqualTo(4L);
        Check.That(resumed!.Session.StreamState!.AppendedLength).IsEqualTo(4L);
        Check.That(await resumed.DownloadNext()).IsTrue();
        Check.That(handler.Offsets.Last()).IsEqualTo(4L);
        Check.That((await File.ReadAllBytesAsync(streamPath)).SequenceEqual(handler.Payload)).IsTrue();
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task ResumeRejectsShorterFile(Boolean bind)
    {
        FollowHandler handler = new();
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        Guid id;
        String path;
        await using (DataControlDownloadSession first = await OpenSession(client,handler,bind))
        {
            Check.That(await first.DownloadNext()).IsTrue();
            id = first.Session.SessionId;
            path = Path.Combine(first.Session.WorkingDirectory,DataStreamTransferManifest.StreamFileName);
        }
        using (FileStream file = new(path,FileMode.Open,FileAccess.Write,FileShare.Read)) { file.SetLength(2); }

        Check.That(await client.ResumeDownload(id,options:new DataControlDownloadOptions { BindLiveStreamItem = bind })).IsNull();
        Check.That((await client.GetLocalSession(id))!.StreamState!.AppendedLength).IsEqualTo(4L);
        Check.That(new FileInfo(path).Length).IsEqualTo(2L);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task RetryDiscardsUncheckpointedTail(Boolean bind)
    {
        FollowHandler handler = new();
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlDownloadSession session = await OpenSession(client,handler,bind);
        Check.That(await session.DownloadNext()).IsTrue();
        String path = Path.Combine(session.Session.WorkingDirectory,DataStreamTransferManifest.StreamFileName);
        await using (FileStream tail = new(path,FileMode.Append,FileAccess.Write,FileShare.ReadWrite)) { await tail.WriteAsync(new Byte[] { 99,99 }); }

        Check.That(await session.DownloadNext()).IsTrue();
        Check.That(handler.Offsets.Last()).IsEqualTo(4L);
        Check.That((await File.ReadAllBytesAsync(path)).SequenceEqual(handler.Payload)).IsTrue();
        Check.That(session.Session.StreamState!.AppendedLength).IsEqualTo(8L);
    }

    [Test]
    public async Task LiveReaderOnlySeesCheckpointedBytes()
    {
        FollowHandler handler = new();
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlDownloadSession session = await OpenSession(client,handler,true);
        using Stream live = session.LiveItem!.GetContentStream()!;
        Byte[] result = new Byte[4];
        Task<Int32> read = live.ReadAsync(result).AsTask();
        Check.That(read.IsCompleted).IsFalse();
        Check.That(await session.DownloadNext()).IsTrue();
        Check.That(await read.WaitAsync(TimeSpan.FromSeconds(5))).IsEqualTo(4);
        Check.That(result.SequenceEqual(handler.Payload.Take(4))).IsTrue();
        Check.That((await client.GetLocalSession(session.Session.SessionId))!.StreamState!.AppendedLength).IsEqualTo(4L);
    }

    [Test]
    public async Task FailedCheckpointKeepsBytesHiddenAndRetryRedownloads()
    {
        FollowHandler handler = new();
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlDownloadSession session = await OpenSession(client,handler,true);
        String path = Path.Combine(session.Session.WorkingDirectory,DataStreamTransferManifest.StreamFileName);
        String manifest = Path.Combine(session.Session.WorkingDirectory,DataStreamTransferManifest.ManifestFileName);
        String backup = manifest + ".backup";
        handler.OnSegment = () => { File.Move(manifest,backup); Directory.CreateDirectory(manifest); };
        using Stream live = session.LiveItem!.GetContentStream()!;
        Byte[] bytes = new Byte[4];
        Task<Int32> read = live.ReadAsync(bytes).AsTask();

        Check.That(await session.DownloadNext()).IsFalse();
        Check.That(session.Session.StreamState!.AppendedLength).IsEqualTo(0L);
        Check.That(new FileInfo(path).Length).IsEqualTo(4L);
        Check.That(read.IsCompleted).IsFalse();
        Check.That(await session.Materialize()).IsNull();

        Directory.Delete(manifest);
        File.Move(backup,manifest);
        handler.OnSegment = null;
        Check.That(await session.DownloadNext()).IsTrue();
        Check.That(handler.Offsets.SequenceEqual(new Int64[] { 0,0 })).IsTrue();
        Check.That(await read.WaitAsync(TimeSpan.FromSeconds(5))).IsEqualTo(4);
        Check.That(bytes.SequenceEqual(handler.Payload.Take(4))).IsTrue();
        Check.That((await client.GetLocalSession(session.Session.SessionId))!.StreamState!.AppendedLength).IsEqualTo(4L);
    }

    [Test]
    public async Task RetryAcceptsCheckpointAlreadyCommittedAfterReportedFailure()
    {
        FollowHandler handler = new();
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlDownloadSession session = await OpenSession(client,handler,true);
        String path = Path.Combine(session.Session.WorkingDirectory,DataStreamTransferManifest.StreamFileName);
        String manifest = Path.Combine(session.Session.WorkingDirectory,DataStreamTransferManifest.ManifestFileName);
        String backup = manifest + ".backup";
        handler.OnSegment = () => { File.Move(manifest,backup); Directory.CreateDirectory(manifest); };
        using Stream live = session.LiveItem!.GetContentStream()!;
        Byte[] bytes = new Byte[4];
        Task<Int32> read = live.ReadAsync(bytes).AsTask();

        Check.That(await session.DownloadNext()).IsFalse();
        Check.That(read.IsCompleted).IsFalse();
        Check.That(await session.Materialize()).IsNull();
        Directory.Delete(manifest);
        DataStreamTransferManifest saved = DataStreamTransferManifest.Load(backup)! with { AppendedLength = 4 };
        await File.WriteAllBytesAsync(manifest,saved.Serialize());
        handler.OnSegment = null;

        Check.That(await session.DownloadNext()).IsTrue();
        Check.That(handler.Offsets.SequenceEqual(new Int64[] { 0,4 })).IsTrue();
        Check.That(await read.WaitAsync(TimeSpan.FromSeconds(5))).IsEqualTo(4);
        Check.That(bytes.SequenceEqual(handler.Payload.Take(4))).IsTrue();
        Check.That((await File.ReadAllBytesAsync(path)).SequenceEqual(handler.Payload)).IsTrue();
    }

    [Test]
    public async Task RetryPublishesAlreadyCommittedFinalCheckpoint()
    {
        FollowHandler handler = new() { Complete = true };
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlDownloadSession session = await OpenSession(client,handler,true);
        Check.That(await session.DownloadNext()).IsTrue();
        String manifest = Path.Combine(session.Session.WorkingDirectory,DataStreamTransferManifest.ManifestFileName);
        String backup = manifest + ".backup";
        handler.OnSegment = () => { File.Move(manifest,backup); Directory.CreateDirectory(manifest); };
        using Stream live = session.LiveItem!.GetContentStream()!;
        Byte[] bytes = new Byte[8];
        Check.That(await live.ReadAsync(bytes.AsMemory(0,4))).IsEqualTo(4);
        Task<Int32> read = live.ReadAsync(bytes.AsMemory(4,4)).AsTask();

        Check.That(await session.DownloadNext()).IsFalse();
        Check.That(read.IsCompleted).IsFalse();
        Directory.Delete(manifest);
        DataStreamTransferManifest saved = DataStreamTransferManifest.Load(backup)! with { AppendedLength = 8 , Status = DataItemTransferStatus.Complete };
        await File.WriteAllBytesAsync(manifest,saved.Serialize());
        handler.OnSegment = null;

        Check.That(await session.DownloadNext()).IsFalse();
        Check.That(await read.WaitAsync(TimeSpan.FromSeconds(5))).IsEqualTo(4);
        Check.That(bytes.SequenceEqual(handler.Payload)).IsTrue();
        Check.That(await live.ReadAsync(new Byte[1]).AsTask().WaitAsync(TimeSpan.FromSeconds(5))).IsEqualTo(0);
        Check.That(session.Session.StreamState!.Status).IsEqualTo(DataItemTransferStatus.Complete);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task DownloadRejectsShortFile(Boolean bind)
    {
        FollowHandler handler = new();
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlDownloadSession session = await OpenSession(client,handler,bind);
        Check.That(await session.DownloadNext()).IsTrue();
        String path = Path.Combine(session.Session.WorkingDirectory,DataStreamTransferManifest.StreamFileName);
        using (FileStream file = new(path,FileMode.Open,FileAccess.Write,FileShare.ReadWrite)) { file.SetLength(2); }

        Check.That(await session.DownloadNext()).IsFalse();
        Check.That(handler.Offsets.Count).IsEqualTo(1);
        Check.That(session.Session.StreamState!.AppendedLength).IsEqualTo(4L);
        Check.That(new FileInfo(path).Length).IsEqualTo(2L);
    }

    [Test]
    public async Task ResumePromotionDiscardsUncheckpointedTail()
    {
        FollowHandler handler = new();
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        Guid id;
        String path;
        await using (DataControlDownloadSession first = await OpenSession(client,handler,false))
        {
            Check.That(await first.DownloadNext()).IsTrue();
            id = first.Session.SessionId;
            path = Path.Combine(first.Session.WorkingDirectory,DataStreamTransferManifest.StreamFileName);
        }
        await using (FileStream tail = new(path,FileMode.Append,FileAccess.Write,FileShare.Read)) { await tail.WriteAsync(new Byte[] { 99,99 }); }
        handler.ReopenUnavailable = true;

        await using DataControlDownloadSession? resumed = await client.ResumeDownload(id,options:new DataControlDownloadOptions { DownloadRequestSizeBytes = 4 });
        Check.That(resumed).IsNotNull();
        Check.That(resumed!.Session.TransferFamily).IsEqualTo(DataControlTransferFamily.Segmented);
        Check.That(resumed.Session.State.RealizedBytes).IsEqualTo(4L);
        String promotedPath = Path.Combine(resumed.Session.WorkingDirectory,DataItemTransferManifest.StreamFileName);
        Check.That((await File.ReadAllBytesAsync(promotedPath)).Take(4).SequenceEqual(handler.Payload.Take(4))).IsTrue();
    }

    [Test]
    public async Task BufferedAppendRollsBackPartialPayload()
    {
        String path = Path.Combine(this.root,"buffer.bin");
        await File.WriteAllBytesAsync(path,new Byte[] { 1,2 });
        await using BufferedFollowStream follow = new(path,2);

        await Assert.ThrowsAsync<IOException>(() => follow.AppendAsync(new FailingStream()));
        Check.That(follow.Length).IsEqualTo(2L);
        Check.That((await File.ReadAllBytesAsync(path)).SequenceEqual(new Byte[] { 1,2 })).IsTrue();
        await follow.AppendAsync(new MemoryStream(new Byte[] { 3,4 }));
        Check.That((await File.ReadAllBytesAsync(path)).SequenceEqual(new Byte[] { 1,2,3,4 })).IsTrue();
    }

    [Test]
    public async Task MismatchedFooterLengthDoesNotAppend()
    {
        FollowHandler handler = new() { IncorrectLength = true };
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlDownloadSession session = await OpenSession(client,handler,false);

        Check.That(await session.DownloadNext()).IsFalse();
        Check.That(session.Session.StreamState!.AppendedLength).IsEqualTo(0L);
        Check.That(new FileInfo(Path.Combine(session.Session.WorkingDirectory,DataStreamTransferManifest.StreamFileName)).Length).IsEqualTo(0L);
    }

    private DataControlClient CreateClient(HttpClient http)
    {
        return new DataControlClient(this.endpoint,options:new DataControlClientOptions
        {
            HttpClient = http,
            WorkingDirectoryOptions = new DataControlClientWorkingDirectoryOptions { RootPath = this.root },
        });
    }

    private static async Task<DataControlDownloadSession> OpenSession(DataControlClient client , FollowHandler handler , Boolean bind)
    {
        DataControlDownloadSession? session = await client.OpenDownload(handler.ItemID,options:new DataControlDownloadOptions
        {
            Mode = DataControlDownloadMode.StreamFollow,
            SourceSessionID = handler.SourceID,
            BindLiveStreamItem = bind,
            DownloadRequestSizeBytes = 4,
        });
        Check.That(session).IsNotNull();
        return session!;
    }

    private sealed class FollowHandler : HttpMessageHandler
    {
        private readonly Byte[] item = new DataStreamItem().Serialize();
        public Guid ItemID { get; } = Guid.NewGuid();
        public Guid SourceID { get; } = Guid.NewGuid();
        public Byte[] Payload { get; } = Enumerable.Range(1,8).Select(value => (Byte)value).ToArray();
        public List<Int64> Offsets { get; } = new();
        public Boolean IncorrectLength { get; init; }
        public Boolean ReopenUnavailable { get; set; }
        public Boolean Complete { get; init; }
        public Action? OnSegment { get; set; }
        private DataStreamTransferState state = null!;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request , CancellationToken cancel)
        {
            switch(request.RequestUri!.AbsolutePath)
            {
                case "/OpenFollowStreamTransfer":
                    OpenFollowStreamTransferRequest open = JsonUtility.Parse<OpenFollowStreamTransferRequest>(await request.Content!.ReadAsStringAsync(cancel))!;
                    this.state = new DataStreamTransferState
                    {
                        SessionID = open.SessionID, ItemID = this.ItemID, SourceSessionID = this.SourceID,
                        Mode = DataItemTransferMode.StreamFollow, Status = this.Complete ? DataItemTransferStatus.Complete : DataItemTransferStatus.Open,
                        AppendedLength = this.Payload.Length, ObjectSHA512 = SHA512.HashData(this.item), Created = DateTimeOffset.UtcNow, Updated = DateTimeOffset.UtcNow,
                    };
                    return Json(new OpenFollowStreamTransferResponse { State = this.state , ObjectPayload = this.item });
                case "/ReOpenFollowStreamTransfer":
                    return this.ReopenUnavailable ? new HttpResponseMessage(HttpStatusCode.NotFound) : Json(new ReOpenFollowStreamTransferResponse { State = this.state , ObjectPayload = this.item });
                case "/OpenGetTransfer":
                {
                    OpenGetTransferRequest publishedRequest = JsonUtility.Parse<OpenGetTransferRequest>(await request.Content!.ReadAsStringAsync(cancel))!;
                    return Json(new OpenGetTransferResponse
                    {
                        ObjectPayload = this.item,
                        State = new DataItemTransferState
                        {
                            SessionID = publishedRequest.SessionID, ItemID = this.ItemID, Mode = DataItemTransferMode.PublishedRead,
                            Status = DataItemTransferStatus.Open, StreamLength = this.Payload.Length, ObjectSHA512 = SHA512.HashData(this.item),
                            Created = DateTimeOffset.UtcNow, Updated = DateTimeOffset.UtcNow,
                        },
                    });
                }
                case "/GetStreamTransferSegment":
                {
                    GetStreamTransferSegmentRequest get = JsonUtility.Parse<GetStreamTransferSegmentRequest>(await request.Content!.ReadAsStringAsync(cancel))!;
                    this.Offsets.Add(get.Offset);
                    Byte[] bytes = this.Payload.Skip((Int32)get.Offset).Take((Int32)get.Length).ToArray();
                    StreamTransferSegmentFooter footer = new()
                    {
                        SessionID = get.SessionID, ItemID = get.ItemID, RequestedOffset = get.Offset, RequestedLength = get.Length,
                        ReturnedOffset = get.Offset, ReturnedLength = bytes.Length + (this.IncorrectLength ? 1 : 0), SegmentSHA512 = SHA512.HashData(bytes),
                        AppendedLength = this.Payload.Length,
                    };
                    using MemoryStream envelope = new();
                    await TransferEnvelope.WriteWithTrailer(envelope,JsonUtility.Serialize(footer),new MemoryStream(bytes,false),bytes.Length,cancel);
                    this.OnSegment?.Invoke();
                    return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(envelope.ToArray()) };
                }
                default:
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
        }

        private static HttpResponseMessage Json<T>(T value) => new(HttpStatusCode.OK) { Content = new StringContent(JsonUtility.ToJsonString(value),Encoding.UTF8,"application/json") };
    }

    private sealed class FailingStream : Stream
    {
        private Boolean read;
        public override Boolean CanRead => true;
        public override Boolean CanSeek => false;
        public override Boolean CanWrite => false;
        public override Int64 Length => throw new NotSupportedException();
        public override Int64 Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public override void Flush() { }
        public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count) => throw new NotSupportedException();
        public override ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancel = default)
        {
            if(this.read) { throw new IOException("Simulated partial payload failure."); }
            this.read = true;
            buffer.Span[0] = 9;
            return ValueTask.FromResult(1);
        }
        public override Int64 Seek(Int64 offset , SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(Int64 value) => throw new NotSupportedException();
        public override void Write(Byte[] buffer , Int32 offset , Int32 count) => throw new NotSupportedException();
    }
}

public sealed class CheckpointHub : Hub
{
    public Task SubscribeItem(Guid itemId) => Task.CompletedTask;
    public Task SubscribeSourceSession(Guid sessionId) => Task.CompletedTask;
    public Task UnsubscribeItem(Guid itemId) => Task.CompletedTask;
    public Task UnsubscribeSourceSession(Guid sessionId) => Task.CompletedTask;
}
