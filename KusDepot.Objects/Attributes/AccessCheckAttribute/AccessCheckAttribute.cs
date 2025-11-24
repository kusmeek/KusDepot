namespace KusDepot; 

/**<include file='AccessCheckAttribute.xml' path='AccessCheckAttribute/class[@name="AccessCheckAttribute"]/main/*'/>*/
[AttributeUsage(AttributeTargets.Method , AllowMultiple = false)]
public sealed class AccessCheckAttribute : Attribute
{
    /**<include file='AccessCheckAttribute.xml' path='AccessCheckAttribute/class[@name="AccessCheckAttribute"]/property[@name="Index"]/*'/>*/
    public Int32 Index {get;}

    /**<include file='AccessCheckAttribute.xml' path='AccessCheckAttribute/class[@name="AccessCheckAttribute"]/constructor[@name="Constructor"]/*'/>*/
    public AccessCheckAttribute(Int32 index) => Index = index;
}