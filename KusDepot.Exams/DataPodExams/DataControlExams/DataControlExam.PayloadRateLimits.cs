namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    [Test]
    public async Task ClientPayloadRateLimitsEnforceIndependentUploadAndDownloadRates()
    {
        String workingRoot = CreateClientWorkingRoot();
        String sourcePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Client.PayloadRateLimits.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(1024 * 1024,0xB3);
        Int64 uploadBytesPerSecond = 128L * 1024;
        Int64 downloadBytesPerSecond = 64L * 1024;
        TimeSpan minimumUploadDuration = TimeSpan.FromSeconds(payload.Length / (Double)uploadBytesPerSecond);
        TimeSpan minimumDownloadDuration = TimeSpan.FromSeconds(payload.Length / (Double)downloadBytesPerSecond);
        Guid itemID = Guid.Empty;
        String? downloadedStreamPath = null;

        try
        {
            using DataControlClient client = new(this.EndpointLocator!,options:new DataControlClientOptions
            {
                UploadPayloadRateLimit = uploadBytesPerSecond,
                DownloadPayloadRateLimit = downloadBytesPerSecond,
                WorkingDirectoryOptions = new DataControlClientWorkingDirectoryOptions(){ RootPath = workingRoot },
            });

            BinaryItem item = await CreateClientBinaryItem(sourcePath,payload);
            itemID = item.GetID()!.Value;

            Stopwatch uploadTimer = Stopwatch.StartNew();
            DataControlUploadCompletionResult? uploaded = await client.Upload(item,this.WriteToken);
            uploadTimer.Stop();

            Check.That(uploaded).IsNotNull();
            Check.That(uploadTimer.Elapsed >= minimumUploadDuration).IsTrue();

            Stopwatch downloadTimer = Stopwatch.StartNew();
            DataControlDownloadCompletionResult? downloaded = await client.Download(itemID,this.ReadToken);
            downloadTimer.Stop();

            Check.That(downloaded).IsNotNull();
            Check.That((await File.ReadAllBytesAsync(downloaded!.StreamPath)).SequenceEqual(payload)).IsTrue();
            Check.That(downloadTimer.Elapsed >= minimumDownloadDuration).IsTrue();

            downloadedStreamPath = downloaded.StreamPath;
        }
        finally
        {
            if(File.Exists(sourcePath)) { File.Delete(sourcePath); }
            if(itemID != Guid.Empty)
            {
                using DataControlClient cleanup = new(this.EndpointLocator!);
                _ = await cleanup.Delete(itemID,this.WriteToken);
            }
            DeleteWorkingDirectory(workingRoot);
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }
    }

    private const Int64 ServerUploadPayloadRateLimitBytesPerSecond = 64L * 1024;
    private const Int64 ServerDownloadPayloadRateLimitBytesPerSecond = 32L * 1024;

    [Test]
    [Explicit("Requires server PayloadRateLimits.UploadBytesPerSecond=65536.")]
    public async Task ServerPayloadRateLimitPacesOpenStreamObjectPayload()
    {
        const Int32 objectPayloadLength = 256 * 1024;
        Byte[] objectPayload = CreateSegmentedTransferPayload(objectPayloadLength,0xB5);
        OpenStreamTransferRequest open = CreateOpenStreamTransferRequest() with
        {
            ObjectPayload = objectPayload,
            ObjectSHA512 = SHA512.HashData(objectPayload),
        };

        using DataControlClient client = new(this.EndpointLocator!);

        try
        {
            Stopwatch timer = Stopwatch.StartNew();
            DataControlResponse<OpenStreamTransferResponse> opened = await client.OpenStreamTransfer(open,this.WriteToken);
            timer.Stop();

            Check.That((Int32)opened.StatusCode).IsEqualTo(StatusCodes.Status200OK);
            Check.That(timer.Elapsed >= TimeSpan.FromSeconds(4)).IsTrue();
        }
        finally
        {
            try { await client.AbortTransfer(new AbortTransferRequest(){ SessionID = open.SessionID , ItemID = open.ItemID },this.WriteToken); }
            catch { }
        }
    }

    [Test]
    [Explicit("Requires server PayloadRateLimits.UploadBytesPerSecond=65536 and DownloadBytesPerSecond=32768.")]
    public async Task ServerPayloadRateLimitsEnforceIndependentUploadAndDownloadRates()
    {
        String workingRoot = CreateClientWorkingRoot();
        String sourcePath = Path.Combine(Path.GetTempPath(),$"KusDepot.Server.PayloadRateLimits.{Guid.NewGuid():N}.bin");
        Byte[] payload = CreateSegmentedTransferPayload(1024 * 1024,0xB4);
        TimeSpan minimumUploadDuration = TimeSpan.FromSeconds(payload.Length / (Double)ServerUploadPayloadRateLimitBytesPerSecond);
        TimeSpan minimumDownloadDuration = TimeSpan.FromSeconds(payload.Length / (Double)ServerDownloadPayloadRateLimitBytesPerSecond);
        Guid itemID = Guid.Empty;
        String? downloadedStreamPath = null;

        try
        {
            using DataControlClient client = new(this.EndpointLocator!,options:new DataControlClientOptions
            {
                WorkingDirectoryOptions = new DataControlClientWorkingDirectoryOptions(){ RootPath = workingRoot },
            });

            BinaryItem item = await CreateClientBinaryItem(sourcePath,payload);
            itemID = item.GetID()!.Value;

            Stopwatch uploadTimer = Stopwatch.StartNew();
            DataControlUploadCompletionResult? uploaded = await client.Upload(item,this.WriteToken);
            uploadTimer.Stop();

            Check.That(uploaded).IsNotNull();
            Check.That(uploadTimer.Elapsed >= minimumUploadDuration).IsTrue();

            Stopwatch downloadTimer = Stopwatch.StartNew();
            DataControlDownloadCompletionResult? downloaded = await client.Download(itemID,this.ReadToken);
            downloadTimer.Stop();

            Check.That(downloaded).IsNotNull();
            Check.That((await File.ReadAllBytesAsync(downloaded!.StreamPath)).SequenceEqual(payload)).IsTrue();
            Check.That(downloadTimer.Elapsed >= minimumDownloadDuration).IsTrue();

            downloadedStreamPath = downloaded.StreamPath;
        }
        finally
        {
            if(File.Exists(sourcePath)) { File.Delete(sourcePath); }
            if(itemID != Guid.Empty)
            {
                using DataControlClient cleanup = new(this.EndpointLocator!);
                _ = await cleanup.Delete(itemID,this.WriteToken);
            }
            DeleteWorkingDirectory(workingRoot);
            DeleteWorkingDirectoryForPath(downloadedStreamPath);
        }
    }
}
