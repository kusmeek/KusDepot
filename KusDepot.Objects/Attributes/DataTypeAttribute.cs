namespace KusDepot
{
    /** <include file = 'DataTypeAttribute.xml' path = 'DataTypeAttribute/class[@Name = "DataTypeAttribute"]'/> */
    [AttributeUsage( (AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.GenericParameter | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue) , AllowMultiple = true)]
    public class DataTypeAttribute : Attribute
    {
        /** <include file = 'DataTypeAttribute.xml' path = 'DataTypeAttribute/class[@Name = "DataTypeAttribute"]/field[@Name = "DataType"]'/> */
        public readonly String? DataType;

        /** <include file = 'DataTypeAttribute.xml' path = 'DataTypeAttribute/class[@Name = "DataTypeAttribute"]/constructor[@Name = "Constructor"]'/> */
        public DataTypeAttribute(String? datatype)
        {
            String?[] AllTypes;

            Type _0 = typeof(DataType);

            FieldInfo[] _1 = _0.GetFields();

            Func<FieldInfo,String?> _2 = (FieldInfo field) => (String?)field.GetValue(null);

            AllTypes = _1.Select(_2).ToArray();

            try
            {
                if( (datatype is not null) && (AllTypes.Contains(datatype)) )
                {
                    this.DataType = datatype;
                }

                else
                {
                    throw new Exception("KusDepot.DataTypeAttribute.Constructor");
                }
            }
            catch ( Exception ie ) { throw new Exception("KusDepot.DataTypeAttribute.Constructor",ie); }
        }

    }

}