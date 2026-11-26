namespace DataPodServices.DataControl;

/**<include file='DeletionAgentOptions.xml' path='DeletionAgentOptions/class[@name="DeletionAgentOptions"]/main/*'/>*/
internal sealed class DeletionAgentOptions
{
    /**<include file='DeletionAgentOptions.xml' path='DeletionAgentOptions/class[@name="DeletionAgentOptions"]/field[@name="LeaderLockName"]/*'/>*/
    public const String LeaderLockName = "DeletionAgentLeaderLock";

    /**<include file='DeletionAgentOptions.xml' path='DeletionAgentOptions/class[@name="DeletionAgentOptions"]/property[@name="PollingInterval"]/*'/>*/
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMinutes(10);

    /**<include file='DeletionAgentOptions.xml' path='DeletionAgentOptions/class[@name="DeletionAgentOptions"]/property[@name="LeaderRetryInterval"]/*'/>*/
    public TimeSpan LeaderRetryInterval { get; set; } = TimeSpan.FromSeconds(30);

    /**<include file='DeletionAgentOptions.xml' path='DeletionAgentOptions/class[@name="DeletionAgentOptions"]/property[@name="InitialJitterMaximum"]/*'/>*/
    public TimeSpan InitialJitterMaximum { get; set; } = TimeSpan.FromMinutes(2);

    /**<include file='DeletionAgentOptions.xml' path='DeletionAgentOptions/class[@name="DeletionAgentOptions"]/property[@name="CycleJitterMaximum"]/*'/>*/
    public TimeSpan CycleJitterMaximum { get; set; } = TimeSpan.FromMinutes(1);

    /**<include file='DeletionAgentOptions.xml' path='DeletionAgentOptions/class[@name="DeletionAgentOptions"]/property[@name="RetryBaseDelay"]/*'/>*/
    public TimeSpan RetryBaseDelay { get; set; } = TimeSpan.FromSeconds(5);

    /**<include file='DeletionAgentOptions.xml' path='DeletionAgentOptions/class[@name="DeletionAgentOptions"]/property[@name="MaximumRetryDelay"]/*'/>*/
    public TimeSpan MaximumRetryDelay { get; set; } = TimeSpan.FromMinutes(15);

    /**<include file='DeletionAgentOptions.xml' path='DeletionAgentOptions/class[@name="DeletionAgentOptions"]/property[@name="CandidatePageSize"]/*'/>*/
    public Int32 CandidatePageSize { get; set; } = 50;
}
