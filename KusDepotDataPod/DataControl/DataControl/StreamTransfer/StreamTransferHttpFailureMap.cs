namespace DataPodServices.DataControl;

internal enum StreamTransferHttpFailureCategory
{
    InternalError,
    BadRequest,
    Conflict,
    NotFound,
    PreconditionFailed,
    Unprocessable,
}

internal static class StreamTransferHttpFailureMap
{
    public static StreamTransferHttpFailureCategory Map(OperationFailedException failure)
    {
        if(failure?.FailureCode is not StreamTransferFailureCode code) { return StreamTransferHttpFailureCategory.InternalError; }

        return code switch
        {
            StreamTransferFailureCode.InvalidArgument      => StreamTransferHttpFailureCategory.BadRequest,
            StreamTransferFailureCode.IdentityMismatch     => StreamTransferHttpFailureCategory.BadRequest,

            StreamTransferFailureCode.AppendOffsetMismatch => StreamTransferHttpFailureCategory.Conflict,
            StreamTransferFailureCode.ConflictOpenSession  => StreamTransferHttpFailureCategory.Conflict,
            StreamTransferFailureCode.InvalidSessionState  => StreamTransferHttpFailureCategory.Conflict,
            StreamTransferFailureCode.RangeUnavailable     => StreamTransferHttpFailureCategory.Conflict,

            StreamTransferFailureCode.SessionNotFound      => StreamTransferHttpFailureCategory.NotFound,
            StreamTransferFailureCode.SourceSessionNotFound => StreamTransferHttpFailureCategory.NotFound,

            StreamTransferFailureCode.InvalidStateVersion  => StreamTransferHttpFailureCategory.PreconditionFailed,

            StreamTransferFailureCode.InvalidObjectHash    => StreamTransferHttpFailureCategory.Unprocessable,
            StreamTransferFailureCode.InvalidSegmentHash   => StreamTransferHttpFailureCategory.Unprocessable,
            StreamTransferFailureCode.InvalidSegmentLength => StreamTransferHttpFailureCategory.Unprocessable,
            StreamTransferFailureCode.StreamHashMismatch   => StreamTransferHttpFailureCategory.Unprocessable,

            StreamTransferFailureCode.AbortFailed          => StreamTransferHttpFailureCategory.InternalError,
            StreamTransferFailureCode.AppendFailed         => StreamTransferHttpFailureCategory.InternalError,
            StreamTransferFailureCode.CompleteFailed       => StreamTransferHttpFailureCategory.InternalError,
            StreamTransferFailureCode.LoadSessionFailed    => StreamTransferHttpFailureCategory.InternalError,
            StreamTransferFailureCode.OpenFailed           => StreamTransferHttpFailureCategory.InternalError,
            StreamTransferFailureCode.ReOpenFailed         => StreamTransferHttpFailureCategory.InternalError,
            StreamTransferFailureCode.RemoveFailed         => StreamTransferHttpFailureCategory.InternalError,
            StreamTransferFailureCode.TransferReadFailed   => StreamTransferHttpFailureCategory.InternalError,

            _                                              => StreamTransferHttpFailureCategory.InternalError,
        };
    }
}
