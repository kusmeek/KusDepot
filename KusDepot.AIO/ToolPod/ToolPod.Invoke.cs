namespace KusDepot.AI;

public sealed partial class ToolPod
{
    [McpServerTool(Name = "ToolPodInvoke")]
    [Description(Invoke)]
    public static async Task<ToolPodResult> InvokeAsync(
        [Description(InvokeRequest)] ToolPodInvokeRequest? request,
        [Description(InvokeAlias)] String? alias = null,
        CancellationToken cancel = default)
    {
        try
        {
            if(request is null) { return ErrorResult(ErrorStrings.InvokeRequestRequired); }

            if(String.IsNullOrWhiteSpace(request.TargetId)) { return ErrorResult(ErrorStrings.InvokeTargetIdRequired); }

            if(String.IsNullOrWhiteSpace(request.Method)) { return ErrorResult(ErrorStrings.InvokeMethodRequired); }

            String? id = ResolveId(request.TargetId); if(String.IsNullOrWhiteSpace(id)) { return ErrorResult(ErrorStrings.ObjectNotFound); }

            Object? target = GetTrackedObject(id); if(target is null) { return ErrorResult(ErrorStrings.ObjectNotFound); }

            ObjectInvoker i = ObjectInvoker.Create(target); Object?[] args = BindArguments(request.Arguments);

            Object? result = await i.InvokeAsync(request.Method,args,cancel).ConfigureAwait(false);

            return NormalizeResult(result,alias);
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.InvokeFailed); return ErrorResult(_.Message); }
    }
}