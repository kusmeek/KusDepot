namespace KusDepot;

/**<include file='DataTypeAttribute.xml' path='DataTypeAttribute/class[@name="DataTypeAttribute"]/main/*'/>*/
[AttributeUsage( (AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue) , AllowMultiple = true)]
public sealed class DataTypeAttribute : Attribute
{
    /**<include file='DataTypeAttribute.xml' path='DataTypeAttribute/class[@name="DataTypeAttribute"]/property[@name="DataType"]/*'/>*/
    public String DataType {get;}

    /**<include file='DataTypeAttribute.xml' path='DataTypeAttribute/class[@name="DataTypeAttribute"]/constructor[@name="Constructor"]/*'/>*/
    public DataTypeAttribute(String? datatype)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(datatype);

            String?[] AllTypes = typeof(DataType).GetFields().Select(_=>(String?)_.GetValue(null)).ToArray();

            if(AllTypes.Contains(datatype)) { this.DataType = datatype; }

            else { throw new ArgumentException("Invalid DataType",datatype); }
        }
        catch { throw; }
    }
}