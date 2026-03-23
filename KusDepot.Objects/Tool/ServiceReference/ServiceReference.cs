namespace KusDepot;

/**<include file='ServiceReference.xml' path='ServiceReference/class[@name="ServiceReference"]/main/*'/>*/
public class ServiceReference
{
    /**<include file='ServiceReference.xml' path='ServiceReference/class[@name="ServiceReference"]/field[@name="key"]/*'/>*/
    private SecurityKey? key;

    /**<include file='ServiceReference.xml' path='ServiceReference/class[@name="ServiceReference"]/field[@name="service"]/*'/>*/
    private IHostedService? service;

    /**<include file='ServiceReference.xml' path='ServiceReference/class[@name="ServiceReference"]/property[@name="Key"]/*'/>*/
    public SecurityKey? Key { get => key; set => key ??= value; }

    /**<include file='ServiceReference.xml' path='ServiceReference/class[@name="ServiceReference"]/property[@name="Service"]/*'/>*/
    public IHostedService? Service { get => service; set => service ??= value; }

    /**<include file='ServiceReference.xml' path='ServiceReference/class[@name="ServiceReference"]/constructor[@name="Constructor"]/*'/>*/
    public ServiceReference(IHostedService? service = null , SecurityKey? key = null)
    {
        this.service = service; this.key = key;
    }
}