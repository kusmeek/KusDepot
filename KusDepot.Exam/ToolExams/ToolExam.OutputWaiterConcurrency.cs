namespace KusDepot.Exams.Tools;

public partial class ToolExam
{
    [Test]
    public async Task GetOutputAsync_MultipleWaiters_AllReceivePublishedOutput()
    {
        using OutputWaiterProbeTool tool = new(); Guid id = Guid.NewGuid(); Object expected = new();
        Task<Object?>[] waiters = Enumerable.Range(0,8).Select(_ => tool.GetOutputAsync(id,timeout:ConcurrencyTestTimeout)).ToArray();

        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(8); Check.That(tool.AddOutput(id,expected)).IsTrue();
        Object?[] results = await Task.WhenAll(waiters).WaitAsync(ConcurrencyTestTimeout);

        Check.That(results.All(_ => ReferenceEquals(_,expected))).IsTrue();
        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(0); Check.That(tool.GetOutput(id)).IsSameReferenceAs(expected);
    }

    [Test]
    public async Task GetOutputAsync_NullOutput_WakesWaitersAndStoresID()
    {
        using OutputWaiterProbeTool tool = new(); Guid id = Guid.NewGuid();
        Task<Object?> first = tool.GetOutputAsync(id,timeout:ConcurrencyTestTimeout);
        Task<Object?> second = tool.GetOutputAsync(id,timeout:ConcurrencyTestTimeout);

        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(2); Check.That(tool.AddOutput(id,null)).IsTrue();

        Check.That(await first.WaitAsync(ConcurrencyTestTimeout)).IsNull(); Check.That(await second.WaitAsync(ConcurrencyTestTimeout)).IsNull();
        Check.That(tool.GetOutputIDs()).Contains(id); Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(0);
    }

    [Test]
    public async Task GetOutputAsync_CanceledWaiter_IsRemovedWithoutAffectingPeer()
    {
        using OutputWaiterProbeTool tool = new(); Guid id = Guid.NewGuid(); using CancellationTokenSource cancel = new();
        Task<Object?> canceled = tool.GetOutputAsync(id,cancel.Token,ConcurrencyTestTimeout);
        Task<Object?> pending = tool.GetOutputAsync(id,timeout:ConcurrencyTestTimeout);

        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(2); cancel.Cancel();
        Check.That(await canceled.WaitAsync(ConcurrencyTestTimeout)).IsNull(); Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(1);

        Check.That(tool.AddOutput(id,"remaining")).IsTrue(); Check.That(await pending.WaitAsync(ConcurrencyTestTimeout)).IsEqualTo("remaining");
        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(0);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task GetOutputAsync_DisposalCompletesPendingWaitersWithNull(Boolean asynchronous)
    {
        OutputWaiterProbeTool tool = new(); Guid id = Guid.NewGuid();
        Task<Object?>[] waiters = Enumerable.Range(0,4).Select(_ => tool.GetOutputAsync(id,timeout:Timeout.InfiniteTimeSpan)).ToArray();

        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(waiters.Length);

        if(asynchronous) { await tool.DisposeAsync().ConfigureAwait(false); }
        else { tool.Dispose(); }

        Object?[] results = await Task.WhenAll(waiters).WaitAsync(ConcurrencyTestTimeout).ConfigureAwait(false);

        Check.That(results.All(static result => result is null)).IsTrue();
        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(0);
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task GetOutputAsync_HostTeardownCanAwaitPendingWaiter(Boolean asynchronous)
    {
        OutputWaitingToolHost host = new(); Task<Object?> pending = host.StartPendingOutputWait();

        if(asynchronous) { await host.DisposeAsync().AsTask().WaitAsync(ConcurrencyTestTimeout).ConfigureAwait(false); }
        else { await Task.Run(host.Dispose).WaitAsync(ConcurrencyTestTimeout).ConfigureAwait(false); }

        Check.That(await pending.WaitAsync(ConcurrencyTestTimeout).ConfigureAwait(false)).IsNull();
        Check.That(host.WaitedOutput).IsNull(); Check.That(host.GetDisposed()).IsTrue();
    }

    [TestCase(false)]
    [TestCase(true)]
    public async Task GetOutputAsync_DisposalLoggingFailureStillDrainsWaiters(Boolean asynchronous)
    {
        OutputWaiterProbeTool tool = new(); tool.SetLogger(new ThrowingOutputLogger());
        Task<Object?> pending = tool.GetOutputAsync(Guid.NewGuid(),timeout:Timeout.InfiniteTimeSpan);
        Exception? failure = null;

        try
        {
            if(asynchronous) { await tool.DisposeAsync().AsTask().WaitAsync(ConcurrencyTestTimeout).ConfigureAwait(false); }
            else { await Task.Run(tool.Dispose).WaitAsync(ConcurrencyTestTimeout).ConfigureAwait(false); }
        }
        catch(Exception error) { failure = error; }

        Check.That(failure).IsNotNull(); Check.That(await pending.WaitAsync(ConcurrencyTestTimeout).ConfigureAwait(false)).IsNull();
        Check.That(tool.GetDisposed()).IsTrue();
    }

    [Test]
    public async Task GetOutputAsync_RegistrationRacingDisposal_AllWaitersTerminate()
    {
        for(Int32 iteration = 0; iteration < 25; iteration++)
        {
            OutputWaiterProbeTool tool = new(); Guid id = Guid.NewGuid();
            Task<Object?>[] waiters = Enumerable.Range(0,8).Select(_ => Task.Run(async () => await tool.GetOutputAsync(id,timeout:Timeout.InfiniteTimeSpan).ConfigureAwait(false))).ToArray();
            Task disposal = Task.Run(() => tool.Dispose());

            await disposal.WaitAsync(ConcurrencyTestTimeout).ConfigureAwait(false);
            Object?[] results = new Object?[waiters.Length];

            for(Int32 index = 0; index < waiters.Length; index++)
            {
                try { results[index] = await waiters[index].WaitAsync(ConcurrencyTestTimeout).ConfigureAwait(false); }
                catch(ObjectDisposedException) { }
            }

            Check.That(results.All(static result => result is null)).IsTrue();
            Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(0);
        }
    }

    [Test]
    public async Task GetOutputAsync_TimedOutWaiter_IsRemovedAndIDCanBeReused()
    {
        using OutputWaiterProbeTool tool = new(); Guid id = Guid.NewGuid();
        Task<Object?> timedout = tool.GetOutputAsync(id,timeout:TimeSpan.FromMilliseconds(100));

        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(1); Check.That(await timedout.WaitAsync(ConcurrencyTestTimeout)).IsNull();
        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(0);

        Task<Object?> reused = tool.GetOutputAsync(id,timeout:ConcurrencyTestTimeout);
        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(1); Check.That(tool.AddOutput(id,"reused")).IsTrue();
        Check.That(await reused.WaitAsync(ConcurrencyTestTimeout)).IsEqualTo("reused"); Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(0);
    }

    [Test]
    public async Task GetOutputAsync_InfiniteTimeout_ReturnsExistingOutputAndWaitsForPublication()
    {
        using OutputWaiterProbeTool tool = new(); Guid existingID = Guid.NewGuid(); Guid waitingID = Guid.NewGuid();
        Object existing = new(); Object published = new();
        Check.That(tool.AddOutput(existingID,existing)).IsTrue();
        Check.That(await tool.GetOutputAsync(existingID,timeout:Timeout.InfiniteTimeSpan).WaitAsync(ConcurrencyTestTimeout)).IsSameReferenceAs(existing);

        Task<Object?> waiting = tool.GetOutputAsync(waitingID,timeout:Timeout.InfiniteTimeSpan);
        Check.That(tool.GetOutputWaiterCount(waitingID)).IsEqualTo(1); Check.That(tool.AddOutput(waitingID,published)).IsTrue();

        Check.That(await waiting.WaitAsync(ConcurrencyTestTimeout)).IsSameReferenceAs(published);
        Check.That(tool.GetOutputWaiterCount(waitingID)).IsEqualTo(0);
    }

    [Test]
    public async Task GetOutputAsync_InfiniteTimeout_CancellationRemovesWaiter()
    {
        using OutputWaiterProbeTool tool = new(); Guid id = Guid.NewGuid(); using CancellationTokenSource cancel = new();
        Task<Object?> waiting = tool.GetOutputAsync(id,cancel.Token,Timeout.InfiniteTimeSpan);

        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(1); cancel.Cancel();

        Check.That(await waiting.WaitAsync(ConcurrencyTestTimeout)).IsNull();
        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(0);
    }

    [Test]
    public async Task GetOutputAsync_ZeroTimeout_OnlyChecksAlreadyPublishedOutput()
    {
        using OutputWaiterProbeTool tool = new(); Guid existingID = Guid.NewGuid(); Guid missingID = Guid.NewGuid();
        Object existing = new(); Check.That(tool.AddOutput(existingID,existing)).IsTrue();

        Check.That(await tool.GetOutputAsync(existingID,timeout:TimeSpan.Zero).WaitAsync(ConcurrencyTestTimeout)).IsSameReferenceAs(existing);
        Check.That(await tool.GetOutputAsync(missingID,timeout:TimeSpan.Zero).WaitAsync(ConcurrencyTestTimeout)).IsNull();
        Check.That(tool.GetOutputWaiterCount(missingID)).IsEqualTo(0);
    }

    [Test]
    public async Task GetRemoveOutputAsync_ConcurrentConsumers_ExactlyOneRemovesOutput()
    {
        using OutputWaiterProbeTool tool = new(); Guid id = Guid.NewGuid(); Object expected = new();
        Task<Object?> first = tool.GetRemoveOutputAsync(id,timeout:ConcurrencyTestTimeout);
        Task<Object?> second = tool.GetRemoveOutputAsync(id,timeout:ConcurrencyTestTimeout);

        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(2); Check.That(tool.AddOutput(id,expected)).IsTrue();
        Object?[] results = await Task.WhenAll(first,second).WaitAsync(ConcurrencyTestTimeout);

        Check.That(results.Count(_ => ReferenceEquals(_,expected))).IsEqualTo(1); Check.That(results.Count(_ => _ is null)).IsEqualTo(1);
        Check.That(tool.GetOutputIDs()).IsNull(); Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(0);
    }

    [Test]
    public async Task GetRemoveOutputAsync_InfiniteTimeout_WaitsForOutputAndRemovesIt()
    {
        using OutputWaiterProbeTool tool = new(); Guid id = Guid.NewGuid(); Object expected = new();
        Task<Object?> waiting = tool.GetRemoveOutputAsync(id,timeout:Timeout.InfiniteTimeSpan);

        Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(1); Check.That(tool.AddOutput(id,expected)).IsTrue();

        Check.That(await waiting.WaitAsync(ConcurrencyTestTimeout)).IsSameReferenceAs(expected);
        Check.That(tool.GetOutputIDs()).IsNull(); Check.That(tool.GetOutputWaiterCount(id)).IsEqualTo(0);
    }

    [Test]
    public async Task GetRemoveOutputAsync_ZeroTimeout_OnlyChecksAlreadyPublishedOutput()
    {
        using OutputWaiterProbeTool tool = new(); Guid existingID = Guid.NewGuid(); Guid missingID = Guid.NewGuid();
        Object existing = new(); Check.That(tool.AddOutput(existingID,existing)).IsTrue();

        Check.That(await tool.GetRemoveOutputAsync(existingID,timeout:TimeSpan.Zero).WaitAsync(ConcurrencyTestTimeout)).IsSameReferenceAs(existing);
        Check.That(await tool.GetRemoveOutputAsync(missingID,timeout:TimeSpan.Zero).WaitAsync(ConcurrencyTestTimeout)).IsNull();
        Check.That(tool.GetOutputIDs()).IsNull(); Check.That(tool.GetOutputWaiterCount(missingID)).IsEqualTo(0);
    }

    [Test]
    public async Task GetOutputAsync_NegativeTimeout_UsesNoExceptionsPolicy()
    {
        using OutputWaiterProbeTool tool = new(); Guid id = Guid.NewGuid();
        tool.EnableMyExceptions();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await tool.GetOutputAsync(id,timeout:TimeSpan.FromTicks(-1)));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await tool.GetRemoveOutputAsync(id,timeout:TimeSpan.FromTicks(-1)));

        tool.DisableMyExceptions();

        Check.That(await tool.GetOutputAsync(id,timeout:TimeSpan.FromTicks(-1))).IsNull();
        Check.That(await tool.GetRemoveOutputAsync(id,timeout:TimeSpan.FromTicks(-1))).IsNull();
    }
}
