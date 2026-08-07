namespace DataPodServices.DataControl;

public sealed partial class DataControl
{
    private static async Task PublishNotificationAsync(IDataControlNotificationPublisher publisher , DataControlNotification notification , CancellationToken cancel)
    {
        try
        {
            await publisher.PublishAsync(notification,cancel).ConfigureAwait(false);
        }
        catch ( Exception exception )
        {
            Log.Warning(exception,"DataControl notification publish failed {EventType} {ItemID} {SessionID}",notification.EventType,notification.ItemID,notification.SessionID);
        }
    }

    private static DataControlNotification CreateSegmentRealizedNotification(PutTransferSegmentRequest request , PutTransferSegmentResponse response)
    {
        return new()
        {
            EventType = DataControlNotificationEventType.DataRealized,
            ItemID = response.ItemID,
            LatestRange = response.Range,
            Mode = DataItemTransferMode.Upload,
            Sequence = response.StateVersion,
            SessionID = response.SessionID,
            SourceSessionID = response.SessionID,
            StateVersion = response.StateVersion,
            Status = response.CompletedCoverage ? DataItemTransferStatus.Complete : DataItemTransferStatus.Open,
            Timestamp = DateTimeOffset.UtcNow,
            TransferFamily = DataControlTransferFamily.Segmented,
        };
    }

    private static DataControlNotification CreateStreamAppendedNotification(PutStreamTransferSegmentRequest request , PutStreamTransferSegmentResponse response)
    {
        return new()
        {
            AppendedLength = response.AppendedLength,
            EventType = DataControlNotificationEventType.DataAppended,
            ItemID = response.ItemID,
            LatestRange = new DataItemTransferRange(){ Offset = request.Offset , Length = request.Length },
            Mode = DataItemTransferMode.StreamUpload,
            Sequence = response.StateVersion,
            SessionID = response.SessionID,
            SourceSessionID = response.SessionID,
            StateVersion = response.StateVersion,
            Status = DataItemTransferStatus.Open,
            Timestamp = DateTimeOffset.UtcNow,
            TransferFamily = DataControlTransferFamily.Stream,
        };
    }

    private static DataControlNotification CreateTerminalSegmentedNotification(DataControlNotificationEventType eventType , DataItemTransferState state)
    {
        return new()
        {
            EventType = eventType,
            ItemID = state.ItemID,
            Mode = state.Mode,
            RealizedBytes = state.RealizedBytes,
            Sequence = state.StateVersion,
            SessionID = state.SessionID,
            SourceSessionID = state.SourceSessionID ?? state.SessionID,
            StateVersion = state.StateVersion,
            Status = state.Status,
            Timestamp = DateTimeOffset.UtcNow,
            TransferFamily = DataControlTransferFamily.Segmented,
        };
    }

    private static DataControlNotification CreateTerminalStreamNotification(DataControlNotificationEventType eventType , DataStreamTransferState state)
    {
        return new()
        {
            AppendedLength = state.AppendedLength,
            EventType = eventType,
            ItemID = state.ItemID,
            Mode = state.Mode,
            Sequence = state.StateVersion,
            SessionID = state.SessionID,
            SourceSessionID = state.SourceSessionID ?? state.SessionID,
            StateVersion = state.StateVersion,
            Status = state.Status,
            Timestamp = DateTimeOffset.UtcNow,
            TransferFamily = DataControlTransferFamily.Stream,
        };
    }

    private static DataControlNotification CreateTransferAbortedNotification(AbortTransferResponse response)
    {
        return new()
        {
            EventType = DataControlNotificationEventType.TransferAborted,
            ItemID = response.ItemID,
            Sequence = 0,
            SessionID = response.SessionID,
            SourceSessionID = response.SessionID,
            Status = DataItemTransferStatus.Aborted,
            Timestamp = DateTimeOffset.UtcNow,
            TransferFamily = null,
        };
    }
}
