namespace KusDepot;

/**<include file='MetaSync.xml' path='MetaSync/class[@name="MetaSync"]/main/*'/>*/
public class MetaSync : SyncNode
{
    /**<include file='MetaSync.xml' path='MetaSync/class[@name="MetaSync"]/field[@name="Meta"]/*'/>*/
    public readonly SemaphoreSlim Meta = new(1,1);
}