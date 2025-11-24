namespace KusDepot;

/**<include file='ToolBuilderContext.xml' path='ToolBuilderContext/class[@name="ToolBuilderContext"]/main/*'/>*/
public class ToolBuilderContext
{
    /**<include file='ToolBuilderContext.xml' path='ToolBuilderContext/class[@name="ToolBuilderContext"]/property[@name="ToolProperties"]/*'/>*/
    public IDictionary<Object,Object> ToolProperties {get;}

    /**<include file='ToolBuilderContext.xml' path='ToolBuilderContext/class[@name="ToolBuilderContext"]/property[@name="Configuration"]/*'/>*/
    public IConfiguration? Configuration {get;set;} = null!;

    /**<include file='ToolBuilderContext.xml' path='ToolBuilderContext/class[@name="ToolBuilderContext"]/constructor[@name="Constructor"]/*'/>*/
    public ToolBuilderContext(Dictionary<Object,Object> toolproperties) => this.ToolProperties = toolproperties ?? new();
}