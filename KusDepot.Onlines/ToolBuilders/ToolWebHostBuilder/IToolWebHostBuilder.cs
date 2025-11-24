namespace KusDepot;

/**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/main/*'/>*/
public interface IToolWebHostBuilder : IToolBuilder
{
    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="LinkLifetime"]/*'/>*/
    IToolWebHostBuilder LinkLifetime();

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/property[@name="Builder"]/*'/>*/
    WebApplicationBuilder Builder {get;}

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="UseConsoleLifetime"]/*'/>*/
    IToolWebHostBuilder UseConsoleLifetime();

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="UseRandomLocalPorts"]/*'/>*/
    IToolWebHostBuilder UseRandomLocalPorts();

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="ConnectWithApplication"]/*'/>*/
    IToolWebHostBuilder ConnectWithApplication();

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="BuildWebHost"]/*'/>*/
    IToolWebHost BuildWebHost(WebApplication? app = null);

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="UseFullBuilderArgs"]/*'/>*/
    IToolWebHostBuilder UseFullBuilder(String[]? args = null);

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="UseSlimBuilderArgs"]/*'/>*/
    IToolWebHostBuilder UseSlimBuilder(String[]? args = null);

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="UseFullBuilder"]/*'/>*/
    IToolWebHostBuilder UseFullBuilder(WebApplicationOptions? options = null);

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="UseSlimBuilder"]/*'/>*/
    IToolWebHostBuilder UseSlimBuilder(WebApplicationOptions? options = null);

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="ConfigureWebApplication"]/*'/>*/
    IToolWebHostBuilder ConfigureWebApplication(Action<WebApplication> action);

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="UseBuilderOptions"]/*'/>*/
    IToolWebHostBuilder UseBuilderOptions(WebApplicationOptions? options = null);

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="ConfigureToolWebHost"]/*'/>*/
    IToolWebHostBuilder ConfigureToolWebHost(Action<ToolBuilderContext,IToolWebHost> action);

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="BuildWebHostAsync"]/*'/>*/
    Task<IToolWebHost> BuildWebHostAsync(WebApplication? app = null , CancellationToken cancel = default);

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="UseMcpServer"]/*'/>*/
    IToolWebHostBuilder UseMcpServer(ServerCapabilities? capabilities = null , String? instructions = null);

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="BuildWebHostOpen"]/*'/>*/
    IToolWebHost BuildWebHost<TToolWebHost>(WebApplication? app = null) where TToolWebHost : notnull , IToolWebHost , new();

    /**<include file='IToolWebHostBuilder.xml' path='IToolWebHostBuilder/interface[@name="IToolWebHostBuilder"]/method[@name="BuildWebHostAsyncOpen"]/*'/>*/
    Task<IToolWebHost> BuildWebHostAsync<TToolWebHost>(WebApplication? app = null , CancellationToken cancel = default) where TToolWebHost : notnull , IToolWebHost , new();
}