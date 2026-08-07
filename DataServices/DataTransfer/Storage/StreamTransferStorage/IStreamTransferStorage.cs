namespace KusDepot.Data.Services.DataTransfer;

/**<include file='IStreamTransferStorage.xml' path='IStreamTransferStorage/interface[@name="IStreamTransferStorage"]/main/*'/>*/
public interface IStreamTransferStorage
{
    /**<include file='IStreamTransferStorage.xml' path='IStreamTransferStorage/interface[@name="IStreamTransferStorage"]/method[@name="CreateSession"]/*'/>*/
    Task<StreamTransferStorageSessionPaths?> CreateSession(DataStreamTransferManifest manifest , CancellationToken cancel = default);

    /**<include file='IStreamTransferStorage.xml' path='IStreamTransferStorage/interface[@name="IStreamTransferStorage"]/method[@name="DeleteSession"]/*'/>*/
    Task<Boolean> DeleteSession(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IStreamTransferStorage.xml' path='IStreamTransferStorage/interface[@name="IStreamTransferStorage"]/method[@name="GetSessionPaths"]/*'/>*/
    StreamTransferStorageSessionPaths GetSessionPaths(Guid sessionid);

    /**<include file='IStreamTransferStorage.xml' path='IStreamTransferStorage/interface[@name="IStreamTransferStorage"]/method[@name="LoadManifest"]/*'/>*/
    Task<DataStreamTransferManifest?> LoadManifest(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IStreamTransferStorage.xml' path='IStreamTransferStorage/interface[@name="IStreamTransferStorage"]/method[@name="ReadObjectPayload"]/*'/>*/
    Task<Stream?> ReadObjectPayload(Guid sessionid , CancellationToken cancel = default);

    /**<include file='IStreamTransferStorage.xml' path='IStreamTransferStorage/interface[@name="IStreamTransferStorage"]/method[@name="ReadRange"]/*'/>*/
    Task<Stream?> ReadRange(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default);

    /**<include file='IStreamTransferStorage.xml' path='IStreamTransferStorage/interface[@name="IStreamTransferStorage"]/method[@name="SaveManifest"]/*'/>*/
    Task<Boolean> SaveManifest(DataStreamTransferManifest manifest , CancellationToken cancel = default);

    /**<include file='IStreamTransferStorage.xml' path='IStreamTransferStorage/interface[@name="IStreamTransferStorage"]/method[@name="SaveObjectPayload"]/*'/>*/
    Task<Boolean> SaveObjectPayload(Guid sessionid , Byte[] objectpayload , CancellationToken cancel = default);

    /**<include file='IStreamTransferStorage.xml' path='IStreamTransferStorage/interface[@name="IStreamTransferStorage"]/method[@name="WriteTail"]/*'/>*/
    Task<StreamTransferAppendResult> WriteTail(Guid sessionid , DataItemTransferRange range , Stream payload , CancellationToken cancel = default);
}
