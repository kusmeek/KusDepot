namespace KusDepot;

/**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/main/*'/>*/
public interface IToolAspireHostBuilder : IToolBuilder
{
    /**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/method[@name="LinkLifetime"]/*'/>*/
    IToolAspireHostBuilder LinkLifetime();

    /**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/method[@name="UseConsoleLifetime"]/*'/>*/
    IToolAspireHostBuilder UseConsoleLifetime();

    /**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/property[@name="Builder"]/*'/>*/
    IDistributedApplicationBuilder Builder {get;}

    /**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/method[@name="ConnectWithApplication"]/*'/>*/
    IToolAspireHostBuilder ConnectWithApplication();

    /**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/method[@name="UseBuilderArguments"]/*'/>*/
    IToolAspireHostBuilder UseBuilderArguments(String[]? args = null);

    /**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/method[@name="BuildAspireHost"]/*'/>*/
    IToolAspireHost BuildAspireHost(DistributedApplication? app = null);

    /**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/method[@name="UseBuilderOptions"]/*'/>*/
    IToolAspireHostBuilder UseBuilderOptions(DistributedApplicationOptions? options = null);

    /**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/method[@name="ConfigureToolAspireHost"]/*'/>*/
    IToolAspireHostBuilder ConfigureToolAspireHost(Action<ToolBuilderContext,IToolAspireHost> action);

    /**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/method[@name="BuildAspireHostAsync"]/*'/>*/
    Task<IToolAspireHost> BuildAspireHostAsync(DistributedApplication? app = null , CancellationToken cancel = default);

    /**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/method[@name="BuildAspireHostOpen"]/*'/>*/
    IToolAspireHost BuildAspireHost<TToolAspireHost>(DistributedApplication? app = null) where TToolAspireHost : notnull , IToolAspireHost , new();

    /**<include file='IToolAspireHostBuilder.xml' path='IToolAspireHostBuilder/interface[@name="IToolAspireHostBuilder"]/method[@name="BuildAspireHostAsyncOpen"]/*'/>*/
    Task<IToolAspireHost> BuildAspireHostAsync<TToolAspireHost>(DistributedApplication? app = null , CancellationToken cancel = default) where TToolAspireHost : notnull , IToolAspireHost , new();
}