namespace KusDepot
{
    /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItem"]'/> */
    [DataContract(Name = "BinaryItem" , Namespace = "KusDepot")]
    public class BinaryItem : DataItem , IComparable<BinaryItem> , IEquatable<BinaryItem>
    {
        /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItem"]/content[@Name = "Content"]'/> */
        [DataMember(Name = "Content" , EmitDefaultValue = false , IsRequired = true)]
        public Byte[]? Content;

        /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItem"]/constructor[@Name = "ParameterlessConstructor"]'/> */
        public BinaryItem() { this.Initialize(new Guid()); }

        /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItem"]/constructor[@Name = "Constructor"]'/> */
        public BinaryItem(Byte[]? content , Guid? id , List<String>? notes , List<String>? tags , String? type)
        {
            try
            {
                this.Initialize(id);
                this.Content = content;
                this.Notes   = notes;
                this.Tags    = tags;
                this.Type    = type;
            }
            catch ( Exception ie ) { throw new Exception("KusDepot.BinaryItem.Constructor",ie); }
        }

        /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItem"]/method[@Name = "IComparable{BinaryItem}.CompareTo"]'/> */
        public Int32 CompareTo(BinaryItem? other)
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
            catch ( Exception ie ) { throw new ArgumentException("KusDepot.BinaryItem.CompareTo - Please Check Argument",ie); }
        }

        /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItem"]/method[@Name = "EqualsObject"]'/> */
        public override Boolean Equals(Object? obj)
        {
            if(ReferenceEquals(this,obj)) { return true; }

            if(obj is null) { return false; }

            if( (obj as IEquatable<BinaryItem>) == null) { return false; }

            IEquatable<BinaryItem>? me = (IEquatable<BinaryItem>)obj;

            return me.Equals(obj);
        }

        /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItem"]/method[@Name = "IEquatable{BinaryItem}.Equals"]'/> */
        public Boolean Equals(BinaryItem? other)
        {
            if(ReferenceEquals(this,other)) { return true; }

            if(other is null) { return false; }

            if( (other as BinaryItem) == null) { return false; }

            try
            {
                if( !(this!.ID.HasValue) && (other!.ID.HasValue) ) { return false; }

                if( (this.ID.HasValue) && !(other.ID.HasValue) ) { return false; }

                if( this!.Content!.Length != other!.Content!.Length){ return false; }

                if( (this.ID == other.ID) && (this.Content.Length == other.Content.Length) )
                {
                    foreach(Byte b in this.Content)
                    {
                        if(Byte.Equals(this.Content[b],other.Content[b])) { continue; }

                        else { return false; }
                    }
                    return true;
                }

                return false;
            }
            catch( Exception ) { return false; }
        }

        /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItem"]/method[@Name = "GetHashCode"]'/> */
        public override Int32 GetHashCode()
        {
            return HashCode.Combine<DateTime?,Byte[]?,Guid?>(BornOn,Content,ID);
        }

        /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItem"]/method[@Name = "Initialize"]'/> */
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

        /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItem"]/method[@Name = "Validate"]'/> */
        public static Boolean Validate(BinaryItem? item)
        {
            if( item == null ) { return false; }

            try
            {
                BinaryItemValidator validator = new BinaryItemValidator();
                ValidationResult result = validator.Validate(item);
                return result.IsValid;
            }
            catch( Exception ) { return false; }
        }

    }

    /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItemValidator"]'/> */
    public class BinaryItemValidator : AbstractValidator<BinaryItem>
    {
        /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItemValidator"]/constructor[@Name = "Constructor"]'/> */
        public BinaryItemValidator()
        {
            this.RuleFor( (item) => item.Content).NotNull().NotEmpty();
            this.RuleFor( (item) => item.Type).NotNull().NotEmpty().Must( (item , String) => { return PassThisTest(item , item.Type); } );
        }

        /** <include file = 'BinaryItem.xml' path = 'BinaryItem/class[@Name = "BinaryItemValidator"]/method[@Name = "PassThisTest"]'/> */
        internal static Boolean PassThisTest(BinaryItem? item , String? type)
        {
            switch (type)
            {
                case DataType.ACCDB : return true;

                case DataType.APPX  : return true;

                case DataType.BAML  : return true;

                case DataType.BIN   : return true;

                case DataType.CAB   : return true;

                case DataType.CAD   : return true;

                case DataType.CER   : return true;

                case DataType.CIL   : return true;

                case DataType.COM   : return true;

                case DataType.DAT   : return true;

                case DataType.DAW   : return true;

                case DataType.DB    : return true;

                case DataType.DIT   : return true;

                case DataType.DKR   : return true;

                case DataType.DLL   : return true;

                case DataType.DMP   : return true;

                case DataType.EDB   : return true;

                case DataType.EOT   : return true;

                case DataType.EPUB  : return true;

                case DataType.ETL   : return true;

                case DataType.EVTX  : return true;

                case DataType.EXE   : return true;

                case DataType.EXT   : return true;

                case DataType.GZ    : return true;

                case DataType.ICO   : return true;

                case DataType.ISO   : return true;

                case DataType.JAR   : return true;

                case DataType.JAVA  : return true;

                case DataType.KEY   : return true;

                case DataType.KML   : return true;

                case DataType.LDF   : return true;

                case DataType.MDB   : return true;

                case DataType.MDF   : return true;

                case DataType.MSIL  : return true;

                case DataType.MSIX  : return true;

                case DataType.NDF   : return true;

                case DataType.OTF   : return true;

                case DataType.P7B   : return true;

                case DataType.PCAP  : return true;

                case DataType.PDB   : return true;

                case DataType.PSD1  : return true;

                case DataType.PSM1  : return true;

                case DataType.PSSC  : return true;

                case DataType.PEM   : return true;

                case DataType.PFX   : return true;

                case DataType.PHP   : return true;

                case DataType.RAR   : return true;

                case DataType.RLIB  : return true;

                case DataType.SCR   : return true;

                case DataType.SO    : return true;

                case DataType.SUO   : return true;

                case DataType.SYS   : return true;

                case DataType.TAR   : return true;

                case DataType.TTF   : return true;

                case DataType.VHDX  : return true;

                case DataType.WASM  : return true;

                case DataType.WIM   : return true;

                case DataType.WINMD : return true;

                case DataType.WOFF  : return true;

                case DataType.XAP   : return true;

                case DataType.ZIP   : return true;

                default : return false;
            }
        }

    }

}