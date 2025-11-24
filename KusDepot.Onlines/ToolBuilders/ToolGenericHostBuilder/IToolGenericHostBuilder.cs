namespace KusDepot;

/**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/main/*'/>*/
public interface IToolGenericHostBuilder : IToolBuilder
{
    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/property[@name="HostBuilder"]/*'/>*/
    IHostBuilder HostBuilder {get;}

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/property[@name="Builder"]/*'/>*/
    HostApplicationBuilder Builder {get;}

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="LinkLifetime"]/*'/>*/
    IToolGenericHostBuilder LinkLifetime();

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="UseConsoleLifetime"]/*'/>*/
    IToolGenericHostBuilder UseConsoleLifetime();

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="ConnectWithApplication"]/*'/>*/
    IToolGenericHostBuilder ConnectWithApplication();

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="BuildGenericHost"]/*'/>*/
    IToolGenericHost BuildGenericHost(IHost? host = null);

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="UseFullBuilderArgs"]/*'/>*/
    IToolGenericHostBuilder UseFullBuilder(String[]? args = null);

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="UseHostBuilder"]/*'/>*/
    IToolGenericHostBuilder UseHostBuilder(String[]? args = null);

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="UseOrleans"]/*'/>*/
    IToolGenericHostBuilder UseOrleans(Action<ISiloBuilder> configureorleans);

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="UseMcpServerDefaults"]/*'/>*/
    IToolGenericHostBuilder UseMcpServerDefaults(Assembly? toolassembly = null);

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="UseFullBuilder"]/*'/>*/
    IToolGenericHostBuilder UseFullBuilder(HostApplicationBuilderSettings? settings = null);

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="UseBuilderSettings"]/*'/>*/
    IToolGenericHostBuilder UseBuilderSettings(HostApplicationBuilderSettings? settings = null);

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="ConfigureToolGenericHost"]/*'/>*/
    IToolGenericHostBuilder ConfigureToolGenericHost(Action<ToolBuilderContext,IToolGenericHost> action);

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="BuildGenericHostAsync"]/*'/>*/
    Task<IToolGenericHost> BuildGenericHostAsync(IHost? host = null , CancellationToken cancel = default);

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="BuildGenericHostOpen"]/*'/>*/
    IToolGenericHost BuildGenericHost<TToolGenericHost>(IHost? host = null) where TToolGenericHost : notnull , IToolGenericHost , new();

    /**<include file='IToolGenericHostBuilder.xml' path='IToolGenericHostBuilder/interface[@name="IToolGenericHostBuilder"]/method[@name="BuildGenericHostAsyncOpen"]/*'/>*/
    Task<IToolGenericHost> BuildGenericHostAsync<TToolGenericHost>(IHost? host = null , CancellationToken cancel = default) where TToolGenericHost : notnull , IToolGenericHost , new();
}