namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private void MapStreamTransfer(WebApplication application)
    {
        MapCompleteStreamTransfer(application);
        MapGetStreamTransferSegment(application);
        MapGetStreamTransferState(application);
        MapOpenFollowStreamTransfer(application);
        MapOpenStreamTransfer(application);
        MapPutStreamTransferSegment(application);
        MapReOpenFollowStreamTransfer(application);
        MapReOpenStreamTransfer(application);
    }

    private static IResult MapStreamTransferFailure(OperationFailedException failure)
    {
        return StreamTransferHttpFailureMap.Map(failure) switch
        {
            StreamTransferHttpFailureCategory.BadRequest         => BadRequest(),
            StreamTransferHttpFailureCategory.Conflict           => Conflict(),
            StreamTransferHttpFailureCategory.NotFound           => NotFound(),
            StreamTransferHttpFailureCategory.PreconditionFailed => PreconditionFailed(),
            StreamTransferHttpFailureCategory.Unprocessable      => Unprocessable(),
            _ => InternalError(),
        };
    }
}
