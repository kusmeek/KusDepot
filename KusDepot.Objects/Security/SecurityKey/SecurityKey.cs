namespace KusDepot.Security;

/**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/main/*'/>*/
[KnownType("GetKnownTypes")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
[GenerateSerializer] [Alias("KusDepot.Security.SecurityKey")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "SecurityKey" , Namespace = "KusDepot.Security")]
[JsonDerivedType(typeof(HostKey),typeDiscriminator:nameof(HostKey))]
[JsonDerivedType(typeof(OwnerKey),typeDiscriminator:nameof(OwnerKey))]
[JsonDerivedType(typeof(TokenKey),typeDiscriminator:nameof(TokenKey))]
[JsonDerivedType(typeof(ClientKey),typeDiscriminator:nameof(ClientKey))]
[JsonDerivedType(typeof(MyHostKey),typeDiscriminator:nameof(MyHostKey))]
[JsonDerivedType(typeof(ServiceKey),typeDiscriminator:nameof(ServiceKey))]
[JsonDerivedType(typeof(CommandKey),typeDiscriminator:nameof(CommandKey))]
[JsonDerivedType(typeof(ManagerKey),typeDiscriminator:nameof(ManagerKey))]
[JsonDerivedType(typeof(ExecutiveKey),typeDiscriminator:nameof(ExecutiveKey))]

public abstract record class SecurityKey : ICloneable , IEquatable<SecurityKey> , IParsable<SecurityKey>
{
    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/property[@name="ID"]/*'/>*/
    [JsonPropertyName("ID")] [JsonRequired] [NotNull] [Id(0)]
    [DataMember(Name = "ID" , EmitDefaultValue = true , IsRequired = true)]
    public Guid? ID { get; init; } = Guid.NewGuid();

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/property[@name="Key"]/*'/>*/
    [Description(SecurityKeyDescription)]
    [JsonPropertyName("Key")] [JsonRequired] [NotNull]
    [DataMember(Name = "Key" , EmitDefaultValue = true , IsRequired = true)]
    public Byte[]? Key
    {
        get { return this.key?.CloneByteArray()!; } init { this.key ??= value?.CloneByteArray(); }
    }

    [JsonIgnore] [IgnoreDataMember] [Id(1)]
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
    Object ICloneable.Clone() { return this.Copy()!; }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="Copy"]/*'/>*/
    public virtual SecurityKey? Copy() { return Parse(this.ToString()); }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="CopyOpen"]/*'/>*/
    public TResult? Copy<TResult>() where TResult : SecurityKey
    {
        try { return Parse<TResult>(this.ToString(),null); }

        catch ( Exception _ ) { KusDepotLog.Error(_,CloneFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="EqualsObject"]/*'/>*/
    public virtual Boolean Equals(SecurityKey? other)
    {
        try
        {
            if(ReferenceEquals(this,other)) { return true; } if(other is null || Equals(this.GetType(),other.GetType()) is false) { return false; }

            return Equals(this.ID,other.ID) && this.key.AsSpan().SequenceEqual(other.key.AsSpan());
        }
        catch { return false; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="FromFile"]/*'/>*/
    public static TResult? FromFile<TResult>(String path) where TResult : SecurityKey
    {
        try { return JsonUtility.FromFile<SecurityKey>(path) as TResult ?? JsonUtility.FromFile<TResult>(path); }

        catch { return JsonUtility.FromFile<TResult>(path); }
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
        try { return JsonUtility.Parse<SecurityKey>(input)!; }

        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); if(NoExceptions) { return null!; } throw; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="Parse"]/*'/>*/
    public static TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : SecurityKey
    {
        try
        {
            try { return JsonUtility.Parse<SecurityKey>(input) as TResult ?? JsonUtility.Parse<TResult>(input); }

            catch { return JsonUtility.Parse<TResult>(input); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="Deserialize"]/*'/>*/
    public static TResult? Deserialize<TResult>(Byte[] input , IFormatProvider? format = null) where TResult : SecurityKey
    {
        try
        {
            try { return JsonUtility.Deserialize<SecurityKey>(input) as TResult ?? JsonUtility.Deserialize<TResult>(input); }

            catch { return JsonUtility.Deserialize<TResult>(input); }
        }
        catch ( Exception _ ) { KusDepotLog.Error(_,ParseFail); if(NoExceptions) { return null; } throw; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="ToFile"]/*'/>*/
    public virtual Boolean ToFile(String path)
    {
        try { return JsonUtility.ToFile<SecurityKey>(path,this); }

        catch ( NotSupportedException ) { return JsonUtility.ToFile(path,this,this.GetType()); }
    }

    ///<inheritdoc/>
    public sealed override String ToString()
    {
        try { return JsonUtility.ToJsonString<SecurityKey>(this); }

        catch ( Exception _ ) { KusDepotLog.Error(_,ToStringFail); if(NoExceptions) { return String.Empty; } throw; }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="Serialize"]/*'/>*/
    public Byte[] Serialize()
    {
        try { return JsonUtility.Serialize<SecurityKey>(this); }

        catch ( NotSupportedException ) { return JsonUtility.Serialize(this,this.GetType()); }
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out SecurityKey result)
    {
        result = Parse(input!,format); return result is not null;
    }

    /**<include file='SecurityKey.xml' path='SecurityKey/class[@name="SecurityKey"]/method[@name="TryParseOpen"]/*'/>*/
    public static Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : SecurityKey
    {
        try { result = Parse<TResult>(input!,format); return result is not null; }

        catch { result = null; return false; }
    }
}