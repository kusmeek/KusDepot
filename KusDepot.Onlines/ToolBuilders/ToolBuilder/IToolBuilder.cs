namespace KusDepot;

/**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/main/*'/>*/
public interface IToolBuilder
{
    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="Build"]/*'/>*/
    ITool Build();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="Seal"]/*'/>*/
    IToolBuilder Seal();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="AutoStart"]/*'/>*/
    IToolBuilder AutoStart();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="UseLogger"]/*'/>*/
    IToolBuilder UseLogger(ILoggerFactory logger);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/property[@name="ToolProperties"]/*'/>*/
    Dictionary<Object,Object> ToolProperties { get; }

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="UseSemanticKernel"]/*'/>*/
    IToolBuilder UseSemanticKernel(Kernel? kernel = null);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="BuildGeneric"]/*'/>*/
    TTool Build<TTool>() where TTool : notnull , ITool , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="BuildAsync"]/*'/>*/
    Task<ITool> BuildAsync(CancellationToken cancel = default);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="UseAccessManager"]/*'/>*/
    IToolBuilder UseAccessManager(IAccessManager accessmanager);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterCommandType"]/*'/>*/
    IToolBuilder RegisterCommand(String handle , Type commandtype);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterCommandInstance"]/*'/>*/
    IToolBuilder RegisterCommand(String handle , ICommand command);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="ConfigureTool"]/*'/>*/
    IToolBuilder ConfigureTool(Action<ToolBuilderContext,ITool> action);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="ConfigureServices"]/*'/>*/
    IToolBuilder ConfigureServices(Action<ToolBuilderContext,IServiceCollection> action);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="ConfigureToolConfiguration"]/*'/>*/
    IToolBuilder ConfigureToolConfiguration(Action<ToolBuilderContext,IConfigurationBuilder> action);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterCommandGenericType"]/*'/>*/
    IToolBuilder RegisterCommand<TCommand>(String handle) where TCommand : notnull , ICommand , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="UseAccessManagerOpen"]/*'/>*/
    IToolBuilder UseAccessManager<TAccessManager>() where TAccessManager : notnull , IAccessManager , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="BuildAsyncGeneric"]/*'/>*/
    Task<TTool> BuildAsync<TTool>(CancellationToken cancel = default) where TTool : notnull , ITool , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterToolType"]/*'/>*/
    IToolBuilder RegisterTool<TTool>() where TTool : class , ITool , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="UseServiceProvider"]/*'/>*/
    IToolBuilder UseServiceProvider(IServiceProvider? serviceprovider);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterToolInstance"]/*'/>*/
    IToolBuilder RegisterTool(ITool tool);
}