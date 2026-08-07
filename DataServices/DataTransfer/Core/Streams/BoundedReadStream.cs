namespace KusDepot.Data.Transfer;

/**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/main/*'/>*/
public sealed class BoundedReadStream : Stream
{
    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/field[@name="Source"]/*'/>*/
    private readonly Stream Source;

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/field[@name="Origin"]/*'/>*/
    private readonly Int64 Origin;

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/field[@name="WindowLength"]/*'/>*/
    private readonly Int64 WindowLength;

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/field[@name="LeaveOpen"]/*'/>*/
    private readonly Boolean LeaveOpen;

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/field[@name="disposed"]/*'/>*/
    private Boolean disposed;

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/constructor[@name="Constructor"]/*'/>*/
    public BoundedReadStream(Stream source , Int64 origin , Int64 length , Boolean leaveopen = false)
    {
        ArgumentNullException.ThrowIfNull(source); ArgumentOutOfRangeException.ThrowIfNegative(origin); ArgumentOutOfRangeException.ThrowIfNegative(length);

        if(source.CanRead is false) { throw new ArgumentException("Source stream must be readable.",nameof(source)); }

        if(source.CanSeek is false) { throw new ArgumentException("Source stream must be seekable.",nameof(source)); }

        if(origin > source.Length || origin + length > source.Length) { throw new ArgumentOutOfRangeException(nameof(length)); }

        this.Source = source; this.Origin = origin; this.WindowLength = length; this.LeaveOpen = leaveopen; this.Source.Position = origin;
    }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/property[@name="CanRead"]/*'/>*/
    public override Boolean CanRead => this.Source.CanRead;

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/property[@name="CanSeek"]/*'/>*/
    public override Boolean CanSeek => this.Source.CanSeek;

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/property[@name="CanWrite"]/*'/>*/
    public override Boolean CanWrite => false;

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/property[@name="Length"]/*'/>*/
    public override Int64 Length => this.WindowLength;

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/property[@name="Position"]/*'/>*/
    public override Int64 Position
    {
        get => this.Source.Position - this.Origin;
        set
        {
            if(value < 0 || value > this.WindowLength) { throw new ArgumentOutOfRangeException(nameof(value)); }

            this.Source.Position = this.Origin + value;
        }
    }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="Flush"]/*'/>*/
    public override void Flush() => this.Source.Flush();

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="FlushAsync"]/*'/>*/
    public override Task FlushAsync(CancellationToken cancel) => this.Source.FlushAsync(cancel);

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="Read"]/*'/>*/
    public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count)
    {
        ArgumentNullException.ThrowIfNull(buffer); ArgumentOutOfRangeException.ThrowIfNegative(offset); ArgumentOutOfRangeException.ThrowIfNegative(count);

        if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

        return this.Read(buffer.AsSpan(offset,count));
    }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="ReadSpan"]/*'/>*/
    public override Int32 Read(Span<Byte> buffer)
    {
        Int64 remaining = this.WindowLength - this.Position;

        if(remaining <= 0 || buffer.IsEmpty) { return 0; }

        Int32 requested = (Int32)Math.Min(buffer.Length,remaining);

        return this.Source.Read(buffer[..requested]);
    }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="ReadAsync"]/*'/>*/
    public override Task<Int32> ReadAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(buffer); ArgumentOutOfRangeException.ThrowIfNegative(offset); ArgumentOutOfRangeException.ThrowIfNegative(count);

        if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

        return this.ReadAsync(buffer.AsMemory(offset,count),cancel).AsTask();
    }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="ReadAsyncMemory"]/*'/>*/
    public override async ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancel = default)
    {
        Int64 remaining = this.WindowLength - this.Position;

        if(remaining <= 0 || buffer.IsEmpty) { return 0; }

        Int32 requested = (Int32)Math.Min(buffer.Length,remaining);

        return await this.Source.ReadAsync(buffer[..requested],cancel).ConfigureAwait(false);
    }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="Seek"]/*'/>*/
    public override Int64 Seek(Int64 offset , SeekOrigin origin)
    {
        Int64 target = origin switch
        {
            SeekOrigin.Begin => offset,

            SeekOrigin.Current => this.Position + offset,

            SeekOrigin.End => this.Length + offset,

            _ => throw new ArgumentOutOfRangeException(nameof(origin))
        };

        this.Position = target; return this.Position;
    }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="SetLength"]/*'/>*/
    public override void SetLength(Int64 value) { throw new NotSupportedException(); }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="Write"]/*'/>*/
    public override void Write(Byte[] buffer , Int32 offset , Int32 count) { throw new NotSupportedException(); }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="WriteSpan"]/*'/>*/
    public override void Write(ReadOnlySpan<Byte> buffer) { throw new NotSupportedException(); }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="WriteAsync"]/*'/>*/
    public override Task WriteAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancel) { throw new NotSupportedException(); }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="WriteAsyncMemory"]/*'/>*/
    public override ValueTask WriteAsync(ReadOnlyMemory<Byte> buffer , CancellationToken cancel = default) { throw new NotSupportedException(); }

    /**<include file='BoundedReadStream.xml' path='BoundedReadStream/class[@name="BoundedReadStream"]/method[@name="Dispose"]/*'/>*/
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
