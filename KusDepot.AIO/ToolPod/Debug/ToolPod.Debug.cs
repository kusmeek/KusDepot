namespace KusDepot.AI;

public sealed partial class ToolPod
{
    [McpServerTool(Name = "Debug-EchoToolValue")]
    [Description("Debug MCP probe that accepts a ToolValue parameter and returns the parameter directly.")]
    public static ToolValue? EchoToolValueDirect(
        [Description("ToolValue value used to validate MCP JSON schema and binding.")] ToolValue? value)
    {
        return value;
    }

    [McpServerTool(Name = "Debug-EchoToolValueNormalized")]
    [Description("Debug MCP probe that accepts a ToolValue parameter and returns its normalized value.")]
    public static ToolPodResult EchoToolValueNormalized(
        [Description("ToolValue value used to validate MCP JSON schema and binding.")] ToolValue? value)
    {
        try
        {
            return NormalizeResult(value);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ToolValue echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoToolValueArgument")]
    [Description("Debug MCP probe that accepts a ToolValueArgument parameter and returns the parameter directly.")]
    public static ToolValueArgument? EchoToolValueArgumentDirect(
        [Description("ToolValueArgument value used to validate MCP JSON schema and binding.")] ToolValueArgument? value)
    {
        return value;
    }

    [McpServerTool(Name = "Debug-EchoToolValueArgumentNormalized")]
    [Description("Debug MCP probe that accepts a ToolValueArgument parameter and returns its normalized value.")]
    public static ToolPodResult EchoToolValueArgumentNormalized(
        [Description("ToolValueArgument value used to validate MCP JSON schema and binding.")] ToolValueArgument? value)
    {
        try
        {
            return NormalizeResult(value);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ToolValueArgument echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoToolData")]
    [Description("Debug MCP probe that accepts a ToolData parameter and returns the parameter directly.")]
    public static ToolData? EchoToolDataDirect(
        [Description("ToolData value used to validate MCP JSON schema and binding.")] ToolData? data)
    {
        return data;
    }

    [McpServerTool(Name = "Debug-EchoToolDataNormalized")]
    [Description("Debug MCP probe that accepts a ToolData parameter and returns its normalized value.")]
    public static ToolPodResult EchoToolDataNormalized(
        [Description("ToolData value used to validate MCP JSON schema and binding.")] ToolData? data)
    {
        try
        {
            return NormalizeResult(data);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ToolData echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoToolInput")]
    [Description("Debug MCP probe that accepts a ToolInput parameter and returns the parameter directly.")]
    public static ToolInput? EchoToolInputDirect(
        [Description("ToolInput value used to validate MCP JSON schema and binding.")] ToolInput? data)
    {
        return data;
    }

    [McpServerTool(Name = "Debug-EchoToolInputNormalized")]
    [Description("Debug MCP probe that accepts a ToolInput parameter and returns its normalized value.")]
    public static ToolPodResult EchoToolInputNormalized(
        [Description("ToolInput value used to validate MCP JSON schema and binding.")] ToolInput? data)
    {
        try
        {
            return NormalizeResult(data);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ToolInput echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoToolOutput")]
    [Description("Debug MCP probe that accepts a ToolOutput parameter and returns the parameter directly.")]
    public static ToolOutput? EchoToolOutputDirect(
        [Description("ToolOutput value used to validate MCP JSON schema and binding.")] ToolOutput? data)
    {
        return data;
    }

    [McpServerTool(Name = "Debug-EchoToolOutputNormalized")]
    [Description("Debug MCP probe that accepts a ToolOutput parameter and returns its normalized value.")]
    public static ToolPodResult EchoToolOutputNormalized(
        [Description("ToolOutput value used to validate MCP JSON schema and binding.")] ToolOutput? data)
    {
        try
        {
            return NormalizeResult(data);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ToolOutput echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoCommandDetails")]
    [Description("Debug MCP probe that accepts a CommandDetails parameter and returns the parameter directly.")]
    public static CommandDetails? EchoCommandDetailsDirect(
        [Description("CommandDetails value used to validate MCP JSON schema and binding.")] CommandDetails? details)
    {
        return details;
    }

    [McpServerTool(Name = "Debug-EchoCommandDetailsNormalized")]
    [Description("Debug MCP probe that accepts a CommandDetails parameter and returns its normalized value.")]
    public static ToolPodResult EchoCommandDetailsNormalized(
        [Description("CommandDetails value used to validate MCP JSON schema and binding.")] CommandDetails? details)
    {
        try
        {
            return NormalizeResult(details);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug CommandDetails echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoCommandWorkflow")]
    [Description("Debug MCP probe that accepts a CommandWorkflow parameter and returns the parameter directly.")]
    public static CommandWorkflow? EchoCommandWorkflowDirect(
        [Description("CommandWorkflow value used to validate MCP JSON schema and binding.")] CommandWorkflow? workflow)
    {
        return workflow;
    }

    [McpServerTool(Name = "Debug-EchoCommandWorkflowNormalized")]
    [Description("Debug MCP probe that accepts a CommandWorkflow parameter and returns its normalized value.")]
    public static ToolPodResult EchoCommandWorkflowNormalized(
        [Description("CommandWorkflow value used to validate MCP JSON schema and binding.")] CommandWorkflow? workflow)
    {
        try
        {
            return NormalizeResult(workflow);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug CommandWorkflow echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoDescriptor")]
    [Description("Debug MCP probe that accepts a Descriptor parameter and returns the parameter directly.")]
    public static Descriptor? EchoDescriptorDirect(
        [Description("Descriptor value used to validate MCP JSON schema and binding.")] Descriptor? descriptor)
    {
        return descriptor;
    }

    [McpServerTool(Name = "Debug-EchoDescriptorNormalized")]
    [Description("Debug MCP probe that accepts a Descriptor parameter and returns its normalized value.")]
    public static ToolPodResult EchoDescriptorNormalized(
        [Description("Descriptor value used to validate MCP JSON schema and binding.")] Descriptor? descriptor)
    {
        try
        {
            return NormalizeResult(descriptor);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug Descriptor echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoCommandDescriptor")]
    [Description("Debug MCP probe that accepts a CommandDescriptor parameter and returns the parameter directly.")]
    public static CommandDescriptor? EchoCommandDescriptorDirect(
        [Description("CommandDescriptor value used to validate MCP JSON schema and binding.")] CommandDescriptor? descriptor)
    {
        return descriptor;
    }

    [McpServerTool(Name = "Debug-EchoCommandDescriptorNormalized")]
    [Description("Debug MCP probe that accepts a CommandDescriptor parameter and returns its normalized value.")]
    public static ToolPodResult EchoCommandDescriptorNormalized(
        [Description("CommandDescriptor value used to validate MCP JSON schema and binding.")] CommandDescriptor? descriptor)
    {
        try
        {
            return NormalizeResult(descriptor);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug CommandDescriptor echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoToolDescriptor")]
    [Description("Debug MCP probe that accepts a ToolDescriptor parameter and returns the parameter directly.")]
    public static ToolDescriptor? EchoToolDescriptorDirect(
        [Description("ToolDescriptor value used to validate MCP JSON schema and binding.")] ToolDescriptor? descriptor)
    {
        return descriptor;
    }

    [McpServerTool(Name = "Debug-EchoToolDescriptorNormalized")]
    [Description("Debug MCP probe that accepts a ToolDescriptor parameter and returns its normalized value.")]
    public static ToolPodResult EchoToolDescriptorNormalized(
        [Description("ToolDescriptor value used to validate MCP JSON schema and binding.")] ToolDescriptor? descriptor)
    {
        try
        {
            return NormalizeResult(descriptor);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug ToolDescriptor echo failed."); return ErrorResult(_.Message); }
    }

    [McpServerTool(Name = "Debug-EchoWorkflowExceptionData")]
    [Description("Debug MCP probe that accepts a WorkflowExceptionData parameter and returns the parameter directly.")]
    public static WorkflowExceptionData? EchoWorkflowExceptionDataDirect(
        [Description("WorkflowExceptionData value used to validate MCP JSON schema and binding.")] WorkflowExceptionData? data)
    {
        return data;
    }

    [McpServerTool(Name = "Debug-EchoWorkflowExceptionDataNormalized")]
    [Description("Debug MCP probe that accepts a WorkflowExceptionData parameter and returns its normalized value.")]
    public static ToolPodResult EchoWorkflowExceptionDataNormalized(
        [Description("WorkflowExceptionData value used to validate MCP JSON schema and binding.")] WorkflowExceptionData? data)
    {
        try
        {
            return NormalizeResult(data);
        }
        catch ( Exception _ ) { Log.Error(_,"ToolPod debug WorkflowExceptionData echo failed."); return ErrorResult(_.Message); }
    }
}