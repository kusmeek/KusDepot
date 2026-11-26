namespace KusDepot.Cryptography;

/**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/main/*'/>*/
public sealed class DecryptionStream : Stream
{
    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/property[@name="CanRead"]/*'/>*/
    public override Boolean CanRead => Inner.CanRead;

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/property[@name="CanSeek"]/*'/>*/
    public override Boolean CanSeek => Inner.CanSeek;

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/property[@name="CanWrite"]/*'/>*/
    public override Boolean CanWrite => false;

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/property[@name="Length"]/*'/>*/
    public override Int64 Length => CanSeek ? 1 + ( Inner.Length - InnerOrigin ) : throw new NotSupportedException();

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/property[@name="Position"]/*'/>*/
    public override Int64 Position
    {
        get => CanSeek ? PrefixOffset + ( Inner.Position - InnerOrigin ) : throw new NotSupportedException();

        set
        {
            if(CanSeek is false) { throw new NotSupportedException(); }

            if(value < 0 || value > Length) { throw new ArgumentOutOfRangeException(nameof(value)); }

            if(value <= 1)
            {
                PrefixOffset = (Int32)value; Inner.Position = InnerOrigin;
            }
            else
            {
                PrefixOffset = 1; Inner.Position = InnerOrigin + ( value - 1 );
            }
        }
    }

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/field[@name="Inner"]/*'/>*/
    private readonly Stream Inner;

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/field[@name="InnerOrigin"]/*'/>*/
    private readonly Int64 InnerOrigin;

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/field[@name="Prefix"]/*'/>*/
    private readonly Byte Prefix;

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/field[@name="PrefixOffset"]/*'/>*/
    private Int32 PrefixOffset;

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/constructor[@name="Constructor"]/*'/>*/
    public DecryptionStream(Stream inner , Byte prefix)
    {
        ArgumentNullException.ThrowIfNull(inner);

        Inner = inner; InnerOrigin = inner.CanSeek ? inner.Position : 0; Prefix = prefix; PrefixOffset = 0;
    }

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/method[@name="Flush"]/*'/>*/
    public override void Flush() {}

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/method[@name="Read"]/*'/>*/
    public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count)
    {
        ArgumentNullException.ThrowIfNull(buffer); ArgumentOutOfRangeException.ThrowIfNegative(offset); ArgumentOutOfRangeException.ThrowIfNegative(count);

        if(buffer.Length - offset < count) { throw new ArgumentException(BufferInsufficient); }

        return this.Read(buffer.AsSpan(offset,count));
    }

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/method[@name="ReadSpan"]/*'/>*/
    public override Int32 Read(Span<Byte> buffer)
    {
        Int32 read = 0;

        if(PrefixOffset == 0 && buffer.IsEmpty is false)
        {
            buffer[0] = Prefix; PrefixOffset = 1; read = 1;

            if(read == buffer.Length) { return read; }
        }

        return read + Inner.Read(buffer[read..]);
    }

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/method[@name="ReadAsync"]/*'/>*/
    public override Task<Int32> ReadAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(buffer); ArgumentOutOfRangeException.ThrowIfNegative(offset); ArgumentOutOfRangeException.ThrowIfNegative(count);

        if(buffer.Length - offset < count) { throw new ArgumentException(BufferInsufficient); }

        return this.ReadAsync(buffer.AsMemory(offset,count),cancellationToken).AsTask();
    }

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/method[@name="ReadAsyncMemory"]/*'/>*/
    public override async ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancellationToken = default)
    {
        Int32 read = 0;

        if(PrefixOffset == 0 && buffer.IsEmpty is false)
        {
            buffer.Span[0] = Prefix; PrefixOffset = 1; read = 1;

            if(read == buffer.Length) { return read; }
        }

        return read + await Inner.ReadAsync(buffer[read..],cancellationToken).ConfigureAwait(false);
    }

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/method[@name="Seek"]/*'/>*/
    public override Int64 Seek(Int64 offset , SeekOrigin origin)
    {
        if(CanSeek is false) { throw new NotSupportedException(); }

        Int64 target = origin switch
        {
            SeekOrigin.Begin => offset,

            SeekOrigin.Current => Position + offset,

            SeekOrigin.End => Length + offset,

            _ => throw new ArgumentOutOfRangeException(nameof(origin))
        };

        Position = target; return Position;
    }

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/method[@name="SetLength"]/*'/>*/
    public override void SetLength(Int64 value) { throw new NotSupportedException(); }

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/method[@name="Write"]/*'/>*/
    public override void Write(Byte[] buffer , Int32 offset , Int32 count) { throw new NotSupportedException(); }

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/method[@name="WriteSpan"]/*'/>*/
    public override void Write(ReadOnlySpan<Byte> buffer) { throw new NotSupportedException(); }

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/method[@name="WriteAsync"]/*'/>*/
    public override Task WriteAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancellationToken) { throw new NotSupportedException(); }

    /**<include file='DecryptionStream.xml' path='DecryptionStream/class[@name="DecryptionStream"]/method[@name="WriteAsyncMemory"]/*'/>*/
    public override ValueTask WriteAsync(ReadOnlyMemory<Byte> buffer , CancellationToken cancellationToken = default) { throw new NotSupportedException(); }
}