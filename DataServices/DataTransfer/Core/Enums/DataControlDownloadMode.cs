namespace KusDepot.Data.Transfer;

/**<include file='DataControlDownloadMode.xml' path='DataControlDownloadMode/enum[@name="DataControlDownloadMode"]/main/*'/>*/
public enum DataControlDownloadMode
{
    /**<include file='DataControlDownloadMode.xml' path='DataControlDownloadMode/enum[@name="DataControlDownloadMode"]/value[@name="Committed"]/*'/>*/
    Committed = 0,

    /**<include file='DataControlDownloadMode.xml' path='DataControlDownloadMode/enum[@name="DataControlDownloadMode"]/value[@name="StagedFollow"]/*'/>*/
    StagedFollow = 1,

    /**<include file='DataControlDownloadMode.xml' path='DataControlDownloadMode/enum[@name="DataControlDownloadMode"]/value[@name="StreamFollow"]/*'/>*/
    StreamFollow = 2,
}