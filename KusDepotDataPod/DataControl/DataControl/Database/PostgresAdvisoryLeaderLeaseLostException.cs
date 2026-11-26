namespace DataPodServices.DataControl;

/**<include file='PostgresAdvisoryLeaderLeaseLostException.xml' path='PostgresAdvisoryLeaderLeaseLostException/class[@name="PostgresAdvisoryLeaderLeaseLostException"]/main/*'/>*/
internal sealed class PostgresAdvisoryLeaderLeaseLostException : InvalidOperationException
{
    /**<include file='PostgresAdvisoryLeaderLeaseLostException.xml' path='PostgresAdvisoryLeaderLeaseLostException/class[@name="PostgresAdvisoryLeaderLeaseLostException"]/constructor[@name="PostgresAdvisoryLeaderLeaseLostException"]/*'/>*/
    public PostgresAdvisoryLeaderLeaseLostException(String lockName , Exception innerException)
        : base($"PostgreSQL advisory leadership lease '{lockName}' is no longer available.",innerException)
    {
    }
}
