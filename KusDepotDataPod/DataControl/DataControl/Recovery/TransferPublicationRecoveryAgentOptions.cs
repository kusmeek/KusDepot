namespace DataPodServices.DataControl;

/**<include file='TransferPublicationRecoveryAgentOptions.xml' path='TransferPublicationRecoveryAgentOptions/class[@name="TransferPublicationRecoveryAgentOptions"]/main/*'/>*/
internal sealed class TransferPublicationRecoveryAgentOptions
{
    /**<include file='TransferPublicationRecoveryAgentOptions.xml' path='TransferPublicationRecoveryAgentOptions/class[@name="TransferPublicationRecoveryAgentOptions"]/field[@name="LeaderLockName"]/*'/>*/
    public const String LeaderLockName = "TransferPublicationRecoveryLeaderLock";

    /**<include file='TransferPublicationRecoveryAgentOptions.xml' path='TransferPublicationRecoveryAgentOptions/class[@name="TransferPublicationRecoveryAgentOptions"]/property[@name="PollingInterval"]/*'/>*/
    public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMinutes(10);

    /**<include file='TransferPublicationRecoveryAgentOptions.xml' path='TransferPublicationRecoveryAgentOptions/class[@name="TransferPublicationRecoveryAgentOptions"]/property[@name="LeaderRetryInterval"]/*'/>*/
    public TimeSpan LeaderRetryInterval { get; set; } = TimeSpan.FromSeconds(30);

    /**<include file='TransferPublicationRecoveryAgentOptions.xml' path='TransferPublicationRecoveryAgentOptions/class[@name="TransferPublicationRecoveryAgentOptions"]/property[@name="InitialJitterMaximum"]/*'/>*/
    public TimeSpan InitialJitterMaximum { get; set; } = TimeSpan.FromMinutes(2);

    /**<include file='TransferPublicationRecoveryAgentOptions.xml' path='TransferPublicationRecoveryAgentOptions/class[@name="TransferPublicationRecoveryAgentOptions"]/property[@name="CycleJitterMaximum"]/*'/>*/
    public TimeSpan CycleJitterMaximum { get; set; } = TimeSpan.FromMinutes(1);

    /**<include file='TransferPublicationRecoveryAgentOptions.xml' path='TransferPublicationRecoveryAgentOptions/class[@name="TransferPublicationRecoveryAgentOptions"]/property[@name="CandidatePageSize"]/*'/>*/
    public Int32 CandidatePageSize { get; set; } = 50;

    /**<include file='TransferPublicationRecoveryAgentOptions.xml' path='TransferPublicationRecoveryAgentOptions/class[@name="TransferPublicationRecoveryAgentOptions"]/property[@name="TerminalFailureThreshold"]/*'/>*/
    public Int32 TerminalFailureThreshold { get; set; } = 9;
}
