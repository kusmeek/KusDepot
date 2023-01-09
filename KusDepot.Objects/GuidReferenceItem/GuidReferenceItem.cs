namespace KusDepot
{
    /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]'/> */
    [DataContract(Name = "GuidReferenceItem" , Namespace = "KusDepot")]
    public class GuidReferenceItem : DataItem , IComparable<GuidReferenceItem> , IEquatable<GuidReferenceItem>
    {
        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/content[@Name = "Content"]'/> */
        [DataMember(Name = "Content" , EmitDefaultValue = false , IsRequired = true)]
        public Guid? Content;

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/constructor[@Name = "ParameterlessConstructor"]'/> */
        public GuidReferenceItem() { this.Initialize(new Guid()); }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/constructor[@Name = "Constructor"]'/> */
        public GuidReferenceItem(Guid? content , Guid? id , List<String>? notes , List<String>? tags)
        {
            try
            {
                this.Initialize(id);
                this.Content = content;
                this.Notes   = notes;
                this.Tags    = tags;
                this.Type    = DataType.GUID;
            }
            catch ( Exception ie ) { throw new Exception("KusDepot.GuidReferenceItem.Constructor",ie); }
        }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/method[@Name = "IComparable{GuidReferenceItem}.CompareTo"]'/> */
        public Int32 CompareTo(GuidReferenceItem? other)
        {
            try
            {
                if (ReferenceEquals(this, other))                         { return 0; }
                if (other is null)                                        { return 1; }

                if (this!.Content.HasValue && !(other!.Content.HasValue)) { return 1; }
                if (!(this.Content.HasValue) && other.Content.HasValue)   { return -1; }
                if (this.Content!.Value < other.Content!.Value)           { return -1; }
                if (this.Content.Value > other.Content.Value)             { return 1; }
                if (this.Content.Value == other.Content.Value)            { return 0; }

                throw new InvalidProgramException();
            }
            catch ( Exception ie ) { throw new ArgumentException("KusDepot.GuidReferenceItem.CompareTo - Please Check Argument",ie); }
        }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/method[@Name = "EqualsObject"]'/> */
        public override Boolean Equals(Object? obj)
        {
            if(ReferenceEquals(this,obj)) { return true; }

            if(obj is null) { return false; }

            if( (obj as IEquatable<GuidReferenceItem>) == null) { return false; }

            IEquatable<GuidReferenceItem>? me = (IEquatable<GuidReferenceItem>)obj;

            return me.Equals(obj);
        }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/method[@Name = "IEquatable{GuidReferenceItem}.Equals"]'/> */
        public Boolean Equals(GuidReferenceItem? other)
        {
            if(ReferenceEquals(this,other)) { return true; }

            if(other is null) { return false; }

            if( (other as GuidReferenceItem) == null) { return false; }

            try
            {
                if( !(this!.ID.HasValue) && (other!.ID.HasValue) ) { return false; }

                if( (this.ID.HasValue) && !(other.ID.HasValue) ) { return false; }

                if( !(this!.Content.HasValue) && (other!.Content.HasValue) ) { return false; }

                if( (this.Content.HasValue) && !(other.Content.HasValue) ) { return false; }

                return ( (this.ID == other.ID) && (this.Content == other.Content) );
            }
            catch( Exception ) { return false; }
        }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/method[@Name = "GetHashCode"]'/> */
        public override Int32 GetHashCode()
        {
            return HashCode.Combine<DateTime?,Guid?,Guid?>(BornOn,Content,ID);
        }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/method[@Name = "Initialize"]'/> */
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

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/method[@Name = "Validate"]'/> */
        public static Boolean Validate(GuidReferenceItem? item)
        {
            if (item == null) { return false; }

            try
            {
                GuidReferenceItemValidator validator = new GuidReferenceItemValidator();
                ValidationResult result = validator.Validate(item);
                return result.IsValid;
            }
            catch( Exception ) { return false; }
        }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/operator[@Name = "OperatorEQ"]'/> */
        public static Boolean operator ==(GuidReferenceItem? _1 , GuidReferenceItem? _2)
        {
            if(ReferenceEquals(_1,_2)) { return true; }

            if (_1 is null || _2 is null) { Trace.WriteLine("KusDepot.GuidReferenceItem.OperatorEQ - Please Check Arguments"); return false; }

            if( !(_1!.Content.HasValue) && (_2!.Content.HasValue) ) { return false; }

            if( (_1.Content.HasValue) && !(_2.Content.HasValue) ) { return false; }

            return _1.Content == _2.Content;
        }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/operator[@Name = "OperatorNE"]'/> */
        public static Boolean operator !=(GuidReferenceItem? _1 , GuidReferenceItem? _2)
        {
            if (_1 is null || _2 is null) { Trace.WriteLine("KusDepot.GuidReferenceItem.OperatorNE - Please Check Arguments"); return false; }

            if( !(_1!.Content.HasValue) && (_2!.Content.HasValue) ) { return true; }

            if( (_1.Content.HasValue) && !(_2.Content.HasValue) ) { return true; }

            return _1.Content != _2.Content;
        }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/operator[@Name = "OperatorLT"]'/> */
        public static Boolean operator <(GuidReferenceItem? _1 , GuidReferenceItem? _2)
        {
            if (_1 is null || _2 is null) { Trace.WriteLine("KusDepot.GuidReferenceItem.OperatorLT - Please Check Arguments"); return false; }

            if( !(_1!.Content.HasValue) && (_2!.Content.HasValue) ) { return true; }

            if( (_1.Content.HasValue) && !(_2.Content.HasValue) ) { return false; }

            return _1.Content < _2.Content;
        }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/operator[@Name = "OperatorLTE"]'/> */
        public static Boolean operator <=(GuidReferenceItem? _1 , GuidReferenceItem? _2)
        {
            if (_1 is null || _2 is null) { Trace.WriteLine("KusDepot.GuidReferenceItem.OperatorLTE - Please Check Arguments"); return false; }

            if( !(_1!.Content.HasValue) && !(_2!.Content.HasValue) ) { return true; }

            if( !(_1.Content.HasValue) && (_2.Content.HasValue) ) { return true; }

            if( (_1.Content.HasValue) && !(_2.Content.HasValue) ) { return false; }

            return _1.Content <= _2.Content;
        }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/operator[@Name = "OperatorGT"]'/> */
        public static Boolean operator >(GuidReferenceItem? _1 , GuidReferenceItem? _2)
        {
            if (_1 is null || _2 is null) { Trace.WriteLine("KusDepot.GuidReferenceItem.OperatorGT - Please Check Arguments"); return false; }

            if( !(_1!.Content.HasValue) && !(_2!.Content.HasValue) ) { return false; }

            if( !(_1.Content.HasValue) && (_2.Content.HasValue) ) { return false; }

            if( (_1.Content.HasValue) && !(_2.Content.HasValue) ) { return true; }

            return _1.Content > _2.Content;
        }

        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItem"]/operator[@Name = "OperatorGTE"]'/> */
        public static Boolean operator >=(GuidReferenceItem? _1 , GuidReferenceItem? _2)
        {
            if (_1 is null || _2 is null) { Trace.WriteLine("KusDepot.GuidReferenceItem.OperatorGTE - Please Check Arguments"); return false; }

            if( !(_1!.Content.HasValue) && !(_2!.Content.HasValue) ) { return true; }

            if( !(_1.Content.HasValue) && (_2.Content.HasValue) ) { return false; }

            if( (_1.Content.HasValue) && !(_2.Content.HasValue) ) { return true; }

            return _1.Content >= _2.Content;
        }

    }

    /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItemValidator"]'/> */
    public class GuidReferenceItemValidator : AbstractValidator<GuidReferenceItem>
    {
        /** <include file = 'GuidReferenceItem.xml' path = 'GuidReferenceItem/class[@Name = "GuidReferenceItemValidator"]/constructor[@Name = "Constructor"]'/> */
        public GuidReferenceItemValidator()
        {
            this.RuleFor( (item) => item.Content).NotNull().NotEmpty();
            this.RuleFor( (item) => item.Type).NotNull().Equal(DataType.GUID);
        }

    }

}