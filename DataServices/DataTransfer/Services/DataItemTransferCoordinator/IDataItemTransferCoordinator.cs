namespace KusDepot.Data.Services.DataTransfer;

/**<include file='IDataItemTransferCoordinator.xml' path='IDataItemTransferCoordinator/interface[@name="IDataItemTransferCoordinator"]/main/*'/>*/
public interface IDataItemTransferCoordinator
{
    /**<include file='IDataItemTransferCoordinator.xml' path='IDataItemTransferCoordinator/interface[@name="IDataItemTransferCoordinator"]/method[@name="AcquireItemLockAsync"]/*'/>*/
    Task<IAsyncDisposable> AcquireItemLockAsync(Guid itemid, CancellationToken cancel = default);

    /**<include file='IDataItemTransferCoordinator.xml' path='IDataItemTransferCoordinator/interface[@name="IDataItemTransferCoordinator"]/method[@name="AcquireSessionLockAsync"]/*'/>*/
    Task<IAsyncDisposable> AcquireSessionLockAsync(Guid sessionid, CancellationToken cancel = default);
}