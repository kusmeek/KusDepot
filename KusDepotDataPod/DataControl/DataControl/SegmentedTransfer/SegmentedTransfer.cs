namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapSegmentedTransfer(WebApplication application)
    {
        MapAbortTransfer(application);
        MapCommitUploadTransfer(application);
        MapGetTransferSegment(application);
        MapGetTransferSessions(application);
        MapGetTransferState(application);
        MapOpenGetTransfer(application);
        MapOpenUploadTransfer(application);
        MapPutTransferSegment(application);
        MapRemoveTransfer(application);
        MapReOpenGetTransfer(application);
        MapReOpenUploadTransfer(application);
    }

    private static IResult MapSegmentedFailure(OperationFailedException failure)
    {
        return SegmentedTransferHttpFailureMap.Map(failure) switch
        {
            SegmentedTransferHttpFailureCategory.BadRequest         => BadRequest(),
            SegmentedTransferHttpFailureCategory.Conflict           => Conflict(),
            SegmentedTransferHttpFailureCategory.NotFound           => NotFound(),
            SegmentedTransferHttpFailureCategory.PreconditionFailed => PreconditionFailed(),
            SegmentedTransferHttpFailureCategory.Unprocessable      => Unprocessable(),
            _ => InternalError(),
        };
    }
}
