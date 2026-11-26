namespace KusDepot.Exams.Tools;

[TestFixture] [Parallelizable(ParallelScope.All)]
public class ToolAspireHostBuilderExam
{
    [Test]
    public void BuildAspireHost()
    {
        Check.That(ToolBuilderFactory.CreateAspireHostBuilder().BuildAspireHost().GetType().IsAssignableTo(typeof(IToolAspireHost))).IsTrue();
    }

    [Test]
    public void BuildAspireHostOpen()
    {
        Check.That(ToolBuilderFactory.CreateAspireHostBuilder().BuildAspireHost<ToolAspireHost>().GetType().IsAssignableTo(typeof(IToolAspireHost))).IsTrue();
    }

    [Test]
    public async Task BuildAspireHostAsync()
    {
        Check.That((await ToolBuilderFactory.CreateAspireHostBuilder().BuildAspireHostAsync()).GetType().IsAssignableTo(typeof(IToolAspireHost))).IsTrue();
    }

    [Test]
    public async Task BuildAspireHostAsyncOpen()
    {
        Check.That((await ToolBuilderFactory.CreateAspireHostBuilder().BuildAspireHostAsync<ToolAspireHost>()).GetType().IsAssignableTo(typeof(IToolAspireHost))).IsTrue();
    }

    [Test]
    public async Task BuildAspireHostAsync_ConfigureActionPreservesAsyncLocalAcrossSuspension()
    {
        AsyncLocal<String?> context = new(); context.Value = "aspire-context";
        TaskCompletionSource<Boolean> callbackEntered = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> releaseCallback = new(TaskCreationOptions.RunContinuationsAsynchronously);
        String? observed = null;
        IToolAspireHostBuilder builder = ToolBuilderFactory.CreateAspireHostBuilder();
        builder.ConfigureTool(async (_,_,cancel) =>
        {
            callbackEntered.TrySetResult(true);
            await releaseCallback.Task.WaitAsync(cancel).ConfigureAwait(false);
            observed = context.Value;
        });
        Task<IToolAspireHost> build = builder.BuildAspireHostAsync();

        try
        {
            await callbackEntered.Task.WaitAsync(TimeSpan.FromSeconds(20));
        }
        finally { releaseCallback.TrySetResult(true); }

        using IToolAspireHost built = await build.WaitAsync(TimeSpan.FromSeconds(20));
        Check.That(observed).IsEqualTo("aspire-context");
    }

    [Test]
    public async Task BuildAspireHostAsync_CanceledCompetingBuildDoesNotDisposeActiveBuild()
    {
        TaskCompletionSource<ITool> captured = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        IToolAspireHostBuilder builder = ToolBuilderFactory.CreateAspireHostBuilder();
        builder.ConfigureTool(async (_,tool,cancel) =>
        {
            captured.TrySetResult(tool);
            await release.Task.WaitAsync(cancel).ConfigureAwait(false);
        });
        Task<IToolAspireHost> firstBuild = builder.BuildAspireHostAsync();

        try
        {
            await captured.Task.WaitAsync(TimeSpan.FromSeconds(20));
            using CancellationTokenSource cancellation = new(); cancellation.Cancel();
            await Assert.CatchAsync<OperationCanceledException>(async () => await builder.BuildAspireHostAsync(cancel:cancellation.Token));
        }
        finally { release.TrySetResult(true); }

        using IToolAspireHost built = await firstBuild.WaitAsync(TimeSpan.FromSeconds(20));
        Check.That(built.GetDisposed()).IsFalse();
        await Assert.ThrowsAsync<OperationFailedException>(async () => await builder.BuildAspireHostAsync());
    }
}