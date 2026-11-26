namespace DataPodServices.DataControl;

/**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/interface[@name="IPostgresAdvisoryLeaderLock"]/main/*'/>*/
internal interface IPostgresAdvisoryLeaderLock
{
    /**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/interface[@name="IPostgresAdvisoryLeaderLock"]/method[@name="TryAcquireAsync"]/*'/>*/
    Task<IPostgresAdvisoryLeaderLease?> TryAcquireAsync(String lockName , CancellationToken cancel = default);
}

/**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/interface[@name="IPostgresAdvisoryLeaderLease"]/main/*'/>*/
internal interface IPostgresAdvisoryLeaderLease : IAsyncDisposable
{
    /**<include file='PostgresAdvisoryLeaderLock.xml' path='PostgresAdvisoryLeaderLock/interface[@name="IPostgresAdvisoryLeaderLease"]/method[@name="VerifyAsync"]/*'/>*/
    Task VerifyAsync(CancellationToken cancel = default);
}
