namespace KusDepot.Data.Services.DataTransfer;

/**<include file='IDataItemTransferRegistry.xml' path='IDataItemTransferRegistry/interface[@name="IDataItemTransferRegistry"]/main/*'/>*/
public interface IDataItemTransferRegistry
{
    /**<include file='IDataItemTransferRegistry.xml' path='IDataItemTransferRegistry/interface[@name="IDataItemTransferRegistry"]/method[@name="FindOpenConflictByItemId"]/*'/>*/
    Task<DataItemTransferOpenConflict?> FindOpenConflictByItemId(Guid itemid , CancellationToken cancel = default);
}