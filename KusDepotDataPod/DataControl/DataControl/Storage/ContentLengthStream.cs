namespace DataPodServices.DataControl;

/**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/main/*'/>*/
internal sealed class ContentLengthStream : Stream
{
    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/field[@name="inner"]/*'/>*/
    private readonly Stream inner;

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/field[@name="length"]/*'/>*/
    private readonly Int64 length;

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/property[@name="CanRead"]/*'/>*/
    public override Boolean CanRead => this.inner.CanRead;

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/property[@name="CanSeek"]/*'/>*/
    public override Boolean CanSeek => false;

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/property[@name="CanWrite"]/*'/>*/
    public override Boolean CanWrite => false;

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/property[@name="Length"]/*'/>*/
    public override Int64 Length => this.length;

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/property[@name="Position"]/*'/>*/
    public override Int64 Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/constructor[@name="ContentLengthStream"]/*'/>*/
    public ContentLengthStream(Stream inner , Int64 length)
    {
        ArgumentNullException.ThrowIfNull(inner);

        ArgumentOutOfRangeException.ThrowIfNegative(length);

        this.inner = inner; this.length = length;
    }

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/method[@name="Flush"]/*'/>*/
    public override void Flush() => throw new NotSupportedException();

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/method[@name="Read"]/*'/>*/
    public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count) => this.inner.Read(buffer,offset,count);

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/method[@name="ReadAsyncMemory"]/*'/>*/
    public override ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancellationToken = default)
    {
        return this.inner.ReadAsync(buffer,cancellationToken);
    }

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/method[@name="ReadAsyncArray"]/*'/>*/
    public override Task<Int32> ReadAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancellationToken)
    {
        return this.inner.ReadAsync(buffer,offset,count,cancellationToken);
    }

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/method[@name="Seek"]/*'/>*/
    public override Int64 Seek(Int64 offset , SeekOrigin origin) => throw new NotSupportedException();

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/method[@name="SetLength"]/*'/>*/
    public override void SetLength(Int64 value) => throw new NotSupportedException();

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/method[@name="Write"]/*'/>*/
    public override void Write(Byte[] buffer , Int32 offset , Int32 count) => throw new NotSupportedException();

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/method[@name="Dispose"]/*'/>*/
    protected override void Dispose(Boolean disposing)
    {
        if(disposing) { this.inner.Dispose(); }

        base.Dispose(disposing);
    }

    /**<include file='ContentLengthStream.xml' path='ContentLengthStream/class[@name="ContentLengthStream"]/method[@name="DisposeAsync"]/*'/>*/
    public override ValueTask DisposeAsync()
    {
        return this.inner.DisposeAsync();
    }
}
