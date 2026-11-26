namespace KusDepot.Builders;

/**<include file='ToolAspireHostFactory.xml' path='ToolAspireHostFactory/delegate[@name="ToolAspireHostFactory"]/main/*'/>*/
public delegate TToolAspireHost ToolAspireHostFactory<TToolAspireHost>(
    IAccessManager? accessmanager,
    ToolServiceProvider? services,
    IConfiguration? configuration,
    ILoggerFactory? logger,
    IHost? aspireapplication) where TToolAspireHost : IToolAspireHost;