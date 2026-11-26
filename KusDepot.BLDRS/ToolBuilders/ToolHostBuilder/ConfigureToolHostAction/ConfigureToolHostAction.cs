namespace KusDepot.Builders;

/**<include file='ConfigureToolHostAction.xml' path='ConfigureToolHostAction/class[@name="ConfigureToolHostAction"]/main/*'/>*/
public sealed record ConfigureToolHostAction
{
    /**<include file='ConfigureToolHostAction.xml' path='ConfigureToolHostAction/class[@name="ConfigureToolHostAction"]/property[@name="Action"]/*'/>*/
    public Action<ToolBuilderContext,IToolHost>? Action { get; }

    /**<include file='ConfigureToolHostAction.xml' path='ConfigureToolHostAction/class[@name="ConfigureToolHostAction"]/property[@name="AsyncAction"]/*'/>*/
    public Func<ToolBuilderContext,IToolHost,CancellationToken,Task>? AsyncAction { get; }

    /**<include file='ConfigureToolHostAction.xml' path='ConfigureToolHostAction/class[@name="ConfigureToolHostAction"]/constructor[@name="ActionConstructor"]/*'/>*/
    public ConfigureToolHostAction(Action<ToolBuilderContext,IToolHost> action)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /**<include file='ConfigureToolHostAction.xml' path='ConfigureToolHostAction/class[@name="ConfigureToolHostAction"]/constructor[@name="AsyncActionConstructor"]/*'/>*/
    public ConfigureToolHostAction(Func<ToolBuilderContext,IToolHost,CancellationToken,Task> asyncaction)
    {
        AsyncAction = asyncaction ?? throw new ArgumentNullException(nameof(asyncaction));
    }

    /**<include file='ConfigureToolHostAction.xml' path='ConfigureToolHostAction/class[@name="ConfigureToolHostAction"]/method[@name="Execute"]/*'/>*/
    public void Execute(ToolBuilderContext context , IToolHost toolhost)
    {
        if(Action is not null) { Action(context,toolhost); return; }

        AsyncAction!(context,toolhost,CancellationToken.None).GetAwaiter().GetResult();
    }

    /**<include file='ConfigureToolHostAction.xml' path='ConfigureToolHostAction/class[@name="ConfigureToolHostAction"]/method[@name="ExecuteAsync"]/*'/>*/
    public Task ExecuteAsync(ToolBuilderContext context , IToolHost toolhost , CancellationToken cancel = default)
    {
        if(Action is not null)
        {
            Action(context,toolhost); return Task.CompletedTask;
        }

        return AsyncAction!(context,toolhost,cancel);
    }
}
