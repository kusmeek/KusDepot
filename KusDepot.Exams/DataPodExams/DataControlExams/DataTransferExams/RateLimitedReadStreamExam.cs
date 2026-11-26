using KusDepot.Data;

namespace KusDepot.DataServices;

[TestFixture]
[Parallelizable(ParallelScope.All)]
internal class RateLimitedReadStreamExam
{
    [Test]
    public async Task ReadAsync_PacesReturnedBytes_And_LeavesSourceOpen()
    {
        Byte[] payload = Enumerable.Range(0,64).Select(i => unchecked((Byte)i)).ToArray();
        using MemoryStream source = new(payload,false);
        AsyncByteRateLimiter limiter = new(640);
        Stopwatch stopwatch = Stopwatch.StartNew();

        using(RateLimitedReadStream stream = new(source,limiter,leaveopen:true))
        {
            using MemoryStream destination = new();

            await stream.CopyToAsync(destination);

            Assert.That(destination.ToArray(), Is.EqualTo(payload));
        }

        Assert.That(stopwatch.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(70)));
        Assert.That(() => source.ReadByte(), Throws.Nothing);
    }

    [Test]
    public async Task Reads_CapEachSourceReadAt80KiB()
    {
        const Int32 maximumReadSize = 80 * 1024;
        Byte[] payload = Enumerable.Range(0,200 * 1024).Select(i => unchecked((Byte)i)).ToArray();
        using MemoryStream source = new(payload,false);
        AsyncByteRateLimiter limiter = new(100L * 1024 * 1024);
        using RateLimitedReadStream stream = new(source,limiter,leaveopen:true);
        Byte[] result = new Byte[payload.Length];

        Int32 firstRead = stream.Read(result,0,result.Length);
        Int32 secondRead = await stream.ReadAsync(result.AsMemory(firstRead,result.Length - firstRead));
        Int32 thirdRead = stream.Read(result.AsSpan(firstRead + secondRead));

        Assert.That(firstRead, Is.EqualTo(maximumReadSize));
        Assert.That(secondRead, Is.EqualTo(maximumReadSize));
        Assert.That(thirdRead, Is.EqualTo(payload.Length - (2 * maximumReadSize)));
        Assert.That(result, Is.EqualTo(payload));
    }

    [Test]
    public async Task ConcurrentWaits_ShareAggregateByteRate()
    {
        AsyncByteRateLimiter limiter = new(1024);
        Stopwatch stopwatch = Stopwatch.StartNew();

        await Task.WhenAll(limiter.WaitAsync(128).AsTask(),limiter.WaitAsync(128).AsTask());

        Assert.That(stopwatch.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(200)));
    }

    [Test]
    public async Task Cancellation_DoesNotReclaimReservedPacingSlot()
    {
        AsyncByteRateLimiter limiter = new(1024);
        using CancellationTokenSource cancellation = new();
        Stopwatch stopwatch = Stopwatch.StartNew();
        Task canceledWait = limiter.WaitAsync(256,cancellation.Token).AsTask();

        await Task.Delay(25);
        cancellation.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(async () => await canceledWait);

        await limiter.WaitAsync(256);
        stopwatch.Stop();

        Assert.That(stopwatch.Elapsed, Is.GreaterThanOrEqualTo(TimeSpan.FromMilliseconds(400)));
    }

    [Test]
    public void Constructor_RejectsNonPositiveByteRate()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncByteRateLimiter(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncByteRateLimiter(-1));
    }
}
