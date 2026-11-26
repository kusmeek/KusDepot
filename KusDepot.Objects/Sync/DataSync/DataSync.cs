namespace KusDepot.Sync;

/**<include file='DataSync.xml' path='DataSync/class[@name="DataSync"]/main/*'/>*/
public sealed class DataSync : MetaSync
{
    /**<include file='DataSync.xml' path='DataSync/class[@name="DataSync"]/field[@name="Data"]/*'/>*/
    public readonly SemaphoreSlim Data = new(1,1);
}