using KusDepot.Exams.Cryptography;

namespace KusDepot.Exams;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public sealed class RuntimeAsyncPipelineExam
{
    [TestCase(DataIsolationMode.Enabled)]
    [TestCase(DataIsolationMode.Disabled)]
    public async Task DataIsolationScope_FlowsAcrossSuspension_AndRestoresNestedScope(DataIsolationMode mode)
    {
        using IDisposable scope = DataIsolation.BeginScope(mode);
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<Boolean> observed = ObserveIsolationAfterSuspensionAsync(release.Task);

        Assert.That(observed.IsCompleted, Is.False);
        release.SetResult(true);

        Assert.That(await observed.ConfigureAwait(false), Is.EqualTo(mode == DataIsolationMode.Enabled));
        Assert.That(DataIsolation.IsEnabled(), Is.EqualTo(mode == DataIsolationMode.Enabled));

        using(DataIsolation.BeginScope(mode == DataIsolationMode.Enabled ? DataIsolationMode.Disabled : DataIsolationMode.Enabled))
        {
            Assert.That(DataIsolation.IsEnabled(), Is.EqualTo(mode != DataIsolationMode.Enabled));
        }

        Assert.That(DataIsolation.IsEnabled(), Is.EqualTo(mode == DataIsolationMode.Enabled));
    }

    [Test]
    public async Task EncryptionEnvelope_ForcedAsyncIo_MultiChunkRoundtrip_Succeeds()
    {
        using X509Certificate2 certificate = CreateCertificate(Guid.NewGuid(),"RuntimeAsync.EncryptionEnvelope",2048,2,null)!;
        Byte[] plaintext = Enumerable.Range(0,40_000).Select(_ => (Byte)(_ % 251)).ToArray();
        await using YieldingReadStream input = new(plaintext);
        await using MemoryStream envelope = new();

        Boolean encrypted = await Utility.EncryptAsync(input,envelope,certificate,chunksizepower:12).ConfigureAwait(false);
        Assert.That(encrypted, Is.True);

        envelope.Position = 0;
        await using YieldingReadStream encryptedInput = new(envelope.ToArray());
        await using MemoryStream output = new();
        Boolean decrypted = await Utility.DecryptAsync(encryptedInput,output,certificate).ConfigureAwait(false);

        Assert.That(decrypted, Is.True);
        Assert.That(output.ToArray(), Is.EqualTo(plaintext));
    }

    [Test]
    public async Task DataItemSecurityEnvelope_ForcedAsyncIo_MultiChunkRoundtrip_Succeeds()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("RuntimeAsync.DataItemSecurityEnvelope");
        Byte[] plaintext = Enumerable.Range(0,40_000).Select(_ => (Byte)(_ % 239)).ToArray();
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);
        await using YieldingReadStream input = new(plaintext);
        await using MemoryStream envelope = new();

        Boolean encrypted = await DataItemSecurityEnvelopeV1TestHelpers.EncryptStreamAsync(input,envelope,[issuer.Recipient],issuer.Object,rootContext,12).ConfigureAwait(false);
        Assert.That(encrypted, Is.True);

        envelope.Position = 0;
        await using YieldingReadStream encryptedInput = new(envelope.ToArray());
        await using MemoryStream output = new();
        Boolean decrypted = await DataItemSecurityEnvelopeV1TestHelpers.DecryptStreamAsync(encryptedInput,output,issuer.Certificate,rootContext).ConfigureAwait(false);

        Assert.That(decrypted, Is.True);
        Assert.That(output.ToArray(), Is.EqualTo(plaintext));
    }

    [Test]
    public async Task EncryptionEnvelope_WriterFailure_CancelsBlockedSourceRead()
    {
        using X509Certificate2 certificate = CreateCertificate(Guid.NewGuid(),"RuntimeAsync.EncryptionEnvelope.Failure",2048,2,null)!;
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(10));
        Byte[] plaintext = new Byte[32_768];
        ReadGateStream input = new(plaintext);
        await using FailingWriteStream output = new(input,0);

        Boolean? result = null;
        try { result = await Utility.EncryptAsync(input,output,certificate,chunksizepower:12,cancel:cancellation.Token).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false); }
        catch(IOException exception) { Assert.That(exception.Message, Is.EqualTo("Injected async write failure.")); }

        Assert.That(result is null || result is false, Is.True);
        Assert.That(input.BlockedReadCanceled, Is.True);
    }

    [Test]
    public async Task DataItemSecurityEnvelope_WriterFailure_CancelsBlockedSourceRead()
    {
        using var issuer = DataItemSecurityEnvelopeV1TestHelpers.CreateParty("RuntimeAsync.DataItemSecurityEnvelope.Failure");
        using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(10));
        Byte[] plaintext = new Byte[32_768];
        ReadGateStream input = new(plaintext);
        await using FailingWriteStream output = new(input,2);
        Byte[] rootContext = DataItemSecurityEnvelopeV1TestHelpers.BuildRootContext(issuer.ObjectId);

        Boolean result = await DataItemSecurityEnvelopeV1TestHelpers.EncryptStreamAsync(input,output,[issuer.Recipient],issuer.Object,rootContext,12,cancellation.Token).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

        Assert.That(result, Is.False);
        Assert.That(input.BlockedReadCanceled, Is.True);
    }

    private static async Task<Boolean> ObserveIsolationAfterSuspensionAsync(Task release)
    {
        await release.ConfigureAwait(false);
        return DataIsolation.IsEnabled();
    }

    private sealed class YieldingReadStream : Stream
    {
        private readonly MemoryStream source;

        internal YieldingReadStream(Byte[] content) => source = new MemoryStream(content,false);

        public override Boolean CanRead => source.CanRead;
        public override Boolean CanSeek => source.CanSeek;
        public override Boolean CanWrite => false;
        public override Int64 Length => source.Length;
        public override Int64 Position { get => source.Position; set => source.Position = value; }
        public override void Flush() { }
        public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count) => source.Read(buffer,offset,count);
        public override Int32 Read(Span<Byte> buffer) => source.Read(buffer);

        public override async ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            cancellationToken.ThrowIfCancellationRequested();
            return await source.ReadAsync(buffer,cancellationToken).ConfigureAwait(false);
        }

        public override Int64 Seek(Int64 offset , SeekOrigin origin) => source.Seek(offset,origin);
        public override void SetLength(Int64 value) => throw new NotSupportedException();
        public override void Write(Byte[] buffer , Int32 offset , Int32 count) => throw new NotSupportedException();

        protected override void Dispose(Boolean disposing)
        {
            if(disposing) { source.Dispose(); }
            base.Dispose(disposing);
        }
    }

    private sealed class ReadGateStream : Stream
    {
        private readonly MemoryStream source;
        private readonly TaskCompletionSource<Boolean> readBlocked = new(TaskCreationOptions.RunContinuationsAsynchronously);
        internal ReadGateStream(Byte[] content) => source = new MemoryStream(content,false);
        internal Boolean BlockedReadCanceled { get; private set; }
        internal Task ReadBlocked => readBlocked.Task;

        public override Boolean CanRead => source.CanRead;
        public override Boolean CanSeek => source.CanSeek;
        public override Boolean CanWrite => false;
        public override Int64 Length => source.Length;
        public override Int64 Position { get => source.Position; set => source.Position = value; }
        public override void Flush() { }
        public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count) => source.Read(buffer,offset,count);
        public override Int32 Read(Span<Byte> buffer) => source.Read(buffer);

        public override async ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancellationToken = default)
        {
            if(Position > 0)
            {
                readBlocked.TrySetResult(true);
                try { await Task.Delay(Timeout.Infinite,cancellationToken).ConfigureAwait(false); }
                catch(OperationCanceledException) { BlockedReadCanceled = true; throw; }
            }

            cancellationToken.ThrowIfCancellationRequested();
            return await source.ReadAsync(buffer,cancellationToken).ConfigureAwait(false);
        }

        public override Int64 Seek(Int64 offset , SeekOrigin origin) => source.Seek(offset,origin);
        public override void SetLength(Int64 value) => throw new NotSupportedException();
        public override void Write(Byte[] buffer , Int32 offset , Int32 count) => throw new NotSupportedException();

        protected override void Dispose(Boolean disposing)
        {
            if(disposing) { source.Dispose(); }
            base.Dispose(disposing);
        }
    }

    private sealed class FailingWriteStream : Stream
    {
        private readonly ReadGateStream source;
        private readonly MemoryStream destination = new();
        private readonly Int32 passAsyncWrites;
        private Int32 writes;

        internal FailingWriteStream(ReadGateStream source , Int32 passAsyncWrites)
        {
            this.source = source;
            this.passAsyncWrites = passAsyncWrites;
        }

        public override Boolean CanRead => false;
        public override Boolean CanSeek => false;
        public override Boolean CanWrite => true;
        public override Int64 Length => destination.Length;
        public override Int64 Position { get => destination.Position; set => throw new NotSupportedException(); }
        public override void Flush() => destination.Flush();
        public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count) => throw new NotSupportedException();
        public override Int64 Seek(Int64 offset , SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(Int64 value) => throw new NotSupportedException();
        public override void Write(Byte[] buffer , Int32 offset , Int32 count) => destination.Write(buffer,offset,count);
        public override void Write(ReadOnlySpan<Byte> buffer) => destination.Write(buffer);

        public override async ValueTask WriteAsync(ReadOnlyMemory<Byte> buffer , CancellationToken cancellationToken = default)
        {
            if(Interlocked.Increment(ref writes) > passAsyncWrites)
            {
                await source.ReadBlocked.WaitAsync(TimeSpan.FromSeconds(3),cancellationToken).ConfigureAwait(false);
                throw new IOException("Injected async write failure.");
            }

            await destination.WriteAsync(buffer,cancellationToken).ConfigureAwait(false);
        }

        protected override void Dispose(Boolean disposing)
        {
            if(disposing) { destination.Dispose(); }
            base.Dispose(disposing);
        }
    }
}