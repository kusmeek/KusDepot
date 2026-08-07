namespace KusDepot.Data.Services.DataTransfer;

/**<include file='ISegmentedTransferStorage.xml' path='ISegmentedTransferStorage/interface[@name="ISegmentedTransferStorage"]/main/*'/>*/
public interface ISegmentedTransferStorage
{
    /**<include file='ISegmentedTransferStorage.xml' path='ISegmentedTransferStorage/interface[@name="ISegmentedTransferStorage"]/method[@name="CreateSession"]/*'/>*/
    Task<SegmentedTransferStorageSessionPaths?> CreateSession(DataItemTransferManifest manifest , Byte[] objectpayload , CancellationToken cancel = default);

    /**<include file='ISegmentedTransferStorage.xml' path='ISegmentedTransferStorage/interface[@name="ISegmentedTransferStorage"]/method[@name="DeleteSession"]/*'/>*/
    Task<Boolean> DeleteSession(Guid sessionid , CancellationToken cancel = default);

    /**<include file='ISegmentedTransferStorage.xml' path='ISegmentedTransferStorage/interface[@name="ISegmentedTransferStorage"]/method[@name="GetSessionPaths"]/*'/>*/
    SegmentedTransferStorageSessionPaths GetSessionPaths(Guid sessionid);

    /**<include file='ISegmentedTransferStorage.xml' path='ISegmentedTransferStorage/interface[@name="ISegmentedTransferStorage"]/method[@name="LoadManifest"]/*'/>*/
    Task<DataItemTransferManifest?> LoadManifest(Guid sessionid , CancellationToken cancel = default);

    /**<include file='ISegmentedTransferStorage.xml' path='ISegmentedTransferStorage/interface[@name="ISegmentedTransferStorage"]/method[@name="ReadObjectPayload"]/*'/>*/
    Task<Byte[]?> ReadObjectPayload(Guid sessionid , CancellationToken cancel = default);

    /**<include file='ISegmentedTransferStorage.xml' path='ISegmentedTransferStorage/interface[@name="ISegmentedTransferStorage"]/method[@name="ReadRange"]/*'/>*/
    Task<Stream?> ReadRange(Guid sessionid , DataItemTransferRange range , CancellationToken cancel = default);

    /**<include file='ISegmentedTransferStorage.xml' path='ISegmentedTransferStorage/interface[@name="ISegmentedTransferStorage"]/method[@name="SaveManifest"]/*'/>*/
    Task<Boolean> SaveManifest(DataItemTransferManifest manifest , CancellationToken cancel = default);

    /**<include file='ISegmentedTransferStorage.xml' path='ISegmentedTransferStorage/interface[@name="ISegmentedTransferStorage"]/method[@name="WriteRange"]/*'/>*/
    Task<SegmentedTransferWriteResult> WriteRange(Guid sessionid , DataItemTransferRange range , Stream payload , CancellationToken cancel = default);
}
