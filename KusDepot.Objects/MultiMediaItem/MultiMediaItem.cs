namespace KusDepot
{
    /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]'/> */
    [DataContract(Name = "MultiMediaItem" , Namespace = "KusDepot")]
    public class MultiMediaItem : DataItem , IComparable<MultiMediaItem> , IEquatable<MultiMediaItem>
    {
        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/property[@Name = "Artists"]'/> */
        [DataMember(Name = "Artists" , EmitDefaultValue = false , IsRequired = false)]
        public List<String>? Artists;

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/property[@Name = "Bitrate"]'/> */
        [DataMember(Name = "Bitrate" , EmitDefaultValue = false , IsRequired = false)]
        public Double? Bitrate;

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/content[@Name = "Content"]'/> */
        [DataMember(Name = "Content" , EmitDefaultValue = false , IsRequired = true)] 
        public Byte[]? Content;

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/property[@Name = "Duration"]'/> */
        [DataMember(Name = "Duration" , EmitDefaultValue = false , IsRequired = false)]
        public Decimal? Duration;

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/property[@Name = "Framerate"]'/> */
        [DataMember(Name = "Framerate" , EmitDefaultValue = false , IsRequired = false)]
        public Single? Framerate;

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/property[@Name = "Language"]'/> */
        [DataMember(Name = "Language" , EmitDefaultValue = false , IsRequired = false)]
        public String? Language;

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/property[@Name = "Source"]'/> */
        [IgnoreDataMember()]
        public Stream? Source;

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/property[@Name = "Title"]'/> */
        [DataMember(Name = "Title" , EmitDefaultValue = false , IsRequired = false)]
        public String? Title;

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/property[@Name = "Volume"]'/> */
        [DataMember(Name = "Volume" , EmitDefaultValue = false , IsRequired = false)]
        public Single? Volume;

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/constructor[@Name = "ParameterlessConstructor"]'/> */
        public MultiMediaItem() { this.Initialize(new Guid()); }

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/constructor[@Name = "Constructor"]'/> */
        public MultiMediaItem(Byte[]? content , List<String>? artists, Double? bitrate , Decimal? duration , Guid? id , Single? framerate , String? language , List<String>? notes , Stream? source , List<String>? tags , String? title ,String? type , Single? volume)
        {
            try
            {
                this.Initialize(id);
                this.Artists   = artists;
                this.Bitrate   = bitrate;
                this.Content   = content;
                this.Duration  = duration;
                this.Framerate = framerate;
                this.Language  = language;
                this.Notes     = notes;
                this.Source    = source;
                this.Tags      = tags;
                this.Title     = title;
                this.Type      = type;
                this.Volume    = volume;
            }
            catch ( Exception ie ) { throw new Exception("KusDepot.MultiMediaItem.Constructor",ie); }
        }

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/method[@Name = "IComparable{MultiMediaItem}.CompareTo"]'/> */
        public Int32 CompareTo(MultiMediaItem? other)
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
            catch ( Exception ie ) { throw new ArgumentException("KusDepot.MultiMediaItem.CompareTo - Please Check Argument",ie); }
        }

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/method[@Name = "EqualsObject"]'/> */
        public override Boolean Equals(Object? obj)
        {
            if(ReferenceEquals(this,obj)) { return true; }

            if(obj is null) { return false; }

            if( (obj as IEquatable<MultiMediaItem>) == null) { return false; }

            IEquatable<MultiMediaItem>? me = (IEquatable<MultiMediaItem>)obj;

            return me.Equals(obj);
        }

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/method[@Name = "IEquatable{MultiMediaItem}.Equals"]'/> */
        public Boolean Equals(MultiMediaItem? other)
        {
            if(ReferenceEquals(this,other)) { return true; }

            if(other is null) { return false; }

            if( (other as MultiMediaItem) == null) { return false; }

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

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/method[@Name = "GetHashCode"]'/> */
        public override Int32 GetHashCode()
        {
            return HashCode.Combine<DateTime?,Byte[]?,Guid?>(BornOn,Content,ID);
        }

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/method[@Name = "Initialize"]'/> */
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

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItem"]/method[@Name = "Validate"]'/> */
        public static Boolean Validate(MultiMediaItem? item)
        {
            if( item == null ) { return false; }

            try
            {
                MultiMediaItemValidator validator = new MultiMediaItemValidator();
                ValidationResult result = validator.Validate(item);
                return result.IsValid;
            }
            catch( Exception ) { return false; }
        }

    }

    /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItemValidator"]'/> */
    public class MultiMediaItemValidator : AbstractValidator<MultiMediaItem>
    {
        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItemValidator"]/constructor[@Name = "Constructor"]'/> */
        public MultiMediaItemValidator()
        {
            this.RuleFor( (item) => item.Content).NotNull().NotEmpty();
            this.RuleFor( (item) => item.Type).NotNull().NotEmpty().Must( (item , String) => { return PassThisTest(item , item.Type); } );
        }

        /** <include file = 'MultiMediaItem.xml' path = 'MultiMediaItem/class[@Name = "MultiMediaItemValidator"]/method[@Name = "PassThisTest"]'/> */
        internal static Boolean PassThisTest(MultiMediaItem? item , String? type)
        {
            switch (type)
            {
                case DataType.AVI  : return true;

                case DataType.BAR  : return true;

                case DataType.BMP  : return true;

                case DataType.FLAC : return true;

                case DataType.GIF  : return true;

                case DataType.IMAX : return true;

                case DataType.JPEG : return true;

                case DataType.MIDI : return true;

                case DataType.MOV  : return true;

                case DataType.M4A  : return true;

                case DataType.MP3  : return true;

                case DataType.MP4  : return true;

                case DataType.MPEG : return true;

                case DataType.OGA  : return true;

                case DataType.OGV  : return true;

                case DataType.OGX  : return true;

                case DataType.PNG  : return true;

                case DataType.QRC  : return true;

                case DataType.SVG  : return true;

                case DataType.TIFF : return true;

                case DataType.WAV  : return true;

                case DataType.WMA  : return true;

                case DataType.WEBA : return true;

                case DataType.WEBM : return true;

                case DataType.WEBP : return true;

                case DataType.WMV  : return true;

                default : return false;
            }
        }

    }

}