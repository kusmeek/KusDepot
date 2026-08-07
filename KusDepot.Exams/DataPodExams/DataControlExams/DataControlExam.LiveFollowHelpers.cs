namespace KusDepot.DataPodExams;

public partial class DataControlExam
{
    private static async Task<Byte[]> ReadExactlyAsync(Stream stream , Int32 length , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentOutOfRangeException.ThrowIfNegative(length);

        Byte[] buffer = new Byte[length];
        Int32 offset = 0;

        while(offset < length)
        {
            Int32 read = await stream.ReadAsync(buffer.AsMemory(offset,length - offset),cancel).ConfigureAwait(false);

            if(read <= 0) { throw new EndOfStreamException($"Expected {length} bytes but reached end of stream after {offset} bytes."); }

            offset += read;
        }

        return buffer;
    }

    private static async Task<Boolean> RemainsIncompleteAsync(Task task , Int32 delayMilliseconds = 250 , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(task);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(delayMilliseconds);

        Task completed = await Task.WhenAny(task,Task.Delay(delayMilliseconds,cancel)).ConfigureAwait(false);

        return ReferenceEquals(completed,task) is false;
    }

    private sealed class ControllableProducerStream : Stream
    {
        private readonly Channel<Byte[]?> channel = Channel.CreateUnbounded<Byte[]?>(new UnboundedChannelOptions(){ SingleReader = true, SingleWriter = false });
        private readonly SemaphoreSlim gate = new(1,1);
        private Byte[]? currentSegment;
        private Int32 currentOffset;
        private Exception? fault;
        private Boolean completed;
        private Boolean disposed;

        public Task PublishAsync(Byte[] segment , CancellationToken cancel = default)
        {
            ArgumentNullException.ThrowIfNull(segment);

            if(segment.Length == 0) { return Task.CompletedTask; }

            ObjectDisposedException.ThrowIf(this.disposed,this);

            return this.channel.Writer.WriteAsync((Byte[])segment.Clone(),cancel).AsTask();
        }

        public void Complete()
        {
            if(this.disposed || this.completed) { return; }

            this.completed = true;
            this.channel.Writer.TryComplete();
        }

        public void Fault(Exception error)
        {
            ArgumentNullException.ThrowIfNull(error);

            if(this.disposed || this.completed) { return; }

            this.fault = error;
            this.completed = true;
            this.channel.Writer.TryComplete(error);
        }

        public override Boolean CanRead => this.disposed is false;

        public override Boolean CanSeek => false;

        public override Boolean CanWrite => false;

        public override Int64 Length => throw new NotSupportedException();

        public override Int64 Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush() {}

        public override Task FlushAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count)
        {
            ArgumentNullException.ThrowIfNull(buffer);
            ArgumentOutOfRangeException.ThrowIfNegative(offset);
            ArgumentOutOfRangeException.ThrowIfNegative(count);

            if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

            return this.ReadAsync(buffer.AsMemory(offset,count),CancellationToken.None).AsTask().GetAwaiter().GetResult();
        }

        public override async ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancel = default)
        {
            ObjectDisposedException.ThrowIf(this.disposed,this);

            if(buffer.IsEmpty) { return 0; }

            await this.gate.WaitAsync(cancel).ConfigureAwait(false);

            try
            {
                while(true)
                {
                    if(this.currentSegment is not null)
                    {
                        Int32 remaining = this.currentSegment.Length - this.currentOffset;

                        if(remaining > 0)
                        {
                            Int32 copied = Math.Min(buffer.Length,remaining);
                            this.currentSegment.AsMemory(this.currentOffset,copied).CopyTo(buffer);
                            this.currentOffset += copied;

                            if(this.currentOffset >= this.currentSegment.Length)
                            {
                                this.currentSegment = null;
                                this.currentOffset = 0;
                            }

                            return copied;
                        }

                        this.currentSegment = null;
                        this.currentOffset = 0;
                    }

                    if(await this.channel.Reader.WaitToReadAsync(cancel).ConfigureAwait(false) is false)
                    {
                        if(this.fault is not null) { throw this.fault; }
                        return 0;
                    }

                    if(this.channel.Reader.TryRead(out Byte[]? next) is false) { continue; }

                    if(next is null) { continue; }

                    this.currentSegment = next;
                    this.currentOffset = 0;
                }
            }
            finally { this.gate.Release(); }
        }

        public override Int64 Seek(Int64 offset , SeekOrigin origin) { throw new NotSupportedException(); }

        public override void SetLength(Int64 value) { throw new NotSupportedException(); }

        public override void Write(Byte[] buffer , Int32 offset , Int32 count) { throw new NotSupportedException(); }

        public override void Write(ReadOnlySpan<Byte> buffer) { throw new NotSupportedException(); }

        public override Task WriteAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancel) { throw new NotSupportedException(); }

        public override ValueTask WriteAsync(ReadOnlyMemory<Byte> buffer , CancellationToken cancel = default) { throw new NotSupportedException(); }

        protected override void Dispose(Boolean disposing)
        {
            if(disposing && this.disposed is false)
            {
                this.disposed = true;
                this.completed = true;
                this.channel.Writer.TryComplete();
                this.gate.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
