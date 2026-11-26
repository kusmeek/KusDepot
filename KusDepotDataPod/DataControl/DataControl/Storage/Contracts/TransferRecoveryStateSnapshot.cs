namespace DataPodServices.DataControl;

/**<include file='TransferRecoveryStateSnapshot.xml' path='TransferRecoveryStateSnapshot/record[@name="TransferRecoveryStateSnapshot"]/main/*'/>*/
public sealed record TransferRecoveryStateSnapshot
{
    /**<include file='TransferRecoveryStateSnapshot.xml' path='TransferRecoveryStateSnapshot/record[@name="TransferRecoveryStateSnapshot"]/property[@name="Session"]/*'/>*/
    public TransferSessionState? Session { get; init; }

    /**<include file='TransferRecoveryStateSnapshot.xml' path='TransferRecoveryStateSnapshot/record[@name="TransferRecoveryStateSnapshot"]/property[@name="Journal"]/*'/>*/
    public TransferPublicationJournalState? Journal { get; init; }

    /**<include file='TransferRecoveryStateSnapshot.xml' path='TransferRecoveryStateSnapshot/record[@name="TransferRecoveryStateSnapshot"]/property[@name="SegmentedRanges"]/*'/>*/
    public IReadOnlyList<SegmentedTransferRangeState> SegmentedRanges { get; init; } = Array.Empty<SegmentedTransferRangeState>();

    /**<include file='TransferRecoveryStateSnapshot.xml' path='TransferRecoveryStateSnapshot/record[@name="TransferRecoveryStateSnapshot"]/property[@name="StreamChunks"]/*'/>*/
    public IReadOnlyList<StreamTransferChunkState> StreamChunks { get; init; } = Array.Empty<StreamTransferChunkState>();
}
