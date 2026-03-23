namespace KusDepot.Security;

/**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/main/*'/>*/
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
[DataContract(Name = "StandardRequest" , Namespace = "KusDepot.Security")]
[Alias("KusDepot.Security.StandardRequest")] [GenerateSerializer] [Immutable]

public sealed record class StandardRequest : AccessRequest , IParsable<StandardRequest>
{
    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public StandardRequest() {}

    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/constructor[@name="Constructor"]/*'/>*/
    public StandardRequest(String data) { Data = data; }

    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(StandardRequest? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }

    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/method[@name="IParsable{StandardRequest}.Parse"]/*'/>*/
    static StandardRequest IParsable<StandardRequest>.Parse(String input , IFormatProvider? format) { return Parse(input,null)!; }

    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/method[@name="Parse"]/*'/>*/
    public static new StandardRequest? Parse(String input , IFormatProvider? format = null)
    {
        return AccessRequest.Parse(input,format) as StandardRequest;
    }

    /**<include file='StandardRequest.xml' path='StandardRequest/class[@name="StandardRequest"]/method[@name="TryParse"]/*'/>*/
    public static Boolean TryParse([NotNullWhen(true)] String? input , IFormatProvider? format , [MaybeNullWhen(false)] out StandardRequest request)
    {
        return AccessRequest.TryParse(input,format,out request);
    }
}