namespace KusDepot.AI;

public sealed partial class ToolPod
{
    [McpServerTool(Name = "Debug-EchoTextItem")]
    [Description("Debug MCP probe that accepts a TextItem parameter and returns the parameter directly.")]
    public static TextItem? EchoTextItemDirect(
        [Description("TextItem value used to validate MCP JSON schema and binding.")] TextItem? item)
    {
        return item;
    }

    [McpServerTool(Name = "Debug-EchoTextItemNormalized")]
    [Description("Debug MCP probe that accepts a TextItem parameter and returns its normalized value.")]
    public static ToolPodResult EchoTextItem(
        [Description("TextItem value used to validate MCP JSON schema and binding.")] TextItem? item)
    {
        try
        {
            return NormalizeResult(item);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug TextItem echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoBinaryItem")]
    [Description("Debug MCP probe that accepts a BinaryItem parameter and returns the parameter directly.")]
    public static BinaryItem? EchoBinaryItemDirect(
        [Description("BinaryItem value used to validate MCP JSON schema and binding.")] BinaryItem? item)
    {
        return item;
    }

    [McpServerTool(Name = "Debug-EchoBinaryItemNormalized")]
    [Description("Debug MCP probe that accepts a BinaryItem parameter and returns its normalized value.")]
    public static ToolPodResult EchoBinaryItem(
        [Description("BinaryItem value used to validate MCP JSON schema and binding.")] BinaryItem? item)
    {
        try
        {
            return NormalizeResult(item);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug BinaryItem echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoCodeItem")]
    [Description("Debug MCP probe that accepts a CodeItem parameter and returns the parameter directly.")]
    public static CodeItem? EchoCodeItemDirect(
        [Description("CodeItem value used to validate MCP JSON schema and binding.")] CodeItem? item)
    {
        return item;
    }

    [McpServerTool(Name = "Debug-EchoCodeItemNormalized")]
    [Description("Debug MCP probe that accepts a CodeItem parameter and returns its normalized value.")]
    public static ToolPodResult EchoCodeItem(
        [Description("CodeItem value used to validate MCP JSON schema and binding.")] CodeItem? item)
    {
        try
        {
            return NormalizeResult(item);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug CodeItem echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoMultiMediaItem")]
    [Description("Debug MCP probe that accepts a MultiMediaItem parameter and returns the parameter directly.")]
    public static MultiMediaItem? EchoMultiMediaItemDirect(
        [Description("MultiMediaItem value used to validate MCP JSON schema and binding.")] MultiMediaItem? item)
    {
        return item;
    }

    [McpServerTool(Name = "Debug-EchoMultiMediaItemNormalized")]
    [Description("Debug MCP probe that accepts a MultiMediaItem parameter and returns its normalized value.")]
    public static ToolPodResult EchoMultiMediaItem(
        [Description("MultiMediaItem value used to validate MCP JSON schema and binding.")] MultiMediaItem? item)
    {
        try
        {
            return NormalizeResult(item);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug MultiMediaItem echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoGuidReferenceItem")]
    [Description("Debug MCP probe that accepts a GuidReferenceItem parameter and returns the parameter directly.")]
    public static GuidReferenceItem? EchoGuidReferenceItemDirect(
        [Description("GuidReferenceItem value used to validate MCP JSON schema and binding.")] GuidReferenceItem? item)
    {
        return item;
    }

    [McpServerTool(Name = "Debug-EchoGuidReferenceItemNormalized")]
    [Description("Debug MCP probe that accepts a GuidReferenceItem parameter and returns its normalized value.")]
    public static ToolPodResult EchoGuidReferenceItem(
        [Description("GuidReferenceItem value used to validate MCP JSON schema and binding.")] GuidReferenceItem? item)
    {
        try
        {
            return NormalizeResult(item);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug GuidReferenceItem echo failed."); return ErrorResult(_.Message); }
    }
}