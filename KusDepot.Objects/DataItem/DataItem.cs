namespace KusDepot
{
    /** <include file = 'DataItem.xml' path = 'DataItem/class[@Name = "DataItem"]'/> */
    [DataContract(Name = "DataItem" , Namespace = "KusDepot")]
    public abstract class DataItem : Context
    {
        /** <include file = 'DataItem.xml' path = 'DataItem/class[@Name = "DataItem"]/field[@Name = "Name"]'/> */
        [DataMember(Name = "Name" , EmitDefaultValue = false , IsRequired = false)]
        public String? Name;

        /** <include file = 'DataItem.xml' path = 'DataItem/class[@Name = "DataItem"]/field[@Name = "Notes"]'/> */
        [DataMember(Name = "Notes" , EmitDefaultValue = false , IsRequired = false)]
        public List<String>? Notes;

        /** <include file = 'DataItem.xml' path = 'DataItem/class[@Name = "DataItem"]/field[@Name = "Tags"]'/> */
        [DataMember(Name = "Tags" , EmitDefaultValue = false , IsRequired = false)]
        public List<String>? Tags;

        /** <include file = 'DataItem.xml' path = 'DataItem/class[@Name = "DataItem"]/field[@Name = "Type"]'/> */
        [DataMember(Name = "Type" , EmitDefaultValue = false , IsRequired = false)]
        public String? Type;
    }

}