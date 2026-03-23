namespace KusDepot;

/**<include file='DataTagAttribute.xml' path='DataTagAttribute/class[@name="DataTagAttribute"]/main/*'/>*/
[AttributeUsage( (AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue) , AllowMultiple = true)]
public sealed class DataTagAttribute : Attribute
{
    /**<include file='DataTagAttribute.xml' path='DataTagAttribute/class[@name="DataTagAttribute"]/property[@name="Tag"]/*'/>*/
    public String Tag {get;}

    /**<include file='DataTagAttribute.xml' path='DataTagAttribute/class[@name="DataTagAttribute"]/constructor[@name="Constructor"]/*'/>*/
    public DataTagAttribute(String? tag)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(tag);

            if(GetAllDataTags().Contains(tag)) { this.Tag = tag; }

            else { throw new ArgumentException("Invalid Tag",tag); }
        }
        catch { throw; }
    }
}