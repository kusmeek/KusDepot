namespace DataPodServices.DataControl;

/**<include file='TransferSessionCleanupAgentOptions.xml' path='TransferSessionCleanupAgentOptions/class[@name="TransferSessionCleanupAgentOptions"]/main/*'/>*/
internal sealed class TransferSessionCleanupAgentOptions
{
    /**<include file='TransferSessionCleanupAgentOptions.xml' path='TransferSessionCleanupAgentOptions/class[@name="TransferSessionCleanupAgentOptions"]/field[@name="LeaderLockName"]/*'/>*/
    public const String LeaderLockName = "TransferSessionCleanupLeaderLock";

    /**<include file='TransferSessionCleanupAgentOptions.xml' path='TransferSessionCleanupAgentOptions/class[@name="TransferSessionCleanupAgentOptions"]/property[@name="InactivityTimeout"]/*'/>*/
    public TimeSpan InactivityTimeout { get; set; } = TimeSpan.FromHours(24);

    /**<include file='TransferSessionCleanupAgentOptions.xml' path='TransferSessionCleanupAgentOptions/class[@name="TransferSessionCleanupAgentOptions"]/property[@name="PollingInterval"]/*'/>*/
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMinutes(15);

    /**<include file='TransferSessionCleanupAgentOptions.xml' path='TransferSessionCleanupAgentOptions/class[@name="TransferSessionCleanupAgentOptions"]/property[@name="LeaderRetryInterval"]/*'/>*/
    public TimeSpan LeaderRetryInterval { get; set; } = TimeSpan.FromSeconds(30);

    /**<include file='TransferSessionCleanupAgentOptions.xml' path='TransferSessionCleanupAgentOptions/class[@name="TransferSessionCleanupAgentOptions"]/property[@name="InitialJitterMaximum"]/*'/>*/
    public TimeSpan InitialJitterMaximum { get; set; } = TimeSpan.FromMinutes(2);

    /**<include file='TransferSessionCleanupAgentOptions.xml' path='TransferSessionCleanupAgentOptions/class[@name="TransferSessionCleanupAgentOptions"]/property[@name="CandidatePageSize"]/*'/>*/
    public Int32 CandidatePageSize { get; set; } = 50;
}
