namespace DataPodServices.DataControl;

/**<include file='ITransferItemLockLease.xml' path='ITransferItemLockLease/interface[@name="ITransferItemLockLease"]/main/*'/>*/
public interface ITransferItemLockLease : IAsyncDisposable
{
    /**<include file='ITransferItemLockLease.xml' path='ITransferItemLockLease/interface[@name="ITransferItemLockLease"]/method[@name="AcquireSessionLockAsync"]/*'/>*/
    Task AcquireSessionLockAsync(Guid sessionid , CancellationToken cancel = default);
}
