namespace KusDepot.Data.Transfer;

/**<include file='DataControlNotificationEventType.xml' path='DataControlNotificationEventType/enum[@name="DataControlNotificationEventType"]/main/*'/>*/
public enum DataControlNotificationEventType
{
    /**<include file='DataControlNotificationEventType.xml' path='DataControlNotificationEventType/enum[@name="DataControlNotificationEventType"]/value[@name="DataAppended"]/*'/>*/
    DataAppended = 0,

    /**<include file='DataControlNotificationEventType.xml' path='DataControlNotificationEventType/enum[@name="DataControlNotificationEventType"]/value[@name="DataRealized"]/*'/>*/
    DataRealized = 1,

    /**<include file='DataControlNotificationEventType.xml' path='DataControlNotificationEventType/enum[@name="DataControlNotificationEventType"]/value[@name="TransferCompleted"]/*'/>*/
    TransferCompleted = 2,

    /**<include file='DataControlNotificationEventType.xml' path='DataControlNotificationEventType/enum[@name="DataControlNotificationEventType"]/value[@name="TransferCommitted"]/*'/>*/
    TransferCommitted = 3,

    /**<include file='DataControlNotificationEventType.xml' path='DataControlNotificationEventType/enum[@name="DataControlNotificationEventType"]/value[@name="TransferAborted"]/*'/>*/
    TransferAborted = 4,
}
