namespace DataPodServices.DataControl;

#pragma warning disable CS1591

public enum StreamTransferFailureCode
{
    AbortFailed,
    AppendFailed,
    AppendOffsetMismatch,
    PublishFailed,
    ConflictOpenSession,
    IdentityMismatch,
    InvalidArgument,
    InvalidObjectHash,
    InvalidSegmentHash,
    InvalidSegmentLength,
    InvalidSessionState,
    InvalidStateVersion,
    JournalMismatch,
    LoadSessionFailed,
    OpenFailed,
    RangeUnavailable,
    ReOpenFailed,
    RemoveFailed,
    SessionNotFound,
    SourceSessionNotFound,
    StreamHashMismatch,
    TransferReadFailed,
}
