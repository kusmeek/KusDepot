namespace KusDepot.Security;

/**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "ManagementRequest" , Namespace = "KusDepot.Security")]
[Alias("KusDepot.Security.ManagementRequest")] [GenerateSerializer] [Immutable]

public sealed record class ManagementRequest : AccessRequest , IParsable<ManagementRequest>
{
    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public ManagementRequest() {}

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/constructor[@name="Constructor"]/*'/>*/
    public ManagementRequest(String key) { Data = key; }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/constructor[@name="ConstructorKey"]/*'/>*/
    public ManagementRequest(ManagementKey key) { Data = key.ToString(); }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(ManagementRequest? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="IParsable{ManagementRequest}.Parse"]/*'/>*/
    static ManagementRequest IParsable<ManagementRequest>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="Parse"]/*'/>*/
    public static new ManagementRequest? Parse(String input , IFormatProvider? format = null)
    {
        return AccessRequest.Parse(input,format) as ManagementRequest;
    }

    /**<include file='ManagementRequest.xml' path='ManagementRequest/class[@name="ManagementRequest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out ManagementRequest request)
    {
        return AccessRequest.TryParse(input,format,out request);
    }
}