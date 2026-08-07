namespace KusDepot.Data.Transfer;

/**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/main/*'/>*/
public enum DataItemTransferMode
{
    /**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/value[@name="Upload"]/*'/>*/
    Upload = 0,

    /**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/value[@name="ReadCommitted"]/*'/>*/
    ReadCommitted = 1,

    /**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/value[@name="ReadStaged"]/*'/>*/
    ReadStaged = 2,

    /**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/value[@name="StreamUpload"]/*'/>*/
    StreamUpload = 3,

    /**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/value[@name="StreamFollow"]/*'/>*/
    StreamFollow = 4
}