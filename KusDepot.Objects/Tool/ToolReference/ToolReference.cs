namespace KusDepot;

/**<include file='ToolReference.xml' path='ToolReference/class[@name="ToolReference"]/main/*'/>*/
public sealed class ToolReference
{
    private ITool? tool;

    private AccessKey? key;

    /**<include file='ToolReference.xml' path='ToolReference/class[@name="ToolReference"]/property[@name="Tool"]/*'/>*/
    public ITool? Tool { get => tool; set => tool ??= value; }

    /**<include file='ToolReference.xml' path='ToolReference/class[@name="ToolReference"]/property[@name="Key"]/*'/>*/
    public AccessKey? Key { get => key; set => key ??= value; }
}