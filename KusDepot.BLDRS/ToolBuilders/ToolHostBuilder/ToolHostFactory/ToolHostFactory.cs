namespace KusDepot.Builders;

/**<include file='ToolHostFactory.xml' path='ToolHostFactory/delegate[@name="ToolHostFactory"]/main/*'/>*/
public delegate TToolHost ToolHostFactory<TToolHost>(
    IAccessManager? accessmanager,
    ToolServiceProvider? services,
    IConfiguration? configuration) where TToolHost : IToolHost;
