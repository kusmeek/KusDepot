namespace KusDepot;

/**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/main/*'/>*/
public sealed class HashingReadStream : Stream
{
    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/field[@name="Source"]/*'/>*/
    private readonly Stream Source;

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/field[@name="Hash"]/*'/>*/
    private readonly IncrementalHash Hash;

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/field[@name="LeaveOpen"]/*'/>*/
    private readonly Boolean LeaveOpen;

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/field[@name="Finalized"]/*'/>*/
    private Boolean Finalized;

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/field[@name="disposed"]/*'/>*/
    private Boolean disposed;

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/constructor[@name="Constructor"]/*'/>*/
    public HashingReadStream(Stream source, Boolean leaveopen = false)
    {
        ArgumentNullException.ThrowIfNull(source); this.Source = source; this.Hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA512); this.LeaveOpen = leaveopen;
    }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="GetHashAndReset"]/*'/>*/
    public Byte[] GetHashAndReset()
    {
        if(this.Finalized) { throw new InvalidOperationException(); }

        this.Finalized = true; return this.Hash.GetHashAndReset();
    }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/property[@name="CanRead"]/*'/>*/
    public override Boolean CanRead => this.Source.CanRead;

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/property[@name="CanSeek"]/*'/>*/
    public override Boolean CanSeek => this.Source.CanSeek;

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/property[@name="CanWrite"]/*'/>*/
    public override Boolean CanWrite => false;

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/property[@name="Length"]/*'/>*/
    public override Int64 Length => this.Source.Length;

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/property[@name="Position"]/*'/>*/
    public override Int64 Position { get => this.Source.Position; set => this.Source.Position = value; }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="Flush"]/*'/>*/
    public override void Flush() => this.Source.Flush();

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="FlushAsync"]/*'/>*/
    public override Task FlushAsync(CancellationToken cancel) => this.Source.FlushAsync(cancel);

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="Read"]/*'/>*/
    public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count)
    {
        ArgumentNullException.ThrowIfNull(buffer); ArgumentOutOfRangeException.ThrowIfNegative(offset); ArgumentOutOfRangeException.ThrowIfNegative(count);

        if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

        return this.Read(buffer.AsSpan(offset,count));
    }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="ReadSpan"]/*'/>*/
    public override Int32 Read(Span<Byte> buffer)
    {
        Int32 read = this.Source.Read(buffer);

        if(read > 0) { this.Hash.AppendData(buffer[..read]); }

        return read;
    }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="ReadByte"]/*'/>*/
    public override Int32 ReadByte()
    {
        Int32 value = this.Source.ReadByte();

        if(value >= 0) { this.Hash.AppendData([(Byte)value]); }

        return value;
    }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="ReadAsync"]/*'/>*/
    public override Task<Int32> ReadAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(buffer); ArgumentOutOfRangeException.ThrowIfNegative(offset); ArgumentOutOfRangeException.ThrowIfNegative(count);

        if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

        return this.ReadAsync(buffer.AsMemory(offset,count),cancel).AsTask();
    }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="ReadAsyncMemory"]/*'/>*/
    public override async ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancel = default)
    {
        Int32 read = await this.Source.ReadAsync(buffer,cancel).ConfigureAwait(false);

        if(read > 0) { this.Hash.AppendData(buffer.Span[..read]); }

        return read;
    }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="Seek"]/*'/>*/
    public override Int64 Seek(Int64 offset , SeekOrigin origin) => this.Source.Seek(offset,origin);

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="SetLength"]/*'/>*/
    public override void SetLength(Int64 value) => this.Source.SetLength(value);

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="Write"]/*'/>*/
    public override void Write(Byte[] buffer , Int32 offset , Int32 count) { throw new NotSupportedException(); }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="WriteSpan"]/*'/>*/
    public override void Write(ReadOnlySpan<Byte> buffer) { throw new NotSupportedException(); }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="WriteByte"]/*'/>*/
    public override void WriteByte(Byte value) { throw new NotSupportedException(); }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="WriteAsync"]/*'/>*/
    public override Task WriteAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancel) { throw new NotSupportedException(); }

    /**<include file='HashingReadStream.xml' path='HashingReadStream/class[@name="HashingReadStream"]/method[@name="WriteAsyncMemory"]/*'/>*/
    public override ValueTask WriteAsync(ReadOnlyMemory<Byte> buffer , CancellationToken cancel = default) { throw new NotSupportedException(); }

    ///<inheritdoc/>
    protected override void Dispose(Boolean disposing)
    {
        if(disposing is false || this.disposed) { return; }

        this.disposed = true;

        this.Hash.Dispose();

        if(this.LeaveOpen is false) { this.Source.Dispose(); }

        base.Dispose(disposing);
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        if(this.disposed) { return; }

        this.disposed = true;

        this.Hash.Dispose();

        if(this.LeaveOpen is false) { await this.Source.DisposeAsync().ConfigureAwait(false); }

        await base.DisposeAsync().ConfigureAwait(false);
    }
}
