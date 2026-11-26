namespace KusDepot.Data.Transfer;

/**<include file='CompositeReadStream.xml' path='CompositeReadStream/record[@name="CompositeReadStreamPart"]/main/*'/>*/
public sealed record CompositeReadStreamPart
{
    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/record[@name="CompositeReadStreamPart"]/property[@name="SourceOffset"]/*'/>*/
    public Int64 SourceOffset { get; init; }

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/record[@name="CompositeReadStreamPart"]/property[@name="Length"]/*'/>*/
    public Int64 Length { get; init; }

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/record[@name="CompositeReadStreamPart"]/property[@name="OpenAsync"]/*'/>*/
    public required Func<CancellationToken,Task<Stream?>> OpenAsync { get; init; }
}

/**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/main/*'/>*/
public sealed class CompositeReadStream : Stream
{
    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/field[@name="Parts"]/*'/>*/
    private readonly CompositeReadStreamPart[] Parts;

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/field[@name="TotalLength"]/*'/>*/
    private readonly Int64 TotalLength;

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/field[@name="PartIndex"]/*'/>*/
    private Int32 PartIndex;

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/field[@name="Current"]/*'/>*/
    private Stream? Current;

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/field[@name="CurrentRemaining"]/*'/>*/
    private Int64 CurrentRemaining;

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/field[@name="Disposed"]/*'/>*/
    private Boolean Disposed;

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/field[@name="PositionValue"]/*'/>*/
    private Int64 PositionValue;

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/constructor[@name="CompositeReadStream"]/*'/>*/
    public CompositeReadStream(IEnumerable<CompositeReadStreamPart> parts)
    {
        ArgumentNullException.ThrowIfNull(parts);

        this.Parts = parts.ToArray();

        Int64 total = 0;

        foreach(CompositeReadStreamPart part in this.Parts)
        {
            ArgumentNullException.ThrowIfNull(part);
            ArgumentNullException.ThrowIfNull(part.OpenAsync);
            ArgumentOutOfRangeException.ThrowIfNegative(part.SourceOffset);
            ArgumentOutOfRangeException.ThrowIfNegative(part.Length);
            total = checked(total + part.Length);
        }

        this.TotalLength = total;
    }

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/property[@name="CanRead"]/*'/>*/
    public override Boolean CanRead => !this.Disposed;

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/property[@name="CanSeek"]/*'/>*/
    public override Boolean CanSeek => false;

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/property[@name="CanWrite"]/*'/>*/
    public override Boolean CanWrite => false;

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/property[@name="Length"]/*'/>*/
    public override Int64 Length => this.TotalLength;

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/property[@name="Position"]/*'/>*/
    public override Int64 Position
    {
        get => this.PositionValue;
        set => throw new NotSupportedException();
    }

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="Flush"]/*'/>*/
    public override void Flush() => throw new NotSupportedException();

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="FlushAsync"]/*'/>*/
    public override Task FlushAsync(CancellationToken cancel) => Task.FromException(new NotSupportedException());

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="ReadArray"]/*'/>*/
    public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

        return this.Read(buffer.AsSpan(offset,count));
    }

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="ReadSpan"]/*'/>*/
    public override Int32 Read(Span<Byte> buffer)
    {
        this.ThrowIfDisposed();

        if(buffer.IsEmpty || this.PositionValue == this.TotalLength) { return 0; }

        Byte[] temporary = ArrayPool<Byte>.Shared.Rent(buffer.Length);

        try
        {
            Int32 read = this.ReadAsync(temporary.AsMemory(0,buffer.Length),CancellationToken.None).AsTask().GetAwaiter().GetResult();

            temporary.AsSpan(0,read).CopyTo(buffer);

            return read;
        }
        finally { ArrayPool<Byte>.Shared.Return(temporary); }
    }

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="ReadAsyncArray"]/*'/>*/
    public override Task<Int32> ReadAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegative(count);
        if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

        return this.ReadAsync(buffer.AsMemory(offset,count),cancel).AsTask();
    }

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="ReadAsyncMemory"]/*'/>*/
    public override async ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancel = default)
    {
        this.ThrowIfDisposed();

        if(buffer.IsEmpty || this.PositionValue == this.TotalLength) { return 0; }

        while(true)
        {
            cancel.ThrowIfCancellationRequested();

            if(this.CurrentRemaining == 0)
            {
                if(this.Current is not null)
                {
                    await this.Current.DisposeAsync().ConfigureAwait(false);

                    this.Current = null;
                }

                if(this.PartIndex >= this.Parts.Length) { return 0; }

                CompositeReadStreamPart part = this.Parts[this.PartIndex++];

                this.Current = await part.OpenAsync(cancel).ConfigureAwait(false);

                if(this.Current is null || this.Current.CanRead is false) { throw new IOException("A composite stream source could not be opened."); }

                if(await SkipAsync(this.Current,part.SourceOffset,cancel).ConfigureAwait(false) is false)
                {
                    await this.Current.DisposeAsync().ConfigureAwait(false); this.Current = null;

                    throw new EndOfStreamException("A composite stream source ended before its requested offset.");
                }

                this.CurrentRemaining = part.Length;

                if(this.CurrentRemaining == 0) { continue; }
            }

            Int32 requested = (Int32)Math.Min(buffer.Length,this.CurrentRemaining);

            Stream current = this.Current ?? throw new IOException("A composite stream source was not available.");

            Int32 read = await current.ReadAsync(buffer[..requested],cancel).ConfigureAwait(false);

            if(read <= 0) { throw new EndOfStreamException("A composite stream source ended before its requested length."); }

            this.CurrentRemaining -= read;

            this.PositionValue += read;

            return read;
        }
    }

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="Seek"]/*'/>*/
    public override Int64 Seek(Int64 offset , SeekOrigin origin) => throw new NotSupportedException();

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="SetLength"]/*'/>*/
    public override void SetLength(Int64 value) => throw new NotSupportedException();

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="Write"]/*'/>*/
    public override void Write(Byte[] buffer , Int32 offset , Int32 count) => throw new NotSupportedException();

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="Dispose"]/*'/>*/
    protected override void Dispose(Boolean disposing)
    {
        if(!disposing || this.Disposed) { base.Dispose(disposing); return; }

        this.Disposed = true;
        this.Current?.Dispose();
        this.Current = null;
        base.Dispose(disposing);
    }

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="DisposeAsync"]/*'/>*/
    public override async ValueTask DisposeAsync()
    {
        if(this.Disposed) { return; }

        this.Disposed = true;
        if(this.Current is not null) { await this.Current.DisposeAsync().ConfigureAwait(false); }
        this.Current = null;
        GC.SuppressFinalize(this);
    }

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="ThrowIfDisposed"]/*'/>*/
    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(this.Disposed,this);
    }

    /**<include file='CompositeReadStream.xml' path='CompositeReadStream/class[@name="CompositeReadStream"]/method[@name="SkipAsync"]/*'/>*/
    private static async Task<Boolean> SkipAsync(Stream source , Int64 length , CancellationToken cancel)
    {
        if(length == 0) { return true; }

        if(source.CanSeek)
        {
            if(length > source.Length - source.Position) { return false; }
            source.Position += length;
            return true;
        }

        Byte[] buffer = ArrayPool<Byte>.Shared.Rent(80 * KiB);

        try
        {
            Int64 remaining = length;

            while(remaining > 0)
            {
                Int32 requested = (Int32)Math.Min(buffer.Length,remaining);

                Int32 read = await source.ReadAsync(buffer.AsMemory(0,requested),cancel).ConfigureAwait(false);

                if(read <= 0) { return false; }

                remaining -= read;
            }

            return true;
        }
        finally { ArrayPool<Byte>.Shared.Return(buffer); }
    }
}
