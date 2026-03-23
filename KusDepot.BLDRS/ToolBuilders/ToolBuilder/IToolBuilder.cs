namespace KusDepot.Builders;

/**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/main/*'/>*/
public interface IToolBuilder
{
    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="AutoStart"]/*'/>*/
    IToolBuilder AutoStart();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="Build"]/*'/>*/
    ITool Build();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="BuildAsync"]/*'/>*/
    Task<ITool> BuildAsync(CancellationToken cancel = default);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="BuildAsyncGeneric"]/*'/>*/
    Task<TTool> BuildAsync<TTool>(CancellationToken cancel = default) where TTool : notnull , ITool , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="BuildGeneric"]/*'/>*/
    TTool Build<TTool>() where TTool : notnull , ITool , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="ConfigureServices"]/*'/>*/
    IToolBuilder ConfigureServices(Action<ToolBuilderContext,IServiceCollection> action);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="ConfigureTool"]/*'/>*/
    IToolBuilder ConfigureTool(Action<ToolBuilderContext,ITool> action);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="ConfigureToolConfiguration"]/*'/>*/
    IToolBuilder ConfigureToolConfiguration(Action<ToolBuilderContext,IConfigurationBuilder> action);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="EnableAllCommands"]/*'/>*/
    IToolBuilder EnableAllCommands();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterCommandInstance"]/*'/>*/
    IToolBuilder RegisterCommand(String handle , ICommand command , ImmutableArray<Int32>? permissions = null);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterCommandFactory"]/*'/>*/
    IToolBuilder RegisterCommand(String handle , Func<ICommand?> factory , ImmutableArray<Int32>? permissions = null);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterCommandType"]/*'/>*/
    IToolBuilder RegisterCommand(String handle , Type commandtype , Object?[]? arguments = null , ImmutableArray<Int32>? permissions = null);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterCommandGenericType"]/*'/>*/
    IToolBuilder RegisterCommand<TCommand>(String handle , Object?[]? arguments = null , ImmutableArray<Int32>? permissions = null) where TCommand : notnull , ICommand , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterServiceInstance"]/*'/>*/
    IToolBuilder RegisterService(String name , IHostedService service);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterServiceFactory"]/*'/>*/
    IToolBuilder RegisterService(String name , Func<IHostedService?> factory);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterServiceType"]/*'/>*/
    IToolBuilder RegisterService(String name , Type servicetype);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterServiceGenericType"]/*'/>*/
    IToolBuilder RegisterService<TService>(String name) where TService : class , IHostedService , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterToolInstance"]/*'/>*/
    IToolBuilder RegisterTool(ITool tool , String? name = null , ImmutableArray<Int32>? permissions = null);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterToolFactory"]/*'/>*/
    IToolBuilder RegisterTool(Func<ITool?> factory , String? name = null , ImmutableArray<Int32>? permissions = null);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterToolType"]/*'/>*/
    IToolBuilder RegisterTool(Type tooltype , Object?[]? arguments = null , String? name = null , ImmutableArray<Int32>? permissions = null);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterToolTypeBulk"]/*'/>*/
    IToolBuilder RegisterTool<TTool>(Int32 count , Func<Int32,String> namefactory , Func<Int32,ImmutableArray<Int32>?>? permissionsfactory = null) where TTool : class , ITool , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="RegisterToolGenericType"]/*'/>*/
    IToolBuilder RegisterTool<TTool>(String? name = null , ImmutableArray<Int32>? permissions = null) where TTool : class , ITool , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="Seal"]/*'/>*/
    IToolBuilder Seal();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/property[@name="ToolProperties"]/*'/>*/
    Dictionary<Object,Object> ToolProperties { get; }

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="UseAccessManager"]/*'/>*/
    IToolBuilder UseAccessManager(IAccessManager accessmanager);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="UseAccessManagerOpen"]/*'/>*/
    IToolBuilder UseAccessManager<TAccessManager>() where TAccessManager : notnull , IAccessManager , new();

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="UseLogger"]/*'/>*/
    IToolBuilder UseLogger(ILoggerFactory logger);

    /**<include file='IToolBuilder.xml' path='IToolBuilder/interface[@name="IToolBuilder"]/method[@name="UseServiceProvider"]/*'/>*/
    IToolBuilder UseServiceProvider(IServiceProvider? serviceprovider);
}