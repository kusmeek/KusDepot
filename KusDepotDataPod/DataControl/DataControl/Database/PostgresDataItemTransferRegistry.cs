namespace DataPodServices.DataControl;

/**<include file='PostgresDataItemTransferRegistry.xml' path='PostgresDataItemTransferRegistry/class[@name="PostgresDataItemTransferRegistry"]/main/*'/>*/
public sealed class PostgresDataItemTransferRegistry : IDataItemTransferRegistry
{
    /**<include file='PostgresDataItemTransferRegistry.xml' path='PostgresDataItemTransferRegistry/class[@name="PostgresDataItemTransferRegistry"]/field[@name="state"]/*'/>*/
    private readonly ITransferStateStore state;

    /**<include file='PostgresDataItemTransferRegistry.xml' path='PostgresDataItemTransferRegistry/class[@name="PostgresDataItemTransferRegistry"]/constructor[@name="PostgresDataItemTransferRegistry"]/*'/>*/
    public PostgresDataItemTransferRegistry(ITransferStateStore state)
    {
        this.state = state ?? throw new ArgumentNullException(nameof(state));
    }

    /// <inheritdoc/>
    public Task<DataItemTransferOpenConflict?> FindOpenConflictByItemId(Guid itemid , String catalogname , CancellationToken cancel = default)
    {
        return this.state.FindOpenConflictAsync(itemid,catalogname,cancel);
    }
}