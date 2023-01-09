namespace KusDepot
{
    /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]'/> */
    [DataContract(Name = "Context" , Namespace = "KusDepot")]
    public abstract class Context
    {
        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "AppDomainID"]'/> */
        [DataMember(Name = "AppDomainID" , EmitDefaultValue = false , IsRequired = false)]
        public Int128? AppDomainID;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "AppDomainUID"]'/> */
        [DataMember(Name = "AppDomainUID" , EmitDefaultValue = false , IsRequired = false)]
        public Int128? AppDomainUID;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "AssemblyVersion"]'/> */
        [DataMember(Name = "AssemblyVersion" , EmitDefaultValue = false , IsRequired = false)]
        public Version? AssemblyVersion;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "BornOn"]'/> */
        [DataMember(Name = "BornOn" , EmitDefaultValue = false , IsRequired = false)]
        public DateTime? BornOn;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "CPUID"]'/> */
        [DataMember(Name = "CPUID" , EmitDefaultValue = false , IsRequired = false)]
        public Int64? CPUID;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "DomainID"]'/> */
        [DataMember(Name = "DomainID" , EmitDefaultValue = false , IsRequired = false)]
        public String? DomainID;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "Extension"]'/> */
        [DataMember(Name = "Extension" , EmitDefaultValue = false , IsRequired = false)]
        public dynamic? Extension;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "GPS"]'/> */
        [DataMember(Name = "GPS" , EmitDefaultValue = false , IsRequired = false)]
        public String? GPS;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "ID"]'/> */
        [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = false)]
        public Guid? ID;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "MachineID"]'/> */
        [DataMember(Name = "MachineID" , EmitDefaultValue = false , IsRequired = false)]
        public String? MachineID;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "Modified"]'/> */
        [DataMember(Name = "Modified" , EmitDefaultValue = false , IsRequired = false)]
        public DateTime? Modified;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "ProcessID"]'/> */
        [DataMember(Name = "ProcessID" , EmitDefaultValue = false , IsRequired = false)]
        public Int128? ProcessID;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "SecurityDescriptor"]'/> */
        [DataMember(Name = "SecurityDescriptor" , EmitDefaultValue = false , IsRequired = false)]
        public String? SecurityDescriptor;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/field[@Name = "URI"]'/> */
        [DataMember(Name = "URI" , EmitDefaultValue = false , IsRequired = false)]
        public String? URI;

        /** <include file = 'Context.xml' path = 'Context/class[@Name = "Context"]/method[@Name = "Initialize"]'/> */
        public abstract void Initialize(Guid? id);
    }

}