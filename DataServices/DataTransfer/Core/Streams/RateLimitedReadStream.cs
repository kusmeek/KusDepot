namespace KusDepot.Data.Transfer;

/**<include file='RateLimitedReadStream.xml' path='RateLimitedReadStream/class[@name="RateLimitedReadStream"]/main/*'/>*/
public sealed class RateLimitedReadStream : Stream
{
    /**<include file='RateLimitedReadStream.xml' path='RateLimitedReadStream/class[@name="RateLimitedReadStream"]/field[@name="MaximumReadSize"]/*'/>*/
    private const Int32 MaximumReadSize = 80 * KiB;

    /**<include file='RateLimitedReadStream.xml' path='RateLimitedReadStream/class[@name="RateLimitedReadStream"]/field[@name="Source"]/*'/>*/
    private readonly Stream Source;

    /**<include file='RateLimitedReadStream.xml' path='RateLimitedReadStream/class[@name="RateLimitedReadStream"]/field[@name="Limiter"]/*'/>*/
    private readonly AsyncByteRateLimiter Limiter;

    /**<include file='RateLimitedReadStream.xml' path='RateLimitedReadStream/class[@name="RateLimitedReadStream"]/field[@name="LeaveOpen"]/*'/>*/
    private readonly Boolean LeaveOpen;

    /**<include file='RateLimitedReadStream.xml' path='RateLimitedReadStream/class[@name="RateLimitedReadStream"]/field[@name="Disposed"]/*'/>*/
    private Boolean Disposed;

    /**<include file='RateLimitedReadStream.xml' path='RateLimitedReadStream/class[@name="RateLimitedReadStream"]/constructor[@name="Constructor"]/*'/>*/
    public RateLimitedReadStream(Stream source , AsyncByteRateLimiter limiter , Boolean leaveopen = true)
    {
        ArgumentNullException.ThrowIfNull(source); ArgumentNullException.ThrowIfNull(limiter);

        if(source.CanRead is false) { throw new ArgumentException("The source stream must be readable.",nameof(source)); }

        this.Source = source; this.Limiter = limiter; this.LeaveOpen = leaveopen;
    }

    ///<inheritdoc/>
    public override Boolean CanRead => this.Disposed is false && this.Source.CanRead;

    ///<inheritdoc/>
    public override Boolean CanSeek => this.Disposed is false && this.Source.CanSeek;

    ///<inheritdoc/>
    public override Boolean CanWrite => false;

    ///<inheritdoc/>
    public override Int64 Length => this.Source.Length;

    ///<inheritdoc/>
    public override Int64 Position
    {
        get => this.Source.Position;

        set => this.Source.Position = value;
    }

    ///<inheritdoc/>
    public override void Flush() { }

    ///<inheritdoc/>
    public override Task FlushAsync(CancellationToken cancel) => Task.CompletedTask;

    ///<inheritdoc/>
    public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count)
    {
        ArgumentNullException.ThrowIfNull(buffer); ArgumentOutOfRangeException.ThrowIfNegative(offset); ArgumentOutOfRangeException.ThrowIfNegative(count);

        if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

        Int64 readstartedtimestamp = Stopwatch.GetTimestamp();

        Int32 read = this.Source.Read(buffer,offset,Math.Min(count,MaximumReadSize));

        if(read > 0) { this.Limiter.WaitAsync(read,readstartedtimestamp,CancellationToken.None).AsTask().GetAwaiter().GetResult(); }

        return read;
    }

    ///<inheritdoc/>
    public override Int32 Read(Span<Byte> buffer)
    {
        Int64 readstartedtimestamp = Stopwatch.GetTimestamp();

        Int32 read = this.Source.Read(buffer[..Math.Min(buffer.Length,MaximumReadSize)]);

        if(read > 0) { this.Limiter.WaitAsync(read,readstartedtimestamp,CancellationToken.None).AsTask().GetAwaiter().GetResult(); }

        return read;
    }

    ///<inheritdoc/>
    public override async Task<Int32> ReadAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(buffer); ArgumentOutOfRangeException.ThrowIfNegative(offset); ArgumentOutOfRangeException.ThrowIfNegative(count);

        if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

        Int64 readstartedtimestamp = Stopwatch.GetTimestamp();

        Int32 read = await this.Source.ReadAsync(buffer.AsMemory(offset,Math.Min(count,MaximumReadSize)),cancel).ConfigureAwait(false);

        if(read > 0) { await this.Limiter.WaitAsync(read,readstartedtimestamp,cancel).ConfigureAwait(false); }

        return read;
    }

    ///<inheritdoc/>
    public override async ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancel = default)
    {
        Int64 readstartedtimestamp = Stopwatch.GetTimestamp();

        Int32 read = await this.Source.ReadAsync(buffer[..Math.Min(buffer.Length,MaximumReadSize)],cancel).ConfigureAwait(false);

        if(read > 0) { await this.Limiter.WaitAsync(read,readstartedtimestamp,cancel).ConfigureAwait(false); }

        return read;
    }

    ///<inheritdoc/>
    public override Int64 Seek(Int64 offset , SeekOrigin origin) => this.Source.Seek(offset,origin);

    ///<inheritdoc/>
    public override void SetLength(Int64 value) => throw new NotSupportedException();

    ///<inheritdoc/>
    public override void Write(Byte[] buffer , Int32 offset , Int32 count) => throw new NotSupportedException();

    /**<include file='RateLimitedReadStream.xml' path='RateLimitedReadStream/class[@name="RateLimitedReadStream"]/method[@name="Dispose"]/*'/>*/
    protected override void Dispose(Boolean disposing)
    {
        if(this.Disposed is false)
        {
            this.Disposed = true;

            if(disposing && this.LeaveOpen is false) { this.Source.Dispose(); }
        }

        base.Dispose(disposing);
    }

    /**<include file='RateLimitedReadStream.xml' path='RateLimitedReadStream/class[@name="RateLimitedReadStream"]/method[@name="DisposeAsync"]/*'/>*/
    public override async ValueTask DisposeAsync()
    {
        if(this.Disposed is false)
        {
            this.Disposed = true;

            if(this.LeaveOpen is false) { await this.Source.DisposeAsync().ConfigureAwait(false); }
        }

        GC.SuppressFinalize(this);
    }
}
