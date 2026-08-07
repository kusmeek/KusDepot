namespace KusDepot.Data.Transfer;

/**<include file='DataItemTransferStatus.xml' path='DataItemTransferStatus/enum[@name="DataItemTransferStatus"]/main/*'/>*/
public enum DataItemTransferStatus
{
    /**<include file='DataItemTransferStatus.xml' path='DataItemTransferStatus/enum[@name="DataItemTransferStatus"]/value[@name="Open"]/*'/>*/
    Open = 0,

    /**<include file='DataItemTransferStatus.xml' path='DataItemTransferStatus/enum[@name="DataItemTransferStatus"]/value[@name="Complete"]/*'/>*/
    Complete = 1,

    /**<include file='DataItemTransferStatus.xml' path='DataItemTransferStatus/enum[@name="DataItemTransferStatus"]/value[@name="Committing"]/*'/>*/
    Committing = 2,

    /**<include file='DataItemTransferStatus.xml' path='DataItemTransferStatus/enum[@name="DataItemTransferStatus"]/value[@name="Committed"]/*'/>*/
    Committed = 3,

    /**<include file='DataItemTransferStatus.xml' path='DataItemTransferStatus/enum[@name="DataItemTransferStatus"]/value[@name="Aborted"]/*'/>*/
    Aborted = 4,

    /**<include file='DataItemTransferStatus.xml' path='DataItemTransferStatus/enum[@name="DataItemTransferStatus"]/value[@name="Faulted"]/*'/>*/
    Faulted = 5
}