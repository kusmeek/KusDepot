namespace KusDepot.Exams.Tools;

public partial class CommandExams
{
    [Test]
    public async Task ConsoleDelegator_ActivityOverrides_DoNotLeakToNextExecution()
    {
        Int32 defaultStarted = 0;
        Int32 overrideStarted = 0;
        ConcurrentQueue<String> defaultOutput = new();
        ConcurrentQueue<String> overrideOutput = new();
        TaskCompletionSource<Boolean> defaultOutputReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> overrideOutputReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);
        ConsoleCommandObserver defaultObserver = CreateTextObserver(defaultOutput,() => Interlocked.Increment(ref defaultStarted),defaultOutputReceived);
        ConsoleCommandObserver overrideObserver = CreateTextObserver(overrideOutput,() => Interlocked.Increment(ref overrideStarted),overrideOutputReceived);
        CliWrap.Command process = Cli.Wrap("powershell.exe").WithArguments("-NoProfile -Command \"$line = [Console]::In.ReadLine(); [Console]::Out.WriteLine($line)\"");
        ConsoleDelegator command = new(process,streamed:true,buffered:null,observer:defaultObserver);
        Tool tool = new();

        Check.That(await tool.RegisterCommand("ConsoleOverrideIsolation",command)).IsTrue();
        Check.That(await tool.Activate()).IsTrue();
        Check.That(await tool.EnableAllCommands()).IsTrue();

        using MemoryStream firstInput = new(Encoding.UTF8.GetBytes("first-console-input\n"));
        CommandDetails first = CommandDetails.Create("ConsoleOverrideIsolation")
            .SetInputStream(firstInput)
            .SetConsoleObserver(overrideObserver);
        Check.That(await tool.ExecuteCommandAsync(first)).IsNotNull();
        Check.That(command.ConsoleCommand).IsSameReferenceAs(process);
        Check.That(overrideStarted).IsEqualTo(1);
        Check.That(defaultStarted).IsEqualTo(0);
        firstInput.Dispose();
        await overrideOutputReceived.Task.WaitAsync(TimeSpan.FromSeconds(10));

        Check.That(await tool.ExecuteCommandAsync(CommandDetails.Create("ConsoleOverrideIsolation"))).IsNotNull();
        await defaultOutputReceived.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Check.That(overrideStarted).IsEqualTo(1);
        Check.That(defaultStarted).IsEqualTo(1);
        Check.That(String.Concat(overrideOutput)).Contains("first-console-input");
        Check.That(String.Concat(defaultOutput)).Not.Contains("first-console-input");
    }

    [Test]
    public async Task BinaryConsoleDelegator_ActivityOverrides_DoNotLeakToNextExecution()
    {
        ConcurrentQueue<String> defaultOutput = new();
        ConcurrentQueue<String> overrideOutput = new();
        TaskCompletionSource<Boolean> defaultOutputReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> overrideOutputReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);
        BinaryCommandObserver defaultObserver = CreateBinaryObserver(defaultOutput,outputReceived:defaultOutputReceived);
        BinaryCommandObserver overrideObserver = CreateBinaryObserver(overrideOutput,outputReceived:overrideOutputReceived);
        CliWrap.Command process = Cli.Wrap("powershell.exe").WithArguments("-NoProfile -Command \"$line = [Console]::In.ReadLine(); [Console]::Out.WriteLine($line)\"");
        BinaryConsoleDelegator command = new(process,observer:defaultObserver);
        Tool tool = new();

        Check.That(await tool.RegisterCommand("BinaryOverrideIsolation",command)).IsTrue();
        Check.That(await tool.Activate()).IsTrue();
        Check.That(await tool.EnableAllCommands()).IsTrue();

        using MemoryStream firstInput = new(Encoding.UTF8.GetBytes("first-binary-input\n"));
        CommandDetails first = CommandDetails.Create("BinaryOverrideIsolation")
            .SetInputStream(firstInput)
            .SetBinaryConsoleObserver(overrideObserver);
        Check.That(await tool.ExecuteCommandAsync(first)).IsNotNull();
        Check.That(command.ConsoleCommand).IsSameReferenceAs(process);
        firstInput.Dispose();
        await overrideOutputReceived.Task.WaitAsync(TimeSpan.FromSeconds(10));

        Check.That(await tool.ExecuteCommandAsync(CommandDetails.Create("BinaryOverrideIsolation"))).IsNotNull();
        await defaultOutputReceived.Task.WaitAsync(TimeSpan.FromSeconds(10));
        Check.That(String.Concat(overrideOutput)).Contains("first-binary-input");
        Check.That(String.Concat(defaultOutput)).Not.Contains("first-binary-input");
    }

    [Test]
    public async Task ConsoleDelegator_ConcurrentExecutions_KeepInputAndObserverIsolated()
    {
        ConcurrentQueue<String> firstOutput = new();
        ConcurrentQueue<String> secondOutput = new();
        TaskCompletionSource<Boolean> firstOutputReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> secondOutputReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);
        ConsoleCommandObserver firstObserver = CreateTextObserver(firstOutput,outputReceived:firstOutputReceived);
        ConsoleCommandObserver secondObserver = CreateTextObserver(secondOutput,outputReceived:secondOutputReceived);
        CliWrap.Command process = Cli.Wrap("powershell.exe").WithArguments("-NoProfile -Command \"$line = [Console]::In.ReadLine(); Start-Sleep -Milliseconds 150; [Console]::Out.WriteLine($line)\"");
        Tool tool = new();

        Check.That(await tool.RegisterCommand("ConsoleOverrideConcurrent",new ConsoleDelegator(process,streamed:true,buffered:null))).IsTrue();
        Check.That(await tool.Activate()).IsTrue();
        Check.That(await tool.EnableAllCommands()).IsTrue();

        CommandDetails first = CommandDetails.Create("ConsoleOverrideConcurrent")
            .SetInputStream(new MemoryStream(Encoding.UTF8.GetBytes("first-console-token\n")))
            .SetConsoleObserver(firstObserver);
        CommandDetails second = CommandDetails.Create("ConsoleOverrideConcurrent")
            .SetInputStream(new MemoryStream(Encoding.UTF8.GetBytes("second-console-token\n")))
            .SetConsoleObserver(secondObserver);

        using MemoryStream firstInput = (MemoryStream)first.GetArgument<Stream>("InputStream")!;
        using MemoryStream secondInput = (MemoryStream)second.GetArgument<Stream>("InputStream")!;
        Guid?[] results = await Task.WhenAll(tool.ExecuteCommandAsync(first),tool.ExecuteCommandAsync(second));
        await Task.WhenAll(firstOutputReceived.Task,secondOutputReceived.Task).WaitAsync(TimeSpan.FromSeconds(10));

        Check.That(results.All(result => result is not null)).IsTrue();
        Check.That(String.Concat(firstOutput)).Contains("first-console-token");
        Check.That(String.Concat(firstOutput)).Not.Contains("second-console-token");
        Check.That(String.Concat(secondOutput)).Contains("second-console-token");
        Check.That(String.Concat(secondOutput)).Not.Contains("first-console-token");
    }

    [Test]
    public async Task BinaryConsoleDelegator_ConcurrentExecutions_KeepInputAndObserverIsolated()
    {
        ConcurrentQueue<String> firstOutput = new();
        ConcurrentQueue<String> secondOutput = new();
        TaskCompletionSource<Boolean> firstOutputReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> secondOutputReceived = new(TaskCreationOptions.RunContinuationsAsynchronously);
        BinaryCommandObserver firstObserver = CreateBinaryObserver(firstOutput,outputReceived:firstOutputReceived);
        BinaryCommandObserver secondObserver = CreateBinaryObserver(secondOutput,outputReceived:secondOutputReceived);
        CliWrap.Command process = Cli.Wrap("powershell.exe").WithArguments("-NoProfile -Command \"$line = [Console]::In.ReadLine(); Start-Sleep -Milliseconds 150; [Console]::Out.WriteLine($line)\"");
        Tool tool = new();

        Check.That(await tool.RegisterCommand("BinaryOverrideConcurrent",new BinaryConsoleDelegator(process))).IsTrue();
        Check.That(await tool.Activate()).IsTrue();
        Check.That(await tool.EnableAllCommands()).IsTrue();

        CommandDetails first = CommandDetails.Create("BinaryOverrideConcurrent")
            .SetInputStream(new MemoryStream(Encoding.UTF8.GetBytes("first-binary-token\n")))
            .SetBinaryConsoleObserver(firstObserver);
        CommandDetails second = CommandDetails.Create("BinaryOverrideConcurrent")
            .SetInputStream(new MemoryStream(Encoding.UTF8.GetBytes("second-binary-token\n")))
            .SetBinaryConsoleObserver(secondObserver);

        using MemoryStream firstInput = (MemoryStream)first.GetArgument<Stream>("InputStream")!;
        using MemoryStream secondInput = (MemoryStream)second.GetArgument<Stream>("InputStream")!;
        Guid?[] results = await Task.WhenAll(tool.ExecuteCommandAsync(first),tool.ExecuteCommandAsync(second));
        await Task.WhenAll(firstOutputReceived.Task,secondOutputReceived.Task).WaitAsync(TimeSpan.FromSeconds(10));

        Check.That(results.All(result => result is not null)).IsTrue();
        Check.That(String.Concat(firstOutput)).Contains("first-binary-token");
        Check.That(String.Concat(firstOutput)).Not.Contains("second-binary-token");
        Check.That(String.Concat(secondOutput)).Contains("second-binary-token");
        Check.That(String.Concat(secondOutput)).Not.Contains("first-binary-token");
    }

    private static ConsoleCommandObserver CreateTextObserver(ConcurrentQueue<String> output , Action? onStarted = null , TaskCompletionSource<Boolean>? outputReceived = null)
    {
        return new()
        {
            OnStarted = (_,_) => { onStarted?.Invoke(); return Task.CompletedTask; },
            OnStdOut = (e,_) => { output.Enqueue(e.Text); outputReceived?.TrySetResult(true); return Task.CompletedTask; },
            OnStdErr = (_,_) => Task.CompletedTask
        };
    }

    private static BinaryCommandObserver CreateBinaryObserver(ConcurrentQueue<String> output , TaskCompletionSource<Boolean>? outputReceived = null)
    {
        return new()
        {
            OnStdOut = async (stream,cancel) =>
            {
                using MemoryStream buffer = new();
                await stream.CopyToAsync(buffer,cancel).ConfigureAwait(false);
                output.Enqueue(Encoding.UTF8.GetString(buffer.ToArray()));
                outputReceived?.TrySetResult(true);
            },
            OnStdErr = async (stream,cancel) => await stream.CopyToAsync(Stream.Null,cancel).ConfigureAwait(false)
        };
    }
}
