namespace KusDepot;

/**<include file='ServiceContainerAttribute.xml' path='ServiceContainerAttribute/class[@name="ServiceContainerAttribute"]/main/*'/>*/
[AttributeUsage(AttributeTargets.Assembly , AllowMultiple = true)]
public sealed class ServiceContainerAttribute : Attribute
{
    /**<include file='ServiceContainerAttribute.xml' path='ServiceContainerAttribute/class[@name="ServiceContainerAttribute"]/property[@name="Type"]/*'/>*/
    public Type Type {get;}

    /**<include file='ServiceContainerAttribute.xml' path='ServiceContainerAttribute/class[@name="ServiceContainerAttribute"]/property[@name="Specifications"]/*'/>*/
    public String Specifications {get;}

    /**<include file='ServiceContainerAttribute.xml' path='ServiceContainerAttribute/class[@name="ServiceContainerAttribute"]/constructor[@name="Constructor"]/*'/>*/
    public ServiceContainerAttribute(Type type , String specifications)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(specifications);

        this.Type = type; this.Specifications = specifications;
    }
}