namespace KusDepot.Security;

/**<include file='HostRequest.xml' path='HostRequest/class[@name="HostRequest"]/main/*'/>*/
public sealed record class HostRequest : AccessRequest
{
    /**<include file='HostRequest.xml' path='HostRequest/class[@name="HostRequest"]/property[@name="Host"]/*'/>*/
    [JsonIgnore]
    public ITool? Host { get; private set; }

    /**<include file='HostRequest.xml' path='HostRequest/class[@name="HostRequest"]/property[@name="External"]/*'/>*/
    public Boolean External { get; private set; } = false;

    /**<include file='HostRequest.xml' path='HostRequest/class[@name="HostRequest"]/constructor[@name="ParameterlessConstructor"]/*'/>*/
    public HostRequest() {}

    /**<include file='HostRequest.xml' path='HostRequest/class[@name="HostRequest"]/constructor[@name="Constructor"]/*'/>*/
    public HostRequest(ITool? host , Boolean external = false) { Host = host; External = external; }

    /**<include file='HostRequest.xml' path='HostRequest/class[@name="HostRequest"]/method[@name="Equals"]/*'/>*/
    public Boolean Equals(HostRequest? other) { return base.Equals(other); }

    ///<inheritdoc/>
    public override Int32 GetHashCode() { return base.GetHashCode(); }
}