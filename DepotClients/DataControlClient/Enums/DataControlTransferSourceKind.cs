namespace KusDepot.Data.Clients;

/**<include file='DataControlTransferSourceKind.xml' path='DataControlTransferSourceKind/enum[@name="DataControlTransferSourceKind"]/main/*'/>*/
public enum DataControlTransferSourceKind
{
    /**<include file='DataControlTransferSourceKind.xml' path='DataControlTransferSourceKind/enum[@name="DataControlTransferSourceKind"]/value[@name="None"]/*'/>*/
    None = 0,

    /**<include file='DataControlTransferSourceKind.xml' path='DataControlTransferSourceKind/enum[@name="DataControlTransferSourceKind"]/value[@name="ExternalFile"]/*'/>*/
    ExternalFile = 1,

    /**<include file='DataControlTransferSourceKind.xml' path='DataControlTransferSourceKind/enum[@name="DataControlTransferSourceKind"]/value[@name="WorkingDirectoryCopy"]/*'/>*/
    WorkingDirectoryCopy = 2
}
