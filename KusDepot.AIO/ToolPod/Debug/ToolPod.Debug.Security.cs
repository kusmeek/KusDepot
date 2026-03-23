namespace KusDepot.AI;

public sealed partial class ToolPod
{
    [McpServerTool(Name = "Debug-EchoManagerKey")]
    [Description("Debug MCP probe that accepts a ManagerKey parameter and returns the parameter directly.")]
    public static ManagerKey? EchoManagerKeyDirect(
        [Description("ManagerKey value used to validate MCP JSON schema and binding.")] ManagerKey? key)
    {
        return key;
    }

    [McpServerTool(Name = "Debug-EchoManagerKeyNormalized")]
    [Description("Debug MCP probe that accepts a ManagerKey parameter and returns its normalized value.")]
    public static ToolPodResult EchoManagerKey(
        [Description("ManagerKey value used to validate MCP JSON schema and binding.")] ManagerKey? key)
    {
        try
        {
            return NormalizeResult(key);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ManagerKey echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoOwnerKey")]
    [Description("Debug MCP probe that accepts an OwnerKey parameter and returns the parameter directly.")]
    public static OwnerKey? EchoOwnerKeyDirect(
        [Description("OwnerKey value used to validate MCP JSON schema and binding.")] OwnerKey? key)
    {
        return key;
    }

    [McpServerTool(Name = "Debug-EchoOwnerKeyNormalized")]
    [Description("Debug MCP probe that accepts an OwnerKey parameter and returns its normalized value.")]
    public static ToolPodResult EchoOwnerKey(
        [Description("OwnerKey value used to validate MCP JSON schema and binding.")] OwnerKey? key)
    {
        try
        {
            return NormalizeResult(key);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug OwnerKey echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoClientKey")]
    [Description("Debug MCP probe that accepts a ClientKey parameter and returns the parameter directly.")]
    public static ClientKey? EchoClientKeyDirect(
        [Description("ClientKey value used to validate MCP JSON schema and binding.")] ClientKey? key)
    {
        return key;
    }

    [McpServerTool(Name = "Debug-EchoClientKeyNormalized")]
    [Description("Debug MCP probe that accepts a ClientKey parameter and returns its normalized value.")]
    public static ToolPodResult EchoClientKey(
        [Description("ClientKey value used to validate MCP JSON schema and binding.")] ClientKey? key)
    {
        try
        {
            return NormalizeResult(key);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ClientKey echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoCommandKey")]
    [Description("Debug MCP probe that accepts a CommandKey parameter and returns the parameter directly.")]
    public static CommandKey? EchoCommandKeyDirect(
        [Description("CommandKey value used to validate MCP JSON schema and binding.")] CommandKey? key)
    {
        return key;
    }

    [McpServerTool(Name = "Debug-EchoCommandKeyNormalized")]
    [Description("Debug MCP probe that accepts a CommandKey parameter and returns its normalized value.")]
    public static ToolPodResult EchoCommandKey(
        [Description("CommandKey value used to validate MCP JSON schema and binding.")] CommandKey? key)
    {
        try
        {
            return NormalizeResult(key);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug CommandKey echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoExecutiveKey")]
    [Description("Debug MCP probe that accepts an ExecutiveKey parameter and returns the parameter directly.")]
    public static ExecutiveKey? EchoExecutiveKeyDirect(
        [Description("ExecutiveKey value used to validate MCP JSON schema and binding.")] ExecutiveKey? key)
    {
        return key;
    }

    [McpServerTool(Name = "Debug-EchoExecutiveKeyNormalized")]
    [Description("Debug MCP probe that accepts an ExecutiveKey parameter and returns its normalized value.")]
    public static ToolPodResult EchoExecutiveKey(
        [Description("ExecutiveKey value used to validate MCP JSON schema and binding.")] ExecutiveKey? key)
    {
        try
        {
            return NormalizeResult(key);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ExecutiveKey echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoHostKey")]
    [Description("Debug MCP probe that accepts a HostKey parameter and returns the parameter directly.")]
    public static HostKey? EchoHostKeyDirect(
        [Description("HostKey value used to validate MCP JSON schema and binding.")] HostKey? key)
    {
        return key;
    }

    [McpServerTool(Name = "Debug-EchoHostKeyNormalized")]
    [Description("Debug MCP probe that accepts a HostKey parameter and returns its normalized value.")]
    public static ToolPodResult EchoHostKey(
        [Description("HostKey value used to validate MCP JSON schema and binding.")] HostKey? key)
    {
        try
        {
            return NormalizeResult(key);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug HostKey echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoMyHostKey")]
    [Description("Debug MCP probe that accepts a MyHostKey parameter and returns the parameter directly.")]
    public static MyHostKey? EchoMyHostKeyDirect(
        [Description("MyHostKey value used to validate MCP JSON schema and binding.")] MyHostKey? key)
    {
        return key;
    }

    [McpServerTool(Name = "Debug-EchoMyHostKeyNormalized")]
    [Description("Debug MCP probe that accepts a MyHostKey parameter and returns its normalized value.")]
    public static ToolPodResult EchoMyHostKey(
        [Description("MyHostKey value used to validate MCP JSON schema and binding.")] MyHostKey? key)
    {
        try
        {
            return NormalizeResult(key);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug MyHostKey echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoServiceKey")]
    [Description("Debug MCP probe that accepts a ServiceKey parameter and returns the parameter directly.")]
    public static ServiceKey? EchoServiceKeyDirect(
        [Description("ServiceKey value used to validate MCP JSON schema and binding.")] ServiceKey? key)
    {
        return key;
    }

    [McpServerTool(Name = "Debug-EchoServiceKeyNormalized")]
    [Description("Debug MCP probe that accepts a ServiceKey parameter and returns its normalized value.")]
    public static ToolPodResult EchoServiceKey(
        [Description("ServiceKey value used to validate MCP JSON schema and binding.")] ServiceKey? key)
    {
        try
        {
            return NormalizeResult(key);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ServiceKey echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoTokenKey")]
    [Description("Debug MCP probe that accepts a TokenKey parameter and returns the parameter directly.")]
    public static TokenKey? EchoTokenKeyDirect(
        [Description("TokenKey value used to validate MCP JSON schema and binding.")] TokenKey? key)
    {
        return key;
    }

    [McpServerTool(Name = "Debug-EchoTokenKeyNormalized")]
    [Description("Debug MCP probe that accepts a TokenKey parameter and returns its normalized value.")]
    public static ToolPodResult EchoTokenKey(
        [Description("TokenKey value used to validate MCP JSON schema and binding.")] TokenKey? key)
    {
        try
        {
            return NormalizeResult(key);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug TokenKey echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoStandardRequest")]
    [Description("Debug MCP probe that accepts a StandardRequest parameter and returns the parameter directly.")]
    public static StandardRequest? EchoStandardRequestDirect(
        [Description("StandardRequest value used to validate MCP JSON schema and binding.")] StandardRequest? request)
    {
        return request;
    }

    [McpServerTool(Name = "Debug-EchoStandardRequestNormalized")]
    [Description("Debug MCP probe that accepts a StandardRequest parameter and returns its normalized value.")]
    public static ToolPodResult EchoStandardRequest(
        [Description("StandardRequest value used to validate MCP JSON schema and binding.")] StandardRequest? request)
    {
        try
        {
            return NormalizeResult(request);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug StandardRequest echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoServiceRequest")]
    [Description("Debug MCP probe that accepts a ServiceRequest parameter and returns the parameter directly.")]
    public static ServiceRequest? EchoServiceRequestDirect(
        [Description("ServiceRequest value used to validate MCP JSON schema and binding.")] ServiceRequest? request)
    {
        return request;
    }

    [McpServerTool(Name = "Debug-EchoServiceRequestNormalized")]
    [Description("Debug MCP probe that accepts a ServiceRequest parameter and returns its normalized value.")]
    public static ToolPodResult EchoServiceRequest(
        [Description("ServiceRequest value used to validate MCP JSON schema and binding.")] ServiceRequest? request)
    {
        try
        {
            return NormalizeResult(request);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ServiceRequest echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoManagementRequest")]
    [Description("Debug MCP probe that accepts a ManagementRequest parameter and returns the parameter directly.")]
    public static ManagementRequest? EchoManagementRequestDirect(
        [Description("ManagementRequest value used to validate MCP JSON schema and binding.")] ManagementRequest? request)
    {
        return request;
    }

    [McpServerTool(Name = "Debug-EchoManagementRequestNormalized")]
    [Description("Debug MCP probe that accepts a ManagementRequest parameter and returns its normalized value.")]
    public static ToolPodResult EchoManagementRequest(
        [Description("ManagementRequest value used to validate MCP JSON schema and binding.")] ManagementRequest? request)
    {
        try
        {
            return NormalizeResult(request);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ManagementRequest echo failed."); return ErrorResult(_.Message); }
    }
}
