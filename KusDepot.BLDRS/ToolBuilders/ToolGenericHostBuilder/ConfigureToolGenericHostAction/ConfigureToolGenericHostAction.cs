namespace KusDepot.Builders;

/**<include file='ConfigureToolGenericHostAction.xml' path='ConfigureToolGenericHostAction/class[@name="ConfigureToolGenericHostAction"]/main/*'/>*/
public sealed record ConfigureToolGenericHostAction
{
    /**<include file='ConfigureToolGenericHostAction.xml' path='ConfigureToolGenericHostAction/class[@name="ConfigureToolGenericHostAction"]/property[@name="Action"]/*'/>*/
    public Action<ToolBuilderContext,IToolGenericHost>? Action { get; }

    /**<include file='ConfigureToolGenericHostAction.xml' path='ConfigureToolGenericHostAction/class[@name="ConfigureToolGenericHostAction"]/property[@name="AsyncAction"]/*'/>*/
    public Func<ToolBuilderContext,IToolGenericHost,CancellationToken,Task>? AsyncAction { get; }

    /**<include file='ConfigureToolGenericHostAction.xml' path='ConfigureToolGenericHostAction/class[@name="ConfigureToolGenericHostAction"]/constructor[@name="ActionConstructor"]/*'/>*/
    public ConfigureToolGenericHostAction(Action<ToolBuilderContext,IToolGenericHost> action)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /**<include file='ConfigureToolGenericHostAction.xml' path='ConfigureToolGenericHostAction/class[@name="ConfigureToolGenericHostAction"]/constructor[@name="AsyncActionConstructor"]/*'/>*/
    public ConfigureToolGenericHostAction(Func<ToolBuilderContext,IToolGenericHost,CancellationToken,Task> asyncaction)
    {
        AsyncAction = asyncaction ?? throw new ArgumentNullException(nameof(asyncaction));
    }

    /**<include file='ConfigureToolGenericHostAction.xml' path='ConfigureToolGenericHostAction/class[@name="ConfigureToolGenericHostAction"]/method[@name="Execute"]/*'/>*/
    public void Execute(ToolBuilderContext context , IToolGenericHost toolgenerichost)
    {
        if(Action is not null) { Action(context,toolgenerichost); return; }

        AsyncAction!(context,toolgenerichost,CancellationToken.None).GetAwaiter().GetResult();
    }

    /**<include file='ConfigureToolGenericHostAction.xml' path='ConfigureToolGenericHostAction/class[@name="ConfigureToolGenericHostAction"]/method[@name="ExecuteAsync"]/*'/>*/
    public Task ExecuteAsync(ToolBuilderContext context , IToolGenericHost toolgenerichost , CancellationToken cancel = default)
    {
        if(Action is not null)
        {
            Action(context,toolgenerichost); return Task.CompletedTask;
        }

        return AsyncAction!(context,toolgenerichost,cancel);
    }
}
