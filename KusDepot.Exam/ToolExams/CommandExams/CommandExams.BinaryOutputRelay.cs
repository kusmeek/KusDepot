namespace KusDepot.Exams.Tools;

public partial class CommandExams
{
    [Test]
    public async Task ExecuteBinaryConsoleDelegatorBufferedLargeOutput_NoOpObservers()
    {
        Object? output = await ExecuteLargeBinaryCommandAsync(new(),buffered:true);

        Check.That(output).IsInstanceOf<BinaryCommandResult>();

        BinaryCommandResult result = (BinaryCommandResult)output!;

        Check.That(result.IsSuccess).IsTrue();
        Check.That(result.StandardOutputBytes.Length).IsEqualTo(1_048_576);
        Check.That(result.StandardErrorBytes.Length).IsEqualTo(1_048_576);
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegatorStreamedLargeOutput_NoOpObservers()
    {
        Object? output = await ExecuteLargeBinaryCommandAsync(new(),buffered:false);

        Check.That(output).IsInstanceOf<ProcessCommandResult>();
        Check.That(((ProcessCommandResult)output!).IsSuccess).IsTrue();
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegatorBufferedLargeOutput_ConsumingObservers()
    {
        Int32 stdoutBytes = 0; Int32 stderrBytes = 0;

        BinaryCommandObserver observer = new()
        {
            OnStdOut = async (stream,cancel) => stdoutBytes = await CountBytesAsync(stream,cancel).ConfigureAwait(false),
            OnStdErr = async (stream,cancel) => stderrBytes = await CountBytesAsync(stream,cancel).ConfigureAwait(false)
        };

        Object? output = await ExecuteLargeBinaryCommandAsync(observer,buffered:true);

        Check.That(output).IsInstanceOf<BinaryCommandResult>();

        BinaryCommandResult result = (BinaryCommandResult)output!;

        Check.That(result.StandardOutputBytes.Length).IsEqualTo(1_048_576);
        Check.That(result.StandardErrorBytes.Length).IsEqualTo(1_048_576);
        Check.That(stdoutBytes).IsEqualTo(1_048_576);
        Check.That(stderrBytes).IsEqualTo(1_048_576);
    }

    private static async Task<Object?> ExecuteLargeBinaryCommandAsync(BinaryCommandObserver observer , Boolean buffered)
    {
        CliWrap.Command command = Cli.Wrap("powershell.exe").WithArguments("-NoProfile -Command \"$bytes = [byte[]]::new(1048576); [Console]::OpenStandardOutput().Write($bytes,0,$bytes.Length); [Console]::OpenStandardError().Write($bytes,0,$bytes.Length)\"");

        ITool tool = ToolBuilderFactory.CreateBuilder()
            .RegisterCommand("LargeBinaryOutput",new BinaryConsoleDelegator(command,buffered,observer))
            .Build();

        Check.That(await tool.Activate()).IsTrue(); Check.That(await tool.EnableAllCommands()).IsTrue();

        Guid? id = await tool.ExecuteCommandAsync(new() { Handle = "LargeBinaryOutput" });

        Check.That(id).IsNotNull();

        return await tool.GetOutputAsync(id,timeout:TimeSpan.FromSeconds(30));
    }

    private static async Task<Int32> CountBytesAsync(Stream stream , CancellationToken cancel)
    {
        Byte[] buffer = new Byte[8192]; Int32 count = 0;

        while(true)
        {
            Int32 read = await stream.ReadAsync(buffer.AsMemory(),cancel).ConfigureAwait(false);

            if(read is 0) { return count; }

            count += read;
        }
    }
}
