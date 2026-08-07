namespace KusDepot;

/**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/main/*'/>*/
public sealed class HashingWriteStream : Stream
{
    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/field[@name="Destination"]/*'/>*/
    private readonly Stream Destination;

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/field[@name="Hash"]/*'/>*/
    private readonly IncrementalHash Hash;

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/field[@name="LeaveOpen"]/*'/>*/
    private readonly Boolean LeaveOpen;

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/field[@name="Finalized"]/*'/>*/
    private Boolean Finalized;

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/field[@name="disposed"]/*'/>*/
    private Boolean disposed;

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/constructor[@name="Constructor"]/*'/>*/
    public HashingWriteStream(Stream destination , Boolean leaveopen = false)
    {
        ArgumentNullException.ThrowIfNull(destination); this.Destination = destination; this.Hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA512); this.LeaveOpen = leaveopen;
    }

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/method[@name="GetHashAndReset"]/*'/>*/
    public Byte[] GetHashAndReset()
    {
        if(this.Finalized) { throw new InvalidOperationException(); }

        this.Finalized = true; return this.Hash.GetHashAndReset();
    }

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/property[@name="CanRead"]/*'/>*/
    public override Boolean CanRead => this.Destination.CanRead;

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/property[@name="CanSeek"]/*'/>*/
    public override Boolean CanSeek => this.Destination.CanSeek;

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/property[@name="CanWrite"]/*'/>*/
    public override Boolean CanWrite => this.Destination.CanWrite;

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/property[@name="Length"]/*'/>*/
    public override Int64 Length => this.Destination.Length;

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/property[@name="Position"]/*'/>*/
    public override Int64 Position { get => this.Destination.Position; set => this.Destination.Position = value; }

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/method[@name="Flush"]/*'/>*/
    public override void Flush() => this.Destination.Flush();

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/method[@name="FlushAsync"]/*'/>*/
    public override Task FlushAsync(CancellationToken cancel) => this.Destination.FlushAsync(cancel);

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/method[@name="Read"]/*'/>*/
    public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count) => this.Destination.Read(buffer,offset,count);

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/method[@name="Seek"]/*'/>*/
    public override Int64 Seek(Int64 offset , SeekOrigin origin) => this.Destination.Seek(offset,origin);

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/method[@name="SetLength"]/*'/>*/
    public override void SetLength(Int64 value) => this.Destination.SetLength(value);

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/method[@name="Write"]/*'/>*/
    public override void Write(Byte[] buffer , Int32 offset , Int32 count)
    {
        this.Destination.Write(buffer,offset,count); this.Hash.AppendData(buffer,offset,count);
    }

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/method[@name="WriteByte"]/*'/>*/
    public override void WriteByte(Byte value)
    {
        this.Destination.WriteByte(value); this.Hash.AppendData([value]);
    }

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/method[@name="WriteAsync"]/*'/>*/
    public override async Task WriteAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancel)
    {
        await this.WriteAsync(buffer.AsMemory(offset,count),cancel).ConfigureAwait(false);
    }

    /**<include file='HashingWriteStream.xml' path='HashingWriteStream/class[@name="HashingWriteStream"]/method[@name="WriteAsyncMemory"]/*'/>*/
    public override async ValueTask WriteAsync(ReadOnlyMemory<Byte> buffer , CancellationToken cancel = default)
    {
        await this.Destination.WriteAsync(buffer,cancel).ConfigureAwait(false); this.Hash.AppendData(buffer.Span);
    }

    ///<inheritdoc/>
    protected override void Dispose(Boolean disposing)
    {
        if(disposing is false || this.disposed) { return; }

        this.disposed = true;

        this.Hash.Dispose();

        if(this.LeaveOpen is false) { this.Destination.Dispose(); }

        base.Dispose(disposing);
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        if(this.disposed) { return; }

        this.disposed = true;

        this.Hash.Dispose();

        if(this.LeaveOpen is false) { await this.Destination.DisposeAsync().ConfigureAwait(false); }

        await base.DisposeAsync().ConfigureAwait(false);
    }
}