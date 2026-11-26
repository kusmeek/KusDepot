namespace DataPodServices.DataControl;

#pragma warning disable CS1591

public enum SegmentedTransferFailureCode
{
    AbortFailed,
    PublishFailed,
    ConflictOpenSession,
    IdentityMismatch,
    InvalidArgument,
    InvalidObjectHash,
    InvalidPublishedSnapshot,
    InvalidSegmentHash,
    InvalidSegmentLength,
    InvalidSessionState,
    InvalidStateVersion,
    JournalMismatch,
    LoadSessionFailed,
    NotFullyRealized,
    OpenFailed,
    PublishedSnapshotUnavailable,
    RangeUnavailable,
    ReOpenFailed,
    RemoveFailed,
    SegmentUploadFailed,
    SessionNotFound,
    StreamHashMismatch,
    TransferReadFailed,
}