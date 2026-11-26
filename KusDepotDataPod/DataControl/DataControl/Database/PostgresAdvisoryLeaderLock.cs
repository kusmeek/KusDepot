namespace DataPodServices.DataControl;

/**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/class[@name="PostgresAdvisoryLeaderLock"]/main/*'/>*/
internal sealed class PostgresAdvisoryLeaderLock : IPostgresAdvisoryLeaderLock
{
    /**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/class[@name="PostgresAdvisoryLeaderLock"]/field[@name="dataSource"]/*'/>*/
    private readonly NpgsqlDataSource dataSource;

    /**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/class[@name="PostgresAdvisoryLeaderLock"]/constructor[@name="PostgresAdvisoryLeaderLock"]/*'/>*/
    public PostgresAdvisoryLeaderLock(NpgsqlDataSource dataSource)
    {
        this.dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
    }

    /**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/class[@name="PostgresAdvisoryLeaderLock"]/method[@name="TryAcquireAsync"]/*'/>*/
    public async Task<IPostgresAdvisoryLeaderLease?> TryAcquireAsync(String lockName , CancellationToken cancel = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(lockName);

        NpgsqlConnection connection = await this.dataSource.OpenConnectionAsync(cancel);

        Int64 lockKey = BinaryPrimitives.ReadInt64LittleEndian(SHA256.HashData(Encoding.UTF8.GetBytes(lockName)));

        try
        {
            await using NpgsqlCommand command = new("SELECT pg_try_advisory_lock(@lockkey)",connection);

            command.Parameters.AddWithValue("lockkey",lockKey);

            Boolean acquired = (Boolean)(await command.ExecuteScalarAsync(cancel) ?? false);

            if(acquired is false)
            {
                await connection.DisposeAsync();

                return null;
            }

            return new AdvisoryLockLease(connection,lockKey,lockName);
        }
        catch
        {
            NpgsqlConnection.ClearPool(connection);
            await connection.DisposeAsync();
            throw;
        }
    }

    /**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/class[@name="AdvisoryLockLease"]/main/*'/>*/
    private sealed class AdvisoryLockLease(NpgsqlConnection connection , Int64 lockKey , String lockName) : IPostgresAdvisoryLeaderLease
    {
        /**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/class[@name="AdvisoryLockLease"]/field[@name="connection"]/*'/>*/
        private NpgsqlConnection? connection = connection;

        /**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/class[@name="AdvisoryLockLease"]/method[@name="VerifyAsync"]/*'/>*/
        public async Task VerifyAsync(CancellationToken cancel = default)
        {
            NpgsqlConnection activeConnection = this.connection ?? throw new ObjectDisposedException(nameof(AdvisoryLockLease));

            try
            {
                await using NpgsqlCommand command = new("SELECT 1",activeConnection);

                await command.ExecuteScalarAsync(cancel);
            }
            catch ( OperationCanceledException )
            {
                NpgsqlConnection.ClearPool(activeConnection);

                throw;
            }
            catch ( Exception error )
            {
                NpgsqlConnection.ClearPool(activeConnection);

                throw new PostgresAdvisoryLeaderLeaseLostException(lockName,error);
            }
        }

        /**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/class[@name="AdvisoryLockLease"]/method[@name="DisposeAsync"]/*'/>*/
        public async ValueTask DisposeAsync()
        {
            NpgsqlConnection? activeConnection = Interlocked.Exchange(ref this.connection,null);

            if(activeConnection is null) { return; }

            try
            {
                await using NpgsqlCommand command = new("SELECT pg_advisory_unlock(@lockkey)",activeConnection);

                command.Parameters.AddWithValue("lockkey",lockKey);

                await command.ExecuteScalarAsync();
            }
            catch
            {
                NpgsqlConnection.ClearPool(activeConnection);
            }
            finally { await activeConnection.DisposeAsync(); }
        }
    }
}
