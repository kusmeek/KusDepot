namespace KusDepot;

/**<include file='DataItemContentMutation.xml' path='DataItemContentMutation/class[@name="DataItemContentMutation"]/main/*'/>*/
internal sealed class DataItemContentMutation : IDisposable
{
    /**<include file='DataItemContentMutation.xml' path='DataItemContentMutation/class[@name="DataItemContentMutation"]/field[@name="item"]/*'/>*/
    private readonly DataItem item;

    /**<include file='DataItemContentMutation.xml' path='DataItemContentMutation/class[@name="DataItemContentMutation"]/field[@name="snapshot"]/*'/>*/
    private readonly DataItemStateSnapshot snapshot;

    /**<include file='DataItemContentMutation.xml' path='DataItemContentMutation/class[@name="DataItemContentMutation"]/field[@name="committed"]/*'/>*/
    private Boolean committed;

    /**<include file='DataItemContentMutation.xml' path='DataItemContentMutation/class[@name="DataItemContentMutation"]/constructor[@name="Constructor"]/*'/>*/
    internal DataItemContentMutation(DataItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        this.item = item;

        this.snapshot = DataItemStateSnapshot.CaptureForContentMutation_NoSync(item);
    }

    /**<include file='DataItemContentMutation.xml' path='DataItemContentMutation/class[@name="DataItemContentMutation"]/method[@name="Commit"]/*'/>*/
    internal void Commit(Action commit)
    {
        if(this.committed) { throw new InvalidOperationException("The content mutation has already been committed."); }

        ArgumentNullException.ThrowIfNull(commit);

        commit();

        this.committed = true;
    }

    /**<include file='DataItemContentMutation.xml' path='DataItemContentMutation/class[@name="DataItemContentMutation"]/method[@name="Dispose"]/*'/>*/
    public void Dispose()
    {
        if(this.committed is false) { this.snapshot.Restore(this.item); }
    }
}
