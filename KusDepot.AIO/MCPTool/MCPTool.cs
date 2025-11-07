namespace KusDepot.AI;

/**<include file='MCPTool.xml' path='MCPTool/class[@name="MCPTool"]/main/*'/>*/
public abstract class MCPTool : McpServerTool , IMCPTool
{
    /**<include file='MCPTool.xml' path='MCPTool/class[@name="MCPTool"]/field[@name="Tool"]/*'/>*/
    protected ITool? Tool;
}