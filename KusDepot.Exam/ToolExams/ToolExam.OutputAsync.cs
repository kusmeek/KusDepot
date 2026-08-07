namespace KusDepot.Exams.Tools;

public partial class ToolExam
{
    [Test]
    public async Task GetOutputAsync_CompletesBeforePollingIntervalWhenOutputArrives()
    {
        Tool tool = new(); Guid id = Guid.NewGuid();

        Task<Object?> pending = tool.GetOutputAsync(id,timeout: TimeSpan.FromSeconds(1),interval: 5000);

        await Task.Delay(50);

        Check.That(tool.AddOutput(id,"Pass")).IsTrue();
        Check.That(await pending).Equals("Pass");
    }

    [Test]
    public async Task GetOutputAsync_AllowsAnotherWaiterToReceiveOutputAfterOneCancellation()
    {
        Tool tool = new(); Guid id = Guid.NewGuid(); using CancellationTokenSource cancel = new();

        Task<Object?> canceled = tool.GetOutputAsync(id,cancel.Token,TimeSpan.FromSeconds(2),5000);
        Task<Object?> pending = tool.GetOutputAsync(id,timeout: TimeSpan.FromSeconds(2),interval: 5000);

        cancel.CancelAfter(50);

        await Task.Delay(150);

        Check.That(tool.AddOutput(id,"Pass")).IsTrue();
        Check.That(await canceled).IsNull();
        Check.That(await pending).Equals("Pass");
    }
}
