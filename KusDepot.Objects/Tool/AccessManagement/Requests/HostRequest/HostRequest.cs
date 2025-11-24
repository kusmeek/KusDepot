namespace KusDepot;

/**<include file='HostRequest.xml' path='HostRequest/class[@name="HostRequest"]/main/*'/>*/
public class HostRequest : AccessRequest
{
    /**<include file='HostRequest.xml' path='HostRequest/class[@name="HostRequest"]/property[@name="Host"]/*'/>*/
    public ITool? Host { get; private set; }

    /**<include file='HostRequest.xml' path='HostRequest/class[@name="HostRequest"]/property[@name="External"]/*'/>*/
    public Boolean External { get; private set; } = false;

    /**<include file='HostRequest.xml' path='HostRequest/class[@name="HostRequest"]/constructor[@name="Constructor"]/*'/>*/
    public HostRequest(ITool? host , Boolean external = false) { Host = host; External = external; }
}