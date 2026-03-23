namespace KusDepot.Builders;

/**<include file='RegisteredHostedTool.xml' path='RegisteredHostedTool/record[@name="RegisteredHostedTool"]/main/*'/>*/
public sealed record RegisteredHostedTool
{
    /**<include file='RegisteredHostedTool.xml' path='RegisteredHostedTool/record[@name="RegisteredHostedTool"]/property[@name="Tool"]/*'/>*/
    public required ITool Tool { get; init; }

    /**<include file='RegisteredHostedTool.xml' path='RegisteredHostedTool/record[@name="RegisteredHostedTool"]/property[@name="Name"]/*'/>*/
    public String? Name { get; init; }

    /**<include file='RegisteredHostedTool.xml' path='RegisteredHostedTool/record[@name="RegisteredHostedTool"]/property[@name="Permissions"]/*'/>*/
    public ImmutableArray<Int32>? Permissions { get; init; }
}