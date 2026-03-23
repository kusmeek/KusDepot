namespace KusDepot;

/**<include file='ToolConnect.xml' path='ToolConnect/class[@name="ToolConnect"]/main/*'/>*/
public sealed class ToolConnect : IToolConnect
{
    private ITool? tool;

    private IToolHost? toolhost;

    private IToolWebHost? toolwebhost;

    private IToolAspireHost? toolaspirehost;

    private IToolGenericHost? toolgenerichost;

    ///<inheritdoc/>
    public ITool? Tool { get => tool; set => tool ??= value; }

    ///<inheritdoc/>
    public IToolHost? ToolHost { get => toolhost; set { if(tool is null && toolhost is null) { tool = toolhost = value; } } }

    ///<inheritdoc/>
    public IToolWebHost? ToolWebHost { get => toolwebhost; set { if(tool is null && toolwebhost is null) { tool = toolwebhost = value; } } }

    ///<inheritdoc/>
    public IToolAspireHost? ToolAspireHost { get => toolaspirehost; set { if(tool is null && toolaspirehost is null) { tool = toolaspirehost = value; } } }

    ///<inheritdoc/>
    public IToolGenericHost? ToolGenericHost { get => toolgenerichost; set { if(tool is null && toolgenerichost is null) { tool = toolgenerichost = value; } } } 
}