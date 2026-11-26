namespace DataPodServices.DataControl;

/**<include file='IDataItemTransferCoordinator.xml' path='IDataItemTransferCoordinator/interface[@name="IDataItemTransferCoordinator"]/main/*'/>*/
public interface IDataItemTransferCoordinator
{
    /**<include file='IDataItemTransferCoordinator.xml' path='IDataItemTransferCoordinator/interface[@name="IDataItemTransferCoordinator"]/method[@name="AcquireItemLockAsync"]/*'/>*/
    Task<IAsyncDisposable> AcquireItemLockAsync(Guid itemid, CancellationToken cancel = default);

    /**<include file='IDataItemTransferCoordinator.xml' path='IDataItemTransferCoordinator/interface[@name="IDataItemTransferCoordinator"]/method[@name="AcquireItemLockLeaseAsync"]/*'/>*/
    Task<ITransferItemLockLease> AcquireItemLockLeaseAsync(Guid itemid, CancellationToken cancel = default);

    /**<include file='IDataItemTransferCoordinator.xml' path='IDataItemTransferCoordinator/interface[@name="IDataItemTransferCoordinator"]/method[@name="TryAcquireItemLockAsync"]/*'/>*/
    Task<IAsyncDisposable?> TryAcquireItemLockAsync(Guid itemid, CancellationToken cancel = default);

    /**<include file='IDataItemTransferCoordinator.xml' path='IDataItemTransferCoordinator/interface[@name="IDataItemTransferCoordinator"]/method[@name="AcquireMutationScopeAsync"]/*'/>*/
    Task<ITransferMutationScope> AcquireMutationScopeAsync(Guid itemid, Guid sessionid, CancellationToken cancel = default);

    /**<include file='IDataItemTransferCoordinator.xml' path='IDataItemTransferCoordinator/interface[@name="IDataItemTransferCoordinator"]/method[@name="AcquireSessionLockAsync"]/*'/>*/
    Task<IAsyncDisposable> AcquireSessionLockAsync(Guid sessionid, CancellationToken cancel = default);
}