namespace KusDepot;

/**<include file='IToolAspireHost.xml' path='IToolAspireHost/interface[@name="IToolAspireHost"]/main/*'/>*/
public interface IToolAspireHost : IToolHost
{
    /**<include file='IToolAspireHost.xml' path='IToolAspireHost/interface[@name="IToolAspireHost"]/method[@name="GetManagedApplication"]/*'/>*/
    IHost? GetManagedApplication(AccessKey? key);
}