namespace KusDepot.Commands;

/**<include file='BinaryOutputRelay.xml' path='BinaryOutputRelay/class[@name="BinaryOutputRelay"]/main/*'/>*/
internal sealed class BinaryOutputRelay : IDisposable
{
    /**<include file='BinaryOutputRelay.xml' path='BinaryOutputRelay/class[@name="BinaryOutputRelay"]/constructor[@name="Constructor"]/*'/>*/
    public BinaryOutputRelay(Boolean capture)
    {
        output = capture
            ? new Pipe(new PipeOptions(pauseWriterThreshold:Int64.MaxValue,resumeWriterThreshold:Int64.MaxValue - 1,useSynchronizationContext:false))
            : new Pipe();

        outputStream = output.Reader.AsStream(leaveOpen:true);

        this.capture = capture ? new MemoryStream() : null;
    }

    /**<include file='BinaryOutputRelay.xml' path='BinaryOutputRelay/class[@name="BinaryOutputRelay"]/property[@name="Stream"]/*'/>*/
    public Stream Stream => outputStream;

    /**<include file='BinaryOutputRelay.xml' path='BinaryOutputRelay/class[@name="BinaryOutputRelay"]/method[@name="PumpAsync"]/*'/>*/
    public async Task PumpAsync(Stream source , CancellationToken cancel)
    {
        Byte[] buffer = new Byte[80*KiB];

        try
        {
            while(true)
            {
                Int32 read = await source.ReadAsync(buffer.AsMemory(),cancel).ConfigureAwait(false);

                if(read is 0) { break; }

                capture?.Write(buffer,0,read);

                await output.Writer.WriteAsync(buffer.AsMemory(0,read),cancel).ConfigureAwait(false);
            }
        }
        finally { Complete(); }
    }

    /**<include file='BinaryOutputRelay.xml' path='BinaryOutputRelay/class[@name="BinaryOutputRelay"]/method[@name="Complete"]/*'/>*/
    public void Complete()
    {
        if(Interlocked.Exchange(ref writerCompleted,1) is 0) { output.Writer.Complete(); }
    }

    /**<include file='BinaryOutputRelay.xml' path='BinaryOutputRelay/class[@name="BinaryOutputRelay"]/method[@name="CopyCapturedBytes"]/*'/>*/
    public Byte[] CopyCapturedBytes()
    {
        return capture?.ToArray() ?? Array.Empty<Byte>();
    }

    /**<include file='BinaryOutputRelay.xml' path='BinaryOutputRelay/class[@name="BinaryOutputRelay"]/method[@name="Dispose"]/*'/>*/
    public void Dispose()
    {
        outputStream.Dispose(); output.Reader.Complete(); output.Writer.Complete(); capture?.Dispose();
    }

    /**<include file='BinaryOutputRelay.xml' path='BinaryOutputRelay/class[@name="BinaryOutputRelay"]/field[@name="output"]/*'/>*/
    private readonly Pipe output;

    /**<include file='BinaryOutputRelay.xml' path='BinaryOutputRelay/class[@name="BinaryOutputRelay"]/field[@name="writerCompleted"]/*'/>*/
    private Int32 writerCompleted;

    /**<include file='BinaryOutputRelay.xml' path='BinaryOutputRelay/class[@name="BinaryOutputRelay"]/field[@name="outputStream"]/*'/>*/
    private readonly Stream outputStream;

    /**<include file='BinaryOutputRelay.xml' path='BinaryOutputRelay/class[@name="BinaryOutputRelay"]/field[@name="capture"]/*'/>*/
    private readonly MemoryStream? capture;
}
