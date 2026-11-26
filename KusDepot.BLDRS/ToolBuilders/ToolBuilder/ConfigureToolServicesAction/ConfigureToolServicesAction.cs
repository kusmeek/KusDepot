namespace KusDepot.Builders;

/**<include file='ConfigureToolServicesAction.xml' path='ConfigureToolServicesAction/class[@name="ConfigureToolServicesAction"]/main/*'/>*/
public sealed record ConfigureToolServicesAction
{
    /**<include file='ConfigureToolServicesAction.xml' path='ConfigureToolServicesAction/class[@name="ConfigureToolServicesAction"]/property[@name="Action"]/*'/>*/
    public Action<ToolBuilderContext,IServiceCollection>? Action { get; }

    /**<include file='ConfigureToolServicesAction.xml' path='ConfigureToolServicesAction/class[@name="ConfigureToolServicesAction"]/property[@name="AsyncAction"]/*'/>*/
    public Func<ToolBuilderContext,IServiceCollection,CancellationToken,Task>? AsyncAction { get; }

    /**<include file='ConfigureToolServicesAction.xml' path='ConfigureToolServicesAction/class[@name="ConfigureToolServicesAction"]/constructor[@name="ActionConstructor"]/*'/>*/
    public ConfigureToolServicesAction(Action<ToolBuilderContext,IServiceCollection> action)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /**<include file='ConfigureToolServicesAction.xml' path='ConfigureToolServicesAction/class[@name="ConfigureToolServicesAction"]/constructor[@name="AsyncActionConstructor"]/*'/>*/
    public ConfigureToolServicesAction(Func<ToolBuilderContext,IServiceCollection,CancellationToken,Task> asyncaction)
    {
        AsyncAction = asyncaction ?? throw new ArgumentNullException(nameof(asyncaction));
    }

    /**<include file='ConfigureToolServicesAction.xml' path='ConfigureToolServicesAction/class[@name="ConfigureToolServicesAction"]/method[@name="Execute"]/*'/>*/
    public void Execute(ToolBuilderContext context , IServiceCollection services)
    {
        if(Action is not null) { Action(context,services); return; }

        AsyncAction!(context,services,CancellationToken.None).GetAwaiter().GetResult();
    }

    /**<include file='ConfigureToolServicesAction.xml' path='ConfigureToolServicesAction/class[@name="ConfigureToolServicesAction"]/method[@name="ExecuteAsync"]/*'/>*/
    public Task ExecuteAsync(ToolBuilderContext context , IServiceCollection services , CancellationToken cancel = default)
    {
        if(Action is not null)
        {
            Action(context,services); return Task.CompletedTask;
        }

        return AsyncAction!(context,services,cancel);
    }
}
