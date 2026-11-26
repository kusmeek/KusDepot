namespace KusDepot.Builders;

/**<include file='ConfigureToolAspireHostAction.xml' path='ConfigureToolAspireHostAction/class[@name="ConfigureToolAspireHostAction"]/main/*'/>*/
public sealed record ConfigureToolAspireHostAction
{
    /**<include file='ConfigureToolAspireHostAction.xml' path='ConfigureToolAspireHostAction/class[@name="ConfigureToolAspireHostAction"]/property[@name="Action"]/*'/>*/
    public Action<ToolBuilderContext,IToolAspireHost>? Action { get; }

    /**<include file='ConfigureToolAspireHostAction.xml' path='ConfigureToolAspireHostAction/class[@name="ConfigureToolAspireHostAction"]/property[@name="AsyncAction"]/*'/>*/
    public Func<ToolBuilderContext,IToolAspireHost,CancellationToken,Task>? AsyncAction { get; }

    /**<include file='ConfigureToolAspireHostAction.xml' path='ConfigureToolAspireHostAction/class[@name="ConfigureToolAspireHostAction"]/constructor[@name="ActionConstructor"]/*'/>*/
    public ConfigureToolAspireHostAction(Action<ToolBuilderContext,IToolAspireHost> action)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /**<include file='ConfigureToolAspireHostAction.xml' path='ConfigureToolAspireHostAction/class[@name="ConfigureToolAspireHostAction"]/constructor[@name="AsyncActionConstructor"]/*'/>*/
    public ConfigureToolAspireHostAction(Func<ToolBuilderContext,IToolAspireHost,CancellationToken,Task> asyncaction)
    {
        AsyncAction = asyncaction ?? throw new ArgumentNullException(nameof(asyncaction));
    }

    /**<include file='ConfigureToolAspireHostAction.xml' path='ConfigureToolAspireHostAction/class[@name="ConfigureToolAspireHostAction"]/method[@name="Execute"]/*'/>*/
    public void Execute(ToolBuilderContext context , IToolAspireHost toolaspirehost)
    {
        if(Action is not null) { Action(context,toolaspirehost); return; }

        AsyncAction!(context,toolaspirehost,CancellationToken.None).GetAwaiter().GetResult();
    }

    /**<include file='ConfigureToolAspireHostAction.xml' path='ConfigureToolAspireHostAction/class[@name="ConfigureToolAspireHostAction"]/method[@name="ExecuteAsync"]/*'/>*/
    public Task ExecuteAsync(ToolBuilderContext context , IToolAspireHost toolaspirehost , CancellationToken cancel = default)
    {
        if(Action is not null)
        {
            Action(context,toolaspirehost); return Task.CompletedTask;
        }

        return AsyncAction!(context,toolaspirehost,cancel);
    }
}
