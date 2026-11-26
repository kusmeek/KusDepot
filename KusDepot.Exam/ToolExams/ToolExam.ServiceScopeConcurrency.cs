namespace KusDepot.Exams.Tools;

public partial class ToolExam
{
    [Test]
    public async Task CreateToolServiceScope_ConcurrentDuplicateName_HasOneWinner()
    {
        using ITool tool = ToolBuilderFactory.CreateBuilder().Build();
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Task<ToolServiceScope?>[] attempts = Enumerable.Range(0,16).Select(index => Task.Run(async () =>
        {
            await release.Task.ConfigureAwait(false); return tool.CreateToolServiceScope("shared");
        })).ToArray();

        release.TrySetResult(true); ToolServiceScope?[] results = await Task.WhenAll(attempts).WaitAsync(ConcurrencyTestTimeout);

        Check.That(results.Count(_ => _ is not null)).IsEqualTo(1);
        Check.That(tool.GetToolServiceScopeNames()).ContainsExactly("default","shared");
        results.Single(_ => _ is not null)!.Dispose();
        Check.That(tool.GetToolServiceScopeNames()).ContainsExactly("default");
    }

    [Test]
    public async Task ToolServiceScope_ConcurrentHandles_DisposesAfterLastReleaseOnce()
    {
        using ITool tool = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,services) => services.AddScoped<CountingScopedService>()).Build();
        ToolServiceScope owner = tool.CreateToolServiceScope("shared") ?? throw new InvalidOperationException();
        ToolServiceScope[] acquired = await Task.WhenAll(Enumerable.Range(0,7).Select(_ => Task.Run(() => tool.GetToolServiceScope("shared")!))).WaitAsync(ConcurrencyTestTimeout);
        ToolServiceScope[] handles = [ owner, .. acquired ]; CountingScopedService service = owner.ServiceProvider.GetRequiredService<CountingScopedService>();

        Check.That(handles.All(_ => ReferenceEquals(_.ServiceProvider,owner.ServiceProvider))).IsTrue();
        await Task.WhenAll(handles[..^1].Select(_ => Task.Run(_.Dispose))).WaitAsync(ConcurrencyTestTimeout);
        Check.That(service.DisposeCount).IsEqualTo(0); Check.That(tool.GetToolServiceScopeNames()).ContainsExactly("default","shared");

        handles[^1].Dispose();

        Check.That(service.DisposeCount).IsEqualTo(1); Check.That(tool.GetToolServiceScope("shared")).IsNull();
        Check.That(tool.GetToolServiceScopeNames()).ContainsExactly("default");
    }

    [Test]
    public async Task ToolDisposeAsync_RacingOutstandingScopeHandles_DisposesScopeOnce()
    {
        ITool tool = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,services) => services.AddScoped<CountingScopedService>()).Build();
        ToolServiceScope owner = tool.CreateToolServiceScope("shared") ?? throw new InvalidOperationException();
        ToolServiceScope[] acquired = Enumerable.Range(0,7).Select(_ => tool.GetToolServiceScope("shared")!).ToArray();
        ToolServiceScope[] handles = [ owner, .. acquired ]; CountingScopedService service = owner.ServiceProvider.GetRequiredService<CountingScopedService>();
        TaskCompletionSource<Boolean> release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Task toolDisposal = Task.Run(async () => { await release.Task.ConfigureAwait(false); await tool.DisposeAsync().ConfigureAwait(false); });
        Task[] handleDisposals = handles.Select(scope => Task.Run(async () => { await release.Task.ConfigureAwait(false); await scope.DisposeAsync().ConfigureAwait(false); })).ToArray();

        release.TrySetResult(true); await Task.WhenAll(handleDisposals.Append(toolDisposal)).WaitAsync(ConcurrencyTestTimeout);

        Check.That(tool.GetDisposed()).IsTrue(); Check.That(service.DisposeCount).IsEqualTo(1);
        foreach(ToolServiceScope scope in handles) { Check.ThatCode(() => _ = scope.ServiceProvider).Throws<ObjectDisposedException>(); }
    }

    [Test]
    public async Task ToolDisposeAsync_ClosesScopeAdmissionBeforeDisposingCapturedScopes()
    {
        ITool tool = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,services) => services.AddScoped<GatedAsyncScopedService>()).Build();
        ToolServiceScope firstScope = tool.CreateToolServiceScope("first") ?? throw new InvalidOperationException();
        ToolServiceScope secondScope = tool.CreateToolServiceScope("second") ?? throw new InvalidOperationException();
        GatedAsyncScopedService firstService = firstScope.ServiceProvider.GetRequiredService<GatedAsyncScopedService>();
        GatedAsyncScopedService secondService = secondScope.ServiceProvider.GetRequiredService<GatedAsyncScopedService>();

        Task disposal = tool.DisposeAsync().AsTask(); await firstService.Entered.WaitAsync(ConcurrencyTestTimeout);

        try
        {
            Check.ThatCode(() => tool.GetToolServiceScope("first")).Throws<ObjectDisposedException>();
            Check.ThatCode(() => tool.CreateToolServiceScope("late")).Throws<ObjectDisposedException>();
            Check.ThatCode(() => tool.GetToolServiceScopeNames()).Throws<ObjectDisposedException>();

            ToolServiceScopeRegistry registry = (ToolServiceScopeRegistry)(typeof(Tool).GetField("ToolServiceScopes",BindingFlags.Instance|BindingFlags.NonPublic)?.GetValue(tool)
                ?? throw new InvalidOperationException());
            MethodInfo get = typeof(ToolServiceScopeRegistry).GetMethod("Get",BindingFlags.Instance|BindingFlags.NonPublic) ?? throw new InvalidOperationException();
            MethodInfo create = typeof(ToolServiceScopeRegistry).GetMethod("Create",BindingFlags.Instance|BindingFlags.NonPublic) ?? throw new InvalidOperationException();
            MethodInfo getNames = typeof(ToolServiceScopeRegistry).GetMethod("GetNames",BindingFlags.Instance|BindingFlags.NonPublic) ?? throw new InvalidOperationException();

            Check.That(get.Invoke(registry,["first"])).IsNull();
            Check.That(create.Invoke(registry,["late"])).IsNull();
            Check.That((IEnumerable<String>)getNames.Invoke(registry,null)!).IsEmpty();
            Check.That(disposal.IsCompleted).IsFalse();

            firstService.Release(); await secondService.Entered.WaitAsync(ConcurrencyTestTimeout);
            Check.That(firstService.DisposeCount).IsEqualTo(1); Check.That(secondService.DisposeCount).IsEqualTo(1);
            Check.That(disposal.IsCompleted).IsFalse(); secondService.Release();
        }
        finally
        {
            firstService.Release(); secondService.Release(); await disposal.WaitAsync(ConcurrencyTestTimeout);
        }

        Check.That(tool.GetDisposed()).IsTrue();
        Check.That(firstService.DisposeCount).IsEqualTo(1); Check.That(secondService.DisposeCount).IsEqualTo(1);
        Check.ThatCode(() => _ = firstScope.ServiceProvider).Throws<ObjectDisposedException>();
        Check.ThatCode(() => _ = secondScope.ServiceProvider).Throws<ObjectDisposedException>();
    }

    [Test]
    public async Task ToolDisposeAsync_ScopeFailure_AggregatesAndContinuesDisposal()
    {
        ITool tool = ToolBuilderFactory.CreateBuilder().ConfigureToolServices((_,services) =>
        {
            services.AddScoped<ThrowingAsyncScopedService>(); services.AddScoped<CountingScopedService>();
        }).Build();
        ToolServiceScope failingScope = tool.CreateToolServiceScope("failing") ?? throw new InvalidOperationException();
        ToolServiceScope laterScope = tool.CreateToolServiceScope("later") ?? throw new InvalidOperationException();
        ThrowingAsyncScopedService failing = failingScope.ServiceProvider.GetRequiredService<ThrowingAsyncScopedService>();
        CountingScopedService later = laterScope.ServiceProvider.GetRequiredService<CountingScopedService>();

        AggregateException? error = null;

        try { await tool.DisposeAsync().ConfigureAwait(false); }
        catch(AggregateException exception) { error = exception; }

        Check.That(error).IsNotNull(); Check.That(tool.GetDisposed()).IsTrue();
        Check.That(failing.DisposeCount).IsEqualTo(1); Check.That(later.DisposeCount).IsEqualTo(1);
        Check.ThatCode(() => _ = failingScope.ServiceProvider).Throws<ObjectDisposedException>();
        Check.ThatCode(() => _ = laterScope.ServiceProvider).Throws<ObjectDisposedException>();
    }
}
