namespace KusDepot
{
    /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItem"]'/> */
    [DataContract(Name = "TextItem" , Namespace = "KusDepot")]
    public class TextItem : DataItem , IComparable<TextItem> , IEquatable<TextItem>
    {
        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItem"]/content[@Name = "Content"]'/> */
        [DataMember(Name = "Content" , EmitDefaultValue = false , IsRequired = true)]
        public String? Content;

        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItem"]/property[@Name = "Language"]'/> */
        [DataMember(Name = "Language" , EmitDefaultValue = false , IsRequired = false)]
        public String? Language;

        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItem"]/constructor[@Name = "ParameterlessConstructor"]'/> */
        public TextItem() { this.Initialize(new Guid()); }

        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItem"]/constructor[@Name = "Constructor"]'/> */
        public TextItem(String? content , Guid? id , String? language , List<String>? notes , List<String>? tags , String? type)
        {
            try
            {
                this.Initialize(id);
                this.Content  = content;
                this.Language = language;
                this.Notes    = notes;
                this.Tags     = tags;
                this.Type     = type;
            }
            catch ( Exception ie ) { throw new Exception("KusDepot.TextItem.Constructor",ie); }
        }

        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItem"]/method[@Name = "IComparable{TextItem}.CompareTo"]'/> */
        public Int32 CompareTo(TextItem? other)
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
            catch ( Exception ie ) { throw new ArgumentException("KusDepot.TextItem.CompareTo - Please Check Argument",ie); }
        }

        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItem"]/method[@Name = "EqualsObject"]'/> */
        public override Boolean Equals(Object? obj)
        {
            if(ReferenceEquals(this,obj)) { return true; }

            if(obj is null) { return false; }

            if( (obj as IEquatable<TextItem>) == null) { return false; }

            IEquatable<TextItem>? me = (IEquatable<TextItem>)obj;

            return me.Equals(obj);
        }

        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItem"]/method[@Name = "IEquatable{TextItem}.Equals"]'/> */
        public Boolean Equals(TextItem? other)
        {
            if(ReferenceEquals(this,other)) { return true; }

            if(other is null) { return false; }

            if( (other as TextItem) == null) { return false; }

            try
            {
                if( !(this!.ID.HasValue) && (other!.ID.HasValue) ) { return false; }

                if( (this.ID.HasValue) && !(other.ID.HasValue) ) { return false; }

                if( (this.ID == other.ID) && (String.Equals(this.Content,other.Content)) ) { return true; }

                return false;
            }
            catch( Exception ) { return false; }
        }

        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItem"]/method[@Name = "GetHashCode"]'/> */
        public override Int32 GetHashCode()
        {
            return HashCode.Combine<DateTime?,String?,Guid?,String?>(BornOn,Content,ID,Language);
        }

        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItem"]/method[@Name = "Initialize"]'/> */
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

        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItem"]/method[@Name = "Validate"]'/> */
        public static Boolean Validate(TextItem? item)
        {
            if( item == null ) { return false; }

            try
            {
                TextItemValidator validator = new TextItemValidator();
                ValidationResult result = validator.Validate(item);
                return result.IsValid;
            }
            catch( Exception ) { return false; }
        }

    }

    /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItemValidator"]'/> */
    public class TextItemValidator : AbstractValidator<TextItem>
    {
        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItemValidator"]/constructor[@Name = "Constructor"]'/> */
        public TextItemValidator()
        {
            this.RuleFor( (item) => item.Content).NotNull().NotEmpty();
            this.RuleFor( (item) => item.Type).NotNull().NotEmpty().Must( (item , String) => { return PassThisTest(item , item.Type); } );
        }

        /** <include file = 'TextItem.xml' path = 'TextItem/class[@Name = "TextItemValidator"]/method[@Name = "PassThisTest"]'/> */
        internal static Boolean PassThisTest(TextItem? item , String? type)
        {
            switch (type)
            {
                case DataType.ADML   : return true;

                case DataType.ADMX   : return true;

                case DataType.AHK    : return true;

                case DataType.API    : return true;

                case DataType.ARM    : return true;

                case DataType.ASM    : return true;

                case DataType.AU3    : return true;

                case DataType.BAT    : return true;

                case DataType.BICEP  : return true;

                case DataType.C      : return true;

                case DataType.CFG    : return true;

                case DataType.CONFIG : return true;

                case DataType.CPP    : return true;

                case DataType.CS     : return true;

                case DataType.CSHTML : return true;

                case DataType.CSPROJ : return true;

                case DataType.CSS    : return true;

                case DataType.CSV    : return true;

                case DataType.CUE    : return true;

                case DataType.DAT    : return true;

                case DataType.DOCX   : return true;

                case DataType.FS     : return true;

                case DataType.GEO    : return true;

                case DataType.GO     : return true;

                case DataType.GPX    : return true;

                case DataType.HTML   : return true;

                case DataType.ICS    : return true;

                case DataType.INF    : return true;

                case DataType.INI    : return true;

                case DataType.JS     : return true;

                case DataType.JSON   : return true;

                case DataType.KQL    : return true;

                case DataType.LOG    : return true;

                case DataType.LUA    : return true;

                case DataType.MOF    : return true;

                case DataType.NFO    : return true;

                case DataType.PDF    : return true;

                case DataType.PPTX   : return true;

                case DataType.PS1    : return true;

                case DataType.PY     : return true;

                case DataType.RAZOR  : return true;

                case DataType.REG    : return true;

                case DataType.RFC    : return true;

                case DataType.RS     : return true;

                case DataType.RTF    : return true;

                case DataType.SGML   : return true;

                case DataType.SH     : return true;

                case DataType.SLN    : return true;

                case DataType.SQL    : return true;

                case DataType.TS     : return true;

                case DataType.TXT    : return true;

                case DataType.VB     : return true;

                case DataType.VSD    : return true;

                case DataType.WAT    : return true;

                case DataType.XAML   : return true;

                case DataType.XHTML  : return true;

                case DataType.XLSX   : return true;

                case DataType.XML    : return true;

                case DataType.YAML   : return true;

                default : return false;
            }
        }

    }

}