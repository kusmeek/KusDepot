namespace DataPodServices.DataControl;

/**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresDataItemTransferCoordinator"]/main/*'/>*/
public sealed class PostgresDataItemTransferCoordinator : IDataItemTransferCoordinator
{
    /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresDataItemTransferCoordinator"]/field[@name="dataSource"]/*'/>*/
    private readonly NpgsqlDataSource dataSource;

    /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresDataItemTransferCoordinator"]/constructor[@name="PostgresDataItemTransferCoordinator"]/*'/>*/
    public PostgresDataItemTransferCoordinator(NpgsqlDataSource dataSource)
    {
        this.dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    /// <inheritdoc/>
    public Task<IAsyncDisposable> AcquireItemLockAsync(Guid itemid , CancellationToken cancel = default)
    {
        return AcquireLockAsync("item",itemid,cancel);
    }

    /// <inheritdoc/>
    public async Task<ITransferItemLockLease> AcquireItemLockLeaseAsync(Guid itemid , CancellationToken cancel = default)
    {
        if(itemid == Guid.Empty) { throw new ArgumentException("A non-empty lock identifier is required.",nameof(itemid)); }

        NpgsqlConnection connection = await this.dataSource.OpenConnectionAsync(cancel);

        try
        {
            Int64 key = ComputeLockKey("item",itemid);

            await AcquireAdvisoryLockAsync(connection,key,cancel);

            return new PostgresItemLockLease(this.dataSource,connection,key);
        }
        catch
        {
            try { this.dataSource.Clear(); }

            finally { await connection.DisposeAsync(); }

            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IAsyncDisposable?> TryAcquireItemLockAsync(Guid itemid , CancellationToken cancel = default)
    {
        if(itemid == Guid.Empty) { throw new ArgumentException("A non-empty lock identifier is required.",nameof(itemid)); }

        NpgsqlConnection connection = await this.dataSource.OpenConnectionAsync(cancel);

        Int64 key = ComputeLockKey("item",itemid);

        try
        {
            await using NpgsqlCommand command = new("SELECT pg_try_advisory_lock(@key)",connection);

            command.Parameters.AddWithValue("key",key);

            Boolean acquired = (Boolean)(await command.ExecuteScalarAsync(cancel) ?? false);

            if(acquired) { return new PostgresLock(connection,key); }

            await connection.DisposeAsync();

            return null;
        }
        catch
        {
            NpgsqlConnection.ClearPool(connection);

            await connection.DisposeAsync();

            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<ITransferMutationScope> AcquireMutationScopeAsync(Guid itemid , Guid sessionid , CancellationToken cancel = default)
    {
        if(sessionid == Guid.Empty) { throw new ArgumentException("A non-empty lock identifier is required.",nameof(sessionid)); }

        ITransferItemLockLease lease = await this.AcquireItemLockLeaseAsync(itemid,cancel);

        try
        {
            await lease.AcquireSessionLockAsync(sessionid,cancel);

            return new TransferMutationScope(itemid,sessionid,lease);
        }
        catch
        {
            await lease.DisposeAsync();

            throw;
        }
    }

    /// <inheritdoc/>
    public Task<IAsyncDisposable> AcquireSessionLockAsync(Guid sessionid , CancellationToken cancel = default)
    {
        return AcquireLockAsync("session",sessionid,cancel);
    }

    /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresDataItemTransferCoordinator"]/method[@name="AcquireLockAsync"]/*'/>*/
    private async Task<IAsyncDisposable> AcquireLockAsync(String kind , Guid id , CancellationToken cancel)
    {
        if(id == Guid.Empty) { throw new ArgumentException("A non-empty lock identifier is required.",nameof(id)); }

        NpgsqlConnection connection = await this.dataSource.OpenConnectionAsync(cancel);

        try
        {
            Int64 key = ComputeLockKey(kind,id);

            await AcquireAdvisoryLockAsync(connection,key,cancel);

            return new PostgresLock(connection,key);
        }
        catch
        {
            await connection.DisposeAsync();

            throw;
        }
    }

    /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresDataItemTransferCoordinator"]/method[@name="AcquireAdvisoryLockAsync"]/*'/>*/
    private static async Task AcquireAdvisoryLockAsync(NpgsqlConnection connection , Int64 key , CancellationToken cancel)
    {
        await using NpgsqlCommand command = new("SELECT pg_advisory_lock(@key)",connection);

        command.Parameters.AddWithValue("key",key);

        await command.ExecuteNonQueryAsync(cancel);
    }

    /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresDataItemTransferCoordinator"]/method[@name="ComputeLockKey"]/*'/>*/
    private static Int64 ComputeLockKey(String kind , Guid id)
    {
        Byte[] digest = SHA256.HashData($"{kind}:{id:N}".ToByteArrayFromUTF16String());

        return BitConverter.ToInt64(digest,0);
    }

    /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="TransferMutationScope"]/main/*'/>*/
    private sealed class TransferMutationScope : ITransferMutationScope
    {
        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="TransferMutationScope"]/field[@name="lease"]/*'/>*/
        private readonly ITransferItemLockLease lease;

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="TransferMutationScope"]/field[@name="released"]/*'/>*/
        private Int32 released;

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="TransferMutationScope"]/constructor[@name="TransferMutationScope"]/*'/>*/
        public TransferMutationScope(Guid itemid , Guid sessionid , ITransferItemLockLease lease)
        {
            this.ItemID = itemid;
            this.SessionID = sessionid;
            this.lease = lease;
        }

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="TransferMutationScope"]/property[@name="ItemID"]/*'/>*/
        public Guid ItemID { get; }

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="TransferMutationScope"]/property[@name="SessionID"]/*'/>*/
        public Guid SessionID { get; }

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="TransferMutationScope"]/method[@name="DisposeAsync"]/*'/>*/
        public async ValueTask DisposeAsync()
        {
            if(Interlocked.Exchange(ref this.released,1) == 0)
            {
                await this.lease.DisposeAsync();
            }
        }
    }

    /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresItemLockLease"]/main/*'/>*/
    private sealed class PostgresItemLockLease : ITransferItemLockLease
    {
        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresItemLockLease"]/field[@name="dataSource"]/*'/>*/
        private readonly NpgsqlDataSource dataSource;

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresItemLockLease"]/field[@name="connection"]/*'/>*/
        private readonly NpgsqlConnection connection;

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresItemLockLease"]/field[@name="itemKey"]/*'/>*/
        private readonly Int64 itemKey;

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresItemLockLease"]/field[@name="gate"]/*'/>*/
        private readonly SemaphoreSlim gate = new(1,1);

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresItemLockLease"]/field[@name="sessionKey"]/*'/>*/
        private Int64? sessionKey;

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresItemLockLease"]/field[@name="released"]/*'/>*/
        private Boolean released;

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresItemLockLease"]/constructor[@name="PostgresItemLockLease"]/*'/>*/
        public PostgresItemLockLease(NpgsqlDataSource dataSource , NpgsqlConnection connection , Int64 itemKey)
        {
            this.dataSource = dataSource; this.connection = connection; this.itemKey = itemKey;
        }

        ///<inheritdoc/>
        public async Task AcquireSessionLockAsync(Guid sessionid , CancellationToken cancel = default)
        {
            if(sessionid == Guid.Empty) { throw new ArgumentException("A non-empty lock identifier is required.",nameof(sessionid)); }

            await this.gate.WaitAsync(cancel);

            try
            {
                ObjectDisposedException.ThrowIf(this.released,this);

                if(this.sessionKey.HasValue) { throw new InvalidOperationException("The lease already owns a session lock."); }

                Int64 key = ComputeLockKey("session",sessionid);

                try
                {
                    await AcquireAdvisoryLockAsync(this.connection,key,cancel);

                    this.sessionKey = key;
                }
                catch
                {
                    this.released = true;

                    try { this.dataSource.Clear(); }

                    finally { await this.connection.DisposeAsync(); }

                    throw;
                }
            }
            finally { this.gate.Release(); }
        }

        ///<inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            await this.gate.WaitAsync();

            try
            {
                if(this.released) { return; }

                this.released = true;

                try
                {
                    if(this.sessionKey is Int64 key) { await this.ReleaseLockAsync(key); }

                    await this.ReleaseLockAsync(this.itemKey);
                }
                catch
                {
                    this.dataSource.Clear();

                    throw;
                }
                finally { await this.connection.DisposeAsync(); }
            }
            finally { this.gate.Release(); }
        }

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresItemLockLease"]/method[@name="ReleaseLockAsync"]/*'/>*/
        private async Task ReleaseLockAsync(Int64 key)
        {
            await using NpgsqlCommand command = new("SELECT pg_advisory_unlock(@key)",this.connection);

            command.Parameters.AddWithValue("key",key);

            if(await command.ExecuteScalarAsync(CancellationToken.None) is not true)
            {
                throw new InvalidOperationException("The PostgreSQL advisory lock was not held by its connection.");
            }
        }
    }

    /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresLock"]/main/*'/>*/
    private sealed class PostgresLock : IAsyncDisposable
    {
        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresLock"]/field[@name="connection"]/*'/>*/
        private readonly NpgsqlConnection connection;

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresLock"]/field[@name="key"]/*'/>*/
        private readonly Int64 key;

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresLock"]/field[@name="released"]/*'/>*/
        private Int32 released;

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresLock"]/constructor[@name="PostgresLock"]/*'/>*/
        public PostgresLock(NpgsqlConnection connection , Int64 key)
        {
            this.connection = connection; this.key = key;
        }

        /**<include file='PostgresDataItemTransferCoordinator.xml' path='PostgresDataItemTransferCoordinator/class[@name="PostgresLock"]/method[@name="DisposeAsync"]/*'/>*/
        public async ValueTask DisposeAsync()
        {
            if(Interlocked.Exchange(ref this.released,1) != 0) { return; }

            try
            {
                await using NpgsqlCommand command = new("SELECT pg_advisory_unlock(@key)",this.connection);

                command.Parameters.AddWithValue("key",this.key);

                await command.ExecuteNonQueryAsync(CancellationToken.None);
            }
            finally
            {
                await this.connection.DisposeAsync();
            }
        }
    }
}