namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolHostBuilderExam
{
    [Test]
    public void BuildHost()
    {
        Check.That(ToolBuilderFactory.CreateHostBuilder().BuildHost().GetType().IsAssignableTo(typeof(IToolHost))).IsTrue();
    }

    [Test]
    public void BuildHostOpen()
    {
        Check.That(ToolBuilderFactory.CreateHostBuilder().BuildHost<ToolHost>().GetType().IsAssignableTo(typeof(IToolHost))).IsTrue();
    }

    [Test]
    public async Task BuildHostAsync()
    {
        Check.That((await ToolBuilderFactory.CreateHostBuilder().BuildHostAsync()).GetType().IsAssignableTo(typeof(IToolHost))).IsTrue();
    }

    [Test]
    public async Task BuildHostAsyncOpen()
    {
        Check.That((await ToolBuilderFactory.CreateHostBuilder().BuildHostAsync<ToolHost>()).GetType().IsAssignableTo(typeof(IToolHost))).IsTrue();
    }

    [Test]
    public async Task BuildHostAsync_ConfigureActionPreservesAsyncLocalAcrossSuspension()
    {
        AsyncLocal<String?> context = new(); context.Value = "builder-context";
        TaskCompletionSource<Boolean> callbackEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> releaseCallback = new(TaskCreationOptions.RunContinuationsAsynchronously);
        String? observed = null;
        IToolHostBuilder builder = ToolBuilderFactory.CreateHostBuilder();
        builder.ConfigureTool(async (_,_,cancel) =>
        {
            callbackEntered.TrySetResult(true);
            await releaseCallback.Task.WaitAsync(cancel).ConfigureAwait(false);
            observed = context.Value;
        });
        Task<IToolHost> build = builder.BuildHostAsync();

        try
        {
            await callbackEntered.Task.WaitAsync(TimeSpan.FromSeconds(20));
        }
        finally { releaseCallback.TrySetResult(true); }

        using IToolHost built = await build.WaitAsync(TimeSpan.FromSeconds(20));
        Check.That(observed).IsEqualTo("builder-context");
    }

    [Test]
    public async Task BuildHostAsync_CanceledCompetingBuildDoesNotDisposeActiveBuild()
    {
        TaskCompletionSource<ITool> captured = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        IToolHostBuilder builder = ToolBuilderFactory.CreateHostBuilder();
        builder.ConfigureTool(async (_,tool,cancel) =>
        {
            captured.TrySetResult(tool);
            await release.Task.WaitAsync(cancel).ConfigureAwait(false);
        });
        Task<IToolHost> firstBuild = builder.BuildHostAsync();

        try
        {
            await captured.Task.WaitAsync(TimeSpan.FromSeconds(20));
            using CancellationTokenSource cancellation = new(); cancellation.Cancel();
            await Assert.CatchAsync<OperationCanceledException>(async () => await builder.BuildHostAsync(cancel:cancellation.Token));
        }
        finally { release.TrySetResult(true); }

        using IToolHost built = await firstBuild.WaitAsync(TimeSpan.FromSeconds(20));
        Check.That(built.GetDisposed()).IsFalse();
        await Assert.ThrowsAsync<OperationFailedException>(async () => await builder.BuildHostAsync());
    }
}