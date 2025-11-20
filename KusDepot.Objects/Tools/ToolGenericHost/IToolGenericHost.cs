namespace KusDepot;

/**<include file='IToolGenericHost.xml' path='IToolGenericHost/interface[@name="IToolGenericHost"]/main/*'/>*/
public interface IToolGenericHost : IToolHost
{
    /**<include file='IToolGenericHost.xml' path='IToolGenericHost/interface[@name="IToolGenericHost"]/method[@name="GetManagedApplication"]/*'/>*/
    IHost? GetManagedApplication(AccessKey? key = null);
}