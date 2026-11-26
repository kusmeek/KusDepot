namespace DataPodServices.DataControl;

/**<include file='IDataItemTransferRegistry.xml' path='IDataItemTransferRegistry/interface[@name="IDataItemTransferRegistry"]/main/*'/>*/
public interface IDataItemTransferRegistry
{
    /**<include file='IDataItemTransferRegistry.xml' path='IDataItemTransferRegistry/interface[@name="IDataItemTransferRegistry"]/method[@name="FindOpenConflictByItemId"]/*'/>*/
    Task<DataItemTransferOpenConflict?> FindOpenConflictByItemId(Guid itemid , String catalogname , CancellationToken cancel = default);
}