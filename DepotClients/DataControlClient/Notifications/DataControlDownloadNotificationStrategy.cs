namespace KusDepot.Data.Clients;

/**<include file='DataControlDownloadNotificationStrategy.xml' path='DataControlDownloadNotificationStrategy/class[@name="DataControlDownloadNotificationStrategy"]/main/*'/>*/
internal static class DataControlDownloadNotificationStrategy
{
    /**<include file='DataControlDownloadNotificationStrategy.xml' path='DataControlDownloadNotificationStrategy/class[@name="DataControlDownloadNotificationStrategy"]/method[@name="Create"]/*'/>*/
    internal static DataControlNotificationSubscription? Create(DataControlTransferSessionInfo session)
    {
        if(session.TransferFamily is DataControlTransferFamily.Stream)
        {
            DataStreamTransferState? state = session.StreamState;

            if(state is null || state.SourceSessionID.HasValue is false || state.SourceSessionID.Value == Guid.Empty) { return null; }

            return new()
            {
                ItemID = state.ItemID,
                SourceSessionID = state.SourceSessionID,
                TransferFamily = DataControlTransferFamily.Stream,
            };
        }

        DataItemTransferState segmentedState = session.State;

        if(segmentedState.Mode is not DataItemTransferMode.ReadStaged || segmentedState.SourceSessionID.HasValue is false || segmentedState.SourceSessionID.Value == Guid.Empty)
        {
            return null;
        }

        return new()
        {
            ItemID = segmentedState.ItemID,
            SourceSessionID = segmentedState.SourceSessionID,
            TransferFamily = DataControlTransferFamily.Segmented,
        };
    }
}
