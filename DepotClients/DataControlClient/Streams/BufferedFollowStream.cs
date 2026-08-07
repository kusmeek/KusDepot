namespace KusDepot.Data.Clients;

/**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/main/*'/>*/
public sealed class BufferedFollowStream : Stream
{
    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/field[@name="path"]/*'/>*/
    private readonly String path;

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/field[@name="gate"]/*'/>*/
    private readonly SemaphoreSlim gate = new(1,1);

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/field[@name="signal"]/*'/>*/
    private TaskCompletionSource<Boolean> signal = CreateSignal();

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/field[@name="reader"]/*'/>*/
    private FileStream? reader;

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/field[@name="readableLength"]/*'/>*/
    private Int64 readableLength;

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/field[@name="terminalState"]/*'/>*/
    private BufferedFollowStreamTerminalState terminalState;

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/field[@name="faultMessage"]/*'/>*/
    private String? faultMessage;

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/field[@name="disposed"]/*'/>*/
    private Boolean disposed;

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/constructor[@name="Constructor"]/*'/>*/
    public BufferedFollowStream(String path , Int64 initialLength = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path); ArgumentOutOfRangeException.ThrowIfNegative(initialLength);

        this.path = Path.GetFullPath(path);

        this.readableLength = initialLength;

        String? directory = Path.GetDirectoryName(this.path);

        if(String.IsNullOrWhiteSpace(directory) is false) { Directory.CreateDirectory(directory); }

        if(File.Exists(this.path) is false)
        {
            using FileStream _ = new(this.path,new FileStreamOptions(){ Access = FileAccess.Write , Mode = FileMode.OpenOrCreate,
                Share = FileShare.ReadWrite , Options = FileOptions.Asynchronous | FileOptions.SequentialScan });
        }
    }

    ///<inheritdoc/>
    public override Boolean CanRead => this.disposed is false;

    ///<inheritdoc/>
    public override Boolean CanSeek => true;

    ///<inheritdoc/>
    public override Boolean CanWrite => false;

    ///<inheritdoc/>
    public override Int64 Length => Interlocked.Read(ref this.readableLength);

    ///<inheritdoc/>
    public override Int64 Position
    {
        get => this.reader?.Position ?? 0;
        set
        {
            ObjectDisposedException.ThrowIf(this.disposed,this);

            ArgumentOutOfRangeException.ThrowIfNegative(value);

            FileStream stream = this.EnsureReader();

            ArgumentOutOfRangeException.ThrowIfGreaterThan(value,stream.Length);

            stream.Position = value;
        }
    }

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/method[@name="AppendAsync"]/*'/>*/
    public async Task AppendAsync(Stream payload , CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(payload); ObjectDisposedException.ThrowIf(this.disposed,this);

        await this.gate.WaitAsync(cancel).ConfigureAwait(false);

        try
        {
            this.ThrowIfTerminalStateRejectsAppend();

            FileStream stream = new(this.path,new FileStreamOptions(){ Access = FileAccess.Write , Mode = FileMode.OpenOrCreate,
                Share = FileShare.ReadWrite , Options = FileOptions.Asynchronous | FileOptions.SequentialScan });

            await using (stream.ConfigureAwait(false))
            {
                stream.Position = stream.Length;

                await payload.CopyToAsync(stream,cancel).ConfigureAwait(false);

                await stream.FlushAsync(cancel).ConfigureAwait(false);

                this.readableLength = stream.Length;
            }

            this.signal.TrySetResult(true);

            this.signal = CreateSignal();
        }
        finally { this.gate.Release(); }
    }

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/method[@name="MarkCompleted"]/*'/>*/
    public void MarkCompleted()
    {
        this.SetTerminalState(BufferedFollowStreamTerminalState.Completed,null);
    }

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/method[@name="MarkAborted"]/*'/>*/
    public void MarkAborted()
    {
        this.SetTerminalState(BufferedFollowStreamTerminalState.Aborted,null);
    }

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/method[@name="MarkFaulted"]/*'/>*/
    public void MarkFaulted(String? message = null)
    {
        this.SetTerminalState(BufferedFollowStreamTerminalState.Faulted,message);
    }

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/method[@name="GetTerminalState"]/*'/>*/
    public BufferedFollowStreamTerminalState GetTerminalState()
    {
        return this.terminalState;
    }

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/method[@name="GetFaultMessage"]/*'/>*/
    public String? GetFaultMessage()
    {
        return this.faultMessage is null ? null : new(this.faultMessage);
    }

    ///<inheritdoc/>
    public override void Flush()
    {
        this.reader?.Flush();
    }

    ///<inheritdoc/>
    public override Task FlushAsync(CancellationToken cancel)
    {
        return this.reader?.FlushAsync(cancel) ?? Task.CompletedTask;
    }

    ///<inheritdoc/>
    public override Int32 Read(Byte[] buffer , Int32 offset , Int32 count)
    {
        ArgumentNullException.ThrowIfNull(buffer);
        ArgumentOutOfRangeException.ThrowIfNegative(offset);
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        if(buffer.Length - offset < count) { throw new ArgumentException("Offset and length exceed buffer bounds."); }

        return this.ReadAsync(buffer.AsMemory(offset,count),CancellationToken.None).AsTask().GetAwaiter().GetResult();
    }

    ///<inheritdoc/>
    public override async ValueTask<Int32> ReadAsync(Memory<Byte> buffer , CancellationToken cancel = default)
    {
        ObjectDisposedException.ThrowIf(this.disposed,this);

        if(buffer.IsEmpty) { return 0; }

        FileStream stream = this.EnsureReader();

        while(true)
        {
            Int64 available = Interlocked.Read(ref this.readableLength) - stream.Position;

            if(available > 0)
            {
                Int32 requested = (Int32)Math.Min(buffer.Length,available);

                return await stream.ReadAsync(buffer[..requested],cancel).ConfigureAwait(false);
            }

            switch(this.terminalState)
            {
                case BufferedFollowStreamTerminalState.Completed:
                    return 0;

                case BufferedFollowStreamTerminalState.Aborted:
                    throw new IOException("The follow stream was aborted.");

                case BufferedFollowStreamTerminalState.Faulted:
                    throw new IOException(this.faultMessage ?? "The follow stream faulted.");
            }

            TaskCompletionSource<Boolean> currentSignal = this.signal;

            await currentSignal.Task.WaitAsync(cancel).ConfigureAwait(false);
        }
    }

    ///<inheritdoc/>
    public override Int64 Seek(Int64 offset , SeekOrigin origin)
    {
        ObjectDisposedException.ThrowIf(this.disposed,this);

        FileStream stream = this.EnsureReader();

        Int64 target = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => stream.Position + offset,
            SeekOrigin.End => Interlocked.Read(ref this.readableLength) + offset,
            _ => throw new ArgumentOutOfRangeException(nameof(origin))
        };

        if(target < 0) { throw new ArgumentOutOfRangeException(nameof(offset)); }

        if(target > Interlocked.Read(ref this.readableLength)) { throw new ArgumentOutOfRangeException(nameof(offset)); }

        return stream.Seek(target,SeekOrigin.Begin);
    }

    ///<inheritdoc/>
    public override void SetLength(Int64 value) { throw new NotSupportedException(); }

    ///<inheritdoc/>
    public override void Write(Byte[] buffer , Int32 offset , Int32 count) { throw new NotSupportedException(); }

    ///<inheritdoc/>
    public override void Write(ReadOnlySpan<Byte> buffer) { throw new NotSupportedException(); }

    ///<inheritdoc/>
    public override Task WriteAsync(Byte[] buffer , Int32 offset , Int32 count , CancellationToken cancel) { throw new NotSupportedException(); }

    ///<inheritdoc/>
    public override ValueTask WriteAsync(ReadOnlyMemory<Byte> buffer , CancellationToken cancel = default) { throw new NotSupportedException(); }

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/method[@name="EnsureReader"]/*'/>*/
    private FileStream EnsureReader()
    {
        if(this.reader is null)
        {
            this.reader = new FileStream(this.path,new FileStreamOptions(){ Access = FileAccess.Read , Mode = FileMode.OpenOrCreate,
                Share = FileShare.ReadWrite , Options = FileOptions.Asynchronous | FileOptions.SequentialScan });
        }

        return this.reader;
    }

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/method[@name="SetTerminalState"]/*'/>*/
    private void SetTerminalState(BufferedFollowStreamTerminalState state , String? message)
    {
        if(this.disposed) { return; }

        this.terminalState = state;
        this.faultMessage = message;
        this.signal.TrySetResult(true);
    }

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/method[@name="ThrowIfTerminalStateRejectsAppend"]/*'/>*/
    private void ThrowIfTerminalStateRejectsAppend()
    {
        if(this.terminalState == BufferedFollowStreamTerminalState.Completed) { throw new InvalidOperationException("The follow stream is already complete."); }
        if(this.terminalState == BufferedFollowStreamTerminalState.Aborted) { throw new InvalidOperationException("The follow stream was aborted."); }
        if(this.terminalState == BufferedFollowStreamTerminalState.Faulted) { throw new InvalidOperationException(this.faultMessage ?? "The follow stream faulted."); }
    }

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/class[@name="BufferedFollowStream"]/method[@name="CreateSignal"]/*'/>*/
    private static TaskCompletionSource<Boolean> CreateSignal()
    {
        return new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    ///<inheritdoc/>
    protected override void Dispose(Boolean disposing)
    {
        if(disposing is false || this.disposed) { return; }

        this.disposed = true;

        this.signal.TrySetCanceled();

        this.reader?.Dispose();

        this.reader = null;

        this.gate.Dispose();

        base.Dispose(disposing);
    }

    ///<inheritdoc/>
    public override async ValueTask DisposeAsync()
    {
        if(this.disposed) { return; }

        this.disposed = true;

        this.signal.TrySetCanceled();

        if(reader is not null) { await reader.DisposeAsync().ConfigureAwait(false); }

        this.reader = null;

        this.gate.Dispose();

        await base.DisposeAsync().ConfigureAwait(false);
    }
}

/**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/enum[@name="BufferedFollowStreamTerminalState"]/main/*'/>*/
public enum BufferedFollowStreamTerminalState
{
    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/enum[@name="BufferedFollowStreamTerminalState"]/field[@name="Open"]/*'/>*/
    Open = 0,

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/enum[@name="BufferedFollowStreamTerminalState"]/field[@name="Completed"]/*'/>*/
    Completed = 1,

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/enum[@name="BufferedFollowStreamTerminalState"]/field[@name="Aborted"]/*'/>*/
    Aborted = 2,

    /**<include file='BufferedFollowStream.xml' path='BufferedFollowStream/enum[@name="BufferedFollowStreamTerminalState"]/field[@name="Faulted"]/*'/>*/
    Faulted = 3,
}
