namespace KusDepot.Builders;

/**<include file='ToolWebHostFactory.xml' path='ToolWebHostFactory/delegate[@name="ToolWebHostFactory"]/main/*'/>*/
public delegate TToolWebHost ToolWebHostFactory<TToolWebHost>(
    IAccessManager? accessmanager,
    ToolServiceProvider? services,
    IConfiguration? configuration,
    Guid? lifeid,
    IToolHostLifetime? lifetime,
    ILoggerFactory? logger,
    ICollection<String>? urls,
    IHost? webapplication) where TToolWebHost : IToolWebHost;