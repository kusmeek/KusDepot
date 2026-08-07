namespace DataPodServices.DataControl;

internal enum SegmentedTransferHttpFailureCategory
{
    InternalError,
    BadRequest,
    Conflict,
    NotFound,
    PreconditionFailed,
    Unprocessable,
}

internal static class SegmentedTransferHttpFailureMap
{
    public static SegmentedTransferHttpFailureCategory Map(OperationFailedException failure)
    {
        if(failure?.FailureCode is not SegmentedTransferFailureCode code) { return SegmentedTransferHttpFailureCategory.InternalError; }

        return code switch
        {
            SegmentedTransferFailureCode.InvalidArgument  => SegmentedTransferHttpFailureCategory.BadRequest,
            SegmentedTransferFailureCode.IdentityMismatch => SegmentedTransferHttpFailureCategory.BadRequest,

            SegmentedTransferFailureCode.ConflictOpenSession => SegmentedTransferHttpFailureCategory.Conflict,
            SegmentedTransferFailureCode.InvalidSessionState => SegmentedTransferHttpFailureCategory.Conflict,
            SegmentedTransferFailureCode.NotFullyRealized    => SegmentedTransferHttpFailureCategory.Conflict,
            SegmentedTransferFailureCode.RangeUnavailable    => SegmentedTransferHttpFailureCategory.Conflict,

            SegmentedTransferFailureCode.SessionNotFound => SegmentedTransferHttpFailureCategory.NotFound,

            SegmentedTransferFailureCode.InvalidStateVersion => SegmentedTransferHttpFailureCategory.PreconditionFailed,

            SegmentedTransferFailureCode.InvalidObjectHash        => SegmentedTransferHttpFailureCategory.Unprocessable,
            SegmentedTransferFailureCode.InvalidPublishedSnapshot => SegmentedTransferHttpFailureCategory.Unprocessable,
            SegmentedTransferFailureCode.InvalidSegmentHash       => SegmentedTransferHttpFailureCategory.Unprocessable,
            SegmentedTransferFailureCode.InvalidSegmentLength     => SegmentedTransferHttpFailureCategory.Unprocessable,
            SegmentedTransferFailureCode.StreamHashMismatch       => SegmentedTransferHttpFailureCategory.Unprocessable,

            SegmentedTransferFailureCode.LoadSessionFailed            => SegmentedTransferHttpFailureCategory.InternalError,
            SegmentedTransferFailureCode.OpenFailed                   => SegmentedTransferHttpFailureCategory.InternalError,
            SegmentedTransferFailureCode.PublishedSnapshotUnavailable => SegmentedTransferHttpFailureCategory.InternalError,
            SegmentedTransferFailureCode.ReOpenFailed                 => SegmentedTransferHttpFailureCategory.InternalError,
            SegmentedTransferFailureCode.RemoveFailed                 => SegmentedTransferHttpFailureCategory.InternalError,
            SegmentedTransferFailureCode.SegmentUploadFailed          => SegmentedTransferHttpFailureCategory.InternalError,
            SegmentedTransferFailureCode.TransferReadFailed           => SegmentedTransferHttpFailureCategory.InternalError,
            SegmentedTransferFailureCode.CommitFailed                 => SegmentedTransferHttpFailureCategory.InternalError,
            SegmentedTransferFailureCode.AbortFailed                  => SegmentedTransferHttpFailureCategory.InternalError,

            _                                                         => SegmentedTransferHttpFailureCategory.InternalError,
        };
    }
}
