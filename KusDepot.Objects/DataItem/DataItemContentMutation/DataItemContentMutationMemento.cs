namespace KusDepot;

/**<include file='DataItemContentMutationMemento.xml' path='DataItemContentMutationMemento/class[@name="DataItemContentMutationMemento"]/main/*'/>*/
internal sealed class DataItemContentMutationMemento
{
    /**<include file='DataItemContentMutationMemento.xml' path='DataItemContentMutationMemento/class[@name="DataItemContentMutationMemento"]/constructor[@name="Constructor"]/*'/>*/
    internal DataItemContentMutationMemento(Action<DataItem> restore)
    {
        this.restore = restore ?? throw new ArgumentNullException(nameof(restore));
    }

    /**<include file='DataItemContentMutationMemento.xml' path='DataItemContentMutationMemento/class[@name="DataItemContentMutationMemento"]/field[@name="restore"]/*'/>*/
    private readonly Action<DataItem> restore;

    /**<include file='DataItemContentMutationMemento.xml' path='DataItemContentMutationMemento/class[@name="DataItemContentMutationMemento"]/method[@name="Restore"]/*'/>*/
    internal void Restore(DataItem item) => this.restore(item);
}
