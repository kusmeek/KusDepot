namespace KusDepot.Data.Clients;

/**<include file='DataItemTransferProgressReporter.xml' path='DataItemTransferProgressReporter/class[@name="DataItemTransferProgressReporter"]/main/*'/>*/
public sealed class DataItemTransferProgressReporter : IProgress<DataItemTransferProgress>
{
    /**<include file='DataItemTransferProgressReporter.xml' path='DataItemTransferProgressReporter/class[@name="DataItemTransferProgressReporter"]/field[@name="callback"]/*'/>*/
    private readonly Action<DataItemTransferProgress>? callback;

    /**<include file='DataItemTransferProgressReporter.xml' path='DataItemTransferProgressReporter/class[@name="DataItemTransferProgressReporter"]/property[@name="Latest"]/*'/>*/
    public DataItemTransferProgress? Latest { get; private set; }

    /**<include file='DataItemTransferProgressReporter.xml' path='DataItemTransferProgressReporter/class[@name="DataItemTransferProgressReporter"]/constructor[@name="DataItemTransferProgressReporter"]/*'/>*/
    public DataItemTransferProgressReporter(Action<DataItemTransferProgress>? callback = null) => this.callback = callback;

    /**<include file='DataItemTransferProgressReporter.xml' path='DataItemTransferProgressReporter/class[@name="DataItemTransferProgressReporter"]/method[@name="Create"]/*'/>*/
    public static DataItemTransferProgressReporter Create(Action<DataItemTransferProgress>? callback = null) => new(callback);

    /**<include file='DataItemTransferProgressReporter.xml' path='DataItemTransferProgressReporter/class[@name="DataItemTransferProgressReporter"]/method[@name="Report"]/*'/>*/
    public void Report(DataItemTransferProgress value)
    {
        this.Latest = value; this.callback?.Invoke(value);
    }
}