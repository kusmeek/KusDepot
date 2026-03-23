namespace KusDepot.AI;

public sealed partial class ToolPod
{
    [McpServerTool(Name = "ToolPodInvokeStatic")]
    [Description(InvokeStatic)]
    public static async Task<ToolPodResult> InvokeStaticAsync(
        [Description(InvokeStaticRequest)] ToolPodStaticInvokeRequest? request,
        [Description(InvokeStaticAlias)] String? alias = null,
        CancellationToken cancel = default)
    {
        try
        {
            if(request is null) { return ErrorResult(ErrorStrings.InvokeStaticRequestRequired); }

            if(String.IsNullOrWhiteSpace(request.Type)) { return ErrorResult(ErrorStrings.InvokeStaticTypeRequired); }

            if(String.IsNullOrWhiteSpace(request.Method)) { return ErrorResult(ErrorStrings.InvokeStaticMethodRequired); }

            TypeProvider tp = TypeProvider.Create(request.Type).WithContext(Context); Type? t = tp.Resolve();

            if(t is null) { return ErrorResult(String.Format(ErrorStrings.InvokeStaticTypeNotFoundFormat,request.Type)); }

            ObjectInvoker i = ObjectInvoker.Create(t); Object?[] args = BindArguments(request.Arguments);

            Object? result = await i.InvokeStaticAsync(request.Method,args,cancel).ConfigureAwait(false);

            return NormalizeResult(result,alias);
        }
        catch ( Exception _ ) { Log.Error(_,LogStrings.InvokeStaticFailed); return ErrorResult(_.Message); }
    }
}