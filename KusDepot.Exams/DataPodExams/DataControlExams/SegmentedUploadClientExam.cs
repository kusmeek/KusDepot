using System.Net;

namespace KusDepot.DataPodExams;

[TestFixture]
internal sealed class SegmentedUploadClientExam
{
    private String root = null!;

    [SetUp]
    public void SetUp()
    {
        this.root = Path.Combine(Path.GetTempPath(),"KusDepot","Exam","SegmentedUploadClient",Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(this.root);
    }

    [TearDown]
    public void TearDown()
    {
        if(Directory.Exists(this.root)) { Directory.Delete(this.root,true); }
    }

    [Test]
    public async Task NewWritesAdvanceAndPersistWithoutStateRefresh()
    {
        UploadHandler handler = new();
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlUploadSession session = await OpenSession(client);

        Check.That(await session.UploadNext()).IsTrue();
        Check.That(handler.StateReads).IsEqualTo(0);
        Check.That(session.Session.State.StateVersion).IsEqualTo(1L);
        Check.That(session.Session.State.RealizedBytes).IsEqualTo(8L);
        Check.That(session.Session.State.RemainingBytes).IsEqualTo(8L);
        Check.That(session.Session.State.Updated).IsEqualTo(handler.Manifest!.Updated);

        DataControlTransferSessionInfo? persisted = await client.GetLocalSession(session.Session.SessionId);
        Check.That(persisted).IsNotNull();
        Check.That(persisted!.State.StateVersion).IsEqualTo(1L);
        Check.That(persisted.State.Updated).IsEqualTo(handler.Manifest.Updated);

        Check.That(await session.UploadNext()).IsTrue();
        Check.That(handler.StateReads).IsEqualTo(0);
        Check.That(session.Session.State.Status).IsEqualTo(DataItemTransferStatus.Complete);
        Check.That(session.Session.State.RealizedBytes).IsEqualTo(16L);
        Check.That(session.Session.State.MissingRanges.Length).IsEqualTo(0);
    }

    [TestCase("replay",true)]
    [TestCase("missing-time",true)]
    [TestCase("version",true)]
    [TestCase("completion",true)]
    [TestCase("identity",false)]
    [TestCase("missing-response",false)]
    [TestCase("malformed",false)]
    [TestCase("wrong-range",false)]
    [TestCase("missing-replayed",false)]
    [TestCase("missing-offset",false)]
    [TestCase("missing-length",false)]
    [TestCase("missing-updated",false)]
    [TestCase("null-range",false)]
    [TestCase("unknown-range",false)]
    [TestCase("stale",false)]
    [TestCase("conflict",false)]
    public async Task UncertainResponsesRefreshAndPersistAuthoritativeState(String scenario , Boolean reportsProgress)
    {
        UploadHandler handler = new() { Scenario = scenario };
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlUploadSession session = await OpenSession(client);

        Check.That(await session.UploadNext()).IsEqualTo(reportsProgress);
        Check.That(handler.StateReads).IsEqualTo(1);
        Check.That(session.Session.State.StateVersion).IsEqualTo(handler.Manifest!.StateVersion);
        Check.That(session.Session.State.RealizedBytes).IsEqualTo(handler.Manifest.ToState().RealizedBytes);
        DataControlTransferSessionInfo? persisted = await client.GetLocalSession(session.Session.SessionId);
        Check.That(persisted).IsNotNull();
        Check.That(persisted!.State.StateVersion).IsEqualTo(handler.Manifest.StateVersion);
    }

    [Test]
    public async Task LostResponseReconcilesThroughReplay()
    {
        UploadHandler handler = new() { Scenario = "lost" };
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlUploadSession session = await OpenSession(client);

        try { Check.That(await session.UploadNext()).IsFalse(); }
        catch (HttpRequestException) { }
        Check.That(session.Session.State.StateVersion).IsEqualTo(0L);
        Check.That(await session.UploadNext()).IsTrue();
        Check.That(handler.StateReads).IsEqualTo(1);
        Check.That(session.Session.State.StateVersion).IsEqualTo(1L);
        Check.That((await client.GetLocalSession(session.Session.SessionId))!.State.StateVersion).IsEqualTo(1L);
    }

    [Test]
    public async Task FailedRefreshLeavesLocalStateUnchanged()
    {
        UploadHandler handler = new() { Scenario = "refresh-failure" };
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlUploadSession session = await OpenSession(client);

        Check.That(await session.UploadNext()).IsFalse();
        Check.That(handler.StateReads).IsEqualTo(1);
        Check.That(session.Session.State.StateVersion).IsEqualTo(0L);
        Check.That((await client.GetLocalSession(session.Session.SessionId))!.State.StateVersion).IsEqualTo(0L);
    }

    [TestCase("refresh-wrong-mode")]
    [TestCase("refresh-wrong-length")]
    [TestCase("refresh-wrong-hash")]
    [TestCase("refresh-older")]
    public async Task IncompatibleRefreshDoesNotReplaceLocalSession(String scenario)
    {
        UploadHandler handler = new() { Scenario = scenario };
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlUploadSession session = await OpenSession(client);

        Check.That(await session.UploadNext()).IsFalse();
        Check.That(handler.StateReads).IsEqualTo(1);
        Check.That(session.Session.State.StateVersion).IsEqualTo(0L);
        Check.That((await client.GetLocalSession(session.Session.SessionId))!.State.StateVersion).IsEqualTo(0L);
    }

    [Test]
    public async Task PersistenceFailureLeavesSessionUnchangedAndRetryReconciles()
    {
        UploadHandler handler = new();
        using HttpClient http = new(handler);
        using DataControlClient client = CreateClient(http);
        await using DataControlUploadSession session = await OpenSession(client);
        String path = session.Session.WorkingDirectory;
        Directory.Delete(path,true);
        await File.WriteAllTextAsync(path,"block directory creation");

        Check.That(await session.UploadNext()).IsFalse();
        Check.That(session.Session.State.StateVersion).IsEqualTo(0L);
        File.Delete(path);
        Check.That(await session.UploadNext()).IsTrue();
        Check.That(handler.StateReads).IsEqualTo(1);
        Check.That(session.Session.State.StateVersion).IsEqualTo(1L);
        Check.That((await client.GetLocalSession(session.Session.SessionId))!.State.StateVersion).IsEqualTo(1L);
    }

    private DataControlClient CreateClient(HttpClient http)
    {
        return new DataControlClient("https://data.example.test/",options:new DataControlClientOptions
        {
            HttpClient = http,
            WorkingDirectoryOptions = new DataControlClientWorkingDirectoryOptions { RootPath = this.root },
        });
    }

    private async Task<DataControlUploadSession> OpenSession(DataControlClient client)
    {
        String path = Path.Combine(this.root,"source.bin");
        await File.WriteAllBytesAsync(path,Enumerable.Range(0,16).Select(index => (Byte)index).ToArray());
        BinaryItem item = new();
        item.SetFILE(path);
        Check.That(await item.SetContentStreamed(true)).IsTrue();
        DataControlUploadSession? session = await client.OpenUpload(item,options:new DataControlUploadOptions
        {
            RequestedSegmentSizePolicy = new DataItemTransferSegmentSizePolicy { MinSegmentBytes = 1 , PreferredSegmentBytes = 8 , MaxSegmentBytes = 8 },
        });
        Check.That(session).IsNotNull();
        return session!;
    }

    private sealed class UploadHandler : HttpMessageHandler
    {
        public String Scenario { get; init; } = String.Empty;
        public DataItemTransferManifest? Manifest { get; private set; }
        public Int32 StateReads { get; private set; }
        public Int32 Writes { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request , CancellationToken cancel)
        {
            switch(request.RequestUri!.AbsolutePath)
            {
                case "/OpenUploadTransfer":
                    OpenUploadTransferRequest open = JsonUtility.Parse<OpenUploadTransferRequest>(await request.Content!.ReadAsStringAsync(cancel))!;
                    DateTimeOffset now = DateTimeOffset.UtcNow;
                    this.Manifest = new DataItemTransferManifest
                    {
                        SessionID = open.SessionID, ItemID = open.ItemID, ObjectSHA512 = open.ObjectSHA512,
                        ObjectInfo = open.ObjectInfo, StreamLength = open.StreamLength, Mode = DataItemTransferMode.Upload,
                        Status = DataItemTransferStatus.Open, SegmentSizePolicy = open.RequestedSegmentSizePolicy!, Created = now, Updated = now,
                    };
                    return Json(new OpenUploadTransferResponse { State = this.Manifest.ToState() });
                case "/PutTransferSegment":
                    Byte[] envelope = await request.Content!.ReadAsByteArrayAsync(cancel);
                    PutTransferSegmentRequest put = JsonUtility.Deserialize<PutTransferSegmentRequest>(TransferEnvelope.GetMetadataBytes(envelope).ToArray())!;
                    DataItemTransferRange range = new() { Offset = put.Offset , Length = put.Length };
                    if(this.Scenario == "stale") { return new HttpResponseMessage(HttpStatusCode.PreconditionFailed) { Content = new StringContent(String.Empty) }; }
                    if(this.Scenario == "conflict") { return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(String.Empty) }; }
                    this.Writes++;
                    Boolean replay = this.Manifest!.CanReadRange(range);
                    if(!replay) { this.Manifest = this.Manifest.WithAddedRange(range); }
                    if(this.Scenario == "lost" && this.Writes == 1) { throw new HttpRequestException("Simulated lost response."); }
                    if(this.Scenario == "malformed") { return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{invalid") }; }
                    PutTransferSegmentResponse response = new()
                    {
                        SessionID = put.SessionID, ItemID = put.ItemID, Range = range, Accepted = true,
                        StateVersion = this.Manifest.StateVersion, CompletedCoverage = this.Manifest.HasFullCoverage(), Updated = this.Manifest.Updated, Replayed = replay,
                    };
                    response = this.Scenario switch
                    {
                        "replay" => response with { Replayed = true },
                        "refresh-failure" => response with { Replayed = true },
                        "refresh-wrong-mode" or "refresh-wrong-length" or "refresh-wrong-hash" or "refresh-older" => response with { Replayed = true },
                        "missing-time" => response with { Updated = null },
                        "version" => response with { StateVersion = response.StateVersion + 1 },
                        "completion" => response with { CompletedCoverage = !response.CompletedCoverage },
                        "identity" => response with { ItemID = Guid.NewGuid() },
                        "wrong-range" => response with { Range = new DataItemTransferRange { Offset = range.Offset + 1 , Length = range.Length } },
                        "missing-response" => new PutTransferSegmentResponse(),
                        _ => response,
                    };
                    if(this.Scenario is "missing-replayed" or "missing-offset" or "missing-length" or "missing-updated" or "null-range" or "unknown-range")
                    {
                        System.Text.Json.Nodes.JsonObject json = System.Text.Json.Nodes.JsonNode.Parse(JsonUtility.ToJsonString(response))!.AsObject();
                        switch(this.Scenario)
                        {
                            case "missing-replayed": json.Remove("Replayed"); break;
                            case "missing-offset": json["Range"]!.AsObject().Remove("Offset"); break;
                            case "missing-length": json["Range"]!.AsObject().Remove("Length"); break;
                            case "missing-updated": json.Remove("Updated"); break;
                            case "null-range": json["Range"] = null; break;
                            case "unknown-range": json["Range"]!.AsObject()["Unexpected"] = true; break;
                        }
                        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json.ToJsonString(),Encoding.UTF8,"application/json") };
                    }
                    return Json(response);
                case "/GetTransferState":
                    this.StateReads++;
                    if(this.Scenario == "refresh-failure") { return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable) { Content = new StringContent(String.Empty) }; }
                    DataItemTransferState state = this.Manifest!.ToState();
                    state = this.Scenario switch
                    {
                        "refresh-wrong-mode" => state with { Mode = DataItemTransferMode.PublishedRead },
                        "refresh-wrong-length" => state with { StreamLength = state.StreamLength + 1 },
                        "refresh-wrong-hash" => state with { ObjectSHA512 = new Byte[64] },
                        "refresh-older" => state with { StateVersion = -1 },
                        _ => state,
                    };
                    return Json(new GetTransferStateResponse { State = state });
                default:
                    return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(String.Empty) };
            }
        }

        private static HttpResponseMessage Json<T>(T value)
        {
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonUtility.ToJsonString(value),Encoding.UTF8,"application/json") };
        }
    }
}
