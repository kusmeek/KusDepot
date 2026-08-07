namespace KusDepot.Data.Services.DataTransfer;

#pragma warning disable CS1591

public enum SegmentedTransferFailureCode
{
    AbortFailed,
    CommitFailed,
    ConflictOpenSession,
    IdentityMismatch,
    InvalidArgument,
    InvalidObjectHash,
    InvalidPublishedSnapshot,
    InvalidSegmentHash,
    InvalidSegmentLength,
    InvalidSessionState,
    InvalidStateVersion,
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