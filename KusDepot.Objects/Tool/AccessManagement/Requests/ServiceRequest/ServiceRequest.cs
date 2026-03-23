namespace KusDepot.Security;

/**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ServiceRequest" , Namespace = "KusDepot.Security")]
[Alias("KusDepot.Security.ServiceRequest")] [GenerateSerializer] [Immutable]

public sealed record class ServiceRequest : AccessRequest , IParsable<ServiceRequest>
{
    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ServiceRequest() {}

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/property[@name="Tool"]/*'/>*/
    [IgnoreDataMember] [JsonIgnore]
    [field: NonSerialized] [NotNull]
    public ITool? Tool { get; private set; }

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/constructor[@name="Constructor"]/*'/>*/
    public ServiceRequest(ITool? tool , String data) { Data = data; Tool = tool; }

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/method[@name="IParsable{ServiceRequest}.Parse"]/*'/>*/
    static ServiceRequest IParsable<ServiceRequest>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/method[@name="Parse"]/*'/>*/
    public static new ServiceRequest? Parse(String input , IFormatProvider? format = null)
    {
        return AccessRequest.Parse(input,format) as ServiceRequest;
    }

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ServiceRequest request)
    {
        return AccessRequest.TryParse(input,format,out request);
    }

    /**<include file='ServiceRequest.xml' path='ServiceRequest/class[@name="ServiceRequest"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(ServiceRequest? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }
}