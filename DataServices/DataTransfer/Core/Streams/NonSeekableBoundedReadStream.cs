namespace KusDepot.Data.Transfer;

/**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/main/*'/>*/
public sealed class NonSeekableBoundedReadStream : Stream
{
    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/field[@name="Source"]/*'/>*/
    private readonly Stream Source;

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/field[@name="LengthLimit"]/*'/>*/
    private readonly Int64 LengthLimit;

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/field[@name="LeaveOpen"]/*'/>*/
    private readonly Boolean LeaveOpen;

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/field[@name="PositionValue"]/*'/>*/
    private Int64 PositionValue;

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/field[@name="disposed"]/*'/>*/
    private Boolean disposed;

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/constructor[@name="Constructor"]/*'/>*/
    public NonSeekableBoundedReadStream(Stream source , Int64 length , Boolean leaveopen = false)
    {
        ArgumentNullException.ThrowIfNull(source); ArgumentOutOfRangeException.ThrowIfNegative(length);

        if(source.CanRead is false) { throw new ArgumentException("Source stream must be readable.",nameof(source)); }

        this.Source = source; this.LengthLimit = length; this.LeaveOpen = leaveopen;
    }

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/property[@name="CanRead"]/*'/>*/
    public override Boolean CanRead => this.Source.CanRead;

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/property[@name="CanSeek"]/*'/>*/
    public override Boolean CanSeek => false;

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/property[@name="CanWrite"]/*'/>*/
    public override Boolean CanWrite => false;

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/property[@name="Length"]/*'/>*/
    public override Int64 Length => this.LengthLimit;

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/property[@name="Position"]/*'/>*/
    public override Int64 Position
    {
        get => this.PositionValue;

        set => throw new NotSupportedException();
    }

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="Flush"]/*'/>*/
    public override void Flush() => this.Source.Flush();

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="FlushAsync"]/*'/>*/
    public override Task FlushAsync(CancellationToken cancel) => this.Source.FlushAsync(cancel);

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="Read"]/*'/>*/
    public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count)
    {
        ArgumentNullException.ThrowIfNull(buffer); ArgumentOutOfRangeException.ThrowIfNegative(offset); ArgumentOutOfRangeException.ThrowIfNegative(count);

        if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

        return this.Read(buffer.AsSpan(offset,count));
    }

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="ReadSpan"]/*'/>*/
    public override Int32 Read(Span<Byte> buffer)
    {
        Int64 remaining = this.LengthLimit - this.PositionValue;

        if(remaining <= 0 || buffer.IsEmpty) { return 0; }

        Int32 requested = (Int32)Math.Min(buffer.Length,remaining);

        Int32 read = this.Source.Read(buffer[..requested]);

        this.PositionValue += read; return read;
    }

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="ReadAsync"]/*'/>*/
    public override Task<Int32> ReadAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(buffer); ArgumentOutOfRangeException.ThrowIfNegative(offset); ArgumentOutOfRangeException.ThrowIfNegative(count);

        if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

        return this.ReadAsync(buffer.AsMemory(offset,count),cancel).AsTask();
    }

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="ReadAsyncMemory"]/*'/>*/
    public override async ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancel = default)
    {
        Int64 remaining = this.LengthLimit - this.PositionValue;

        if(remaining <= 0 || buffer.IsEmpty) { return 0; }

        Int32 requested = (Int32)Math.Min(buffer.Length,remaining);

        Int32 read = await this.Source.ReadAsync(buffer[..requested],cancel).ConfigureAwait(false);

        this.PositionValue += read; return read;
    }

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="Seek"]/*'/>*/
    public override Int64 Seek(Int64 offset , SeekOrigin origin) { throw new NotSupportedException(); }

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="SetLength"]/*'/>*/
    public override void SetLength(Int64 value) { throw new NotSupportedException(); }

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="Write"]/*'/>*/
    public override void Write(Byte[] buffer , Int32 offset , Int32 count) { throw new NotSupportedException(); }

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="WriteSpan"]/*'/>*/
    public override void Write(ReadOnlySpan<Byte> buffer) { throw new NotSupportedException(); }

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="WriteAsync"]/*'/>*/
    public override Task WriteAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancel) { throw new NotSupportedException(); }

    /**<include file='NonSeekableBoundedReadStream.xml' path='NonSeekableBoundedReadStream/class[@name="NonSeekableBoundedReadStream"]/method[@name="WriteAsyncMemory"]/*'/>*/
    public override ValueTask WriteAsync(ReadOnlyMemory<Byte> buffer , CancellationToken cancel = default) { throw new NotSupportedException(); }

    ///<inheritdoc/>
    protected override void Dispose(Boolean disposing)
    {
        if(disposing is false || this.disposed) { return; }

        this.disposed = true;

        if(this.LeaveOpen is false) { this.Source.Dispose(); }

        base.Dispose(disposing);
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        if(this.disposed) { return; }

        this.disposed = true;

        if(this.LeaveOpen is false) { await this.Source.DisposeAsync().ConfigureAwait(false); }

        await base.DisposeAsync().ConfigureAwait(false);
    }
}
