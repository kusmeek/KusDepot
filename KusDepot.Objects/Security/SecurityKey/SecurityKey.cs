namespace KusDepot;

/**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[GenerateSerializer] [Alias("SecurityKey")]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
[DataContract(Name = "SecurityKey" , Namespace = "KusDepot")]
public abstract class SecurityKey : ICloneable , IEquatable<SecurityKey> , IParsable<SecurityKey>
{
    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/property[@name="ID"]/*'/>*/
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)] [NotNull] [Id(0)]
    public Guid? ID { get; protected set; } = Guid.NewGuid();

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/property[@name="Key"]/*'/>*/
    [NotNull]
    public Byte[]? Key { get { return this.key?.CloneByteArray()!; } protected set { this.key ??= value; } }

    [DataMember(Name = "Key" , EmitDefaultValue = true , IsRequired = true)] [Id(1)]
    private Byte[]? key;

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/finalizer[@name="Finalizer"]/*'/>*/
    ~SecurityKey() { this.ClearKey(); }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="ClearKey"]/*'/>*/
    public Boolean ClearKey()
    {
        try { if(this.key is not null) { ZeroMemory(this.key); this.key = Array.Empty<Byte>(); } return true; }

        catch ( Exception _ ) { KusDepotLog.Error(_,ClearKeyFail); if(NoExceptions) { return false; } throw; }
    }

    ///<inheritdoc/>
    Object ICloneable.Clone() { return this.Clone()!; }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="Clone"]/*'/>*/
    public virtual SecurityKey? Clone() { return Parse(this.ToString()); }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="CloneOpen"]/*'/>*/
    public TResult? Clone<TResult>() where TResult : SecurityKey
    {
        try { return Parse<TResult>(this.ToString(),null); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CloneFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="EqualsObject"]/*'/>*/
    public override Boolean Equals(Object? other) { return this.Equals(other as SecurityKey); }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="IEquatable{SecurityKey}.Equals"]/*'/>*/
    public virtual Boolean Equals(SecurityKey? other)
    {
        try
        {
            if(ReferenceEquals(this,other)) { return true; } if(other is null || Equals(this.GetType(),other.GetType()) is false) { return false; }

            return Equals(this.ID,other.ID) && this.key.AsSpan().SequenceEqual(other.key.AsSpan());
        }
        catch { return false; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="GetDebuggerDisplay"]/*'/>*/
    protected String GetDebuggerDisplay() { return $"{this.GetType().Name} - [{this.ID}]"; }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { var _ = new HashCode(); _.Add(this.ID); _.AddBytes(this.key); return _.ToHashCode(); }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="GetKnownTypes"]/*'/>*/
    public static IEnumerable<Type> GetKnownTypes() => GetSecurityKnownTypes();

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="ParseAny"]/*'/>*/
    public static SecurityKey Parse(String input , IFormatProvider? format = null)
    {
        try
        {
            if(String.IsNullOrEmpty(input)) { return null!; } SecurityKey? k;

            try { k = ManagementKey.Parse(input,format); if(k is not null) { return k; } } catch { }

            try { k = AccessKey.Parse(input,format);     if(k is not null) { return k; } } catch { }

            return null!;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); if(NoExceptions) { return null!; } throw; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="Parse"]/*'/>*/
    public static TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : SecurityKey
    {
        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(new MemoryStream(input.ToByteArrayFromBase64()),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { return _2; }

            throw new FormatException();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="ToSecurityKeyWeb"]/*'/>*/
    public SecurityKeyWeb ToSecurityKeyWeb() { return new() { ID = this.ID , Key = this.key.CloneByteArray() }; }

    ///<inheritdoc/>
    public override String ToString()
    {
        try
        {
            MemoryStream _0 = new(); using XmlDictionaryWriter _1 = XmlDictionaryWriter.CreateBinaryWriter(_0);

            DataContractSerializer _2 = new(this.GetType(),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            _2.WriteObject(_1,this); _1.Flush(); _0.Seek(0,SeekOrigin.Begin);

            return _0.ToArray().ToBase64FromByteArray();
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); if(NoExceptions) { return String.Empty; } throw; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out SecurityKey result)
    {
        result = Parse(input!,format); return result is not null;
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="TryParseOpen"]/*'/>*/
    public static Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : SecurityKey
    {
        result = null; if(input is null) { return false; }

        try
        {
            using XmlDictionaryReader _0 = XmlDictionaryReader.CreateBinaryReader(input.ToByteArrayFromBase64(),XmlDictionaryReaderQuotas.Max);

            DataContractSerializer _1 = new(typeof(TResult),new DataContractSerializerSettings(){MaxItemsInObjectGraph = Int32.MaxValue});

            TResult? _2 = _1.ReadObject(_0) as TResult; if( _2 is not null ) { result = _2; return true; }

            return false;
        }
        catch ( Exception _ ) { if(NoExceptions) { return false; } KusDepotLog.Error(_,TryParseFail); throw; }
    }
}

/**<include file='SecurityKey.xml' path='SecurityKey/record[@name="SecurityKeyWeb"]/main/*'/>*/
public record SecurityKeyWeb
{
    /**<include file='SecurityKey.xml' path='SecurityKey/record[@name="SecurityKeyWeb"]/property[@name="ID"]/*'/>*/
    public Guid? ID { get; init; }

    /**<include file='SecurityKey.xml' path='SecurityKey/record[@name="SecurityKeyWeb"]/property[@name="Key"]/*'/>*/
    public Byte[]? Key { get; init; }

    /**<include file='SecurityKey.xml' path='SecurityKey/record[@name="SecurityKeyWeb"]/method[@name="ClearKey"]/*'/>*/
    public Boolean ClearKey()
    {
        try
        {
            if(this.Key is not null) { ZeroMemory(this.Key); return true; } return false;
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ClearKeyFail); if(NoExceptions) { return false; } throw; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/record[@name="SecurityKeyWeb"]/method[@name="ToSecurityKey"]/*'/>*/
    public virtual SecurityKey? ToSecurityKey() => null;

    ///<inheritdoc/>
    public override String ToString() { try { return JsonSerializer.Serialize(this); } catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); return String.Empty; } }

    /**<include file='SecurityKey.xml' path='SecurityKey/record[@name="SecurityKeyWeb"]/method[@name="Parse"]/*'/>*/
    public static SecurityKeyWeb? Parse(String input) { try { return String.IsNullOrEmpty(input) ? null : JsonSerializer.Deserialize<SecurityKeyWeb>(input); } catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); return null; } }
}