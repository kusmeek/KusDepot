namespace KusDepot.Builders;

/**<include file='ConfigureToolAction.xml' path='ConfigureToolAction/class[@name="ConfigureToolAction"]/main/*'/>*/
public sealed record ConfigureToolAction
{
    /**<include file='ConfigureToolAction.xml' path='ConfigureToolAction/class[@name="ConfigureToolAction"]/property[@name="Action"]/*'/>*/
    public Action<ToolBuilderContext,ITool>? Action { get; }

    /**<include file='ConfigureToolAction.xml' path='ConfigureToolAction/class[@name="ConfigureToolAction"]/property[@name="AsyncAction"]/*'/>*/
    public Func<ToolBuilderContext,ITool,CancellationToken,Task>? AsyncAction { get; }

    /**<include file='ConfigureToolAction.xml' path='ConfigureToolAction/class[@name="ConfigureToolAction"]/constructor[@name="ActionConstructor"]/*'/>*/
    public ConfigureToolAction(Action<ToolBuilderContext,ITool> action)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /**<include file='ConfigureToolAction.xml' path='ConfigureToolAction/class[@name="ConfigureToolAction"]/constructor[@name="AsyncActionConstructor"]/*'/>*/
    public ConfigureToolAction(Func<ToolBuilderContext,ITool,CancellationToken,Task> asyncaction)
    {
        AsyncAction = asyncaction ?? throw new ArgumentNullException(nameof(asyncaction));
    }

    /**<include file='ConfigureToolAction.xml' path='ConfigureToolAction/class[@name="ConfigureToolAction"]/method[@name="Execute"]/*'/>*/
    public void Execute(ToolBuilderContext context , ITool tool)
    {
        if(Action is not null) { Action(context,tool); return; }

        AsyncAction!(context,tool,CancellationToken.None).GetAwaiter().GetResult();
    }

    /**<include file='ConfigureToolAction.xml' path='ConfigureToolAction/class[@name="ConfigureToolAction"]/method[@name="ExecuteAsync"]/*'/>*/
    public Task ExecuteAsync(ToolBuilderContext context , ITool tool , CancellationToken cancel = default)
    {
        if(Action is not null)
        {
            Action(context,tool); return Task.CompletedTask;
        }

        return AsyncAction!(context,tool,cancel);
    }
}
