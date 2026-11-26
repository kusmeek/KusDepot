namespace DataPodServices.DataControl;

/**<include file='PublicationRecoveryResult.xml' path='PublicationRecoveryResult/record[@name="PublicationRecoveryResult"]/main/*'/>*/
internal sealed record PublicationRecoveryResult
{
    /**<include file='PublicationRecoveryResult.xml' path='PublicationRecoveryResult/record[@name="PublicationRecoveryResult"]/property[@name="Succeeded"]/*'/>*/
    public Boolean Succeeded { get; init; }

    /**<include file='PublicationRecoveryResult.xml' path='PublicationRecoveryResult/record[@name="PublicationRecoveryResult"]/property[@name="FailureReason"]/*'/>*/
    public String? FailureReason { get; init; }

    /**<include file='PublicationRecoveryResult.xml' path='PublicationRecoveryResult/record[@name="PublicationRecoveryResult"]/method[@name="Success"]/*'/>*/
    public static PublicationRecoveryResult Success() => new() { Succeeded = true };

    /**<include file='PublicationRecoveryResult.xml' path='PublicationRecoveryResult/record[@name="PublicationRecoveryResult"]/method[@name="Failure"]/*'/>*/
    public static PublicationRecoveryResult Failure(String reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        return new() { FailureReason = reason };
    }
}
