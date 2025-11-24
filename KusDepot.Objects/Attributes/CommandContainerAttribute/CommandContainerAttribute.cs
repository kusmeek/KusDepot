namespace KusDepot;

/**<include file='CommandContainerAttribute.xml' path='CommandContainerAttribute/class[@name="CommandContainerAttribute"]/main/*'/>*/
[AttributeUsage(AttributeTargets.Assembly , AllowMultiple = true)]
public sealed class CommandContainerAttribute : Attribute
{
    /**<include file='CommandContainerAttribute.xml' path='CommandContainerAttribute/class[@name="CommandContainerAttribute"]/property[@name="Type"]/*'/>*/
    public Type Type {get;}

    /**<include file='CommandContainerAttribute.xml' path='CommandContainerAttribute/class[@name="CommandContainerAttribute"]/property[@name="Handle"]/*'/>*/
    public String Handle {get;}

    /**<include file='CommandContainerAttribute.xml' path='CommandContainerAttribute/class[@name="CommandContainerAttribute"]/property[@name="Specifications"]/*'/>*/
    public String Specifications {get;}

    /**<include file='CommandContainerAttribute.xml' path='CommandContainerAttribute/class[@name="CommandContainerAttribute"]/constructor[@name="Constructor"]/*'/>*/
    public CommandContainerAttribute(Type type , String handle , String specifications)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(handle);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(specifications);

        this.Type = type; this.Handle = handle; this.Specifications = specifications;
    }
}