using CliWrap.Exceptions;

namespace KusDepot.Exams.Tools;

public partial class CommandExams
{
    [Test]
    public async Task ExecuteBinaryConsoleDelegator_Template_SynchronousBufferedSuccess()
    {
        const String handle = "BinaryConsoleDelegatorTemplateSync"; Guid id = Guid.NewGuid();
        CliWrap.Command process = Cli.Wrap("cmd.exe").WithArguments("/c echo binary-template-sync");
        BinaryConsoleDelegator command = new(process,buffered:true);
        Tool tool = new();

        Check.That(await tool.RegisterCommand(handle,command)).IsTrue();
        Check.That(await tool.Activate()).IsTrue();
        Check.That(await tool.EnableAllCommands()).IsTrue();

        Guid? result = tool.ExecuteCommand(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);

        Check.That(result).IsEqualTo(id);
        Check.That(activity.Task).IsNull();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Success);
        Check.That(activity.Record.HasOutput).IsTrue();
        Check.That(activity.Record.Exception).IsNull();
        Check.That(command.CommandResult).IsInstanceOf<BinaryCommandResult>();
        Check.That(tool.GetOutput(id)).IsSameReferenceAs(command.CommandResult);

        BinaryCommandResult output = (BinaryCommandResult)command.CommandResult!;
        Check.That(output.IsSuccess).IsTrue();
        Check.That(Encoding.Default.GetString(output.StandardOutputBytes)
            .Contains("binary-template-sync",StringComparison.OrdinalIgnoreCase)).IsTrue();
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegator_Template_FreeModeBufferedCancellation()
    {
        const String handle = "BinaryConsoleDelegatorTemplateCanceled"; Guid id = Guid.NewGuid();
        CliWrap.Command process = Cli.Wrap("powershell.exe")
            .WithArguments("-NoProfile -Command \"Start-Sleep -Seconds 30\"");
        BinaryConsoleDelegator command = new(process,buffered:true);
        Tool tool = new();
        using CancellationTokenSource cancellation = new();

        Check.That(await tool.RegisterCommand(handle,command)).IsTrue();
        Check.That(await tool.Activate()).IsTrue();
        Check.That(await tool.EnableAllCommands()).IsTrue();

        Guid? result = await tool.ExecuteCommandAsync(
            CommandDetails.Create(handle).SetID(id).SetFreeMode(true).PreserveActivity(true),cancellation.Token);
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);

        cancellation.Cancel();
        KusDepot.CommandResult envelope = (KusDepot.CommandResult)(await activity.Task!.WaitAsync(TimeSpan.FromSeconds(10)))!;

        Check.That(result).IsEqualTo(id);
        Check.That(activity.Task.IsCompletedSuccessfully).IsTrue();
        Check.That(envelope.Code).IsEqualTo(ActivityRecordCode.Canceled);
        Check.That(envelope.HasOutput).IsTrue();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Canceled);
        Check.That(activity.Record.HasOutput).IsTrue();
        Check.That(activity.Record.Exception).IsInstanceOf<OperationCanceledException>();
        Check.That(activity.Record.Exception).IsSameReferenceAs(envelope.Exception);
        Check.That(tool.GetOutput(id)).IsInstanceOf<CanceledCommandResult>();
    }

    [Test]
    public async Task ExecuteBinaryConsoleDelegator_Template_CommandExecutionFault()
    {
        const String handle = "BinaryConsoleDelegatorTemplateFaulted"; const Int32 exitcode = 37; Guid id = Guid.NewGuid();
        CliWrap.Command process = Cli.Wrap("cmd.exe").WithArguments($"/c exit /b {exitcode}")
            .WithValidation(CommandResultValidation.ZeroExitCode);
        BinaryConsoleDelegator command = new(process,buffered:true);
        Tool tool = new();

        Check.That(await tool.RegisterCommand(handle,command)).IsTrue();
        Check.That(await tool.Activate()).IsTrue();
        Check.That(await tool.EnableAllCommands()).IsTrue();

        Guid? result = await tool.ExecuteCommandAsync(CommandDetails.Create(handle).SetID(id).PreserveActivity(true));
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);
        KusDepot.CommandResult envelope = (KusDepot.CommandResult)(await activity.Task!)!;

        Check.That(result).IsNull();
        Check.That(activity.Task!.IsCompletedSuccessfully).IsTrue();
        Check.That(envelope.Code).IsEqualTo(ActivityRecordCode.Faulted);
        Check.That(envelope.HasOutput).IsTrue();
        Check.That(activity.Record).IsNotNull();
        Check.That(activity.Record!.Code).IsEqualTo(ActivityRecordCode.Faulted);
        Check.That(activity.Record.HasOutput).IsTrue();
        Check.That(activity.Record.Exception).IsInstanceOf<CommandExecutionException>();
        Check.That(activity.Record.Exception).IsSameReferenceAs(envelope.Exception);
        Check.That(tool.GetOutput(id)).IsInstanceOf<FaultedCommandResult>();
        Check.That(((FaultedCommandResult)tool.GetOutput(id)!).ExitCode).IsEqualTo(exitcode);
    }
}
