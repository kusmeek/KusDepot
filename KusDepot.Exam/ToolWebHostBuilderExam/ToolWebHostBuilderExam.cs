namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolWebHostBuilderExam
{
    [Test]
    public void BuildWebHost()
    {
        Check.That(ToolBuilderFactory.CreateWebHostBuilder().BuildWebHost().GetType().IsAssignableTo(typeof(IToolWebHost))).IsTrue();
    }

    [Test]
    public void BuildWebHostOpen()
    {
        Check.That(ToolBuilderFactory.CreateWebHostBuilder().BuildWebHost<ToolWebHost>().GetType().IsAssignableTo(typeof(IToolWebHost))).IsTrue();
    }

    [Test]
    public async Task BuildWebHostAsync()
    {
        Check.That((await ToolBuilderFactory.CreateWebHostBuilder().BuildWebHostAsync()).GetType().IsAssignableTo(typeof(IToolWebHost))).IsTrue();
    }

    [Test]
    public async Task BuildWebHostAsyncOpen()
    {
        Check.That((await ToolBuilderFactory.CreateWebHostBuilder().BuildWebHostAsync<ToolWebHost>()).GetType().IsAssignableTo(typeof(IToolWebHost))).IsTrue();
    }

    [Test]
    public async Task BuildWebHostAsync_CanceledCompetingBuildDoesNotDisposeActiveBuild()
    {
        TaskCompletionSource<ITool> captured = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        IToolWebHostBuilder builder = ToolBuilderFactory.CreateWebHostBuilder();
        builder.ConfigureTool(async (_,tool,cancel) =>
        {
            captured.TrySetResult(tool);
            await release.Task.WaitAsync(cancel).ConfigureAwait(false);
        });
        Task<IToolWebHost> firstBuild = builder.BuildWebHostAsync();

        try
        {
            await captured.Task.WaitAsync(TimeSpan.FromSeconds(20));
            using CancellationTokenSource cancellation = new(); cancellation.Cancel();
            await Assert.CatchAsync<OperationCanceledException>(async () => await builder.BuildWebHostAsync(cancel:cancellation.Token));
        }
        finally { release.TrySetResult(true); }

        using IToolWebHost built = await firstBuild.WaitAsync(TimeSpan.FromSeconds(20));
        Check.That(built.GetDisposed()).IsFalse();
        await Assert.ThrowsAsync<OperationFailedException>(async () => await builder.BuildWebHostAsync());
    }
}