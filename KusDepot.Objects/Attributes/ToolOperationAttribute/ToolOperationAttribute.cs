namespace KusDepot;

/**<include file='ToolOperationAttribute.xml' path='ToolOperationAttribute/class[@name="ToolOperationAttribute"]/main/*'/>*/
[AttributeUsage(AttributeTargets.Method , AllowMultiple = false , Inherited = false)]
public sealed class ToolOperationAttribute : Attribute
{
    /**<include file='ToolOperationAttribute.xml' path='ToolOperationAttribute/class[@name="ToolOperationAttribute"]/property[@name="Description"]/*'/>*/
    public String? Description { get; init; }
}