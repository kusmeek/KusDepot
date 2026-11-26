namespace KusDepot.Data.Transfer;

/**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/main/*'/>*/
public enum DataItemTransferMode
{
    /**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/value[@name="Upload"]/*'/>*/
    Upload = 0,

    /**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/value[@name="PublishedRead"]/*'/>*/
    PublishedRead = 1,

    /**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/value[@name="StagedRead"]/*'/>*/
    StagedRead = 2,

    /**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/value[@name="StreamUpload"]/*'/>*/
    StreamUpload = 3,

    /**<include file='DataItemTransferMode.xml' path='DataItemTransferMode/enum[@name="DataItemTransferMode"]/value[@name="StreamFollow"]/*'/>*/
    StreamFollow = 4
}