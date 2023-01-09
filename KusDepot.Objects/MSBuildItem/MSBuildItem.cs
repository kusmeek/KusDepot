namespace KusDepot
{
    /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItem"]'/> */
    [DataContract(Name = "MSBuildItem" , Namespace = "KusDepot")]
    public class MSBuildItem : DataItem , IComparable<MSBuildItem> , IEquatable<MSBuildItem>
    {
        /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItem"]/content[@Name = "Content"]'/> */
        [DataMember(Name = "Content" , EmitDefaultValue = false , IsRequired = true)]
        public String? Content;

        /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItem"]/constructor[@Name = "ParameterlessConstructor"]'/> */
        public MSBuildItem() { this.Initialize(new Guid()); }

        /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItem"]/constructor[@Name = "Constructor"]'/> */
        public MSBuildItem(String? content , Guid? id , List<String>? notes , List<String>? tags)
        {
            try
            {
                this.Initialize(id);
                this.Content = content;
                this.Notes   = notes;
                this.Tags    = tags;
                this.Type    = DataType.MSB;
            }
            catch ( Exception ie ) { throw new Exception("KusDepot.MSBuildItem.Constructor",ie); }
        }

        /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItem"]/method[@Name = "IComparable{MSBuildItem}.CompareTo"]'/> */
        public Int32 CompareTo(MSBuildItem? other)
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
            catch ( Exception ie ) { throw new ArgumentException("KusDepot.MSBuildItem.CompareTo - Please Check Argument",ie); }
        }

        /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItem"]/method[@Name = "EqualsObject"]'/> */
        public override Boolean Equals(Object? obj)
        {
            if(ReferenceEquals(this,obj)) { return true; }

            if(obj is null) { return false; }

            if( (obj as IEquatable<MSBuildItem>) == null) { return false; }

            IEquatable<MSBuildItem>? me = (IEquatable<MSBuildItem>)obj;

            return me.Equals(obj);
        }

        /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItem"]/method[@Name = "IEquatable{MSBuildItem}.Equals"]'/> */
        public Boolean Equals(MSBuildItem? other)
        {
            if(ReferenceEquals(this,other)) { return true; }

            if(other is null) { return false; }

            if( (other as MSBuildItem) == null) { return false; }

            try
            {
                if( !(this!.ID.HasValue) && (other!.ID.HasValue) ) { return false; }

                if( (this.ID.HasValue) && !(other.ID.HasValue) ) { return false; }

                if( (this.ID == other.ID) && (String.Equals(this.Content,other.Content)) ) { return true; }

                return false;
            }
            catch( Exception ) { return false; }
        }

        /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItem"]/method[@Name = "GetHashCode"]'/> */
        public override Int32 GetHashCode()
        {
            return HashCode.Combine<DateTime?,String?>(BornOn,Content);
        }

        /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItem"]/method[@Name = "Initialize"]'/> */
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

        /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItem"]/method[@Name = "Validate"]'/> */
        public static Boolean Validate(MSBuildItem? item)
        {
            if( item == null ) { return false; }

            try
            {
                MSBuildItemValidator validator = new MSBuildItemValidator();
                ValidationResult result = validator.Validate(item);
                return result.IsValid;
            }
            catch( Exception ) { return false; }
        }

    }

    /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItemValidator"]'/> */
    public class MSBuildItemValidator : AbstractValidator<MSBuildItem>
    {
        /** <include file = 'MSBuildItem.xml' path = 'MSBuildItem/class[@Name = "MSBuildItemValidator"]/constructor[@Name = "Constructor"]'/> */
        public MSBuildItemValidator()
        {
            this.RuleFor( (item) => item.Content).NotNull().NotEmpty();
            this.RuleFor( (item) => item.Type).NotNull().Equal(DataType.MSB);
        }

    }

}