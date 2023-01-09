namespace KusDepot
{
    /** <include file = 'GenericItem.xml' path = 'GenericItem/class[@Name = "GenericItem"]'/> */
    [DataContract(Name = "GenericItem" , Namespace = "KusDepot")]
    public class GenericItem : DataItem , IComparable<GenericItem> , IEquatable<GenericItem>
    {
        /** <include file = 'GenericItem.xml' path = 'GenericItem/class[@Name = "GenericItem"]/content[@Name = "Content"]'/> */
        [DataMember(Name = "Content" , EmitDefaultValue = false , IsRequired = true)]
        public List<Object>? Content;

        /** <include file = 'GenericItem.xml' path = 'GenericItem/class[@Name = "GenericItem"]/constructor[@Name = "ParameterlessConstructor"]'/> */
        public GenericItem() { this.Initialize(new Guid()); }

        /** <include file = 'GenericItem.xml' path = 'GenericItem/class[@Name = "GenericItem"]/constructor[@Name = "Constructor"]'/> */
        public GenericItem(List<Object>? content , Guid? id , List<String>? notes , List<String>? tags , String? type)
        {
            try
            {
                this.Initialize(id);
                this.Content = content;
                this.Notes   = notes;
                this.Tags    = tags;
                this.Type    = type;
            }
            catch ( Exception ie ) { throw new Exception("KusDepot.GenericItem.Constructor",ie); }
        }

        /** <include file = 'GenericItem.xml' path = 'GenericItem/class[@Name = "GenericItem"]/method[@Name = "IComparable{GenericItem}.CompareTo"]'/> */
        public Int32 CompareTo(GenericItem? other)
        {
            try
            {
                if (ReferenceEquals(this, other))                         { return 0; }
                if (other is null)                                        { return 1; }

                if (this.BornOn < other.BornOn)                           { return -1; }
                if (this.BornOn > other.BornOn)                           { return 1; }
                if (this.BornOn == other.BornOn)                          { return 0; }

                throw new InvalidProgramException();
            }
            catch ( Exception ie ) { throw new ArgumentException("KusDepot.GenericItem.CompareTo - Please Check Argument",ie); }
        }

        /** <include file = 'GenericItem.xml' path = 'GenericItem/class[@Name = "GenericItem"]/method[@Name = "EqualsObject"]'/> */
        public override Boolean Equals(Object? obj)
        {
            if(ReferenceEquals(this,obj)) { return true; }

            if(obj is null) { return false; }

            if( (obj as IEquatable<GenericItem>) == null) { return false; }

            IEquatable<GenericItem>? me = (IEquatable<GenericItem>)obj;

            return me.Equals(obj);
        }

        /** <include file = 'GenericItem.xml' path = 'GenericItem/class[@Name = "GenericItem"]/method[@Name = "IEquatable{GenericItem}.Equals"]'/> */
        public Boolean Equals(GenericItem? other)
        {
            if(ReferenceEquals(this,other)) { return true; }

            if(other is null) { return false; }

            if( (other as GenericItem) == null ) { return false; }

            try
            {
                if( !(this!.ID.HasValue) && (other!.ID.HasValue) ) { return false; }

                if( (this.ID.HasValue) && !(other.ID.HasValue) ) { return false; }

                return this.ID == other.ID;
            }
            catch( Exception ) { return false; }
        }

        /** <include file = 'GenericItem.xml' path = 'GenericItem/class[@Name = "GenericItem"]/method[@Name = "GetHashCode"]'/> */
        public override Int32 GetHashCode()
        {
            return HashCode.Combine<DateTime?,Guid?>(BornOn,ID);
        }

        /** <include file = 'GenericItem.xml' path = 'GenericItem/class[@Name = "GenericItem"]/method[@Name = "Initialize"]'/> */
        public override void Initialize(Guid? id)
        {
            try
            {
                this.AppDomainID     = AppDomain.CurrentDomain.Id;

                this.AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

                this.BornOn          = DateTime.Now;

                this.ID              = id ?? new Guid();

                Process me           = Process.GetCurrentProcess();

                this.MachineID       = me.MachineName;

                this.ProcessID       = me.Id;
            }
            catch( Exception ) { return; }
        }

    }

}