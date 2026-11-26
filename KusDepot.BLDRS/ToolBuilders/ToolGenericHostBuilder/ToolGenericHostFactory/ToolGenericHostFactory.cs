namespace KusDepot.Builders;

/**<include file='ToolGenericHostFactory.xml' path='ToolGenericHostFactory/delegate[@name="ToolGenericHostFactory"]/main/*'/>*/
public delegate TToolGenericHost ToolGenericHostFactory<TToolGenericHost>(
    IAccessManager? accessmanager,
    ToolServiceProvider? services,
    IConfiguration? configuration,
    IHost? host,
    ILoggerFactory? logger) where TToolGenericHost : IToolGenericHost;