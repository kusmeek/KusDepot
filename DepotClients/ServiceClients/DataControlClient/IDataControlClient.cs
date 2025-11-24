namespace KusDepot.Data.Clients;

/**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/main/*'/>*/
public interface IDataControlClient : IDataControlClientBase , IDisposable
{
    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="Delete"]/*'/>*/
    Task<RestResponse<Guid?>> Delete(Guid? id , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="Get"]/*'/>*/
    Task<RestResponse<DataControlDownload?>> Get(Guid? id , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="GetStream"]/*'/>*/
    Task<DownloadInfo?> GetStream(Guid? id , String? filepath = null , String? itempath = null , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="Store"]/*'/>*/
    Task<RestResponse<Guid?>> Store(DataControlUpload dcu , String? token = null , CancellationToken cancel = default);

    /**<include file='IDataControlClient.xml' path='IDataControlClient/interface[@name="IDataControlClient"]/method[@name="StoreStream"]/*'/>*/
    Task<HttpResponseMessage?>? StoreStream(DataItem? it , String? token = null , CancellationToken cancel = default);
}