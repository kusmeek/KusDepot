namespace KusDepot.Data.Services.DataTransfer;

#pragma warning disable CS1591

public enum StreamTransferFailureCode
{
    AbortFailed,
    AppendFailed,
    AppendOffsetMismatch,
    CompleteFailed,
    ConflictOpenSession,
    IdentityMismatch,
    InvalidArgument,
    InvalidObjectHash,
    InvalidSegmentHash,
    InvalidSegmentLength,
    InvalidSessionState,
    InvalidStateVersion,
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
