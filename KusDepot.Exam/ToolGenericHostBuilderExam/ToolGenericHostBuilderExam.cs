namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolGenericHostBuilderExam
{
    [Test]
    public void BuildGenericHost()
    {
        Check.That(ToolBuilderFactory.CreateGenericHostBuilder().BuildGenericHost().GetType().IsAssignableTo(typeof(IToolGenericHost))).IsTrue();
    }

    [Test]
    public void BuildGenericHostOpen()
    {
        Check.That(ToolBuilderFactory.CreateGenericHostBuilder().BuildGenericHost<ToolGenericHost>().GetType().IsAssignableTo(typeof(IToolGenericHost))).IsTrue();
    }

    [Test]
    public async Task BuildGenericHostAsync()
    {
        Check.That((await ToolBuilderFactory.CreateGenericHostBuilder().BuildGenericHostAsync()).GetType().IsAssignableTo(typeof(IToolGenericHost))).IsTrue();
    }

    [Test]
    public async Task BuildGenericHostAsyncOpen()
    {
        Check.That((await ToolBuilderFactory.CreateGenericHostBuilder().BuildGenericHostAsync<ToolGenericHost>()).GetType().IsAssignableTo(typeof(IToolGenericHost))).IsTrue();
    }

    [Test]
    public async Task BuildGenericHostAsync_CanceledCompetingBuildDoesNotDisposeActiveBuild()
    {
        TaskCompletionSource<ITool> captured = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        IToolGenericHostBuilder builder = ToolBuilderFactory.CreateGenericHostBuilder();
        builder.ConfigureTool(async (_,tool,cancel) =>
        {
            captured.TrySetResult(tool);
            await release.Task.WaitAsync(cancel).ConfigureAwait(false);
        });
        Task<IToolGenericHost> firstBuild = builder.BuildGenericHostAsync();

        try
        {
            await captured.Task.WaitAsync(TimeSpan.FromSeconds(20));
            using CancellationTokenSource cancellation = new(); cancellation.Cancel();
            await Assert.CatchAsync<OperationCanceledException>(async () => await builder.BuildGenericHostAsync(cancel:cancellation.Token));
        }
        finally { release.TrySetResult(true); }

        using IToolGenericHost built = await firstBuild.WaitAsync(TimeSpan.FromSeconds(20));
        Check.That(built.GetDisposed()).IsFalse();
        await Assert.ThrowsAsync<OperationFailedException>(async () => await builder.BuildGenericHostAsync());
    }
}