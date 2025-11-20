namespace KusDepot;

/**<include file='IToolWebHost.xml' path='IToolWebHost/interface[@name="IToolWebHost"]/main/*'/>*/
public interface IToolWebHost : IToolHost
{
    /**<include file='IToolWebHost.xml' path='IToolWebHost/interface[@name="IToolWebHost"]/property[@name="Urls"]/*'/>*/
    ICollection<String>? Urls { get; }

    /**<include file='IToolWebHost.xml' path='IToolWebHost/interface[@name="IToolWebHost"]/method[@name="GetManagedApplication"]/*'/>*/
    IHost? GetManagedApplication(AccessKey? key = null);
}