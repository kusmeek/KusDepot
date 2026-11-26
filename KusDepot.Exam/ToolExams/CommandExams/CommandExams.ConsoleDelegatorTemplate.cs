using CliWrap.Exceptions;

namespace KusDepot.Exams.Tools;

public partial class CommandExams
{
    [Test]
    public async Task ExecuteConsoleDelegator_Template_FreeModeCancellation()
    {
        const String handle = "ConsoleDelegatorTemplateCanceled"; Guid id = Guid.NewGuid();
        CliWrap.Command process = Cli.Wrap("powershell.exe")
            .WithArguments("-NoProfile -Command \"Start-Sleep -Seconds 30\"");
        ConsoleDelegator command = new(process);
        Tool tool = new();
        using CancellationTokenSource cancellation = new();

        Check.That(await tool.RegisterCommand(handle,command)).IsTrue();
        Check.That(await tool.Activate()).IsTrue();
        Check.That(await tool.EnableAllCommands()).IsTrue();

        Guid? result = await tool.ExecuteCommandAsync(
            CommandDetails.Create(handle).SetID(id).SetFreeMode(true).PreserveActivity(true),cancellation.Token);
        Activity activity = tool.GetActivities()!.Single(a => a.ID == id);

        cancellation.Cancel();
        Object? completed = await activity.Task!.WaitAsync(TimeSpan.FromSeconds(10));
        KusDepot.CommandResult envelope = (KusDepot.CommandResult)completed!;

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
    public async Task ExecuteConsoleDelegator_Template_CommandExecutionFault()
    {
        const String handle = "ConsoleDelegatorTemplateFaulted"; const Int32 exitcode = 37; Guid id = Guid.NewGuid();
        CliWrap.Command process = Cli.Wrap("cmd.exe").WithArguments($"/c exit /b {exitcode}")
            .WithValidation(CommandResultValidation.ZeroExitCode);
        ConsoleDelegator command = new(process);
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
