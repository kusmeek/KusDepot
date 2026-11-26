namespace KusDepot;

/**<include file='IToolHost.xml' path='IToolHost/interface[@name="IToolHost"]/main/*'/>*/
public interface IToolHost : IHost , ITool
{
    /**<include file='IToolHost.xml' path='IToolHost/interface[@name="IToolHost"]/method[@name="GetServices"]/*'/>*/
    IServiceProvider? GetServices(AccessKey? key);

    /**<include file='IToolHost.xml' path='IToolHost/interface[@name="IToolHost"]/method[@name="Run"]/*'/>*/
    Boolean Run(AccessKey? key = null , TimeSpan? timeout = null);

    /**<include file='IToolHost.xml' path='IToolHost/interface[@name="IToolHost"]/method[@name="RunAsync"]/*'/>*/
    Task<Boolean> RunAsync(AccessKey? key = null , TimeSpan? timeout = null , CancellationToken cancel = default);
}