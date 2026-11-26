namespace KusDepot.Builders;

/**<include file='ToolFactory.xml' path='ToolFactory/delegate[@name="ToolFactory"]/main/*'/>*/
public delegate TTool ToolFactory<TTool>(
    IAccessManager? accessmanager,
    ToolServiceProvider? services,
    IConfiguration? configuration) where TTool : ITool;