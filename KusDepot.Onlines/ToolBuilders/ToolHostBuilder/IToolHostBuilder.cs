namespace KusDepot;

/**<include file='IToolHostBuilder.xml' path='IToolHostBuilder/interface[@name="IToolHostBuilder"]/main/*'/>*/
public interface IToolHostBuilder : IToolBuilder
{
    /**<include file='IToolHostBuilder.xml' path='IToolHostBuilder/interface[@name="IToolHostBuilder"]/method[@name="BuildHost"]/*'/>*/
    IToolHost BuildHost();

    /**<include file='IToolHostBuilder.xml' path='IToolHostBuilder/interface[@name="IToolHostBuilder"]/method[@name="UseConsoleLifetime"]/*'/>*/
    IToolHostBuilder UseConsoleLifetime();

    /**<include file='IToolHostBuilder.xml' path='IToolHostBuilder/interface[@name="IToolHostBuilder"]/method[@name="BuildHostAsync"]/*'/>*/
    Task<IToolHost> BuildHostAsync(CancellationToken cancel = default);

    /**<include file='IToolHostBuilder.xml' path='IToolHostBuilder/interface[@name="IToolHostBuilder"]/method[@name="BuildHostOpen"]/*'/>*/
    IToolHost BuildHost<TToolHost>() where TToolHost : notnull , IToolHost , new();

    /**<include file='IToolHostBuilder.xml' path='IToolHostBuilder/interface[@name="IToolHostBuilder"]/method[@name="BuildHostAsyncOpen"]/*'/>*/
    Task<IToolHost> BuildHostAsync<TToolHost>(CancellationToken cancel = default) where TToolHost : notnull , IToolHost , new();

    /**<include file='IToolHostBuilder.xml' path='IToolHostBuilder/interface[@name="IToolHostBuilder"]/method[@name="ConfigureToolHost"]/*'/>*/
    IToolHostBuilder ConfigureToolHost(Action<ToolBuilderContext,IToolHost> action);
}