namespace KusDepot;

/**<include file='IToolHost.xml' path='IToolHost/interface[@name="IToolHost"]/main/*'/>*/
public interface IToolHost : IHost , ITool
{
    /**<include file='IToolHost.xml' path='IToolHost/interface[@name="IToolHost"]/method[@name="Run"]/*'/>*/
    void Run();

    /**<include file='IToolHost.xml' path='IToolHost/interface[@name="IToolHost"]/property[@name="Lifetime"]/*'/>*/
    IToolHostLifetime? Lifetime { get; }

    /**<include file='IToolHost.xml' path='IToolHost/interface[@name="IToolHost"]/method[@name="GetServices"]/*'/>*/
    IServiceProvider? GetServices(AccessKey? key);

    /**<include file='IToolHost.xml' path='IToolHost/interface[@name="IToolHost"]/method[@name="RunAsync"]/*'/>*/
    Task RunAsync(CancellationToken cancel = default);
}