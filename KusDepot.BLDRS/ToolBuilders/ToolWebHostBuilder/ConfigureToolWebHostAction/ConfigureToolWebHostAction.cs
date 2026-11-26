namespace KusDepot.Builders;

/**<include file='ConfigureToolWebHostAction.xml' path='ConfigureToolWebHostAction/class[@name="ConfigureToolWebHostAction"]/main/*'/>*/
public sealed record ConfigureToolWebHostAction
{
    /**<include file='ConfigureToolWebHostAction.xml' path='ConfigureToolWebHostAction/class[@name="ConfigureToolWebHostAction"]/property[@name="Action"]/*'/>*/
    public Action<ToolBuilderContext,IToolWebHost>? Action { get; }

    /**<include file='ConfigureToolWebHostAction.xml' path='ConfigureToolWebHostAction/class[@name="ConfigureToolWebHostAction"]/property[@name="AsyncAction"]/*'/>*/
    public Func<ToolBuilderContext,IToolWebHost,CancellationToken,Task>? AsyncAction { get; }

    /**<include file='ConfigureToolWebHostAction.xml' path='ConfigureToolWebHostAction/class[@name="ConfigureToolWebHostAction"]/constructor[@name="ActionConstructor"]/*'/>*/
    public ConfigureToolWebHostAction(Action<ToolBuilderContext,IToolWebHost> action)
    {
        Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    /**<include file='ConfigureToolWebHostAction.xml' path='ConfigureToolWebHostAction/class[@name="ConfigureToolWebHostAction"]/constructor[@name="AsyncActionConstructor"]/*'/>*/
    public ConfigureToolWebHostAction(Func<ToolBuilderContext,IToolWebHost,CancellationToken,Task> asyncaction)
    {
        AsyncAction = asyncaction ?? throw new ArgumentNullException(nameof(asyncaction));
    }

    /**<include file='ConfigureToolWebHostAction.xml' path='ConfigureToolWebHostAction/class[@name="ConfigureToolWebHostAction"]/method[@name="Execute"]/*'/>*/
    public void Execute(ToolBuilderContext context , IToolWebHost toolwebhost)
    {
        if(Action is not null) { Action(context,toolwebhost); return; }

        AsyncAction!(context,toolwebhost,CancellationToken.None).GetAwaiter().GetResult();
    }

    /**<include file='ConfigureToolWebHostAction.xml' path='ConfigureToolWebHostAction/class[@name="ConfigureToolWebHostAction"]/method[@name="ExecuteAsync"]/*'/>*/
    public Task ExecuteAsync(ToolBuilderContext context , IToolWebHost toolwebhost , CancellationToken cancel = default)
    {
        if(Action is not null)
        {
            Action(context,toolwebhost); return Task.CompletedTask;
        }

        return AsyncAction!(context,toolwebhost,cancel);
    }
}
