namespace KusDepot.Data.Services.DataTransfer;

/**<include file='DataItemTransferCoordinator.xml' path='DataItemTransferCoordinator/class[@name="DataItemTransferCoordinator"]/main/*'/>*/
public sealed class DataItemTransferCoordinator : IDataItemTransferCoordinator
{
    /**<include file='DataItemTransferCoordinator.xml' path='DataItemTransferCoordinator/class[@name="DataItemTransferCoordinator"]/field[@name="locks"]/*'/>*/
    private readonly ConcurrentDictionary<String,SemaphoreSlim> locks = new(StringComparer.Ordinal);

    ///<inheritdoc/>
    public Task<IAsyncDisposable> AcquireItemLockAsync(Guid itemid , CancellationToken cancel = default)
    {
        return AcquireAsync($"item:{itemid:N}",cancel);
    }

    ///<inheritdoc/>
    public Task<IAsyncDisposable> AcquireSessionLockAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return AcquireAsync($"session:{sessionid:N}",cancel);
    }

    /**<include file='DataItemTransferCoordinator.xml' path='DataItemTransferCoordinator/class[@name="DataItemTransferCoordinator"]/method[@name="AcquireAsync"]/*'/>*/
    private async Task<IAsyncDisposable> AcquireAsync(String key , CancellationToken cancel)
    {
        SemaphoreSlim gate = this.locks.GetOrAdd(key,static _ => new SemaphoreSlim(1,1));

        await gate.WaitAsync(cancel).ConfigureAwait(false);

        return new Releaser(gate);
    }

    /**<include file='DataItemTransferCoordinator.xml' path='DataItemTransferCoordinator/class[@name="Releaser"]/main/*'/>*/
    private sealed class Releaser : IAsyncDisposable
    {
        /**<include file='DataItemTransferCoordinator.xml' path='DataItemTransferCoordinator/class[@name="Releaser"]/field[@name="gate"]/*'/>*/
        private readonly SemaphoreSlim gate;

        /**<include file='DataItemTransferCoordinator.xml' path='DataItemTransferCoordinator/class[@name="Releaser"]/field[@name="released"]/*'/>*/
        private Int32 released;

        /**<include file='DataItemTransferCoordinator.xml' path='DataItemTransferCoordinator/class[@name="Releaser"]/constructor[@name="Releaser"]/*'/>*/
        public Releaser(SemaphoreSlim gate)
        {
            this.gate = gate;
        }

        /**<include file='DataItemTransferCoordinator.xml' path='DataItemTransferCoordinator/class[@name="Releaser"]/method[@name="DisposeAsync"]/*'/>*/
        public ValueTask DisposeAsync()
        {
            if(Interlocked.Exchange(ref this.released,1) == 0) { this.gate.Release(); }

            return ValueTask.CompletedTask;
        }
    }
}
