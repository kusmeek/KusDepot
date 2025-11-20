namespace KusDepot;

/**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/main/*'/>*/
public sealed class ToolSync : SyncNode
{
    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="Activities"]/*'/>*/
    public readonly Object Activities = new Object();

    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="Commands"]/*'/>*/
    public readonly SemaphoreSlim Commands = new(1,1);

    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="CommandTypes"]/*'/>*/
    public readonly Object CommandTypes = new Object();

    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="Data"]/*'/>*/
    public readonly Object Data = new Object();

    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="Disposed"]/*'/>*/
    public readonly SemaphoreSlim Disposed = new(1,1);

    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="HostedServices"]/*'/>*/
    public readonly Object HostedServices = new Object();

    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="Inputs"]/*'/>*/
    public readonly Object Inputs = new Object();

    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="Life"]/*'/>*/
    public readonly SemaphoreSlim Life = new(1,1);

    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="Logger"]/*'/>*/
    public readonly Object Logger = new Object();

    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="Outputs"]/*'/>*/
    public readonly SemaphoreSlim Outputs = new(1,1);

    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="OwnerSecret"]/*'/>*/
    public readonly Object OwnerSecret = new Object();

    /**<include file='ToolSync.xml' path='ToolSync/class[@name="ToolSync"]/field[@name="Status"]/*'/>*/
    public readonly Object Status = new Object();
}