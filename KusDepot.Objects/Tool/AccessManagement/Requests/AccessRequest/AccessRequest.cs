namespace KusDepot.Security;

/**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/main/*'/>*/
[GenerateSerializer] [Immutable]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[Alias("KusDepot.Security.AccessRequest")] [KnownType("GetKnownTypes")]
[DataContract(Name = "AccessRequest" , Namespace = "KusDepot.Security")]
[JsonDerivedType(typeof(HostRequest),typeDiscriminator:nameof(HostRequest))]
[JsonDerivedType(typeof(ServiceRequest),typeDiscriminator:nameof(ServiceRequest))]
[JsonDerivedType(typeof(StandardRequest),typeDiscriminator:nameof(StandardRequest))]
[JsonDerivedType(typeof(ManagementRequest),typeDiscriminator:nameof(ManagementRequest))]

public abstract record class AccessRequest : IEquatable<AccessRequest>
{
    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/property[@name="Data"]/*'/>*/
    [JsonPropertyName("Data")] [JsonRequired] [NotNull] [Id(0)]
    [DataMember(Name = "Data" , EmitDefaultValue = true , IsRequired = true)]
    public String? Data { get; init; }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="Equals"]/*'/>*/
    public virtual Boolean Equals(AccessRequest? other)
    {
        try
        {
            if(ReferenceEquals(this,other)) { return true; }

            if(other is null || Equals(this.GetType(),other.GetType()) is false) { return false; }

            return (this.Data ?? String.Empty).AsSpan().SequenceEqual((other.Data ?? String.Empty).AsSpan());
        }
        catch { return false; }
    }

    ///<inheritdoc/>
    public override Int32 GetHashCode()
    {
        var _ = new HashCode(); _.AddBytes(this.Data.ToByteArrayFromUTF16String()); return _.ToHashCode();
    }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="GetKnownTypes"]/*'/>*/
    public static IEnumerable<Type> GetKnownTypes() => GetSecurityKnownTypes();

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="ParseAny"]/*'/>*/
    public static AccessRequest? Parse(String input , IFormatProvider? format = null)
    {
        try { return JsonUtility.Parse<AccessRequest>(input); }

        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="Parse"]/*'/>*/
    public static TResult? Parse<TResult>(String input , IFormatProvider? format = null) where TResult : AccessRequest
    {
        try { return JsonUtility.Parse<AccessRequest>(input) as TResult; }

        catch ( Exception _ ) { if(NoExceptions) { return null; } KusDepotLog.Error(_,ParseFail); throw; }
    }

    ///<inheritdoc/>
    public sealed override String ToString()
    {
        try { return JsonUtility.ToJsonString<AccessRequest>(this); }

        catch ( Exception _ ) { if(NoExceptions) { return String.Empty; } KusDepotLog.Error(_,ToStringFail); throw; }
    }

    /**<include file='AccessRequest.xml' path='AccessRequest/class[@name="AccessRequest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse<TResult>(String? input , IFormatProvider? format , [MaybeNullWhen(false)] out TResult result) where TResult : AccessRequest
    {
        try { result = Parse<TResult>(input!,format); return result is not null; }

        catch { result = null; return false; }
    }
}